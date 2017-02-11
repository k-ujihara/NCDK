/* Copyright (C) 2008  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.LibIO.CML;
using NCDK.Tools.Diff;

namespace NCDK.IO.CML
{
    // @cdk.module test-libiocml
    [TestClass()]
    public class CDKRoundTripTest : CDKTestCase
    {
        private static IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;

        private static Convertor convertor = new Convertor(false, "");

        [TestMethod()]
        public void TestIElement_Symbol()
        {
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom atom = builder.CreateAtom("C");
            atom.Id = "a1";
            mol.Atoms.Add(atom);
            IAtomContainer copy = CMLRoundTripTool.RoundTripMolecule(convertor, mol);
            string difference = AtomDiff.Diff(atom, copy.Atoms[0]);
            Assert.AreEqual(0, difference.Length, "Found non-zero diff: " + difference);
        }

        [TestMethod()]
        public void TestIElement_AtomicNumber()
        {
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom atom = builder.CreateAtom("C");
            atom.Id = "a1";
            atom.AtomicNumber = 6;
            mol.Atoms.Add(atom);
            IAtomContainer copy = CMLRoundTripTool.RoundTripMolecule(convertor, mol);
            string difference = AtomDiff.Diff(atom, copy.Atoms[0]);
            Assert.AreEqual(0, difference.Length, "Found non-zero diff: " + difference);
        }

        //@Ignore
        //[TestMethod()]
        public void TestIIsotope_NaturalAbundance()
        {
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom atom = builder.CreateAtom("C");
            atom.Id = "a1";
            atom.NaturalAbundance = 99;
            mol.Atoms.Add(atom);
            IAtomContainer copy = CMLRoundTripTool.RoundTripMolecule(convertor, mol);
            string difference = AtomDiff.Diff(atom, copy.Atoms[0]);
            Assert.AreEqual(0, difference.Length, "Found non-zero diff: " + difference);
        }

        //@Ignore("exact mass not currently supported in CML implmenetation")
        //[TestMethod()]
        public void TestIIsotope_ExactMass()
        {
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom atom = builder.CreateAtom("C");
            atom.Id = "a1";
            atom.ExactMass = 12;
            mol.Atoms.Add(atom);
            IAtomContainer copy = CMLRoundTripTool.RoundTripMolecule(convertor, mol);
            string difference = AtomDiff.Diff(atom, copy.Atoms[0]);
            Assert.AreEqual(0, difference.Length, "Found non-zero diff: " + difference);
        }

        [TestMethod()]
        public void TestIIsotope_MassNumber()
        {
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom atom = builder.CreateAtom("C");
            atom.Id = "a1";
            atom.MassNumber = 13;
            mol.Atoms.Add(atom);
            IAtomContainer copy = CMLRoundTripTool.RoundTripMolecule(convertor, mol);
            string difference = AtomDiff.Diff(atom, copy.Atoms[0]);
            Assert.AreEqual(0, difference.Length, "Found non-zero diff: " + difference);
        }

        //@Ignore
        //[TestMethod()]
        public void TestIAtomType_Name()
        {
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom atom = builder.CreateAtom("C");
            atom.Id = "a1";
            atom.AtomTypeName = "C.sp3";
            mol.Atoms.Add(atom);
            IAtomContainer copy = CMLRoundTripTool.RoundTripMolecule(convertor, mol);
            string difference = AtomDiff.Diff(atom, copy.Atoms[0]);
            Assert.AreEqual(0, difference.Length, "Found non-zero diff: " + difference);
        }

        //@Ignore
        //[TestMethod()]
        public void TestIAtomType_MaxBondOrder()
        {
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom atom = builder.CreateAtom("C");
            atom.Id = "a1";
            atom.MaxBondOrder = BondOrder.Triple;
            mol.Atoms.Add(atom);
            IAtomContainer copy = CMLRoundTripTool.RoundTripMolecule(convertor, mol);
            string difference = AtomDiff.Diff(atom, copy.Atoms[0]);
            Assert.AreEqual(0, difference.Length, "Found non-zero diff: " + difference);
        }

        //@Ignore
        //[TestMethod()]
        public void TestIAtomType_BondOrderSum()
        {
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom atom = builder.CreateAtom("C");
            atom.Id = "a1";
            atom.BondOrderSum = 4;
            mol.Atoms.Add(atom);
            IAtomContainer copy = CMLRoundTripTool.RoundTripMolecule(convertor, mol);
            string difference = AtomDiff.Diff(atom, copy.Atoms[0]);
            Assert.AreEqual(0, difference.Length, "Found non-zero diff: " + difference);
        }

        [TestMethod()]
        public void TestIAtomType_FormalCharge()
        {
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom atom = builder.CreateAtom("C");
            atom.Id = "a1";
            atom.FormalCharge = +1;
            mol.Atoms.Add(atom);
            IAtomContainer copy = CMLRoundTripTool.RoundTripMolecule(convertor, mol);
            string difference = AtomDiff.Diff(atom, copy.Atoms[0]);
            Assert.AreEqual(0, difference.Length, "Found non-zero diff: " + difference);
        }

        //@Ignore
        //[TestMethod()]
        public void TestIAtomType_FormalNeighborCount()
        {
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom atom = builder.CreateAtom("C");
            atom.Id = "a1";
            atom.FormalNeighbourCount = 4;
            mol.Atoms.Add(atom);
            IAtomContainer copy = CMLRoundTripTool.RoundTripMolecule(convertor, mol);
            string difference = AtomDiff.Diff(atom, copy.Atoms[0]);
            Assert.AreEqual(0, difference.Length, "Found non-zero diff: " + difference);
        }

        //@Ignore
        //[TestMethod()]
        public void TestIAtomType_Hybridization()
        {
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom atom = builder.CreateAtom("C");
            atom.Id = "a1";
            atom.Hybridization = Hybridization.SP3;
            mol.Atoms.Add(atom);
            IAtomContainer copy = CMLRoundTripTool.RoundTripMolecule(convertor, mol);
            string difference = AtomDiff.Diff(atom, copy.Atoms[0]);
            Assert.AreEqual(0, difference.Length, "Found non-zero diff: " + difference);
        }

        //@Ignore
        //[TestMethod()]
        public void TestIAtomType_CovalentRadius()
        {
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom atom = builder.CreateAtom("C");
            atom.Id = "a1";
            atom.CovalentRadius = 1.5;
            mol.Atoms.Add(atom);
            IAtomContainer copy = CMLRoundTripTool.RoundTripMolecule(convertor, mol);
            string difference = AtomDiff.Diff(atom, copy.Atoms[0]);
            Assert.AreEqual(0, difference.Length, "Found non-zero diff: " + difference);
        }

        //@Ignore
        //[TestMethod()]
        public void TestIAtomType_Valency()
        {
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom atom = builder.CreateAtom("C");
            atom.Id = "a1";
            atom.Valency = 4;
            mol.Atoms.Add(atom);
            IAtomContainer copy = CMLRoundTripTool.RoundTripMolecule(convertor, mol);
            string difference = AtomDiff.Diff(atom, copy.Atoms[0]);
            Assert.AreEqual(0, difference.Length, "Found non-zero diff: " + difference);
        }

        [TestMethod()]
        public void TestIAtom_Charge()
        {
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom atom = builder.CreateAtom("C");
            atom.Id = "a1";
            atom.Charge = 0.3;
            mol.Atoms.Add(atom);
            IAtomContainer copy = CMLRoundTripTool.RoundTripMolecule(convertor, mol);
            string difference = AtomDiff.Diff(atom, copy.Atoms[0]);
            Assert.AreEqual(0, difference.Length, "Found non-zero diff: " + difference);
        }

        [TestMethod()]
        public void TestIAtom_HydrogenCount()
        {
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom atom = builder.CreateAtom("C");
            atom.Id = "a1";
            atom.ImplicitHydrogenCount = 4;
            mol.Atoms.Add(atom);
            IAtomContainer copy = CMLRoundTripTool.RoundTripMolecule(convertor, mol);
            string difference = AtomDiff.Diff(atom, copy.Atoms[0]);
            Assert.AreEqual(0, difference.Length, "Found non-zero diff: " + difference);
        }

        [TestMethod()]
        public void TestIAtom_Point2d()
        {
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom atom = builder.CreateAtom("C");
            atom.Id = "a1";
            atom.Point2D = new Vector2(1.0, 2.0);
            mol.Atoms.Add(atom);
            IAtomContainer copy = CMLRoundTripTool.RoundTripMolecule(convertor, mol);
            string difference = AtomDiff.Diff(atom, copy.Atoms[0]);
            Assert.AreEqual(0, difference.Length, "Found non-zero diff: " + difference);
        }

        [TestMethod()]
        public void TestIAtom_Point3d()
        {
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom atom = builder.CreateAtom("C");
            atom.Id = "a1";
            atom.Point3D = new Vector3(1, 2, 3);
            mol.Atoms.Add(atom);
            IAtomContainer copy = CMLRoundTripTool.RoundTripMolecule(convertor, mol);
            string difference = AtomDiff.Diff(atom, copy.Atoms[0]);
            Assert.AreEqual(0, difference.Length, "Found non-zero diff: " + difference);
        }

        [TestMethod()]
        public void TestIAtom_FractionalPoint3d()
        {
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom atom = builder.CreateAtom("C");
            atom.Id = "a1";
            atom.FractionalPoint3D = new Vector3(1, 2, 3);
            mol.Atoms.Add(atom);
            IAtomContainer copy = CMLRoundTripTool.RoundTripMolecule(convertor, mol);
            string difference = AtomDiff.Diff(atom, copy.Atoms[0]);
            Assert.AreEqual(0, difference.Length, "Found non-zero diff: " + difference);
        }

        [TestMethod()]
        public void TestIAtom_Point8d()
        {
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom atom = builder.CreateAtom("C");
            atom.Id = "a1";
            atom.Point2D = new Vector2(0.0, 0.0);
            atom.Point3D = new Vector3(-1, -2, -3);
            atom.FractionalPoint3D = new Vector3(1, 2, 3);
            mol.Atoms.Add(atom);
            IAtomContainer copy = CMLRoundTripTool.RoundTripMolecule(convertor, mol);
            string difference = AtomDiff.Diff(atom, copy.Atoms[0]);
            Assert.AreEqual(0, difference.Length, "Found non-zero diff: " + difference);
        }

        //@Ignore
        //[TestMethod()]
        public void TestIAtom_StereoParity()
        {
            IAtomContainer mol = builder.CreateAtomContainer();
            IAtom atom = builder.CreateAtom("C");
            atom.Id = "a1";
            atom.StereoParity = -1;
            mol.Atoms.Add(atom);
            IAtomContainer copy = CMLRoundTripTool.RoundTripMolecule(convertor, mol);
            string difference = AtomDiff.Diff(atom, copy.Atoms[0]);
            Assert.AreEqual(0, difference.Length, "Found non-zero diff: " + difference);
        }
    }
}
