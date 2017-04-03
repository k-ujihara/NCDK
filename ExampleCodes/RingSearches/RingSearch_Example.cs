using NCDK.Templates;
using System;

namespace NCDK.RingSearches
{
    class RingSearch_Example
    {
        static void Main(string[] args)
        {
            {
                #region 
                // construct the search for a given molecule, if an adjacency list
                // representation (int[][]) is available this can be passed to the
                // constructor for improved performance
                IAtomContainer container = TestMoleculeFactory.MakeAlphaPinene();
                RingSearch ringSearch = new RingSearch(container);

                // indices of cyclic vertices
                int[] cyclic = ringSearch.Cyclic();

                // iterate over fused systems (atom indices)
                foreach (int[] fused in ringSearch.Fused())
                {
                    // ...
                }

                // iterate over isolated rings (atom indices)
                foreach (int[] isolated in ringSearch.Isolated())
                {
                    // ...
                }

                // convenience methods for getting the fragments
                IAtomContainer fragments = ringSearch.RingFragments();

                foreach (IAtomContainer fragment in ringSearch.FusedRingFragments())
                {
                    // ...
                }
                foreach (IAtomContainer fragment in ringSearch.IsolatedRingFragments())
                {
                    // ...
                }
                #endregion
            }
            {
                #region Cyclic
                IAtomContainer mol = TestMoleculeFactory.MakeAlphaPinene();
                RingSearch ringSearch = new RingSearch(mol);
                foreach (var atom in mol.Atoms)
                {
                    if (ringSearch.Cyclic(atom))
                    {
                        // ...
                    }
                }
                #endregion
            }
            {
                #region Cyclic_int
                IAtomContainer mol = TestMoleculeFactory.MakeAlphaPinene();
                RingSearch tester = new RingSearch(mol);

                int n = mol.Atoms.Count;
                for (int i = 0; i < n; i++)
                {
                    if (tester.Cyclic(i))
                    {
                        // ...
                    }
                }
                #endregion
            }
            {
                #region Isolated
                IAtomContainer biphenyl = TestMoleculeFactory.MakeBiphenyl();
                RingSearch ringSearch = new RingSearch(biphenyl);

                int[][] isolated = ringSearch.Isolated();
                Console.WriteLine(isolated.Length); // 2 isolated rings in biphenyl
                Console.WriteLine(isolated[0].Length); // 6 vertices in one benzene
                Console.WriteLine(isolated[1].Length); // 6 vertices in the other benzene
                #endregion
            }
            if (true)
            {
                #region Fused
                IAtomContainer mol = new Smiles.SmilesParser(Default.ChemObjectBuilder.Instance).ParseSmiles("c1cc(cc2cc(ccc12)C3C4CC34)C6CC5CCC6(C5)");
                RingSearch ringSearch = new RingSearch(mol);
                
                int[][] fused = ringSearch.Fused();
                Console.WriteLine(fused.Length); // e.g. 3 separate fused ring systems
                Console.WriteLine(fused[0].Length); // e.g. 10 vertices in the first system
                Console.WriteLine(fused[1].Length); // e.g. 4 vertices in the second system
                Console.WriteLine(fused[2].Length); // e.g. 7 vertices in the third system
                #endregion
            }
        }
    }
}
