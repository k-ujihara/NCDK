/* Copyright (C) 2007-2011  Egon Willighagen <egonw@users.sf.net>
 *               2007       Rajarshi Guha
 *                    2011  Nimish Gopal <nimishg@ebi.ac.uk>
 *                    2011  Syed Asad Rahman <asad@ebi.ac.uk>
 *                    2011  Gilleain Torrance <gilleain.torrance@gmail.com>
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
using NCDK.Config;
using NCDK.Templates;
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.AtomTypes
{
    /// <summary>
    /// This class tests the matching of atom types defined in the
    /// CDK atom type list. All tests in this class <b>must</b> use
    /// explicit <see cref="IAtomContainer"/>s; test using data files
    /// must be placed in <see cref="CDKAtomTypeMatcherFilesTest"/>.
    /// </summary>
    // @cdk.module test-core
    [TestClass()]
    public class CDKAtomTypeMatcherTest : AbstractCDKAtomTypeTest
    {
        private static readonly Dictionary<string, int> testedAtomTypes = new Dictionary<string, int>();

        private readonly IChemObjectBuilder builder = CDK.Builder;

        [TestMethod()]
        public void TestGetInstance_IChemObjectBuilder()
        {
            var matcher = CDK.AtomTypeMatcher;
            Assert.IsNotNull(matcher);
        }

        [TestMethod()]
        public void TestGetInstance_IChemObjectBuilder_int()
        {
            var matcher = CDKAtomTypeMatcher.GetInstance(CDKAtomTypeMatcher.Mode.RequireExplicitHydrogens);
            Assert.IsNotNull(matcher);
        }

        [TestMethod()]
        public void TestFindMatchingAtomType_IAtomContainer_IAtom()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("C");
            var thisHybridization = Hybridization.SP3;
            atom.Hybridization = thisHybridization;
            mol.Atoms.Add(atom);

            string[] expectedTypes = { "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestFindMatchingAtomType_IAtomContainer()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("C");
            var thisHybridization = Hybridization.SP3;
            atom.Hybridization = thisHybridization;
            mol.Atoms.Add(atom);

            // just check consistency; other methods do perception testing
            var matcher = CDK.AtomTypeMatcher;
            var types = matcher.FindMatchingAtomTypes(mol).ToReadOnlyList();
            for (int i = 0; i < types.Count; i++)
            {
                var type = matcher.FindMatchingAtomType(mol, mol.Atoms[i]);
                Assert.AreEqual(type.AtomTypeName, types[i].AtomTypeName);
            }
        }

        [TestMethod()]
        public void TestDummy()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewPseudoAtom("R");
            mol.Atoms.Add(atom);

            string[] expectedTypes = { "X" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.bug 2445178
        [TestMethod()]
        public void TestNonExistingType()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom();
            mol.Atoms.Add(atom);
            var matcher = CDK.AtomTypeMatcher;
            var type = matcher.FindMatchingAtomType(mol, atom);
            Assert.IsNotNull(type);
            Assert.AreEqual("X", type.AtomTypeName);
        }

        [TestMethod()]
        public void TestEthene()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);

            string[] expectedTypes = { "C.sp2", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestEthyneKation()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("C");
            atom2.FormalCharge = +1;
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Triple);

            string[] expectedTypes = { "C.sp", "C.plus.sp1" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestEthyneRadical()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddSingleElectronTo(mol.Atoms[1]);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Triple);

            string[] expectedTypes = { "C.sp", "C.radical.sp1" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestImine()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("N");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);

            string[] expectedTypes = { "C.sp2", "N.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestImineRadical()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("N");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddSingleElectronTo(mol.Atoms[1]);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);

            string[] expectedTypes = { "C.sp2", "N.sp2.radical" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestEtheneRadical()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddSingleElectronTo(mol.Atoms[1]);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);

            string[] expectedTypes = { "C.sp2", "C.radical.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestGuanineMethyl()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom1 = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("N");
            IAtom atom3 = builder.NewAtom("C");
            IAtom atom4 = builder.NewAtom("N");
            IAtom atom5 = builder.NewAtom("C");
            IAtom atom6 = builder.NewAtom("C");
            IAtom atom7 = builder.NewAtom("N");
            IAtom atom8 = builder.NewAtom("C");
            IAtom atom9 = builder.NewAtom("C");
            IAtom atom10 = builder.NewAtom("N");
            IAtom atom11 = builder.NewAtom("O");
            IAtom atom12 = builder.NewAtom("N");
            mol.Atoms.Add(atom1);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.Atoms.Add(atom5);
            mol.Atoms.Add(atom6);
            mol.Atoms.Add(atom7);
            mol.Atoms.Add(atom8);
            mol.Atoms.Add(atom9);
            mol.Atoms.Add(atom10);
            mol.Atoms.Add(atom11);
            mol.Atoms.Add(atom12);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Double);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[4], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[5], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[6], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[7], BondOrder.Single);
            mol.AddBond(mol.Atoms[3], mol.Atoms[8], BondOrder.Double);
            mol.AddBond(mol.Atoms[5], mol.Atoms[6], BondOrder.Double);
            mol.AddBond(mol.Atoms[7], mol.Atoms[9], BondOrder.Single);
            mol.AddBond(mol.Atoms[7], mol.Atoms[10], BondOrder.Double);
            mol.AddBond(mol.Atoms[8], mol.Atoms[11], BondOrder.Single);
            mol.AddBond(mol.Atoms[8], mol.Atoms[9], BondOrder.Single);

            string[] expectedTypes = {"C.sp2", "N.planar3", "C.sp2", "N.sp2", "C.sp3", "C.sp2", "N.sp2", "C.sp2", "C.sp2",
                "N.amide", "O.sp2", "N.sp3"};
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestPropyne()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("C");
            IAtom atom3 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Triple);
            mol.AddBond(mol.Atoms[2], mol.Atoms[1], BondOrder.Single);

            string[] expectedTypes = { "C.sp", "C.sp", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestFormaldehyde()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("O");
            IAtom atom2 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);

            string[] expectedTypes = { "O.sp2", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestCarboxylate()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("O");
            IAtom atom1 = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("O");
            atom2.FormalCharge = -1;
            IAtom atom3 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom1);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);

            string[] expectedTypes = { "O.sp2.co2", "C.sp2", "O.minus.co2", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestFormaldehydeRadicalKation()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("O");
            atom.FormalCharge = +1;
            IAtom atom2 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddSingleElectronTo(mol.Atoms[0]);

            string[] expectedTypes = { "O.plus.sp2.radical", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        /// <summary>
        /// Aim of this test is to see if the atom type matcher is OK with
        /// partial filled implicit hydrogen counts.
        /// </summary>
        [TestMethod()]
        public void TestPartialMethane()
        {
            IAtomContainer methane = builder.NewAtomContainer();
            IAtom carbon = builder.NewAtom("C");
            methane.Atoms.Add(carbon);

            string[] expectedTypes = { "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, methane);

            carbon.ImplicitHydrogenCount = 1;
            AssertAtomTypes(testedAtomTypes, expectedTypes, methane);

            carbon.ImplicitHydrogenCount = 2;
            AssertAtomTypes(testedAtomTypes, expectedTypes, methane);

            carbon.ImplicitHydrogenCount = 3;
            AssertAtomTypes(testedAtomTypes, expectedTypes, methane);

            carbon.ImplicitHydrogenCount = 4;
            AssertAtomTypes(testedAtomTypes, expectedTypes, methane);
        }

        [TestMethod()]
        public void TestMethanol()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("O");
            IAtom atom2 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);

            string[] expectedTypes = { "O.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestLithiumMethanoxide()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("O");
            IAtom atom2 = builder.NewAtom("C");
            IAtom atom3 = builder.NewAtom("Li");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);

            string[] expectedTypes = { "O.sp3", "C.sp3", "Li" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestHCN()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("N");
            IAtom atom2 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Triple);

            string[] expectedTypes = { "N.sp1", "C.sp" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestHNO2()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("N");
            IAtom atom2 = builder.NewAtom("O");
            IAtom atom3 = builder.NewAtom("O");
            IAtom atom4 = builder.NewAtom("H");
            mol.Atoms.Add(atom);
            atom.FormalCharge = +1;
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            atom3.FormalCharge = -1;
            mol.Atoms.Add(atom4);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);

            string[] expectedTypes = { "N.plus.sp2", "O.sp2", "O.minus", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestNitromethane()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("N");
            IAtom atom2 = builder.NewAtom("O");
            IAtom atom3 = builder.NewAtom("O");
            IAtom atom4 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Double);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);

            string[] expectedTypes = { "N.nitro", "O.sp2", "O.sp2", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestMethylAmine()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("N");
            IAtom atom2 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);

            string[] expectedTypes = { "N.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestMethylAmineRadical()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("N");
            IAtom atom2 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddSingleElectronTo(mol.Atoms[0]);
            atom.FormalCharge = +1;
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);

            string[] expectedTypes = { "N.plus.sp3.radical", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestMethyleneImine()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("N");
            IAtom atom2 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);

            string[] expectedTypes = { "N.sp2", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestEthene_withHybridInfo()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("C");
            Hybridization thisHybridization = Hybridization.SP2;
            atom.Hybridization = thisHybridization;
            atom2.Hybridization = thisHybridization;
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);

            string[] expectedTypes = { "C.sp2", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestPiperidine()
        {
            IAtomContainer molecule = TestMoleculeFactory.MakePiperidine();
            string[] expectedTypes = { "N.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestTetrahydropyran()
        {
            IAtomContainer molecule = TestMoleculeFactory.MakeTetrahydropyran();
            string[] expectedTypes = { "O.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestS3()
        {
            var mol = builder.NewAtomContainer();
            IAtom s = builder.NewAtom("S");
            IAtom o1 = builder.NewAtom("O");
            IAtom o2 = builder.NewAtom("O");

            IBond b1 = builder.NewBond(s, o1, BondOrder.Double);
            IBond b2 = builder.NewBond(s, o2, BondOrder.Double);

            mol.Atoms.Add(s);
            mol.Atoms.Add(o1);
            mol.Atoms.Add(o2);

            mol.Bonds.Add(b1);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "S.oxide", "O.sp2", "O.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestH2S()
        {
            var mol = builder.NewAtomContainer();
            IAtom s = builder.NewAtom("S");
            IAtom h1 = builder.NewAtom("H");
            IAtom h2 = builder.NewAtom("H");

            IBond b1 = builder.NewBond(s, h1, BondOrder.Single);
            IBond b2 = builder.NewBond(s, h2, BondOrder.Single);

            mol.Atoms.Add(s);
            mol.Atoms.Add(h1);
            mol.Atoms.Add(h2);

            mol.Bonds.Add(b1);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "S.3", "H", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.inchi InChI=1/H2Se/h1H2
        [TestMethod()]
        public void TestH2Se()
        {
            var mol = builder.NewAtomContainer();
            IAtom se = builder.NewAtom("Se");
            IAtom h1 = builder.NewAtom("H");
            IAtom h2 = builder.NewAtom("H");

            IBond b1 = builder.NewBond(se, h1, BondOrder.Single);
            IBond b2 = builder.NewBond(se, h2, BondOrder.Single);

            mol.Atoms.Add(se);
            mol.Atoms.Add(h1);
            mol.Atoms.Add(h2);

            mol.Bonds.Add(b1);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "Se.3", "H", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.inchi InChI=1/H2Se/h1H2
        [TestMethod()]
        public void TestH2Se_oneImplH()
        {
            var mol = builder.NewAtomContainer();
            IAtom se = builder.NewAtom("Se");
            se.ImplicitHydrogenCount = 1;
            IAtom h1 = builder.NewAtom("H");

            IBond b1 = builder.NewBond(se, h1, BondOrder.Single);

            mol.Atoms.Add(se);
            mol.Atoms.Add(h1);
            mol.Bonds.Add(b1);

            string[] expectedTypes = { "Se.3", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.inchi InChI=1/H2Se/h1H2
        [TestMethod()]
        public void TestH2Se_twoImplH()
        {
            var mol = builder.NewAtomContainer();
            IAtom se = builder.NewAtom("Se");
            se.ImplicitHydrogenCount = 2;
            mol.Atoms.Add(se);

            string[] expectedTypes = { "Se.3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestSelenide()
        {
            var mol = builder.NewAtomContainer();
            IAtom se = builder.NewAtom("Se");
            se.ImplicitHydrogenCount = 0;
            se.FormalCharge = -2;
            mol.Atoms.Add(se);

            string[] expectedTypes = { "Se.2minus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestH2S_Hybridization()
        {
            var mol = builder.NewAtomContainer();
            IAtom s = builder.NewAtom("S");
            s.Hybridization = Hybridization.SP3;
            mol.Atoms.Add(s);
            string[] expectedTypes = { "S.3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestHS()
        {
            var mol = builder.NewAtomContainer();
            IAtom s = builder.NewAtom("S");
            s.FormalCharge = -1;
            IAtom h1 = builder.NewAtom("H");

            IBond b1 = builder.NewBond(s, h1, BondOrder.Single);

            mol.Atoms.Add(s);
            mol.Atoms.Add(h1);
            mol.Bonds.Add(b1);

            string[] expectedTypes = { "S.minus", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestDMSOCharged()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("O");
            atom.FormalCharge = -1;
            IAtom atom2 = builder.NewAtom("S");
            atom2.FormalCharge = 1;
            IAtom atom3 = builder.NewAtom("C");
            IAtom atom4 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);

            string[] expectedTypes = { "O.minus", "S.inyl.charged", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestDMSO()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("O");
            IAtom atom2 = builder.NewAtom("S");
            IAtom atom3 = builder.NewAtom("C");
            IAtom atom4 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);

            string[] expectedTypes = { "O.sp2", "S.inyl", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestDMSOO()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("O");
            IAtom atom1 = builder.NewAtom("O");
            IAtom atom2 = builder.NewAtom("S");
            IAtom atom3 = builder.NewAtom("C");
            IAtom atom4 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom1);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Double);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "O.sp2", "O.sp2", "S.onyl", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestStrioxide()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("O");
            IAtom atom1 = builder.NewAtom("O");
            IAtom atom2 = builder.NewAtom("S");
            IAtom atom3 = builder.NewAtom("O");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom1);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Double);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Double);

            string[] expectedTypes = { "O.sp2", "O.sp2", "S.trioxide", "O.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestAmide()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("O");
            IAtom atom2 = builder.NewAtom("C");
            IAtom atom3 = builder.NewAtom("N");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);

            string[] expectedTypes = { "O.sp2", "C.sp2", "N.amide" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestAmineOxide()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("O");
            IAtom atom2 = builder.NewAtom("N");
            IAtom atom3 = builder.NewAtom("C");
            IAtom atom4 = builder.NewAtom("C");
            IAtom atom5 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.Atoms.Add(atom5);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "O.sp2", "N.oxide", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestThioAmide()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("S");
            IAtom atom2 = builder.NewAtom("C");
            IAtom atom3 = builder.NewAtom("N");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);

            string[] expectedTypes = { "S.2", "C.sp2", "N.thioamide" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestAdenine()
        {
            var mol = TestMoleculeFactory.MakeAdenine();
            string[] expectedTypes = {"C.sp2", "C.sp2", "C.sp2", "N.sp2", "N.sp2", "N.planar3", "N.sp2", "N.sp3", "C.sp2",
                "C.sp2"};
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestAmide2()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("O");
            IAtom atom1 = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("N");
            IAtom atom3 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom1);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);

            string[] expectedTypes = { "O.sp2", "C.sp2", "N.amide", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestAmide3()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("O");
            IAtom atom1 = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("N");
            IAtom atom3 = builder.NewAtom("C");
            IAtom atom4 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom1);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "O.sp2", "C.sp2", "N.amide", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestLactam()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("O");
            IAtom atom1 = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("N");
            IAtom atom3 = builder.NewAtom("C");
            IAtom atom4 = builder.NewAtom("C");
            IAtom atom5 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom1);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.Atoms.Add(atom5);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[4], mol.Atoms[5], BondOrder.Single);
            mol.AddBond(mol.Atoms[5], mol.Atoms[1], BondOrder.Single);

            string[] expectedTypes = { "O.sp2", "C.sp2", "N.amide", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestThioAcetone()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("S");
            IAtom atom2 = builder.NewAtom("C");
            IAtom atom3 = builder.NewAtom("C");
            IAtom atom4 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);

            string[] expectedTypes = { "S.2", "C.sp2", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestSulphuricAcid()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("O");
            IAtom atom2 = builder.NewAtom("S");
            IAtom atom3 = builder.NewAtom("O");
            IAtom atom4 = builder.NewAtom("O");
            IAtom atom5 = builder.NewAtom("O");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.Atoms.Add(atom5);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "O.sp2", "S.onyl", "O.sp2", "O.sp3", "O.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.inchi InChI=1S/CH4O2S2/c1-5(2,3)4/h1H3,(H,2,3,4)
        [TestMethod()]
        public void TestThioSulphonate()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("S");
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("S");
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("O");
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("O");
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("C");
            mol.Atoms.Add(a5);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a4, BondOrder.Double);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);

            string[] expectedTypes = { "S.thionyl", "S.2", "O.sp3", "O.sp2", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestSulphuricAcid_Charged()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("O");
            IAtom atom2 = builder.NewAtom("S");
            IAtom atom3 = builder.NewAtom("O");
            IAtom atom4 = builder.NewAtom("O");
            IAtom atom5 = builder.NewAtom("O");
            mol.Atoms.Add(atom);
            atom.FormalCharge = -1;
            mol.Atoms.Add(atom2);
            atom2.FormalCharge = +2;
            mol.Atoms.Add(atom3);
            atom3.FormalCharge = -1;
            mol.Atoms.Add(atom4);
            mol.Atoms.Add(atom5);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "O.minus", "S.onyl.charged", "O.minus", "O.sp3", "O.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestSF6()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("F");
            IAtom atom2 = builder.NewAtom("S");
            IAtom atom3 = builder.NewAtom("F");
            IAtom atom4 = builder.NewAtom("F");
            IAtom atom5 = builder.NewAtom("F");
            IAtom atom6 = builder.NewAtom("F");
            IAtom atom7 = builder.NewAtom("F");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.Atoms.Add(atom5);
            mol.Atoms.Add(atom6);
            mol.Atoms.Add(atom7);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[5], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[6], BondOrder.Single);

            string[] expectedTypes = { "F", "S.octahedral", "F", "F", "F", "F", "F" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestMnF4()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("F");
            IAtom atom2 = builder.NewAtom("Mn");
            IAtom atom3 = builder.NewAtom("F");
            IAtom atom4 = builder.NewAtom("F");
            IAtom atom5 = builder.NewAtom("F");
            mol.Atoms.Add(atom);
            atom.FormalCharge = -1;
            mol.Atoms.Add(atom2);
            atom2.FormalCharge = +2;
            mol.Atoms.Add(atom3);
            atom3.FormalCharge = -1;
            mol.Atoms.Add(atom4);
            atom4.FormalCharge = -1;
            mol.Atoms.Add(atom5);
            atom5.FormalCharge = -1;

            string[] expectedTypes = { "F.minus", "Mn.2plus", "F.minus", "F.minus", "F.minus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestCrF6()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("F");
            IAtom atom2 = builder.NewAtom("Cr");
            IAtom atom3 = builder.NewAtom("F");
            IAtom atom4 = builder.NewAtom("F");
            IAtom atom5 = builder.NewAtom("F");
            IAtom atom6 = builder.NewAtom("F");
            IAtom atom7 = builder.NewAtom("F");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.Atoms.Add(atom5);
            mol.Atoms.Add(atom6);
            mol.Atoms.Add(atom7);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[5], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[6], BondOrder.Single);

            string[] expectedTypes = { "F", "Cr", "F", "F", "F", "F", "F" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestXeF4()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("F");
            IAtom atom2 = builder.NewAtom("Xe");
            IAtom atom3 = builder.NewAtom("F");
            IAtom atom4 = builder.NewAtom("F");
            IAtom atom5 = builder.NewAtom("F");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.Atoms.Add(atom5);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "F", "Xe.3", "F", "F", "F" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestPhosphate()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("O");
            IAtom atom2 = builder.NewAtom("P");
            IAtom atom3 = builder.NewAtom("O");
            IAtom atom4 = builder.NewAtom("O");
            IAtom atom5 = builder.NewAtom("O");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.Atoms.Add(atom5);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "O.sp2", "P.ate", "O.sp3", "O.sp3", "O.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.inchi InChI=1/C3H10OP/c1-5(2,3)4/h4H,1-3H3/q+1
        [TestMethod()]
        public void TestHydroxyTriMethylPhophanium()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("P"); atom2.FormalCharge = +1;
            IAtom atom3 = builder.NewAtom("C");
            IAtom atom4 = builder.NewAtom("C");
            IAtom atom5 = builder.NewAtom("O");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.Atoms.Add(atom5);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "C.sp3", "P.ate.charged", "C.sp3", "C.sp3", "O.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestPhosphateCharged()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("O"); atom.FormalCharge = -1;
            IAtom atom2 = builder.NewAtom("P"); atom2.FormalCharge = 1;
            IAtom atom3 = builder.NewAtom("O");
            IAtom atom4 = builder.NewAtom("O");
            IAtom atom5 = builder.NewAtom("O");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.Atoms.Add(atom5);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "O.minus", "P.ate.charged", "O.sp3", "O.sp3", "O.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestPhosphorusTriradical()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("P");
            mol.Atoms.Add(atom);
            mol.AddSingleElectronTo(mol.Atoms[0]);
            mol.AddSingleElectronTo(mol.Atoms[0]);
            mol.AddSingleElectronTo(mol.Atoms[0]);

            string[] expectedTypes = { "P.se.3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestAmmonia()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("H");
            IAtom atom2 = builder.NewAtom("N");
            IAtom atom3 = builder.NewAtom("H");
            IAtom atom4 = builder.NewAtom("H");
            IAtom atom5 = builder.NewAtom("H");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            atom2.FormalCharge = +1;
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.Atoms.Add(atom5);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "H", "N.plus", "H", "H", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestNitrogenRadical()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("H");
            IAtom atom2 = builder.NewAtom("N");
            IAtom atom3 = builder.NewAtom("H");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddSingleElectronTo(mol.Atoms[1]);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);

            string[] expectedTypes = { "H", "N.sp3.radical", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestTMS()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("Si");
            IAtom atom3 = builder.NewAtom("C");
            IAtom atom4 = builder.NewAtom("C");
            IAtom atom5 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.Atoms.Add(atom5);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "C.sp3", "Si.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestTinCompound()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("Sn");
            IAtom atom3 = builder.NewAtom("C");
            IAtom atom4 = builder.NewAtom("C");
            IAtom atom5 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.Atoms.Add(atom5);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "C.sp3", "Sn.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestArsenicPlus()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("As"); atom2.FormalCharge = +1;
            IAtom atom3 = builder.NewAtom("C");
            IAtom atom4 = builder.NewAtom("C");
            IAtom atom5 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.Atoms.Add(atom5);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "C.sp3", "As.plus", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestPhosphine()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("H");
            IAtom atom2 = builder.NewAtom("P");
            IAtom atom3 = builder.NewAtom("H");
            IAtom atom4 = builder.NewAtom("H");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);

            string[] expectedTypes = { "H", "P.ine", "H", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.inchi InChI=1S/HO3P/c1-4(2)3/h(H-,1,2,3)/p+1
        [TestMethod()]
        public void TestPhosphorousAcid()
        {
            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("P");
            a1.FormalCharge = 1;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("O");
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("O");
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("O");
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("H");
            mol.Atoms.Add(a5);
            IAtom a6 = builder.NewAtom("H");
            mol.Atoms.Add(a6);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a4, BondOrder.Double);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a2, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.NewBond(a3, a6, BondOrder.Single);
            mol.Bonds.Add(b5);

            string[] expectedTypes = { "P.anium", "O.sp3", "O.sp3", "O.sp2", "H", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestDiethylPhosphine()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("P");
            IAtom atom3 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);

            string[] expectedTypes = { "C.sp3", "P.ine", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.inchi InChI=1/C2H5P/c1-3-2/h1H2,2H3
        [TestMethod()]
        public void TestPhosphorCompound()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("P");
            IAtom atom3 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);

            string[] expectedTypes = { "C.sp2", "P.irane", "C.sp3" }; // FIXME: compare with previous test... can't both be P.ine...
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestCarbokation()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("H");
            IAtom atom2 = builder.NewAtom("C"); atom2.FormalCharge = +1;
            IAtom atom3 = builder.NewAtom("H");
            IAtom atom4 = builder.NewAtom("H");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);

            string[] expectedTypes = { "H", "C.plus.planar", "H", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestCarbokation_implicitHydrogen()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom2 = builder.NewAtom("C"); atom2.FormalCharge = +1;
            mol.Atoms.Add(atom2);

            string[] expectedTypes = { "C.plus.sp2" }; // FIXME: compare with previous test... same compound!
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestHydrogen()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("H");
            mol.Atoms.Add(atom);

            string[] expectedTypes = { "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestHydroxyl()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("H");
            IAtom oxygen = builder.NewAtom("O"); oxygen.FormalCharge = -1;
            mol.Atoms.Add(atom);
            mol.Atoms.Add(oxygen);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);

            string[] expectedTypes = { "H", "O.minus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestHydroxyl2()
        {
            var mol = builder.NewAtomContainer();
            IAtom oxygen = builder.NewAtom("O"); oxygen.FormalCharge = -1;
            mol.Atoms.Add(oxygen);

            string[] expectedTypes = { "O.minus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestHydroxonium()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("H");
            IAtom atom1 = builder.NewAtom("H");
            IAtom atom2 = builder.NewAtom("H");
            IAtom oxygen = builder.NewAtom("O"); oxygen.FormalCharge = +1;
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom1);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(oxygen);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);

            string[] expectedTypes = { "H", "H", "H", "O.plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestPositiveCarbonyl()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("H");
            IAtom atom1 = builder.NewAtom("H");
            IAtom atom2 = builder.NewAtom("H");
            IAtom oxygen = builder.NewAtom("O");
            IAtom carbon = builder.NewAtom("C");
            oxygen.FormalCharge = +1;
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom1);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(oxygen);
            mol.Atoms.Add(carbon);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Double);

            string[] expectedTypes = { "H", "H", "H", "O.plus.sp2", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestProton()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("H"); atom.FormalCharge = 1;
            mol.Atoms.Add(atom);

            string[] expectedTypes = { "H.plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestHalides()
        {
            var mol = builder.NewAtomContainer();

            IAtom atom = builder.NewAtom("Cl"); atom.FormalCharge = -1;
            mol.Atoms.Add(atom);
            string[] expectedTypes = { "Cl.minus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = builder.NewAtomContainer();
            atom = builder.NewAtom("F"); atom.FormalCharge = -1;
            mol.Atoms.Add(atom);
            expectedTypes = new string[] { "F.minus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = builder.NewAtomContainer();
            atom = builder.NewAtom("Br"); atom.FormalCharge = -1;
            mol.Atoms.Add(atom);
            expectedTypes = new string[] { "Br.minus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = builder.NewAtomContainer();
            atom = builder.NewAtom("I"); atom.FormalCharge = -1;
            mol.Atoms.Add(atom);
            expectedTypes = new string[] { "I.minus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestHalogens()
        {
            var mol = builder.NewAtomContainer();

            IAtom atom = builder.NewAtom("Cl");
            IAtom hydrogen = builder.NewAtom("H");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(hydrogen);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            string[] expectedTypes = new string[] { "Cl", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = builder.NewAtomContainer();
            atom = builder.NewAtom("I");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(hydrogen);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            expectedTypes = new string[] { "I", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = builder.NewAtomContainer();
            atom = builder.NewAtom("Br");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(hydrogen);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            expectedTypes = new string[] { "Br", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = builder.NewAtomContainer();
            atom = builder.NewAtom("F");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(hydrogen);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            expectedTypes = new string[] { "F", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestFluorRadical()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("F");
            mol.Atoms.Add(atom);
            mol.AddSingleElectronTo(mol.Atoms[0]);

            string[] expectedTypes = { "F.radical" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestChlorRadical()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("Cl");
            mol.Atoms.Add(atom);
            mol.AddSingleElectronTo(mol.Atoms[0]);

            string[] expectedTypes = { "Cl.radical" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestBromRadical()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("Br");
            mol.Atoms.Add(atom);
            mol.AddSingleElectronTo(mol.Atoms[0]);

            string[] expectedTypes = { "Br.radical" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestIodRadical()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("I");
            mol.Atoms.Add(atom);
            mol.AddSingleElectronTo(mol.Atoms[0]);

            string[] expectedTypes = { "I.radical" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestIMinusF2()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("F");
            IAtom atom2 = builder.NewAtom("I");
            IAtom atom3 = builder.NewAtom("F");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            atom2.FormalCharge = -1;
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);

            string[] expectedTypes = { "F", "I.minus.5", "F" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestHydride()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("H"); atom.FormalCharge = -1;
            mol.Atoms.Add(atom);

            string[] expectedTypes = new string[] { "H.minus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestHydrogenRadical()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("H");
            mol.Atoms.Add(atom);
            mol.AddSingleElectronTo(mol.Atoms[0]);

            string[] expectedTypes = new string[] { "H.radical" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestAzide()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("N"); atom2.FormalCharge = -1;
            IAtom atom3 = builder.NewAtom("N"); atom3.FormalCharge = +1;
            IAtom atom4 = builder.NewAtom("N");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Triple);

            string[] expectedTypes = new string[] { "C.sp3", "N.minus.sp3", "N.plus.sp1", "N.sp1" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestAllene()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom1 = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("C");
            IAtom atom3 = builder.NewAtom("C");
            mol.Atoms.Add(atom1);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Double);

            string[] expectedTypes = new string[] { "C.sp2", "C.allene", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestAzide2()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("N");
            IAtom atom3 = builder.NewAtom("N"); atom3.FormalCharge = +1;
            IAtom atom4 = builder.NewAtom("N"); atom4.FormalCharge = -1;
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Double);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Double);

            string[] expectedTypes = new string[] { "C.sp3", "N.sp2", "N.plus.sp1", "N.minus.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestMercuryComplex()
        {
            var mol = builder.NewAtomContainer();

            IAtom atom = builder.NewAtom("Hg"); atom.FormalCharge = -1;
            IAtom atom1 = builder.NewAtom("O"); atom1.FormalCharge = +1;
            IAtom atom2 = builder.NewAtom("C");
            IAtom atom3 = builder.NewAtom("C");
            IAtom atom4 = builder.NewAtom("N");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom1);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Double);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Double);
            mol.AddBond(mol.Atoms[4], mol.Atoms[0], BondOrder.Single);
            string[] expectedTypes = new string[] { "Hg.minus", "O.plus.sp2", "C.sp2", "C.sp2", "N.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Hg_2plus()
        {
            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Hg");
            a1.FormalCharge = 2;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Hg.2plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Hg_plus()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Hg");
            a1.FormalCharge = 1;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);

            string[] expectedTypes = { "Hg.plus", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Hg_metallic()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Hg");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Hg.metallic" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Hg_1()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Hg");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);

            string[] expectedTypes = { "Hg.1", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Hg_2()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Hg");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "Hg.2", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestPoloniumComplex()
        {
            var mol = builder.NewAtomContainer();

            IAtom atom = builder.NewAtom("O");
            IAtom atom1 = builder.NewAtom("Po");
            IAtom atom2 = builder.NewAtom("C");
            IAtom atom3 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom1);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            string[] expectedTypes = new string[] { "O.sp3", "Po", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestStronglyBoundKations()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("O"));
            mol.Atoms[1].FormalCharge = +1;
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            IAtom atom = builder.NewAtom("Na");
            mol.Atoms.Add(atom);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);

            string[] expectedTypes = new string[] { "C.sp2", "O.plus.sp2", "Na" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestMetallics()
        {
            var mol = builder.NewAtomContainer();

            IAtom atom = builder.NewAtom("W");
            mol.Atoms.Add(atom);
            string[] expectedTypes = new string[] { "W.metallic" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = builder.NewAtomContainer();
            atom = builder.NewAtom("K");
            mol.Atoms.Add(atom);
            expectedTypes = new string[] { "K.metallic" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = builder.NewAtomContainer();
            atom = builder.NewAtom("Co");
            mol.Atoms.Add(atom);
            expectedTypes = new string[] { "Co.metallic" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestSalts()
        {
            var mol = builder.NewAtomContainer();

            IAtom atom = builder.NewAtom("Na"); atom.FormalCharge = +1;
            mol.Atoms.Add(atom);
            string[] expectedTypes = new string[] { "Na.plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = builder.NewAtomContainer();
            atom = builder.NewAtom("K"); atom.FormalCharge = +1;
            mol.Atoms.Add(atom);
            expectedTypes = new string[] { "K.plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = builder.NewAtomContainer();
            atom = builder.NewAtom("Ca"); atom.FormalCharge = +2;
            mol.Atoms.Add(atom);
            expectedTypes = new string[] { "Ca.2plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = builder.NewAtomContainer();
            atom = builder.NewAtom("Mg"); atom.FormalCharge = +2;
            mol.Atoms.Add(atom);
            expectedTypes = new string[] { "Mg.2plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = builder.NewAtomContainer();
            atom = builder.NewAtom("Ni"); atom.FormalCharge = +2;
            mol.Atoms.Add(atom);
            expectedTypes = new string[] { "Ni.2plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = builder.NewAtomContainer();
            atom = builder.NewAtom("Pt"); atom.FormalCharge = +2;
            mol.Atoms.Add(atom);
            expectedTypes = new string[] { "Pt.2plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = builder.NewAtomContainer();
            atom = builder.NewAtom("Co"); atom.FormalCharge = +2;
            mol.Atoms.Add(atom);
            expectedTypes = new string[] { "Co.2plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = builder.NewAtomContainer();
            atom = builder.NewAtom("Co"); atom.FormalCharge = +3;
            mol.Atoms.Add(atom);
            expectedTypes = new string[] { "Co.3plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = builder.NewAtomContainer();
            atom = builder.NewAtom("Cu"); atom.FormalCharge = +2;
            mol.Atoms.Add(atom);
            expectedTypes = new string[] { "Cu.2plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            mol = builder.NewAtomContainer();
            atom = builder.NewAtom("Al"); atom.FormalCharge = +3;
            mol.Atoms.Add(atom);
            expectedTypes = new string[] { "Al.3plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Fix_Ca_2()
        {
            //string molName = "Ca_2";

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Ca");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "Ca.2", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Fix_Ca_1()
        {
            //string molName1 = "Ca_1";

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Ca");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);

            string[] expectedTypes1 = { "Ca.1", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes1, mol);
        }

        [TestMethod()]
        public void TestCyclopentadienyl()
        {
            IAtomContainer cp = builder.NewAtomContainer();
            cp.Atoms.Add(builder.NewAtom("C"));
            cp.Atoms[0].Hybridization = Hybridization.SP2;
            cp.Atoms[0].ImplicitHydrogenCount = 1;
            cp.Atoms.Add(builder.NewAtom("C"));
            cp.Atoms[1].Hybridization = Hybridization.SP2;
            cp.Atoms[1].ImplicitHydrogenCount = 1;
            cp.Atoms.Add(builder.NewAtom("C"));
            cp.Atoms[2].Hybridization = Hybridization.SP2;
            cp.Atoms[2].ImplicitHydrogenCount = 1;
            cp.Atoms.Add(builder.NewAtom("C"));
            cp.Atoms[3].Hybridization = Hybridization.SP2;
            cp.Atoms[3].ImplicitHydrogenCount = 1;
            cp.Atoms.Add(builder.NewAtom("C"));
            cp.Atoms[4].FormalCharge = -1;
            cp.Atoms[4].Hybridization = Hybridization.Planar3;
            cp.Atoms.Add(builder.NewAtom("H"));
            cp.AddBond(cp.Atoms[0], cp.Atoms[1], BondOrder.Double);
            cp.AddBond(cp.Atoms[1], cp.Atoms[2], BondOrder.Single);
            cp.AddBond(cp.Atoms[2], cp.Atoms[3], BondOrder.Double);
            cp.AddBond(cp.Atoms[3], cp.Atoms[4], BondOrder.Single);
            cp.AddBond(cp.Atoms[4], cp.Atoms[0], BondOrder.Single);
            cp.AddBond(cp.Atoms[4], cp.Atoms[5], BondOrder.Single);

            string[] expectedTypes = new string[] { "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.minus.planar", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, cp);
        }

        [TestMethod()]
        public void TestFerrocene()
        {
            IAtomContainer ferrocene = builder.NewAtomContainer();
            ferrocene.Atoms.Add(builder.NewAtom("C"));
            ferrocene.Atoms.Add(builder.NewAtom("C"));
            ferrocene.Atoms.Add(builder.NewAtom("C"));
            ferrocene.Atoms.Add(builder.NewAtom("C"));
            ferrocene.Atoms.Add(builder.NewAtom("C"));
            ferrocene.Atoms[4].FormalCharge = -1;
            ferrocene.Atoms.Add(builder.NewAtom("C"));
            ferrocene.Atoms.Add(builder.NewAtom("C"));
            ferrocene.Atoms.Add(builder.NewAtom("C"));
            ferrocene.Atoms.Add(builder.NewAtom("C"));
            ferrocene.Atoms.Add(builder.NewAtom("C"));
            ferrocene.Atoms[9].FormalCharge = -1;
            ferrocene.Atoms.Add(builder.NewAtom("Fe"));
            ferrocene.Atoms[10].FormalCharge = +2;
            ferrocene.AddBond(ferrocene.Atoms[0], ferrocene.Atoms[1], BondOrder.Double);
            ferrocene.AddBond(ferrocene.Atoms[1], ferrocene.Atoms[2], BondOrder.Single);
            ferrocene.AddBond(ferrocene.Atoms[2], ferrocene.Atoms[3], BondOrder.Double);
            ferrocene.AddBond(ferrocene.Atoms[3], ferrocene.Atoms[4], BondOrder.Single);
            ferrocene.AddBond(ferrocene.Atoms[4], ferrocene.Atoms[0], BondOrder.Single);
            ferrocene.AddBond(ferrocene.Atoms[5], ferrocene.Atoms[6], BondOrder.Double);
            ferrocene.AddBond(ferrocene.Atoms[6], ferrocene.Atoms[7], BondOrder.Single);
            ferrocene.AddBond(ferrocene.Atoms[7], ferrocene.Atoms[8], BondOrder.Double);
            ferrocene.AddBond(ferrocene.Atoms[8], ferrocene.Atoms[9], BondOrder.Single);
            ferrocene.AddBond(ferrocene.Atoms[9], ferrocene.Atoms[5], BondOrder.Single);

            string[] expectedTypes = new string[]{"C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.minus.planar", "C.sp2", "C.sp2",
                "C.sp2", "C.sp2", "C.minus.planar", "Fe.2plus"};
            AssertAtomTypes(testedAtomTypes, expectedTypes, ferrocene);
        }

        [TestMethod()]
        public void TestFuran()
        {
            IAtomContainer furan = builder.NewAtomContainer();
            furan.Atoms.Add(builder.NewAtom("C"));
            furan.Atoms.Add(builder.NewAtom("C"));
            furan.Atoms.Add(builder.NewAtom("C"));
            furan.Atoms.Add(builder.NewAtom("C"));
            furan.Atoms.Add(builder.NewAtom("O"));
            furan.AddBond(furan.Atoms[0], furan.Atoms[1], BondOrder.Double);
            furan.AddBond(furan.Atoms[1], furan.Atoms[2], BondOrder.Single);
            furan.AddBond(furan.Atoms[2], furan.Atoms[3], BondOrder.Double);
            furan.AddBond(furan.Atoms[3], furan.Atoms[4], BondOrder.Single);
            furan.AddBond(furan.Atoms[4], furan.Atoms[0], BondOrder.Single);
            string[] expectedTypes = new string[] { "C.sp2", "C.sp2", "C.sp2", "C.sp2", "O.planar3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, furan);
        }

        [TestMethod()]
        public void TestPerchlorate()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("O");
            IAtom atom2 = builder.NewAtom("Cl");
            IAtom atom3 = builder.NewAtom("O");
            IAtom atom4 = builder.NewAtom("O");
            IAtom atom5 = builder.NewAtom("O");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.Atoms.Add(atom5);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Double);

            string[] expectedTypes = new string[] { "O.sp3", "Cl.perchlorate", "O.sp2", "O.sp2", "O.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        /// <summary>
        /// Gallium tetrahydroxide.
        /// </summary>
        [TestMethod()]
        public void TestGallate()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("O"); atom.FormalCharge = -1;
            IAtom atom2 = builder.NewAtom("Ga"); atom2.FormalCharge = +3;
            IAtom atom3 = builder.NewAtom("O"); atom3.FormalCharge = -1;
            IAtom atom4 = builder.NewAtom("O"); atom4.FormalCharge = -1;
            IAtom atom5 = builder.NewAtom("O"); atom5.FormalCharge = -1;
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.Atoms.Add(atom5);

            string[] expectedTypes = new string[] { "O.minus", "Ga.3plus", "O.minus", "O.minus", "O.minus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        /// <summary>
        /// Gallium trihydroxide.
        /// </summary>
        [TestMethod()]
        public void TestGallateCovalent()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("O");
            IAtom atom2 = builder.NewAtom("Ga");
            IAtom atom3 = builder.NewAtom("O");
            IAtom atom4 = builder.NewAtom("O");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.AddBond(mol.Atoms[1], mol.Atoms[0], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);

            string[] expectedTypes = new string[] { "O.sp3", "Ga", "O.sp3", "O.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestPerchlorate_ChargedBonds()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("O");
            IAtom atom2 = builder.NewAtom("Cl");
            IAtom atom3 = builder.NewAtom("O");
            IAtom atom4 = builder.NewAtom("O");
            IAtom atom5 = builder.NewAtom("O");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            atom2.FormalCharge = +3;
            mol.Atoms.Add(atom3);
            atom3.FormalCharge = -1;
            mol.Atoms.Add(atom4);
            atom4.FormalCharge = -1;
            mol.Atoms.Add(atom5);
            atom5.FormalCharge = -1;
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = new string[] { "O.sp3", "Cl.perchlorate.charged", "O.minus", "O.minus", "O.minus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestChlorate()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("O");
            IAtom atom2 = builder.NewAtom("Cl");
            IAtom atom3 = builder.NewAtom("O");
            IAtom atom4 = builder.NewAtom("O");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Double);

            string[] expectedTypes = new string[] { "O.sp3", "Cl.chlorate", "O.sp2", "O.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestOxide()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("O"); atom.FormalCharge = -2;
            mol.Atoms.Add(atom);

            string[] expectedTypes = new string[] { "O.minus2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestAzulene()
        {
            IAtomContainer molecule = TestMoleculeFactory.MakeAzulene();
            string[] expectedTypes = new string[]{"C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2",
                "C.sp2", "C.sp2"};
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestIndole()
        {
            string[] expectedTypes = { "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "N.planar3" };
            IAtomContainer molecule = TestMoleculeFactory.MakeIndole();
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        /// <summary>
        /// Test for the structure in XLogPDescriptorTest.Testno937().
        /// </summary>
        [TestMethod()]
        public void Testno937()
        {
            string[] expectedTypes = { "C.sp2", "N.planar3", "C.sp2", "N.sp2", "C.sp2", "C.sp3" };
            IAtomContainer molecule = TestMoleculeFactory.MakePyrrole();
            molecule.Atoms[3].Symbol = "N";
            molecule.Atoms.Add(molecule.Builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[5], BondOrder.Single);
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestBenzene()
        {
            string[] expectedTypes = { "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2" };
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Add(builder.NewRing(6, "C"));
            foreach (var bond in molecule.Bonds)
            {
                bond.IsAromatic = true;
            }
            foreach (var atom in molecule.Atoms)
            {
                atom.ImplicitHydrogenCount = 1;
            }
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestBenzene_SingleOrDouble()
        {
            string[] expectedTypes = { "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2" };
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Add(builder.NewRing(6, "C"));
            foreach (var bond in molecule.Bonds)
            {
                bond.Order = BondOrder.Unset;
                bond.IsSingleOrDouble = true;
            }
            foreach (var atom in molecule.Atoms)
            {
                atom.ImplicitHydrogenCount = 1;
            }
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestPyrrole()
        {
            string[] expectedTypes = { "C.sp2", "N.planar3", "C.sp2", "C.sp2", "C.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.MakePyrrole();
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestPyrrole_SingleOrDouble()
        {
            string[] expectedTypes = { "C.sp2", "N.planar3", "C.sp2", "C.sp2", "C.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.MakePyrrole();
            foreach (var bond in molecule.Bonds)
            {
                bond.Order = BondOrder.Unset;
                bond.IsSingleOrDouble = true;
            }
            foreach (var atom in molecule.Atoms)
            {
                atom.ImplicitHydrogenCount = 1;
            }
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestPyrroleAnion()
        {
            string[] expectedTypes = { "C.sp2", "N.minus.planar3", "C.sp2", "C.sp2", "C.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.MakePyrroleAnion();
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestImidazole()
        {
            string[] expectedTypes = { "C.sp2", "N.planar3", "C.sp2", "N.sp2", "C.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.MakeImidazole();
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestPyrazole()
        {
            string[] expectedTypes = { "C.sp2", "N.planar3", "N.sp2", "C.sp2", "C.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.MakePyrazole();
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void Test124Triazole()
        {
            string[] expectedTypes = { "C.sp2", "N.planar3", "N.sp2", "C.sp2", "N.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.Make124Triazole();
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void Test123Triazole()
        {
            string[] expectedTypes = { "C.sp2", "N.planar3", "N.sp2", "N.sp2", "C.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.Make123Triazole();
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestTetrazole()
        {
            string[] expectedTypes = { "N.sp2", "N.planar3", "N.sp2", "N.sp2", "C.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.MakeTetrazole();
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestOxazole()
        {
            string[] expectedTypes = { "C.sp2", "O.planar3", "C.sp2", "N.sp2", "C.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.MakeOxazole();
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestIsoxazole()
        {
            string[] expectedTypes = { "C.sp2", "O.planar3", "N.sp2", "C.sp2", "C.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.MakeIsoxazole();
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        // testThiazole can be found below...

        [TestMethod()]
        public void TestIsothiazole()
        {
            string[] expectedTypes = { "C.sp2", "S.planar3", "N.sp2", "C.sp2", "C.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.MakeIsothiazole();
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestThiadiazole()
        {
            string[] expectedTypes = { "C.sp2", "S.planar3", "C.sp2", "N.sp2", "N.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.MakeThiadiazole();
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestOxadiazole()
        {
            string[] expectedTypes = { "C.sp2", "O.planar3", "C.sp2", "N.sp2", "N.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.MakeOxadiazole();
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestPyridine()
        {
            string[] expectedTypes = { "C.sp2", "N.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.MakePyridine();
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestPyridine_SingleOrDouble()
        {
            string[] expectedTypes = { "C.sp2", "N.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.MakePyridine();
            foreach (var bond in molecule.Bonds)
            {
                bond.Order = BondOrder.Unset;
                bond.IsSingleOrDouble = true;
            }
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestPyridineDirect()
        {
            string[] expectedTypes = { "N.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2" };
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("N"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Double);
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Double);
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[4], mol.Atoms[5], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[5], BondOrder.Double);
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.bug 1957958
        [TestMethod()]
        public void TestPyridineWithSP2()
        {
            string[] expectedTypes = { "N.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2" };
            var mol = builder.NewAtomContainer();
            IAtom a1 = mol.Builder.NewAtom("N");
            IAtom a2 = mol.Builder.NewAtom("C");
            IAtom a3 = mol.Builder.NewAtom("C");
            IAtom a4 = mol.Builder.NewAtom("C");
            IAtom a5 = mol.Builder.NewAtom("C");
            IAtom a6 = mol.Builder.NewAtom("C");

            a1.Hybridization = Hybridization.SP2;
            a2.Hybridization = Hybridization.SP2;
            a3.Hybridization = Hybridization.SP2;
            a4.Hybridization = Hybridization.SP2;
            a5.Hybridization = Hybridization.SP2;
            a6.Hybridization = Hybridization.SP2;

            mol.Atoms.Add(a1);
            mol.Atoms.Add(a2);
            mol.Atoms.Add(a3);
            mol.Atoms.Add(a4);
            mol.Atoms.Add(a5);
            mol.Atoms.Add(a6);

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[4], mol.Atoms[5], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[5], BondOrder.Single);
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.bug 1879589
        [TestMethod()]
        public void TestChargedSulphurSpecies()
        {
            string[] expectedTypes = { "C.sp2", "N.sp2", "C.sp2", "C.sp2", "S.plus", "C.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.MakePyridine();
            molecule.Atoms[4].Symbol = "S";
            molecule.Atoms[4].FormalCharge = +1;
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestPyridineOxide_Charged()
        {
            string[] expectedTypes = { "C.sp2", "N.plus.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "O.minus" };
            IAtomContainer molecule = TestMoleculeFactory.MakePyridineOxide();
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestPyridineOxide()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("C")); // 0
            mol.Atoms.Add(builder.NewAtom("N")); // 1
            mol.Atoms.Add(builder.NewAtom("C")); // 2
            mol.Atoms.Add(builder.NewAtom("C")); // 3
            mol.Atoms.Add(builder.NewAtom("C")); // 4
            mol.Atoms.Add(builder.NewAtom("C")); // 5
            mol.Atoms.Add(builder.NewAtom("O")); // 6

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double); // 1
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single); // 2
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Double); // 3
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single); // 4
            mol.AddBond(mol.Atoms[4], mol.Atoms[5], BondOrder.Double); // 5
            mol.AddBond(mol.Atoms[5], mol.Atoms[0], BondOrder.Single); // 6
            mol.AddBond(mol.Atoms[1], mol.Atoms[6], BondOrder.Double); // 7

            string[] expectedTypes = { "C.sp2", "N.sp2.3", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "O.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestPyridineOxide_SP2()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("C")); // 0
            mol.Atoms[0].Hybridization = Hybridization.SP2;
            mol.Atoms.Add(builder.NewAtom("N")); // 1
            mol.Atoms[1].Hybridization = Hybridization.SP2;
            mol.Atoms.Add(builder.NewAtom("C")); // 2
            mol.Atoms[2].Hybridization = Hybridization.SP2;
            mol.Atoms.Add(builder.NewAtom("C")); // 3
            mol.Atoms[3].Hybridization = Hybridization.SP2;
            mol.Atoms.Add(builder.NewAtom("C")); // 4
            mol.Atoms[4].Hybridization = Hybridization.SP2;
            mol.Atoms.Add(builder.NewAtom("C")); // 5
            mol.Atoms[5].Hybridization = Hybridization.SP2;
            mol.Atoms.Add(builder.NewAtom("O")); // 6
            mol.Atoms[6].Hybridization = Hybridization.SP2;

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single); // 1
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single); // 2
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single); // 3
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single); // 4
            mol.AddBond(mol.Atoms[4], mol.Atoms[5], BondOrder.Single); // 5
            mol.AddBond(mol.Atoms[5], mol.Atoms[0], BondOrder.Single); // 6
            mol.AddBond(mol.Atoms[1], mol.Atoms[6], BondOrder.Double); // 7

            string[] expectedTypes = { "C.sp2", "N.sp2.3", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "O.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestPyridineOxideCharged_SP2()
        {
            string[] expectedTypes = { "C.sp2", "N.plus.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "O.minus" };
            IAtomContainer molecule = TestMoleculeFactory.MakePyridineOxide();
            foreach (var bond in molecule.Bonds)
                bond.Order = BondOrder.Single;
            for (int i = 0; i < 6; i++)
            {
                molecule.Atoms[i].Hybridization = Hybridization.SP2;
            }
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestPyrimidine()
        {
            string[] expectedTypes = { "C.sp2", "N.sp2", "C.sp2", "N.sp2", "C.sp2", "C.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.MakePyrimidine();
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestPyridazine()
        {
            string[] expectedTypes = { "C.sp2", "N.sp2", "N.sp2", "C.sp2", "C.sp2", "C.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.MakePyridazine();
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestTriazine()
        {
            string[] expectedTypes = { "C.sp2", "N.sp2", "C.sp2", "N.sp2", "C.sp2", "N.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.MakeTriazine();
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        [TestMethod()]
        public void TestThiazole()
        {
            string[] expectedTypes = { "C.sp2", "N.sp2", "C.sp2", "S.planar3", "C.sp2" };
            IAtomContainer molecule = TestMoleculeFactory.MakeThiazole();
            AssertAtomTypes(testedAtomTypes, expectedTypes, molecule);
        }

        /// <summary>
        /// SDF version of the PubChem entry for the given InChI uses uncharged Ni.
        /// </summary>
        // @cdk.inchi InChI=1/C2H6S2.Ni/c3-1-2-4;/h3-4H,1-2H2;/q;+2/p-2/fC2H4S2.Ni/h3-4h;/q-2;m
        [TestMethod()]
        public void TestNiCovalentlyBound()
        {
            string[] expectedTypes = { "C.sp3", "C.sp3", "S.3", "Ni", "S.3" };
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("S"));
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("Ni"));
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("S"));
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[4], BondOrder.Single);
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestHaloniumsF()
        {
            var mol = builder.NewAtomContainer();

            IAtom carbon1 = builder.NewAtom("C");
            IAtom carbon2 = builder.NewAtom("C");

            IAtom atom = builder.NewAtom("F"); atom.FormalCharge = +1;
            mol.Atoms.Add(atom);
            mol.Atoms.Add(carbon1);
            mol.Atoms.Add(carbon2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);

            string[] expectedTypes = { "F.plus.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestHaloniumsCl()
        {
            var mol = builder.NewAtomContainer();

            IAtom carbon1 = builder.NewAtom("C");
            IAtom carbon2 = builder.NewAtom("C");

            IAtom atom = builder.NewAtom("Cl"); atom.FormalCharge = +1;
            mol.Atoms.Add(atom);
            mol.Atoms.Add(carbon1);
            mol.Atoms.Add(carbon2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);

            string[] expectedTypes = { "Cl.plus.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestHaloniumsBr()
        {
            var mol = builder.NewAtomContainer();

            IAtom carbon1 = builder.NewAtom("C");
            IAtom carbon2 = builder.NewAtom("C");

            IAtom atom = builder.NewAtom("Br"); atom.FormalCharge = +1;
            mol.Atoms.Add(atom);
            mol.Atoms.Add(carbon1);
            mol.Atoms.Add(carbon2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);

            string[] expectedTypes = { "Br.plus.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestHaloniumsI()
        {
            var mol = builder.NewAtomContainer();

            IAtom carbon1 = builder.NewAtom("C");
            IAtom carbon2 = builder.NewAtom("C");

            IAtom atom = builder.NewAtom("I"); atom.FormalCharge = +1;
            mol.Atoms.Add(atom);
            mol.Atoms.Add(carbon1);
            mol.Atoms.Add(carbon2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);

            string[] expectedTypes = { "I.plus.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestRearrangementCarbokation()
        {
            var mol = builder.NewAtomContainer();

            IAtom carbon1 = builder.NewAtom("C"); carbon1.FormalCharge = +1;
            IAtom carbon2 = builder.NewAtom("C");
            IAtom carbon3 = builder.NewAtom("C");

            mol.Atoms.Add(carbon1);
            mol.Atoms.Add(carbon2);
            mol.Atoms.Add(carbon3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);

            string[] expectedTypes = { "C.plus.sp2", "C.sp2", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestChargedSpecies()
        {
            var mol = builder.NewAtomContainer();

            IAtom atom1 = builder.NewAtom("C"); atom1.FormalCharge = -1;
            IAtom atom2 = builder.NewAtom("O"); atom2.FormalCharge = +1;

            mol.Atoms.Add(atom1);
            mol.Atoms.Add(atom2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Triple);

            string[] expectedTypes = { "C.minus.sp1", "O.plus.sp1" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        //    [O+]=C-[C-]
        [TestMethod()]
        public void TestChargedSpecies2()
        {
            var mol = builder.NewAtomContainer();

            IAtom atom1 = builder.NewAtom("O"); atom1.FormalCharge = +1;
            IAtom atom2 = builder.NewAtom("C");
            IAtom atom3 = builder.NewAtom("C"); atom3.FormalCharge = -1;

            mol.Atoms.Add(atom1);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);

            string[] expectedTypes = { "O.plus.sp2", "C.sp2", "C.minus.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        //    [C-]=C-C
        [TestMethod()]
        public void TestChargedSpecies3()
        {
            var mol = builder.NewAtomContainer();

            IAtom atom1 = builder.NewAtom("C"); atom1.FormalCharge = -1;
            IAtom atom2 = builder.NewAtom("C");
            IAtom atom3 = builder.NewAtom("C");

            mol.Atoms.Add(atom1);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);

            string[] expectedTypes = { "C.minus.sp2", "C.sp2", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // [C-]#[N+]C
        [TestMethod()]
        public void TestIsonitrile()
        {
            var mol = builder.NewAtomContainer();

            IAtom atom1 = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("N"); atom2.FormalCharge = +1;
            IAtom atom3 = builder.NewAtom("C"); atom3.FormalCharge = -1;

            mol.Atoms.Add(atom1);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Triple);

            string[] expectedTypes = { "C.sp3", "N.plus.sp1", "C.minus.sp1" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestNobleGases()
        {
            var mol = builder.NewAtomContainer();

            mol.Atoms.Add(builder.NewAtom("He"));
            mol.Atoms.Add(builder.NewAtom("Ne"));
            mol.Atoms.Add(builder.NewAtom("Ar"));
            mol.Atoms.Add(builder.NewAtom("Kr"));
            mol.Atoms.Add(builder.NewAtom("Xe"));
            mol.Atoms.Add(builder.NewAtom("Rn"));

            string[] expectedTypes = { "He", "Ne", "Ar", "Kr", "Xe", "Rn" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestZincChloride()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("Zn"));
            mol.Atoms.Add(builder.NewAtom("Cl"));
            mol.Atoms.Add(builder.NewAtom("Cl"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);

            string[] expectedTypes = { "Zn", "Cl", "Cl" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestZinc()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("Zn"));
            mol.Atoms[0].FormalCharge = +2;

            string[] expectedTypes = { "Zn.2plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestSilicon()
        {
            var mol = builder.NewAtomContainer();
            IAtom a1 = mol.Builder.NewAtom("Si");
            mol.Atoms.Add(a1);
            IAtom a2 = mol.Builder.NewAtom("O");
            mol.Atoms.Add(a2);
            IAtom a3 = mol.Builder.NewAtom("O");
            mol.Atoms.Add(a3);
            IAtom a4 = mol.Builder.NewAtom("O");
            mol.Atoms.Add(a4);
            IAtom a5 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(a5);
            IAtom a6 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(a6);
            IAtom a7 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(a7);
            IAtom a8 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a8);
            IAtom a9 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a9);
            IAtom a10 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a10);
            IAtom a11 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a11);
            IAtom a12 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a12);
            IAtom a13 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a13);
            IAtom a14 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a14);
            IAtom a15 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a15);
            IAtom a16 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a16);
            IAtom a17 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a17);
            IBond b1 = mol.Builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = mol.Builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = mol.Builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = mol.Builder.NewBond(a2, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = mol.Builder.NewBond(a3, a6, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = mol.Builder.NewBond(a4, a7, BondOrder.Single);
            mol.Bonds.Add(b6);
            IBond b7 = mol.Builder.NewBond(a5, a8, BondOrder.Single);
            mol.Bonds.Add(b7);
            IBond b8 = mol.Builder.NewBond(a5, a9, BondOrder.Single);
            mol.Bonds.Add(b8);
            IBond b9 = mol.Builder.NewBond(a5, a10, BondOrder.Single);
            mol.Bonds.Add(b9);
            IBond b10 = mol.Builder.NewBond(a6, a11, BondOrder.Single);
            mol.Bonds.Add(b10);
            IBond b11 = mol.Builder.NewBond(a6, a12, BondOrder.Single);
            mol.Bonds.Add(b11);
            IBond b12 = mol.Builder.NewBond(a6, a13, BondOrder.Single);
            mol.Bonds.Add(b12);
            IBond b13 = mol.Builder.NewBond(a7, a14, BondOrder.Single);
            mol.Bonds.Add(b13);
            IBond b14 = mol.Builder.NewBond(a7, a15, BondOrder.Single);
            mol.Bonds.Add(b14);
            IBond b15 = mol.Builder.NewBond(a7, a16, BondOrder.Single);
            mol.Bonds.Add(b15);
            IBond b16 = mol.Builder.NewBond(a1, a17, BondOrder.Single);
            mol.Bonds.Add(b16);

            string[] expectedTypes = {"Si.sp3", "O.sp3", "O.sp3", "O.sp3", "C.sp3", "C.sp3", "C.sp3", "H", "H", "H", "H",
                "H", "H", "H", "H", "H", "H"};
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestScandium()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("Sc"));
            mol.Atoms[0].FormalCharge = -3;
            mol.Atoms.Add(builder.NewAtom("O"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("O"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("O"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[5], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.AddBond(mol.Atoms[5], mol.Atoms[6], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("O"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[7], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.AddBond(mol.Atoms[7], mol.Atoms[8], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("O"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[9], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.AddBond(mol.Atoms[9], mol.Atoms[10], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("O"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[11], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.AddBond(mol.Atoms[11], mol.Atoms[12], BondOrder.Single);

            string[] expectedTypes = {"Sc.3minus", "O.sp3", "H", "O.sp3", "H", "O.sp3", "H", "O.sp3", "H", "O.sp3", "H",
                "O.sp3", "H"};
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestVanadium()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("V"));
            mol.Atoms[0].FormalCharge = -3;
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("N"));
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Triple);
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("N"));
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Triple);
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[5], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("N"));
            mol.AddBond(mol.Atoms[5], mol.Atoms[6], BondOrder.Triple);
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[7], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("N"));
            mol.AddBond(mol.Atoms[7], mol.Atoms[8], BondOrder.Triple);
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[9], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("N"));
            mol.AddBond(mol.Atoms[9], mol.Atoms[10], BondOrder.Triple);
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[11], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("N"));
            mol.AddBond(mol.Atoms[11], mol.Atoms[12], BondOrder.Triple);

            string[] expectedTypes = {"V.3minus", "C.sp", "N.sp1", "C.sp", "N.sp1", "C.sp", "N.sp1", "C.sp", "N.sp1",
                "C.sp", "N.sp1", "C.sp", "N.sp1"};
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestTitanium()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("Ti"));
            mol.Atoms[0].FormalCharge = -3;
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("N"));
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Triple);
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("N"));
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Triple);
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[5], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("N"));
            mol.AddBond(mol.Atoms[5], mol.Atoms[6], BondOrder.Triple);
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[7], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("N"));
            mol.AddBond(mol.Atoms[7], mol.Atoms[8], BondOrder.Triple);
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[9], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("N"));
            mol.AddBond(mol.Atoms[9], mol.Atoms[10], BondOrder.Triple);
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[11], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("N"));
            mol.AddBond(mol.Atoms[11], mol.Atoms[12], BondOrder.Triple);

            string[] expectedTypes = {"Ti.3minus", "C.sp", "N.sp1", "C.sp", "N.sp1", "C.sp", "N.sp1", "C.sp", "N.sp1",
                "C.sp", "N.sp1", "C.sp", "N.sp1"};
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestBoronTetraFluoride()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("B"));
            mol.Atoms[0].FormalCharge = -1;
            mol.Atoms.Add(builder.NewAtom("F"));
            mol.Atoms.Add(builder.NewAtom("F"));
            mol.Atoms.Add(builder.NewAtom("F"));
            mol.Atoms.Add(builder.NewAtom("F"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "B.minus", "F", "F", "F", "F" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestBerylliumTetraFluoride()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("Be"));
            mol.Atoms[0].FormalCharge = -2;
            mol.Atoms.Add(builder.NewAtom("F"));
            mol.Atoms.Add(builder.NewAtom("F"));
            mol.Atoms.Add(builder.NewAtom("F"));
            mol.Atoms.Add(builder.NewAtom("F"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "Be.2minus", "F", "F", "F", "F" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestArsine()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("As"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);

            string[] expectedTypes = { "As", "H", "H", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestBoron()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("B"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);

            string[] expectedTypes = { "B", "H", "H", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestCarbonMonoxide()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms[0].FormalCharge = -1;
            mol.Atoms.Add(builder.NewAtom("O"));
            mol.Atoms[1].FormalCharge = 1;
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Triple);

            string[] expectedTypes = { "C.minus.sp1", "O.plus.sp1" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestTitaniumFourCoordinate()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("Ti"));
            mol.Atoms.Add(builder.NewAtom("Cl"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("Cl"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("Cl"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("Cl"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "Ti.sp3", "Cl", "Cl", "Cl", "Cl" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.bug 1872969
        [TestMethod()]
        public void Bug1872969()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("S"));
            mol.Atoms.Add(builder.NewAtom("O"));
            mol.Atoms.Add(builder.NewAtom("O"));
            mol.Atoms.Add(builder.NewAtom("O"));
            mol.Atoms[4].FormalCharge = -1;
            mol.Atoms.Add(builder.NewAtom("Na"));
            mol.Atoms[5].FormalCharge = +1;
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Double);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "C.sp3", "S.onyl", "O.sp2", "O.sp2", "O.minus", "Na.plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        /// <summary>
        /// Test if all elements up to and including Uranium have atom types.
        /// </summary>
        [TestMethod()]
        public void TestAllElementsRepresented()
        {
            var factory = AtomTypeFactory.GetInstance("NCDK.Dict.Data.cdk-atom-types.owl");
            Assert.IsTrue(factory.Count != 0, "Could not read the atom types");
            var errorMessage = "Elements without atom type(s) defined in the XML:";
            int testUptoAtomicNumber = 36; // TODO: 92 ?
            int elementsMissingTypes = 0;
            for (int i = 1; i < testUptoAtomicNumber; i++)
            {
                var symbol = PeriodicTable.GetSymbol(i);
                var expectedTypes = factory.GetAtomTypes(symbol);
                if (expectedTypes.Count() == 0)
                {
                    errorMessage += " " + symbol;
                    elementsMissingTypes++;
                }
            }
            Assert.AreEqual(0, elementsMissingTypes, errorMessage);
        }

        [TestMethod()]
        public void TestAssumeExplicitHydrogens()
        {
            var mol = builder.NewAtomContainer();
            var atm = CDKAtomTypeMatcher.GetInstance(CDKAtomTypeMatcher.Mode.RequireExplicitHydrogens);

            mol.Atoms.Add(builder.NewAtom("O"));
            mol.Atoms[0].FormalCharge = +1;
            var type = atm.FindMatchingAtomType(mol, mol.Atoms[0]);
            Assert.IsNotNull(type);
            Assert.AreEqual("X", type.AtomTypeName);

            for (int i = 0; i < 3; i++)
            {
                mol.Atoms.Add(builder.NewAtom("H"));
                mol.Bonds.Add(builder.NewBond(mol.Atoms[i + 1], mol.Atoms[0], BondOrder.Single));
            }
            AssertAtomType(testedAtomTypes, "O.plus", atm.FindMatchingAtomType(mol, mol.Atoms[0]));
        }

        [TestMethod()]
        public void TestStructGenMatcher()
        {
            var matcher = CDK.AtomTypeMatcher;
            Assert.IsNotNull(matcher);
        }

        [TestMethod()]
        public void TestCarbonRadical()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("C");
            IAtom atom3 = builder.NewAtom("C");
            IAtom atom4 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddSingleElectronTo(mol.Atoms[1]);
            mol.Atoms.Add(atom3);
            mol.Atoms.Add(atom4);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);

            string[] expectedTypes = { "C.sp3", "C.radical.planar", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.bug 1382 
        [TestMethod()]
        public void TestCarbonDiradical()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            mol.AddSingleElectronTo(mol.Atoms[0]);
            mol.AddSingleElectronTo(mol.Atoms[0]);

            IAtomTypeMatcher atm = GetAtomTypeMatcher(mol.Builder);
            IAtomType foundType = atm.FindMatchingAtomType(mol, atom);
            Assert.AreEqual("X", foundType.AtomTypeName);
        }

        [TestMethod()]
        public void TestEthoxyEthaneRadical()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("O");
            IAtom atom2 = builder.NewAtom("C");
            IAtom atom3 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            atom.FormalCharge = +1;
            mol.AddSingleElectronTo(mol.Atoms[0]);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[1], BondOrder.Single);

            string[] expectedTypes = { "O.plus.radical", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestMethylFluorRadical()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("F");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            atom2.FormalCharge = +1;
            mol.AddSingleElectronTo(mol.Atoms[1]);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);

            string[] expectedTypes = { "C.sp3", "F.plus.radical" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestMethylChloroRadical()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("Cl");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            atom2.FormalCharge = +1;
            mol.AddSingleElectronTo(mol.Atoms[1]);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);

            string[] expectedTypes = { "C.sp3", "Cl.plus.radical" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestMethylBromoRadical()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("Br");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            atom2.FormalCharge = +1;
            mol.AddSingleElectronTo(mol.Atoms[1]);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);

            string[] expectedTypes = { "C.sp3", "Br.plus.radical" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestMethylIodoRadical()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("I");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            atom2.FormalCharge = +1;
            mol.AddSingleElectronTo(mol.Atoms[1]);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);

            string[] expectedTypes = { "C.sp3", "I.plus.radical" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestMethyleneFluorKation()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("F");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            atom2.FormalCharge = +1;
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);

            string[] expectedTypes = { "C.sp2", "F.plus.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestMethyleneChlorKation()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("Cl");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            atom2.FormalCharge = +1;
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);

            string[] expectedTypes = { "C.sp2", "Cl.plus.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestMethyleneBromKation()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("Br");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            atom2.FormalCharge = +1;
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);

            string[] expectedTypes = { "C.sp2", "Br.plus.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestMethyleneIodKation()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("I");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            atom2.FormalCharge = +1;
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);

            string[] expectedTypes = { "C.sp2", "I.plus.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestMethanolRadical()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("C");
            IAtom atom2 = builder.NewAtom("O");
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.AddSingleElectronTo(mol.Atoms[1]);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);

            string[] expectedTypes = { "C.sp3", "O.sp3.radical" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestMethylMethylimineRadical()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("N");
            IAtom atom2 = builder.NewAtom("C");
            IAtom atom3 = builder.NewAtom("C");
            mol.Atoms.Add(atom);
            atom.FormalCharge = +1;
            mol.AddSingleElectronTo(mol.Atoms[0]);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Double);
            mol.AddBond(mol.Atoms[2], mol.Atoms[1], BondOrder.Single);

            string[] expectedTypes = { "N.plus.sp2.radical", "C.sp2", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestChargeSeparatedFluoroEthane()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("F");
            IAtom atom2 = builder.NewAtom("C"); atom2.FormalCharge = +1;
            IAtom atom3 = builder.NewAtom("C"); atom3.FormalCharge = -1;
            mol.Atoms.Add(atom);
            mol.Atoms.Add(atom2);
            mol.Atoms.Add(atom3);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[1], BondOrder.Single);

            string[] expectedTypes = { "F", "C.plus.planar", "C.minus.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.inchi InChI=1/C2H7NS/c1-4(2)3/h3H,1-2H3
        [TestMethod()]
        public void TestSulphurCompound()
        {
            var mol = builder.NewAtomContainer();
            IAtom a1 = mol.Builder.NewAtom("S");
            mol.Atoms.Add(a1);
            IAtom a2 = mol.Builder.NewAtom("N");
            mol.Atoms.Add(a2);
            IAtom a3 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(a3);
            IAtom a4 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(a4);
            IBond b1 = mol.Builder.NewBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);
            IBond b2 = mol.Builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = mol.Builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);

            string[] expectedTypes = { "S.inyl", "N.sp2", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestAluminumChloride()
        {
            var mol = builder.NewAtomContainer();
            IAtom a1 = mol.Builder.NewAtom("Cl");
            mol.Atoms.Add(a1);
            IAtom a2 = mol.Builder.NewAtom("Cl");
            mol.Atoms.Add(a2);
            IAtom a3 = mol.Builder.NewAtom("Cl");
            mol.Atoms.Add(a3);
            IAtom a4 = mol.Builder.NewAtom("Al");
            mol.Atoms.Add(a4);
            IBond b1 = mol.Builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = mol.Builder.NewBond(a2, a4, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = mol.Builder.NewBond(a3, a4, BondOrder.Single);
            mol.Bonds.Add(b3);

            string[] expectedTypes = { "Cl", "Cl", "Cl", "Al" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.inchi InChI=1/C3H9NO/c1-4(2,3)5/h1-3H3
        [TestMethod()]
        public void Cid1145()
        {
            var mol = builder.NewAtomContainer();
            IAtom a1 = mol.Builder.NewAtom("O");
            mol.Atoms.Add(a1);
            a1.FormalCharge = -1;
            IAtom a2 = mol.Builder.NewAtom("N");
            mol.Atoms.Add(a2);
            a2.FormalCharge = +1;
            IAtom a3 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(a3);
            IAtom a4 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(a4);
            IAtom a5 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(a5);
            IAtom a6 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a6);
            IAtom a7 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a7);
            IAtom a8 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a8);
            IAtom a9 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a9);
            IAtom a10 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a10);
            IAtom a11 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a11);
            IAtom a12 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a12);
            IAtom a13 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a13);
            IAtom a14 = mol.Builder.NewAtom("H");
            mol.Atoms.Add(a14);
            IBond b1 = mol.Builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = mol.Builder.NewBond(a2, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = mol.Builder.NewBond(a2, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = mol.Builder.NewBond(a2, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = mol.Builder.NewBond(a3, a6, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = mol.Builder.NewBond(a3, a7, BondOrder.Single);
            mol.Bonds.Add(b6);
            IBond b7 = mol.Builder.NewBond(a3, a8, BondOrder.Single);
            mol.Bonds.Add(b7);
            IBond b8 = mol.Builder.NewBond(a4, a9, BondOrder.Single);
            mol.Bonds.Add(b8);
            IBond b9 = mol.Builder.NewBond(a4, a10, BondOrder.Single);
            mol.Bonds.Add(b9);
            IBond b10 = mol.Builder.NewBond(a4, a11, BondOrder.Single);
            mol.Bonds.Add(b10);
            IBond b11 = mol.Builder.NewBond(a5, a12, BondOrder.Single);
            mol.Bonds.Add(b11);
            IBond b12 = mol.Builder.NewBond(a5, a13, BondOrder.Single);
            mol.Bonds.Add(b12);
            IBond b13 = mol.Builder.NewBond(a5, a14, BondOrder.Single);
            mol.Bonds.Add(b13);

            string[] expectedTypes = {"O.minus", "N.plus", "C.sp3", "C.sp3", "C.sp3", "H", "H", "H", "H", "H", "H", "H",
                "H", "H"};
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestChiPathFail()
        {
            var mol = builder.NewAtomContainer();
            IAtom a1 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(a1);
            IAtom a2 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(a2);
            IAtom a3 = mol.Builder.NewAtom("C");
            mol.Atoms.Add(a3);
            IAtom a4 = mol.Builder.NewAtom("O");
            mol.Atoms.Add(a4);
            IBond b1 = mol.Builder.NewBond(a2, a1, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = mol.Builder.NewBond(a3, a2, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = mol.Builder.NewBond(a4, a3, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = mol.Builder.NewBond(a4, a2, BondOrder.Single);
            mol.Bonds.Add(b4);

            string[] expectedTypes = { "C.sp3", "C.sp3", "C.sp3", "O.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.inchi InChI=1S/C6H5IO/c8-7-6-4-2-1-3-5-6/h1-5H
        [TestMethod()]
        public void TestIodosobenzene()
        {
            var mol = TestMoleculeFactory.MakeBenzene();
            IAtom iodine = mol.Builder.NewAtom("I");
            IAtom oxygen = mol.Builder.NewAtom("O");
            mol.Atoms.Add(iodine);
            mol.Atoms.Add(oxygen);
            mol.AddBond(mol.Atoms[0], mol.Atoms[6], BondOrder.Single);
            mol.AddBond(mol.Atoms[6], mol.Atoms[7], BondOrder.Double);

            string[] expectedTypes = { "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "I.3", "O.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.inchi InChI=1S/C6H5IO2/c8-7(9)6-4-2-1-3-5-6/h1-5H
        [TestMethod()]
        public void TestIodoxybenzene()
        {
            var mol = TestMoleculeFactory.MakeBenzene();
            IAtom iodine = mol.Builder.NewAtom("I");
            IAtom oxygen1 = mol.Builder.NewAtom("O");
            IAtom oxygen2 = mol.Builder.NewAtom("O");
            mol.Atoms.Add(iodine);
            mol.Atoms.Add(oxygen1);
            mol.Atoms.Add(oxygen2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[6], BondOrder.Single);
            mol.AddBond(mol.Atoms[6], mol.Atoms[7], BondOrder.Double);
            mol.AddBond(mol.Atoms[6], mol.Atoms[8], BondOrder.Double);

            string[] expectedTypes = { "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "I.5", "O.sp2", "O.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.inchi InChI=1S/C7H7NOS/c8-7(10-9)6-4-2-1-3-5-6/h1-5H,8H2
        [TestMethod()]
        public void TestThiobenzamideSOxide()
        {
            var mol = TestMoleculeFactory.MakeBenzene();
            IAtom carbon = mol.Builder.NewAtom("C");
            IAtom sulphur = mol.Builder.NewAtom("S");
            IAtom oxygen = mol.Builder.NewAtom("O");
            IAtom nitrogen = mol.Builder.NewAtom("N");
            mol.Atoms.Add(carbon);
            mol.Atoms.Add(sulphur);
            mol.Atoms.Add(oxygen);
            mol.Atoms.Add(nitrogen);
            mol.AddBond(mol.Atoms[0], mol.Atoms[6], BondOrder.Single);
            mol.AddBond(mol.Atoms[6], mol.Atoms[7], BondOrder.Double);
            mol.AddBond(mol.Atoms[7], mol.Atoms[8], BondOrder.Double);
            mol.AddBond(mol.Atoms[6], mol.Atoms[9], BondOrder.Single);

            string[] expectedTypes = {"C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "C.sp2", "S.inyl.2", "O.sp2",
                "N.thioamide"};
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.inchi InChI=1S/C4H10S/c1-5(2)3-4-5/h3-4H2,1-2H3
        [TestMethod()]
        public void TestDimethylThiirane()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(mol.Builder.NewAtom("C"));
            mol.Atoms.Add(mol.Builder.NewAtom("C"));
            mol.Atoms.Add(mol.Builder.NewAtom("C"));
            mol.Atoms.Add(mol.Builder.NewAtom("C"));
            mol.Atoms.Add(mol.Builder.NewAtom("S"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[4], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[4], mol.Atoms[3], BondOrder.Single);

            string[] expectedTypes = { "C.sp3", "C.sp3", "C.sp3", "C.sp3", "S.anyl" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.inchi     InChI=1/C3H8S/c1-4(2)3/h1H2,2-3H3
        [TestMethod()]
        public void TestSulphonylLookalike()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(mol.Builder.NewAtom("C"));
            mol.Atoms.Add(mol.Builder.NewAtom("C"));
            mol.Atoms.Add(mol.Builder.NewAtom("C"));
            mol.Atoms.Add(mol.Builder.NewAtom("S"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Double);

            string[] expectedTypes = { "C.sp3", "C.sp3", "C.sp2", "S.inyl" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestNOxide()
        {
            var mol = builder.NewAtomContainer();
            IAtom a1 = mol.Builder.NewAtom("C");
            IAtom a2 = mol.Builder.NewAtom("C");
            IAtom a3 = mol.Builder.NewAtom("N");
            IAtom a4 = mol.Builder.NewAtom("O");
            IAtom a5 = mol.Builder.NewAtom("O");

            mol.Atoms.Add(a1);
            mol.Atoms.Add(a2);
            mol.Atoms.Add(a3);
            mol.Atoms.Add(a4);
            mol.Atoms.Add(a5);

            mol.Bonds.Add(mol.Builder.NewBond(a1, a2, BondOrder.Single));
            mol.Bonds.Add(mol.Builder.NewBond(a2, a3, BondOrder.Single));
            mol.Bonds.Add(mol.Builder.NewBond(a3, a4, BondOrder.Double));
            mol.Bonds.Add(mol.Builder.NewBond(a3, a5, BondOrder.Double));

            string[] expectedTypes = { "C.sp3", "C.sp3", "N.nitro", "O.sp2", "O.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestGermaniumFourCoordinate()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("Ge"));
            mol.Atoms.Add(builder.NewAtom("Cl"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("Cl"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("Cl"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("Cl"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "Ge", "Cl", "Cl", "Cl", "Cl" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestPlatinumFourCoordinate()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("Pt"));
            mol.Atoms.Add(builder.NewAtom("Cl"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("Cl"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("Cl"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("Cl"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[4], BondOrder.Single);

            string[] expectedTypes = { "Pt.4", "Cl", "Cl", "Cl", "Cl" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestPlatinumSixCoordinate()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("Pt"));
            mol.Atoms.Add(builder.NewAtom("Cl"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("Cl"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[2], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("Cl"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[3], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("Cl"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[4], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("O"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[5], BondOrder.Single);
            mol.Atoms.Add(builder.NewAtom("O"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[6], BondOrder.Single);

            string[] expectedTypes = { "Pt.6", "Cl", "Cl", "Cl", "Cl", "O.sp3", "O.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.bug 2424511
        [TestMethod()]
        public void TestWeirdNitrogen()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("N"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("C"));

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Triple);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Double);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);

            string[] expectedTypes = { "C.sp", "N.sp1.2", "C.sp2", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        /// <summary>
        /// Testing a nitrogen as found in this SMILES input: c1c2cc[nH]cc2nc1.
        /// </summary>
        [TestMethod()]
        public void TestAnotherNitrogen()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms[0].Hybridization = Hybridization.SP2;
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms[1].Hybridization = Hybridization.SP2;
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms[2].Hybridization = Hybridization.SP2;
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms[3].Hybridization = Hybridization.SP2;
            mol.Atoms.Add(builder.NewAtom("N"));
            mol.Atoms[4].Hybridization = Hybridization.Planar3;
            mol.Atoms[4].ImplicitHydrogenCount = 1;
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms[5].Hybridization = Hybridization.SP2;
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms[6].Hybridization = Hybridization.SP2;
            mol.Atoms.Add(builder.NewAtom("N"));
            mol.Atoms[7].Hybridization = Hybridization.SP2;
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms[8].Hybridization = Hybridization.SP2;

            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[0], mol.Atoms[8], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[6], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[3], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[4], mol.Atoms[5], BondOrder.Single);
            mol.AddBond(mol.Atoms[5], mol.Atoms[6], BondOrder.Single);
            mol.AddBond(mol.Atoms[6], mol.Atoms[7], BondOrder.Single);
            mol.AddBond(mol.Atoms[7], mol.Atoms[8], BondOrder.Single);

            string[] expectedTypes = { "C.sp2", "C.sp2", "C.sp2", "C.sp2", "N.planar3", "C.sp2", "C.sp2", "N.sp2", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.bug 3061263
        [TestMethod()]
        public void TestFormalChargeRepresentation()
        {
            var mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom("O");
            Hybridization thisHybridization = Hybridization.SP3;
            atom.Hybridization = thisHybridization;
            mol.Atoms.Add(atom);
            string[] expectedTypes = { "O.minus" };

            // option one: int.Parse(, NumberFormatInfo.InvariantInfo)
            atom.FormalCharge = -1;
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            // option one: autoboxing
            atom.FormalCharge = -1;
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);

            // option one: new Integer()
            atom.FormalCharge = -1;
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.bug 3190151
        [TestMethod()]
        public void TestP()
        {
            IAtom atomP = builder.NewAtom("P");
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(atomP);
            string[] expectedTypes = { "P.ine" };

            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.bug 3190151
        [TestMethod()]
        public void TestPine()
        {
            IAtom atomP = builder.NewAtom(ChemicalElement.P);
            IAtomType atomTypeP = builder.NewAtomType(ChemicalElement.P);
            AtomTypeManipulator.Configure(atomP, atomTypeP);

            IAtomContainer ac = atomP.Builder.NewAtomContainer();
            ac.Atoms.Add(atomP);
            IAtomType type = null;
            foreach (var atom in ac.Atoms)
            {
                type = CDK.AtomTypeMatcher.FindMatchingAtomType(ac, atom);
                Assert.IsNotNull(type);
            }
        }

        [TestMethod()]
        public void Test_S_sp3d1()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("S");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.NewAtom("C");
            a6.FormalCharge = 0;
            mol.Atoms.Add(a6);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a4, BondOrder.Double);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.NewBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b5);

            string[] expectedTypes = { "S.sp3d1", "C.sp3", "C.sp3", "C.sp2", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_S_inyl_2()
        {
            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("S");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Double);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "S.inyl.2", "C.sp2", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_S_2minus()
        {
            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("S");
            a1.FormalCharge = -2;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "S.2minus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_S_sp3()
        {
            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("S");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "S.3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_S_sp3_4()
        {
            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("S");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Double);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);

            string[] expectedTypes = { "S.sp3.4", "C.sp2", "C.sp2", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Co_3plus()
        {
            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Co");
            a1.FormalCharge = 3;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Co.3plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Co_metallic()
        {
            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Co");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Co.metallic" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Co_plus_6()
        {
            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Co");
            a1.FormalCharge = 1;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.NewAtom("C");
            a6.FormalCharge = 0;
            mol.Atoms.Add(a6);
            IAtom a7 = builder.NewAtom("C");
            a7.FormalCharge = 0;
            mol.Atoms.Add(a7);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.NewBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = builder.NewBond(a1, a7, BondOrder.Single);
            mol.Bonds.Add(b6);

            string[] expectedTypes = { "Co.plus.6", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Co_2plus()
        {
            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Co");
            a1.FormalCharge = 2;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Co.2plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Co_plus_2()
        {
            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Co");
            a1.FormalCharge = 1;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "Co.plus.2", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Co_2()
        {
            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("C");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("Co");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IBond b1 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a2, a3, BondOrder.Single);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "C.sp3", "C.sp3", "Co.2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Co_6()
        {
            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Co");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.NewAtom("C");
            a6.FormalCharge = 0;
            mol.Atoms.Add(a6);
            IAtom a7 = builder.NewAtom("C");
            a7.FormalCharge = 0;
            mol.Atoms.Add(a7);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.NewBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = builder.NewBond(a1, a7, BondOrder.Single);
            mol.Bonds.Add(b6);

            string[] expectedTypes = { "Co.6", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Co_plus_4()
        {
            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Co");
            a1.FormalCharge = 1;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);

            string[] expectedTypes = { "Co.plus.4", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Co_4()
        {
            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Co");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);

            string[] expectedTypes = { "Co.4", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Co_plus_5()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Co");
            a1.FormalCharge = 1;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.NewAtom("C");
            a6.FormalCharge = 0;
            mol.Atoms.Add(a6);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.NewBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b5);

            string[] expectedTypes = { "Co.plus.5", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.bug 3529082
        [TestMethod()]
        public void Test_Co_plus()
        {

            var mol = builder.NewAtomContainer();
            IAtom a2 = builder.NewAtom("Co");
            a2.FormalCharge = 1;
            mol.Atoms.Add(a2);

            string[] expectedTypes = { "Co.plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Co_plus_1()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("C");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("Co");
            a2.FormalCharge = 1;
            mol.Atoms.Add(a2);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);

            string[] expectedTypes = { "C.sp3", "Co.plus.1" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Co_1()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Co");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);

            string[] expectedTypes = { "Co.1", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        /// <summary>
        /// Bromic acid (CHEBI:49382).
        /// </summary>
        // @cdk.inchi InChI=1S/BrHO3/c2-1(3)4/h(H,2,3,4)
        [TestMethod()]
        public void Test_Br_3()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Br");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("O");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("O");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("O");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Double);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);

            string[] expectedTypes = { "Br.3", "O.sp2", "O.sp2", "O.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Zn_metallic()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Zn");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Zn.metallic" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Zn_1()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Zn");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);

            string[] expectedTypes = { "Zn.1", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        /// <summary>
        /// Vanadate. PDB HET ID : VO4.
        /// </summary>
        // @cdk.inchi InChI=1S/4O.V/q;3*-1;
        [TestMethod()]
        public void Test_V_3minus_4()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("V");
            a1.FormalCharge = -3;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("O");
            a2.FormalCharge = -1;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("O");
            a3.FormalCharge = -1;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("O");
            a4.FormalCharge = -1;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("O");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IBond b1 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a1, a5, BondOrder.Double);
            mol.Bonds.Add(b4);

            string[] expectedTypes = { "V.3minus.4", "O.minus", "O.minus", "O.minus", "O.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        /// <summary>
        /// Hexafluoroaluminate
        /// </summary>
        // @cdk.inchi InChI=1S/Al.6FH.3Na/h;6*1H;;;/q+3;;;;;;;3*+1/p-6
        [TestMethod()]
        public void Test_Al_3minus()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Al");
            a1.FormalCharge = -3;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.NewAtom("C");
            a6.FormalCharge = 0;
            mol.Atoms.Add(a6);
            IAtom a7 = builder.NewAtom("C");
            a7.FormalCharge = 0;
            mol.Atoms.Add(a7);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.NewBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = builder.NewBond(a1, a7, BondOrder.Single);
            mol.Bonds.Add(b6);

            string[] expectedTypes = { "Al.3minus", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestSe_sp3d1_4()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Se");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);

            string[] expectedTypes1 = { "Se.sp3d1.4", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes1, mol);
        }

        [TestMethod()]
        public void TestSe_sp3_4()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Se");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a4, BondOrder.Double);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a1, a5, BondOrder.Double);
            mol.Bonds.Add(b4);

            string[] expectedTypes = { "Se.sp3.4", "C.sp3", "C.sp3", "C.sp2", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestSe_sp2_2()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Se");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Double);
            mol.Bonds.Add(b2);

            string[] expectedTypes2 = { "Se.sp2.2", "C.sp2", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes2, mol);
        }

        [TestMethod()]
        public void TestSe_1()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("C");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("Se");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);

            string[] expectedTypes3 = { "C.sp2", "Se.1" };
            AssertAtomTypes(testedAtomTypes, expectedTypes3, mol);
        }

        [TestMethod()]
        public void TestSe_3()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Se");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);

            string[] expectedTypes4 = { "Se.3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes4, mol);
        }

        [TestMethod()]
        public void TestSe_sp3_3()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("C");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("Se");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a2, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a2, a4, BondOrder.Double);
            mol.Bonds.Add(b3);

            string[] expectedTypes5 = { "C.sp3", "Se.sp3.3", "C.sp3", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes5, mol);
        }

        [TestMethod()]
        public void TestSe_4plus()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Se");
            a1.FormalCharge = 4;
            mol.Atoms.Add(a1);

            string[] expectedTypes6 = { "Se.4plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes6, mol);
        }

        [TestMethod()]
        public void TestSe_plus_3()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("C");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("Se");
            a2.FormalCharge = 1;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a2, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a2, a4, BondOrder.Single);
            mol.Bonds.Add(b3);

            string[] expectedTypes7 = { "C.sp3", "Se.plus.3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes7, mol);
        }

        [TestMethod()]
        public void TestSe_5()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Se");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.NewAtom("C");
            a6.FormalCharge = 0;
            mol.Atoms.Add(a6);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.NewBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b5);

            string[] expectedTypes8 = { "Se.5", "C.sp2", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes8, mol);
        }

        [TestMethod()]
        public void Test_Se_2()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Se");
            a1.ImplicitHydrogenCount = 0;
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Se.2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.inchi InChI=1S/H2Te/h1H2
        [TestMethod()]
        public void TestTellane()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Te");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("H");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("H");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "Te.3", "H", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.inchi InChI=1S/C3H6P/c1-3-4-2/h3H,2H2,1H3/q+1
        [TestMethod()]
        public void TestPhosphanium()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("P");
            a1.FormalCharge = 1;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IBond b1 = builder.NewBond(a1, a3, BondOrder.Double);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a4, BondOrder.Double);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a2, a3, BondOrder.Single);
            mol.Bonds.Add(b3);

            string[] expectedTypes = { "P.sp1.plus", "C.sp3", "C.sp2", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.inchi InChI=1S/CHP/c1-2/h1H
        [TestMethod()]
        public void TestPhosphide()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("P");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("H");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Triple);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a2, a3, BondOrder.Single);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "P.ide", "C.sp", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void TestPentaMethylPhosphane()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("P");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.NewAtom("C");
            a6.FormalCharge = 0;
            mol.Atoms.Add(a6);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.NewBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b5);

            string[] expectedTypes = { "P.ane", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Sb_4()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("C");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("Sb");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a2, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a2, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a2, a5, BondOrder.Double);
            mol.Bonds.Add(b4);

            string[] expectedTypes = { "C.sp3", "Sb.4", "C.sp3", "C.sp3", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Sb_3()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Sb");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);

            string[] expectedTypes = { "Sb.3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_B_3plus()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("B");
            a1.FormalCharge = 3;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);

            string[] expectedTypes = { "B.3plus", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Sr_2plus()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Sr");
            a1.FormalCharge = 2;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Sr.2plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Te_4plus()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Te");
            a1.FormalCharge = 4;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Te.4plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Be_neutral()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Be");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Be.neutral" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Cl_2()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Cl");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Double);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "Cl.2", "C.sp3", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_K_neutral()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("K");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);

            string[] expectedTypes = { "K.neutral", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Li_neutral()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Li");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Li.neutral" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Li_plus()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Li");
            a1.FormalCharge = 1;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Li.plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_I_sp3d2_3()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("I");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);

            string[] expectedTypes = { "I.sp3d2.3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public override void TestForDuplicateDefinitions()
        {
            base.TestForDuplicateDefinitions();
        }

        // @cdk.inchi InChI=1S/CH2N2/c1-3-2/h1H2
        [TestMethod()]
        public void TestAzoCompound()
        {
            var mol = builder.NewAtomContainer();
            IAtom a1 = mol.Builder.NewAtom("N");
            a1.FormalCharge = 1;
            mol.Atoms.Add(a1);
            IAtom a2 = mol.Builder.NewAtom("N");
            a2.FormalCharge = -1;
            mol.Atoms.Add(a2);
            IAtom a3 = mol.Builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = mol.Builder.NewAtom("H");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = mol.Builder.NewAtom("H");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IBond b1 = mol.Builder.NewBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);
            IBond b2 = mol.Builder.NewBond(a1, a3, BondOrder.Double);
            mol.Bonds.Add(b2);
            IBond b3 = mol.Builder.NewBond(a3, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = mol.Builder.NewBond(a3, a5, BondOrder.Single);
            mol.Bonds.Add(b4);

            string[] expectedTypes = { "N.plus.sp1", "N.minus.sp2", "C.sp2", "H", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.bug   3141611
        // @cdk.inchi InChI=1S/CH5O2P/c1-4(2)3/h4H,1H3,(H,2,3)
        [TestMethod()]
        public void TestMethylphosphinicAcid()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("P");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("O");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("O");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("H");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.NewAtom("H");
            a6.FormalCharge = 0;
            mol.Atoms.Add(a6);
            IAtom a7 = builder.NewAtom("H");
            a7.FormalCharge = 0;
            mol.Atoms.Add(a7);
            IAtom a8 = builder.NewAtom("H");
            a8.FormalCharge = 0;
            mol.Atoms.Add(a8);
            IAtom a9 = builder.NewAtom("H");
            a9.FormalCharge = 0;
            mol.Atoms.Add(a9);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Double);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.NewBond(a2, a9, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = builder.NewBond(a4, a6, BondOrder.Single);
            mol.Bonds.Add(b6);
            IBond b7 = builder.NewBond(a4, a7, BondOrder.Single);
            mol.Bonds.Add(b7);
            IBond b8 = builder.NewBond(a4, a8, BondOrder.Single);
            mol.Bonds.Add(b8);

            string[] expectedTypes = { "P.ate", "O.sp3", "O.sp2", "C.sp3", "H", "H", "H", "H", "H" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Ti_2()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Ti");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Double);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "Ti.2", "C.sp2", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Ni_metallic()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Ni");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Ni.metallic" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Ni_plus()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("C");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("Ni");
            a2.FormalCharge = 1;
            mol.Atoms.Add(a2);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);

            string[] expectedTypes = { "C.sp3", "Ni.plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Pb_1()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Pb");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);

            string[] expectedTypes = { "Pb.1", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Pb_2plus()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Pb");
            a1.FormalCharge = 2;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Pb.2plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Pb_neutral()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Pb");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Pb.neutral" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Tl_neutral()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Tl");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Tl" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Tl_1()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("C");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("Tl");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);

            string[] expectedTypes = { "C.sp3", "Tl.1" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Tl_plus()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Tl");
            a1.FormalCharge = 1;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Tl.plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Mg_neutral_2()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("C");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("Mg");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a2, a3, BondOrder.Single);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "C.sp3", "Mg.neutral.2", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Mg_neutral_4()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("C");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("Mg");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IBond b1 = builder.NewBond(a2, a3, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a3, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a3, a5, BondOrder.Single);
            mol.Bonds.Add(b4);

            string[] expectedTypes = { "C.sp3", "C.sp3", "Mg.neutral", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Mg_neutral_1()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Mg");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);

            string[] expectedTypes = { "Mg.neutral.1", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Gd_3plus()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Gd");
            a1.FormalCharge = 3;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Gd.3plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Mo_4()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Mo");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Double);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);

            string[] expectedTypes = { "Mo.4", "C.sp2", "C.sp2", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Mo_metallic()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Mo");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Mo.metallic" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Pt_2()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Pt");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "Pt.2", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Pt_2plus_4()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Pt");
            a1.FormalCharge = 2;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);

            string[] expectedTypes = { "Pt.2plus.4", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Cu_metallic()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Cu");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Cu.metallic" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Cu_plus()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Cu");
            a1.FormalCharge = 1;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Cu.plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Cu_1()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Cu");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);

            string[] expectedTypes = { "Cu.1", "C.sp3", };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Ra()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Ra");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Ra.neutral" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Cr_neutral()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Cr");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Cr.neutral" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Rb_neutral()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Rb");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Rb.neutral" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Rb_plus()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Rb");
            a1.FormalCharge = 1;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Rb.plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Cr_4()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Cr");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Double);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);

            string[] expectedTypes = { "Cr.4", "C.sp2", "C.sp2", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Cr_3plus()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Cr");
            a1.FormalCharge = 3;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Cr.3plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Cr_6plus()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Cr");
            a1.FormalCharge = 6;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Cr.6plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Ba_2plus()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Ba");
            a1.FormalCharge = 2;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Ba.2plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Au_1()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("C");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("Au");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);

            string[] expectedTypes = { "C.sp3", "Au.1" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Ag_neutral()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Ag");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Ag.neutral" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        /// <summary>
        /// For example PubChem CID 3808730.
        /// </summary>
        [TestMethod()]
        public void Test_Ag_plus()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Ag");
            a1.FormalCharge = 1;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Ag.plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        /// <summary>
        /// For example PubChem CID 139654.
        /// </summary>
        [TestMethod()]
        public void Test_Ag_covalent()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Ag");
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("Cl");
            mol.Atoms.Add(a2);
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);

            string[] expectedTypes = { "Ag.1", "Cl" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_In_3plus()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("In");
            a1.FormalCharge = 3;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "In.3plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_In_3()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("In");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);

            string[] expectedTypes = { "In.3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_In_1()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("In");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Triple);
            mol.Bonds.Add(b1);

            string[] expectedTypes = { "In.1", "C.sp" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_In()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("In");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "In" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Cd_2plus()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Cd");
            a1.FormalCharge = 2;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Cd.2plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Cd_2()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Cd");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "Cd.2", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Cd_metallic()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Cd");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Cd.metallic" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Pu()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Pu");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Pu" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Th()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Th");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Th" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Ge_3()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("C");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("Ge");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a2, a3, BondOrder.Double);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a2, a4, BondOrder.Single);
            mol.Bonds.Add(b3);

            string[] expectedTypes = { "C.sp3", "Ge.3", "C.sp2", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Na_neutral()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Na");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Na.neutral" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Mn_3plus()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Mn");
            a1.FormalCharge = 3;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Mn.3plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Mn_2()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Mn");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "Mn.2", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Mn_metallic()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Mn");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "Mn.metallic" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Si_2minus_6()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Si");
            a1.FormalCharge = -2;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.NewAtom("C");
            a6.FormalCharge = 0;
            mol.Atoms.Add(a6);
            IAtom a7 = builder.NewAtom("C");
            a7.FormalCharge = 0;
            mol.Atoms.Add(a7);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.NewBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = builder.NewBond(a1, a7, BondOrder.Single);
            mol.Bonds.Add(b6);

            string[] expectedTypes = { "Si.2minus.6", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Si_3()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Si");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);

            string[] expectedTypes = { "Si.3", "C.sp2", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Si_2()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Si");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Double);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Double);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "Si.2", "C.sp2", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_As_minus()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("As");
            a1.FormalCharge = -1;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.NewAtom("C");
            a6.FormalCharge = 0;
            mol.Atoms.Add(a6);
            IAtom a7 = builder.NewAtom("C");
            a7.FormalCharge = 0;
            mol.Atoms.Add(a7);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.NewBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = builder.NewBond(a1, a7, BondOrder.Single);
            mol.Bonds.Add(b6);

            string[] expectedTypes = { "As.minus", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_As_3plus()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("As");
            a1.FormalCharge = 3;
            mol.Atoms.Add(a1);

            string[] expectedTypes = { "As.3plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_As_2()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("C");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("As");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a2, a3, BondOrder.Double);
            mol.Bonds.Add(b2);

            string[] expectedTypes = { "C.sp3", "As.2", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_As_5()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("As");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a1, a5, BondOrder.Double);
            mol.Bonds.Add(b4);

            string[] expectedTypes = { "As.5", "C.sp3", "C.sp3", "C.sp3", "C.sp2" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Fe_metallic()
        {
            //string molName = "Fe_metallic";

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Fe");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            string[] expectedTypes = { "Fe.metallic" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Fe_plus()
        {
            //string molName1 = "Fe_plus";

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("C");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("Fe");
            a3.FormalCharge = 1;
            mol.Atoms.Add(a3);
            IBond b1 = builder.NewBond(a2, a3, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);

            string[] expectedTypes1 = { "C.sp3", "C.sp3", "Fe.plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes1, mol);
        }

        [TestMethod()]
        public void Test_Fe_4()
        {
            //string molName2 = "Fe_4";

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("C");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("Fe");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IBond b1 = builder.NewBond(a2, a3, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a3, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a3, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            string[] expectedTypes2 = { "C.sp3", "C.sp3", "Fe.4", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes2, mol);
        }

        [TestMethod()]
        public void Test_Fe_3minus()
        {
            //string molName3 = "Fe_3minus";

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Fe");
            a1.FormalCharge = -3;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.NewAtom("C");
            a6.FormalCharge = 0;
            mol.Atoms.Add(a6);
            IAtom a7 = builder.NewAtom("C");
            a7.FormalCharge = 0;
            mol.Atoms.Add(a7);
            IBond b1 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.NewBond(a1, a7, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b6);
            string[] expectedTypes3 = { "Fe.3minus", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes3, mol);
        }

        [TestMethod()]
        public void Test_Fe_2plus()
        {
            //string molName4 = "Fe_2plus";

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Fe");
            a1.FormalCharge = 2;
            mol.Atoms.Add(a1);
            string[] expectedTypes4 = { "Fe.2plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes4, mol);
        }

        [TestMethod()]
        public void Test_Fe_4minus()
        {
            //string molName5 = "Fe_4minus";

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Fe");
            a1.FormalCharge = -4;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.NewAtom("C");
            a6.FormalCharge = 0;
            mol.Atoms.Add(a6);
            IAtom a7 = builder.NewAtom("C");
            a7.FormalCharge = 0;
            mol.Atoms.Add(a7);
            IBond b1 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.NewBond(a1, a7, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b6);

            string[] expectedTypes5 = { "Fe.4minus", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes5, mol);
        }

        [TestMethod()]
        public void Test_Fe_5()
        {
            //string molNameFe5 = "Fe_5";

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Fe");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.NewAtom("C");
            a6.FormalCharge = 0;
            mol.Atoms.Add(a6);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.NewBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b5);
            string[] expectedTypesFe5 = { "Fe.5", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypesFe5, mol);
        }

        [TestMethod()]
        public void Test_Fe_6()
        {
            //string molName7 = "Fe_6";

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Fe");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.NewAtom("C");
            a6.FormalCharge = 0;
            mol.Atoms.Add(a6);
            IAtom a7 = builder.NewAtom("C");
            a7.FormalCharge = 0;
            mol.Atoms.Add(a7);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.NewBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = builder.NewBond(a1, a7, BondOrder.Single);
            mol.Bonds.Add(b6);
            string[] expectedTypes7 = { "Fe.6", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes7, mol);
        }

        [TestMethod()]
        public void Test_Fe_2minus()
        {
            //string molName8 = "Fe_2minus";

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Fe");
            a1.FormalCharge = -2;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.NewAtom("C");
            a6.FormalCharge = 0;
            mol.Atoms.Add(a6);
            IAtom a7 = builder.NewAtom("C");
            a7.FormalCharge = 0;
            mol.Atoms.Add(a7);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.NewBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = builder.NewBond(a1, a7, BondOrder.Single);
            mol.Bonds.Add(b6);
            string[] expectedTypes8 = { "Fe.2minus", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes8, mol);
        }

        [TestMethod()]
        public void Test_Fe_3plus()
        {
            //string molName9 = "Fe_3plus";

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Fe");
            a1.FormalCharge = 3;
            mol.Atoms.Add(a1);
            string[] expectedTypes9 = { "Fe.3plus" };
            AssertAtomTypes(testedAtomTypes, expectedTypes9, mol);
        }

        [TestMethod()]
        public void Test_Fe_2()
        {
            //string molNameA = "Fe_2";

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("C");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("Fe");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a2, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            string[] expectedTypesA = { "C.sp3", "Fe.2", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypesA, mol);
        }

        [TestMethod()]
        public void Test_Fe_3()
        {
            //string molNameB = "Fe_3";

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Fe");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            string[] expectedTypesB = { "Fe.3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypesB, mol);
        }

        /// <summary>
        // @cdk.inchi InChI=1S/C8H16S/c1-6-3-8-4-7(6)5-9(8)2/h6-9H,3-5H2,1-2H3/t6-,7-,8+/m0/s1
        /// </summary>
        [TestMethod()]
        public void TestSulphur4()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("S");
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("C");
            mol.Atoms.Add(a5);
            IAtom a6 = builder.NewAtom("C");
            mol.Atoms.Add(a6);
            IAtom a7 = builder.NewAtom("C");
            mol.Atoms.Add(a7);
            IAtom a8 = builder.NewAtom("C");
            mol.Atoms.Add(a8);
            IAtom a9 = builder.NewAtom("C");
            mol.Atoms.Add(a9);
            IBond b1 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a8, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b5 = builder.NewBond(a2, a4, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = builder.NewBond(a2, a7, BondOrder.Single);
            mol.Bonds.Add(b6);
            IBond b8 = builder.NewBond(a3, a4, BondOrder.Single);
            mol.Bonds.Add(b8);
            IBond b9 = builder.NewBond(a3, a5, BondOrder.Single);
            mol.Bonds.Add(b9);
            IBond b10 = builder.NewBond(a3, a6, BondOrder.Single);
            mol.Bonds.Add(b10);
            IBond b14 = builder.NewBond(a5, a7, BondOrder.Single);
            mol.Bonds.Add(b14);
            IBond b15 = builder.NewBond(a5, a9, BondOrder.Single);
            mol.Bonds.Add(b15);

            string[] expectedTypes = { "S.anyl", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        /// <summary>
        /// One of the ruthenium atom types in ruthenium red (CHEBI:34956).
        /// </summary>
        [TestMethod()]
        public void Test_Ru_3minus_6()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Ru");
            a1.FormalCharge = -3;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("N");
            a2.FormalCharge = +1;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("N");
            a3.FormalCharge = +1;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("N");
            a4.FormalCharge = +1;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("N");
            a5.FormalCharge = +1;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.NewAtom("N");
            a6.FormalCharge = +1;
            mol.Atoms.Add(a6);
            IAtom a7 = builder.NewAtom("O");
            a7.FormalCharge = 0;
            mol.Atoms.Add(a7);
            IBond b1 = builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a6, a1, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a1, a7, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.NewBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b6);

            string[] expectedTypes = { "Ru.3minus.6", "N.plus", "N.plus", "N.plus", "N.plus", "N.plus", "O.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        /// <summary>
        /// One of the ruthenium atom types in ruthenium red (CHEBI:34956).
        /// </summary>
        [TestMethod()]
        public void Test_Ru_2minus_6()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Ru");
            a1.FormalCharge = -2;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("N");
            a2.FormalCharge = +1;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("N");
            a3.FormalCharge = +1;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("N");
            a4.FormalCharge = +1;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("N");
            a5.FormalCharge = +1;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.NewAtom("O");
            a6.FormalCharge = 0;
            mol.Atoms.Add(a6);
            IAtom a7 = builder.NewAtom("O");
            a7.FormalCharge = 0;
            mol.Atoms.Add(a7);
            IBond b1 = builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a6, a1, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a1, a7, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.NewBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b6);

            string[] expectedTypes = { "Ru.2minus.6", "N.plus", "N.plus", "N.plus", "N.plus", "O.sp3", "O.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Ru_10plus_6()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Ru");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.NewAtom("C");
            a6.FormalCharge = 0;
            mol.Atoms.Add(a6);
            IAtom a7 = builder.NewAtom("C");
            a7.FormalCharge = 0;
            mol.Atoms.Add(a7);
            IBond b1 = builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a6, a1, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a1, a7, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.NewBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b6);

            string[] expectedTypes = { "Ru.6", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        [TestMethod()]
        public void Test_Ru_6()
        {

            var mol = builder.NewAtomContainer();
            IAtom a1 = builder.NewAtom("Ru");
            a1.FormalCharge = 0;
            mol.Atoms.Add(a1);
            IAtom a2 = builder.NewAtom("C");
            a2.FormalCharge = 0;
            mol.Atoms.Add(a2);
            IAtom a3 = builder.NewAtom("C");
            a3.FormalCharge = 0;
            mol.Atoms.Add(a3);
            IAtom a4 = builder.NewAtom("C");
            a4.FormalCharge = 0;
            mol.Atoms.Add(a4);
            IAtom a5 = builder.NewAtom("C");
            a5.FormalCharge = 0;
            mol.Atoms.Add(a5);
            IAtom a6 = builder.NewAtom("C");
            a6.FormalCharge = 0;
            mol.Atoms.Add(a6);
            IAtom a7 = builder.NewAtom("C");
            a7.FormalCharge = 0;
            mol.Atoms.Add(a7);
            IBond b1 = builder.NewBond(a1, a5, BondOrder.Single);
            mol.Bonds.Add(b1);
            IBond b2 = builder.NewBond(a1, a6, BondOrder.Single);
            mol.Bonds.Add(b2);
            IBond b3 = builder.NewBond(a7, a1, BondOrder.Single);
            mol.Bonds.Add(b3);
            IBond b4 = builder.NewBond(a1, a2, BondOrder.Single);
            mol.Bonds.Add(b4);
            IBond b5 = builder.NewBond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b5);
            IBond b6 = builder.NewBond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b6);

            string[] expectedTypes = { "Ru.6", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3", "C.sp3" };
            AssertAtomTypes(testedAtomTypes, expectedTypes, mol);
        }

        // @cdk.inchi InChI=1S/C4H5N/c1-2-4-5-3-1/h1-5H
        [TestMethod()]
        public void Test_n_planar3_sp2_aromaticity()
        {
            var builder = CDK.Builder;

            // simulate an IAtomContainer returned from a SDFile with bond order 4 to indicate aromaticity
            IAtomContainer pyrrole = builder.NewAtomContainer();

            IAtom n1 = builder.NewAtom("N");
            IAtom c2 = builder.NewAtom("C");
            IAtom c3 = builder.NewAtom("C");
            IAtom c4 = builder.NewAtom("C");
            IAtom c5 = builder.NewAtom("C");

            IBond b1 = builder.NewBond(n1, c2, BondOrder.Single);
            b1.IsAromatic = true;
            IBond b2 = builder.NewBond(c2, c3, BondOrder.Single);
            b2.IsAromatic = true;
            IBond b3 = builder.NewBond(c3, c4, BondOrder.Single);
            b3.IsAromatic = true;
            IBond b4 = builder.NewBond(c4, c5, BondOrder.Single);
            b4.IsAromatic = true;
            IBond b5 = builder.NewBond(c5, n1, BondOrder.Single);
            b5.IsAromatic = true;

            pyrrole.Atoms.Add(n1);
            pyrrole.Atoms.Add(c2);
            pyrrole.Atoms.Add(c3);
            pyrrole.Atoms.Add(c4);
            pyrrole.Atoms.Add(c5);
            pyrrole.Bonds.Add(b1);
            pyrrole.Bonds.Add(b2);
            pyrrole.Bonds.Add(b3);
            pyrrole.Bonds.Add(b4);
            pyrrole.Bonds.Add(b5);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(pyrrole);

            Assert.AreEqual(Hybridization.Planar3, pyrrole.Atoms[0].Hybridization);
        }

        // @cdk.inchi InChI=1S/C4H5N/c1-2-4-5-3-1/h1-5H
        [TestMethod()]
        public void Test_n_planar3_sp2_aromaticity_explicitH()
        {
            var builder = CDK.Builder;

            // simulate an IAtomContainer returned from a SDFile with bond order 4 to indicate aromaticity
            IAtomContainer pyrrole = builder.NewAtomContainer();

            IAtom n1 = builder.NewAtom("N");
            IAtom c2 = builder.NewAtom("C");
            IAtom c3 = builder.NewAtom("C");
            IAtom c4 = builder.NewAtom("C");
            IAtom c5 = builder.NewAtom("C");

            IBond b1 = builder.NewBond(n1, c2, BondOrder.Single);
            b1.IsAromatic = true;
            IBond b2 = builder.NewBond(c2, c3, BondOrder.Single);
            b2.IsAromatic = true;
            IBond b3 = builder.NewBond(c3, c4, BondOrder.Single);
            b3.IsAromatic = true;
            IBond b4 = builder.NewBond(c4, c5, BondOrder.Single);
            b4.IsAromatic = true;
            IBond b5 = builder.NewBond(c5, n1, BondOrder.Single);
            b5.IsAromatic = true;

            pyrrole.Atoms.Add(n1);
            pyrrole.Atoms.Add(c2);
            pyrrole.Atoms.Add(c3);
            pyrrole.Atoms.Add(c4);
            pyrrole.Atoms.Add(c5);
            pyrrole.Bonds.Add(b1);
            pyrrole.Bonds.Add(b2);
            pyrrole.Bonds.Add(b3);
            pyrrole.Bonds.Add(b4);
            pyrrole.Bonds.Add(b5);

            // add explicit hydrogens
            IAtom h1 = builder.NewAtom("H");
            IAtom h2 = builder.NewAtom("H");
            IAtom h3 = builder.NewAtom("H");
            IAtom h4 = builder.NewAtom("H");
            IAtom h5 = builder.NewAtom("H");
            pyrrole.Atoms.Add(h1);
            pyrrole.Atoms.Add(h2);
            pyrrole.Atoms.Add(h3);
            pyrrole.Atoms.Add(h4);
            pyrrole.Atoms.Add(h5);
            pyrrole.Bonds.Add(builder.NewBond(n1, h1));
            pyrrole.Bonds.Add(builder.NewBond(c2, h2));
            pyrrole.Bonds.Add(builder.NewBond(c3, h3));
            pyrrole.Bonds.Add(builder.NewBond(c4, h4));
            pyrrole.Bonds.Add(builder.NewBond(c5, h5));

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(pyrrole);

            Assert.AreEqual(Hybridization.Planar3, pyrrole.Atoms[0].Hybridization);
        }
    }
}
