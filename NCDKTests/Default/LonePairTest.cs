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
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCDK.Default
{
    /**
     * Checks the functionality of the LonePair class.
     *
     * @see org.openscience.cdk.LonePair
     *
     * @cdk.module test-data
     */
	[TestClass()]
    public class LonePairTest
        : AbstractLonePairTest
    {

        public override IChemObject NewChemObject()
        {
            return new LonePair();
        }

        [TestMethod()]
        public virtual void TestLonePair()
        {
            ILonePair lp = new LonePair();
            Assert.IsNull(lp.Atom);
            Assert.AreEqual(2, lp.ElectronCount.Value);
        }

        [TestMethod()]
        public virtual void TestLonePair_IAtom()
        {
            IAtom atom = new Atom("N");
            ILonePair lp = new LonePair(atom);
            Assert.AreEqual(2, lp.ElectronCount.Value);
            Assert.AreEqual(atom, lp.Atom);
            Assert.IsTrue(lp.Contains(atom));
        }

    }
}
