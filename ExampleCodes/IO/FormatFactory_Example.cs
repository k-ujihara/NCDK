using NCDK.IO.Formats;
using System.IO;

namespace NCDK.IO
{
    class FormatFactory_Example
    {
        void Main()
        {
            {
                #region 
                StringReader stringReader = new StringReader("<molecule/>");
                IChemFormat format = new FormatFactory().GuessFormat(stringReader);
                #endregion
            }
        }
    }
}
