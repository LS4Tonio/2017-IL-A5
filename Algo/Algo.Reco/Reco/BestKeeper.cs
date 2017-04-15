using System.Collections.Generic;

namespace Algo
{
    public static class ListExtension
    {
        public static void AddInBestKeeper<T>(this List<T> @this, T value, BestKeeper<T> bestKeeper)
        {
            bestKeeper.Add(value);
            @this.Add(value);
        }
    }

    public class BestKeeper<T>
    {
        private readonly List<T> _bestKeeper;
        private readonly IComparer<T> _comparer;
        private readonly int _length;

        public BestKeeper(int length, IComparer<T> comparer)
        {
            _bestKeeper = new List<T>();
            _comparer = comparer;
            _length = length;
        }

        /// <summary>
        /// Adds an element in the best keeper list if necessary
        /// </summary>
        /// <param name="value">Value to add</param>
        public void Add(T value)
        {
            var index = FindIndex(value);
            if (index >= 0)
            {
                _bestKeeper.Insert(index, value);
                if (_bestKeeper.Count > _length)
                {
                    _bestKeeper.RemoveAt(_length);
                }
            }
            else if (_bestKeeper.Count < _length)
            {
                _bestKeeper.Add(value);
            }
        }

        public List<T> GetBestKeeper()
        {
            return _bestKeeper;
        }

        private int FindIndex(T value)
        {
            foreach (var element in _bestKeeper)
            {
                if (_comparer.Compare(value, element) >= 0)
                {
                    return _bestKeeper.IndexOf(element);
                }
            }
            return -1;
        }
    }
}