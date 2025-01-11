namespace superscalar_arch_sim_gui.UserControls.Core.Static
{
    partial class StageView
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
            this.StageNameLabel = new System.Windows.Forms.Label();
            this.InstructionTextBox = new System.Windows.Forms.TextBox();
            this.StallingCheckBox = new System.Windows.Forms.CheckBox();
            this.LocalPCTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // StageNameLabel
            // 
            this.StageNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.StageNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.StageNameLabel.Location = new System.Drawing.Point(3, 26);
            this.StageNameLabel.Name = "StageNameLabel";
            this.StageNameLabel.Size = new System.Drawing.Size(109, 16);
            this.StageNameLabel.TabIndex = 0;
            this.StageNameLabel.Text = "Name";
            this.StageNameLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // InstructionTextBox
            // 
            this.InstructionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InstructionTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.InstructionTextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.InstructionTextBox.Location = new System.Drawing.Point(3, 127);
            this.InstructionTextBox.Name = "InstructionTextBox";
            this.InstructionTextBox.ReadOnly = true;
            this.InstructionTextBox.Size = new System.Drawing.Size(109, 20);
            this.InstructionTextBox.TabIndex = 2;
            this.InstructionTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.InstructionTextBox.WordWrap = false;
            // 
            // StallingCheckBox
            // 
            this.StallingCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.StallingCheckBox.AutoSize = true;
            this.StallingCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.StallingCheckBox.Enabled = false;
            this.StallingCheckBox.Location = new System.Drawing.Point(66, 101);
            this.StallingCheckBox.Name = "StallingCheckBox";
            this.StallingCheckBox.Size = new System.Drawing.Size(46, 17);
            this.StallingCheckBox.TabIndex = 3;
            this.StallingCheckBox.Text = "Stall";
            this.StallingCheckBox.UseVisualStyleBackColor = true;
            // 
            // LocalPCTextBox
            // 
            this.LocalPCTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LocalPCTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.LocalPCTextBox.Enabled = false;
            this.LocalPCTextBox.Location = new System.Drawing.Point(3, 3);
            this.LocalPCTextBox.Name = "LocalPCTextBox";
            this.LocalPCTextBox.ReadOnly = true;
            this.LocalPCTextBox.Size = new System.Drawing.Size(109, 20);
            this.LocalPCTextBox.TabIndex = 6;
            this.LocalPCTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // StageView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.Controls.Add(this.LocalPCTextBox);
            this.Controls.Add(this.StallingCheckBox);
            this.Controls.Add(this.InstructionTextBox);
            this.Controls.Add(this.StageNameLabel);
            this.Name = "StageView";
            this.Size = new System.Drawing.Size(115, 150);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label StageNameLabel;
        private System.Windows.Forms.TextBox InstructionTextBox;
        private System.Windows.Forms.CheckBox StallingCheckBox;
        private System.Windows.Forms.TextBox LocalPCTextBox;
    }
}
