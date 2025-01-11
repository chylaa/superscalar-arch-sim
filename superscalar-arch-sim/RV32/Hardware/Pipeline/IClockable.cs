using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace superscalar_arch_sim.RV32.Hardware.Pipeline
{
    /// <summary>
    /// Provides an interface for synchronus stage-like units that are driven by clock signal 
    /// and where data can be "latched" if stage is done processing.
    /// </summary>
    internal interface IClockable
    {
        bool Stalling { get; set; }
        void Cycle();
        void Latch();
        void Reset();
    }
}
