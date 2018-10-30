/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All I ask is that proper credit is given for my work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
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
using NCDK.Config;
using NCDK.Graphs;
using NCDK.Smiles;
using NCDK.Templates;

namespace NCDK.StructGen
{
    // @cdk.module test-structgen
    [TestClass()]
    public class VicinitySamplerTest : CDKTestCase
    {
        private static SmilesParser parser;

        static VicinitySamplerTest()
        {
            parser = CDK.SilentSmilesParser;
        }

        [TestMethod()]
        public void TestVicinitySampler_sample()
        {
            IAtomContainer mol = TestMoleculeFactory.MakeEthylPropylPhenantren();

            BODRIsotopeFactory.Instance.ConfigureAtoms(mol);
            AddImplicitHydrogens(mol);

            var structures = VicinitySampler.Sample(mol);
            var count = 0;
            foreach (var temp in structures)
            {
                Assert.IsNotNull(temp);
                Assert.IsTrue(ConnectivityChecker.IsConnected(temp));
                Assert.AreEqual(mol.Atoms.Count, temp.Atoms.Count);
                count++;
            }
            Assert.AreEqual(37, count);
        }

        // @cdk.bug 1632610
        public void TestCycloButene()
        {
            var mol = parser.ParseSmiles("C=CC=C");

            BODRIsotopeFactory.Instance.ConfigureAtoms(mol);
            AddImplicitHydrogens(mol);

            var structures = VicinitySampler.Sample(mol);
            var count = 0;
            foreach (var temp in structures)
            {
                Assert.IsNotNull(temp);
                Assert.IsTrue(ConnectivityChecker.IsConnected(temp));
                Assert.AreEqual(mol.Atoms.Count, temp.Atoms.Count);
                count++;
            }
            Assert.AreEqual(1, count);
        }
    }
}
