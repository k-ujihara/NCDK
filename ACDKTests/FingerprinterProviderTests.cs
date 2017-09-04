using Microsoft.VisualStudio.TestTools.UnitTesting;
using ACDK;

namespace ACDK.Tests
{
    [TestClass()]
    public class FingerprinterProviderTests
    {
        static FingerprinterProvider provider = new FingerprinterProvider();
        static ObjectFactory of = new ObjectFactory();
        static SmilesParser sp = of.NewSmilesParser(of.SilentChemObjectBuilder);
        static IAtomContainer mol = sp.ParseSmiles("C");

        [TestMethod()]
        public void GetFingerprinterTest()
        {
            IFingerprinterProvider provider = new FingerprinterProvider();
            var fper = provider.GetFingerprinter(nameof(NCDK.Fingerprints.CircularFingerprinter));
            var fp = fper.GetBitFingerprint(mol);
            Assert.IsTrue(fp.Count > 0);
        }

        [TestMethod()]
        public void BitFingerprintFromStringTest()
        {
            var fp = provider.BitFingerprintFromString("{100}");
            Assert.IsFalse(fp[99]);
            Assert.IsTrue(fp[100]);
            Assert.IsFalse(fp[101]);
        }

        [TestMethod()]
        public void BitFingerprintToStringTest()
        {
            IFingerprinter fp = provider.GetFingerprinter("CircularFingerprinter");
            var val = fp.GetBitFingerprint(sp.ParseSmiles("C"));
            var s = val.ToString();
        }

        [TestMethod()]
        public void BitFingerprintToBitStringTest()
        {
            IFingerprinter fp = provider.GetFingerprinter("CircularFingerprinter");
            var val = fp.GetBitFingerprint(sp.ParseSmiles("C"));
            var s = provider.BitFingerprintToBitString(val);
        }
    }
}