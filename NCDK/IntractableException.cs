namespace NCDK
{
    public sealed class IntractableException
        : CDKException
    {
        public IntractableException(string message)
            : base(message)
        {
        }

        public IntractableException(long t)
            : this("Operation", t)
        {
        }

        public IntractableException(string desc, long t)
            : this(desc + " did not finish after " + t + " ms.")
        {
        }
    }
}

