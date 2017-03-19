using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using NCDK.Common.Base;

namespace NCDK.Beam
{
   /// <summary> <author>John May </author></summary>
    [TestClass()]
    public class MatchingTest
    {
        [TestMethod()] public void IsEmptyTest()
        {
            Matching matching = Matching.CreateEmpty(Graph.FromSmiles("CCCCC"));
            Assert.AreEqual(0, matching.GetMatches().Count());
        }

        [TestMethod()] public void Basic()
        {
            Matching matching = Matching.CreateEmpty(Graph.FromSmiles("CCCCC"));
            matching.Match(0, 1);
            matching.Match(2, 3);
            Assert.AreEqual(2, matching.GetMatches().Count());
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(
                new[] { Tuple.Of(0, 1), Tuple.Of(2, 3) },
                matching.GetMatches()));
        }

        [TestMethod()]
        public void Adjusted()
        {
            Matching matching = Matching.CreateEmpty(Graph.FromSmiles("CCCCC"));
            matching.Match(0, 1);
            matching.Match(2, 3);
            matching.Match(1, 2); // 0-1 and 2-3 should not be 

            Assert.IsFalse(Compares.AreOrderLessDeepEqual(
                new[] { Tuple.Of(0, 1), Tuple.Of(2, 3) },
                matching.GetMatches()));
            Assert.AreEqual(1, matching.GetMatches().Count());
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(
                new[] { Tuple.Of(1, 2) },
                matching.GetMatches()));
        }

        [TestMethod()] public void Adjusted_contains() {
            Matching matching = Matching.CreateEmpty(Graph.FromSmiles("CCCCC"));
            matching.Match(0, 1);
            matching.Match(2, 3);
            matching.Match(1, 2); // 0-1 and 2-3 should not be 

            Assert.IsFalse(matching.Unmatched(1));
            Assert.IsFalse(matching.Unmatched(2));
            Assert.IsTrue(matching.Unmatched(0));
            Assert.IsTrue(matching.Unmatched(3));
        }

        [TestMethod()] public void Adjusted_other() {
            Matching matching = Matching.CreateEmpty(Graph.FromSmiles("CCCCC"));
            matching.Match(0, 1);
            matching.Match(2, 3);
            matching.Match(1, 2); // 0-1 and 2-3 should not be 

            Assert.AreEqual(matching.Other(1), 2);
            Assert.AreEqual(matching.Other(2), 1);
        }

        [TestMethod()][ExpectedException(typeof(ArgumentException))]
        public void Adjusted_other_invalid() {
            Matching matching = Matching.CreateEmpty(Graph.FromSmiles("CCCCC"));
            matching.Match(0, 1);
            matching.Match(2, 3);
            matching.Match(1, 2); // 0-1 and 2-3 should not be 

            matching.Other(0);
        }
    }
}
