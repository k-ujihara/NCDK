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
    public class MassNumberRuleTest : CDKTestCase
    {
        static SmilesParser smiles = CDK.SilentSmilesParser;
        static IAtomContainer molecule = smiles.ParseSmiles("CC([13C])([2H])[H]");

        [TestMethod()]
        public void TestCompareIdentity()
        {
            var ligand = new Ligand(molecule, new VisitedAtoms(), molecule.Atoms[1], molecule.Atoms[0]);
            var rule = new MassNumberRule();
            Assert.AreEqual(0, rule.Compare(ligand, ligand));
        }

        [TestMethod()]
        public void TestCompare()
        {
            var ligand1 = new Ligand(molecule, new VisitedAtoms(), molecule.Atoms[1], molecule.Atoms[0]);
            var ligand2 = new Ligand(molecule, new VisitedAtoms(), molecule.Atoms[1], molecule.Atoms[2]);
            var rule = new MassNumberRule();
            Assert.AreEqual(-1, rule.Compare(ligand1, ligand2));
            Assert.AreEqual(1, rule.Compare(ligand2, ligand1));
        }

        [TestMethod()]
        public void TestOrder()
        {
            var visitedAtoms = new VisitedAtoms();
            var ligand1 = new Ligand(molecule, visitedAtoms, molecule.Atoms[1], molecule.Atoms[4]);
            var ligand2 = new Ligand(molecule, visitedAtoms, molecule.Atoms[1], molecule.Atoms[3]);
            var ligand3 = new Ligand(molecule, visitedAtoms, molecule.Atoms[1], molecule.Atoms[2]);
            var ligand4 = new Ligand(molecule, visitedAtoms, molecule.Atoms[1], molecule.Atoms[0]);
            var ligands = new List<ILigand>
            {
                ligand1,
                ligand2,
                ligand3,
                ligand4
            };

            ligands.Sort(new MassNumberRule());
            Assert.IsNotNull(ligands[0]);
            Assert.AreEqual("H", ligands[0].LigandAtom.Symbol);
            Assert.AreEqual("H", ligands[1].LigandAtom.Symbol);
            Assert.AreEqual(2, ligands[1].LigandAtom.MassNumber.Value);
            Assert.AreEqual("C", ligands[2].LigandAtom.Symbol);
            Assert.AreEqual("C", ligands[3].LigandAtom.Symbol);
            Assert.AreEqual(13, ligands[3].LigandAtom.MassNumber.Value);
        }

        [TestMethod()]
        public void TestImplicitHydrogenSame()
        {
            var ligand1 = new ImplicitHydrogenLigand(molecule, new VisitedAtoms(), molecule.Atoms[1]);
            var ligand2 = new Ligand(molecule, new VisitedAtoms(), molecule.Atoms[1], molecule.Atoms[4]);
            var rule = new MassNumberRule();
            Assert.AreEqual(0, rule.Compare(ligand1, ligand2));
            Assert.AreEqual(0, rule.Compare(ligand2, ligand1));
        }

        [TestMethod()]
        public void TestImplicitHydrogen()
        {
            var ligand1 = new ImplicitHydrogenLigand(molecule, new VisitedAtoms(), molecule.Atoms[1]);
            var ligand2 = new Ligand(molecule, new VisitedAtoms(), molecule.Atoms[1], molecule.Atoms[3]);
            var rule = new MassNumberRule();
            Assert.AreEqual(-1, rule.Compare(ligand1, ligand2));
            Assert.AreEqual(1, rule.Compare(ligand2, ligand1));
        }
    }
}
