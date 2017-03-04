using NCDK.Common.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Graphs
{
    [TestClass()]
    public class AllCyclesTests
    {
        static PrivateType AllCyclesType = new PrivateType(typeof(AllCycles));

        [TestMethod()]
        public virtual void Rank()
        {
            // given vertices based on degree
            int[][] g = new int[][]{ new int [] {0, 0, 0, 0}, // 5th
                    new int [] {0, 0, 0, 0}, // 6th
                    new int [] {0, 0, 0}, // 4th
                    new int [] {0}, // 2nd
                    new int [] {0, 0}, // 3rd
                    new int [] {} // 1st
            };
            var actual = AllCyclesType.InvokeStatic("GetRank", new object[] { g });
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 4, 5, 3, 1, 2, 0 }, actual));
        }

        [TestMethod()]
        public virtual void VerticesInOrder()
        {
            var vertices = AllCyclesType.InvokeStatic("GetVerticesInOrder", new object[] { new int[] { 4, 3, 1, 2, 0 } });
            Assert.IsTrue(Compares.AreDeepEqual(new int[] { 4, 2, 3, 1, 0 }, vertices));
        }

        [TestMethod()]
        public virtual void CompletedTest()
        {
            AllCycles ac = new AllCycles(RegularPathGraphTests.CompleteGraphOfSize(4), 4, 100);
            Assert.IsTrue(ac.Completed);
            Assert.AreEqual(7, ac.Count);
        }

        [TestMethod()]
        [Timeout(50)]
            public virtual void Impractical()
        {
            // k12 - ouch
            AllCycles ac = new AllCycles(RegularPathGraphTests.CompleteGraphOfSize(12), 12, 100);
            Assert.IsFalse(ac.Completed);
        }

        [TestMethod()]
        public virtual void K4Paths()
        {
            AllCycles ac = new AllCycles(RegularPathGraphTests.CompleteGraphOfSize(4), 4, 1000);
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[][]{
                    new[]{2, 1, 0, 2}, new[]{3, 1, 0, 3}, new[]{3, 2, 0, 3}, new[]{3, 2, 1, 3}, new[]{3, 2, 1, 0, 3},
                    new[]{3, 2, 0, 1, 3}, new[]{3, 0, 2, 1, 3}},
                ac.GetPaths()));
        }

        [TestMethod()]
        public virtual void K5Paths()
        {
            AllCycles ac = new AllCycles(RegularPathGraphTests.CompleteGraphOfSize(5), 5, 1000);
            Assert.IsTrue(Compares.AreDeepEqual(
                new int[][]{
                    new[] {2, 1, 0, 2}, new[] {3, 1, 0, 3}, new[] {4, 1, 0, 4}, new[]{3, 2, 0, 3}, new[]{3, 2, 1, 3},
                    new[]{3, 2, 1, 0, 3}, new[]{3, 2, 0, 1, 3}, new[]{4, 2, 0, 4}, new[]{4, 2, 1, 4}, new[]{4, 2, 1, 0, 4}, new[]{4, 2, 0, 1, 4},
                    new[]{3, 0, 2, 1, 3}, new[]{4, 0, 2, 1, 4}, new[]{4, 3, 0, 4}, new[]{4, 3, 1, 4}, new[]{4, 3, 1, 0, 4}, new[]{4, 3, 0, 1, 4},
                    new[]{4, 3, 2, 4}, new[]{4, 3, 2, 0, 4}, new[]{4, 3, 2, 1, 4}, new[]{4, 3, 2, 1, 0, 4}, new[]{4, 3, 2, 0, 1, 4},
                    new[]{4, 3, 0, 2, 4}, new[]{4, 3, 1, 2, 4}, new[]{4, 3, 0, 1, 2, 4}, new[]{4, 3, 1, 0, 2, 4}, new[]{4, 3, 0, 2, 1, 4},
                    new[]{4, 3, 1, 2, 0, 4}, new[]{4, 0, 3, 1, 4}, new[]{4, 0, 3, 2, 4}, new[]{4, 0, 3, 2, 1, 4}, new[]{4, 0, 3, 1, 2, 4},
                    new[]{4, 1, 3, 2, 4}, new[]{4, 1, 3, 2, 0, 4}, new[]{4, 1, 3, 0, 2, 4}, new[]{4, 0, 1, 3, 2, 4}, new[]{4, 1, 0, 3, 2, 4}},
                ac.GetPaths()));
        }

        [TestMethod()]
        public virtual void K4Size()
        {
            AllCycles ac = new AllCycles(RegularPathGraphTests.CompleteGraphOfSize(4), 4, 1000);
            Assert.AreEqual(7, ac.Count);
        }

        [TestMethod()]
        public virtual void K5Size()
        {
            AllCycles ac = new AllCycles(RegularPathGraphTests.CompleteGraphOfSize(5), 5, 1000);
            Assert.AreEqual(37, ac.Count);
        }

        [TestMethod()]
        public virtual void K6Size()
        {
            AllCycles ac = new AllCycles(RegularPathGraphTests.CompleteGraphOfSize(6), 6, 1000);
            Assert.AreEqual(197, ac.Count);
        }

        [TestMethod()]
        public virtual void K7Size()
        {
            AllCycles ac = new AllCycles(RegularPathGraphTests.CompleteGraphOfSize(7), 7, 1000);
            Assert.AreEqual(1172, ac.Count);
        }
    }
}
