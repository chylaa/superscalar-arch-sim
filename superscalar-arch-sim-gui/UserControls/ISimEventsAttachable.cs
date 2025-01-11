
namespace superscalar_arch_sim_gui.UserControls
{
    public interface ISimEventsAttachable
    {
        bool EventsAttached { get; }
        void AttatchSimEventHandlers();
        void DetachSimEventHandlers();

    }
}
