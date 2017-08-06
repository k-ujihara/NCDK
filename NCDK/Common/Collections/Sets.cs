using System;
using System.Collections;
using System.Collections.Generic;

namespace NCDK.Common.Collections
{
    public class Sets
    {
        public static ISet<T> Empty<T>() => EmptySet<T>.instance;

        public static ISet<T> Wrap<T>(ICollection<T> collection)
        {
            return new WrappedSet<T>(collection);
        }

        class WrappedSet<T> : ISet<T>
        {
            ICollection<T> collection;

            public WrappedSet(ICollection<T> collection)
            {
                this.collection = collection;
            }

            public int Count => collection.Count;

            public bool IsReadOnly => true;

            public bool Add(T item) { throw new InvalidOperationException(); }

            public void Clear() { collection.Clear(); }

            public bool Contains(T item) => collection.Contains(item);

            public void CopyTo(T[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public void ExceptWith(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<T> GetEnumerator() => collection.GetEnumerator();

            public void IntersectWith(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public bool IsProperSubsetOf(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public bool IsProperSupersetOf(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public bool IsSubsetOf(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public bool IsSupersetOf(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public bool Overlaps(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public bool Remove(T item) => collection.Remove(item); 

            public bool SetEquals(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public void SymmetricExceptWith(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public void UnionWith(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            void ICollection<T>.Add(T item) { throw new InvalidOperationException(); }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
       