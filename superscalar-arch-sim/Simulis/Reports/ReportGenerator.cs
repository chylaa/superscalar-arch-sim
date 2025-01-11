using superscalar_arch_sim.RV32.Hardware.Pipeline;
using superscalar_arch_sim.RV32.Hardware.Register;
using superscalar_arch_sim.RV32.ISA;
using superscalar_arch_sim.RV32.ISA.Instructions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using static superscalar_arch_sim.Simulis.Reports.SimReporter;

namespace superscalar_arch_sim.Simulis.Reports
{
    [Flags]
    public enum Report : int
    {
        None = 1 >> 1,
        RetiredInstrucitonsList = 1 << 0,
        DataWritten = 1 << 1,
        RegisterStatusContent = 1 << 2,
        SimCounters = 1 << 3,
        SimMeasures = 1 << 4,
    }
    public class ReportGenerator
    {
        private const uint RETIRED_I32_COLLECTION_MAXSIZE = 131_072; // 1<<17
        private const uint DATA_WRITTEN_COLLECTION_MAXSIZE = 131_072;

        public static readonly Encoding DefaultFileEncoding = Encoding.ASCII;
        public readonly string DefaultCommitedInstructionFile;
        public readonly string DefaultDataWrittenFile;
        public readonly string DefaultRegisterStatusFile;
        public readonly string DefaultCountersReportFile;
        public readonly string DefaultAggregatesReportFile;
        public readonly string DefaultHistorgamsReportFile;


        #region Private mem variables
        private readonly int[] _LastRegStatusState = new int[ISAProperties.NO_INT_REGISTERS];
        private readonly char PlatformPrefix = '\0';
        private Report createReport = Report.None;
        #endregion

        public string CurrentReportFolder { get; private set; }

        #region Settings
        public bool Enabled { get => CreateReport != Report.None; }
        public bool IncludeClockCycleInDataWriteLog { get; set; } = false;
        public bool IncludeBubbleNopInProgramListing { get; set; } = false;
        #endregion

        #region Report paths
        public string LogRecordsMainFolder { get; set; } = null;
        public Report CreateReport { get => GetCreateReport(); set => SetCreateReport(value); }
        private static string NewDateTimeNowStr() => DateTime.Now.ToString("yyyyMMddHHmmss");
        private string GetReportFolderName() => $"{PlatformPrefix}-report-{NewDateTimeNowStr()}";
        #endregion

        #region Report collections
        /// <summary>Stores list of commited instructions (<see cref="Instruction.ASM"/>) with their retire cycle number and fetch addresses.</summary>
        readonly List<Tuple<ulong, uint, string, uint>> RetiredCycleAdressInstructionList;
        readonly List<DataWriteEventArgs> DataWriteEventsList;
        readonly List<Tuple<ulong, int[]>> RegisterStatusContent;
        #endregion

        public SimReporter Reporter { get; }

        public ReportGenerator(in int pipelineDepth, in char platformPrefix) 
        {
            PlatformPrefix = platformPrefix;
            Reporter = new SimReporter(pipelineDepth, platformPrefix);

            DefaultCommitedInstructionFile = $"{PlatformPrefix}-prglst.txt";
            DefaultDataWrittenFile = $"{PlatformPrefix}-datawr.txt";
            DefaultRegisterStatusFile = $"{PlatformPrefix}-regstat.txt";
            DefaultCountersReportFile = $"{PlatformPrefix}-simcounters.txt";
            DefaultAggregatesReportFile = $"{PlatformPrefix}-measures-aggregate.json";
            DefaultHistorgamsReportFile = $"{PlatformPrefix}-measures-histograms.txt";

            RetiredCycleAdressInstructionList = new List<Tuple<ulong, uint, string, uint>>();
            DataWriteEventsList = new List<DataWriteEventArgs>();
            RegisterStatusContent = new List<Tuple<ulong, int[]>>();

            Reset();
        }

