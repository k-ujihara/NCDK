using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.QSAR.Result;
using NCDK.Smiles;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// TestSuite that runs all QSAR tests.
    /// </summary>
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class CarbonTypesDescriptorTest : MolecularDescriptorTest
    {
        public CarbonTypesDescriptorTest()
        {
            SetDescriptor(typeof(CarbonTypesDescriptor));
        }

        [TestMethod()]
        public void TestButane()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("CCCC");

            IntegerArrayResult ret = (IntegerArrayResult)Descriptor.Calculate(mol).GetValue();

            Assert.AreEqual(0, ret[0]);
            Assert.AreEqual(0, ret[1]);
            Assert.AreEqual(0, ret[2]);
            Assert.AreEqual(0, ret[3]);
            Assert.AreEqual(0, ret[4]);
            Assert.AreEqual(2, ret[5]);
            Assert.AreEqual(2, ret[6]);
            Assert.AreEqual(0, ret[7]);
            Assert.AreEqual(0, ret[8]);
        }

        [TestMethod()]
        public void TestComplex1()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("C(C)(C)C=C(C)C");

            IntegerArrayResult ret = (IntegerArrayResult)Descriptor.Calculate(mol).GetValue();

            Assert.AreEqual(0, ret[0]);
            Assert.AreEqual(0, ret[1]);
            Assert.AreEqual(0, ret[2]);
            Assert.AreEqual(1, ret[3]);
            Assert.AreEqual(1, ret[4]);
            Assert.AreEqual(4, ret[5]);
            Assert.AreEqual(0, ret[6]);
            Assert.AreEqual(1, ret[7]);
            Assert.AreEqual(0, ret[8]);
        }

        [TestMethod()]
        public void TestComplex2()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("C#CC(C)=C");

            IntegerArrayResult ret = (IntegerArrayResult)Descriptor.Calculate(mol).GetValue();

            Assert.AreEqual(1, ret[0]);
            Assert.AreEqual(1, ret[1]);
            Assert.AreEqual(1, ret[2]);
            Assert.AreEqual(0, ret[3]);
            Assert.AreEqual(1, ret[4]);
            Assert.AreEqual(1, ret[5]);
            Assert.AreEqual(0, ret[6]);
            Assert.AreEqual(0, ret[7]);
            Assert.AreEqual(0, ret[8]);
        }
    }
}
