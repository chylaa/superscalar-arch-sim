using superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Units;
using superscalar_arch_sim_gui.Utilis;
using System;
using System.Windows.Forms;

namespace superscalar_arch_sim_gui.UserControls.Core.Dynamic
{
    
    public partial class ReservationStationsView : CustomControls.ReservationStationsViewNonGenericParent
    {   
        private readonly ToolStripMenuItem showFetchAddressToolStripMenuItem;
        private readonly ToolStripMenuItem showNextAddressToolStripMenuItem;
        public string LabelNameText { get => NameLabel.Text; set => NameLabel.Text = value; }
        public ReservationStationsView() : base()
        {
            InitializeComponent();
            InitializeBaseDataGrid(RSDataGridView);

            showFetchAddressToolStripMenuItem = new ToolStripMenuItem("Show Fetch Address") 
            {
                CheckOnClick = true,
                Checked = true,
            };
            showNextAddressToolStripMenuItem = new ToolStripMenuItem("Show Next Address")
            {
                CheckOnClick = true,
                Checked = true,
            };
            showFetchAddressToolStripMenuItem.CheckedChanged += AdjustVisibleColumns;
            showNextAddressToolStripMenuItem.CheckedChanged += AdjustVisibleColumns;
            ContextMenuStrip.Items.Add(showFetchAddressToolStripMenuItem);
            ContextMenuStrip.Items.Add(showNextAddressToolStripMenuItem);

            StylingColumns = new string[] { nameof(ReservationStation.OpVal1), nameof(ReservationStation.OpVal2), nameof(ReservationStation.A) };
        }

        protected override void AdjustVisibleColumns(object sender, EventArgs e)
        {
            if (sender == showFetchAddressToolStripMenuItem) 
            {
                bool visible = showFetchAddressToolStripMenuItem.Checked;
                GUIUtilis.SetDataGridColumnsVisible(RSDataGridView, visible, nameof(ReservationStation.FetchLocalPC));
                GUIUtilis.SetDataGridColumnIndex(RSDataGridView, index: -1, nameof(ReservationStation.FetchLocalPC));
                UpdateBindings();
            } 
            else if (sender == showNextAddressToolStripMenuItem)
            {
                bool visible = showNextAddressToolStripMenuItem.Checked;
                GUIUtilis.SetDataGridColumnsVisible(RSDataGridView, visible, nameof(ReservationStation.NextLocalPC));
                GUIUtilis.SetDataGridColumnIndex(RSDataGridView, index: -1, nameof(ReservationStation.NextLocalPC));
                UpdateBindings();
            }
            else
            {
                base.AdjustVisibleColumns(sender, e);
            }
        }

        public override void BindEntryCollection(ReservationStationCollection collection)
        {
            base.BindEntryCollection(collection);
            AdjustVisibleColumns(showFetchAddressToolStripMenuItem, EventArgs.Empty);
            AdjustVisibleColumns(showNextAddressToolStripMenuItem, EventArgs.Empty);
        }

    }
}
