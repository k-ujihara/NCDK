/* Copyright (C) 2007  Miguel Rojasch <miguelrojasch@users.sf.net>
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

namespace NCDK.Formula
{
    /// <summary>
    /// Checks the functionality of the MolecularFormulaSet class.
    /// </summary>
    /// <seealso cref="MolecularFormulaSet"/>
    // @cdk.module test-data
    [TestClass()]
    public class MolecularFormulaSetTest : AbstractMolecularFormulaSetTest
    {
        protected override IChemObjectBuilder Builder => Default.ChemObjectBuilder.Instance;

        [TestMethod()]
        public void TestMolecularFormulaSet() {
            IMolecularFormulaSet mfS = new MolecularFormulaSet();
            Assert.IsNotNull(mfS);
        }

        [TestMethod()]
        public void TestMolecularFormulaSet_IMolecularFormula() {
            IMolecularFormulaSet mfS = new MolecularFormulaSet(Builder.NewMolecularFormula());
            Assert.AreEqual(1, mfS.Count);
        }
    }
}