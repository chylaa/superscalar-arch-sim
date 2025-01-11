
namespace superscalar_arch_sim_gui.UserControls.Core.Dynamic
{
    partial class SuperscalarCoreView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.PanelInstructionFetchReg = new System.Windows.Forms.Panel();
            this.GlobalPCTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dispatchGroupBox = new System.Windows.Forms.GroupBox();
            this.BranchReservationStationsView = new superscalar_arch_sim_gui.UserControls.Core.Dynamic.ReservationStationsView();
            this.MemoryReservationStationsView = new superscalar_arch_sim_gui.UserControls.Core.Dynamic.ReservationStationsView();
            this.IntReservationStationsView = new superscalar_arch_sim_gui.UserControls.Core.Dynamic.ReservationStationsView();
            this.panel3 = new System.Windows.Forms.Panel();
            this.AddressUnitVirtualPanel = new System.Windows.Forms.Panel();
            this.AddressUnit_AddressLabel = new System.Windows.Forms.Label();
            this.AddressUnit_InstructionLabel = new System.Windows.Forms.Label();
            this.AddressUnit_NameLabel = new System.Windows.Forms.Label();
            this.interstageMockPanel1 = new System.Windows.Forms.Panel();
            this.interstageMockPanel2 = new System.Windows.Forms.Panel();
            this.interstageMockPanel3 = new System.Windows.Forms.Panel();
            this.interstageMockPanel4 = new System.Windows.Forms.Panel();
            this.interstageMockPanel5 = new System.Windows.Forms.Panel();
            this.interstageMockPanel6 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.quickMemViewer1 = new superscalar_arch_sim_gui.UserControls.Inspection.QuickMemViewer();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.MemoryExecuteUnitSetView = new superscalar_arch_sim_gui.UserControls.Core.Dynamic.ExecuteUnitSetView();
            this.BranchExecuteUnitSetView = new superscalar_arch_sim_gui.UserControls.Core.Dynamic.ExecuteUnitSetView();
            this.IntegerExecuteUnitSetView = new superscalar_arch_sim_gui.UserControls.Core.Dynamic.ExecuteUnitSetView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.DispatchInstructionQueueView = new superscalar_arch_sim_gui.UserControls.Core.Dynamic.InstructionQueueView();
            this.reorderBufferView1 = new superscalar_arch_sim_gui.UserControls.Core.Dynamic.ReorderBufferView();
            this.RetireStageView = new superscalar_arch_sim_gui.UserControls.Core.Dynamic.MultiIssueStageView();
            this.simCountersView1 = new superscalar_arch_sim_gui.UserControls.Inspection.SimCountersView();
            this.CompleteStageView = new superscalar_arch_sim_gui.UserControls.Core.Dynamic.MultiIssueStageView();
            this.branchPredictorView1 = new superscalar_arch_sim_gui.UserControls.Units.BranchPredictorView();
            this.DecodeStageView = new superscalar_arch_sim_gui.UserControls.Core.Dynamic.MultiIssueStageView();
            this.FetchStageView = new superscalar_arch_sim_gui.UserControls.Core.Dynamic.MultiIssueStageView();
            this.fwDatapathCDB1 = new superscalar_arch_sim_gui.UserControls.Core.Static.FWDatapath();
            this.fwDatapathCDB2 = new superscalar_arch_sim_gui.UserControls.Core.Static.FWDatapath();
            this.GPCDatapath = new superscalar_arch_sim_gui.UserControls.CustomControls.Datapath();
            this.dispatchGroupBox.SuspendLayout();
            this.AddressUnitVirtualPanel.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // PanelInstructionFetchReg
            // 
            this.PanelInstructionFetchReg.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.PanelInstructionFetchReg.Location = new System.Drawing.Point(39, 131);
            this.PanelInstructionFetchReg.Name = "PanelInstructionFetchReg";
            this.PanelInstructionFetchReg.Size = new System.Drawing.Size(10, 363);
            this.PanelInstructionFetchReg.TabIndex = 35;
            // 
            // GlobalPCTextBox
            // 
            this.GlobalPCTextBox.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.GlobalPCTextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.GlobalPCTextBox.Location = new System.Drawing.Point(2, 195);
            this.GlobalPCTextBox.Name = "GlobalPCTextBox";
            this.GlobalPCTextBox.ReadOnly = true;
            this.GlobalPCTextBox.Size = new System.Drawing.Size(146, 20);
            this.GlobalPCTextBox.TabIndex = 36;
            this.GlobalPCTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(49, 179);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 37;
            this.label1.Text = "Global PC";
            // 
            // dispatchGroupBox
            // 
            this.dispatchGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dispatchGroupBox.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.dispatchGroupBox.Controls.Add(this.BranchReservationStationsView);
            this.dispatchGroupBox.Controls.Add(this.MemoryReservationStationsView);
            this.dispatchGroupBox.Controls.Add(this.IntReservationStationsView);
            this.dispatchGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.dispatchGroupBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.dispatchGroupBox.Location = new System.Drawing.Point(438, 46);
            this.dispatchGroupBox.Name = "dispatchGroupBox";
            this.dispatchGroupBox.Size = new System.Drawing.Size(517, 541);
            this.dispatchGroupBox.TabIndex = 39;
            this.dispatchGroupBox.TabStop = false;
            this.dispatchGroupBox.Text = "Dispatch";
            // 
            // BranchReservationStationsView
            // 
            this.BranchReservationStationsView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BranchReservationStationsView.BackColorOnMarkedEmpty = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(174)))), ((int)(((byte)(174)))));
            this.BranchReservationStationsView.BindedCollection = null;
            this.BranchReservationStationsView.LabelNameText = "Branch Reservation Stations";
            this.BranchReservationStationsView.Location = new System.Drawing.Point(111, 14);
            this.BranchReservationStationsView.Margin = new System.Windows.Forms.Padding(4);
            this.BranchReservationStationsView.Name = "BranchReservationStationsView";
            this.BranchReservationStationsView.Size = new System.Drawing.Size(394, 126);
            this.BranchReservationStationsView.SpecialColorRowIndex = -1;
            this.BranchReservationStationsView.SpecialRowColor = System.Drawing.SystemColors.Window;
            this.BranchReservationStationsView.TabIndex = 0;
            // 
            // MemoryReservationStationsView
            // 
            this.MemoryReservationStationsView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MemoryReservationStationsView.BackColorOnMarkedEmpty = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(174)))), ((int)(((byte)(174)))));
            this.MemoryReservationStationsView.BindedCollection = null;
            this.MemoryReservationStationsView.LabelNameText = "Memory Buffer";
            this.MemoryReservationStationsView.Location = new System.Drawing.Point(111, 144);
            this.MemoryReservationStationsView.Margin = new System.Windows.Forms.Padding(9, 6, 9, 6);
            this.MemoryReservationStationsView.Name = "MemoryReservationStationsView";
            this.MemoryReservationStationsView.Size = new System.Drawing.Size(394, 210);
            this.MemoryReservationStationsView.SpecialColorRowIndex = -1;
            this.MemoryReservationStationsView.SpecialRowColor = System.Drawing.SystemColors.Window;
            this.MemoryReservationStationsView.TabIndex = 2;
            // 
            // IntReservationStationsView
            // 
            this.IntReservationStationsView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.IntReservationStationsView.BackColorOnMarkedEmpty = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(174)))), ((int)(((byte)(174)))));
            this.IntReservationStationsView.BindedCollection = null;
            this.IntReservationStationsView.LabelNameText = "Integer Reservation Stations";
            this.IntReservationStationsView.Location = new System.Drawing.Point(111, 359);
            this.IntReservationStationsView.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.IntReservationStationsView.Name = "IntReservationStationsView";
            this.IntReservationStationsView.Size = new System.Drawing.Size(398, 171);
            this.IntReservationStationsView.SpecialColorRowIndex = -1;
            this.IntReservationStationsView.SpecialRowColor = System.Drawing.SystemColors.Window;
            this.IntReservationStationsView.TabIndex = 1;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Location = new System.Drawing.Point(40, 218);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(10, 18);
            this.panel3.TabIndex = 38;
            // 
            // AddressUnitVirtualPanel
            // 
            this.AddressUnitVirtualPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.AddressUnitVirtualPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.AddressUnitVirtualPanel.Controls.Add(this.AddressUnit_AddressLabel);
            this.AddressUnitVirtualPanel.Controls.Add(this.AddressUnit_InstructionLabel);
            this.AddressUnitVirtualPanel.Controls.Add(this.AddressUnit_NameLabel);
            this.AddressUnitVirtualPanel.Location = new System.Drawing.Point(15, 163);
            this.AddressUnitVirtualPanel.Name = "AddressUnitVirtualPanel";
            this.AddressUnitVirtualPanel.Size = new System.Drawing.Size(175, 65);
            this.AddressUnitVirtualPanel.TabIndex = 39;
            // 
            // AddressUnit_AddressLabel
            // 
            this.AddressUnit_AddressLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.AddressUnit_AddressLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AddressUnit_AddressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.AddressUnit_AddressLabel.Location = new System.Drawing.Point(0, 20);
            this.AddressUnit_AddressLabel.Name = "AddressUnit_AddressLabel";
            this.AddressUnit_AddressLabel.Size = new System.Drawing.Size(171, 21);
            this.AddressUnit_AddressLabel.TabIndex = 2;
            this.AddressUnit_AddressLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AddressUnit_InstructionLabel
            // 
            this.AddressUnit_InstructionLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.AddressUnit_InstructionLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.AddressUnit_InstructionLabel.Enabled = false;
            this.AddressUnit_InstructionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.AddressUnit_InstructionLabel.Location = new System.Drawing.Point(0, 41);
            this.AddressUnit_InstructionLabel.Name = "AddressUnit_InstructionLabel";
            this.AddressUnit_InstructionLabel.Size = new System.Drawing.Size(171, 20);
            this.AddressUnit_InstructionLabel.TabIndex = 1;
            this.AddressUnit_InstructionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AddressUnit_NameLabel
            // 
            this.AddressUnit_NameLabel.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.AddressUnit_NameLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.AddressUnit_NameLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.AddressUnit_NameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.AddressUnit_NameLabel.Location = new System.Drawing.Point(0, 0);
            this.AddressUnit_NameLabel.Name = "AddressUnit_NameLabel";
            this.AddressUnit_NameLabel.Size = new System.Drawing.Size(171, 20);
            this.AddressUnit_NameLabel.TabIndex = 0;
            this.AddressUnit_NameLabel.Text = "Address Unit";
            this.AddressUnit_NameLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // interstageMockPanel1
            // 
            this.interstageMockPanel1.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.interstageMockPanel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.interstageMockPanel1.Location = new System.Drawing.Point(186, 305);
            this.interstageMockPanel1.Name = "interstageMockPanel1";
            this.interstageMockPanel1.Size = new System.Drawing.Size(258, 10);
            this.interstageMockPanel1.TabIndex = 36;
            // 
            // interstageMockPanel2
            // 
            this.interstageMockPanel2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.interstageMockPanel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.interstageMockPanel2.Location = new System.Drawing.Point(186, 321);
            this.interstageMockPanel2.Name = "interstageMockPanel2";
            this.interstageMockPanel2.Size = new System.Drawing.Size(258, 10);
            this.interstageMockPanel2.TabIndex = 37;
            // 
            // interstageMockPanel3
            // 
            this.interstageMockPanel3.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.interstageMockPanel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.interstageMockPanel3.Location = new System.Drawing.Point(186, 337);
            this.interstageMockPanel3.Name = "interstageMockPanel3";
            this.interstageMockPanel3.Size = new System.Drawing.Size(258, 10);
            this.interstageMockPanel3.TabIndex = 37;
            // 
            // interstageMockPanel4
            // 
            this.interstageMockPanel4.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.interstageMockPanel4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.interstageMockPanel4.Location = new System.Drawing.Point(186, 353);
            this.interstageMockPanel4.Name = "interstageMockPanel4";
            this.interstageMockPanel4.Size = new System.Drawing.Size(258, 10);
            this.interstageMockPanel4.TabIndex = 38;
            // 
            // interstageMockPanel5
            // 
            this.interstageMockPanel5.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.interstageMockPanel5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.interstageMockPanel5.Location = new System.Drawing.Point(186, 369);
            this.interstageMockPanel5.Name = "interstageMockPanel5";
            this.interstageMockPanel5.Size = new System.Drawing.Size(258, 10);
            this.interstageMockPanel5.TabIndex = 39;
            // 
            // interstageMockPanel6
            // 
            this.interstageMockPanel6.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.interstageMockPanel6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.interstageMockPanel6.Location = new System.Drawing.Point(186, 385);
            this.interstageMockPanel6.Name = "interstageMockPanel6";
            this.interstageMockPanel6.Size = new System.Drawing.Size(258, 10);
            this.interstageMockPanel6.TabIndex = 40;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.quickMemViewer1);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox1.Location = new System.Drawing.Point(3, 590);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(513, 74);
            this.groupBox1.TabIndex = 44;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Quick Memory preview";
            // 
            // quickMemViewer1
            // 
            this.quickMemViewer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.quickMemViewer1.Location = new System.Drawing.Point(6, 19);
            this.quickMemViewer1.MaximumSize = new System.Drawing.Size(505, 50);
            this.quickMemViewer1.MemValueStyle = superscalar_arch_sim_gui.Utilis.StrConverter.StringStyle.Hex;
            this.quickMemViewer1.MinimumSize = new System.Drawing.Size(505, 50);
            this.quickMemViewer1.Name = "quickMemViewer1";
            this.quickMemViewer1.ObservedMemory = null;
            this.quickMemViewer1.Size = new System.Drawing.Size(505, 50);
            this.quickMemViewer1.TabIndex = 7;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.groupBox2.Controls.Add(this.MemoryExecuteUnitSetView);
            this.groupBox2.Controls.Add(this.AddressUnitVirtualPanel);
            this.groupBox2.Controls.Add(this.BranchExecuteUnitSetView);
            this.groupBox2.Controls.Add(this.IntegerExecuteUnitSetView);
            this.groupBox2.Controls.Add(this.panel3);
            this.groupBox2.Controls.Add(this.panel2);
            this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.groupBox2.Location = new System.Drawing.Point(965, 46);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(206, 541);
            this.groupBox2.TabIndex = 40;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Execute";
            // 
            // MemoryExecuteUnitSetView
            // 
            this.MemoryExecuteUnitSetView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MemoryExecuteUnitSetView.BackColor = System.Drawing.SystemColors.ControlLight;
            this.MemoryExecuteUnitSetView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MemoryExecuteUnitSetView.Location = new System.Drawing.Point(8, 233);
            this.MemoryExecuteUnitSetView.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.MemoryExecuteUnitSetView.Name = "MemoryExecuteUnitSetView";
            this.MemoryExecuteUnitSetView.Size = new System.Drawing.Size(188, 116);
            this.MemoryExecuteUnitSetView.SubLabelsFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.MemoryExecuteUnitSetView.TabIndex = 40;
            this.MemoryExecuteUnitSetView.UnitName = "Memory Unit";
            // 
            // BranchExecuteUnitSetView
            // 
            this.BranchExecuteUnitSetView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BranchExecuteUnitSetView.BackColor = System.Drawing.SystemColors.ControlLight;
            this.BranchExecuteUnitSetView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.BranchExecuteUnitSetView.Location = new System.Drawing.Point(9, 37);
            this.BranchExecuteUnitSetView.Margin = new System.Windows.Forms.Padding(4);
            this.BranchExecuteUnitSetView.Name = "BranchExecuteUnitSetView";
            this.BranchExecuteUnitSetView.Size = new System.Drawing.Size(187, 109);
            this.BranchExecuteUnitSetView.SubLabelsFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.BranchExecuteUnitSetView.TabIndex = 39;
            this.BranchExecuteUnitSetView.UnitName = "Branch Unit";
            // 
            // IntegerExecuteUnitSetView
            // 
            this.IntegerExecuteUnitSetView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.IntegerExecuteUnitSetView.BackColor = System.Drawing.SystemColors.ControlLight;
            this.IntegerExecuteUnitSetView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.IntegerExecuteUnitSetView.Location = new System.Drawing.Point(8, 362);
            this.IntegerExecuteUnitSetView.Margin = new System.Windows.Forms.Padding(14, 7, 14, 7);
            this.IntegerExecuteUnitSetView.Name = "IntegerExecuteUnitSetView";
            this.IntegerExecuteUnitSetView.Size = new System.Drawing.Size(188, 172);
            this.IntegerExecuteUnitSetView.SubLabelsFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.IntegerExecuteUnitSetView.TabIndex = 42;
            this.IntegerExecuteUnitSetView.UnitName = "Integer Unit";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Location = new System.Drawing.Point(151, 217);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(10, 27);
            this.panel2.TabIndex = 39;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.panel1.Location = new System.Drawing.Point(1615, 160);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(10, 363);
            this.panel1.TabIndex = 36;
            // 
            // DispatchInstructionQueueView
            // 
            this.DispatchInstructionQueueView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.DispatchInstructionQueueView.IRDataQueue = null;
            this.DispatchInstructionQueueView.Location = new System.Drawing.Point(386, 241);
            this.DispatchInstructionQueueView.Margin = new System.Windows.Forms.Padding(4);
            this.DispatchInstructionQueueView.Name = "DispatchInstructionQueueView";
            this.DispatchInstructionQueueView.Size = new System.Drawing.Size(156, 159);
            this.DispatchInstructionQueueView.TabIndex = 38;
            // 
            // reorderBufferView1
            // 
            this.reorderBufferView1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.reorderBufferView1.BackColorOnMarkedEmpty = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(174)))), ((int)(((byte)(174)))));
            this.reorderBufferView1.BindedCollection = null;
            this.reorderBufferView1.HeadEntryBackcolor = System.Drawing.Color.LightBlue;
            this.reorderBufferView1.LabelNameText = "Reorder Buffer";
            this.reorderBufferView1.Location = new System.Drawing.Point(1198, 46);
            this.reorderBufferView1.Name = "reorderBufferView1";
            this.reorderBufferView1.Size = new System.Drawing.Size(406, 198);
            this.reorderBufferView1.SpecialColorRowIndex = -1;
            this.reorderBufferView1.SpecialRowColor = System.Drawing.Color.LightBlue;
            this.reorderBufferView1.TabIndex = 50;
            // 
            // RetireStageView
            // 
            this.RetireStageView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RetireStageView.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.RetireStageView.Location = new System.Drawing.Point(1415, 250);
            this.RetireStageView.Name = "RetireStageView";
            this.RetireStageView.Size = new System.Drawing.Size(196, 147);
            this.RetireStageView.StageName = "Retire";
            this.RetireStageView.TabIndex = 47;
            this.RetireStageView.VisibleColumns = new string[] {
        "IR32",
        "LocalPC"};
            // 
            // simCountersView1
            // 
            this.simCountersView1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.simCountersView1.Location = new System.Drawing.Point(0, 690);
            this.simCountersView1.Name = "simCountersView1";
            this.simCountersView1.Size = new System.Drawing.Size(1635, 80);
            this.simCountersView1.TabIndex = 46;
            // 
            // CompleteStageView
            // 
            this.CompleteStageView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CompleteStageView.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.CompleteStageView.Location = new System.Drawing.Point(1181, 250);
            this.CompleteStageView.Name = "CompleteStageView";
            this.CompleteStageView.Size = new System.Drawing.Size(228, 147);
            this.CompleteStageView.StageName = "Complete";
            this.CompleteStageView.TabIndex = 45;
            this.CompleteStageView.VisibleColumns = new string[] {
        "IR32",
        "LocalPC",
        "ReservationStationSourceTag"};
            // 
            // branchPredictorView1
            // 
            this.branchPredictorView1.BranchPredictorDetailsForm = null;
            this.branchPredictorView1.Location = new System.Drawing.Point(16, 19);
            this.branchPredictorView1.Name = "branchPredictorView1";
            this.branchPredictorView1.Size = new System.Drawing.Size(187, 143);
            this.branchPredictorView1.TabIndex = 2;
            // 
            // DecodeStageView
            // 
            this.DecodeStageView.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.DecodeStageView.Location = new System.Drawing.Point(218, 250);
            this.DecodeStageView.Name = "DecodeStageView";
            this.DecodeStageView.Size = new System.Drawing.Size(150, 147);
            this.DecodeStageView.StageName = "Decode";
            this.DecodeStageView.TabIndex = 1;
            this.DecodeStageView.VisibleColumns = new string[] {
        "IR32",
        "LocalPC",
        "NextPC"};
            // 
            // FetchStageView
            // 
            this.FetchStageView.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.FetchStageView.Location = new System.Drawing.Point(53, 250);
            this.FetchStageView.Name = "FetchStageView";
            this.FetchStageView.Size = new System.Drawing.Size(150, 147);
            this.FetchStageView.StageName = "Fetch";
            this.FetchStageView.TabIndex = 0;
            this.FetchStageView.VisibleColumns = new string[] {
        "IR32",
        "LocalPC"};
            // 
            // fwDatapathCDB1
            // 
            this.fwDatapathCDB1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fwDatapathCDB1.Appearance = superscalar_arch_sim_gui.UserControls.CustomControls.Datapath.LineApperance.Double;
            this.fwDatapathCDB1.ColorActive = System.Drawing.Color.Red;
            this.fwDatapathCDB1.DataFormat = "X8";
            this.fwDatapathCDB1.DataValue = null;
            this.fwDatapathCDB1.DoubleActiveOutlineColor = System.Drawing.Color.Black;
            this.fwDatapathCDB1.DoubleInactiveInnerColor = System.Drawing.SystemColors.Control;
            this.fwDatapathCDB1.EndCapSize = 2F;
            this.fwDatapathCDB1.EndPoint = new System.Drawing.Point(1, -1);
            this.fwDatapathCDB1.Font = new System.Drawing.Font("Microsoft YaHei UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.fwDatapathCDB1.Location = new System.Drawing.Point(926, 19);
            this.fwDatapathCDB1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.fwDatapathCDB1.MiddlePoint_1 = new System.Drawing.Point(-1, 1);
            this.fwDatapathCDB1.MiddlePoint_2 = new System.Drawing.Point(1, 1);
            this.fwDatapathCDB1.Name = "fwDatapathCDB1";
            this.fwDatapathCDB1.Padding = new System.Windows.Forms.Padding(4);
            this.fwDatapathCDB1.Rotation = 0;
            this.fwDatapathCDB1.Size = new System.Drawing.Size(265, 239);
            this.fwDatapathCDB1.StartPoint = new System.Drawing.Point(-1, 0);
            this.fwDatapathCDB1.TabIndex = 48;
            this.fwDatapathCDB1.TextCPoint = new System.Drawing.Point(87, 92);
            this.fwDatapathCDB1.WidthOfLine = 4;
            // 
            // fwDatapathCDB2
            // 
            this.fwDatapathCDB2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fwDatapathCDB2.Appearance = superscalar_arch_sim_gui.UserControls.CustomControls.Datapath.LineApperance.Double;
            this.fwDatapathCDB2.ColorActive = System.Drawing.Color.Red;
            this.fwDatapathCDB2.DataFormat = "X8";
            this.fwDatapathCDB2.DataValue = null;
            this.fwDatapathCDB2.DoubleActiveOutlineColor = System.Drawing.Color.Black;
            this.fwDatapathCDB2.DoubleInactiveInnerColor = System.Drawing.SystemColors.Control;
            this.fwDatapathCDB2.EndCapSize = 2F;
            this.fwDatapathCDB2.EndPoint = new System.Drawing.Point(1, 1);
            this.fwDatapathCDB2.Font = new System.Drawing.Font("Microsoft YaHei UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.fwDatapathCDB2.Location = new System.Drawing.Point(926, 356);
            this.fwDatapathCDB2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.fwDatapathCDB2.MiddlePoint_1 = new System.Drawing.Point(-1, -1);
            this.fwDatapathCDB2.MiddlePoint_2 = new System.Drawing.Point(1, -1);
            this.fwDatapathCDB2.Name = "fwDatapathCDB2";
            this.fwDatapathCDB2.Padding = new System.Windows.Forms.Padding(4);
            this.fwDatapathCDB2.Rotation = 0;
            this.fwDatapathCDB2.Size = new System.Drawing.Size(265, 255);
            this.fwDatapathCDB2.StartPoint = new System.Drawing.Point(-1, 0);
            this.fwDatapathCDB2.TabIndex = 49;
            this.fwDatapathCDB2.TextCPoint = new System.Drawing.Point(87, 92);
            this.fwDatapathCDB2.WidthOfLine = 4;
            // 
            // GPCDatapath
            // 
            this.GPCDatapath.Appearance = superscalar_arch_sim_gui.UserControls.CustomControls.Datapath.LineApperance.Double;
            this.GPCDatapath.ColorActive = System.Drawing.Color.Red;
            this.GPCDatapath.DataFormat = "X8";
            this.GPCDatapath.DataValue = null;
            this.GPCDatapath.DoubleActiveOutlineColor = System.Drawing.Color.Black;
            this.GPCDatapath.DoubleInactiveInnerColor = System.Drawing.SystemColors.Control;
            this.GPCDatapath.EndCapSize = 2F;
            this.GPCDatapath.EndPoint = new System.Drawing.Point(-1, 1);
            this.GPCDatapath.Font = new System.Drawing.Font("Microsoft YaHei UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.GPCDatapath.Location = new System.Drawing.Point(136, 197);
            this.GPCDatapath.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.GPCDatapath.MiddlePoint_1 = new System.Drawing.Point(1, 1);
            this.GPCDatapath.MiddlePoint_2 = null;
            this.GPCDatapath.Name = "GPCDatapath";
            this.GPCDatapath.Padding = new System.Windows.Forms.Padding(4);
            this.GPCDatapath.Rotation = 0;
            this.GPCDatapath.Size = new System.Drawing.Size(247, 117);
            this.GPCDatapath.StartPoint = new System.Drawing.Point(1, -1);
            this.GPCDatapath.TabIndex = 51;
            this.GPCDatapath.TextCPoint = new System.Drawing.Point(132, 16);
            this.GPCDatapath.WidthOfLine = 4;
            // 
            // SuperscalarCoreView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.DispatchInstructionQueueView);
            this.Controls.Add(this.reorderBufferView1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.RetireStageView);
            this.Controls.Add(this.simCountersView1);
            this.Controls.Add(this.CompleteStageView);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.GlobalPCTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.branchPredictorView1);
            this.Controls.Add(this.PanelInstructionFetchReg);
            this.Controls.Add(this.DecodeStageView);
            this.Controls.Add(this.FetchStageView);
            this.Controls.Add(this.dispatchGroupBox);
            this.Controls.Add(this.interstageMockPanel1);
            this.Controls.Add(this.interstageMockPanel3);
            this.Controls.Add(this.interstageMockPanel2);
            this.Controls.Add(this.interstageMockPanel6);
            this.Controls.Add(this.interstageMockPanel5);
            this.Controls.Add(this.interstageMockPanel4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.fwDatapathCDB1);
            this.Controls.Add(this.fwDatapathCDB2);
            this.Controls.Add(this.GPCDatapath);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(1635, 680);
            this.Name = "SuperscalarCoreView";
            this.Size = new System.Drawing.Size(1635, 770);
            this.Load += new System.EventHandler(this.SuperscalarCoreView_Load);
            this.dispatchGroupBox.ResumeLayout(false);
            this.AddressUnitVirtualPanel.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MultiIssueStageView FetchStageView;
        private MultiIssueStageView DecodeStageView;
        private Units.BranchPredictorView branchPredictorView1;
        private System.Windows.Forms.Panel PanelInstructionFetchReg;
        private System.Windows.Forms.TextBox GlobalPCTextBox;
        private System.Windows.Forms.Label label1;
        private InstructionQueueView DispatchInstructionQueueView;
        private System.Windows.Forms.GroupBox dispatchGroupBox;
        private ReservationStationsView BranchReservationStationsView;
        private ReservationStationsView MemoryReservationStationsView;
        private ReservationStationsView IntReservationStationsView;
        private System.Windows.Forms.Panel interstageMockPanel1;
        private System.Windows.Forms.Panel interstageMockPanel2;
        private System.Windows.Forms.Panel interstageMockPanel3;
        private System.Windows.Forms.Panel interstageMockPanel4;
        private System.Windows.Forms.Panel interstageMockPanel5;
        private System.Windows.Forms.Panel interstageMockPanel6;
        private System.Windows.Forms.GroupBox groupBox1;
        private Inspection.QuickMemViewer quickMemViewer1;
        private ExecuteUnitSetView BranchExecuteUnitSetView;
        private ExecuteUnitSetView IntegerExecuteUnitSetView;
        private ExecuteUnitSetView MemoryExecuteUnitSetView;
        private System.Windows.Forms.GroupBox groupBox2;
        private MultiIssueStageView CompleteStageView;
        private Inspection.SimCountersView simCountersView1;
        private MultiIssueStageView RetireStageView;
        private System.Windows.Forms.Panel panel1;
        private Static.FWDatapath fwDatapathCDB1;
        private Static.FWDatapath fwDatapathCDB2;
        private System.Windows.Forms.Panel AddressUnitVirtualPanel;
        private System.Windows.Forms.Label AddressUnit_NameLabel;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label AddressUnit_AddressLabel;
        private System.Windows.Forms.Label AddressUnit_InstructionLabel;
        private ReorderBufferView reorderBufferView1;
        private System.Windows.Forms.Panel panel2;
        private CustomControls.Datapath GPCDatapath;
    }
}
