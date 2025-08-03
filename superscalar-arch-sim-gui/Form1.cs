using superscalar_arch_sim;
using superscalar_arch_sim.RV32.Hardware.CPU;
using superscalar_arch_sim.Simulis.Reports;
using superscalar_arch_sim_gui.Forms;
using superscalar_arch_sim_gui.UserControls;
using superscalar_arch_sim_gui.Utilis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using static superscalar_arch_sim.Utilis.Utilis;

namespace superscalar_arch_sim_gui
{
    public partial class MainForm : Form
    {
        /// <summary>Contains definitions for states of <see cref="SimuRunner"/>.</summary>
        enum SimulationStatus { 
        /// <summary>Simulation task running </summary>
            RUNNING, 
        /// <summary>Simulation task break by user, or breakpointAddr hit.</summary>
            PAUSED, 
        /// <summary>Simulation task stopped and ready to be run.</summary>
            READY 
        };

        const string SIM_STEP_CYCLE_BUTTON_TEXT = " ⮥▷ Cycle";
        const string SIM_STEP_LATCH_BUTTON_TEXT = " ⮧▷ Latch";
        const string SIM_RUN_BUTTON_TEXT    = " ▶▶ Run   ";
        const string SIM_RESUME_BUTTON_TEXT = " ▶▶ Resume";
        const string TOOLTIP_SIMSPEED_LABEL = "Effective real-time simulator core frequency, represented as number of clock cycles per second [Hz]";
        const string TOOLTIP_SIMTIME_LABEL = "Simulation runtime [hh:mm:ss] ";
        const string TOOLTIP_TIMEOUT_LABEL = "Simulation timeout target time [hh:mm:ss]";

        readonly static Type __DefaultProcessorView = typeof(SuperscalarCPU);

        readonly Dictionary<SimulationStatus, Color> SimStatusColors = new Dictionary<SimulationStatus, Color>() {
            { SimulationStatus.RUNNING, Color.Aqua}, { SimulationStatus.PAUSED, Color.IndianRed},{ SimulationStatus.READY, Color.ForestGreen} 
        };

        ScalarCPU ScalarCPU { get; set; }
        SuperscalarCPU SuperscalarCPU { get; set; }
        ICPU SimulatedCPU { 
            get { if (tabControlCPUViewType.SelectedIndex == 0) { return this.ScalarCPU; } else { return this.SuperscalarCPU; } }
        }
        IGenericCPUView<ICPU> ActiveCPUView {
            get { if (tabControlCPUViewType.SelectedIndex == 0) { return (scalarCoreView); } else { return (superscalarCoreView); } }
        } 

        MemoryViewer MemoryViewerForm { get; set; }
        RegisterFileView RegisterFileForm { get; set; }
        SimSettings SimSettingsForm { get; set; }
        IOTerminal IOTerminalForm { get; set; }

        System.Windows.Forms.Timer SimTimeUpdateDisplayTimer { get; set; } = null;
        System.Windows.Forms.Timer SimRunExperimentDelayed { get; set; } = null;
        SimulationStatus SimStatus { get; set; } = SimulationStatus.READY;

        /// <summary>General <see cref="Semaphore"/> for <see cref="ShowShortStatusMessage(string, int)"/> method.</summary>
        private readonly SemaphoreSlim StatusLabelSemaphore = new SemaphoreSlim(1, 1);
        private readonly TabPage SuperscalarViewTabpage = null;

        public MainForm()
        {
            InitializeComponent();
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

            KeyPreview = true;
            AllowDrop = true;
            SimTimeUpdateDisplayTimer = new System.Windows.Forms.Timer() { Interval = 1000, Enabled = false };
            InitEventHandlers();

            SuperscalarViewTabpage = tabControlCPUViewType.TabPages[1];
            tabControlCPUViewType.TabPages.Remove(SuperscalarViewTabpage);
            Shown += delegate { tabControlCPUViewType.TabPages.Add(SuperscalarViewTabpage); };
            var elItem = new ToolStripMenuItem("Retired log") { ShowShortcutKeys = true, ShortcutKeys = Keys.F12 };
            exporToolStripMenuItem.DropDownItems.Add(elItem);
        }

        /// <summary>Returns passed <paramref name="form"/> if it exist and is not <see cref="Form.Disposing"/> or <see cref="Form.IsDisposed"/>, otherwise <see langword="null"/>.</summary>
        Form GetFormOrNull(Form form)
            => form is null || (form.Disposing || form.IsDisposed) ? null : form;

