using System;
using System.Collections.Generic;
using System.Linq;
using superscalar_arch_sim.RV32.Hardware.Register;
using superscalar_arch_sim.RV32.Hardware.Units;
using superscalar_arch_sim.RV32.Hardware.Pipeline;
using superscalar_arch_sim.Simulis;
using superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.FuncUnit;
using superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Stage;
using superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Units;
using superscalar_arch_sim.RV32.ISA.Instructions;
using superscalar_arch_sim.RV32.ISA;
using superscalar_arch_sim.Simulis.Reports;
using superscalar_arch_sim.Utilis;

namespace superscalar_arch_sim.RV32.Hardware.CPU
{
    /// <summary>
    /// Out-Of-Order Superscalar Central Processing Unit
    /// </summary>
    public class SuperscalarCPU : ICPU
    {
        /// <summary>Number of clock cycles performed by <see cref="ScalarCPU"/> (see <see cref="CycleAndLatch"/>).</summary>
        public ulong ClockCycles { get => SimReport.ClockCycles; set => SimReport.ClockCycles = value; }
        /// <summary>
        /// Raw short-formatted value of <see cref="GlobalProgramCounter"/>.
        /// Set avaliable as <see langword="value"/> <see cref="int.Parse(string)"/> into <see cref="Register32.Write(int)"/>.
        /// </summary>
        public string GlobalPCString
        {
            get => GlobalProgramCounter.ShortFormat;
            set => GlobalProgramCounter.Write(string.IsNullOrWhiteSpace(value) ? 0 : int.Parse(value));
        }
        public bool EBreakInFlight { get; private set; } = false;
        public bool FetchStalling { get => Fetch.Stalling; }
        public string[] StageNames => Pipeline.Select(s => s.Name).ToArray();

        public TEMStage[] Pipeline { get; private set; }
        public List<ExecuteUnit> ExecuteUnits { get; private set; }
        public BranchPredictor BranchPredictor { get; private set; }

        #region Memory
        public Memory.Memory RAM { get; private set; }
        public Memory.Memory ROM { get; private set; }
        public MemoryManagmentUnit MMU { get; private set; }
        public uint ROMStart { get => ROM.Origin; set => ROM.ResizeMemory(value, ROM.ByteSize); }
        public uint RAMStart { get => RAM.Origin; set => RAM.ResizeMemory(value, RAM.ByteSize); }
        public uint ROMSize { get => ROM.ByteSize; set => ROM.ResizeMemory(ROM.Origin, value); }
        public uint RAMSize { get => RAM.ByteSize; set => RAM.ResizeMemory(RAM.Origin, value); }
        #endregion
        #region Stages
        public Fetch Fetch { get; private set; }
        public Decode Decode { get; private set; }
        public Dispatch Dispatch { get; private set; }
        public Execute Execute { get; private set; }
        public Complete Complete { get; private set; }
        public Retire Retire { get; private set; }
        #endregion
        #region Buffers
        public InstructionDataQueue IRDataQueue { get; private set; }
        public ReorderBuffer ROB { get; private set; }
        #endregion

        #region Execute Units
        public BranchUnit BranchExecuteUnit { get; private set; }
        public List<MemUnit> MemoryBufferUnits { get; private set; }
        public List<IntUnit> IntegerExecuteUnits { get; private set; }

        #endregion

        #region Registers
        public Register32 GlobalProgramCounter { get; private set; }
        public Register32File RegisterFile { get; private set; }
        public Register32File FPRegisterFile { get; private set; }
        #endregion

        #region Pipeline Settings

        /// <summary>Width of pipeline, expressed as <see cref="Settings.MaxIssuesPerClock"/> number./summary>
        public int PipelineWidth => Settings.MaxIssuesPerClock;
        /// <summary>Number of cycles that takes to pass instruction through whole pipeline.</summary>
        public int NumberOfPipelineStages => Pipeline.Length;

        public event EventHandler<DatapathEntryEventArgs<IUniqueInstructionEntry>> StoreValueBypassedToLoad;

        public ReportGenerator ReportGenerator { get; }
        public SimReporter SimReport { get => ReportGenerator.Reporter; }

        #endregion

