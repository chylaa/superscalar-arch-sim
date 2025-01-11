using superscalar_arch_sim.RV32.Hardware.Memory;
using System;
using System.Windows.Forms;

namespace superscalar_arch_sim_gui.Forms
{
    public partial class MemoryViewer : Form
    {

        public MemoryViewer(Memory ram, Memory rom)
        {
            InitializeComponent();
            RAMMemoryViewControl.BindMemoryObject(ram);
            ROMMemoryViewControl.BindMemoryObject(rom);
            Shown += delegate { RAMMemoryViewControl.RefreshMemoryView(); ROMMemoryViewControl.RefreshMemoryView(); };
            TopMost = false;
        }
        public void BindGlobalPCAddressGetterToROM(Func<uint> getPC)
        {
            ROMMemoryViewControl.HighlightAddress = getPC;
        }
        public void RefreshMemoryView(bool ram, bool rom, bool romHighlight)
        {
            if (ram) { RAMMemoryViewControl.RefreshMemoryView(force:true); }
            if (rom) { ROMMemoryViewControl.RefreshMemoryView(force: true); }
            else if (romHighlight) { ROMMemoryViewControl.RefreshHighlightAddressIfSet(); }
        }
    }
}
