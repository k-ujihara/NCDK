/*
The MIT License (MIT)

Copyright (c) 2014 matarillo

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;

public class LinkedHashMap<TKey, TValue> : IDictionary<TKey, TValue>
{
    private readonly Dictionary<TKey, int> dict;

    private readonly List<KeyValuePair<TKey, TValue>> list;

    #region constructor

    public LinkedHashMap()
    {
        dict = new Dictionary<TKey, int>();
        list = new List<KeyValuePair<TKey, TValue>>();
    }

    public LinkedHashMap(IEqualityComparer<TKey> comparer)
    {
        dict = new Dictionary<TKey, int>(comparer);
        list = new List<KeyValuePair<TKey, TValue>>();
    }

    public LinkedHashMap(int capacity)
    {
        dict = new Dictionary<TKey, int>(capacity);
        list = new List<KeyValuePair<TKey, TValue>>(capacity);
    }

    public LinkedHashMap(int capacity, IEqualityComparer<TKey> comparer)
    {
        dict = new Dictionary<TKey, int>(capacity, comparer);
        list = new List<KeyValuePair<TKey, TValue>>(capacity);
    }

    public LinkedHashMap(IEnumerable<KeyValuePair<TKey, TValue>> source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        if (source is System.Collections.ICollection countable)
        {
            dict = new Dictionary<TKey, int>(countable.Count);
            list = new List<KeyValuePair<TKey, TValue>>(countable.Count);
        }
        else
        {
            dict = new Dictionary<TKey, int>();
            list = new List<KeyValuePair<TKey, TValue>>();
        }
        foreach (var pair in source)
        {
            this[pair.Key] = pair.Value;
        }
    }

    public LinkedHashMap(IEnumerable<KeyValuePair<TKey, TValue>> source, IEqualityComparer<TKey> comparer)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        if (source is System.Collections.ICollection countable)
        {
            dict = new Dictionary<TKey, int>(countable.Count, comparer);
            list = new List<KeyValuePair<TKey, TValue>>(countable.Count);
        }
        else
        {
            dict = new Dictionary<TKey, int>(comparer);
            list = new List<KeyValuePair<TKey, TValue>>();
        }
        foreach (var pair in source)
        {
            this[pair.Key] = pair.Value;
        }
    }

    #endregion

    #region IDictionary implementation

    public bool ContainsKey(TKey key)
    {
        return dict.ContainsKey(key);
    }

    public void Add(TKey key, TValue value)
    {
        DoAdd(key, value);
    }

    private void DoAdd(TKey key, TValue value)
    {
        var pair = new KeyValuePair<TKey, TValue>(key, value);
        list.Add(pair);
        dict.Add(key, list.Count - 1);
    }

    public bool Remove(TKey key)
    {
        int index;
        if (!dict.TryGetValue(key, out index))
        {
            return false;
        }
        DoRemove(index, key);
        return true;
    }

    private void DoRemove(int index, TKey key)
    {
        list.RemoveAt(index);
        dict.Remove(key);
        for (var i = index; i < list.Count; i++)
        {
            var pair = list[i];
            dict[pair.Key] = i;
        }
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        int index;
        if (dict.TryGetValue(key, out index))
        {
            value = list[index].Value;
            return true;
        }
        value = default(TValue);
        return false;
    }

    private int IndexOf(TKey key, TValue value)
    {
        int index;
        if (dict.TryGetValue(key, out index))
        {
            if (EqualityComparer<TValue>.Default.Equals(value, list[index].Value))
            {
                return index;
            }
        }
        return -1;
    }

    public TValue this[TKey key]
    {
        get { return list[dict[key]].Value; }
        set
        {
            int index;
            if (!dict.TryGetValue(key, out index))
            {
                DoAdd(key, value);
                return;
            }
            DoSet(index, key, value);
        }
    }

    private void DoSet(int index, TKey key, TValue value)
    {
        var pair = new KeyValuePair<TKey, TValue>(key, value);
        list[index] = pair;
    }

    public ICollection<TKey> Keys
    {
        get
        {
            return list.Select(p => p.Key).ToArray();
        }
    }

    public ICollection<TValue> Values
    {
        get
        {
            return list.Select(p => p.Value).ToArray();
        }
    }

    #endregion

    #region ICollection implementation

    public void Clear()
    {
        dict.Clear();
        list.Clear();
    }

    public int Count
    {
        get { return dict.Count; }
    }

    public bool IsReadOnly
    {
        get { return false; }
    }

    #endregion

    #region IEnumerable implementation

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return list.GetEnumerator();
    }

    #endregion

    #region IEnumerable implementation

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion

    #region explicit ICollection implementation

    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
    {
        return (IndexOf(item.Key, item.Value) >= 0);
    }

    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        list.CopyTo(array, arrayIndex);
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
    {
        var index = IndexOf(item.Key, item.Value);
        if (index < 0)
        {
            return false;
        }
        DoRemove(index, item.Key);
        return true;
    }

    #endregion
}
