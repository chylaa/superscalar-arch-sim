using superscalar_arch_sim.RV32.Hardware.CPU;
using superscalar_arch_sim.RV32.Hardware.Units;
using superscalar_arch_sim.Simulis.Reports;
using superscalar_arch_sim_gui.Utilis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using CoreSettings = superscalar_arch_sim.Simulis.Settings;

namespace superscalar_arch_sim_gui.Forms
{
    public partial class SimSettings : Form
    {
        public const int MAX_FUNCTIONAL_UNITS = 32;
        public const int MAX_RESERVATION_STATIONS = 128;
        public const int MAX_ROB_ENTRIES = (MAX_FUNCTIONAL_UNITS * MAX_RESERVATION_STATIONS);
        public const int MAX_INSTRUCTION_QUEUE_CAPACITY = (MAX_FUNCTIONAL_UNITS * MAX_RESERVATION_STATIONS);

        public const string DefaultLogpath = "..\\..\\..\\logs\\";

        private List<CheckBox> DataCollectionCheckBoxes = null;
        /// <summary>Invoked on <see cref="FormClosing"/> if changes were applied. </summary>
        public event EventHandler SettingsChangedCallback;

        private readonly Control[] StaticCoreOnlyControls;
        private readonly Control[] DynamicCoreOnlyControls;

        public ICPU Core { get; }

        public SimSettings(ICPU core)
        {
            InitializeComponent();
            Load += SimSettings_Load;
            checkBoxEnableSimReport.CheckedChanged += CheckBoxEnableSimReport_CheckedChanged;
            checkBoxCreateRaport.CheckedChanged += CheckBoxCreateRaport_CheckedChanged;
            MaxIssuesPerClockNumUpDown.ValueChanged += MaxIssuesPerClockNumUpDown_ValueChanged;
            MaxIssuesPerClockNumUpDown.Minimum = 1;

            BranchRSNumUpDown.ValueChanged += RSNumUpDown_ValueChanged;
            MEMRSNumUpDown.ValueChanged += RSNumUpDown_ValueChanged;
            INTRSNumUpDown.ValueChanged += RSNumUpDown_ValueChanged;

            MEMFUNumUpDown.Maximum = INTFUNumUpDown.Maximum = MAX_FUNCTIONAL_UNITS;
            MEMRSNumUpDown.Maximum = INTRSNumUpDown.Maximum = MAX_RESERVATION_STATIONS;
            RobEntriesNumUpDown.Maximum = MAX_ROB_ENTRIES;
            InstrQueueCapNumUpDown.Maximum = MAX_INSTRUCTION_QUEUE_CAPACITY;

            var addrNumUpDowns = GUIUtilis.GetAllDirectChildrenOfType<NumericUpDown>(AddressSpaceGroupBox);
            addrNumUpDowns.ForEach(nud => nud.Maximum = new decimal(UInt32.MaxValue));
            addrNumUpDowns.ForEach(nud => nud.Increment = sizeof(UInt32));

            MaximizeBox = false;
            checkBoxEnableSimReport.Checked = true;
            Core = core;

            StaticCoreOnlyControls = new Control[] { StaticForwardingEnabledCheckBox };
            DynamicCoreOnlyControls = new Control[] { DynamicCoreGroupBox, DynamicCoreUnitsGroupBox, DynamicStoreLoadBypassCheckBox, DynamicAllowSpeculativeLoads, DynamicWriteCDBInExecute };

            Report[] reportValues = (Report[])Enum.GetValues(typeof(Report));
            for (int i = 0; i < reportValues.Length; i++)
            {
                var report = reportValues[i];
                checkedListBox1.Items.Add(TextFormatting.SeparateCamelCaseWords(report.ToString()));
                checkedListBox1.SetItemChecked(i, (Core.ReportGenerator.CreateReport.HasFlag(report)));
            }
            checkedListBox1.Items.Remove(TextFormatting.SeparateCamelCaseWords(Report.None.ToString()));
            checkedListBox1.CheckOnClick = true;

            tabControl1.SelectedIndexChanged += delegate {
                ImportButton.Enabled = (tabControl1.TabPages[tabControl1.SelectedIndex] == CoreSettingsTabPage);
            };
            tabControl1.MouseDoubleClick += TabControl1_MouseDoubleClick;
        }

