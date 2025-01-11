using superscalar_arch_sim.RV32.Hardware.Units;
using superscalar_arch_sim.Simulis.Reports;
using superscalar_arch_sim_gui.Utilis;
using System;
using System.Windows.Forms;

namespace superscalar_arch_sim_gui.UserControls.Units
{
    public partial class BranchPredictorView : UserControl, IBindUpdate
    {
        private BranchPredictor Predictor { get; set; }
        private SimReporter SimReport { get; set; }
        public Forms.BranchPredictorDetailsView BranchPredictorDetailsForm { get; set; }

        private readonly TextBox[] BindedTextBoxes;

        public BranchPredictorView()
        {
            InitializeComponent();
            branchPredictorDetailsButton.Click += BranchPredictorDetailsButton_Click;
            BindedTextBoxes = GUIUtilis.RecursivelyGetAllChildrenOfType<TextBox>(this).ToArray();
        }

        public void InitBranchPredictorBindings(BranchPredictor predictor, SimReporter reporter)
        {
            SimReport = reporter;
            Predictor = predictor;

            GUIUtilis.ClearBindings(
                branchPredictorGroupBox,
                BPSchemeTextBox,
                BPPredictionTextBox,
                BConditionTextBox,
                BTargetAddrTextBox
            );

            branchPredictorGroupBox.DataBindings.Add(new Binding(nameof(branchPredictorGroupBox.Enabled), predictor, nameof(predictor.Enabled)));
            BPSchemeTextBox.DataBindings.Add(new Binding(nameof(BPSchemeTextBox.Text), predictor, nameof(predictor.UsedPredictionScheme)));
            BPPredictionTextBox.DataBindings.Add(new Binding(nameof(BPPredictionTextBox.Text), predictor, nameof(predictor.CurrentPrediction)));
            BConditionTextBox.DataBindings.Add(new Binding(nameof(BConditionTextBox.Text), predictor, nameof(predictor.EvalCondition)));
            BTargetAddrTextBox.DataBindings.Add(new Binding(nameof(BTargetAddrTextBox.Text), predictor, nameof(predictor.TargetAddress)));
        }

        public void UpdateBindings()
        {
            if (Predictor != null)
            {
                GUIUtilis.ReadBinding(branchPredictorGroupBox);
                GUIUtilis.ReadBinding(BindedTextBoxes);
            }
            if (BranchPredictorDetailsForm != null && false == BranchPredictorDetailsForm.IsDisposed)
            {
                BranchPredictorDetailsForm.UpdateBindings();
            }
        }

        public bool CloseDetailedViewIfShown()
        {
            if (BranchPredictorDetailsForm != null) {
                if (false == BranchPredictorDetailsForm.IsDisposed) {
                    if (false == BranchPredictorDetailsForm.Disposing) {
                        BranchPredictorDetailsForm.Close();
                        return true;
                    }
                }
            }
            return false;
        }

        private void BranchPredictorDetailsButton_Click(object sender, EventArgs e)
        {
            if (BranchPredictorDetailsForm == null || BranchPredictorDetailsForm.IsDisposed || BranchPredictorDetailsForm.Disposing)
                (BranchPredictorDetailsForm = new Forms.BranchPredictorDetailsView(Predictor, SimReport)).Show();
            else
                BranchPredictorDetailsForm.BringToFront();
        }
    }
}
