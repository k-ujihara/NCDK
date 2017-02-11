using System;

namespace NCDK
{
    public sealed class Intractable
        : CDKException
    {
        public Intractable(string message)
            : base(message)
        {
        }

        public Intractable(long t)
            : this("Operation", t)
        {
        }

        public Intractable(string desc, long t)
            : this(desc + " did not finish after " + t + " ms.")
        {
        }
    }
}

