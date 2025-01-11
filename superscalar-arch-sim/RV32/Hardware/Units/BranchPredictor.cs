using superscalar_arch_sim.RV32.Hardware.Pipeline;
using superscalar_arch_sim.RV32.Hardware.Register;
using superscalar_arch_sim.RV32.ISA.Instructions;
using superscalar_arch_sim.Simulis;
using System;
using System.Collections.Generic;

using BranchTargetAddressTable = System.Collections.Generic.Dictionary<uint, int>;
using BranchAddressPatternTable = System.Collections.Generic.Dictionary<uint, uint>;
using FSMLookupTable = System.Collections.Generic.Dictionary<uint, uint>;
using AddressFSMLookupTable = System.Collections.Generic.Dictionary<uint,
                              System.Collections.Generic.Dictionary<uint, uint>>;

namespace superscalar_arch_sim.RV32.Hardware.Units
{
    public readonly struct PredictionDetails {
        public readonly UInt32 BranchAddress;
        public readonly UInt32 BranchTarget;
        public readonly BranchPredictor.Prediction Prediction;
        public readonly UInt32 Evaluation;

        public PredictionDetails(uint branchAddress, uint branchTarget, BranchPredictor.Prediction prediction, uint evaluation)
        {
            BranchAddress = branchAddress; BranchTarget = branchTarget; Prediction = prediction; Evaluation = evaluation;
        }
        public override string ToString() => $"{BranchAddress};{BranchTarget};{Prediction};{Evaluation}";
    }

    public class BranchPredictor
    {
        /// <summary>
        /// Branch decision, with additional state of <see cref="None"/> indicating irrelevant value.
        /// </summary>
        public enum Prediction { 
            NotTaken = 0, 
            Taken = 1,
            None = 2,
        };
        /// <summary>
        /// Counter-like finite-state-machine. The high-order bit specifies the prediction 
        /// and the low-order bit specifies the hysteresis (how “strong” the prediction is).
        /// </summary>
        public enum FSMScheme
        {
            /// <summary>Saturate, incrementing/decrementing by one.</summary>
            SaturatingCounter,
            /// <summary>
            /// When crossing treshold value of the 'weakest' prediction, FSM saturates completly in that direction
            /// (mispredictions made in the weakly not-taken state move the FSM directly into the strongly taken state and vice versa).
            /// </summary>
            ImmediateSaturation
        }

        public enum PredictionScheme
        {
            /// <summary>Always predict <see cref="Prediction.NotTaken"/> or <see cref="Prediction.Taken"/> (specified by <see cref="StaticPredictionStrategy"/>).</summary>
            Static,
            /// <summary>Set of finite state machines indexed by branch address in <see cref="BranchAddressHistoryTable"/>.</summary>
            BranchHistoryTable,
            /// <summary>X-way adaptive predictor with n-bit branch pattern, indexed by <see cref="BranchAddressToLocalPatternTable"/> in <see cref="BranchPatternHistoryTable"/>. Uses a finite state machine for each pattern.</summary>
            AdaptivePredictor
        }

        private uint _bitsOfEntriesBHT = 1;
        private uint _bitsOfEntriesBTB = 32;
        private uint _predictionBits = 2;
        private uint _patternBits = 2;
        private Prediction _staticPrediction = Prediction.None;

        private readonly Dictionary<PredictionScheme, Func<uint, Prediction>> SchemePredictionMethodMap;

        /// <summary>Max value that can be represented using <see cref="_predictionBits"/> bit amout.</summary>
        private uint MaxValueRepresentedOnBits(uint usedBits) => unchecked((uint)((1UL << (int)usedBits) - 1));
        /// <summary>Specifies number <b>N</b> in N-bit saturating counter for prediction scheme. <u>Max value is 32</u>.</summary>
        public uint NumberOfPredictionBitsForFSMs { get => _predictionBits; set => _predictionBits = (value % 33); }


        #region Saturating Counter

        private BranchTargetAddressTable BranchTargetBuffer;
        private FSMLookupTable BranchAddressHistoryTable;
        /// <summary>Sets number of entries for Branch History Table (BHT - for branch target). Will be eqal (2^<see langword="value"/>)-1. <u>Max value is 32</u>.</summary>
        public uint BitsOfEntriesInBranchHistoryTable { get => _bitsOfEntriesBHT; set => _bitsOfEntriesBHT = (value % 33); }
        /// <summary>Initial value for Brach History Table entry's FSM if it is a new one.</summary>
        public uint InitialValueOfFSMinBranchHistTable { get; set; } = 0; // Strongly not taken

        /// <summary>Sets number of entries for Branch Target Buffer (BTB - for branch address). Will be eqal (2^<see langword="value"/>)-1. <u>Max value is 32</u>.</summary>
        public uint BitsOfEntriesInBranchTargetBuffer { get => _bitsOfEntriesBTB; set => _bitsOfEntriesBTB = (value % 33); }
        #endregion

