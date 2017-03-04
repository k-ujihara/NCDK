using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Templates;
using NCDK.Tools.Manipulator;
using System.Collections.Generic;

namespace NCDK.Fingerprint
{
    /// <summary>
    // @author John May
    // @cdk.module test-fingerprint
    /// </summary>
    [TestClass()]
    public class SimpleAtomCanonicalizerTest
    {
        [TestMethod()]
        public void TestCanonicalizeAtoms()
        {
            IAtomContainer container = TestMoleculeFactory.MakeAdenine();
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(container);

            var atoms = new SimpleAtomCanonicalizer().CanonicalizeAtoms(container);

            List<IAtom> mutable = new List<IAtom>(atoms);
            foreach (var atom in mutable.GetRange(0, 5))
            {
                Assert.AreEqual("C", atom.Symbol, "expect sp2 carbons in first 4 entries");
                Assert.AreEqual(Hybridization.SP2, atom.Hybridization, "expect sp2 carbons in first 4 entries");
            }
            foreach (var atom in mutable.GetRange(5, 3))
            {
                Assert.AreEqual("N", atom.Symbol, "expect sp2 nitrogen at indices 5-7");
                Assert.AreEqual(Hybridization.SP2, atom.Hybridization, "expect sp2 nitrogen at indices 5-7");
            }

            Assert.AreEqual("N", mutable[8].Symbol, "expect nitrogen at indices 8");
            Assert.AreEqual(Hybridization.SP3, mutable[8].Hybridization, "expect sp3 nitrogen at indices 8");

            Assert.AreEqual("N", mutable[9].Symbol, "expect nitrogen at indices 9");
            Assert.AreEqual(Hybridization.Planar3, mutable[9].Hybridization, "expect sp3 nitrogen at indices 9");
        }
    }
}
