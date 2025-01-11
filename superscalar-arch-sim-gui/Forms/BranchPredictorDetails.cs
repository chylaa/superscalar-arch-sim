using superscalar_arch_sim.RV32.Hardware.Units;
using superscalar_arch_sim.Simulis.Reports;
using superscalar_arch_sim_gui.UserControls;
using superscalar_arch_sim_gui.Utilis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using static superscalar_arch_sim.RV32.Hardware.Units.BranchPredictor;

namespace superscalar_arch_sim_gui.Forms
{
    public partial class BranchPredictorDetailsView : Form, IBindUpdate
    {
        private readonly BranchPredictor Predictor;
        private readonly SimReporter Reporter;

        private static readonly System.Drawing.Color CustomSelectedBackColor = System.Drawing.SystemColors.Highlight;
        private ListViewItem _CustomSelectedItem = null;

        private readonly TextBox[] BranchDetailsStatsTextBoxes;
     
        public BranchPredictorDetailsView(BranchPredictor branchPredictor, SimReporter reporter)
        {
            InitializeComponent();
            Load += BranchPredictorDetails_Load;
            Predictor = branchPredictor;
            Reporter = reporter; 
            BHTgroupBox.Width = ((Width / 2) - 10);
            BHTListView.SelectedIndexChanged += delegate { UpdateBindings(); };
            BHTListView.MouseClick += BHTListView_MouseClick;
            BHTListView.HideSelection = false;

            var bDetailsTB = BranchPredictorDetailsPanel.Controls.OfType<TextBox>();
            var bStatistTB = BranchStatCountersPanel.Controls.OfType<TextBox>();
            BranchDetailsStatsTextBoxes = bDetailsTB.Concat(bStatistTB).ToArray();
        }

