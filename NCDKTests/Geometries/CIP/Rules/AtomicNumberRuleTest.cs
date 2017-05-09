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
    // @cdk.module test-cip
    [TestClass()]
    public class AtomicNumberRuleTest : CDKTestCase
    {
        static SmilesParser smiles = new SmilesParser(Silent.ChemObjectBuilder.Instance);
        static IAtomContainer molecule = smiles.ParseSmiles("ClC(Br)(I)[H]");

        [TestMethod()]
        public void TestCompare_Identity()
        {
            ILigand ligand = new Ligand(molecule, new VisitedAtoms(), molecule.Atoms[1], molecule.Atoms[0]);
            ISequenceSubRule<ILigand> rule = new AtomicNumberRule();
            Assert.AreEqual(0, rule.Compare(ligand, ligand));
        }

        [TestMethod()]
        public void TestCompare()
        {
            ILigand ligand1 = new Ligand(molecule, new VisitedAtoms(), molecule.Atoms[1], molecule.Atoms[0]);
            ILigand ligand2 = new Ligand(molecule, new VisitedAtoms(), molecule.Atoms[1], molecule.Atoms[2]);
            ISequenceSubRule<ILigand> rule = new AtomicNumberRule();
            Assert.AreEqual(-1, rule.Compare(ligand1, ligand2));
            Assert.AreEqual(1, rule.Compare(ligand2, ligand1));
        }

        [TestMethod()]
        public void TestOrder()
        {
            VisitedAtoms visitedAtoms = new VisitedAtoms();
            ILigand ligand1 = new Ligand(molecule, visitedAtoms, molecule.Atoms[1], molecule.Atoms[4]);
            ILigand ligand2 = new Ligand(molecule, visitedAtoms, molecule.Atoms[1], molecule.Atoms[3]);
            ILigand ligand3 = new Ligand(molecule, visitedAtoms, molecule.Atoms[1], molecule.Atoms[2]);
            ILigand ligand4 = new Ligand(molecule, visitedAtoms, molecule.Atoms[1], molecule.Atoms[0]);
            List<ILigand> ligands = new List<ILigand>();
            ligands.Add(ligand1);
            ligands.Add(ligand2);
            ligands.Add(ligand3);
            ligands.Add(ligand4);

            ligands.Sort(new AtomicNumberRule());
            Assert.AreEqual("H", ligands[0].GetLigandAtom().Symbol);
            Assert.AreEqual("Cl", ligands[1].GetLigandAtom().Symbol);
            Assert.AreEqual("Br", ligands[2].GetLigandAtom().Symbol);
            Assert.AreEqual("I", ligands[3].GetLigandAtom().Symbol);
        }

        [TestMethod()]
        public void TestImplicitHydrogen_Same()
        {
            ILigand ligand1 = new ImplicitHydrogenLigand(molecule, new VisitedAtoms(), molecule.Atoms[1]);
            ILigand ligand2 = new Ligand(molecule, new VisitedAtoms(), molecule.Atoms[1], molecule.Atoms[4]);
            ISequenceSubRule<ILigand> rule = new AtomicNumberRule();
            Assert.AreEqual(0, rule.Compare(ligand1, ligand2));
            Assert.AreEqual(0, rule.Compare(ligand2, ligand1));
        }

        [TestMethod()]
        public void TestImplicitHydrogen()
        {
            ILigand ligand1 = new ImplicitHydrogenLigand(molecule, new VisitedAtoms(), molecule.Atoms[1]);
            ILigand ligand2 = new Ligand(molecule, new VisitedAtoms(), molecule.Atoms[1], molecule.Atoms[3]);
            ISequenceSubRule<ILigand> rule = new AtomicNumberRule();
            Assert.AreEqual(-1, rule.Compare(ligand1, ligand2));
            Assert.AreEqual(1, rule.Compare(ligand2, ligand1));
        }
    }
}



