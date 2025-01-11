using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace superscalar_arch_sim.RV32.Hardware.Pipeline.TYP.Buffer
{
    /// <summary>
    /// Somewhat "virtual" buffer - creates <see cref="Instruction"/> instance for <see cref="Stage.Fetch"/> stage.
    /// Allows to <see cref="Pipeline.Buffer.InsertBubble"/>, that will flow through the entire pipeline.
    /// </summary>
    internal class FetchBuffer : Pipeline.Buffer
    {

    }
}
