using System;

namespace NCDK
{
    public class CDKException
        : Exception
    {
        public CDKException(string message)
            : base(message)
        {
        }

        public CDKException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