        readonly HashSet<ROBEntry> ENewerPlaceholder = new HashSet<ROBEntry>();
        readonly HashSet<ROBEntry> EOlderPlaceholder = new HashSet<ROBEntry>();
        #region State variables 

        private bool RefetchNeeded = false;
        private int RefetchTargetAddress = int.MaxValue;

        private bool AddressMissprediction = false;
        private bool ConditionMissprediction = false;
        private ROBEntry MispredictionInstructionSource = null;
        private uint ActualTargetAddress = uint.MaxValue;
        private uint PredictedTargetAddress = uint.MaxValue;

        #endregion
        #region Events
        /// <summary><inheritdoc cref="FetchAddressSelector.PCSelectedFromPipelineRegister"/></summary>
        public event GUIEventHandler<DatapathRegisterEventArgs> PCSelectedFromDecode;
        /// <summary>
        /// Invoked when effective address of Load <see cref="Instruction"/> 
        /// is calculated in <see cref="Dispatch"/> stage and <see cref="ReservationStation.A"/> field updates.
        /// </summary>
        public event GUIEventHandler<StageDataArgs> LoadEffectiveAddressCalculated
        {
            add => Dispatch.LoadEffectiveAddressCalculated += value;
            remove => Dispatch.LoadEffectiveAddressCalculated -= value;
        }
        #endregion

        private Dictionary<TEMStage, uint> StageStallAmount { get; set; }
        private IntUnit GetNewInt(ReservationStationCollection stations) => new IntUnit(stations);
        private MemUnit GetNewMem(ReservationStationCollection stations) => new MemUnit(stations, MMU);