        #region View Initialization
        private void SimSettings_Load(object sender, EventArgs e)
        {
            CoreSettings.SettingsChanged = false;
            InitializeViewFromSettings();
        }
        private void InitializeViewFromSettings()
        {
            void SetRoundToNearestMultiple(NumericUpDown rs, NumericUpDown fu, int init) {
                rs.Increment = init;
                fu.ValueChanged += delegate
                {
                    rs.Increment = fu.Value;
                    int newVal = superscalar_arch_sim.Utilis.Utilis.NearestMultiple((double)rs.Value, (double)fu.Value);
                    rs.Value = Math.Min(newVal, rs.Maximum);
                };
            }

            DataCollectionCheckBoxes = GUIUtilis.RecursivelyGetAllChildrenOfType<CheckBox>(groupBoxDataCollection);
            InitSimAddressSpaceView();
            InitBranchPredictorSettingsView();
            EnableCoreDependentViews((Core is ScalarCPU));
            InitDynamicCoreView();
            checkBoxCreateRaport.Checked = (false == string.IsNullOrEmpty(Core.ReportGenerator.LogRecordsMainFolder));
            CheckBoxCreateRaport_CheckedChanged(checkBoxCreateRaport, null); // force update

            SetRoundToNearestMultiple(INTRSNumUpDown, INTFUNumUpDown, CoreSettings.NumberOfIntegerFunctionalUnits);
            SetRoundToNearestMultiple(MEMRSNumUpDown, MEMFUNumUpDown, CoreSettings.NumberOfMemoryFunctionalUnits);

            InOrderExecRadioButton.CheckedChanged += delegate { if (InOrderExecRadioButton.Checked) DynamicStoreLoadBypassCheckBox.Checked = false; };
        }
        private void EnableCoreDependentViews(bool staticCore)
        {
            Array.ForEach(StaticCoreOnlyControls, c => c.Enabled = staticCore);
            Array.ForEach(DynamicCoreOnlyControls, c => c.Enabled = (false == staticCore));
        }
        private void InitSimAddressSpaceView()
        {
            ROMOriginNumUpDown.Value = CoreSettings.OriginROMAddress;
            RAMOriginNumUpDown.Value = CoreSettings.OriginRAMAddress;
            ROMSizeNumUpDown.Value = CoreSettings.ROMBytesLength;
            RAMSizeNumUpDown.Value = CoreSettings.RAMBytesLength;
        }
        private void InitBranchPredictorSettingsView()
        {
            if (Directory.Exists(DefaultLogpath))
                textBoxLoggingFolderPath.Text = new DirectoryInfo(DefaultLogpath).FullName;
            string defaultDir = Path.GetDirectoryName(DefaultLogpath);
            if (Directory.Exists(defaultDir))
                textBoxLoggingFolderPath.Text = new DirectoryInfo(defaultDir).FullName;

            GUIUtilis.AddEnumRangeToComboBox(PredictionSchemeComboBox , selected: CoreSettings.UsedPredictionScheme);
            GUIUtilis.AddEnumRangeToComboBox(StaticPredictionStrategyComboBox, selected: CoreSettings.StaticPrediction, skip: BranchPredictor.Prediction.None);
            GUIUtilis.AddEnumRangeToComboBox(FSMModeComboBox, selected: CoreSettings.UsedFMSScheme);

            BTBEntriesBitsNumUpDown.Value = CoreSettings.BitsOfEntriesInBranchTargetBuffer;
            FSMPredBitsNumUpDown.Value = CoreSettings.NumberOfPredictionBitsForFSMs;
            BHTEntriesBitsNumUpDown.Value = CoreSettings.BitsOfEntriesInBranchHistoryTable;
            BHTEntryInitValNumUpDown.Value = CoreSettings.InitialValueOfFSMinBranchHistTable;
            PHTPatternBitsNumUpDown.Value = CoreSettings.NumberOfHistoryBitsInPatternHistoryTable;
            PHTEntryInitValNumUpDown.Value = CoreSettings.InitialBranchPatternSequence;

            BranchPredictionEnabledCheckBox.Checked = CoreSettings.BranchPredictionEnabled;
            StaticPredictionStrategyComboBox.SelectedItem = CoreSettings.StaticPrediction;
            StaticForwardingEnabledCheckBox.Checked = CoreSettings.Static_UseForwarding;
            DynamicStoreLoadBypassCheckBox.Checked = CoreSettings.Dynamic_BypassStoreLoad;
            DynamicAllowSpeculativeLoads.Checked = CoreSettings.Dynamic_AllowSpeculativeLoads;
            DynamicWriteCDBInExecute.Checked = CoreSettings.Dynamic_WriteCommonDataBusFromExecute;
            DynamicIgnoreIllegal.Checked = CoreSettings.Dynamic_DispatchIgnoreIllegalInstructions;
        }

