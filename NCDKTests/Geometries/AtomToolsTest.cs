/*
 * =====================================
 *  Copyright (c) 2020 NextMove Software
 * =====================================
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Config;
using NCDK.IO;
using NCDK.Numerics;
using System.IO;

namespace NCDK.Geometries
{
    [TestClass()]
    public class AtomToolsTest
    {
        [TestMethod()]
        public void Calculate3DCoordinates3()
        {
            const string molfile = "\n" +
                    "  Mrv1810 12152010123D          \n" +
                    "\n" +
                    "  4  3  0  0  0  0            999 V2000\n" +
                    "    2.0575    1.4744   -0.0102 O   0  0  0  0  0  0  0  0  0  0  0  0\n" +
                    "    1.5159    0.0200    0.0346 C   0  0  1  0  0  0  0  0  0  0  0  0\n" +
                    "    2.0575   -0.7460    1.2717 N   0  0  0  0  0  0  0  0  0  0  0  0\n" +
                    "   -0.0359   -0.0059   -0.0102 C   0  0  0  0  0  0  0  0  0  0  0  0\n" +
                    "  1  2  1  0  0  0  0\n" +
                    "  2  3  1  0  0  0  0\n" +
                    "  2  4  1  0  0  0  0\n" +
                    "M  END\n";
            using (var mdlr = new MDLV2000Reader(new StringReader(molfile)))
            {
                IAtomContainer mol = mdlr.Read(CDK.Builder.NewAtomContainer());
                var newP = AtomTools.Calculate3DCoordinates3(
                    mol.Atoms[1].Point3D.Value,
                    mol.Atoms[0].Point3D.Value,
                    mol.Atoms[2].Point3D.Value,
                    mol.Atoms[3].Point3D.Value, 
                    1.5);
                Assert.IsNotNull(newP);
                Assert.AreEqual(2.0160, newP.Value.X, 0.001);
                Assert.AreEqual(-0.6871, newP.Value.Y, 0.001);
                Assert.AreEqual(-1.1901, newP.Value.Z, 0.001);
            }
        }
    }
}
