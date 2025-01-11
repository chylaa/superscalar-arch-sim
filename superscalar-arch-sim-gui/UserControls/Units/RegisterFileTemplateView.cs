using superscalar_arch_sim.RV32.Hardware.Register;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace superscalar_arch_sim_gui.UserControls.Units
{
    public partial class RegisterFileTemplateView : UserControl
    {
        private Register32File RegisterFile;
        readonly Dictionary<ToolStripMenuItem, Utilis.StrConverter.StringStyle> DisplayStyleItems;

        private Utilis.StrConverter.StringStyle _valueFormat = Utilis.StrConverter.StringStyle.SignedInt;

        public void RefreshAll() => PopulateListViews();

        public RegisterFileTemplateView()
        {
            InitializeComponent();

            DisplayStyleItems = new Dictionary<ToolStripMenuItem, Utilis.StrConverter.StringStyle>()
            {
                { signedIntegerToolStripMenuItem, Utilis.StrConverter.StringStyle.SignedInt },
                { singleFloatingPointToolStripMenuItem, Utilis.StrConverter.StringStyle.Float },
                { aSCIIToolStripMenuItem, Utilis.StrConverter.StringStyle.ASCII },
            };
            RegDetailsListView.ContextMenuStrip = contextMenuStrip1;
            
            InitHandlers();
        }
        public void InitView(Register32File regfile, bool showTagList)
        {
            RegisterFile = regfile;
            InitializeListViews(showTagList);
            PopulateListViews();
        }

        private void InitHandlers() 
        {
            Array.ForEach(DisplayStyleItems.Keys.ToArray(), item => item.Click += OnValueFormatSelect_Click);
            RegDetailsListView.AfterLabelEdit += RegDetailsListView_AfterLabelEdit;
            RegDetailsListView.MouseDoubleClick += RegDetailsListView_MouseDoubleClick; ;
            refreshToolStripMenuItem.Click += delegate { RefreshAll(); };
        }
        private void InitListViewProperties(ListView lv)
        {
            lv.Clear();
            lv.View = View.Details;
            lv.FullRowSelect = true;
        }
        private void InitializeListViews(bool showTagList)
        {
            ResStationTagListView.Scrollable = false;
            InitListViewProperties(ResStationTagListView);
            ResStationTagListView.Columns.Add("RS Tag", -2, HorizontalAlignment.Center);
            if(false == (ResStationTagListView.Visible = showTagList)) 
            {
                ResStationTagListView.Width = 0;
            }

            InitListViewProperties(RegDetailsListView);

            RegDetailsListView.Columns.Add("Content (Hex)", 100, HorizontalAlignment.Center);
            RegDetailsListView.Columns.Add($"Value ({_valueFormat})", 100, HorizontalAlignment.Center);
            RegDetailsListView.Columns.Add("Register", 80, HorizontalAlignment.Center);
            RegDetailsListView.Columns.Add("ABI Name", 80, HorizontalAlignment.Center);
            RegDetailsListView.Columns.Add("Description", -2, HorizontalAlignment.Center);
            RegDetailsListView.AllowColumnReorder = false;
        }
        private void OnValueFormatSelect_Click(object sender, EventArgs e)
        {
            var _checked = DisplayStyleItems.Keys.SingleOrDefault(item => item.Checked);
            if (_checked == default)
            {
                _checked = DisplayStyleItems.First().Key;
                _valueFormat = DisplayStyleItems[_checked];
                _checked.Checked = true;
            } else if (sender is ToolStripMenuItem && _checked != sender)
            {
                Array.ForEach(DisplayStyleItems.Keys.ToArray(), item => item.Checked = (item == sender));
                _valueFormat = DisplayStyleItems[sender as ToolStripMenuItem];
                RegDetailsListView.Columns[1].Text = $"Value ({_valueFormat})";
                RefreshAll();
            }
        }
        private void PopulateListViews()
        {
            void BeginUpdateAndClear(ListView v) { v.BeginUpdate(); v.Items.Clear();  }
            
            BeginUpdateAndClear(ResStationTagListView);
            BeginUpdateAndClear(RegDetailsListView);

            for (int i = 0; i < RegisterFile.Count; i++)
            {
                Register32 register = RegisterFile.GetRegister(i);

                int tag = RegisterFile.GetTagFromRegisterStatus(i);
                ListViewItem tagitem = new ListViewItem(tag == 0 ? string.Empty : tag.ToString());
                ListViewItem regitem = new ListViewItem(new string[]
                {
                    register.ReadUnsigned().ToString("X8"),
                    Utilis.StrConverter.FormatValue(_valueFormat, register.ReadUnsigned()),
                    register.Name,
                    register.ABIMnemonic,
                    register.Meaning
                });
                regitem.SubItems[2].BackColor = SystemColors.ActiveCaption; // item.UseItemStyleForSubItems should be 'false'
                ResStationTagListView.Items.Add(tagitem);
                RegDetailsListView.Items.Add(regitem);
            }
            ResStationTagListView.EndUpdate();
            RegDetailsListView.EndUpdate();
        }

        private void RegDetailsListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {   // xD
            (RegDetailsListView.GetItemAt(e.X, e.Y)
            ?? (RegDetailsListView.SelectedItems.Count > 0
            ? RegDetailsListView.SelectedItems[0] : null)
            )?.BeginEdit();
        }

        private void RegDetailsListView_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (long.TryParse(e.Label, System.Globalization.NumberStyles.HexNumber, null, out long newValue))
            {
                RegisterFile[(uint)e.Item] = unchecked((uint)(newValue & 0xFF_FF_FF_FF));
                RegDetailsListView.Items[e.Item].Text = newValue.ToString("X8");
                RegDetailsListView.Items[e.Item].SubItems[0].Text = newValue.ToString();
            } else
            {
                e.CancelEdit = true;
            }
        }



    }
}
