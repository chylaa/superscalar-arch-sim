using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
using System.Windows.Forms;

namespace superscalar_arch_sim_gui.UserControls.CustomControls
{
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip)]
    public class ToolStripNumberControl : ToolStripControlHost
    {
        public event EventHandler ValueChanged;
        public Control NumericUpDownControl => (Control as NumericUpDown);
        public decimal Value { get => (NumericUpDownControl as NumericUpDown).Value; set => (NumericUpDownControl as NumericUpDown).Value = value; }
        public decimal Maximum { get => (NumericUpDownControl as NumericUpDown).Maximum; set => (NumericUpDownControl as NumericUpDown).Maximum = value; }
        public decimal Minimum { get => (NumericUpDownControl as NumericUpDown).Minimum; set => (NumericUpDownControl as NumericUpDown).Minimum = value; }
        public decimal Increment { get => (NumericUpDownControl as NumericUpDown).Increment; set => (NumericUpDownControl as NumericUpDown).Increment = value; }
        public bool Hexadecimal { get => (NumericUpDownControl as NumericUpDown).Hexadecimal; set => (NumericUpDownControl as NumericUpDown).Hexadecimal = value; }

        public ToolStripNumberControl() : base(new NumericUpDown()) { (NumericUpDownControl as NumericUpDown).TextAlign = HorizontalAlignment.Center; }
        
        protected override void OnSubscribeControlEvents(Control control)
        {
            base.OnSubscribeControlEvents(control);
            ((NumericUpDown)control).ValueChanged += new EventHandler(OnValueChanged);
        }

        protected override void OnUnsubscribeControlEvents(Control control)
        {
            base.OnUnsubscribeControlEvents(control);
            ((NumericUpDown)control).ValueChanged -= new EventHandler(OnValueChanged);
        }

        protected void OnValueChanged(object sender, EventArgs e)
        {
            ValueChanged?.Invoke(this, e);
        }
    }
}
