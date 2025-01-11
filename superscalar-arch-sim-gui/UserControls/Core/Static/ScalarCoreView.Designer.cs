namespace superscalar_arch_sim_gui.UserControls.Core.Static
{
    partial class ScalarCoreView
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
            this.label1 = new System.Windows.Forms.Label();
            this.GlobalPCTextBox = new System.Windows.Forms.TextBox();
            this.PanelInstructionFetchReg = new System.Windows.Forms.Panel();
            this.PanelTerminationBuffer = new System.Windows.Forms.Panel();
            this.PanelFeedbackFWdatapath = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.quickMemViewer1 = new superscalar_arch_sim_gui.UserControls.Inspection.QuickMemViewer();
            this.branchPredictorView1 = new superscalar_arch_sim_gui.UserControls.Units.BranchPredictorView();
            this.EXMEMbufferView = new superscalar_arch_sim_gui.UserControls.Core.Static.BufferView();
            this.MEMWBbufferView = new superscalar_arch_sim_gui.UserControls.Core.Static.BufferView();
            this.IDEXbufferView = new superscalar_arch_sim_gui.UserControls.Core.Static.BufferView();
            this.IFIDbufferView = new superscalar_arch_sim_gui.UserControls.Core.Static.BufferView();
            this.stageViewWB = new superscalar_arch_sim_gui.UserControls.Core.Static.StageView();
            this.stageViewMEM = new superscalar_arch_sim_gui.UserControls.Core.Static.StageView();
            this.stageViewEX = new superscalar_arch_sim_gui.UserControls.Core.Static.StageView();
            this.stageViewID = new superscalar_arch_sim_gui.UserControls.Core.Static.StageView();
            this.stageViewIF = new superscalar_arch_sim_gui.UserControls.Core.Static.StageView();
            this.fwDatapathMEMtoEX = new superscalar_arch_sim_gui.UserControls.Core.Static.FWDatapath();
            this.PCNewDatapath = new superscalar_arch_sim_gui.UserControls.Core.Static.FWDatapath();
            this.fwDatapathWBtoMEM = new superscalar_arch_sim_gui.UserControls.Core.Static.FWDatapath();
            this.fwDatapathWBtoEX = new superscalar_arch_sim_gui.UserControls.Core.Static.FWDatapath();
            this.simCountersView1 = new superscalar_arch_sim_gui.UserControls.Inspection.SimCountersView();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(107, 187);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 22;
            this.label1.Text = "Global PC";
            // 
            // GlobalPCTextBox
            // 
            this.GlobalPCTextBox.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.GlobalPCTextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.GlobalPCTextBox.Location = new System.Drawing.Point(60, 203);
            this.GlobalPCTextBox.Name = "GlobalPCTextBox";
            this.GlobalPCTextBox.ReadOnly = true;
            this.GlobalPCTextBox.Size = new System.Drawing.Size(146, 20);
            this.GlobalPCTextBox.TabIndex = 21;
            this.GlobalPCTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // PanelInstructionFetchReg
            // 
            this.PanelInstructionFetchReg.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.PanelInstructionFetchReg.Location = new System.Drawing.Point(92, 159);
            this.PanelInstructionFetchReg.Name = "PanelInstructionFetchReg";
            this.PanelInstructionFetchReg.Size = new System.Drawing.Size(10, 386);
            this.PanelInstructionFetchReg.TabIndex = 34;
            // 
            // PanelTerminationBuffer
            // 
            this.PanelTerminationBuffer.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.PanelTerminationBuffer.Location = new System.Drawing.Point(1135, 148);
            this.PanelTerminationBuffer.Name = "PanelTerminationBuffer";
            this.PanelTerminationBuffer.Size = new System.Drawing.Size(10, 397);
            this.PanelTerminationBuffer.TabIndex = 35;
            // 
            // PanelFeedbackFWdatapath
            // 
            this.PanelFeedbackFWdatapath.Location = new System.Drawing.Point(483, 42);
            this.PanelFeedbackFWdatapath.Name = "PanelFeedbackFWdatapath";
            this.PanelFeedbackFWdatapath.Size = new System.Drawing.Size(500, 46);
            this.PanelFeedbackFWdatapath.TabIndex = 39;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.quickMemViewer1);
            this.groupBox1.Location = new System.Drawing.Point(1013, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(513, 80);
            this.groupBox1.TabIndex = 43;
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
            // branchPredictorView1
            // 
            this.branchPredictorView1.BranchPredictorDetailsForm = null;
            this.branchPredictorView1.Location = new System.Drawing.Point(25, 10);
            this.branchPredictorView1.Name = "branchPredictorView1";
            this.branchPredictorView1.Size = new System.Drawing.Size(170, 145);
            this.branchPredictorView1.TabIndex = 42;
            // 
            // EXMEMbufferView
            // 
            this.EXMEMbufferView.BackColor = System.Drawing.SystemColors.ControlLight;
            this.EXMEMbufferView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.EXMEMbufferView.Location = new System.Drawing.Point(679, 72);
            this.EXMEMbufferView.Name = "EXMEMbufferView";
            this.EXMEMbufferView.Size = new System.Drawing.Size(100, 473);
            this.EXMEMbufferView.TabIndex = 32;
            this.EXMEMbufferView.Virtual = false;
            // 
            // MEMWBbufferView
            // 
            this.MEMWBbufferView.BackColor = System.Drawing.SystemColors.ControlLight;
            this.MEMWBbufferView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MEMWBbufferView.Location = new System.Drawing.Point(907, 72);
            this.MEMWBbufferView.Name = "MEMWBbufferView";
            this.MEMWBbufferView.Size = new System.Drawing.Size(100, 473);
            this.MEMWBbufferView.TabIndex = 33;
            this.MEMWBbufferView.Virtual = false;
            // 
            // IDEXbufferView
            // 
            this.IDEXbufferView.BackColor = System.Drawing.SystemColors.ControlLight;
            this.IDEXbufferView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.IDEXbufferView.Location = new System.Drawing.Point(452, 72);
            this.IDEXbufferView.Name = "IDEXbufferView";
            this.IDEXbufferView.Size = new System.Drawing.Size(100, 473);
            this.IDEXbufferView.TabIndex = 31;
            this.IDEXbufferView.Virtual = false;
            // 
            // IFIDbufferView
            // 
            this.IFIDbufferView.BackColor = System.Drawing.SystemColors.ControlLight;
            this.IFIDbufferView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.IFIDbufferView.Location = new System.Drawing.Point(225, 72);
            this.IFIDbufferView.Name = "IFIDbufferView";
            this.IFIDbufferView.Size = new System.Drawing.Size(100, 473);
            this.IFIDbufferView.TabIndex = 30;
            this.IFIDbufferView.Virtual = false;
            // 
            // stageViewWB
            // 
            this.stageViewWB.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.stageViewWB.Location = new System.Drawing.Point(1014, 229);
            this.stageViewWB.Name = "stageViewWB";
            this.stageViewWB.Size = new System.Drawing.Size(115, 115);
            this.stageViewWB.TabIndex = 20;
            this.stageViewWB.Virtual = false;
            // 
            // stageViewMEM
            // 
            this.stageViewMEM.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.stageViewMEM.Location = new System.Drawing.Point(785, 229);
            this.stageViewMEM.Name = "stageViewMEM";
            this.stageViewMEM.Size = new System.Drawing.Size(115, 115);
            this.stageViewMEM.TabIndex = 19;
            this.stageViewMEM.Virtual = false;
            // 
            // stageViewEX
            // 
            this.stageViewEX.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.stageViewEX.Location = new System.Drawing.Point(558, 229);
            this.stageViewEX.Name = "stageViewEX";
            this.stageViewEX.Size = new System.Drawing.Size(115, 115);
            this.stageViewEX.TabIndex = 18;
            this.stageViewEX.Virtual = false;
            // 
            // stageViewID
            // 
            this.stageViewID.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.stageViewID.Location = new System.Drawing.Point(331, 229);
            this.stageViewID.Name = "stageViewID";
            this.stageViewID.Size = new System.Drawing.Size(115, 115);
            this.stageViewID.TabIndex = 17;
            this.stageViewID.Virtual = false;
            // 
            // stageViewIF
            // 
            this.stageViewIF.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.stageViewIF.Location = new System.Drawing.Point(104, 229);
            this.stageViewIF.Name = "stageViewIF";
            this.stageViewIF.Size = new System.Drawing.Size(115, 115);
            this.stageViewIF.TabIndex = 16;
            this.stageViewIF.Virtual = false;
            // 
            // fwDatapathMEMtoEX
            // 
            this.fwDatapathMEMtoEX.Appearance = superscalar_arch_sim_gui.UserControls.CustomControls.Datapath.LineApperance.Double;
            this.fwDatapathMEMtoEX.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.fwDatapathMEMtoEX.ColorActive = System.Drawing.Color.Red;
            this.fwDatapathMEMtoEX.DataFormat = "X8";
            this.fwDatapathMEMtoEX.DataValue = null;
            this.fwDatapathMEMtoEX.DoubleActiveOutlineColor = System.Drawing.Color.Black;
            this.fwDatapathMEMtoEX.DoubleInactiveInnerColor = System.Drawing.SystemColors.Control;
            this.fwDatapathMEMtoEX.EndCapSize = 2F;
            this.fwDatapathMEMtoEX.EndPoint = new System.Drawing.Point(-1, 1);
            this.fwDatapathMEMtoEX.Font = new System.Drawing.Font("Microsoft YaHei UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.fwDatapathMEMtoEX.Location = new System.Drawing.Point(486, 52);
            this.fwDatapathMEMtoEX.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.fwDatapathMEMtoEX.MiddlePoint_1 = new System.Drawing.Point(1, -1);
            this.fwDatapathMEMtoEX.MiddlePoint_2 = new System.Drawing.Point(1, 1);
            this.fwDatapathMEMtoEX.Name = "fwDatapathMEMtoEX";
            this.fwDatapathMEMtoEX.Padding = new System.Windows.Forms.Padding(2);
            this.fwDatapathMEMtoEX.Rotation = 90;
            this.fwDatapathMEMtoEX.Size = new System.Drawing.Size(225, 25);
            this.fwDatapathMEMtoEX.StartPoint = new System.Drawing.Point(-1, -1);
            this.fwDatapathMEMtoEX.TabIndex = 37;
            this.fwDatapathMEMtoEX.TextCPoint = new System.Drawing.Point(-1, 1);
            this.fwDatapathMEMtoEX.WidthOfLine = 4;
            // 
            // PCNewDatapath
            // 
            this.PCNewDatapath.Appearance = superscalar_arch_sim_gui.UserControls.CustomControls.Datapath.LineApperance.Double;
            this.PCNewDatapath.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.PCNewDatapath.ColorActive = System.Drawing.Color.Red;
            this.PCNewDatapath.DataFormat = "X8";
            this.PCNewDatapath.DataValue = null;
            this.PCNewDatapath.DoubleActiveOutlineColor = System.Drawing.Color.Black;
            this.PCNewDatapath.DoubleInactiveInnerColor = System.Drawing.SystemColors.Control;
            this.PCNewDatapath.EndCapSize = 2F;
            this.PCNewDatapath.EndPoint = new System.Drawing.Point(1, 1);
            this.PCNewDatapath.Font = new System.Drawing.Font("Microsoft YaHei UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.PCNewDatapath.Location = new System.Drawing.Point(92, 530);
            this.PCNewDatapath.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.PCNewDatapath.MiddlePoint_1 = new System.Drawing.Point(-1, -1);
            this.PCNewDatapath.MiddlePoint_2 = new System.Drawing.Point(1, -1);
            this.PCNewDatapath.Name = "PCNewDatapath";
            this.PCNewDatapath.Padding = new System.Windows.Forms.Padding(2);
            this.PCNewDatapath.Rotation = 0;
            this.PCNewDatapath.Size = new System.Drawing.Size(593, 39);
            this.PCNewDatapath.StartPoint = new System.Drawing.Point(-1, 1);
            this.PCNewDatapath.TabIndex = 41;
            this.PCNewDatapath.TextCPoint = new System.Drawing.Point(-1, -1);
            this.PCNewDatapath.WidthOfLine = 4;
            // 
            // fwDatapathWBtoMEM
            // 
            this.fwDatapathWBtoMEM.Appearance = superscalar_arch_sim_gui.UserControls.CustomControls.Datapath.LineApperance.Double;
            this.fwDatapathWBtoMEM.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.fwDatapathWBtoMEM.ColorActive = System.Drawing.Color.Red;
            this.fwDatapathWBtoMEM.DataFormat = "X8";
            this.fwDatapathWBtoMEM.DataValue = null;
            this.fwDatapathWBtoMEM.DoubleActiveOutlineColor = System.Drawing.Color.Black;
            this.fwDatapathWBtoMEM.DoubleInactiveInnerColor = System.Drawing.SystemColors.Control;
            this.fwDatapathWBtoMEM.EndCapSize = 2F;
            this.fwDatapathWBtoMEM.EndPoint = new System.Drawing.Point(-1, 1);
            this.fwDatapathWBtoMEM.Font = new System.Drawing.Font("Microsoft YaHei UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.fwDatapathWBtoMEM.Location = new System.Drawing.Point(753, 52);
            this.fwDatapathWBtoMEM.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.fwDatapathWBtoMEM.MiddlePoint_1 = new System.Drawing.Point(1, -1);
            this.fwDatapathWBtoMEM.MiddlePoint_2 = new System.Drawing.Point(1, 1);
            this.fwDatapathWBtoMEM.Name = "fwDatapathWBtoMEM";
            this.fwDatapathWBtoMEM.Padding = new System.Windows.Forms.Padding(2);
            this.fwDatapathWBtoMEM.Rotation = 90;
            this.fwDatapathWBtoMEM.Size = new System.Drawing.Size(225, 25);
            this.fwDatapathWBtoMEM.StartPoint = new System.Drawing.Point(-1, -1);
            this.fwDatapathWBtoMEM.TabIndex = 38;
            this.fwDatapathWBtoMEM.TextCPoint = new System.Drawing.Point(109, 13);
            this.fwDatapathWBtoMEM.WidthOfLine = 4;
            // 
            // fwDatapathWBtoEX
            // 
            this.fwDatapathWBtoEX.Appearance = superscalar_arch_sim_gui.UserControls.CustomControls.Datapath.LineApperance.Double;
            this.fwDatapathWBtoEX.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.fwDatapathWBtoEX.ColorActive = System.Drawing.Color.Red;
            this.fwDatapathWBtoEX.DataFormat = "X8";
            this.fwDatapathWBtoEX.DataValue = null;
            this.fwDatapathWBtoEX.DoubleActiveOutlineColor = System.Drawing.Color.Black;
            this.fwDatapathWBtoEX.DoubleInactiveInnerColor = System.Drawing.SystemColors.Control;
            this.fwDatapathWBtoEX.EndCapSize = 2F;
            this.fwDatapathWBtoEX.EndPoint = new System.Drawing.Point(-1, 1);
            this.fwDatapathWBtoEX.Font = new System.Drawing.Font("Microsoft YaHei UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.fwDatapathWBtoEX.Location = new System.Drawing.Point(473, 31);
            this.fwDatapathWBtoEX.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.fwDatapathWBtoEX.MiddlePoint_1 = new System.Drawing.Point(1, -1);
            this.fwDatapathWBtoEX.MiddlePoint_2 = new System.Drawing.Point(1, 1);
            this.fwDatapathWBtoEX.Name = "fwDatapathWBtoEX";
            this.fwDatapathWBtoEX.Padding = new System.Windows.Forms.Padding(2);
            this.fwDatapathWBtoEX.Rotation = 90;
            this.fwDatapathWBtoEX.Size = new System.Drawing.Size(519, 46);
            this.fwDatapathWBtoEX.StartPoint = new System.Drawing.Point(-1, -1);
            this.fwDatapathWBtoEX.TabIndex = 36;
            this.fwDatapathWBtoEX.TextCPoint = new System.Drawing.Point(253, 13);
            this.fwDatapathWBtoEX.WidthOfLine = 4;
            // 
            // simCountersView1
            // 
            this.simCountersView1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.simCountersView1.Location = new System.Drawing.Point(0, 598);
            this.simCountersView1.Name = "simCountersView1";
            this.simCountersView1.Size = new System.Drawing.Size(1550, 80);
            this.simCountersView1.TabIndex = 44;
            // 
            // ScalarCoreView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.simCountersView1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.branchPredictorView1);
            this.Controls.Add(this.EXMEMbufferView);
            this.Controls.Add(this.GlobalPCTextBox);
            this.Controls.Add(this.PanelTerminationBuffer);
            this.Controls.Add(this.PanelInstructionFetchReg);
            this.Controls.Add(this.MEMWBbufferView);
            this.Controls.Add(this.IDEXbufferView);
            this.Controls.Add(this.IFIDbufferView);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.stageViewWB);
            this.Controls.Add(this.stageViewMEM);
            this.Controls.Add(this.stageViewEX);
            this.Controls.Add(this.stageViewID);
            this.Controls.Add(this.stageViewIF);
            this.Controls.Add(this.fwDatapathMEMtoEX);
            this.Controls.Add(this.PCNewDatapath);
            this.Controls.Add(this.fwDatapathWBtoMEM);
            this.Controls.Add(this.PanelFeedbackFWdatapath);
            this.Controls.Add(this.fwDatapathWBtoEX);
            this.Name = "ScalarCoreView";
            this.Size = new System.Drawing.Size(1550, 678);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox GlobalPCTextBox;
        private StageView stageViewWB;
        private StageView stageViewMEM;
        private StageView stageViewEX;
        private StageView stageViewID;
        private StageView stageViewIF;
        private BufferView IFIDbufferView;
        private BufferView IDEXbufferView;
        private BufferView EXMEMbufferView;
        private BufferView MEMWBbufferView;
        private System.Windows.Forms.Panel PanelInstructionFetchReg;
        private System.Windows.Forms.Panel PanelTerminationBuffer;
        private FWDatapath fwDatapathWBtoEX;
        private System.Windows.Forms.Panel PanelFeedbackFWdatapath;
        private FWDatapath PCNewDatapath;
        private FWDatapath fwDatapathMEMtoEX;
        private FWDatapath fwDatapathWBtoMEM;
        private Units.BranchPredictorView branchPredictorView1;
        private System.Windows.Forms.GroupBox groupBox1;
        private Inspection.QuickMemViewer quickMemViewer1;
        private Inspection.SimCountersView simCountersView1;
    }
}
