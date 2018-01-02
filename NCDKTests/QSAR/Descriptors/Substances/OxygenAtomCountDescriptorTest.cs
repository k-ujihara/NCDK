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
using NCDK.Default;
using NCDK.QSAR.Results;
using NCDK.Tools.Manipulator;

namespace NCDK.QSAR.Descriptors.Substances
{
    [TestClass()]
    public class OxygenAtomCountDescriptorTest : SubstanceDescriptorTest
    {
        public OxygenAtomCountDescriptorTest()
        {
            SetDescriptor(typeof(OxygenAtomCountDescriptor));
        }

        [TestMethod()]
        public void TestCalculate_ZnO()
        {
            ISubstance material = new Substance();
            material.Add(
                MolecularFormulaManipulator.GetAtomContainer(
                    "ZnO", Default.ChemObjectBuilder.Instance
                )
            );
            var value = descriptor.Calculate(material);
            Assert.IsNotNull(value);
            IDescriptorResult result = value.Value;
            Assert.IsNotNull(result);
            Assert.AreEqual(1, ((Result<int>)result).Value);
        }

        [TestMethod()]
        public void TestCalculate_IronOxide()
        {
            ISubstance material = new Substance();
            material.Add(
                MolecularFormulaManipulator.GetAtomContainer(
                    "Fe3O4", Default.ChemObjectBuilder.Instance
                )
            );
            var value = descriptor.Calculate(material);
            Assert.IsNotNull(value);
            IDescriptorResult result = value.Value;
            Assert.IsNotNull(result);
            Assert.AreEqual(4, ((Result<int>)result).Value);
        }
    }
}
