using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace superscalar_arch_sim.Simulis.Reports
{
    #region Counters
    public interface ICounter
    {
        void Inc();
        void Add(uint v);
        void Reset();
    }
    public abstract class Counter<T> : ICounter
    {
        public T Count;
        public void Reset() => Count = default;
        public abstract void Inc();
        public abstract void Add(uint v);
        public override string ToString() => Count.ToString();
    }
    public class ShortCounter : Counter<byte>
    {
        public override void Inc() => ++Count;
        public override void Add(uint v) => Count = (byte)(v + Count);
        public static implicit operator byte(ShortCounter sc) => sc.Count;
    }
    public class LongCounter : Counter<ulong>
    {
        public override void Inc() => ++Count;
        public override void Add(uint v) => Count += v;
        public static implicit operator ulong(LongCounter lc) => lc.Count;
    }
    #endregion
    /// <summary>
    /// A structure that allows the collection of aggregate values like 
    /// <see cref="Sum"/>, <see cref="Count"/>, <see cref="Min"/> and <see cref="Max"/>
    /// from signle value <see cref="Update(double)"/> per measure Unit (e.g amout over time).
    /// </summary>
    public class Aggregate
    {
        public static Aggregate Default => new Aggregate(double.NaN);

        private double SumOfDeviations;

        /// <summary>Sum of all samples</summary>
        public double Sum;
        /// <summary>Sample count</summary>
        public ulong Count;
        /// <summary>Min collected sample.</summary>
        public double Min;
        /// <summary>Maximim collected sample.</summary>
        public double Max;

        public double Average => (Sum / Count);
        public double Variance => (SumOfDeviations / Count);
        public double StdDev => Math.Sqrt(Variance);

        public bool IsValid => (Count != 0) && (Max >= Min);

        public Aggregate(double count = double.NaN) 
        { Min = double.MaxValue; Max = double.MinValue; SumOfDeviations = Sum = 0; Count = (ulong)(double.IsNaN(count) ? 0 : count); }
        public void Reset() 
        { Min = double.MaxValue; Max = double.MinValue; SumOfDeviations = Sum = Count = 0; }
        public void Update(ICollection sample) 
            => Update(sample.Count);
        public void Update(double sample)
        {
            ++Count;
            if (sample < Min) { Min = sample; }
            if (sample > Max) { Max = sample; }
            Sum += sample; SumOfDeviations += ((sample - Average) * (sample - Average));
        }
        public override string ToString() => IsValid ? $"{{A}}[{Sum}, {Count}, {Min}, {Max}, {Average}, {Variance}, {StdDev}]" : "[0]";
    }

    public class FrequencyDistribution<TClass, TValue> : IDictionary 
        where TClass : IComparable
        where TValue : ICounter, new()
    {
        public readonly bool AcceptNewClass = false;
        public readonly bool RightOpenBin = false;
        public readonly Dictionary<TClass, TValue> Values;

        public FrequencyDistribution()
        : this(((TClass[])Enum.GetValues(typeof(TClass))).ToHashSet(), acceptNew: false, asRightOpenBin:false) {}
        public FrequencyDistribution(ISet<TClass> classes, bool acceptNew = false, bool asRightOpenBin = false)
        {
            if (acceptNew && asRightOpenBin)
                throw new ArgumentException($"Cannot create instance of {GetType().Name} that will accept new values and act as collection of bins");

            AcceptNewClass = acceptNew; RightOpenBin = asRightOpenBin;
            Values = new Dictionary<TClass, TValue>();
            foreach (var @class in classes) Values[@class] = new TValue();
            Reset();
        }
        public void Collect(TClass @class)
        {
            bool classPresent = Values.ContainsKey(@class);
            if (AcceptNewClass && false == classPresent) {
                Values[@class] = new TValue();
                Values[@class].Inc();
            } else if (classPresent) {
                Values[@class].Inc();
            } else if (RightOpenBin) {
                Values[Values.Keys.First(key => @class.CompareTo(key) >= 0)].Inc();
            } else {
                throw new ArgumentException($"Cannot increment class {@class}: Value not present and cannot accept new");
            }
        }
        public void Reset() 
            => Values.Keys.ToList().ForEach(k => Values[k].Reset());
        public static implicit operator Dictionary<TClass, TValue>(FrequencyDistribution<TClass, TValue> fd)
            => fd.Values;
        
        #region IDictionary
        public object this[object key] { get => ((IDictionary)Values)[key]; set => ((IDictionary)Values)[key] = value; }
        public ICollection Keys => ((IDictionary)Values).Keys;
        public bool IsReadOnly => ((IDictionary)Values).IsReadOnly;
        public bool IsFixedSize => ((IDictionary)Values).IsFixedSize;
        public int Count => ((ICollection)Values).Count;
        public object SyncRoot => ((ICollection)Values).SyncRoot;
        public bool IsSynchronized => ((ICollection)Values).IsSynchronized;
        ICollection IDictionary.Values => ((IDictionary)Values).Values;
        public void Add(object key, object value) => ((IDictionary)Values).Add(key, value);
        public void Clear() => ((IDictionary)Values).Clear();
        public bool Contains(object key) => ((IDictionary)Values).Contains(key);
        public void CopyTo(Array array, int index) => ((ICollection)Values).CopyTo(array, index);
        public IDictionaryEnumerator GetEnumerator() => ((IDictionary)Values).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Values).GetEnumerator();
        public void Remove(object key) => ((IDictionary)Values).Remove(key);
        #endregion


    }
}
