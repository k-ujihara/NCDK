/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Numerics;
using NCDK.Silent;

namespace NCDK.Charges
{
    /// <summary>
    /// TestSuite that runs a test for the <see cref="MMFF94PartialCharges"/>.
    /// </summary>
    // @cdk.module test-charges
    // @author        mfe4
    // @cdk.created       2004-11-04
    [TestClass()]
    public class InductivePartialChargesTest : CDKTestCase
    {
        private static IAtomContainer mol;

        static InductivePartialChargesTest()
        {
            IChemObjectBuilder builder = ChemObjectBuilder.Instance;
            mol = builder.NewAtomContainer();
            IAtom atom1 = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("Cl");
            IAtom atom3 = builder.NewAtom("Br");
            IAtom atom4 = builder.NewAtom("H");
            IAtom atom5 = builder.NewAtom("O");
            atom5.Point3D = new Vector3(2.24, 1.33, 0.0);
            atom1.Point3D = new Vector3(1.80, 0.0, 0.0);
            atom2.Point3D = new Vector3(0.0, 0.0, 0.0);
            atom3.Point3D = new Vector3(2.60, -0.79, 1.59);
            atom4.Point3D = new Vector3(2.15, -0.60, -0.87);

            IBond bond1 = builder.NewBond(atom1, atom2, BondOrder.Single);
            IBond bond2 = builder.NewBond(atom1, atom3, BondOrder.Single);
            IBond bond3 = builder.NewBond(atom1, atom4, BondOrder.Single);
            IBond bond4 = builder.NewBond(atom1, atom5, BondOrder.Single);

            mol.Atoms.Add(atom1);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.Atoms.Add(atom5);

            mol.Bonds.Add(bond1);
            mol.Bonds.Add(bond2);
            mol.Bonds.Add(bond3);
            mol.Bonds.Add(bond4);
        }

        /// <summary>
        ///  A unit test for JUnit with beta-amino-acetic-acid
        ///
        /// </summary>
        [TestMethod()]
        public void TestCalculateCharges_IAtomContainer()
        {
            double[] testResult = { 0.197, -0.492, 0.051, 0.099, 0.099 };
            Vector3 c_coord = new Vector3(1.392, 0.0, 0.0);
            Vector3 f_coord = new Vector3(0.0, 0.0, 0.0);
            Vector3 h1_coord = new Vector3(1.7439615035767404, 1.0558845107302222, 0.0);
            Vector3 h2_coord = new Vector3(1.7439615035767404, -0.5279422553651107, 0.914422809754875);
            Vector3 h3_coord = new Vector3(1.7439615035767402, -0.5279422553651113, -0.9144228097548747);

            var mol = new AtomContainer(); // molecule is CF

            Atom c = new Atom("C");
            mol.Atoms.Add(c);
            c.Point3D = c_coord;

            Atom f = new Atom("F");
            mol.Atoms.Add(f);
            f.Point3D = f_coord;

            Atom h1 = new Atom("H");
            mol.Atoms.Add(h1);
            h1.Point3D = h1_coord;

            Atom h2 = new Atom("H");
            mol.Atoms.Add(h2);
            h2.Point3D = h2_coord;

            Atom h3 = new Atom("H");
            mol.Atoms.Add(h3);
            h3.Point3D = h3_coord;

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single); // 1
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single); // 1
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single); // 1
            mol.AddBond(mol.Atoms[0], mol.Atoms[4], BondOrder.Single); // 1
            InductivePartialCharges ipc = new InductivePartialCharges();
            ipc.AssignInductivePartialCharges(mol);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                Assert.AreEqual(testResult[i],
                        (mol.Atoms[i].GetProperty<double>("InductivePartialCharge")), 0.1);
                //Debug.WriteLine("CHARGE AT " + ac.GetAtomAt(i).Symbol + " " + ac.GetAtomAt(i).GetProperty("MMFF94charge"));
            }
        }

        /// <summary>
        ///  A unit test for JUnit with beta-amino-acetic-acid
        /// </summary>
        [TestMethod()]
        public void TestInductivePartialCharges()
        {
            double[] testResult = { 0.197, -0.492, 0.051, 0.099, 0.099 };
            Vector3 c_coord = new Vector3(1.392, 0.0, 0.0);
            Vector3 f_coord = new Vector3(0.0, 0.0, 0.0);
            Vector3 h1_coord = new Vector3(1.7439615035767404, 1.0558845107302222, 0.0);
            Vector3 h2_coord = new Vector3(1.7439615035767404, -0.5279422553651107, 0.914422809754875);
            Vector3 h3_coord = new Vector3(1.7439615035767402, -0.5279422553651113, -0.9144228097548747);

            var mol = new AtomContainer(); // molecule is CF

            Atom c = new Atom("C");
            mol.Atoms.Add(c);
            c.Point3D = c_coord;

            Atom f = new Atom("F");
            mol.Atoms.Add(f);
            f.Point3D = f_coord;

            Atom h1 = new Atom("H");
            mol.Atoms.Add(h1);
            h1.Point3D = h1_coord;

            Atom h2 = new Atom("H");
            mol.Atoms.Add(h2);
            h2.Point3D = h2_coord;

            Atom h3 = new Atom("H");
            mol.Atoms.Add(h3);
            h3.Point3D = h3_coord;

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single); // 1
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single); // 1
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single); // 1
            mol.AddBond(mol.Atoms[0], mol.Atoms[4], BondOrder.Single); // 1
            InductivePartialCharges ipc = new InductivePartialCharges();
            ipc.AssignInductivePartialCharges(mol);
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                Assert.AreEqual(testResult[i],
                        (mol.Atoms[i].GetProperty<double>("InductivePartialCharge")), 0.1);
                //Debug.WriteLine("CHARGE AT " + ac.GetAtomAt(i).Symbol + " " + ac.GetAtomAt(i).GetProperty("MMFF94charge"));
            }
        }

        [TestMethod()]
        public void TestGetPaulingElectronegativities()
        {
            InductivePartialCharges ipc = new InductivePartialCharges();
            double[] eneg = ipc.GetPaulingElectronegativities(mol, true);
            long[] expected = { };
            Assert.AreEqual(2.20, eneg[0], 0.01, "Error in C electronegativity");
            Assert.AreEqual(3.28, eneg[1], 0.01, "Error in Cl electronegativity");
            Assert.AreEqual(3.13, eneg[2], 0.01, "Error in Br electronegativity");
            Assert.AreEqual(2.10, eneg[3], 0.01, "Error in H electronegativity");
            Assert.AreEqual(3.20, eneg[4], 0.01, "Error in O electronegativity");
        }

        [TestMethod(), Ignore()]
        public void TestGetAtomicSoftness()
        {
            InductivePartialCharges ipc = new InductivePartialCharges();
            double softness = ipc.GetAtomicSoftnessCore(mol, 0);
            Assert.Fail("Not validated - need known values");
        }
    }
}
