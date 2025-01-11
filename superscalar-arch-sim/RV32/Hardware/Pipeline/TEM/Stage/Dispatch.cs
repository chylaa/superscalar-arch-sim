using superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Units;
using superscalar_arch_sim.RV32.Hardware.Register;
using superscalar_arch_sim.RV32.ISA;
using superscalar_arch_sim.RV32.ISA.Instructions;
using superscalar_arch_sim.Simulis;
using System;
using System.Linq;
using static superscalar_arch_sim.Simulis.Reports.SimReporter;
using I32Type = superscalar_arch_sim.RV32.ISA.ISAProperties.InstType;

namespace superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Stage
{
    /// <summary>
    /// (Also 'Issue'). Third stage of TEM pipeline (only present in superscalar units). 
    /// In this stage (still in-order), instructions are routed (issued) to the appropriate functional unit for execution.
    /// </summary>
    public class Dispatch : TEMStage
    {
        protected override int MaxInstructionsProcessedPerCycle { get => Settings.MaxIssuesPerClock; }

        /// <summary>Invoked when <see cref="Pipeline.Stage.ProcessedInstruction"/> <see cref="Instruction"/> is issued into any <see cref="ReservationStation"/>.</summary>
        public EventHandler<StageDataArgs> NewInstructionIssued;
        /// <summary>Invoked when effective address of Load <see cref="Instruction"/> is calculated and <see cref="ReservationStation.A"/> field updates.</summary>
        public GUIEventHandler<StageDataArgs> LoadEffectiveAddressCalculated;

        /// <summary>Number of instructions issued (written to <see cref="AllReservationStations"/>) in current cycle.</summary>
        public int IssuedInstructionsBundleSize { get; private set; } = 0;
        /// <summary>Number of instructions fetched from <see cref="IRDataQueue"/> on <see cref="Cycle"/>.</summary>
        private int SizeofBundleFetchedFromIQueue { get; set; } = 0;

        private readonly Register32File RegisterFile;
        private readonly InstructionDataQueue IRDataQueue;
        private readonly ReorderBuffer ROB;

        private ReservationStationCollection BranchStations;
        private ReservationStationCollection MemoryBuffer;
        private ReservationStationCollection IntStations;
        private ReservationStationCollection FloatStations;

        public ReservationStationCollection AllReservationStations { get; private set; }
        /// <summary>
        /// Returns minimum number between <see cref="ReorderBuffer.EmptyEntries"/> and <see cref="ReservationStationCollection.EmptyEntries"/>
        /// <br></br>(where <see cref="ReservationStationCollection"/> consist of <see cref="AllReservationStations"/>).
        /// </summary>
        public int NumberOfFreeReservationStationsAndROBEntries
            => Math.Min(ROB.EmptyEntries, AllReservationStations.EmptyEntries);

        public int NumberOfAddressCalculationsInSingleClock { get; private set;} = 1;

        #region Events
        /// <summary>Invoked when <see cref="Instruction.Illegal"/> flag is set during dispatch procedure of <see cref="ProcessedInstruction"/>.</summary>
        internal event EventHandler<StageDataArgs> AttemptedToDispatchIllegalInstruction;
        #endregion

        public Dispatch(Register32File regfile, InstructionDataQueue iQueue, ReorderBuffer rob)
            : base(HardwareProperties.TEMPipelineStage.Dispatch)
        {
            IRDataQueue = iQueue;
            RegisterFile = regfile;
            ROB = rob;
            Reset();
        }

