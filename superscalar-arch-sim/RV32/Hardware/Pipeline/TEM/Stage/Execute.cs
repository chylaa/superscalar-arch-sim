using superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.FuncUnit;
using superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Units;
using superscalar_arch_sim.RV32.ISA.Instructions;
using superscalar_arch_sim.Simulis;
using System;
using System.Collections.Generic;
using System.Linq;
using static superscalar_arch_sim.Simulis.Reports.SimReporter;

namespace superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Stage
{
    /// <summary>
    /// Fourth stage of TEM pipeline. Instructions enters and leaves this stage out-of-order.
    /// <br></br>
    /// In superscalar pipeline design, execute stage consist of several specialized functional units,
    /// such as integer unit, floating-point unit and load-store unit. 
    /// <br></br>
    /// TEM Execute stage corresponds to a two-step pipelined unit consisting of the
    /// ALU and MEM stages of the scalar TYP pipeline.
    /// </summary>
    public class Execute : TEMStage
    {
        protected override int MaxInstructionsProcessedPerCycle => Settings.TotalNumberOfExecutionUnits;
        private readonly List<ExecuteUnit> ExecUnits;

        private ReorderBuffer ROB { get; set; }
        private ReservationStationCollection AllReservationStations { get; set; }

        public BranchUnit BranchUnits { get; private set; }
        public List<MemUnit> MemoryUnits { get; }
        public List<IntUnit> IntUnits { get; }
        public List<FPUnit> FPUnits { get; }

        public Action<ROBEntry> WriteCommonDataBus;

        #region Events
        /// <summary>
        /// Invoked when there is no <see cref="ReservationStation"/> in <see cref="UnitReservationStations"/>
        /// with <see cref="ReservationStation.Busy"/> flag set to <see langword="false"/>. <br></br> 
        /// <see cref="EventHandler.Invoke(object, EventArgs)"/> arguments are: <see langword="this"/> 
        /// <see cref="ExecuteUnit"/> (<see langword="object"/>) and <see cref="StageDataArgs.Empty"/> (<see cref="EventArgs"/>).
        /// </summary>
        public event EventHandler<StageDataArgs> NoReservationStationReady;
        /// <summary>
        /// Invoked when division by 0 detected. 
        /// Result of an operation is set to -1 (DIV) or value of dividend (REM) as defined in RISC-V ISA spec.
        /// </summary>
        public event EventHandler<StageDataArgs> DivisionByZero;
        /// <summary>
        /// Invoked when signed division overflows (on <see cref="Int32.MinValue"/> / -1). 
        /// Result of an operation is set to <see cref="Int32.MinValue"/> (DIV) or 0 (REM) as defined in RISC-V ISA spec.
        /// </summary>
        public event EventHandler<StageDataArgs> SignedOverflow;

        /// <summary>Invoked when an EBREAK <see cref="Instruction"/> is issued to execution.</summary>
        public event EventHandler<StageDataArgs> EnvironmentBreakIssuedToExecutionUnit;
        #endregion

        public ReservationStationCollection GetReservationStations(ExecuteUnit execUnit)
            => ExecUnits.Single(eu => eu == execUnit).UnitReservationStations;
        public ReservationStationCollection GetAllReservationStations()
            => AllReservationStations;

        public Execute(List<ExecuteUnit> execUnits, ReorderBuffer rob) 
            : base(HardwareProperties.TEMPipelineStage.Execute)
        {
            if (execUnits.Count != MaxInstructionsProcessedPerCycle)
                throw new InvalidPipelineState($"Number of '{nameof(ExecUnits)}' must be equal to '{nameof(MaxInstructionsProcessedPerCycle)}'");

            ROB = rob; 

            ExecUnits = new List<ExecuteUnit>();
            MemoryUnits = new List<MemUnit>();
            IntUnits = new List<IntUnit>();
            FPUnits = new List<FPUnit>();
            InitExecutionUnits(execUnits);
        }

        public void InitExecutionUnits(IEnumerable<ExecuteUnit> execUnits)
        {
            ExecUnits.Clear();
            MemoryUnits.Clear();
            IntUnits.Clear();
            FPUnits.Clear();

            ExecUnits.AddRange(execUnits);
            BranchUnits = execUnits.OfType<BranchUnit>().Single();
            MemoryUnits.AddRange(execUnits.OfType<MemUnit>());
            IntUnits.AddRange(execUnits.OfType<IntUnit>());
            FPUnits.AddRange(execUnits.OfType<FPUnit>());

            AllReservationStations = new ReservationStationCollection(ExecUnits.SelectMany(e => e.UnitReservationStations).Distinct());
            AssignEventHandlersToUnits();
        }

