namespace Project
{ 
    public class IndexedRegistrator<T>
    {
        private readonly IndexedRegistry<T> _damageableRegistry = null;
        private int _idCounter = 0;

        protected IndexedRegistrator(IndexedRegistry<T> damageableRegistry)
        {
            _damageableRegistry = damageableRegistry;
        }

        public void Register(T item)
        {
            _damageableRegistry.Add(_idCounter, item);
            _idCounter++;
        }

        public void ResetCounter()
        {
            _idCounter = 0;
        }

        public void Unregister(T item)
        {
            _damageableRegistry.Remove(item);
        }

        public bool HasDestructible(T item)
        {
            return _damageableRegistry.HasItem(item);
        }
    }
}