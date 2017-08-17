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

namespace NCDK.Default
{
    /// <summary>
    /// Checks the functionality of the AtomType class.
    /// </summary>
    /// <seealso cref="AtomType"/>
    // @cdk.module test-data
    [TestClass()]
    public class AtomTypeTest
        : AbstractAtomTypeTest
    {
        public override IChemObject NewChemObject()
        {
            return new AtomType("C");
        }

        [TestMethod()]
        public virtual void TestAtomType_String()
        {
            IAtomType at = new AtomType("C");
            Assert.AreEqual("C", at.Symbol);
        }

        [TestMethod()]
        public virtual void TestAtomType_IElement()
        {
            IElement element = NewChemObject().Builder.NewElement("C");
            IAtomType at = new AtomType(element);
            Assert.AreEqual("C", at.Symbol);
        }

        [TestMethod()]
        public virtual void TestAtomType_String_String()
        {
            IAtomType at = new AtomType("C4", "C");
            Assert.AreEqual("C", at.Symbol);
            Assert.AreEqual("C4", at.AtomTypeName);
        }

        [TestMethod()]
        public virtual void TestCompare()
        {
            IAtomType at = new AtomType("C4", "C");
            if (at is AtomType)
            {
                AtomType at1 = (AtomType)at;
                IAtomType at2 = at.Builder.NewAtomType("C3", "C");
                Assert.IsFalse(at1.Compare("C4"));
                Assert.IsFalse(at1.Compare(at2));
            }
        }

        [TestMethod()]
        public override void TestCompare_Object()
        {
            IAtomType someAt = new AtomType("C");
            if (someAt is AtomType)
            {
                AtomType at = (AtomType)someAt;
                Assert.IsTrue(at.Compare(at));
                IAtomType hydrogen = someAt.Builder.NewAtomType("H");
                Assert.IsFalse(at.Compare(hydrogen));
                Assert.IsFalse(at.Compare("Li"));
            }
        }

        [TestMethod()]
        public virtual void TestCompare_AtomTypeName()
        {
            AtomType at1 = new AtomType("C");
            AtomType at2 = new AtomType("C");
            at1.AtomTypeName = "C4";
            at2.AtomTypeName = "C4";
            Assert.IsTrue(at1.Compare(at2));
        }

        [TestMethod()]
        public virtual void TestCompare_DiffAtomTypeName()
        {
            AtomType at1 = new AtomType("C");
            AtomType at2 = new AtomType("C");
            at1.AtomTypeName = "C4";
            at2.AtomTypeName = "C3";
            Assert.IsFalse(at1.Compare(at2));
        }

        [TestMethod()]
        public virtual void TestCompare_BondOrderSum()
        {
            AtomType at1 = new AtomType("C");
            AtomType at2 = new AtomType("C");
            at1.BondOrderSum = 1.5;
            at2.BondOrderSum = 1.5;
            Assert.IsTrue(at1.Compare(at2));
        }

        [TestMethod()]
        public virtual void TestCompare_DiffBondOrderSum()
        {
            AtomType at1 = new AtomType("C");
            AtomType at2 = new AtomType("C");
            at1.BondOrderSum = 1.5;
            at2.BondOrderSum = 2.0;
            Assert.IsFalse(at1.Compare(at2));
        }
    }
}
