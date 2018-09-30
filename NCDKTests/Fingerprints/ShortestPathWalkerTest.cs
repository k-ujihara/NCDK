using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Common.Base;
using NCDK.Templates;
using System.Collections.Generic;

namespace NCDK.Fingerprints
{
    // @author John May
    // @cdk.module test-fingerprint
    [TestClass()]
    public class ShortestPathWalkerTest
    {
        [TestMethod()]
        public void TestPaths()
        {
            var triazole = TestMoleculeFactory.Make123Triazole();
            var walker = new ShortestPathWalker(triazole);
            var expected = new SortedSet<string>(new[] {"C", "N2N1N", "N", "N1N1C", "N1C2C", "C1N", "N2N1C",
                "C1N2N", "C1N1N", "N1C", "C2C1N", "C2C", "N2N", "N1N2N", "N1N"});
            var actual = walker.GetPaths();
            Assert.IsTrue(Compares.AreDeepEqual(expected, actual));
        }

        [TestMethod()]
        public void TestToString()
        {
            IAtomContainer triazole = TestMoleculeFactory.Make123Triazole();
            ShortestPathWalker walker = new ShortestPathWalker(triazole);
            Assert.AreEqual(
                "C->C1N->C1N1N->C1N2N->C2C->C2C1N->N->N1C->N1C2C->N1N->N1N1C->N1N2N->N2N->N2N1C->N2N1N",
                walker.ToString());
        }
    }
}
