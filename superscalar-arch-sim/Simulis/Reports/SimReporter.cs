using superscalar_arch_sim.RV32.Hardware.CPU;
using superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Stage;
using superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Units;
using superscalar_arch_sim.RV32.Hardware.Register;
using superscalar_arch_sim.RV32.Hardware.Units;
using superscalar_arch_sim.RV32.ISA;
using superscalar_arch_sim.RV32.ISA.Instructions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace superscalar_arch_sim.Simulis.Reports
{
    public class SimReporter
    {
        public const char D_SUPERSCALAR_PLATFORM = 'D';
        public const char S_SCALAR_PLATFORM = 'S';

        public enum SimMeasures {
            /// <summary>Represents <see cref="ReorderBuffer"/> measures.</summary>
            ROB = 1,
            /// <summary>Represents <see cref="InstructionDataQueue"/> measures.</summary>
            IRQueue = 2,
            /// <summary>Can represent <see cref="BranchUnit"/> or related <see cref="ReservationStationCollection"/> measures.</summary>
            BranchUnit = 3,
            /// <summary>Can represent <see cref="MemUnit"/> or related <see cref="ReservationStationCollection"/> measures.</summary>
            MemUnit = 4,
            /// <summary>Can represent <see cref="IntUnit"/> or related <see cref="ReservationStationCollection"/> measures.</summary>
            IntUnit = 5,
        }

        #region Private mem variables
        private bool _simMeasuresEnabled = true;
        private ulong _LastMispredictedBranchCycle = ulong.MaxValue;
        private readonly char PlatformPrefix = '\0';
        private readonly Dictionary<ulong, ulong> InstructionCycleMap = new Dictionary<ulong, ulong>();
        #endregion

        #region Settings
        public bool Enabled { get; set; } = true;
        public bool SimMeasuresEnabled { get => Enabled && _simMeasuresEnabled; set => _simMeasuresEnabled = value; }
        #endregion

        #region Public Counters Properties
        public ulong ClockCycles { get; set; } = 0;
        public double CPI { get; set; } = 0;
        public double IPC { get; set; } = 0;
        /// <summary>
        /// Sum of all forwarding events, <see cref="FeedbackALUInput"/>, <see cref="FeedbackMEMInput"/> and <see cref="ForwardMEMLoadToALU"/>
        /// in case of <see cref="ScalarCPU"/> and <see cref="FeedbackViaCommonDataBus"/> for <see cref="SuperscalarCPU"/>.
        /// </summary>
        public ulong Forwardings => (FeedbackALUInput + FeedbackMEMInput + ForwardMEMLoadToALU + FeedbackViaCommonDataBus);
        /// <summary><see cref="Aggregate"/> of cycles that takes instruction to move through entire pipeline.</summary>
        public Aggregate I32Measure_ProcessingCycles { get; } = Aggregate.Default;

        #region Static core

        #region Data forwarding
        /// <summary>Number of times that Load Memory Data result was feed back as Memory Input.</summary>
        public ulong FeedbackMEMInput { get; set; } = 0;
        /// <summary>Number of times that ALU Output was feed back to ALU Input.</summary>
        public ulong FeedbackALUInput { get; set; } = 0;
        /// <summary>Number of times that Load Memory Data result was send back to EX as ALU Input.</summary>
        public ulong ForwardMEMLoadToALU { get; set; } = 0;
        /// <summary>Number of times that RAW hazard result in pipeline stall due to Load instruction interlock.</summary>
        public ulong LoadInterlocks { get; set; } = 0;
        /// <summary>Number of times that data dependency between instructions was detected.</summary>
        public ulong DataDependencies { get; set; } = 0;
        #endregion

        #endregion

        #region Dynamic core
        public ulong PipelineFlushes { get; set; } = 0;
        public ulong StoreLoadBypasses { get; set; } = 0;
        public ulong FeedbackViaCommonDataBus { get; set; } = 0;

        /// <summary><see cref="Aggregate"/> number of instructions in bundle, fetched per cycle by <see cref="Fetch"/> stage.</summary>
        public Aggregate I32Measure_FetchThroughput { get; } = Aggregate.Default;
        /// <summary><see cref="Aggregate"/> number of instructions in bundle, dispatched per cycle by <see cref="Dispatch"/> stage.</summary>
        public Aggregate I32Measure_DispatchThroughput { get; } = Aggregate.Default;
        /// <summary><see cref="Aggregate"/> values of instructions per cycle passed through <see cref="Execute"/> stage.</summary>
        public Aggregate I32Measure_ExecuteThrougput { get; } = Aggregate.Default;
        
        /// <summary><see cref="Aggregate"/> values of instructions per cycle passed through <see cref="Retire"/> stage.</summary>
        public Aggregate I32Measure_RetireThrougput { get; } = Aggregate.Default;
        
        /// <summary><see cref="Aggregate"/> sizes per cycle, divided to related <see cref="SimMeasures"/> collections.</summary>
        public Dictionary<SimMeasures, Aggregate> I32Measures_Size { get; }

        /// <summary>Number of times that related <see cref="SimMeasures"/> collection was full.</summary>
        public FrequencyDistribution<SimMeasures, LongCounter> I32Measures_CollectionFull { get; }

        /// <summary>Histogram data of number of items in related to <see cref="SimMeasures"/> processor units.</summary>
        public Dictionary<SimMeasures, FrequencyDistribution<int, LongCounter>> I32Measures_SizeHist { get; }
        /// <summary>Number of instruction in Dispatch bundle in cycle.</summary>
        public FrequencyDistribution<int, LongCounter> I32Measures_DispatchBundleSizeHist { get; }
        /// <summary>Number of instruction in Fetch bundle in cycle.</summary>
        public FrequencyDistribution<int, LongCounter> I32Measures_FetchBundleSizeHist { get; }
        /// <summary>Number of instruction in <see cref="ReservationStationCollection"/> in cycle.</summary>
        public FrequencyDistribution<int, LongCounter> I32Measures_RSSizeHist { get; }
        public FrequencyDistribution<int, LongCounter> I32Measures_ExecuteSizeHist { get; }
        #endregion

        #region Instruction counters
        public ulong CommitedInstructions { get; set; } = 0;
        /// <summary>Maps each instruction type to amout of commited instructions of that type.</summary>
        private ConcurrentDictionary<ISAProperties.InstType, ulong> CommitedInstructionTypes { get; set; } = null;
        private ConcurrentDictionary<RV32IOpcode, ulong> CommitedInstructionOpcodes { get; set; } = null;
        private ConcurrentBag<PredictionDetails> ExecutedBranchInstructions { get; set; } = null;
        /// <summary>
        /// Returns <see cref="CommitedInstructionTypes"/> as <see cref="IReadOnlyDictionary{ISAProperties.InstType, Int64}"/> snapshot.
        /// </summary>
        public IReadOnlyDictionary<ISAProperties.InstType, ulong> CommitedInstructionTypesThreadSafe
            => CommitedInstructionTypes;
        public IReadOnlyDictionary<RV32IOpcode, ulong> CommitedInstructionOpcodesThreadSafe
            => CommitedInstructionOpcodes;
        public IReadOnlyCollection<PredictionDetails> ExecutedBranchInstructionsThreadSafe
            => ExecutedBranchInstructions;

        #endregion
        #region Branches/Jumps
        /// <summary>Number of control transfer instructions which address was not known and was not predicted properly.</summary>
        public ulong AddressMisprediction { get; set; } = 0;

        /// <summary>Number of branch instructions which condition was evaluated to <see langword="true"/>.</summary>
        public ulong BranchesTaken { get; set; } = 0;
        /// <summary>Number of branch instructions which condition was evaluated to <see langword="false"/> - base on overall number of branches and <see cref="BranchesTaken"/>.</summary>
        public ulong BranchesNotTaken => (CommitedInstructionTypes[ISAProperties.InstType.B] - BranchesTaken);
        /// <summary>Number of mispredicted branch instructions.</summary>
        public ulong BranchMispredictions { get; set; } = 0; 
        public ulong CorrectBranchPredictions => (CommitedInstructionTypes[ISAProperties.InstType.B] - BranchMispredictions);

        //// Skewed by counting mispredictions at the ‘Complete’ stage vs. the number of jumps after ‘Retire’?
        /// <summary>Successfully predicted branches over all <see cref="ISAProperties.InstType.B"/> type instructions.</summary>
        public double BranchPredictionAccuracy => (((double)CorrectBranchPredictions) / (CommitedInstructionTypes[ISAProperties.InstType.B]));
        #endregion
        #region Stalls
        /// <summary>
        /// Number of stalls introduced to pipeline, equal to number of times that IF stage was stalled
        /// (if IF or any later stages are stalled, IF also will be stalled).
        /// </summary>
        public ulong Stalls => StagesStalls[0]; // If pipeline was stalled IF 

        /// <summary>Number of stalls in each stage, where <see cref="StagesStalls"/> <see cref="Array.Length"/> equals depth of pipeline.</summary>
        public ulong[] StagesStalls { get; set; }
        #endregion
        #endregion

        public SimReporter(int pipelineDepth, char platformPrefix)
        {
            PlatformPrefix = platformPrefix;

            StagesStalls = new ulong[pipelineDepth];
            CommitedInstructionTypes = new ConcurrentDictionary<ISAProperties.InstType, ulong>();
            CommitedInstructionOpcodes = new ConcurrentDictionary<RV32IOpcode, ulong>();

            I32Measures_Size = new Dictionary<SimMeasures, Aggregate>();
            I32Measures_CollectionFull = new FrequencyDistribution<SimMeasures, LongCounter>();
            I32Measures_SizeHist = new Dictionary<SimMeasures, FrequencyDistribution<int, LongCounter>>();
            I32Measures_RSSizeHist = new FrequencyDistribution<int, LongCounter>(Utilis.Utilis.GetUniqueInts(0, 65), acceptNew: true);
            I32Measures_ExecuteSizeHist = new FrequencyDistribution<int, LongCounter>(Utilis.Utilis.GetUniqueInts(0, 9), acceptNew: true);
            I32Measures_DispatchBundleSizeHist = new FrequencyDistribution<int, LongCounter>(Utilis.Utilis.GetUniqueInts(0, 9), acceptNew: true);
            I32Measures_FetchBundleSizeHist = new FrequencyDistribution<int, LongCounter>(Utilis.Utilis.GetUniqueInts(0, 9), acceptNew: true);
            Reset();
        }

        public void UpdateBranchMispredictions()
        {
            if (_LastMispredictedBranchCycle != ClockCycles)
            {
                ++BranchMispredictions;
                _LastMispredictedBranchCycle = ClockCycles;
            }
        }

        public void UpdateStalls(int zeroBasedStageIdx, bool isStalling)
        {
            if (Enabled && isStalling) StagesStalls[zeroBasedStageIdx]++;
        }

        private void UpdatePlatformSpecificCounters(Instruction retired, Register32 localPC, params ulong[] datas)
        {
            if (PlatformPrefix == D_SUPERSCALAR_PLATFORM)
            {

            } else if (PlatformPrefix == S_SCALAR_PLATFORM)
            {

            }
        }
        public void ClearInstructionCycleMap(ulong instructionId)
        {
            if (SimMeasuresEnabled)
            {
                InstructionCycleMap.Remove(instructionId);
                if (InstructionCycleMap.Count > 50000) // remove old ones that were missed
                {
                    while (InstructionCycleMap.Count > 500)
                        InstructionCycleMap.Remove(InstructionCycleMap.ElementAt(0).Key);
                }
            }
        }
        public void UpdateFetchedInstructionCounters(Instruction fetch, ulong instructionId)
        {
            if (SimMeasuresEnabled)
            {
                if (instructionId != 0 && instructionId != ulong.MaxValue)
                {
                    //System.Diagnostics.Debug.WriteLine(fetch.ToString());
                    InstructionCycleMap.Add(instructionId, ClockCycles);
                }
            }
        }
        public void UpdateRetiredInstructionCounters(Instruction retired, Register32 localPC, ulong instructionId, params ulong[] datas)
        {
            if (false == Enabled)
                return;

            if (false == retired.BubbleInstruction)
            {
                ++CommitedInstructionTypes[retired.Type];
                ++CommitedInstructionOpcodes[(RV32IOpcode)(retired.opcode & 0xFF)];
                ++CommitedInstructions;
            }

            CPI = (double)ClockCycles / (double)(CommitedInstructions);
            IPC = (double)CommitedInstructions / ClockCycles;

            if (SimMeasuresEnabled)
            {
                if (InstructionCycleMap.TryGetValue(instructionId, out ulong issueCycle))
                {
                    ulong processCycles = (ClockCycles - issueCycle);
                    I32Measure_ProcessingCycles.Update(processCycles);
                    ClearInstructionCycleMap(instructionId);
                }
            }
            UpdatePlatformSpecificCounters(retired, localPC, datas);
        }

        private void ResetNonNumerical()
        {
            Aggregate GetDefault() => Aggregate.Default;
            FrequencyDistribution<int, LongCounter> GetDefaultDistibution()
                => new FrequencyDistribution<int, LongCounter>(Utilis.Utilis.GetUniqueInts(0, 65), acceptNew:true);

            Array.Clear(StagesStalls, 0, StagesStalls.Length);
            InstructionCycleMap.Clear();
            SetInsertDictionary(I32Measures_Size, @default: GetDefault);
            SetInsertDictionary(I32Measures_SizeHist, @default: GetDefaultDistibution);
            SetInsertDictionary(CommitedInstructionTypes, ISAProperties.InstType.Invalid, ISAProperties.InstType.R4);
            SetInsertDictionary(CommitedInstructionOpcodes, RV32IOpcode.SYSTEM);
            Enabled = true;
        }
        public void Reset()
        {
            bool enabled = Enabled;
            bool simMeasuresEnabled = SimMeasuresEnabled;
            ResetNonNumerical();
            foreach (var p in GetType().GetProperties())
            {
                if (p.CanWrite && (p.PropertyType.IsValueType || p.PropertyType == typeof(string)))
                {
                    p.SetValue(this, default);
                }
                else // try invoke .Reset() method of on property if exist
                {
                    p.PropertyType.GetMethod("Reset")?.Invoke(p.GetValue(this, null), null);
                }
            }
            Enabled = enabled;
            SimMeasuresEnabled = simMeasuresEnabled;
        }

        #region Static helpers

        private static void SetInsertDictionary<TKey>(IDictionary<TKey, ulong> dict, params TKey[] exclude) where TKey : Enum
            => SetInsertDictionary(dict, (TKey[])Enum.GetValues(typeof(TKey)), null, exclude);

        private static void SetInsertDictionary<TKey, TValue>(IDictionary<TKey, TValue> dict, Func<TValue> @default, params TKey[] exclude) where TKey : Enum
            => SetInsertDictionary(dict, (TKey[])Enum.GetValues(typeof(TKey)), @default, exclude);

        private static void SetInsertDictionary<TKey, TValue>(IDictionary<TKey, TValue> dict, TKey[] categories, Func<TValue> @default = null, params TKey[] exclude)
        {
            dict.Clear();
            foreach (TKey type in categories)
            {
                if (exclude is null || false == exclude.Contains(type))
                {
                    dict[type] = @default is null ? default : @default();
                }
            }
        }
        #endregion

    }
}
