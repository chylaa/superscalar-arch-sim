using superscalar_arch_sim.RV32.Hardware.Register;
using superscalar_arch_sim.RV32.ISA.Instructions;
using System;
using System.ComponentModel;

namespace superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Units
{
    public class ReservationStation : IUniqueInstructionEntry, IEquatable<ReservationStation>
    {
        /// <summary>
        /// Creates new <see cref="ReservationStation"/> with passed <paramref name="tag"/>.
        /// Throws <see cref="ArgumentException"/> if <paramref name="tag"/> is less or equal 0.
        /// </summary>
        /// <param name="tag"><inheritdoc cref="Tag" path="/summary/node()"/></param>
        /// <exception cref="ArgumentException"></exception>
        public ReservationStation(int tag) 
        {
            if (tag > 0) Tag = tag;
            else throw new ArgumentException("Reservation station tag must be positive non-zero integer!");
            FetchLocalPC = new Register32("LPC", name:"Local PC");
            NextLocalPC = new Register32("NPC", name:"Next PC");
        }
        /// <summary>Uniquely indentifies <see cref="ReservationStation"/> between each of the <see cref="FuncUnit.ExecuteUnit"/>. </summary>
        public int Tag { get; } = -1;

        /// <summary><see cref="Instruction"/> stored in <see cref="ReservationStation"/>.</summary>
        public Instruction IR32 { get; set; } = null;
        /// <summary>Indicates that this <see cref="ReservationStation"/> and its <see cref="FuncUnit"/> is occupied.</summary>
        public bool Busy { get; set; } = false;

        #region Simulator-related properties
        /// <summary>Direct getter for <see cref="Instruction.ASM"/> of stored <see cref="IR32"/>. Might return <see langword="null"/>!</summary>
        //public string HumanReadableInstruction => I32?.ASM;

        /// <summary>
        /// [Qj] <see cref="ProducerOperand1.Tag"/> of <see cref="ROBEntry"/> that will produce value for <see cref="OpVal1"/>.
        /// If <see langword="is"/> <see langword="null"/> or equals 0- operand is already avaliable or unnecessary.
        /// </summary>
        public int? Qj => ProducerOperand1?.Tag;
        /// <summary>
        /// [Qk] <see cref="ProducerOperand2.Tag"/> of <see cref="ROBEntry"/> that will produce value for <see cref="OpVal2"/>.
        /// If <see langword="is"/> <see langword="null"/> or equals 0 - operand is already avaliable or unnecessary.
        /// </summary>
        public int? Qk => ProducerOperand2?.Tag;

        /// <summary>
        /// [Dest] <see cref="ROBEntry.Tag"/> of <see cref="ReorderBuffer"/>, pointing to <see cref="ROBEntry"/> 
        /// which should be updated with result of this <see cref="ReservationStation"/>.
        /// </summary>
        [DisplayName("ROB")]
        public int? Dest => ROBDest?.Tag;

        /// <summary>Allows to keep track of <see cref="IR32"/> order of assignment to <see cref="ReservationStation"/>.</summary>
        public ulong InstructionIndex { get; private set; } = 0;
        /// <summary>
        /// Flag used by Load/Store to indicate that <see cref="IR32"/> was in Address Unit. 
        /// For the rest of <see cref="Instruction"/>, property set to <see langword="null"/> (Not applicable).
        /// </summary>
        internal bool? EffectiveAddressCalulated { get; set; } = null;

        /// <summary>After execution is completed, <see cref="ReservationStation"/> containing instruction is marked as empty (can be reused).</summary>
        [DisplayName("Empty")]
        public bool MarkedEmpty { get; set; } = true;

        /// <summary>
        /// Register containing information about Fetch address of <see cref="IR32"/>.<br></br>
        /// Note: Field does not normally exist in real-hardware implementations of reservation stations.
        /// </summary>
        [DisplayName("LPC")]
        public Register32 FetchLocalPC { get; private set; }
        /// <summary>
        /// Register containing information about next fetch address after <see cref="IR32"/>.<br></br>
        /// Note: Field does not normally exist in real-hardware implementations of reservation stations.
        /// </summary>
        [DisplayName("NPC")]
        public Register32 NextLocalPC { get; private set; }
        #endregion

        /// <summary>
        /// [Qj] <see cref="ROBEntry"/> that will produce value for <see cref="OpVal1"/>.
        /// If is <see langword="null"/> - operand is already avaliable or unnecessary.
        /// </summary>
        internal ROBEntry ProducerOperand1 { get; set; }
        /// <summary>
        /// [Qk] <see cref="ROBEntry"/> that will produce value for <see cref="OpVal2"/>.
        /// If is <see langword="null"/> - operand is already avaliable or unnecessary.
        /// </summary>
        internal ROBEntry ProducerOperand2 { get; set; }

        /// <summary>[Vj] Value of <see cref="Instruction"/> first source operand.</summary>
        [DisplayName("Vj")]
        public int? OpVal1 { get; set; }

        /// <summary>
        /// [Vk] Value of <see cref="Instruction"/> second source operand. For <see cref="ISA.ISAProperties.InstType.I"/> instructions, 
        /// holds immediate field (except for offset in <see cref="Opcodes.OP_I_TYPE_LOADS"/> (same as in <see cref="ISA.ISAProperties.InstType.S"/>))
        /// </summary>
        [DisplayName("Vk")]
        public int? OpVal2 { get; set; }

        /// <summary>
        /// [Dest] Destination <see cref="ROBEntry"/> of <see cref="ReorderBuffer"/>, 
        /// which should be updated with result of this <see cref="ReservationStation"/>.
        /// </summary>
        internal ROBEntry ROBDest { get; set; }

