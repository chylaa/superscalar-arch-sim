using superscalar_arch_sim.RV32.Hardware.Memory;
using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace superscalar_arch_sim_gui.UserControls.Units
{
    public partial class MemoryViewControl : UserControl
    {

        readonly ToolStripMenuItem[] DisplayStyleItems;

        const int VALUES_PER_ROW = 8;
        const int MAX_ROWS = 256;

        private const string DEFAULT_ADDR_ST = "0x0000_0000";
        private const string DEFAULT_ADDR_END = "0x0000_0100";

        private string _lastStAddr = null;
        private string _lastEndAddr = null;

        private DataGridViewCell _lastHighlightedCell = null;

        private Memory Memory { get; set; }
        bool DisplayValsNeedReversing => (Memory.IsLittleEndiann != littleToolStripMenuItem.Checked);

        /// <summary>
        /// Handle to method allowing to retreive address to highlight. 
        /// For example, on ROM memory, can be used as PC pointer.
        /// If not set (is <see langword="null"/>), no cell will be highligted.
        /// </summary>
        public Func<uint> HighlightAddress { get; set; }

        public MemoryViewControl()
        {
            InitializeComponent();
            Load += MemoryView_Load;
            ViewGrid.CellEndEdit += ViewGrid_CellEndEdit;
            ViewGrid.CellClick += ViewGrid_CellClick;
            StartATextBox.KeyDown += RefreshOnEnter_KeyDown;
            EndATextBox.KeyDown += RefreshOnEnter_KeyDown;
            littleToolStripMenuItem.Click += EndiannessToolStripMenuItem_Click;
            bigToolStripMenuItem.Click += EndiannessToolStripMenuItem_Click;
            RefreshToolStripMenuItem.Click += delegate { RefreshMemoryView(force:true); };

            DisplayStyleItems = new ToolStripMenuItem[] { hexToolStripMenuItem, signedIntegerToolStripMenuItem, floatToolStripMenuItem, 
                                                          aSCIIToolStripMenuItem, instructionToolStripMenuItem };
            Array.ForEach(DisplayStyleItems, item => item.Click += DisplayModeToolStripMenuItem_Clicked);

        }
        /// <summary>Highlights address returned by <see cref="HighlightAddress"/> <see cref="Func{void, UInt32}"/> if set.</summary>
        public void RefreshHighlightAddressIfSet() { TryHighlightCellAtAddress(HighlightAddress?.Invoke()); ViewGrid.Refresh(); }
        
        public void BindMemoryObject(Memory memory)
        {
            Memory = memory;
            StatusLabel_MemoryDetails.Text = $"0x{memory.ByteSize:X2} bytes of {memory.Name} at 0x{memory.Origin:X8}";
            StatusLabel_SelectedValue.Text = CreateTextForSelectedValueStatusLabel(null, null);
            groupBox1.Text = memory.Name + " View";
        }
        private static string CreateTextForSelectedValueStatusLabel(UInt32? address, UInt32? value)
            => ("Address " + (address is null ? "--------" : $"{address:X8}") + " | " +
                "Value " + (value is null ? "----" : $"0x{value:X2} ({unchecked((Int32)value)})"));


        private uint ParseAddress(string s)
        {
            NumberStyles ns = s.StartsWith("0x") ? NumberStyles.HexNumber : NumberStyles.None;
            s = s.Replace(" ", "").Replace("0x", "").Replace("_", "");
            if (false == uint.TryParse(s, ns, null, out uint address)) 
                throw new Exception($"Cannot parse {s} address to unsigned integer number, enter valid decimal value or hexadecimal with '0x' prefix");
            
            address = superscalar_arch_sim.Utilis.Utilis.NearestAlligned(address, Allign.WORD);

            if (AbsoluteAddrRadioBtn.Checked) 
                address -= Memory.Origin; // set to relative addressing
            if (address > Memory.ByteSize) 
                throw new Exception($"Cannot display address at: {address} of {Memory.Name}. Byte size is set to {Memory.ByteSize}.");
            if (unchecked((int)(address)) < 0) 
                address = 0;

            return address;
        }

        /// <summary>
        /// TODO: Implement refreshing selected values (if none selected, refresh all).
        ///  </summary>
        public void RefreshMemoryView(bool force = false)
        {
            Exception parseEx = null;
            uint staddr = 0; uint endaddr = 0;
            int previouslyFirstDisplayedRowIdx = ViewGrid.FirstDisplayedScrollingRowIndex;

            try { staddr = ParseAddress(StartATextBox.Text); } 
            catch (Exception ex) { ex.Source = nameof(StartATextBox); parseEx = ex; } 
            
            try { endaddr = ParseAddress(EndATextBox.Text);  } 
            catch (Exception ex) { ex.Source = nameof(EndATextBox); parseEx = ex; } 
            
            if (parseEx is null)
            {
                if (force || StartATextBox.Text != _lastStAddr || EndATextBox.Text != _lastEndAddr)
                {
                    PopulateDataGridView(staddr, endaddr);
                    TryHighlightCellAtAddress(HighlightAddress?.Invoke());
                    _lastStAddr = StartATextBox.Text; _lastEndAddr = EndATextBox.Text;
                }
            }
            else
            {
                MessageBox.Show($"Address cannot be displayed: {parseEx.Message}", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (parseEx.Source == nameof(StartATextBox)) StartATextBox.Text = DEFAULT_ADDR_ST;
                else if (parseEx.Source == nameof(EndATextBox)) EndATextBox.Text = DEFAULT_ADDR_END; 
            }

            if (previouslyFirstDisplayedRowIdx > 0 && ViewGrid.Rows.Count > previouslyFirstDisplayedRowIdx) {
                ViewGrid.FirstDisplayedScrollingRowIndex = previouslyFirstDisplayedRowIdx;
            }
        }

        private void MemoryView_Load(object sender, EventArgs e)
        {
            groupBox1.ContextMenuStrip = contextMenuStrip1;
            ViewGrid.ContextMenuStrip = contextMenuStrip1;

            StartATextBox.Text = DEFAULT_ADDR_ST;
            EndATextBox.Text = DEFAULT_ADDR_END;
            SetupDataGridView(ViewGrid);
        }


        private void SetupDataGridView(DataGridView dataGridView)
        {
            dataGridView.VirtualMode = false;
            dataGridView.AutoGenerateColumns = false;
            dataGridView.Columns.Clear();
            dataGridView.ColumnHeadersVisible = true;
            dataGridView.RowHeadersVisible = true;
            dataGridView.ShowCellToolTips = true;

            for (int i = 0; i < VALUES_PER_ROW; i++)
            {
                int byteaddr = ((i * sizeof(UInt32)));
                var column = new DataGridViewTextBoxColumn
                {
                    Name = i.ToString(),
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                    ValueType = typeof(string),
                    HeaderText = $"{byteaddr:X2} : {byteaddr + 3:X2}",
                };
                column.HeaderCell.Style = new DataGridViewCellStyle(column.HeaderCell.Style) { Alignment = DataGridViewContentAlignment.MiddleCenter };
                dataGridView.Columns.Add(column);
            }
        }

        private Utilis.StrConverter.StringStyle GetSelectedDisplayStyle()
        {
            if (hexToolStripMenuItem.Checked)
                return Utilis.StrConverter.StringStyle.Hex;
            if (signedIntegerToolStripMenuItem.Checked)
                return Utilis.StrConverter.StringStyle.SignedInt;
            if (aSCIIToolStripMenuItem.Checked)
                return Utilis.StrConverter.StringStyle.ASCII;
            if (instructionToolStripMenuItem.Checked)
                return Utilis.StrConverter.StringStyle.Instruction;
            if (floatToolStripMenuItem.Checked)
                return Utilis.StrConverter.StringStyle.Float;
            return Utilis.StrConverter.StringStyle.None;
        }

        private void PopulateDataGridView(uint st, uint end)
        {
            end += (end - st) % VALUES_PER_ROW; // allign to 8 WORD's per line
            if (end <= st) return; // dont do anything on invalid range.

            ViewGrid.SuspendLayout();
            ViewGrid.Rows.Clear();
            for (uint iaddr = st; iaddr < end; iaddr += (VALUES_PER_ROW * sizeof(UInt32)))
            {
                DataGridViewRowHeaderCell header = new DataGridViewRowHeaderCell
                {
                    ValueType = typeof(string),
                    Value = iaddr.ToString("X8")
                };

                string[] items = new string[VALUES_PER_ROW];
                Utilis.StrConverter.StringStyle style = GetSelectedDisplayStyle();
                for (int i = 0; i < VALUES_PER_ROW; i++)                
                    items[i] = Utilis.StrConverter.FormatValue(style, Memory.ReadWord((uint)(iaddr + (i * sizeof(UInt32)))) );
                
                ViewGrid.Rows.Add(items);
                ViewGrid.Rows[ViewGrid.Rows.Count - 1].HeaderCell = header;
                ViewGrid.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
                
                if (ViewGrid.Rows.Count >= MAX_ROWS) {
                    MessageBox.Show($"Hit maximum number of rows: {MAX_ROWS}", "INFO: Cannot load more rows", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                }
            }
            ViewGrid.ResumeLayout();
        }

        private DataGridViewCell GetCellOfAddress(uint? address)
        {
            int cellCount = ViewGrid.GetCellCount(DataGridViewElementStates.Visible);
            if (address is null || cellCount <= address) 
                return null;

            address /= sizeof(UInt32);
            int row = (int)(address / VALUES_PER_ROW);
            int cell = (int)(address % VALUES_PER_ROW);
            return ViewGrid.Rows[row].Cells[cell];
        }

        private void TryHighlightCellAtAddress(uint? address)
        {
            _lastHighlightedCell?.Style.ApplyStyle(ViewGrid.DefaultCellStyle);
            if((_lastHighlightedCell = GetCellOfAddress(address)) != null)
                _lastHighlightedCell.Style.BackColor = Color.Aqua;
        }

        #region Event Handlers

        private void EndiannessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bigToolStripMenuItem.Checked = (sender == bigToolStripMenuItem);
            littleToolStripMenuItem.Checked = (sender == littleToolStripMenuItem);
            RefreshMemoryView(force: true);
        }

        private void DisplayModeToolStripMenuItem_Clicked(object sender, EventArgs e)
        {
            var _checked = DisplayStyleItems.SingleOrDefault(item => item.Checked);
            if (_checked == default)
            {
                DisplayStyleItems[0].Checked = true;
            } else if (_checked != sender)
            {
                Array.ForEach(DisplayStyleItems, item => item.Checked = (item == sender));
                _lastStAddr = null; // change one to ensure force refresh
                RefreshMemoryView(force: true);
            }
        }
        private void RefreshOnEnter_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox && e.KeyCode == Keys.Enter) 
                RefreshMemoryView(force: true);
        }
    
        private uint GetHeaderCellValue(int rowindex)
            => uint.Parse(ViewGrid.Rows[rowindex].HeaderCell.Value.ToString(), NumberStyles.HexNumber, null);

        private uint GetMemoryAddress(DataGridViewCellEventArgs e)
            => (uint)(GetHeaderCellValue(e.RowIndex) + (e.ColumnIndex * sizeof(UInt32)) );

        private void ViewGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var cell = ViewGrid[e.ColumnIndex, e.RowIndex];
            uint address = GetMemoryAddress(e);
            Utilis.StrConverter.StringStyle style = GetSelectedDisplayStyle();
            if (Utilis.StrConverter.TryParse(style, cell.Value.ToString(), out uint value))
            {
                Memory[address] = value;
                cell.Value = Utilis.StrConverter.FormatValue(style, value);
                cell.ErrorText = string.Empty;
                StatusLabel_SelectedValue.Text = CreateTextForSelectedValueStatusLabel(address, value);
            }
            else
            {
                string val = Utilis.StrConverter.FormatValue(style, Memory[address]);
                cell.ErrorText = "Invalid value, cannot write to memory. Previously: " + val;
            }
        }
        private void ViewGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
            UInt32? address = null; UInt32? value = null;
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                address = GetMemoryAddress(e);
                value = Memory[address.Value];
            }
            StatusLabel_SelectedValue.Text = CreateTextForSelectedValueStatusLabel(address, value);
        }


        #endregion

    }
}
