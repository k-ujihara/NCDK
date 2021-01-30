using System;
using System.Collections.Generic;

namespace NCDK.Common.Collections
{
    public class ArrayDeque<T> : Deque<T>
    { }

    public class Deque<T> : List<T>
    {
        public void Push(T e)
        {
            Add(e);
        }

        public T Pop()
        {
            var ret = Peek();
            RemoveAt(Count - 1);
            return ret;
        }

        public T Peek() => this[Count - 1];

        public T Poll()
        {
            if (Count == 0)
                throw new Exception();
            var ret = this[0];
            RemoveAt(0);
            return ret;
        }
    }
}
