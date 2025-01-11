using superscalar_arch_sim.RV32.Hardware.Pipeline.TYP.Units;
using superscalar_arch_sim.RV32.Hardware.Register;
using superscalar_arch_sim.RV32.Hardware.Units;
using superscalar_arch_sim.RV32.ISA.Instructions;
using System;
using static superscalar_arch_sim.RV32.Hardware.Units.BranchPredictor;
using static superscalar_arch_sim.RV32.ISA.ISAProperties;

namespace superscalar_arch_sim.RV32.Hardware.Pipeline.TYP.Stage
{
    /// <summary>
    /// [ <i>ALU/EX</i> ] Third stage of TYP pipeline. Consist of simple ALU, for performing Integer operations 
    /// and optionally FPU (Floating-Point Unit) for floating-point operations.
    /// </summary>
    public class ALUExecute : TYPStage
    {
        /// <summary>Mask for upper 32 bits of 64bit number.</summary>
        private const Int64 MASK_UPPER_XLEN = (0xFF_FF_FF_FFL << 32);

        /// <summary>Bitmask used to extract shift amount (<i>shamt</i>) operand from instruction.</summary>
        private const int SHIFT_MASK = 0b0001_1111;

        /// <summary>Temporary local register, contains value from global <see cref="Register32File"/> pointed by <see cref="Instruction.rs1"/>.</summary>
        private Register32 BP_SourceA => BufferPrev.A;
        /// <summary>Temporary local register, contains value from global <see cref="Register32File"/> pointed by <see cref="Instruction.rs2"/>.</summary>
        private Register32 BP_SourceB => BufferPrev.B;
        /// <summary>Temporary local register, contains sign extended, not shifted value of <see cref="Instruction.imm"/> operand.</summary>
        private Register32 BP_SignImm => BufferPrev.Imm;
        /// <summary>ALU output register from previous buffer - contains target addres for branch instructions.</summary>
        private Register32 BP_ALUOutput => BufferPrev.ALUOutput;
        /// <summary>Next PC register from previous buffer - contains target addres for jump instructions.</summary>
        private Register32 BP_PCNext => BufferPrev.NextPC;

        /// <summary>Buffer register for execution result (effective value). Points to <see cref="Pipeline.Stage.BufferNext"/></summary>
        private Register32 BN_ALUOutput => BufferNext.ALUOutput;
        /// <summary>Buffer register for target address for Jump instructions. Updated in EX only by JALR. Points to <see cref="Pipeline.Stage.BufferNext"/>.</summary>
        private Register32 BN_PCNext => BufferNext.NextPC;

        private bool? EvalCondition = null;
        private Int32? JALRTargetAddress = null; // stores recalculated JALR target address with valid rs1 value
        private Int32 EffectiveValue = Int32.MaxValue;
        bool controlTransferNeccessary = false;

        private readonly BranchPredictor Predictor;

        /// <summary>
        /// Invoked when division by 0 detected. 
        /// Result of an operation is set to -1 (DIV) or value of dividend (REM) as defined in RISC-V ISA spec.
        /// </summary>
        public event EventHandler<StageDataArgs> DivisionByZero;
        /// <summary>
        /// Invoked when signed division overflows (on <see cref="Int32.MinValue"/> / -1). 
        /// Result of an operation is set to <see cref="Int32.MinValue"/> (DIV) or 0 (REM) as defined in RISC-V ISA spec.
        /// </summary>
        public event EventHandler<StageDataArgs> SignedOverflow;
        /// <summary>
        /// Invoked right after branch instruction condition (<see cref="EvalCondition"/>) value 
        /// is known to be <see langword="true"/> or jump instructon executed and target address is not missaligned.
        /// Passes <see cref="ProcessedInstruction"/>, <see cref="BP_ALUOutput"/>, <see cref="BP_PCNext"/> and <see cref="LocalPC"/>
        /// in <see cref="StageDataArgs"/> for control/debugging purposes.
        /// </summary>
        public event EventHandler<StageDataArgs> ControlTransfer;


        public ALUExecute(PipeRegisters prev, BranchPredictor predictor, PipeRegisters next) 
            : base(HardwareProperties.TYPPipelineStage.Execute, prev:prev, next:next)  
        {
            Predictor = predictor;
        }



