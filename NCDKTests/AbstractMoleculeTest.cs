/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
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

namespace NCDK
{
    /// <summary>
    /// Checks the functionality of <see cref="IAtomContainer"/> implementations.
    ///
    // @cdk.module test-interfaces
    /// </summary>
    [TestClass()]
    public abstract class AbstractMoleculeTest : AbstractAtomContainerTest
    {
        [TestMethod()]
        public override void TestClone()
        {
            IAtomContainer molecule = (IAtomContainer)NewChemObject();
            object clone = molecule.Clone();
            Assert.IsTrue(clone is IAtomContainer);
            Assert.AreNotSame(molecule, clone);
        }

        /// <summary>Test for RFC #9</summary>
        [TestMethod()]
        public override void TestToString()
        {
            IAtomContainer m = (IAtomContainer)NewChemObject();
            string description = m.ToString();
            for (int i = 0; i < description.Length; i++)
            {
                Assert.IsTrue(description[i] != '\n');
                Assert.IsTrue(description[i] != '\r');
            }
        }

        [TestMethod()]
        public virtual void TestGetLonePairCount_Molecule()
        {
            IAtomContainer acetone = (IAtomContainer)NewChemObject();
            IAtom c1 = acetone.Builder.CreateAtom("C");
            IAtom c2 = acetone.Builder.CreateAtom("C");
            IAtom o = acetone.Builder.CreateAtom("O");
            IAtom c3 = acetone.Builder.CreateAtom("C");
            acetone.Atoms.Add(c1);
            acetone.Atoms.Add(c2);
            acetone.Atoms.Add(c3);
            acetone.Atoms.Add(o);
            IBond b1 = acetone.Builder.CreateBond(c1, c2, BondOrder.Single);
            IBond b2 = acetone.Builder.CreateBond(c1, o, BondOrder.Double);
            IBond b3 = acetone.Builder.CreateBond(c1, c3, BondOrder.Single);
            acetone.Bonds.Add(b1);
            acetone.Bonds.Add(b2);
            acetone.Bonds.Add(b3);

            // add lone pairs on oxygen
            ILonePair lp1 = acetone.Builder.CreateLonePair(o);
            ILonePair lp2 = acetone.Builder.CreateLonePair(o);
            acetone.LonePairs.Add(lp1);
            acetone.LonePairs.Add(lp2);

            Assert.AreEqual(2, acetone.LonePairs.Count);
        }
    }
}
