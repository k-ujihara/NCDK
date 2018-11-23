using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    // @author      chhoppe from EUROSCREEN
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class LargestPiSystemDescriptorTest : MolecularDescriptorTest<LargestPiSystemDescriptor>
    {
        [TestMethod()]
        public void Test1LargestPiSystemDescriptor()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("c1ccccc1"); // benzol
            Assert.AreEqual(6, CreateDescriptor(mol).Calculate().Value);
        }

        [TestMethod()]
        public void Test2LargestPiSystemDescriptor()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("C=CC=Cc1ccccc1");
            Assert.AreEqual(10, CreateDescriptor(mol).Calculate().Value);
        }

        [TestMethod()]
        public void Test3LargestPiSystemDescriptor()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("C=CC=CCc2ccc(Cc1ccncc1C=C)cc2");
            Assert.AreEqual(8, CreateDescriptor(mol).Calculate().Value);
        }

        [TestMethod()]
        public void Test4LargestPiSystemDescriptor()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CC=CNCC");
            Assert.AreEqual(3, CreateDescriptor(mol).Calculate().Value);
        }

        [TestMethod()]
        public void Test5LargestPiSystemDescriptor()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("C=C[NH2+]CC");
            Assert.AreEqual(3, CreateDescriptor(mol).Calculate().Value);
        }

        [TestMethod()]
        public void Test6LargestPiSystemDescriptor()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CCNOC");
            Assert.AreEqual(2, CreateDescriptor(mol).Calculate().Value);
        }

        [TestMethod()]
        public void Test7LargestPiSystemDescriptor()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CC=CC(C)=O");
            Assert.AreEqual(4, CreateDescriptor(mol).Calculate().Value);
        }
    }
}