        public void InitReservationStations()
        {
            var execute = (StageNext as Execute);
            BranchStations = execute.GetReservationStations(execute.BranchUnits);

            MemoryBuffer = new ReservationStationCollection(execute.MemoryUnits.SelectMany(mu => mu.UnitReservationStations).Distinct());
            IntStations = new ReservationStationCollection(execute.IntUnits.SelectMany(iu => iu.UnitReservationStations).Distinct());
            FloatStations = new ReservationStationCollection(execute.FPUnits.SelectMany(fpu => fpu.UnitReservationStations).Distinct());

            AllReservationStations = ReservationStationCollection.Concat(BranchStations, MemoryBuffer, IntStations, FloatStations);
        }
        public void ResizeReservationStationsFromExecUnits(Execute execute)
        {
            BranchStations.ResetResize(execute.GetReservationStations(execute.BranchUnits));

            MemoryBuffer.ResetResize(execute.MemoryUnits.SelectMany(mu => mu.UnitReservationStations).Distinct());
            IntStations.ResetResize(execute.IntUnits.SelectMany(iu => iu.UnitReservationStations).Distinct());
            FloatStations.ResetResize(execute.FPUnits.SelectMany(fpu => fpu.UnitReservationStations).Distinct());

            AllReservationStations.ContactResetResize(BranchStations, MemoryBuffer, IntStations, FloatStations);
        }
        public override void SetPreviousAndNextStages(TEMStage prev, TEMStage next)
        {
            base.SetPreviousAndNextStages(prev, next);
            InitReservationStations();
        }

        private ROBEntry GetROBEntryOrOperand(out int? operand, int? immsrc, int regsrc = 0)
        {
            if (immsrc.HasValue)
            {
                operand = immsrc.Value;
                return null;
            }
            if (regsrc >= ISAProperties.NO_INT_REGISTERS)
            {
                operand = null;
                return null;
            }

            int Q = RegisterFile.GetTagFromRegisterStatus(regsrc);
            if (Q == 0)
            {
                operand = RegisterFile[regsrc];
                return null;
            } else
            {
                operand = null;
                return ROB.GetEntryByTag(Q);
            }
        }

        private (ReservationStation, ROBEntry) GetFreeStationAndROBEntry(ReservationStationCollection stations)
        {
            for (int rsIndex = 0; rsIndex < stations.Count; rsIndex++)
            {
                if (stations[rsIndex].MarkedEmpty && false == stations[rsIndex].Busy)
                {
                    for (int tag = 0; tag < ROB.Count; tag++)
                    {
                        if (ROB[tag].MarkedEmpty && false == ROB[tag].Busy)
                        {
                            return (stations[rsIndex], ROB[tag]);
                        }
                    }
                    break; // if no ROB entry found for this station, leave, we need both to be non-null
                }
            }
            return (null, null);
        }

        #region Issue methods
        private ReservationStation IssueIType(ReservationStationCollection stations, PipeRegisters workingBuffer)
        {
            (ReservationStation rs, ROBEntry re) = GetFreeStationAndROBEntry(stations);
            if (rs is null || re is null)
                return null;

            Instruction i32 = workingBuffer.IR32;
            ROBEntry producer1 = GetROBEntryOrOperand(out int? Vj, immsrc: null, regsrc: i32.rs1);
            int Vk = i32.imm;

            int DstRegister = i32.rd;
            //if (i32.rd == 0) { DstRegister = null; } // skip register zero

            int faddr = workingBuffer.LocalPC.Read();
            int naddr = workingBuffer.NextPC.Read();
            ulong instrID = workingBuffer.InstructionIndex;
            rs.Issue(instrID, i32, producer1, null, re, Vj, Vk, a: null, fetchAddress: faddr, nextAddress: naddr);
            re.Issue(instrID, i32, Stage, DstRegister, value: null, rs.Tag, fetchAddress: faddr);
            return rs;
        }


