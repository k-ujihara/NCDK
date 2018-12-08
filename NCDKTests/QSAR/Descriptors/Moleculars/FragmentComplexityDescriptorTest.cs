using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Fragments;
using NCDK.IO;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    // @author      chhoppe from EUROSCREEN
    // @cdk.module  test-qsarmolecular 
    [TestClass()]
    public class FragmentComplexityDescriptorTest : MolecularDescriptorTest<FragmentComplexityDescriptor>
    {
        IChemObjectBuilder builder = CDK.Builder;

        [TestMethod()]
        public void Test1FragmentComplexityDescriptor()
        {
            var filename = "NCDK.Data.MDL.murckoTest1.mol";
            IAtomContainer mol;
            using (var reader = new MDLV2000Reader(ResourceLoader.GetAsStream(filename), ChemObjectReaderMode.Strict))
            {
                mol = reader.Read(builder.NewAtomContainer());
            }
            var gf = new MurckoFragmenter();
            gf.GenerateFragments(mol);
            var setOfFragments = gf.GetFrameworksAsContainers();
            double Complexity = 0;
            foreach (var setOfFragment in setOfFragments)
            {
                AddExplicitHydrogens(setOfFragment);
                Complexity = CreateDescriptor(setOfFragment).Calculate().Value;
            }
            Assert.AreEqual(659.00, Complexity, 0.01);
        }

        [TestMethod()]
        public void Test2FragmentComplexityDescriptor()
        {
            var filename = "NCDK.Data.MDL.murckoTest10.mol";
            IAtomContainer mol;
            using (var reader = new MDLV2000Reader(ResourceLoader.GetAsStream(filename), ChemObjectReaderMode.Strict))
            {
                mol = reader.Read(builder.NewAtomContainer());
            }
            var gf = new MurckoFragmenter();
            gf.GenerateFragments(mol);
            var setOfFragments = gf.GetFrameworksAsContainers();
            double Complexity = 0;
            foreach (var setOfFragment in setOfFragments)
            {
                AddExplicitHydrogens(setOfFragment);
                Complexity = CreateDescriptor(setOfFragment).Calculate().Value;
            }
            Assert.AreEqual(544.01, Complexity, 0.01);
        }
    }
}
