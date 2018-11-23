
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    public partial class ChiChainDescriptorTest : MolecularDescriptorTest<ChiChainDescriptor>
    {
        // @cdk.bug 3023326
        [TestMethod()]
        public void TestCovalentMetal()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CCCC[Sn](CCCC)(CCCC)c1cc(Cl)c(Nc2nc(C)nc(N(CCC)CC3CC3)c2Cl)c(Cl)c1");
            dynamic result = CreateDescriptor(mol).Calculate();

            var ret = (IEnumerable<double>)result.Values;
            Assert.IsNotNull(ret);
        }

        // @cdk.bug 3023326
        [TestMethod()]
        public void TestCovalentPlatinum()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CC1CN[Pt]2(N1)OC(=O)C(C)P(=O)(O)O2");
            var result = CreateDescriptor(mol).Calculate();
            Assert.IsNotNull(result.Exception);
        }
    }
    public partial class ChiPathDescriptorTest : MolecularDescriptorTest<ChiPathDescriptor>
    {
        // @cdk.bug 3023326
        [TestMethod()]
        public void TestCovalentMetal()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CCCC[Sn](CCCC)(CCCC)c1cc(Cl)c(Nc2nc(C)nc(N(CCC)CC3CC3)c2Cl)c(Cl)c1");
            dynamic result = CreateDescriptor(mol).Calculate();

            var ret = (IEnumerable<double>)result.Values;
            Assert.IsNotNull(ret);
        }

        // @cdk.bug 3023326
        [TestMethod()]
        public void TestCovalentPlatinum()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CC1CN[Pt]2(N1)OC(=O)C(C)P(=O)(O)O2");
            var result = CreateDescriptor(mol).Calculate();
            Assert.IsNotNull(result.Exception);
        }
    }
    public partial class ChiClusterDescriptorTest : MolecularDescriptorTest<ChiClusterDescriptor>
    {
        // @cdk.bug 3023326
        [TestMethod()]
        public void TestCovalentMetal()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CCCC[Sn](CCCC)(CCCC)c1cc(Cl)c(Nc2nc(C)nc(N(CCC)CC3CC3)c2Cl)c(Cl)c1");
            dynamic result = CreateDescriptor(mol).Calculate();

            var ret = (IEnumerable<double>)result.Values;
            Assert.IsNotNull(ret);
        }

        // @cdk.bug 3023326
        [TestMethod()]
        public void TestCovalentPlatinum()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CC1CN[Pt]2(N1)OC(=O)C(C)P(=O)(O)O2");
            var result = CreateDescriptor(mol).Calculate();
            Assert.IsNotNull(result.Exception);
        }
    }
    public partial class ChiPathClusterDescriptorTest : MolecularDescriptorTest<ChiPathClusterDescriptor>
    {
        // @cdk.bug 3023326
        [TestMethod()]
        public void TestCovalentMetal()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CCCC[Sn](CCCC)(CCCC)c1cc(Cl)c(Nc2nc(C)nc(N(CCC)CC3CC3)c2Cl)c(Cl)c1");
            dynamic result = CreateDescriptor(mol).Calculate();

            var ret = (IEnumerable<double>)result.Values;
            Assert.IsNotNull(ret);
        }

        // @cdk.bug 3023326
        [TestMethod()]
        public void TestCovalentPlatinum()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CC1CN[Pt]2(N1)OC(=O)C(C)P(=O)(O)O2");
            var result = CreateDescriptor(mol).Calculate();
            Assert.IsNotNull(result.Exception);
        }
    }
}
