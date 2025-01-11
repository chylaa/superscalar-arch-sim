using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Units
{
    public class ReservationStationCollection : IInstructionEntryCollection<ReservationStation>
    {
        public static Comparer<ReservationStation> InstructionIndexComparer
            = Comparer<ReservationStation>.Create((rs1, rs2) => rs1.InstructionIndex.CompareTo(rs2.InstructionIndex));
        
        private readonly Dictionary<int, ReservationStation> _tagdictionary;
        private ReservationStation[] _reservations;

        public ReservationStation this[int index] => _reservations[index]; 
        public int Count => _reservations.Length;

        /// <summary><inheritdoc cref="IInstructionEntryCollection{T}.EmptyEntries"/></summary>
        public int EmptyEntries => EmptyEntriesCount();
        /// <summary><inheritdoc cref="IInstructionEntryCollection{T}.IsSortedSet"/></summary>
        public bool IsSortedSet => false;
        /// <summary><inheritdoc cref="ICollection.SyncRoot"/></summary>
        public object SyncRoot => _reservations.SyncRoot;
        /// <summary><inheritdoc cref="ICollection.IsSynchronized"/></summary>
        public bool IsSynchronized => _reservations.IsSynchronized;

        /// <summary>
        /// Creates new <see cref="ReservationStationCollection"/> from existing <see cref="ReservationStation"/> <paramref name="reservations"/>.
        /// Throws <see cref="ArgumentException"/> if not all <see cref="ReservationStation.Tag"/> in <paramref name="reservations"/> are unique.</summary>
        /// <param name="collections">Existing set of <see cref="ReservationStation"/> items.</param>
        /// <exception cref="ArgumentException"></exception>
        public ReservationStationCollection(IEnumerable<ReservationStation> reservations)
        {
            _reservations = reservations.ToArray();
            _tagdictionary = _reservations.ToDictionary(x => x.Tag);
        }
        /// <summary>
        /// Creates new <see cref="ReservationStationCollection"/> from existing set of <paramref name="collections"/>.
        /// Throws <see cref="ArgumentException"/> if not all <see cref="ReservationStation.Tag"/> in every of <paramref name="collections"/> are unique.</summary>
        /// <param name="collections">Existing set of <paramref name="collections"/></param>
        /// <exception cref="ArgumentException"></exception>
        public ReservationStationCollection(IEnumerable<ReservationStationCollection> collections)
        {
            _reservations = collections.SelectMany(c => c._reservations).ToArray();
            _tagdictionary = _reservations.ToDictionary(x => x.Tag);
        }
        public ReservationStationCollection(ISet<int> tags)
        {
            int first = tags.First();
            _reservations = new ReservationStation[tags.Count];
            _tagdictionary = new Dictionary<int, ReservationStation>();
            for (int tag = first; tag < tags.Count + first; tag++)
            {
                var station = (_reservations[tag - first] = new ReservationStation(tag));
                _tagdictionary.Add(tag, station);
            }
        }
        #region Reservation Station specific

        /// <summary>Calls <see cref="ReservationStation.Reset"/> on each <see cref="ReservationStation"/> in collection.</summary>
        public void ResetAll()
        {
            foreach (var station in _reservations) {
                station.Reset();
            }
        }
       
        /// <summary>
        /// Returns <see cref="ReservationStation"/> with lowest <see cref="ReservationStation.InstructionIndex"/>,
        /// and <see cref="ReservationStation.MarkedEmpty"/> equal to <see langword="false"/>.
        /// </summary>
        /// <returns><see cref="ReservationStation"/> object that meets conditions or <see langword="null"/> if none valid found.</returns>
        public ReservationStation GetOldestFromAllOrDefault(bool busy, Predicate<ReservationStation> busyRelax)
        {
            ReservationStation oldest = null;
            for (int i = 0; i < Count; i++)
            {
                var rs = _reservations[i];
                if (false == rs.MarkedEmpty && (busy == rs.Busy || busyRelax(rs)) && false == rs.ROBDest.Busy)
                {
                    if (oldest is null || oldest.InstructionIndex > rs.InstructionIndex)
                    {
                        oldest = rs;
                    }
                }
            }
            return oldest;
        }
        public bool HasTag(int tag)
        {
            for (int i = 0; i < Count; i++)
            {
                var rs = _reservations[i];
                if (tag == rs.Tag)
                {
                    return true;
                }
            }
            return false;
        }
        public int EmptyEntriesCount()
        {
            int num = 0;
            for (int i = 0; i < Count; i++)
            {
                if (_reservations[i].MarkedEmpty)
                    ++num;
            }
            return num;
        }

        #endregion

        #region General Enumerable access
        /// <summary>
        /// Creates <see langword="new"/> <see cref="ReservationStationCollection"/> that consist of all combined <see cref="ReservationStation"/>
        /// object from passes <paramref name="collections"/>.
        /// </summary>
        /// <param name="collections">Collections of <see cref="ReservationStation"/> from new <see cref="ReservationStationCollection"/>.</param>
        /// <returns><see langword="new"/> <see cref="ReservationStationCollection"/> created from <see cref="ReservationStation"/> objects in <paramref name="collections"/>.</returns>
        public static ReservationStationCollection Concat(params ReservationStationCollection[] collections)
            => new ReservationStationCollection(collections.SelectMany(c => c._reservations));
        

        /// <exception cref="System.InvalidOperationException"></exception>
        public ReservationStation GetStationByTag(int tag)
            => _tagdictionary[tag];
        public ReservationStation GetStationByTagOrDefault(int tag)
            => _tagdictionary.TryGetValue(tag, out ReservationStation rs) ? rs : null;
        
        public IEnumerator<ReservationStation> GetEnumerator()
            => _reservations.AsEnumerable().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
            => _reservations.GetEnumerator();
        public ReservationStation[] GetUnderlyingArray()
            => _reservations;

        public void ContactResetResize(params ReservationStationCollection[] stations)
            => ResetResize(stations.SelectMany(collection => collection.AsEnumerable()));
        public void ResetResize(IEnumerable<ReservationStation> stations)
        {
            if (stations != null && stations.Any())
            {
                Array.Resize(ref _reservations, stations.Count());
                _tagdictionary.Clear();
                for (int i = 0; i < _reservations.Length; i++)
                {
                    _reservations[i] = stations.ElementAt(i);
                    _tagdictionary.Add(_reservations[i].Tag, _reservations[i]); // ensures uniquity
                }
            }
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
