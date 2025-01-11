using superscalar_arch_sim.RV32.ISA.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Buffer
{
    /// <summary>
    /// Also "Completion Buffer". Gets executed out-of-order instruction and passes them to <see cref="Stage.Complete"/> in-order.
    /// </summary>
    internal class ReorderBuffer : Pipeline.Buffer
    {
      
    }
}
