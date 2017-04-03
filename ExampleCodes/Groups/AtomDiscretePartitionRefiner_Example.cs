using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCDK.Groups
{
    class AtomDiscretePartitionRefiner_Example
    {
        void Main()
        {
            IAtomContainer someMolecule = null;
            {
                #region 1
                IAtomContainer ac = someMolecule; // get an atom container somehow
                AtomDiscretePartitionRefiner refiner = new AtomDiscretePartitionRefiner();
                PermutationGroup autG = refiner.GetAutomorphismGroup(ac);
                foreach (var automorphism in autG.GenerateAll())
                {
                    // do something with the permutation
                }
                #endregion
            }
            {
                #region 2
                IAtomContainer ac = someMolecule; // get an atom container somehow
                AtomDiscretePartitionRefiner refiner = new AtomDiscretePartitionRefiner();
                if (refiner.IsCanonical(ac))
                {
                    // do something with the atom container
                }
                #endregion
            }
            {
                IAtomContainer ac = someMolecule;
                #region 3
                AtomDiscretePartitionRefiner refiner = new AtomDiscretePartitionRefiner();
                refiner.Refine(ac);
                bool isCanon = refiner.IsCanonical();
                PermutationGroup autG = refiner.GetAutomorphismGroup();
                #endregion
            }
        }
    }
}
