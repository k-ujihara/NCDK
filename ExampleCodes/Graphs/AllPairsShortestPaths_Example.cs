using NCDK.Templates;

namespace NCDK.Graphs
{
    public class AllPairsShortestPaths_Example
    {
        public void Main()
        {
            {
                #region
                IAtomContainer benzene = TestMoleculeFactory.MakeBenzene();
                AllPairsShortestPaths apsp = new AllPairsShortestPaths(benzene);
                for (int i = 0; i < benzene.Atoms.Count; i++)
                {
                    // only to half the comparisons, we can reverse the
                    // path[] to get all j to i
                    for (int j = i + 1; j < benzene.Atoms.Count; j++)
                    {
                        // reconstruct shortest path from i to j
                        int[] path = apsp.From(i).GetPathTo(j);

                        // reconstruct all shortest paths from i to j
                        int[][] paths = apsp.From(i).GetPathsTo(j);

                        // reconstruct the atoms in the path from i to j
                        IAtom[] atoms = apsp.From(i).GetAtomsTo(j);

                        // access the number of paths from i to j
                        int nPaths = apsp.From(i).GetNPathsTo(j);

                        // access the distance from i to j
                        int distance = apsp.From(i).GetNPathsTo(j);
                    }
                }
                #endregion
            }
            {
                IAtomContainer benzene = TestMoleculeFactory.MakeBenzene();
                #region From
                AllPairsShortestPaths apsp = new AllPairsShortestPaths(benzene);

                // access explicitly
                ShortestPaths sp = apsp.From(0);
                // or chain method calls
                int[] path = apsp.From(0).GetPathTo(5);
                #endregion
            }
            {
                IAtomContainer molecule = TestMoleculeFactory.MakeBenzene();
                #region From_IAtom
                AllPairsShortestPaths apsp = new AllPairsShortestPaths(molecule);
                IAtom start = molecule.Atoms[0];
                IAtom end = molecule.Atoms[1];

                // access explicitly
                ShortestPaths sp = apsp.From(start);
                
                 // or chain the method calls together
                
                 // first path from start to end atom
                 int[] path = apsp.From(start).GetPathTo(end);
                
                 // first atom path from start to end atom
                 IAtom[] atoms = apsp.From(start).GetAtomsTo(end);
                #endregion
            }
        }
    }
}