        private ReservationStation IssueRType(ReservationStationCollection stations, PipeRegisters workingBuffer)
        {
            (ReservationStation rs, ROBEntry re) = GetFreeStationAndROBEntry(stations);
            if (rs is null || re is null)
                return null;

            Instruction i32 = workingBuffer.IR32;
            ROBEntry producer1 = GetROBEntryOrOperand(out int? Vj, immsrc: null, regsrc: i32.rs1);
            ROBEntry producer2 = GetROBEntryOrOperand(out int? Vk, immsrc: null, regsrc: i32.rs2);

            int DstRegister = i32.rd;
            //if (i32.rd == 0) { DstRegister = null; } // skip register zero
            int faddr = workingBuffer.LocalPC.Read();
            int naddr = workingBuffer.NextPC.Read();
            ulong instrID = workingBuffer.InstructionIndex;
            rs.Issue(instrID, i32, producer1, producer2, re, Vj, Vk, a: null, fetchAddress: faddr, nextAddress: naddr);
            re.Issue(instrID, i32, Stage, DstRegister, value: null, rs.Tag, fetchAddress: faddr);
            return rs;
        }
        private ReservationStation IssueUJType(ReservationStationCollection stations, PipeRegisters workingBuffer)
        {
            (ReservationStation rs, ROBEntry re) = GetFreeStationAndROBEntry(stations);
            if (rs is null || re is null)
                return null;

            Instruction i32 = workingBuffer.IR32;
            int Vk = i32.imm;
            int DstRegister = i32.rd;
            //if (i32.rd == 0) { DstRegister = null; } // skip register zero

            int faddr = workingBuffer.LocalPC.Read();
            int naddr = workingBuffer.NextPC.Read();
            ulong instrID = workingBuffer.InstructionIndex;
            rs.Issue(instrID, i32, null, null, re, null, Vk, a: null, fetchAddress: faddr, nextAddress: naddr);
            re.Issue(instrID, i32, Stage, DstRegister, value: null, rs.Tag, fetchAddress: faddr);
            return rs;
        }

        private ReservationStation IssueBType(ReservationStationCollection stations, PipeRegisters workingBuffer)
        {
            (ReservationStation rs, ROBEntry re) = GetFreeStationAndROBEntry(stations);
            if (rs is null || re is null)
                return null;

            Instruction i32 = workingBuffer.IR32;
            ROBEntry producer1 = GetROBEntryOrOperand(out int? Vj, immsrc: null, regsrc: i32.rs1);
            ROBEntry producer2 = GetROBEntryOrOperand(out int? Vk, immsrc: null, regsrc: i32.rs2);
            int target = i32.imm;

            int faddr = workingBuffer.LocalPC.Read();
            int naddr = workingBuffer.NextPC.Read();
            ulong instrID = workingBuffer.InstructionIndex;
            rs.Issue(instrID, i32, producer1, producer2, re, Vj, Vk, a: target, fetchAddress: faddr, nextAddress: naddr);
            re.Issue(instrID, i32, Stage, null, value: null, rs.Tag, fetchAddress: faddr);
            return rs;
        }
        private ReservationStation IssueLoad(ReservationStationCollection stations, PipeRegisters workingBuffer)
        {
            (ReservationStation rs, ROBEntry re) = GetFreeStationAndROBEntry(stations);
            if (rs is null || re is null)
                return null;

            Instruction i32 = workingBuffer.IR32;
            // address offset from register source 1
            ROBEntry producer2 = GetROBEntryOrOperand(out int? Vk, immsrc: null, regsrc: i32.rs1);
            int A = i32.imm;
            int DstRegister = i32.rd;
            //if (i32.rd == 0) { DstRegister = null; } // skip register zero

            int faddr = workingBuffer.LocalPC.Read();
            int naddr = workingBuffer.NextPC.Read();
            ulong instrID = workingBuffer.InstructionIndex;
            rs.Issue(instrID, i32, null, producer2, re, null, Vk, a: A, fetchAddress: faddr, nextAddress: naddr);
            rs.EffectiveAddressCalulated = false;
            rs.Busy = true; // Set Busy until Effective Address is calculated
            re.Issue(instrID, i32, Stage, DstRegister, value: null, rs.Tag, fetchAddress: faddr);
            return rs;
        }
        private ReservationStation IssueStore(ReservationStationCollection stations, PipeRegisters workingBuffer)
        {
            (ReservationStation rs, ROBEntry re) = GetFreeStationAndROBEntry(stations);
            if (rs is null || re is null)
                return null;

            Instruction i32 = workingBuffer.IR32;
            // address offset from register source 1
            ROBEntry producer2 = GetROBEntryOrOperand(out int? Vk, immsrc: null, regsrc: i32.rs1);
            // value to store from register source 2
            ROBEntry producer1 = GetROBEntryOrOperand(out int? Vj, immsrc: null, regsrc: i32.rs2);
            int A = i32.imm;

            int faddr = workingBuffer.LocalPC.Read();
            int naddr = workingBuffer.NextPC.Read();
            ulong instrID = workingBuffer.InstructionIndex;
            // Should be issued to RS - with ROB+Speculation Store's effective address is calculated at Execute,
            //  and write to MEM only after Complete (instruction no longer speculative) ?? [TODO]
            rs.Issue(instrID, i32, producer1, producer2, re, Vj, Vk, a: A, fetchAddress: faddr, nextAddress: naddr);
            rs.EffectiveAddressCalulated = false;
            rs.Busy = false; // Effective address calculation for stores, in Execute stage
            re.Issue(instrID, i32, Stage, null, value: null, rs.Tag, fetchAddress: faddr);
            return rs;
        }
        #endregion

