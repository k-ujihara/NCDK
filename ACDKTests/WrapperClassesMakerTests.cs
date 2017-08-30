using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ACDK
{
    [TestClass()]
    public class WrapperClassesMakerTests
    {
        [TestMethod()]
        public void ChemObjectBuilderTest()
        {
            var a = new W_IChemObjectBuilder(NCDK.Default.ChemObjectBuilder.Instance);
            var b = (IChemObjectBuilder)a;
        }

        [TestMethod()]
        public void IntArrayFingerprintTest()
        {
            var o = new IntArrayFingerprint();
            var bfp = (IBitFingerprint)o;
            bfp[10] = true;
            bfp[20] = true;
            Assert.AreEqual(2, bfp.Cardinality);
            bfp[10] = false;
            Assert.AreEqual(1, bfp.Cardinality);
        }

        [TestMethod()]
        public void BitSetFingerprintTest()
        {
            var o = new BitSetFingerprint();
            var bfp = (IBitFingerprint)o;
            Assert.AreEqual(0, bfp.Count);
        }
    }
}
