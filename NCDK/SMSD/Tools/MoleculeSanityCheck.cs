/*
 *
 * Copyright (C) 2006-2010  Syed Asad Rahman <asad@ebi.ac.uk>
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
using NCDK.Aromaticities;
using NCDK.Graphs;
using NCDK.RingSearches;
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System;

namespace NCDK.SMSD.Tools
{
    /// <summary>
    /// Class that cleans a molecule before MCS search.
    /// </summary>
    // @cdk.module smsd
    // @cdk.githash
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    public class MoleculeSanityCheck
    {
        /// <summary>
        /// Modules for cleaning a molecule
        /// </summary>
        /// <param name="molecule"></param>
        /// <returns>cleaned AtomContainer</returns>
        public static IAtomContainer CheckAndCleanMolecule(IAtomContainer molecule)
        {
            bool isMarkush = false;
            foreach (var atom in molecule.Atoms)
            {
                if (atom.Symbol.Equals("R"))
                {
                    isMarkush = true;
                    break;
                }
            }

            if (isMarkush)
            {
                Console.Error.WriteLine("Skipping Markush structure for sanity check");
            }

            // Check for salts and such
            if (!ConnectivityChecker.IsConnected(molecule))
            {
                // lets see if we have just two parts if so, we assume its a salt and just work
                // on the larger part. Ideally we should have a check to ensure that the smaller
                //  part is a metal/halogen etc.
                var fragments = ConnectivityChecker.PartitionIntoMolecules(molecule);
                if (fragments.Count > 2)
                {
                    Console.Error.WriteLine("More than 2 components. Skipped");
                }
                else
                {
                    IAtomContainer frag1 = fragments[0];
                    IAtomContainer frag2 = fragments[1];
                    if (frag1.Atoms.Count > frag2.Atoms.Count)
                    {
                        molecule = frag1;
                    }
                    else
                    {
                        molecule = frag2;
                    }
                }
            }
            Configure(molecule);
            return molecule;
        }

        /// <summary>
        /// Fixes Aromaticity of the molecule
        /// i.e. need to find rings and aromaticity again since added H's
        /// <param name="mol"></param>
        /// </summary>
        public static void Configure(IAtomContainer mol)
        {
            // need to find rings and aromaticity again since added H's

            IRingSet ringSet = null;
            try
            {
                AllRingsFinder arf = new AllRingsFinder();
                ringSet = arf.FindAllRings(mol);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.StackTrace);
            }

            try
            {
                // figure out which atoms are in aromatic rings:
                CDKHydrogenAdder cdk = CDKHydrogenAdder.GetInstance(Default.ChemObjectBuilder.Instance);
                ExtAtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
                cdk.AddImplicitHydrogens(mol);

                Aromaticity.CDKLegacy.Apply(mol);
                // figure out which rings are aromatic:
                RingSetManipulator.MarkAromaticRings(ringSet);
                // figure out which simple (non cycles) rings are aromatic:

                // only atoms in 6 membered rings are aromatic
                // determine largest ring that each atom is a part of

                foreach (var atom in mol.Atoms)
                {
                    atom.IsAromatic = false;
                    jloop: foreach (var ring in ringSet)
                    {
                        if (!ring.IsAromatic)
                            continue;
                        bool haveatom = ring.Contains(atom);
                        //Debug.WriteLine("haveatom="+haveatom);
                        if (haveatom && ring.Atoms.Count == 6)
                        {
                            atom.IsAromatic = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.StackTrace);
            }
        }
    }
}
