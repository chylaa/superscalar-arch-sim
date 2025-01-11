namespace superscalar_arch_sim.RV32.ISA.Instructions
{
    public enum RV32IOpcode : byte
    {
        R_ARITHMETIC   = Opcodes.OP_R_TYPE_ARITHMETIC,
        B_BRANCH       = Opcodes.OP_B_TYPE_BRANCH,
        I_JUMP         = Opcodes.OP_I_TYPE_JUMP,
        I_ARITHMETIC   = Opcodes.OP_I_TYPE_ARITHMETIC,
        I_LOADS        = Opcodes.OP_I_TYPE_LOADS,
        S_STORE        = Opcodes.OP_S_TYPE_STORE,
        U_JUMP         = Opcodes.OP_U_TYPE_JUMP,
        U_AUIPC        = Opcodes.OPCODE_AUIPC,
        U_LUI          = Opcodes.OPCODE_LUI,
        SYSTEM         = Opcodes.OP_SYSTEM,
    }
    internal static class Opcodes
    {
        public const int OP_R_TYPE_ARITHMETIC   = 0b0110011;
        public const int OP_B_TYPE_BRANCH       = 0b1100011;
        public const int OP_I_TYPE_JUMP         = 0b1100111;
        public const int OP_I_TYPE_ARITHMETIC   = 0b0010011;
        public const int OP_I_TYPE_LOADS        = 0b0000011;
        public const int OP_S_TYPE_STORE        = 0b0100011;
        public const int OP_U_TYPE_JUMP         = 0b1101111;
        public const int OP_SYSTEM              = 0b1110011;

        public const int OPCODE_AUIPC   = 0b0010111;
        public const int OPCODE_LUI     = 0b0110111;
        public const int OPCODE_FENCE   = 0b0001111;

        public const int INSTR_ECALL = 0b000000000000_00000_000_00000_1110011;
        public const int INSTR_EBREAK = 0b000000000001_00000_000_00000_1110011;


        /// <summary><paramref name="i32"/> is Register-Register ALU operation - <see cref="Instruction.opcode"/> equals <see cref="OP_R_TYPE_ARITHMETIC"/></summary>
        public static bool IsRegRegALU(Instruction i32) => i32.opcode == OP_R_TYPE_ARITHMETIC;
        /// <summary>
        /// <paramref name="i32"/> is ALU Immediate operation - <see cref="Instruction.opcode"/> equals 
        /// <see cref="OP_I_TYPE_ARITHMETIC"/> or <see cref="OPCODE_AUIPC"/>/<see cref="OPCODE_LUI"/>.
        /// </summary>
        public static bool IsImmALU(Instruction i32) => (i32.opcode == OP_I_TYPE_ARITHMETIC || i32.opcode == OPCODE_LUI || i32.opcode == OPCODE_AUIPC);

        /// <summary><paramref name="i32"/> is Load instruction - <see cref="Instruction.opcode"/> equals <see cref="OP_I_TYPE_LOADS"/></summary>
        public static bool IsLoad(Instruction i32) => i32.opcode == OP_I_TYPE_LOADS;
        /// <summary><paramref name="i32"/> is Store instruction - <see cref="Instruction.opcode"/> equals <see cref="OP_S_TYPE_STORE"/></summary>
        public static bool IsStore(Instruction i32) => i32.opcode == OP_S_TYPE_STORE;
        /// <summary><paramref name="i32"/> is Branch instruction - <see cref="Instruction.opcode"/> equals <see cref="OP_B_TYPE_BRANCH"/>.</summary>
        public static bool IsBranch(Instruction i32) => i32.opcode == OP_B_TYPE_BRANCH;
        /// <summary><paramref name="i32"/> is Jump instruction - <see cref="Instruction.opcode"/> equals <see cref="OP_I_TYPE_JUMP"/> || <see cref="OP_U_TYPE_JUMP"/>.</summary>
        public static bool IsJump(Instruction i32) => i32.opcode == OP_U_TYPE_JUMP || i32.opcode == OP_I_TYPE_JUMP;
        /// <summary><paramref name="i32"/> is System\CSR instruction - <see cref="Instruction.opcode"/> equals <see cref="OP_SYSTEM"/>.</summary>
        public static bool IsSystem(Instruction i32) => i32.opcode == OP_SYSTEM;
        /// <summary><paramref name="i32"/> is CSR instruction - <see cref="Instruction.opcode"/> equals <see cref="OP_SYSTEM"/> and <see cref="Instruction.funct3"/> is not 0.</summary>
        public static bool IsCSR(Instruction i32) => (i32.opcode == OP_SYSTEM) && (i32.funct3 != 0);
        /// <summary><paramref name="i32"/> is Jump instruction or Branch instruction - <see cref="IsBranch(Instruction)"/> || <see cref="IsJump(Instruction)"/>.</summary>
        public static bool IsControlTransfer(Instruction i32) => (IsBranch(i32) || IsJump(i32));
        /// <summary><paramref name="i32"/> is Branch instruction or Jump-And-Link immediate - target address is known from <see cref="Instruction.imm"/> and PC.</summary> 
        /// <remarks><see cref="IsBranch(Instruction)"/> || <see cref="OP_U_TYPE_JUMP"/>.</remarks>
        public static bool IsImmediateControlTransfer(Instruction i32) => IsBranch(i32) || i32.opcode == OP_U_TYPE_JUMP;

        /// <summary>
        /// <paramref name="i32"/> is Register-Register or Immediate ALU operation - 
        /// <see cref="Instruction.opcode"/> has bit 4 set to '1' (RV32IM ext.)
        /// <br></br><b>Note</b>: currently checked by comparing multiple opcodes to not attract bugs while extending ISA impl.
        /// </summary>
        public static bool IsRegRegOrImmALU(Instruction i32) => IsRegRegALU(i32) || IsImmALU(i32);
        /// <summary>
        /// <paramref name="i32"/> is Register-Register ALU operation or Branch instruction -
        ///  <see cref="Instruction.opcode"/> equals <see cref="OP_R_TYPE_ARITHMETIC"/> || <see cref="OP_B_TYPE_BRANCH"/>.
        /// </summary>
        public static bool IsRegRegALUOrBranch(Instruction i32) => IsRegRegALU(i32) || IsBranch(i32);

    }
}
