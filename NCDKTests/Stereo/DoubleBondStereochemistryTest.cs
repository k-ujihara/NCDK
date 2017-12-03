/* Copyright (C) 2012  Egon Willighagen <egonw@users.sf.net>
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
    public class DoubleBondStereochemistryTest : CDKTestCase
    {
        private static IAtomContainer molecule;
        private static IBond[] ligands;

        /// <summary>
        /// This method creates <i>E</i>-but-2-ene.
        /// </summary>
        static DoubleBondStereochemistryTest()
        {
            molecule = new AtomContainer();
            molecule.Atoms.Add(new Atom("C"));
            molecule.Atoms.Add(new Atom("C"));
            molecule.Atoms.Add(new Atom("C"));
            molecule.Atoms.Add(new Atom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);
            ligands = new IBond[] { molecule.Bonds[0], molecule.Bonds[2] };
        }

        /// <summary>
        /// Unit test ensures an exception is thrown if more the two elements are
        /// passed to the constructor. When IDoubleBondStereoChemistry.Bonds
        /// is invoked the fixed size array is copied to an array of size 2. If
        /// more then 2 bonds are given they would be truncated.
        /// </summary>
        // @cdk.bug 1273
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstructor_TooManyBonds()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            IBond b1 = builder.NewBond();
            IBond b2 = builder.NewBond();
            IBond b3 = builder.NewBond();
            new DoubleBondStereochemistry(builder.NewBond(), new IBond[] { b1, b2, b3 }, DoubleBondConformation.Opposite);
        }

        [TestMethod()]
        public void TestConstructor()
        {
            DoubleBondStereochemistry stereo = new DoubleBondStereochemistry(molecule.Bonds[1], ligands,
                    DoubleBondConformation.Opposite);
            Assert.IsNotNull(stereo);
        }

        [TestMethod()]
        public void TestBuilder()
        {
            DoubleBondStereochemistry stereo = new DoubleBondStereochemistry(molecule.Bonds[1], ligands,
                    DoubleBondConformation.Opposite);
            stereo.Builder = Default.ChemObjectBuilder.Instance;
            Assert.AreEqual(Default.ChemObjectBuilder.Instance, stereo.Builder);
        }

        [TestMethod()]
        public void TestGetStereoBond()
        {
            DoubleBondStereochemistry stereo = new DoubleBondStereochemistry(molecule.Bonds[1], ligands,
                    DoubleBondConformation.Opposite);
            Assert.IsNotNull(stereo);
            Assert.AreEqual(molecule.Bonds[1], stereo.StereoBond);
        }

        [TestMethod()]
        public void TestGetStereo()
        {
            DoubleBondStereochemistry stereo = new DoubleBondStereochemistry(molecule.Bonds[1], ligands,
                    DoubleBondConformation.Opposite);
            Assert.IsNotNull(stereo);
            Assert.AreEqual(DoubleBondConformation.Opposite, stereo.Stereo);
        }

        [TestMethod()]
        public void TestGetBonds()
        {
            DoubleBondStereochemistry stereo = new DoubleBondStereochemistry(molecule.Bonds[1], ligands,
                    DoubleBondConformation.Opposite);
            Assert.IsNotNull(stereo);
            for (int i = 0; i < ligands.Length; i++)
            {
                Assert.AreEqual(ligands[i], stereo.Bonds[i]);
            }
        }

        [TestMethod()]
        public void Contains()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;

            IAtom c1 = builder.NewAtom("C");
            IAtom c2 = builder.NewAtom("C");
            IAtom o3 = builder.NewAtom("O");
            IAtom o4 = builder.NewAtom("O");

            IBond c1c2 = builder.NewBond(c1, c2, BondOrder.Double);
            IBond c1o3 = builder.NewBond(c1, o3, BondOrder.Single);
            IBond c2o4 = builder.NewBond(c2, o4, BondOrder.Single);

            // new stereo element
            DoubleBondStereochemistry element = new DoubleBondStereochemistry(c1c2, new IBond[] { c1o3, c2o4 },
                    DoubleBondConformation.Opposite);

            Assert.IsTrue(element.Contains(c1));
            Assert.IsTrue(element.Contains(c2));
            Assert.IsTrue(element.Contains(o3));
            Assert.IsTrue(element.Contains(o4));

            Assert.IsFalse(element.Contains(builder.NewAtom()));
            Assert.IsFalse(element.Contains(null));
        }

        [TestMethod()]
        public void TestMap_Map_Map_EmptyMapping()
        {
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;

            IAtom c1 = builder.NewAtom("C");
            IAtom c2 = builder.NewAtom("C");
            IAtom o3 = builder.NewAtom("O");
            IAtom o4 = builder.NewAtom("O");

            IBond c1c2 = builder.NewBond(c1, c2, BondOrder.Double);
            IBond c1o3 = builder.NewBond(c1, o3, BondOrder.Single);
            IBond c2o4 = builder.NewBond(c2, o4, BondOrder.Single);

            // new stereo element
            IDoubleBondStereochemistry original = new DoubleBondStereochemistry(c1c2, new IBond[] { c1o3, c2o4 },
                    DoubleBondConformation.Opposite);

            // map the existing element a new element - should through an ArgumentException
            IDoubleBondStereochemistry mapped = (IDoubleBondStereochemistry)original.Clone(new CDKObjectMap());

            Assert.AreSame(original, mapped);
        }

        [TestMethod()]
        public void TestToString()
        {
            DoubleBondStereochemistry stereo = new DoubleBondStereochemistry(molecule.Bonds[1], ligands,
                    DoubleBondConformation.Opposite);
            string stringRepr = stereo.ToString();
            Assert.AreNotSame(0, stringRepr.Length);
            Assert.IsFalse(stringRepr.Contains("\n"));
        }
    }
}
