/* Copyright (C) 2018  Jeffrey Plante (Lhasa Limited)  <Jeffrey.Plante@lhasalimited.org>
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
using System.Linq;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    [TestClass()]
    public class JPlogPDescriptorTest
    {
        [TestMethod()]
        public void TestPyridine()
        {
            var mol = AtomTyperTests.ParseSmiles("c1ncccc1");
            var desc = new JPlogPDescriptor();
            var answer = desc.Calculate(mol);
            var output = answer.Value;
            Assert.AreEqual(0.9, output, 0.1);
        }

        [TestMethod()]
        public void TestPropionicAcid()
        {
            var mol = AtomTyperTests.ParseSmiles("CCC(=O)O");
            var desc = new JPlogPDescriptor();
            var answer = desc.Calculate(mol);
            var output = answer.Value;
            Assert.AreEqual(0.3, output, 0.1);
        }

        [TestMethod()]
        public void TestAcetonitrile()
        {
            var mol = AtomTyperTests.ParseSmiles("CC#N");
            var desc = new JPlogPDescriptor();
            var answer = desc.Calculate(mol);
            var output = answer.Value;
            Assert.AreEqual(0.4, output, 0.1);
        }

        [TestMethod()]
        public void TestAniline()
        {
            var mol = AtomTyperTests.ParseSmiles("Nc1ccccc1");
            var desc = new JPlogPDescriptor();
            var answer = desc.Calculate(mol);
            var output = answer.Value;
            Assert.AreEqual(1.2, output, 0.1);
        }

        [TestMethod()]
        public void TestFluorobenzene()
        {
            var mol = AtomTyperTests.ParseSmiles("Fc1ccccc1");
            var desc = new JPlogPDescriptor();
            var answer = desc.Calculate(mol);
            var output = answer.Value;
            Assert.AreEqual(2.0, output, 0.1);
        }

        [TestMethod()]
        public void TestSimpleTextFields()
        {
            var attributes = (DescriptorSpecificationAttribute)typeof(JPlogPDescriptor).GetCustomAttributes(typeof(DescriptorSpecificationAttribute), true)[0];
            Assert.AreEqual("JPlogP developed at Lhasa Limited www.lhasalimited.org", attributes.Reference);
            Assert.AreEqual("Jeffrey Plante - Lhasa Limited", attributes.Vendor);

            var mol = AtomTyperTests.ParseSmiles("C");
            var desc = new JPlogPDescriptor();
            var answer = desc.Calculate(mol);
            Assert.IsTrue(answer.Keys.Contains("JLogP"));
            Assert.AreEqual(1, answer.Keys.Count());
        }

        [TestMethod()]
        public void TestGetHologram()
        {
            var molecule = AtomTyperTests.ParseSmiles("c1ccccc1");
            var holo = JPlogPDescriptor.JPlogPCalculator.GetMappedHologram(molecule);
            Assert.AreEqual(2, holo.Count);
            Assert.AreEqual(6, holo[106204]);
        }
    }
}