        public SuperscalarCPU()
        {
            #region Memory

            ROM = new Memory.Memory(Settings.OriginROMAddress, Settings.ROMBytesLength, "ROM")
            {
                Access = HardwareProperties.MemoryAccess.Read
            };
            RAM = new Memory.Memory(Settings.OriginRAMAddress, Settings.RAMBytesLength, "RAM")
            {
                Access = HardwareProperties.MemoryAccess.RW
            };
            MMU = new MemoryManagmentUnit(RAM, ROM);

            #endregion
            #region Registers
            GlobalProgramCounter = new Register32(name: "PC", abiMnem: "GPC", meaning: "Program Counter");
            RegisterFile = Reg32FileFactory.InitArchitecturalIntegerRegisters();
            //FPRegisterFile = Reg32FileFactory.InitArchitecturalFloatRegisters();
            //ControlStatusRegisters = Reg32FileFactory.InitControlStatusRegisterFile();
            //ControlStatusRegisters.Origin = 0x0000_0000;
            #endregion

            BranchPredictor = new BranchPredictor() { Enabled = true };
            IRDataQueue = new InstructionDataQueue(capacity: Settings.InstructionQueueCapacity);
            ROB = new ReorderBuffer(Settings.NumberOfReorderBufferEntries);


            BranchExecuteUnit = new BranchUnit(new ReservationStationCollection(Utilis.Utilis.GetUniqueInts(1, Settings.NumberOfBranchReservationStations)));
            MemoryBufferUnits =   CreateExecutionUnits<MemUnit>(MemoryBufferUnits, GetNewMem, Settings.NumberOfMemoryFunctionalUnits, Settings.NumberOfReservationStationsPerMemoryUnit, BranchExecuteUnit);
            IntegerExecuteUnits = CreateExecutionUnits<IntUnit>(IntegerExecuteUnits, GetNewInt, Settings.NumberOfIntegerFunctionalUnits, Settings.NumberOfReservationStationsPerIntegerUnit, MemoryBufferUnits.Last());
            
            ExecuteUnits = new List<ExecuteUnit>() { BranchExecuteUnit };
            ExecuteUnits.AddRange(MemoryBufferUnits);
            ExecuteUnits.AddRange(IntegerExecuteUnits);

            #region Stages 

            Fetch = new Fetch(MMU, GlobalProgramCounter, BranchPredictor);
            Decode = new Decode(IRDataQueue);
            Dispatch = new Dispatch(RegisterFile, IRDataQueue, ROB);
            Execute = new Execute(ExecuteUnits, ROB);
            Complete = new Complete(ExecuteUnits, ROB, BranchPredictor);
            Retire = new Retire(ExecuteUnits, ROB, RegisterFile, MMU, BranchPredictor);

            Fetch.SetPreviousAndNextStages(null, Decode);
            Decode.SetPreviousAndNextStages(Fetch, Dispatch);
            Dispatch.SetPreviousAndNextStages(Decode, Execute);
            Execute.SetPreviousAndNextStages(Dispatch, Complete);
            Complete.SetPreviousAndNextStages(Execute, Retire);
            Retire.SetPreviousAndNextStages(Complete, null);

            Pipeline = new TEMStage[] { Fetch, Decode, Dispatch, Execute, Complete, Retire };
            #endregion
            StageStallAmount = new Dictionary<TEMStage, uint>() {
                {Fetch, 0}, {Decode, 0}, {Dispatch, 0}, {Execute, 0}, {Complete, 0 }, {Retire, 0 }
            };
            ReportGenerator = new ReportGenerator(Pipeline.Length, platformPrefix: 'D');
            Array.ForEach(Pipeline, stage => stage.SetSimReporter(SimReport));
            Reset(preserveMemory: false);
            InitPipelineEventHandlers();
        }
        #region Execute units and Reservation Stations setup methods
        private List<TUnit> CreateExecutionUnits<TUnit>(List<TUnit> units, Func<ReservationStationCollection, TUnit> @new, int numOfUnits, int rsPerUnit, ExecuteUnit previous) 
            where TUnit : ExecuteUnit
        {
            units = units ?? new List<TUnit>();
            units.Clear();
            
            int firstIntRSTag = (1 + previous.UnitReservationStations.Max(x => x.Tag));
            int numberOfRS = (numOfUnits * rsPerUnit);
            var stations = new ReservationStationCollection(Utilis.Utilis.GetUniqueInts(firstIntRSTag, numberOfRS));

            for (int i = 0; i < numOfUnits; i++)
                units.Add(@new(stations));
            return units;
        }
        private void ResetReservationStationsCount()
        {
            ExecuteUnits.Clear();

            BranchExecuteUnit.SetStations(new ReservationStationCollection(Utilis.Utilis.GetUniqueInts(1, Settings.NumberOfBranchReservationStations)));
            _ = CreateExecutionUnits<MemUnit>(MemoryBufferUnits, GetNewMem, Settings.NumberOfMemoryFunctionalUnits, Settings.NumberOfReservationStationsPerMemoryUnit, BranchExecuteUnit);
            _ = CreateExecutionUnits<IntUnit>(IntegerExecuteUnits, GetNewInt, Settings.NumberOfIntegerFunctionalUnits, Settings.NumberOfReservationStationsPerIntegerUnit, MemoryBufferUnits.Last());

            ExecuteUnits.Add(BranchExecuteUnit);
            ExecuteUnits.AddRange(MemoryBufferUnits);
            ExecuteUnits.AddRange(IntegerExecuteUnits);
            ExecuteUnits.ForEach(e => e.Reset());
        }
        #endregion

        private void ResetRefetchStateVariables()
        {
            RefetchNeeded = false;
            RefetchTargetAddress = int.MaxValue;
            PCSelectedFromDecode?.Invoke(this, DatapathRegisterEventArgs.Empty); // reset UI
        }
        private void ResetBranchStateVariables()
        {
            AddressMissprediction = false;
            ConditionMissprediction = false;
            MispredictionInstructionSource = null;
            ActualTargetAddress = uint.MaxValue;
            PredictedTargetAddress = uint.MaxValue;
        }
        public void Reset(bool preserveMemory)
        {
            if (!preserveMemory)
                MMU.Reset();

            EBreakInFlight = false;
            ClockCycles = 0;
            GlobalProgramCounter.WriteUnsigned(Settings.ProgramStart);
            RegisterFile.Reset();
            FPRegisterFile?.Reset();
            BranchPredictor.Reset();
            ResetBranchStateVariables();
            ResetRefetchStateVariables();

            Array.ForEach(Pipeline, stage => stage.Reset());
            IRDataQueue.Clear();
            ROB.ResetResize(Settings.NumberOfReorderBufferEntries); // if settings not changed doesn't resizes

            if (Settings.SettingsChanged)
            {
                IRDataQueue.Limit = Settings.InstructionQueueCapacity;

                ResetReservationStationsCount();
                Execute.InitExecutionUnits(ExecuteUnits);
                Dispatch.ResizeReservationStationsFromExecUnits(Execute);
                Complete.ResizeCommonDataBusCollectionContent(ExecuteUnits);
                Retire.ResizeCommonDataBusCollectionContent(ExecuteUnits);
            }
            ReportGenerator.Reset();
        }

