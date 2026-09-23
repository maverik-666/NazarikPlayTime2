namespace Project
{
    using System.Collections.Generic;

    public class IndexedRegistry<T>
    {
        private readonly Dictionary<int, T> _byIds;
        private readonly Dictionary<T, int> _byItems;
        
        public int Count => _byIds.Count;
        
        public IndexedRegistry(int capacity)
        {
            _byIds = new Dictionary<int, T>(capacity);
            _byItems = new Dictionary<T, int>(capacity);
        }
        
        public void Add(int id, T item)
        {
            _byIds.Add(id, item);
            _byItems.Add(item, id);
        }
        
        public void Remove(T item)
        {
            _byItems.Remove(item, out int id);
            _byIds.Remove(id);
        }

        public T GetItemBy(int id)
        {
            return _byIds[id];
        }

        public int GetIdBy(T item)
        {
            return _byItems[item];
        }

        public bool HasItem(T item)
        {
            return _byItems.ContainsKey(item);
        }
    }
}