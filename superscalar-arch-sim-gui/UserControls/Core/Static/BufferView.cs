using superscalar_arch_sim.RV32.Hardware.Pipeline.TYP.Units;
using superscalar_arch_sim.RV32.Hardware.Register;
using superscalar_arch_sim_gui.Utilis;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace superscalar_arch_sim_gui.UserControls.Core.Static
{
    public partial class BufferView : UserControl, IBindUpdate
    {
        public static readonly Color RegisterValueColorOnForwarded = Color.IndianRed;
        public static readonly Color RegisterValueColorOnDefault = TextBox.DefaultBackColor;

        public bool Virtual { get; set; } = false;
        public PipeRegisters BindedPipelineRegister { get; private set; }
        public Dictionary<Register32, TextBox> RegistersTextBox { get; private set; }
        public ToolTip MainToolTip { private get; set; } = null;
        public BufferView()
        {
            InitializeComponent();
            RegistersTextBox = new Dictionary<Register32, TextBox>();

            Load += delegate {
                if (Virtual) {
                    BackColor = ControlPaint.Light(BackColor);
                }
            };
        }

        private void InitRegistersTooltips()
        {
            if (MainToolTip is null) return;
            foreach (KeyValuePair<Register32, TextBox> RegTB in RegistersTextBox)
                MainToolTip.SetToolTip(RegTB.Value, RegTB.Key.Name + ": " + RegTB.Key.Meaning);
        }

        private string GetRegisterBindingSource(PipeRegisters buffer)
        {
            if (buffer.IsVirtual)
                return nameof(Register32.HexString);
            return nameof(Register32.ShortFormat);
        }

        public void BindBufferRegisters(PipeRegisters buffer)
        {
            BindedPipelineRegister = buffer;

            RegistersTextBox.Add(buffer.A, textBoxOpA);
            RegistersTextBox.Add(buffer.B, textBoxOpB);
            RegistersTextBox.Add(buffer.Imm, textBoxImm);
            RegistersTextBox.Add(buffer.ALUOutput, textBoxALUOut);
            RegistersTextBox.Add(buffer.LoadMemoryData, textBoxLDData);
            RegistersTextBox.Add(buffer.NextPC, textBoxNextPC);
            InitRegistersTooltips();

            textBoxIReg.DataBindings.Add(new Binding(nameof(TextBox.Text), buffer, nameof(buffer.IR32)));
            textBoxOpA.DataBindings.Add(new Binding(nameof(TextBox.Text), buffer.A, GetRegisterBindingSource(buffer)));
            textBoxOpB.DataBindings.Add(new Binding(nameof(TextBox.Text), buffer.B, GetRegisterBindingSource(buffer)));
            textBoxImm.DataBindings.Add(new Binding(nameof(TextBox.Text), buffer.Imm, GetRegisterBindingSource(buffer)));
            textBoxALUOut.DataBindings.Add(new Binding(nameof(TextBox.Text), buffer.ALUOutput, GetRegisterBindingSource(buffer)));
            textBoxLDData.DataBindings.Add(new Binding(nameof(TextBox.Text), buffer.LoadMemoryData, GetRegisterBindingSource(buffer)));
            textBoxNextPC.DataBindings.Add(new Binding(nameof(TextBox.Text), buffer.NextPC, GetRegisterBindingSource(buffer)));
            textBoxCondition.DataBindings.Add(new Binding(nameof(TextBox.Text), buffer, nameof(buffer.Condition)));
        }

        public void UpdateBindings()
        {
            GUIUtilis.ReadBinding(textBoxIReg);
            GUIUtilis.ReadBinding(textBoxCondition);
            GUIUtilis.ReadBinding(RegistersTextBox.Values);
        }

        public void SetRegistersTextBoxesBackColor(Register32 reg, int? _val)
        {
            RegistersTextBox[reg].BackColor = _val is null ? RegisterValueColorOnDefault : RegisterValueColorOnForwarded;
        }

        public void ResetAllRegisterTextBoxesBackColor()
        {
            foreach (TextBox tb in RegistersTextBox.Values)
                tb.BackColor = RegisterValueColorOnDefault;
        }
    }
}
