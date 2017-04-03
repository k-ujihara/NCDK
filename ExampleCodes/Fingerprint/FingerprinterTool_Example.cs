using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Templates;
using System;

namespace NCDK.Fingerprint
{
    [TestClass]
    public class FingerprinterTool_Example
    {
        [TestMethod]
        [TestCategory("Example")]
        public void Main()
        {
            {
                #region IsSubset
                var mol = TestMoleculeFactory.MakeIndole();
                var fingerprinter = new Fingerprinter();
                var bs = fingerprinter.GetBitFingerprint(mol);
                var frag1 = TestMoleculeFactory.MakePyrrole();
                var bs1 = fingerprinter.GetBitFingerprint(frag1);
                if (FingerprinterTool.IsSubset(bs.AsBitSet(), bs1.AsBitSet()))
                {
                    Console.Out.WriteLine("Pyrrole is subset of Indole.");
                }
                #endregion
            }
        }
    }
}