        void OnIllegalInstructionEvent(object sender, StageDataArgs e)
        {
            if (false == EBreakInFlight)
            {
                var msg = $"Invalid instruction in {(sender as TEMStage).Name}: {e.Instruction.Value:X8} from address {e.LocalPC.HexString}.";
                throw new NotImplementedInstructionException(msg, e.Instruction);
            }
        }

        #region Event Handlers methods

        #region Stall Hadlers methods
        /// <summary> Adds <paramref name="_for"/> to each of <see cref="StageStallAmount"/> values which key is not <paramref name="notStall"/>. </summary>
        /// <param name="notStall">First <see cref="TEMStage"/> that should not be stalled.</param>
        /// <param name="_for">Amount of cycles that stall signal should be hold for.</param>
        private void StallEachStageUntil(TEMStage notStall, uint _for)
        {
            for (int i = 0; i < Pipeline.Length; i++)
            {
                if (Pipeline[i] != notStall)
                    StageStallAmount[Pipeline[i]] += _for;
                else
                    break;
            }
        }
        #endregion
        
        #region Branch prediction     
        void OnImmediateControlTransferDecoded(object sender, StageDataArgs e)
        {
            var npc = e.DataB;
            int opcode = e.Instruction.opcode;
            int target = e.DataA.Read();
            int predicted = npc.Read();

            bool isJumpAndLink = (opcode == Opcodes.OP_U_TYPE_JUMP);
            bool isBranch = (opcode == Opcodes.OP_B_TYPE_BRANCH);
            bool predictTaken = isBranch && (BranchPredictor.CurrentPrediction == BranchPredictor.Prediction.Taken);
            if (predictTaken || isJumpAndLink)
            {
                if (predicted != target)
                {
                    RefetchNeeded = true;
                    RefetchTargetAddress = target;
                    npc.Write(target);
                    StallEachStageUntil(Decode, 1);
                    if (PCSelectedFromDecode != null)
                    {
                        var args = new DatapathRegisterEventArgs(e.DataA, GlobalProgramCounter, target);
                        PCSelectedFromDecode.Invoke(this, args);
                    }
                }
            }
        }

        void OnControlTransferInstructionComplete(ROBEntry sender, StageDataArgs e)
        {
            var i32 = e.Instruction;
            uint localPC = e.LocalPC.ReadUnsigned();

            PredictedTargetAddress = e.DataA.ReadUnsigned(); // aka NextPC
            bool predictTaken = (PredictedTargetAddress != localPC + ISAProperties.WORD_BYTESIZE);
            bool actualTaken = BranchPredictor.EvalShouldBranch();

            ActualTargetAddress = e.DataB.ReadUnsigned();
            // address misspredicion only valid for JALR instruction
            AddressMissprediction = i32.opcode.Equals(Opcodes.OP_I_TYPE_JUMP) && (ActualTargetAddress != PredictedTargetAddress); 
            ConditionMissprediction = Opcodes.IsBranch(i32) && (predictTaken != actualTaken);
            MispredictionInstructionSource = sender;
        }
        #endregion

