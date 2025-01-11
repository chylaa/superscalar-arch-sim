using superscalar_arch_sim.RV32.ISA.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace superscalar_arch_sim.RV32.Hardware.Pipeline.TYP.Buffer
{
    /// <summary>
    /// Also "Instruction Buffer". Passes fetched instructions from <see cref="Stage.Fetch"/> to <see cref="Stage.Decode"/>.
    /// </summary>
    internal class DecodeBuffer : Pipeline.Buffer
    { 

    }
}
