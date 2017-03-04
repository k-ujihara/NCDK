using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Linq;
using NCDK.Common.Base;

namespace NCDK.Beam
{
    /// <summary> <author>John May </author>*/
    [TestClass()]
    public class ArbitraryMatchingTest
    {
        // simple example on furan (happens to be maximum)
        [TestMethod()]
        public void Furan()
        {
            Graph g = Graph.FromSmiles("o1cccc1");
            Matching m = Matching.CreateEmpty(g);
            ArbitraryMatching.Initial(g, m,
                                      AllOf(1, 2, 3, 4));
            // note this matching is maximum
            Assert.AreEqual(2, m.GetMatches().Count());
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(
                new[] { Tuple.Of(1, 2), Tuple.Of(3, 4) },
                m.GetMatches()));
        }

        // furan different Order - non maximum this time
        [TestMethod()]
        public void Furan_2()
        {
            Graph g = Graph.FromSmiles("c1ccoc1");
            Matching m = Matching.CreateEmpty(g);
            ArbitraryMatching.Initial(g,
                                      m,
                                      AllOf(0, 1, 2, 4));
            Assert.AreEqual(1, m.GetMatches().Count());
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(
                new[] { Tuple.Of(0, 1) },
                m.GetMatches()));
        }

        [TestMethod()]
        public void Benzene()
        {
            Graph g = Graph.FromSmiles("c1ccccc1");
            Matching m = Matching.CreateEmpty(g);
            ArbitraryMatching.Initial(g,
                                      m,
                                      AllOf(0, 1, 2, 3, 4, 5));
            Assert.AreEqual(3, m.GetMatches().Count());
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(
                new[] {
                    Tuple.Of(0, 1),
                    Tuple.Of(2, 3),
                    Tuple.Of(4, 5), },
                m.GetMatches()));
        }

        static BitArray AllOf(params int[] xs)
        {
            BitArray s = new BitArray(1000);    // it means big enough
            foreach (var x in xs)
                s.Set(x, true);
            return s;
        }
    }
}
