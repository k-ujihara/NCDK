using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.IO;
using NCDK.Tools.Manipulator;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NCDK.Similarity
{
    // @cdk.module test-fingerprint
    [TestClass()]
    public class DistanceMomentTest : CDKTestCase
    {
        private IAtomContainer LoadMolecule(string path)
        {
            Stream ins = ResourceLoader.GetAsStream(path);
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
            ChemFile chemFile = (ChemFile)reader.Read((ChemObject)new ChemFile());
            reader.Close();
            var containersList = ChemFileManipulator.GetAllAtomContainers(chemFile).ToList();
            return (IAtomContainer)containersList[0];
        }

        [TestMethod()]
        public void Test3DSim1()
        {
            string filename = "NCDK.Data.MDL.sim3d1.sdf";
            IAtomContainer ac = LoadMolecule(filename);
            var sim = DistanceMoment.Calculate(ac, ac);
            Assert.AreEqual(1.0000, sim, 0.00001);
        }

        [TestMethod()]
        public void TestGenerateMoments()
        {
            string filename = "NCDK.Data.MDL.sim3d1.sdf";
            IAtomContainer ac = LoadMolecule(filename);
            var expected = new double[]{3.710034f, 1.780116f, 0.26535583f, 3.7945938f, 2.2801101f, 0.20164771f, 7.1209f,
                9.234152f, -0.49032924f, 6.6067924f, 8.89391f, -0.048539735f};
            var actual = DistanceMoment.GenerateMoments(ac);

            // no assertArrayEquals for junit 4.5
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i], 0.000001);
            }
        }

        [TestMethod()]
        public void Test3DSim2()
        {
            IAtomContainer ac1 = LoadMolecule("NCDK.Data.MDL.sim3d1.sdf");
            IAtomContainer ac2 = LoadMolecule("NCDK.Data.MDL.sim3d2.sdf");
            var sim = DistanceMoment.Calculate(ac1, ac2);
            Assert.AreEqual(0.24962, sim, 0.00001);
        }
    }
}
