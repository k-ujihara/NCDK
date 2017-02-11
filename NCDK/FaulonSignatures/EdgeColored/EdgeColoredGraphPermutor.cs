using FaulonSignatures;
using System.Collections.Generic;
using System.Collections;

namespace FaulonSignatures.EdgeColored
{
    public class EdgeColoredGraphPermutor : Permutor, IEnumerable<EdgeColoredGraph>
    {
        private EdgeColoredGraph graph;

        public EdgeColoredGraphPermutor(EdgeColoredGraph graph)
            : base(graph.GetVertexCount())
        {
            this.graph = graph;
        }

        public IEnumerator<EdgeColoredGraph> GetEnumerator()
        {
            while (base.HasNext())
            {
                int[] nextPermutation = base.GetNextPermutation();
                yield return new EdgeColoredGraph(graph, nextPermutation);
            }
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
