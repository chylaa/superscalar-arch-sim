using superscalar_arch_sim.RV32.Hardware.CPU;

namespace superscalar_arch_sim_gui.UserControls
{
    internal interface ICPUBindable
    {
        Inspection.QuickMemViewer GetQuickMemView { get; }
        CPU GetCore<CPU>() where CPU : ICPU; 
        void InitControlData(ICPU core);
        void UpdateAllSubviews(bool pipeline);
    }
}
