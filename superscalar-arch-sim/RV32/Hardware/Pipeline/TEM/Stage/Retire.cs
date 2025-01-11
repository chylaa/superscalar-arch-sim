using superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.FuncUnit;
using superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Units;
using superscalar_arch_sim.RV32.Hardware.Register;
using superscalar_arch_sim.RV32.Hardware.Units;
using superscalar_arch_sim.RV32.ISA.Instructions;
using superscalar_arch_sim.Simulis;
using superscalar_arch_sim.Simulis.Reports;
using System;
using System.Collections.Generic;
using System.Linq;

namespace superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Stage
{
    /// <summary>
    /// Sixth, final stage of TEM pipeline. Instructions arrives at this stage in-order.
    /// <br></br>
    /// In this stage instruction result are written into an architecture register or memory.
    /// With instructions that update memory locations, there can be a time period between when they are 
    /// <b>architecturally completed</b> (see <see cref="Complete"/> stage) and when the memory locations are updated.
    /// For example, store instruction is considered retired when it exits the <see cref="Buffer.MemoryBuffer"/> buffer and updates the Data-Cache.
    /// <br></br>
    /// (<i>For instructions that do not update the memory, <see cref="Retire"/> stage occurs at the same time as <see cref="Complete"/>.</i>)
    /// </summary>
    public class Retire : TEMStage
    {
        private long _LastRetireInstructionIndex = -1;

        protected override int MaxInstructionsProcessedPerCycle => Settings.TotalNumberOfExecutionUnits;
        
        private readonly ReservationStationCollection CommonDataBus;
        private readonly Register32File RegisterFile;
        private readonly ReorderBuffer ROB;
        private readonly MemoryManagmentUnit MMU;
        private readonly BranchPredictor Predictor;
        
        public event EventHandler<StageDataArgs> RetireCompleted;
        public event DataWriteEventHandler DataWritten;

        public Action<ROBEntry> WriteCommonDataBus;

        public Retire(List<ExecuteUnit> executionUnits, ReorderBuffer rob, Register32File regfile, MemoryManagmentUnit mmu, BranchPredictor predictor) 
            : base(HardwareProperties.TEMPipelineStage.Retire)
        {
            CommonDataBus = new ReservationStationCollection(executionUnits.SelectMany(e => e.UnitReservationStations).Distinct());
            RegisterFile = regfile;
            ROB = rob;
            MMU = mmu;
            Predictor = predictor;
        }

        public void ResizeCommonDataBusCollectionContent(List<ExecuteUnit> executionUnits)
        {
            CommonDataBus.ResetResize(executionUnits.SelectMany(e => e.UnitReservationStations).Distinct());
        }

        private void WriteMemory(in ROBEntry robHead)
        {
            Instruction i32 = robHead.IR32;
#if DEBUG
            if (robHead.Value is null)
                throw new InvalidPipelineState($"Store value {nameof(ROBEntry.Value)} from ROB Head {robHead.Tag} is null ({i32}).");
            if (robHead.Destination is null)
                throw new InvalidPipelineState($"Store address {nameof(ROBEntry.Destination)} from ROB Head {robHead.Tag}, is null ({i32}).");
#endif
            uint address = unchecked((uint)(robHead.Destination.Value));
            uint storeValue = unchecked((uint)(robHead.Value));
            switch (i32.funct3)
            {
                case 0b000: // SB  
                    MMU.WriteByte(address, (byte)(storeValue &= 0x00_00_00_FF));
                    break;
                case 0b001: // SH 
                    MMU.WriteHWord(address, (UInt16)(storeValue &= 0x00_00_FF_FF));
                    break;
                case 0b010: // SW 
                    MMU.WriteWord(address, storeValue);
                    break;
                default:
                    throw new NotImplementedInstructionException(i32, cause: nameof(Instruction.funct3));
            }
            uint i32address = robHead.FetchLocalPC.ReadUnsigned();
            DataWritten?.Invoke(this, new DataWriteEventArgs(storeValue, address, DataWriteEventArgs.WriteDestination.Memory, i32, i32address));
        }

