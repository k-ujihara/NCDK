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
using NCDK.Default;
using NCDK.IO;
using NCDK.QSAR.Result;


namespace NCDK.QSAR.Descriptors.Moleculars
{
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class AutocorrelationDescriptorMassTest : MolecularDescriptorTest
    {
        public AutocorrelationDescriptorMassTest()
                : base()
        {
            SetDescriptor(typeof(AutocorrelationDescriptorMass));
        }

        [TestMethod()]
        public void Test1()
        {
            string filename = "NCDK.Data.MDL.chlorobenzene.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins);
            IAtomContainer container = reader.Read(new AtomContainer());
            DescriptorValue count = new AutocorrelationDescriptorMass().Calculate(container);
            Assert.AreEqual(5, count.Value.Length);
            Assert.IsTrue(count.Value is DoubleArrayResult);
            DoubleArrayResult result = (DoubleArrayResult)count.Value;
            for (int i = 0; i < 5; i++)
            {
                Assert.IsFalse(double.IsNaN(result[i]));
                Assert.IsTrue(0.0 != result[i]);
            }
        }
    }
}
