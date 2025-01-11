using superscalar_arch_sim.RV32.Hardware.Register;
using superscalar_arch_sim.RV32.ISA.Instructions;
using System;
using System.ComponentModel;
using static superscalar_arch_sim.RV32.Hardware.HardwareProperties;

namespace superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Units
{
    /// <summary>Represents single <see cref="ReorderBuffer"/> entry.</summary>
    public class ROBEntry : IUniqueInstructionEntry, IEquatable<ROBEntry>, IEquatable<int>
    {
        [DisplayName("Entry")]
        public int Tag { get; } = -1;
        [DisplayName("Busy")]
        public bool Busy { get; set; } = false;
        [DisplayName("I32")]
        public Instruction IR32 { get; set; } = null;
        [DisplayName("State")]
        public TEMPipelineStage FinishedState { get; set; } = TEMPipelineStage.None;
        
        /// <summary>
        /// Holds destination of <see cref="IR32"/> <see cref="Instruction"/>. 
        /// For Register-Register instructions this can be a register index,
        /// or memory address in case of <see cref="Opcodes.OP_S_TYPE_STORE"/> <see cref="Instruction"/>.
        /// </summary>
        [DisplayName("Dest")]
        public Int32? Destination { get; set; } = null;
        [DisplayName("Value")]
        public Int32? Value { get; set; } = null;

        [DisplayName("Empty")]
        public bool MarkedEmpty { get; set; } = true;

        /// <summary>
        /// Register containing information about Fetch address of <see cref="IR32"/>.<br></br>
        /// Note: Field does not normally exist in real-hardware implementations of ROB.
        /// </summary>
        [DisplayName("LPC")]
        public Register32 FetchLocalPC { get; private set; }

        /// <summary>
        /// Readonly flag, indicates that <see cref="Value"/> field has valid value (<see cref="IR32"/> was completed).
        /// </summary>
        /// <remarks>
        /// In case of store instrucitons (<see cref="ISA.ISAProperties.InstType.S"/>), indicates that effective address was calculated
        /// but value is not necessary already written to memory!
        /// </remarks>
        /// <returns>
        /// <see langword="true"/> if <see cref="FinishedState"/> is 
        /// <see cref="TEMPipelineStage.Execute"/>, <see cref="TEMPipelineStage.Complete"/> or <see cref="TEMPipelineStage.Retire"/>
        /// .</returns> 
        internal bool ValueReady => (FinishedState != TEMPipelineStage.None && FinishedState >= TEMPipelineStage.Execute);
        
        /// <summary>Allows to keep track of <see cref="IR32"/> order of assignment to <see cref="ReorderBuffer"/>.</summary>
        public ulong InstructionIndex { get; private set; } = 0;

        /// <summary>Allows to keep track of <see cref="ReservationStation"/> associated with <see langword="this"/> <see cref="ROBEntry"/>.</summary>
        internal int? ReservationStationTag { get; private set; } = null;

        public ROBEntry(int entryTag, TEMPipelineStage state) 
        { 
            Tag = entryTag; 
            FinishedState = state;
            MarkedEmpty = true;
            FetchLocalPC = new Register32("LPC", name: "Local PC");
        }
        /// <summary>Resets all properties to default state.</summary>
        public void Reset()
        {
            Busy = false;
            IR32 = null;
            FinishedState = TEMPipelineStage.None;
            Destination = null;
            Value = null;
            MarkedEmpty = true;
            InstructionIndex = ulong.MaxValue;
            ReservationStationTag = null;
            FetchLocalPC.Reset();
        }
        /// <summary>
        /// Sets all passed parameters to corresponding properties and <see cref="MarkedEmpty"/> to <see langword="false"/>.
        /// Throws <see cref="InvalidPipelineState"/> if <see cref="MarkedEmpty"/> was already <see langword="false"/>.
        /// </summary>
        /// <exception cref="InvalidPipelineState"></exception>
        public void Issue(ulong instructionIndex, Instruction i32, TEMPipelineStage state, int? dest, int? value, int rsTag, int fetchAddress)
        {
#if DEBUG
            if (false == MarkedEmpty) { 
                throw new InvalidPipelineState($"ROB Entry {Tag} not marked empty at 'Issue'"); 
            }
#endif
            InstructionIndex = instructionIndex;
            //Busy = true;
            IR32 = i32;
            FinishedState = state;
            Destination = dest;
            Value = value;
            MarkedEmpty = false;
            ReservationStationTag = rsTag;
            FetchLocalPC.Write(fetchAddress);
        }
        /// <summary>Cast <paramref name="entry"/> to <see langword="int"/> value being <see cref="Tag"></see></summary>
        public static explicit operator Int32(ROBEntry entry)
        {
            return entry.Tag;
        }
        public override int GetHashCode()
        {
            return Tag.GetHashCode(); // essentialy => this.Tag
        }

        public bool Equals(ROBEntry other)
        {
            return Tag == other.Tag;
        }
        public bool Equals(int other)
        {
            return Tag == other;
        }
    }
}
