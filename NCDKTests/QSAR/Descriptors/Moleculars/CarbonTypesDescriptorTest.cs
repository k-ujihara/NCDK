using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class CarbonTypesDescriptorTest : MolecularDescriptorTest<CarbonTypesDescriptor>
    {
        [TestMethod()]
        public void TestButane()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CCCC");

            var ret = CreateDescriptor().Calculate(mol).Values;

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
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("C(C)(C)C=C(C)C");

            var ret = CreateDescriptor().Calculate(mol).Values;

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
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("C#CC(C)=C");

            var ret = CreateDescriptor().Calculate(mol).Values;

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
