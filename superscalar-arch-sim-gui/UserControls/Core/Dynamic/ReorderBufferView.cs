using superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Units;
using System.Drawing;

namespace superscalar_arch_sim_gui.UserControls.Core.Dynamic
{
    public partial class ReorderBufferView : CustomControls.ReorderBufferViewNonGenericParent 
    {

        public string LabelNameText { get => NameLabel.Text; set => NameLabel.Text = value; }
        public Color HeadEntryBackcolor { get => SpecialRowColor; set => SpecialRowColor = value; } 

        public ReorderBufferView() : base()
        {
            InitializeComponent();
            InitializeBaseDataGrid(ROBDataGridView);

            HeadEntryBackcolor = Color.LightBlue;
            StylingColumns = new string[] { nameof(ROBEntry.Value) };
        }

        private void MarkReorderBufferHead()
        {   // Color set in StandardEntryCollectionView.DataGridView_CellFormatting
            SpecialColorRowIndex = (BindedCollection.HeadEntry?.Tag ?? 0) - ReorderBuffer.TAGIDX_OFFSET;
        }
        public override void UpdateBindings()
        {
            base.UpdateBindings();
            MarkReorderBufferHead();
        }
    }
}
