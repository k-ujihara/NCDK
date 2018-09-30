using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.QSAR.Results;
using NCDK.Silent;
using NCDK.Smiles;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// TestSuite for the LargestChainDescriptor.
    /// </summary>
    // @author      chhoppe from EUROSCREEN
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class LargestChainDescriptorTest : MolecularDescriptorTest
    {
        public LargestChainDescriptorTest()
        {
            SetDescriptor(typeof(LargestChainDescriptor));
        }

        [TestMethod()]
        public void Test1LargestChainDescriptor()
        {
            Descriptor.Parameters = new object[] { true, true };
            SmilesParser sp = new SmilesParser(ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("c1ccccc1"); // benzol
                                                             //Debug.WriteLine("test1>:"+((Result<int>)Descriptor.Calculate(mol).GetValue()).Value);
            Assert.AreEqual(0, ((Result<int>)Descriptor.Calculate(mol).Value).Value);
        }

        [TestMethod()]
        public void Test2LargestChainDescriptor()
        {
            Descriptor.Parameters = new object[] { true, true };
            SmilesParser sp = new SmilesParser(ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("C=CC=Cc1ccccc1");
            //Debug.WriteLine("test2>"+((Result<int>)Descriptor.Calculate(mol).GetValue()).Value);
            Assert.AreEqual(4, ((Result<int>)Descriptor.Calculate(mol).Value).Value);
        }

        [TestMethod()]
        public void Test3LargestChainDescriptor()
        {
            Descriptor.Parameters = new object[] { true, true };
            SmilesParser sp = new SmilesParser(ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("C=CC=CCc2ccc(Cc1ccncc1C=C)cc2");
            //Debug.WriteLine("test3>"+((Result<int>)Descriptor.Calculate(mol).GetValue()).Value);
            Assert.AreEqual(5, ((Result<int>)Descriptor.Calculate(mol).Value).Value);
        }

        [TestMethod()]
        public void Test4LargestChainDescriptor()
        {
            Descriptor.Parameters = new object[] { true, true };
            SmilesParser sp = new SmilesParser(ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("CC=CNCC");
            //Debug.WriteLine("test4>"+((Result<int>)Descriptor.Calculate(mol).GetValue()).Value);
            Assert.AreEqual(6, ((Result<int>)Descriptor.Calculate(mol).Value).Value);
        }

        [TestMethod()]
        public void Test5LargestChainDescriptor()
        {
            Descriptor.Parameters = new object[] { true, true };
            SmilesParser sp = new SmilesParser(ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("C=C[NH2+]CC");
            //Debug.WriteLine("test5>"+((Result<int>)Descriptor.Calculate(mol).GetValue()).Value);
            Assert.AreEqual(5, ((Result<int>)Descriptor.Calculate(mol).Value).Value);
        }

        [TestMethod()]
        public void Test6LargestChainDescriptor()
        {
            Descriptor.Parameters = new object[] { true, true };
            SmilesParser sp = new SmilesParser(ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("CCNOC");
            //Debug.WriteLine("test6>"+((Result<int>)Descriptor.Calculate(mol).GetValue()).Value);
            Assert.AreEqual(5, ((Result<int>)Descriptor.Calculate(mol).Value).Value);
        }

        [TestMethod()]
        public void Test7LargestChainDescriptor()
        {
            Descriptor.Parameters = new object[] { true, true };
            SmilesParser sp = new SmilesParser(ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("CC=CC(C)=O");
            //Debug.WriteLine("test7>"+((Result<int>)Descriptor.Calculate(mol).GetValue()).Value);
            Assert.AreEqual(5, ((Result<int>)Descriptor.Calculate(mol).Value).Value);
        }

        [TestMethod()]
        public void TestSingleCAtom()
        {
            Descriptor.Parameters = new object[] { true, true };
            SmilesParser sp = new SmilesParser(ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("C");
            //Debug.WriteLine("test7>"+((Result<int>)Descriptor.Calculate(mol).GetValue()).Value);
            Assert.AreEqual(0, ((Result<int>)Descriptor.Calculate(mol).Value).Value);
        }

        [TestMethod()]
        public void TestSingleOAtom()
        {
            Descriptor.Parameters = new object[] { true, true };
            SmilesParser sp = new SmilesParser(ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("O");
            //Debug.WriteLine("test7>"+((Result<int>)Descriptor.Calculate(mol).GetValue()).Value);
            Assert.AreEqual(0, ((Result<int>)Descriptor.Calculate(mol).Value).Value);
        }

        [TestMethod()]
        public void Test8LargestChainDescriptor()
        {
            Descriptor.Parameters = new object[] { true, true };
            SmilesParser sp = new SmilesParser(ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("Cc1nn(c(c1)N)c1nc2c(s1)cccc2");
            Assert.AreEqual(0, ((Result<int>)Descriptor.Calculate(mol).Value).Value);
        }

        [TestMethod()]
        public void Test9LargestChainDescriptor()
        {
            Descriptor.Parameters = new object[] { true, true };
            SmilesParser sp = new SmilesParser(ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("Nc1c(cn[nH]1)C#N");
            Assert.AreEqual(2, ((Result<int>)Descriptor.Calculate(mol).Value).Value);
        }

        [TestMethod()]
        public void Test10LargestChainDescriptor()
        {
            Descriptor.Parameters = new object[] { true, true };
            SmilesParser sp = new SmilesParser(ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("OCc1ccccc1CN");
            Assert.AreEqual(2, ((Result<int>)Descriptor.Calculate(mol).Value).Value);
        }

        [TestMethod()]
        public void Test11LargestChainDescriptor()
        {
            Descriptor.Parameters = new object[] { true, true };
            SmilesParser sp = new SmilesParser(ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("COc1ccc(cc1)c1noc(c1)Cn1nc(C)c(c(c1=O)C#N)C");
            Assert.AreEqual(2, ((Result<int>)Descriptor.Calculate(mol).Value).Value);
        }
    }
}
