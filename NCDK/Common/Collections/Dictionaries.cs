using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NCDK.Common.Collections
{
    public static class Dictionaries
    {
        public static IDictionary<TKey, TValue> Empty<TKey, TValue>() => EmptyDictioary<TKey, TValue>.instance;

        class EmptyDictioary<TKey, TValue>
        {
            public static IDictionary<TKey, TValue> instance = new ReadOnlyDictionary<TKey, TValue>(new Dictionary<TKey, TValue>());
        }
    }
}
