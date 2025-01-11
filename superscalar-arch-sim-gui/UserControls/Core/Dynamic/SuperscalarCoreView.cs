using superscalar_arch_sim.RV32.Hardware.CPU;
using superscalar_arch_sim.RV32.Hardware.Pipeline;
using superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.FuncUnit;
using superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Stage;
using superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Units;
using superscalar_arch_sim_gui.UserControls.Inspection;
using superscalar_arch_sim_gui.UserControls.Units;
using superscalar_arch_sim_gui.Utilis;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace superscalar_arch_sim_gui.UserControls.Core.Dynamic
{
    public partial class SuperscalarCoreView : UserControl, IGenericCPUView<ICPU>
    {
        private readonly Color DefaultGlobalPCBackColor;
        private readonly Panel[] InterstageMockPanels;
        private readonly MultiIssueStageView[] StagesViews;
        private readonly ReservationStationsView[] ReservationViews;
        private readonly ExecuteUnitSetView[] ExecuteUnitsViews;

        private Control[] ControlsWithBindings; 
        private Dictionary<MultiIssueStageView, TEMStage> ViewToStage;

        public SuperscalarCPU Core { get; private set; }
        ICPU IGenericCPUView<ICPU>.Core => Core;
        public Size DesirableSize { get; private set; }
        public QuickMemViewer GetQuickMemView => quickMemViewer1;
        public BranchPredictorView GetBranchPredictorView => branchPredictorView1;
        public List<IBindUpdate> UpdateableComponents { get; private set; }
        public bool EventsAttached { get; private set; } = false;

        public SuperscalarCoreView()
        {
            InitializeComponent();
            DesirableSize = new Size(Size.Width, Size.Height);

            InterstageMockPanels = GUIUtilis.GetAllDirectChildControlsIntersectingWithBounds<Panel>(this, DecodeStageView.Bounds).ToArray();
            StagesViews = GUIUtilis.RecursivelyGetAllChildrenOfType<MultiIssueStageView>(this).ToArray();
            ReservationViews = GUIUtilis.RecursivelyGetAllChildrenOfType<ReservationStationsView>(this).ToArray();
            ExecuteUnitsViews = GUIUtilis.RecursivelyGetAllChildrenOfType<ExecuteUnitSetView>(this).ToArray();

            DefaultGlobalPCBackColor = GlobalPCTextBox.BackColor;
        }
        private void SuperscalarCoreView_Load(object sender, EventArgs e)
        {

        }
        private void SetFirstNInterstagePanelsVisible(int n)
        {
            for (int i = 0; i < InterstageMockPanels.Length; i++)
                InterstageMockPanels[i].Visible = (i < n);
        }

        private void InitBindings()
        {
            ReservationStationCollection CreateCollection(params ExecuteUnit[] units)
                => new ReservationStationCollection(units.Select(u => u.UnitReservationStations));

            branchPredictorView1.InitBranchPredictorBindings(Core.BranchPredictor, Core.SimReport);
            simCountersView1.InitBindings(Core);

            GUIUtilis.ClearBindings(GlobalPCTextBox);
            var gpcBind = new Binding(nameof(GlobalPCTextBox.Text), Core, nameof(Core.GlobalPCString));
            GlobalPCTextBox.DataBindings.Add(gpcBind);

            DispatchInstructionQueueView.BindIntructionQueueObject(Core.IRDataQueue);

            ViewToStage = new Dictionary<MultiIssueStageView, TEMStage>() {
                { FetchStageView , Core.Fetch}, {DecodeStageView, Core.Decode}, {CompleteStageView, Core.Complete}, {RetireStageView, Core.Retire}
            };
            Array.ForEach(StagesViews, sv => sv.BindStageData(ViewToStage[sv]));
            DecodeStageView.ColumnHeadersMap[nameof(PipeRegisters.ALUOutput)] = "BTarget";
            DecodeStageView.ColumnHeadersMap[nameof(PipeRegisters.NextPC)] = "JTarget";


            BranchReservationStationsView.BindEntryCollection(CreateCollection(Core.BranchExecuteUnit));
            IntReservationStationsView.BindEntryCollection(CreateCollection(Core.IntegerExecuteUnits[0]));
            MemoryReservationStationsView.BindEntryCollection(CreateCollection(Core.MemoryBufferUnits[0]));

            BranchExecuteUnitSetView.BindExecutionUnits(Core.BranchExecuteUnit);
            IntegerExecuteUnitSetView.BindExecutionUnits(Core.IntegerExecuteUnits.ToArray());
            MemoryExecuteUnitSetView.BindExecutionUnits(Core.MemoryBufferUnits.ToArray());

            quickMemViewer1.SetMemoryComponent(Core.MMU);
            quickMemViewer1.SetAddressValue(numUpDownIndex: 2, addr: Core.RAMStart);
            quickMemViewer1.SetAddressValue(numUpDownIndex: 3, addr: Core.RAMStart);

            reorderBufferView1.BindEntryCollection(Core.ROB);

            UpdateableComponents = new List<IBindUpdate>() { 
                branchPredictorView1, 
                DispatchInstructionQueueView, 
                simCountersView1,
                reorderBufferView1,
            };
            UpdateableComponents.AddRange(StagesViews);
            UpdateableComponents.AddRange(ReservationViews);
            UpdateableComponents.AddRange(ExecuteUnitsViews);
        }

        public void InitControlData(ICPU core)
        {
            if (core is SuperscalarCPU scalar) {
                InitControlData(scalar);
            }
        }
        public void InitControlData(SuperscalarCPU core)
        {
            Core = core;
            InitBindings();
            UpdateBindings();
            if (ControlsWithBindings is null)
            {
                ControlsWithBindings = GUIUtilis.RecursivelyGetAllChildrenOfType<Control>(this).Where(c => c.DataBindings.Count > 0).ToArray();
                AttatchSimEventHandlers();
            }
            foreach (var item in ControlsWithBindings)
            {
                if (item.DataBindings.Count > 1)
                {
                    throw new Exception($"More than one data binding in control '{item.Name}'");
                }
            }
            ClearAfterReset();
        }

        public void AttatchSimEventHandlers()
        {
            if (false == EventsAttached)
            {
                Core.LoadEffectiveAddressCalculated += Core_LoadStoreEffectiveAddressCalculated;
                Core.PCSelectedFromDecode += Core_PCSelectedFromDecode;
                OnEachControlsBinding(
                    controls: ControlsWithBindings,
                    action: b => b.BindingManagerBase.ResumeBinding(),
                    filter: b => b.BindingManagerBase != null && b.BindingManagerBase.IsBindingSuspended
                );
                EventsAttached = (Enabled = true);
            }
        }

        public void DetachSimEventHandlers()
        {
            if (EventsAttached)
            {
                Core.LoadEffectiveAddressCalculated -= Core_LoadStoreEffectiveAddressCalculated;
                Core.PCSelectedFromDecode -= Core_PCSelectedFromDecode;
                OnEachControlsBinding(
                    controls: ControlsWithBindings,
                    action: b => b.BindingManagerBase.SuspendBinding(),
                    filter: b => b.BindingManagerBase != null && b.IsBinding
                );
                EventsAttached = (Enabled = false);
                simCountersView1.Enabled = true; // re-enable couters view
            }
        }

        public void UpdateBindings()
        {
            #region "Base"
            GetBranchPredictorView?.UpdateBindings();
            GetQuickMemView?.UpdateAllMemoryValues();
            UpdateableComponents?.ForEach(ucom => ucom.UpdateBindings());
            #endregion

            SetFirstNInterstagePanelsVisible(superscalar_arch_sim.Simulis.Settings.MaxIssuesPerClock);
            GUIUtilis.ReadBinding(GlobalPCTextBox);
        }

        public void ClearAfterReset()
        {
            AddressUnit_NameLabel.Text = $"Address Unit (x{Core.Dispatch.NumberOfAddressCalculationsInSingleClock})";
            AddressUnit_AddressLabel.Text = string.Empty;
            AddressUnit_InstructionLabel.Text = string.Empty;
        }
        public void CloseAllSubforms()
        {
            GetBranchPredictorView?.CloseDetailedViewIfShown();
        }

        #region Core Event Handlers
        private void Core_LoadStoreEffectiveAddressCalculated(object sender, StageDataArgs e)
        {
            EventHandler<StageDataArgs> SetAddressUnit = (object s, StageDataArgs sda) =>
            {
                AddressUnit_AddressLabel.Text = sda.DataA?.HexString ?? string.Empty;
                AddressUnit_InstructionLabel.Text = sda.Instruction?.ASM ?? string.Empty;
            };
            if (InvokeRequired) { Invoke(SetAddressUnit, sender, e); }
            else { SetAddressUnit.Invoke(sender, e);  }
        }

        private void Core_PCSelectedFromDecode(object sender, DatapathRegisterEventArgs e)
        {
            EventHandler SetGlobalPCDatapath = delegate
            {
                GPCDatapath.DataValue = e.Value;
                GlobalPCTextBox.BackColor = e.Value is null ? DefaultGlobalPCBackColor : Color.IndianRed; 
            };
            if (InvokeRequired) { Invoke(SetGlobalPCDatapath, sender, e); } else { SetGlobalPCDatapath.Invoke(sender, e); }
        }
        #endregion


        private static void OnEachControlsBinding(Control[] controls, Action<Binding> action, Predicate<Binding> filter = null)
        {
            foreach (var item in controls)
            {
                foreach (var binding in item.DataBindings)
                {
                    if (binding is Binding b)
                    {
                        if (filter is null || filter(b))
                        {
                            action(b);
                        }
                    }
                }
            }
        }
    }
}
