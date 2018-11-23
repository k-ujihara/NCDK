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
using NCDK.Silent;

namespace NCDK.QSAR.Descriptors.Substances
{
    [TestClass()]
    public abstract class SubstanceDescriptorTest<T> : DescriptorTest<T> where T: ISubstanceDescriptor, new()
    {
        protected override T Descriptor => new T();

        [TestMethod()]
        public void TestCalculate_Empty()
        {
            var material = new Substance();
            var value = Descriptor.Calculate(material);
            Assert.IsNotNull(value);
            Assert.AreNotEqual(0, value.Count);
        }

        [TestMethod()]
        public void TestCalculate_Null()
        {
            var value = Descriptor.Calculate(null);
            Assert.IsNotNull(value);
            Assert.AreNotEqual(0, value.Count);
        }
    }
}
