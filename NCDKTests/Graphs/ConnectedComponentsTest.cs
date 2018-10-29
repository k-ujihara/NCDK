using NCDK.Common.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Graphs
{
    // @author John May
    // @cdk.module test-core
    [TestClass()]
    public class ConnectedComponentsTest
    {
        [TestMethod()]
        public virtual void Connected()
        {
            int[][] g = { new[] { 1 }, new[] { 0, 2 }, new[] { 1, 3 }, new[] { 2 } };
            Assert.AreEqual(1, new ConnectedComponents(g).NumberOfComponents);
            Assert.IsTrue(Compares.AreEqual(new int[] { 1, 1, 1, 1 }, new ConnectedComponents(g).GetComponents()));
        }

        [TestMethod()]
        public virtual void Disconnected()
        {
            int[][] g = { new[] { 1 }, new[] { 0, 2 }, new[] { 1, 3 }, new[] { 2 }, new[] { 5, 6 }, new[] { 4, 6 }, new[] { 4, 5 }, new int[] { }, new[] { 9 }, new[] { 8 } };
            Assert.AreEqual(4, new ConnectedComponents(g).NumberOfComponents);
            Assert.IsTrue(Compares.AreEqual(
                new int[] { 1, 1, 1, 1, 2, 2, 2, 3, 4, 4 },
                new ConnectedComponents(g).GetComponents()));
        }
    }
}