        public void AssignEventHandlersToUnits()
        {
            foreach (var unit in ExecUnits)
            {
                unit.SignedOverflow = SignedOverflow;
                unit.DivisionByZero = DivisionByZero;
                if (unit is IntUnit intunit) {
                    intunit.EnvironmentBreakIssuedToExecutionUnit = EnvironmentBreakIssuedToExecutionUnit;
                }
            }
        }
        public override bool IsReady()
        {
            return ExecUnits.Any(e => e.IsReady());
        }
        private bool IsMemoryAndEffectiveAddressNotCalculated(ReservationStation station)
        {   // set to false only for Load/Store, otherwise is null
            return (station.EffectiveAddressCalulated.HasValue && station.EffectiveAddressCalulated == false);
        }

        private ReservationStation GetFuncForStartingExecution_InOrder()
        {
            var oldestNotBusy = AllReservationStations.GetOldestFromAllOrDefault(busy:false, busyRelax: IsMemoryAndEffectiveAddressNotCalculated);
            if(oldestNotBusy != null && oldestNotBusy.InstructionDataReady())
            {
                if (oldestNotBusy.EffectiveAddressKnownOrNotApplicable() || Opcodes.IsStore(oldestNotBusy.IR32))
                {
                    bool NotUnresolvedBranch(ROBEntry robEntry)// TODO: could be excluding JAL as it would never flush pipeline? 
                         => Settings.Dynamic_AllowSpeculativeLoads || false == Opcodes.IsControlTransfer(robEntry.IR32) || robEntry.FinishedState >= HardwareProperties.TEMPipelineStage.Complete;
                    
                    var olderEntries = ROB.GetAllOlderEntries(newest: oldestNotBusy.ROBDest, markedEmpty: false);
                    if (olderEntries.All(NotUnresolvedBranch))
                    {
                        return oldestNotBusy;
                    }
                }
            }
            return null; // we're executing in order, so if oldestNotBusy one is not ready, abort. 
        }
        private ReservationStation GetFuncForStartingExecution_OutOfOrder(ExecuteUnit eu)
        {
            var notBusy = eu.UnitReservationStations.Where(s => (false == s.MarkedEmpty) && (false == s.Busy)).ToArray();
            Array.Sort(notBusy, comparer:ReservationStationCollection.InstructionIndexComparer);
            // search starting from "oldestNotBusy" instructions in RS
            foreach (ReservationStation rs in notBusy) 
            {
                if (Opcodes.IsLoad(rs.IR32) && rs.InstructionDataReady())
                {
                    bool IsNotDependentStore(ROBEntry robEntry)  
                        => ((false == Opcodes.IsStore(robEntry.IR32)) || (robEntry.ValueReady && (rs.A != robEntry.Destination)));
                    bool IsNotUnresolvedBranch(ROBEntry robEntry)// TODO: could be excluding JAL as it would never flush pipeline? 
                        => Settings.Dynamic_AllowSpeculativeLoads || false == Opcodes.IsControlTransfer(robEntry.IR32) || robEntry.FinishedState >= HardwareProperties.TEMPipelineStage.Complete;
                    bool NoDependencies(ROBEntry robEntry)
                        => IsNotDependentStore(robEntry) && IsNotUnresolvedBranch(robEntry);

                    var olderEntries = ROB.GetAllOlderEntries(newest: rs.ROBDest, markedEmpty: false);
                    if (olderEntries.All(NoDependencies)) 
                    {
                        return rs;
                    }
                    continue; // we're executing out of order so check if next waiting is notBusy and valid
                }    
                else if (Opcodes.IsStore(rs.IR32))  
                {
                    if (rs.OpVal2.HasValue)
                        return rs; // store can execute if address offset in Vk is ready
                }
                else if (rs.InstructionDataReady())
                {
                    return rs;  // rest of instruction that passed initial filtering can be executed immediately, return first of them
                }
            }
            return null;
        }
        private bool CanExecuteFrom(ReservationStation rs) => (rs != null && rs.IR32 != null);
        private void InOrderCycle()
        {
            for (int i = 0; i < MaxInstructionsProcessedPerCycle; i++)
                ExecUnits[i].SetIntermittentContent(usedRS: null, processedI32: null);

            int startedExecution = 0; int branchExecutions = 0; int memExecutions = 0; int intExecutions = 0;
            for (int i = 0; i < Settings.MaxIssuesPerClock; i++)
            {
                var selectedReservationStation = GetFuncForStartingExecution_InOrder();
                if (CanExecuteFrom(selectedReservationStation))
                {
                    for (int eIdx = 0; eIdx < MaxInstructionsProcessedPerCycle; eIdx++)
                    {
                        ExecuteUnit eu = ExecUnits[eIdx];
                        if (eu.HasStation(selectedReservationStation.Tag) && eu.UsedReservationStation is null) 
                        {
                            var processedInstruction = selectedReservationStation?.IR32;
                            eu.SetIntermittentContent(selectedReservationStation, processedInstruction);
                            selectedReservationStation.Busy = true;
                            selectedReservationStation.ROBDest.Busy = true;
                            eu.Cycle();
                            ++startedExecution;
                            switch (eu.Name)
                            {
                                case nameof(BranchUnit): ++branchExecutions; break;
                                case nameof(MemUnit): ++memExecutions; break;
                                case nameof(IntUnit): ++intExecutions; break;
                                default: break;
                            }
                            break;
                        }
                    }
                }
                else
                {
                    break;
                }
                if (branchExecutions > 0)
                {
                    break; // after branch exec do not execute nothing else
                }
            }
            if (Reporter.SimMeasuresEnabled)
            {
                Reporter.I32Measures_ExecuteSizeHist.Collect(startedExecution);
                Reporter.I32Measure_ExecuteThrougput.Update(startedExecution);
                Reporter.I32Measures_SizeHist[SimMeasures.BranchUnit].Collect(branchExecutions);
                Reporter.I32Measures_SizeHist[SimMeasures.MemUnit].Collect(memExecutions);
                Reporter.I32Measures_SizeHist[SimMeasures.IntUnit].Collect(intExecutions);
            }
        }
        private void OutOfOrderCycle()
        {
            int startedExecution = 0; int branchExecutions = 0; int memExecutions = 0; int intExecutions = 0;
            for (int i = 0; i < MaxInstructionsProcessedPerCycle; i++)
            {
                ExecuteUnit eu = ExecUnits[i];
                if (false == eu.Stalling)
                {
                    eu.SetIntermittentContent(usedRS: null, processedI32: null);
                    if (startedExecution < Settings.MaxIssuesPerClock)
                    {
                        var selectedReservationStation = GetFuncForStartingExecution_OutOfOrder(eu);
                        if (CanExecuteFrom(selectedReservationStation))
                        {
                            var processedInstruction = selectedReservationStation?.IR32;
                            eu.SetIntermittentContent(selectedReservationStation, processedInstruction);
                            selectedReservationStation.Busy = true;
                            selectedReservationStation.ROBDest.Busy = true;
                            eu.Cycle();
                            ++startedExecution;
                            switch (eu.Name)
                            {
                                case nameof(BranchUnit): ++branchExecutions; break;
                                case nameof(MemUnit): ++memExecutions; break;
                                case nameof(IntUnit): ++intExecutions; break;
                                default: break;
                            }
                        } else
                        {
                            NoReservationStationReady?.Invoke(eu, StageDataArgs.Empty);
                        }
                    }
                }
            }
            if (Reporter.SimMeasuresEnabled)
            {
                Reporter.I32Measures_ExecuteSizeHist.Collect(startedExecution);
                Reporter.I32Measure_ExecuteThrougput.Update(startedExecution);
                Reporter.I32Measures_SizeHist[SimMeasures.BranchUnit].Collect(branchExecutions);
                Reporter.I32Measures_SizeHist[SimMeasures.MemUnit].Collect(memExecutions);
                Reporter.I32Measures_SizeHist[SimMeasures.IntUnit].Collect(intExecutions);
            }
        }

