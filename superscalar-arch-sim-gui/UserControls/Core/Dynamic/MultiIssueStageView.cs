using superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Stage;
using superscalar_arch_sim_gui.Utilis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace superscalar_arch_sim_gui.UserControls.Core.Dynamic
{
    public partial class MultiIssueStageView : UserControl, IBindUpdate
    {
        private readonly Color DefaultColor = SystemColors.Control;
        public TEMStage Stage { get; private set; }
        public string StageName { get => StageNameLabel.Text; set => StageNameLabel.Text = value; }
        
        [Description("Allows to specify which columns should be visible and in which order by providing their names " +
        "(set of strings divided by space, where each should be Pipeline.TEM.Units.PipeRegisters property name). " +
        "Empty string causes all columns to be displayed. ")]
        public string[] VisibleColumns { get; set; }
        [Description("Specifies strings that will be used to set diferent headers for columns as in {columnName;headerString}.")]
        public Dictionary<string, string> ColumnHeadersMap { get; }

        public MultiIssueStageView()
        {
            InitializeComponent();
            InitHandlers();
            StandardControls.StdDataGridView.InitStdDataGridView(StageGridView, AdjustVisibleColumns);
            DefaultColor = BackColor;
            ColumnHeadersMap = new Dictionary<string, string>();
        }

        private void InitHandlers()
        {
            StageGridView.CellFormatting += GUIUtilis.FormatRegister32OnDataGridViewCellFormatting;
            StallingCheckBox.CheckedChanged += delegate 
            {
                BackColor = StallingCheckBox.Checked 
                            ? SystemColors.Info 
                            : DefaultColor;
            };
        }

        private void AdjustVisibleColumns(object sender, EventArgs e)
        {
            string[] ordered = (VisibleColumns is null) ? new string[0] : VisibleColumns;
            List<string> toRemove = new List<string>();
            for (int i = 0; i < StageGridView.Columns.Count; i++) {
                string name = StageGridView.Columns[i].Name;
                string propName = StageGridView.Columns[i].DataPropertyName;
                if (false == (ordered.Contains(name) || ordered.Contains(propName))) {
                    toRemove.Add(name);
                }
            }
            GUIUtilis.RemoveColumnsFromDataGrid(StageGridView, toRemove.ToArray());
            GUIUtilis.OrderColumnsInDataGrid(StageGridView, throwOnMissing: true, ordered);
           
            //RenameColumnsHeadersBaseOnMap();
        }

        public void BindStageData(TEMStage stage)
        {
            Stage = stage;
            StageName = stage.Name;
            GUIUtilis.ClearBindings(StallingCheckBox);
            StallingCheckBox.DataBindings.Add(new Binding(nameof(CheckBox.Checked), Stage, nameof(TEMStage.Stalling)));

            Array.ForEach(VisibleColumns, name => ColumnHeadersMap[name] = name);
            StandardControls.StdDataGridView.BindToDataGrid(StageGridView, Stage, nameof(TEMStage.LatchDataBuffers), DataSourceUpdateMode.Never);
            UpdateBindings();
        }

        public void UpdateBindings()
        {
            GUIUtilis.ReadBinding(StallingCheckBox);
            StandardControls.StdDataGridView.UpdateBinding(StageGridView);
        }

    }
}
