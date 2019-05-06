/* This work is the product of a US Government employee as part of his/her regular duties
 * and is thus in the public domain.
 * 
 * Author: Lyle D. Burgoon, Ph.D. (lyle.d.burgoon@usace.army.mil)
 * Date: 5 FEBRUARY 2018
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Smiles;

namespace NCDK.Fingerprints
{
    // @cdk.module test-fingerprint
    [TestClass()]
    public class AtomPairs2DFingerprintTest
    {
        private readonly SmilesParser parser = CDK.SmilesParser;

        [TestMethod()]
        public void TestFingerprint()
        {
            // We are going to test hexane. Hexane is a good test b/c it has 10 carbons.
            // Since the max distance for this fingerprint is 10, the final C-C fingerprint slot
            // at distance 10 should return false, while all the other C-C fingerprint slots
            // should return true.
            var printer = new AtomPairs2DFingerprinter();
            var mol1 = parser.ParseSmiles("cccccccccc");
            var bsfp = (BitSetFingerprint)printer.GetBitFingerprint(mol1);
            Assert.AreEqual(9, bsfp.Cardinality);
            Assert.AreEqual(true, bsfp[0]);        //Distance 1
            Assert.AreEqual(true, bsfp[78]);    //Distance 2
            Assert.AreEqual(true, bsfp[156]);    //Distance 3
            Assert.AreEqual(true, bsfp[234]);    //Distance 4
            Assert.AreEqual(true, bsfp[312]);    //Distance 5
            Assert.AreEqual(true, bsfp[390]);    //Distance 6
            Assert.AreEqual(true, bsfp[468]);    //Distance 7
            Assert.AreEqual(true, bsfp[546]);    //Distance 8
            Assert.AreEqual(true, bsfp[624]);    //Distance 9
            Assert.AreEqual(false, bsfp[702]);    //Distance 10
        }

        [TestMethod()]
        public void TestChlorobenzene()
        {
            var printer = new AtomPairs2DFingerprinter();
            var mol1 = parser.ParseSmiles("Clc1ccccc1");
            var bsfp = (BitSetFingerprint)printer.GetBitFingerprint(mol1);
        }

        [TestMethod()]
        public void TestHalogen()
        {
            var printer = new AtomPairs2DFingerprinter();
            var mol1 = parser.ParseSmiles("Clc1ccccc1");
            var map = printer.GetRawFingerprint(mol1);
            Assert.IsTrue(map.ContainsKey("1_X_C"));
            Assert.IsTrue(map.ContainsKey("1_Cl_C"));
            Assert.IsTrue(map.ContainsKey("2_X_C"));
            Assert.IsTrue(map.ContainsKey("2_Cl_C"));
            Assert.IsTrue(map.ContainsKey("3_X_C"));
            Assert.IsTrue(map.ContainsKey("3_Cl_C"));
            Assert.IsTrue(map.ContainsKey("4_X_C"));
            Assert.IsTrue(map.ContainsKey("4_Cl_C"));
        }

        [TestMethod()]
        public void IgnoredAtom()
        {
            var printer = new AtomPairs2DFingerprinter();
            var mol1 = parser.ParseSmiles("[Te]1cccc1");
            var map = printer.GetRawFingerprint(mol1);
            Assert.IsTrue(map.ContainsKey("1_C_C"));
            Assert.IsTrue(map.ContainsKey("2_C_C"));
        }

        [TestMethod()]
        public void TestGetCountFingerprint()
        {
            var printer = new AtomPairs2DFingerprinter();
            var mol1 = parser.ParseSmiles("cccccccccc");
            ICountFingerprint icfp = printer.GetCountFingerprint(mol1);
            Assert.AreEqual(9, icfp.GetNumberOfPopulatedBins());
        }

        [TestMethod()]
        public void TestGetRawFingerprint()
        {
            var printer = new AtomPairs2DFingerprinter();
        }

        [TestMethod()]
        public void TestNullPointerExceptionInGetBitFingerprint()
        {
            var printer = new AtomPairs2DFingerprinter();
            IAtomContainer chlorobenzene;
            chlorobenzene = parser.ParseSmiles("Clc1ccccc1");
            var bsfp1 = (BitSetFingerprint)printer.GetBitFingerprint(chlorobenzene);
            chlorobenzene = parser.ParseSmiles("c1ccccc1Cl");
            var bsfp2 = (BitSetFingerprint)printer.GetBitFingerprint(chlorobenzene);
        }
    }
}
