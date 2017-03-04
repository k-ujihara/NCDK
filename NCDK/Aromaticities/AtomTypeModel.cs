/*
 * Copyright (c) 2013 European Bioinformatics Institute (EMBL-EBI)
 *                    John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 2.1 of the License, or (at
 * your option) any later version. All we ask is that proper credit is given
 * for our work, which includes - but is not limited to - adding the above
 * copyright notice to the beginning of your source code files, and to any
 * copyright notice that you may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * Any WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using NCDK.Common.Collections;
using NCDK.Graphs;
using NCDK.RingSearches;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using static NCDK.Graphs.GraphUtil;
using static NCDK.Common.Base.Preconditions;
using NCDK.Config;

namespace NCDK.Aromaticities
{
    /// <summary>
    /// Electron donation model using the CDK atom types. One can choose to allow
    /// contribution from exocyclic pi bonds in the constructor. Allowing exocyclic
    /// pi bonds results in molecules such as hexamethylidenecyclohexane (
    /// "C=C1C(=C)C(=C)C(=C)C(=C)C1=C") being considered aromatic.
    /// </summary>
    // @author John May
    // @cdk.module standard
    /// mores tests in - org.openscience.cdk.aromaticity.ExocyclicAtomTypeModelTest
    sealed class AtomTypeModel : ElectronDonation
    {
        /// <summary>Predefined electron contribution for several atom types.</summary>
        private readonly static Dictionary<string, int> TYPES =
            new Dictionary<string, int>()
            {
                {"N.planar3", 2},
                {"N.minus.planar3", 2},
                {"N.amide", 2},
                {"S.2", 2},
                {"S.planar3", 2},
                {"C.minus.planar", 2},
                {"O.planar3", 2},
                {"N.sp2.3", 1},
                {"C.sp2", 1},
            };

        /// <summary>Allow exocyclic pi bonds.</summary>
        private readonly bool exocyclic;

        /// <summary>
        /// Create the electron donation model specifying whether exocyclic pi bonds
        /// are allowed. Exocyclic pi bonds <i>sprout</i> from a ring, allowing these
        /// bonds to contribute means structure such as hexamethylidenecyclohexane,
        /// "C=C1C(=C)C(=C)C(=C)C(=C)C1=C" are considered <i>aromatic</i>.
        /// </summary>
        /// <param name="exocyclic">allow exocyclic double bonds</param>
        public AtomTypeModel(bool exocyclic)
        {
            this.exocyclic = exocyclic;
        }

        /// <inheritdoc/>
        public override int[] Contribution(IAtomContainer container, RingSearch ringSearch)
        {
            int nAtoms = container.Atoms.Count;
            int[] electrons = new int[nAtoms];

            Arrays.Fill(electrons, -1);

            IDictionary<IAtom, int> indexMap = new Dictionary<IAtom, int>();

            for (int i = 0; i < nAtoms; i++)
            {

                IAtom atom = container.Atoms[i];
                indexMap.Add(atom, i);

                // acyclic atom skipped
                if (!ringSearch.Cyclic(i)) continue;

                Hybridization hyb = atom.Hybridization;

                CheckNotNull(atom.AtomTypeName, "atom has unset atom type");

                // atom has been assigned an atom type but we don't know the hybrid state,
                // typically for atom type 'X' (unknown)
                switch (hyb.Ordinal)
                {
                    case Hybridization.O.SP2:
                    case Hybridization.O.Planar3:
                        electrons[i] = ElectronsForAtomType(atom);
                        break;
                    case Hybridization.O.SP3:
                        electrons[i] = LonePairCount(atom) > 0 ? 2 : -1;
                        break;
                }
            }

            // exocyclic double bonds are allowed no further processing
            if (exocyclic) return electrons;

            // check for exocyclic double/triple bonds and disallow their contribution
            foreach (var bond in container.Bonds)
            {
                if (bond.Order == BondOrder.Double || bond.Order == BondOrder.Triple)
                {
                    IAtom a1 = bond.Atoms[0];
                    IAtom a2 = bond.Atoms[1];

                    string a1Type = a1.AtomTypeName;
                    string a2Type = a2.AtomTypeName;

                    int u = indexMap[a1];
                    int v = indexMap[a2];

                    if (!ringSearch.Cyclic(u, v))
                    {

                        // XXX: single exception - we could make this more general but
                        // for now this mirrors the existing behavior
                        if (a1Type.Equals("N.sp2.3") && a2Type.Equals("O.sp2") || a1Type.Equals("O.sp2")
                                && a2Type.Equals("N.sp2.3")) continue;

                        electrons[u] = electrons[v] = -1;
                    }
                }
            }

            return electrons;
        }

        /// <summary>
        /// The number of contributed electrons for the atom type of the specified atom type.
        /// </summary>
        /// <param name="atom">an atom to get the contribution of</param>
        /// <returns>the number of electrons</returns>
        private static int ElectronsForAtomType(IAtom atom)
        {
            int electrons;
            if (TYPES.TryGetValue(atom.AtomTypeName, out electrons))
                return electrons;

            try
            {
                IAtomType atomType = AtomTypeFactory.GetInstance("NCDK.Dict.Data.cdk-atom-types.owl",
                        atom.Builder).GetAtomType(atom.AtomTypeName);
                var propPiBondCount = atomType.GetProperty<int>(CDKPropertyName.PI_BOND_COUNT, 0);
                return propPiBondCount;
            }
            catch (NoSuchAtomTypeException e)
            {
                throw new ArgumentException(e.Message, e);
            }
        }

        /// <summary>
        /// Access to the number of lone-pairs (specified as a property of the atom).
        /// </summary>
        /// <param name="atom">the atom to get the lone pairs from</param>
        /// <returns>number of lone pairs</returns>
        private static int LonePairCount(IAtom atom)
        {
            // XXX: LONE_PAIR_COUNT is not currently set!
            return atom.GetProperty<int>(CDKPropertyName.LONE_PAIR_COUNT, -1);
        }
    }
}