        #region X-way adaptive predictor with N-bit history

        /// <summary>Taken/Not-taken pattern of each branch - local version of <see cref="GlobalBranchPatternSequence"/>.</summary>
        private BranchAddressPatternTable BranchAddressToLocalPatternTable;
        /// <summary>Known branch patterns of each branch, mapped to their FSMs.</summary>
        private AddressFSMLookupTable BranchAddressToBranchPatternHistoryTable;
        /// <summary>Specifies number <b>N</b> in N-bit pattern history table for <see cref="PredictionScheme.AdaptivePredictor"/>. <u>Max value is 32</u>.</summary>
        public uint NumberOfHistoryBitsInPatternHistoryTable { get => _patternBits; set => _patternBits = (value % 33); }
        public uint InitialBranchPatternSequence { get; set; } = 0;
        public uint GlobalBranchPatternSequence { get; private set; } = 0;
        #endregion

        public Prediction CurrentPrediction { get; private set; } = Prediction.None;
        public bool? EvalCondition { get; private set; } = null;
        public Int32? TargetAddress { get; private set; } = null;

        public event EventHandler<StageDataArgs> OnBranchMisprediction;

        #region Settings
        /// <summary>If set to <see langword="true"/>, <see cref="BranchPredictor"/> is enabled, otherwise pipeline will stall on branch instructions.</summary>
        public bool Enabled { get; set; } = true;
        /// <summary>Specifies scheme for counter-like Finite-FinishedState-Machines used in <see cref="BranchPredictor"/>.</summary>
        public FSMScheme UsedFSMScheme { get; set; } = FSMScheme.SaturatingCounter;
        /// <summary>Specifies used prediction scheme in current <see cref="BranchPredictor"/> instance.</summary>
        public PredictionScheme UsedPredictionScheme { get; set; } = PredictionScheme.Static;
        /// <summary>Allow to specify which static prediction strategy should be used.</summary>
        public Prediction StaticPredictionStrategy { 
            get => _GetStaticPrediction();
            set => CurrentPrediction = (UsedPredictionScheme == PredictionScheme.Static ? (_staticPrediction = value) : CurrentPrediction);
        }

        #endregion

        public BranchPredictor() 
        {
            SchemePredictionMethodMap = new Dictionary<PredictionScheme, Func<uint, Prediction>>()
            {
                {PredictionScheme.Static, _GetStaticPrediction},
                {PredictionScheme.BranchHistoryTable, _GetFSMPrediction},
                {PredictionScheme.AdaptivePredictor, _GetAdaptvePrediction},
            };
            Reset();
        }

        #region Getters/Setters

        #region UI
        /// <summary>Thread-safe <see cref="IReadOnlyDictionary{TKey, TValue}"/> containing addresses of branches mapped to FSMs.</summary>
        public IReadOnlyDictionary<uint, uint> GetReadonlyBranchAddressHistoryTable => BranchAddressHistoryTable;
        /// <summary>Thread-safe <see cref="IReadOnlyDictionary{TKey, TValue}"/> associated with given <paramref name="addressKey"/>, containing barch bit patterns mapped to FSMs.</summary>
        public IReadOnlyDictionary<uint, uint> GetReadonlyBranchPatternHistoryTable(uint addressKey) 
            => BranchAddressToBranchPatternHistoryTable[addressKey] = GetPatternHistoryTable(addressKey);
        public IReadOnlyDictionary<uint, int> GetPredictedBranchTargetCollection()
            => BranchTargetBuffer;

        /// <summary><see langword="true"/> if <see cref="UsedPredictionScheme"/> is set to <see cref="PredictionScheme.Static"/>.</summary>
        public bool IsStaticPredictionUsed => UsedPredictionScheme == PredictionScheme.Static;
        /// <summary><see langword="true"/> if <see cref="UsedPredictionScheme"/> is set to <see cref="PredictionScheme.Static"/> and <see cref="StaticPredictionStrategy"/> is <see cref="Prediction.Taken"/>.</summary>
        public bool IsStaticTakenSchemeUsed => (UsedPredictionScheme == PredictionScheme.Static) && (StaticPredictionStrategy == Prediction.Taken);
        #endregion
        
