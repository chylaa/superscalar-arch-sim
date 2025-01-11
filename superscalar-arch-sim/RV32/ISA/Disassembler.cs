using superscalar_arch_sim.RV32.ISA.Instructions;
using System;
using System.Collections.Generic;

namespace superscalar_arch_sim.RV32.ISA
{
    //Initial verification: https://luplab.gitlab.io/rvcodecjs/#q=ADDI+x0,+x0,+0&abi=false&isa=RV32I

    public static class Disassembler
    {
        private const int REG_ABI_NAME_OFFSET = 32;

        public class DecodeException : Exception { public DecodeException(string msg) : base(msg) { } }

        private static int _offset = 0;
        /// <summary>
        /// If set to <see langword="true"/>, register x-number names will be replaced with ABI mnemonic names in
        /// <see cref="DecodeToHumanReadable(Instruction, bool?)"/>. Default is <see langword="false"/>.
        /// </summary>
        public static bool UseABIRegisterMnemonic { get => (_offset == REG_ABI_NAME_OFFSET); set => _offset = value ? REG_ABI_NAME_OFFSET : 0; }

        private static readonly Dictionary<uint, string> DisassemblyCache = new Dictionary<uint, string>();

        private static readonly string[] RegistersNames = new string[64] {
            "x0",
            "x1","x2","x3","x4",
            "x5","x6","x7",
            "x8","x9","x10","x11","x12","x13","x14","x15","x16","x17",
            "x18","x19","x20","x21","x22","x23","x24","x25","x26","x27",
            "x28","x29","x30","x31",

            "x0",
            "ra","sp","gp","tp",
            "t0","t1","t2",
            "s0","s1","a0","a1","a2","a3","a4","a5","a6","a7",
            "s2","s3","s4","s5","s6","s7","s8","s9","s10","s11",
            "t3","t4","t5","t6"
        };

        private static readonly Dictionary<int, string[]> OpcodeFunc3ToName = new Dictionary<int, string[]>()
        {
            { Opcodes.OPCODE_LUI, new string[] {"LUI" } },
            { Opcodes.OPCODE_AUIPC, new string[] { "AUIPC" } },
            { Opcodes.OP_U_TYPE_JUMP, new string[] { "JAL" } },
            { Opcodes.OP_I_TYPE_JUMP, new string[] { "JALR" } },
            { Opcodes.OP_B_TYPE_BRANCH, new string[] { "BEQ", "BNE", null, null, "BLT", "BGE", "BLTU", "BGEU" } },
            { Opcodes.OP_I_TYPE_LOADS, new string[] { "LB", "LH", "LW", null, "LBU", "LHU" } },
            { Opcodes.OP_S_TYPE_STORE, new string[] { "SB", "SH", "SW" } },
            { Opcodes.OP_I_TYPE_ARITHMETIC, new string[] { "ADDI", "SLLI", "SLTI", "SLTIU", "XORI", "SRLI", "ORI", "ANDI" } },
            { Opcodes.OPCODE_FENCE, new string[] { "FENCE" } },
            { Opcodes.OP_SYSTEM, new string[] { null, "CSRRW", "CSRRS", "CSRRC", "CSRRWI", "CSRRSI", "CSRRCI" } }
        };

        private static readonly Dictionary<int, Dictionary<int?, string[]>> OpcodeFunc7Func3ToName = new Dictionary<int, Dictionary<int?, string[]>>()
        {
            { Opcodes.OP_R_TYPE_ARITHMETIC, new Dictionary<int?, string[]>() {
                                {0b0000000, new string[] {"ADD", "SLL", "SLT", "SLTU", "XOR", "SRL", "OR", "AND"} },
                                {0b0100000, new string[] {"SUB", null, null, null, null, "SRA", null, null} },
                                //RV32M Standard Extension
                                {0b0000001, new string[] {"MUL", "MULH", "MULHSU", "MULHU", "DIV", "DIVU", "REM", "REMU" } }, 
             }},
        };

