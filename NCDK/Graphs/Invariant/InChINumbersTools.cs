/* Copyright (C) 2011  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.Common.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using NCDK.NInChI;
using NCDK.Graphs.InChi;

namespace NCDK.Graphs.Invariant
{
    /**
     * Tool for calculating atom numbers using the InChI algorithm.
     *
     * @cdk.module  inchi
     * @cdk.githash
     */
    public class InChINumbersTools
    {
        /**
		 * Makes an array containing the InChI atom numbers of the non-hydrogen
		 * atoms in the atomContainer. It returns zero for all hydrogens.
		 *
		 * @param  atomContainer  The <see cref="IAtomContainer"/> to analyze.
		 * @return                The number from 1 to the number of heavy atoms.
		 * @   When the InChI could not be generated
		 */
        public static long[] GetNumbers(IAtomContainer atomContainer) {
            string aux = AuxInfo(atomContainer);
            aux = aux.Substring(aux.IndexOf("/N:") + 3);
            string numberStringAux = aux.Substring(0, aux.IndexOf('/'));
            int i = 1;
            long[] numbers = new long[atomContainer.Atoms.Count];
            foreach (var numberString in numberStringAux.Split(','))
                numbers[int.Parse(numberString) - 1] = i++;
            return numbers;
        }

        /**
		 * Obtain the InChI numbers for the input container to be used to order
		 * atoms in Universal SMILES {@cdk.cite OBoyle12}. The numbers are obtained
		 * using the fixedH and RecMet options of the InChI. All non-bridged
		 * hydrogens are labelled as 0.
		 *
		 * @param container the structure to obtain the numbers of
		 * @return the atom numbers
		 * @
		 */
        public static long[] GetUSmilesNumbers(IAtomContainer container)
        {
            string aux = AuxInfo(container, INCHI_OPTION.RecMet, INCHI_OPTION.FixedH);
            return ParseUSmilesNumbers(aux, container);
        }

        /**
		 * Parse the InChI canonical atom numbers (from the AuxInfo) to use in
		 * Universal SMILES.
		 *
		 * The parsing follows: "Rule A: The correspondence between the input atom
		 * order and the InChI canonical labels should be obtained from the
		 * reconnected metal layer (/R:) in preference to the initial layer, and
		 * then from the fixed hydrogen labels (/F:) in preference to the standard
		 * labels (/N:)." <p/>
		 *
		 * The labels are also adjust for "Rule E: If the start atom is a negatively
		 * charged oxygen atom, start instead at any carbonyl oxygen attached to the
		 * same neighbour." <p/>
		 *
		 * All unlabelled atoms (e.g. hydrogens) are assigned the same label which
		 * is different but larger then all other labels. The hydrogen
		 * labelling then needs to be adjusted externally as universal SMILES
		 * suggests hydrogens should be visited first.
		 *
		 * @param aux       inchi AuxInfo
		 * @param container the structure to obtain the numbering of
		 * @return the numbers string to use
		 */
#if TEST
		public
#endif
        static long[] ParseUSmilesNumbers(string aux, IAtomContainer container) {

            int index;
            long[] numbers = new long[container.Atoms.Count];
            int[] first = null;
            int label = 1;

            if ((index = aux.IndexOf("/R:")) >= 0) { // reconnected metal numbers
                int endIndex = aux.IndexOf('/', index + 8);
                if (endIndex < 0)
                    endIndex = aux.Length;
                string[] baseNumbers = aux.Substring(index + 8, endIndex - (index + 8)).Split(';');
                first = new int[baseNumbers.Length];
                Arrays.Fill(first, -1);
                for (int i = 0; i < baseNumbers.Length; i++) {
                    string[] numbering = baseNumbers[i].Split(',');
                    first[i] = int.Parse(numbering[0]) - 1;
                    foreach (string number in numbering) {
                        numbers[int.Parse(number) - 1] = label++;
                    }
                }
            } else if ((index = aux.IndexOf("/N:")) >= 0) { // standard numbers

                // read the standard numbers first (need to reference back for some structures)
                string[] baseNumbers = aux.Substring(index + 3, aux.IndexOf('/', index + 3) - (index + 3)).Split(';');
                first = new int[baseNumbers.Length];
                Arrays.Fill(first, -1);

                if ((index = aux.IndexOf("/F:")) >= 0) {
                    string[] fixedHNumbers = aux.Substring(index + 3, aux.IndexOf('/', index + 3)-(index + 3)).Split(';');
                    for (int i = 0; i < fixedHNumbers.Length; i++) {

                        string component = fixedHNumbers[i];

                        // m, 2m, 3m ... need to lookup number in the base numbering
                        if (component[component.Length - 1] == 'm') {
                            int n = component.Length > 1 ? int
                                    .Parse(component.Substring(0, component.Length - 1)) : 1;
                            for (int j = 0; j < n; j++) {
                                string[] numbering = baseNumbers[i + j].Split(',');
                                first[i + j] = int.Parse(numbering[0]) - 1;
                                foreach (var number in numbering)
                                    numbers[int.Parse(number) - 1] = label++;
                            }
                        } else {
                            string[] numbering = component.Split(',');
                            first[i] = int.Parse(numbering[0]) - 1;
                            foreach (var number in numbering)
                                numbers[int.Parse(number) - 1] = label++;
                        }
                    }
                } else {
                    for (int i = 0; i < baseNumbers.Length; i++) {
                        string[] numbering = baseNumbers[i].Split(',');
                        first[i] = int.Parse(numbering[0]) - 1;
                        foreach (var number in numbering)
                            numbers[int.Parse(number) - 1] = label++;
                    }
                }
            } else {
                throw new ArgumentException("AuxInfo did not contain extractable base numbers (/N: or /R:).");
            }

            // Rule E: swap any oxygen anion for a double bonded oxygen (InChI sees
            // them as equivalent)
            foreach (var v in first) {
                if (v >= 0) {
                    IAtom atom = container.Atoms[v];
                    if (atom.FormalCharge == null) continue;
                    if (atom.AtomicNumber == 8 && atom.FormalCharge == -1) {
                        var neighbors = container.GetConnectedAtoms(atom);
                        if (neighbors.Count() == 1) {
                            IAtom correctedStart = FindPiBondedOxygen(container, neighbors.First());
                            if (correctedStart != null) Exch(numbers, v, container.Atoms.IndexOf(correctedStart));
                        }
                    }
                }
            }

            // assign unlabelled atoms
            for (int i = 0; i < numbers.Length; i++)
                if (numbers[i] == 0) numbers[i] = label++;

            return numbers;
        }

        /**
		 * Exchange the elements at index i with that at index j.
		 *
		 * @param values an array of values
		 * @param i an index
		 * @param j another index
		 */
        private static void Exch(long[] values, int i, int j) {
            long k = values[i];
            values[i] = values[j];
            values[j] = k;
        }

        /**
		 * Find a neutral oxygen bonded to the {@code atom} with a pi bond.
		 *
		 * @param container the container
		 * @param atom      an atom from the container
		 * @return a pi bonded oxygen (or null if not found)
		 */
        private static IAtom FindPiBondedOxygen(IAtomContainer container, IAtom atom) {
            foreach (var bond in container.GetConnectedBonds(atom)) {
                if (bond.Order == BondOrder.Double) {
                    IAtom neighbor = bond.GetConnectedAtom(atom);
                    int charge = neighbor.FormalCharge ?? 0;
                    if (neighbor.AtomicNumber == 8 && charge == 0) return neighbor;
                }
            }
            return null;
        }

        /**
		 * Obtain the InChI auxiliary info for the provided structure using
		 * using the specified InChI options.
		 *
		 * @param  container the structure to obtain the numbers of
		 * @return auxiliary info
		 * @ the inchi could not be generated
		 */
        public static string AuxInfo(IAtomContainer container, params INCHI_OPTION[] options) {
            InChIGeneratorFactory factory = InChIGeneratorFactory.Instance;
            bool org = factory.IgnoreAromaticBonds;
            factory.IgnoreAromaticBonds = true;
            InChIGenerator gen = factory.GetInChIGenerator(container, new List<INCHI_OPTION>(options));
            factory.IgnoreAromaticBonds = org; // an option on the singleton so we should reset for others
            if (gen.ReturnStatus != INCHI_RET.OKAY && gen.ReturnStatus != INCHI_RET.WARNING)
                throw new CDKException("Could not generate InChI Numbers: " + gen.Message);
            return gen.AuxInfo;
        }
    }
}
