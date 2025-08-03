using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static superscalar_arch_sim.RV32.Hardware.HardwareProperties;

namespace superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Units
{

    /// <summary>Represents ROB unit in out-of-order CPU.</summary>
    public class ReorderBuffer : IInstructionEntryCollection<ROBEntry>, ICollection<ROBEntry>, IReadOnlyCollection<ROBEntry>, IListSource
    {
        public static Comparer<ROBEntry> InstructionIndexComparer
            = Comparer<ROBEntry>.Create((re1, re2) => re1.InstructionIndex.CompareTo(re2.InstructionIndex));

        public const int TAGIDX_OFFSET = 1; // start "RobEntry.Tag" from 1

        private readonly List<ROBEntry> _entries = new List<ROBEntry>();
        
        /// <param name="index">Index of element in collection (matches/equal to <see cref="ROBEntry.Tag"/> - 1).</param>
        /// <returns><see cref="ROBEntry"/> at <paramref name="index"/>.</returns>
        public ROBEntry this[int index] => _entries[index];

        /// <summary>Special field for saving <b>h</b> - cuurent head entry of <see cref="ReorderBuffer"/>, 
        /// set while issuing insruction. Initialized with <see langword="null"/>.</summary>
        public ROBEntry HeadEntry { get; private set; }
        
        /// <summary><inheritdoc cref="ICollection.Count"/></summary>
        public int Count => _entries.Count;
        /// <summary><inheritdoc cref="ICollection.SyncRoot"/></summary>
        public object SyncRoot => ((ICollection)_entries).SyncRoot;
        /// <summary><inheritdoc cref="ICollection.IsSynchronized"/></summary>
        public bool IsSynchronized => ((ICollection)_entries).IsSynchronized;
        /// <summary><inheritdoc cref="IListSource.ContainsListCollection"/></summary>
        public bool ContainsListCollection => false;

        /// <summary><inheritdoc cref="IInstructionEntryCollection{T}.EmptyEntries"/></summary>
        public int EmptyEntries => EmptyEntriesCount();
        /// <summary><inheritdoc cref="IInstructionEntryCollection{T}.IsSortedSet"/></summary>
        public bool IsSortedSet => false;

        public bool IsReadOnly => ((ICollection<ROBEntry>)_entries).IsReadOnly;

        public ReorderBuffer(int size) 
        {
            for (int i = TAGIDX_OFFSET; i < size; i++)
            {
                Add(new ROBEntry(i, TEMPipelineStage.None));
            }
            HeadEntry = _entries[0];
        }
        public ReorderBuffer(IEnumerable<ROBEntry> entries)
        {
            int? firstTag = entries.FirstOrDefault()?.Tag;
            if (firstTag != TAGIDX_OFFSET)
                throw new ArgumentException($"First ROB Entry Tag must equal {TAGIDX_OFFSET} ({firstTag})");
            _entries.AddRange(entries);
            HeadEntry = _entries[0];
        }

        /// <summary>Calls <see cref="ROBEntry.Reset"/> on each entry in collection, and sets <see cref="HeadEntry"/> as first of entries.</summary>
        public void ResetAll()
        {
            for (int i = 0; i < Count; i++)
            {
                this[i].Reset();
            }
            HeadEntry = _entries[0];
        }

        /// <summary>
        /// Resets all existing <see cref="ROBEntry"/> 
        /// and adds/removes <see cref="ROBEntry"/> objects to/from end of internal collection. 
        /// </summary>
        /// <param name="size">New <see cref="Count"/> of <see cref="ReorderBuffer"/>.</param>
        /// <param name="tagCollection">Collection of tags for new <see cref="ROBEntry"/> items.</param>
        public void ResetResize(int size)
        {
            Resize(size, state:TEMPipelineStage.None);
            ResetAll();
        }

        /// <summary>
        /// Search <see cref="ReorderBuffer"/> entries, to update <see cref="HeadEntry"/>, by finding
        /// lowest <see cref="ROBEntry.InstructionIndex"/> of entry that is not <see cref="ROBEntry.MarkedEmpty"/>.
        /// If all are, <see cref="ROBEntry"/> with tag = 0 is assumed.
        /// </summary>
        public void UpdateHeadEntry()
        {
            int headIdx = 0;
            for (int i = 1; i < _entries.Count; i++)
            {
                if (false == _entries[i].MarkedEmpty)
                {
                    if (_entries[i].InstructionIndex < _entries[headIdx].InstructionIndex)
                        headIdx = i;
                }
            }
            HeadEntry = _entries[headIdx];
        }

