using superscalar_arch_sim.RV32.Hardware.Pipeline.TYP.Units;
using superscalar_arch_sim.RV32.Hardware.Register;
using superscalar_arch_sim.RV32.ISA.Instructions;
using System;
using System.Linq;
using static superscalar_arch_sim.RV32.Hardware.HardwareProperties;

namespace superscalar_arch_sim.RV32.Hardware.Pipeline.TYP.Stage
{
    public abstract class TYPStage : IClockable
    {

        private bool _stalling = false;
        /// <summary> Name of pipeline stage.</summary>
        public string Name { get; set; } = "Stage";
        /// <summary>Index of stage in the pipeline. Initialized with -1.</summary>
        public int Index { get; set; } = -1;
        /// <summary>Number of cycles neccessary to complete stage. Default is 1.</summary>
        //public uint NeccessaryCycles { get; set; } = 1;
        /// <summary>Consecutive number of cycle within maximum of <see cref="NeccessaryCycles"/></summary>
        public uint CurrentCycle { get; protected set; } = 0;

        /// <summary>Signal indicating that stage data is ready and can be <see cref="Latch"/>ed.</summary>
        //public bool Ready => (CurrentCycle >= NeccessaryCycles);
        /// <summary><see langword="true"/> if this is last cycle of <see cref="TYPStage"/> ( <see cref="CurrentCycle"/> equals <see cref="NeccessaryCycles"/>).</summary>
        //public bool LastCycle => (CurrentCycle == NeccessaryCycles);
        /// <summary>
        /// <see langword="get"/> -> <see langword="return"/> value inticates if pipeline stage is stalling on current cycle and should not be <see cref="Cycle"/>ed. <br></br>
        /// <see langword="set"/>  -> <see langword="set"/> <see cref="Stalling"/> signal for current <see cref="TYPStage"/> and on stall, forces <see cref="Ready"/> to <see langword="true"/> (so data can be <see cref="Latch"/>ed).
        /// </summary>
        public bool Stalling { get => _stalling; set { _stalling = value; } }
        //public bool Stalling { get => _stalling; set { if (_stalling = value) CurrentCycle += NeccessaryCycles; } }

        /// <summary>Instruction from Instruction Register (<b>IR</b>) of this stage - currently processed.</summary>
        public Instruction ProcessedInstruction { get; protected set; }

        /// <summary>Buffer down into pipeline. Can be null is this is a last stage.</summary>
        protected PipeRegisters BufferNext { get; set; }
        /// <summary>Buffer before current pipeline stage. Can be null is this is a first stage.</summary>
        protected PipeRegisters BufferPrev { get; set; }

        /// <summary>
        /// <see cref="Register32"/> storing fetch address of <see cref="ProcessedInstruction"/>.
        /// It's value is passed between stages with <see cref="ProcessedInstruction"/> on <see cref="Latch"/>.
        /// </summary>
        public Register32 LocalPC { get; set; }
        /// <summary>
        /// <see cref="Register32"/> storing address of next instruction after <see cref="ProcessedInstruction"/>.
        /// In case of <see cref="ISA.ISAProperties.InstType.J"/> and <see cref="ISA.ISAProperties.InstType.B"/>,
        /// holds target address after Execute stage.<br></br>
        /// It's value is passed between stages with <see cref="ProcessedInstruction"/> on <see cref="Latch"/>.
        /// </summary>
        public Register32 NextPC { get => BufferPrev?.NextPC; }


        protected TYPStage(TYPPipelineStage stage, string name = null, PipeRegisters prev = null, PipeRegisters next = null)
        {
            if (prev is null)
                throw new ArgumentNullException("Stage param " + nameof(prev) + " cannot be null!");

            Name = name??stage.ToString();
            Index = (int)stage;

            ProcessedInstruction = null;
            LocalPC = new Register32("LPC", name: "Local PC");

            BufferNext = next;
            BufferPrev = prev;
            //NeccessaryCycles = cycles;
        }

