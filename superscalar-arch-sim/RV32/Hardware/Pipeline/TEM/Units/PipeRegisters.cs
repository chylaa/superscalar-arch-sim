using superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Stage;
using superscalar_arch_sim.RV32.Hardware.Register;
using superscalar_arch_sim.RV32.ISA.Instructions;
using System;


namespace superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Units
{
    /// <summary>
    /// Pipeline Buffer Registers for passing data between consecutive stages. 
    /// Imitates hardware register in pipelined architecture. 
    /// </summary>
    public class PipeRegisters : IPipelineBuffer
    {
        public static readonly Type RelatedPipelineStage = typeof(TEMStage);

        /// <summary>Optional name of <see cref="PipeRegisters"/> instance for debugging purposes.</summary>
        internal string Name { get; set; } = null;

        [System.ComponentModel.DisplayName("RS-Tag")]
        /// <summary>Contains <see cref="ReservationStation.Tag"/> of <see cref="ReservationStation"/> from which <see cref="IR32"/> was issued.</summary>
        public int ReservationStationSourceTag { get; set; } = 0;
        /// <summary>(Instruction Register) Processed <see cref="IR32"/> object.</summary>
        public Instruction IR32 { get; set; } = null;

        [System.ComponentModel.DisplayName("LPC")]
        /// <summary>Stores Address of <see cref="IR32"/>.</summary>
        public Register32 LocalPC { get; private set; } = new Register32("LPC", name: "Local Program Counter");

        [System.ComponentModel.DisplayName("NPC")]
        /// <summary>Stores next address after <see cref="IR32"/>.</summary>
        public Register32 NextPC { get; private set; } = new Register32("NPC", name: "Next Program Counter");

        [System.ComponentModel.DisplayName("OUT")]
        /// <summary>Stores result from ALU operation.</summary>
        public Register32 ALUOutput { get; private set; } = new Register32("OUT", name: "ALU Output");

        [System.ComponentModel.DisplayName("LMD")]
        /// <summary>(Load Memory Data) Contains data from memory, requested on Decode stage.</summary>
        public Register32 LoadMemoryData { get; private set; } = new Register32("LMD", name: "Load Memory Data");

        [System.ComponentModel.DisplayName("Branch")]
        /// <summary>Result of condition from branch instruction (<see cref="ISA.ISAProperties.InstType.B"/>).</summary>
        public bool? Condition { get; set; } = null;

        /// <summary>Allows to keep track of <see cref="IR32"/> order of creation.</summary>
        internal ulong InstructionIndex { get; set; } = 0;

        /// <summary>Creates new Pipeline Registers buffer instance.</summary>
        public PipeRegisters() { }

        /// <returns><see cref="Instruction"/> stored in buffer or <see langword="null"/> if no instruction stored.</returns>
        public Instruction Read() => IR32?.GetCopy();

        /// <returns>Address of <see cref="IR32"/> <see cref="Instruction"/> from <see cref="LocalPC"/>.</returns>
        public uint ReadPC() => LocalPC.ReadUnsigned();
        /// <returns>Address of next <see cref="Instruction"/> after related <see cref="IR32"/> base on <see cref="NextPC"/>.</returns>
        public uint ReadPCNext() => NextPC.ReadUnsigned();

        /// <summary>Writes copy of <see cref="Instruction"/> <paramref name="inst"/> to buffer <see cref="PipeRegisters"/>.</summary>
        /// <param name="inst"><see cref="InstructionData"/> to write.</param>
        public void WriteInstruction(Instruction inst) => IR32 = inst?.GetCopy();
        /// <summary>Writes result parameter to buffer's <see cref="ALUOutput"/> <see cref="Register32"/>.</summary>
        public void WriteALUOut(Int32 result) => ALUOutput.Write(result);
        /// <summary>Writes result parameter to buffer's <see cref="LoadMemoryData"/> <see cref="Register32"/>.</summary>
        public void WriteMemoryData(Int32 result) => LoadMemoryData.Write(result);
        /// <summary>Writes Fetch address of <see cref="IR32"/> <see cref="Instruction"/> to <see cref="LocalPC"/>.</summary>
        /// <param name="src"><see cref="Register32"/> containing address to write</param>
        public void WriteLocalPC(Register32 src) => LocalPC.Write(src.Read());
        /// <summary>Writes <paramref name="src"/> value to <see cref="NextPC"/>.</summary>
        /// <param name="src"><see cref="Register32"/> containing address to write</param>
        public void WriteNextPC(Register32 src) => NextPC.Write(src.Read());
        /// <summary>Writes <paramref name="addr"/> value to <see cref="NextPC"/>.</summary>
        /// <param name="src">Address to write</param>
        public void WriteNextPC(Int32 addr) => NextPC.Write(addr);

        /// <summary>Set <see cref="IR32"/> to new <see cref="Instruction.NOP"/>.</summary>
        public void InsertBubble() => IR32 = Instruction.NOP;

        /// <summary>Copies content of <paramref name="source"/> into calling instance of <see cref="PipeRegisters"/>.</summary>
        /// <param name="source">Copy source.</param>
        public void PassFrom(PipeRegisters source)
        {
            LocalPC.Write(source.LocalPC.Read());
            NextPC.Write(source.NextPC.Read());
            ALUOutput.Write(source.ALUOutput.Read());
            LoadMemoryData.Write(source.LoadMemoryData.Read());
            Condition = source.Condition;
            ReservationStationSourceTag = source.ReservationStationSourceTag;
            InstructionIndex = source.InstructionIndex;
        }

        public void Reset()
        {
            LocalPC.Reset();
            NextPC.Reset();
            ALUOutput.Reset();
            LoadMemoryData.Reset();
            ReservationStationSourceTag = 0;
            InstructionIndex = 0;
            Condition = null;
            IR32 = null;
        }

        public override string ToString()
        {
            string name = Name is null ? string.Empty : Name + " \n";
            string I32 = "Instruction = " + ((IR32 is null) ? "<null>" : IR32.ToString());
            string con = "Condition = " + (Condition.HasValue ? Condition.ToString() : "<null>");
            string rstag = "Source RS Tag = " + ReservationStationSourceTag.ToString();
            return $"[{RelatedPipelineStage}] {name}{I32}\n{LocalPC}\n{ALUOutput}\n{LoadMemoryData}\n{con}\n{rstag}";
        }
    }
}
