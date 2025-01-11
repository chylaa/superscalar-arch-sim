namespace superscalar_arch_sim_gui.UserControls.Units
{
    partial class RegisterFileTemplateView
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
            this.components = new System.ComponentModel.Container();
            this.RegDetailsListView = new System.Windows.Forms.ListView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.valueFormatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.signedIntegerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.singleFloatingPointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aSCIIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ResStationTagListView = new System.Windows.Forms.ListView();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // RegDetailsListView
            // 
            this.RegDetailsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RegDetailsListView.FullRowSelect = true;
            this.RegDetailsListView.GridLines = true;
            this.RegDetailsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.RegDetailsListView.HideSelection = false;
            this.RegDetailsListView.LabelEdit = true;
            this.RegDetailsListView.Location = new System.Drawing.Point(50, 0);
            this.RegDetailsListView.Margin = new System.Windows.Forms.Padding(10);
            this.RegDetailsListView.Name = "RegDetailsListView";
            this.RegDetailsListView.Size = new System.Drawing.Size(560, 575);
            this.RegDetailsListView.TabIndex = 1;
            this.RegDetailsListView.UseCompatibleStateImageBehavior = false;
            this.RegDetailsListView.View = System.Windows.Forms.View.Details;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshToolStripMenuItem,
            this.valueFormatToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(147, 48);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            // 
            // valueFormatToolStripMenuItem
            // 
            this.valueFormatToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.signedIntegerToolStripMenuItem,
            this.singleFloatingPointToolStripMenuItem,
            this.aSCIIToolStripMenuItem});
            this.valueFormatToolStripMenuItem.Name = "valueFormatToolStripMenuItem";
            this.valueFormatToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.valueFormatToolStripMenuItem.Text = "Value Format ";
            // 
            // signedIntegerToolStripMenuItem
            // 
            this.signedIntegerToolStripMenuItem.Checked = true;
            this.signedIntegerToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.signedIntegerToolStripMenuItem.Name = "signedIntegerToolStripMenuItem";
            this.signedIntegerToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.signedIntegerToolStripMenuItem.Text = "Signed Integer";
            // 
            // singleFloatingPointToolStripMenuItem
            // 
            this.singleFloatingPointToolStripMenuItem.Name = "singleFloatingPointToolStripMenuItem";
            this.singleFloatingPointToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.singleFloatingPointToolStripMenuItem.Text = "Single Floating Point";
            // 
            // aSCIIToolStripMenuItem
            // 
            this.aSCIIToolStripMenuItem.Name = "aSCIIToolStripMenuItem";
            this.aSCIIToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.aSCIIToolStripMenuItem.Text = "ASCII";
            // 
            // ResStationTagListView
            // 
            this.ResStationTagListView.Dock = System.Windows.Forms.DockStyle.Left;
            this.ResStationTagListView.FullRowSelect = true;
            this.ResStationTagListView.GridLines = true;
            this.ResStationTagListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.ResStationTagListView.HideSelection = false;
            this.ResStationTagListView.LabelEdit = true;
            this.ResStationTagListView.Location = new System.Drawing.Point(0, 0);
            this.ResStationTagListView.Margin = new System.Windows.Forms.Padding(10);
            this.ResStationTagListView.Name = "ResStationTagListView";
            this.ResStationTagListView.Scrollable = false;
            this.ResStationTagListView.Size = new System.Drawing.Size(50, 575);
            this.ResStationTagListView.TabIndex = 2;
            this.ResStationTagListView.UseCompatibleStateImageBehavior = false;
            this.ResStationTagListView.View = System.Windows.Forms.View.Details;
            // 
            // RegisterFileTemplateView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.RegDetailsListView);
            this.Controls.Add(this.ResStationTagListView);
            this.Name = "RegisterFileTemplateView";
            this.Size = new System.Drawing.Size(610, 575);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView RegDetailsListView;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem valueFormatToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem signedIntegerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem singleFloatingPointToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aSCIIToolStripMenuItem;
        private System.Windows.Forms.ListView ResStationTagListView;
    }
}
