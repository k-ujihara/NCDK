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
    /// <summary>
    // @cdk.module test-standard
    /// </summary>
    [TestClass()]
    public class AtomContainerPermutorTest : CDKTestCase
    {
        [TestMethod()]
        public void TestAtomPermutation()
        {
            AtomContainer ac = new AtomContainer();
            AtomContainer result;
            string atoms = "";
            ac.Add(new Atom("C"));
            ac.Add(new Atom("N"));
            ac.Add(new Atom("P"));
            ac.Add(new Atom("O"));
            ac.Add(new Atom("S"));
            ac.Add(new Atom("Br"));
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
                atoms = "";
                result = (AtomContainer)acap.Current;
                for (int f = 0; f < result.Atoms.Count; f++)
                {
                    atoms += result.Atoms[f].Symbol;
                }
            }
            Assert.AreEqual(719, counter);
        }

        [TestMethod()]
        public void TestBondPermutation()
        {
            AtomContainer ac = new AtomContainer();
            AtomContainer result;
            string bonds = "";
            ac.Add(new Atom("C"));
            ac.Add(new Atom("N"));
            ac.Add(new Atom("P"));
            ac.Add(new Atom("O"));
            ac.Add(new Atom("S"));
            ac.Add(new Atom("Br"));
            ac.AddBond(ac.Atoms[0], ac.Atoms[1], BondOrder.Single);
            ac.AddBond(ac.Atoms[1], ac.Atoms[2], BondOrder.Double);
            ac.AddBond(ac.Atoms[2], ac.Atoms[3], BondOrder.Triple);
            ac.AddBond(ac.Atoms[3], ac.Atoms[4], BondOrder.Quadruple);
            ac.AddBond(ac.Atoms[4], ac.Atoms[5], BondOrder.Single); // was 5.0 !
            AtomContainerBondPermutor acap = new AtomContainerBondPermutor(ac);
            int counter = 0;
            while (acap.MoveNext())
            {
                counter++;
                bonds = "";
                result = (AtomContainer)acap.Current;
                for (int f = 0; f < result.Bonds.Count; f++)
                {
                    bonds += result.Bonds[f].Order;
                }
                //Debug.WriteLine(bonds);
            }
            Assert.AreEqual(119, counter);
        }
    }
}