        #region Effective Address Calculation



        // Loads and stores are maintained in program order through effective address calculation in AddressUnit
        // Method calculates effective address for LOAD operation (offset from register source 1 (in Vk) + immeditate value)
        private bool TrySearchMemoryBufferAndPerformEffectiveAddressCalculation()
        {
            bool EffectiveAddressNotCalculated(ReservationStation station)
                => false == station.EffectiveAddressCalulated.HasValue || false == station.EffectiveAddressCalulated.Value;

            ReservationStation rs = null;
            for (int i = 0; i < MemoryBuffer.Count; i++)
            {
                if (false == MemoryBuffer[i].MarkedEmpty && MemoryBuffer[i].Busy && Opcodes.IsLoad(MemoryBuffer[i].IR32))
                {
                    if (MemoryBuffer[i].InstructionDataReady() && EffectiveAddressNotCalculated(MemoryBuffer[i]))
                    {
                        rs = MemoryBuffer[i];
                    }
                }
            }
            if (rs != null)
            {
#if DEBUG
                if (rs.A is null || rs.OpVal2 is null)
                    throw new InvalidPipelineState($"{rs.IR32} from station {rs.Tag} does not have avaliable A or Vj values!");
#endif
                int effectiveAddress = (rs.OpVal2.Value + rs.A.Value); // calculate effective address value
                rs.A = effectiveAddress; // update effective address value
                rs.Busy = false; // no longer busy - can be executed
                rs.EffectiveAddressCalulated = true; // singnal for address unit to not fetch that again
                LoadEffectiveAddressCalculated?.Invoke(this, new StageDataArgs(rs.IR32, rs.A.Value, lpc: rs.FetchLocalPC));
                return true;
            }
            return false;
        }

        #endregion

        private ReservationStationCollection GetTargetStations(Instruction i32, out SimMeasures @class)
        {
            switch (i32.Type)
            {
                case I32Type.R:
                case I32Type.U:
                    @class = SimMeasures.IntUnit;
                    return IntStations;
                case I32Type.I:
                    if (Opcodes.IsLoad(i32)) {
                        @class = SimMeasures.MemUnit;
                        return MemoryBuffer;
                    } else if (Opcodes.IsJump(i32)){ // JARL
                        @class = SimMeasures.BranchUnit;
                        return BranchStations;
                    } else {
                        @class = SimMeasures.IntUnit;
                        return IntStations;
                    }
                case I32Type.S:
                    @class = SimMeasures.MemUnit;
                    return MemoryBuffer;
                case I32Type.B:
                    @class = SimMeasures.BranchUnit;
                    return BranchStations;
                case I32Type.J:
                    @class = SimMeasures.BranchUnit;
                    return BranchStations;
                default:
                    @class = 0;
                    return null;
            }
        }

