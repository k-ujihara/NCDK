/* Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;

namespace NCDK
{
    /// <summary>
    /// Checks the functionality of the Association class.
    /// </summary>
    // @cdk.module test-extra
    // @see org.openscience.cdk.Association
    [TestClass()]
    public class AssociationTest : CDKTestCase
    {
        [TestMethod()]
        public void TestAssociation()
        {
            Association association = new Association();
            Assert.AreEqual(0, association.ElectronCount.Value);
            Assert.AreEqual(0, association.AssociatedAtoms.Count);
        }

        [TestMethod()]
        public void TestAssociation_IAtom_IAtom()
        {
            Association association = new Association(new Atom("C"), new Atom("C"));
            Assert.AreEqual(0, association.ElectronCount.Value);
            Assert.AreEqual(2, association.AssociatedAtoms.Count);
        }

        /** Test for RFC #9 */
        [TestMethod()]
        public void TestToString()
        {
            Association association = new Association();
            string description = association.ToString();
            for (int i = 0; i < description.Length; i++)
            {
                Assert.IsTrue(description[i] != '\n');
                Assert.IsTrue(description[i] != '\r');
            }
        }

        [TestMethod()]
        public void TestToStringWithAtoms()
        {
            Association association = new Association(new Atom("C"), new Atom("C"));
            string description = association.ToString();
            Assert.IsTrue(description.Contains(","));
        }

        [TestMethod()]
        public void TestContains()
        {
            Atom c = new Atom("C");
            Atom o = new Atom("O");

            Association association = new Association(c, o);

            Assert.IsTrue(association.AssociatedAtoms.Contains(c));
            Assert.IsTrue(association.AssociatedAtoms.Contains(o));
        }

        [TestMethod()]
        public void TestGetAtomCount()
        {
            Atom c = new Atom("C");
            Atom o = new Atom("O");

            Association association = new Association(c, o);

            Assert.AreEqual(2, association.AssociatedAtoms.Count);
        }

        [TestMethod()]
        public void TestGetAtoms()
        {
            Atom c = new Atom("C");
            Atom o = new Atom("O");

            Association association = new Association(c, o);

            var atoms = association.AssociatedAtoms;
            Assert.AreEqual(2, atoms.Count);
            Assert.IsNotNull(atoms[0]);
            Assert.IsNotNull(atoms[1]);
        }

        [TestMethod()]
        public void TestSetAtoms()
        {
            Atom c = new Atom("C");
            Atom o = new Atom("O");
            Association association = new Association(new IAtom[] { c, o });

            Assert.IsTrue(association.AssociatedAtoms.Contains(c));
            Assert.IsTrue(association.AssociatedAtoms.Contains(o));
        }

        [TestMethod()]
        public void TestSetAtomAt()
        {
            Atom c = new Atom("C");
            Atom o = new Atom("O");
            Atom n = new Atom("N");
            Association association = new Association(c, o);
            association.AssociatedAtoms[1] = n;

            Assert.IsTrue(association.AssociatedAtoms.Contains(c));
            Assert.IsTrue(association.AssociatedAtoms.Contains(n));
            Assert.IsFalse(association.AssociatedAtoms.Contains(o));
        }

        [TestMethod()]
        public void TestGetAtomAt()
        {
            Atom c = new Atom("C");
            Atom o = new Atom("O");
            Atom n = new Atom("N");
            Association association = new Association(c, o);

            Assert.AreEqual(c, association.AssociatedAtoms[0]);
            Assert.AreEqual(o, association.AssociatedAtoms[1]);

            association.AssociatedAtoms[0] = n;
            Assert.AreEqual(n, association.AssociatedAtoms[0]);
        }

        [TestMethod()]
        public void TestGetElectronCount()
        {
            Association association = new Association();
            Assert.AreEqual(0, association.ElectronCount.Value, 0.00001);
        }
    }
}
