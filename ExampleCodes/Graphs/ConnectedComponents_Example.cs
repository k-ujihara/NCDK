using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Templates;
using System;

namespace NCDK.Graphs
{
    [TestClass]
    public class ConnectedComponents_Example
    {
        [TestMethod]
        public void Main()
        {
            var container = TestMoleculeFactory.MakeBenzene();
            #region
            int[][] g = GraphUtil.ToAdjList(container);
            ConnectedComponents cc = new ConnectedComponents(g);
            int[] components = cc.Components();
            for (int v = 0; v < g.Length; v++)
                Console.WriteLine(components[v]);
            #endregion
        }
    }
}
