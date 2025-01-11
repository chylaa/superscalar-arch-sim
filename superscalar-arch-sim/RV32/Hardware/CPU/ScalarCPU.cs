using System;
using System.Collections.Generic;
using System.Linq;

using superscalar_arch_sim.RV32.Hardware.Pipeline;
using superscalar_arch_sim.RV32.Hardware.Pipeline.TYP.Stage;
using superscalar_arch_sim.RV32.Hardware.Pipeline.TYP.Units;
using superscalar_arch_sim.RV32.Hardware.Pipeline.Units;
using superscalar_arch_sim.RV32.Hardware.Register;
using superscalar_arch_sim.RV32.Hardware.Units;
using superscalar_arch_sim.RV32.ISA.Instructions;
using superscalar_arch_sim.Simulis;
using superscalar_arch_sim.Simulis.Reports;

namespace superscalar_arch_sim.RV32.Hardware.CPU
{

    /// <summary>
    /// Pipelined scalar Central Processing Unit.
    /// </summary>
    public class ScalarCPU : ICPU
    {
        /// <summary>Number of clock cycles performed by <see cref="ScalarCPU"/> (see <see cref="CycleAndLatch"/>).</summary>
        public ulong ClockCycles { get => SimReport.ClockCycles; set => SimReport.ClockCycles = value; }
        /// <summary>
        /// Raw short-formatted value of <see cref="GlobalProgramCounter"/>.
        /// Set avaliable as <see langword="value"/> <see cref="int.Parse(string)"/> into <see cref="Register32.Write(int)"/>.
        /// </summary>
        public string GlobalPCString { get => GlobalProgramCounter.ShortFormat; set => GlobalProgramCounter.Write(int.Parse(value)); }
        public bool FetchStalling { get => IF.Stalling; }
        public string[] StageNames => Pipeline.Select(s => s.Name).ToArray();

        #region Settings

        /// <summary>Allows to set at which <see cref="PipeRegisters"/> branch condition is known.</summary>
        PipeRegisters ControlTransferConditionKnownAt { get; }

        #endregion

        #region Memory

        public Memory.Memory RAM { get; private set; }
        public Memory.Memory ROM { get; private set; }
        //public Cache DCache { get; private set; }
        //public Cache ICache { get; private set; }


        public uint ROMStart { get => ROM.Origin; set => ROM.ResizeMemory(value, ROM.ByteSize); }
        public uint RAMStart { get => RAM.Origin; set => RAM.ResizeMemory(value, RAM.ByteSize); }
        public uint ROMSize { get => ROM.ByteSize; set => ROM.ResizeMemory(ROM.Origin, value); }
        public uint RAMSize { get => RAM.ByteSize; set => RAM.ResizeMemory(RAM.Origin, value); }

        #endregion
        #region Stages

        public Fetch IF { get; private set; }
        public Decode ID { get; private set; }
        public ALUExecute EX { get; private set; }
        public MemoryReach MEM { get; private set; }
        public RegWriteBack WB { get; private set; }

        #endregion
        #region Buffers

        public PipeRegisters IFBuffer { get; private set; }
        public PipeRegisters IF_IDBuffer { get; private set; }
        public PipeRegisters ID_EXBuffer { get; private set; }
        public PipeRegisters EX_MEMBuffer { get; private set; }
        public PipeRegisters MEM_WBBuffer { get; private set; }
        public PipeRegisters TerminationBuffer { get; private set; }

        #endregion

        #region Signals

        /// <summary>Stores amount of clock cycles for which selected <see cref="PipeRegisters.StallSignal"/> should be hold.</summary>
        private Dictionary<PipeRegisters, uint> StageStallAmount { get; set; }

        #endregion

        public TYPStage[] Pipeline { get; private set; }
        public PipeRegisters[] PipelineBuffers { get; private set; }

        #region Registers

        public Register32File RegisterFile { get; private set; }
        public Register32File FPRegisterFile { get; private set; }
        public ControlStatusRegFile ControlStatusRegisters { get; private set; }
        public Register32 GlobalProgramCounter { get; private set; }

        #endregion

        #region Hardware Units
        private FetchAddressSelector FetchAddrMux { get; set; }
        private DataDependencyController DependencyController { get; set; }
        public BranchPredictor BranchPredictor { get; private set; }
        public MemoryManagmentUnit MMU { get; private set; }

        #endregion