        public ROBEntry[] GetSortedFromOldest()
        {
            var sorted = _entries.ToArray();
            Array.Sort(sorted, InstructionIndexComparer);
            return sorted;
        }
        /// <summary>
        /// Returns <see cref="ROBEntry"/> which <see cref="ROBEntry.Tag"/> equals <paramref name="tag"/>.<br></br>
        /// Note: <paramref name="tag"/> corresponds to index of <see cref="ROBEntry"/> in <see cref="ReorderBuffer"/>
        /// <b>minus <see cref="TAGIDX_OFFSET"/></b> (tags indexing starting from <see cref="TAGIDX_OFFSET"/>).
        /// To access internal entries list via 0-based indexing, see <see cref="this[int]"/>.
        /// </summary>
        /// <param name="tag"><see cref="ROBEntry.Tag"/> of <see cref="ROBEntry"/> to be returned. Matches index of entry - 1.</param>
        /// <returns><see cref="ROBEntry"/> which <see cref="ROBEntry.Tag"/> equals given <paramref name="tag"/>.</returns>
        public ROBEntry GetEntryByTag(int tag)
        {
            return _entries[tag-TAGIDX_OFFSET];
        }
        /// <returns>
        /// <see cref="ROBEntry"/> collection consisting of all entries that are older than (issued before) given <paramref name="newest"/> entry.
        /// Result entries will include only those which <see cref="ROBEntry.MarkedEmpty"/> field equals <paramref name="markedEmpty"/>.
        /// </returns>
        public IEnumerable<ROBEntry> GetAllOlderEntries(ROBEntry newest, bool markedEmpty = false)
            => _entries.Where(e => e.MarkedEmpty == markedEmpty && e.InstructionIndex < newest.InstructionIndex);

        /// <returns>
        /// <see cref="ROBEntry"/> collection consisting of all entries that were issued more "recenly"
        /// than given <paramref name="oldest"/> entry. 
        /// Result entries will include only those which <see cref="ROBEntry.MarkedEmpty"/> field equals <paramref name="markedEmpty"/>.
        /// </returns>
        public IEnumerable<ROBEntry> GetAllNewerEntries(ROBEntry oldest, bool markedEmpty = false)
            => _entries.Where(e => e.MarkedEmpty == markedEmpty && e.InstructionIndex > oldest.InstructionIndex);

        public void GetNewerAndOlderEntries(ROBEntry entry, bool markedEmpty, in List<ROBEntry> older, in List<ROBEntry> newer)
        {
            for (int i = 0; i < Count; i++)
            {
                if (_entries[i].MarkedEmpty == markedEmpty)
                {
                    if (entry.InstructionIndex < _entries[i].InstructionIndex)
                        newer.Add(_entries[i]);
                    else if (entry.InstructionIndex > _entries[i].InstructionIndex)
                        older.Add(_entries[i]);
                }
            }
        }

        public int EmptyEntriesCount()
        {
            int num = 0; 
            for (int i = 0; i < Count; i++) {
                if (_entries[i].MarkedEmpty)
                    ++num;
            }
            return num;
        }

        /// <summary><inheritdoc cref="ICollection.CopyTo(Array, int)"/></summary>
        public void CopyTo(Array array, int index)
            => ((ICollection)_entries).CopyTo(array, index);
        /// <summary><inheritdoc cref="IEnumerable{ROBEntry}.GetEnumerator"/></summary>
        public IEnumerator<ROBEntry> GetEnumerator()
            =>_entries.GetEnumerator();
        /// <summary><inheritdoc cref="IListSource.GetList"/></summary>
        public IList GetList()
            => _entries;
        /// <summary><inheritdoc cref="IEnumerable.GetEnumerator"/></summary>
        IEnumerator IEnumerable.GetEnumerator()
            => _entries.GetEnumerator();

        private void Resize(int newsize, TEMPipelineStage state = TEMPipelineStage.None)
        {
            int oldsize = Count;
            if (oldsize < newsize)
            {
                for (int i = oldsize; i < newsize; i++)
                {
                    Add(new ROBEntry(TAGIDX_OFFSET + i, state));
                }
            }
            else if (newsize < oldsize)
            {
                for (int i = newsize; i < oldsize; i++)
                {
                    Remove(this[newsize]);
                }
            }
        }
        #region ICollection<ROBEntry>
        public void Add(ROBEntry item) 
            => _entries.Add(item);
        public void Clear()
            => _entries.Clear();
        public bool Contains(ROBEntry item)
            => _entries.Contains(item);
        public void CopyTo(ROBEntry[] array, int arrayIndex)
            => _entries.CopyTo(array, arrayIndex);
        public bool Remove(ROBEntry item)
            => _entries.Remove(item);
        #endregion
    }
}
