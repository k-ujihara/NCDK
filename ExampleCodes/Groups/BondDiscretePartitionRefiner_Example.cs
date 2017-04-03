using System.Linq;

namespace NCDK.Groups
{
    class BondDiscretePartitionRefiner_Example
    {
        void Main()
        {
            IAtomContainer someMolecule = null;
            {
                #region 1
                IAtomContainer ac = someMolecule; // get an atom container somehow
                BondDiscretePartitionRefiner refiner = new BondDiscretePartitionRefiner();
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
                BondDiscretePartitionRefiner refiner = new BondDiscretePartitionRefiner();
                if (refiner.IsCanonical(ac))
                {
                     // do something with the atom container
                 }
                #endregion
            }
            {
                IAtomContainer ac = someMolecule;
                #region 3
                BondDiscretePartitionRefiner refiner = new BondDiscretePartitionRefiner();
                refiner.Refine(ac);
                bool isCanon = refiner.IsCanonical();
                PermutationGroup autG = refiner.GetAutomorphismGroup();
                #endregion
            }
        }
    }
}