        #region Pipeline properties
        /// <summary>Width of pipeline - greater than 1 only in <see cref="SuperscalarCPU"/>.</summary>
        public int PipelineWidth => 1;
        /// <summary>Number of stages in pipeline. Not neccessary equal to <see cref="PipelineDepth"/>!</summary>
        public int NumberOfPipelineStages => Pipeline.Length;

        #endregion

        #region Events
        /// <summary><inheritdoc cref="DataDependencyController.DataForwarded"/></summary>
        public GUIEventHandler<DatapathBufferEventArgs<PipeRegisters>> DataForwarded
        {
            get => DependencyController.DataForwarded;
            set => DependencyController.DataForwarded = value;
        }
        /// <summary><inheritdoc cref="FetchAddressSelector.PCSelectedFromPipelineRegister"/></summary>
        public GUIEventHandler<DatapathBufferEventArgs<PipeRegisters>> PCSelectedFromPipeRegister
        {
            get => FetchAddrMux.PCSelectedFromPipelineRegister;
            set => FetchAddrMux.PCSelectedFromPipelineRegister = value;
        }

        #endregion

        #region Simulation components

        private ActionSheduler ActSheduler { get; }
        public ReportGenerator ReportGenerator { get; }
        public SimReporter SimReport { get => ReportGenerator.Reporter; }

        #endregion

        public ScalarCPU()
        {
            #region Memory
            ROM = new Memory.Memory(Settings.OriginROMAddress, Settings.ROMBytesLength, "ROM");
            RAM = new Memory.Memory(Settings.OriginRAMAddress, Settings.RAMBytesLength, "RAM");
            RAM.Access = HardwareProperties.MemoryAccess.RW;

            //ICache = null;//new Cache(0x100, HardwareProperties.MemoryAccess.Read, ROM);
            //Cache = null;//new Cache(0x1000, HardwareProperties.MemoryAccess.RW, RAM);            
            #endregion

            #region Registers

            GlobalProgramCounter = new Register32(name: "PC", abiMnem: "GPC", meaning: "Program Counter");
            RegisterFile = Reg32FileFactory.InitArchitecturalIntegerRegisters();
            FPRegisterFile = Reg32FileFactory.InitArchitecturalFloatRegisters();
            ControlStatusRegisters = Reg32FileFactory.InitControlStatusRegisterFile();
            ControlStatusRegisters.Origin = 0x0000_0000;

            #endregion
            InitPipeline();
            ControlTransferConditionKnownAt = EX_MEMBuffer; // Known after EXecute stage

            ActSheduler = new ActionSheduler();
            ReportGenerator = new ReportGenerator(NumberOfPipelineStages, 'S');
            DependencyController = new DataDependencyController(ID_EXBuffer, EX_MEMBuffer, MEM_WBBuffer, SimReport);
            StageStallAmount = new Dictionary<PipeRegisters, uint>() {
                {IFBuffer, 0}, {IF_IDBuffer, 0}, {ID_EXBuffer, 0}, {EX_MEMBuffer, 0}, {MEM_WBBuffer, 0 }, {TerminationBuffer, 0 }
            };
            Reset(preserveMemory: true);
            InitPipelineEventHandlers();
        }

        private void InitPipeline()
        {
            #region Buffers

            IFBuffer = new PipeRegisters() { Name = nameof(IFBuffer) };
            IF_IDBuffer = new PipeRegisters() { Name = nameof(IF_IDBuffer) };
            ID_EXBuffer = new PipeRegisters() { Name = nameof(ID_EXBuffer) };
            EX_MEMBuffer = new PipeRegisters() { Name = nameof(EX_MEMBuffer) };
            MEM_WBBuffer = new PipeRegisters() { Name = nameof(MEM_WBBuffer) };
            TerminationBuffer = new PipeRegisters() { Name = nameof(TerminationBuffer) };

            PipelineBuffers = new PipeRegisters[] { IFBuffer, IF_IDBuffer, ID_EXBuffer, EX_MEMBuffer, MEM_WBBuffer, TerminationBuffer };
            #endregion

            #region Units

            MMU = new MemoryManagmentUnit(RAM, ROM);
            FetchAddrMux = new FetchAddressSelector(EX_MEMBuffer);
            BranchPredictor = new BranchPredictor();

            #endregion

            #region Stages 
            IF = new Fetch(MMU, GlobalProgramCounter, FetchAddrMux, BranchPredictor, IFBuffer, IF_IDBuffer);
            ID = new Decode(IF_IDBuffer, RegisterFile, BranchPredictor, ID_EXBuffer);
            EX = new ALUExecute(ID_EXBuffer, BranchPredictor, EX_MEMBuffer);
            MEM = new MemoryReach(EX_MEMBuffer, GlobalProgramCounter, MMU, ControlStatusRegisters, MEM_WBBuffer);
            WB = new RegWriteBack(MEM_WBBuffer, RegisterFile, ControlStatusRegisters, TerminationBuffer);

            Pipeline = new TYPStage[] { IF, ID, EX, MEM, WB };
            #endregion
        }

