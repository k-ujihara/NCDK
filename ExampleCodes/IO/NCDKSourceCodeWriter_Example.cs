using System;
using System.IO;

namespace NCDK.IO
{
    class NCDKSourceCodeWriter_Example
    {
        void Main()
        {
            {
                IAtomContainer molecule = null;
                #region 
                using (var stringWriter = new StringWriter())
                {
                    using (var writer = new NCDKSourceCodeWriter(stringWriter))
                    {
                        writer.Write(molecule);
                    }
                    Console.Out.Write(stringWriter.ToString());
                }
                #endregion
            }
        }
    }
}
