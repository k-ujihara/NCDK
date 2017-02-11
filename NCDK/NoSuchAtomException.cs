using System;

namespace NCDK
{
    public class NoSuchAtomException
        : CDKException
    {
        public NoSuchAtomException(string message)
            : base(message)
        {
        }
    }
}
