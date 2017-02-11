using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NCDK.Common.Collections
{
    public static class GenericCollections
    {
        public static IDictionary<T, V> GetEmptyDictionary<T, V>() => new ReadOnlyDictionary<T, V>(new Dictionary<T, V>());
    }
}

