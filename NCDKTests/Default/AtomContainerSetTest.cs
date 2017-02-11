/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
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
using NCDK.Default;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCDK.Default
{
    /**
     * Checks the functionality of the MoleculeSet class.
     *
     * @cdk.module test-data
     *
     * @see org.openscience.cdk.MoleculeSet
     */
	[TestClass()]
    public class AtomContainerSetTest
        :  AbstractAtomContainerSetTest<IAtomContainer>
    {
        public override IChemObject NewChemObject()
        {
            return new AtomContainerSet<IAtomContainer>();
        }

        public override IAtomContainer NewContainerObject()
        {
            return new AtomContainer();
        }

        [TestMethod()]
        public virtual void TestAtomContainerSet()
        {
            IAtomContainerSet<IAtomContainer> som = new AtomContainerSet<IAtomContainer>();
            Assert.IsNotNull(som);
            Assert.AreEqual(0, som.Count);
        }

    }
}
