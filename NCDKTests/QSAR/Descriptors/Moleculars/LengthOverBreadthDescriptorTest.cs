using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Config;
using NCDK.Default;
using NCDK.IO;
using NCDK.QSAR.Result;
using NCDK.Tools.Manipulator;
using System.Linq;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// TestSuite that runs all QSAR tests.
    /// </summary>
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class LengthOverBreadthDescriptorTest : MolecularDescriptorTest
    {
        public LengthOverBreadthDescriptorTest()
        {
            SetDescriptor(typeof(LengthOverBreadthDescriptor));
        }

        [TestMethod()]
        public void TestLOBDescriptorCholesterol()
        {
            string filename = "NCDK.Data.MDL.lobtest.sdf";
            var ins = ResourceLoader.GetAsStream(filename);
            ISimpleChemObjectReader reader = new MDLV2000Reader(ins);
            ChemFile content = (ChemFile)reader.Read(new ChemFile());
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToList();
            IAtomContainer ac = (IAtomContainer)cList[0];
            Isotopes.Instance.ConfigureAtoms(ac);

            DoubleArrayResult result = (DoubleArrayResult)Descriptor.Calculate(ac).GetValue();

            Assert.AreEqual(3.5029, result[0], 0.001);
            Assert.AreEqual(3.5029, result[1], 0.001);
        }

        [TestMethod()]
        public void TestLOBDescriptorCyclohexane()
        {
            string filename = "NCDK.Data.MDL.lobtest.sdf";
            var ins = ResourceLoader.GetAsStream(filename);
            ISimpleChemObjectReader reader = new MDLV2000Reader(ins);
            ChemFile content = (ChemFile)reader.Read(new ChemFile());
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToList();
            IAtomContainer ac = (IAtomContainer)cList[1];
            Isotopes.Instance.ConfigureAtoms(ac);

            DoubleArrayResult result = (DoubleArrayResult)Descriptor.Calculate(ac).GetValue();

            Assert.AreEqual(1.1476784, result[0], 0.01);
            Assert.AreEqual(1.0936984, result[1], 0.01);
        }

        [TestMethod()]
        public void TestLOBDescriptorNaphthalene()
        {
            string filename = "NCDK.Data.MDL.lobtest.sdf";
            var ins = ResourceLoader.GetAsStream(filename);
            ISimpleChemObjectReader reader = new MDLV2000Reader(ins);
            ChemFile content = (ChemFile)reader.Read(new ChemFile());
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToList();
            IAtomContainer ac = (IAtomContainer)cList[2];

            Isotopes.Instance.ConfigureAtoms(ac);

            DoubleArrayResult result = (DoubleArrayResult)Descriptor.Calculate(ac).GetValue();

            Assert.AreEqual(1.3083278, result[0], 0.01);
            Assert.AreEqual(1.3083278, result[1], 0.01);
        }

        [TestMethod()]
        public void TestLOBDescriptorNButane()
        {
            string filename = "NCDK.Data.MDL.lobtest.sdf";
            var ins = ResourceLoader.GetAsStream(filename);
            ISimpleChemObjectReader reader = new MDLV2000Reader(ins);
            ChemFile content = (ChemFile)reader.Read(new ChemFile());
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToList();
            IAtomContainer ac = (IAtomContainer)cList[3];
            Isotopes.Instance.ConfigureAtoms(ac);

            DoubleArrayResult result = (DoubleArrayResult)Descriptor.Calculate(ac).GetValue();

            Assert.AreEqual(2.0880171, result[0], 0.000001);
            Assert.AreEqual(2.0880171, result[1], 0.000001);
        }

        // @cdk.bug 1965254
        [TestMethod()]
        public void TestLOBDescriptor2()
        {
            string filename = "NCDK.Data.MDL.lobtest2.sdf";
            var ins = ResourceLoader.GetAsStream(filename);
            ISimpleChemObjectReader reader = new MDLV2000Reader(ins);
            ChemFile content = (ChemFile)reader.Read(new ChemFile());
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToList();
            IAtomContainer ac = (IAtomContainer)cList[0];
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(ac);

            DoubleArrayResult result = (DoubleArrayResult)Descriptor.Calculate(ac).GetValue();
            Assert.IsNotNull(result);
        }
    }
}
