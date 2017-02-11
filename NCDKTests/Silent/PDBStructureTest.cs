/* Copyright (C) 2004-2007  Miguel Rojas <miguel.rojas@uni-koeln.de>
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

namespace NCDK.Silent
{
    /**
     * Checks the functionality of the {@link PDBStructure}.
     *
     * @cdk.module test-silent
     */
    [TestClass()]
    public class PDBStructureTest : AbstractPDBStructureTest
    {
        [TestInitialize()]
        public void Initialize()
        {
            ChemObject = new PDBStructure();
        }

        [TestMethod()]
        public void TestPDBStructure()
        {
            IPDBStructure structure = new PDBStructure();
            Assert.IsNotNull(structure);
        }

        [TestMethod()]
        public void TestGetBuilder()
        {
            PDBStructure structure = new PDBStructure();
            Assert.IsTrue(structure.Builder is Silent.ChemObjectBuilder);
        }

        [TestMethod()]
        public void TestAddListener_IChemObjectListener()
        {
            ChemObjectTestHelper.TestAddListener_IChemObjectListener(NewChemObject());
        }
    }
}
