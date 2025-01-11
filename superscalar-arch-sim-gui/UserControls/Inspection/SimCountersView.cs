using superscalar_arch_sim.RV32.Hardware.CPU;
using superscalar_arch_sim.Simulis.Reports;
using superscalar_arch_sim_gui.Utilis;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace superscalar_arch_sim_gui.UserControls.Inspection
{
    public partial class SimCountersView : UserControl, IBindUpdate
    {
        private SimReporter SimReport { get; set; }

        private readonly Control[] NameLabels;
        private readonly Control[] Counters;
        
        public SimCountersView()
        {
            InitializeComponent();
            var countersY = CyclesLabelCnt.Location.Y - 1;
            var countersBounds = new Rectangle(new Point(0,countersY), new Size(Width, Height - countersY));
            Control[] AllLabels = new Control[CountersGroupBox.Controls.Count];
            CountersGroupBox.Controls.CopyTo(AllLabels, 0);
            Counters = AllLabels.Where(l => countersBounds.IntersectsWith(l.Bounds)).OrderBy(l => l.Location.X).ToArray();
            NameLabels = AllLabels.Except(Counters).OrderBy(l => l.Location.X).ToArray();
        }

        public void InitBindings(ICPU core)
        {
            SimReport = core.SimReport;
            GUIUtilis.ClearBindings(Counters);

            CyclesLabelCnt.DataBindings.Add(new Binding(nameof(TextBox.Text), SimReport, nameof(SimReporter.ClockCycles)));
            CPILabelCnt.DataBindings.Add(new Binding(nameof(TextBox.Text), SimReport, nameof(SimReporter.CPI)));
            IPCLabelCnt.DataBindings.Add(new Binding(nameof(TextBox.Text), SimReport, nameof(SimReporter.IPC)));
            CommitedLabelCnt.DataBindings.Add(new Binding(nameof(TextBox.Text), SimReport, nameof(SimReporter.CommitedInstructions)));
            DataFWLabelCnt.DataBindings.Add(new Binding(nameof(TextBox.Text), SimReport, nameof(SimReporter.Forwardings)));
            StallsLabelCnt.DataBindings.Add(new Binding(nameof(TextBox.Text), SimReport, nameof(SimReporter.Stalls)));
            BranchAccuracyCnt.DataBindings.Add(new Binding(nameof(TextBox.Text), SimReport, nameof(SimReporter.BranchPredictionAccuracy)));
            
            CPILabelCnt.DataBindings[0].FormattingEnabled = true;
            CPILabelCnt.DataBindings[0].FormatString = "0.###";
            IPCLabelCnt.DataBindings[0].FormattingEnabled = true;
            IPCLabelCnt.DataBindings[0].FormatString = "0.###";
            BranchAccuracyCnt.DataBindings[0].FormattingEnabled = true;
            BranchAccuracyCnt.DataBindings[0].FormatString = "0.###";

            if (typeof(SuperscalarCPU).IsInstanceOfType(core))
            {
                Custom1LabelCnt.DataBindings.Add(new Binding(nameof(TextBox.Text), SimReport, nameof(SimReporter.StoreLoadBypasses)));
                NameLabels[System.Array.IndexOf(Counters, Custom1LabelCnt)].Text = "Store Bypasses";
            }
            else if (typeof(ScalarCPU).IsInstanceOfType(core))
            {
                Custom1LabelCnt.DataBindings.Add(new Binding(nameof(TextBox.Text), SimReport, nameof(SimReporter.LoadInterlocks)));
                NameLabels[System.Array.IndexOf(Counters, Custom1LabelCnt)].Text = "Load Interlocks";
            }                                                                      
        }

        private void UpdateDictionaryComboBox<TKey>(ComboBox destination, IReadOnlyDictionary<TKey, ulong> sourceSnapshot) 
            where TKey : System.Enum
        {
            //int maxDigits = (int)(System.Math.Log10(1 + sourceSnapshot.Values.Max()) + 1);
            int maxKeychars = sourceSnapshot.Keys.Max(key => key.ToString().Length);

            string FormatEnum(TKey key) => key.ToString().Replace('_', ' ').PadRight(maxKeychars);
            string FormatValue(ulong val) => val.ToString();
            int idx = destination.SelectedIndex;
            destination.Items.Clear();
            foreach (var kvp in sourceSnapshot)
                destination.Items.Add($"{FormatEnum(kvp.Key)} : {FormatValue(kvp.Value)}");
            destination.SelectedIndex = idx;
        }
        public void UpdateBindings()
        {
            GUIUtilis.ReadBinding(Counters);
            UpdateDictionaryComboBox(ITypesCntComboBox, SimReport.CommitedInstructionTypesThreadSafe);
            UpdateDictionaryComboBox(IOpcodesCntComboBox, SimReport.CommitedInstructionOpcodesThreadSafe);
        }
    }
}
