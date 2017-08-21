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
using NCDK.QSAR.Result;
using System;

namespace NCDK.QSAR.Descriptors.Substances
{
    [TestClass()]
    public abstract class SubstanceDescriptorTest
    {
        protected ISubstanceDescriptor descriptor;

        //@SuppressWarnings("rawtypes")
        public void SetDescriptor(Type descriptorClass)
        {
            if (descriptor == null)
            {
                var descriptor = descriptorClass.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
                if (!(descriptor is ISubstanceDescriptor))
                {
                    throw new Exception(
                        "The passed descriptor class must be a ISubstanceDescriptor"
                    );
                }
                this.descriptor = (ISubstanceDescriptor)descriptor;
            }
        }

        [TestMethod()]
        public void TestCalculate_Empty()
        {
            ISubstance material = new Substance();
            DescriptorValue value = descriptor.Calculate(material);
            Assert.IsNotNull(value);
            IDescriptorResult result = value.Value;
            Assert.IsNotNull(result);
            Assert.AreNotSame(0, result.Length);
        }

        [TestMethod()]
        public void TestCalculate_Null()
        {
            DescriptorValue value = descriptor.Calculate(null);
            Assert.IsNotNull(value);
            IDescriptorResult result = value.Value;
            Assert.IsNotNull(result);
            Assert.AreNotSame(0, result.Length);
        }
    }
}
