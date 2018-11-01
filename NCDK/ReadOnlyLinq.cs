using System.Collections.Generic;

namespace NCDK
{
    internal static class ReadOnlyLinq
    {
        public static IReadOnlyList<TSource> ToReadOnlyList<TSource>(this IEnumerable<TSource> source)
        {
            return source is IReadOnlyList<TSource> 
                ? (IReadOnlyList<TSource>)source 
                : new List<TSource>(source);
        }
    }
}
