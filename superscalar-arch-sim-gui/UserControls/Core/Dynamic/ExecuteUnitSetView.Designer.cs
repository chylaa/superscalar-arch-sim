namespace superscalar_arch_sim_gui.UserControls.Core.Dynamic
{
    partial class ExecuteUnitSetView
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
            this.UnitNameLabel = new System.Windows.Forms.Label();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // UnitNameLabel
            // 
            this.UnitNameLabel.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.UnitNameLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.UnitNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.UnitNameLabel.Location = new System.Drawing.Point(0, 0);
            this.UnitNameLabel.Margin = new System.Windows.Forms.Padding(3);
            this.UnitNameLabel.Name = "UnitNameLabel";
            this.UnitNameLabel.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.UnitNameLabel.Size = new System.Drawing.Size(148, 22);
            this.UnitNameLabel.TabIndex = 2;
            this.UnitNameLabel.Text = "Name";
            this.UnitNameLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // mainPanel
            // 
            this.mainPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 22);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(148, 126);
            this.mainPanel.TabIndex = 3;
            // 
            // ExecuteUnitSetView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.UnitNameLabel);
            this.Name = "ExecuteUnitSetView";
            this.Size = new System.Drawing.Size(148, 148);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label UnitNameLabel;
        private System.Windows.Forms.Panel mainPanel;
    }
}
