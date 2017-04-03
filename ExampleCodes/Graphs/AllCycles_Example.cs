using NCDK.RingSearches;
using NCDK.Templates;

namespace NCDK.Graphs
{
    class AllCycles_Example
    {
        public static void Main(string[] args)
        {
            // convert the molecule to adjacency list - may be redundant in future
            IAtomContainer m = TestMoleculeFactory.MakeAlphaPinene();
            int[][] g = GraphUtil.ToAdjList(m);

            // efficient computation/partitioning of the ring systems
            RingSearch rs = new RingSearch(m, g);

            // isolated cycles don't need to be run
            rs.Isolated();

            // process fused systems separately
            foreach (var fused in rs.Fused())
            {
                const int maxDegree = 100;
                // given the fused subgraph, max cycle size is
                // the number of vertices
                AllCycles ac = new AllCycles(GraphUtil.Subgraph(g, fused), fused.Length, maxDegree);
                // cyclic walks
                int[][] paths = ac.GetPaths();
            }
        }
    }
}
