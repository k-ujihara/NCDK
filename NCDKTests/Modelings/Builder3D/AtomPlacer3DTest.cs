/* Copyright (C) 2005-2007  Christian Hoppe <chhoppe@users.sf.net>
 *                    2014  Mark B Vine (orcid:0000-0002-7794-0426)
 *
 *  Contact: cdk-devel@list.sourceforge.net
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
using NCDK.Config;
using NCDK.IO;
using NCDK.Tools.Manipulator;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NCDK.Numerics;

namespace NCDK.Modelings.Builder3D
{
    /// <summary>
    /// Tests for AtomPlacer3D
    /// </summary>
    // @cdk.module test-builder3d
    [TestClass()]
    public class AtomPlacer3DTest 
        : CDKTestCase
    {
        private readonly IChemObjectBuilder builder = CDK.Builder;
        bool standAlone = false;

        /// <summary>
        ///  Sets the standAlone attribute
        /// </summary>
        /// <param name="standAlone">The new standAlone value</param>
        public void SetStandAlone(bool standAlone)
        {
            this.standAlone = standAlone;
        }

        /// <summary>
        /// Create a test molecule (alpha-pinene).
        /// This code has been inlined from MoleculeFactory.java
        /// </summary>
        /// <returns>the created test molecule</returns>
        private IAtomContainer MakeAlphaPinene()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("C"));

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[4], mol.Atoms[5], BondOrder.Single);
            mol.AddBond(mol.Atoms[5], mol.Atoms[0], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[6], BondOrder.Single);
            mol.AddBond(mol.Atoms[3], mol.Atoms[7], BondOrder.Single);
            mol.AddBond(mol.Atoms[5], mol.Atoms[7], BondOrder.Single);
            mol.AddBond(mol.Atoms[7], mol.Atoms[8], BondOrder.Single);
            mol.AddBond(mol.Atoms[7], mol.Atoms[9], BondOrder.Single);
            try
            {
                BODRIsotopeFactory.Instance.ConfigureAtoms(mol);
            }
            catch (IOException ex)
            {
                Trace.TraceError(ex.Message);
            }
            return mol;
        }

        private IAtomContainer MakeMethaneWithExplicitHydrogens()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("H"));

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[4], BondOrder.Single);

            return mol;
        }

        [TestMethod()]
        public void TestAllHeavyAtomsPlaced_IAtomContainer()
        {
            var ac = MakeAlphaPinene();
            Assert.IsFalse(new AtomPlacer3D().AllHeavyAtomsPlaced(ac));
            foreach (var atom in ac.Atoms)
            {
                atom.IsPlaced = true;
            }
            Assert.IsTrue(new AtomPlacer3D().AllHeavyAtomsPlaced(ac));
        }

        [TestMethod()]
        public void TestFindHeavyAtomsInChain_IAtomContainer_IAtomContainer()
        {
            var filename = "NCDK.Data.MDL.allmol232.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            // TODO: shk3-cleanuptests: best to use the STRICT IO mode here
            var reader = new MDLV2000Reader(ins);
            var chemFile = reader.Read(builder.NewChemFile());
            reader.Close();
            var containersList = ChemFileManipulator.GetAllAtomContainers(chemFile);
            var ac = new Silent.AtomContainer(containersList.First());
            AddExplicitHydrogens(ac);
            var chain = ac.Builder.NewAtomContainer();
            for (int i = 16; i < 25; i++)
            {
                chain.Atoms.Add(ac.Atoms[i]);
            }
            chain.Atoms.Add(ac.Atoms[29]);
            chain.Atoms.Add(ac.Atoms[30]);
            int[] result = new AtomPlacer3D().FindHeavyAtomsInChain(ac, chain);
            Assert.AreEqual(16, result[0]);
            Assert.AreEqual(11, result[1]);
        }

        [TestMethod()]
        public virtual void TestNumberOfUnplacedHeavyAtoms_IAtomContainer()
        {
            var ac = MakeAlphaPinene();
            var count = new AtomPlacer3D().NumberOfUnplacedHeavyAtoms(ac);
            Assert.AreEqual(10, count);
        }

        /// <summary>
        /// Demonstrate bug where <see cref="AtomPlacer3D.NumberOfUnplacedHeavyAtoms(IAtomContainer)"/> counts
        /// explicit hydrogens as heavy atoms.
        /// </summary>
        [TestMethod()]
        public virtual void TestNumberOfUnplacedHeavyAtoms_IAtomContainerWithExplicitHydrogens()
        {
            var ac = MakeMethaneWithExplicitHydrogens();
            var count = new AtomPlacer3D().NumberOfUnplacedHeavyAtoms(ac);
            Assert.AreEqual(1, count);
        }

        [TestMethod()]
        public virtual void TestGetPlacedHeavyAtoms_IAtomContainer_IAtom()
        {
            var ac = MakeAlphaPinene();
            var acplaced = new AtomPlacer3D().GetPlacedHeavyAtoms(ac, ac.Atoms[0]);
            Assert.AreEqual(0, acplaced.Atoms.Count);
            ac.Atoms[1].IsPlaced = true;
            acplaced = new AtomPlacer3D().GetPlacedHeavyAtoms(ac, ac.Atoms[0]);
            Assert.AreEqual(1, acplaced.Atoms.Count);
        }

        [TestMethod()]
        public virtual void TestGetPlacedHeavyAtom_IAtomContainer_IAtom_IAtom()
        {
            var ac = MakeAlphaPinene();
            var acplaced = new AtomPlacer3D().GetPlacedHeavyAtom(ac, ac.Atoms[0], ac.Atoms[1]);
            Assert.IsNull(acplaced);
            ac.Atoms[1].IsPlaced = true;
            acplaced = new AtomPlacer3D().GetPlacedHeavyAtom(ac, ac.Atoms[0], ac.Atoms[2]);
            Assert.AreEqual(ac.Atoms[1], acplaced);
            acplaced = new AtomPlacer3D().GetPlacedHeavyAtom(ac, ac.Atoms[0], ac.Atoms[1]);
            Assert.IsNull(acplaced);
        }

        [TestMethod()]
        public virtual void TestGetPlacedHeavyAtom_IAtomContainer_IAtom()
        {
            var ac = MakeAlphaPinene();
            var acplaced = new AtomPlacer3D().GetPlacedHeavyAtom(ac, ac.Atoms[0]);
            Assert.IsNull(acplaced);
            ac.Atoms[1].IsPlaced = true;
            acplaced = new AtomPlacer3D().GetPlacedHeavyAtom(ac, ac.Atoms[0]);
            Assert.AreEqual(ac.Atoms[1], acplaced);
        }

        [TestMethod()]
        public virtual void TestGeometricCenterAllPlacedAtoms_IAtomContainer()
        {
            var ac = MakeAlphaPinene();
            for (int i = 0; i < ac.Atoms.Count; i++)
            {
                ac.Atoms[i].IsPlaced = true;
            }
            ac.Atoms[0].Point3D = new Vector3(1.39, 2.04, 0);
            ac.Atoms[0].Point3D = new Vector3(2.02, 2.28, -1.12);
            ac.Atoms[0].Point3D = new Vector3(3.44, 2.80, -1.09);
            ac.Atoms[0].Point3D = new Vector3(3.91, 2.97, 0.35);
            ac.Atoms[0].Point3D = new Vector3(3.56, 1.71, 1.16);
            ac.Atoms[0].Point3D = new Vector3(2.14, 2.31, 1.29);
            ac.Atoms[0].Point3D = new Vector3(0, 1.53, 0);
            ac.Atoms[0].Point3D = new Vector3(2.83, 3.69, 1.17);
            ac.Atoms[0].Point3D = new Vector3(3.32, 4.27, 2.49);
            ac.Atoms[0].Point3D = new Vector3(2.02, 4.68, 0.35);
            var center = new AtomPlacer3D().GeometricCenterAllPlacedAtoms(ac);
            Assert.AreEqual(2.02, center.X, 0.01);
            Assert.AreEqual(4.68, center.Y, 0.01);
            Assert.AreEqual(0.35, center.Z, 0.01);
        }

        [TestMethod()]
        public void TestIsUnplacedHeavyAtom()
        {
            var ac = MakeMethaneWithExplicitHydrogens();
            var carbon = ac.Atoms[0];
            var hydrogen = ac.Atoms[1];
            AtomPlacer3D placer = new AtomPlacer3D();

            bool result = false;
            result = placer.IsUnplacedHeavyAtom(carbon);
            Assert.IsTrue(result);
            result = placer.IsUnplacedHeavyAtom(hydrogen);
            Assert.IsFalse(result);

            carbon.IsPlaced = true;
            result = placer.IsUnplacedHeavyAtom(carbon);
            Assert.IsFalse(result);
            hydrogen.IsPlaced = true;
            result = placer.IsUnplacedHeavyAtom(hydrogen);
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void TestIsPlacedHeavyAtom()
        {
            var ac = MakeMethaneWithExplicitHydrogens();
            var carbon = ac.Atoms[0];
            var hydrogen = ac.Atoms[1];
            var placer = new AtomPlacer3D();

            bool result = false;
            result = placer.IsPlacedHeavyAtom(carbon);
            Assert.IsFalse(result);
            result = placer.IsPlacedHeavyAtom(hydrogen);
            Assert.IsFalse(result);

            carbon.IsPlaced = true;
            result = placer.IsPlacedHeavyAtom(carbon);
            Assert.IsTrue(result);
            hydrogen.IsPlaced = true;
            result = placer.IsPlacedHeavyAtom(hydrogen);
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void TestIsAliphaticHeavyAtom()
        {
            var ac = MakeMethaneWithExplicitHydrogens();
            var carbon = ac.Atoms[0];
            var hydrogen = ac.Atoms[1];
            var placer = new AtomPlacer3D();

            bool result = false;
            result = placer.IsAliphaticHeavyAtom(carbon);
            Assert.IsFalse(result);
            result = placer.IsAliphaticHeavyAtom(hydrogen);
            Assert.IsFalse(result);

            carbon.IsAliphatic = true;
            result = placer.IsAliphaticHeavyAtom(carbon);
            Assert.IsTrue(result);
            hydrogen.IsAliphatic = true;
            result = placer.IsAliphaticHeavyAtom(hydrogen);
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void TestIsRingHeavyAtom()
        {

            var ac = MakeMethaneWithExplicitHydrogens();
            var carbon = ac.Atoms[0];
            var hydrogen = ac.Atoms[1];
            var placer = new AtomPlacer3D();

            bool result = false;
            result = placer.IsRingHeavyAtom(carbon);
            Assert.IsFalse(result);
            result = placer.IsRingHeavyAtom(hydrogen);
            Assert.IsFalse(result);

            carbon.IsInRing = true;
            result = placer.IsRingHeavyAtom(carbon);
            Assert.IsTrue(result);
            hydrogen.IsInRing = true;
            result = placer.IsRingHeavyAtom(hydrogen);
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void TestIsHeavyAtom()
        {

            var ac = MakeMethaneWithExplicitHydrogens();
            var carbon = ac.Atoms[0];
            var hydrogen = ac.Atoms[1];
            var placer = new AtomPlacer3D();

            bool result = false;
            result = placer.IsHeavyAtom(carbon);
            Assert.IsTrue(result);
            result = placer.IsHeavyAtom(hydrogen);
            Assert.IsFalse(result);
        }
    }
}