        private void InitDynamicCoreView()
        {
            OOOExecRadioButton.Checked = CoreSettings.CoreMode == CoreSettings.DynamicCoreMode.OutOfOrderExecution;
            InOrderExecRadioButton.Checked = CoreSettings.CoreMode == CoreSettings.DynamicCoreMode.InOrderExecution;

            RobEntriesNumUpDown.Value = CoreSettings.NumberOfReorderBufferEntries;

            BranchRSNumUpDown.Value = CoreSettings.NumberOfBranchReservationStations;
            MEMRSNumUpDown.Value    = CoreSettings.NumberOfMemoryReservationStationBuffers;
            INTRSNumUpDown.Value    = CoreSettings.NumberOfIntegerReservationStations;
            FPRSNumUpDown.Value     = 0;

            BranchFUNumUpDown.Value = CoreSettings.NumberOfBranchFunctionalUnits;
            MEMFUNumUpDown.Value = CoreSettings.NumberOfMemoryFunctionalUnits;
            INTFUNumUpDown.Value = CoreSettings.NumberOfIntegerFunctionalUnits;
            FPFUNumUpDown.Value = CoreSettings.NumberOfFPFunctionalUnits;

            MaxIssuesPerClockNumUpDown.Value = CoreSettings.MaxIssuesPerClock;
            InstrQueueCapNumUpDown.Value = CoreSettings.InstructionQueueCapacity;
            MaxFetchesPerClockNumUpDown.Value = CoreSettings.FetchWidth;
        }
        #endregion

