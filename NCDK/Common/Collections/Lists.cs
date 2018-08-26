using System;
using System.Collections;
using System.Collections.Generic;

namespace NCDK.Common.Collections
{
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
