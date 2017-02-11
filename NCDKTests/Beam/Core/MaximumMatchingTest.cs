using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using NCDK.Common.Base;

namespace NCDK.Beam
{
    /// <summary> <author>John May </author>*/
    [TestClass()]
    public class MaximumMatchingTest
    {
        /// <summary>Contrived example to test blossoming.</summary>
        [TestMethod()] public void Blossom() {

            Graph g = Graph.FromSmiles("CCCCCC1CCCC1CC");
            Matching m = Matching.Empty(g);

            // initial matching from double-bonds (size = 5) 
            m.Match(1, 2);
            m.Match(3, 4);
            m.Match(5, 6);
            m.Match(7, 8);
            m.Match(9, 10);

            MaximumMatching.Maximise(g, m, 10);

            // once maximised the matching has been augmented such that there
            // are now six disjoint edges (only possibly by contracting blossom)
            Assert.AreEqual(6, m.GetMatches().Count());
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(
                new[] {
                    Tuple.Of(0, 1),
                    Tuple.Of(2, 3),
                    Tuple.Of(4, 5),
                    Tuple.Of(6, 7),
                    Tuple.Of(8, 9),
                    Tuple.Of(10, 11), },
                m.GetMatches()));
        }

        [TestMethod()]
        public void Simple_maximal()
        {
            Graph g = Graph.FromSmiles("cccc");
            Matching m = MaximumMatching.Maximal(g);
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(
                new[] { Tuple.Of(0, 1), Tuple.Of(2, 3) },
                m.GetMatches()));
        }

        [TestMethod()] public void Simple_augment() {
            Graph g = Graph.FromSmiles("cccc");
            Matching m = Matching.Empty(g);
            m.Match(1, 2);
            MaximumMatching.Maximise(g, m, 2);
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(
                 new[] { Tuple.Of(0, 1), Tuple.Of(2, 3) },
                 m.GetMatches()));
        }

        [TestMethod()] public void Simple_augment_subset() {
            Graph g = Graph.FromSmiles("cccc");
            Matching m = Matching.Empty(g);
            m.Match(1, 2);
            // no vertex '3' matching can not be improved
            MaximumMatching.Maximise(g, m, 2, IntSet.AllOf(0, 1, 2));
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(
                 new[] { Tuple.Of(1, 2) },
                 m.GetMatches()));
        }

        [TestMethod()]
        public void Furan()
        {
            Graph g = Graph.FromSmiles("o1cccc1");
            IntSet s = IntSet.AllOf(1, 2, 3, 4); // exclude the oxygen
            Matching m = Matching.Empty(g);
            MaximumMatching.Maximise(g, m, 0, s);
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(
                 new[] { Tuple.Of(1, 2), Tuple.Of(3, 4) },
                 m.GetMatches()));
        }

        [TestMethod()] public void Furan_augment() {
            Graph g = Graph.FromSmiles("o1cccc1");
            IntSet s = IntSet.AllOf(1, 2, 3, 4); // exclude the oxygen
            Matching m = Matching.Empty(g);
            m.Match(2, 3);
            MaximumMatching.Maximise(g, m, 2, s);
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(
                 new[] { Tuple.Of(1, 2), Tuple.Of(3, 4) },
                 m.GetMatches()));
        }

        [TestMethod()] public void quinone() {
            Graph g = Graph.FromSmiles("oc1ccc(o)cc1");
            Matching m = MaximumMatching.Maximal(g);
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(
                new[] {
                    Tuple.Of(0, 1),
                    Tuple.Of(2, 3),
                    Tuple.Of(4, 5),
                    Tuple.Of(6, 7), },
                m.GetMatches()));
        }

        [TestMethod()]
        public void quinone_subset()
        {
            Graph g = Graph.FromSmiles("oc1ccc(o)cc1");
            // mocks the case where the oxygen atoms are already double bonded - we
            // therefore don't include those of the adjacent carbons in the vertex
            // subset to be matched
            Matching m = Matching.Empty(g);
            MaximumMatching.Maximise(g, m, 0, IntSet.AllOf(2, 3, 6, 7));
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(
                new[] {
                    Tuple.Of(2, 3),
                    Tuple.Of(6, 7), },
                m.GetMatches()));
        }

        [TestMethod()]
        public void Napthalene_augment()
        {
            Graph g = Graph.FromSmiles("C1C=CC2=CCC=CC2=C1");
            Matching m = Matching.Empty(g);
            m.Match(1, 2);
            m.Match(3, 4);
            m.Match(6, 7);
            m.Match(8, 9);
            MaximumMatching.Maximise(g, m, 8);
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(
                new[] {
                    Tuple.Of(0, 1),
                    Tuple.Of(2, 3),
                    Tuple.Of(4, 5),
                    Tuple.Of(6, 7),
                    Tuple.Of(8, 9), },
                m.GetMatches()));
        }

        [TestMethod()] public void azulene() {
            Graph g = Graph.FromSmiles("C1CC2CCCCCC2C1");
            Matching m = MaximumMatching.Maximal(g);
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(
               new[] {
                    Tuple.Of(0, 1),
                    Tuple.Of(2, 3),
                    Tuple.Of(4, 5),
                    Tuple.Of(6, 7),
                    Tuple.Of(8, 9), },
               m.GetMatches()));
        }

        [TestMethod()] public void imidazole() {
            Graph g = Graph.FromSmiles("[nH]1ccnc1");
            Matching m = Matching.Empty(g);
            MaximumMatching.Maximise(g,
                                     m,
                                     0,
                                     IntSet.AllOf(1, 2, 3, 4)); // not the 'nH'
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(
                 new[] { Tuple.Of(1, 2), Tuple.Of(3, 4) },
                 m.GetMatches()));
        }

        [TestMethod()] public void Benzimidazole() {
            Graph g = Graph.FromSmiles("c1nc2ccccc2[nH]1");
            Matching m = Matching.Empty(g);
            MaximumMatching.Maximise(g,
                                     m,
                                     0,
                                     IntSet.NoneOf(8)); // not the 'nH'
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(
                new[] {
                    Tuple.Of(0, 1),
                    Tuple.Of(2, 3),
                    Tuple.Of(4, 5),
                    Tuple.Of(6, 7), },
                m.GetMatches()));
        }
    }
}