        public Report GetCreateReport() 
            => createReport;
        public void SetCreateReport(Report report)
            => Reporter.SimMeasuresEnabled = IsEnabled(createReport = report, Report.SimMeasures);
        public bool ReportEnabled(Report report) 
            => (IsEnabled(CreateReport, report) && LogRecordsMainFolder != null);

        public void Reset()
        {
            Reporter.Reset();
            CurrentReportFolder = null;
            DataWriteEventsList.Clear();
            RetiredCycleAdressInstructionList.Clear();
            RegisterStatusContent.Clear();
        }

        #region Collection updates
        public void UpdateDataWrittenList(object sender, DataWriteEventArgs e)
        {
            if (ReportEnabled(Report.DataWritten))
            {
                DataWriteEventsList.Add(e);
                if (DataWriteEventsList.Count >= DATA_WRITTEN_COLLECTION_MAXSIZE)
                {
                    _ = AsyncAppendDataWrittenEventsLogToFile(CurrentReportFolder); // will clear snapshot
                    SpinWait.SpinUntil(() => DataWriteEventsList.Count == 0); // wait for filewrite to begin
                }
            }
        }
        public void UpdateRegisterStatus(ulong cycle, Register32File regfile)
        {
            if (ReportEnabled(Report.RegisterStatusContent))
            {
                int[] regstat = regfile.GetRegisterStatusCopy();
                if (false == regstat.SequenceEqual(_LastRegStatusState))
                {
                    regstat.CopyTo(_LastRegStatusState, 0);
                    RegisterStatusContent.Add(new Tuple<ulong, int[]>(cycle, regstat));
                    if (RegisterStatusContent.Count >= DATA_WRITTEN_COLLECTION_MAXSIZE)
                    {
                        _ = AsyncAppendRegisterStatusLogToFile(CurrentReportFolder); // will clear snapshot
                        SpinWait.SpinUntil(() => RegisterStatusContent.Count == 0); // wait for filewrite to begin
                    }
                }
            }

        }
        public void UpdateRetiredInstructions(Instruction retired, Register32 localPC, ulong clockCycles)
        {
            if (ReportEnabled(Report.RetiredInstrucitonsList))
            {
                if (IncludeBubbleNopInProgramListing || false == retired.BubbleInstruction)
                {
                    uint fetchAddr = localPC.ReadUnsigned();
                    RetiredCycleAdressInstructionList.Add(new Tuple<ulong, uint, string, uint>(clockCycles, fetchAddr, retired.ASM, retired.Value));

                    if (RetiredCycleAdressInstructionList.Count >= RETIRED_I32_COLLECTION_MAXSIZE)
                    {
                        _ = AsyncAppendRetiredInstructionPairsToFile(CurrentReportFolder); // will clear snapshot
                        SpinWait.SpinUntil(() => RetiredCycleAdressInstructionList.Count == 0); // wait for filewrite to begin
                    }
                }
            }
        }

        #endregion
        #region File Reports 

