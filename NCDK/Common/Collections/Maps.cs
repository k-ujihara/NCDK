using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NCDK.Common.Collections
{
    public static class GenericCollections
    {
        public static IDictionary<T, V> GetEmptyDictionary<T, V>() => new ReadOnlyDictionary<T, V>(new Dictionary<T, V>());
        public static ISet<T> GetEmptySet<T>() => new EmptySet<T>();

        class EmptySet<T> : ISet<T>
        {
            public int Count => 0;
            public bool IsReadOnly => true;
            public bool Add(T item) { throw new InvalidOperationException(); }
            public void Clear() { }
            public bool Contains(T item) => false;
            public void CopyTo(T[] array, int arrayIndex) { }
            public void ExceptWith(IEnumerable<T> other) { }
            public IEnumerator<T> GetEnumerator() => ((IList<T>)Array.Empty<T>()).GetEnumerator();
            public void IntersectWith(IEnumerable<T> other) { }
            public bool IsProperSubsetOf(IEnumerable<T> other) => !other.Any();
            public bool IsProperSupersetOf(IEnumerable<T> other) => !other.Any();
            public bool IsSubsetOf(IEnumerable<T> other) => !other.Any();
            public bool IsSupersetOf(IEnumerable<T> other) => !other.Any();
            public bool Overlaps(IEnumerable<T> other) => !other.Any();
            public bool Remove(T item) { throw new InvalidOperationException(); }
            public bool SetEquals(IEnumerable<T> other) => !other.Any();
            public void SymmetricExceptWith(IEnumerable<T> other) { }
            public void UnionWith(IEnumerable<T> other) { if (other.Any()) throw new InvalidOperationException(); }
            void ICollection<T>.Add(T item) { throw new InvalidOperationException(); }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