        private void InitEventHandlers()
        {
            Load += MainForm_Load;
            FormClosing += MainForm_FormClosing;
            KeyUp += MainForm_KeyUp;
            KeyDown += MainForm_KeyDown;
            SimTimeUpdateDisplayTimer.Tick += SimTimeTimer_Tick;

            DragEnter += delegate (object sender, DragEventArgs e) { 
                e.Effect = DragDropEffects.Link; 
            };
            useABIRegisterNamesToolStripMenuItem.CheckedChanged += delegate (object sender, EventArgs e) {
                superscalar_arch_sim.RV32.ISA.Disassembler.UseABIRegisterMnemonic = (sender as ToolStripMenuItem).Checked;
            };
            BreakpointAddrNumericUpDown.ValueChanged += delegate {
                uint value = (uint)BreakpointAddrNumericUpDown.Value;
                value = NearestAlligned(value, superscalar_arch_sim.RV32.Hardware.Memory.Allign.WORD);
                BreakpointAddrNumericUpDown.Value = value;
            };

            resetCoreToolStripMenuItem.Click += delegate { 
                SimuRunner.ResetCPU(SimulatedCPU, SimuRunner.ResetUnit.Core); 
                ActiveCPUView.UpdateBindings();
                ActiveCPUView.ClearAfterReset(); 
                SetCycleLatchStepToolStripButtonsBaseOnNextEdge();
                ShowShortStatusMessage("CPU Core reset successfully.");
            };
            EventHandler MemResetAction(SimuRunner.ResetUnit reset) {
                return delegate {
                    SimuRunner.ResetCPU(SimulatedCPU, reset);
                    ((MemoryViewer)GetFormOrNull(MemoryViewerForm))?.RefreshMemoryView(ram: true, rom: true, romHighlight:true);
                    ActiveCPUView.GetQuickMemView.UpdateAllMemoryValues();
                    ShowShortStatusMessage(reset.ToString()+" memory reset successfully.");
                };
            };
            resetRAMToolStripMenuItem.Click += MemResetAction(SimuRunner.ResetUnit.RAM); 
            resetROMToolStripMenuItem.Click += MemResetAction(SimuRunner.ResetUnit.ROM); 

            resetRegisterFileToolStripMenuItem.Click += delegate { 
                SimuRunner.ResetCPU(SimulatedCPU, SimuRunner.ResetUnit.Registers);
                ((RegisterFileView)GetFormOrNull(RegisterFileForm))?.RefreshAll(); 
                ShowShortStatusMessage("CPU Register File reset successfully.");
            };

            tabControlCPUViewType.SelectedIndexChanged += delegate
            {
                CloseAllForms();
                SimulatedCPU.Reset(preserveMemory: true); // ensure settings are applied between ICPUs
                ActiveCPUView.UpdateBindings();

                if (tabControlCPUViewType.SelectedTab == tabPageDynamicCPU)
                    BreakpointAddrNumericUpDown.ToolTipText = "Address of Instruction that when Dispatched fron Queue to Reservation Station, will trigger simulation PAUSE event.";
                else if (tabControlCPUViewType.SelectedTab == tabPageStaticCPU)
                    BreakpointAddrNumericUpDown.ToolTipText = "Address of Global Program Counter that will trigger simulation PAUSE event.";
            };

            void TimeWriteLine(string text) => Console.WriteLine((string.IsNullOrEmpty(text) ? "" : (DateTime.Now.ToString("[HH:mm:ss.ff]") + ' ')) + text);
            void SaveErrorFile(ExperimentRunner.ExperimentRunError ex){
                if (Directory.Exists(ExperimentRunner.CurrentOutputDir)) {
                    var path = $"{ExperimentRunner.CurrentOutputDir}{nameof(ExperimentRunner.ExperimentRunError)}.txt";
                    var content = $"[{DateTime.Now:HH:mm:ss.ff}] {ex.Message}{Environment.NewLine}{ex.Cause}{Environment.NewLine}";
                    UserFilesController.SaveFile(path, content, showerr: false, append:true);
                } 
            }
            ExperimentRunner.Logging = TimeWriteLine;
            ExperimentRunner.ClearLog = Console.Clear;
            ExperimentRunner.OnRunError = SaveErrorFile;
            ExperimentRunner.NewRunStarting += ExperimentRunner_NewRunStarting;
            ExperimentRunner.NewRunStarted += ExperimentRunner_NewRunStarted;
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F3)
            {
                // F3 on DataGridView aparently tries to sort it (and crashes app). I dont have time to disable it for each DGV so this failsafe should work
                e.Handled = e.SuppressKeyPress = true;             
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Enabled = false;
            if(SimuRunner.SimulationTask != null && false == SimuRunner.SimulationTask.IsCompleted) 
                SimuRunner.CancelSimulation(wait_ms: 500);

            CloseAllForms();
            GetFormOrNull(MemoryViewerForm)?.Dispose();
            GetFormOrNull(RegisterFileForm)?.Dispose();
            GetFormOrNull(RegisterFileForm)?.Dispose();
            GetFormOrNull(IOTerminalForm)?.Dispose();
            SimTimeUpdateDisplayTimer?.Stop(); SimTimeUpdateDisplayTimer?.Dispose();
            StatusLabelSemaphore.Wait(2000); StatusLabelSemaphore.Dispose();
            SimuRunner.SimulationTask?.Dispose();
            SimuRunner.CancellationTokenSource?.Dispose();
            //SimulatedCPU?.Reset(preserveMemory:false);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            FormsToolTip.Active = true;
            FormsToolTip.ShowAlways = true;

            ScalarCPU = new ScalarCPU();
            SuperscalarCPU = new SuperscalarCPU();
            SimuRunner.SetNextEdge(ScalarCPU, SimuRunner.ClockEdge.Rising);
            SimuRunner.SetNextEdge(SuperscalarCPU, SimuRunner.ClockEdge.Rising);

            scalarCoreView.InitControlData(ScalarCPU);
            superscalarCoreView.InitControlData(SuperscalarCPU);

            if (__DefaultProcessorView == typeof(SuperscalarCPU))
                tabControlCPUViewType.SelectedTab = tabPageDynamicCPU;
            
            SetSimStatus(SimulationStatus.READY);
            SetSimulationStatusLabels(active: false);
            RunSimulationToolStripButton.Text = SIM_RUN_BUTTON_TEXT;
            CycleHalfToolStripButton.Text = GetSingleStepToolStripText();
            ToolStripShortStatusMessageLabel.Text = string.Empty; // clear status info label
            RefreshAllCPUUserInterfaceComponents();
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            void PerformClickIfEnabled(ToolStripButton tsb) { if (tsb.Enabled) tsb.PerformClick(); }

            if (e.KeyCode == Keys.Escape)
                Close();
            else if (e.KeyCode == Keys.F1)
                PerformClickIfEnabled(CycleHalfToolStripButton);
            else if (e.KeyCode == Keys.F2)
                PerformClickIfEnabled(CycleLatchToolStripButton);
            else if (e.KeyCode == Keys.F5)
                PerformClickIfEnabled(UpdateViewToolStripButton);
            else if (e.KeyCode == Keys.F8)
                PerformClickIfEnabled(RunSimulationToolStripButton);
            else if (e.KeyCode == Keys.F9)
                PerformClickIfEnabled(PauseSimulationToolStripButton);

            else if (e.KeyCode == Keys.F12)
            {
                string commitedmsg = "At least one of log files was not saved. Collecting disabled or nothing collected.";
                if (SimulatedCPU.ReportGenerator.CreateReport != superscalar_arch_sim.Simulis.Reports.Report.None) {
                    if (SimulatedCPU.ReportGenerator.FinalizeAllEnabledLogFiles()) {
                        commitedmsg = "Enabled logs listing finalized.";
                    }
                }
                ShowShortStatusMessage(commitedmsg);
            }
        }

