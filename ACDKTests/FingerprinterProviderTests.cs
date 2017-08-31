using Microsoft.VisualStudio.TestTools.UnitTesting;
using ACDK;

namespace ACDK.Tests
{
    [TestClass()]
    public class FingerprinterProviderTests
    {
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
    }
}