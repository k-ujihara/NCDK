
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace NCDK.Common.Collections
{
    public sealed class EmptyEnumerator<T> : IEnumerator<T>
    {
        public T Current { get { throw new System.InvalidOperationException(); } }
        object IEnumerator.Current { get { throw new System.InvalidOperationException(); } }
        public void Dispose() { }
        public bool MoveNext() => false;
        public void Reset() { }
    }

    public sealed class EmptyEnumerable<T> : IEnumerable<T>
    {
        static EmptyEnumerator<T> enumerator = new EmptyEnumerator<T>();
        
        public IEnumerator<T> GetEnumerator() => enumerator;
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        

    }

    public sealed class EmptyCollection<T> : ICollection<T>
    {
        static EmptyEnumerator<T> enumerator = new EmptyEnumerator<T>();
        
        public IEnumerator<T> GetEnumerator() => enumerator;
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        

        public int Count => 0;
        public bool IsReadOnly => true;
        public void Add(T item) { throw new System.InvalidOperationException(); }
        public void Clear() { }
        public bool Contains(T item) => false;
        public void CopyTo(T[] array, int arrayIndex) { }
        public bool Remove(T item) => false;
        

	}

    public sealed class EmptySet<T> : ISet<T>
    {
        static EmptyEnumerator<T> enumerator = new EmptyEnumerator<T>();
        
        public IEnumerator<T> GetEnumerator() => enumerator;
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        

        public int Count => 0;
        public bool IsReadOnly => true;
        public void Add(T item) { throw new System.InvalidOperationException(); }
        public void Clear() { }
        public bool Contains(T item) => false;
        public void CopyTo(T[] array, int arrayIndex) { }
        public bool Remove(T item) => false;
        

        internal static EmptySet<T> instance = new EmptySet<T>();
        
        public void ExceptWith(IEnumerable<T> other) { }
        public void IntersectWith(IEnumerable<T> other) { }
        public bool IsProperSubsetOf(IEnumerable<T> other) => true;
        public bool IsProperSupersetOf(IEnumerable<T> other) => false;
        public bool IsSubsetOf(IEnumerable<T> other) => true;
        public bool IsSupersetOf(IEnumerable<T> other) => false;
        public bool Overlaps(IEnumerable<T> other) => false;
        public bool SetEquals(IEnumerable<T> other) => other.Count() == 0;
        public void SymmetricExceptWith(IEnumerable<T> other) { }
        public void UnionWith(IEnumerable<T> other) { throw new System.InvalidOperationException(); }
        bool ISet<T>.Add(T item) { throw new System.InvalidOperationException(); }
        

    }
}
