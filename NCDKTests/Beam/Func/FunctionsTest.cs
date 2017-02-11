using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Beam
{
    /// <author>John May </author>
    [TestClass()]
    public class FunctionsTest
    {
        [TestMethod()]
        public void reverse_ethanol()
        {
            Graph g = Graph.FromSmiles("CCO");
            Assert.AreEqual("OCC", Functions.Reverse(g).ToSmiles());
        }

        [TestMethod()]
        public void reverse_withBranch()
        {
            Graph g = Graph.FromSmiles("CC(CC(CO)C)CCO");
            Assert.AreEqual("OCCC(CC(C)CO)C", Functions.Reverse(g).ToSmiles());
        }

        [TestMethod()]
        public void AtomBasedDBStereo()
        {
            Graph g = Graph.FromSmiles("F/C=C/F");
            Assert.AreEqual("F[C@H]=[C@@H]F", Functions.AtomBasedDBStereo(g).ToSmiles());
        }


        [TestMethod()]
        public void BondBasedDBStereo()
        {
            Graph g = Graph.FromSmiles("F[C@H]=[C@@H]F");
            Assert.AreEqual("F/C=C/F", Functions.BondBasedDBStereo(g).ToSmiles());
        }

        [TestMethod()]
        public void EnsureAlleneStereoDoesntBreakConversion()
        {
            Graph g = Graph.FromSmiles("CC=[C@]=CC");
            Assert.AreEqual("CC=[C@]=CC", Functions.BondBasedDBStereo(g).ToSmiles());
        }

        [TestMethod()]
        public void Canoncalise()
        {
            Graph g = Graph.FromSmiles("CCOCC");
            Graph h = Functions.Canonicalize(g,
                                             new long[] { 56, 67, 3, 67, 56 });
            Assert.AreEqual("O(CC)CC", h.ToSmiles());
        }

        [TestMethod()]
        public void Canoncalise2()
        {
            Graph g = Graph.FromSmiles("CN1CCC1");
            Graph h = Functions.Canonicalize(g,
                                             new long[] { 2, 1, 3, 5, 4 });
            Assert.AreEqual("N1(C)CCC1", h.ToSmiles());
        }
    }
}