        public override void Cycle()
        {
            if (Settings.CoreMode == Settings.DynamicCoreMode.InOrderExecution)
                InOrderCycle();
            else if (Settings.CoreMode == Settings.DynamicCoreMode.OutOfOrderExecution)
                OutOfOrderCycle();
            else
                throw new NotImplementedException($"Unknown mode of dynamic core: {Settings.CoreMode}.");
        }

        public override void Latch()
        {
            for (int i=0; i < ExecUnits.Count; i++) 
            {
                ExecuteUnit eu = ExecUnits[i];
                PipeRegisters latchBuffer = LatchDataBuffers[i];
                latchBuffer.Reset();

                if (false == eu.Stalling)
                {
                    eu.Latch();
                    if (eu.UsedReservationStation != null)
                    {
                        Instruction i32 = eu.ProcessedInstruction;              
                        eu.UsedReservationStation.ROBDest.FinishedState = Stage;
                        if (Opcodes.IsStore(i32))
                        {
                            eu.UsedReservationStation.ROBDest.Destination = eu.UsedReservationStation.A;
                            eu.UsedReservationStation.ROBDest.Value = eu.UsedReservationStation.OpVal1; 
                        }
                        else
                        {
                            eu.UsedReservationStation.ROBDest.Value = eu.EffectiveValue;
                            if (Settings.Dynamic_WriteCommonDataBusFromExecute)
                                WriteCommonDataBus(eu.UsedReservationStation.ROBDest);
                        }
                    }
                }
            }
        }

        public override void Reset()
        {
            base.Reset();
            ExecUnits?.ForEach(x => x.Reset());
        }
    }
}
