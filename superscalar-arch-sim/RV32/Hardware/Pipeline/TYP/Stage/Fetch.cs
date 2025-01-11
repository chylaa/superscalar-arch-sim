using superscalar_arch_sim.RV32.Hardware.Register;
using superscalar_arch_sim.RV32.ISA.Instructions;
using superscalar_arch_sim.RV32.Hardware.Units;
using superscalar_arch_sim.RV32.Hardware.Pipeline.Units;
using superscalar_arch_sim.RV32.Hardware.Pipeline.TYP.Units;

namespace superscalar_arch_sim.RV32.Hardware.Pipeline.TYP.Stage
{
    /// <summary>
    /// [ <i>IF</i> ] First stage of TYP pipeline. Instructions are fetched from Memory (Instruction Cache) and CurrentProgram Counter is incremented.
    /// </summary>
    public class Fetch : TYPStage
    {
        /// <summary>
        /// Virtual counter providing new value for <see cref="PipeRegisters.InstructionIndex"/> 
        /// to keep track of instruction fetch order for raport generating purposes.
        /// </summary>
        private ulong VirtualIssueIndex { get; set; } = 0;

        /// <summary>Global CurrentProgram Counter</summary>
        readonly private Register32 GlobalPC;
        /// <summary>Memory managment unit handle.</summary>
        readonly private MemoryManagmentUnit MMU;
        /// <summary>Represents multiplexer connected with global PC and pipeline register containing branch/jump target address.</summary>
        readonly private FetchAddressSelector AddressSelector;
        /// <summary>Branch Prediction unit.</summary>
        readonly private BranchPredictor Predictor;

        public Fetch(MemoryManagmentUnit mmu, Register32 pc, FetchAddressSelector addrMux, BranchPredictor bpredictor, PipeRegisters fetchBuffer, PipeRegisters decodeBuffer) 
                    : base(HardwareProperties.TYPPipelineStage.Fetch, prev: fetchBuffer, next: decodeBuffer) 
        {
            MMU = mmu; 
            GlobalPC = pc;
            AddressSelector = addrMux;
            Predictor = bpredictor;
        }

        /// <summary>
        /// Assigns value from <see cref="Pipeline.Stage.LocalPC"/> (addr of <see cref="Pipeline.Stage.ProcessedInstruction"/>) 
        /// into <see cref="GlobalPC"/>, refetches it from memory and <see cref="Latch"/>. 
        /// </summary>
        public void ReFetchAndLatchProcessedInstruction(PipeRegisters targetAddressSource)
        {
            int pc = AddressSelector.SelectPCValueFromPipeRegister(targetAddressSource, LocalPC, fetch:this, Predictor);
            GlobalPC.Write(pc);
            FetchInstruction();
            Latch();
        }

        public void FetchInstruction()
        {
            int fetchaddr = AddressSelector.SelectPCValue(GlobalPC, fetch:this, Predictor);
            LocalPC.Write(fetchaddr); // save global pc for local address
            ProcessedInstruction = new Instruction(MMU.ReadWord(LocalPC.ReadUnsigned())); // read instruction
            NextPC.Write(LocalPC.Read() + ISA.ISAProperties.WORD_BYTESIZE); // Inc. PC
            BufferPrev.InstructionIndex = (++VirtualIssueIndex); // include new unique instruction index
        }

        /// <summary>
        /// Performs single cycle of fetching instruction. 
        /// Increments CurrentProgram Counter Register <see cref="GlobalPC"/>. 
        /// Calls <see cref="Pipeline.Buffer.Cycle"/> on <see cref="Pipeline.Stage.BufferNext"/>.
        /// </summary>
        public override void Cycle()
        {
            base.Cycle();
            FetchInstruction();
        }

        /// <summary>
        /// Writes LocalPC of next <see cref="PipeRegisters"/>. Increments PC if stage is not stalling. 
        /// <br></br><inheritdoc/>
        /// </summary>
        public override void Latch()
        {
            base.Latch();
            BufferPrev.IR32 = null;
            GlobalPC.Write(NextPC.Read()); // Write new PC address
        }

        public override void Reset()
        {
            base.Reset();
            VirtualIssueIndex = 0;
        }
    }
}