        private bool TryPutInstructionToReservationStation(PipeRegisters workingBuffer, out int rsTag, out int robTag)
        {
            ReservationStation station = null;
            Instruction i32 = workingBuffer.IR32;
            var DefaultStageArgs = new StageDataArgs(i32, workingBuffer.LocalPC);
            switch (i32.Type)
            {
                case I32Type.R:
                    station = IssueRType(IntStations, workingBuffer);
                    break;

                case I32Type.U:
                    station = IssueUJType(IntStations, workingBuffer);
                    break;

                case I32Type.I:
                    if (Opcodes.IsLoad(i32))
                    {
                        station = IssueLoad(MemoryBuffer, workingBuffer); // LoadMemBuffer
                    } else if (Opcodes.IsJump(i32)) // JARL
                    {
                        station = IssueIType(BranchStations, workingBuffer);
                    } else
                    {
                        station = IssueIType(IntStations, workingBuffer);
                    }
                    break;

                case I32Type.S:
                    station = IssueStore(MemoryBuffer, workingBuffer); //StoreMemBuffer
                    break;

                case I32Type.B:
                    station = IssueBType(BranchStations, workingBuffer);
                    break;

                case I32Type.J: // JAL
                    station = IssueUJType(BranchStations, workingBuffer);
                    break;

                default:
                    if (i32.Illegal)
                    {
                        var args = new StageDataArgs(i32, lpc: _LocalPC);
                        AttemptedToDispatchIllegalInstruction?.Invoke(this, args);
                    }
                    break;
            }

            if (station is null)
            {
                rsTag = 0;
                robTag = 0;
                return false;
            } else
            {
                rsTag = station.Tag;
                robTag = station.ROBDest.Tag;
                NewInstructionIssued?.Invoke(this, DefaultStageArgs);
                return true;
            }
        }

        public override void Cycle()
        {
            bool @break = false;
            int sizeofBundle = 0; int intBundle = 0; int memBundle = 0; int branchBundle = 0;
            int robEmptyEntries = ROB.EmptyEntries;

            int GetAndPostIntSpecificBundleSize(SimMeasures target) { // shouldn't depend on SimMeasures, but have internal enum/id
                switch (target) {
                    case SimMeasures.BranchUnit: return branchBundle++;
                    case SimMeasures.IntUnit: return intBundle++;
                    case SimMeasures.MemUnit: return memBundle++;
                    default: return -1;
                }
            }

            if (robEmptyEntries == 0) {
                Reporter.I32Measures_CollectionFull.Collect(SimMeasures.ROB);
                @break = true;
            }

            for (int i = 0; i < LatchDataBuffers.Count; i++)
            {
                LatchDataBuffers[i].Reset();
                if (@break) continue; // reset all LatchDataBuffers

                // Enough instructions waiting in Queue and we have SOME place to put them
                if (IRDataQueue.Count > sizeofBundle && robEmptyEntries > sizeofBundle)
                {
                    // get but not dequeue yet, cause we dont know if corresponding stations are free
                    var item = IRDataQueue.Peek(index: sizeofBundle);
                    var i32 = item.IR32;
                    // break bundle if no more available ReservationStations/ROBEntry for this instruction
                    var targetStations = GetTargetStations(i32, out SimMeasures @class);
                    int stationBundleSize = GetAndPostIntSpecificBundleSize(@class);
                    if (targetStations.EmptyEntries <= stationBundleSize)
                    {
                        Reporter.I32Measures_CollectionFull.Collect(@class);
                        @break = true;
                        continue;
                    }
                    LatchDataBuffers[i].PassFrom(item);
                    LatchDataBuffers[i].IR32 = i32;
                    if (++sizeofBundle >= MaxInstructionsProcessedPerCycle)
                    {
                        @break = true;
                    }
                } else
                {
                    @break = true;
                }
            }
            SizeofBundleFetchedFromIQueue = sizeofBundle;
        }

