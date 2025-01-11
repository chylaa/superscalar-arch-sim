namespace superscalar_arch_sim_gui.UserControls.Core.Dynamic
{
    partial class MultiIssueStageView
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.StageNameLabel = new System.Windows.Forms.Label();
            this.StallingCheckBox = new System.Windows.Forms.CheckBox();
            this.StageGridView = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.StageGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // StageNameLabel
            // 
            this.StageNameLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.StageNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.StageNameLabel.Location = new System.Drawing.Point(0, 0);
            this.StageNameLabel.Margin = new System.Windows.Forms.Padding(3);
            this.StageNameLabel.Name = "StageNameLabel";
            this.StageNameLabel.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.StageNameLabel.Size = new System.Drawing.Size(250, 22);
            this.StageNameLabel.TabIndex = 1;
            this.StageNameLabel.Text = "Name";
            this.StageNameLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // StallingCheckBox
            // 
            this.StallingCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.StallingCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
            this.StallingCheckBox.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.StallingCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.StallingCheckBox.Enabled = false;
            this.StallingCheckBox.Location = new System.Drawing.Point(230, 3);
            this.StallingCheckBox.Name = "StallingCheckBox";
            this.StallingCheckBox.Size = new System.Drawing.Size(17, 17);
            this.StallingCheckBox.TabIndex = 4;
            this.StallingCheckBox.UseVisualStyleBackColor = false;
            // 
            // StageGridView
            // 
            this.StageGridView.AllowUserToAddRows = false;
            this.StageGridView.AllowUserToDeleteRows = false;
            this.StageGridView.BackgroundColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.StageGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.StageGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.StageGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.StageGridView.DefaultCellStyle = dataGridViewCellStyle4;
            this.StageGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.StageGridView.GridColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.StageGridView.Location = new System.Drawing.Point(0, 22);
            this.StageGridView.Margin = new System.Windows.Forms.Padding(0);
            this.StageGridView.Name = "StageGridView";
            this.StageGridView.ReadOnly = true;
            this.StageGridView.RowHeadersVisible = false;
            this.StageGridView.Size = new System.Drawing.Size(250, 128);
            this.StageGridView.TabIndex = 5;
            // 
            // MultiIssueStageView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.Controls.Add(this.StageGridView);
            this.Controls.Add(this.StallingCheckBox);
            this.Controls.Add(this.StageNameLabel);
            this.Name = "MultiIssueStageView";
            this.Size = new System.Drawing.Size(250, 150);
            ((System.ComponentModel.ISupportInitialize)(this.StageGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label StageNameLabel;
        private System.Windows.Forms.CheckBox StallingCheckBox;
        private System.Windows.Forms.DataGridView StageGridView;
    }
}
