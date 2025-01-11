using superscalar_arch_sim.RV32.Hardware.CPU;
using superscalar_arch_sim_gui.UserControls.Units;
using System.Collections.Generic;
using System.Drawing;

namespace superscalar_arch_sim_gui.UserControls
{
    public interface IGenericCPUView<CPU> : IBindUpdate, ISimEventsAttachable where CPU : ICPU
    {
        CPU Core { get; }
        Size DesirableSize { get; }
        BranchPredictorView GetBranchPredictorView { get; }
        Inspection.QuickMemViewer GetQuickMemView { get; }
        
        /// <summary>
        /// Collection of components implementing <see cref="IBindUpdate"/> interface, which allows to iterate and invoke 
        /// <see cref="IBindUpdate.UpdateBindings"/> method on each of elements.
        /// </summary>
        List<IBindUpdate> UpdateableComponents { get; }

        void InitControlData(CPU core);
        void ClearAfterReset();
        void CloseAllSubforms();
    }
}
