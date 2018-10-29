using NCDK.Templates;
using System;

namespace NCDK.Graphs
{
    public class ConnectedComponents_Example
    {
        public void Main()
        {
            var container = TestMoleculeFactory.MakeBenzene();
            #region
            int[][] g = GraphUtil.ToAdjList(container);
            ConnectedComponents cc = new ConnectedComponents(g);
            int[] components = cc.GetComponents();
            for (int v = 0; v < g.Length; v++)
                Console.WriteLine(components[v]);
            #endregion
        }
    }
}
