/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
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

namespace NCDK.QSAR.Descriptors.Atomic
{
    /// <summary>
    /// TestSuite that runs all QSAR tests.
    /// </summary>
    // @cdk.module test-qsaratomic
    [TestClass()]
    public class IsProtonInConjugatedPiSystemDescriptorTest : AtomicDescriptorTest<IsProtonInConjugatedPiSystemDescriptor>
    {
        public IsProtonInConjugatedPiSystemDescriptor CreateDescriptor(IAtomContainer mol, bool checkAromaticity) => new IsProtonInConjugatedPiSystemDescriptor(mol, checkAromaticity);

        [TestMethod()]
        public void TestIsProtonInConjugatedPiSystemDescriptor()
        {
            var sp = CDK.SmilesParser;
            var mol = sp.ParseSmiles("CNC=CC=C");
            AddExplicitHydrogens(mol);
            var descriptor = CreateDescriptor(mol, true);
            Assert.IsTrue(descriptor.Calculate(mol.Atoms[13]).Value);
        }
    }
}
