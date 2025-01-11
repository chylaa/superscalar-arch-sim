using superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.FuncUnit;
using superscalar_arch_sim_gui.Utilis;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace superscalar_arch_sim_gui.UserControls.Core.Dynamic
{
    public partial class ExecuteUnitSetView : UserControl, IBindUpdate
    {
        private readonly List<IBindableComponent> BindableComponents;
        private readonly List<ExecuteUnit> ExecuteUnits;

        public Font SubLabelsFont { get; set; }
        public string UnitName { get => UnitNameLabel.Text; set => UnitNameLabel.Text = value; }

        public ExecuteUnitSetView()
        {
            InitializeComponent();
            SubLabelsFont = new Font(Font.FontFamily, 8.25f, FontStyle.Regular);
            BindableComponents = new List<IBindableComponent>();
            ExecuteUnits = new List<ExecuteUnit>();
        }

        private void InitExecuteUnitSetView(Panel entrypanel)
        {
            ClearRemoveDispose<IBindableComponent>(BindableComponents);
            ClearRemoveDispose<Control>(entrypanel.Controls);
            
            int panelHeight = (entrypanel.Height / ExecuteUnits.Count);
            for (int i = 0; i < ExecuteUnits.Count; i++)
            {
                Panel panel = new Panel
                {
                    Width = entrypanel.Width,
                    Height = panelHeight,
                    Location = new Point(0, i * panelHeight),
                    BorderStyle = BorderStyle.FixedSingle
                };
                Label label = new Label()
                {
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill,
                    Font = SubLabelsFont,
                };
                label.DataBindings.Add(new Binding(nameof(Label.Text), ExecuteUnits[i], nameof(ExecuteUnit.ProcessedInstruction)));
                panel.Controls.Add(label);
                BindableComponents.Add(label);
                entrypanel.Controls.Add(panel);
            }
        }
        public void BindExecutionUnits(params ExecuteUnit[] units)
        {
            ExecuteUnits.Clear();
            ExecuteUnits.AddRange(units);
            InitExecuteUnitSetView(entrypanel:mainPanel);   
        }

        public void UpdateBindings()
        {
            GUIUtilis.ReadBinding(BindableComponents);
        }

        private static void ClearRemoveDispose<T>(System.Collections.IList values) where T : class, IDisposable, IBindableComponent
        {
            if (values is null)
                return;

            while (values.Count > 0)
            {
                int i = values.Count - 1;
                var component = (values[i] as T);
                component.DataBindings.Clear();
                values.Remove(component);
                component.Dispose();
            }
        }
    }
}
