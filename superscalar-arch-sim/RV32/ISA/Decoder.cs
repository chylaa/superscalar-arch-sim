using superscalar_arch_sim.RV32.ISA.Instructions;
using static superscalar_arch_sim.Utilis.Utilis;

namespace superscalar_arch_sim.RV32.ISA
{
    public static class Decoder
    {
        /// <summary>Mask for Register Operand value decoding. For <see cref="Instruction.rd"/>, <see cref="Instruction.rs1"/>, <see cref="Instruction.rs2"/> operands.</summary>
        public const int MSKREG = 0b0001_1111;

        private static void DecodeRType(in Instruction i32)
        { // i32.opcode && i32.funct3 // already set
            i32.Type = ISAProperties.InstType.R;

            i32.funct7 = (int)(i32.Value >> 25);
            i32.rs2 = (int)((i32.Value >> 20) & MSKREG);
            i32.rs1 = (int)((i32.Value >> 15) & MSKREG);
            i32.rd = (int)((i32.Value >> 7) & MSKREG);
        }
        /// <summary>Used by <see cref="ISAProperties.ISA32.F"/> instructions.</summary>
        private static void DecodeR4Type(in Instruction i32)
        {
            i32.Type = ISAProperties.InstType.R4;
        }
        private static void DecodeIType(in Instruction i32)
        {
            i32.Type = ISAProperties.InstType.I;

            i32.rs1 = (int)((i32.Value >> 15) & MSKREG);
            i32.rd = (int)((i32.Value >> 7) & MSKREG);
            i32.imm = SignExtendToInt32(((i32.Value >> 20) & 0b1111_1111_1111), 11);
        }

        private static void DecodeSType(in Instruction i32)
        {
            i32.Type = ISAProperties.InstType.S;

            i32.rs2 = (int)((i32.Value >> 20) & MSKREG);
            i32.rs1 = (int)((i32.Value >> 15) & MSKREG);
            uint imm_11_5 = ((i32.Value >> 25) & 0b0111_1111);
            uint imm_4_0 = ((i32.Value >> 7) & 0b0001_1111);
            uint imm_11_0 = ((imm_11_5 << 5) | imm_4_0);
            i32.imm = SignExtendToInt32(imm_11_0, 11);
        }
        /// <summary>
        /// ! Remember to lshift by 1 while executing, for imm_b is produced from bits 12:1 of imm ( imm_b = i32.imm << 1 )
        /// </summary>
        private static void DecodeBType(in Instruction i32)
        {
            i32.Type = ISAProperties.InstType.B;

            i32.rs1 = (int)((i32.Value >> 15) & MSKREG);
            i32.rs2 = (int)((i32.Value >> 20) & MSKREG);
            uint imm_4_1 = ((i32.Value >> 8) & 0b1111);
            uint imm_10_5 = ((unchecked(i32.Value << 1) >> 26) & 0b0011_1111);
            uint imm_11 = ((i32.Value >> 7) & 0b0001);
            uint imm_12 = ((uint)((i32.Value & (1 << 31)) >> 31));
            uint imm_12_1 = (imm_12 << 12 - 1) | (imm_11 << 11 - 1) | (imm_10_5 << 5 - 1) | (imm_4_1 << 1 - 1);
            i32.imm = SignExtendToInt32(imm_12_1, 12 - 1);
        }
        /// <summary>
        /// ! Remember that U type imm produces imm_u value for bits 31:12 ( effectively executed with imm_u = i32.imm << 12 )
        /// </summary>
        private static void DecodeUType(in Instruction i32)
        {
            i32.Type = ISAProperties.InstType.U;

            i32.rd = (int)((i32.Value >> 7) & MSKREG);
            i32.imm = SignExtendToInt32(i32.Value >> 12, 19);
        }
        /// <summary>
        /// ! Remember to lshift by 1 while executing, for imm_j is produced from bits 20:1 of imm ( imm_j = i32.imm << 1 )
        /// </summary>
        private static void DecodeJType(in Instruction i32)
        {
            i32.Type = ISAProperties.InstType.J;

            i32.rd = (int)((i32.Value >> 7) & MSKREG);
            uint imm_20 = ((uint)((i32.Value & (1 << 31)) >> 31));
            uint imm_10_1 = ((i32.Value >> 21) & 0b0011_1111_1111);
            uint imm_11 = ((uint)((i32.Value & (1 << 20)) >> 20) & 0b0001);
            uint imm_19_12 = ((i32.Value >> 12) & 0b1111_1111);
            uint imm_20_1 = (imm_20 << 20 - 1) | (imm_19_12 << 12 - 1) | (imm_11 << 11 - 1) | (imm_10_1 << 1 - 1);
            i32.imm = SignExtendToInt32(imm_20_1, 20 - 1);
        }
        private static void DecodeOpcodeAndFunct3(in Instruction i32)
        {
            i32.opcode = ((int)(i32.Value & 0b1111111));
            i32.funct3 = ((int)((i32.Value & 0b0111_0000_0000_0000) >> 12));
        }

        /// <summary>
        /// Fills in operands of <paramref name="i32"/> object base on its <see cref="Instruction.Value"/>.
        /// </summary>
        /// <param name="i32"><see cref="Instruction"/> object to modify.</param>
        public static Instruction DecodeInstruction(in Instruction i32)
        {
            DecodeOpcodeAndFunct3(i32);

            switch (i32.opcode)
            {
                case Opcodes.OPCODE_AUIPC: // AUIPC
                case Opcodes.OPCODE_LUI: // LUI
                    DecodeUType(in i32);
                    break;

                case Opcodes.OP_U_TYPE_JUMP: // JAL
                    DecodeJType(in i32);
                    break;

                case Opcodes.OP_B_TYPE_BRANCH: // BEQ // BNE // BLT // BGE // BLTU // BGEU
                    DecodeBType(in i32);
                    break;

                case Opcodes.OP_S_TYPE_STORE: // SB // SH // SW
                    DecodeSType(in i32);
                    break;

                case Opcodes.OP_I_TYPE_JUMP: // JALR
                    DecodeIType(in i32);
                    break;

                case Opcodes.OP_I_TYPE_LOADS: // LB // LH // LW // LBU // LHU
                case Opcodes.OP_I_TYPE_ARITHMETIC: // ADDI // SLTI // SLTIU // XORI // ORI // ANDI // SLLI // SRLI // SRAI
                    DecodeIType(in i32);
                    break;

                case Opcodes.OP_R_TYPE_ARITHMETIC: // ADD // SUB // SLL // SLT // SLTU // XOR // SRL // SRA // OR // AND
                                                   // MUL // MULH // MULHSU // MULHU // DIV // DIVU // REM // REMU
                    DecodeRType(in i32);
                    break;

                case 0b0001111: // FENCE
                    DecodeIType(in i32);
                    break;

                case 0b1110011: // CSR // ECALL // EBREAK
                    DecodeIType(in i32);
                    break;

                default:
                    i32.MarkIllegal();
                    break;

            }
            return i32;
        }
    }
}
