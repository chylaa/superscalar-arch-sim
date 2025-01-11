using System.Windows.Forms;

namespace superscalar_arch_sim_gui.UserControls
{
    public interface IBindUpdate
    {
        /// <summary>
        /// Method intefrace for executing all neccessary operation for updating view of all underlying components 
        /// implementng <see cref="IBindUpdate"/> or <see cref="System.Windows.Forms.IBindableComponent"/>.
        /// </summary>
        void UpdateBindings();
    }
}
