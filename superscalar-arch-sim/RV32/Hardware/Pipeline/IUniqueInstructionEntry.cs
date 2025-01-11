using superscalar_arch_sim.RV32.Hardware.Register;
using superscalar_arch_sim.RV32.ISA.Instructions;

namespace superscalar_arch_sim.RV32.Hardware.Pipeline
{
    public interface IUniqueInstructionEntry
    {
        int Tag { get; }
        Instruction IR32 { get; }
        Register32 FetchLocalPC { get; }
        bool Busy { get; }
        bool MarkedEmpty { get; }

        ulong InstructionIndex { get; }

        void Reset();
    }
}
