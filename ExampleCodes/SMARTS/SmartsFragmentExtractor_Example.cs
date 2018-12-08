using NCDK.Smiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCDK.SMARTS
{
    class SmartsFragmentExtractor_Example
    {
        static void Main()
        {
            #region
            var smipar = new SmilesParser();

            IAtomContainer mol = smipar.ParseSmiles("[nH]1ccc2c1cccc2");
            var subsmarts = new SmartsFragmentExtractor(mol);

            string smarts;
            // smarts=[nH1v3X3+0][cH1v4X3+0][cH1v4X3+0][cH0v4X3+0]
            // hits  =1
            smarts = subsmarts.Generate(new int[]{0,1,3,4});

            subsmarts.SetMode(SubstructureSelectionMode.JCompoundMapper);
            // smarts=n(ccc(a)a)a
            // hits  = 0 - one of the 'a' atoms needs to match the nitrogen
            smarts = subsmarts.Generate(new int[]{0,1,3,4});
            #endregion
        }
    }
}
