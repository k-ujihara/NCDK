/* Copyright (C) 2005-2007  The Chemistry Development Kit (CDK) project
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

namespace NCDK
{
    /// <summary>
    /// TestCase for <see cref="IAminoAcid"/> implementations.
    /// </summary>
    // @cdk.module test-interfaces
    // @author  Edgar Luttman <edgar@uni-paderborn.de>
    // @cdk.created 2001-08-09
    [TestClass()]
    public abstract class AbstractAminoAcidTest
        : AbstractMonomerTest
    {
        [TestMethod()]
        public virtual void TestAddCTerminus_IAtom()
        {
            IAminoAcid m = (IAminoAcid)NewChemObject();
            IAtom cTerminus = m.Builder.NewAtom("C");
            m.AddCTerminus(cTerminus);
            Assert.AreEqual(cTerminus, m.CTerminus);
        }

        [TestMethod()]
        public virtual void TestGetCTerminus()
        {
            IAminoAcid m = (IAminoAcid)NewChemObject();
            Assert.IsNull(m.CTerminus);
        }

        [TestMethod()]
        public virtual void TestAddNTerminus_IAtom()
        {
            IAminoAcid m = (IAminoAcid)NewChemObject();
            IAtom nTerminus = m.Builder.NewAtom("N");
            m.AddNTerminus(nTerminus);
            Assert.AreEqual(nTerminus, m.NTerminus);
        }

        [TestMethod()]
        public virtual void TestGetNTerminus()
        {
            IAminoAcid m = (IAminoAcid)NewChemObject();
            Assert.IsNull(m.NTerminus);
        }

        /// <summary>
        /// Method to test whether the class complies with RFC #9.
        /// </summary>
        [TestMethod()]
        public override void TestToString()
        {
            IAminoAcid m = (IAminoAcid)NewChemObject();
            IAtom nTerminus = m.Builder.NewAtom("N");
            m.AddNTerminus(nTerminus);
            string description = m.ToString();
            for (int i = 0; i < description.Length; i++)
            {
                Assert.IsTrue('\n' != description[i]);
                Assert.IsTrue('\r' != description[i]);
            }

            m = (IAminoAcid)NewChemObject();
            IAtom cTerminus = m.Builder.NewAtom("C");
            m.AddCTerminus(cTerminus);
            description = m.ToString();
            for (int i = 0; i < description.Length; i++)
            {
                Assert.IsTrue('\n' != description[i]);
                Assert.IsTrue('\r' != description[i]);
            }
        }

        [TestMethod()]

        public override void TestClone()
        {
            IAminoAcid aa = (IAminoAcid)NewChemObject();
            object clone = aa.Clone();
            Assert.IsTrue(clone is IAminoAcid);
            Assert.AreNotSame(aa, clone);

            aa = (IAminoAcid)NewChemObject();
            IAtom nTerminus = aa.Builder.NewAtom("N");
            aa.AddNTerminus(nTerminus);
            clone = aa.Clone();
            Assert.IsTrue(clone is IAminoAcid);
            Assert.AreNotSame(aa, clone);

            aa = (IAminoAcid)NewChemObject();
            IAtom cTerminus = aa.Builder.NewAtom("C");
            aa.AddCTerminus(cTerminus);
            clone = aa.Clone();
            Assert.IsTrue(clone is IAminoAcid);
            Assert.AreNotSame(aa, clone);
        }
    }
}
