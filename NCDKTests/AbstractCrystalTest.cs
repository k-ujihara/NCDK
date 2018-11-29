/* Copyright (C) 2002-2007  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
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
using NCDK.Numerics;

namespace NCDK
{
    /// <summary>
    /// Checks the functionality of <see cref="ICrystal"/> implementations.
    /// </summary>
    // @cdk.module test-interfaces
    public abstract class AbstractCrystalTest
        : AbstractAtomContainerTest
    {
        [TestMethod()]
        public override void TestAdd_IAtomContainer()
        {
            ICrystal crystal = (ICrystal)NewChemObject();

            IAtomContainer acetone = crystal.Builder.NewAtomContainer();
            IAtom c1 = crystal.Builder.NewAtom("C");
            IAtom c2 = crystal.Builder.NewAtom("C");
            IAtom o = crystal.Builder.NewAtom("O");
            IAtom c3 = crystal.Builder.NewAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            IBond b1 = crystal.Builder.NewBond(c1, c2, BondOrder.Single);
            IBond b2 = crystal.Builder.NewBond(c1, o, BondOrder.Double);
            IBond b3 = crystal.Builder.NewBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            crystal.Add(acetone);
            Assert.AreEqual(4, crystal.Atoms.Count);
            Assert.AreEqual(3, crystal.Bonds.Count);
        }

        [TestMethod()]
        public override void TestAddAtom_IAtom()
        {
            ICrystal crystal = (ICrystal)NewChemObject();
            IAtom c1 = crystal.Builder.NewAtom("C");
            crystal.Atoms.Add(c1);
            Assert.AreEqual(1, crystal.Atoms.Count);
        }

        [TestMethod()]
        public virtual void TestSetA_Vector3d()
        {
            ICrystal crystal = (ICrystal)NewChemObject();

            crystal.A = new Vector3(1, 2, 3);
            Vector3 a = crystal.A;
            Assert.AreEqual(1.0, a.X, 0.001);
            Assert.AreEqual(2.0, a.Y, 0.001);
            Assert.AreEqual(3.0, a.Z, 0.001);
        }

        [TestMethod()]
        public virtual void TestGetA()
        {
            ICrystal crystal = (ICrystal)NewChemObject();

            crystal.A = new Vector3(1, 2, 3);
            Vector3 a = crystal.A;
            Assert.IsNotNull(a);
        }

        [TestMethod()]
        public virtual void TestGetB()
        {
            ICrystal crystal = (ICrystal)NewChemObject();

            crystal.B = new Vector3(1, 2, 3);
            Vector3 a = crystal.B;
            Assert.IsNotNull(a);
        }

        [TestMethod()]
        public virtual void TestGetC()
        {
            ICrystal crystal = (ICrystal)NewChemObject();

            crystal.C = new Vector3(1, 2, 3);
            Vector3 a = crystal.C;
            Assert.IsNotNull(a);
        }

        [TestMethod()]
        public virtual void TestSetB_Vector3d()
        {
            ICrystal crystal = (ICrystal)NewChemObject();

            crystal.B = new Vector3(1, 2, 3);
            Vector3 b = crystal.B;
            Assert.AreEqual(1.0, b.X, 0.001);
            Assert.AreEqual(2.0, b.Y, 0.001);
            Assert.AreEqual(3.0, b.Z, 0.001);
        }

        [TestMethod()]
        public virtual void TestSetC_Vector3d()
        {
            ICrystal crystal = (ICrystal)NewChemObject();

            crystal.C = new Vector3(1, 2, 3);
            Vector3 c = crystal.C;
            Assert.AreEqual(1.0, c.X, 0.001);
            Assert.AreEqual(2.0, c.Y, 0.001);
            Assert.AreEqual(3.0, c.Z, 0.001);
        }

        [TestMethod()]
        public virtual void TestSetSpaceGroup_String()
        {
            ICrystal crystal = (ICrystal)NewChemObject();
            string spacegroup = "P 2_1 2_1 2_1";
            crystal.SpaceGroup = spacegroup;
            Assert.AreEqual(spacegroup, crystal.SpaceGroup);
        }

        [TestMethod()]
        public virtual void TestGetSpaceGroup()
        {
            ICrystal crystal = (ICrystal)NewChemObject();
            string spacegroup = "P 2_1 2_1 2_1";
            crystal.SpaceGroup = spacegroup;
            Assert.IsNotNull(crystal.SpaceGroup);
            Assert.AreEqual(spacegroup, crystal.SpaceGroup);
        }

        [TestMethod()]
        public virtual void TestSetZ_Integer()
        {
            ICrystal crystal = (ICrystal)NewChemObject();
            int z = 2;
            crystal.Z = z;
            Assert.AreEqual(z, crystal.Z.Value);
        }

        [TestMethod()]
        public virtual void TestGetZ()
        {
            TestSetZ_Integer();
        }

        /// <summary>
        /// Method to test whether the class complies with RFC #9.
        /// </summary>
        [TestMethod()]
        public override void TestToString()
        {
            ICrystal crystal = (ICrystal)NewChemObject();
            string description = crystal.ToString();
            for (int i = 0; i < description.Length; i++)
            {
                Assert.IsTrue(description[i] != '\n');
                Assert.IsTrue(description[i] != '\r');
            }
        }

        [TestMethod()]
        public override void TestClone()
        {
            ICrystal crystal = (ICrystal)NewChemObject();
            object clone = crystal.Clone();
            Assert.IsTrue(clone is ICrystal);
        }

        [TestMethod()]
        public virtual void TestClone_Axes()
        {
            ICrystal crystal1 = (ICrystal)NewChemObject();
            Vector3 axes = new Vector3(1, 2, 3);
            crystal1.A = axes;
            ICrystal crystal2 = (ICrystal)crystal1.Clone();

            // test cloning of axes
            var cpy = crystal1.A; cpy.X = 5; crystal1.A = cpy; // NCDK's Vector3 is value type.
            Assert.AreEqual(1.0, crystal2.A.X, 0.001);
        }

        [TestMethod()]
        public virtual void TestSetZeroAxes()
        {
            ICrystal crystal = (ICrystal)NewChemObject();

            crystal.A = new Vector3(1, 2, 3);
            Vector3 a = crystal.A;
            Assert.IsNotNull(a);
        }
    }
}
