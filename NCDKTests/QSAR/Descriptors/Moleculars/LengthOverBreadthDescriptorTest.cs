using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Config;
using NCDK.IO;
using NCDK.Tools.Manipulator;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class LengthOverBreadthDescriptorTest : MolecularDescriptorTest<LengthOverBreadthDescriptor>
    {
        [TestMethod()]
        public void TestLOBDescriptorCholesterol()
        {
            var filename = "NCDK.Data.MDL.lobtest.sdf";
            IChemFile content;
            using (var reader = new MDLV2000Reader(ResourceLoader.GetAsStream(filename)))
            {
                content = reader.Read(CDK.Builder.NewChemFile());
            }
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToReadOnlyList();
            var ac = cList[0];
            BODRIsotopeFactory.Instance.ConfigureAtoms(ac);

            var result = CreateDescriptor(ac).Calculate();

            Assert.AreEqual(3.5029, result.Values[0], 0.001);
            Assert.AreEqual(3.5029, result.Values[1], 0.001);
        }

        [TestMethod()]
        public void TestLOBDescriptorCyclohexane()
        {
            var filename = "NCDK.Data.MDL.lobtest.sdf";
            IChemFile content;
            using (var reader = new MDLV2000Reader(ResourceLoader.GetAsStream(filename)))
            {
                content = reader.Read(CDK.Builder.NewChemFile());
            }
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToReadOnlyList();
            var ac = cList[1];
            BODRIsotopeFactory.Instance.ConfigureAtoms(ac);

            var result = CreateDescriptor(ac).Calculate();

            Assert.AreEqual(1.1476784, result.Values[0], 0.01);
            Assert.AreEqual(1.0936984, result.Values[1], 0.01);
        }

        [TestMethod()]
        public void TestLOBDescriptorNaphthalene()
        {
            var filename = "NCDK.Data.MDL.lobtest.sdf";
            IChemFile content;
            using (var reader = new MDLV2000Reader(ResourceLoader.GetAsStream(filename)))
            {
                content = reader.Read(CDK.Builder.NewChemFile());
            }
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToReadOnlyList();
            var ac = cList[2];

            BODRIsotopeFactory.Instance.ConfigureAtoms(ac);

            var result = CreateDescriptor(ac).Calculate();

            Assert.AreEqual(1.3083278, result.Values[0], 0.01);
            Assert.AreEqual(1.3083278, result.Values[1], 0.01);
        }

        [TestMethod()]
        public void TestLOBDescriptorNButane()
        {
            var filename = "NCDK.Data.MDL.lobtest.sdf";
            IChemFile content;
            using (var reader = new MDLV2000Reader(ResourceLoader.GetAsStream(filename)))
            {
                content = reader.Read(CDK.Builder.NewChemFile());
            }
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToReadOnlyList();
            var ac = cList[3];
            BODRIsotopeFactory.Instance.ConfigureAtoms(ac);

            var result = CreateDescriptor(ac).Calculate();

            Assert.AreEqual(2.0880171, result.Values[0], 0.000001);
            Assert.AreEqual(2.0880171, result.Values[1], 0.000001);
        }

        // @cdk.bug 1965254
        [TestMethod()]
        public void TestLOBDescriptor2()
        {
            var filename = "NCDK.Data.MDL.lobtest2.sdf";
            IChemFile content;
            using (var reader = new MDLV2000Reader(ResourceLoader.GetAsStream(filename)))
            {
                content = reader.Read(CDK.Builder.NewChemFile());
            }
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToReadOnlyList();
            var ac = cList[0];
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(ac);

            var result = CreateDescriptor(ac).Calculate();
            Assert.IsNotNull(result);
        }
    }
}
