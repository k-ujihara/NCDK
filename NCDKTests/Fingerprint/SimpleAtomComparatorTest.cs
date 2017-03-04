using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Fingerprint;

namespace NCDK.Fingerprint
{
    /// <summary>
    // @author John May
    // @cdk.module test-fingerprint
    /// </summary>
    [TestClass()]
    public class SimpleAtomComparatorTest
    {

        private IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;

        [TestMethod()]
        public void TestCompare_NullHybridization()
        {

            SimpleAtomComparator comparator = new SimpleAtomComparator();

            IAtom a1 = builder.CreateAtom("C");
            IAtom a2 = builder.CreateAtom("C");

            Assert.AreEqual(0, comparator.Compare(a1, a2), "Null hybridzation should be equals");

        }

        [TestMethod()]
        public void TestCompare_SameHybridization()
        {

            SimpleAtomComparator comparator = new SimpleAtomComparator();

            IAtom a1 = builder.CreateAtom("C");
            IAtom a2 = builder.CreateAtom("C");

            a1.Hybridization = Hybridization.SP3;
            a2.Hybridization = Hybridization.SP3;

            Assert.AreEqual(0, comparator.Compare(a1, a2), "Same hybridzation should be equals");

        }

        [TestMethod()]
        public void TestCompare_DifferentHybridization()
        {

            SimpleAtomComparator comparator = new SimpleAtomComparator();

            IAtom a1 = builder.CreateAtom("C");
            IAtom a2 = builder.CreateAtom("C");

            a1.Hybridization = Hybridization.SP2;
            a2.Hybridization = Hybridization.SP3;

            Assert.AreEqual(-1, comparator.Compare(a1, a2), "Atom 2 should have priority");

        }

        [TestMethod()]
        public void TestCompare_DifferentSymbol()
        {

            SimpleAtomComparator comparator = new SimpleAtomComparator();

            IAtom a1 = builder.CreateAtom("C");
            IAtom a2 = builder.CreateAtom("O");

            // can't do less than correctly without hamcrest?
            Assert.IsTrue(comparator.Compare(a1, a2) < 0, "oxygen should rank above carbon");
            Assert.IsTrue(comparator.Compare(a2, a1) > 0, "oxygen should rank above carbon (inverse)");

        }
    }
}