        private void BHTListView_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                ListViewItem clicked = BHTListView.GetItemAt(e.X, e.Y);
                if (false == string.IsNullOrEmpty(clicked?.Text))
                {
                    foreach (var item in BHTListView.Items)
                        (item as ListViewItem).BackColor = BHTListView.BackColor;

                    clicked.BackColor = CustomSelectedBackColor;
                    _CustomSelectedItem = clicked;

                    UpdatePatternTableView(clicked);
                }
            } catch { MessageBox.Show($"Cannot select item at ({e.X};{e.Y})", "ERROR"); }
        }

        private void BindData()
        {
            BPSchemeTextBox.DataBindings.Add(new Binding(nameof(BPSchemeTextBox.Text), Predictor, nameof(Predictor.UsedPredictionScheme)));
            BPPredictionTextBox.DataBindings.Add(new Binding(nameof(BPPredictionTextBox.Text), Predictor, nameof(Predictor.CurrentPrediction)));
            BConditionTextBox.DataBindings.Add(new Binding(nameof(BConditionTextBox.Text), Predictor, nameof(Predictor.EvalCondition)));
            BTargetAddrTextBox.DataBindings.Add(new Binding(nameof(BTargetAddrTextBox.Text), Predictor, nameof(Predictor.TargetAddress)));

            BTakenTextBoxCnt.DataBindings.Add(new Binding(nameof(BTakenTextBoxCnt.Text), Reporter, nameof(Reporter.BranchesTaken)));
            BNotTakenTextBoxCnt.DataBindings.Add(new Binding(nameof(BNotTakenTextBoxCnt.Text), Reporter, nameof(Reporter.BranchesNotTaken)));
            BPredictedTextBoxCnt.DataBindings.Add(new Binding(nameof(BPredictedTextBoxCnt.Text), Reporter, nameof(Reporter.CorrectBranchPredictions)));
            BMispredictedTextBoxCnt.DataBindings.Add(new Binding(nameof(BMispredictedTextBoxCnt.Text), Reporter, nameof(Reporter.BranchMispredictions)));
        }

        private void InitListView(ListView lv, string keyColumnHeader, string valueColumnHeader)
        {
            lv.Alignment = ListViewAlignment.Top;
            if (lv.Enabled)
            {
                int width = valueColumnHeader is null ? lv.Width : ((lv.Width / 2) - 15);
                lv.Columns.Add(keyColumnHeader, width, HorizontalAlignment.Center);
                if (valueColumnHeader != null) {
                    lv.Columns.Add(valueColumnHeader, -2, HorizontalAlignment.Center);
                }
                lv.AllowColumnReorder = false;
                lv.MultiSelect = false;
            }
        }

        private void BranchPredictorDetails_Load(object sender, EventArgs e)
        {
            BHTgroupBox.Enabled = Predictor.UsedPredictionScheme != (PredictionScheme.Static);
            if(PHTgroupBox.Enabled = Predictor.UsedPredictionScheme == (PredictionScheme.AdaptivePredictor))
            {
                BHTgroupBox.Width = (Width / 3) + 10;
                InitListView(BHTListView, "Branch Address", null);
            } 
            else
            {
                BHTgroupBox.Width = ((Width / 2) - 10);
                InitListView(BHTListView, "Branch Address", "Finite-State-Machine");
            }

            InitListView(PHTListView, "Branch Pattern", "Finite-State-Machine");
            
            BindData();
            UpdateBindings();
        }
        private void FormatAndUpdateBranchPattern()
        {
            string sequence = Convert.ToString(Predictor.GlobalBranchPatternSequence, 2).PadLeft(32, '0');

            for (int i = sequence.Length - 8; i > 0; i -= 8) {
                sequence = sequence.Insert(i, "-");
            }

            int usedBits = (int)Predictor.NumberOfHistoryBitsInPatternHistoryTable;
            if (usedBits > 0)
            {
                int offset = (usedBits / 8) - ((usedBits % 8 == 0) ? 1 : 0);
                int startBracketPosition = sequence.Length - usedBits - offset;  // Adjust for the number of inserted '-'
                sequence += ']';
                sequence = sequence.Insert(startBracketPosition, "[");
            }
            BranchPatternLabel.Text = sequence;
        }

        private bool TryGetSelectedOrFirst(ListView listView, out ListViewItem selected)
        {

            if (_CustomSelectedItem != null)
            {
                selected = _CustomSelectedItem;
                return true;
            } 
            if (listView.Items.Count > 0)
            {
                selected = listView.Items[0];
                return true;
            }
            selected = null;
            return false;
        }

        private void UpdateBranchTableView(ListView tableView, IReadOnlyDictionary<uint, uint> sourceTable, int keyBase, int keyPad, bool showValue = true)
        {
            tableView.BeginUpdate();
            tableView.Items.Clear();
            foreach(var kvp in sourceTable)
            {
                string k = Convert.ToString(kvp.Key, keyBase).PadLeft(keyPad, '0');
                k = (keyBase == 2) ? ("0b" + k) : k.ToUpper();
                string v = "0b" + Convert.ToString(kvp.Value, 2).PadLeft((int)Predictor.NumberOfPredictionBitsForFSMs, '0');
                ListViewItem newitem  = new ListViewItem(k);
                if (showValue) { newitem.SubItems.Add(v); }
                tableView.Items.Add(newitem);

                if (_CustomSelectedItem != null && _CustomSelectedItem.Text == k)
                    newitem.BackColor = _CustomSelectedItem.BackColor;
                else if (tableView == BHTListView)
                    BHTListView_MouseClick(tableView, new MouseEventArgs(MouseButtons, 1, newitem.Bounds.Left, newitem.Bounds.Top, 0));
            }
            tableView.EndUpdate();
        }
        private void UpdateBranchAddressTableView()
        {
            int noHexDigits = (int)Math.Ceiling((double)Predictor.BitsOfEntriesInBranchHistoryTable / 4);
            bool showValue = (false == PHTListView.Enabled);
            UpdateBranchTableView(BHTListView, Predictor.GetReadonlyBranchAddressHistoryTable, keyBase: 16, keyPad: noHexDigits, showValue);
        }
        private void UpdatePatternTableView(ListViewItem addressItem = null)
        {
            if (Predictor.UsedPredictionScheme == PredictionScheme.AdaptivePredictor)
            {
                if (addressItem != null || TryGetSelectedOrFirst(PHTListView, out addressItem))
                {
                    uint address = uint.Parse(addressItem.Text, System.Globalization.NumberStyles.HexNumber);
                    var branchPatternHT = Predictor.GetReadonlyBranchPatternHistoryTable(address);
                    UpdateBranchTableView(PHTListView, branchPatternHT, keyBase: 2, keyPad: (int)Predictor.NumberOfHistoryBitsInPatternHistoryTable);
                }
            }
        }

        public void UpdateBindings()
        {
            GUIUtilis.ReadBinding(BranchDetailsStatsTextBoxes);

            UpdateBranchAddressTableView();
            UpdatePatternTableView();

            FormatAndUpdateBranchPattern();
        }
    }
}
