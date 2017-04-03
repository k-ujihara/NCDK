using static NCDK.Graphs.GraphUtil;
using NCDK.Templates;
using NCDK.RingSearches;

namespace NCDK.Graphs
{
    class RelevantCycles_Example
    {
        void Main()
        {
            {
                #region
                // using NCDK.Graphs.GraphUtil;
                IAtomContainer m = TestMoleculeFactory.MakeAnthracene();

                // compute on the whole graph
                RelevantCycles relevant = new RelevantCycles(ToAdjList(m));

                // it is much faster to compute on the separate ring systems of the molecule
                int[][] graph = ToAdjList(m);
                RingSearch ringSearch = new RingSearch(m, graph);

                // all isolated cycles are relevant
                foreach (int[] isolated in ringSearch.Isolated())
                {
                    int[] path = Cycle(graph, isolated);
                }

                // compute the relevant cycles for each system
                foreach (int[] fused in ringSearch.Fused())
                {
                    int[][] subgraph = Subgraph(graph, fused);
                    RelevantCycles relevantOfSubgraph = new RelevantCycles(subgraph);

                    foreach (int[] path in relevantOfSubgraph.GetPaths())
                    {
                        // convert the sub graph vertices back to the super graph indices
                        for (int i = 0; i < path.Length; i++)
                        {
                            path[i] = fused[path[i]];
                        }
                    }
                }
                #endregion
            }
            {
                IAtomContainer mol = null;
                #region GetPaths
                RelevantCycles relevant = new RelevantCycles(ToAdjList(mol));

                // ensure the number is manageable
                if (relevant.Count() < 100) {
                    foreach (int[] path in relevant.GetPaths())
                    {
                        // process the path
                    }
                }
                #endregion
            }
        }
    }
}