        #region Append/Write implementations
        public async Task<bool> AsyncAppendDataWrittenEventsLogToFile(string reportDir = null)
        {
            if (false == IsEnabled(CreateReport, Report.DataWritten))
                return false;

            string ParseLine(DataWriteEventArgs e)
            {
                string dest = null;
                if (e.Destination == DataWriteEventArgs.WriteDestination.Register)
                    dest = $"REG[r{e.TargetAddress:00}] [{Disassembler.GetRegisterAbiName((int)e.TargetAddress)}] = ";
                else if (e.Destination == DataWriteEventArgs.WriteDestination.Memory)
                    dest = $"MEM[{e.TargetAddress:X8}] = ";

                string logline = $"{dest}{e.EffectiveValue:X8} | at {e.InstructionAddress:X8}  {e.I32}";
                string clockCycle = IncludeClockCycleInDataWriteLog ? (e.Cycle?.ToString() ?? string.Empty) : string.Empty;
                if (false == string.IsNullOrEmpty(clockCycle))
                    clockCycle = "c:" + clockCycle;
                return (logline.PadRight(64) + clockCycle);
            }

            string path = CreateFolderDirectoryGetPath(reportDir, DefaultDataWrittenFile);
            await AsyncWriteFileFromCollection(path, DataWriteEventsList, ParseLine, header: null);
            return true;
        }
        public async Task<bool> AsyncAppendRetiredInstructionPairsToFile(string reportDir = null)
        {
            if (false == IsEnabled(CreateReport, Report.RetiredInstrucitonsList))
                return false;

            string ParseLine(Tuple<ulong, uint, string, uint> set) => $"{set.Item1:X8} | {set.Item2:X8} | {set.Item4:X8} | {set.Item3}";
            string header;
            header = "  CYCLE  |   ADDR   |  I_HEX   |    I_ASM    " + Environment.NewLine;
            header += "---------+----------+----------+-------------";

            string path = CreateFolderDirectoryGetPath(reportDir, DefaultCommitedInstructionFile);
            await AsyncWriteFileFromCollection(path, RetiredCycleAdressInstructionList, ParseLine, header);
            return true;
        }
        public async Task<bool> AsyncAppendRegisterStatusLogToFile(string reportDir = null)
        {
            if (false == IsEnabled(CreateReport, Report.RegisterStatusContent))
                return false;

            string header;
            header = "  CYCLE  |                          STATUS[0:31]                          " + Environment.NewLine;
            header += "--------+----------------------------------------------------------------";
            string ParseLine(Tuple<ulong, int[]> set) => $"{set.Item1:X8}: {string.Join(",", set.Item2)}";

            string path = CreateFolderDirectoryGetPath(reportDir, DefaultRegisterStatusFile);
            await AsyncWriteFileFromCollection(path, RegisterStatusContent, ParseLine, header);
            return true;
        }
        public async Task<bool> AsyncWriteSimCountersToFile(string reportDir = null)
        {
            if (false == Reporter.Enabled)
                return false;
            
            string path = CreateFolderDirectoryGetPath(reportDir, DefaultCountersReportFile);
            using (StreamWriter writer = new StreamWriter(path ?? reportDir, append: false, DefaultFileEncoding))
            {
                await writer.WriteAsync(BuildSimCountersReportString(Reporter));
            }
            return true;
        }
        public async Task<bool> AsyncWriteMeasuresToFiles(string reportDir = null)
        {
            if (false == Reporter.Enabled || false == Reporter.SimMeasuresEnabled)
                return false;

            string agrcontent = "{" + Environment.NewLine;
            agrcontent += ParseAggregate("ProcessingCycles", Reporter.I32Measure_ProcessingCycles);
            agrcontent += ParseAggregate("Fetch_Throughput", Reporter.I32Measure_FetchThroughput);
            agrcontent += ParseAggregate("Dispatch_Throughput", Reporter.I32Measure_DispatchThroughput);
            agrcontent += ParseAggregate("Execute_Througput", Reporter.I32Measure_ExecuteThrougput);
            agrcontent += ParseAggregate("Retire_Througput", Reporter.I32Measure_RetireThrougput);
            agrcontent += ParseAggregate("IRQueue_Size", Reporter.I32Measures_Size[SimMeasures.IRQueue]);
            agrcontent += ParseAggregate("BranchUnit_Size", Reporter.I32Measures_Size[SimMeasures.BranchUnit]);
            agrcontent += ParseAggregate("MemUnit_Size", Reporter.I32Measures_Size[SimMeasures.MemUnit]);
            agrcontent += ParseAggregate("IntUnit_Size", Reporter.I32Measures_Size[SimMeasures.IntUnit]);
            agrcontent += ParseAggregate("ROB_Size", Reporter.I32Measures_Size[SimMeasures.ROB], last:true);
            agrcontent += Environment.NewLine + "}";

            string agrpath = CreateFolderDirectoryGetPath(reportDir, DefaultAggregatesReportFile);
            using (StreamWriter writer = new StreamWriter(agrpath ?? reportDir, append: false, DefaultFileEncoding))
            {
                await writer.WriteAsync(agrcontent);
            }
            
            string dictcontent = ParseDictionary("UnitFull", Reporter.I32Measures_CollectionFull);
            dictcontent += ParseDictionary("IRQueue_Sizes_Hist", Reporter.I32Measures_SizeHist[SimMeasures.IRQueue]);
            dictcontent += ParseDictionary("FetchBundle_Sizes_Hist", Reporter.I32Measures_FetchBundleSizeHist);
            dictcontent += ParseDictionary("DispatchBundle_Sizes_Hist", Reporter.I32Measures_DispatchBundleSizeHist);
            dictcontent += ParseDictionary("AllReservationStations_Sizes_Hist", Reporter.I32Measures_RSSizeHist);
            dictcontent += ParseDictionary("ROB_Sizes_Hist", Reporter.I32Measures_SizeHist[SimMeasures.ROB]);
            dictcontent += ParseDictionary("AllExecute_Sizes_Hist", Reporter.I32Measures_ExecuteSizeHist);
            dictcontent += ParseDictionary("BranchExecute_Sizes_Hist", Reporter.I32Measures_SizeHist[SimMeasures.BranchUnit]);
            dictcontent += ParseDictionary("MemoryExecute_Sizes_Hist", Reporter.I32Measures_SizeHist[SimMeasures.MemUnit]);
            dictcontent += ParseDictionary("IntegerExecute_Sizes_Hist", Reporter.I32Measures_SizeHist[SimMeasures.IntUnit]);

            string dictpath = CreateFolderDirectoryGetPath(reportDir, DefaultHistorgamsReportFile);
            using (StreamWriter writer = new StreamWriter(dictpath ?? reportDir, append: false, DefaultFileEncoding))
            {
                await writer.WriteAsync(dictcontent);
            }
            return true;
        }
        public async Task<bool> AsyncWriteSettingsDetailsToFile(string reportDir = null)
        {
            if (false == Reporter.Enabled)
                return false;

            string extension = ".txt";
            string defaultSettingsFilename = $"{PlatformPrefix}-{Settings.GetStandardSettingsFilename(extension)}";
            string path = CreateFolderDirectoryGetPath(reportDir, defaultSettingsFilename);
            using (StreamWriter writer = new StreamWriter(path ?? reportDir, append: false, DefaultFileEncoding))
            {
                await writer.WriteAsync(BuildReportDetailsString(typeof(Settings)));
            }
            return true;
        }

