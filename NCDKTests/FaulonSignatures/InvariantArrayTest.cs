using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Common.Base;
using System.Collections.Generic;

namespace NCDK.FaulonSignatures
{
    [TestClass()]
    public class InvariantArrayTest
    {
        public List<InvariantArray> MakeUnsortedList()
        {
            int[] a = { 1, 1, 2, 3 };
            int[] b = { 2, 2, 2, 3 };
            int[] c = { 1, 2, 2, 3 };
            int[] d = { 1, 2, 2, 2 };
            int[] e = { 1, 1, 2, 3 };
            List<InvariantArray> list = new List<InvariantArray>();
            list.Add(new InvariantArray(a, 0));
            list.Add(new InvariantArray(b, 1));
            list.Add(new InvariantArray(c, 2));
            list.Add(new InvariantArray(d, 3));
            list.Add(new InvariantArray(e, 4));
            return list;
        }

        [TestMethod()]
        public void TestSort()
        {
            List<InvariantArray> list = MakeUnsortedList();
            list.Sort();
            Assert.AreEqual(list[0].originalIndex, 0);
            Assert.AreEqual(list[1].originalIndex, 4);
            Assert.AreEqual(list[2].originalIndex, 3);
            Assert.AreEqual(list[3].originalIndex, 2);
            Assert.AreEqual(list[4].originalIndex, 1);
        }

        [TestMethod()]
        public void TestRank()
        {
            List<InvariantArray> list = MakeUnsortedList();
            list.Sort();
            int rank = 1;
            int[] ranks = new int[list.Count];
            ranks[0] = 1;
            for (int i = 1; i < list.Count; i++)
            {
                InvariantArray a = list[i - 1];
                InvariantArray b = list[i];
                if (!a.Equals(b))
                {
                    rank++;
                }
                ranks[i] = rank;
            }
            int[] expecteds = { 1, 1, 2, 3, 4 };
            Assert.IsTrue(Compares.AreEqual(expecteds, ranks));
        }
    }
}

