using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Silent;
using System;

namespace NCDK.Fingerprints
{
    [TestClass]
    public class HybridizationFingerprinter_Example
    {
        [TestMethod]
        [TestCategory("Example")]
        public void Main()
        {
            {
                #region 
                var molecule = new AtomContainer();
                var fingerprinter = new HybridizationFingerprinter();
                var fingerprint = fingerprinter.GetBitFingerprint(molecule);
                Console.WriteLine(fingerprint.Count); // returns 1024 by default
                #endregion
            }
        }
    }
}
