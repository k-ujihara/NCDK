/* Copyright (C) 2018  Rajarshi Guha <rajarshi.guha@gmail.com>
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
using System.Linq;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    [TestClass]
    public class SpiroAtomCountDescriptorTest : MolecularDescriptorTest<SpiroAtomCountDescriptor>
    {
        Smiles.SmilesParser sp = CDK.SmilesParser;

        [TestMethod()]
        public void TestDecalin()
        {
            var mol = sp.ParseSmiles("C1CCC2CCCCC2C1"); // ethanol
            var value = CreateDescriptor(mol).Calculate();
            Assert.AreEqual(0, value.Value);
            Assert.AreEqual(1, value.Keys.Count());
            Assert.AreEqual("nSpiroAtoms", value.Keys.First());
        }

        [TestMethod()]
        public void TestNorbornane()
        {
            var mol = sp.ParseSmiles("C1CC2CCC1C2"); // ethanol
            var value = CreateDescriptor(mol).Calculate();
            Assert.AreEqual(0, value.Value);
        }

        [TestMethod()]
        public void TestSpiroUndecane()
        {
            var mol = sp.ParseSmiles("C1CCC2(CC1)CCCCC2"); // ethanol
            var value = CreateDescriptor(mol).Calculate();
            Assert.AreEqual(1, value.Value);
        }

        [TestMethod()]
        public void TestDiSpiroPentane()
        {
            var mol = sp.ParseSmiles("CC1C[C]11(CC1)[C]123CC1.C2C3"); // ethanol
            var value = CreateDescriptor(mol).Calculate();
            Assert.AreEqual(2, value.Value);
        }

        [TestMethod()]
        public void TestSpiroNaphthalene()
        {
            var mol = sp.ParseSmiles("C1CCC2(CC1)CC=C1C=CC=CC1=C2"); // ethanol
            var value = CreateDescriptor(mol).Calculate();
            Assert.AreEqual(1, value.Value);
        }

        [TestMethod()]
        public void TestTriSpiro()
        {
            var mol = sp.ParseSmiles("C1OOC[Fe]1123COOC1.C2OOC3"); // ethanol
            var value = CreateDescriptor(mol).Calculate();
            Assert.AreEqual(1, value.Value);
        }
    }
}
