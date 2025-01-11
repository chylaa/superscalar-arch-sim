using superscalar_arch_sim.RV32.ISA.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 

namespace superscalar_arch_sim.RV32.ISA
{
    public static class ISAProperties
    {
        public static readonly Type DWORD = typeof(Int64);
        public static readonly Type WORD = typeof(Int32);
        public static readonly Type HALF_WORD = typeof(Int16);
        public static readonly Type BYTE = typeof(Byte);

        /// <summary>Count of Instrucion Set WORD in bits</summary>
        public const int ILEN = 32;
        /// <summary>Instruction alligment in memory to 32bit boundary (4 bytes).</summary>
        public const int IALLIGN = 32;
        /// <summary>Instruction Sign Bit position - '... the sign bit for all immediates is always in bit 31 of the instruction to speed sign-extension circuitry'</summary>
        public const int ISIGN = 31;
        
        public const uint UNSIGNED_MAX = UInt32.MaxValue;
        /// <summary>Count of Instrucion Set WORD in bytes</summary>
        public const int WORD_BYTESIZE = 4;
        /// <summary><see langword="true"></see> if default WORD reffers to signed type, <see langword="false"></see> otherwsie.</summary>
        public const bool IS_WORD_SIGNED = true;
        /// <summary><see langword="true"></see> if ISA uses Little-Endiann data format (always in RISC-V) .</summary>
        public const bool LITTLE_ENDIAN = true;

        /// <summary>Branch/jump effective address, <see cref="Instruction.imm"/> field left-shift amount.<br></br>
        /// TODO [CHECK]: 1 or 2? For now multiple of 4 (as in CA Quantative Approach Appendix 3 C-28 page 802)</summary>
        public const int JMP_BRANCH_IMM_SHAMT = 1;

        /// <summary>Number of architectural integer registers.</summary>
        public const int NO_INT_REGISTERS = 32;
        /// <summary>Number of architectural floating-point registers.</summary>
        public const int NO_FP_REGISTERS = 32;

        /// <summary>
        /// Allows to set which instruction value should be treated as no operation instruction.
        /// In 32bit Integer ISA, NOP is encoded as ADDI x0, x0, 0.
        /// </summary>
        public static UInt32 NOP_Instruction { get; set; } = 0b00000000_00000000_00000000_00010011;


        /// <summary>
        /// Encodes default instruction types for RV32 Integer Instruction Set
        /// </summary>
        public enum InstType {
            Invalid = 0,
            /// <summary><b>Register-register</b> instructions use only registers as source and destiantions. 
            /// This instruction type is mostly used for arithmetic and logic operations involving the ALU.</summary>
            R = 1,
            /// <summary><b>Immediate</b> instructions has one of the two source operands specified 
            /// within the 32-bit instruction word as a 12-bit constant (or immediate). This
            /// constant is regards as 12-bit signed 2’s complement number, which is always
            /// sign extended to form a 32-bit operand.</summary>
            I = 2,
            /// <summary><b>Store</b> instructions are exclusively used for storing contents of a register to data memory</summary>
            S = 3,
            /// <summary><b>Branch</b> instructions are used to control program flow. 
            /// It compares two operands stored in registers and branch to a destination address relative
            /// to the current CurrentProgram Counter value. On execution, B-Type immediate specifies bits 20:1 of value.</summary>
            B = 4,
            /// <summary><b>Upper-immediate</b> instructions are used to specify the upper 20 bits of immediate value from a register
			/// (U-Type immediate specifies bits 31:12 of value).</summary>
            U = 5,
            /// <summary><b>Jump</b> instructions are used for subroutine calls. 
			/// J-Type immediate specifies bits 20:1 of effective address</summary>
            J = 6,
            /// <summary>
            /// <see cref="ISA32.F"/> instructions
            /// </summary>
            R4 = 7,
        }

        /// <summary> Operand types of <see cref="InstType"/></summary>
        public enum IOperands { 
            /// <summary>Immeditate value (almost always sign extended).</summary>
            imm, 
            /// <summary>Function 7-bit value</summary>
            funct7, 
            /// <summary>Register Source (operand) 2 or "shamt" (shift amount)</summary>
            rs2, 
            /// <summary>Register Source (operand) 1</summary>
            rs1, 
            /// <summary>Function 3-bit value</summary>
            funct3, 
            /// <summary>Register Destination</summary>
            rd, 
            /// <summary>Instruction RV32IOpcode</summary>
            opcode  
        }

        /// <summary>
        /// Encodes all standard (32 bit) Instruction sets from standard "I" (Integer) up to "G" (General-purpose, as combination of all) 
        /// </summary>
        [Flags]
        public enum ISA32 
        { 
            /// <summary>Base Integer Instruction Set</summary>
            I = 0x00,
            /// <summary>Standard Extension for Integer Multiplication and Division</summary>
            M = 0x01,
            /// <summary> Standard Extension for Atomic Instructions, Version 2.1 47</summary>
            A = 0x02,
            /// <summary>Control and Status Register (CSR) Instructions</summary>
            Zicsr = 0x04,
            /// <summary> Standard Extension for Single-Precision Floating-Point, Version 2.2 63</summary>
            F = 0x08 | Zicsr,
            /// <summary> Standard Extension for Double-Precision Floating-Point, Version 2.2 73</summary>
            D = 0x10 | F,
            /// <summary> Standard Extension for Compressed Instructions, Version 2.0 97</summary>
            C = 0x20,
            /// <summary>Instruction-Fetch Fence</summary>
            Zifencei = 0x40,
            /// <summary>
            /// Defines a combination of a base ISA (<see cref="I"/>) plus selected standard
            /// extensions as a “general-purpose” ISA. Abbreviation G is used for
            /// IMAFDZicsr Zifencei combination of instruction-set extensions
            /// </summary>
            G = I | M | A | F | D | C | Zicsr | Zifencei,

        }
    }
}