        private void WriteRegister(in ROBEntry robHead)
        {
            if (robHead.Destination.HasValue && robHead.Destination != 0)
            {
                Instruction i32 = robHead.IR32;
                int robTag = robHead.Tag;
                int rd = (int)robHead.Destination;
                int value = (int)robHead.Value;
#if DEBUG
                if (i32.rd != rd) // Sanity check
                {
                    throw new InvalidPipelineState($"Destination register of {nameof(ROBEntry.Destination)} " +
                        $"from ROB Head {robTag}, does not equal {i32}.rd ({i32.rd} != {rd})");
                }
#endif
                RegisterFile[rd] = value; 
                if (CommonDataBus.All(rs => rs.ROBDest is null || rs.Dest.Equals(robTag) || (rs.IR32.rd != rd)))
                    RegisterFile.ClearRegisterStatusProducerTag(rd);

                uint i32address = robHead.FetchLocalPC.ReadUnsigned();
                DataWritten?.Invoke(this, new DataWriteEventArgs(unchecked((uint)value), unchecked((uint)rd), DataWriteEventArgs.WriteDestination.Register, i32, i32address));
            }
        }

        public override void Cycle()    // Reg./Mem. Write in first half of cycle?
        {
            ROBEntry robHead = null;
            int retiredInstructions = 0;
            for (int i = 0; i < MaxInstructionsProcessedPerCycle; i++)
            {
                LatchDataBuffers[i].Reset();
                if (robHead == ROB.HeadEntry) // if nothing changed (same head again) - abort
                    continue;

                robHead = ROB.HeadEntry;
                Instruction i32 = robHead?.IR32;
                if (robHead != null && i32 != null && robHead.FinishedState == HardwareProperties.TEMPipelineStage.Complete) 
                {
                    LatchDataBuffers[i].IR32 = i32; // assigning to LatchDataBuffers only for UI visibility
                    ProcessedInstructions[i] = i32;
                    LatchDataBuffers[i].LocalPC.Write(robHead.FetchLocalPC.Read());
                    var issueStation = CommonDataBus.GetStationByTag(robHead.ReservationStationTag.Value);

                    if (robHead.Value.HasValue)
                    {
                        WriteCommonDataBus(robHead);
                        if (Opcodes.IsStore(i32))
                        {
                            WriteMemory(robHead);
                        } else
                        {
                            WriteRegister(robHead);
                        }
                    }
                    if (Opcodes.IsControlTransfer(i32))
                    {
                        var lpc = issueStation.FetchLocalPC;
                        int targetAddress = issueStation.A.Value;
                        int npc = issueStation.NextLocalPC.Read();
                        bool controlTransfer = Opcodes.IsJump(i32) || (robHead.Value.Value == 1);

                        Predictor.SetTargetAddress(targetAddress);
                        //Predictor.SetEvaluatedConditionValue(controlTransfer);
                        Predictor.UpdateBranchHistory(i32, lpc, controlTransfer);
                        if (controlTransfer && Opcodes.IsBranch(i32))
                        {
                            ++(Reporter.BranchesTaken);
                        }
                    }
                    
                    robHead.FinishedState = Stage;
                    StageDataArgs args = new StageDataArgs(i32, robHead.Tag, robHead.Value, lpc: robHead.FetchLocalPC);
                    RetireCompleted?.Invoke(this, args);
                    ++retiredInstructions;
#if DEBUG
                    // sanity check
                    System.Diagnostics.Debug.Assert(i32.BubbleInstruction || _LastRetireInstructionIndex < (long)robHead.InstructionIndex); 
                    _LastRetireInstructionIndex = (long)robHead.InstructionIndex;
#endif
                    //issueStation.Busy = false; // complete
                    //issueStation.MarkedEmpty = true; // can overwrite
                    //issueStation.EffectiveAddressCalulated = null; // no longer valid
                    issueStation.Reset(); // not neccessary with prev lines uncommented, (could keep for debugging puprpoeses) but it look ugly on UI

                    robHead.Reset();
                    ROB.UpdateHeadEntry();
                }
            }
            Reporter.I32Measure_RetireThrougput.Update(retiredInstructions);
        }

        public override void Latch()
        {
           
        }

        public override bool IsReady()
        {
            return true;
        }

        public override void Reset()
        {
            base.Reset();
            _LastRetireInstructionIndex = -1;
        }
    }
}
