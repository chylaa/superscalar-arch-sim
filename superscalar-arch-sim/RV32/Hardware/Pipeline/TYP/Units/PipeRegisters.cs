using superscalar_arch_sim.RV32.Hardware.Pipeline.TYP.Stage;
using superscalar_arch_sim.RV32.Hardware.Register;
using superscalar_arch_sim.RV32.ISA.Instructions;
using System;


namespace superscalar_arch_sim.RV32.Hardware.Pipeline.TYP.Units
{
    /// <summary>
    /// Pipeline Buffer Registers for passing data between consecutive stages. 
    /// Imitates hardware register in pipelined architecture. 
    /// </summary>
    public class PipeRegisters : IPipelineBuffer
    {
        public static readonly Type RelatedPipelineStage = typeof(TYPStage); 

        /// <summary>Creates new pipeline register object with <see cref="IsVirtual"/> property set to <see langword="true"/>.</summary>
        public static PipeRegisters GetNewVirtualRegister(string name = null) => new PipeRegisters() { IsVirtual = true, Name = name };

        /// <summary>Optional name of <see cref="PipeRegisters"/> instance for debugging purposes.</summary>
        public string Name { get; set; } = null;
        /// <summary>If set to <see langword="true"/> indicates that <see cref="PipeRegisters"/> is added as additional 'virtual' buffer to add pipeline depth.</summary>
        public bool IsVirtual { get; private set; } = false;        
        /// <summary>Passed between <see cref="PipeRegisters"/> indicates that following <see cref="Stage"/> should stall.</summary>
        //public bool StallSignal { get; set; }
        
        /// <summary>Stores Address of <see cref="IR32"/>.</summary>
        public Register32 LocalPC { get; private set; } = new Register32("LPC", name: "Local Program Counter");
        /// <summary>Stores next address after <see cref="IR32"/>.</summary>
        public Register32 NextPC { get; private set; } = new Register32("NPC", name: "Next Program Counter");

        /// <summary>Temporary Register storing value from <see cref="Register32"/> selected by <see cref="Instruction.rs1"/>. Set in decoding stage.</summary>
        public Register32 A { get; private set; } = new Register32("A", name: "Operand A");
        /// <summary>Temporary Register storing value from <see cref="Register32"/> selected by <see cref="Instruction.rs2"/>. Set in decoding stage.</summary>
        public Register32 B { get; private set; } = new Register32("B", name: "Operand B");
        /// <summary>Temporary Register storing effective value of <see cref="Instruction.imm"/> field. Set in decoding stage.</summary>
        public Register32 Imm { get; private set; } = new Register32("IMM", name: "Sign-Extended Immediate");
        /// <summary>(Result data) Effective value/address (or register destination) being the result of <see cref="IR32"/> <see cref="Instruction"/> execution.</summary>
        public Register32 ALUOutput { get; private set; } = new Register32("OUT", name: "ALU Output");
        /// <summary>(LMD) Contains data from memory, requested on Decode stage. Applicable in/after <b>MEM</b> stage</summary>
        public Register32 LoadMemoryData { get; private set; } = new Register32("LMD", name: "Load Memory Data");

        /// <summary>Result of condition from branch instruction (<see cref="ISA.ISAProperties.InstType.B"/>). Applicable in/after <b>EX</b> stage.</summary>
        public bool? Condition { get; set; } = null;

        /// <summary>(Instruction Register) Processed <see cref="IR32"/> object.</summary>
        public Instruction IR32 { get; set; } = null;

        /// <summary>Allows to keep track of <see cref="IR32"/> order of creation.</summary>
        internal ulong InstructionIndex { get; set; } = 0;

        /// <summary>Creates new Pipeline Registers buffer instance.</summary>
        public PipeRegisters() { }

        /// <returns>
        /// <see cref="Instruction"/> stored in buffer or <see langword="null"/> if no instruction stored.
        /// </returns>
        public virtual Instruction Read() => IR32?.GetCopy();

        /// <returns>Address of <see cref="IR32"/> <see cref="Instruction"/> from <see cref="LocalPC"/>.</returns>
        public uint ReadPC() => LocalPC.ReadUnsigned();
        /// <returns>Address of next <see cref="Instruction"/> after related <see cref="IR32"/> base on <see cref="NextPC"/>.</returns>
        public uint ReadPCNext() => NextPC.ReadUnsigned();

        /// <summary>Writes <see cref="Instruction"/> <paramref name="inst"/> to buffer <see cref="PipeRegisters"/>.</summary>
        /// <param name="inst"><see cref="InstructionData"/> to write.</param>
        public void WriteInstruction(Instruction inst) => IR32 = inst?.GetCopy();
        /// <summary>Writes result parameter to buffer's <see cref="ALUOutput"/> <see cref="Register32"/>.</summary>
        public void WriteALUOut(Int32 result) => ALUOutput.Write(result);
        /// <summary>Writes Fetch address of <see cref="IR32"/> <see cref="Instruction"/> to <see cref="LocalPC"/>.</summary>
        /// <param name="src"><see cref="Register32"/> containing address to write</param>
        public void WriteLocalPC(Register32 src) => LocalPC.Write(src.Read());

        /// <summary>Set <see cref="IR32"/> to new <see cref="Instruction.NOP"/>.</summary>
        public void InsertBubble() => IR32 = Instruction.NOP;

        /// <summary>Copies content of <paramref name="source"/> into calling instance of <see cref="PipeRegisters"/>.</summary>
        /// <param name="source">Copy source.</param>
        /// <param name="passNextPC">If <see langword="true"/>, set current <see cref="NextPC"/> from <paramref name="source"/>.</param>
        public void PassFrom(PipeRegisters source, bool passNextPC = true)
        {
            LocalPC.Write(source.LocalPC.Read());
            A.Write(source.A.Read());
            B.Write(source.B.Read());
            Imm.Write(source.Imm.Read());
            ALUOutput.Write(source.ALUOutput.Read());
            Condition = source.Condition;
            InstructionIndex = source.InstructionIndex;

            if (passNextPC) {
                NextPC.Write(source.NextPC.Read());
            }
        }

        public void Reset()
        {
            LocalPC.Reset();
            NextPC.Reset();
            A.Reset();
            B.Reset();
            Imm.Reset();
            ALUOutput.Reset();
            LoadMemoryData.Reset();
            InstructionIndex = 0;
            Condition = null;
            IR32 = null;
        }

        public override string ToString()
        {
            string name = Name is null ? string.Empty : Name+" \n";
            string I32 = "Instruction = " + ((IR32 is null) ? "<null>" : IR32.ToString());
            string con = "Condition = " + (Condition.HasValue ? Condition.ToString() : "<null>");
            return $"[{RelatedPipelineStage}] {name}{I32}\n{LocalPC}\n{A}\n{B}\n{Imm}\n{ALUOutput}\n{LoadMemoryData}\n{con}";
        }
    }
}
