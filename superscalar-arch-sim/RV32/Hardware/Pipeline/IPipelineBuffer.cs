using superscalar_arch_sim.RV32.Hardware.Register;
using superscalar_arch_sim.RV32.ISA.Instructions;

namespace superscalar_arch_sim.RV32.Hardware.Pipeline
{
    public interface IPipelineBuffer
    {
        Instruction IR32 { get; set; }
        Register32 ALUOutput { get; }
        Register32 LoadMemoryData { get; }
        Register32 LocalPC { get; }
        Register32 NextPC { get; }
        bool? Condition { get; set; }

        Instruction Read();
        void InsertBubble();
        void Reset();
    }
}