        private Int32 ExecRType(in Instruction i32)
        {
            // Overflows are ignored and the low XLEN bits of results are written to the destination rd.
            if (i32.opcode == Opcodes.OP_R_TYPE_ARITHMETIC) // arithmetic operations
            {
                int RA = BP_SourceA.Read(); int RB = BP_SourceB.Read();
                uint RAU = BP_SourceA.ReadUnsigned(); uint RBU = BP_SourceB.ReadUnsigned();

                // RV32M Standard Extension
                if (i32.funct7 == 0b0000001)
                {
                    switch (i32.funct3) // DIV [rs1=dividend rs2=divisor] | MUL [rs1=multiplier rs2=multiplicand]
                    {
                        case 0b000: // MUL      [return XLEN bits of product for signed * signed]
                            return (Int32)((RA * RB) & ISA.ISAProperties.UNSIGNED_MAX);
                        case 0b001: // MULH     [return upper XLEN bits of product for signed * signed]
                            return unchecked((Int32)(((RA * (Int64)RB ) & MASK_UPPER_XLEN) >> 32));
                        case 0b010: // MULHSU   [return upper XLEN bits of product for signed * unsigned]
                            return unchecked((Int32)((RA * RBU) & MASK_UPPER_XLEN) >> 32);
                        case 0b011: // MULHU    [return upper XLEN bits of product for unsigned * unsigned]
                            return ((Int32)((RAU * RBU) & MASK_UPPER_XLEN) >> 32);
                        
                        case 0b100: // DIV      [return XLEN bits signed * signed, rounding towards zero]                  
                            if (RB == 0) { 
                                DivisionByZero?.Invoke(this, new StageDataArgs(i32, BP_SourceB, null)); 
                                return -1; 
                            } else if (RA == Int32.MinValue && RB == -1) {
                                SignedOverflow?.Invoke(this, new StageDataArgs(i32, BP_SourceA, BP_SourceB));
                                return Int32.MinValue;
                            } else 
                                return (RA / RB);

                        case 0b101: // DIVU     [return XLEN bits unsigned * unsigned, rounding towards zero]
                            if (RBU == 0) {
                                DivisionByZero?.Invoke(this, new StageDataArgs(i32, BP_SourceB, null));
                                return -1;
                            } else
                                return (Int32)(RAU / RBU);

                        case 0b110: // REM      [return signed remiainder of div, sign equals sign of rs1]
                            if (RA == Int32.MinValue && RB == -1) {
                                SignedOverflow?.Invoke(this, new StageDataArgs(i32, BP_SourceA, BP_SourceB));
                                return 0;
                            } else
                                return (RA % RB);

                        case 0b111: // REMU     [return unsigned remiainder of div]
                            return unchecked((Int32)(RAU % RBU));
                        default:
                            throw new NotImplementedInstructionException(i32, cause: nameof(Instruction.funct3));
                    }
                }
                // else RV32I base 
                switch (i32.funct3) 
                {
                    case 0b000 when i32.funct7 == 0b0000000: // ADD
                        return (Int32)((RA + RB) & ISA.ISAProperties.UNSIGNED_MAX);
                    case 0b000 when i32.funct7 == 0b0100000: // SUB
                        return (Int32)((RA - RB) & ISA.ISAProperties.UNSIGNED_MAX); 
                    case 0b001: // SLL
                        return (RA << RB);
                    case 0b010: // SLT Set Less Than [ set '1' in rd (if rs1 < rs2) else '0' in rd ]
                        return (RA < RB) ? 1 : 0;
                    case 0b011: // SLTU              [ same as SLT but treat numbers as unsigned ]
                        return (RAU < RBU) ? 1 : 0;
                    case 0b100: // XOR
                        return (RA ^ RB);
                    case 0b101 when i32.funct7 == 0b0000000: // SRL
                        return unchecked((Int32)(RAU >> RB));  // shift right logic
                    case 0b101 when i32.funct7 == 0b0100000: // SRA
                        return (RA >> RB);  // shift right arithmetic
                    case 0b110: // OR
                        return (RA | RB);
                    case 0b111: // AND
                        return (RA & RB);
                    default:
                        throw new NotImplementedInstructionException(i32, cause: nameof(Instruction.funct3));
                }
            } 
            else
            {
                throw new NotImplementedInstructionException(i32, cause: nameof(Instruction.opcode));
            }
        }