        /// <returns><see langword="true"/> if <see cref="EvalCondition"/> is set to <see langword="true"/></returns>
        public bool EvalShouldBranch() => EvalCondition != null && EvalCondition.Value;
        /// <returns><see langword="true"/> if <see cref="CurrentPrediction"/> is set to <see cref="Prediction.Taken"/>.</returns>
        public bool PredictShouldBranch() => CurrentPrediction == Prediction.Taken;
        public bool MispredictedTaken() => (false == EvalCondition && (CurrentPrediction == Prediction.Taken));
        public bool MispredictedNotTaken() => (true == EvalCondition && (CurrentPrediction == Prediction.NotTaken));
        public void SetEvaluatedConditionValue(bool? result) => EvalCondition = result;
        public void SetTargetAddress(Int32? targetAddress) => TargetAddress = targetAddress;
        public bool IsCallMispredictionEventSet() => OnBranchMisprediction != null;
        public void CallMispredictionEvent(StageDataArgs e) => OnBranchMisprediction?.Invoke(this, e);
        #endregion

        private uint GetTableEntry(FSMLookupTable table, UInt32 fullKey, UInt32 usedBits, UInt32 initVal, out UInt32 usedKey)
        {
            uint mask = MaxValueRepresentedOnBits(usedBits);
            usedKey = fullKey & mask;
            if (false == table.ContainsKey(usedKey)) {
                table[usedKey] = initVal;
            }
            return table[usedKey];
        }

        private uint GetBranchAddressToLocalPatternTableEntry(UInt32 branchAddress, out UInt32 usedKey)
            => GetTableEntry(BranchAddressToLocalPatternTable, branchAddress, BitsOfEntriesInBranchHistoryTable,
                InitialBranchPatternSequence, out usedKey);

        private uint GetBranchHistoryTableEntry(UInt32 branchAddress, out UInt32 usedKey)
            => GetTableEntry(BranchAddressHistoryTable, branchAddress, BitsOfEntriesInBranchHistoryTable,
                             InitialValueOfFSMinBranchHistTable, out usedKey);

        private uint GetPatternHistoryTableEntry(FSMLookupTable patternTable, uint patternSequenceKey, out UInt32 usedKey)
            => GetTableEntry(patternTable, patternSequenceKey, NumberOfHistoryBitsInPatternHistoryTable,
                            InitialBranchPatternSequence, out usedKey);

        private FSMLookupTable GetPatternHistoryTable(UInt32 branchAddress)
        {
            uint mask = MaxValueRepresentedOnBits(BitsOfEntriesInBranchHistoryTable);
            uint usedKeyAddr = branchAddress & mask;
            if (false == BranchAddressToBranchPatternHistoryTable.ContainsKey(usedKeyAddr)) {
                BranchAddressToBranchPatternHistoryTable[usedKeyAddr] = new FSMLookupTable();
            }
            return BranchAddressToBranchPatternHistoryTable[usedKeyAddr];
        }

        /// <summary>
        /// With an n-bit counter (<see cref="NumberOfPredictionBitsForFSMs"/>), the counter can take on values between 0 and (2^n)-1.
        /// When the counter is <u>greater than or equal</u> to one-half of its maximum value (high order bit set) the branch is <see cref="Prediction.Taken"/>; otherwise, <see cref="Prediction.NotTaken"/>.
        /// </summary>
        private Prediction GetFSMPrediction(UInt32 newValue)
        {
            uint highBit = unchecked((uint)((int)(newValue & (1 << (int)(NumberOfPredictionBitsForFSMs-1))) >> (int)(NumberOfPredictionBitsForFSMs-1)));
            return (highBit == 1) ? Prediction.Taken : Prediction.NotTaken;
        }

        #region Prediction methods
        private Prediction _GetStaticPrediction(uint dump = 0)
        {
            return _staticPrediction;
        }
        private Prediction _GetFSMPrediction(UInt32 branchAddress)
        {
            return GetFSMPrediction(newValue: GetBranchHistoryTableEntry(branchAddress, out _));
        }
        private Prediction _GetAdaptvePrediction(UInt32 branchAddress)
        {
            uint pattern = GetBranchAddressToLocalPatternTableEntry(branchAddress, out _);
            FSMLookupTable patternHistoryTable = GetPatternHistoryTable(branchAddress);
            return GetFSMPrediction(newValue: GetPatternHistoryTableEntry(patternHistoryTable, pattern, out _));
        }

        private Prediction GetPrediction(UInt32 lookupValue)
        {
            return SchemePredictionMethodMap[UsedPredictionScheme](lookupValue);
        }
        #endregion


