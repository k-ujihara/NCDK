using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace NCDK.Fingerprint
{
    [TestClass()]
    public class IntArrayCountFingerprintTest
    {

        [TestMethod()]
        public void TestMerge()
        {
            IntArrayCountFingerprint fp1 = new IntArrayCountFingerprint(new Dictionary<string, int>() {
                { "A", 1 },
                { "B", 2 },
                { "C", 3 },
			});

            IntArrayCountFingerprint fp2 = new IntArrayCountFingerprint(new Dictionary<string, int>() {
				{ "A", 1 },
                { "E", 2 },
                { "F", 3 },
			});

            IDictionary<int, int> hashCounts = new Dictionary<int, int>();
            for (int i = 0; i < fp1.GetNumOfPopulatedbins(); i++)
            {
                hashCounts[fp1.GetHash(i)] = fp1.GetCount(i);
            }
            for (int i = 0; i < fp2.GetNumOfPopulatedbins(); i++)
            {
                int hash = fp2.GetHash(i);
                int count;
                if (!hashCounts.TryGetValue(hash, out count))
                {
                    count = 0;
                }
                hashCounts[hash] = count + fp2.GetCount(i);
            }

            fp1.Merge(fp2);

            Assert.AreEqual(fp1.GetNumOfPopulatedbins(), hashCounts.Count);

            for (int i = 0; i < fp1.GetNumOfPopulatedbins(); i++)
            {
                int hash = fp1.GetHash(i);
                int count = fp1.GetCount(i);
                Assert.IsTrue(hashCounts.ContainsKey(hash));
                Assert.AreEqual(count, hashCounts[hash]);
            }

            int Aindex = Array.BinarySearch(fp1.hitHashes, "A".GetHashCode());
            Assert.IsTrue(Aindex >= 0, "A should be in the fingerprint");
            Assert.AreEqual(fp1.numOfHits[Aindex], 2);
            int Cindex = Array.BinarySearch(fp1.hitHashes, "C".GetHashCode());
            Assert.IsTrue(Cindex >= 0, "C should be in the fingerprint");
            Assert.AreEqual(fp1.numOfHits[Cindex], 3);
        }
    }
}
