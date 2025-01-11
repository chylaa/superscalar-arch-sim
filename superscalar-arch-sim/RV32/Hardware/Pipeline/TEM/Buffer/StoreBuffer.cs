using superscalar_arch_sim.RV32.ISA.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Buffer
{
    /// <summary>
    /// Final buffer of pipeline, redirecting architecturaly completed instructions to <see cref="Stage.Retire"/>.
    /// </summary>
    internal class StoreBuffer : Pipeline.Buffer
    {
       
    }
}
