using System;

namespace superscalar_arch_sim.RV32.Hardware
{
    public static class HardwareProperties
    {
        [Flags]
        public enum MemoryAccess { Read = 1, Write = 2, RW = 3, Execute = 4 }

        public enum TEMPipelineStage { Fetch = 0, Decode = 1, Dispatch = 2, Execute = 3, Complete = 4, Retire = 5, None = 6 }
        public enum TYPPipelineStage { Fetch = 0, Decode = 1, Execute = 2, Memory = 3, Writeback = 4, Invalid = 5 }

    }
}
