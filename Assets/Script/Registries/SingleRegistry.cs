namespace Project
{
    public class SingleRegistry<T>
    {
        private T _value;
        private bool _hasValue = false;

        public T Value => _value;

        public bool HasValue => _hasValue;

        public void Set(T value)
        {
            _value = value;
            _hasValue = true;
        }

        public void Unset(T value)
        {
            if (_value.Equals(value))
            {
                _hasValue = false;
                _value = default;
            }
        }
    }
}