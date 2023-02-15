using System;
using System.Collections.Generic;
using System.Linq;


namespace CommonTools
{
    /// <summary>
    ///  омпаратор, который сравнивает элементы по тому, какое место они занимаю в заранее определенной очереди.
    /// </summary>
    public class DeterminedOrderComparerer<T> : IComparer<T>
    {
        private Dictionary<T, int> _indicies;


        public DeterminedOrderComparerer(IEnumerable<T> possibleKeyInOrder)
        {
            int i = 0;
            _indicies = possibleKeyInOrder.ToDictionary(k => k, _ => i++);
        }


        public int Compare(T x, T y)
        {
            if (_indicies.TryGetValue(x, out var xIndex) &&
                _indicies.TryGetValue(y, out var yIndex))
            {
                return xIndex - yIndex;
            }

            throw new InvalidOperationException($"Can not compare '{x}' and '{y}'.");
        }
    }
}