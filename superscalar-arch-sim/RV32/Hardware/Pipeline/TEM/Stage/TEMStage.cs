using superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Units;
using superscalar_arch_sim.RV32.Hardware.Register;
using superscalar_arch_sim.RV32.ISA.Instructions;
using superscalar_arch_sim.Simulis;
using superscalar_arch_sim.Simulis.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using static superscalar_arch_sim.RV32.Hardware.HardwareProperties;

namespace superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Stage
{
    public abstract class TEMStage : IClockable
    {
        /// <summary>Number of <see cref="PipeRegisters"/> element in source set <see cref="PipeRegistersSourceSet"/>.</summary>
        const int PREGS_SRC_SIZE = 128;
        readonly protected PipeRegisters[] PipelineRegistersSourceSet;

        /// <summary>Number of <see cref="Instruction"/> objects that can be processed in single <see cref="Cycle"/>. Default value equal <see cref="Settings.MaxIssuesPerClock"/>.</summary>
        protected abstract int MaxInstructionsProcessedPerCycle { get; }

        /// <summary> Name of pipeline stage.</summary>
        public string Name { get; set; } = "Stage";
        /// <summary>Index of stage in the pipeline. Initialized with -1.</summary>
        public int Index { get; set; } = -1;
        /// <summary>Stall signal of current <see cref="TEMStage"/></summary>
        public bool Stalling { get; set; } = false;
        /// <summary>Encodes type of implementing stage in TEM pipeline.</summary>
        public TEMPipelineStage Stage { get; }

        /// <summary>Currently processed <see cref="Instruction"/>.</summary>
        public List<Instruction> ProcessedInstructions { get; protected set; }
        /// <summary>Local generic end-buffers storing <see cref="ProcessedInstructions"/>-related data.</summary>
        public List<PipeRegisters> LatchDataBuffers { get; protected set; }

        /// <summary>/// <see cref="SimReporter"/> instance for updating simulation measures related to <see cref="TEMStage"/>.</summary>
        protected SimReporter Reporter { get; private set; }

        /// <summary>Stage down into pipeline. Can be null is this is a last stage.</summary>
        protected TEMStage StageNext { get; set; }
        /// <summary>Stage before current pipeline stage. Can be null is this is a first stage.</summary>
        protected TEMStage StagePrev { get; set; }

        /// <summary>Placeholder <see cref="Register32"/> storing fetch address of <see cref="ProcessedInstruction"/>.</summary>
        protected Register32 _LocalPC { get; set; } = new Register32("LPC");
        /// <summary>Placeholder <see cref="Register32"/> storing next address after currently processed <see cref="Instruction"/>.</summary>
        protected Register32 _NextPC { get; set; } = new Register32("NPC");


        protected TEMStage(TEMPipelineStage stage, string name = null)
        {
            Name = name??stage.ToString();
            Stage = stage;
            Index = (int)(stage);

            ProcessedInstructions = new List<Instruction>();
            LatchDataBuffers = new List<PipeRegisters>();

            PipelineRegistersSourceSet = new PipeRegisters[PREGS_SRC_SIZE]; 
            for (int i=0; i<PREGS_SRC_SIZE; i++) 
                PipelineRegistersSourceSet[i] = new PipeRegisters();

            Reset();
        }

        public void SetSimReporter(SimReporter reporter)
        {
            Reporter = reporter;
        }
        public virtual void SetPreviousAndNextStages(TEMStage prev, TEMStage next)
        {
            StagePrev = prev;
            StageNext = next;
        }

        public abstract bool IsReady();
        public abstract void Cycle();
        public abstract void Latch();

        /// <summary>
        /// Resets only internal <see cref="TEMStage"/> common state; <see cref="ProcessedInstructions"/> and <see cref="LatchDataBuffers"/>.
        /// </summary>
        public void SoftReset()
        {
            for (int i = 0; i < MaxInstructionsProcessedPerCycle; i++)
            {
                Reporter.ClearInstructionCycleMap(LatchDataBuffers[i].InstructionIndex);
                ProcessedInstructions[i] = Instruction.NOP;
                LatchDataBuffers[i].Reset();
            }
            Stalling = false;
        }

        /// <summary>
        /// Performs reset and resize (if neccessay) of all comonents. Can be overriden to perform hard-reset of implementation specific state.
        /// </summary>
        public virtual void Reset()
        {
            ProcessedInstructions.Clear();
            for (int i = 0; i < MaxInstructionsProcessedPerCycle; i++)
                ProcessedInstructions.Add(Instruction.NOP);

            LatchDataBuffers.Clear();
            LatchDataBuffers.AddRange(PipelineRegistersSourceSet.Take(MaxInstructionsProcessedPerCycle));
            LatchDataBuffers.ForEach(b => { b.Reset(); });

            Stalling = false;
        }

        /// <summary>
        /// Writes <see cref="PipeRegisters"/> at <paramref name="bufferIndex"/> in <see cref="LatchDataBuffers"/>
        /// and assing copy of instruction to <see cref="ProcessedInstructions"/>. Throws if <paramref name="bufferIndex"/>
        /// is not smaller than <see cref="MaxInstructionsProcessedPerCycle"/>.
        /// </summary>
        /// <param name="bufferIndex">Index of source/destination buffer/processed instruction array.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void GetLatchDataFromPreviousStage(int bufferIndex)
        {
            if (bufferIndex >= MaxInstructionsProcessedPerCycle)
                throw new ArgumentOutOfRangeException(nameof(bufferIndex)+" must be less than "+nameof(MaxInstructionsProcessedPerCycle));
            
            int i = bufferIndex;
            LatchDataBuffers[i].PassFrom(StagePrev.LatchDataBuffers[i]);
            LatchDataBuffers[i].IR32 = (StagePrev.LatchDataBuffers[i].IR32);
            ProcessedInstructions[i] = LatchDataBuffers[i].IR32;
            _LocalPC.Write(LatchDataBuffers[i].LocalPC.Read());
            _NextPC.Write(LatchDataBuffers[i].NextPC.Read());
        }


        public static uint GetStagesDistance(TEMStage prev, TEMStage next)
            => (uint)Math.Abs(next.Index - prev.Index);
    }
}
