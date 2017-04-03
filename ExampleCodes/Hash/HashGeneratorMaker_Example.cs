using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCDK.Hash
{
    class HashGeneratorMaker_Example
    {
        void Main()
        {
            #region
            // simple
            IMoleculeHashGenerator generator0 = new HashGeneratorMaker().Depth(16)
                                                                       .Elemental()
                                                                       .Molecular();

            // fast
            IMoleculeHashGenerator generator1 = new HashGeneratorMaker().Depth(8)
                                                                       .Elemental()
                                                                       .Isotopic()
                                                                       .Charged()
                                                                       .Orbital()
                                                                       .Molecular();
            // comprehensive
            IMoleculeHashGenerator generator2 = new HashGeneratorMaker().Depth(32)
                                                                       .Elemental()
                                                                       .Isotopic()
                                                                       .Charged()
                                                                       .Chiral()
                                                                       .Perturbed()
                                                                       .Molecular();
            #endregion
        }
    }
}
