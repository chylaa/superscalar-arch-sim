using superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Units;
using superscalar_arch_sim.RV32.Hardware.Register;
using superscalar_arch_sim.RV32.Hardware.Units;
using superscalar_arch_sim.RV32.ISA.Instructions;
using superscalar_arch_sim.Simulis;
using superscalar_arch_sim.Simulis.Reports;
using System.Collections.Generic;

namespace superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Stage
{
    /// <summary>
    /// First stage of TEM pipeline. Instructions are fetched from Memory (Instruction Cache) in-order.
    /// </summary>
    public class Fetch : TEMStage
    {
        protected override int MaxInstructionsProcessedPerCycle { get => Settings.FetchWidth; }

        /// <summary>
        /// Virtual counter providing new value for <see cref="PipeRegisters.InstructionIndex"/> 
        /// to keep track of instruction fetch order for raport generating purposes.
        /// </summary>
        private ulong VirtualIssueIndex { get; set; } = 0;

        readonly private Register32 GlobalPC;
        readonly private MemoryManagmentUnit MMU;
        readonly private BranchPredictor BranchPredictor;
        readonly private FetchAddressSelector FetchMux;

        readonly private List<int> LocalPCValues = new List<int>();
        readonly private List<int> NextPCValues = new List<int>();

        public Fetch(MemoryManagmentUnit mmu, Register32 gpc, BranchPredictor predictor) 
            : base(HardwareProperties.TEMPipelineStage.Fetch) 
        { 
            MMU = mmu; 
            GlobalPC = gpc;
            BranchPredictor = predictor;
            FetchMux = new FetchAddressSelector(BranchPredictor);
        }
        public void SetProgramCounter(int value)
        {
            GlobalPC.Write(value);
            _LocalPC.Write(value);
            _NextPC.Write(value);
        }
        public void SetProgramCounter(uint value)
        {
            GlobalPC.WriteUnsigned(value);
            _LocalPC.WriteUnsigned(value);
            _NextPC.WriteUnsigned(value);
        }

        public override bool IsReady()
        {
            return true;
        }

        /// <summary>
        /// Performs single cycle of fetching <see cref="TEMStage.IssueWidth"/> instructions. 
        /// Increments CurrentProgram Counter Register <see cref="GlobalPC"/>. 
        /// </summary>
        public override void Cycle()
        {
            bool @break = false;
            int bundleSize = 0;
            for (int i = 0; i < MaxInstructionsProcessedPerCycle; i++)
            {
                LatchDataBuffers[i].Reset();
                if (@break) {
                    ProcessedInstructions[i] = null;
                    continue;
                }
                _LocalPC.Write(_NextPC.Read());
                Instruction i32 = new Instruction(MMU.ReadWord(_LocalPC.ReadUnsigned()));

                int localPc = _LocalPC.Read();
                int? pcnext = BranchPredictor.GetPredictedTargetAddress(_LocalPC);

                _NextPC.Write(FetchMux.GetNextFetchAddress(_LocalPC, i32, out _));
                ProcessedInstructions[i] = i32;

                LocalPCValues.Add(_LocalPC.Read());
                NextPCValues.Add(_NextPC.Read());

                // control transfer, additonal cycle for refetch neccessary
                @break = (pcnext.HasValue && pcnext.Value != (localPc + ISA.ISAProperties.WORD_BYTESIZE)); 
                ++bundleSize;
            }
            if (Reporter.SimMeasuresEnabled)
            {
                Reporter.I32Measures_FetchBundleSizeHist.Collect(bundleSize);
                Reporter.I32Measure_FetchThroughput.Update(bundleSize);
            }
        }

        /// <summary>
        /// <br></br><inheritdoc/>
        /// </summary>
        public override void Latch()
        {
            GlobalPC.Write(_NextPC.Read());
            for (int i = 0; i < MaxInstructionsProcessedPerCycle; i++)
            {
                if (ProcessedInstructions[i] != null)
                {
                    LatchDataBuffers[i].IR32 = ProcessedInstructions[i];
                    LatchDataBuffers[i].InstructionIndex = (++VirtualIssueIndex);
                    LatchDataBuffers[i].LocalPC.Write(LocalPCValues[i]);
                    LatchDataBuffers[i].NextPC.Write(NextPCValues[i]);
                    Reporter.UpdateFetchedInstructionCounters(LatchDataBuffers[i].IR32, LatchDataBuffers[i].InstructionIndex);
                }
            }
            LocalPCValues.Clear();
            NextPCValues.Clear();
        }
        public void ResetInternalProgramCounters()
        {
            if (GlobalPC != null)
            {
                _LocalPC.Write(GlobalPC.Read());
                _NextPC.Write(GlobalPC.Read());
            }
            LocalPCValues.Clear();
            NextPCValues.Clear();
        }
        public override void Reset() 
        {
            base.Reset();
            VirtualIssueIndex = 0;
            ResetInternalProgramCounters();
        }
    }
}
