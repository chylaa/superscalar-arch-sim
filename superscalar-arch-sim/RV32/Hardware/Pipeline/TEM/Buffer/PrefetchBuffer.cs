using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using superscalar_arch_sim.RV32.ISA.Instructions;

namespace superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Buffer
{
    /// <summary>
    /// Somewhat "virtual" buffer - on <see cref="Pipeline.Buffer.Get"/> will return <see langword="null"/> to <see cref="Stage.Fetch"/> stage.
    /// Stage should fetch value from memory then, but if <see cref="Pipeline.Buffer.InsertBubble"/> was called,
    /// <see cref="Instruction.NOP"/> is returned that will flow through the entire pipeline.
    /// </summary>
    internal class PrefetchBuffer : Pipeline.Buffer
    {

    }
}
