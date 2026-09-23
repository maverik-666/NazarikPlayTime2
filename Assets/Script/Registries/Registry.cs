namespace Project
{
    using System.Collections.Generic;

    public class Registry<T>
    {
        private readonly List<T> _list;

        public IReadOnlyList<T> Items => _list;

        public int Count => _list.Count;

        public Registry(int capacity)
        {
            _list = new List<T>(capacity);
        }

        public void AddRange(T[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                _list.Add(array[i]);
            }
        }

        public void Add(T item)
        {
            if (_list.Contains(item))
            {
                return;
            }

            _list.Add(item);
        }
        
        public void Remove(T item)
        {
            _list.Remove(item);
        }
        
        public void RemoveRange(T[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                _list.Remove(array[i]);
            }
        }

        public void Clear()
        {
            _list.Clear();
        }
        
    }
}