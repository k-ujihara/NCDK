using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Common.Collections
{
    public interface IMultiDictionary<T, V>
        : IEnumerable<KeyValuePair<T, ICollection<V>>>
    {
        int Count { get; }
        void Add(T key, V value);
        bool ContainsKey(T key);
        IEnumerable<T> Keys { get; }
        IEnumerable<V> Values { get; }
        IEnumerable<V> this[T key] { get; }
        void Remove(T key, V value);
        IEnumerable<KeyValuePair<T, V>> Entries { get; }
    }

    public sealed class MultiDictionary<T, V>
        : MultiDictionaryBase<T, V>
    {
        IDictionary<T, ICollection<V>> dic = new Dictionary<T, ICollection<V>>();

        protected override IDictionary<T, ICollection<V>> BaseMap => dic;
    }

    public sealed class SortedMultiDictionary<T, V>
        : MultiDictionaryBase<T, V>
    {
        IDictionary<T, ICollection<V>> dic = new SortedDictionary<T, ICollection<V>>();

        protected override IDictionary<T, ICollection<V>> BaseMap => dic;
    }

    public abstract class MultiDictionaryBase<T, V> : IMultiDictionary<T, V>
    {
        protected abstract IDictionary<T, ICollection<V>> BaseMap { get; }

        public int Count => BaseMap.Values.Select(n => n.Count).Sum();

        public void Add(T key, V value)
        {
            ICollection<V> list;
            if (!BaseMap.TryGetValue(key, out list))
            {
                list = new HashSet<V>();
                BaseMap.Add(key, list);
            }
            list.Add(value);
        }

        public void Remove(T key, V value)
        {
            ICollection<V> list;
            if (BaseMap.TryGetValue(key, out list))
            {
                list.Remove(value);
            }
        }

        public bool ContainsKey(T key) => BaseMap.ContainsKey(key);

        public IEnumerator<KeyValuePair<T, ICollection<V>>> GetEnumerator()
            => BaseMap.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<T> Keys => BaseMap.Keys;

        public IEnumerable<V> Values
        {
            get
            {
                foreach (var v in BaseMap.Values)
                    foreach (var w in v)
                        yield return w;
                yield break;
            }
        }

        public IEnumerable<KeyValuePair<T, V>> Entries
        {
            get
            {
                foreach (var e in BaseMap)
                    foreach (var v in e.Value)
                        yield return new KeyValuePair<T, V>(e.Key, v);
                yield break;
            }
        }

        private static readonly V[] empty = new V[0];

        public IEnumerable<V> this[T key]
        {
            get
            {
                ICollection<V> v;
                if (!BaseMap.TryGetValue(key, out v))
                {
                    return empty;
                }
                return v;
            }
        }

        public void Clear()
        {
            foreach (var e in BaseMap)
                e.Value.Clear();
            BaseMap.Clear();
        }
    }
}
