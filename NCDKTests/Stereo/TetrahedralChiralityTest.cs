/* Copyright (C) 2010  Egon Willighagen <egonw@users.sf.net>
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using System;

namespace NCDK.Stereo
{
    // @cdk.module test-core
    [TestClass()]
    public class TetrahedralChiralityTest : CDKTestCase
    {
        private static IAtomContainer molecule;
        private static IAtom[] ligands;

        static TetrahedralChiralityTest()
        {
            molecule = new AtomContainer();
            molecule.Atoms.Add(new Atom("Cl"));
            molecule.Atoms.Add(new Atom("C"));
            molecule.Atoms.Add(new Atom("Br"));
            molecule.Atoms.Add(new Atom("I"));
            molecule.Atoms.Add(new Atom("H"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[3], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[4], BondOrder.Single);
            ligands = new IAtom[] { molecule.Atoms[4], molecule.Atoms[3], molecule.Atoms[2], molecule.Atoms[0] };
        }

        [TestMethod()]
        public void TestTetrahedralChirality_IAtom_arrayIAtom_ITetrahedralChirality_Stereo()
        {
            TetrahedralChirality chirality = new TetrahedralChirality(molecule.Atoms[1], ligands, TetrahedralStereo.Clockwise);
            Assert.IsNotNull(chirality);
        }

        [TestMethod()]
        public void TestBuilder()
        {
            TetrahedralChirality chirality = new TetrahedralChirality(molecule.Atoms[1], ligands, TetrahedralStereo.Clockwise);
            Assert.IsNull(chirality.Builder);
            chirality.Builder = Default.ChemObjectBuilder.Instance;
            Assert.AreEqual(Default.ChemObjectBuilder.Instance, chirality.Builder);
        }

        [TestMethod()]
        public void TestGetChiralAtom()
        {
            TetrahedralChirality chirality = new TetrahedralChirality(molecule.Atoms[1], ligands, TetrahedralStereo.Clockwise);
            Assert.IsNotNull(chirality);
            Assert.AreEqual(molecule.Atoms[1], chirality.ChiralAtom);
        }

        [TestMethod()]
        public void TestGetStereo()
        {
            TetrahedralChirality chirality = new TetrahedralChirality(molecule.Atoms[1], ligands, TetrahedralStereo.Clockwise);
            Assert.IsNotNull(chirality);
            Assert.AreEqual(molecule.Atoms[1], chirality.ChiralAtom);
            for (int i = 0; i < ligands.Length; i++)
            {
                Assert.AreEqual(ligands[i], chirality.Ligands[i]);
            }
            Assert.AreEqual(TetrahedralStereo.Clockwise, chirality.Stereo);
        }

        [TestMethod()]
        public void TestGetLigands()
        {
            TetrahedralChirality chirality = new TetrahedralChirality(molecule.Atoms[1], ligands, TetrahedralStereo.Clockwise);
            Assert.IsNotNull(chirality);
            for (int i = 0; i < ligands.Length; i++)
            {
                Assert.AreEqual(ligands[i], chirality.Ligands[i]);
            }
        }

        [TestMethod()]
        public void TestMap_Map_Map()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;

            IAtom c1 = builder.CreateAtom("C");
            IAtom o2 = builder.CreateAtom("O");
            IAtom n3 = builder.CreateAtom("N");
            IAtom c4 = builder.CreateAtom("C");
            IAtom h5 = builder.CreateAtom("H");

            // new stereo element
            ITetrahedralChirality original = new TetrahedralChirality(c1, new IAtom[] { o2, n3, c4, h5 }, TetrahedralStereo.Clockwise);

            // clone the atoms and place in a map
            var mapping = new CDKObjectMap();

            // map the existing element a new element
            ITetrahedralChirality mapped = (ITetrahedralChirality)original.Clone(mapping);

            IAtom c1clone = mapping.AtomMap[c1];
            IAtom o2clone = mapping.AtomMap[o2];
            IAtom n3clone = mapping.AtomMap[n3];
            IAtom c4clone = mapping.AtomMap[c4];
            IAtom h5clone = mapping.AtomMap[h5];

            Assert.AreNotSame(original.ChiralAtom, mapped.ChiralAtom, "mapped chiral atom was the same as the original");
            Assert.AreSame(c1clone, mapped.ChiralAtom, "mapped chiral atom was not the clone");

            var originalLigands = original.Ligands;
            var mappedLigands = mapped.Ligands;

            Assert.AreNotSame(originalLigands[0], mappedLigands[0], "first ligand was the same as the original");
            Assert.AreSame(o2clone, mappedLigands[0], "first mapped ligand was not the clone");
            Assert.AreNotSame(originalLigands[1], mappedLigands[1], "second ligand was the same as the original");
            Assert.AreSame(n3clone, mappedLigands[1], "second mapped ligand was not the clone");
            Assert.AreNotSame(originalLigands[2], mappedLigands[2], "third ligand was the same as the original");
            Assert.AreSame(c4clone, mappedLigands[2], "third mapped ligand was not the clone");
            Assert.AreNotSame(originalLigands[3], mappedLigands[3], "forth ligand was te same as the original");
            Assert.AreSame(h5clone, mappedLigands[3], "forth mapped ligand was not the clone");
            Assert.AreEqual(original.Stereo, mapped.Stereo, "stereo was not mapped");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestMap_Null_Map()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;

            IAtom c1 = builder.CreateAtom("C");
            IAtom o2 = builder.CreateAtom("O");
            IAtom n3 = builder.CreateAtom("N");
            IAtom c4 = builder.CreateAtom("C");
            IAtom h5 = builder.CreateAtom("H");

            // new stereo element
            ITetrahedralChirality original = new TetrahedralChirality(c1, new IAtom[] { o2, n3, c4, h5 }, TetrahedralStereo.Clockwise);

            // map the existing element a new element 
            ITetrahedralChirality mapped = (ITetrahedralChirality)original.Clone(null);
        }

        [TestMethod()]
        public void TestMap_Map_Map_NullElement()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;

            IAtom c1 = builder.CreateAtom("C");
            IAtom o2 = builder.CreateAtom("O");
            IAtom n3 = builder.CreateAtom("N");
            IAtom c4 = builder.CreateAtom("C");
            IAtom h5 = builder.CreateAtom("H");

            // new stereo element
            ITetrahedralChirality original = new TetrahedralChirality(null, new IAtom[4], TetrahedralStereo.Unset);

            // map the existing element a new element
            ITetrahedralChirality mapped = (ITetrahedralChirality)original.Clone();

            Assert.IsNull(mapped.ChiralAtom);
            Assert.IsNull(mapped.Ligands[0]);
            Assert.IsNull(mapped.Ligands[1]);
            Assert.IsNull(mapped.Ligands[2]);
            Assert.IsNull(mapped.Ligands[3]);
            Assert.AreEqual(TetrahedralStereo.Unset, mapped.Stereo);
        }

        [TestMethod()]
        public void TestMap_Map_Map_EmptyMapping()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;

            IAtom c1 = builder.CreateAtom("C");
            IAtom o2 = builder.CreateAtom("O");
            IAtom n3 = builder.CreateAtom("N");
            IAtom c4 = builder.CreateAtom("C");
            IAtom h5 = builder.CreateAtom("H");

            // new stereo element
            ITetrahedralChirality original = new TetrahedralChirality(c1, new IAtom[] { o2, n3, c4, h5 }, TetrahedralStereo.Clockwise);

            // map the existing element a new element 
            ITetrahedralChirality mapped = (ITetrahedralChirality)original.Clone();

            Assert.IsNotNull(mapped.ChiralAtom);
            Assert.IsNotNull(mapped.Ligands[0]);
            Assert.IsNotNull(mapped.Ligands[1]);
            Assert.IsNotNull(mapped.Ligands[2]);
            Assert.IsNotNull(mapped.Ligands[3]);
            Assert.AreNotEqual(TetrahedralStereo.Unset, mapped.Stereo);
        }

        [TestMethod()]
        public void Contains()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;

            IAtom c1 = builder.CreateAtom("C");
            IAtom o2 = builder.CreateAtom("O");
            IAtom n3 = builder.CreateAtom("N");
            IAtom c4 = builder.CreateAtom("C");
            IAtom h5 = builder.CreateAtom("H");

            // new stereo element
            ITetrahedralChirality element = new TetrahedralChirality(c1, new IAtom[] { o2, n3, c4, h5 }, TetrahedralStereo.Clockwise);

            Assert.IsTrue(element.Contains(c1));
            Assert.IsTrue(element.Contains(o2));
            Assert.IsTrue(element.Contains(n3));
            Assert.IsTrue(element.Contains(c4));
            Assert.IsTrue(element.Contains(h5));

            Assert.IsFalse(element.Contains(builder.CreateAtom()));
            Assert.IsFalse(element.Contains(null));
        }

        [TestMethod()]
        public void TestToString()
        {
            TetrahedralChirality chirality = new TetrahedralChirality(molecule.Atoms[1], ligands, TetrahedralStereo.Clockwise);
            string stringRepr = chirality.ToString();
            Assert.AreNotSame(0, stringRepr.Length);
            Assert.IsFalse(stringRepr.Contains("\n"));
        }
    }
}