        #endregion
        public bool FinalizeAllEnabledLogFiles(string reportDir = null)
        {
            reportDir = (reportDir ?? CurrentReportFolder);
            List<Task<bool>> tasks = new List<Task<bool>>();

            if (IsEnabled(CreateReport, Report.RetiredInstrucitonsList))
                tasks.Add(Task.Run(() => AsyncAppendRetiredInstructionPairsToFile(reportDir)));
            if (IsEnabled(CreateReport, Report.DataWritten))
                tasks.Add(Task.Run(() => AsyncAppendDataWrittenEventsLogToFile(reportDir)));
            if (PlatformPrefix == D_SUPERSCALAR_PLATFORM && IsEnabled(CreateReport, Report.RegisterStatusContent))
                tasks.Add(Task.Run(() => AsyncAppendRegisterStatusLogToFile(reportDir)));
            if (IsEnabled(CreateReport, Report.SimCounters))
                tasks.Add(Task.Run(() => AsyncWriteSimCountersToFile(reportDir)));
            if (IsEnabled(CreateReport, Report.SimMeasures))
                tasks.Add(Task.Run(() => AsyncWriteMeasuresToFiles(reportDir)));

            if (Enabled && tasks.Count > 0)
                tasks.Add(Task.Run(() => AsyncWriteSettingsDetailsToFile(reportDir)));

            Task.WhenAll(tasks).Wait();
            return tasks.All(t => t.Result);
        }
        private string CreateFolderDirectoryGetPath(string logpath, string file)
        {
            if (LogRecordsMainFolder is null)
                return null;
            if (logpath is null)
            {
                logpath = LogRecordsMainFolder;
                if (logpath[logpath.Length - 1] != Path.DirectorySeparatorChar)
                    logpath += Path.DirectorySeparatorChar;
                logpath += GetReportFolderName();
                if (false == Directory.Exists(logpath))
                    Directory.CreateDirectory(logpath);
            }
            CurrentReportFolder = logpath;
            return logpath + Path.DirectorySeparatorChar + file;
        }
        #endregion

