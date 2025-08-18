namespace superscalar_arch_sim_gui.Forms
{
    partial class IOTerminal
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
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.settingsGroupBox = new System.Windows.Forms.GroupBox();
            this.txKeyViewLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txByteAddressNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.rxByteAddressNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.enableBufferingCheckBox = new System.Windows.Forms.CheckBox();
            this.terminalTextBox = new superscalar_arch_sim_gui.UserControls.CustomControls.TerminalTextBox();
            this.panel1.SuspendLayout();
            this.settingsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txByteAddressNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rxByteAddressNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.settingsGroupBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 439);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(943, 61);
            this.panel1.TabIndex = 1;
            // 
            // settingsGroupBox
            // 
            this.settingsGroupBox.Controls.Add(this.enableBufferingCheckBox);
            this.settingsGroupBox.Controls.Add(this.txKeyViewLabel);
            this.settingsGroupBox.Controls.Add(this.label2);
            this.settingsGroupBox.Controls.Add(this.txByteAddressNumericUpDown);
            this.settingsGroupBox.Controls.Add(this.label1);
            this.settingsGroupBox.Controls.Add(this.rxByteAddressNumericUpDown);
            this.settingsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingsGroupBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.settingsGroupBox.Location = new System.Drawing.Point(0, 0);
            this.settingsGroupBox.Name = "settingsGroupBox";
            this.settingsGroupBox.Size = new System.Drawing.Size(943, 61);
            this.settingsGroupBox.TabIndex = 0;
            this.settingsGroupBox.TabStop = false;
            this.settingsGroupBox.Text = "Settings";
            // 
            // txKeyViewLabel
            // 
            this.txKeyViewLabel.AutoSize = true;
            this.txKeyViewLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.txKeyViewLabel.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txKeyViewLabel.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.txKeyViewLabel.Location = new System.Drawing.Point(3, 18);
            this.txKeyViewLabel.Name = "txKeyViewLabel";
            this.txKeyViewLabel.Padding = new System.Windows.Forms.Padding(0, 15, 0, 0);
            this.txKeyViewLabel.Size = new System.Drawing.Size(0, 34);
            this.txKeyViewLabel.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(489, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "TX byte address";
            // 
            // txByteAddressNumericUpDown
            // 
            this.txByteAddressNumericUpDown.Hexadecimal = true;
            this.txByteAddressNumericUpDown.Location = new System.Drawing.Point(470, 30);
            this.txByteAddressNumericUpDown.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.txByteAddressNumericUpDown.Name = "txByteAddressNumericUpDown";
            this.txByteAddressNumericUpDown.Size = new System.Drawing.Size(147, 22);
            this.txByteAddressNumericUpDown.TabIndex = 2;
            this.txByteAddressNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.txByteAddressNumericUpDown, "Address of byte sent from IOTerminal -> CPU.");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(322, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "RX byte address";
            // 
            // rxByteAddressNumericUpDown
            // 
            this.rxByteAddressNumericUpDown.Hexadecimal = true;
            this.rxByteAddressNumericUpDown.Location = new System.Drawing.Point(306, 30);
            this.rxByteAddressNumericUpDown.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.rxByteAddressNumericUpDown.Name = "rxByteAddressNumericUpDown";
            this.rxByteAddressNumericUpDown.Size = new System.Drawing.Size(147, 22);
            this.rxByteAddressNumericUpDown.TabIndex = 0;
            this.rxByteAddressNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.rxByteAddressNumericUpDown, "Address of byte received from CPU -> IOTerminal");
            // 
            // enableBufferingCheckBox
            // 
            this.enableBufferingCheckBox.AutoSize = true;
            this.enableBufferingCheckBox.Checked = true;
            this.enableBufferingCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.enableBufferingCheckBox.Location = new System.Drawing.Point(802, 29);
            this.enableBufferingCheckBox.Name = "enableBufferingCheckBox";
            this.enableBufferingCheckBox.Size = new System.Drawing.Size(129, 20);
            this.enableBufferingCheckBox.TabIndex = 7;
            this.enableBufferingCheckBox.Text = "Buffer Crtl+V input";
            this.toolTip1.SetToolTip(this.enableBufferingCheckBox, "When pasting text into terminal using Ctrl+V shortcut, \r\nthe input will be buffer" +
        "ed and printed on each new line.");
            this.enableBufferingCheckBox.UseVisualStyleBackColor = true;
            // 
            // terminalTextBox
            // 
            this.terminalTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.terminalTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.terminalTextBox.EnableBuffering = false;
            this.terminalTextBox.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.terminalTextBox.ForeColor = System.Drawing.SystemColors.Info;
            this.terminalTextBox.Location = new System.Drawing.Point(0, 0);
            this.terminalTextBox.Name = "terminalTextBox";
            this.terminalTextBox.ReadOnly = true;
            this.terminalTextBox.Size = new System.Drawing.Size(943, 439);
            this.terminalTextBox.TabIndex = 0;
            // 
            // IOTerminal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(943, 500);
            this.Controls.Add(this.terminalTextBox);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "IOTerminal";
            this.Text = "IOTerminal";
            this.panel1.ResumeLayout(false);
            this.settingsGroupBox.ResumeLayout(false);
            this.settingsGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txByteAddressNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rxByteAddressNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private superscalar_arch_sim_gui.UserControls.CustomControls.TerminalTextBox terminalTextBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox settingsGroupBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown txByteAddressNumericUpDown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown rxByteAddressNumericUpDown;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label txKeyViewLabel;
        private System.Windows.Forms.CheckBox enableBufferingCheckBox;
    }
}