using superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Units;
using superscalar_arch_sim_gui.Utilis;
using System;
using System.Collections.Concurrent;
using System.Windows.Forms;

namespace superscalar_arch_sim_gui.UserControls.Core.Dynamic
{
    public partial class InstructionQueueView : UserControl, IBindUpdate
    {
        public InstructionDataQueue IRDataQueue { get; set; }
        public InstructionQueueView()
        {
            InitializeComponent();
            StandardControls.StdDataGridView.InitStdDataGridView(QueueDataGridView, null);
            QueueDataGridView.AutoGenerateColumns = false;
            AdjustVisibleColumns(null, null);
        }

        private void AdjustVisibleColumns(object sender, EventArgs e)
        {
            QueueDataGridView.Columns.Add(nameof(PipeRegisters.LocalPC), "LPC");
            QueueDataGridView.Columns.Add(nameof(PipeRegisters.IR32), "IR32");
        }

        public void BindIntructionQueueObject(InstructionDataQueue queue)
        {
            IRDataQueue = queue;
        }

        public void UpdateBindings()
        {
            QueueDataGridView.Rows.Clear();
            ConcurrentBag<PipeRegisters> snapshot = new ConcurrentBag<PipeRegisters>(IRDataQueue?.GetSnapshot());
            if (snapshot is null)
                return;

            foreach (PipeRegisters item in snapshot) 
            {
                if (QueueDataGridView?.Rows != null && item?.LocalPC?.HexString != null && item?.IR32 != null)
                {
                    QueueDataGridView.Rows.Add(item.LocalPC.HexString, item.IR32.ToString());
                }
            }
        }
    }
}
