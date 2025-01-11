using System;
using superscalar_arch_sim.RV32.Hardware.Register;
using superscalar_arch_sim.RV32.Hardware.Units;
using superscalar_arch_sim.RV32.ISA;
using superscalar_arch_sim.RV32.ISA.Instructions;
using superscalar_arch_sim.RV32.Hardware.Pipeline.TYP.Units;

using static superscalar_arch_sim.RV32.ISA.Disassembler;
namespace superscalar_arch_sim.RV32.Hardware.Pipeline.TYP.Stage
{
    /// <summary>
    /// [ <i>ID</i> ] Second stage of TYP pipeline. Fetched in previous stage instructions are decoded.
    /// </summary>
    public class Decode : TYPStage
    {
        /// <summary>Invoked when <see cref="Pipeline.Stage.ProcessedInstruction"/> (sender) contains <see cref="ISAProperties.InstType.B"/> <see cref="Instruction"/>.</summary>
        public event EventHandler<StageDataArgs> BranchLatched;
        /// <summary>Invoked when <see cref="Pipeline.Stage.ProcessedInstruction"/> (sender) contains <see cref="ISAProperties.InstType.J"/> <see cref="Instruction"/>.</summary>
        public event EventHandler<StageDataArgs> JumpLatched;
        /// <summary>Invoked when <see cref="Pipeline.Stage.ProcessedInstruction"/> (sender) is a FENCE instruction. Decoded as <see cref="ISAProperties.InstType.I"/> type instruction.</summary>
        public event EventHandler<StageDataArgs> FenceDecoded;
        /// <summary>Invoked when <see cref="Pipeline.Stage.ProcessedInstruction"/> (sender) is a Control&Status Register (CSR) instruction (see <see cref="ISAProperties.ISA32.Zicsr"/> extension). Decoded as <see cref="ISAProperties.InstType.I"/> type instruction.</summary>
        public event EventHandler<StageDataArgs> SystemCSRDecoded;
        /// <summary>Invoked when <see cref="Pipeline.Stage.ProcessedInstruction"/> (sender) is a ECALL instruction. Decoded as <see cref="ISAProperties.InstType.I"/> type instruction.</summary>
        public event EventHandler<StageDataArgs> EnvironmentCallDecoded;
        /// <summary>Invoked when <see cref="Pipeline.Stage.ProcessedInstruction"/> (sender) is a EBREAK instruction. Decoded as <see cref="ISAProperties.InstType.I"/> type instruction.</summary>
        public event EventHandler<StageDataArgs> EnvironmentBreakDecoded;

        private Register32 BN_SourceA => BufferNext.A;
        private Register32 BN_SourceB => BufferNext.B;
        private Register32 BN_SignImm => BufferNext.Imm;
        private Register32 BN_ALUOutput => BufferNext.ALUOutput; // branch target address
        private Register32 BN_NextPC => BufferNext.NextPC; // jump target address

        private readonly Register32File RegFile;
        private readonly BranchPredictor Predictor;

        private int? _branchTargetAddress = null;
        private int? _jumpTargetAddress = null;

        public Decode(PipeRegisters decodeBuff, Register32File regfile, BranchPredictor predictor, PipeRegisters dispatchBuff) 
            : base(HardwareProperties.TYPPipelineStage.Decode, prev:decodeBuff, next:dispatchBuff)
        {
            RegFile = regfile;
            Predictor = predictor;
        }


        /// <summary>
        /// Calculates effective address as if <paramref name="inst32"/> was control transfer instruction.
        /// Ignores not alligned addresses, as they will be check in EX stage (see RISC-V spec). 
        /// </summary>
        /// <param name="inst32">Potential Jump or Branch instructon.</param>
        void TryEvaluateAsControlTransferInstruction(in Instruction inst32)
        {
            // JALR only redirects imm to NextPC, cause it uses register value that can only be forwarded up to EX stage
            if (inst32.opcode == Opcodes.OP_I_TYPE_JUMP) // JALR
            {
                _jumpTargetAddress = inst32.imm; // on EX target will be: (rs1 + imm) with bit 0 cleared (imm == NextPC)
            } 
            else if (inst32.opcode == Opcodes.OP_U_TYPE_JUMP) // JAL
            {
                _jumpTargetAddress = unchecked((Int32)((inst32.imm << ISAProperties.JMP_BRANCH_IMM_SHAMT) + LocalPC.ReadUnsigned()));
            }
            else if (inst32.opcode == Opcodes.OP_B_TYPE_BRANCH)
            {
                _branchTargetAddress = unchecked((Int32)((inst32.imm << ISAProperties.JMP_BRANCH_IMM_SHAMT) + LocalPC.ReadUnsigned()));
            }
        }

        /// <summary>
        /// Fills in operands of <paramref name="inst32"/> object base on its <see cref="Instruction.Value"/>.
        /// If <paramref name="inst32"/> is jump/branch, calculates target address and check condition 
        /// (values avaliable after <see cref="Latch"/>). <br></br>
        /// Sets <see cref="Instruction.Illegal"/> flag and if <paramref name="inst32"/> is not known instruction.
        /// </summary>
        /// <param name="inst32"><see cref="Instruction"/> to decode.</param>
        /// <returns><paramref name="inst32"/> object with assigned operand properties.</returns>
        /// <exception cref="DecodeException"></exception>
        /// <exception cref="InstructionAddressMisaligned"></exception>
        public Instruction DecodeInstruction(in Instruction inst32)
        {
            if ((inst32.Value & 0b11) != 0b11)
                throw new DecodeException("Illegal instruction value - must end with binary ...11 sufix");
            
            Decoder.DecodeInstruction(in inst32);
            
            if (inst32.opcode == Opcodes.OPCODE_FENCE)
            {
                FenceDecoded?.Invoke(sender: this, new StageDataArgs(inst32));
            }
            else if (Opcodes.IsSystem(inst32))
            {
                if (inst32.Equals(Opcodes.INSTR_EBREAK))
                    EnvironmentBreakDecoded?.Invoke(sender: this, new StageDataArgs(inst32, null, null, lpc: LocalPC));
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
            _branchTargetAddress = null;
            _jumpTargetAddress = null;
            base.Cycle();
            
            DecodeInstruction(ProcessedInstruction);
            TryEvaluateAsControlTransferInstruction(ProcessedInstruction);
        }

        public override void Latch()
        {
            base.Latch();
            BN_SourceA.Write(RegFile[ProcessedInstruction.rs1]);   // writing read source registers value
            BN_SourceB.Write(RegFile[ProcessedInstruction.rs2]);   // -||-
            BN_SignImm.Write(ProcessedInstruction.imm);            // writing calculated sign-extended immeditate

            if (_branchTargetAddress.HasValue)
            {
                Predictor.SetTargetAddress(_branchTargetAddress);
                BN_ALUOutput.Write(_branchTargetAddress.Value);
                BranchLatched?.Invoke(sender: this, new StageDataArgs(ProcessedInstruction, _branchTargetAddress.Value, lpc:LocalPC));
            } 
            else if (_jumpTargetAddress.HasValue)
            {
                Predictor.SetTargetAddress(_jumpTargetAddress);
                BN_NextPC.Write(_jumpTargetAddress.Value);
                JumpLatched?.Invoke(sender: this, new StageDataArgs(ProcessedInstruction, _jumpTargetAddress.Value, lpc: LocalPC));
            } 
            else
            {
                Predictor.SetTargetAddress(null);
            }
        }
    }
}
