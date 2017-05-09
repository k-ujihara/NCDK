using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Silent;
using NCDK.Tools.Manipulator;
using System;

namespace NCDK.Fingerprints
{
    [TestClass]
    public class ShortestPathFingerprinter_Example
    {
        [TestMethod]
        [TestCategory("Example")]
        public void Main()
        {
            {
                #region 
                var molecule = new AtomContainer();
                AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
                var fingerprinter = new ShortestPathFingerprinter();
                var fingerprint = fingerprinter.GetBitFingerprint(molecule);
                Console.WriteLine(fingerprint.Count); // returns 1024 by default
                #endregion
            }
        }
    }
}
