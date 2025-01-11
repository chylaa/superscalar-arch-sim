using superscalar_arch_sim.RV32.ISA.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Buffer
{
    /// <summary>
    /// Passes instructions that finished decoding from <see cref="Stage.Decode"/> to <see cref="Stage.Dispatch"/>.
    /// </summary>
    internal class DispatchBuffer : Pipeline.Buffer
    {
       
    }
}
