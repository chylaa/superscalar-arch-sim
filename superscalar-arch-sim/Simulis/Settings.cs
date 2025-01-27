using superscalar_arch_sim.Utilis;
using System;
using System.Collections.Generic;
using System.IO;
using static superscalar_arch_sim.RV32.Hardware.Units.BranchPredictor;


namespace superscalar_arch_sim.Simulis
{
    public static class Settings
    {
        public const string FileExtension = ".scs";
        public enum DynamicCoreMode { 
            /// <summary>Static in-order execution - each execution unit must be ready to begin execution of oldest issued instructions.</summary>
            InOrderExecution, 
            /// <summary>Dynamic out-of-order execution - instructions can pass eachother being issued to free execute units.</summary>
            OutOfOrderExecution, 
        }
        
        #region DEFAULTS
        public const uint ROM_ORIGIN = 0x00000000; const uint ROM_LENGTH = 0x10000;
        public const uint RAM_ORIGIN = 0x00010000; const uint RAM_LENGTH = 0x80000;
        #endregion

        public static bool SettingsChanged { get; set; } = false;

        #region Data Dependencies
        /// <summary>
        /// Set to <see langword="true"/> to use additional forwarding datapaths for resolving hazards,
        /// <see langword="false"/> if pipeline should always be stalled until hazard is resolved.
        /// </summary>
        public static bool Static_UseForwarding { get; set; } = true;
        /// <summary>
        /// Set to <see langword="true"/> to use additional forwarding datapaths for bypassing
        /// Store value to first consecutive Load that loads it from the same address.
        /// </summary>
        public static bool Dynamic_BypassStoreLoad { get; set; } = true;
        /// <summary>If set to true, breaks dispatch bundle on illegal instructions and continues execution.</summary>
        public static bool Dynamic_DispatchIgnoreIllegalInstructions { get; set; } = true;

        public static bool Dynamic_AllowSpeculativeLoads { get; set; } = false;
        public static bool Dynamic_WriteCommonDataBusFromExecute { get; set; } = false;
        #endregion

        #region Exceptions settings
        public static bool ThrowOnExecuteOverflow { get; set; } = true;
        public static bool ThrowOnExecuteZeroDivision { get; set; } = true;
        #endregion

        #region Branch Predictor
        /// <summary>Specifies if branch prediction unit shuuld be used (<see langword="true"/>) or pipeline must stall on branch instruction (<see langword="false"/>).</summary>
        public static bool BranchPredictionEnabled { get; set; } = true;
        /// <summary>Allow to specify which static prediction strategy should be used when <see cref="UseDynamicBranchPrediction"/> is set to <see langword="true".</summary>
        public static Prediction StaticPrediction { get; set; } = Prediction.NotTaken;
        public static PredictionScheme UsedPredictionScheme { get; set; } = PredictionScheme.Static;
        public static FSMScheme UsedFMSScheme { get; set; } = FSMScheme.SaturatingCounter;

        public static uint NumberOfPredictionBitsForFSMs { get; set; } = 2;
        /// <summary>Sets number of entries for Branch Target Buffer (BTB - for branch address). Will be eqal (2^<see langword="value"/>)-1. <u>Max value is 32</u>.</summary>
        public static uint BitsOfEntriesInBranchTargetBuffer { get; set; } = 32;
        /// <summary>Sets number of entries for Branch History Table, which will be eqal (2^<see langword="value"/>)-1. <u>Max value is 32</u>.</summary>
        public static uint BitsOfEntriesInBranchHistoryTable { get; set; } = 2;
        /// <summary>Initial value for Brach History Table entry if it is a new one.</summary>
        public static uint InitialValueOfFSMinBranchHistTable { get; set; } = 0; // Strongly not taken
        /// <summary>Specifies number <b>N</b> in N-bit pattern history table for <see cref="PredictionScheme.AdaptivePredictor"/>. <u>Max value is 32</u>.</summary>
        public static uint NumberOfHistoryBitsInPatternHistoryTable { get; set; } = 4;
        public static uint InitialBranchPatternSequence { get; set; } = 0;
        #endregion

