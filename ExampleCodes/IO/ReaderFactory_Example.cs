using System.IO;

namespace NCDK.IO
{
    class ReaderFactory_Example
    {
        void Main()
        {
            #region
            using (StringReader stringReader = new StringReader("<molecule/>"))
            using (var reader = new ReaderFactory().CreateReader(stringReader))
            {
                //
            }
            #endregion
        }
    }
}
