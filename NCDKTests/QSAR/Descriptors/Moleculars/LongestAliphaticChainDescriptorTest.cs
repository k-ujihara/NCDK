using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Smiles;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    // @author      chhoppe from EUROSCREEN
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class LongestAliphaticChainDescriptorTest : MolecularDescriptorTest<LongestAliphaticChainDescriptor>
    {
        protected override LongestAliphaticChainDescriptor CreateDescriptor() => new LongestAliphaticChainDescriptor(true);

        [TestMethod()]
        public void Test1LongestAliphaticChainDescriptor()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CCCCc1ccccc1"); // benzol
            Assert.AreEqual(4, CreateDescriptor().Calculate(mol).Value);
        }

        [TestMethod()]
        public void Test2LargestChainDescriptor()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("C=CC=Cc1ccccc1");
            Assert.AreEqual(4, CreateDescriptor().Calculate(mol).Value);
        }

        [TestMethod()]
        public void Test3LargestChainDescriptor()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("C=C(CCC1CC1C(C)C(C)C)C(C)CC2CCCC2");
            Assert.AreEqual(5, CreateDescriptor().Calculate(mol).Value);
        }

        [TestMethod()]
        public void Test4LargestChainDescriptor()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CCCCNCC");
            Assert.AreEqual(4, CreateDescriptor().Calculate(mol).Value);
        }

        [TestMethod()]
        public void Test5LargestChainDescriptor()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CC(C)(C)c1ccccc1");
            Assert.AreEqual(3, CreateDescriptor().Calculate(mol).Value);
        }

        [TestMethod()]
        public void Test6LargestChainDescriptor()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CC(C)(C)c2ccc(OCCCC(=O)Nc1nccs1)cc2");
            Assert.AreEqual(4, CreateDescriptor().Calculate(mol).Value);
        }

        [TestMethod()]
        public void Test7LargestChainDescriptor()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CC(=O)N1CCN(CC1)c2ccc(NC(=O)COc3ccc(cc3)C(C)(C)C)cc2");
            Assert.AreEqual(3, CreateDescriptor().Calculate(mol).Value);
        }

        [TestMethod()]
        public void Ethanol()
        {
            AssertSmiles("CCO", 2);
            AssertSmiles("OCC", 2);
        }

        [TestMethod()]
        public void TestHeptabarb()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("O=C1NC(=O)NC(=O)C1(/C2=C/CCCCC2)CC");
            Assert.AreEqual(2, CreateDescriptor().Calculate(mol).Value);
        }

        private void AssertSmiles(string smi, int expected)
        {
            SmilesParser smipar = CDK.SmilesParser;
            var mol = smipar.ParseSmiles(smi);
            Assert.AreEqual(expected.ToString(), CreateDescriptor().Calculate(mol).Value.ToString());
        }
    }
}
