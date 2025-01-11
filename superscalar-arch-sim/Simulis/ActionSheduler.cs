using System;
using System.Collections.Generic;
using System.Linq;

namespace superscalar_arch_sim.Simulis
{
    internal class ActionSheduler
    {
        const ulong AllActions = ulong.MaxValue;

        readonly Dictionary<ulong, Queue<Action>> Shedules;

        /// <summary>Removes all sheduled <see cref="Action"/>s from invocation list.</summary>
        public void Reset() => RemoveAction(AllActions);
        
        /// <summary>Initializes a new instance of <see cref="ActionSheduler"/> class.</summary>
        public ActionSheduler() 
        {
            Shedules = new Dictionary<ulong, Queue<Action>>();
        }
        /// <summary>Checks if any <see cref="Action"/> was sheduled at <paramref name="cycle"/> and if so, invokes it and deletes from shedules. </summary>
        /// <param name="cycle">Current cycle, for which <see cref="Action"/> existance will be checked.</param>
        public void Update(ulong cycle)
        {
            if (Shedules.TryGetValue(cycle, out Queue<Action> invocationList)) 
            {
                while (invocationList.Count > 0)
                {
                    invocationList.Dequeue().Invoke();
                }
            }
        }

        /// <summary>
        /// Adds new <paramref name="action"/> to list of <see cref="Action"/>s to invoke when <see cref="Update(long)"/>
        /// is called with parameter equal to <paramref name="cycle"/>.
        /// </summary>
        /// <param name="cycle">Clock cycle at which <paramref name="action"/> should be invoked.</param>
        /// <param name="action"><see cref="Action"/> to invoke when <see cref="Update(long)"/> with <paramref name="cycle"/> is called.</param>
        public void SheduleAt(ulong cycle, Action action) {
            if (false == Shedules.ContainsKey(cycle))
                 Shedules[cycle] = new Queue<Action>();
            Shedules[cycle].Enqueue(action);
        }

        /// <summary>
        /// Removes single (or all if <paramref name="cycle"/> == -1) action(s) from <see cref="ActionSheduler"/> list.</summary>
        /// <param name="cycle">Cycle number that <see cref="Action"/> was sheduled at. -1 to clear all sheduled.</param>
        public void RemoveAction(ulong cycle)
        {
            if (cycle == ulong.MaxValue) Shedules.Clear();
            else Shedules.Remove(cycle);
        }

        public bool IsSheduled(Action action) => Shedules.Any(x => x.Value.Contains(action));
        public bool IsSheduledAt(ulong cycle) => Shedules.ContainsKey(cycle);

    }
}
