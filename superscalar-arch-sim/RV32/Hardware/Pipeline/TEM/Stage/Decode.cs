using System;
using superscalar_arch_sim.RV32.ISA;
using superscalar_arch_sim.RV32.ISA.Instructions;
using superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Units;
using static superscalar_arch_sim.RV32.ISA.Disassembler;
using superscalar_arch_sim.Simulis;
using System.Linq;
using System.Reflection.Emit;
using static superscalar_arch_sim.Simulis.Reports.SimReporter;

namespace superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Stage
{
    /// <summary>
    /// Second stage of TEM pipeline. Fetched in previous stage instructions are decoded in-order.
    /// </summary>
    public class Decode : TEMStage
    {
        private static PipeRegisters[] LatchRegistersSource = new PipeRegisters[0];
        protected override int MaxInstructionsProcessedPerCycle { get => Settings.FetchWidth; }

        /// <summary>Invoked when <see cref="Pipeline.Stage.ProcessedInstruction"/> (sender) is a FENCE instruction. Decoded as <see cref="ISAProperties.InstType.I"/> type instruction.</summary>
        public event EventHandler<StageDataArgs> FenceDecoded;
        /// <summary>Invoked when <see cref="Pipeline.Stage.ProcessedInstruction"/> (sender) is a Control&Status Register (CSR) instruction (see <see cref="ISAProperties.ISA32.Zicsr"/> extension). Decoded as <see cref="ISAProperties.InstType.I"/> type instruction.</summary>
        public event EventHandler<StageDataArgs> SystemCSRDecoded;
        /// <summary>Invoked when <see cref="Pipeline.Stage.ProcessedInstruction"/> (sender) is a ECALL instruction. Decoded as <see cref="ISAProperties.InstType.I"/> type instruction.</summary>
        public event EventHandler<StageDataArgs> EnvironmentCallDecoded;
        /// <summary>Invoked when <see cref="Pipeline.Stage.ProcessedInstruction"/> (sender) is a EBREAK instruction. Decoded as <see cref="ISAProperties.InstType.I"/> type instruction.</summary>
        public event EventHandler<StageDataArgs> EnvironmentBreakDecoded;

        /// <summary>Invoked when <see cref="Pipeline.Stage.ProcessedInstruction"/> is control transfer <see cref="Instruction"/>.</summary>
        public event EventHandler<StageDataArgs> ImmediateControlTransferLatched;

        /// <summary>Invoked when <see cref="Instruction.Illegal"/> flag is set during decoding of processed instruction.</summary>
        public event EventHandler<StageDataArgs> IllegalInstructionDecoded;

        private readonly InstructionDataQueue IRDataQueue;


        public Decode(InstructionDataQueue iQueue) 
            : base(HardwareProperties.TEMPipelineStage.Decode)
        {
            IRDataQueue = iQueue;
        }

        public Instruction DecodeInstruction(Instruction inst32)
        {
            if ((inst32.Value & 0b11) != 0b11) {
                inst32.MarkIllegal();
                return inst32;
            }

            Decoder.DecodeInstruction(in inst32);

            if (inst32.opcode == Opcodes.OPCODE_FENCE)
            {
                FenceDecoded?.Invoke(sender: this, new StageDataArgs(inst32));
            } 
            else if (Opcodes.IsSystem(inst32))
            {
                if (inst32.Equals(Opcodes.INSTR_EBREAK))
                    EnvironmentBreakDecoded?.Invoke(sender: this, new StageDataArgs(inst32, lpc: _LocalPC));
                else if (inst32.Equals(Opcodes.INSTR_ECALL))
                    EnvironmentCallDecoded?.Invoke(sender: this, new StageDataArgs(inst32));
                else
                    SystemCSRDecoded?.Invoke(sender: this, new StageDataArgs(inst32));
            }

            if (false == inst32.Illegal)
                inst32.ASM = DecodeToHumanReadable(inst32);

            return inst32;
        }


        public override void Cycle()
        {
            for (int i = 0; i < MaxInstructionsProcessedPerCycle; i++)
            {
                GetLatchDataFromPreviousStage(bufferIndex: i);
                Instruction i32 = ProcessedInstructions[i];
                if (i32 != null)
                {
                    i32 = DecodeInstruction(i32);
                    if (i32.Illegal)
                    {
                        IllegalInstructionDecoded?.Invoke(this, new StageDataArgs(i32, lpc: _LocalPC));
                    }
                }
            }
        }

        private void EnqueueInstructionData(PipeRegisters latchBuffer, Instruction i32)
        {
            PipeRegisters queueBuffer = new PipeRegisters();// LatchRegistersSource[IRDataQueue.Count];
            latchBuffer.IR32 = i32;
            queueBuffer.PassFrom(latchBuffer);
            queueBuffer.WriteInstruction(i32);
            IRDataQueue.Enqueue(queueBuffer);
        }

        private void EvaluateAsControlTransfer(in Instruction i32, in PipeRegisters latchRegisters)
        {
            if (Opcodes.IsImmediateControlTransfer(i32))
            {
                var lpc = latchRegisters.LocalPC;
                var npc = latchRegisters.NextPC;
                var @out = latchRegisters.ALUOutput;
                
                Int32 targetaddr = unchecked((Int32)((i32.imm << ISAProperties.JMP_BRANCH_IMM_SHAMT) + lpc.ReadUnsigned()));
                @out.Write(targetaddr);
                ImmediateControlTransferLatched?.Invoke(this, new StageDataArgs(i32, @out, npc, lpc));
            }
        }

        public override void Latch()
        {
#if DEBUG
            System.Diagnostics.Debug.Assert(IsReady());
#endif
            for (int i=0; i < MaxInstructionsProcessedPerCycle; i++)
            {
                var i32 = ProcessedInstructions[i];
                if (i32 != null)
                {
                    var pipeRegisters = LatchDataBuffers[i];
                    int npc = LatchDataBuffers[i].NextPC.Read();
                    EvaluateAsControlTransfer(i32, pipeRegisters);
                    EnqueueInstructionData(pipeRegisters, i32);

                    // ugly but works - ImmediateControlTransferLatched modifies NextPC if refetch neccessary
                    bool refetch = (npc != LatchDataBuffers[i].NextPC.Read());
                    if (refetch) break; 
                    // break on misspredicted address of ControlTransfer instruction - Decode state will be flushed anyway
                }
            }
            if (Reporter.SimMeasuresEnabled)
            {
                int queueSize = IRDataQueue.Count;
                Reporter.I32Measures_Size[SimMeasures.IRQueue].Update(queueSize);
                Reporter.I32Measures_SizeHist[SimMeasures.IRQueue].Collect(queueSize);
            }
        }

        public override bool IsReady()
        {
            return IRDataQueue.CanEnqueueN(MaxInstructionsProcessedPerCycle);
        }

        public override void Reset()
        {
            base.Reset();
            _NextPC.Reset();
            _LocalPC.Reset();
            if (LatchRegistersSource.Length != Settings.InstructionQueueCapacity)
            {
                Array.Resize(ref LatchRegistersSource, Settings.InstructionQueueCapacity);
                for (int i =0; i < Settings.InstructionQueueCapacity; i++)
                {
                    if (LatchRegistersSource[i] is null)
                    {
                        LatchRegistersSource[i] = new PipeRegisters();
                    }
                    LatchRegistersSource[i].Reset();
                }
            }
        }
    }
}
