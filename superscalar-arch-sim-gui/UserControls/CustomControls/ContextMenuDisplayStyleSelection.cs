using superscalar_arch_sim_gui.Utilis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace superscalar_arch_sim_gui.UserControls.CustomControls
{
    public partial class ContextMenuDisplayStyleSelection : ContextMenuStrip
    {
        private readonly Dictionary<ToolStripMenuItem, StrConverter.StringStyle> DisplayStyleItems;
        private EventHandler _checkedChanged { get; set; }
        private StrConverter.StringStyle _valueFormat { get; set; } = StrConverter.StringStyle.Hex;

        private bool _unchecking = false;
        
        private readonly ToolStripMenuItem ValueFormatToolStripMenuItem = new ToolStripMenuItem("Value Format");
        private readonly ToolStripMenuItem HexadecimalToolStripMenuItem = new ToolStripMenuItem("Hexadecimal");
        private readonly ToolStripMenuItem SignedIntegerToolStripMenuItem = new ToolStripMenuItem("Signed Integer");
        private readonly ToolStripMenuItem SingleFloatingPointToolStripMenuItem = new ToolStripMenuItem("Single Floating Point");
        private readonly ToolStripMenuItem ASCIIToolStripMenuItem = new ToolStripMenuItem("ASCII");

        public StrConverter.StringStyle ValueFormat {
            get => _valueFormat; 
            set => DisplayStyleItems.SingleOrDefault(x => x.Value == value).Key?.PerformClick();
        }

        public event EventHandler OnCheckedStyleChanged { add => _checkedChanged += value; remove => _checkedChanged -= value; }

        public ContextMenuDisplayStyleSelection()
        {
            InitializeComponent();

            DisplayStyleItems = new Dictionary<ToolStripMenuItem, StrConverter.StringStyle>()
            {
                { HexadecimalToolStripMenuItem, StrConverter.StringStyle.Hex },
                { SignedIntegerToolStripMenuItem, StrConverter.StringStyle.SignedInt },
                { SingleFloatingPointToolStripMenuItem, StrConverter.StringStyle.Float },
                { ASCIIToolStripMenuItem, StrConverter.StringStyle.ASCII },
            };

            ValueFormatToolStripMenuItem.DropDownItems.AddRange(DisplayStyleItems.Keys.ToArray());
            Items.Add(ValueFormatToolStripMenuItem);

            Array.ForEach( DisplayStyleItems.Keys.ToArray(), 
                item => { item.Click += OnValueFormatSelect_Click; item.CheckOnClick = true; item.Checked = false; } 
            );
            ValueFormat = StrConverter.StringStyle.Hex;
        }
        
        private void OnValueFormatSelect_Click(object sender, EventArgs e)
        {
            if (false == _unchecking)
            {
                _unchecking = true;
                Array.ForEach(DisplayStyleItems.Keys.ToArray(), item => item.Checked = (item == sender));
                _valueFormat = DisplayStyleItems[sender as ToolStripMenuItem];
                _checkedChanged?.Invoke(sender, e);
                _unchecking = false;
            }
        }
    }
}
