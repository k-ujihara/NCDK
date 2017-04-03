using NCDK.Templates;

namespace NCDK.Graphs.Invariant
{
    class Canon_Example
    {
        public void Main()
        {
            #region
            IAtomContainer m = TestMoleculeFactory.MakeAlphaPinene();
            int[][] g = GraphUtil.ToAdjList(m);

            // obtain canon labelling
            long[] canon_labels = Canon.Label(m, g);

            // obtain symmetry classes
            long[] symmetry_labels = Canon.Symmetry(m, g);
            #endregion
        }
    }
}
