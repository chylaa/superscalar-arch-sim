using superscalar_arch_sim.RV32.Hardware.Pipeline;
using superscalar_arch_sim.RV32.Hardware.Pipeline.TYP.Units;

namespace superscalar_arch_sim_gui.UserControls.Core.Static
{

    /// <summary>
    /// Represents <see cref="CustomControls.Datapath"/> that is used for forwarding data in pipeline.
    /// </summary>
    public partial class FWDatapath : CustomControls.Datapath
    {
        public PipeRegisters SourceBuffer { get; private set; }
        public PipeRegisters DestinationBuffer { get; private set; }

        public FWDatapath()
        {
            InitializeComponent();
        }

        public void SetForwardingDatapathPipeRegisters(PipeRegisters src, PipeRegisters dst)
        {
            SourceBuffer = src; 
            DestinationBuffer = dst;
        }

        public void OnDataForwarded(object sender, DatapathBufferEventArgs<PipeRegisters> e)
        {
            if (e.DataSource == SourceBuffer && e.DataDest == DestinationBuffer) {
                SetDataValue(e.Value);
            }
        }

        public void ResetFWDataPath() => SetDataValue(null);
    }
}