        /// <summary>Allows to reassign Prev&Next <see cref="PipeRegisters"/> of pipeline for this <see cref="TYPStage"/>.</summary>
        /// <param name="prev">New <see cref="BufferPrev"/></param>
        /// <param name="next">New <see cref="BufferNext"/></param>
        public void AssignPipeRegisters(PipeRegisters prev, PipeRegisters next)
        {
            BufferPrev = prev;
            BufferNext = next;
        }

        /// <summary>
        /// Performs incrementation of <see cref="CurrentCycle"/> counter. Only if after that operation, 
        /// counter equals <see cref="NeccessaryCycles"/>, method sets <see cref="Ready"/> signal to <see langword="true"></see>
        /// and counter to 0. 
        /// Gets <see cref="ProcessedInstruction"/> from <see cref="BufferPrev"/> and base on its value sets <see cref="IsStalling"/> signal.
        /// </summary>
        public virtual void Cycle()
        {
            ++CurrentCycle;
            ProcessedInstruction = BufferPrev.Read();
            if (false == Stalling)
                LocalPC.Write(BufferPrev.LocalPC.Read());
        }
        /// <summary>
        /// Sets <see cref="CurrentCycle"/> back to 0.<br></br> 
        /// Passes data from <see cref="BufferPrev"/> to <see cref="BufferNext"/>, except for 
        /// <see cref="PipeRegisters.IR32"/> which is overwritten with <see cref="ProcessedInstruction"/>.<br></br><br></br>
        /// Throws <see cref="InvalidPipelineState"/> if <see cref="Stalling"/> signal is <see langword="true"></see> during method invoke.</summary>
        /// <exception cref="InvalidPipelineState"></exception>
        public virtual void Latch()
        {
            // if Latch() is called on stall, next pc should not be overwriten with prev stage values and remain. 
            BufferNext.PassFrom(BufferPrev, passNextPC: (false == Stalling)); // Copy all data from PipeRegiesters Prev -> Next
            BufferNext.WriteInstruction(ProcessedInstruction); // Overwrite I32 in next
            BufferNext.WriteLocalPC(LocalPC); // Overwrite LocalPC in next
            if (false == Stalling) BufferPrev.InsertBubble();
            //if (Stalling) ProcessedInstruction = Instruction.NOP;
            CurrentCycle = 0;
            //Stalling = false;
        }

        /// <summary>Overwrites <see cref="ProcessedInstruction"/> with cpoy of <paramref name="overwrite"/> <see cref="Instruction"/>.</summary>
        public void KillProcessedInstruction(Instruction overwrite)
            => ProcessedInstruction = overwrite;

        /// <summary>Resets all volatile values in <see cref="TYPStage"/> object: <see cref="CurrentCycle"/> and <see cref="ProcessedInstruction"/>.</summary>
        public virtual void Reset()
        {
            CurrentCycle = 0;
            Stalling = false;
            ProcessedInstruction = null;
        }

        public static uint GetStagesDistance(TYPStage prev, TYPStage next)
            => (uint)Math.Abs(next.Index - prev.Index);

        public static uint GetStagesDistance(TYPStage[] pipeline, PipeRegisters bnFirst, PipeRegisters bnSecond)
        {
            var prev = pipeline.Single(x => x.BufferNext.Equals(bnFirst));
            var next = pipeline.Single(x => x.BufferNext.Equals(bnSecond));
            return GetStagesDistance(prev, next);
            //int temp = 0;
            //for (int i = 0; i < pipeline.Count; i++)
            //{
            //    if (pipeline[i].BufferNext == bnFirst) temp = i;
            //    if (pipeline[i].BufferNext == bnSecond) return (i - temp);
            //}
            //return -1;
        }

        public override string ToString()
        {
            return
                $"{Name} [{Index}]\n"
                + ($"Cycle : {CurrentCycle}\n"
                + $"Instruction : {ProcessedInstruction}\n"
                + $"{LocalPC}\n"
                + $"Stalling : {Stalling}");
        }
    }
}