        #region Memory & Addresses
        public static uint OriginROMAddress { get; set; } = ROM_ORIGIN;
        public static uint OriginRAMAddress { get; set; } = RAM_ORIGIN;
        public static uint ROMBytesLength { get; set; } = ROM_LENGTH;
        public static uint RAMBytesLength { get; set; } = RAM_LENGTH; // (uint.MaxValue - RAM_ORIGIN) + 1U;
        /// <summary>Address that <see cref="GlobalProgramCounter"/> is initialized with.</summary>
        public static uint ProgramStart { get; set; } = ROM_ORIGIN;
        #endregion

        #region Superscalar execution
        public static DynamicCoreMode CoreMode { get; set; } = DynamicCoreMode.OutOfOrderExecution;//DynamicCoreMode.InOrderExecution;

        /// <summary>Max number of instructions that can be issued in single clock cycle.</summary>
        public static int MaxIssuesPerClock { get; set; } = 1;
        /// <summary>Max number of instruction that can be processed by front-end Fetch and Decode stages in Supersacalar unit.</summary>
        public static int FetchWidth { get; set; } = 1;
        /// <summary>
        /// Max number of instructions that can be queued from <see cref="RV32.Hardware.Pipeline.TEM.Stage.Fetch"/> and
        /// <see cref="RV32.Hardware.Pipeline.TEM.Stage.Decode"/> stages into <see cref="RV32.Hardware.Pipeline.TEM.Stage.Dispatch"/>.
        public static int InstructionQueueCapacity { get; set; } = 16;

        public const int NumberOfFPFunctionalUnits = 0; // not implemented
        public const int NumberOfBranchFunctionalUnits = 1; // always 1

        public static int NumberOfMemoryFunctionalUnits { get; set; } = 1;
        public static int NumberOfIntegerFunctionalUnits { get; set; } = 1;

        public static int NumberOfReorderBufferEntries { get; set; } = 10;
        public static int NumberOfBranchReservationStations { get; set; } = 2;
        public static int NumberOfIntegerReservationStations { get; set; } = 4;
        public static int NumberOfMemoryReservationStationBuffers { get; set; } = 4;

        public static int NumberOfReservationStationsPerMemoryUnit
            => (NumberOfMemoryReservationStationBuffers / NumberOfMemoryFunctionalUnits);
        public static int NumberOfReservationStationsPerIntegerUnit 
            => (NumberOfIntegerReservationStations / NumberOfIntegerFunctionalUnits);

        public static int TotalNumberOfExecutionUnits
            => (NumberOfMemoryFunctionalUnits + NumberOfBranchFunctionalUnits + NumberOfIntegerFunctionalUnits + NumberOfFPFunctionalUnits);
        public static int TotalNumberOfReservationStations
            => (NumberOfBranchReservationStations + NumberOfIntegerReservationStations + NumberOfMemoryReservationStationBuffers);

        #endregion