        public void Reset(bool preserveMemory)
        {
            if (!preserveMemory)
                MMU.Reset();
            
            BranchPredictor.Reset();

            // insert NOP to all but the first buffer (must have null for IF to fetch from memory)
            Array.ForEach(PipelineBuffers, buff => { buff.Reset(); buff.InsertBubble(); StageStallAmount[buff] = 0; });
            Array.ForEach(Pipeline, stage => stage.Reset());
            IFBuffer.IR32 = null; // start with null to propagete stall but fetch valid instruction

            ClockCycles = 0;
            RegisterFile.Reset();
            FPRegisterFile?.Reset();
            GlobalProgramCounter.WriteUnsigned(Settings.ProgramStart);

            ReportGenerator.Reset();
            ActSheduler.Reset();
        }

        public int FlashROM(UInt32[] data, uint _startAddr = 0) => unchecked((int)ROM.Write(_startAddr - ROM.Origin, data)); 
        public int FlashROM(byte[] data, uint _startAddr = 0) => unchecked((int)ROM.Write(_startAddr - ROM.Origin, data)); 
        public int FlashRAM(UInt32[] data, uint _startAddr = 0) => unchecked((int)RAM.Write(_startAddr - RAM.Origin, data)); 
        public int FlashRAM(byte[] data, uint _startAddr = 0) => unchecked((int)RAM.Write(_startAddr - RAM.Origin, data));

        #region Event Handlers methods
        void SimReportUpdateDataWritten(object s, DataWriteEventArgs e) 
        { 
            e.Cycle = ClockCycles; 
            ReportGenerator.UpdateDataWrittenList(s, e); 
        }
        #region Stall Hadlers methods
        /// <summary> Adds <paramref name="_for"/> to each of <see cref="StageStallAmount"/> values which key is not <paramref name="notStall"/>. </summary>
        /// <param name="notStall"><see cref="PipeRegisters"/> preceeding <see cref="TYPStage"/> that should not be stalled.</param>
        /// <param name="_for">Amount of cycles that stall signal should be hold for.</param>
        private void StallEachStageUntil(PipeRegisters notStall, uint _for)
        {
            foreach (PipeRegisters preg in PipelineBuffers.TakeWhile(x => x != notStall))
                StageStallAmount[preg] += _for;
        }
        /// <summary>
        /// Calls <see cref="StallEachStageUntil(PipeRegisters, int)"/> with calculated distance between last stalled stage
        /// (specified by <paramref name="stallStagesUntil"/>) and stage after which result is known (specified by <paramref name="resultRegisters"/>).
        /// </summary>
        /// <param name="stallStagesUntil"><see cref="PipeRegisters"/> buffer being output of last stalled <see cref="TYPStage"/>.</param>
        /// <param name="resultRegisters"><see cref="PipeRegisters"/> buffer being output for <see cref="TYPStage"/> which completion indicates that stall is no longer required.</param>
        /// <param name="stallOffset">Number of stall cycles that should be added those equal to distance between passed <see cref="PipeRegisters"/>.</param>
        private void StallStagesUntilPipelineRegisterContainsResult(PipeRegisters stallStagesUntil, PipeRegisters resultRegisters, int stallOffset = 0)
        {
            long distance = TYPStage.GetStagesDistance(Pipeline, stallStagesUntil, resultRegisters);
            StallEachStageUntil(stallStagesUntil, (uint)(distance + stallOffset));
        }
        /// <summary>Flushes pipeline. Can be used to simulate <see cref="Settings.CPUType.Subscalar"/> processor.</summary>
        private void StallUntilFetchedWrittenBack() => StallStagesUntilPipelineRegisterContainsResult(IF_IDBuffer, TerminationBuffer);

