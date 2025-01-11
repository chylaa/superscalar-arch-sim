using superscalar_arch_sim.RV32.ISA.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace superscalar_arch_sim.RV32.Hardware.Pipeline.TYP.Buffer
{
    /// <summary>
    /// Passes instructions that finished decoding from <see cref="Stage.Decode"/> to <see cref="Stage.ALUExecute"/>.
    /// </summary>
    internal class ExecuteBuffer : Pipeline.Buffer
    {
       
    }
}
