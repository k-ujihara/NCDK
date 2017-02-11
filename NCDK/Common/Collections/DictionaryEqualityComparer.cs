using System.Collections.Generic;

namespace NCDK.Common.Collections
{
    /// <summary>
    /// <see langword="null"/> key and value is not supported.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="V"></typeparam>
    public class DictionaryEqualityComparer<T, V> : IEqualityComparer<IDictionary<T, V>>
    {
        public bool Equals(IDictionary<T, V> x, IDictionary<T, V> y)
        {
            if (x.Count != y.Count)
                return false;
            foreach (var xkey in x.Keys)
            {
                if (!y.ContainsKey(xkey))
                    return false;
                if (!x[xkey].Equals(y[xkey]))
                    return false;
            }
            return true;
        }

        public int GetHashCode(IDictionary<T, V> obj)
        {
            int hash = obj.Count;
            foreach (var key in obj.Keys)
            {
                hash *= 17;
                hash += key.GetHashCode();
                hash *= 17;
                hash += obj[key].GetHashCode();
            }
            return hash;
        }
    }
}