        #endregion
        private void InitPipelineEventHandlers()
        {
            Decode.EnvironmentBreakDecoded += delegate
            {
                EBreakInFlight = true;
            };
            Decode.ImmediateControlTransferLatched += OnImmediateControlTransferDecoded;
            Decode.IllegalInstructionDecoded += OnIllegalInstructionEvent;
            
            Dispatch.AttemptedToDispatchIllegalInstruction += OnIllegalInstructionEvent;
            
            Execute.WriteCommonDataBus = WriteCommonDataBusResult;
            
            Complete.ControlTransferInstructionComplete += OnControlTransferInstructionComplete;
            Complete.WriteCommonDataBus = WriteCommonDataBusResult;

            Retire.WriteCommonDataBus = WriteCommonDataBusResult;
            Retire.DataWritten += delegate (object s, DataWriteEventArgs e)
            { 
                e.Cycle = ClockCycles; 
                ReportGenerator.UpdateDataWrittenList(s, e); 
            };
            Retire.RetireCompleted += delegate (object s, StageDataArgs e)
            {
                if (e.Instruction.Equals(Opcodes.INSTR_EBREAK))
                    throw new EnvironmentBreak($"EBREAK instruction at {e.LocalPC} retired.");
                ROBEntry entry = ROB.GetEntryByTag(e.DataA.Read());
                SimReport.UpdateRetiredInstructionCounters(e.Instruction, e.LocalPC, entry.InstructionIndex);
                ReportGenerator.UpdateRetiredInstructions(e.Instruction, e.LocalPC, ClockCycles);
                ReportGenerator.UpdateRegisterStatus(ClockCycles, RegisterFile);
            };
            if (Settings.ThrowOnExecuteZeroDivision)
            {
                Execute.DivisionByZero += delegate (object s, StageDataArgs e)
                {
                    throw new DivideByZeroException($"{e.Instruction} with registers {e.DataA} " + e.DataB?.ToString() ?? "");
                };
            }
            if (Settings.ThrowOnExecuteOverflow)
            {
                Execute.SignedOverflow += delegate (object s, StageDataArgs e)
                {
                    throw new OverflowException($"{e.Instruction} with registers {e.DataA} " + e.DataB?.ToString() ?? "");
                };
            }
            Execute.AssignEventHandlersToUnits();
        }


        /// <summary>
        /// Observes <see cref="StageStallAmount"/> values. On stall required, sets <see cref="PipeRegisters.StallSignal"/> 
        /// of selected buffer register key. Decrements <see cref="StageStallAmount"/> value and disables 
        /// <see cref="PipeRegisters.StallSignal"/> if it hits 0.
        /// </summary>
        private void StallControl()
        {
            for (int i = 0; i < StageStallAmount.Count - 1; i++)
            {
                TEMStage stage = Pipeline[i];
                uint shouldStallFor = StageStallAmount[stage];
                if (shouldStallFor > 0)
                {
                    stage.Stalling = true;
                    StageStallAmount[stage] = (shouldStallFor - 1);
                    // Bubble should be inserted in Stage.Latch() when it is not stalled
                } else
                {
                    stage.Stalling = false;
                }
                SimReport.UpdateStalls(i, stage.Stalling);
            }
        }

        private void BypassStoreToFirstDependentLoad(ROBEntry robEntry, int? storeValue)
        {
#if DEBUG
            bool sanityCheck = Settings.Dynamic_BypassStoreLoad && Opcodes.IsStore(robEntry.IR32) && robEntry.ValueReady;
#else
            const bool sanityCheck = true;
#endif
            if (false == sanityCheck)
                return;   
#if DEBUG
            if (storeValue is null) { throw new InvalidPipelineState($"EffectiveValue is null on {robEntry.IR32} instruction"); }
#endif
            StoreValueBypassedToLoad?.Invoke(this, DatapathEntryEventArgs<IUniqueInstructionEntry>.Empty); // potential clear UI event
            var memCommonDataBus = Execute.MemoryUnits[0].UnitReservationStations; // get first - all units have the same set
            var branchesBus = Execute.BranchUnits.UnitReservationStations;
            for (int i = 0; i < memCommonDataBus.Count; i++)
            {
                var station = memCommonDataBus[i];
                if (false == (station.MarkedEmpty) && Opcodes.IsLoad(station.IR32) && station.EffectiveAddressCalulated.Value) // Loads always .HasValue
                {
                    var loadAddress = station.A.Value; // if .EffectiveAddressCalulated always .HasValue
                    var storeAddress = robEntry.Destination.Value;
                    if (loadAddress == storeAddress && false == station.ROBDest.ValueReady)
                    {
                        var loadIIdx = station.InstructionIndex;
                        var storeIIdx = robEntry.InstructionIndex; // // TODO below: could be excluding JAL as it would never flush pipeline? 
                        if (branchesBus.All(brs => brs.MarkedEmpty || false == Utilis.Utilis.InBetweenNotEqual(loadIIdx, brs.InstructionIndex, storeIIdx)))
                        {   // proceed only if no branch instruction between Store and selected Load
                            station.ROBDest.Value = storeValue;
                            station.ROBDest.FinishedState = HardwareProperties.TEMPipelineStage.Complete;
                            StoreValueBypassedToLoad?.Invoke(this, new DatapathEntryEventArgs<IUniqueInstructionEntry>(robEntry, station, storeValue));
                            ++(SimReport.StoreLoadBypasses);
                        }
                    }
                }
            }
            
        }

