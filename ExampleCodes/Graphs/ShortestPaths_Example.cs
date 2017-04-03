using NCDK.Default;
using NCDK.Templates;
using System;

namespace NCDK.Graphs
{
    class ShortestPaths_Example
    {
        void Main()
        {
            {
                #region
                IAtomContainer benzene = TestMoleculeFactory.MakeBenzene();

                IAtom c1 = benzene.Atoms[0];
                IAtom c4 = benzene.Atoms[3];

                // shortest paths from C1
                ShortestPaths sp = new ShortestPaths(benzene, c1);

                // number of paths from C1 to C4
                int nPaths = sp.GetNPathsTo(c4);

                // distance between C1 to C4
                int distance = sp.GetDistanceTo(c4);

                // reconstruct a path to the C4 - determined by storage order
                int[] path = sp.GetPathTo(c4);

                // reconstruct all paths
                int[][] paths = sp.GetPathsTo(c4);
                int[] org = paths[0];  // paths[0] == path
                int[] alt = paths[1];
                #endregion
            }
            {
                IAtomContainer benzene = TestMoleculeFactory.MakeBenzene();
                IAtom c1 = benzene.Atoms[0];

                #region GetPathTo_int
                ShortestPaths sp = new ShortestPaths(benzene, c1);

                // reconstruct first path
                int[] path = sp.GetPathTo(5);

                // check there is only one path
                if (sp.GetNPathsTo(5) == 1)
                {
                    path = sp.GetPathTo(5); // reconstruct the path
                }
                #endregion
            }
            {
                IAtomContainer benzene = TestMoleculeFactory.MakeBenzene();
                IAtom c1 = benzene.Atoms[0];

                #region GetPathTo_IAtom
                ShortestPaths sp = new ShortestPaths(benzene, c1);
                IAtom end = benzene.Atoms[3];

                // reconstruct first path
                int[] path = sp.GetPathTo(end);

                // check there is only one path
                if (sp.GetNPathsTo(end) == 1)
                {
                    path = sp.GetPathTo(end); // reconstruct the path
                }
                #endregion
            }
            {
                IAtomContainer benzene = TestMoleculeFactory.MakeBenzene();
                IAtom c1 = benzene.Atoms[0];

                #region GetPathsTo_int
                int threshold = 20;
                ShortestPaths sp = new ShortestPaths(benzene, c1);

                // reconstruct shortest paths
                int[][] paths = sp.GetPathsTo(5);

                // only reconstruct shortest paths below a threshold
                if (sp.GetNPathsTo(5) < threshold)
                {
                    int[][] path = sp.GetPathsTo(5); // reconstruct shortest paths
                }
                #endregion
            }
            {
                IAtomContainer benzene = TestMoleculeFactory.MakeBenzene();
                IAtom c1 = benzene.Atoms[0];

                #region GetPathsTo_IAtom
                int threshold = 20;
                ShortestPaths sp = new ShortestPaths(benzene, c1);
                IAtom end = benzene.Atoms[3];

                // reconstruct all shortest paths
                int[][] paths = sp.GetPathsTo(end);

                // only reconstruct shortest paths below a threshold
                if (sp.GetNPathsTo(end) < threshold)
                {
                    paths = sp.GetPathsTo(end); // reconstruct shortest paths
                }
                #endregion
            }
            {
                IAtomContainer benzene = TestMoleculeFactory.MakeBenzene();
                IAtom c1 = benzene.Atoms[0];

                #region GetAtomsTo_int
                ShortestPaths sp = new ShortestPaths(benzene, c1);

                // reconstruct a shortest path
                IAtom[] path = sp.GetAtomsTo(5);

                // ensure single shortest path
                if (sp.GetNPathsTo(5) == 1)
                {
                    path = sp.GetAtomsTo(5); // reconstruct shortest path
                }
                #endregion
            }
            {
                IAtomContainer benzene = TestMoleculeFactory.MakeBenzene();
                IAtom c1 = benzene.Atoms[0];

                #region GetAtomsTo_IAtom
                ShortestPaths sp = new ShortestPaths(benzene, c1);
                IAtom end = benzene.Atoms[3];

                // reconstruct a shortest path
                IAtom[] path = sp.GetAtomsTo(end);

                // ensure single shortest path
                if (sp.GetNPathsTo(end) == 1)
                {
                    path = sp.GetAtomsTo(end); // reconstruct shortest path
                }
                #endregion
            }
            {
                IAtomContainer benzene = TestMoleculeFactory.MakeBenzene();
                IAtom c1 = benzene.Atoms[0];

                #region GetNPathsTo_int
                ShortestPaths sp = new ShortestPaths(benzene, c1);

                sp.GetNPathsTo(5); // number of paths

                sp.GetNPathsTo(-1); // returns 0 - there are no paths
                #endregion
            }
            {
                IAtomContainer benzene = TestMoleculeFactory.MakeBenzene();
                IAtom c1 = benzene.Atoms[0];

                #region GetNPathsTo_IAtom
                ShortestPaths sp = new ShortestPaths(benzene, c1);
                IAtom end = benzene.Atoms[3];

                sp.GetNPathsTo(end); // number of paths

                sp.GetNPathsTo(null);           // returns 0 - there are no paths
                sp.GetNPathsTo(new Atom("C"));  // returns 0 - there are no paths
                #endregion
            }
            {
                #region GetDistanceTo_int_1
                IAtomContainer container = TestMoleculeFactory.MakeBenzene();
                IAtom c1 = container.Atoms[0];
                ShortestPaths sp = new ShortestPaths(container, c1); // start = 0

                int n = container.Atoms.Count;

                if (sp.GetDistanceTo(5) < n)
                {
                    // these is a path from 0 to 5
                }
                #endregion
            }
            {
                #region GetDistanceTo_int_2
                IAtomContainer container = TestMoleculeFactory.MakeBenzene();
                IAtom c1 = container.Atoms[0];
                ShortestPaths sp = new ShortestPaths(container, c1); // start = 0

                int[] path = sp.GetPathTo(5);
                
                 int start = path[0];
                 int end   = path[sp.GetDistanceTo(5)];                
                #endregion
            }
            {
                #region GetDistanceTo_IAtom_1
                 IAtomContainer container = TestMoleculeFactory.MakeBenzene();
                IAtom c1 = container.Atoms[0];
                ShortestPaths sp = new ShortestPaths(container, c1); // start atom
                IAtom end = container.Atoms[3];

                int n = container.Atoms.Count;
                
                 if( sp.GetDistanceTo(end) < n) {
                     // these is a path from start to end
                 }
                #endregion
            }
            {
                #region GetDistanceTo_IAtom_2
                 IAtomContainer container = TestMoleculeFactory.MakeBenzene();
                IAtom c1 = container.Atoms[0];
                ShortestPaths  sp = new ShortestPaths(container, c1); // start atom
                IAtom end = container.Atoms[3];

                IAtom[] atoms = sp.GetAtomsTo(end);
                Console.WriteLine(end == atoms[sp.GetDistanceTo(end)]); // true
                #endregion
            }
        }
    }
}
