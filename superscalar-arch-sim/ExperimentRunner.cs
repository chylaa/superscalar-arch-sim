using superscalar_arch_sim.RV32.Hardware.CPU;
using superscalar_arch_sim.RV32.Hardware.Pipeline;
using superscalar_arch_sim.Simulis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace superscalar_arch_sim
{
    public static class ExperimentRunner
    {
        #region Models
        public enum ExperimentState
        { 
            Uninitialized, 
            Ready, 
            Running, 
            Error, 
            Finished,
            Failed,
        }
        public class ExperimentStateError : Exception 
        { 
            public ExperimentState ErrorState { get; } 
            public string ErrorLine { get; } 
            public ExperimentStateError(ExperimentState state, string line, string msg) : base(msg) 
            { ErrorState = state; ErrorLine = line; }
        }
        public class ExperimentRunError : Exception
        {
            public Exception Cause { get; }
            public ExperimentRunError(Exception cause, string msg) : base(msg) { Cause = cause; }
        }
        #endregion
        public const char COMMENT = '#';
        public const char INIT_FLAG_SOUND = 'S';
        public const char INIT_FLAG_SETUP_CLEAR_OUT = 'X';
        public const char INIT_FLAG_CONTINUE_ON_ERROR = 'C';

        public const string CMD_SET_OUTPUT_DIR          = "OUT";
        public const string CMD_RUN_PROGRAM_ON_CORE     = "RUN";
        public const string CMD_LOAD_SETTINGS           = "LS";
        public const string CMD_LOAD_SETTINGS_AND_RUN   = "LSR";
        public const string CMD_LOAD_PROGRAM            = "LP";
        public const string CMD_OUT_LOAD_PROGRAM        = "OLP";
        public const string CMD_SWITCH_CORE             = "SC";
        public const string CMD_END_EXPERIMENT          = "END";

        public const string ARG_SHORT_SWITCH_CORE_SCALAR = "-s";
        public const string ARG_SHORT_SWITCH_CORE_SUPERSCALAR = "-d";
        public const string ARG_SHORT_SWITCH_CORE_BOTH = "-b";

        public const string ARG_CORE_SCALAR = "scalar";
        public const string ARG_CORE_SUPERSCALAR = "superscalar";

        public const string FileExtension = ".exp";

        public static string ExperimentStepDetailsFile = "expdetails.txt";

        public static Action<string> Logging {get; set;}
        public static Action<ExperimentRunError> OnRunError {get; set;}
        public static Action ClearLog {get; set;}

        public static event EventHandler NewRunStarting;
        public static event EventHandler NewRunStarted;

        public static ExperimentState State { get; private set; } = ExperimentState.Uninitialized;
        public static ExperimentRunError LastRunError { get; private set; } = null;
        public static char[] InitFlags { get; private set; } = null;
        public static int Version { get; private set; } = -1;
        public static string PathDirSettings { get; private set; } = null;
        public static string PathDirPrograms { get; private set; } = null;
        public static string PathDirOutput { get; private set; } = null;

        private static readonly string[] KnownCores = new string[] {
            ARG_CORE_SCALAR, ARG_CORE_SUPERSCALAR
        };
        private static readonly string[] KnownCommands = new string[] {
            CMD_SET_OUTPUT_DIR, CMD_RUN_PROGRAM_ON_CORE, CMD_LOAD_SETTINGS,
            CMD_LOAD_PROGRAM, CMD_SWITCH_CORE, CMD_END_EXPERIMENT, CMD_LOAD_SETTINGS_AND_RUN,
            CMD_OUT_LOAD_PROGRAM
        };
        private static readonly Dictionary<int, int> VersionAdditionalParams = new Dictionary<int, int>() {
            {1, 3},
        };
        private static string _scalarLogOutFolder;
        private static string _superscalarLogOutFolder;

        private static ScalarCPU ScalarCPU;
        private static SuperscalarCPU SuperscalarCPU;

        private static List<string>.Enumerator StepsEnumerator;
        private readonly static List<string> ExperimentSteps = new List<string>();

        public static string CurrentOutputDir { get; private set; }
        public static string CurrentProgramPath { get; private set; }
        public static UInt32[] CurrentProgram { get; private set; }
        public static ICPU CurrentCore { get; private set; }

        public static void Clean()
        {
            StepsEnumerator.Dispose();
            ExperimentSteps?.Clear();
            ScalarCPU = null; SuperscalarCPU = null;
            CurrentOutputDir = null;
            CurrentCore = null;
            CurrentProgramPath = null;
            CurrentProgram = null;
            LastRunError = null;     
        }
        public static bool ExperimentInitFlagSet(char flag)
        {
            return InitFlags.Contains(flag);
        }
        public static void SetupExperiment(Stream expfile, ScalarCPU scalarCPU, SuperscalarCPU superscalarCPU)
        {
            using (StreamReader sr = new StreamReader(expfile))
                SetupExperiment(sr.ReadToEnd(), scalarCPU, superscalarCPU);
        }
        public static void SetupExperiment(string expfileContent, ScalarCPU scalarCPU, SuperscalarCPU superscalarCPU)
        {
            Clean();
            State = ExperimentState.Uninitialized;
            
            var contnet = expfileContent.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var firstLine = contnet.First().Trim().ToUpper();
            InitFlags = (firstLine.Length > 1 && firstLine[0] == COMMENT) ? firstLine.Skip(1).ToArray() : new char[0];
            
            if (ExperimentInitFlagSet(INIT_FLAG_SETUP_CLEAR_OUT) && ClearLog != null)
                ClearLog.Invoke();
            Logging?.Invoke("Experiment init flags: " + (InitFlags.Length > 0 ? string.Join(", ", InitFlags) : "None"));

            ExperimentSteps.AddRange(contnet.Select(line => line.Trim()).Where(line => false == line.FirstOrDefault().Equals(COMMENT)));
            ExperimentSteps.RemoveAll(line => (line.Length == 0));
            int err = ValidateExperimentFile(ExperimentSteps);
            if (err != -1) {
                throw new ExperimentStateError(State, ExperimentSteps[err], $"File parse error in line {err}: {ExperimentSteps[err]}");
            }
            
            StepsEnumerator = ExperimentSteps.GetEnumerator();
            ScalarCPU = scalarCPU; SuperscalarCPU = superscalarCPU;
            _scalarLogOutFolder = ScalarCPU.ReportGenerator.LogRecordsMainFolder;
            _superscalarLogOutFolder = SuperscalarCPU.ReportGenerator.LogRecordsMainFolder;

            int i = 0;
            do {
                StepsEnumerator.MoveNext();
                string line = StepsEnumerator.Current;
                Logging?.Invoke(line);
                SetupFromHeaderLine(line);
            } while (VersionAdditionalParams[Version] > i++);

            CurrentOutputDir = PathDirOutput;
            Logging?.Invoke(string.Empty);
            Logging?.Invoke("Setup finished");
            State = ExperimentState.Ready;
        }

        public static bool RunStep()
        {
            if (State != ExperimentState.Ready && State != ExperimentState.Running) 
                throw new ExperimentStateError(State, string.Empty, nameof(RunStep));

            if (false == StepsEnumerator.MoveNext()) {
                FinalizeExperiment();
                State = ExperimentState.Finished;
                return false;
            }
            State = ExperimentState.Running;
            var line = StepsEnumerator.Current;
            Logging?.Invoke(line);

            string cmd = GetCommand(line);
            switch (cmd)
            {
                case CMD_SET_OUTPUT_DIR:
                    CmdSetOutputFolder(line);
                    return true;
                case CMD_OUT_LOAD_PROGRAM:
                    CmdSetOutLoadProgram(line);
                    return true;
                case CMD_LOAD_PROGRAM:
                    CmdLoadProgram(line);
                    return true;
                case CMD_LOAD_SETTINGS:
                    CmdLoadSettings(line);
                    return true;
                case CMD_LOAD_SETTINGS_AND_RUN:
                    CmdLoadSettingsAndRun(line);
                    return true;
                case CMD_SWITCH_CORE:
                    _ = CmdSwitchCore(line);
                    return true;
                case CMD_RUN_PROGRAM_ON_CORE:
                    return CmdRun();
                case CMD_END_EXPERIMENT:
                    FinalizeExperiment();
                    State = ExperimentState.Finished;
                    return false;
                default:
                    FinalizeExperiment();
                    Logging.Invoke($"Unknown command \"{cmd}\" in line: {line}");
                    State = ExperimentState.Failed;
                    return false;
            }
        }

        #region private
        private static string GetCommand(string line)
        {
            int sep = line.IndexOf(' ');
            string cmd = line;
            if (sep >= 0) cmd = line.Substring(0, sep);
            return cmd.Trim().ToUpper();
        }
        private static string GetArgument(string line)
            => GetArguments(line).FirstOrDefault() ?? string.Empty;
        private static string[] GetArguments(string line)
        {
            string[] args = line.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i].Trim();
                args[i] = (arg[0] != '"' || arg[arg.Length - 1] != '"') ? arg.ToLower() : arg.Trim('"');
            } 
            return args;
        }
        private static int ValidateExperimentFile(List<string> experiment)
        {
            for (int i = 0; i < experiment.Count; i++) 
            {
                string line = experiment[i].Trim();
                if (string.IsNullOrWhiteSpace(line))
                    return i;
                if (line[0] == COMMENT)
                    return i;
                if (line[0] == '[')
                    continue;
                string command = GetCommand(line);
                if (false == KnownCommands.Contains(command))
                    return i;
                if (command.Equals(CMD_SWITCH_CORE) && false == KnownCores.Contains(GetArgument(line)))
                    return i;
            }
            return -1;
        }
        private static void FinalizeExperiment()
        {
            ScalarCPU.ReportGenerator.LogRecordsMainFolder = _scalarLogOutFolder;
            SuperscalarCPU.ReportGenerator.LogRecordsMainFolder = _superscalarLogOutFolder;
        }
        private static void SetupCheckPathDir(string pathDir)
        {
            if (false == Directory.Exists(pathDir))
                throw new ExperimentStateError(State, pathDir, nameof(SetupCheckPathDir));
        }
        private static void SetupFromHeaderLine(string line)
        {
            if (State != ExperimentState.Uninitialized)
                throw new ExperimentStateError(State, line, nameof(SetupFromHeaderLine));
            if (line.Contains('"') && line[line.LastIndexOf('"') - 1] != '\\')
                throw new ExperimentStateError(State, line, nameof(SetupFromHeaderLine) + ": path must end with '\\' sumbol!");

            var args = line.Split('=');
            string header = args.First().Trim().ToLower();
            string value = args.Last().Trim().Trim('"');
            if (header.Equals("[ver]", StringComparison.InvariantCultureIgnoreCase))
                Version = int.Parse(value);
            else if (header.Equals("[settings]", StringComparison.InvariantCultureIgnoreCase))
                SetupCheckPathDir(PathDirSettings = value);
            else if (header.Equals("[programs]", StringComparison.InvariantCultureIgnoreCase))
                SetupCheckPathDir(PathDirPrograms = value);
            else if (header.Equals("[outpath]", StringComparison.InvariantCultureIgnoreCase))
                SetupCheckPathDir(PathDirOutput = value);
            else
                throw new ExperimentStateError(State, line, nameof(SetupFromHeaderLine));
        }
        #region Commands
        private static ICPU CmdSwitchCore(string sc)
        {
            var arg = GetArgument(sc);
            if (arg.Equals(ARG_CORE_SCALAR)) return (CurrentCore = ScalarCPU);
            else if (arg.Equals(ARG_CORE_SUPERSCALAR)) return (CurrentCore = SuperscalarCPU);
            else throw new ExperimentStateError(State, sc, nameof(CmdSwitchCore));
        }
        private static void CmdLoadSettings(string ls)
        {
            string path = PathDirSettings + GetArgument(ls);
            if (false == Settings.ImportSettings(path))
                throw new ExperimentStateError(State, ls, nameof(CmdLoadSettings));
            if (false == Settings.ValidateSettings())
                throw new ExperimentStateError(State, ls, nameof(Settings.ValidateSettings));
        }
        private static void CmdLoadProgram(string lp)
        {
            var arr = lp.Split();
            if (arr.Length < 2) throw new ExperimentStateError(State, lp, "Invalid syntax: command and at least 1 arguments expected!");

            string prg = arr[1];
            string optim = arr.Length > 2 ? arr[2] : null;
            string path = PathDirPrograms + prg + '\\' + (optim is null ? "" : optim + '\\') + $"{prg}.text";

            uint rombytesize = Math.Min(ScalarCPU.ROMSize, SuperscalarCPU.ROMSize);
            CurrentProgram = Utilis.Utilis.GetUInt32sFromFile(path, out long filesize, rombytesize, input_little_endian: true);
            if (filesize == 0)
                throw new ExperimentStateError(State, lp, nameof(CmdLoadProgram) + ": " + nameof(filesize));
            CurrentProgramPath = path;
        }
        private static void CmdSetOutputFolder(string @out)
        {
            string path = PathDirOutput + GetArgument(@out);
            if (false == Directory.Exists(path))
                Directory.CreateDirectory(path);
            CurrentOutputDir = path + (path.EndsWith("\\") ? string.Empty : "\\");
        }
        private static void CmdSetOutLoadProgram(string olp)
        {
            string[] args = GetArguments(olp);
            if (args.Length < 2) throw new ExperimentStateError(State, olp, "Invalid syntax: at least 2 arguments expected");

            string @out = CMD_SET_OUTPUT_DIR + " \"" + args[0] + (args[0].EndsWith("\\") ? "" : "\\") + args[1] + '"';
            string lp = CMD_LOAD_PROGRAM + ' ' + args[1] + (args.Length > 2 ? (' ' + args[2]) : "");
            CmdSetOutputFolder(@out);
            CmdLoadProgram(lp);
        }
        private static void CheckRunState()
        {
            string err = string.Empty;
            if (CurrentProgram is null || CurrentOutputDir.Length == 0)
                err += (nameof(CmdRun) + ": No program loaded ");
            if (CurrentCore is null)
                err += (nameof(CmdRun) + ": No core set ");
            if (CurrentOutputDir is null)
                err += (nameof(CmdRun) + ": Output dir not set ");
            if (SimuRunner.SimulationRunning)
                err += (nameof(CmdRun) + ": SimuRunner - Simulation Running");
            if (false == string.IsNullOrEmpty(err))
                throw new ExperimentStateError(State, CMD_RUN_PROGRAM_ON_CORE, err);
        }
        private static bool CmdLoadSettingsAndRun(string lsr)
        {
            string[] args = GetArguments(lsr);
            if (args.Length == 0) 
                throw new ExperimentStateError(State, lsr, "Invalid syntax: at least 1 argument expected");
            if (args.Any(a => string.IsNullOrEmpty(a)))
                throw new ExperimentStateError(State, lsr, "Invalid syntax: empty argument");

            bool flagRunBoth = false;
            CmdLoadSettings($"{CMD_LOAD_SETTINGS} \"{args[1]}\"");
            if (args[0][0] == '-')
            {
                switch (args[0])
                {
                    case ARG_SHORT_SWITCH_CORE_SCALAR:
                        CmdSwitchCore($"{CMD_SWITCH_CORE} {ARG_CORE_SCALAR}");
                        break;
                    case ARG_SHORT_SWITCH_CORE_SUPERSCALAR:
                        CmdSwitchCore($"{CMD_SWITCH_CORE} {ARG_CORE_SUPERSCALAR}");
                        break;
                    case ARG_SHORT_SWITCH_CORE_BOTH:
                        CmdSwitchCore($"{CMD_SWITCH_CORE} {ARG_CORE_SCALAR}");
                        flagRunBoth = true;
                        break;
                    default:
                        break;
                }
            }
            bool success = CmdRun();
            if (success && flagRunBoth)
            {
                CmdSwitchCore($"{CMD_SWITCH_CORE} {ARG_CORE_SUPERSCALAR}");
                success &= CmdRun();
            }
            return success;
        }
        private static bool CmdRun()
        {
            CheckRunState();
            NewRunStarting?.Invoke(CurrentCore, EventArgs.Empty);

            Settings.SettingsChanged = true;
            CurrentCore.Reset(preserveMemory:false);
            CurrentCore.FlashROM(CurrentProgram);
            CurrentCore.ReportGenerator.LogRecordsMainFolder = CurrentOutputDir;
            CurrentCore.ReportGenerator.CreateReport = Simulis.Reports.Report.SimCounters;
            CurrentCore.ReportGenerator.CreateReport |= Simulis.Reports.Report.SimMeasures;

            Logging?.Invoke($"Running {Path.GetFileName(CurrentProgramPath)} on {CurrentCore.GetType().Name} into {CurrentOutputDir}");
            NewRunStarted?.Invoke(CurrentCore, EventArgs.Empty);

            bool result = false;
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            try
            {
                while (true)
                {
                    CurrentCore.Cycle(); 
                    CurrentCore.Latch();
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                var simfrequency = ((CurrentCore.ClockCycles / (1.0f + (stopwatch.ElapsedMilliseconds / 1000))));
                Logging?.Invoke($"Effective core speed = {simfrequency:0}[Hz]");
                if (ex is EnvironmentBreak)
                {
                    Logging?.Invoke("Generating report...");
                    result = CurrentCore.ReportGenerator.FinalizeAllEnabledLogFiles();
                } 
                else 
                {
                    var expRunErr = new ExperimentRunError(ex, $"During {CurrentProgramPath??string.Empty} on {CurrentCore?.GetType().Name??string.Empty}");
                    LastRunError = expRunErr;
                    Logging?.Invoke($"Error in {CurrentCore.ClockCycles} cycle at GPC={CurrentCore.GlobalProgramCounter.Value:X}: {ex}");
                    OnRunError?.Invoke(expRunErr);
                    if (false == (result = ExperimentInitFlagSet(INIT_FLAG_CONTINUE_ON_ERROR)))
                        State = ExperimentState.Error;
                }
            }
            stopwatch.Reset();
            return result;
        }
        #endregion

        #endregion
    }
}
