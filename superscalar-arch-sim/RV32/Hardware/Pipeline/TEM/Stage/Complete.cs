using superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.FuncUnit;
using superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Units;
using superscalar_arch_sim.RV32.Hardware.Units;
using superscalar_arch_sim.RV32.ISA;
using superscalar_arch_sim.RV32.ISA.Instructions;
using superscalar_arch_sim.Simulis;
using System;
using System.Collections.Generic;
using System.Linq;


namespace superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Stage
{
    /// <summary>
    /// Fith stage of TEM pipeline. Instructions arrives at this stage in-order.
    /// <br></br>
    /// An instruction is considered completed (<b>architecturally completed</b>) when it finishes 
    /// execution and updates the machine state (but i.e it's results are not yet written into memory/architectural register).
    /// <br></br>
    /// (<i>For instructions that do not update the memory, <see cref="Retire"/> stage occurs at the same time as <see cref="Complete"/>.</i>)
    /// </summary>
    /// <remarks>Also known as <b>Write Result</b>.</remarks>
    public class Complete : TEMStage
    {
        protected override int MaxInstructionsProcessedPerCycle => Settings.TotalNumberOfExecutionUnits;

        private readonly ReorderBuffer ROB;
        private readonly BranchPredictor Predictor;
        private readonly ReservationStationCollection CommonDataBus;

        public event DynamicStageROBDataEventHandler ControlTransferInstructionComplete;
        public Action<ROBEntry> WriteCommonDataBus;

        public Complete(List<ExecuteUnit> executionUnits, ReorderBuffer rob, BranchPredictor predictor) 
            : base(HardwareProperties.TEMPipelineStage.Complete)
        {
            ROB = rob;
            Predictor = predictor;
            CommonDataBus = new ReservationStationCollection(executionUnits.SelectMany(e => e.UnitReservationStations).Distinct());
        }

        public void ResizeCommonDataBusCollectionContent(List<ExecuteUnit> executionUnits)
        {
            CommonDataBus.ResetResize(executionUnits.SelectMany(e => e.UnitReservationStations).Distinct());
        }

        public override void Cycle()
        {
            ROBEntry[] processed = new ROBEntry[MaxInstructionsProcessedPerCycle];
            ROBEntry[] entries = ROB.GetSortedFromOldest(); 
            for (int i = 0; i < MaxInstructionsProcessedPerCycle; i++)
            {
                LatchDataBuffers[i].Reset();
                ProcessedInstructions[i] = null;
                for (int j = 0; j < entries.Length;  j++) 
                {
                    if (entries[j].ValueReady && false == processed.Contains(entries[j]) && false == entries[j].MarkedEmpty) 
                    {
                        ROBEntry ready = entries[j];
                        processed[i] = ready;
                        ProcessedInstructions[i] = ready.IR32;
                        LatchDataBuffers[i].IR32 = ready.IR32;
                        LatchDataBuffers[i].LocalPC.Write(ready.FetchLocalPC.Read());
                        LatchDataBuffers[i].NextPC.Write(CommonDataBus.GetStationByTag(ready.ReservationStationTag.Value).NextLocalPC.Read());
                        LatchDataBuffers[i].ReservationStationSourceTag = ready.ReservationStationTag.Value;
                        //LatchDataBuffers[i].ALUOutput.Write(ready.Value.Value);
                        //LatchDataBuffers[i].LoadMemoryData.Write(ready.Value.Value);
                        break;
                    }
                }
            }
        }

        public override void Latch()
        {
            for (int i = 0; i < MaxInstructionsProcessedPerCycle; i++)
            {
                int rstag = LatchDataBuffers[i].ReservationStationSourceTag;
                int aluOutput = LatchDataBuffers[i].ALUOutput.Read();
                int loadedData = LatchDataBuffers[i].LoadMemoryData.Read();
                ReservationStation issueStation = CommonDataBus.GetStationByTagOrDefault(rstag);
                ROBEntry robEntry = issueStation?.ROBDest;

                if (issueStation != null && robEntry != null && robEntry.FinishedState == HardwareProperties.TEMPipelineStage.Execute)
                {
                    var i32 = robEntry.IR32;
                    if (Opcodes.IsStore(i32))
                    {
                        if (issueStation.OpVal1 is null)
                        {
                            continue; // Proceed with Store only if operand 1 (store data) is avaliable
                        }
                        robEntry.Value = issueStation.OpVal1.Value;
                    }

                    if (Opcodes.IsControlTransfer(i32))
                    {
                        var lpc = issueStation.FetchLocalPC;
                        int targetAddress = issueStation.A.Value;
                        int npc = issueStation.NextLocalPC.Read();
                        bool controlTransfer = Opcodes.IsJump(i32) || (robEntry.Value.Value == 1);
                        
                        //Predictor.SetTargetAddress(targetAddress);
                        Predictor.SetEvaluatedConditionValue(controlTransfer);
                        //Predictor.UpdateBranchHistory(i32, lpc, controlTransfer);

                        ControlTransferInstructionComplete?.Invoke(robEntry, new StageDataArgs(i32, npc, targetAddress, lpc));
                        //bool addressMissprediction = (Opcodes.OP_I_TYPE_JUMP == i32.opcode) && targetAddress != npc;
                        //bool predictTaken = (unchecked((uint)npc) != (lpc.ReadUnsigned() + ISAProperties.WORD_BYTESIZE));
                        //bool conditionMissprediction = Opcodes.IsBranch(i32) && (predictTaken != controlTransfer);
                        //shouldBreak = (conditionMissprediction || addressMissprediction);
                    }
                    else
                    {
                        Predictor.SetEvaluatedConditionValue(null);
                        Predictor.SetTargetAddress(null);
                    }

                    WriteCommonDataBus(robEntry);
                    robEntry.FinishedState = HardwareProperties.TEMPipelineStage.Complete;
                }

            }
        }
        public override bool IsReady()
        {
            return true;
        }
    }
}