        private void OnBranchDecoded(object o, StageDataArgs e)
        {
            if (false == BranchPredictor.Enabled) 
            {
                StallStagesUntilPipelineRegisterContainsResult(ID_EXBuffer, ControlTransferConditionKnownAt, stallOffset: +1);
            } 
            else
            {
                BranchPredictor.SetCurrentPrediction(e.LocalPC.ReadUnsigned());
                if (BranchPredictor.CurrentPrediction == BranchPredictor.Prediction.Taken)
                {
                    IF.ReFetchAndLatchProcessedInstruction(ID_EXBuffer);
                }
                int? targetAddressPrediction = BranchPredictor.GetPredictedTargetAddress(e.LocalPC);
                if (targetAddressPrediction is null)
                {
                    ++(SimReport.AddressMisprediction);
                } // [22:26 16.10.2024 MC Hotfix]
                // Moved outside "if" block -> spaghetti code for branch hadling - rewrite all!.
                // GPC address changes driven from Fetch - when Jump instruction is next after Branch, fetch is stalled
                // (after-jump window for calculating JALR target addr), missing branch target address refetch
                // and jumping from next cycle instruction instead (from Jump target).
                StallEachStageUntil(ID_EXBuffer, 1); 
            }
        }
        /// <summary>
        /// Creates new <see cref="EventHandler"/> <see langword="delegate"/> that will invoke method responsible 
        /// for stalling decoded instruction until previous one finishes execution, arriving at <see cref="TerminationBuffer"/>.
        /// </summary>
        private void StallDecodedUntilExWrittenBack()
        {
            StallStagesUntilPipelineRegisterContainsResult(ID_EXBuffer, TerminationBuffer);
        }

        private void RelatchValuesFromFetchDecodeBuffers() { ID.Latch(); IF.Latch(); }
        #endregion
        #endregion
        private void InitPipelineEventHandlers()
        {
            ID.BranchLatched += OnBranchDecoded;
            ID.JumpLatched += delegate (object sender, StageDataArgs e)
            {
                if (e.DataA.ReadUnsigned() != (e.LocalPC.ReadUnsigned() + ISA.ISAProperties.WORD_BYTESIZE))
                {   // if NextPC is not next instruction
                    StallStagesUntilPipelineRegisterContainsResult(ID_EXBuffer, ControlTransferConditionKnownAt, stallOffset: +1);
                }
            };

            ID.EnvironmentBreakDecoded += delegate (object s, StageDataArgs e)
            {
                ulong distance = (ulong)TYPStage.GetStagesDistance(Pipeline, ID_EXBuffer, TerminationBuffer);
                //SimReport.StagesStalls[0] -= distance; // offset stalls to not include those from EBREAK
                StallDecodedUntilExWrittenBack(); // Stall till EBREAK in WB stage
                Action exec = new Action(() => throw new EnvironmentBreak($"EBREAK instruction decoded at {e.LocalPC}."));
                ActSheduler.SheduleAt(ClockCycles + distance, exec);
            };

            EX.ControlTransfer += delegate (object s, StageDataArgs e)
            {
                if (Opcodes.IsBranch(e.Instruction))
                {
                    ++(SimReport.BranchesTaken);
                }
                if (false == BranchPredictor.Enabled || Opcodes.IsJump(e.Instruction))
                {
                    IF.ReFetchAndLatchProcessedInstruction(EX_MEMBuffer); // On Jump or Branch - Refetch Instruction using new address in EX_MEM 
                }
            };
            
            MEM.MemoryWritten += SimReportUpdateDataWritten;
            WB.RegisterWritten += SimReportUpdateDataWritten;

            BranchPredictor.OnBranchMisprediction += delegate (object o, StageDataArgs e)
            {
                SimReport.UpdateBranchMispredictions();
                ID.KillProcessedInstruction(Instruction.NOP); EX.KillProcessedInstruction(Instruction.NOP);
            };

            DependencyController.NoForwardingDataDependencyDetected += delegate (object o, DatapathBufferEventArgs<PipeRegisters> e)
            {
                StallStagesUntilPipelineRegisterContainsResult(e.DataSource, TerminationBuffer);
                if (e.DataDest == ID_EXBuffer)
                {
                    ActSheduler.SheduleAt(ClockCycles, RelatchValuesFromFetchDecodeBuffers);
                }
            };

            if (Settings.ThrowOnExecuteZeroDivision)
            {
                EX.DivisionByZero += delegate (object s, StageDataArgs e)
                {
                    throw new DivideByZeroException($"{e.Instruction} with registers {e.DataA} " + e.DataB?.ToString() ?? "");
                };
            }
            if (Settings.ThrowOnExecuteOverflow)
            {
                EX.SignedOverflow += delegate (object s, StageDataArgs e)
                {
                    throw new OverflowException($"{e.Instruction} with registers {e.DataA} " + e.DataB?.ToString() ?? "");
                };
            }
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
                TYPStage stage = Pipeline[i];
                PipeRegisters pbprev = PipelineBuffers[i];
                uint shouldStallFor = StageStallAmount[pbprev];
                if (shouldStallFor > 0)
                {
                    stage.Stalling = true;
                    StageStallAmount[pbprev] = (shouldStallFor - 1);
                    // Bubble inserted in Stage.Latch() when it is not stalled
                } else
                {
                    stage.Stalling = false;
                }

                SimReport.UpdateStalls(i, stage.Stalling);
            }
        }