        private void UpdateFSM(FSMLookupTable table, uint key, uint currentValue, int addVal)
        {
            uint max = MaxValueRepresentedOnBits(NumberOfPredictionBitsForFSMs);
            bool Overflows() { return (currentValue == 0 && addVal < 0) || (currentValue == max && addVal > 0); }

            if (false == Overflows()) 
            {
                // On UsedFSMScheme == FSMScheme.SaturatingCounter
                table[key] = (uint)(currentValue + addVal);
                // fix if another
                if (UsedFSMScheme == FSMScheme.ImmediateSaturation) 
                {
                    Prediction @new = GetFSMPrediction(table[key]);
                    if (CurrentPrediction == Prediction.Taken && @new == Prediction.NotTaken)
                        table[key] = 0;
                    else if (CurrentPrediction == Prediction.NotTaken && @new == Prediction.Taken)
                        table[key] = max;
                }
            }
        }
        internal int? GetPredictedTargetAddress(UInt32 localPC)
        {
            uint mask = MaxValueRepresentedOnBits(BitsOfEntriesInBranchTargetBuffer);
            uint usedKey = localPC & mask;
            if (BranchTargetBuffer.TryGetValue(usedKey, out int target))
                return target;
            return null;
        }
        internal int? GetPredictedTargetAddress(Register32 localPC)
        {
            return GetPredictedTargetAddress(localPC.ReadUnsigned());
        }

        public void UpdateBranchTargetBuffer(uint branchAddress)
        {
            uint mask = MaxValueRepresentedOnBits(BitsOfEntriesInBranchTargetBuffer);
            uint usedKey = branchAddress & mask;
            BranchTargetBuffer[usedKey] = TargetAddress.Value;
        }

        internal void UpdateBranchHistory(Instruction i32, Register32 localPC, bool? evalTaken)
        {
            if (Enabled)
            {   // update addresses also for jumps
                uint branchAddress = localPC.ReadUnsigned();
                UpdateBranchTargetBuffer(branchAddress);
                if (Opcodes.IsBranch(i32))
                {
#if DEBUG
                    System.Diagnostics.Debug.Assert(evalTaken.HasValue, $"{i32.ASM} is branch but evalTaken == null");
#endif
                    int counterAddDiff = (evalTaken.Value ? 1 : -1);
                    uint counterValue = GetBranchHistoryTableEntry(branchAddress, out uint usedKeyAddr);
                    UpdateFSM(BranchAddressHistoryTable, usedKeyAddr, counterValue, counterAddDiff);

                    GlobalBranchPatternSequence = unchecked((GlobalBranchPatternSequence << 1) | (uint)(evalTaken.Value ? 1 : 0)); // update as shift register
                    if (UsedPredictionScheme == PredictionScheme.AdaptivePredictor)
                    {
                        uint pattern = GetBranchAddressToLocalPatternTableEntry(branchAddress, out usedKeyAddr);
                        pattern = unchecked((pattern << 1) | (uint)(evalTaken.Value ? 1 : 0)); // update as shift register
                        BranchAddressToLocalPatternTable[usedKeyAddr] = pattern; // update local branch pattern
                        
                        FSMLookupTable patternTable = GetPatternHistoryTable(branchAddress);
                        counterValue = GetPatternHistoryTableEntry(patternTable, pattern, out uint sequenceKey);
                        UpdateFSM(patternTable, sequenceKey, counterValue, counterAddDiff);
                    }
                }
            }
        }
        /// <summary>Sets prediction base on given <paramref name="branchAddress"/> using selected <see cref="UsedPredictionScheme"/>.</summary>
        internal Prediction SetCurrentPrediction(UInt32 branchAddress)
        {
            if (Enabled)
            {
                return CurrentPrediction = GetPrediction(branchAddress);
            }
            return Prediction.None;
        }

        private void ApplySettings()
        {
            Enabled = Settings.BranchPredictionEnabled;
            UsedPredictionScheme = Settings.UsedPredictionScheme;
            UsedFSMScheme = Settings.UsedFMSScheme;
            StaticPredictionStrategy = Settings.StaticPrediction;
            NumberOfHistoryBitsInPatternHistoryTable = Settings.NumberOfHistoryBitsInPatternHistoryTable;
            NumberOfPredictionBitsForFSMs = Settings.NumberOfPredictionBitsForFSMs;
            BitsOfEntriesInBranchHistoryTable = Settings.BitsOfEntriesInBranchHistoryTable;
            InitialValueOfFSMinBranchHistTable = Settings.InitialValueOfFSMinBranchHistTable;
            InitialBranchPatternSequence = Settings.InitialBranchPatternSequence;
            BitsOfEntriesInBranchTargetBuffer = Settings.BitsOfEntriesInBranchTargetBuffer;

        }
        public void Reset()
        {
            ApplySettings();

            TargetAddress = null;
            EvalCondition = null;

            BranchTargetBuffer = new BranchTargetAddressTable();
            BranchAddressHistoryTable = new FSMLookupTable();
            BranchAddressToLocalPatternTable = new BranchAddressPatternTable();
            BranchAddressToBranchPatternHistoryTable = new AddressFSMLookupTable();
            GlobalBranchPatternSequence = InitialBranchPatternSequence;

            if (UsedPredictionScheme == PredictionScheme.Static)
                CurrentPrediction = StaticPredictionStrategy;
            else
                CurrentPrediction = GetFSMPrediction(InitialValueOfFSMinBranchHistTable);
        }
    }
}