        public void WriteCommonDataBusResult(ROBEntry robEntry)
        {
            var cdb = Dispatch.AllReservationStations;
#if DEBUG
            // TODO: delete all this after successfull InstructionIndex refactor:!
            var diffChecker = new HashSet<ulong>(); // InstructionIndex changes in progress: checking to be sure
            if (false == cdb.All(rs => rs.InstructionIndex == 0 || diffChecker.Add(rs.InstructionIndex)))
                throw new InvalidPipelineState("Not unique Instruction index in CDB");
#endif
            bool notNop = (false == robEntry.IR32.IsNop());  // never for NOP instructions, they do not write anywhere 
            bool notStore = (false == Opcodes.IsStore(robEntry.IR32));  // never for Store instructions, they do not write registers
            bool notBranch = (false == Opcodes.IsBranch(robEntry.IR32));  // never for Branch instructions, they do not write registers
            if (robEntry.Value.HasValue)
            {
                int effectiveValue = robEntry.Value.Value;
                if (notStore)
                {
                    if (notNop && notBranch && robEntry.Destination.HasValue)
                    {
                        // feedback to reservation stations
                        int robtag = robEntry.Tag;
                        for (int i = 0; i < cdb.Count; i++)
                        {
                            if (cdb[i].Qj == robtag)
                            {
                                cdb[i].OpVal1 = effectiveValue;
                                cdb[i].ProducerOperand1 = null;
                                ++(SimReport.FeedbackViaCommonDataBus);
                            }
                            if (cdb[i].Qk == robtag)
                            {
                                cdb[i].OpVal2 = effectiveValue;
                                cdb[i].ProducerOperand2 = null;
                                ++(SimReport.FeedbackViaCommonDataBus);
                            }
                        }
                    }
                }
                else if (Settings.Dynamic_BypassStoreLoad)
                {
                    BypassStoreToFirstDependentLoad(robEntry, effectiveValue);
                }
            }
        }
        /// <summary>
        /// Flushes pipeline instructions that were dispatched to <see cref="ROB"/>
        /// before <paramref name="source"/> <see cref="ROBEntry.IR32"/> (not including this in <paramref name="source"/>).
        /// </summary>
        private void FlushPipelineDownFromROBEntry(ROBEntry source)
        {
            ++(SimReport.PipelineFlushes);

            EBreakInFlight = false; // was not neccessarly true but will be always false
            ResetRefetchStateVariables(); // no control transfer decode refetch after flush
            Fetch.ResetInternalProgramCounters();
            Fetch.SoftReset();            
            Decode.SoftReset(); 
            Dispatch.InvokeEmptyLoadEffectiveAddressCalculatedEvent();
            for (int i = 0; i < IRDataQueue.Count; i++) SimReport.ClearInstructionCycleMap(IRDataQueue.Peek(i).InstructionIndex);
            IRDataQueue.Clear(); // flush instruction queue

            var all = Execute.GetAllReservationStations();

            EOlderPlaceholder.Clear(); ENewerPlaceholder.Clear();
            ROB.GetNewerAndOlderEntries(source, false, EOlderPlaceholder, ENewerPlaceholder);

            foreach (var newerEntry in ENewerPlaceholder)
            {   // ROBEntry.Destination exist and it is not memory address
                if (newerEntry.Destination.HasValue && false == Opcodes.IsStore(newerEntry.IR32))
                {
                    RegisterFile.ClearRegisterStatusProducerTag(newerEntry.Destination.Value);

                    ROBEntry lastOldReassigned = null;
                    foreach (var olderEntry in EOlderPlaceholder)
                    {
                        if (olderEntry.Destination.HasValue && olderEntry.Destination.Value == newerEntry.Destination.Value)
                        {
                            if (false == Opcodes.IsStore(olderEntry.IR32))
                            {
                                if (lastOldReassigned is null || lastOldReassigned.InstructionIndex < olderEntry.InstructionIndex)
                                {
                                    RegisterFile.ClearRegisterStatusProducerTag(lastOldReassigned?.Destination ?? 0);
                                    RegisterFile.SetProducerTag(olderEntry.Destination.Value, olderEntry.Tag);
                                    lastOldReassigned = olderEntry;
                                }
                            }
                        }
                    }
                }
                SimReport.ClearInstructionCycleMap(newerEntry.InstructionIndex);
                var station = all.GetStationByTag(newerEntry.ReservationStationTag.Value);
                station.Reset();
                newerEntry.Reset();
            }
        }
        private void CheckControlTransferDecoded()
        {
            if (RefetchNeeded)
            {
                Fetch.SoftReset();
                Fetch.ResetInternalProgramCounters();
                Fetch.SetProgramCounter(RefetchTargetAddress);
                //Decode.SoftReset();
                RefetchNeeded = false;
                RefetchTargetAddress = int.MaxValue;
            }
            else
            {
                PCSelectedFromDecode?.Invoke(this, DatapathRegisterEventArgs.Empty);
            }
        }
        private void CheckMisspredictionState()
        {
            if (ConditionMissprediction)
            {
                ConditionMissprediction = false; // reset state
                FlushPipelineDownFromROBEntry(MispredictionInstructionSource);
                Fetch.SetProgramCounter(ActualTargetAddress);
                SimReport.UpdateBranchMispredictions();
                if (BranchPredictor.IsCallMispredictionEventSet())
                {
                    var i32 = MispredictionInstructionSource.IR32;
                    var localPC = MispredictionInstructionSource.FetchLocalPC;
                    var args = new StageDataArgs(i32, PredictedTargetAddress, ActualTargetAddress, localPC);
                    BranchPredictor.CallMispredictionEvent(args);
                }

            } 
            else if (AddressMissprediction)
            {
                AddressMissprediction = false; // reset state
                FlushPipelineDownFromROBEntry(MispredictionInstructionSource);
                Fetch.SetProgramCounter(ActualTargetAddress);
                ++(SimReport.AddressMisprediction);
            }
        }

