using System;
using System.Collections.Generic;

namespace Prem.PTC.Utils
{
    /// <summary>
    /// Decorator for ISet providing information about collection state basing upon hashcode
    /// </summary>
    [Serializable]
    public class StateHashableSet<T> : ISet<T>
    {
        private ISet<T> obj;


        public StateHashableSet(ISet<T> obj)
        {
            this.obj = obj;
        }


        #region ISet methods

        public bool Add(T item) { return obj.Add(item); }
        public void ExceptWith(IEnumerable<T> other) { obj.ExceptWith(other); }
        public void IntersectWith(IEnumerable<T> other) { obj.IntersectWith(other); }
        public bool IsProperSubsetOf(IEnumerable<T> other) { return obj.IsProperSubsetOf(other); }
        public bool IsProperSupersetOf(IEnumerable<T> other) { return obj.IsProperSupersetOf(other); }
        public bool IsSubsetOf(IEnumerable<T> other) { return obj.IsSubsetOf(other); }
        public bool IsSupersetOf(IEnumerable<T> other) { return obj.IsSupersetOf(other); }
        public bool Overlaps(IEnumerable<T> other) { return obj.Overlaps(other); }
        public bool SetEquals(IEnumerable<T> other) { return obj.SetEquals(other); }
        public void SymmetricExceptWith(IEnumerable<T> other) { obj.SymmetricExceptWith(other); }
        public void UnionWith(IEnumerable<T> other) { obj.UnionWith(other); }
        void ICollection<T>.Add(T item) { obj.Add(item); }
        public void Clear() { obj.Clear(); }
        public bool Contains(T item) { return obj.Contains(item); }
        public void CopyTo(T[] array, int arrayIndex) { obj.CopyTo(array, arrayIndex); }
        public int Count { get { return obj.Count; } }
        public bool IsReadOnly { get { return obj.IsReadOnly; } }
        public bool Remove(T item) { return obj.Remove(item); }
        public IEnumerator<T> GetEnumerator() { return obj.GetEnumerator(); }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return obj.GetEnumerator(); }

        #endregion ISet methods


        #region Additional methods

        public long HashState
        {
            get
            {
                long hash = 0;
                foreach (var item in obj) hash = (hash + (item.GetHashCode() * 17) % long.MaxValue) % long.MaxValue;

                return hash;
            }
        }

        #endregion Additional methods
    }
}