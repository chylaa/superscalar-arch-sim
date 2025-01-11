using superscalar_arch_sim.RV32.Hardware.Pipeline.TYP.Stage;
using superscalar_arch_sim.RV32.Hardware.Register;
using superscalar_arch_sim_gui.Utilis;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace superscalar_arch_sim_gui.UserControls.Core.Static
{
    public partial class StageView : UserControl, IBindUpdate
    {
        [DllImport("user32.dll")]
        static extern bool HideCaret(IntPtr hWnd);

        private Color DefaultColor = SystemColors.Control;

        public bool Virtual { get; set; } = false;
        public string StageName => StageNameLabel?.Text??string.Empty;
        public TextBox GetLocalPCTextBox => LocalPCTextBox;

        public StageView()
        {
            InitializeComponent();
            DefaultColor = BackColor;

            StallingCheckBox.CheckedChanged += delegate { 
                BackColor = StallingCheckBox.Checked ?
                SystemColors.Info:
                DefaultColor; 
            };
            Load += delegate {
                InstructionTextBox.ForeColor = SystemColors.WindowText;
                InstructionTextBox.GotFocus += delegate { HideCaret(InstructionTextBox.Handle); };
                if (Virtual) {
                    StageNameLabel.ForeColor = SystemColors.GrayText;
                    DefaultColor = (BackColor = ControlPaint.Light(BackColor));
                }
            };
        }


        public void BindStageData(TYPStage stage)
        {
            StageNameLabel.Text = stage.Name;

            InstructionTextBox.DataBindings.Add(new Binding(nameof(TextBox.Text), stage, nameof(TYPStage.ProcessedInstruction)));
            LocalPCTextBox.DataBindings.Add(new Binding(nameof(TextBox.Text), stage.LocalPC, nameof(Register32.ShortFormat)));
            StallingCheckBox.DataBindings.Add(new Binding(nameof(CheckBox.Checked), stage, nameof(TYPStage.Stalling)));

            UpdateBindings();
        }

        public void UpdateBindings() 
        {
            GUIUtilis.ReadBinding(InstructionTextBox);
            GUIUtilis.ReadBinding(LocalPCTextBox);
            GUIUtilis.ReadBinding(StallingCheckBox);
        }
    }
}
