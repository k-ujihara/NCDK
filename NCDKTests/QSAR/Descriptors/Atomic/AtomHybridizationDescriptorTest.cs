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
using NCDK.QSAR.Result;
using NCDK.Smiles;
using NCDK.Tools.Manipulator;

namespace NCDK.QSAR.Descriptors.Atomic
{
    /// <summary>
    /// TestSuite that runs all QSAR tests.
    /// </summary>
    // @cdk.module test-qsaratomic
    [TestClass()]
    public class AtomHybridizationDescriptorTest : AtomicDescriptorTest
    {
        public AtomHybridizationDescriptorTest()
        {
            SetDescriptor(typeof(AtomHybridizationDescriptor));
        }

        [TestMethod()]
        public void TestAtomHybridizationDescriptorTest()
        {
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol = sp.ParseSmiles("C#CC=CC"); //
            AddExplicitHydrogens(mol);
            var expectedStates = new Hybridization[]{Hybridization.SP1,
                Hybridization.SP1, Hybridization.SP2, Hybridization.SP2,
                Hybridization.SP3};
            for (int i = 0; i < expectedStates.Length; i++)
            {
                Assert.AreEqual(expectedStates[i].Ordinal, ((IntegerResult)descriptor.Calculate(mol.Atoms[i], mol).Value).Value);
            }
        }

        [TestMethod()]
        public void TestBug1701073()
        {
            string[] smiles = new string[]{"C1CCCC=2[C]1(C(=O)NN2)C", "C1CCCC=2[C]1(C(=O)NN2)O",
                "C[Si](C)(C)[CH](Br)CC(F)(Br)F", "c1(ccc(cc1)O)C#N", "CCN(CC)C#CC#CC(=O)OC",
                "C(#CN1CCCCC1)[Sn](C)(C)C", "c1([As+](c2ccccc2)(c2ccccc2)C)ccccc1.[I-]",
                "c1(noc(n1)CCC(=O)N(CC)CC)c1ccc(cc1)C", "c1c(c(ccc1)O)/C=N/CCCC", "c1(ccc(cc1)C#Cc1ccc(cc1)C#C)OC"};

            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer mol;

            foreach (var smile in smiles)
            {
                mol = sp.ParseSmiles(smile);
                AddImplicitHydrogens(mol);
                AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(mol);
                foreach (var atom in mol.Atoms)
                {
                    var dummy = ((IntegerResult)descriptor.Calculate(atom, mol).Value).Value;
                }
            }
        }
    }
}
