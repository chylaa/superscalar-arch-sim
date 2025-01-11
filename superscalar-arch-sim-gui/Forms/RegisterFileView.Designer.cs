namespace superscalar_arch_sim_gui.Forms
{
    partial class RegisterFileView
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
            this.RegFileTemplateView = new superscalar_arch_sim_gui.UserControls.Units.RegisterFileTemplateView();
            this.SuspendLayout();
            // 
            // RegFileTemplateView
            // 
            this.RegFileTemplateView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RegFileTemplateView.Location = new System.Drawing.Point(3, 3);
            this.RegFileTemplateView.Name = "RegFileTemplateView";
            this.RegFileTemplateView.Size = new System.Drawing.Size(613, 575);
            this.RegFileTemplateView.TabIndex = 0;
            // 
            // RegisterFileView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(619, 581);
            this.Controls.Add(this.RegFileTemplateView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "RegisterFileView";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "CPU Register File";
            this.ResumeLayout(false);

        }

        #endregion

        private UserControls.Units.RegisterFileTemplateView RegFileTemplateView;
    }
}