        #region Static 
        public static string ParseDictionary(string name, IDictionary dict)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"::[{name}],");
            foreach (var item in dict)
            {
                DictionaryEntry? kvp = (item as DictionaryEntry?);
                sb.AppendLine($"[{kvp?.Key?.ToString()}, {kvp?.Value?.ToString()}]");
            }
            sb.AppendLine();
            return sb.ToString();
        }
        public static string ParseAggregate(string name, Aggregate aggregate, bool last = false)
        {
            string agrstr = string.Empty; string nl = Environment.NewLine;
            agrstr += $"\t\"{name}\":";
            agrstr += nl+"\t{";
            agrstr += nl+$"\t\t\"{nameof(Aggregate.Sum)}\": {aggregate.Sum},";
            agrstr += nl+$"\t\t\"{nameof(Aggregate.Count)}\": {aggregate.Count},";
            agrstr += nl+$"\t\t\"{nameof(Aggregate.Min)}\": {aggregate.Min},";
            agrstr += nl+$"\t\t\"{nameof(Aggregate.Max)}\": {aggregate.Max},";
            agrstr += nl+$"\t\t\"{nameof(Aggregate.Average)}\": "+(aggregate.Count > 0 ? aggregate.Average : double.NaN).ToString("0.000", CultureInfo.InvariantCulture)+",";
            agrstr += nl+$"\t\t\"{nameof(Aggregate.Variance)}\": "+(aggregate.Count > 0 ? aggregate.Variance : double.NaN).ToString("0.000", CultureInfo.InvariantCulture) + ",";
            agrstr += nl+$"\t\t\"{nameof(Aggregate.StdDev)}\": "+(aggregate.Count > 0 ? aggregate.StdDev : double.NaN).ToString("0.000", CultureInfo.InvariantCulture); // wihtout comma!
            agrstr += nl+"\t}" + (last ? "" : ",") +nl;
            return agrstr;
        }
        public static string BuildSimCountersReportString(SimReporter smc)
        {
            var L_DOUBLE_FORMAT = "0.000000";
            var S_DOUBLE_FORMAT = "0.00";
            string GetPrecentage(double val, double all) => ((val / all) * 100).ToString(S_DOUBLE_FORMAT);
            
            var execTypes = '[' + string.Join(", ", smc.CommitedInstructionTypesThreadSafe.Select(x => $"{x.Key}:{x.Value}")) + ']';
            var execTPrc = '[' + string.Join(", ", smc.CommitedInstructionTypesThreadSafe.Select(x => $"{x.Key}:{GetPrecentage(x.Value, smc.CommitedInstructions)}")) + ']';
            var execOpcodes= '[' + string.Join(", ", smc.CommitedInstructionOpcodesThreadSafe.Select(x => $"{x.Key}:{x.Value}")) + ']';
            var execOPrc = '[' + string.Join(", ", smc.CommitedInstructionOpcodesThreadSafe.Select(x => $"{x.Key}:{GetPrecentage(x.Value, smc.CommitedInstructions)}")) + ']';
            
            var sb = new StringBuilder();
            sb.AppendLine("Clock Cycles = "                 + smc.ClockCycles.ToString());
            sb.AppendLine("CPI = "                          + smc.CPI.ToString(L_DOUBLE_FORMAT));
            sb.AppendLine("IPC = "                          + smc.IPC.ToString(L_DOUBLE_FORMAT));
            sb.AppendLine("Commited = "                     + smc.CommitedInstructions.ToString());
            sb.AppendLine("I32 Types [#] = "                + execTypes);
            sb.AppendLine("I32 Types [%] = "                + execTPrc);
            sb.AppendLine("I32 Opcodes [#] = "              + execOpcodes);
            sb.AppendLine("I32 Opcodes [%] = "              + execOPrc);
            sb.AppendLine();
            sb.AppendLine("Data Dependencies = "            + smc.DataDependencies.ToString());
            sb.AppendLine("Overall Forwardings = "          + smc.Forwardings.ToString());
            sb.AppendLine("Feedback MEM LMD->AddrIn = "     + smc.FeedbackMEMInput.ToString());
            sb.AppendLine("Feedback ALU Out->In = "         + smc.FeedbackALUInput.ToString());
            sb.AppendLine("Forward MEM LMD to ALU = "       + smc.ForwardMEMLoadToALU.ToString());
            sb.AppendLine("Load Interlocks = "              + smc.LoadInterlocks.ToString());
            sb.AppendLine("Store Load Bypasses = "          + smc.StoreLoadBypasses.ToString());
            sb.AppendLine();
            sb.AppendLine("Overall Stalls = "               + smc.Stalls.ToString());
            sb.AppendLine("Each stage stall = "             + string.Join(", ", smc.StagesStalls));
            sb.AppendLine();
            sb.AppendLine("Branch Prediction Accuracy = "   + (100 * smc.BranchPredictionAccuracy).ToString(L_DOUBLE_FORMAT) + "[%]");
            sb.AppendLine("Mispredicted Branches = "        + smc.BranchMispredictions.ToString());
            sb.AppendLine("Predicted Branches = "           + smc.CorrectBranchPredictions.ToString());
            sb.AppendLine("Taken Branches = "               + smc.BranchesTaken.ToString());
            sb.AppendLine("Not Taken Branches = "           + smc.BranchesNotTaken.ToString());
            sb.AppendLine("Address misspredictions = "      + smc.AddressMisprediction.ToString());
            sb.AppendLine();
            return sb.ToString();
        }

        private string BuildReportDetailsString(Type staticSettings)
        {
            PropertyInfo[] properties = staticSettings.GetProperties((BindingFlags.Static | BindingFlags.Public));
            var sb = new StringBuilder();
            for (int i =0; i < properties.Length; i++)
                sb.AppendLine($"{properties[i].Name} = {properties[i].GetValue(null)}");
            return sb.ToString();
        }

        #region Helpers

        private static bool IsEnabled(Report report, Report flag) 
            => ((report & flag) == flag);
        private static object GetFirst(ICollection collection)
        {
            foreach (object o in collection) return o;
            return null;
        }

        private static Tout RunTaskSynchronusly<Tin, Tout>(Func<Tin, Task<Tout>> action, Tin arg)
        {
            Task<Tout> task = Task.Run(() => action(arg));
            task.Wait();
            return task.Result;
        }

        private static IReadOnlyCollection<T> GetSnapshot<T>(ICollection<T> original)
            => original.ToArray();


        private async static Task AsyncWriteFileFromCollection<T>(
            string logpath,
            ICollection<T> original,
            Func<T, string> parseLine,
            string header = null
        )
        {
            if (original is null || original.Count == 0)
                return;

            var snapshot = GetSnapshot(original);
            original.Clear();
            using (StreamWriter writer = new StreamWriter(logpath, append: true, DefaultFileEncoding))
            {
                if (header != null && header.Length > 0 && writer.BaseStream.Length == 0)
                {
                    await writer.WriteLineAsync(header);
                }
                foreach (var item in snapshot)
                {
                    await writer.WriteLineAsync(parseLine(item));
                }
            }
        }
        private async static Task AsyncWriteFileFromCollection<T>(
            string logpath,
            ICollection<T> original,
            Func<T, byte[]> parseLine,
            byte[] header = null
        )
        {
            if (original is null || original.Count == 0)
                return;

            var snapshot = GetSnapshot(original);
            original.Clear();
            using (FileStream fs = new FileStream(logpath, FileMode.Append, FileAccess.Write))
            {
                if (header != null && header.Length > 0 && fs.Length == 0)
                {
                    await fs.WriteAsync(header, 0, header.Length);
                }
                foreach (var item in snapshot)
                {
                    byte[] raw = parseLine(item);
                    await fs.WriteAsync(raw, 0, raw.Length);
                }
            }
        }
        #endregion
        #endregion
    }
}
