namespace superscalar_arch_sim_gui.Forms
{
    partial class MemoryViewer
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.ROMMemoryViewControl = new superscalar_arch_sim_gui.UserControls.Units.MemoryViewControl();
            this.RAMMemoryViewControl = new superscalar_arch_sim_gui.UserControls.Units.MemoryViewControl();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.ROMMemoryViewControl);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.RAMMemoryViewControl);
            this.splitContainer1.Size = new System.Drawing.Size(1184, 561);
            this.splitContainer1.SplitterDistance = 601;
            this.splitContainer1.TabIndex = 0;
            // 
            // ROMMemoryViewControl
            // 
            this.ROMMemoryViewControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ROMMemoryViewControl.HighlightAddress = null;
            this.ROMMemoryViewControl.Location = new System.Drawing.Point(0, 0);
            this.ROMMemoryViewControl.Name = "ROMMemoryViewControl";
            this.ROMMemoryViewControl.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.ROMMemoryViewControl.Size = new System.Drawing.Size(601, 561);
            this.ROMMemoryViewControl.TabIndex = 0;
            // 
            // RAMMemoryViewControl
            // 
            this.RAMMemoryViewControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RAMMemoryViewControl.HighlightAddress = null;
            this.RAMMemoryViewControl.Location = new System.Drawing.Point(0, 0);
            this.RAMMemoryViewControl.Name = "RAMMemoryViewControl";
            this.RAMMemoryViewControl.Size = new System.Drawing.Size(579, 561);
            this.RAMMemoryViewControl.TabIndex = 0;
            // 
            // MemoryViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 561);
            this.Controls.Add(this.splitContainer1);
            this.Name = "MemoryViewer";
            this.ShowIcon = false;
            this.Text = "CPU Memory";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private UserControls.Units.MemoryViewControl ROMMemoryViewControl;
        private UserControls.Units.MemoryViewControl RAMMemoryViewControl;
    }
}