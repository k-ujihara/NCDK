/* Copyright (C) 2010  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Smiles;
using NCDK.Tools.Manipulator;

namespace NCDK.AtomTypes
{
    /**
     * This class tests that a second atom typing results in the same atom
     * types as the first perception.
     *
     * @cdk.module test-core
     */
    [TestClass()]
    public class RepeatedCDKAtomTypeMatcherSMILESTest : CDKTestCase
    {
        private static SmilesParser smilesParser = new SmilesParser(Silent.ChemObjectBuilder.Instance);
        private static CDKAtomTypeMatcher atomTypeMatcher = CDKAtomTypeMatcher.GetInstance(Silent.ChemObjectBuilder.Instance);

        [TestMethod()]
        public void TestSMILES()
        {
            typeAndRetype("C=1N=CNC=1");
        }

        [TestMethod()]
        public void TestSMILES2()
        {
            typeAndRetype("OCN1C=CN=C1");
        }

        [TestMethod()]
        public void TestSMILES3()
        {
            typeAndRetype("OC(=O)N1C=CN=C1");
        }

        [TestMethod()]
        public void TestSMILES4()
        {
            typeAndRetype("CN(C)CCC1=CNC2=C1C=C(C=C2)CC1NC(=O)OC1");
        }

        [TestMethod()]
        public void TestSMILES5()
        {
            typeAndRetype("CN(C)CCC1=CNc2c1cc(cc2)CC1NC(=O)OC1");
        }

        [TestMethod()]
        public void TestSMILES6()
        {
            typeAndRetype("c1c2cc[NH]cc2nc1");
        }

        [TestMethod()]
        public void TestSMILES7()
        {
            typeAndRetype("c1cnc2s[cH][cH]n12");
        }

        [TestMethod()]
        public void TestSMILES8()
        {
            typeAndRetype("Cl[Pt]1(Cl)(Cl)(Cl)NC2CCCCC2N1");
        }

        [TestMethod()]
        public void TestSMILES9()
        {
            typeAndRetype("[Pt](Cl)(Cl)(N)N");
        }

        [TestMethod()]
        public void TestSMILES10()
        {
            typeAndRetype("CN(C)(=O)CCC=C2c1ccccc1CCc3ccccc23");
        }

        [TestMethod()]
        public void TestSMILES11()
        {
            typeAndRetype("CCCN1CC(CSC)CC2C1Cc3c[nH]c4cccc2c34");
        }

        private void typeAndRetype(string smiles)
        {
            IAtomContainer mol = smilesParser.ParseSmiles(smiles);
            IAtomType[] types = atomTypeMatcher.FindMatchingAtomTypes(mol);
            for (int i = 0; i < types.Length; i++)
            {
                AtomTypeManipulator.Configure(mol.Atoms[i], types[i]);
            }
            IAtomType[] retyped = atomTypeMatcher.FindMatchingAtomTypes(mol);
            for (int i = 0; i < types.Length; i++)
            {
                Assert.AreEqual(types[i], retyped[i],
                    "First perception resulted in " + types[i] + " but the second perception " + "gave "
                        + retyped[i]);
            }
            retyped = atomTypeMatcher.FindMatchingAtomTypes(mol);
            for (int i = 0; i < types.Length; i++)
            {
                Assert.AreEqual(types[i], retyped[i],
                    "First perception resulted in " + types[i] + " but the third perception " + "gave "
                        + retyped[i]);
            }
        }
    }
}
