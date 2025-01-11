using superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Units;
using superscalar_arch_sim.RV32.ISA.Instructions;
using System;
using System.Linq;

namespace superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.FuncUnit
{
    public abstract class ExecuteUnit : IClockable
    {
        protected int CurrentCycle = 0;
        public string Name { get; protected set; }
        public ReservationStationCollection UnitReservationStations { get; private set; }
        public bool Stalling { get; set; }
        public int NeccessaryCycles { get; }

        /// <summary>
        /// Used <see cref="ReservationStation"/> get during base implementation of <see cref="Cycle"/> using 
        /// <see cref="ReservationStationCollection.GetOldestFromAllReadyOrDefault(bool?)"/> as filtering method.
        /// </summary>
        public ReservationStation UsedReservationStation { get; protected set; }
        /// <summary>
        /// Currently processed <see cref="Instruction"/> get from <see cref="UsedReservationStation"/>
        /// in base implementation of <see cref="Cycle"/>.
        /// </summary>
        public Instruction ProcessedInstruction { get; protected set; }
        
        /// <summary>Stores value being an effect of execution between <see cref="Cycle"/> and <see cref="Latch"/> invokes.</summary>
        public Int32 EffectiveValue = Int32.MaxValue;

        #region Events

        /// <summary>Invkoed from <see cref="ExecuteUnit"/> on <see cref="Latch"/> when instruction finishes execution and effective value is ready.</summary>
        internal EventHandler<ExecReadyArgs> EffectiveValueProduced;
        /// <summary>
        /// Invoked when division by 0 detected. 
        /// Result of an operation is set to -1 (DIV) or value of dividend (REM) as defined in RISC-V ISA spec.
        /// </summary>
        internal EventHandler<StageDataArgs> DivisionByZero;
        /// <summary>
        /// Invoked when signed division overflows (on <see cref="Int32.MinValue"/> / -1). 
        /// Result of an operation is set to <see cref="Int32.MinValue"/> (DIV) or 0 (REM) as defined in RISC-V ISA spec.
        /// </summary>
        internal EventHandler<StageDataArgs> SignedOverflow;
        #endregion

        protected ExecuteUnit(ReservationStationCollection stations, string name=nameof(ExecuteUnit), int cyclesToComplete = 1) 
        {
            UnitReservationStations = stations;
            NeccessaryCycles = cyclesToComplete;
            Name = name;
            Reset();
        }
        internal void SetStations(ReservationStationCollection stations)
        {
            UnitReservationStations = stations;
        }
        /// <summary>Allows to set intermittent processing properties. On default, values are discarded (parameters are <see langword="null"/>).</summary>
        /// <param name="usedRS">New value of <see cref="UsedReservationStation"/>.</param>
        /// <param name="processedI32">New value of <see cref="ProcessedInstruction"/>.</param>
        public void SetIntermittentContent(ReservationStation usedRS = null, Instruction processedI32 = null)
        {
            UsedReservationStation = usedRS;
            ProcessedInstruction = processedI32;
        }

        public bool HasStation(int tag)
        {
            return UnitReservationStations.HasTag(tag);
        }

        public ReservationStation GetStationOrDefault(int tag)
        {
            return UnitReservationStations.GetStationByTagOrDefault(tag);
        }

        public virtual bool IsReady()
        {
            return (CurrentCycle >= NeccessaryCycles);
        }

        public virtual void Cycle()
        {
            if (false == Stalling) 
            {
                ++CurrentCycle;
            }
        }
        public virtual void Latch()
        {
            if (false == Stalling)
            {
                CurrentCycle = 0;
            }
        }

        public virtual void Reset()
        {
            Stalling = default;
            CurrentCycle = 0;
            ProcessedInstruction = Instruction.NOP;
            UsedReservationStation = default;
            UnitReservationStations?.ResetAll();
        }

        public override string ToString()
        {
            return (Name + (ProcessedInstruction is null ? string.Empty : $" [{ProcessedInstruction}]"));
        }
    }
}
