using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.IO;
using NCDK.Numerics;
using NCDK.Tools.Manipulator;
using System.Linq;

namespace NCDK.Geometries.Alignments
{
    /// <summary>
    /// This class defines regression tests that should ensure that the source code
    /// of the org.openscience.cdk.geometry.alignment.KabschAlignment is not broken.
    /// </summary>
    /// <seealso cref="KabschAlignment"/>
    // @cdk.module test-extra
    // @author     Rajarshi Guha
    // @cdk.created    2004-12-11
    [TestClass()]
    public class KabschAlignmentTest : CDKTestCase
    {
        [TestMethod()]
        public void TestAlign()
        {
            {
                IAtomContainer ac;
                string filename = "NCDK.Data.HIN.gravindex.hin";
                var ins = ResourceLoader.GetAsStream(filename);
                ISimpleChemObjectReader reader = new HINReader(ins);
                ChemFile content = (ChemFile)reader.Read((ChemObject)new ChemFile());
                var cList = ChemFileManipulator.GetAllAtomContainers(content);
                ac = cList.First();

                KabschAlignment ka = new KabschAlignment(ac, ac);
                Assert.IsNotNull(ka);
                ka.Align();
                double rmsd = ka.RMSD;
                Assert.IsTrue(1e-8 > rmsd);
                Assert.IsNotNull(ka.RotationMatrix);
            }

            {
                double[][] p1 = new[] {new[] {16.754, 20.462, 45.049}, new[] {19.609, 18.145, 46.011}, new[] {17.101, 17.256, 48.707},
                new[] {13.963, 18.314, 46.820}, new[] {14.151, 15.343, 44.482}, new[] {14.959, 12.459, 46.880}, new[] {11.987, 13.842, 48.862},
                new[] {9.586, 12.770, 46.123}, new[] {11.006, 9.245, 46.116}, new[] {10.755, 9.090, 49.885}};
                double[][] p2 = new[] {new[] {70.246, 317.510, 188.263}, new[] {73.457, 317.369, 190.340}, new[] {71.257, 318.976, 193.018},
                new[] {68.053, 317.543, 191.651}, new[] {68.786, 313.954, 192.637}, new[] {70.248, 314.486, 196.151},
                new[] {67.115, 316.584, 196.561}, new[] {64.806, 313.610, 196.423}, new[] {66.804, 311.735, 199.035},
                new[] {66.863, 314.832, 201.113}};
                Atom[] a1 = new Atom[10];
                Atom[] a2 = new Atom[10];
                for (int i = 0; i < 10; i++)
                {
                    a1[i] = new Atom("C");
                    Vector3 newCoord = new Vector3();
                    newCoord.X = p1[i][0];
                    newCoord.Y = p1[i][1];
                    newCoord.Z = p1[i][2];
                    a1[i].Point3D = newCoord;
                    a2[i] = new Atom("C");
                    newCoord = new Vector3();
                    newCoord.X = p2[i][0];
                    newCoord.Y = p2[i][1];
                    newCoord.Z = p2[i][2];
                    a2[i].Point3D = newCoord;
                }
                var ka = new KabschAlignment(a1, a2);
                ka.Align();
                var rmsd = ka.RMSD;
                Assert.AreEqual(0.13479726, rmsd, 0.00000001);
            }
        }
    }
}
