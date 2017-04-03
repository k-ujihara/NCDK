using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Templates;

namespace NCDK.Graphs
{
    [TestClass]
    public class GraphUtil_Example
    {
        [TestMethod]
        public void Main()
        {
            var naphthalene = TestMoleculeFactory.MakeNaphthalene();
            #region Subgraph
            int[][] g = GraphUtil.ToAdjList(naphthalene);
            int[] vs = new int[] { 0, 1, 2, 3, 4, 5 };

            int[][] h = GraphUtil.Subgraph(g, vs);
            // for the vertices in h, the provided 'vs' gives the original index
            for (int v = 0; v < h.Length; v++)
            {
                // vs[v] is 'v' in 'g'
            }
            #endregion
        }
    }
}
