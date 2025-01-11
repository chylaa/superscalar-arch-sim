using superscalar_arch_sim.RV32.ISA.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace superscalar_arch_sim.RV32.Hardware.Pipeline
{
    internal class Buffer : IClockable
    {
        private const int BufferSteps = 2;

        protected uint CycleCounter;
        
        protected readonly Instruction[] _buffer;

        public uint NeccessaryCycles { get; set; } = 1;
        public bool Ready { get; private set; } = false;

        /// <summary>Creates new general <see cref="Buffer"/> instance.</summary>
        /// <param name="needcycles">Number of cycless neccessary for buffer to complete operation (see <see cref="IClockable.Ready"/> signal).</param>
        /// <param name="init"><see cref="Instruction"/> instance to act as placeholder for initial buffer content (default <see langword="null"/>).</param>
        public Buffer(uint needcycles = 1, Instruction init = null)
        {
            NeccessaryCycles = needcycles;
            _buffer = new Instruction[BufferSteps];
            Reset(init);
        }

        /// <summary>Sets <see cref="IClockable.Ready"/> signal if method was called <see cref="IClockable.NeccessaryCycles"/> times.</summary>
        public virtual void Cycle() => Ready = (++CycleCounter == NeccessaryCycles);

        /// <summary>Copies <see cref="Instruction"/> from input to output, using <see cref="Instruction.GetCopy"/> method. Input is cleared (set to <see langword="null"/>).</summary>
        public virtual void Latch() { _buffer[1] = _buffer[0]?.GetCopy(); _buffer[0] = null; }

        /// <summary>Allows to get <see cref="Instruction"/> from <see cref="Buffer"/> output.</summary>
        /// <returns><see cref="Instruction"/> at output position of internal buffer array.</returns>
        public Instruction Get() => _buffer[1];

        /// <summary>Allows to put <see cref="Instruction"/> into <see cref="Buffer"/> input.</summary>
        /// <param name="instruction"><see cref="Instruction"/> to put in input position of internal buffer array.</param>
        public void Put(Instruction instruction) => _buffer[0] = instruction;

        /// <summary>Resets internal buffer.</summary>
        /// <param name="init"><see cref="Instruction"/> instance to act as placeholder for initial buffer content (default <see langword="null"/>).</param>
        public virtual void Reset(Instruction init = null)
        {
            _buffer[0] = init; _buffer[1] = init;
            CycleCounter = 0;
        }

        /// <summary>
        /// Causes next <see cref="Stage"/> after this <see cref="Buffer"/> to stall in the next <see cref="Stage.Cycle"/>, 
        /// by putting <see cref="Instruction.NOP"/> into internal buffer.
        /// <br></br>
        /// As the result, when next <see cref="Stage"/> will <see cref="Get"/> <see cref="Instruction.NOP"/> into it's Instruction Register,
        /// <see cref="Stage.IsStalling"/> should be set (in <see cref="Stage.Cycle"/>). Then all the following stages of the pipeline will also stall.
        /// <br></br> 
        /// <u><b>Note:</b> this method should be called after <b>unneccessary</b> <see cref="Put(Instruction)"/> and before <see cref="Latch"/> 
        /// to avoid destroing instruction for next <see cref="Stage"/> and overwriting the no-operation instruction by the previous.</u>
        /// </summary>
        public void InsertBubble() { Put(Instruction.NOP); }

        /// <summary>
        /// Overwrites old content of <see cref="Buffer"/> output with <paramref name="new"/> <see cref="Instruction"/>.
        /// </summary>
        /// <param name="new">New instruction that will be returned by <see cref="Get"/> if <see cref="Latch"/> was not invoked in the meantime.</param>
        public void KillOutput(Instruction @new) { _buffer[1] = @new; }
    }
}