        #region Apply Settings
        private void ApplySimAddressSpaceSettings()
        {
            CoreSettings.OriginROMAddress = (UInt32)ROMOriginNumUpDown.Value;
            CoreSettings.OriginRAMAddress = (UInt32)RAMOriginNumUpDown.Value;
            CoreSettings.ROMBytesLength = (UInt32)ROMSizeNumUpDown.Value;
            CoreSettings.RAMBytesLength = (UInt32)RAMSizeNumUpDown.Value;
        }
        private void ApplyBranchPredictorSettings()
        {
            CoreSettings.BranchPredictionEnabled = BranchPredictionEnabledCheckBox.Checked;

            CoreSettings.UsedPredictionScheme = (BranchPredictor.PredictionScheme)PredictionSchemeComboBox.SelectedItem;
            CoreSettings.StaticPrediction = (BranchPredictor.Prediction)(StaticPredictionStrategyComboBox.SelectedItem ?? BranchPredictor.Prediction.None);
            CoreSettings.UsedFMSScheme = (BranchPredictor.FSMScheme)FSMModeComboBox.SelectedItem;

            CoreSettings.BitsOfEntriesInBranchTargetBuffer = decimal.ToUInt32(BTBEntriesBitsNumUpDown.Value);
            CoreSettings.NumberOfPredictionBitsForFSMs = decimal.ToUInt32(FSMPredBitsNumUpDown.Value);
            CoreSettings.BitsOfEntriesInBranchHistoryTable = decimal.ToUInt32(BHTEntriesBitsNumUpDown.Value);
            CoreSettings.InitialValueOfFSMinBranchHistTable = decimal.ToUInt32(BHTEntryInitValNumUpDown.Value);
            CoreSettings.NumberOfHistoryBitsInPatternHistoryTable = decimal.ToUInt32(PHTPatternBitsNumUpDown.Value);
            CoreSettings.InitialBranchPatternSequence = decimal.ToUInt32(PHTEntryInitValNumUpDown.Value);
        }
        private void ApplyDynamicCoreSettings()
        {
            CoreSettings.CoreMode = 
                OOOExecRadioButton.Checked 
                ? CoreSettings.DynamicCoreMode.OutOfOrderExecution 
                : CoreSettings.DynamicCoreMode.InOrderExecution;

            CoreSettings.NumberOfReorderBufferEntries = (int)RobEntriesNumUpDown.Value;

            CoreSettings.NumberOfBranchReservationStations = (int)BranchRSNumUpDown.Value;
            CoreSettings.NumberOfMemoryReservationStationBuffers = (int)MEMRSNumUpDown.Value;
            CoreSettings.NumberOfIntegerReservationStations = (int)INTRSNumUpDown.Value;

            CoreSettings.NumberOfIntegerFunctionalUnits = (int)INTFUNumUpDown.Value;
            CoreSettings.NumberOfMemoryFunctionalUnits = (int)MEMFUNumUpDown.Value;

            CoreSettings.MaxIssuesPerClock = (int)MaxIssuesPerClockNumUpDown.Value;
            CoreSettings.InstructionQueueCapacity = (int)InstrQueueCapNumUpDown.Value;
            CoreSettings.FetchWidth = (int)MaxFetchesPerClockNumUpDown.Value;
        }
        private void SetCollectingInstructionsRecord()
        {
            Core.SimReport.Enabled = checkBoxEnableSimReport.Checked;
            if (checkBoxCreateRaport.Checked)
            {
                string path = textBoxLoggingFolderPath.Text;
                if (false == string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
                {
                    Core.ReportGenerator.LogRecordsMainFolder = path;
                    Core.ReportGenerator.IncludeClockCycleInDataWriteLog = checkBoxInclClockInDatalog.Checked;
                }
            } 
            else
            {
                Core.ReportGenerator.LogRecordsMainFolder = null;
            }
        }
        private void ApplyReportSettings()
        {
            SetCollectingInstructionsRecord();
            Report enabledReports = Report.None;
            foreach (string item in checkedListBox1.CheckedItems) 
            {
                if (Enum.TryParse(item.Replace(" ", string.Empty), out Report report))
                {
                    enabledReports |= report;
                }
            }
            Core.ReportGenerator.CreateReport = enabledReports;
        }
        private void ApplySettings()
        {
            ApplySimAddressSpaceSettings();
            ApplyBranchPredictorSettings();
            ApplyDynamicCoreSettings();
            ApplyReportSettings();
            CoreSettings.Static_UseForwarding = StaticForwardingEnabledCheckBox.Checked;
            CoreSettings.Dynamic_BypassStoreLoad = DynamicStoreLoadBypassCheckBox.Checked;
            CoreSettings.Dynamic_AllowSpeculativeLoads = DynamicAllowSpeculativeLoads.Checked;
            CoreSettings.Dynamic_WriteCommonDataBusFromExecute = DynamicWriteCDBInExecute.Checked;
            CoreSettings.Dynamic_DispatchIgnoreIllegalInstructions = DynamicIgnoreIllegal.Checked;
            CoreSettings.SettingsChanged = true;
        }
        private void ResetView()
        {
            Core.Reset(preserveMemory: true);
            ApplyReportSettings();// after Core.Reset!
            SettingsChangedCallback?.Invoke(null, EventArgs.Empty);
            ExportButton.Enabled = true;
        }

        #endregion

        #region Settings file
        public void ImportSettings(string path)
        {
            if (CoreSettings.ImportSettings(path))
            {
                if (CoreSettings.ValidateSettings())
                {
                    InitializeViewFromSettings();
                    ResetView();
                    MessageBox.Show($"Settings loaded from {Path.GetFileName(path)} file", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"Invalid settings detected in {Path.GetFileName(path)} file!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            } 
            else
            {
                MessageBox.Show($"Failed to load settings from {Path.GetFileName(path)} file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void ExportSettings(string path)
        {
            if (false == CoreSettings.ValidateSettings())
            {
                MessageBox.Show($"Invalid settings detected!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (CoreSettings.ExportSettings(path))
            {
                MessageBox.Show($"Settings saved to {Path.GetFileName(path)} file", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } 
            else
            {
                MessageBox.Show($"Failed to save settings to {Path.GetFileName(path)} file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Event Handlers
        private void ApplySettingsButton_Click(object sender, EventArgs e)
        {
            ExportButton.Enabled = false;
            if (DialogResult.Yes == MessageBox.Show("Changing settings require core reset. Reset simulation?", "Reset required",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Information, defaultButton:MessageBoxDefaultButton.Button2))
            {
                ApplySettings();
                ResetView();
            }
        }
        private void CheckBoxCreateRaport_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxInclClockInDatalog.Enabled = checkBoxCreateRaport.Checked;
            checkBoxInclClockInDatalog.Checked = checkBoxCreateRaport.Checked && Core.ReportGenerator.IncludeClockCycleInDataWriteLog;
            checkedListBox1.Enabled = checkBoxCreateRaport.Checked;
        }
        private void CheckBoxEnableSimReport_CheckedChanged(object sender, EventArgs e)
        {
            if (false == (groupBoxDataCollection.Enabled = checkBoxEnableSimReport.Checked))
            {
                foreach (var checkbox in DataCollectionCheckBoxes)
                    checkbox.Checked = false;
            }
        }
        private void buttonBrowseFolder_Click(object sender, EventArgs e)
        {
            string folderpath = (UserFilesController.AskForFolder() ?? string.Empty);
            if (false == string.IsNullOrWhiteSpace(folderpath) && false == folderpath.EndsWith("\\"))
                folderpath += "\\";
            textBoxLoggingFolderPath.Text = folderpath;
        }
        private void buttonBrowseFile_Click(object sender, EventArgs e)
        {
            textBoxLoggingFolderPath.Text = (UserFilesController.AskForOpenFilePath(UserFilesController.AllFilesFilter) ?? string.Empty);
        }

        private void checkBoxBranchPredictionEnabled_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (sender as CheckBox);
            GUIUtilis.SetEnableAllDirectChildren(groupBoxBranchPrediction, enabled: cb.Checked, withparent:false);
            cb.Enabled = true;

            if (cb.Checked) {
                PredictionSchemeComboBox_SelectedIndexChanged(null, null);
                UseSingleGlobalFSMCheckBox_CheckedChanged(null, null);
            }
        }

        private void FSMPredBitsNumUpDown_ValueChanged(object sender, EventArgs e)
        {
            BHTEntryInitValNumUpDown.Maximum = (1 << decimal.ToInt32((sender as NumericUpDown).Value));
        }

        private void PHTPatternBitsNumUpDown_ValueChanged(object sender, EventArgs e)
        {
            PHTEntryInitValNumUpDown.Maximum = (1 << decimal.ToInt32((sender as NumericUpDown).Value));
        }
        private void RSNumUpDown_ValueChanged(object sender, EventArgs e)
        {
            RobEntriesNumUpDown.Value = (BranchRSNumUpDown.Value + MEMRSNumUpDown.Value + INTRSNumUpDown.Value);
        }

        private void PredictionSchemeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            BranchPredictor.PredictionScheme item = (BranchPredictor.PredictionScheme)(PredictionSchemeComboBox.SelectedItem);
            if (item == BranchPredictor.PredictionScheme.Static)
            {
                FSMModeComboBox.Enabled = false;
                StaticPredictionStrategyComboBox.Enabled = true;
                FSMgroupBox.Enabled = false;
                BHTgroupBox.Enabled = false;
                PHTgroupBox.Enabled = false;
            }
            else if (item == BranchPredictor.PredictionScheme.BranchHistoryTable)
            {
                FSMModeComboBox.Enabled = true;
                StaticPredictionStrategyComboBox.Enabled = false;
                FSMgroupBox.Enabled = true;
                BHTgroupBox.Enabled = true;
                PHTgroupBox.Enabled = false;

            } 
            else if (item == BranchPredictor.PredictionScheme.AdaptivePredictor)
            {
                FSMModeComboBox.Enabled = true;
                StaticPredictionStrategyComboBox.Enabled = false;
                FSMgroupBox.Enabled = true;
                BHTgroupBox.Enabled = true;
                PHTgroupBox.Enabled = true;
            }
        }

        private void UseSingleGlobalFSMCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (UseSingleGlobalFSMCheckBox.Checked)
            {
                PredictionSchemeComboBox.SelectedItem = BranchPredictor.PredictionScheme.BranchHistoryTable;
                PredictionSchemeComboBox.Enabled = false;
                BHTEntriesBitsNumUpDown.Value = 0;
                BHTEntriesBitsNumUpDown.Enabled = false;
            } 
            else { BHTEntriesBitsNumUpDown.Enabled = BHTgroupBox.Enabled; PredictionSchemeComboBox.Enabled = true; }
        }

        private void MaxIssuesPerClockNumUpDown_ValueChanged(object sender, EventArgs e)
        {
            var totalEu = INTFUNumUpDown.Value + BranchFUNumUpDown.Value + MEMFUNumUpDown.Value + FPFUNumUpDown.Value;
            var value = Math.Min(MaxIssuesPerClockNumUpDown.Value, totalEu);
            MaxIssuesPerClockNumUpDown.Value = value;
        }


        private void ImportButton_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("Import settings will automatically apply settings and perform core reset. Continue?", "Immediate apply warning",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Information, defaultButton: MessageBoxDefaultButton.Button2))
            {
                ImportSettings(UserFilesController.AskForOpenFilePath(UserFilesController.CoreSettingsFilesFilter));
            }
            
        }
       
        private void ExportButton_Click(object sender, EventArgs e)
        {
            string stdFilename = CoreSettings.GetStandardSettingsFilename();
            ExportSettings(UserFilesController.AskForSaveFilePath(stdFilename, UserFilesController.CoreSettingsFilesFilter));
        }
        #region Temp quick settings generation - if leaved should be properly included in UI
        private void TabControl1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Middle || e.Button == MouseButtons.Right) && e.Clicks == 2)
            {
                var order = e.Button == MouseButtons.Middle ? CoreSettings.DynamicCoreMode.OutOfOrderExecution : CoreSettings.DynamicCoreMode.InOrderExecution;
                string dir = "C:\\Users\\Dell\\Desktop\\Magister\\Superscalar_Processor_Simulator\\Project\\experiments\\.settings\\gen-superscalar-core-settings";
                dir += order == CoreSettings.DynamicCoreMode.OutOfOrderExecution ? "\\ooo" : "\\io";
                if (Directory.Exists(dir)) Array.ForEach(Directory.EnumerateFiles(dir, $"*fetch-*{CoreSettings.FileExtension}").ToArray(), file => File.Delete(file));
                else Directory.CreateDirectory(dir);
                int count = GenerateSettingsPermutations(dir, order);
                MessageBox.Show($"Generated {count} different settings files to {dir}", "Settings generation complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public int GenerateSettingsPermutations(string dir, CoreSettings.DynamicCoreMode order)
        {
            dir = (dir[dir.Length - 1] == '\\' ? dir : dir + '\\');
            if ((CoreSettings.CoreMode = order) == CoreSettings.DynamicCoreMode.InOrderExecution)
                CoreSettings.Dynamic_BypassStoreLoad = false;

            CoreSettings.BranchPredictionEnabled = true;
            CoreSettings.UsedPredictionScheme = BranchPredictor.PredictionScheme.BranchHistoryTable;
            CoreSettings.UsedFMSScheme = BranchPredictor.FSMScheme.SaturatingCounter;
            CoreSettings.StaticPrediction = BranchPredictor.Prediction.None;
            CoreSettings.NumberOfHistoryBitsInPatternHistoryTable = 2; // N/A
            CoreSettings.NumberOfPredictionBitsForFSMs = 2; // N/A
            CoreSettings.BitsOfEntriesInBranchHistoryTable = 16;
            CoreSettings.InitialValueOfFSMinBranchHistTable = 0;
            CoreSettings.InitialBranchPatternSequence = 0; // N/A
            // Pipeline Fronted Settings options
            const int FE_SIZE = 5;
            int[] FrontendSet_FetchWidth = new int[FE_SIZE] { 1, 2, 4, 6, 8};
            int[] FrontendSet_InstructionQueueCapacity = new int[FE_SIZE] { 8, 16, 24, 32, 64 };
            // Pipeline Backend Settings options
            const int BE_SIZE = 4;
            int[] BackedSet_MaxIssuesPerClock                   = new int[BE_SIZE] { 01, 02, 04, 08 };
            int[] BackedSet_NumberOfMemoryFunctionalUnits       = new int[BE_SIZE] { 01, 02, 04, 08 };
            int[] BackedSet_NumberOfIntegerFunctionalUnits      = new int[BE_SIZE] { 01, 02, 04, 08 };
            int[] BackedSet_NumberOfBranchReservationStations   = new int[BE_SIZE] { 02, 04, 04, 04 };
            int[] BackedSet_NumberOfMemoryReservationStations   = new int[BE_SIZE] { 04, 08, 16, 32 };
            int[] BackedSet_NumberOfIntegerReservationStations  = new int[BE_SIZE] { 04, 08, 16, 32 };

            int count = 0;
            for (int iFE = 0; iFE < FE_SIZE; iFE++)
            {
                CoreSettings.FetchWidth = FrontendSet_FetchWidth[iFE];
                CoreSettings.InstructionQueueCapacity = FrontendSet_InstructionQueueCapacity[iFE];
                for (int iBE = 0; iBE < BE_SIZE; iBE++)
                {
                    CoreSettings.MaxIssuesPerClock = BackedSet_MaxIssuesPerClock[iBE];
                    CoreSettings.NumberOfMemoryFunctionalUnits = BackedSet_NumberOfMemoryFunctionalUnits[iBE];
                    CoreSettings.NumberOfIntegerFunctionalUnits = BackedSet_NumberOfIntegerFunctionalUnits[iBE];
                    CoreSettings.NumberOfBranchReservationStations = BackedSet_NumberOfBranchReservationStations[iBE];
                    CoreSettings.NumberOfMemoryReservationStationBuffers = BackedSet_NumberOfMemoryReservationStations[iBE];
                    CoreSettings.NumberOfIntegerReservationStations = BackedSet_NumberOfIntegerReservationStations[iBE];
                    CoreSettings.NumberOfReorderBufferEntries = CoreSettings.TotalNumberOfReservationStations;
                    CoreSettings.ExportSettings(dir + CoreSettings.GetStandardSettingsFilename());
                    ++count;
                }
            }
            return count;
        }
        #endregion
        #endregion
    }
}
