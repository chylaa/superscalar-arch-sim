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
            this.terminalTextBox = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.settingsGroupBox = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.controlByteAddressNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.inputByteAddressNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.outputByteAddressNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.panel1.SuspendLayout();
            this.settingsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.controlByteAddressNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inputByteAddressNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.outputByteAddressNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // terminalTextBox
            // 
            this.terminalTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.terminalTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.terminalTextBox.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.terminalTextBox.Location = new System.Drawing.Point(0, 0);
            this.terminalTextBox.Multiline = true;
            this.terminalTextBox.Name = "terminalTextBox";
            this.terminalTextBox.ReadOnly = true;
            this.terminalTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.terminalTextBox.Size = new System.Drawing.Size(800, 389);
            this.terminalTextBox.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.settingsGroupBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 389);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 61);
            this.panel1.TabIndex = 1;
            // 
            // settingsGroupBox
            // 
            this.settingsGroupBox.Controls.Add(this.label3);
            this.settingsGroupBox.Controls.Add(this.controlByteAddressNumericUpDown);
            this.settingsGroupBox.Controls.Add(this.label2);
            this.settingsGroupBox.Controls.Add(this.inputByteAddressNumericUpDown);
            this.settingsGroupBox.Controls.Add(this.label1);
            this.settingsGroupBox.Controls.Add(this.outputByteAddressNumericUpDown);
            this.settingsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingsGroupBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.settingsGroupBox.Location = new System.Drawing.Point(0, 0);
            this.settingsGroupBox.Name = "settingsGroupBox";
            this.settingsGroupBox.Size = new System.Drawing.Size(800, 61);
            this.settingsGroupBox.TabIndex = 0;
            this.settingsGroupBox.TabStop = false;
            this.settingsGroupBox.Text = "Settings";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(504, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(131, 16);
            this.label3.TabIndex = 5;
            this.label3.Text = "Control byte address";
            // 
            // controlByteAddressNumericUpDown
            // 
            this.controlByteAddressNumericUpDown.Hexadecimal = true;
            this.controlByteAddressNumericUpDown.Location = new System.Drawing.Point(495, 33);
            this.controlByteAddressNumericUpDown.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.controlByteAddressNumericUpDown.Name = "controlByteAddressNumericUpDown";
            this.controlByteAddressNumericUpDown.Size = new System.Drawing.Size(147, 22);
            this.controlByteAddressNumericUpDown.TabIndex = 4;
            this.controlByteAddressNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(338, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Input byte address";
            // 
            // inputByteAddressNumericUpDown
            // 
            this.inputByteAddressNumericUpDown.Hexadecimal = true;
            this.inputByteAddressNumericUpDown.Location = new System.Drawing.Point(329, 33);
            this.inputByteAddressNumericUpDown.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.inputByteAddressNumericUpDown.Name = "inputByteAddressNumericUpDown";
            this.inputByteAddressNumericUpDown.Size = new System.Drawing.Size(147, 22);
            this.inputByteAddressNumericUpDown.TabIndex = 2;
            this.inputByteAddressNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(174, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Output byte address";
            // 
            // outputByteAddressNumericUpDown
            // 
            this.outputByteAddressNumericUpDown.Hexadecimal = true;
            this.outputByteAddressNumericUpDown.Location = new System.Drawing.Point(165, 33);
            this.outputByteAddressNumericUpDown.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.outputByteAddressNumericUpDown.Name = "outputByteAddressNumericUpDown";
            this.outputByteAddressNumericUpDown.Size = new System.Drawing.Size(147, 22);
            this.outputByteAddressNumericUpDown.TabIndex = 0;
            this.outputByteAddressNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // IOTerminal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.terminalTextBox);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.KeyPreview = true;
            this.Name = "IOTerminal";
            this.Text = "IOTerminal";
            this.panel1.ResumeLayout(false);
            this.settingsGroupBox.ResumeLayout(false);
            this.settingsGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.controlByteAddressNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inputByteAddressNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.outputByteAddressNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox terminalTextBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox settingsGroupBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown controlByteAddressNumericUpDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown inputByteAddressNumericUpDown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown outputByteAddressNumericUpDown;
    }
}