        private void StallFrontendIfInstructionQueueFull()
        {
            if (false == Decode.IsReady())
            { // Instruction Queue full 
                StallEachStageUntil(Dispatch, 1);
                SimReport.I32Measures_CollectionFull.Collect(SimReporter.SimMeasures.IRQueue);
            }
        }

        private void Cycle(TEMStage stage) { if (false == stage.Stalling) { stage.Cycle(); } }
        private void Latch(TEMStage stage) { if (false == stage.Stalling) { stage.Latch(); } }

        public void Cycle()
        {
            Cycle(Retire);
            Cycle(Complete);
            Cycle(Execute);
            Cycle(Dispatch);
            Cycle(Decode);
            Cycle(Fetch);
        }

        public void Latch()
        {
            Latch(Retire);
            Latch(Complete);
            Latch(Execute);
            Latch(Dispatch);
            Latch(Decode);
            Latch(Fetch);

            CheckMisspredictionState();
            CheckControlTransferDecoded();
            StallFrontendIfInstructionQueueFull();
            StallControl();
            //ActSheduler.Update(ClockCycles);

            ++ClockCycles;
            // SimReport.UpdateRetiredInstructionCounters() on "Retired.RetireCompleted"
        }

        public void CycleLatchSingle(int StageBufferNo)
        {
            if (StageBufferNo < 0 || StageBufferNo >= Pipeline.Length)
                throw new ArgumentOutOfRangeException($"Stage number {StageBufferNo} is not a number of implemented pipeline stage.");

            Cycle(Pipeline[StageBufferNo]);
            Latch(Pipeline[StageBufferNo]);
            ++ClockCycles;
        }


        public int FlashROM(UInt32[] data, uint _startAddr = 0) => unchecked((int)ROM.Write(_startAddr - ROM.Origin, data));
        public int FlashROM(byte[] data, uint _startAddr = 0) => unchecked((int)ROM.Write(_startAddr - ROM.Origin, data));
        public int FlashRAM(UInt32[] data, uint _startAddr = 0) => unchecked((int)RAM.Write(_startAddr - RAM.Origin, data));
        public int FlashRAM(byte[] data, uint _startAddr = 0) => unchecked((int)RAM.Write(_startAddr - RAM.Origin, data));



    }
}