        /// <summary>
        /// Additional operand that stores information for memory address calculation for <see cref="Opcodes.OP_I_TYPE_LOADS"/> and <see cref="Opcodes.OP_S_TYPE_STORE"/>.
        /// Initially, processed <see cref="Instruction.imm"/> value is stored here; after address calculation, the effective address.
        /// <br></br>([???] In case of <see cref="Opcodes.OP_B_TYPE_BRANCH"/> instructions, after decoding Imm value is stored here, and after execution - target address.)
        /// </summary>
        public int? A { get; set; }

        /// <summary>Checks if <see cref="Instruction"/> in <see cref="ReservationStation"/> has its arguments ready to read wihout hazards.</summary>
        /// <returns><see langword="true"/> if both <see cref="ProducerOperand1"/> and <see cref="ProducerOperand2"/> are not set.</returns>
        public bool InstructionDataReady()
        {
            return ((ProducerOperand1 is null) && (ProducerOperand2 is null));
        }
        /// <summary>
        /// Checks if <see cref="Instruction"/> in <see cref="ReservationStation"/> has <see cref="EffectiveAddressCalulated"/> (if it is Load/Store)
        /// or property is <see langword="null"/> (in case of other ones).
        /// </summary>
        /// <returns><see langword="true"/> if <see cref="EffectiveAddressCalulated"/> or is <see langword="null"/>, otherwise <see langword="false"/>.</returns>
        public bool EffectiveAddressKnownOrNotApplicable()
        {
            return (EffectiveAddressCalulated is null) || (EffectiveAddressCalulated == true);
        }

        /// <summary>
        /// Assigns passed values to coressponding fields in current <see cref="ReservationStation"/> and sets
        /// <see cref="MarkedEmpty"/> field to <see langword="false"/>. Assigns shallow copy of <paramref name="i32"/> to <see cref="IR32"/> <see cref="Instruction"/>.<br></br>
        /// Throws <see cref="InvalidPipelineState"/> if <see cref="Busy"/> was already set or <see cref="MarkedEmpty"/> was <see langword="false"/>.
        /// </summary>
        /// <param name="instructionIndex"><inheritdoc cref="InstructionIndex" path="/summary/node()"/></param>
        /// <param name="i32"><inheritdoc cref="IR32" path="/summary/node()"/></param>
        /// <param name="producer1"><inheritdoc cref="ProducerOperand1" path="/summary/node()"/></param>
        /// <param name="producer2"><inheritdoc cref="ProducerOperand2" path="/summary/node()"/></param>
        /// <param name="destination"><inheritdoc cref="ROBDest" path="/summary/node()"/></param>
        /// <param name="operand1"><inheritdoc cref="OpVal1" path="/summary/node()"/></param>
        /// <param name="operand2"><inheritdoc cref="OpVal2" path="/summary/node()"/></param>
        /// <param name="a"><inheritdoc cref="A" path="/summary/node()"/></param>
        /// <exception cref="InvalidPipelineState"></exception>
        public void Issue(ulong instructionIndex, Instruction i32, ROBEntry producer1, ROBEntry producer2, 
            ROBEntry destination, int? operand1, int? operand2, int? a, int fetchAddress = Register32.DefaultValue, int nextAddress = Register32.DefaultValue) 
        {
#if DEBUG
            if (Busy) 
                throw new InvalidPipelineState($"{nameof(Busy)} == true when issuing {i32} instruction {instructionIndex}!");
            if (false == MarkedEmpty)
                throw new InvalidPipelineState($"{nameof(MarkedEmpty)} == false when issuing {i32} instruction {instructionIndex}!");
#endif
            EffectiveAddressCalulated = null;
            MarkedEmpty = false;
            InstructionIndex = instructionIndex;
            IR32 = i32;
            //Busy = true;
            ProducerOperand1 = producer1;
            ProducerOperand2 = producer2;
            OpVal1 = operand1;
            OpVal2 = operand2;
            ROBDest = destination;
            A = a;
            FetchLocalPC.Write(fetchAddress);
            NextLocalPC.Write(nextAddress);
        }
        /// <summary>Copies all properties of <paramref name="src"/> into current instace of <see cref="ReservationStation"/>.</summary>
        /// <param name="src">Source of operands.</param>
        public void IssueFrom(ReservationStation src)
        {
            Issue(src.InstructionIndex, src.IR32, src.ProducerOperand1, src.ProducerOperand2, 
                src.ROBDest, src.OpVal1, src.OpVal2, src.A, src.FetchLocalPC.Read(), src.NextLocalPC.Read());
        }
        /// <summary>Sets all writable parameters to <see langword="default"/> value.</summary>
        public void Reset()
        {
            InstructionIndex = default;
            IR32 = default;
            Busy = default;
            ProducerOperand1 = default;
            ProducerOperand2 = default;
            ROBDest = default;
            OpVal1 = default;
            OpVal2 = default;
            A = default;
            EffectiveAddressCalulated = default;
            MarkedEmpty = true;
            FetchLocalPC.Reset();
            NextLocalPC.Reset();
        }

        public bool HasDataReadyAndBusyEquals(bool? busyFilter = null)
        {
            return InstructionDataReady() && (busyFilter is null || Busy == busyFilter);
        }

        /// <summary>Cast <paramref name="station"/> to <see langword="int"/> value being <see cref="Tag"></see>.</summary>
        public static explicit operator Int32 (ReservationStation station)
        {
            return station.Tag;
        }
        public override int GetHashCode()
        {
            return Tag.GetHashCode(); // essentialy => this.Tag
        }

        public bool Equals(ReservationStation other)
        {
            return Tag == other.Tag;
        }
    }
}
