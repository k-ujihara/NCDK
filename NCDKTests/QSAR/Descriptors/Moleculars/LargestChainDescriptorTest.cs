using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    // @author      chhoppe from EUROSCREEN
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class LargestChainDescriptorTest : MolecularDescriptorTest<LargestChainDescriptor>
    {
        LargestChainDescriptor CreateDescriptor(bool checkAromaticity, bool checkRingSystem) => new LargestChainDescriptor(checkAromaticity, checkRingSystem);

        [TestMethod()]
        public void Test1LargestChainDescriptor()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("c1ccccc1"); // benzol

            Assert.AreEqual(0, CreateDescriptor(true, true).Calculate(mol).Value);
        }

        [TestMethod()]
        public void Test2LargestChainDescriptor()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("C=CC=Cc1ccccc1");
            Assert.AreEqual(4, CreateDescriptor(true, true).Calculate(mol).Value);
        }

        [TestMethod()]
        public void Test3LargestChainDescriptor()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("C=CC=CCc2ccc(Cc1ccncc1C=C)cc2");
            Assert.AreEqual(5, CreateDescriptor(true, true).Calculate(mol).Value);
        }

        [TestMethod()]
        public void Test4LargestChainDescriptor()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CC=CNCC");
            Assert.AreEqual(6, CreateDescriptor(true, true).Calculate(mol).Value);
        }

        [TestMethod()]
        public void Test5LargestChainDescriptor()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("C=C[NH2+]CC");
            Assert.AreEqual(5, CreateDescriptor(true, true).Calculate(mol).Value);
        }

        [TestMethod()]
        public void Test6LargestChainDescriptor()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CCNOC");
            Assert.AreEqual(5, CreateDescriptor(true, true).Calculate(mol).Value);
        }

        [TestMethod()]
        public void Test7LargestChainDescriptor()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CC=CC(C)=O");
            Assert.AreEqual(5, CreateDescriptor(true, true).Calculate(mol).Value);
        }

        [TestMethod()]
        public void TestSingleCAtom()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("C");
            Assert.AreEqual(0, CreateDescriptor(true, true).Calculate(mol).Value);
        }

        [TestMethod()]
        public void TestSingleOAtom()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("O");
            Assert.AreEqual(0, CreateDescriptor(true, true).Calculate(mol).Value);
        }

        [TestMethod()]
        public void Test8LargestChainDescriptor()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("Cc1nn(c(c1)N)c1nc2c(s1)cccc2");
            Assert.AreEqual(0, CreateDescriptor(true, true).Calculate(mol).Value);
        }

        [TestMethod()]
        public void Test9LargestChainDescriptor()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("Nc1c(cn[nH]1)C#N");
            Assert.AreEqual(2, CreateDescriptor(true, true).Calculate(mol).Value);
        }

        [TestMethod()]
        public void Test10LargestChainDescriptor()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("OCc1ccccc1CN");
            Assert.AreEqual(2, CreateDescriptor(true, true).Calculate(mol).Value);
        }

        [TestMethod()]
        public void Test11LargestChainDescriptor()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("COc1ccc(cc1)c1noc(c1)Cn1nc(C)c(c(c1=O)C#N)C");
            Assert.AreEqual(2, CreateDescriptor(true, true).Calculate(mol).Value);
        }
    }
}
