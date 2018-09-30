/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
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

using NCDK.Silent;
using NCDK.Layout;
using NCDK.Numerics;
using NCDK.Templates;
using NCDK.Tools.Manipulator;
using System;

namespace NCDK.StructGen
{
    // @cdk.module test-structgen
    public class SingleStructureRandomGeneratorTest
    {
        string mf;
        SingleStructureRandomGenerator ssrg;

        public SingleStructureRandomGeneratorTest()
        {
            Console.Out.WriteLine("Instantiating MoleculeListViewer");
            Console.Out.WriteLine("Instantiating SingleStructureRandomGenerator");
            ssrg = new SingleStructureRandomGenerator();
            Console.Out.WriteLine("Assining unbonded set of atoms");
            AtomContainer ac = GetBunchOfUnbondedAtoms();
            mf = MolecularFormulaManipulator.GetString(MolecularFormulaManipulator.GetMolecularFormula(ac));
            Console.Out.WriteLine("Molecular Formula is: " + mf);
            ssrg.SetAtomContainer(ac);
        }

        private bool ShowIt(IAtomContainer molecule, string name)
        {
            StructureDiagramGenerator sdg = new StructureDiagramGenerator { Molecule = (IAtomContainer)molecule.Clone() };
            sdg.GenerateCoordinates(new Vector2(0, 1));
            return true;
        }

        private AtomContainer GetBunchOfUnbondedAtoms()
        {
            IAtomContainer molecule = TestMoleculeFactory.MakeAlphaPinene();
            FixCarbonHCount(molecule);
            molecule.RemoveAllElectronContainers();
            return (AtomContainer)molecule;
        }

        private void FixCarbonHCount(IAtomContainer mol)
        {
            // the following line are just a quick fix for this particluar
            // carbon-only molecule until we have a proper hydrogen count
            // configurator
            double bondCount = 0;
            IAtom atom;
            for (int f = 0; f < mol.Atoms.Count; f++)
            {
                atom = mol.Atoms[f];
                bondCount = mol.GetBondOrderSum(atom);
                if (bondCount > 4) Console.Out.WriteLine("bondCount: " + bondCount);
                atom.ImplicitHydrogenCount = 4 - (int)bondCount - (int)(atom.Charge ?? 0);
            }
        }

        //class MoreAction : AbstractAction {
        //    
        //    public void ActionPerformed(ActionEvent e) {
        //        try {
        //            IAtomContainer ac = ssrg.Generate();
        //            ShowIt(ac, "Randomly generated for " + mf);
        //        } catch (Exception ex) {
        //            Console.Error.WriteLine(ex.Message);
        //        }
        //    }
        //}
    }
}


