using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.QSAR.Results;
using NCDK.Silent;
using NCDK.Smiles;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    // @author      chhoppe from EUROSCREEN
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class LongestAliphaticChainDescriptorTest : MolecularDescriptorTest
    {
        public LongestAliphaticChainDescriptorTest()
        {
            SetDescriptor(typeof(LongestAliphaticChainDescriptor));
        }

        [TestMethod()]
        public void Test1LongestAliphaticChainDescriptor()
        {
            Descriptor.Parameters = new object[] { true };
            var sp = CDK.SilentSmilesParser;
            var mol = sp.ParseSmiles("CCCCc1ccccc1"); // benzol
            Assert.AreEqual(4, ((Result<int>)Descriptor.Calculate(mol).Value).Value);
        }

        [TestMethod()]
        public void Test2LargestChainDescriptor()
        {
            Descriptor.Parameters = new object[] { true };
            var sp = CDK.SilentSmilesParser;
            var mol = sp.ParseSmiles("C=CC=Cc1ccccc1");
            Assert.AreEqual(4, ((Result<int>)Descriptor.Calculate(mol).Value).Value);
        }

        [TestMethod()]
        public void Test3LargestChainDescriptor()
        {
            Descriptor.Parameters = new object[] { true };
            var sp = CDK.SilentSmilesParser;
            var mol = sp.ParseSmiles("C=C(CCC1CC1C(C)C(C)C)C(C)CC2CCCC2");
            Assert.AreEqual(5, ((Result<int>)Descriptor.Calculate(mol).Value).Value);
        }

        [TestMethod()]
        public void Test4LargestChainDescriptor()
        {
            Descriptor.Parameters = new object[] { true };
            var sp = CDK.SilentSmilesParser;
            var mol = sp.ParseSmiles("CCCCNCC");
            Assert.AreEqual(4, ((Result<int>)Descriptor.Calculate(mol).Value).Value);
        }

        [TestMethod()]
        public void Test5LargestChainDescriptor()
        {
            Descriptor.Parameters = new object[] { true };
            var sp = CDK.SilentSmilesParser;
            var mol = sp.ParseSmiles("CC(C)(C)c1ccccc1");
            Assert.AreEqual(3, ((Result<int>)Descriptor.Calculate(mol).Value).Value);
        }

        [TestMethod()]
        public void Test6LargestChainDescriptor()
        {
            Descriptor.Parameters = new object[] { true };
            var sp = CDK.SilentSmilesParser;
            var mol = sp.ParseSmiles("CC(C)(C)c2ccc(OCCCC(=O)Nc1nccs1)cc2");
            Assert.AreEqual(4, ((Result<int>)Descriptor.Calculate(mol).Value).Value);
        }

        [TestMethod()]
        public void Test7LargestChainDescriptor()
        {
            Descriptor.Parameters = new object[] { true };
            var sp = CDK.SilentSmilesParser;
            var mol = sp.ParseSmiles("CC(=O)N1CCN(CC1)c2ccc(NC(=O)COc3ccc(cc3)C(C)(C)C)cc2");
            Assert.AreEqual(3, ((Result<int>)Descriptor.Calculate(mol).Value).Value);
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
            Descriptor.Parameters = new object[] { true };
            var sp = CDK.SilentSmilesParser;
            var mol = sp.ParseSmiles("O=C1NC(=O)NC(=O)C1(/C2=C/CCCCC2)CC");
            Assert.AreEqual(2, ((Result<int>)Descriptor.Calculate(mol).Value).Value);
        }

        private void AssertSmiles(string smi, int expected)
        {
            SmilesParser smipar = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            var mol = smipar.ParseSmiles(smi);
            Descriptor.Parameters = new object[] { true };
            Assert.AreEqual(expected.ToString(), Descriptor.Calculate(mol).Value.ToString());
        }
    }
}
