using System;
using System.Collections.Concurrent;

namespace Prem.PTC.Utils
{
    /// <summary>
    /// Generic class for pool of T 
    /// </summary>
    public class ObjectPool<T>
    {
        private ConcurrentBag<T> _objects;
        private Func<T> _objectGenerator;

        /// <param name="objectGenerator">Function returning object T,
        /// used to get new reusable</param>
        public ObjectPool(Func<T> objectGenerator)
        {
            if (objectGenerator == null) 
                throw new ArgumentNullException("objectGenerator cannot be null.");
            _objects = new ConcurrentBag<T>();
            _objectGenerator = objectGenerator;
        }

        public virtual T AcquireReusable()
        {
            T item = default(T);
            if (_objects.TryTake(out item)) return item;
            return _objectGenerator();
        }

        public virtual void ReleaseReusable(T item)
        {
            _objects.Add(item);
        }
    }

    public class ObjectWrapper<T>
    {
        public T Instance { get; private set; }

        public ObjectWrapper(T instance)
        {
            Instance = instance;
        }
    }
}