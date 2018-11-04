/* Copyright (C) 2009-2010 Syed Asad Rahman <asad@ebi.ac.uk>
 *
 * Contact: cdk-devel@lists.sourceforge.net
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
using NCDK.Aromaticities;
using NCDK.Silent;
using NCDK.Smiles;
using NCDK.SMSD.Tools;
using NCDK.Tools.Manipulator;

namespace NCDK.SMSD
{
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    // @cdk.module test-smsd
    [TestClass()]
    public class SMSDBondInsensitiveTest
    {
        private static IAtomContainer Napthalene;
        private static IAtomContainer Cyclohexane;
        private static IAtomContainer Benzene;

        static SMSDBondInsensitiveTest()
        {
            Napthalene = CreateNaphthalene();
            Cyclohexane = CreateCyclohexane();
            Benzene = CreateBenzene();
            ExtAtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(Napthalene);
            ExtAtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(Cyclohexane);
            ExtAtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(Benzene);

            Aromaticity.CDKLegacy.Apply(Napthalene);
            Aromaticity.CDKLegacy.Apply(Cyclohexane);
            Aromaticity.CDKLegacy.Apply(Benzene);
        }

        [TestMethod()]
        public void TestVFLib()
        {
            Isomorphism sbf = new Isomorphism(Algorithm.SubStructure, false);
            sbf.Init(Benzene, Benzene, true, true);
            sbf.SetChemFilters(true, true, true);
            Assert.IsTrue(sbf.IsSubgraph());
        }

        [TestMethod()]
        public void TestSubgraph()
        {
            Isomorphism sbf = new Isomorphism(Algorithm.SubStructure, false);
            sbf.Init(Benzene, Benzene, true, true);
            sbf.SetChemFilters(false, false, false);
            Assert.IsTrue(sbf.IsSubgraph());
        }

        [TestMethod()]
        public void TestCDKMCS()
        {
            Isomorphism ebimcs = new Isomorphism(Algorithm.CDKMCS, false);
            ebimcs.Init(Cyclohexane, Benzene, true, true);
            ebimcs.SetChemFilters(true, true, true);
            Assert.AreEqual(6, ebimcs.GetFirstMapping().Count);
            Assert.IsTrue(ebimcs.IsSubgraph());
        }

        [TestMethod()]
        public void TestMCSPlus()
        {
            //TO DO fix me this error
            Isomorphism ebimcs = new Isomorphism(Algorithm.MCSPlus, false);
            ebimcs.Init(Cyclohexane, Benzene, true, true);
            ebimcs.SetChemFilters(true, true, true);
            Assert.IsTrue(ebimcs.IsSubgraph());
        }

        [TestMethod()]
        public void TestSMSD()
        {
            Isomorphism ebimcs = new Isomorphism(Algorithm.Default, false);
            ebimcs.Init(Cyclohexane, Benzene, true, true);
            ebimcs.SetChemFilters(true, true, true);
            Assert.AreEqual(6, ebimcs.GetFirstMapping().Count);
        }

        [TestMethod()]
        public void TestSMSDCyclohexaneBenzeneSubgraph()
        {

            Isomorphism ebimcs1 = new Isomorphism(Algorithm.SubStructure, false);
            ebimcs1.Init(Cyclohexane, Benzene, true, true);
            ebimcs1.SetChemFilters(true, true, true);
            Assert.IsTrue(ebimcs1.IsSubgraph());
        }

        [TestMethod()]
        public void TestSMSDBondInSensitive()
        {
            Isomorphism ebimcs1 = new Isomorphism(Algorithm.Default, false);
            ebimcs1.Init(Cyclohexane, Benzene, true, true);
            ebimcs1.SetChemFilters(true, true, true);
            Assert.AreEqual(6, ebimcs1.GetFirstAtomMapping().Count);

            Isomorphism ebimcs2 = new Isomorphism(Algorithm.Default, false);
            ebimcs2.Init(Benzene, Napthalene, true, true);
            ebimcs2.SetChemFilters(true, true, true);
            Assert.AreEqual(6, ebimcs2.GetFirstAtomMapping().Count);
        }

        [TestMethod()]
        public void TestSMSDChemicalFilters()
        {
            Isomorphism ebimcs = new Isomorphism(Algorithm.Default, false);
            ebimcs.Init(Cyclohexane, Benzene, true, true);
            ebimcs.SetChemFilters(true, true, true);
            Assert.AreEqual(12, ebimcs.GetAllMapping().Count);
            Assert.IsTrue(ebimcs.IsSubgraph());
        }

        [TestMethod()]
        public void TestCyclopropaneNotASubgraphOfIsoButane()
        {
            IAtomContainer cycloPropane = CreateCyclopropane();
            IAtomContainer isobutane = CreateIsobutane();

            ExtAtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(cycloPropane);
            ExtAtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(isobutane);

            IAtomContainer source = ExtAtomContainerManipulator.RemoveHydrogensExceptSingleAndPreserveAtomID(cycloPropane);
            IAtomContainer target = ExtAtomContainerManipulator.RemoveHydrogensExceptSingleAndPreserveAtomID(isobutane);

            Aromaticity.CDKLegacy.Apply(source);
            Aromaticity.CDKLegacy.Apply(target);

            bool bondSensitive = false;
            bool removeHydrogen = true;
            bool stereoMatch = true;
            bool fragmentMinimization = true;
            bool energyMinimization = true;

            //    Calling the main algorithm to perform MCS cearch
            Isomorphism comparison = new Isomorphism(Algorithm.SubStructure, bondSensitive);
            comparison.Init(source, target, removeHydrogen, true);
            comparison.SetChemFilters(stereoMatch, fragmentMinimization, energyMinimization);

            //        Cyclopropane is not a subgraph of Isobutane
            Assert.IsFalse(comparison.IsSubgraph());
            Assert.AreEqual(0.625, comparison.GetTanimotoSimilarity());
        }

        [TestMethod()]
        public void TestSingleMappingTesting()
        {
            var sp = CDK.SmilesParser;
            var atomContainer = sp.ParseSmiles("C");

            IAtomContainer mol2 = Create4Toluene();

            ExtAtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol2);
            Aromaticity.CDKLegacy.Apply(mol2);

            bool bondSensitive = false;
            bool removeHydrogen = true;
            bool stereoMatch = false;
            bool fragmentMinimization = false;
            bool energyMinimization = false;

            Isomorphism comparison = new Isomorphism(Algorithm.Default, bondSensitive);
            comparison.Init(atomContainer, mol2, removeHydrogen, true);
            comparison.SetChemFilters(stereoMatch, fragmentMinimization, energyMinimization);

            Assert.IsTrue(comparison.IsSubgraph());
            Assert.AreEqual(7, comparison.GetAllMapping().Count);
        }

        /// <summary>
        /// frag is a subgraph of the het mol
        /// </summary>
        [TestMethod()]
        public void TestSMSDFragHetSubgraph()
        {
            var sp = CDK.SmilesParser;
            string file1 = "O=C1NC(=O)C2=C(N1)NC(=O)C=N2";
            string file2 = "OC[C@@H](O)[C@@H](O)[C@@H](O)CN1C(O)C(CCC(O)O)NC2C(O)NC(O)NC12";

            var mol1 = sp.ParseSmiles(file1);
            var mol2 = sp.ParseSmiles(file2);

            ExtAtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol1);
            ExtAtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol2);

            IAtomContainer source = ExtAtomContainerManipulator.RemoveHydrogensExceptSingleAndPreserveAtomID(mol1);
            IAtomContainer target = ExtAtomContainerManipulator.RemoveHydrogensExceptSingleAndPreserveAtomID(mol2);

            //    Calling the main algorithm to perform MCS search

            Aromaticity.CDKLegacy.Apply(source);
            Aromaticity.CDKLegacy.Apply(target);

            bool bondSensitive = false;
            bool removeHydrogen = true;
            bool stereoMatch = true;
            bool fragmentMinimization = true;
            bool energyMinimization = true;

            Isomorphism comparison = new Isomorphism(Algorithm.Default, bondSensitive);
            comparison.Init(source, target, removeHydrogen, true);
            comparison.SetChemFilters(stereoMatch, fragmentMinimization, energyMinimization);

            Assert.IsTrue(comparison.IsSubgraph());
            Assert.AreEqual(13, comparison.GetFirstMapping().Count);
        }

        private IAtomContainer Create4Toluene()
        {
            IAtomContainer result = ChemObjectBuilder.Instance.NewAtomContainer();
            IAtom c1 = ChemObjectBuilder.Instance.NewAtom("C");
            c1.Id = "1";
            IAtom c2 = ChemObjectBuilder.Instance.NewAtom("C");
            c2.Id = "2";
            IAtom c3 = ChemObjectBuilder.Instance.NewAtom("C");
            c3.Id = "3";
            IAtom c4 = ChemObjectBuilder.Instance.NewAtom("C");
            c4.Id = "4";
            IAtom c5 = ChemObjectBuilder.Instance.NewAtom("C");
            c5.Id = "5";
            IAtom c6 = ChemObjectBuilder.Instance.NewAtom("C");
            c6.Id = "6";
            IAtom c7 = ChemObjectBuilder.Instance.NewAtom("C");
            c7.Id = "7";

            result.Atoms.Add(c1);
            result.Atoms.Add(c2);
            result.Atoms.Add(c3);
            result.Atoms.Add(c4);
            result.Atoms.Add(c5);
            result.Atoms.Add(c6);
            result.Atoms.Add(c7);

            IBond bond1 = new Bond(c1, c2, BondOrder.Single);
            IBond bond2 = new Bond(c2, c3, BondOrder.Double);
            IBond bond3 = new Bond(c3, c4, BondOrder.Single);
            IBond bond4 = new Bond(c4, c5, BondOrder.Double);
            IBond bond5 = new Bond(c5, c6, BondOrder.Single);
            IBond bond6 = new Bond(c6, c1, BondOrder.Double);
            IBond bond7 = new Bond(c7, c4, BondOrder.Single);

            result.Bonds.Add(bond1);
            result.Bonds.Add(bond2);
            result.Bonds.Add(bond3);
            result.Bonds.Add(bond4);
            result.Bonds.Add(bond5);
            result.Bonds.Add(bond6);
            result.Bonds.Add(bond7);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(result);

            return result;
        }

        public IAtomContainer CreateMethane()
        {
            IAtomContainer result = ChemObjectBuilder.Instance.NewAtomContainer();
            IAtom c1 = ChemObjectBuilder.Instance.NewAtom("C");
            result.Atoms.Add(c1);

            return result;
        }

        public IAtomContainer CreatePropane()
        {
            IAtomContainer result = ChemObjectBuilder.Instance.NewAtomContainer();
            IAtom c1 = ChemObjectBuilder.Instance.NewAtom("C");
            IAtom c2 = ChemObjectBuilder.Instance.NewAtom("C");
            IAtom c3 = ChemObjectBuilder.Instance.NewAtom("C");

            result.Atoms.Add(c1);
            result.Atoms.Add(c2);
            result.Atoms.Add(c3);

            IBond bond1 = new Bond(c1, c2, BondOrder.Single);
            IBond bond2 = new Bond(c2, c3, BondOrder.Single);

            result.Bonds.Add(bond1);
            result.Bonds.Add(bond2);

            return result;
        }

        public IAtomContainer CreateHexane()
        {
            IAtomContainer result = ChemObjectBuilder.Instance.NewAtomContainer();

            IAtom c1 = ChemObjectBuilder.Instance.NewAtom("C");
            c1.Id = "1";
            IAtom c2 = ChemObjectBuilder.Instance.NewAtom("C");
            c2.Id = "2";
            IAtom c3 = ChemObjectBuilder.Instance.NewAtom("C");
            c3.Id = "3";
            IAtom c4 = ChemObjectBuilder.Instance.NewAtom("C");
            c4.Id = "4";
            IAtom c5 = ChemObjectBuilder.Instance.NewAtom("C");
            c5.Id = "5";
            IAtom c6 = ChemObjectBuilder.Instance.NewAtom("C");
            c6.Id = "6";

            result.Atoms.Add(c1);
            result.Atoms.Add(c2);
            result.Atoms.Add(c3);
            result.Atoms.Add(c4);
            result.Atoms.Add(c5);
            result.Atoms.Add(c6);

            IBond bond1 = new Bond(c1, c2, BondOrder.Single);
            IBond bond2 = new Bond(c2, c3, BondOrder.Single);
            IBond bond3 = new Bond(c3, c4, BondOrder.Single);
            IBond bond4 = new Bond(c4, c5, BondOrder.Single);
            IBond bond5 = new Bond(c5, c6, BondOrder.Single);

            result.Bonds.Add(bond1);
            result.Bonds.Add(bond2);
            result.Bonds.Add(bond3);
            result.Bonds.Add(bond4);
            result.Bonds.Add(bond5);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(result);

            return result;
        }

        public static IAtomContainer CreateBenzene()
        {
            IAtomContainer result = ChemObjectBuilder.Instance.NewAtomContainer();

            IAtom c1 = ChemObjectBuilder.Instance.NewAtom("C");
            c1.Id = "1";
            IAtom c2 = ChemObjectBuilder.Instance.NewAtom("C");
            c2.Id = "2";
            IAtom c3 = ChemObjectBuilder.Instance.NewAtom("C");
            c3.Id = "3";
            IAtom c4 = ChemObjectBuilder.Instance.NewAtom("C");
            c4.Id = "4";
            IAtom c5 = ChemObjectBuilder.Instance.NewAtom("C");
            c5.Id = "5";
            IAtom c6 = ChemObjectBuilder.Instance.NewAtom("C");
            c6.Id = "6";

            result.Atoms.Add(c1);
            result.Atoms.Add(c2);
            result.Atoms.Add(c3);
            result.Atoms.Add(c4);
            result.Atoms.Add(c5);
            result.Atoms.Add(c6);

            IBond bond1 = new Bond(c1, c2, BondOrder.Single);
            IBond bond2 = new Bond(c2, c3, BondOrder.Double);
            IBond bond3 = new Bond(c3, c4, BondOrder.Single);
            IBond bond4 = new Bond(c4, c5, BondOrder.Double);
            IBond bond5 = new Bond(c5, c6, BondOrder.Single);
            IBond bond6 = new Bond(c6, c1, BondOrder.Double);

            result.Bonds.Add(bond1);
            result.Bonds.Add(bond2);
            result.Bonds.Add(bond3);
            result.Bonds.Add(bond4);
            result.Bonds.Add(bond5);
            result.Bonds.Add(bond6);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(result);
            return result;
        }

        //
        //    public static Molecule CreatePyridine() {
        //        Molecule result = new DefaultMolecule();
        //        Atom c1 = result.Atoms.Add("C");
        //        Atom c2 = result.Atoms.Add("C");
        //        Atom c3 = result.Atoms.Add("C");
        //        Atom c4 = result.Atoms.Add("C");
        //        Atom c5 = result.Atoms.Add("C");
        //        Atom c6 = result.Atoms.Add("N");
        //
        //        result.Connect(c1, c2, 1);
        //        result.Connect(c2, c3, 2);
        //        result.Connect(c3, c4, 1);
        //        result.Connect(c4, c5, 2);
        //        result.Connect(c5, c6, 1);
        //        result.Connect(c6, c1, 2);
        //
        //        return result;
        //    }
        //
        //    public static Molecule CreateToluene() {
        //        Molecule result = new DefaultMolecule();
        //        Atom c1 = result.Atoms.Add("C");
        //        Atom c2 = result.Atoms.Add("C");
        //        Atom c3 = result.Atoms.Add("C");
        //        Atom c4 = result.Atoms.Add("C");
        //        Atom c5 = result.Atoms.Add("C");
        //        Atom c6 = result.Atoms.Add("C");
        //        Atom c7 = result.Atoms.Add("C");
        //
        //        result.Connect(c1, c2, 1);
        //        result.Connect(c2, c3, 2);
        //        result.Connect(c3, c4, 1);
        //        result.Connect(c4, c5, 2);
        //        result.Connect(c5, c6, 1);
        //        result.Connect(c6, c1, 2);
        //        result.Connect(c7, c1, 1);
        //
        //        return result;
        //    }
        //
        //    public static Molecule CreatePhenol() {
        //        Molecule result = new DefaultMolecule();
        //        Atom c1 = result.Atoms.Add("C");
        //        Atom c2 = result.Atoms.Add("C");
        //        Atom c3 = result.Atoms.Add("C");
        //        Atom c4 = result.Atoms.Add("C");
        //        Atom c5 = result.Atoms.Add("C");
        //        Atom c6 = result.Atoms.Add("C");
        //        Atom c7 = result.Atoms.Add("O");
        //
        //        result.Connect(c1, c2, 1);
        //        result.Connect(c2, c3, 2);
        //        result.Connect(c3, c4, 1);
        //        result.Connect(c4, c5, 2);
        //        result.Connect(c5, c6, 1);
        //        result.Connect(c6, c1, 2);
        //        result.Connect(c7, c1, 1);
        //
        //        return result;
        //    }

        public static IAtomContainer CreateNaphthalene()
        {
            IAtomContainer result = ChemObjectBuilder.Instance.NewAtomContainer();

            IAtom c1 = ChemObjectBuilder.Instance.NewAtom("C");
            c1.Id = "1";
            IAtom c2 = ChemObjectBuilder.Instance.NewAtom("C");
            c2.Id = "2";
            IAtom c3 = ChemObjectBuilder.Instance.NewAtom("C");
            c3.Id = "3";
            IAtom c4 = ChemObjectBuilder.Instance.NewAtom("C");
            c4.Id = "4";
            IAtom c5 = ChemObjectBuilder.Instance.NewAtom("C");
            c5.Id = "5";
            IAtom c6 = ChemObjectBuilder.Instance.NewAtom("C");
            c6.Id = "6";
            IAtom c7 = ChemObjectBuilder.Instance.NewAtom("C");
            c4.Id = "7";
            IAtom c8 = ChemObjectBuilder.Instance.NewAtom("C");
            c5.Id = "8";
            IAtom c9 = ChemObjectBuilder.Instance.NewAtom("C");
            c6.Id = "9";
            IAtom c10 = ChemObjectBuilder.Instance.NewAtom("C");
            c6.Id = "10";

            result.Atoms.Add(c1);
            result.Atoms.Add(c2);
            result.Atoms.Add(c3);
            result.Atoms.Add(c4);
            result.Atoms.Add(c5);
            result.Atoms.Add(c6);
            result.Atoms.Add(c7);
            result.Atoms.Add(c8);
            result.Atoms.Add(c9);
            result.Atoms.Add(c10);

            IBond bond1 = new Bond(c1, c2, BondOrder.Single);
            IBond bond2 = new Bond(c2, c3, BondOrder.Double);
            IBond bond3 = new Bond(c3, c4, BondOrder.Single);
            IBond bond4 = new Bond(c4, c5, BondOrder.Double);
            IBond bond5 = new Bond(c5, c6, BondOrder.Single);
            IBond bond6 = new Bond(c6, c1, BondOrder.Double);
            IBond bond7 = new Bond(c5, c7, BondOrder.Single);
            IBond bond8 = new Bond(c7, c8, BondOrder.Double);
            IBond bond9 = new Bond(c8, c9, BondOrder.Single);
            IBond bond10 = new Bond(c9, c10, BondOrder.Double);
            IBond bond11 = new Bond(c10, c6, BondOrder.Single);

            result.Bonds.Add(bond1);
            result.Bonds.Add(bond2);
            result.Bonds.Add(bond3);
            result.Bonds.Add(bond4);
            result.Bonds.Add(bond5);
            result.Bonds.Add(bond6);
            result.Bonds.Add(bond7);
            result.Bonds.Add(bond8);
            result.Bonds.Add(bond9);
            result.Bonds.Add(bond10);
            result.Bonds.Add(bond11);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(result);

            return result;
        }

        //
        //    public static Molecule CreateAcetone() {
        //        Molecule result = new DefaultMolecule();
        //        Atom c0 = result.Atoms.Add("C");
        //        Atom c1 = result.Atoms.Add("C");
        //        Atom c2 = result.Atoms.Add("C");
        //        Atom o3 = result.Atoms.Add("O");
        //
        //        result.Connect(c0, c1, 1);
        //        result.Connect(c1, c2, 1);
        //        result.Connect(c1, o3, 2);
        //
        //        return result;
        //    }
        //
        //    public static Molecule CreateNeopentane() {
        //        Molecule result = new DefaultMolecule();
        //        Atom c0 = result.Atoms.Add("C");
        //        Atom c1 = result.Atoms.Add("C");
        //        Atom c2 = result.Atoms.Add("C");
        //        Atom c3 = result.Atoms.Add("C");
        //        Atom c4 = result.Atoms.Add("C");
        //
        //        result.Connect(c0, c1, 1);
        //        result.Connect(c0, c2, 1);
        //        result.Connect(c0, c3, 1);
        //        result.Connect(c0, c4, 1);
        //
        //        return result;
        //    }
        //
        //    public static Molecule CreateCubane() {
        //        Molecule result = new DefaultMolecule();
        //        Atom c0 = result.Atoms.Add("C");
        //        Atom c1 = result.Atoms.Add("C");
        //        Atom c2 = result.Atoms.Add("C");
        //        Atom c3 = result.Atoms.Add("C");
        //        Atom c4 = result.Atoms.Add("C");
        //        Atom c5 = result.Atoms.Add("C");
        //        Atom c6 = result.Atoms.Add("C");
        //        Atom c7 = result.Atoms.Add("C");
        //
        //        result.Connect(c0, c1, 1);
        //        result.Connect(c1, c2, 1);
        //        result.Connect(c2, c3, 1);
        //        result.Connect(c3, c0, 1);
        //
        //        result.Connect(c4, c5, 1);
        //        result.Connect(c5, c6, 1);
        //        result.Connect(c6, c7, 1);
        //        result.Connect(c7, c4, 1);
        //
        //        result.Connect(c0, c4, 1);
        //        result.Connect(c1, c5, 1);
        //        result.Connect(c2, c6, 1);
        //        result.Connect(c3, c7, 1);
        //
        //        return result;
        //    }
        //
        //    public static Molecule CreateBicyclo220hexane() {
        //        Molecule result = new DefaultMolecule();
        //        Atom c0 = result.Atoms.Add("C");
        //        Atom c1 = result.Atoms.Add("C");
        //        Atom c2 = result.Atoms.Add("C");
        //        Atom c3 = result.Atoms.Add("C");
        //        Atom c4 = result.Atoms.Add("C");
        //        Atom c5 = result.Atoms.Add("C");
        //
        //        result.Connect(c0, c1, 1);
        //        result.Connect(c1, c2, 1);
        //        result.Connect(c2, c3, 1);
        //        result.Connect(c3, c4, 1);
        //        result.Connect(c4, c5, 1);
        //        result.Connect(c5, c0, 1);
        //        result.Connect(c2, c5, 1);
        //
        //        return result;
        //    }
        //
        //    public static Molecule CreateEthylbenzeneWithSuperatom() {
        //        Molecule result = Molecules.CreateBenzene();
        //        Atom carbon1 = result.Atoms.Add("C");
        //        Atom carbon2 = result.Atoms.Add("C");
        //        Bond crossingBond = result.Connect(result.Atoms[0], carbon1, 1);
        //        result.Connect(carbon1, carbon2, 1);
        //
        //        Superatom substructure = result.AddSuperatom();
        //        substructure.Atoms.Add(carbon1);
        //        substructure.Atoms.Add(carbon2);
        //        substructure.AddCrossingBond(crossingBond);
        //        substructure.SetCrossingVector(crossingBond, 0.1, 0.1);
        //        substructure.Label = "Ethyl";
        //
        //        return result;
        //    }

        public static IAtomContainer CreateCyclohexane()
        {

            IAtomContainer result = ChemObjectBuilder.Instance.NewAtomContainer();

            IAtom c1 = ChemObjectBuilder.Instance.NewAtom("C");
            c1.Id = "1";
            IAtom c2 = ChemObjectBuilder.Instance.NewAtom("C");
            c2.Id = "2";
            IAtom c3 = ChemObjectBuilder.Instance.NewAtom("C");
            c3.Id = "3";
            IAtom c4 = ChemObjectBuilder.Instance.NewAtom("C");
            c4.Id = "4";
            IAtom c5 = ChemObjectBuilder.Instance.NewAtom("C");
            c5.Id = "5";
            IAtom c6 = ChemObjectBuilder.Instance.NewAtom("C");
            c6.Id = "6";

            result.Atoms.Add(c1);
            result.Atoms.Add(c2);
            result.Atoms.Add(c3);
            result.Atoms.Add(c4);
            result.Atoms.Add(c5);
            result.Atoms.Add(c6);

            IBond bond1 = new Bond(c1, c2, BondOrder.Single);
            IBond bond2 = new Bond(c2, c3, BondOrder.Single);
            IBond bond3 = new Bond(c3, c4, BondOrder.Single);
            IBond bond4 = new Bond(c4, c5, BondOrder.Single);
            IBond bond5 = new Bond(c5, c6, BondOrder.Single);
            IBond bond6 = new Bond(c6, c1, BondOrder.Single);

            result.Bonds.Add(bond1);
            result.Bonds.Add(bond2);
            result.Bonds.Add(bond3);
            result.Bonds.Add(bond4);
            result.Bonds.Add(bond5);
            result.Bonds.Add(bond6);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(result);

            return result;

        }

        public static IAtomContainer CreateCyclopropane()
        {
            IAtomContainer result = ChemObjectBuilder.Instance.NewAtomContainer();

            IAtom c1 = ChemObjectBuilder.Instance.NewAtom("C");
            IAtom c2 = ChemObjectBuilder.Instance.NewAtom("C");
            IAtom c3 = ChemObjectBuilder.Instance.NewAtom("C");

            result.Atoms.Add(c1);
            result.Atoms.Add(c2);
            result.Atoms.Add(c3);

            IBond bond1 = new Bond(c1, c2, BondOrder.Single);
            IBond bond2 = new Bond(c2, c3, BondOrder.Single);
            IBond bond3 = new Bond(c3, c1, BondOrder.Single);

            result.Bonds.Add(bond1);
            result.Bonds.Add(bond2);
            result.Bonds.Add(bond3);

            return result;
        }

        public static IAtomContainer CreateIsobutane()
        {
            IAtomContainer result = ChemObjectBuilder.Instance.NewAtomContainer();

            IAtom c1 = ChemObjectBuilder.Instance.NewAtom("C");
            IAtom c2 = ChemObjectBuilder.Instance.NewAtom("C");
            IAtom c3 = ChemObjectBuilder.Instance.NewAtom("C");
            IAtom c4 = ChemObjectBuilder.Instance.NewAtom("C");

            result.Atoms.Add(c1);
            result.Atoms.Add(c2);
            result.Atoms.Add(c3);
            result.Atoms.Add(c4);

            IBond bond1 = new Bond(c1, c2, BondOrder.Single);
            IBond bond2 = new Bond(c2, c3, BondOrder.Single);
            IBond bond3 = new Bond(c2, c4, BondOrder.Single);

            result.Bonds.Add(bond1);
            result.Bonds.Add(bond2);
            result.Bonds.Add(bond3);

            return result;
        }
    }
}
