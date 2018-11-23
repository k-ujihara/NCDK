/* Copyright (C) 2011  Egon Willighagen <egonw@users.sf.net>
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License as published by the Free
 * Software Foundation; either version 2.1 of the License, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License for more
 * details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software Foundation, Inc.,
 * 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Tools.Manipulator;

namespace NCDK.QSAR.Descriptors.Substances
{
    [TestClass()]
    public class OxygenAtomCountDescriptorTest : SubstanceDescriptorTest<OxygenAtomCountDescriptor>
    {
        [TestMethod()]
        public void TestCalculate_ZnO()
        {
            var material = CDK.Builder.NewSubstance();
            material.Add(MolecularFormulaManipulator.GetAtomContainer("ZnO", material.Builder));
            var value = Descriptor.Calculate(material);
            Assert.IsNotNull(value);
            Assert.AreEqual(1, value.Value);
        }

        [TestMethod()]
        public void TestCalculate_IronOxide()
        {
            var material = CDK.Builder.NewSubstance();
            material.Add(MolecularFormulaManipulator.GetAtomContainer("Fe3O4", material.Builder));
            var value = Descriptor.Calculate(material);
            Assert.IsNotNull(value);
            Assert.AreEqual(4, value.Value);
        }
    }
}
