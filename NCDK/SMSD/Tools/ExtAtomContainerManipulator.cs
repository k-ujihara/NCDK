/*
 *
 * Copyright (C) 2006-2010  Syed Asad Rahman {asad@ebi.ac.uk}
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
 * You should have received atom copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using NCDK.Aromaticities;
using NCDK.AtomTypes;
using NCDK.Config;
using NCDK.RingSearches;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Linq;
using static NCDK.Tools.Manipulator.AtomContainerManipulator;

namespace NCDK.SMSD.Tools
{
    /// <summary>
    /// Class that handles some customised features for SMSD atom containers.
    /// <para>This is an extension of CDK AtomContainer.
    /// Some part of this code was taken from CDK source code and modified.</para>
    /// </summary>
    // @cdk.module smsd
    // @cdk.githash
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    public static class ExtAtomContainerManipulator
    {
        private static void PrintAtoms(IAtomContainer mol)
        {
            Console.Out.Write("Atom: ");
            foreach (var a in mol.Atoms)
            {

                Console.Out.Write(a.Symbol);
                Console.Out.Write("[" + a.FormalCharge + "]");
                if (a.Id != null)
                {
                    Console.Out.Write("[" + a.Id + "]");
                }

            }
            Console.Out.WriteLine();
            Console.Out.WriteLine();
        }

        /// <summary>
        /// Retrurns deep copy of the molecule
        /// </summary>
        /// <param name="container"></param>
        /// <returns>deep copy of the mol</returns>
        public static IAtomContainer MakeDeepCopy(IAtomContainer container)
        {
            IAtomContainer newAtomContainer = (IAtomContainer)container.Clone();
            newAtomContainer.NotifyChanged();
            return newAtomContainer;
        }

        /// <summary>
        /// This function finds rings and uses aromaticity detection code to
        /// aromatize the molecule.
        /// </summary>
        /// <param name="mol">input molecule</param>
        public static void AromatizeMolecule(IAtomContainer mol)
        {
            // need to find rings and aromaticity again since added H's

            IRingSet ringSet = null;
            try
            {
                AllRingsFinder arf = new AllRingsFinder();
                ringSet = arf.FindAllRings(mol);

                // SSSRFinder s = new SSSRFinder(atomContainer);
                // srs = s.FindEssentialRings();
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.StackTrace);
            }

            try
            {
                // figure out which atoms are in aromatic rings:
                //            PrintAtoms(atomContainer);
                ExtAtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
                //            PrintAtoms(atomContainer);
                Aromaticity.CDKLegacy.Apply(mol);
                //            PrintAtoms(atomContainer);
                // figure out which rings are aromatic:
                RingSetManipulator.MarkAromaticRings(ringSet);
                //            PrintAtoms(atomContainer);
                // figure out which simple (non cycles) rings are aromatic:
                // HueckelAromaticityDetector.DetectAromaticity(atomContainer, srs);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.StackTrace);
            }

            // only atoms in 6 membered rings are aromatic
            // determine largest ring that each atom is atom part of

            for (int i = 0; i <= mol.Atoms.Count - 1; i++)
            {

                mol.Atoms[i].IsAromatic = false;

                foreach (var ring in ringSet)
                {
                    if (!ring.IsAromatic)
                    {
                        continue;
                    }

                    bool haveatom = ring.Contains(mol.Atoms[i]);

                    //Debug.WriteLine("haveatom="+haveatom);

                    if (haveatom && ring.Atoms.Count == 6)
                    {
                        mol.Atoms[i].IsAromatic = true;
                    }
                }
            }
        }

        /// <summary>
        /// Returns The number of explicit hydrogens for a given IAtom.
        /// </summary>
        /// <param name="atomContainer"></param>
        /// <param name="atom"></param>
        /// <returns>The number of explicit hydrogens on the given IAtom.</returns>
        public static int GetExplicitHydrogenCount(IAtomContainer atomContainer, IAtom atom)
        {
            int hCount = 0;
            foreach (var iAtom in atomContainer.GetConnectedAtoms(atom))
            {
                IAtom connectedAtom = iAtom;
                if (connectedAtom.AtomicNumber.Equals(NaturalElements.H.AtomicNumber))
                {
                    hCount++;
                }
            }
            return hCount;
        }

        /// <summary>
        /// Returns The number of Implicit Hydrogen Count for a given IAtom.
        /// </summary>
        /// <param name="atom"></param>
        /// <returns>Implicit Hydrogen Count</returns>
        public static int GetImplicitHydrogenCount(IAtom atom)
        {
            return atom.ImplicitHydrogenCount ?? 0;
        }

        /// <summary>
        /// The summed implicit + explicit hydrogens of the given IAtom.
        /// </summary>
        /// <param name="atomContainer"></param>
        /// <param name="atom"></param>
        /// <returns>The summed implicit + explicit hydrogens of the given IAtom.</returns>
        public static int GetHydrogenCount(IAtomContainer atomContainer, IAtom atom)
        {
            return GetExplicitHydrogenCount(atomContainer, atom) + GetImplicitHydrogenCount(atom);
        }

        /// <summary>
        /// Returns IAtomContainer without Hydrogen. If an AtomContainer has atom single atom which
        /// is atom Hydrogen then its not removed.
        /// </summary>
        /// <param name="atomContainer"></param>
        /// <returns>IAtomContainer without Hydrogen. If an AtomContainer has atom single atom which is atom Hydrogen then its not removed.</returns>
        public static IAtomContainer RemoveHydrogensExceptSingleAndPreserveAtomID(IAtomContainer atomContainer)
        {
            var map = new CDKObjectMap(); // maps original object to clones.
            if (atomContainer.Bonds.Count > 0)
            {
                var mol = (IAtomContainer)atomContainer.Clone(map);
                List<IAtom> remove = new List<IAtom>(); // lists removed Hs.
                foreach (var atom in atomContainer.Atoms)
                {
                    if (atom.AtomicNumber.Equals(NaturalElements.H.AtomicNumber))
                        remove.Add(atom);
                }
                foreach (var a in remove)
                    mol.RemoveAtomAndConnectedElectronContainers(map.Get(a));
                foreach (var atom in mol.Atoms.Where(n => !n.ImplicitHydrogenCount.HasValue))
                    atom.ImplicitHydrogenCount = 0;
                //            Recompute hydrogen counts of neighbours of removed Hydrogens.
                mol = RecomputeHydrogens(mol, atomContainer, remove, map);
                return mol;
            }
            else
            {
                var mol = (IAtomContainer)atomContainer.Clone(map);
                if (string.Equals(atomContainer.Atoms[0].Symbol, "H", StringComparison.OrdinalIgnoreCase))
                {
                    Console.Error.WriteLine("WARNING: single hydrogen atom removal not supported!");
                }
                return mol;
            }
        }

        /// <summary>
        /// Returns IAtomContainer without Hydrogen. If an AtomContainer has atom single atom which
        /// is atom Hydrogen then its not removed.
        /// </summary>
        /// <param name="atomContainer"></param>
        /// <returns>IAtomContainer without Hydrogen. If an AtomContainer has atom single atom which is atom Hydrogen then its not removed.</returns>
        public static IAtomContainer ConvertExplicitToImplicitHydrogens(IAtomContainer atomContainer)
        {
            IAtomContainer mol = (IAtomContainer)atomContainer.Clone();
            ConvertImplicitToExplicitHydrogens(mol);
            if (mol.Atoms.Count > 1)
            {
                mol = RemoveHydrogens(mol);
            }
            else if (string.Equals(mol.Atoms.First().Symbol, "H", StringComparison.OrdinalIgnoreCase))
            {
                Console.Error.WriteLine("WARNING: single hydrogen atom removal not supported!");
            }
            return mol;
        }

        /// <summary>
        /// Convenience method to perceive atom types for all <see cref="IAtom"/>s in the
        /// <see cref="IAtomContainer"/>, using the <see cref="CDKAtomTypeMatcher"/>. If the
        /// matcher finds atom matching atom type, the <see cref="IAtom"/> will be configured
        /// to have the same properties as the <see cref="IAtomType"/>. If no matching atom
        /// type is found, no configuration is performed.
        /// </summary>
        /// <param name="container"></param>
        /// <exception cref="CDKException"></exception>
        public static void PercieveAtomTypesAndConfigureAtoms(IAtomContainer container)
        {
            var matcher = CDK.AtomTypeMatcher;
            foreach (var atom in container.Atoms)
            {
                if (!(atom is IPseudoAtom))
                {
                    var matched = matcher.FindMatchingAtomType(container, atom);
                    if (matched != null)
                    {
                        AtomTypeManipulator.Configure(atom, matched);
                    }
                }
            }
        }

        private static IAtomContainer RecomputeHydrogens(IAtomContainer mol, IAtomContainer atomContainer, List<IAtom> remove, CDKObjectMap map)
        {
            // Recompute hydrogen counts of neighbours of removed Hydrogens.
            foreach (var aRemove in remove)
            {
                // Process neighbours.
                foreach (var iAtom in atomContainer.GetConnectedAtoms(aRemove))
                {
                    if (!map.TryGetValue(iAtom, out IAtom neighb))
                    {
                        continue; // since for the case of H2, neight H has atom heavy atom neighbor
                    }
                    //Added by Asad
                    if (!(neighb is IPseudoAtom))
                    {
                        neighb.ImplicitHydrogenCount = (neighb.ImplicitHydrogenCount ?? 0) + 1;
                    }
                    else
                    {
                        neighb.ImplicitHydrogenCount = 0;
                    }
                }
            }
            return mol;
        }
    }
}
