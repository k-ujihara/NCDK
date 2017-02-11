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
 *  */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCDK.Default
{
    /**
     * TestCase for the Polymer class.
     *
     * @author      Edgar Luttmann <edgar@uni-paderborn.de>
     * @author      Martin Eklund <martin.eklund@farmbio.uu.se>
     * @cdk.created 2001-08-09
     * @cdk.module  test-data
     */
	[TestClass()]
    public class PolymerTest : AbstractPolymerTest
    {
        public override IChemObject NewChemObject()
        {
            return new Polymer();
        }

        [TestMethod()]
        public virtual void TestPolymer()
        {
            IPolymer oPolymer = new Polymer();
            Assert.IsNotNull(oPolymer);
            Assert.AreEqual(oPolymer.GetMonomerMap().Count(), 0);
        }

        /**
         * A clone must deep clone everything, so that after the clone, operations
         * on the original do not modify the clone.
         *
         * @cdk.bug 2454890
         */
        [TestMethod()]
        public virtual void TestPolymerClone()
        {
            IPolymer oPolymer = new Polymer();
            Assert.IsNotNull(oPolymer);
            Assert.AreEqual(0, oPolymer.GetMonomerMap().Count());
            Polymer clone = (Polymer)oPolymer.Clone();
            Monomer monomer = new Monomer();
            monomer.MonomerName = "TYR55";
            oPolymer.AddAtom(new Atom("C"), monomer);

            // changes should not occur in the clone
            Assert.AreEqual(0, clone.GetMonomerMap().Count());
            Assert.AreEqual(0, clone.GetMonomerNames().Count());

            // new clone should see the changes
            clone = (Polymer)oPolymer.Clone();
            Assert.AreEqual(1, clone.GetMonomerMap().Count());
            Assert.AreEqual(1, clone.GetMonomerNames().Count());
            Assert.AreEqual(1, clone.Atoms.Count);

            oPolymer.Add(new Atom("N"));
            clone = (Polymer)oPolymer.Clone();
            Assert.AreEqual(1, clone.GetMonomerMap().Count());
            Assert.AreEqual(2, clone.Atoms.Count);
        }

        /**
         * @cdk.bug  2454890
         */
        [TestMethod()]
        public virtual void TestPolymerClone2()
        {
            IPolymer oPolymer = new Polymer();
            Assert.IsNotNull(oPolymer);
            Assert.AreEqual(0, oPolymer.GetMonomerMap().Count());

            Monomer monomer = new Monomer();
            monomer.MonomerName = "TYR55";
            IAtom atom = monomer.Builder.CreateAtom("C");
            oPolymer.AddAtom(atom, monomer);

            Polymer clone = (Polymer)oPolymer.Clone();
            IMonomer clonedMonomer = clone.GetMonomer("TYR55");
            Assert.AreNotSame(monomer, clonedMonomer);
            IAtom clonedAtom = clone.Atoms[0];
            Assert.AreNotSame(atom, clonedAtom);

            IAtom atomFromMonomer = clone.GetMonomer("TYR55").Atoms[0];
            Assert.AreEqual("C", atomFromMonomer.Symbol);
            Assert.AreNotSame(atom, atomFromMonomer);
            Assert.AreSame(atomFromMonomer, clonedAtom);
        }
    }
}
