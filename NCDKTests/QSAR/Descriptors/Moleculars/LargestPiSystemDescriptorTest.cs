using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.QSAR.Result;
using NCDK.Smiles;
using System;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// TestSuite that runs all QSAR tests.
    /// </summary>
    // @author      chhoppe from EUROSCREEN
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class LargestPiSystemDescriptorTest : MolecularDescriptorTest
    {
        public LargestPiSystemDescriptorTest()
        {
            SetDescriptor(typeof(LargestPiSystemDescriptor));
        }

        [TestMethod()]
        public void Test1LargestPiSystemDescriptor()
        {
            Descriptor.Parameters = new object[] { false };
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("c1ccccc1"); // benzol
                                                             //Assert.AreEqual(6, ((IntegerResult)Descriptor.Calculate(mol).GetValue()).Value);
            Console.Out.WriteLine("test1>:" + ((IntegerResult)Descriptor.Calculate(mol).Value).Value);
        }

        [TestMethod()]
        public void Test2LargestPiSystemDescriptor()
        {
            Descriptor.Parameters = new object[] { false };
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("C=CC=Cc1ccccc1");
            Assert.AreEqual(10, ((IntegerResult)Descriptor.Calculate(mol).Value).Value);
            //Debug.WriteLine("test2>"+((IntegerResult)Descriptor.Calculate(mol).GetValue()).Value);
        }

        [TestMethod()]
        public void Test3LargestPiSystemDescriptor()
        {
            Descriptor.Parameters = new object[] { false };
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("C=CC=CCc2ccc(Cc1ccncc1C=C)cc2");
            //Debug.WriteLine("test3>"+((IntegerResult)Descriptor.Calculate(mol).GetValue()).Value);
            Assert.AreEqual(8, ((IntegerResult)Descriptor.Calculate(mol).Value).Value);
        }

        [TestMethod()]
        public void Test4LargestPiSystemDescriptor()
        {
            Descriptor.Parameters = new object[] { false };
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("CC=CNCC");
            //Debug.WriteLine("test4>"+((IntegerResult)Descriptor.Calculate(mol).GetValue()).Value);
            Assert.AreEqual(3, ((IntegerResult)Descriptor.Calculate(mol).Value).Value);
        }

        [TestMethod()]
        public void Test5LargestPiSystemDescriptor()
        {
            Descriptor.Parameters = new object[] { false };
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("C=C[NH2+]CC");
            //Debug.WriteLine("test5>"+((IntegerResult)Descriptor.Calculate(mol).GetValue()).Value);
            Assert.AreEqual(3, ((IntegerResult)Descriptor.Calculate(mol).Value).Value);
        }

        [TestMethod()]
        public void Test6LargestPiSystemDescriptor()
        {
            Descriptor.Parameters = new object[] { false };
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("CCNOC");
            //Debug.WriteLine("test6>"+((IntegerResult)Descriptor.Calculate(mol).GetValue()).Value);
            Assert.AreEqual(2, ((IntegerResult)Descriptor.Calculate(mol).Value).Value);
        }

        [TestMethod()]
        public void Test7LargestPiSystemDescriptor()
        {
            Descriptor.Parameters = new object[] { false };
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("CC=CC(C)=O");
            //Debug.WriteLine("test7>"+((IntegerResult)Descriptor.Calculate(mol).GetValue()).Value);
            Assert.AreEqual(4, ((IntegerResult)Descriptor.Calculate(mol).Value).Value);
        }
    }
}
