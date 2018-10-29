using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCDK.IO
{
    class MDLRXNWriter_Example
    {
        void Main()
        {
            {
                IAtomContainer molecule = null;
                #region 
                using (var writer = new MDLRXNWriter(new FileStream("output.mol", FileMode.Create)))
                {
                    writer.Write(molecule);
                }
                #endregion
            }
        }
    }
}
