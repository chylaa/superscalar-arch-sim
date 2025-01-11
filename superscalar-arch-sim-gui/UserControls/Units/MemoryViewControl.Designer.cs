namespace superscalar_arch_sim_gui.UserControls.Units
{
    partial class MemoryViewControl
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.ViewGrid = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.EndATextBox = new System.Windows.Forms.TextBox();
            this.StartATextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.AbsoluteAddrRadioBtn = new System.Windows.Forms.RadioButton();
            this.RelativeAddrRadioBtn = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusLabel_SelectedValue = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabel_MemoryDetails = new System.Windows.Forms.ToolStripStatusLabel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.RefreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.signedIntegerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aSCIIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.instructionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.endiannessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.littleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.floatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.ViewGrid)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ViewGrid
            // 
            this.ViewGrid.AllowUserToAddRows = false;
            this.ViewGrid.AllowUserToDeleteRows = false;
            this.ViewGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.ViewGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.ViewGrid.BackgroundColor = System.Drawing.SystemColors.Control;
            this.ViewGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ViewGrid.ColumnHeadersVisible = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ViewGrid.DefaultCellStyle = dataGridViewCellStyle1;
            this.ViewGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ViewGrid.GridColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ViewGrid.Location = new System.Drawing.Point(0, 119);
            this.ViewGrid.Name = "ViewGrid";
            this.ViewGrid.RowHeadersWidth = 40;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            this.ViewGrid.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.ViewGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.ViewGrid.Size = new System.Drawing.Size(587, 332);
            this.ViewGrid.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.EndATextBox);
            this.groupBox1.Controls.Add(this.StartATextBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.AbsoluteAddrRadioBtn);
            this.groupBox1.Controls.Add(this.RelativeAddrRadioBtn);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(587, 119);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Memory View";
            // 
            // EndATextBox
            // 
            this.EndATextBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.EndATextBox.Location = new System.Drawing.Point(303, 51);
            this.EndATextBox.Name = "EndATextBox";
            this.EndATextBox.Size = new System.Drawing.Size(100, 22);
            this.EndATextBox.TabIndex = 9;
            this.EndATextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // StartATextBox
            // 
            this.StartATextBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.StartATextBox.Location = new System.Drawing.Point(179, 51);
            this.StartATextBox.Name = "StartATextBox";
            this.StartATextBox.Size = new System.Drawing.Size(100, 22);
            this.StartATextBox.TabIndex = 8;
            this.StartATextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label2.Location = new System.Drawing.Point(244, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 16);
            this.label2.TabIndex = 7;
            this.label2.Text = "Address range";
            // 
            // AbsoluteAddrRadioBtn
            // 
            this.AbsoluteAddrRadioBtn.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.AbsoluteAddrRadioBtn.AutoSize = true;
            this.AbsoluteAddrRadioBtn.Location = new System.Drawing.Point(303, 79);
            this.AbsoluteAddrRadioBtn.Name = "AbsoluteAddrRadioBtn";
            this.AbsoluteAddrRadioBtn.Size = new System.Drawing.Size(78, 20);
            this.AbsoluteAddrRadioBtn.TabIndex = 6;
            this.AbsoluteAddrRadioBtn.Text = "Absolute";
            this.AbsoluteAddrRadioBtn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.AbsoluteAddrRadioBtn.UseVisualStyleBackColor = true;
            // 
            // RelativeAddrRadioBtn
            // 
            this.RelativeAddrRadioBtn.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.RelativeAddrRadioBtn.AutoSize = true;
            this.RelativeAddrRadioBtn.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.RelativeAddrRadioBtn.Checked = true;
            this.RelativeAddrRadioBtn.Location = new System.Drawing.Point(204, 79);
            this.RelativeAddrRadioBtn.Name = "RelativeAddrRadioBtn";
            this.RelativeAddrRadioBtn.Size = new System.Drawing.Size(75, 20);
            this.RelativeAddrRadioBtn.TabIndex = 5;
            this.RelativeAddrRadioBtn.TabStop = true;
            this.RelativeAddrRadioBtn.Text = "Relative";
            this.RelativeAddrRadioBtn.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.Location = new System.Drawing.Point(285, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(11, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = ":";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel_SelectedValue,
            this.toolStripStatusLabel2,
            this.StatusLabel_MemoryDetails});
            this.statusStrip1.Location = new System.Drawing.Point(0, 451);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(587, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // StatusLabel_SelectedValue
            // 
            this.StatusLabel_SelectedValue.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.StatusLabel_SelectedValue.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.StatusLabel_SelectedValue.Name = "StatusLabel_SelectedValue";
            this.StatusLabel_SelectedValue.Size = new System.Drawing.Size(187, 17);
            this.StatusLabel_SelectedValue.Text = "Address: 00000000 Value: 0x00 (00)";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(185, 17);
            this.toolStripStatusLabel2.Spring = true;
            this.toolStripStatusLabel2.Text = "toolStripStatusLabel2";
            // 
            // StatusLabel_MemoryDetails
            // 
            this.StatusLabel_MemoryDetails.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.StatusLabel_MemoryDetails.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.StatusLabel_MemoryDetails.Name = "StatusLabel_MemoryDetails";
            this.StatusLabel_MemoryDetails.Size = new System.Drawing.Size(200, 17);
            this.StatusLabel_MemoryDetails.Text = "0x00 bytes of Memory at 0x00000000";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.RefreshToolStripMenuItem,
            this.displayModeToolStripMenuItem,
            this.endiannessToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(181, 92);
            // 
            // RefreshToolStripMenuItem
            // 
            this.RefreshToolStripMenuItem.Name = "RefreshToolStripMenuItem";
            this.RefreshToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.RefreshToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.RefreshToolStripMenuItem.Text = "Refresh";
            this.RefreshToolStripMenuItem.ToolTipText = "Refreshes current view on memory";
            // 
            // displayModeToolStripMenuItem
            // 
            this.displayModeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hexToolStripMenuItem,
            this.signedIntegerToolStripMenuItem,
            this.floatToolStripMenuItem,
            this.aSCIIToolStripMenuItem,
            this.instructionToolStripMenuItem});
            this.displayModeToolStripMenuItem.Name = "displayModeToolStripMenuItem";
            this.displayModeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.displayModeToolStripMenuItem.Text = "Display mode";
            // 
            // hexToolStripMenuItem
            // 
            this.hexToolStripMenuItem.Checked = true;
            this.hexToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.hexToolStripMenuItem.Name = "hexToolStripMenuItem";
            this.hexToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.hexToolStripMenuItem.Text = "Hex";
            // 
            // signedIntegerToolStripMenuItem
            // 
            this.signedIntegerToolStripMenuItem.Name = "signedIntegerToolStripMenuItem";
            this.signedIntegerToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.signedIntegerToolStripMenuItem.Text = "Signed Integer";
            // 
            // aSCIIToolStripMenuItem
            // 
            this.aSCIIToolStripMenuItem.Name = "aSCIIToolStripMenuItem";
            this.aSCIIToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.aSCIIToolStripMenuItem.Text = "ASCII";
            // 
            // instructionToolStripMenuItem
            // 
            this.instructionToolStripMenuItem.Name = "instructionToolStripMenuItem";
            this.instructionToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.instructionToolStripMenuItem.Text = "Instruction";
            // 
            // endiannessToolStripMenuItem
            // 
            this.endiannessToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.littleToolStripMenuItem,
            this.bigToolStripMenuItem});
            this.endiannessToolStripMenuItem.Enabled = false;
            this.endiannessToolStripMenuItem.Name = "endiannessToolStripMenuItem";
            this.endiannessToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.endiannessToolStripMenuItem.Text = "Endianness";
            // 
            // littleToolStripMenuItem
            // 
            this.littleToolStripMenuItem.CheckOnClick = true;
            this.littleToolStripMenuItem.Name = "littleToolStripMenuItem";
            this.littleToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.littleToolStripMenuItem.Text = "Little";
            // 
            // bigToolStripMenuItem
            // 
            this.bigToolStripMenuItem.Checked = true;
            this.bigToolStripMenuItem.CheckOnClick = true;
            this.bigToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.bigToolStripMenuItem.Name = "bigToolStripMenuItem";
            this.bigToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.bigToolStripMenuItem.Text = "Big";
            // 
            // floatToolStripMenuItem
            // 
            this.floatToolStripMenuItem.Name = "floatToolStripMenuItem";
            this.floatToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.floatToolStripMenuItem.Text = "Float (IEEE754)";
            // 
            // MemoryViewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ViewGrid);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox1);
            this.Name = "MemoryViewControl";
            this.Size = new System.Drawing.Size(587, 473);
            ((System.ComponentModel.ISupportInitialize)(this.ViewGrid)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView ViewGrid;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton AbsoluteAddrRadioBtn;
        private System.Windows.Forms.RadioButton RelativeAddrRadioBtn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel_SelectedValue;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel_MemoryDetails;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem RefreshToolStripMenuItem;
        private System.Windows.Forms.TextBox EndATextBox;
        private System.Windows.Forms.TextBox StartATextBox;
        private System.Windows.Forms.ToolStripMenuItem displayModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem signedIntegerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aSCIIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem endiannessToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem littleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bigToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem instructionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem floatToolStripMenuItem;
    }
}
