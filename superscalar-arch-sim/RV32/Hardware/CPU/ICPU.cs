using superscalar_arch_sim.RV32.Hardware.Register;
using superscalar_arch_sim.RV32.Hardware.Units;
using superscalar_arch_sim.Simulis.Reports;

namespace superscalar_arch_sim.RV32.Hardware.CPU
{
    public interface ICPU
    {
        bool FetchStalling {  get; }
        string GlobalPCString { get; set; }
        ulong ClockCycles { get; set; }
        int NumberOfPipelineStages { get; }
        int PipelineWidth { get; }
        string[] StageNames { get; }

        uint ROMSize { get; set; }
        uint RAMSize { get; set; }
        uint ROMStart { get; set; }
        uint RAMStart { get; set; }
        Memory.Memory RAM { get; }
        Memory.Memory ROM { get; }
        MemoryManagmentUnit MMU { get; }
        
        Register32 GlobalProgramCounter { get; }
        Register32File RegisterFile { get; }
        Register32File FPRegisterFile { get; }

        SimReporter SimReport { get; }
        ReportGenerator ReportGenerator { get; }

        int FlashRAM(byte[] data, uint _startAddr = 0);
        int FlashRAM(uint[] data, uint _startAddr = 0);
        int FlashROM(byte[] data, uint _startAddr = 0);
        int FlashROM(uint[] data, uint _startAddr = 0);

        void Cycle();
        void CycleLatchSingle(int StageBufferNo);
        void Latch();
        void Reset(bool preserveMemory);
    }
}