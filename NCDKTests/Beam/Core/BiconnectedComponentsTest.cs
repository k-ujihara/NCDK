using NCDK.Common.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Beam
{
    /// <summary> <author>John May </author>*/
    [TestClass()]
    public class BiconnectedComponentsTest
    {
        [TestMethod()]
        public void Benzene()
        {
            Graph g = Graph.FromSmiles("c1ccccc1");
            BiconnectedComponents bc = new BiconnectedComponents(g);
            Assert.AreEqual(bc.Components.Count, 1);
            Assert.AreEqual(bc.Components[0].Count, 6);
        }

        [TestMethod()]
        public void Benzylbenzene()
        {
            Graph g = Graph.FromSmiles("c1ccccc1Cc1ccccc1");
            BiconnectedComponents bc = new BiconnectedComponents(g, false);
            Assert.AreEqual(12, BitArrays.Cardinality(bc.Cyclic));
        }


        [TestMethod()]
        public void Spiro()
        {
            Graph g = Graph.FromSmiles("C1CCCCC11CCCCC1");
            BiconnectedComponents bc = new BiconnectedComponents(g);
            Assert.AreEqual(bc.Components.Count, 2);
            Assert.AreEqual(bc.Components[0].Count, 6);
            Assert.AreEqual(bc.Components[0].Count, 6);
        }

        [TestMethod()]
        public void Fused()
        {
            Graph g = Graph.FromSmiles("C1=CC2=CC=CC=C2C=C1");
            BiconnectedComponents bc = new BiconnectedComponents(g);
            Assert.AreEqual(bc.Components.Count, 1);
            Assert.AreEqual(bc.Components[0].Count, 11);
        }

        [TestMethod()]
        public void Bridged()
        {
            Graph g = Graph.FromSmiles("C1CC2CCC1C2");
            BiconnectedComponents bc = new BiconnectedComponents(g);
            Assert.AreEqual(bc.Components.Count, 1);
            Assert.AreEqual(bc.Components[0].Count, 8);
        }

        [TestMethod()]
        public void Exocyclic()
        {
            Graph g = Graph.FromSmiles("[AsH]=C1C=CC=CC=C1");
            BiconnectedComponents bc = new BiconnectedComponents(g);
            Assert.AreEqual(bc.Components.Count, 1);
            Assert.AreEqual(bc.Components[0].Count, 7);
        }
    }
}
