using superscalar_arch_sim.RV32.ISA.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace superscalar_arch_sim.RV32.Hardware.Pipeline.TYP.Buffer
{
    /// <summary>
    /// Final buffer of pipeline, passes instructions from <see cref="Stage.MemoryReach"/> to <see cref="Stage.RegWriteBack"/>.
    /// </summary>
    internal class WritebackBuffer : Pipeline.Buffer
    {
       
    }
}
