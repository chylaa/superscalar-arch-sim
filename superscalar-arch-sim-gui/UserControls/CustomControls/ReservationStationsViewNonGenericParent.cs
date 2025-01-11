using superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Units;

namespace superscalar_arch_sim_gui.UserControls.CustomControls
{
    /// <summary>Hack around Windows Forms limitation of using generics with Designer (also will compile but throw at runtime).</summary>
    public class ReservationStationsViewNonGenericParent 
        : Utilis.StandardControls.StandardEntryCollectionView<ReservationStationCollection, ReservationStation> 
    { 
    
    }
}