        /// <summary>
        /// Returns name of <see cref="Instruction"/> base on <see cref="Instruction.opcode"/> and <see cref="Instruction.funct3"/> properties.
        /// Currently, only base <see cref="ISAProperties.ISA32.I"/> instructions supported.     
        /// Always checks first if <see cref="Instruction.NOP"/> was passed, and returns "NOP" in that case.
        /// </summary>
        /// <param name="i32">Instruction which name should be decoded, with opcode and (optionally) funct3 fields set.</param>
        /// <returns>Name of <paramref name="i32"/>, "NOP" if <paramref name="i32"/> equals <see cref="Instruction.NOP"/> or null if instruction was not recognized.</returns>
        private static string DecodeName(Instruction i32)
        {
            if (i32.IsNop()) 
                return "NOP";
            
            // Catch Environment instruction before OpcodeFunc3ToName Dictionary usage
            if (i32.opcode == Opcodes.OP_SYSTEM && i32.funct3 == 0)
            {
                if ((i32.Value & 1 << 20) >> 20 == 0) return "ECALL";
                else return "EBREAK";
            }
            // special case for I-type SRAI, due to imm11 split
            if (i32.opcode == 0b0010011 && i32.funct3 == 0b101 && ((i32.imm & 1<<10)>>10 == 1)) 
                return "SRAI";

            if (false == OpcodeFunc3ToName.TryGetValue(i32.opcode, out string[] names))
            {
                if (false == OpcodeFunc7Func3ToName.TryGetValue(i32.opcode, out Dictionary<int?, string[]> func7names))
                    return null;
                else if (false == func7names.TryGetValue(i32.funct7, out names))
                    return null;
            }

            if (names.Length == 1) 
                return names[0];

            if (names.Length > i32.funct3)
                return names[i32.funct3];
            
            return null;
        }
        /// <summary>
        /// Decodes <paramref name="i32"/> instruction to human-readable assembly format base on operands values.
        /// If <see langword="static"/> <see cref="UseABIRegisterMnemonic"/> flag is set to <see langword="true"/>,
        /// register x-number names will be replaced with ABI mnemonic names.
        /// </summary> 
        /// <returns><see cref="String"/> containing name of instruction with formatted register/immeditate arguments.</returns>
        public static string DecodeToHumanReadable(in Instruction i32)
        {
            if (i32.Illegal)
                return Instruction.ASMInstructionIllegal;
            if (i32.IsNop())
                return "NOP";

            string rs1 = RegistersNames[_offset + i32.rs1];
            string rs2 = RegistersNames[_offset + i32.rs2];
            string rd = RegistersNames[_offset + i32.rd]; 
            string name = DecodeName(i32);

            switch (i32.Type) 
            {
                case ISAProperties.InstType.R:
                    return $"{name} {rd}, {rs1}, {rs2}";
                case ISAProperties.InstType.I:
                    switch (i32.opcode)
                    {
                        case Opcodes.OP_I_TYPE_JUMP:
                        case Opcodes.OP_I_TYPE_LOADS:
                            return $"{name} {rd}, {i32.imm}({rs1})";
                        case Opcodes.OP_SYSTEM: // CSR's
                            if (i32.Value == Opcodes.INSTR_EBREAK) return name;
                            return $"{name} {rd}, {i32.imm}, {rs1}";
                        default: // Rest of I types
                            return $"{name} {rd}, {rs1}, {i32.imm}";
                    }
                case ISAProperties.InstType.S:
                    return $"{name} {rs2}, {i32.imm}({rs1})";
                case ISAProperties.InstType.B:
                    return $"{name} {rs1}, {rs2}, {i32.imm << ISAProperties.JMP_BRANCH_IMM_SHAMT}"; // imm_b produced by lshift by one!
                case ISAProperties.InstType.U:
                    return $"{name} {rd}, {i32.imm}"; // not shif ON DECODE, even though the effective value of imm_u is produced by lshift by twelve!
                case ISAProperties.InstType.J:
                    return $"{name} {rd}, {i32.imm << ISAProperties.JMP_BRANCH_IMM_SHAMT}"; //// imm_j produced by lshift by one!
                default: 
                    return name;
            }
        }

        public static string DecodeToHumanReadableWithCaching(in Instruction i32)
        {
            if (DisassemblyCache.TryGetValue(i32.Value, out string asm))
            {
                return asm;
            }
            DisassemblyCache[i32.Illegal ? uint.MaxValue : i32.Value] = (asm = DecodeToHumanReadable(i32));
            return asm;
        }

        public static string GetRegisterAbiName(int r)
            => RegistersNames[REG_ABI_NAME_OFFSET + r];
    }
}

/*
 * 
imm[31:12] rd 						 0110111 LUI
imm[31:12] rd 						 0010111 AUIPC
imm[20|10:1|11|19:12] rd 			 1101111 JAL
imm[11:0] rs1 000 rd 				 1100111 JALR
imm[12|10:5] rs2 rs1 000 imm[4:1|11] 1100011 BEQ
imm[12|10:5] rs2 rs1 001 imm[4:1|11] 1100011 BNE
imm[12|10:5] rs2 rs1 100 imm[4:1|11] 1100011 BLT
imm[12|10:5] rs2 rs1 101 imm[4:1|11] 1100011 BGE
imm[12|10:5] rs2 rs1 110 imm[4:1|11] 1100011 BLTU
imm[12|10:5] rs2 rs1 111 imm[4:1|11] 1100011 BGEU
imm[11:0] rs1 000 rd 				 0000011 LB
imm[11:0] rs1 001 rd 				 0000011 LH
imm[11:0] rs1 010 rd 				 0000011 LW
imm[11:0] rs1 100 rd 				 0000011 LBU
imm[11:0] rs1 101 rd 				 0000011 LHU
imm[11:5] rs2 rs1 000 imm[4:0] 		 0100011 SB
imm[11:5] rs2 rs1 001 imm[4:0] 		 0100011 SH
imm[11:5] rs2 rs1 010 imm[4:0] 		 0100011 SW
imm[11:0] rs1 000 rd 				 0010011 ADDI
imm[11:0] rs1 010 rd 				 0010011 SLTI
imm[11:0] rs1 011 rd 				 0010011 SLTIU
imm[11:0] rs1 100 rd 				 0010011 XORI
imm[11:0] rs1 110 rd 				 0010011 ORI
imm[11:0] rs1 111 rd 				 0010011 ANDI
0000000 shamt rs1 001 rd 			 0010011 SLLI
0000000 shamt rs1 101 rd 			 0010011 SRLI
0100000 shamt rs1 101 rd 			 0010011 SRAI
0000000 rs2 rs1 000 rd 				 0110011 ADD
0100000 rs2 rs1 000 rd 				 0110011 SUB
0000000 rs2 rs1 001 rd 				 0110011 SLL
0000000 rs2 rs1 010 rd 				 0110011 SLT
0000000 rs2 rs1 011 rd 				 0110011 SLTU
0000000 rs2 rs1 100 rd 				 0110011 XOR
0000000 rs2 rs1 101 rd 				 0110011 SRL
0100000 rs2 rs1 101 rd 				 0110011 SRA
0000000 rs2 rs1 110 rd 				 0110011 OR
0000000 rs2 rs1 111 rd 				 0110011 AND
fm pred succ rs1 000 rd 			 0001111 FENCE
000000000000 00000 000 00000 		 1110011 ECALL
000000000001 00000 000 00000 		 1110011 EBREA
  
 */