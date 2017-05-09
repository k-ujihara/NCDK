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
 * You should have received a copy of the GNU Lesserf General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;

namespace NCDK.Graphs
{
    // @cdk.module test-standard
    [TestClass()]
    public class AtomContainerAtomPermutorTest : CDKTestCase
    {

        public AtomContainerAtomPermutorTest()
                : base()
        { }

        [TestMethod()]
        public void ConstructorTest()
        {
            IAtomContainer atomContainer = new AtomContainer();
            atomContainer.Atoms.Add(new Atom("C"));
            atomContainer.Atoms.Add(new Atom("O"));
            atomContainer.Atoms.Add(new Atom("S"));
            atomContainer.AddBond(atomContainer.Atoms[0], atomContainer.Atoms[1], BondOrder.Single);
            atomContainer.AddBond(atomContainer.Atoms[0], atomContainer.Atoms[2], BondOrder.Single);
            AtomContainerAtomPermutor acap = new AtomContainerAtomPermutor(atomContainer);
            Assert.IsNotNull(acap);
        }

        [TestMethod()]
        public void TestCountAtomPermutation()
        {
            AtomContainer ac = new AtomContainer();
            ac.Atoms.Add(new Atom("C"));
            ac.Atoms.Add(new Atom("N"));
            ac.Atoms.Add(new Atom("P"));
            ac.Atoms.Add(new Atom("O"));
            ac.Atoms.Add(new Atom("S"));
            ac.Atoms.Add(new Atom("Br"));
            ac.AddBond(ac.Atoms[0], ac.Atoms[1], BondOrder.Single);
            ac.AddBond(ac.Atoms[1], ac.Atoms[2], BondOrder.Single);
            ac.AddBond(ac.Atoms[2], ac.Atoms[3], BondOrder.Single);
            ac.AddBond(ac.Atoms[3], ac.Atoms[4], BondOrder.Single);
            ac.AddBond(ac.Atoms[4], ac.Atoms[5], BondOrder.Single);
            AtomContainerAtomPermutor acap = new AtomContainerAtomPermutor(ac);
            int counter = 0;
            while (acap.MoveNext())
            {
                counter++;
            }
            Assert.AreEqual(719, counter);
        }

        [TestMethod()]
        public void ContainerFromPermutationTest()
        {
            IAtomContainer atomContainer = new AtomContainer();
            atomContainer.Atoms.Add(new Atom("C"));
            atomContainer.Atoms.Add(new Atom("O"));
            atomContainer.Atoms.Add(new Atom("S"));
            atomContainer.AddBond(atomContainer.Atoms[0], atomContainer.Atoms[1], BondOrder.Single);
            atomContainer.AddBond(atomContainer.Atoms[0], atomContainer.Atoms[2], BondOrder.Single);
            AtomContainerAtomPermutor acap = new AtomContainerAtomPermutor(atomContainer);
            IAtomContainer permuted = acap.ContainerFromPermutation(new int[] { 1, 0, 2 });
            Assert.IsNotNull(permuted);
            Assert.AreEqual(atomContainer.Atoms.Count, permuted.Atoms.Count);
            Assert.AreEqual(atomContainer.Bonds.Count, permuted.Bonds.Count);
        }
    }
}
