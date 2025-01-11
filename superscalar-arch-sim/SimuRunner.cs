using superscalar_arch_sim.RV32.Hardware.CPU;
using superscalar_arch_sim.RV32.Hardware.Pipeline;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace superscalar_arch_sim
{
    /// <summary>
    /// New Main class providing UI with set of static methods for handling 
    /// <see cref="RV32.Hardware.CPU"/> classes simulation.
    /// </summary>
    public class SimuRunner
    {
        public enum ClockEdge { Rising, Falling }
        [Flags]
        public enum ResetUnit { Core = 1, RAM = 2, ROM = 4, Memory = 6, Registers = 8, All = (Core | Memory | Registers) }

        private static Func<bool> BreakpointPredicate = CheckBreakPoint_GPC<ICPU>;

        private static readonly Dictionary<ICPU, bool> _wascycled = new Dictionary<ICPU, bool>();
        private static readonly Dictionary<ICPU, bool> _siminitialized = new Dictionary<ICPU, bool>();
        private static ICPU BindedCore { get; set; }

        private static bool GetCoreFlag(ICPU core, Dictionary<ICPU, bool> src) => src[(core ?? BindedCore)];
        private static bool SetCoreFlag(ICPU core, Dictionary<ICPU, bool> src, bool flag) => src[(core ?? BindedCore)] = flag;

        public static ClockEdge GetNextEdge(ICPU core) 
            => (GetCoreFlag(core, _wascycled) ? ClockEdge.Falling : ClockEdge.Rising);
        public static void SetNextEdge(ICPU core, ClockEdge @value)
            => SetCoreFlag(core, _wascycled, (@value == ClockEdge.Falling));
        
        public static ClockEdge GetLastEdge(ICPU core) 
            => (GetCoreFlag(core, _wascycled) ? ClockEdge.Rising : ClockEdge.Falling);

        public static bool SimulationRunning { get; private set; } = false;
        public static long BreakpointAddress { get; private set; } = -1;
        public static long BreakpointCycle { get; private set; } = -1;
        public static TimeSpan SimulationTimeout { get; private set; } = Timeout.InfiniteTimeSpan;

        public static System.Globalization.CultureInfo SimThreadCulture { get; set; } = System.Globalization.CultureInfo.DefaultThreadCurrentCulture;
        public static CancellationTokenSource CancellationTokenSource { get; private set; }
        public static Task SimulationTask { get; private set; } = null;
        private static Stopwatch SimStopwatch { get; set; } = null;

        /// <summary>Called always from <see cref="TaskContinuationInvoker(Task)"/> if simulation finishes for any reason.</summary>
        public static Action OnSimulationTaskFinishesExecution { get; set; }
        public static Action OnSimulationEndItself { get; set; }
        public static Action OnSimulationCancelled { get; set; }
        public static Action<Exception> OnSimulationError { get; set; }
        /// <summary>Real-time simulator core frequency, represented as number of clock cycles per second [Hz].</summary>
        public static int SimulationSpeed => ((int)(BindedCore.ClockCycles / (1.0f + (SimStopwatch.ElapsedMilliseconds / 1000))));

        /// <summary>Checks if Instruction Fetch stage has fetched instruction from <see cref="BreakpointAddress"/> while not <see cref="Stage.Stalling"/>.</summary>
        /// <returns><see langword="true"/> if IF not <see cref="Stage.Stalling"/> and <see cref="BreakpointAddress"/> equals <see cref="Stage.LocalPC"/>, otherwise <see langword="false"/>.</returns>
        private static bool CheckBreakPoint_GPC<ICpu>() where ICpu : ICPU
            => BreakpointAddress > 0L && 
            (false == BindedCore.FetchStalling && BreakpointAddress == BindedCore.GlobalProgramCounter.Value);
        private static bool CheckBreakPoint_Dispatch<ICpu>() where ICpu : SuperscalarCPU
            => BreakpointAddress > 0L && (BindedCore as ICpu).Dispatch.LatchDataBuffers.Any(buffer => buffer.LocalPC.Read().Equals(BreakpointAddress));
        private static bool CheckBreakPoint_Cycle<ICpu>() where ICpu : ICPU
            => BreakpointCycle > 0L && unchecked((ulong)BreakpointCycle) == BindedCore.ClockCycles; 

        public static bool CheckBreakPoint() => BreakpointPredicate();

        /// <summary>Creates new instance of <see cref="Stopwatch"/> and starts it.</summary>
        public static void StartNewStopwatch() => SimStopwatch = Stopwatch.StartNew();
        /// <summary>Stop <see cref="SimStopwatch"/> if exists.</summary>
        public static void PauseStopwatch() => SimStopwatch?.Stop();
        /// <summary>Rensumes <see cref="SimStopwatch"/> if exist, otherwise calls <see cref="StartNewStopwatch"/>.</summary>
        public static void RensumeStopwatch() {if (SimStopwatch is null) StartNewStopwatch(); else SimStopwatch.Start(); }

        /// <returns><see cref="Stopwatch.Elapsed"/> <see cref="TimeSpan"/>.</returns>
        public static TimeSpan GetStopwatchEnlapsedTimespan() => SimStopwatch.Elapsed;

        public static void CycleOrLatch(ICPU core=null)
        {
            if ((SetCoreFlag(core, _wascycled, !GetCoreFlag(core, _wascycled)))) {
                (core ?? BindedCore).Cycle();
            } else { 
                (core ?? BindedCore).Latch();
            }
        }

        public static void CycleAndLatch(ICPU core = null)
        {
            var c = (core ?? BindedCore);
            if (GetNextEdge(core) == ClockEdge.Falling) c.Latch(); // fix if not ended on Latch();
            c.Cycle(); c.Latch();
            SetNextEdge(core, ClockEdge.Rising);
        }

        public static void ResetCPU(ICPU core = null, ResetUnit reset = ResetUnit.Core)
        {
            SetCoreFlag(core, _wascycled, false);
            ICPU c = core ?? BindedCore;
            if (reset.HasFlag(ResetUnit.Core)) c.Reset(preserveMemory: true);
            if (reset.HasFlag(ResetUnit.Registers)) c.RegisterFile.Reset();
            if (reset.HasFlag(ResetUnit.RAM)) c.RAM.Reset();
            if (reset.HasFlag(ResetUnit.ROM)) c.ROM.Reset();
        }                                           

        public static void InitSimulation(ICPU core, long breakpointAddress, long breakpointCycle, TimeSpan timeout, out CancellationToken breakToken)
        {
            BindedCore = core;
            if (core is SuperscalarCPU) {
                BreakpointPredicate = () => (CheckBreakPoint_Dispatch<SuperscalarCPU>() || CheckBreakPoint_Cycle<SuperscalarCPU>());
            } else if (core is ScalarCPU) {
                BreakpointPredicate = () => (CheckBreakPoint_GPC<ScalarCPU>() || CheckBreakPoint_Cycle<ScalarCPU>());
            } else { throw new ArgumentException($"{core.GetType().Name} is not supported {nameof(ICPU)} core!"); }

            BreakpointAddress = breakpointAddress;
            BreakpointCycle = breakpointCycle;
            SimulationTimeout = timeout;
            CancellationTokenSource?.Dispose();
            CancellationTokenSource = new CancellationTokenSource(timeout);
            breakToken = CancellationTokenSource.Token;
            SetCoreFlag(BindedCore, _siminitialized, true);
        }

        public static void RunInitializedSimulation()
        {
            if (false == GetCoreFlag(BindedCore, _siminitialized))
                throw new Exception("[Code Error]: Simulation not initialized. Use " + nameof(InitSimulation) + " method!");
            
            if (SimulationTask != null && false == SimulationTask.IsCompleted)
                throw new Exception("Invalid State: Simulation already running!");

            TaskCreationOptions options = TaskCreationOptions.LongRunning;
            SimulationTask?.Dispose();
            SimulationTask = new Task(CycleUntil, CancellationTokenSource.Token, options);
            SimulationTask.ContinueWith(TaskContinuationInvoker, TaskScheduler.FromCurrentSynchronizationContext());
            SimulationRunning = true;
            SimulationTask.Start();
        }

        public static bool CancelSimulation(int wait_ms)
        {
            CancellationTokenSource.Cancel();
            return SpinWait.SpinUntil(() => (false == SimulationTask.IsCanceled), wait_ms);
        }

        private static void TaskContinuationInvoker(Task task)
        {
            SetCoreFlag(BindedCore, _siminitialized, false);
            OnSimulationTaskFinishesExecution?.Invoke();

            if (task.IsFaulted)
                OnSimulationError?.Invoke(task.Exception);
            else if (task.IsCanceled || CancellationTokenSource.IsCancellationRequested)
                OnSimulationCancelled?.Invoke();
            else
                OnSimulationEndItself?.Invoke();

            SimulationRunning = false;
        }
        private static void CycleUntil()
        {
            Thread.CurrentThread.CurrentCulture = SimThreadCulture;
            try
            {
                if (GetNextEdge(BindedCore) == ClockEdge.Falling) BindedCore.Latch(); // if user steps does not end on Latch()
                do { BindedCore.Cycle(); BindedCore.Latch(); }
                while (false == (CancellationTokenSource.IsCancellationRequested || CheckBreakPoint()) );
            } 
            catch (Exception ex)
            {
                if(false == (ex is EnvironmentBreak))
                {
                    Console.WriteLine(ex.ToString());
                    System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex).Throw();
                }
            } 
        }
        
    }
}