        private void CloseAllForms()
        {
            GetFormOrNull(SimSettingsForm)?.Close();
            GetFormOrNull(MemoryViewerForm)?.Close();
            GetFormOrNull(RegisterFileForm)?.Close();
            GetFormOrNull(IOTerminalForm)?.Close();
        }

        #region Message status presenter -> (extract to separate class)
        private void InvokeSetStatusMessageLabelTextAndColor(string msg, Color color)
        {
            EventHandler setEvent = (object sender, EventArgs e) =>
            {
                var label = ToolStripShortStatusMessageLabel;
                if (msg != null) { label.Text = msg; label.BackColor = (msg.Length == 0 ? BottomStatusStrip.BackColor : SystemColors.Info); }
                if (color != Color.Empty) ToolStripShortStatusMessageLabel.ForeColor = color;
            };
            if (InvokeRequired) Invoke(setEvent);
            else setEvent(null, null);
        }

        private async void ShowShortStatusMessage(string msg, int msduration = 1500)
        {
            if (false == Enabled || Disposing || IsDisposed || StatusLabelSemaphore is null) // apparently Form closed, so ignore msg requests 
                return;
            if (msg == ToolStripShortStatusMessageLabel.Text)
                return; // not show identical message again 

            await StatusLabelSemaphore.WaitAsync();            
            try
            {
                // Display the message in the ToolStripShortStatusMessageLabel
                InvokeSetStatusMessageLabelTextAndColor(msg, Color.Empty);

                await Task.Delay(msduration);
                // Fade out the message
                for (int i = 0; i <= 10; i++)
                {
                    await Task.Delay(50);
                    if (Disposing || IsDisposed) return;
                    double blendFactor = i / 10.0;
                    Color blendedColor = GUIUtilis.ColorBlend(SystemColors.ControlText, SystemColors.Control, blendFactor);
                    ToolStripShortStatusMessageLabel.ForeColor = blendedColor;
                }
                InvokeSetStatusMessageLabelTextAndColor(string.Empty, SystemColors.ControlText);
            } 
            finally
            {
                if (false == (Disposing || IsDisposed))
                    StatusLabelSemaphore?.Release();
            }
        }
        #endregion
        private void ShowFormItem_Click(object sender, EventArgs e)
        {
            if (sender == ViewMemoryToolStripMenuItem)
                CreateMemoryViewFormOrBringToFront();
            else if (sender == ViewRegistersToolStripMenuItem)
                CreateRegisterViewFormOrBringToFront();
            else if (sender == OpenSettingsToolStripMenuItem)
                CreateSimSettingsViewFormOrBringToFront();
            else if (sender == TerminalViewToolStripMenuItem)
                CreateIOTerminalFormOrBringToFront();
        }
        private void CreateMemoryViewFormOrBringToFront()
        {
            if (GetFormOrNull(MemoryViewerForm) is null) {
                MemoryViewerForm = new MemoryViewer(SimulatedCPU.RAM, SimulatedCPU.ROM);
                MemoryViewerForm.BindGlobalPCAddressGetterToROM(SimulatedCPU.GlobalProgramCounter.ReadUnsigned);
                MemoryViewerForm.Show();
            } else { 
                MemoryViewerForm.BringToFront();
            }
        }
        private void CreateRegisterViewFormOrBringToFront()
        {
            if (GetFormOrNull(RegisterFileForm) is null)
                (RegisterFileForm = new RegisterFileView(SimulatedCPU)).Show();
            else
                RegisterFileForm.BringToFront();
        }
        private void CreateSimSettingsViewFormOrBringToFront()
        {
            SimSettings settingsForm = (SimSettings)GetFormOrNull(SimSettingsForm);
            if (settingsForm != null && SimulatedCPU != settingsForm.Core)
            {
                SimSettingsForm.Close();
                SimSettingsForm.Dispose();
                SimSettingsForm = null;
            }
            if (settingsForm is null) {
                SimSettingsForm = new SimSettings(SimulatedCPU);
                SimSettingsForm.SettingsChangedCallback += SimSettingsForm_SettingsChangedCallback;
                SimSettingsForm.Show();
            } else {
                SimSettingsForm.BringToFront();
            }
        }
        private void CreateIOTerminalFormOrBringToFront()
        {
            if (GetFormOrNull(IOTerminalForm) is null)
                (IOTerminalForm = new IOTerminal(SimulatedCPU)).Show();
            else
                IOTerminalForm.BringToFront();
        }

