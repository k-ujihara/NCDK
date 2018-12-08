using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.IO;
using NCDK.Tools.Manipulator;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class MDEDescriptorTest : MolecularDescriptorTest<MDEDescriptor>
    {
        [TestMethod()]
        public void TestMDE1()
        {
            var filename = "NCDK.Data.MDL.mdeotest.sdf";
            IChemFile content;
            using (var reader = new MDLV2000Reader(ResourceLoader.GetAsStream(filename)))
            {
                content = reader.Read(CDK.Builder.NewChemFile());
            }
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToReadOnlyList();
            var ac = cList[0];

            var result = CreateDescriptor(ac).Calculate();

            Assert.AreEqual(0.0000, result.MDEO11, 0.0001);
            Assert.AreEqual(1.1547, result.MDEO12, 0.0001);
            Assert.AreEqual(2.9416, result.MDEO22, 0.0001);
        }
    }
}
