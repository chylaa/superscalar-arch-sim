using System.Collections;
using System.Collections.Generic;

namespace superscalar_arch_sim.RV32.Hardware.Pipeline
{
    public interface IInstructionEntryCollection<T> : ICollection, IEnumerable<T>, IEnumerable where T : IUniqueInstructionEntry
    {
        /// <summary>
        /// Indicates that this <see cref="IInstructionEntryCollection{T}"/> 
        /// consist of elements sorted by <see cref="IUniqueInstructionEntry.Tag"/>.
        /// </summary>
        bool IsSortedSet { get; }
        /// <summary>Number of <see cref="T.MarkedEmpty"/> objects in internal collection.</summary>
        int EmptyEntries { get; }
        void ResetAll();
    }
}