        private void SimSettingsForm_SettingsChangedCallback(object sender, EventArgs e)
        {
            if (superscalar_arch_sim.Simulis.Settings.SettingsChanged)
            {
                ActiveCPUView.InitControlData(SimulatedCPU);
                RefreshAllCPUUserInterfaceComponents();
                RefreshAllCPUUserInterfaceComponents();
                superscalar_arch_sim.Simulis.Settings.SettingsChanged = false;
            }
        }

        private bool SheduleExperiment(string path)
        {
            if (false == (path != null && path.EndsWith(ExperimentRunner.FileExtension)))
                return false;
            if (SimStatus != SimulationStatus.READY)
                return false;
            var decision = MessageBox.Show($"Run experiment from {path}?", "Experiment file dropped", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (decision != DialogResult.Yes)
                return false;

            if (SimRunExperimentDelayed != null)
            {
                SimRunExperimentDelayed.Stop();
                SimRunExperimentDelayed.Dispose();
            }
            SimRunExperimentDelayed = new System.Windows.Forms.Timer();
            SimRunExperimentDelayed.Interval = 2500;
            SimRunExperimentDelayed.Tick += delegate { RunExperiment(path); };
            SimRunExperimentDelayed.Start();
            return true;
        }

        private bool ImportSettings(string filepath)
        {
            string extension = Path.GetExtension(filepath).ToLowerInvariant();
            bool dotScs = string.Equals(extension, superscalar_arch_sim.Simulis.Settings.FileExtension, StringComparison.OrdinalIgnoreCase);
            if (dotScs)
            {
                CreateSimSettingsViewFormOrBringToFront();
                SimSettingsForm.ImportSettings(filepath);
                //SimSettingsForm.Close();
            }
            return dotScs;
        }
        private bool FlashSimulatedCPUMemory(string filepath)
        {
            string extension = Path.GetExtension(filepath).ToLowerInvariant();
            bool dotText = string.Equals(extension, ".text", StringComparison.OrdinalIgnoreCase);
            bool dotData = string.Equals(extension, ".data", StringComparison.OrdinalIgnoreCase);
            if (false == (dotText || dotData))
                return false;

            int flashed = -1;
            string memname = null;
            uint[] content = null;
            long filesize = -1;
            uint rombytesize = SimulatedCPU.ROMSize;
            
            try { content = GetUInt32sFromFile(filepath, out filesize, maxsize: rombytesize, input_little_endian: true); } 
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Data Read Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            if (content is null) { 
                return false; 
            }
            if (rombytesize < filesize)
            {
                string msg = $"Warning: Size of data (0x{filesize:X} bytes) trimmed to ROM size (0x{rombytesize:X} bytes). Proceed?";
                var decision = MessageBox.Show(this, msg, "Data Read Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (decision == DialogResult.No) {
                    return false;
                }
            }

            if (dotText)
            {
                flashed = SimulatedCPU.FlashROM(content, SimulatedCPU.ROMStart);
                memname = SimulatedCPU.ROM.Name;
            } 
            else if (dotData)
            {
                flashed = SimulatedCPU.FlashRAM(content, SimulatedCPU.RAMStart);
                memname = SimulatedCPU.RAM.Name;
            }
            bool processed;
            if (processed = (flashed > 0 && memname != null))
            {
                string msg = $"{flashed} bytes of {extension} written. Reset CPU core and registers?";
                string title = $"{memname} successfully flashed with {extension.Remove(0, 1).ToUpper()}!";
                if (DialogResult.Yes == MessageBox.Show(this, msg, title, MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2))
                {
                    ResetCPUToSimulationRunReadyState();
                } 
                else
                {
                    ActiveCPUView.GetQuickMemView.UpdateAllMemoryValues();
                }
            } 
            else
            {
                string msg = $"Failed to write {filepath} data - no data written. Check if provided valid .text/.data file";
                MessageBox.Show(this, msg, "Data Upload Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return processed;
        }

        private void ProgramMemoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filepath = UserFilesController.AskForOpenFilePath(UserFilesController.DataTextFilter);
            if (filepath != null) {
                FlashSimulatedCPUMemory(filepath);
            }
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            if (SimStatus != SimulationStatus.READY)
            {
                MessageBox.Show($"Cannot accept files on simulation status {SimStatus}. STOP simulation first.",
                                "Info: Cannot acceppt drag-drop.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (e.Effect == DragDropEffects.Link && e.Data.GetDataPresent(DataFormats.FileDrop)) 
            {
                foreach (string file in (string[])e.Data.GetData(DataFormats.FileDrop)) 
                {
                    if (File.Exists(file))
                    {
                        FlashSimulatedCPUMemory(file);
                        ImportSettings(file);
                        SheduleExperiment(file);
                    }
                }
            }

        }
        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            // Disallow changing CPU views while simulation not stopped/not latched, and Units Views shown
            bool sEdgeRising = SimuRunner.GetNextEdge(ScalarCPU) == SimuRunner.ClockEdge.Rising;
            bool dEdgeRising = SimuRunner.GetNextEdge(SuperscalarCPU) == SimuRunner.ClockEdge.Rising;
            if (SimStatus != SimulationStatus.READY || false == (sEdgeRising && dEdgeRising)) {
                e.Cancel = true;
            } else {
                // Tab changed
                CloseAllForms();
                scalarCoreView.CloseAllSubforms();
                superscalarCoreView.CloseAllSubforms();
                if (WindowState != FormWindowState.Maximized) { 
                    Width = (ActiveCPUView.DesirableSize.Width + Padding.Horizontal + DefaultMargin.Horizontal + 20);
                }
            }
        }

        private void simCountersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SimulatedCPU != null) {
                string path = UserFilesController.AskForSaveFilePath(SimulatedCPU.ReportGenerator.DefaultCountersReportFile, UserFilesController.TextFilter);
                if (path != null) {
                    string msg = "Failed to save file ";
                    if (SimulatedCPU.ReportGenerator.AsyncWriteSimCountersToFile(path).Result)
                        msg = "Simulation couters report saved to ";
                    ShowShortStatusMessage(msg + path);
                }
            }
        }
        private string GetSingleStepToolStripText()
        {
            if (SimuRunner.GetNextEdge(SimulatedCPU) == SimuRunner.ClockEdge.Rising)
            {
                return SIM_STEP_CYCLE_BUTTON_TEXT;
            } 
            else
            {
                return SIM_STEP_LATCH_BUTTON_TEXT;
            }
        }
        private void SetCycleLatchStepToolStripButtonsBaseOnNextEdge()
        {
            CycleHalfToolStripButton.Text = GetSingleStepToolStripText();
            CycleLatchToolStripButton.Enabled = (SimuRunner.GetNextEdge(SimulatedCPU) == SimuRunner.ClockEdge.Rising);
        }


        #region Simulation Methods
        private TimeSpan GetUserSimulationTimeout(out bool infinite)
        {
            int timeoutsecs = (int)SimTimeoutNumericUpDown.Value;
            if (infinite = (timeoutsecs == -1)) return Timeout.InfiniteTimeSpan;
            else return new TimeSpan(0, 0, timeoutsecs);
        }
        private void ResetCPUToSimulationRunReadyState()
        {
            SimuRunner.ResetCPU(SimulatedCPU, SimuRunner.ResetUnit.Core | SimuRunner.ResetUnit.Registers | SimuRunner.ResetUnit.RAM);
            ActiveCPUView.UpdateBindings();
            ActiveCPUView.ClearAfterReset();
        }
        private void SetSimStatus(SimulationStatus status)
        {
            SimStatus = status;
            SimStatusToolStripLabel.Text = status.ToString();
            SimStatusToolStripLabel.BackColor = SimStatusColors[status];
        }
        
        private void SimTimeTimer_Tick(object sender, EventArgs e)
        {
            SimTimeToolStripStatusLabel.Text = SimuRunner.GetStopwatchEnlapsedTimespan().ToString(@"hh\:mm\:ss");
            SimSpeedStatusStripLabel.Text = (SimuRunner.SimulationSpeed.ToString() + "Hz");
        }

        private void RefreshAllCPUUserInterfaceComponents()
        {
            ActiveCPUView.UpdateBindings();
            ((RegisterFileView)GetFormOrNull(RegisterFileForm))?.RefreshAll();
            ((MemoryViewer)GetFormOrNull(MemoryViewerForm))?.RefreshMemoryView(ram: true, rom: false, romHighlight:true);
        }
        private void SetSimulationStatusLabels(bool active)
        {
            ToolStripStatusLabel[] labels = new ToolStripStatusLabel[] { SimTimeoutToolStripStatusLabel, SimTimeToolStripStatusLabel, SimSpeedStatusStripLabel };
            if (active) 
            {
                SimTimeToolStripStatusLabel.Text = SimuRunner.GetStopwatchEnlapsedTimespan().ToString(@"hh\:mm\:ss");
                SimSpeedStatusStripLabel.Text = "?????Hz";
                TimeSpan timeout = GetUserSimulationTimeout(out bool infTimeout);
                SimTimeoutToolStripStatusLabel.Text = (infTimeout ? "Inf." : timeout.ToString(@"hh\:mm\:ss"));
                SimTimeoutToolStripStatusLabel.ToolTipText = TOOLTIP_TIMEOUT_LABEL;
                SimTimeToolStripStatusLabel.ToolTipText = TOOLTIP_SIMTIME_LABEL;
                SimSpeedStatusStripLabel.ToolTipText = TOOLTIP_SIMSPEED_LABEL;
            } 
            else 
            {
                Array.ForEach(labels, l => l.ToolTipText = string.Empty);
                Array.ForEach(labels, l => l.Text = "      ");
            }
            Array.ForEach(labels, l => l.Enabled = active);
        }

        private void SetControlsEnabled_ResetCPUToolStripItems(bool enabled)
        {
            resetToolStripMenuItem.Enabled = enabled;
            foreach (ToolStripItem item in resetToolStripMenuItem.DropDownItems)
                item.Enabled = enabled;
            foreach (ToolStripItem item in resetMemoryToolStripMenuItem.DropDownItems)
                item.Enabled = enabled;
        }

        #region Simulator Task Finish Callbacks
        private void SimulationFinishedExecutionCallback()
        {
            SimuRunner.PauseStopwatch();
        }

        private void SimulationCancelledByPauseCallback()
        {
            if (SimuRunner.GetStopwatchEnlapsedTimespan() >= GetUserSimulationTimeout(out _))
                MessageBox.Show(this, "Simulation timed out! Runtime paused.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else 
                MessageBox.Show(this, "Simulation paused.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SimulationEndedCallback()
        {
            Invoke(new EventHandler(PauseSimulationToolStripButton_Click), null, null); // PAUSE not pressed so invoke to update GUI
            if (SimuRunner.CheckBreakPoint()) {
                MessageBox.Show("Breakpoint Hit! Simulation paused.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } 
            else {
                MessageBox.Show("Simulation paused due to EBREAK instruction execution.", 
                                "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void SimulationErrorCallback(Exception ex)
        {
            Invoke(new EventHandler(PauseSimulationToolStripButton_Click), null, null); // PAUSE not pressed so invoke to update GUI
            Exception @base = ex.InnerException is null ? null : ex.GetBaseException();
            string baseMsg = @base is null ? string.Empty : (": " + @base.Message);
            MessageBox.Show("Core execution error \"" + ex.Message + "\"" + baseMsg, "Error type: " + (@base??ex).GetType().Name,
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion

        #region Top Status Strip Controls Handlers
        private void ExecuteSetOfStepsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int steps = 0;
            if (sender == Execute10StepsButton) steps = 10;
            else if (sender == Execute100StepsButton) steps = 100;
            else if (sender == Execute1000StepsButton) steps = 1000;
            steps *= 2; // Cycle/Latch separate

            try 
            { 
                for (int i = 0; i < steps; i++)  
                {
                    SimuRunner.CycleOrLatch(SimulatedCPU);
                    SetCycleLatchStepToolStripButtonsBaseOnNextEdge();
                }
                RefreshAllCPUUserInterfaceComponents();
            } 
            catch (Exception ex) 
            {
                MessageBox.Show($"Core execution error after {SimuRunner.GetNextEdge(SimulatedCPU).ToString().ToLower()} edge: " + ex.Message,
                "Error type: " + ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CycleToolStrip_Click(object sender, EventArgs e)
        {
            try
            {
                if (sender == CycleHalfToolStripButton)
                {
                    SimuRunner.CycleOrLatch(SimulatedCPU);
                    SetCycleLatchStepToolStripButtonsBaseOnNextEdge();
                } 
                else if (sender == CycleLatchToolStripButton)
                { 
                    SimuRunner.CycleAndLatch(SimulatedCPU);                    
                }
                RefreshAllCPUUserInterfaceComponents();
            } 
            catch (Exception ex)
            {
                MessageBox.Show($"Core execution error after {SimuRunner.GetNextEdge(SimulatedCPU).ToString().ToLower()} edge: " + ex.Message,
                                "Error type: " + ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RunSimulationToolStripButton_Click(object sender, EventArgs e)
        {
            if (SimStatus == SimulationStatus.RUNNING) {
                System.Media.SystemSounds.Beep.Play();
                return;
            }

            RunSimulationToolStripButton.Enabled = false;   
            PauseSimulationToolStripButton.Enabled = true;
            StopAndResetSimulationToolStripButton.Enabled = true;
            CycleHalfToolStripButton.Enabled = false;
            CycleLatchToolStripButton.Enabled = false;
            DropDownButtonProgramMemory.Enabled = false;
            SettingsToolStripButton.Enabled = false;
            SetControlsEnabled_ResetCPUToolStripItems(enabled:false);

            ActiveCPUView.DetachSimEventHandlers();
            SimuRunner.OnSimulationTaskFinishesExecution = SimulationFinishedExecutionCallback;
            SimuRunner.OnSimulationCancelled = SimulationCancelledByPauseCallback;
            SimuRunner.OnSimulationEndItself = SimulationEndedCallback;
            SimuRunner.OnSimulationError = SimulationErrorCallback;

            // Init simulation
            long breakpointAddr = (long)BreakpointAddrNumericUpDown.Value;
            long breakpointCycle = (long)BreakpoinCycleNumericUpDown.Value;
            TimeSpan timeout = GetUserSimulationTimeout(out _);
            SimuRunner.InitSimulation(SimulatedCPU, breakpointAddr, breakpointCycle, timeout, out _);
            if (SimStatus == SimulationStatus.PAUSED) { SimuRunner.RensumeStopwatch(); }
            else if (SimStatus == SimulationStatus.READY) { SimuRunner.StartNewStopwatch(); }
            SimuRunner.RunInitializedSimulation();

            // Update GUI
            
            SimTimeUpdateDisplayTimer.Start();
            SetSimulationStatusLabels(active: true); 
            SetSimStatus(SimulationStatus.RUNNING);
            ShowShortStatusMessage("Simulation running.");
        }

        private void PauseSimulationToolStripButton_Click(object sender, EventArgs e)
        {
            if (sender != null && SimStatus != SimulationStatus.RUNNING)
            {
                System.Media.SystemSounds.Beep.Play();
                return;
            }

            RunSimulationToolStripButton.Text = SIM_RESUME_BUTTON_TEXT;
            RunSimulationToolStripButton.Enabled = true;
            PauseSimulationToolStripButton.Enabled = false;
            StopAndResetSimulationToolStripButton.Enabled = true;
            CycleHalfToolStripButton.Enabled = true;
            CycleLatchToolStripButton.Enabled = true;

            SimTimeUpdateDisplayTimer.Stop();
            SimSpeedStatusStripLabel.Text = "  0Hz";

            if (sender != null && false == SimuRunner.CancelSimulation(wait_ms: 500)) {
                MessageBox.Show("Warning: Cancelation not requested!", "Invalid state");
            }
                
            ActiveCPUView.AttatchSimEventHandlers();
            RefreshAllCPUUserInterfaceComponents();
            
            SetSimStatus(SimulationStatus.PAUSED);
            ShowShortStatusMessage($"Simulation paused. Effective speed: {SimuRunner.SimulationSpeed}Hz");
        }

        private void StopStripStatusLabel_Click(object sender, EventArgs e)
        {
            if (SimStatus != SimulationStatus.PAUSED) {
                PauseSimulationToolStripButton.PerformClick(); // Should set SimStatus.PAUSED
            }
            if (SimStatus != SimulationStatus.PAUSED) {
                MessageBox.Show("Warning: Simulation not paused!", "Invalid State");
                return;
            }
            string commitedmsg = string.Empty;
            if (SimulatedCPU.ReportGenerator.CreateReport != Report.None) {
                if (SimulatedCPU.ReportGenerator.FinalizeAllEnabledLogFiles()){
                    commitedmsg += "Enabled logs listing finalized. ";
                }
            }
            DropDownButtonProgramMemory.Enabled = true;
            RunSimulationToolStripButton.Text = SIM_RUN_BUTTON_TEXT;
            StopAndResetSimulationToolStripButton.Enabled = false;
            SettingsToolStripButton.Enabled = true;
            SetControlsEnabled_ResetCPUToolStripItems(enabled: true);
            SetSimulationStatusLabels(active: false);

            ResetCPUToSimulationRunReadyState();
            RefreshAllCPUUserInterfaceComponents();
            SetSimStatus(SimulationStatus.READY);
            ShowShortStatusMessage(commitedmsg+"CPU state, registers and RAM reset.");
        }

        private void RefreshToolStripStatusLabel_Click(object sender, EventArgs e)
        {
            RefreshAllCPUUserInterfaceComponents();
            ShowShortStatusMessage("Pipeline view updated.");
        }


        #endregion

        #endregion

        #region Experiment Methods

        private IGenericCPUView<ICPU> GetCpuView(ICPU core)
        {
            if (core is ScalarCPU) { return (scalarCoreView); } 
            else if (core is SuperscalarCPU) { return (superscalarCoreView); }
            else { return null; }
        }
        private void ExperimentRunner_NewRunStarting(object sender, EventArgs e)
        {
            var cpuView =  GetCpuView(ExperimentRunner.CurrentCore);
            cpuView.AttatchSimEventHandlers();
            if (superscalar_arch_sim.Simulis.Settings.SettingsChanged)
            {
                cpuView.InitControlData(SimulatedCPU);
                superscalar_arch_sim.Simulis.Settings.SettingsChanged = false;
            }
            RefreshAllCPUUserInterfaceComponents();
        }
        private void ExperimentRunner_NewRunStarted(object sender, EventArgs e)
        {
            GetCpuView(ExperimentRunner.CurrentCore).DetachSimEventHandlers();
            SetControlsEnabled_ResetCPUToolStripItems(enabled: false);
            Thread.Sleep(1);
        }
        private void RunExperiment(string path)
        {
            if (SimRunExperimentDelayed != null)
            {
                SimRunExperimentDelayed.Stop();
                SimRunExperimentDelayed.Dispose();
                SimRunExperimentDelayed = null;
            }
            CloseAllForms();

            ResetCPUToSimulationRunReadyState();
            SimuRunner.ResetCPU(ScalarCPU, SimuRunner.ResetUnit.All);
            SimuRunner.ResetCPU(SuperscalarCPU, SimuRunner.ResetUnit.All);

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                ExperimentRunner.SetupExperiment(fs, ScalarCPU, SuperscalarCPU);

            string exmsg = null;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                while (ExperimentRunner.RunStep())
                {
                    if (Console.KeyAvailable && Console.ReadKey().Key == ConsoleKey.Q)
                        break;
                }
            } 
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                exmsg = ex.Message;
                if (ex is ExperimentRunner.ExperimentStateError expError)
                    exmsg += $" throws during {expError.ErrorState} at line: {expError.ErrorLine}";
            }

            int beeps = ExperimentRunner.ExperimentInitFlagSet(ExperimentRunner.INIT_FLAG_SOUND) ? 5 : 0;
            for (int i = 0; i < beeps; i++) { Console.Beep(); Thread.Sleep(500); }
            
            if (exmsg != null)
            {
                MessageBox.Show("Error while running experiment: " + exmsg,
                    $"Experiment Error after {stopwatch.Elapsed}",
                    MessageBoxButtons.OK, MessageBoxIcon.Error
                );
            } 
            else
            {
                string error = "";
                MessageBoxIcon icon = MessageBoxIcon.Information;
                if (ExperimentRunner.LastRunError != null)
                {
                    icon = MessageBoxIcon.Error;
                    error = $" | {ExperimentRunner.LastRunError.Cause.GetType().Name} \"{ExperimentRunner.LastRunError.Message}\": {ExperimentRunner.LastRunError.Cause}";
                }
                string details = $"{ExperimentRunner.State}{error}";
                string info = $"Experiment finished. Details: {details}";
                string title = $"Finished afer : {stopwatch.Elapsed}";
                MessageBox.Show(info, title, MessageBoxButtons.OK, icon);
                Console.WriteLine(title + " -> " + info); // breakpointAddr can be placed here before Clear()
            }
            GetCpuView(ExperimentRunner.CurrentCore)?.AttatchSimEventHandlers();
            SetControlsEnabled_ResetCPUToolStripItems(enabled: true);
            if (ExperimentRunner.State != ExperimentRunner.ExperimentState.Error && exmsg is null)
            {
                SimuRunner.ResetCPU(ScalarCPU, SimuRunner.ResetUnit.All);
                SimuRunner.ResetCPU(SuperscalarCPU, SimuRunner.ResetUnit.All);
                ExperimentRunner.Clean();
            }
            stopwatch.Reset();
            RefreshAllCPUUserInterfaceComponents();
        }

        #endregion

        #region CPU State dump
        private (UInt32?, UInt32?) CreateShowAskRangeForm(ICPU cpu, uint defaultStart, uint defaultCount = 0x1000)
        {
            using (Form range = new Form())
            {
                range.Text = "Memory Range Selection";
                range.TopMost = true;
                range.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                range.Size = new Size(300, 140);

                var lblStart = new Label() { Text = "Start Address: ", AutoSize = true, Location = new Point(20, 10) };
                var numStart = new NumericUpDown()
                {
                    Location = new Point(120, 10),
                    Width = 150,
                    Minimum = 0,
                    Maximum = cpu.RAM.LastAddress,
                    Increment = sizeof(UInt32),
                    Hexadecimal = true,
                    Value = defaultStart
                };

                var lblCount = new Label() { Text = "Count:", AutoSize = true, Location = new Point(20, 35) };
                var numCount = new NumericUpDown()
                {
                    Location = new Point(120, 35),
                    Width = 150,
                    Minimum = sizeof(UInt32),
                    Maximum = cpu.RAMSize,
                    Increment = sizeof(UInt32),
                    Hexadecimal = true,
                    Value = defaultCount
                };

                var btnOK = new Button() { Text = "OK", DialogResult = DialogResult.Yes, Location = new Point(70, 70), Width = 70 };
                var btnCancel = new Button() { Text = "Cancel", DialogResult = DialogResult.Cancel, Location = new Point(160, 70), Width = 70 };

                range.Controls.Add(lblStart);
                range.Controls.Add(numStart);
                range.Controls.Add(lblCount);
                range.Controls.Add(numCount);
                range.Controls.Add(btnOK);
                range.Controls.Add(btnCancel);

                range.AcceptButton = btnOK;
                range.CancelButton = btnCancel;

                return (range.ShowDialog() == DialogResult.Yes) ? ((UInt32?)numStart.Value, (UInt32?)numCount.Value) : (null, null);
            }
        }
        private void MemoryDumpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SimulatedCPU is null || SimStatus == SimulationStatus.RUNNING)
            {
                ShowShortStatusMessage("Cannot dump registers during simulation run!", msduration: 1000);
                return;
            }
            UInt32[] GetData(UInt32 start, UInt32 count)
            {
                UInt32[] data = new UInt32[count/sizeof(UInt32)]; int i = 0;
                for (UInt32 addr = start; addr < (start + count); addr+=sizeof(UInt32))
                    data[i++] = SimulatedCPU.MMU.ReadWord(addr);
                return data;
            }
            UInt32? numStartValue; UInt32? numStartCount;
            (numStartValue, numStartCount) = CreateShowAskRangeForm(SimulatedCPU, SimulatedCPU.RAMStart);
            if (numStartValue.HasValue && numStartCount.HasValue)
            {
                string file = $"memdump-{UserFilesController.ShortDateTimeNowFilename}.txt";
                string path = UserFilesController.AskForSaveFilePath(file, string.Empty);
                if (false == string.IsNullOrEmpty(path))
                {
                    UInt32[] data = GetData(numStartValue.Value, numStartCount.Value);
                    string datastr = "";
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (i % 8 == 0) datastr += $"{Environment.NewLine}{i + numStartValue:X8}: ";
                        datastr += $"{data[i]:X8}";
                    }
                    datastr = datastr.Remove(0, Environment.NewLine.Length);
                    if (UserFilesController.SaveFile(path, datastr))
                        ShowShortStatusMessage($"File saved: {path}");
                }
            }
        }
        private void RegistersDumpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SimulatedCPU is null || SimStatus == SimulationStatus.RUNNING)
            {
                ShowShortStatusMessage("Cannot dump registers during simulation run!", msduration: 1000);
                return;
            }

            string file = $"regfile-{UserFilesController.ShortDateTimeNowFilename}.txt";
            string path = UserFilesController.AskForSaveFilePath(file, string.Empty);
            if (false == string.IsNullOrEmpty(path))
            {
                string data = "";
                foreach (superscalar_arch_sim.RV32.Hardware.Register.Register32 register in SimulatedCPU.RegisterFile)
                    data += (register.ToString() + Environment.NewLine);

                if(UserFilesController.SaveFile(path, data))
                    ShowShortStatusMessage($"File saved: {path}");
            }
        }
        #endregion
    }
}
