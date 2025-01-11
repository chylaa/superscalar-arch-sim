namespace superscalar_arch_sim_gui.Forms
{
    partial class BranchPredictorDetailsView
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label14 = new System.Windows.Forms.Label();
            this.BPSchemeTextBox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.BPPredictionTextBox = new System.Windows.Forms.TextBox();
            this.BTargetAddrTextBox = new System.Windows.Forms.TextBox();
            this.BConditionTextBox = new System.Windows.Forms.TextBox();
            this.BranchPredictorDetailsPanel = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.PHTgroupBox = new System.Windows.Forms.GroupBox();
            this.PHTListView = new System.Windows.Forms.ListView();
            this.BHTgroupBox = new System.Windows.Forms.GroupBox();
            this.BHTListView = new System.Windows.Forms.ListView();
            this.BranchStatCountersPanel = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.BMispredictedTextBoxCnt = new System.Windows.Forms.TextBox();
            this.BPredictedTextBoxCnt = new System.Windows.Forms.TextBox();
            this.BNotTakenTextBoxCnt = new System.Windows.Forms.TextBox();
            this.BTakenTextBoxCnt = new System.Windows.Forms.TextBox();
            this.BranchPatternLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.BranchPredictorDetailsPanel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.PHTgroupBox.SuspendLayout();
            this.BHTgroupBox.SuspendLayout();
            this.BranchStatCountersPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label14
            // 
            this.label14.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(14, 10);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(46, 13);
            this.label14.TabIndex = 15;
            this.label14.Text = "Scheme";
            // 
            // BPSchemeTextBox
            // 
            this.BPSchemeTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.BPSchemeTextBox.Location = new System.Drawing.Point(66, 6);
            this.BPSchemeTextBox.Name = "BPSchemeTextBox";
            this.BPSchemeTextBox.ReadOnly = true;
            this.BPSchemeTextBox.Size = new System.Drawing.Size(100, 20);
            this.BPSchemeTextBox.TabIndex = 14;
            this.BPSchemeTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label10
            // 
            this.label10.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 36);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(54, 13);
            this.label10.TabIndex = 13;
            this.label10.Text = "Prediction";
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(276, 36);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(38, 13);
            this.label9.TabIndex = 12;
            this.label9.Text = "Target";
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(263, 10);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(51, 13);
            this.label8.TabIndex = 11;
            this.label8.Text = "Condition";
            // 
            // BPPredictionTextBox
            // 
            this.BPPredictionTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.BPPredictionTextBox.Location = new System.Drawing.Point(66, 32);
            this.BPPredictionTextBox.Name = "BPPredictionTextBox";
            this.BPPredictionTextBox.ReadOnly = true;
            this.BPPredictionTextBox.Size = new System.Drawing.Size(100, 20);
            this.BPPredictionTextBox.TabIndex = 10;
            this.BPPredictionTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // BTargetAddrTextBox
            // 
            this.BTargetAddrTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.BTargetAddrTextBox.Location = new System.Drawing.Point(320, 32);
            this.BTargetAddrTextBox.Name = "BTargetAddrTextBox";
            this.BTargetAddrTextBox.ReadOnly = true;
            this.BTargetAddrTextBox.Size = new System.Drawing.Size(100, 20);
            this.BTargetAddrTextBox.TabIndex = 9;
            this.BTargetAddrTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // BConditionTextBox
            // 
            this.BConditionTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.BConditionTextBox.Location = new System.Drawing.Point(320, 6);
            this.BConditionTextBox.Name = "BConditionTextBox";
            this.BConditionTextBox.ReadOnly = true;
            this.BConditionTextBox.Size = new System.Drawing.Size(100, 20);
            this.BConditionTextBox.TabIndex = 8;
            this.BConditionTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // BranchPredictorDetailsPanel
            // 
            this.BranchPredictorDetailsPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.BranchPredictorDetailsPanel.Controls.Add(this.label5);
            this.BranchPredictorDetailsPanel.Controls.Add(this.BranchPatternLabel);
            this.BranchPredictorDetailsPanel.Controls.Add(this.label8);
            this.BranchPredictorDetailsPanel.Controls.Add(this.label14);
            this.BranchPredictorDetailsPanel.Controls.Add(this.BConditionTextBox);
            this.BranchPredictorDetailsPanel.Controls.Add(this.BPSchemeTextBox);
            this.BranchPredictorDetailsPanel.Controls.Add(this.BTargetAddrTextBox);
            this.BranchPredictorDetailsPanel.Controls.Add(this.label10);
            this.BranchPredictorDetailsPanel.Controls.Add(this.BPPredictionTextBox);
            this.BranchPredictorDetailsPanel.Controls.Add(this.label9);
            this.BranchPredictorDetailsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.BranchPredictorDetailsPanel.Location = new System.Drawing.Point(0, 0);
            this.BranchPredictorDetailsPanel.Name = "BranchPredictorDetailsPanel";
            this.BranchPredictorDetailsPanel.Size = new System.Drawing.Size(434, 90);
            this.BranchPredictorDetailsPanel.TabIndex = 16;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.PHTgroupBox);
            this.panel2.Controls.Add(this.BHTgroupBox);
            this.panel2.Controls.Add(this.BranchStatCountersPanel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 90);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(434, 216);
            this.panel2.TabIndex = 17;
            // 
            // PHTgroupBox
            // 
            this.PHTgroupBox.Controls.Add(this.PHTListView);
            this.PHTgroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PHTgroupBox.Location = new System.Drawing.Point(215, 0);
            this.PHTgroupBox.Name = "PHTgroupBox";
            this.PHTgroupBox.Size = new System.Drawing.Size(215, 165);
            this.PHTgroupBox.TabIndex = 1;
            this.PHTgroupBox.TabStop = false;
            this.PHTgroupBox.Text = "PHT (Pattern History Table)";
            // 
            // PHTListView
            // 
            this.PHTListView.BackColor = System.Drawing.SystemColors.Control;
            this.PHTListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.PHTListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PHTListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.PHTListView.HideSelection = false;
            this.PHTListView.Location = new System.Drawing.Point(3, 16);
            this.PHTListView.Name = "PHTListView";
            this.PHTListView.Size = new System.Drawing.Size(209, 146);
            this.PHTListView.TabIndex = 1;
            this.PHTListView.UseCompatibleStateImageBehavior = false;
            this.PHTListView.View = System.Windows.Forms.View.Details;
            // 
            // BHTgroupBox
            // 
            this.BHTgroupBox.Controls.Add(this.BHTListView);
            this.BHTgroupBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.BHTgroupBox.Location = new System.Drawing.Point(0, 0);
            this.BHTgroupBox.Name = "BHTgroupBox";
            this.BHTgroupBox.Size = new System.Drawing.Size(215, 165);
            this.BHTgroupBox.TabIndex = 0;
            this.BHTgroupBox.TabStop = false;
            this.BHTgroupBox.Text = "BHT (Branch History Table)";
            // 
            // BHTListView
            // 
            this.BHTListView.BackColor = System.Drawing.SystemColors.Control;
            this.BHTListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.BHTListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BHTListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.BHTListView.HideSelection = false;
            this.BHTListView.Location = new System.Drawing.Point(3, 16);
            this.BHTListView.Name = "BHTListView";
            this.BHTListView.Size = new System.Drawing.Size(209, 146);
            this.BHTListView.TabIndex = 0;
            this.BHTListView.UseCompatibleStateImageBehavior = false;
            this.BHTListView.View = System.Windows.Forms.View.Details;
            // 
            // BranchStatCountersPanel
            // 
            this.BranchStatCountersPanel.Controls.Add(this.label3);
            this.BranchStatCountersPanel.Controls.Add(this.label4);
            this.BranchStatCountersPanel.Controls.Add(this.label2);
            this.BranchStatCountersPanel.Controls.Add(this.label1);
            this.BranchStatCountersPanel.Controls.Add(this.BMispredictedTextBoxCnt);
            this.BranchStatCountersPanel.Controls.Add(this.BPredictedTextBoxCnt);
            this.BranchStatCountersPanel.Controls.Add(this.BNotTakenTextBoxCnt);
            this.BranchStatCountersPanel.Controls.Add(this.BTakenTextBoxCnt);
            this.BranchStatCountersPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.BranchStatCountersPanel.Location = new System.Drawing.Point(0, 165);
            this.BranchStatCountersPanel.Name = "BranchStatCountersPanel";
            this.BranchStatCountersPanel.Size = new System.Drawing.Size(430, 47);
            this.BranchStatCountersPanel.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.Enabled = false;
            this.label3.Location = new System.Drawing.Point(328, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Mispredicted";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.Enabled = false;
            this.label4.Location = new System.Drawing.Point(222, 5);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Predicted";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.Enabled = false;
            this.label2.Location = new System.Drawing.Point(109, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Not Taken";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.Enabled = false;
            this.label1.Location = new System.Drawing.Point(3, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Taken";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BMispredictedTextBoxCnt
            // 
            this.BMispredictedTextBoxCnt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BMispredictedTextBoxCnt.Location = new System.Drawing.Point(328, 21);
            this.BMispredictedTextBoxCnt.Name = "BMispredictedTextBoxCnt";
            this.BMispredictedTextBoxCnt.ReadOnly = true;
            this.BMispredictedTextBoxCnt.Size = new System.Drawing.Size(100, 20);
            this.BMispredictedTextBoxCnt.TabIndex = 3;
            this.BMispredictedTextBoxCnt.Text = "0";
            this.BMispredictedTextBoxCnt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // BPredictedTextBoxCnt
            // 
            this.BPredictedTextBoxCnt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BPredictedTextBoxCnt.Location = new System.Drawing.Point(222, 21);
            this.BPredictedTextBoxCnt.Name = "BPredictedTextBoxCnt";
            this.BPredictedTextBoxCnt.ReadOnly = true;
            this.BPredictedTextBoxCnt.Size = new System.Drawing.Size(100, 20);
            this.BPredictedTextBoxCnt.TabIndex = 2;
            this.BPredictedTextBoxCnt.Text = "0";
            this.BPredictedTextBoxCnt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // BNotTakenTextBoxCnt
            // 
            this.BNotTakenTextBoxCnt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BNotTakenTextBoxCnt.Location = new System.Drawing.Point(109, 21);
            this.BNotTakenTextBoxCnt.Name = "BNotTakenTextBoxCnt";
            this.BNotTakenTextBoxCnt.ReadOnly = true;
            this.BNotTakenTextBoxCnt.Size = new System.Drawing.Size(100, 20);
            this.BNotTakenTextBoxCnt.TabIndex = 1;
            this.BNotTakenTextBoxCnt.Text = "0";
            this.BNotTakenTextBoxCnt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // BTakenTextBoxCnt
            // 
            this.BTakenTextBoxCnt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BTakenTextBoxCnt.Location = new System.Drawing.Point(3, 21);
            this.BTakenTextBoxCnt.Name = "BTakenTextBoxCnt";
            this.BTakenTextBoxCnt.ReadOnly = true;
            this.BTakenTextBoxCnt.Size = new System.Drawing.Size(100, 20);
            this.BTakenTextBoxCnt.TabIndex = 0;
            this.BTakenTextBoxCnt.Text = "0";
            this.BTakenTextBoxCnt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // BranchPatternLabel
            // 
            this.BranchPatternLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BranchPatternLabel.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.BranchPatternLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.BranchPatternLabel.Font = new System.Drawing.Font("Cascadia Mono SemiBold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.BranchPatternLabel.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.BranchPatternLabel.Location = new System.Drawing.Point(3, 69);
            this.BranchPatternLabel.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.BranchPatternLabel.Name = "BranchPatternLabel";
            this.BranchPatternLabel.Size = new System.Drawing.Size(425, 16);
            this.BranchPatternLabel.TabIndex = 31;
            this.BranchPatternLabel.Text = "0000-0000-0000-0000-0000-0000-0000-0000";
            this.BranchPatternLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label5.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label5.Location = new System.Drawing.Point(3, 54);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(424, 15);
            this.label5.TabIndex = 32;
            this.label5.Text = "Branch Pattern";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // BranchPredictorDetailsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 306);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.BranchPredictorDetailsPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(450, 1000);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(450, 100);
            this.Name = "BranchPredictorDetailsView";
            this.Text = "Branch Predictor Details";
            this.TopMost = true;
            this.BranchPredictorDetailsPanel.ResumeLayout(false);
            this.BranchPredictorDetailsPanel.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.PHTgroupBox.ResumeLayout(false);
            this.BHTgroupBox.ResumeLayout(false);
            this.BranchStatCountersPanel.ResumeLayout(false);
            this.BranchStatCountersPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox BPSchemeTextBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox BPPredictionTextBox;
        private System.Windows.Forms.TextBox BTargetAddrTextBox;
        private System.Windows.Forms.TextBox BConditionTextBox;
        private System.Windows.Forms.Panel BranchPredictorDetailsPanel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox BHTgroupBox;
        private System.Windows.Forms.GroupBox PHTgroupBox;
        private System.Windows.Forms.ListView PHTListView;
        private System.Windows.Forms.ListView BHTListView;
        private System.Windows.Forms.Panel BranchStatCountersPanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox BMispredictedTextBoxCnt;
        private System.Windows.Forms.TextBox BPredictedTextBoxCnt;
        private System.Windows.Forms.TextBox BNotTakenTextBoxCnt;
        private System.Windows.Forms.TextBox BTakenTextBoxCnt;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label BranchPatternLabel;
        private System.Windows.Forms.Label label5;
    }
}