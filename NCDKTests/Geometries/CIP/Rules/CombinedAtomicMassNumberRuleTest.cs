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
    public class CombinedAtomicMassNumberRuleTest : CDKTestCase
    {

        static SmilesParser smiles = new SmilesParser(Silent.ChemObjectBuilder.Instance);
        static IAtomContainer molecule = smiles.ParseSmiles("CC(Br)([13C])[H]");

        [TestMethod()]
        public void TestCompare_Identity()
        {
            ILigand ligand = new Ligand(molecule, new VisitedAtoms(), molecule.Atoms[1], molecule.Atoms[0]);
            ISequenceSubRule<ILigand> rule = new CombinedAtomicMassNumberRule();
            Assert.AreEqual(0, rule.Compare(ligand, ligand));
        }

        [TestMethod()]
        public void TestCompare()
        {
            ILigand ligand1 = new Ligand(molecule, new VisitedAtoms(), molecule.Atoms[1], molecule.Atoms[0]);
            ILigand ligand2 = new Ligand(molecule, new VisitedAtoms(), molecule.Atoms[1], molecule.Atoms[2]);
            ISequenceSubRule<ILigand> rule = new CombinedAtomicMassNumberRule();
            Assert.AreEqual(-1, rule.Compare(ligand1, ligand2));
            Assert.AreEqual(1, rule.Compare(ligand2, ligand1));
        }

        [TestMethod()]
        public void TestOrder()
        {
            ILigand ligand1 = new Ligand(molecule, new VisitedAtoms(), molecule.Atoms[1], molecule.Atoms[4]);
            ILigand ligand2 = new Ligand(molecule, new VisitedAtoms(), molecule.Atoms[1], molecule.Atoms[3]);
            ILigand ligand3 = new Ligand(molecule, new VisitedAtoms(), molecule.Atoms[1], molecule.Atoms[2]);
            ILigand ligand4 = new Ligand(molecule, new VisitedAtoms(), molecule.Atoms[1], molecule.Atoms[0]);
            List<ILigand> ligands = new List<ILigand>();
            ligands.Add(ligand1);
            ligands.Add(ligand2);
            ligands.Add(ligand3);
            ligands.Add(ligand4);

            ligands.Sort(new CombinedAtomicMassNumberRule());
            Assert.AreEqual("H", ligands[0].GetLigandAtom().Symbol);
            Assert.AreEqual("C", ligands[1].GetLigandAtom().Symbol);
            Assert.AreEqual("C", ligands[2].GetLigandAtom().Symbol);
            Assert.AreEqual(13, ligands[2].GetLigandAtom().MassNumber.Value);
            Assert.AreEqual("Br", ligands[3].GetLigandAtom().Symbol);
        }
    }
}
