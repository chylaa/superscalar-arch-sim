using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Units
{
    public class InstructionDataQueue : ICollection, ICollection<PipeRegisters>, IReadOnlyCollection<PipeRegisters>, IEnumerable<PipeRegisters>, IEnumerable, IListSource
    {
        [NonSerialized]
        private object _syncRoot = new object();        
        private readonly List<PipeRegisters> _registers;

        /// <summary>
        /// Max amount of elements in Queue. If exceeded at <see cref="Enqueue(PipeRegisters)"/>, 
        /// <see cref="InvalidOperationException"/> will be thrown. When <see cref="Limit"/>
        /// is set to negative, class behaves as <see cref="Queue{PipeRegisters}"/>.
        /// </summary>
        public int Limit { get; set; } = 0;
        /// <summary>Indicates whenever item <see cref="PipeRegisters"/> can be enqueued without exceeding <see cref="Limit"/>.</summary>
        public bool CanEnqueue => (Count <= Limit);
        /// <summary><inheritdoc cref="Queue.Count"/></summary>
        public int Count => _registers.Count;
        /// <summary><inheritdoc cref="Queue.SyncRoot"/></summary>
        public object SyncRoot => _syncRoot;
        /// <summary><inheritdoc cref="Queue.IsSynchronized"/></summary>
        public bool IsSynchronized => false;
        /// <summary><inheritdoc cref="IListSource.ContainsListCollection"/></summary>
        public bool ContainsListCollection => false;
        public bool IsReadOnly => false;

        public InstructionDataQueue(int capacity) 
        {
            _registers = new List<PipeRegisters>();
            Limit = capacity; 
        }
        /// <summary><inheritdoc cref="Queue.Enqueue(object)"/></summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void Enqueue(PipeRegisters item)
        {
            if (Count + 1 > Limit)
            {
                throw new InvalidOperationException($"Cannot enqueue item. Limit of {Limit} items hit!");
            } 
            else
            {
                Add(item);
            }
        }
        /// <summary>
        /// Check if N items (<paramref name="numOfItems"/>) can be enqueued into <see cref="InstructionDataQueue"/>.</summary>
        /// <param name="numOfItems">Number of items that has to be enqueued.</param>
        /// <returns>
        /// <see langword="true"/> if after enqueue is called <paramref name="numOfItems"/> times, limit will not be exceeded
        /// (can be equal). Returns <see langword="false"/> if not all items will fit into <see cref="InstructionDataQueue"/>.
        /// </returns>
        public bool CanEnqueueN(int numOfItems)
            => (Count + numOfItems) <= Limit;

        /// <summary>Enqueues <paramref name="item"/> if <see cref="CanEnqueue"/> wihout throwing on fail.</summary>
        /// <param name="item"><inheritdoc cref="Queue{T}.Enqueue(T)"/></param>
        /// <returns><see langword="true"/> if <paramref name="item"/> sucessfully enqueued, <see langword="false"/> otherwise.</returns>
        public bool TryEnqueue(PipeRegisters item)
        {
            if (CanEnqueue) { Add(item); return true; } else { return false; }
        }

        /// <summary><inheritdoc cref="Queue.Dequeue"/></summary>
        /// <returns><inheritdoc cref="Queue.Dequeue"/></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public PipeRegisters Dequeue()
        {
            PipeRegisters first = _registers[0];
            _registers.RemoveAt(0);
            return first;
        }
        /// <summary>Dequeues into <paramref name="item"/> if <see cref="Count"/> is not 0, wihout throwing on fail.</summary>
        /// <param name="item">Container for dequeued item.</param>
        /// <returns><see langword="true"/> if <paramref name="item"/> sucessfully enqueued, <see langword="false"/> otherwise.</returns>
        public bool TryDequeue(out PipeRegisters item)
        {
            if (_registers.Count > 0) { item = Dequeue(); return true; } else { item = null; return false; }
        }

        /// <summary><inheritdoc cref="Queue.Peek"/></summary>
        /// <returns><inheritdoc cref="Queue.Peek"/></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public PipeRegisters Peek()
            => _registers[0];

        /// <summary>Returns the object from <see cref="Queue"/> at <paramref name="index"/> wihout removing it.</summary>
        /// <returns>The object from <see cref="Queue"/> at <paramref name="index"/></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public PipeRegisters Peek(int index)
            => _registers[index];

        /// <summary><inheritdoc cref="Queue.Clear"/></summary>
        public void Clear()
            => _registers.Clear();

        /// <summary><inheritdoc cref="Queue.Synchronized(Queue)"/></summary>
        /// <returns><inheritdoc cref="Queue.Synchronized(Queue)"/></returns>
        public Queue SynchronizedNonGeneric()
            => Queue.Synchronized(new Queue(_registers));

        /// <summary><inheritdoc cref="Queue.CopyTo(Array, int)"/></summary>
        public void CopyTo(Array array, int index)
            => ((ICollection)_registers).CopyTo(array, index);
        /// <summary><inheritdoc/></summary>
        /// <returns><inheritdoc/></returns>
        public IEnumerator GetEnumerator()
            => _registers.GetEnumerator();
        /// <summary><inheritdoc/></summary>
        /// <returns><inheritdoc/></returns>
        IEnumerator<PipeRegisters> IEnumerable<PipeRegisters>.GetEnumerator()
            => _registers.GetEnumerator();
        /// <summary><inheritdoc cref="IListSource.GetList"/></summary>
        /// <returns><inheritdoc cref="IListSource.GetList"/></returns>
        public IList GetList()
            => _registers;

        public IReadOnlyCollection<PipeRegisters> GetSnapshot()
            => new ConcurrentBag<PipeRegisters>(_registers.ToArray());

        public void Add(PipeRegisters item)
            => _registers.Add(item);
        public bool Contains(PipeRegisters item)
            => _registers.Contains(item);
        public void CopyTo(PipeRegisters[] array, int arrayIndex)
            => _registers.CopyTo(array, arrayIndex);
        public bool Remove(PipeRegisters item)
            => _registers.Remove(item);
    }
}
