namespace superscalar_arch_sim_gui
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing"><see langword="true"></see> if managed resources should be disposed; otherwise, <see langword="false"></see>.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.BottomStatusStrip = new System.Windows.Forms.StatusStrip();
            this.SimStatusToolStripLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.SimTimeToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.SimTimeoutToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.SimSpeedStatusStripLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripShortStatusMessageLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetCoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetRegisterFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetMemoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetRAMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetROMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exporToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.simCountersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.memoryDumpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.registersDumpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.ViewMemoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewRegistersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripSeparator();
            this.DropDownButtonProgramMemory = new System.Windows.Forms.ToolStripDropDownButton();
            this.flashMemoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Execute10StepsButton = new System.Windows.Forms.ToolStripMenuItem();
            this.Execute100StepsButton = new System.Windows.Forms.ToolStripMenuItem();
            this.Execute1000StepsButton = new System.Windows.Forms.ToolStripMenuItem();
            this.SettingsToolStripButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.useABIRegisterNamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FormsToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripStatusLabel5 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripStatusLabel8 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.UpdateViewToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripStatusLabel7 = new System.Windows.Forms.ToolStripLabel();
            this.CycleHalfToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.CycleLatchToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.RunSimulationToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.PauseSimulationToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.StopAndResetSimulationToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripStatusLabel6 = new System.Windows.Forms.ToolStripLabel();
            this.SimTimeoutNumericUpDown = new superscalar_arch_sim_gui.UserControls.CustomControls.ToolStripNumberControl();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.BreakpointAddrNumericUpDown = new superscalar_arch_sim_gui.UserControls.CustomControls.ToolStripNumberControl();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.BreakpoinCycleNumericUpDown = new superscalar_arch_sim_gui.UserControls.CustomControls.ToolStripNumberControl();
            this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this.tabControlCPUViewType = new System.Windows.Forms.TabControl();
            this.tabPageStaticCPU = new System.Windows.Forms.TabPage();
            this.scalarCoreView = new superscalar_arch_sim_gui.UserControls.Core.Static.ScalarCoreView();
            this.tabPageDynamicCPU = new System.Windows.Forms.TabPage();
            this.superscalarCoreView = new superscalar_arch_sim_gui.UserControls.Core.Dynamic.SuperscalarCoreView();
            this.TerminalViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.BottomStatusStrip.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.tabControlCPUViewType.SuspendLayout();
            this.tabPageStaticCPU.SuspendLayout();
            this.tabPageDynamicCPU.SuspendLayout();
            this.SuspendLayout();
            // 
            // BottomStatusStrip
            // 
            this.BottomStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SimStatusToolStripLabel,
            this.toolStripSeparator6,
            this.SimTimeToolStripStatusLabel,
            this.SimTimeoutToolStripStatusLabel,
            this.toolStripSeparator7,
            this.SimSpeedStatusStripLabel,
            this.toolStripSeparator4,
            this.ToolStripShortStatusMessageLabel});
            this.BottomStatusStrip.Location = new System.Drawing.Point(0, 797);
            this.BottomStatusStrip.Name = "BottomStatusStrip";
            this.BottomStatusStrip.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.BottomStatusStrip.ShowItemToolTips = true;
            this.BottomStatusStrip.Size = new System.Drawing.Size(1556, 24);
            this.BottomStatusStrip.TabIndex = 0;
            this.BottomStatusStrip.Text = "statusStrip1";
            // 
            // SimStatusToolStripLabel
            // 
            this.SimStatusToolStripLabel.BackColor = System.Drawing.Color.ForestGreen;
            this.SimStatusToolStripLabel.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.SimStatusToolStripLabel.Name = "SimStatusToolStripLabel";
            this.SimStatusToolStripLabel.Size = new System.Drawing.Size(43, 18);
            this.SimStatusToolStripLabel.Text = "READY";
            this.SimStatusToolStripLabel.ToolTipText = "Simulation status";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 24);
            // 
            // SimTimeToolStripStatusLabel
            // 
            this.SimTimeToolStripStatusLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.SimTimeToolStripStatusLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.SimTimeToolStripStatusLabel.Enabled = false;
            this.SimTimeToolStripStatusLabel.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.SimTimeToolStripStatusLabel.Name = "SimTimeToolStripStatusLabel";
            this.SimTimeToolStripStatusLabel.Size = new System.Drawing.Size(26, 19);
            this.SimTimeToolStripStatusLabel.Text = "     ";
            // 
            // SimTimeoutToolStripStatusLabel
            // 
            this.SimTimeoutToolStripStatusLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.SimTimeoutToolStripStatusLabel.Enabled = false;
            this.SimTimeoutToolStripStatusLabel.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.SimTimeoutToolStripStatusLabel.Name = "SimTimeoutToolStripStatusLabel";
            this.SimTimeoutToolStripStatusLabel.Size = new System.Drawing.Size(32, 19);
            this.SimTimeoutToolStripStatusLabel.Text = "       ";
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 24);
            // 
            // SimSpeedStatusStripLabel
            // 
            this.SimSpeedStatusStripLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.SimSpeedStatusStripLabel.Enabled = false;
            this.SimSpeedStatusStripLabel.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.SimSpeedStatusStripLabel.Name = "SimSpeedStatusStripLabel";
            this.SimSpeedStatusStripLabel.Size = new System.Drawing.Size(38, 19);
            this.SimSpeedStatusStripLabel.Text = "         ";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 24);
            // 
            // ToolStripShortStatusMessageLabel
            // 
            this.ToolStripShortStatusMessageLabel.Name = "ToolStripShortStatusMessageLabel";
            this.ToolStripShortStatusMessageLabel.Size = new System.Drawing.Size(1376, 19);
            this.ToolStripShortStatusMessageLabel.Spring = true;
            this.ToolStripShortStatusMessageLabel.Text = "info";
            this.ToolStripShortStatusMessageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.SystemColors.MenuBar;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1,
            this.toolStripDropDownButton2,
            this.toolStripButton1,
            this.DropDownButtonProgramMemory,
            this.SettingsToolStripButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1556, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetToolStripMenuItem,
            this.exporToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(38, 22);
            this.toolStripDropDownButton1.Text = "File";
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetCoreToolStripMenuItem,
            this.resetRegisterFileToolStripMenuItem,
            this.resetMemoryToolStripMenuItem});
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.resetToolStripMenuItem.Text = "Reset";
            // 
            // resetCoreToolStripMenuItem
            // 
            this.resetCoreToolStripMenuItem.Name = "resetCoreToolStripMenuItem";
            this.resetCoreToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.resetCoreToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.resetCoreToolStripMenuItem.Text = "Core";
            // 
            // resetRegisterFileToolStripMenuItem
            // 
            this.resetRegisterFileToolStripMenuItem.Name = "resetRegisterFileToolStripMenuItem";
            this.resetRegisterFileToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.resetRegisterFileToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.resetRegisterFileToolStripMenuItem.Text = "Register File";
            // 
            // resetMemoryToolStripMenuItem
            // 
            this.resetMemoryToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetRAMToolStripMenuItem,
            this.resetROMToolStripMenuItem});
            this.resetMemoryToolStripMenuItem.Name = "resetMemoryToolStripMenuItem";
            this.resetMemoryToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.resetMemoryToolStripMenuItem.Text = "Memory";
            // 
            // resetRAMToolStripMenuItem
            // 
            this.resetRAMToolStripMenuItem.Name = "resetRAMToolStripMenuItem";
            this.resetRAMToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.A)));
            this.resetRAMToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.resetRAMToolStripMenuItem.Text = "RAM";
            // 
            // resetROMToolStripMenuItem
            // 
            this.resetROMToolStripMenuItem.Name = "resetROMToolStripMenuItem";
            this.resetROMToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.O)));
            this.resetROMToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.resetROMToolStripMenuItem.Text = "ROM";
            // 
            // exporToolStripMenuItem
            // 
            this.exporToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.simCountersToolStripMenuItem,
            this.memoryDumpToolStripMenuItem,
            this.registersDumpToolStripMenuItem});
            this.exporToolStripMenuItem.Name = "exporToolStripMenuItem";
            this.exporToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exporToolStripMenuItem.Text = "Export";
            // 
            // simCountersToolStripMenuItem
            // 
            this.simCountersToolStripMenuItem.Name = "simCountersToolStripMenuItem";
            this.simCountersToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.simCountersToolStripMenuItem.Text = "Sim Counters";
            this.simCountersToolStripMenuItem.Click += new System.EventHandler(this.simCountersToolStripMenuItem_Click);
            // 
            // memoryDumpToolStripMenuItem
            // 
            this.memoryDumpToolStripMenuItem.Name = "memoryDumpToolStripMenuItem";
            this.memoryDumpToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.memoryDumpToolStripMenuItem.Text = "Memory dump";
            this.memoryDumpToolStripMenuItem.Click += new System.EventHandler(this.MemoryDumpToolStripMenuItem_Click);
            // 
            // registersDumpToolStripMenuItem
            // 
            this.registersDumpToolStripMenuItem.Name = "registersDumpToolStripMenuItem";
            this.registersDumpToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.registersDumpToolStripMenuItem.Text = "Registers dump ";
            this.registersDumpToolStripMenuItem.Click += new System.EventHandler(this.RegistersDumpToolStripMenuItem_Click);
            // 
            // toolStripDropDownButton2
            // 
            this.toolStripDropDownButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ViewMemoryToolStripMenuItem,
            this.ViewRegistersToolStripMenuItem,
            this.TerminalViewToolStripMenuItem});
            this.toolStripDropDownButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton2.Image")));
            this.toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            this.toolStripDropDownButton2.Size = new System.Drawing.Size(45, 22);
            this.toolStripDropDownButton2.Text = "View";
            // 
            // ViewMemoryToolStripMenuItem
            // 
            this.ViewMemoryToolStripMenuItem.Name = "ViewMemoryToolStripMenuItem";
            this.ViewMemoryToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.M)));
            this.ViewMemoryToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.ViewMemoryToolStripMenuItem.Text = "Memory";
            this.ViewMemoryToolStripMenuItem.Click += new System.EventHandler(this.ShowFormItem_Click);
            // 
            // ViewRegistersToolStripMenuItem
            // 
            this.ViewRegistersToolStripMenuItem.Name = "ViewRegistersToolStripMenuItem";
            this.ViewRegistersToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.R)));
            this.ViewRegistersToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.ViewRegistersToolStripMenuItem.Text = "Registers";
            this.ViewRegistersToolStripMenuItem.Click += new System.EventHandler(this.ShowFormItem_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(6, 25);
            // 
            // DropDownButtonProgramMemory
            // 
            this.DropDownButtonProgramMemory.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.DropDownButtonProgramMemory.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.flashMemoryToolStripMenuItem,
            this.Execute10StepsButton,
            this.Execute100StepsButton,
            this.Execute1000StepsButton});
            this.DropDownButtonProgramMemory.Image = ((System.Drawing.Image)(resources.GetObject("DropDownButtonProgramMemory.Image")));
            this.DropDownButtonProgramMemory.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.DropDownButtonProgramMemory.Name = "DropDownButtonProgramMemory";
            this.DropDownButtonProgramMemory.Size = new System.Drawing.Size(66, 22);
            this.DropDownButtonProgramMemory.Text = "Program";
            // 
            // flashMemoryToolStripMenuItem
            // 
            this.flashMemoryToolStripMenuItem.Name = "flashMemoryToolStripMenuItem";
            this.flashMemoryToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.flashMemoryToolStripMenuItem.Text = "Flash memory";
            this.flashMemoryToolStripMenuItem.Click += new System.EventHandler(this.ProgramMemoryToolStripMenuItem_Click);
            // 
            // Execute10StepsButton
            // 
            this.Execute10StepsButton.Name = "Execute10StepsButton";
            this.Execute10StepsButton.ShortcutKeys = System.Windows.Forms.Keys.F7;
            this.Execute10StepsButton.Size = new System.Drawing.Size(223, 22);
            this.Execute10StepsButton.Text = "Execute 10 steps";
            this.Execute10StepsButton.Click += new System.EventHandler(this.ExecuteSetOfStepsToolStripMenuItem_Click);
            // 
            // Execute100StepsButton
            // 
            this.Execute100StepsButton.Name = "Execute100StepsButton";
            this.Execute100StepsButton.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F7)));
            this.Execute100StepsButton.Size = new System.Drawing.Size(223, 22);
            this.Execute100StepsButton.Text = "Execute 100 steps";
            this.Execute100StepsButton.Click += new System.EventHandler(this.ExecuteSetOfStepsToolStripMenuItem_Click);
            // 
            // Execute1000StepsButton
            // 
            this.Execute1000StepsButton.Name = "Execute1000StepsButton";
            this.Execute1000StepsButton.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F7)));
            this.Execute1000StepsButton.Size = new System.Drawing.Size(223, 22);
            this.Execute1000StepsButton.Text = "Execute 1000 steps";
            this.Execute1000StepsButton.Click += new System.EventHandler(this.ExecuteSetOfStepsToolStripMenuItem_Click);
            // 
            // SettingsToolStripButton
            // 
            this.SettingsToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.SettingsToolStripButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.useABIRegisterNamesToolStripMenuItem,
            this.OpenSettingsToolStripMenuItem});
            this.SettingsToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("SettingsToolStripButton.Image")));
            this.SettingsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SettingsToolStripButton.Name = "SettingsToolStripButton";
            this.SettingsToolStripButton.Size = new System.Drawing.Size(62, 22);
            this.SettingsToolStripButton.Text = "Settings";
            // 
            // useABIRegisterNamesToolStripMenuItem
            // 
            this.useABIRegisterNamesToolStripMenuItem.CheckOnClick = true;
            this.useABIRegisterNamesToolStripMenuItem.Name = "useABIRegisterNamesToolStripMenuItem";
            this.useABIRegisterNamesToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.useABIRegisterNamesToolStripMenuItem.Text = "Use ABI register names";
            this.useABIRegisterNamesToolStripMenuItem.ToolTipText = "If checked, instructions after Decode stage will be assigned with ABI mnemonic re" +
    "gister names";
            // 
            // OpenSettingsToolStripMenuItem
            // 
            this.OpenSettingsToolStripMenuItem.Name = "OpenSettingsToolStripMenuItem";
            this.OpenSettingsToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.OpenSettingsToolStripMenuItem.Text = "More . . .";
            this.OpenSettingsToolStripMenuItem.Click += new System.EventHandler(this.ShowFormItem_Click);
            // 
            // FormsToolTip
            // 
            this.FormsToolTip.AutoPopDelay = 10000;
            this.FormsToolTip.InitialDelay = 500;
            this.FormsToolTip.ReshowDelay = 100;
            this.FormsToolTip.ShowAlways = true;
            // 
            // toolStrip2
            // 
            this.toolStrip2.BackColor = System.Drawing.SystemColors.Menu;
            this.toolStrip2.CanOverflow = false;
            this.toolStrip2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel5,
            this.toolStripLabel3,
            this.toolStripStatusLabel8,
            this.toolStripSeparator3,
            this.UpdateViewToolStripButton,
            this.toolStripSeparator2,
            this.toolStripStatusLabel7,
            this.CycleHalfToolStripButton,
            this.CycleLatchToolStripButton,
            this.RunSimulationToolStripButton,
            this.toolStripSeparator1,
            this.PauseSimulationToolStripButton,
            this.StopAndResetSimulationToolStripButton,
            this.toolStripSeparator5,
            this.toolStripStatusLabel6,
            this.SimTimeoutNumericUpDown,
            this.toolStripLabel1,
            this.BreakpointAddrNumericUpDown,
            this.toolStripLabel2,
            this.BreakpoinCycleNumericUpDown,
            this.toolStripLabel4});
            this.toolStrip2.Location = new System.Drawing.Point(0, 25);
            this.toolStrip2.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.toolStrip2.Size = new System.Drawing.Size(1556, 28);
            this.toolStrip2.Stretch = true;
            this.toolStrip2.TabIndex = 6;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripStatusLabel5
            // 
            this.toolStripStatusLabel5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStatusLabel5.Enabled = false;
            this.toolStripStatusLabel5.Name = "toolStripStatusLabel5";
            this.toolStripStatusLabel5.Size = new System.Drawing.Size(220, 23);
            this.toolStripStatusLabel5.Text = "                                                     ";
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.BackColor = System.Drawing.SystemColors.ControlLight;
            this.toolStripLabel3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripLabel3.Enabled = false;
            this.toolStripLabel3.Margin = new System.Windows.Forms.Padding(0);
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(28, 28);
            this.toolStripLabel3.Text = "     ";
            // 
            // toolStripStatusLabel8
            // 
            this.toolStripStatusLabel8.BackColor = System.Drawing.SystemColors.ControlLight;
            this.toolStripStatusLabel8.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStatusLabel8.Enabled = false;
            this.toolStripStatusLabel8.Margin = new System.Windows.Forms.Padding(0);
            this.toolStripStatusLabel8.Name = "toolStripStatusLabel8";
            this.toolStripStatusLabel8.Size = new System.Drawing.Size(28, 28);
            this.toolStripStatusLabel8.Text = "     ";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 28);
            // 
            // UpdateViewToolStripButton
            // 
            this.UpdateViewToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.UpdateViewToolStripButton.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.UpdateViewToolStripButton.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.UpdateViewToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("UpdateViewToolStripButton.Image")));
            this.UpdateViewToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.UpdateViewToolStripButton.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.UpdateViewToolStripButton.Name = "UpdateViewToolStripButton";
            this.UpdateViewToolStripButton.Size = new System.Drawing.Size(57, 26);
            this.UpdateViewToolStripButton.Text = "🔻 Pull";
            this.UpdateViewToolStripButton.ToolTipText = "[F5] Pull&Update GUI with simulated CPU data.";
            this.UpdateViewToolStripButton.Click += new System.EventHandler(this.RefreshToolStripStatusLabel_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 28);
            // 
            // toolStripStatusLabel7
            // 
            this.toolStripStatusLabel7.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripStatusLabel7.BackColor = System.Drawing.SystemColors.ControlLight;
            this.toolStripStatusLabel7.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStatusLabel7.Enabled = false;
            this.toolStripStatusLabel7.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.toolStripStatusLabel7.Name = "toolStripStatusLabel7";
            this.toolStripStatusLabel7.Size = new System.Drawing.Size(28, 28);
            this.toolStripStatusLabel7.Text = "     ";
            // 
            // CycleHalfToolStripButton
            // 
            this.CycleHalfToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.CycleHalfToolStripButton.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.CycleHalfToolStripButton.ForeColor = System.Drawing.Color.Green;
            this.CycleHalfToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("CycleHalfToolStripButton.Image")));
            this.CycleHalfToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.CycleHalfToolStripButton.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.CycleHalfToolStripButton.Name = "CycleHalfToolStripButton";
            this.CycleHalfToolStripButton.Size = new System.Drawing.Size(71, 26);
            this.CycleHalfToolStripButton.Text = " ⮥▷ Cycle";
            this.CycleHalfToolStripButton.ToolTipText = "[F1] Activate clock\'s rising or falling edge to cycle/latch pipeline";
            this.CycleHalfToolStripButton.Click += new System.EventHandler(this.CycleToolStrip_Click);
            // 
            // CycleLatchToolStripButton
            // 
            this.CycleLatchToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.CycleLatchToolStripButton.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.CycleLatchToolStripButton.ForeColor = System.Drawing.Color.Green;
            this.CycleLatchToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("CycleLatchToolStripButton.Image")));
            this.CycleLatchToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.CycleLatchToolStripButton.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.CycleLatchToolStripButton.Name = "CycleLatchToolStripButton";
            this.CycleLatchToolStripButton.Size = new System.Drawing.Size(63, 26);
            this.CycleLatchToolStripButton.Text = " I▶ Step";
            this.CycleLatchToolStripButton.ToolTipText = "[F2] Perform single, full clock cycle on pipeline";
            this.CycleLatchToolStripButton.Click += new System.EventHandler(this.CycleToolStrip_Click);
            // 
            // RunSimulationToolStripButton
            // 
            this.RunSimulationToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.RunSimulationToolStripButton.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.RunSimulationToolStripButton.ForeColor = System.Drawing.Color.Green;
            this.RunSimulationToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("RunSimulationToolStripButton.Image")));
            this.RunSimulationToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.RunSimulationToolStripButton.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.RunSimulationToolStripButton.Name = "RunSimulationToolStripButton";
            this.RunSimulationToolStripButton.Size = new System.Drawing.Size(68, 26);
            this.RunSimulationToolStripButton.Text = " ▶▶ Run";
            this.RunSimulationToolStripButton.ToolTipText = "[F8] Run or resume simulator until breakpoint, timeout or EBREAK instruction";
            this.RunSimulationToolStripButton.Click += new System.EventHandler(this.RunSimulationToolStripButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 28);
            // 
            // PauseSimulationToolStripButton
            // 
            this.PauseSimulationToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.PauseSimulationToolStripButton.Enabled = false;
            this.PauseSimulationToolStripButton.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.PauseSimulationToolStripButton.ForeColor = System.Drawing.Color.Firebrick;
            this.PauseSimulationToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("PauseSimulationToolStripButton.Image")));
            this.PauseSimulationToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.PauseSimulationToolStripButton.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.PauseSimulationToolStripButton.Name = "PauseSimulationToolStripButton";
            this.PauseSimulationToolStripButton.Size = new System.Drawing.Size(72, 26);
            this.PauseSimulationToolStripButton.Text = "  II  Pause";
            this.PauseSimulationToolStripButton.ToolTipText = "[F9] Pause running simulation";
            this.PauseSimulationToolStripButton.Click += new System.EventHandler(this.PauseSimulationToolStripButton_Click);
            // 
            // StopAndResetSimulationToolStripButton
            // 
            this.StopAndResetSimulationToolStripButton.AutoToolTip = false;
            this.StopAndResetSimulationToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.StopAndResetSimulationToolStripButton.Enabled = false;
            this.StopAndResetSimulationToolStripButton.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.StopAndResetSimulationToolStripButton.ForeColor = System.Drawing.Color.DarkRed;
            this.StopAndResetSimulationToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("StopAndResetSimulationToolStripButton.Image")));
            this.StopAndResetSimulationToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.StopAndResetSimulationToolStripButton.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.StopAndResetSimulationToolStripButton.Name = "StopAndResetSimulationToolStripButton";
            this.StopAndResetSimulationToolStripButton.Size = new System.Drawing.Size(60, 26);
            this.StopAndResetSimulationToolStripButton.Text = "■ Reset";
            this.StopAndResetSimulationToolStripButton.ToolTipText = "Stops simulation if running and resets simulated CPU core state.";
            this.StopAndResetSimulationToolStripButton.Click += new System.EventHandler(this.StopStripStatusLabel_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 28);
            // 
            // toolStripStatusLabel6
            // 
            this.toolStripStatusLabel6.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripStatusLabel6.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStatusLabel6.Enabled = false;
            this.toolStripStatusLabel6.Margin = new System.Windows.Forms.Padding(0);
            this.toolStripStatusLabel6.Name = "toolStripStatusLabel6";
            this.toolStripStatusLabel6.Size = new System.Drawing.Size(28, 28);
            this.toolStripStatusLabel6.Text = "     ";
            // 
            // SimTimeoutNumericUpDown
            // 
            this.SimTimeoutNumericUpDown.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.SimTimeoutNumericUpDown.AutoSize = false;
            this.SimTimeoutNumericUpDown.BackColor = System.Drawing.SystemColors.MenuBar;
            this.SimTimeoutNumericUpDown.Hexadecimal = false;
            this.SimTimeoutNumericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.SimTimeoutNumericUpDown.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.SimTimeoutNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.SimTimeoutNumericUpDown.Name = "SimTimeoutNumericUpDown";
            this.SimTimeoutNumericUpDown.Size = new System.Drawing.Size(100, 25);
            this.SimTimeoutNumericUpDown.Text = "600";
            this.SimTimeoutNumericUpDown.ToolTipText = "Maximum time in seconds, after which simulation will be automatically stopped.";
            this.SimTimeoutNumericUpDown.Value = new decimal(new int[] {
            600,
            0,
            0,
            0});
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripLabel1.Enabled = false;
            this.toolStripLabel1.Font = new System.Drawing.Font("Arial Unicode MS", 9.75F);
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(116, 25);
            this.toolStripLabel1.Text = "Simulation timeout";
            this.toolStripLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // BreakpointAddrNumericUpDown
            // 
            this.BreakpointAddrNumericUpDown.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.BreakpointAddrNumericUpDown.AutoSize = false;
            this.BreakpointAddrNumericUpDown.BackColor = System.Drawing.SystemColors.MenuBar;
            this.BreakpointAddrNumericUpDown.Hexadecimal = true;
            this.BreakpointAddrNumericUpDown.Increment = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.BreakpointAddrNumericUpDown.Margin = new System.Windows.Forms.Padding(0, 1, 5, 2);
            this.BreakpointAddrNumericUpDown.Maximum = new decimal(new int[] {
            2147483644,
            0,
            0,
            0});
            this.BreakpointAddrNumericUpDown.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.BreakpointAddrNumericUpDown.Name = "BreakpointAddrNumericUpDown";
            this.BreakpointAddrNumericUpDown.Size = new System.Drawing.Size(100, 25);
            this.BreakpointAddrNumericUpDown.Text = "0";
            this.BreakpointAddrNumericUpDown.ToolTipText = "Address of Global Program Counter that will trigger simulation PAUSE event.";
            this.BreakpointAddrNumericUpDown.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripLabel2.Enabled = false;
            this.toolStripLabel2.Font = new System.Drawing.Font("Arial Unicode MS", 9.75F);
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(159, 25);
            this.toolStripLabel2.Text = "Breakpoint address [HEX]";
            this.toolStripLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // BreakpoinCycleNumericUpDown
            // 
            this.BreakpoinCycleNumericUpDown.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.BreakpoinCycleNumericUpDown.AutoSize = false;
            this.BreakpoinCycleNumericUpDown.BackColor = System.Drawing.SystemColors.MenuBar;
            this.BreakpoinCycleNumericUpDown.Hexadecimal = false;
            this.BreakpoinCycleNumericUpDown.Increment = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.BreakpoinCycleNumericUpDown.Margin = new System.Windows.Forms.Padding(0, 1, 5, 2);
            this.BreakpoinCycleNumericUpDown.Maximum = new decimal(new int[] {
            2147483644,
            0,
            0,
            0});
            this.BreakpoinCycleNumericUpDown.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.BreakpoinCycleNumericUpDown.Name = "BreakpoinCycleNumericUpDown";
            this.BreakpoinCycleNumericUpDown.Size = new System.Drawing.Size(100, 25);
            this.BreakpoinCycleNumericUpDown.Text = "0";
            this.BreakpoinCycleNumericUpDown.ToolTipText = "Specific processor cycle that will trigger simulation PAUSE event.";
            this.BreakpoinCycleNumericUpDown.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // toolStripLabel4
            // 
            this.toolStripLabel4.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripLabel4.Enabled = false;
            this.toolStripLabel4.Font = new System.Drawing.Font("Arial Unicode MS", 9.75F);
            this.toolStripLabel4.Name = "toolStripLabel4";
            this.toolStripLabel4.Size = new System.Drawing.Size(105, 25);
            this.toolStripLabel4.Text = "Breakpoint cycle";
            this.toolStripLabel4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tabControlCPUViewType
            // 
            this.tabControlCPUViewType.Controls.Add(this.tabPageStaticCPU);
            this.tabControlCPUViewType.Controls.Add(this.tabPageDynamicCPU);
            this.tabControlCPUViewType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlCPUViewType.Location = new System.Drawing.Point(0, 53);
            this.tabControlCPUViewType.Name = "tabControlCPUViewType";
            this.tabControlCPUViewType.SelectedIndex = 0;
            this.tabControlCPUViewType.Size = new System.Drawing.Size(1556, 744);
            this.tabControlCPUViewType.TabIndex = 9;
            this.tabControlCPUViewType.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl1_Selecting);
            // 
            // tabPageStaticCPU
            // 
            this.tabPageStaticCPU.Controls.Add(this.scalarCoreView);
            this.tabPageStaticCPU.Location = new System.Drawing.Point(4, 22);
            this.tabPageStaticCPU.Name = "tabPageStaticCPU";
            this.tabPageStaticCPU.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageStaticCPU.Size = new System.Drawing.Size(1548, 718);
            this.tabPageStaticCPU.TabIndex = 0;
            this.tabPageStaticCPU.Text = "Static Core";
            this.tabPageStaticCPU.UseVisualStyleBackColor = true;
            // 
            // scalarCoreView
            // 
            this.scalarCoreView.AutoScroll = true;
            this.scalarCoreView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.scalarCoreView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scalarCoreView.Location = new System.Drawing.Point(3, 3);
            this.scalarCoreView.Name = "scalarCoreView";
            this.scalarCoreView.Size = new System.Drawing.Size(1542, 712);
            this.scalarCoreView.TabIndex = 3;
            // 
            // tabPageDynamicCPU
            // 
            this.tabPageDynamicCPU.Controls.Add(this.superscalarCoreView);
            this.tabPageDynamicCPU.Location = new System.Drawing.Point(4, 22);
            this.tabPageDynamicCPU.Name = "tabPageDynamicCPU";
            this.tabPageDynamicCPU.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDynamicCPU.Size = new System.Drawing.Size(1548, 718);
            this.tabPageDynamicCPU.TabIndex = 1;
            this.tabPageDynamicCPU.Text = "Dynamic Core";
            this.tabPageDynamicCPU.UseVisualStyleBackColor = true;
            // 
            // superscalarCoreView
            // 
            this.superscalarCoreView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.superscalarCoreView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.superscalarCoreView.Location = new System.Drawing.Point(3, 3);
            this.superscalarCoreView.MinimumSize = new System.Drawing.Size(1635, 680);
            this.superscalarCoreView.Name = "superscalarCoreView";
            this.superscalarCoreView.Size = new System.Drawing.Size(1635, 712);
            this.superscalarCoreView.TabIndex = 0;
            // 
            // TerminalViewToolStripMenuItem
            // 
            this.TerminalViewToolStripMenuItem.Name = "terminalToolStripMenuItem";
            this.TerminalViewToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.T)));
            this.TerminalViewToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.TerminalViewToolStripMenuItem.Text = "Terminal";
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1556, 821);
            this.Controls.Add(this.tabControlCPUViewType);
            this.Controls.Add(this.toolStrip2);
            this.Controls.Add(this.BottomStatusStrip);
            this.Controls.Add(this.toolStrip1);
            this.Name = "MainForm";
            this.Text = "SsAS";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.BottomStatusStrip.ResumeLayout(false);
            this.BottomStatusStrip.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.tabControlCPUViewType.ResumeLayout(false);
            this.tabPageStaticCPU.ResumeLayout(false);
            this.tabPageDynamicCPU.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip BottomStatusStrip;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton2;
        private System.Windows.Forms.ToolStripMenuItem ViewMemoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ViewRegistersToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripButton1;
        private System.Windows.Forms.ToolStripDropDownButton DropDownButtonProgramMemory;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetCoreToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetMemoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetRegisterFileToolStripMenuItem;
        private System.Windows.Forms.ToolTip FormsToolTip;
        private System.Windows.Forms.ToolStripMenuItem exporToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem simCountersToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel SimTimeToolStripStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel SimTimeoutToolStripStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel SimStatusToolStripLabel;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel5;
        private System.Windows.Forms.ToolStripButton UpdateViewToolStripButton;
        private System.Windows.Forms.ToolStripButton CycleHalfToolStripButton;
        private System.Windows.Forms.ToolStripButton RunSimulationToolStripButton;
        private System.Windows.Forms.ToolStripButton PauseSimulationToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripStatusLabel8;
        private System.Windows.Forms.ToolStripLabel toolStripStatusLabel7;
        private System.Windows.Forms.ToolStripLabel toolStripStatusLabel6;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private UserControls.CustomControls.ToolStripNumberControl BreakpointAddrNumericUpDown;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private UserControls.CustomControls.ToolStripNumberControl SimTimeoutNumericUpDown;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripButton StopAndResetSimulationToolStripButton;
        private System.Windows.Forms.ToolStripStatusLabel SimSpeedStatusStripLabel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton CycleLatchToolStripButton;
        private System.Windows.Forms.ToolStripStatusLabel ToolStripShortStatusMessageLabel;
        private System.Windows.Forms.ToolStripDropDownButton SettingsToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem useABIRegisterNamesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OpenSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem flashMemoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetRAMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetROMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Execute100StepsButton;
        private System.Windows.Forms.ToolStripMenuItem Execute1000StepsButton;
        private System.Windows.Forms.TabControl tabControlCPUViewType;
        private System.Windows.Forms.TabPage tabPageStaticCPU;
        private UserControls.Core.Static.ScalarCoreView scalarCoreView;
        private System.Windows.Forms.TabPage tabPageDynamicCPU;
        private UserControls.Core.Dynamic.SuperscalarCoreView superscalarCoreView;
        private System.Windows.Forms.ToolStripMenuItem Execute10StepsButton;
        private System.Windows.Forms.ToolStripMenuItem memoryDumpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem registersDumpToolStripMenuItem;
        private UserControls.CustomControls.ToolStripNumberControl BreakpoinCycleNumericUpDown;
        private System.Windows.Forms.ToolStripLabel toolStripLabel4;
        private System.Windows.Forms.ToolStripMenuItem TerminalViewToolStripMenuItem;
    }
}

