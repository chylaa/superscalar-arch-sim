using superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Units;
using superscalar_arch_sim.RV32.Hardware.Register;
using superscalar_arch_sim.RV32.ISA.Instructions;
using System;

namespace superscalar_arch_sim.RV32.Hardware.Pipeline
{
    /// <summar>Generic <see langword="delegate"/> for user-interface-related <see cref="EventHandler"/>s.</summary>
    public delegate void GUIEventHandler<Args>(object sender, Args e) where Args : EventArgs;

    public delegate void InstructionEventHandler<TData>(object sender, InstructionDataEventArgs<TData> e);
    public delegate void DataWriteEventHandler(object sender, DataWriteEventArgs e);
    public delegate void DynamicStageROBDataEventHandler(ROBEntry sender, StageDataArgs e);

    /// <summary>
    /// General <see cref="EventArgs"/> class for <see cref="Stage"/> <see cref="EventHandler"/>s.
    /// </summary>
    public class StageDataArgs : EventArgs 
    {
        new public static StageDataArgs Empty = new StageDataArgs();

        /// <summary>Instruction related to event.</summary>
        public Instruction Instruction;
        /// <summary>Handle to <see cref="Register32"/> related to passed data.</summary>
        public Register32 DataA;
        /// <summary>Handle to <see cref="Register32"/> related to passed data.</summary>
        public Register32 DataB;
        /// <summary>Handle to Local PC of <see cref="Instruction"/> at event.</summary>
        public Register32 LocalPC;

        #region Constructors
        /// <summary>Initializes new instance of <see cref="StageDataArgs"/> class.</summary>
        public StageDataArgs(): base() { }

        /// <summary>
        /// Initializes new instance of <see cref="StageDataArgs"/> class with related
        /// <see cref="ISA.Instructions.Instruction"/> <see cref="Instruction"/> property set.
        /// </summary>
        public StageDataArgs(Instruction i32)
        {
            Instruction = i32;
        }
        /// <summary>
        /// Initializes new instance of <see cref="StageDataArgs"/> class with related
        /// <see cref="ISA.Instructions.Instruction"/> <see cref="Instruction"/> and <see cref="Register32"/>
        /// <see cref="DataA"/>, <see cref="DataB"/>, <see cref="LocalPC"/> (each can be <see langword="null"/>) 
        /// properties set.
        /// </summary>
        public StageDataArgs(Instruction i32, Register32 reg1=null, Register32 reg2=null, Register32 lpc=null)
        {
            Instruction = i32;
            DataA = reg1; DataB = reg2; 
            LocalPC = lpc;
        }
        /// <summary>
        /// Initializes new instance of <see cref="StageDataArgs"/> class with related
        /// <see cref="ISA.Instructions.Instruction"/> <see cref="Instruction"/> and new temporary <see cref="Register32"/>
        /// <see cref="DataA"/>, <see cref="DataB"/>, with <paramref name="reg1val"/>, <paramref name="reg2val"/> values
        /// written (each can be <see langword="null"/>). <see cref="LocalPC"/>  is set base on <paramref name="lpc"/> param. 
        /// </summary>
        public StageDataArgs(Instruction i32, int reg1val, int? reg2val = null, Register32 lpc = null)
        : this(i32, new Register32(), null, lpc)
        {
            DataA.Write(reg1val);
            if (reg2val.HasValue) {
                DataB = new Register32();
                DataB.Write(reg2val.Value);
            }
        }
        /// <summary>
        /// Initializes new instance of <see cref="StageDataArgs"/> class with related
        /// <see cref="ISA.Instructions.Instruction"/> <see cref="Instruction"/> and new temporary <see cref="Register32"/>
        /// <see cref="DataA"/>, <see cref="DataB"/>, with <paramref name="reg1val"/>, <paramref name="reg2val"/> values
        /// written (each can be <see langword="null"/>). <see cref="LocalPC"/>  is set base on <paramref name="lpc"/> param. 
        /// </summary>
        public StageDataArgs(Instruction i32, uint reg1val, uint? reg2val = null, Register32 lpc = null)
        : this (i32, new Register32(), null, lpc)
        {
            DataA.WriteUnsigned(reg1val);
            if (reg2val.HasValue) {
                DataB = new Register32();
                DataB.WriteUnsigned(reg2val.Value);
            }
        }

        /// <summary>Allows to compare instance against <see cref="Empty"/> object.</summary>
        /// <returns><see langword="true"/> if current instance is <see cref="Empty"/> <see langword="false"/> otherwise.</returns>
        public bool IsEmpty() => ((Instruction is null) && (LocalPC is null) && (DataA is null) && (DataB is null));

        #endregion 
    }

    public class DataWriteEventArgs : EventArgs
    {
        public enum WriteDestination { Register, Memory }
        
        public ulong? Cycle { get; set; } = null;
        public Instruction I32 { get; }
        public UInt32 EffectiveValue { get; }
        public UInt32 TargetAddress { get; }
        public WriteDestination Destination { get; }
        public UInt32 InstructionAddress { get; }
        public DataWriteEventArgs(UInt32 value, UInt32 target, WriteDestination destination, Instruction i32, uint i32Address)
        {
            EffectiveValue = value;
            TargetAddress = target;
            Destination = destination;
            I32 = i32;
            InstructionAddress = i32Address;
        }
    }
    public class InstructionDataEventArgs<TData> : EventArgs
    {
        public readonly Instruction I32;
        public readonly TData Value;
        public InstructionDataEventArgs(Instruction i32, TData value) { I32 = i32; Value = value; }
    }

    public class ExecReadyArgs : EventArgs
    {
        new public static ExecReadyArgs Empty = new ExecReadyArgs();

        public readonly ReservationStation SourceStation = null;
        public int? EffectiveValue = null;

        protected ExecReadyArgs():base() { }
        public ExecReadyArgs(ReservationStation sourceStation, int? effectiveValue) : base()
        { 
            SourceStation = sourceStation;
            EffectiveValue = effectiveValue;
        }
    }
    public class DatapathEventArgs<T> : EventArgs where T : class
    {
        public T DataSource;
        public T DataDest;
        public Int32? Value;
        public DatapathEventArgs(T source, T dest, int? value)
        { DataSource = source; DataDest = dest; Value = value; }
    }
    public sealed class DatapathBufferEventArgs<T> : DatapathEventArgs<T> where T : class, IPipelineBuffer
    {
        new public static DatapathBufferEventArgs<T> Empty = new DatapathBufferEventArgs<T>(null, null, null, null, null);

        public Register32 RegSource;
        public Register32 RegDest;
        public DatapathBufferEventArgs(T dataSource, T dataDest,
                                 Register32 srcreg, Register32 dstreg,
                                 int? forwardedValue)
            : base(dataSource, dataDest, forwardedValue)
        {
            RegSource = srcreg;
            RegDest = dstreg;
        }
    }
    public sealed class DatapathEntryEventArgs<T> : DatapathEventArgs<T> where T : class, IUniqueInstructionEntry
    {
        new public static DatapathEntryEventArgs<T> Empty = new DatapathEntryEventArgs<T>(null, null, null);
        public DatapathEntryEventArgs(T source, T dest, int? value) 
            : base (source, dest, value) { }
    }
    public sealed class DatapathRegisterEventArgs : DatapathEventArgs<Register32>
    {
        new public static DatapathRegisterEventArgs Empty = new DatapathRegisterEventArgs(null, null, null);
        public DatapathRegisterEventArgs(Register32 source, Register32 dest, int? value)
            : base(source, dest, value) { }
    }
}
