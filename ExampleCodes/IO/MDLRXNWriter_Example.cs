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
                using (var srm = new FileStream("output.mol", FileMode.Create))
                using (MDLRXNWriter writer = new MDLRXNWriter(srm))
                {
                    writer.Write(molecule);
                }
                #endregion
            }
        }
    }
}
