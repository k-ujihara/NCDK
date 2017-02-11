/* Copyright (C) 2001-2007  The Chemistry Development Kit (CDK) project
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

namespace NCDK.Default
{
    /// <summary>
    /// TestCase for the IChemObject class.
    /// </summary>
    // @author      Edgar Luttmann <edgar@uni-paderborn.de>
    // @cdk.module  test-data
    // @cdk.created 2001-08-09
    [TestClass()]
    public class ChemObjectTest
        : AbstractChemObjectTest
    {
        public override IChemObject NewChemObject()
        {
            return new ChemObject();
        }

        [TestMethod()]
        public virtual void TestChemObject()
        {
            IChemObject chemObject = new ChemObject();
            Assert.IsNotNull(chemObject);
        }

        [TestMethod()]
        public virtual void TestChemObject_IChemObject()
        {
            IChemObject chemObject1 = new ChemObject();
            IChemObject chemObject = new ChemObject(chemObject1);
            Assert.IsNotNull(chemObject);
        }

        [TestMethod()]
        public virtual void Compare()
        {
            ChemObject co1 = new ChemObject();
            ChemObject co2 = new ChemObject();
            co1.Id = "a1";
            co2.Id = "a1";
            Assert.IsTrue(co1.Compare(co2));
        }

        [TestMethod()]
        public virtual void CompareDifferent()
        {
            ChemObject co1 = new ChemObject();
            ChemObject co2 = new ChemObject();
            co1.Id = "a1";
            co2.Id = "a2";
            Assert.IsFalse(co1.Compare(co2));
        }
    }
}
