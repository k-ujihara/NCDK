using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.Fragments;
using NCDK.IO;
using NCDK.QSAR.Result;


namespace NCDK.QSAR.Descriptors.Moleculars
{
    // @author      chhoppe from EUROSCREEN
    // @cdk.module  test-qsarmolecular 
    [TestClass()]
    public class FragmentComplexityDescriptorTest : MolecularDescriptorTest
    {
        public FragmentComplexityDescriptorTest()
        {
            SetDescriptor(typeof(FragmentComplexityDescriptor));
        }

        [TestMethod()]
        public void Test1FragmentComplexityDescriptor()
        {
            IMolecularDescriptor Descriptor = new FragmentComplexityDescriptor();
            string filename = "NCDK.Data.MDL.murckoTest1.mol";
            //Console.Out.WriteLine("\nFragmentComplexityTest: " + filename);
            var ins = ResourceLoader.GetAsStream(filename);
            MurckoFragmenter gf = new MurckoFragmenter();
            double Complexity = 0;
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
            IAtomContainer mol = reader.Read(new AtomContainer());
            gf.GenerateFragments(mol);
            var setOfFragments = gf.GetFrameworksAsContainers();
            foreach (var setOfFragment in setOfFragments)
            {
                AddExplicitHydrogens(setOfFragment);
                Complexity = ((DoubleResult)Descriptor.Calculate(setOfFragment).GetValue()).Value;
                //Console.Out.WriteLine("Complexity:"+Complexity);
            }
            Assert.AreEqual(659.00, Complexity, 0.01);
        }

        [TestMethod()]
        public void Test2FragmentComplexityDescriptor()
        {
            IMolecularDescriptor Descriptor = new FragmentComplexityDescriptor();
            string filename = "NCDK.Data.MDL.murckoTest10.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MurckoFragmenter gf = new MurckoFragmenter();
            double Complexity = 0;
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
            IAtomContainer mol = reader.Read(Default.ChemObjectBuilder.Instance.NewAtomContainer());
            gf.GenerateFragments(mol);
            var setOfFragments = gf.GetFrameworksAsContainers();
            foreach (var setOfFragment in setOfFragments)
            {
                AddExplicitHydrogens(setOfFragment);
                Complexity = ((DoubleResult)Descriptor.Calculate(setOfFragment).GetValue()).Value;
            }
            Assert.AreEqual(544.01, Complexity, 0.01);
        }
    }
}
