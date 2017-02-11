using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace NCDK.Beam
{
    /// <summary> <author>John May </author>*/
    [TestClass()]
    public class IntSetTest
    {
        [TestMethod()]
        public void Universe()
        {
            IntSet universe = IntSet.Universe;
            Random rnd = new Random();
            for (int i = 0; i < 1000; i++)
            {
                Assert.IsTrue(universe.Contains(rnd.Next()));
            }
        }

        [TestMethod()]
        public void IsEmpty()
        {
            IntSet empty = IntSet.IsEmpty;
            Random rnd = new Random();
            for (int i = 0; i < 1000; i++)
            {
                Assert.IsFalse(empty.Contains(rnd.Next()));
            }
        }

        [TestMethod()]
        public void Singleton()
        {
            IntSet one = IntSet.AllOf(1);
            Assert.IsFalse(one.Contains(0));
            Assert.IsTrue(one.Contains(1));
            Assert.IsFalse(one.Contains(2));
            Assert.IsFalse(one.Contains(3));
            Assert.IsFalse(one.Contains(4));
            Assert.IsFalse(one.Contains(5));
        }

        [TestMethod()]
        public void AllOf()
        {
            IntSet one = IntSet.AllOf(4, 2);
            Assert.IsFalse(one.Contains(0));
            Assert.IsFalse(one.Contains(1));
            Assert.IsTrue(one.Contains(2));
            Assert.IsFalse(one.Contains(3));
            Assert.IsTrue(one.Contains(4));
            Assert.IsFalse(one.Contains(5));
        }

        [TestMethod()]
        public void NoneOf()
        {
            IntSet one = IntSet.NoneOf(0, 1, 3, 5);
            Assert.IsFalse(one.Contains(0));
            Assert.IsFalse(one.Contains(1));
            Assert.IsTrue(one.Contains(2));
            Assert.IsFalse(one.Contains(3));
            Assert.IsTrue(one.Contains(4));
            Assert.IsFalse(one.Contains(5));
            Assert.IsTrue(one.Contains(6));
        }
    }
}
