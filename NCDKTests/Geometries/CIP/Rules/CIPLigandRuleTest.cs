/* Copyright (C) 2010  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.Smiles;
using System.Collections.Generic;

namespace NCDK.Geometries.CIP.Rules
{
    /**
     * @cdk.module test-cip
     */
    [TestClass()]
    public class CIPLigandRuleTest : CDKTestCase
    {
        static SmilesParser smiles = new SmilesParser(Silent.ChemObjectBuilder.Instance);

        [TestMethod()]
        public void TestCBrIFCl()
        {
            IAtomContainer molecule = smiles.ParseSmiles("FC(Br)(Cl)I");
            ILigand ligandF = new Ligand(molecule, new VisitedAtoms(), molecule.Atoms[1], molecule.Atoms[0]);
            ILigand ligandBr = new Ligand(molecule, new VisitedAtoms(), molecule.Atoms[1], molecule.Atoms[2]);
            ILigand ligandCl = new Ligand(molecule, new VisitedAtoms(), molecule.Atoms[1], molecule.Atoms[3]);
            ILigand ligandI = new Ligand(molecule, new VisitedAtoms(), molecule.Atoms[1], molecule.Atoms[4]);
            ISequenceSubRule<ILigand> rule = new CIPLigandRule();
            Assert.AreEqual(-1, rule.Compare(ligandF, ligandI));
            Assert.AreEqual(-1, rule.Compare(ligandF, ligandBr));
            Assert.AreEqual(-1, rule.Compare(ligandF, ligandCl));
            Assert.AreEqual(-1, rule.Compare(ligandCl, ligandI));
            Assert.AreEqual(-1, rule.Compare(ligandCl, ligandBr));
            Assert.AreEqual(-1, rule.Compare(ligandBr, ligandI));

            List<ILigand> ligands = new List<ILigand>();
            ligands.Add(ligandI);
            ligands.Add(ligandBr);
            ligands.Add(ligandF);
            ligands.Add(ligandCl);
            ligands.Sort(new CIPLigandRule());

            Assert.AreEqual("F", ligands[0].GetLigandAtom().Symbol);
            Assert.AreEqual("Cl", ligands[1].GetLigandAtom().Symbol);
            Assert.AreEqual("Br", ligands[2].GetLigandAtom().Symbol);
            Assert.AreEqual("I", ligands[3].GetLigandAtom().Symbol);
        }

        [TestMethod()]
        public void TestCompare_Identity()
        {
            IAtomContainer molecule = smiles.ParseSmiles("CC(Br)([13C])[H]");
            ILigand ligand = new Ligand(molecule, new VisitedAtoms(), molecule.Atoms[1], molecule.Atoms[0]);
            ISequenceSubRule<ILigand> rule = new CIPLigandRule();
            Assert.AreEqual(0, rule.Compare(ligand, ligand));
        }

        [TestMethod()]
        public void TestCompare()
        {
            IAtomContainer molecule = smiles.ParseSmiles("CC(Br)([13C])[H]");
            ILigand ligand1 = new Ligand(molecule, new VisitedAtoms(), molecule.Atoms[1], molecule.Atoms[0]);
            ILigand ligand2 = new Ligand(molecule, new VisitedAtoms(), molecule.Atoms[1], molecule.Atoms[2]);
            ISequenceSubRule<ILigand> rule = new CIPLigandRule();
            Assert.AreEqual(-1, rule.Compare(ligand1, ligand2));
            Assert.AreEqual(1, rule.Compare(ligand2, ligand1));
        }

        [TestMethod()]
        public void TestOrder()
        {
            IAtomContainer molecule = smiles.ParseSmiles("CC(Br)([13C])[H]");
            List<ILigand> ligands = new List<ILigand>();
            VisitedAtoms visitedAtoms = new VisitedAtoms();
            ligands.Add(CIPTool.DefineLigand(molecule, visitedAtoms, 1, 4));
            ligands.Add(CIPTool.DefineLigand(molecule, visitedAtoms, 1, 3));
            ligands.Add(CIPTool.DefineLigand(molecule, visitedAtoms, 1, 2));
            ligands.Add(CIPTool.DefineLigand(molecule, visitedAtoms, 1, 0));

            ligands.Sort(new CIPLigandRule());
            Assert.AreEqual("H", ligands[0].GetLigandAtom().Symbol);
            Assert.AreEqual("C", ligands[1].GetLigandAtom().Symbol);
            Assert.AreEqual("C", ligands[2].GetLigandAtom().Symbol);
            Assert.AreEqual(13, ligands[2].GetLigandAtom().MassNumber.Value);
            Assert.AreEqual("Br", ligands[3].GetLigandAtom().Symbol);
        }

        /**
         * Test that verifies the branching of the side chains determines precedence for ties.
         */
        [TestMethod()]
        public void TestSideChains()
        {
            IAtomContainer molecule = smiles.ParseSmiles("CC(C)C([H])(C)CC");
            ILigand ligand1 = CIPTool.DefineLigand(molecule, new VisitedAtoms(), 3, 6);
            ILigand ligand2 = CIPTool.DefineLigand(molecule, new VisitedAtoms(), 3, 1);
            ISequenceSubRule<ILigand> rule = new CIPLigandRule();
            Assert.AreEqual(-1, rule.Compare(ligand1, ligand2));
            Assert.AreEqual(1, rule.Compare(ligand2, ligand1));
        }

        /**
         * Test that verifies the branching of the side chains determines precedence for ties,
         * but unlike {@link #TestSideChains()}, the tie only gets resolved after recursion.
         */
        [TestMethod()]
        public void TestSideChains_Recursive()
        {
            IAtomContainer molecule = smiles.ParseSmiles("CCCC([H])(C)CC");
            ILigand ligand1 = CIPTool.DefineLigand(molecule, new VisitedAtoms(), 3, 6);
            ILigand ligand2 = CIPTool.DefineLigand(molecule, new VisitedAtoms(), 3, 1);
            ISequenceSubRule<ILigand> rule = new CIPLigandRule();
            Assert.AreEqual(-1, rule.Compare(ligand1, ligand2));
            Assert.AreEqual(1, rule.Compare(ligand2, ligand1));
        }

        /**
         * The CIP sequence rule prescribes that double bonded side chains of a ligand
         * are counted twice. This alone, is not enough to distinguish between a
         * hypothetical dialcohol and a aldehyde.
         */
        [TestMethod()]
        public void TestTwoVersusDoubleBondedOxygen()
        {
            IAtomContainer molecule = smiles.ParseSmiles("OC(O)C([H])(C)C=O");
            ILigand ligand1 = CIPTool.DefineLigand(molecule, new VisitedAtoms(), 3, 1);
            ILigand ligand2 = CIPTool.DefineLigand(molecule, new VisitedAtoms(), 3, 6);
            ISequenceSubRule<ILigand> rule = new CIPLigandRule();
            Assert.AreEqual(-1, rule.Compare(ligand1, ligand2));
            Assert.AreEqual(1, rule.Compare(ligand2, ligand1));
        }

        /**
         * Tests deep recursion.
         */
        [TestMethod()]
        public void TestDeepRecursion()
        {
            IAtomContainer molecule = smiles.ParseSmiles("CC([H])(CCCCCCCCCC)CCCCCCCCC");
            ILigand ligand1 = CIPTool.DefineLigand(molecule, new VisitedAtoms(), 1, 3);
            ILigand ligand2 = CIPTool.DefineLigand(molecule, new VisitedAtoms(), 1, 13);
            ISequenceSubRule<ILigand> rule = new CIPLigandRule();
            Assert.AreEqual(1, rule.Compare(ligand1, ligand2));
            Assert.AreEqual(-1, rule.Compare(ligand2, ligand1));
        }

        [TestMethod()]
        public void TestImplicitHydrogen_Same()
        {
            IAtomContainer molecule = smiles.ParseSmiles("CC(Br)([13C])[H]");
            ILigand ligand1 = new ImplicitHydrogenLigand(molecule, new VisitedAtoms(), molecule.Atoms[1]);
            ILigand ligand2 = new Ligand(molecule, new VisitedAtoms(), molecule.Atoms[1], molecule.Atoms[4]);
            ISequenceSubRule<ILigand> rule = new CIPLigandRule();
            Assert.AreEqual(0, rule.Compare(ligand1, ligand2));
            Assert.AreEqual(0, rule.Compare(ligand2, ligand1));
        }

        [TestMethod()]
        public void TestImplicitHydrogen()
        {
            IAtomContainer molecule = smiles.ParseSmiles("CC(Br)([2H])[H]");
            ILigand ligand1 = new ImplicitHydrogenLigand(molecule, new VisitedAtoms(), molecule.Atoms[1]);
            ILigand ligand2 = new Ligand(molecule, new VisitedAtoms(), molecule.Atoms[1], molecule.Atoms[3]);
            ISequenceSubRule<ILigand> rule = new CIPLigandRule();
            Assert.AreEqual(-1, rule.Compare(ligand1, ligand2));
            Assert.AreEqual(1, rule.Compare(ligand2, ligand1));

            ligand2 = new Ligand(molecule, new VisitedAtoms(), molecule.Atoms[1], molecule.Atoms[2]);
            Assert.AreEqual(-1, rule.Compare(ligand1, ligand2));
            Assert.AreEqual(1, rule.Compare(ligand2, ligand1));
        }
    }
}