        private Int32 ExecIType(in Instruction i32)
        {
            if (i32.Value == Opcodes.INSTR_EBREAK)
                return EffectiveValue; // Pass EBREAK

            if (i32.opcode == Opcodes.OP_I_TYPE_ARITHMETIC) // arithmetic operations
            {
                switch (i32.funct3)
                {
                    case 0b000: // ADDI
                        return unchecked((Int32)(((Int64)BP_SourceA.Read() + (Int64)BP_SignImm.Read()) & ISA.ISAProperties.UNSIGNED_MAX)); // overflow ignored (result is simply the low XLEN bits of the result)
                    case 0b010: // SLTI - Set < Immediate [ set '1' in rd (if rs1 < Immediate) else '0' in rd ]
                        return (BP_SourceA.Read() < BP_SignImm.Read()) ? 1 : 0;  
                    case 0b011: // SLTIU - Set < Immediate Unsigned [ same as SLTI but treat numbers as unsigned ]
                        return (BP_SourceA.ReadUnsigned()) < BP_SignImm.ReadUnsigned() ? 1 : 0;  
                    case 0b100: // XORI
                        return (BP_SourceA.Read() ^ BP_SignImm.Read());
                    case 0b110: // ORI
                        return (BP_SourceA.Read() | BP_SignImm.Read());
                    case 0b111: // ANDI
                        return (BP_SourceA.Read() & BP_SignImm.Read());
                    case 0b001: // SLLI [shift left logical imm]
                        return (BP_SourceA.Read() << (BP_SignImm.Read() & SHIFT_MASK)); // shift amout encoded in lower 5 bits of value
                    case 0b101: // SRLI, SRAI
                        if (((BP_SignImm.Read() & (1 << 10)) >> 10) == 0) // SRLI [shift right logical imm] - ignore sign-bit: fill with zeroes
                            return unchecked((int)((BP_SourceA.ReadUnsigned() >> (BP_SignImm.Read() & SHIFT_MASK)))); // C#: If the left-hand operand is of type uint or ulong, the right-shift operator performs a LOGICAL shift.
                        else // SRAI [shift right arithmetic imm] - preserve sign-bit: fill with ones
                            return (BP_SourceA.Read() >> (BP_SignImm.Read() & SHIFT_MASK)); // C#: If the left-hand operand is int or long, the right-shift operator performs a ARITMETIC shift:
                    default:
                        throw new NotImplementedInstructionException(i32, cause: nameof(Instruction.funct3));
                }
            } 
            else if (i32.opcode == Opcodes.OP_I_TYPE_LOADS) // Load instructions
            {
                switch (i32.funct3) // returns effective address for LOAD operation (addr in register source 1 + immeditate value)
                {
                    case 0b000: case 0b001: case 0b010: case 0b100: case 0b101: // LB: LH: LW: LBU: LHU 
                        return (BP_SignImm.Read() + BP_SourceA.Read());
                    default:
                        throw new NotImplementedInstructionException(i32, cause: nameof(Instruction.funct3));
                }
            }
            else if (i32.opcode == Opcodes.OP_I_TYPE_JUMP) // JALR
            {// apply rs1 to Imm stored in PCNext to get final target address of Jump instruction from ID stage
                JALRTargetAddress = ((BP_SourceA.Read() + BP_PCNext.Read()) & ~1);
                if (false == Utilis.Utilis.IsAlligned((int)JALRTargetAddress, Memory.Allign.WORD))
                    throw new InstructionAddressMisaligned((uint)JALRTargetAddress, i32, Memory.Allign.WORD, LocalPC.ToString());

                controlTransferNeccessary = true;
                Int32 nexti32addr = (LocalPC.Read() + WORD_BYTESIZE);
                return nexti32addr; // return: address of the next instruction (written to ALUOutput and then to RegFile[rd] on WB)
            }
            else
            {
                throw new NotImplementedInstructionException(i32, cause:nameof(Instruction.opcode));
            }
        }
        private Int32 ExecSType(in Instruction i32) 
        {
            if (i32.opcode == Opcodes.OP_S_TYPE_STORE)
            {
                if (i32.funct3 <= 2) // 0b000 - SB, 0b001 - SH, 0b010 - SW
                    return (BP_SignImm.Read() + BP_SourceA.Read()); // The effective address is obtained by adding register rs1 to the sign-extended 12bit imm
                else
                    throw new NotImplementedInstructionException(i32, cause: nameof(Instruction.funct3));
            } 
            else 
                throw new NotImplementedInstructionException(i32, cause:nameof(Instruction.opcode));
        }
        private Int32 ExecUType(in Instruction i32)
        {
            if (i32.opcode == 0b0110111) // LUI
                return (BP_SignImm.Read() << 12);
            else if (i32.opcode == 0b0010111) // AUIPC 
                // Add address of 'AUIPC' instruction to specified upper imm
                return unchecked((Int32)(UInt32.MaxValue & ((BP_SignImm.Read() << 12) + LocalPC.ReadUnsigned()) )); // (& UInt32.MaxValue to not overflow but 'cut' to only 32bit's [following RISC-V ISA])
            else
                throw new NotImplementedInstructionException(i32, cause: nameof(Instruction.opcode));
        }