        public override void Latch()
        {
            IssuedInstructionsBundleSize = 0;

            int addrCalcs = 0;
            InvokeEmptyLoadEffectiveAddressCalculatedEvent(); // UI Clear Event 
            while (TrySearchMemoryBufferAndPerformEffectiveAddressCalculation() && addrCalcs++ < NumberOfAddressCalculationsInSingleClock);

            // Get bundle of instructions fetched in Cycle() from IRDataQueue
            for (int i = 0; i < SizeofBundleFetchedFromIQueue; i++)
            {
                if (TryPutInstructionToReservationStation(LatchDataBuffers[i], out _, out int robTag))
                {
                    _ = IRDataQueue.Dequeue(); // issued successfully so yeet it from queue
                    ROB.UpdateHeadEntry();     // Update Head entry of reorder buffer
                    ++IssuedInstructionsBundleSize;

                    int rdest = LatchDataBuffers[i].IR32.rd; // ROB[robTag].Destination;
                    if (rdest != 0 && rdest != Instruction.OperandNotApplicable)
                    {
                        RegisterFile.SetProducerTag(rdest, robTag);
                    }
                } 
                else
                {
                    throw new InvalidPipelineState($"Issue bundle break at index {i} due to not-issued instruction " +
                        $"while theoretically {SizeofBundleFetchedFromIQueue} entries were free at Cycle().");
                }
            }
            if (Reporter.SimMeasuresEnabled)
            {
                Reporter.I32Measure_DispatchThroughput.Update(IssuedInstructionsBundleSize);
                Reporter.I32Measures_DispatchBundleSizeHist.Collect(IssuedInstructionsBundleSize);
                int allRsOccupiedEntries = (AllReservationStations.Count - AllReservationStations.EmptyEntries);
                int brRsOccupiedEntries  = BranchStations.Count - BranchStations.EmptyEntries;
                int memRsOccupiedEntries = MemoryBuffer.Count - MemoryBuffer.EmptyEntries;
                int intRsOccupiedEntries = IntStations.Count - IntStations.EmptyEntries;
                int robOccupiedEntries   = ROB.Count - ROB.EmptyEntries;
                Reporter.I32Measures_Size[SimMeasures.BranchUnit].Update(brRsOccupiedEntries);
                Reporter.I32Measures_Size[SimMeasures.MemUnit].Update(memRsOccupiedEntries);
                Reporter.I32Measures_Size[SimMeasures.IntUnit].Update(intRsOccupiedEntries);
                Reporter.I32Measures_Size[SimMeasures.ROB].Update(robOccupiedEntries);
                Reporter.I32Measures_SizeHist[SimMeasures.ROB].Collect(robOccupiedEntries);
                Reporter.I32Measures_RSSizeHist.Collect(allRsOccupiedEntries);
            }
        }
        public override bool IsReady()
        {
            return NumberOfFreeReservationStationsAndROBEntries > 0;
        }

        /// <summary>
        /// Calls event for address unit  <see cref="LoadEffectiveAddressCalculated"/> with empty arguments.
        /// Used for clearing relevant UI control.
        /// </summary>
        public void InvokeEmptyLoadEffectiveAddressCalculatedEvent()
            => LoadEffectiveAddressCalculated?.Invoke(this, StageDataArgs.Empty); // UI Clear Event for address unit 

        public override void Reset()
        {
            base.Reset();
            InvokeEmptyLoadEffectiveAddressCalculatedEvent();
            IssuedInstructionsBundleSize = 0;
            _LocalPC.Reset();
            _NextPC.Reset();
            NumberOfAddressCalculationsInSingleClock = Math.Min(
                MaxInstructionsProcessedPerCycle, Settings.NumberOfMemoryFunctionalUnits
            );
        }
    }
}