        #region Import/Export
        public static bool ValidateSettings()
        {
            bool ok = true;
            ok &= (InstructionQueueCapacity > 0 && InstructionQueueCapacity % MaxIssuesPerClock == 0);
            ok &= (FetchWidth > 0 && FetchWidth < InstructionQueueCapacity);
            ok &= (MaxIssuesPerClock > 0 && MaxIssuesPerClock <= TotalNumberOfExecutionUnits);
            ok &= (NumberOfBranchReservationStations >= NumberOfBranchFunctionalUnits);
            ok &= (NumberOfIntegerReservationStations >= NumberOfIntegerFunctionalUnits);
            ok &= (NumberOfMemoryReservationStationBuffers >= NumberOfMemoryFunctionalUnits);
            return ok;
        }
        public static bool ImportSettings(string path)
        {
            if (path is null || false == path.EndsWith(FileExtension) || false == File.Exists(path))
                return false;
			Dictionary<string, string> values = new Dictionary<string, string>();
			using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
			using (StreamReader sr = new StreamReader(fs))
			{
                string line;
                while ((line = sr.ReadLine()) != null) 
                {
                    string[] keyval = line.Split('=');
                    if (keyval.Length == 2)
                        values.Add(keyval[0], keyval[1]);
                }
			}
            var props = typeof(Settings).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            if (values.Count < (props.Length / 2))
            {   // try parse as old format
                return StaticClassSerializer.Load(typeof(Settings), path);
            }
            SettingsChanged = true;
            // "Using .Reflection is nasty and non pure C# way" says who now
            if (values.TryGetValue(nameof(Static_UseForwarding), out var staticUseForwarding))
                Static_UseForwarding = bool.Parse(staticUseForwarding);
            if (values.TryGetValue(nameof(Dynamic_BypassStoreLoad), out var dynamicBypassStoreLoad))
                Dynamic_BypassStoreLoad = bool.Parse(dynamicBypassStoreLoad);
            if (values.TryGetValue(nameof(ThrowOnExecuteOverflow), out var throwOnExecuteOverflow))
                ThrowOnExecuteOverflow = bool.Parse(throwOnExecuteOverflow);
            if (values.TryGetValue(nameof(ThrowOnExecuteZeroDivision), out var throwOnExecuteZeroDivision))
                ThrowOnExecuteZeroDivision = bool.Parse(throwOnExecuteZeroDivision);
            if (values.TryGetValue(nameof(BranchPredictionEnabled), out var branchPredictionEnabled))
                BranchPredictionEnabled = bool.Parse(branchPredictionEnabled);
            if (values.TryGetValue(nameof(StaticPrediction), out var staticPrediction))
                StaticPrediction = Enum.TryParse<Prediction>(staticPrediction, out var parsedPrediction) ? parsedPrediction : throw new ArgumentException("Invalid StaticPrediction value.");
            if (values.TryGetValue(nameof(BitsOfEntriesInBranchTargetBuffer), out var bitOfEntriesInBTB))
                BitsOfEntriesInBranchTargetBuffer = uint.TryParse(bitOfEntriesInBTB, out uint parsedBitsInBTB) ? parsedBitsInBTB : throw new ArgumentException("Invalid BitsOfEntriesInBranchTargetBuffer value."); 
            if (values.TryGetValue(nameof(UsedPredictionScheme), out var usedPredictionScheme))
                UsedPredictionScheme = Enum.TryParse<PredictionScheme>(usedPredictionScheme, out var parsedScheme) ? parsedScheme : throw new ArgumentException("Invalid UsedPredictionScheme value.");
            if (values.TryGetValue(nameof(UsedFMSScheme), out var usedFMSScheme))
                UsedFMSScheme = Enum.TryParse<FSMScheme>(usedFMSScheme, out var parsedFsmScheme) ? parsedFsmScheme : throw new ArgumentException("Invalid UsedFMSScheme value.");
            if (values.TryGetValue(nameof(NumberOfPredictionBitsForFSMs), out var numPredBitsForFSM))
                NumberOfPredictionBitsForFSMs = uint.TryParse(numPredBitsForFSM, out var parsedNumPredBits) ? parsedNumPredBits : throw new ArgumentException("Invalid NumberOfPredictionBitsForFSMs value.");
            if (values.TryGetValue(nameof(BitsOfEntriesInBranchHistoryTable), out var bitsInBranchHistoryTable))
                BitsOfEntriesInBranchHistoryTable = uint.TryParse(bitsInBranchHistoryTable, out var parsedBitsBranchTable) ? parsedBitsBranchTable : throw new ArgumentException("Invalid BitsOfEntriesInBranchHistoryTable value.");
            if (values.TryGetValue(nameof(InitialValueOfFSMinBranchHistTable), out var initialValueFSMinBranchHistTable))
                InitialValueOfFSMinBranchHistTable = uint.TryParse(initialValueFSMinBranchHistTable, out var parsedInitValue) ? parsedInitValue : throw new ArgumentException("Invalid InitialValueOfFSMinBranchHistTable value.");
            if (values.TryGetValue(nameof(NumberOfHistoryBitsInPatternHistoryTable), out var numHistBitsInPatternHistory))
                NumberOfHistoryBitsInPatternHistoryTable = uint.TryParse(numHistBitsInPatternHistory, out var parsedNumHistBits) ? parsedNumHistBits : throw new ArgumentException("Invalid NumberOfHistoryBitsInPatternHistoryTable value.");
            if (values.TryGetValue(nameof(InitialBranchPatternSequence), out var initialBranchPatternSeq))
                InitialBranchPatternSequence = uint.TryParse(initialBranchPatternSeq, out var parsedInitPatternSeq) ? parsedInitPatternSeq : throw new ArgumentException("Invalid InitialBranchPatternSequence value.");
            if (values.TryGetValue(nameof(OriginROMAddress), out var originRomAddress))
                OriginROMAddress = uint.TryParse(originRomAddress, out var parsedRomAddress) ? parsedRomAddress : throw new ArgumentException("Invalid OriginROMAddress value.");
            if (values.TryGetValue(nameof(OriginRAMAddress), out var originRamAddress))
                OriginRAMAddress = uint.TryParse(originRamAddress, out var parsedRamAddress) ? parsedRamAddress : throw new ArgumentException("Invalid OriginRAMAddress value.");
            if (values.TryGetValue(nameof(ROMBytesLength), out var romBytesLength))
                ROMBytesLength = uint.TryParse(romBytesLength, out var parsedRomBytes) ? parsedRomBytes : throw new ArgumentException("Invalid ROMBytesLength value.");
            if (values.TryGetValue(nameof(RAMBytesLength), out var ramBytesLength))
                RAMBytesLength = uint.TryParse(ramBytesLength, out var parsedRamBytes) ? parsedRamBytes : throw new ArgumentException("Invalid RAMBytesLength value.");
            if (values.TryGetValue(nameof(ProgramStart), out var programStart))
                ProgramStart = uint.TryParse(programStart, out var parsedProgStart) ? parsedProgStart : throw new ArgumentException("Invalid ProgramStart value.");
            if (values.TryGetValue(nameof(CoreMode), out var coreMode))
                CoreMode = Enum.TryParse<DynamicCoreMode>(coreMode, out var parsedCoreMode) ? parsedCoreMode : throw new ArgumentException("Invalid CoreMode value.");
            if (values.TryGetValue(nameof(MaxIssuesPerClock), out var maxIssuesPerClock))
                MaxIssuesPerClock = int.TryParse(maxIssuesPerClock, out var parsedMaxIssues) ? parsedMaxIssues : throw new ArgumentException("Invalid MaxIssuesPerClock value.");
            if (values.TryGetValue(nameof(FetchWidth), out var fetchWidth))
                FetchWidth = int.TryParse(fetchWidth, out var parsedFetchWidth) ? parsedFetchWidth : throw new ArgumentException("Invalid FetchWidth value.");
            if (values.TryGetValue(nameof(InstructionQueueCapacity), out var instructionQueueCapacity))
                InstructionQueueCapacity = int.TryParse(instructionQueueCapacity, out var parsedInstrQueueCap) ? parsedInstrQueueCap : throw new ArgumentException("Invalid InstructionQueueCapacity value.");
            if (values.TryGetValue(nameof(NumberOfMemoryFunctionalUnits), out var numMemoryFuncUnits))
                NumberOfMemoryFunctionalUnits = int.TryParse(numMemoryFuncUnits, out var parsedMemUnits) ? parsedMemUnits : throw new ArgumentException("Invalid NumberOfMemoryFunctionalUnits value.");
            if (values.TryGetValue(nameof(NumberOfIntegerFunctionalUnits), out var numIntegerFuncUnits))
                NumberOfIntegerFunctionalUnits = int.TryParse(numIntegerFuncUnits, out var parsedIntUnits) ? parsedIntUnits : throw new ArgumentException("Invalid NumberOfIntegerFunctionalUnits value.");
            if (values.TryGetValue(nameof(NumberOfReorderBufferEntries), out var numReorderBufferEntries))
                NumberOfReorderBufferEntries = int.TryParse(numReorderBufferEntries, out var parsedReorderBuf) ? parsedReorderBuf : throw new ArgumentException("Invalid NumberOfReorderBufferEntries value.");
            if (values.TryGetValue(nameof(NumberOfBranchReservationStations), out var numBranchResStations))
                NumberOfBranchReservationStations = int.TryParse(numBranchResStations, out var parsedBranchResStations) ? parsedBranchResStations : throw new ArgumentException("Invalid NumberOfBranchReservationStations value.");
            if (values.TryGetValue(nameof(NumberOfIntegerReservationStations), out var numIntResStations))
                NumberOfIntegerReservationStations = int.TryParse(numIntResStations, out var parsedIntResStations) ? parsedIntResStations : throw new ArgumentException("Invalid NumberOfIntegerReservationStations value.");
            if (values.TryGetValue(nameof(NumberOfMemoryReservationStationBuffers), out var numMemResStationBuffers))
                NumberOfMemoryReservationStationBuffers = int.TryParse(numMemResStationBuffers, out var parsedMemResBuffers) ? parsedMemResBuffers : throw new ArgumentException("Invalid NumberOfMemoryReservationStationBuffers value.");
            if (values.TryGetValue(nameof(Dynamic_AllowSpeculativeLoads), out var allowSpeculativeLoads))
                Dynamic_AllowSpeculativeLoads = bool.Parse(allowSpeculativeLoads);
            if (values.TryGetValue(nameof(Dynamic_WriteCommonDataBusFromExecute), out var writeCommonDataBus))
                Dynamic_WriteCommonDataBusFromExecute = bool.Parse(writeCommonDataBus);
            if (values.TryGetValue(nameof(Dynamic_DispatchIgnoreIllegalInstructions), out var dispatchIgnoreIllegal))
                Dynamic_DispatchIgnoreIllegalInstructions = bool.Parse(dispatchIgnoreIllegal);
            return true;
        }
        public static bool ExportSettings(string path)
        {
            if (path != null && path.EndsWith(FileExtension))
            {
                var content = string.Empty; var nl = Environment.NewLine;
                content += $"{nl}{nameof(SettingsChanged)}={SettingsChanged}";
                content += $"{nl}{nameof(Static_UseForwarding)}={Static_UseForwarding}";
                content += $"{nl}{nameof(Dynamic_BypassStoreLoad)}={Dynamic_BypassStoreLoad}";
                content += $"{nl}{nameof(Dynamic_AllowSpeculativeLoads)}={Dynamic_AllowSpeculativeLoads}";
                content += $"{nl}{nameof(Dynamic_WriteCommonDataBusFromExecute)}={Dynamic_WriteCommonDataBusFromExecute}";
                content += $"{nl}{nameof(Dynamic_DispatchIgnoreIllegalInstructions)}={Dynamic_DispatchIgnoreIllegalInstructions}";
                content += $"{nl}{nameof(ThrowOnExecuteOverflow)}={ThrowOnExecuteOverflow}";
                content += $"{nl}{nameof(ThrowOnExecuteZeroDivision)}={ThrowOnExecuteZeroDivision}";
                content += $"{nl}{nameof(BranchPredictionEnabled)}={BranchPredictionEnabled}";
                content += $"{nl}{nameof(StaticPrediction)}={StaticPrediction}";
                content += $"{nl}{nameof(UsedPredictionScheme)}={UsedPredictionScheme}";
                content += $"{nl}{nameof(UsedFMSScheme)}={UsedFMSScheme}";
                content += $"{nl}{nameof(BitsOfEntriesInBranchTargetBuffer)}={BitsOfEntriesInBranchTargetBuffer}";
                content += $"{nl}{nameof(NumberOfPredictionBitsForFSMs)}={NumberOfPredictionBitsForFSMs}";
                content += $"{nl}{nameof(BitsOfEntriesInBranchHistoryTable)}={BitsOfEntriesInBranchHistoryTable}";
                content += $"{nl}{nameof(InitialValueOfFSMinBranchHistTable)}={InitialValueOfFSMinBranchHistTable}"; // Strongly not taken
                content += $"{nl}{nameof(NumberOfHistoryBitsInPatternHistoryTable)}={NumberOfHistoryBitsInPatternHistoryTable}";
                content += $"{nl}{nameof(InitialBranchPatternSequence)}={InitialBranchPatternSequence}";
                content += $"{nl}{nameof(OriginROMAddress)}={OriginROMAddress}";
                content += $"{nl}{nameof(OriginRAMAddress)}={OriginRAMAddress}";
                content += $"{nl}{nameof(ROMBytesLength)}={ROMBytesLength}";
                content += $"{nl}{nameof(RAMBytesLength)}={RAMBytesLength}";
                content += $"{nl}{nameof(ProgramStart)}={ProgramStart}";
                content += $"{nl}{nameof(CoreMode)}={CoreMode}";
                content += $"{nl}{nameof(MaxIssuesPerClock)}={MaxIssuesPerClock}";
                content += $"{nl}{nameof(FetchWidth)}={FetchWidth}";
                content += $"{nl}{nameof(InstructionQueueCapacity)}={InstructionQueueCapacity}";
                content += $"{nl}{nameof(NumberOfMemoryFunctionalUnits)}={NumberOfMemoryFunctionalUnits}";
                content += $"{nl}{nameof(NumberOfIntegerFunctionalUnits)}={NumberOfIntegerFunctionalUnits}";
                content += $"{nl}{nameof(NumberOfReorderBufferEntries)}={NumberOfReorderBufferEntries}";
                content += $"{nl}{nameof(NumberOfBranchReservationStations)}={NumberOfBranchReservationStations}";
                content += $"{nl}{nameof(NumberOfIntegerReservationStations)}={NumberOfIntegerReservationStations}";
                content += $"{nl}{nameof(NumberOfMemoryReservationStationBuffers)}={NumberOfMemoryReservationStationBuffers}";
                File.WriteAllText(path, content);
                return true;
            }
            return false;
        }
        public static string GetStandardSettingsFilename(string ext = FileExtension)
        {
            string stdFilename = (CoreMode == DynamicCoreMode.OutOfOrderExecution ? "ooo-" : "io-");
            stdFilename += $"{FetchWidth}fetch-";
            stdFilename += $"{MaxIssuesPerClock}issue-";
            stdFilename += $"{InstructionQueueCapacity}queue-";
            stdFilename += $"{NumberOfReorderBufferEntries}rob-";
            stdFilename += $"{NumberOfBranchFunctionalUnits}brch{NumberOfBranchReservationStations}-";
            stdFilename += $"{NumberOfMemoryFunctionalUnits}mem{NumberOfReservationStationsPerMemoryUnit}-";
            stdFilename += $"{NumberOfIntegerFunctionalUnits}int{NumberOfReservationStationsPerIntegerUnit}-";
            string predicion = UsedPredictionScheme.ToString();
            predicion += (UsedPredictionScheme == PredictionScheme.Static)
                ? ('-' + StaticPrediction.ToString())
                : string.Empty;
            stdFilename += $"{predicion}{ext}";
            return stdFilename;
        }
        #endregion

    }
}
