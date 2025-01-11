using superscalar_arch_sim.RV32.Hardware.CPU;
using System.Windows.Forms;

namespace superscalar_arch_sim_gui.Forms
{
    public partial class RegisterFileView : Form
    {
        public void RefreshAll() => RegFileTemplateView.RefreshAll();
        public RegisterFileView(ICPU simulatedCPU)
        {
            InitializeComponent();
            RegFileTemplateView.InitView(simulatedCPU.RegisterFile, simulatedCPU.GetType() == typeof(SuperscalarCPU));
        }
    }
}
