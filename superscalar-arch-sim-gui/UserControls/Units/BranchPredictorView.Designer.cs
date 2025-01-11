namespace superscalar_arch_sim_gui.UserControls.Units
{
    partial class BranchPredictorView
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
            this.branchPredictorGroupBox = new System.Windows.Forms.GroupBox();
            this.branchPredictorDetailsButton = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.BPSchemeTextBox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.BPPredictionTextBox = new System.Windows.Forms.TextBox();
            this.BTargetAddrTextBox = new System.Windows.Forms.TextBox();
            this.BConditionTextBox = new System.Windows.Forms.TextBox();
            this.branchPredictorGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // branchPredictorGroupBox
            // 
            this.branchPredictorGroupBox.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.branchPredictorGroupBox.Controls.Add(this.branchPredictorDetailsButton);
            this.branchPredictorGroupBox.Controls.Add(this.label14);
            this.branchPredictorGroupBox.Controls.Add(this.BPSchemeTextBox);
            this.branchPredictorGroupBox.Controls.Add(this.label10);
            this.branchPredictorGroupBox.Controls.Add(this.label9);
            this.branchPredictorGroupBox.Controls.Add(this.label8);
            this.branchPredictorGroupBox.Controls.Add(this.BPPredictionTextBox);
            this.branchPredictorGroupBox.Controls.Add(this.BTargetAddrTextBox);
            this.branchPredictorGroupBox.Controls.Add(this.BConditionTextBox);
            this.branchPredictorGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.branchPredictorGroupBox.Location = new System.Drawing.Point(0, 0);
            this.branchPredictorGroupBox.Name = "branchPredictorGroupBox";
            this.branchPredictorGroupBox.Size = new System.Drawing.Size(170, 143);
            this.branchPredictorGroupBox.TabIndex = 43;
            this.branchPredictorGroupBox.TabStop = false;
            this.branchPredictorGroupBox.Text = "Branch Predictor";
            // 
            // branchPredictorDetailsButton
            // 
            this.branchPredictorDetailsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.branchPredictorDetailsButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.branchPredictorDetailsButton.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.branchPredictorDetailsButton.Location = new System.Drawing.Point(130, 13);
            this.branchPredictorDetailsButton.Name = "branchPredictorDetailsButton";
            this.branchPredictorDetailsButton.Size = new System.Drawing.Size(33, 18);
            this.branchPredictorDetailsButton.TabIndex = 8;
            this.branchPredictorDetailsButton.Text = "...";
            this.branchPredictorDetailsButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.branchPredictorDetailsButton.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(11, 44);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(46, 13);
            this.label14.TabIndex = 7;
            this.label14.Text = "Scheme";
            // 
            // BPSchemeTextBox
            // 
            this.BPSchemeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BPSchemeTextBox.Location = new System.Drawing.Point(63, 41);
            this.BPSchemeTextBox.Name = "BPSchemeTextBox";
            this.BPSchemeTextBox.ReadOnly = true;
            this.BPSchemeTextBox.Size = new System.Drawing.Size(100, 20);
            this.BPSchemeTextBox.TabIndex = 6;
            this.BPSchemeTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 120);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(54, 13);
            this.label10.TabIndex = 5;
            this.label10.Text = "Prediction";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(19, 96);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(38, 13);
            this.label9.TabIndex = 4;
            this.label9.Text = "Target";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 70);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(51, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "Condition";
            // 
            // BPPredictionTextBox
            // 
            this.BPPredictionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BPPredictionTextBox.Location = new System.Drawing.Point(63, 117);
            this.BPPredictionTextBox.Name = "BPPredictionTextBox";
            this.BPPredictionTextBox.ReadOnly = true;
            this.BPPredictionTextBox.Size = new System.Drawing.Size(100, 20);
            this.BPPredictionTextBox.TabIndex = 2;
            this.BPPredictionTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // BTargetAddrTextBox
            // 
            this.BTargetAddrTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BTargetAddrTextBox.Location = new System.Drawing.Point(63, 93);
            this.BTargetAddrTextBox.Name = "BTargetAddrTextBox";
            this.BTargetAddrTextBox.ReadOnly = true;
            this.BTargetAddrTextBox.Size = new System.Drawing.Size(100, 20);
            this.BTargetAddrTextBox.TabIndex = 1;
            this.BTargetAddrTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // BConditionTextBox
            // 
            this.BConditionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BConditionTextBox.Location = new System.Drawing.Point(63, 67);
            this.BConditionTextBox.Name = "BConditionTextBox";
            this.BConditionTextBox.ReadOnly = true;
            this.BConditionTextBox.Size = new System.Drawing.Size(100, 20);
            this.BConditionTextBox.TabIndex = 0;
            this.BConditionTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // BranchPredictorView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.branchPredictorGroupBox);
            this.Name = "BranchPredictorView";
            this.Size = new System.Drawing.Size(170, 143);
            this.branchPredictorGroupBox.ResumeLayout(false);
            this.branchPredictorGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox branchPredictorGroupBox;
        private System.Windows.Forms.Button branchPredictorDetailsButton;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox BPSchemeTextBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox BPPredictionTextBox;
        private System.Windows.Forms.TextBox BTargetAddrTextBox;
        private System.Windows.Forms.TextBox BConditionTextBox;
    }
}