        private void UpdateCSRsCounters()
        {
            ControlStatusRegisters[ControlStatusRegFile.CSRs.CYCLE] = (uint)(ClockCycles & UInt32.MaxValue);
            ControlStatusRegisters[ControlStatusRegFile.CSRs.CYCLEH] = (uint)(ClockCycles >> 32);
            ControlStatusRegisters[ControlStatusRegFile.CSRs.INSTRET] = (uint)(SimReport.CommitedInstructions & UInt32.MaxValue);
            ControlStatusRegisters[ControlStatusRegFile.CSRs.INSTRET] = (uint)(SimReport.CommitedInstructions >> 32);
            ulong time = unchecked((ulong)DateTime.Now.Ticks);
            ControlStatusRegisters[ControlStatusRegFile.CSRs.TIME] = (uint)(time & UInt32.MaxValue); 
            ControlStatusRegisters[ControlStatusRegFile.CSRs.TIME] = (uint)(time >> 32); 
        }

        private void UpdateSimMeasuresReport()
        {
            var tb = TerminationBuffer; var fdb = IF_IDBuffer;
            SimReport.UpdateRetiredInstructionCounters(tb.IR32, tb.LocalPC, tb.InstructionIndex);
            ReportGenerator.UpdateRetiredInstructions(tb.IR32, tb.LocalPC, ClockCycles);
        }

        public void Cycle()
        {
            if (false == IF.Stalling)
                SimReport.UpdateFetchedInstructionCounters(IF_IDBuffer.IR32, IF_IDBuffer.InstructionIndex);

            for (int stageidx = (Pipeline.Length - 1); stageidx >= 0; stageidx--)
            {
                if (false == Pipeline[stageidx].Stalling)
                    Pipeline[stageidx].Cycle();
            }
        }

        public void Latch()
        {
            for (int stageidx = (Pipeline.Length - 1); stageidx >= 0; stageidx--)
            {
                if (false == Pipeline[stageidx].Stalling)
                    Pipeline[stageidx].Latch();
            }

            ActSheduler.Update(ClockCycles);
            ++ClockCycles;
            UpdateSimMeasuresReport();

            DependencyController.ForwardingControl();
            if (DependencyController.LoadInterlockHazardDetected())
            {
                //1 cycle "LOAD DELAY SLOT", allowing data to by forwarded from MEM_WB
                StallStagesUntilPipelineRegisterContainsResult(EX_MEMBuffer, resultRegisters: MEM_WBBuffer);
                ++(SimReport.LoadInterlocks);
                ++(SimReport.DataDependencies);
            }
            StallControl();
            //UpdateCSRsCounters();
        }

        /// <summary>
        /// Performs single <see cref="IClockable.Cycle"/> on selected <see cref="TYPStage"/> and is precceding <see cref="PipeRegisters"/>.
        /// <see cref="IClockable.Latch"/> is invoked if <see cref="IClockable.Stalling"/> signal is <see langword="false"/>.
        /// </summary>
        /// <param name="StageBufferNo">Zero-indexed position of <see cref="TYPStage"/> and it's precceding <see cref="PipeRegisters"/></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void CycleLatchSingle(int StageBufferNo)
        {
            if (StageBufferNo < 0 || StageBufferNo >= Pipeline.Length)
                throw new ArgumentOutOfRangeException($"Stage number {StageBufferNo} is not a number of implemented pipeline stage.");

            //StallController();
            if (false == Pipeline[StageBufferNo].Stalling)
            {
                Pipeline[StageBufferNo].Cycle();
                Pipeline[StageBufferNo].Latch();
            }
            ++ClockCycles;
        }


    }
}
