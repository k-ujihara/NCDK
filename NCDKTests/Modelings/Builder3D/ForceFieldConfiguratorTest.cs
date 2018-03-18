/* Copyright (C) 2012 Daniel Szisz
 *
 * Contact: orlando@caesar.elte.hu
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
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
using NCDK.Smiles;
using NCDK.Tools;

namespace NCDK.Modelings.Builder3D
{
    /// <summary>
    /// Checks the functionality of <see cref="forceFieldConfigurator"/>.
    /// </summary>
    // @author danielszisz
    // @version 09/05/2012
    // @cdk.module test-forcefield
    [TestClass()]
    public class ForceFieldConfiguratorTest
    {
        ForceFieldConfigurator forceFieldConfigurator = new ForceFieldConfigurator();

        // @cdk.bug : ArrayIndexOutOfBoundsException because of wrong for loop
        [TestMethod()]
        public void TestCheckForceFieldType_String()
        {
            Assert.AreEqual(2, forceFieldConfigurator.GetFfTypes().Length);
            string validForceFieldType = "mm2";
            string invalidForceFieldType = "mmff2001";
            Assert.IsTrue(forceFieldConfigurator.CheckForceFieldType(validForceFieldType));
            Assert.IsFalse(forceFieldConfigurator.CheckForceFieldType(invalidForceFieldType));
        }

        [TestMethod()]
        public void TestSetForceFieldConfigurator_String()
        {
            string forceFieldName = "mmff94";
            forceFieldConfigurator.SetForceFieldConfigurator(forceFieldName, Default.ChemObjectBuilder.Instance);
            var mmff94AtomTypes = forceFieldConfigurator.AtomTypes;
            Assert.IsNotNull(mmff94AtomTypes);
            IAtomType atomtype0 = mmff94AtomTypes[0];
            Assert.AreEqual("C", atomtype0.AtomTypeName);
            IAtomType atomtype1 = mmff94AtomTypes[1];
            Assert.AreEqual("Csp2", atomtype1.AtomTypeName);

            forceFieldName = "mm2";
            forceFieldConfigurator.SetForceFieldConfigurator(forceFieldName, Default.ChemObjectBuilder.Instance);
            var mm2AtomTypes = forceFieldConfigurator.AtomTypes;
            Assert.IsNotNull(mm2AtomTypes);
            IAtomType atomtype2 = mm2AtomTypes[2];
            Assert.AreEqual("C=", atomtype2.AtomTypeName);
            IAtomType atomtype3 = mm2AtomTypes[3];
            Assert.AreEqual("Csp", atomtype3.AtomTypeName);
        }

        [TestMethod()]
        public void TestSetMM2Parameters()
        {
            forceFieldConfigurator.SetMM2Parameters(Default.ChemObjectBuilder.Instance);
            Assert.IsNotNull(forceFieldConfigurator.GetParameterSet());
            var atomtypeList = forceFieldConfigurator.AtomTypes;
            IAtomType atomtype1 = atomtypeList[1];
            Assert.AreEqual("Csp2", atomtype1.AtomTypeName);
            Assert.AreEqual(6, (int)atomtype1.AtomicNumber);
            Assert.AreEqual(12, (int)atomtype1.MassNumber);
        }

        [TestMethod()]
        public void TestSetMMFF94Parameters()
        {
            forceFieldConfigurator.SetMMFF94Parameters(Default.ChemObjectBuilder.Instance);
            Assert.IsNotNull(forceFieldConfigurator.GetParameterSet());
            var atomtypeList = forceFieldConfigurator.AtomTypes;
            IAtomType atomtype4 = atomtypeList[4];
            Assert.AreEqual("CO2M", atomtype4.AtomTypeName);
            Assert.AreEqual(6, (int)atomtype4.AtomicNumber);
            Assert.AreEqual(3, (int)atomtype4.FormalNeighbourCount);
            Assert.AreEqual(12, (int)atomtype4.MassNumber);
        }

        [TestMethod()]
        public void TestRemoveAromaticityFlagsFromHoseCode_String()
        {
            string hosecode1 = "***HO*SE*CODE***";
            string cleanHoseCode = forceFieldConfigurator.RemoveAromaticityFlagsFromHoseCode(hosecode1);
            Assert.AreEqual("HOSECODE", cleanHoseCode);
            string hosecode2 = "HOSECODE";
            cleanHoseCode = forceFieldConfigurator.RemoveAromaticityFlagsFromHoseCode(hosecode2);
            Assert.AreEqual("HOSECODE", cleanHoseCode);
        }

        // @cdk.bug  #3515122:  N atom type instead of NC=O
        [TestMethod(), Ignore()] // Old atom typing method - see new Mmff class
        public void TestConfigureMMFF94BasedAtom_IAtom_String_boolean_hydroxyurea()
        {
            string husmi = "NC(=O)NO";
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            SmilesParser parser = new SmilesParser(builder);
            IAtomContainer hu = parser.ParseSmiles(husmi);
            ForceFieldConfigurator ffc = new ForceFieldConfigurator();
            ffc.SetForceFieldConfigurator("mmff94", builder);
            IAtom N1 = hu.Atoms[0];
            IAtom N2 = hu.Atoms[3];
            ffc.ConfigureAtom(N1, new HOSECodeGenerator().GetHOSECode(hu, N1, 3), false);
            ffc.ConfigureAtom(N2, new HOSECodeGenerator().GetHOSECode(hu, N2, 3), false);
            Assert.AreEqual("NC=O", N1.AtomTypeName);
            Assert.AreEqual("N2OX", N2.AtomTypeName);
        }

        [TestMethod()]
        public void TestConfigureMMFF94BasedAtom_IAtom_String_boolean_propanamide()
        {
            string pasmi = "NC(=O)CC";
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            SmilesParser parser = new SmilesParser(builder);
            IAtomContainer pa = parser.ParseSmiles(pasmi);
            ForceFieldConfigurator ffc = new ForceFieldConfigurator();
            ffc.SetForceFieldConfigurator("mmff94", builder);
            IAtom amideN = pa.Atoms[0];
            ffc.ConfigureMMFF94BasedAtom(amideN, new HOSECodeGenerator().GetHOSECode(pa, amideN, 3), false);
            Assert.AreEqual("NC=O", amideN.AtomTypeName);
        }

        // @cdk.bug  #3515122 : mmff94 atomtype N instead of NC=O
        [TestMethod()]
        public void TestConfigureMMFF94BasedAtom_IAtom_String_boolean_urea()
        {
            string usmi = "NC(N)=O";
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            SmilesParser parser = new SmilesParser(builder);
            IAtomContainer urea = parser.ParseSmiles(usmi);
            ForceFieldConfigurator ffc = new ForceFieldConfigurator();
            ffc.SetForceFieldConfigurator("mmff94", builder);
            IAtom amideN = urea.Atoms[0];
            ffc.ConfigureMMFF94BasedAtom(amideN, new HOSECodeGenerator().GetHOSECode(urea, amideN, 3), false);
            //        Console.Error.WriteLine(amideN.AtomTypeName);
            Assert.AreEqual("NC=O", amideN.AtomTypeName);
        }

        // @cdk.bug : bad atom types
        [TestMethod(), Ignore()] // Old atom typing method - see new Mmff class
        public void TestAssignAtomTyps_test4_hydroxyurea()
        {
            string smiles = "C(=O)(NO)N";
            string[] originalAtomTypes = { "C.sp2", "O.sp2", "N.amide", "O.sp3", "N.amide" };
            string[] expectedAtomTypes = { "C=", "O=", "NC=O", "O", "N2OX" };
            IAtomContainer molecule = null;
            string[] ffAtomTypes = null;

            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            SmilesParser smilesParser = new SmilesParser(builder);
            molecule = smilesParser.ParseSmiles(smiles);
            ffAtomTypes = new string[molecule.Atoms.Count];

            for (int i = 0; i < molecule.Atoms.Count; i++)
            {
                Assert.AreEqual(originalAtomTypes[i], molecule.Atoms[i].AtomTypeName);
            }
            forceFieldConfigurator.SetForceFieldConfigurator("mmff94", builder);
            IRingSet moleculeRingSet = forceFieldConfigurator.AssignAtomTyps(molecule);
            //no rings
            Assert.AreEqual(0, moleculeRingSet.Count);
            for (int i = 0; i < molecule.Atoms.Count; i++)
            {
                IAtom mmff94atom = molecule.Atoms[i];
                Assert.IsTrue(mmff94atom.IsAliphatic);
                ffAtomTypes[i] = mmff94atom.AtomTypeName;
            }
            Assert.AreEqual(expectedAtomTypes, ffAtomTypes);
        }

        // @cdk.bug #3523240
        [TestMethod(), Ignore()] // Old atom typing method - see new Mmff class
        public void TestAssignAtomTyps_bug()
        {
            string smiles = "CC(C)C1CCC(CC1)C(=O)NC(Cc1ccccc1)C(=O)O";
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            SmilesParser parser = new SmilesParser(builder);
            IAtomContainer bugmol = parser.ParseSmiles(smiles);
            forceFieldConfigurator.SetForceFieldConfigurator("mmff94", builder);
            IAtom amideN = bugmol.Atoms[11];
            forceFieldConfigurator.ConfigureMMFF94BasedAtom(amideN, new HOSECodeGenerator().GetHOSECode(bugmol, amideN, 3), false);
            Assert.AreEqual("NC=O", amideN.AtomTypeName);
        }

        // @cdk.bug #3524734
        [TestMethod(), Ignore()] // Old atom typing method - see new Mmff class
        public void TestAssignAtomTyps_bug_no2()
        {
            string smiles = "CC[N+](=O)[O-]";
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            SmilesParser parser = new SmilesParser(builder);
            IAtomContainer bugmol = parser.ParseSmiles(smiles);
            forceFieldConfigurator.SetForceFieldConfigurator("mmff94", builder);
            IAtom amideN = bugmol.Atoms[2];
            forceFieldConfigurator.ConfigureMMFF94BasedAtom(amideN, new HOSECodeGenerator().GetHOSECode(bugmol, amideN, 3), false);
            Assert.AreEqual("NO3", amideN.AtomTypeName);
        }

        // @cdk.bug #3525096
        [TestMethod()]
        public void TestAssignAtomTyps_bug_so2()
        {
            string smiles = "CS(=O)(=O)NC(=O)NN1CC2CCCC2C1";
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            SmilesParser parser = new SmilesParser(builder);
            IAtomContainer bugmol = parser.ParseSmiles(smiles);
            forceFieldConfigurator.SetForceFieldConfigurator("mmff94", builder);
            IAtom sulphur = bugmol.Atoms[1];
            HOSECodeGenerator hscodegen = new HOSECodeGenerator();
            forceFieldConfigurator.ConfigureAtom(sulphur, hscodegen.GetHOSECode(bugmol, sulphur, 3), false);
            Assert.AreEqual("SO2", sulphur.AtomTypeName);
        }

        // @cdk.bug #3525144
        [TestMethod(), Ignore()] // Old atom typing method - see new Mmff class
        public void TestAssignAtomTyps_bug_nitrogenatomType()
        {
            string smiles = "CNC(=O)N(C)N=O";
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            SmilesParser parser = new SmilesParser(builder);
            IAtomContainer bugmol = parser.ParseSmiles(smiles);
            forceFieldConfigurator.SetForceFieldConfigurator("mmff94", builder);
            IAtom nitrogen1 = bugmol.Atoms[1];
            IAtom nitrogen2 = bugmol.Atoms[4];
            IAtom nitrogen3 = bugmol.Atoms[6];
            HOSECodeGenerator hscodegen = new HOSECodeGenerator();
            forceFieldConfigurator.ConfigureAtom(nitrogen1, hscodegen.GetHOSECode(bugmol, nitrogen1, 3), false);
            forceFieldConfigurator.ConfigureAtom(nitrogen2, hscodegen.GetHOSECode(bugmol, nitrogen2, 3), false);
            forceFieldConfigurator.ConfigureAtom(nitrogen3, hscodegen.GetHOSECode(bugmol, nitrogen3, 3), false);
            Assert.AreEqual("NC=O", nitrogen1.AtomTypeName);
            Assert.AreEqual("NC=O", nitrogen2.AtomTypeName);
        }

        // @cdk.bug #3526295
        [TestMethod()]
        public void TestAssignAtomTyps_bug_amideRingAtomType()
        {
            string smiles = "O=C1N(C(=O)C(C(=O)N1)(CC)CC)C";
            IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;
            SmilesParser parser = new SmilesParser(builder);
            IAtomContainer bugmol = parser.ParseSmiles(smiles);
            forceFieldConfigurator.SetForceFieldConfigurator("mmff94", builder);
            IAtom nitrogen1 = bugmol.Atoms[2];
            HOSECodeGenerator hscodegen = new HOSECodeGenerator();
            forceFieldConfigurator.ConfigureAtom(nitrogen1, hscodegen.GetHOSECode(bugmol, nitrogen1, 3), false);
            Assert.AreEqual("NC=O", nitrogen1.AtomTypeName);
        }
    }
}