        private Int32 ExecBType(in Instruction i32)
        {
            switch (i32.funct3)
            {
                case 0b000: // BEQ 
                    EvalCondition = (BP_SourceA.Compare(BP_SourceB));
                    break;
                case 0b001:// BNE
                    EvalCondition = !(BP_SourceA.Compare(BP_SourceB));
                    break;
                case 0b100:// BLT (Branch if rs1 less than rs2 [signed])
                    EvalCondition = (BP_SourceA.Read() < BP_SourceB.Read());
                    break;
                case 0b101:// BGE (Branch if rs1 greater or Equal rs2 [signed])
                    EvalCondition = (BP_SourceA.Read() >= BP_SourceB.Read());
                    break;
                case 0b110:// BLTU -||- [unsigned]
                    EvalCondition = (BP_SourceA.ReadUnsigned() < BP_SourceB.ReadUnsigned());
                    break;
                case 0b111:// BGEU -||- [unsigned]
                    EvalCondition = (BP_SourceA.ReadUnsigned() >= BP_SourceB.ReadUnsigned());
                    break;

                default:
                    throw new NotImplementedInstructionException(i32, cause: nameof(Instruction.funct3));
            }

            Int32 targetaddr = BP_ALUOutput.Read(); // target address of Branch instruction from ID stage
            if (EvalCondition.Value && false == Utilis.Utilis.IsAlligned(targetaddr, Memory.Allign.WORD))
                throw new InstructionAddressMisaligned(LocalPC.ReadUnsigned(), i32, Memory.Allign.WORD, LocalPC.ToString());
            
            controlTransferNeccessary = EvalCondition.Value;
            return targetaddr;
        }

        /// <summary>
        /// JAL instruction saves the next address (CurrentProgram Counter +4) to the destination register, 
        /// adds the immediate value encoded in the instruction to the CurrentProgram Counter,
        /// and jumps to that address
        /// </summary>
        /// <returns>Address of next instruction</returns>
        private Int32 ExecJType(in Instruction i32) // JAL
        {
            Int32 targetaddr = BP_PCNext.Read(); // target address of Jump instruction from ID stage
            if (false == Utilis.Utilis.IsAlligned(targetaddr, Memory.Allign.WORD))
                throw new InstructionAddressMisaligned((uint)targetaddr, i32, Memory.Allign.WORD, LocalPC.ToString());

            Int32 nexti32addr = (LocalPC.Read() + WORD_BYTESIZE); 
            controlTransferNeccessary = targetaddr != nexti32addr;
            return nexti32addr; // return: address of the next instruction (written to ALUOutput and then to RegFile[rd] on WB)
        }


        /// <summary><inheritdoc/></summary>
        /// <exception cref="NotImplementedInstructionException"></exception>
        public override void Cycle()
        {
            base.Cycle();
            controlTransferNeccessary = false;
            EvalCondition = null;

            switch (ProcessedInstruction.Type) 
            {
                case InstType.R:
                    EffectiveValue = ExecRType(ProcessedInstruction);
                    break;
                case InstType.I:
                    EffectiveValue = ExecIType(ProcessedInstruction);
                    break;
                case InstType.S:
                    EffectiveValue = ExecSType(ProcessedInstruction);
                    break;
                case InstType.B:
                    EffectiveValue = ExecBType(ProcessedInstruction);
                    break;
                case InstType.U:
                    EffectiveValue = ExecUType(ProcessedInstruction);
                    break;
                case InstType.J:
                    EffectiveValue = ExecJType(ProcessedInstruction);
                    break;
                default:
                    throw new NotImplementedInstructionException(ProcessedInstruction, cause: nameof(Instruction.Type));
            }
        }

        public override void Latch()
        {
            base.Latch();
            BN_ALUOutput.Write(EffectiveValue);
            BufferNext.Condition = EvalCondition;
            
            Predictor.SetEvaluatedConditionValue(EvalCondition);
            if (Opcodes.IsControlTransfer(ProcessedInstruction))
            {
                if (JALRTargetAddress.HasValue)
                {
                    Predictor.SetTargetAddress(JALRTargetAddress);
                    BN_PCNext.Write(JALRTargetAddress.Value);
                    JALRTargetAddress = null;
                }
                if (controlTransferNeccessary) // on jump or branch with 'true' condtition signal is send from EX stage
                {
                    ControlTransfer?.Invoke(this, new StageDataArgs(ProcessedInstruction, BP_ALUOutput, BP_PCNext, lpc: LocalPC));
                }
                Predictor.UpdateBranchHistory(ProcessedInstruction, LocalPC, EvalCondition);
            }
        }
    }
}
