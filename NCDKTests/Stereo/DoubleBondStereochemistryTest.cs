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
using System.Collections.Generic;

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
        ///
        // @cdk.bug 1273
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstructor_TooManyBonds()
        {

            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;

            new DoubleBondStereochemistry(builder.CreateBond(), new IBond[3], DoubleBondConformation.Opposite);
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
            Assert.IsNull(stereo.Builder);
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

            IAtom c1 = builder.CreateAtom("C");
            IAtom c2 = builder.CreateAtom("C");
            IAtom o3 = builder.CreateAtom("O");
            IAtom o4 = builder.CreateAtom("O");

            IBond c1c2 = builder.CreateBond(c1, c2, BondOrder.Double);
            IBond c1o3 = builder.CreateBond(c1, o3, BondOrder.Single);
            IBond c2o4 = builder.CreateBond(c2, o4, BondOrder.Single);

            // new stereo element
            DoubleBondStereochemistry element = new DoubleBondStereochemistry(c1c2, new IBond[] { c1o3, c2o4 },
                    DoubleBondConformation.Opposite);

            Assert.IsTrue(element.Contains(c1));
            Assert.IsTrue(element.Contains(c2));
            Assert.IsTrue(element.Contains(o3));
            Assert.IsTrue(element.Contains(o4));

            Assert.IsFalse(element.Contains(builder.CreateAtom()));
            Assert.IsFalse(element.Contains(null));
        }

#if false
        [TestMethod()]
    public void TestMap_Map_Map()  {

        IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;

        IAtom c1 = builder.CreateAtom("C");
        IAtom c2 = builder.CreateAtom("C");
        IAtom o3 = builder.CreateAtom("O");
        IAtom o4 = builder.CreateAtom("O");

        IBond c1c2 = builder.CreateBond(c1, c2, BondOrder.Double);
        IBond c1o3 = builder.CreateBond(c1, o3, BondOrder.Single);
        IBond c2o4 = builder.CreateBond(c2, o4, BondOrder.Single);

        // new stereo element
        DoubleBondStereochemistry original = new DoubleBondStereochemistry(c1c2, new IBond[]{c1o3, c2o4},
                DoubleBondConformation.Opposite);

        // clone the atoms and place in a map
        IDictionary<IBond, IBond> mapping = new Dictionary<IBond, IBond>();
        IBond c1c2clone = (IBond) c1c2.Clone();
        mapping.Add(c1c2, c1c2clone);
        IBond c1o3clone = (IBond) c1o3.Clone();
        mapping.Add(c1o3, c1o3clone);
        IBond c2o4clone = (IBond) c2o4.Clone();
        mapping.Add(c2o4, c2o4clone);

        // map the existing element a new element
        IDoubleBondStereochemistry mapped = original.Map(Collections.EMPTY_MAP, mapping);

        Assert.AssertThat("mapped chiral atom was the same as the original", mapped.StereoBond,
                Is(Not(SameInstance(original.StereoBond))));
        Assert.AreEqual(SameInstance(c1c2clone), "mapped chiral atom was not the clone", mapped.StereoBond);

        IBond[] originalBonds = original.Bonds;
        IBond[] mappedBonds = mapped.Bonds;

        Assert.AssertThat("first bond was te same as the original", mappedBonds[0],
                Is(Not(SameInstance(originalBonds[0]))));
        Assert.AreEqual(SameInstance(c1o3clone), "first mapped bond was not the clone", mappedBonds[0]);
        Assert.AssertThat("second bond was te same as the original", mappedBonds[1],
                Is(Not(SameInstance(originalBonds[1]))));
        Assert.AreEqual(SameInstance(c2o4clone), "second mapped bond was not the clone", mappedBonds[1]);

        Assert.AreEqual(original.Stereo, "stereo was not mapped", mapped.Stereo);

    }

    [TestMethod()][ExpectedException(typeof(ArgumentException))]
    public void TestMap_Null_Map()  {

        IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;

        IAtom c1 = builder.CreateAtom("C");
        IAtom c2 = builder.CreateAtom("C");
        IAtom o3 = builder.CreateAtom("O");
        IAtom o4 = builder.CreateAtom("O");

        IBond c1c2 = builder.CreateBond(c1, c2, BondOrder.Double);
        IBond c1o3 = builder.CreateBond(c1, o3, BondOrder.Single);
        IBond c2o4 = builder.CreateBond(c2, o4, BondOrder.Single);

        // new stereo element
        IDoubleBondStereochemistry original = new DoubleBondStereochemistry(c1c2, new IBond[]{c1o3, c2o4},
                Conformation.Opposite);

        // map the existing element a new element - should through an ArgumentException
        IDoubleBondStereochemistry mapped = original.Map(Collections.EMPTY_MAP, null);

    }

    [TestMethod()]
    public void TestMap_Map_Map_NullElement()  {

        IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;

        // new stereo element
        IDoubleBondStereochemistry original = new DoubleBondStereochemistry(null, new IBond[2], null);

        // map the existing element a new element
        IDoubleBondStereochemistry mapped = original.Map(Collections.EMPTY_MAP, Collections.EMPTY_MAP);

        Assert.IsNull(mapped.StereoBond);
        Assert.IsNull(mapped.Bonds[0]);
        Assert.IsNull(mapped.Bonds[1]);
        Assert.IsNull(mapped.Stereo);
    }

    [TestMethod()]
    public void TestMap_Map_Map_EmptyMapping()  
    {
        IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;

        IAtom c1 = builder.CreateAtom("C");
        IAtom c2 = builder.CreateAtom("C");
        IAtom o3 = builder.CreateAtom("O");
        IAtom o4 = builder.CreateAtom("O");

        IBond c1c2 = builder.CreateBond(c1, c2, BondOrder.Double);
        IBond c1o3 = builder.CreateBond(c1, o3, BondOrder.Single);
        IBond c2o4 = builder.CreateBond(c2, o4, BondOrder.Single);

        // new stereo element
        IDoubleBondStereochemistry original = new DoubleBondStereochemistry(c1c2, new IBond[]{c1o3, c2o4},
                Conformation.Opposite);

        // map the existing element a new element - should through an ArgumentException
        IDoubleBondStereochemistry mapped = original.Map(Collections.EMPTY_MAP, Collections.EMPTY_MAP);

        Assert.AreEqual(original.GetStereoBond(), mapped.GetStereoBond());
        Assert.AreEqual(original.Bonds, mapped.Bonds);
        Assert.IsNotNull(mapped.Stereo);
    }
#endif

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
