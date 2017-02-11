using System;

namespace NCDK
{
    public class NoSuchAtomTypeException
        : CDKException
    {
        public NoSuchAtomTypeException(string message)
            : base(message)
        {
        }
    }
}
