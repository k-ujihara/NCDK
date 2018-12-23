
/* Copyright (C) 2007  Federico
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
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.IO;
using System.Collections.Generic;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class AutocorrelationDescriptorChargeTest : MolecularDescriptorTest<AutocorrelationDescriptorCharge>
    {
        [TestMethod()]
        public void Test1()
        {
            var filename = "NCDK.Data.MDL.chlorobenzene.mol";
            IAtomContainer container;
            using (var reader = new MDLV2000Reader(ResourceLoader.GetAsStream(filename)))
            {
                container = reader.Read(CDK.Builder.NewAtomContainer());
            }
            var result = CreateDescriptor().Calculate(container);
            Assert.AreEqual(5, result.Values.Count);
            Assert.IsTrue(result.Values is IEnumerable<double>);
            for (int i = 0; i < 5; i++)
            {
                Assert.IsFalse(double.IsNaN(result.Values[i]));
                Assert.IsTrue(0.0 != result.Values[i]);
            }
        }
    }
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class AutocorrelationDescriptorMassTest : MolecularDescriptorTest<AutocorrelationDescriptorMass>
    {
        [TestMethod()]
        public void Test1()
        {
            var filename = "NCDK.Data.MDL.chlorobenzene.mol";
            IAtomContainer container;
            using (var reader = new MDLV2000Reader(ResourceLoader.GetAsStream(filename)))
            {
                container = reader.Read(CDK.Builder.NewAtomContainer());
            }
            var result = CreateDescriptor().Calculate(container);
            Assert.AreEqual(5, result.Values.Count);
            Assert.IsTrue(result.Values is IEnumerable<double>);
            for (int i = 0; i < 5; i++)
            {
                Assert.IsFalse(double.IsNaN(result.Values[i]));
                Assert.IsTrue(0.0 != result.Values[i]);
            }
        }
    }
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class AutocorrelationDescriptorPolarizabilityTest : MolecularDescriptorTest<AutocorrelationDescriptorPolarizability>
    {
        [TestMethod()]
        public void Test1()
        {
            var filename = "NCDK.Data.MDL.chlorobenzene.mol";
            IAtomContainer container;
            using (var reader = new MDLV2000Reader(ResourceLoader.GetAsStream(filename)))
            {
                container = reader.Read(CDK.Builder.NewAtomContainer());
            }
            var result = CreateDescriptor().Calculate(container);
            Assert.AreEqual(5, result.Values.Count);
            Assert.IsTrue(result.Values is IEnumerable<double>);
            for (int i = 0; i < 5; i++)
            {
                Assert.IsFalse(double.IsNaN(result.Values[i]));
                Assert.IsTrue(0.0 != result.Values[i]);
            }
        }
    }
}
