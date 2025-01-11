using System;
using System.Linq;
using System.Text;
using static superscalar_arch_sim.RV32.ISA.ISAProperties;

namespace superscalar_arch_sim.RV32.ISA.Instructions
{
    /// <summary>
    /// Represents CPU's 32bit Instruction of specific <see cref="ISAProperties.InstType"/>, 
    /// that consist of different operands.
    /// </summary>
    public class Instruction
    {
        /// <summary>Default string assigned as <see cref="ASM"/> when <see cref="MarkIllegal"/> called.</summary>
        public const string ASMInstructionIllegal = "<illegal>";
        /// <summary>Default value for operands.</summary>
        public const Int32 OperandDefaultValue = 0;
        /// <summary>Value indicating that operand value should not be accessed.</summary>
        public const Int32 OperandNotApplicable = Int32.MaxValue;
        /// <summary>No operation instruction which value is defined in <see cref="ISAProperties.NOP_Instruction"/>.</summary>
        public static readonly Instruction NOP = CreateFullNewNop();

        #region Operands
        /// <summary><inheritdoc cref="ISAProperties.IOperands.imm"/></summary>
        public Int32 imm { get; set; } = OperandDefaultValue;
        /// <summary><inheritdoc cref="ISAProperties.IOperands.funct7"/></summary>
        public Int32 funct7 {get; set;} = OperandDefaultValue;
        /// <summary><inheritdoc cref="ISAProperties.IOperands.rs2"/></summary>
        public Int32 rs2 {get; set;} = OperandDefaultValue;
        /// <summary><inheritdoc cref="ISAProperties.IOperands.rs1"/></summary>
        public Int32 rs1 {get; set;} = OperandDefaultValue;
        /// <summary><inheritdoc cref="ISAProperties.IOperands.funct3"/></summary>
        public Int32 funct3 {get; set;} = OperandDefaultValue;
        /// <summary><inheritdoc cref="ISAProperties.IOperands.rd"/></summary>
        public Int32 rd {get; set;} = OperandDefaultValue;
        /// <summary><inheritdoc cref="ISAProperties.IOperands.opcode"/></summary>
        public Int32 opcode {get; set;} = OperandDefaultValue;
        #endregion

        #region General Properties
        /// <summary><inheritdoc cref="ISAProperties.InstType"/></summary>
        public ISAProperties.InstType Type { get; set; }
        /// <summary>Unsigned WORD representation of encoded instruction.</summary>
        public UInt32 Value { get; set; } = ISAProperties.NOP_Instruction;
        #endregion

        #region Additional Properties
        /// <summary>Set to <see langword="true"/> only in <see cref="NOP"/> to indicate it does not come from program.</summary>
        public bool BubbleInstruction { get; private set; } = false;        
        /// <summary>Flag marking current instruction object as Illegal/Invalid/Unknown - not supported by ISA/Simulator. Initialized with <see langword="false"/></summary>
        public bool Illegal { get; set; } = false;
        /// <summary>
        /// Human-readable representation of instruction in RISC-V assembly. Initialized with <see langword="null"/>. 
        /// <br></br><i>Longest representation in RV32I does not have more than 20 chars</i>.
        /// </summary>
        public string ASM { get; set; } = null;
        #endregion

        #region Constructors
        /// <summary>Creates new instruction with parameter <see cref="Value"/> set as default to <see cref="ISAProperties.NOP_Instruction"/>.</summary>
        public Instruction() { }
        
        /// <summary>Creates new instruction with parameter <see cref="Value"/> set to <paramref name="value"/>.</summary>
        public Instruction(UInt32 value) { Value = value; }

        #endregion

        #region Methods

        /// <summary>Creates <see langword="new"/> NOP <see cref="Instruction"/> with <see cref="InstType.I"/> fields set to 0.</summary>
        /// <returns><see langword="new"/> NOP <see cref="Instruction"/> object.</returns>
        static private Instruction CreateFullNewNop()
            => new Instruction(ISAProperties.NOP_Instruction) { Type = InstType.I, opcode = (int)ISAProperties.NOP_Instruction, 
                                                                imm = 0, funct3 = 0, rs1 = 0, rd = 0, ASM = "NOP", BubbleInstruction = true};
        
        /// <summary>Compares to <see cref="Instruction"/> instances by their <see cref="Value"/>.</summary>
        /// <returns><see langword="true"/> if both <see langword="this"/> and <paramref name="inst"/> property <see cref="Value"/> are equal.</returns>
        public bool Equals(Instruction inst) => (Value == inst.Value);
        /// <summary>Compares give value-representation of instruction to <see cref="Value"/> of calling instance.</summary>
        /// <returns><see langword="true"/> if <paramref name="instval"/> equals <see langword="this"/> <see cref="Value"/>.</returns>
        public bool Equals(uint instval) => (Value == instval);


        /// <summary>
        /// Checks if this <see cref="Instruction"/> object is no-operation instruction. 
        /// Works the same as using <see cref="Equals(Instruction)"/> with <see cref="NOP"/> parameter.
        /// </summary>
        /// <returns><see langword="true"/> if current object is no operation instruction.</returns>
        public bool IsNop() => (Value == ISAProperties.NOP_Instruction);

        /// <summary>Creates shallow copy of current <see cref="Instruction"/>.</summary>
        /// <returns>A shallow copy of current <see cref="Instruction"/>.</returns>
        public Instruction GetCopy() => (Instruction)MemberwiseClone();

        /// <summary>Sets <see cref="Illegal"/> property to <see langword="true"/> and <see cref="ASM"/> representation to "<illegal>"</summary>
        public void MarkIllegal()
        {
            Illegal = true;
            ASM = ASMInstructionIllegal;
        }

        public override string ToString() => ASM??("0x"+Value.ToString("X8"));

        public string ToString(string format)
        {
            if (format == "I") 
            {
                StringBuilder sb = new StringBuilder(); 
                foreach (var p in GetType().GetProperties()) 
                {
                    if ((p.PropertyType == typeof(int) || p.PropertyType == typeof(uint)))
                        sb.AppendLine($"{p.Name} = {p.GetValue(this)}");
                }
                return sb.ToString();
            }
            if (format.First() == 'B') 
                return Convert.ToString(Value, 2).PadLeft(int.Parse(format.Remove(0, 1)), '0');
            
            return Value.ToString(format);
        }
        #endregion
    }
}
