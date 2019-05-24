namespace Visualizer.Mac
{
    public class Locked<T>
    {
        private readonly object _lock = new object();
        private T _value;

        public T Get()
        {
            lock (_lock)
            {
                return _value;
            }
        }

        public void Set(T value)
        {
            lock (_lock)
            {
                _value = value;
            }
        }

        public static implicit operator T(Locked<T> locked)
        {
            return locked.Get();
        }

    }
}
