using superscalar_arch_sim.RV32.Hardware.Memory;
using superscalar_arch_sim_gui.Utilis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace superscalar_arch_sim_gui.UserControls.Inspection
{
    public partial class QuickMemViewer : UserControl
    {
        private readonly Dictionary<NumericUpDown, Label> AddressValueDisplayPairs = null;
        public IMemoryComponent ObservedMemory { get; set; } = null;

        //TODO: Allow user to select style for each of individual labels
        public StrConverter.StringStyle MemValueStyle { get; set; } = StrConverter.StringStyle.Hex;

        public QuickMemViewer()
        {
            InitializeComponent();
            AddressValueDisplayPairs = new Dictionary<NumericUpDown, Label>()
            {
                {numericUpDown1, label1},
                {numericUpDown2, label2},
                {numericUpDown3, label3},
                {numericUpDown4, label4}
            };

            foreach (var numud in AddressValueDisplayPairs.Keys) {
                numud.ValueChanged += OnNewAddressValueSelected;
            }
        }

        public void SetMemoryComponent(IMemoryComponent memory)
        {
            ObservedMemory = memory;
            foreach (var numud in AddressValueDisplayPairs.Keys)
            {
                numud.Increment = sizeof(UInt32);
                numud.Minimum = memory.Origin;
                numud.Maximum = (memory.ByteSize - numud.Increment);
            }
        }

        public void SetAddressValue(int numUpDownIndex, uint addr)
        {
            NumericUpDown addrsource = AddressValueDisplayPairs.ElementAtOrDefault(numUpDownIndex).Key;
            if (addrsource != null) addrsource.Value = addr;
        }
        
        public void UpdateAllMemoryValues()
        {
            foreach (var numud in AddressValueDisplayPairs.Keys)
                UpdateMemoryValue(numud);
        }

        private void UpdateMemoryValue(NumericUpDown addrSource)
        {
            uint address = decimal.ToUInt32(addrSource.Value);
            string text = StrConverter.FormatValue(MemValueStyle, ObservedMemory.ReadWord(address));
            AddressValueDisplayPairs[addrSource].Text = text;
        }

        private void OnNewAddressValueSelected(object sender, EventArgs e)
        {
            if (sender is NumericUpDown addrSelector)
            {
                try 
                { 
                    UpdateMemoryValue(addrSelector);
                }
                catch (Exception ex) 
                {
                    addrSelector.Value = 0;
                    MessageBox.Show($"Cannot display selected memory address. Memory range does not match: {ex.Message}",
                                    ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
                
        }
    }
}
