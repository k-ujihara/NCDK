using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCDK.Common.Collections
{
    public static class Lists<T>
    {
        private static IList<T> empty = new ReadOnlyCollection<T>(new List<T>());
        public static IList<T> Empty => empty;
    }

    public static class Lists
    {
        public static int GetDeepHashCode(IEnumerable list)
        {
            int ret = 1;
            foreach (var e in list)
            {
                ret *= 31;
                if (e != null)
                {
                    ret += e is IEnumerable ? GetDeepHashCode((IEnumerable)e) : e.GetHashCode();
                }
            }
            return ret;
        }

        public static int GetHashCode<T>(IEnumerable<T> list)
        {
            int ret = 1;
            foreach (var e in list)
            {
                ret *= 31;
                ret += e == null ? 0 : e.GetHashCode();
            }
            return ret;
        }
    }
}
