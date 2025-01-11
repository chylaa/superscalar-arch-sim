using superscalar_arch_sim.RV32.Hardware.CPU;
using superscalar_arch_sim.RV32.Hardware.Pipeline;
using superscalar_arch_sim.RV32.Hardware.Pipeline.TYP.Stage;
using superscalar_arch_sim.RV32.Hardware.Pipeline.TYP.Units;
using superscalar_arch_sim.Simulis;
using superscalar_arch_sim_gui.UserControls.Units;
using superscalar_arch_sim_gui.Utilis;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace superscalar_arch_sim_gui.UserControls.Core.Static
{
    public partial class ScalarCoreView : UserControl, IGenericCPUView<ICPU>
    {
        readonly FWDatapath[] ForwardingDatapats;

        readonly Dictionary<PipeRegisters, BufferView> PipeRegistersToBufferViews;
        readonly Dictionary<TYPStage, StageView> PipeStagesToViews;

        readonly Color DefaultStageLocalPCBackColor;
        
        private readonly StageView[] StageViews;
        private readonly BufferView[] BufferViews;

        public Size DesirableSize { get; private set; }
        public ScalarCPU Core { get; private set; }
        ICPU IGenericCPUView<ICPU>.Core => Core;
        public Inspection.QuickMemViewer GetQuickMemView => quickMemViewer1;
        public BranchPredictorView GetBranchPredictorView => branchPredictorView1;
        public List<IBindUpdate> UpdateableComponents { get; private set; }
        public bool EventsAttached { get; private set; } = false;

        public ScalarCoreView()
        {
            InitializeComponent();
            DesirableSize = new Size(Size.Width, Size.Height);

            StageViews = new StageView[] { stageViewIF, stageViewID, stageViewEX, stageViewMEM, stageViewWB };
            BufferViews = new BufferView[] { IFIDbufferView, IDEXbufferView, EXMEMbufferView, MEMWBbufferView };
            ForwardingDatapats = new FWDatapath[] { fwDatapathWBtoEX, fwDatapathMEMtoEX, fwDatapathWBtoMEM };

            PipeRegistersToBufferViews = new Dictionary<PipeRegisters, BufferView>();
            PipeStagesToViews = new Dictionary<TYPStage, StageView>();
            DefaultStageLocalPCBackColor = stageViewIF.GetLocalPCTextBox.BackColor;
        }

        public void AttatchSimEventHandlers()
        {
            if (false == EventsAttached)
            {
                for (int i = 0; i < ForwardingDatapats.Length; i++)
                {
                    Core.DataForwarded += ForwardingDatapats[i].OnDataForwarded;
                    Core.DataForwarded += OnDataForwarded;
                }
                Core.PCSelectedFromPipeRegister += OnNewPCSelectedFromIDEX;
                EventsAttached = true;
            }
        }

        public void DetachSimEventHandlers()
        {
            if (EventsAttached)
            {
                Core.DataForwarded = null;
                Core.PCSelectedFromPipeRegister = null;
                EventsAttached = false;
            }
        }

        private void InitBindings()
        {
            branchPredictorView1.InitBranchPredictorBindings(Core.BranchPredictor, Core.SimReport);
            simCountersView1.InitBindings(Core);

            var gpcBind = new Binding(nameof(TextBox.Text), Core, nameof(Core.GlobalPCString));
            GlobalPCTextBox.DataBindings.Add(gpcBind);

            for (int i = 0; i < StageViews.Length; i++) {
                StageViews[i].BindStageData(Core.Pipeline[i]);
                PipeStagesToViews.Add(Core.Pipeline[i], StageViews[i]);

                if (i == 0) continue; // Skip not shown IFBuffer
                BufferViews[i-1].BindBufferRegisters(Core.PipelineBuffers[i]);
                PipeRegistersToBufferViews.Add(Core.PipelineBuffers[i], BufferViews[i - 1]);
            }

            UpdateableComponents = new List<IBindUpdate>() { branchPredictorView1, simCountersView1 };
            UpdateableComponents.AddRange(StageViews);
            UpdateableComponents.AddRange(BufferViews);
        }

        private void OnDataForwarded(object sender, DatapathBufferEventArgs<PipeRegisters> e)
        {
            if (e.RegSource != null)
                PipeRegistersToBufferViews[e.DataSource].SetRegistersTextBoxesBackColor(e.RegSource, e.Value);
            if (e.RegDest != null)
                PipeRegistersToBufferViews[e.DataDest].SetRegistersTextBoxesBackColor(e.RegDest, e.Value);
        }

        private void OnNewPCSelectedFromIDEX(object sender, DatapathBufferEventArgs<PipeRegisters> e) 
        {
            PCNewDatapath.SetForwardingDatapathPipeRegisters(e.DataSource, null);
            PCNewDatapath.DataValue = e.Value;
            BufferView sourceView = PipeRegistersToBufferViews[e.DataSource];
            PCNewDatapath.Width = (sourceView.Location.X - PCNewDatapath.Location.X + PCNewDatapath.VerticalOffset);
            //sourceView.SetRegistersTextBoxesBackColor(e.RegSource, e.Value);
            stageViewIF.GetLocalPCTextBox.BackColor = e.Value is null ? DefaultStageLocalPCBackColor : BufferView.RegisterValueColorOnForwarded;
        }

        private void ClearFWDatapaths()
        {
            Array.ForEach(ForwardingDatapats, fwd => fwd.ResetFWDataPath());
            Array.ForEach(PipeRegistersToBufferViews.Values.ToArray(), bv => bv.ResetAllRegisterTextBoxesBackColor());
            PCNewDatapath.ResetFWDataPath();
            stageViewIF.GetLocalPCTextBox.BackColor = DefaultStageLocalPCBackColor;
        }
        public void InitControlData(ICPU core)
        {
            if (core is ScalarCPU scalar) {
                InitControlData(scalar);
            }
        }

        public void InitControlData(ScalarCPU core)
        {
            if (Core == core)
                return;

            Core = core;
            
            fwDatapathWBtoEX.SetForwardingDatapathPipeRegisters(Core.MEM_WBBuffer, Core.ID_EXBuffer);
            fwDatapathMEMtoEX.SetForwardingDatapathPipeRegisters(Core.EX_MEMBuffer, Core.ID_EXBuffer);
            fwDatapathWBtoMEM.SetForwardingDatapathPipeRegisters(Core.MEM_WBBuffer, Core.EX_MEMBuffer);

            PCNewDatapath.SetForwardingDatapathPipeRegisters(Core.EX_MEMBuffer, null);
            PCNewDatapath.Width = (EXMEMbufferView.Location.X - PCNewDatapath.Location.X + PCNewDatapath.VerticalOffset);

            quickMemViewer1.SetMemoryComponent(Core.MMU);
            quickMemViewer1.SetAddressValue(numUpDownIndex: 2, addr: Core.RAMStart);
            quickMemViewer1.SetAddressValue(numUpDownIndex: 3, addr: Core.RAMStart);

            AttatchSimEventHandlers();
            InitBindings();
            UpdateBindings();
        }

        public void UpdateBindings()
        {
            #region "Base"
            GetBranchPredictorView?.UpdateBindings();
            GetQuickMemView?.UpdateAllMemoryValues();
            UpdateableComponents?.ForEach(ucom => ucom.UpdateBindings());
            #endregion

            GUIUtilis.ReadBinding(GlobalPCTextBox);
            Array.ForEach(ForwardingDatapats, fwd => fwd.Visible = Settings.Static_UseForwarding);
        }

        public void ClearAfterReset()
        {
            ClearFWDatapaths();
        }

        public void CloseAllSubforms()
        {
            GetBranchPredictorView?.CloseDetailedViewIfShown();
        }

        public override void Refresh()
        {
            base.Refresh();
            fwDatapathWBtoEX.SendToBack();
        }
    }
}
