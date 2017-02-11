/* Copyright (C) 2006-2010  Syed Asad Rahman <asad@ebi.ac.uk>
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
using NCDK.Isomorphisms.Matchers;
using NCDK.SMSD.Tools;
using System;
using System.Collections.Generic;

namespace NCDK.SMSD.Algorithms.Single
{
    /**
     * This class handles single atom mapping.
     * Either query and/or target molecule with single atom is mapped by this class.
     * @cdk.module smsd
     * @cdk.githash
     * @author Syed Asad Rahman <asad@ebi.ac.uk>
     */
    public class SingleMapping
    {
        private IAtomContainer source = null;
        private IAtomContainer target = null;
        private List<IDictionary<IAtom, IAtom>> mappings = null;
        private IDictionary<int, double> connectedBondOrder = null;

        /**
         * Default
         */
        public SingleMapping() { }

        /**
         * Returns single mapping solutions.
         * @param source
         * @param target
         * @param removeHydrogen
         * @return Mappings
         * @throws CDKException
         */
#if TEST
		public
#else
        protected 
#endif
		IList<IDictionary<IAtom, IAtom>> GetOverLaps(IAtomContainer source, IAtomContainer target, bool removeHydrogen)
        {
            mappings = new List<IDictionary<IAtom, IAtom>>();
            connectedBondOrder = new SortedDictionary<int, double>();
            this.source = source;
            this.target = target;

            if (source.Atoms.Count == 1 || (source.Atoms.Count > 0 && source.Bonds.Count == 0))
            {
                SetSourceSingleAtomMap(removeHydrogen);
            }
            if (target.Atoms.Count == 1 || (target.Atoms.Count > 0 && target.Bonds.Count == 0))
            {
                SetTargetSingleAtomMap(removeHydrogen);
            }

            PostFilter();
            return mappings;
        }

        /**
         * Returns single mapping solutions.
         * @param source
         * @param target
         * @param removeHydrogen
         * @return Mappings
         * @throws CDKException
         */
#if TEST
        public
#else
        protected 
#endif
		IList<IDictionary<IAtom, IAtom>> GetOverLaps(IQueryAtomContainer source, IAtomContainer target,
                bool removeHydrogen)
        {
            mappings = new List<IDictionary<IAtom, IAtom>>();
            connectedBondOrder = new SortedDictionary<int, double>();
            this.source = source;
            this.target = target;

            if (source.Atoms.Count == 1 || (source.Atoms.Count > 0 && source.Bonds.Count == 0))
            {
                SetSourceSingleAtomMap(source, removeHydrogen);
            }
            if (target.Atoms.Count == 1 || (target.Atoms.Count > 0 && target.Bonds.Count == 0))
            {
                SetTargetSingleAtomMap(removeHydrogen);
            }

            PostFilter();
            return mappings;
        }

        private void SetSourceSingleAtomMap(IQueryAtomContainer source, bool removeHydrogen)
        {
            int counter = 0;
            BondEnergies be = BondEnergies.Instance;
            foreach (var sourceAtom in source.Atoms)
            {
                IQueryAtom smartAtom = (IQueryAtom)sourceAtom;
                if ((removeHydrogen && !smartAtom.Symbol.Equals("H")) || (!removeHydrogen))
                {
                    foreach (var targetAtom in target.Atoms)
                    {
                        IDictionary<IAtom, IAtom> mapAtoms = new Dictionary<IAtom, IAtom>();
                        if (smartAtom.Matches(targetAtom))
                        {
                            mapAtoms[sourceAtom] = targetAtom;
                            var bonds = target.GetConnectedBonds(targetAtom);

                            double totalOrder = 0;
                            foreach (var bond in bonds)
                            {
                                BondOrder order = bond.Order;
                                totalOrder += order.Numeric + be.GetEnergies(bond);
                            }
                            if (targetAtom.FormalCharge != sourceAtom.FormalCharge)
                            {
                                totalOrder += 0.5;
                            }
                            connectedBondOrder[counter] = totalOrder;
                            mappings.Insert(counter++, mapAtoms);
                        }
                    }
                }
                else
                {
                    Console.Error.WriteLine("Skippping Hydrogen mapping or This is not a single mapping case!");
                }
            }
        }

        private void SetSourceSingleAtomMap(bool removeHydrogen)
        {
            int counter = 0;
            BondEnergies be = BondEnergies.Instance;
            foreach (var sourceAtom in source.Atoms)
            {
                if ((removeHydrogen && !sourceAtom.Symbol.Equals("H")) || (!removeHydrogen))
                {
                    foreach (var targetAtom in target.Atoms)
                    {
                        IDictionary<IAtom, IAtom> mapAtoms = new Dictionary<IAtom, IAtom>();
                        if (string.Equals(sourceAtom.Symbol, targetAtom.Symbol, StringComparison.OrdinalIgnoreCase))
                        {
                            mapAtoms[sourceAtom] = targetAtom;
                            var bonds = target.GetConnectedBonds(targetAtom);

                            double totalOrder = 0;
                            foreach (var bond in bonds)
                            {
                                BondOrder order = bond.Order;
                                totalOrder += order.Numeric + be.GetEnergies(bond);
                            }
                            if (targetAtom.FormalCharge != sourceAtom.FormalCharge)
                            {
                                totalOrder += 0.5;
                            }
                            connectedBondOrder[counter] = totalOrder;
                            mappings.Insert(counter++, mapAtoms);
                        }
                    }
                }
                else
                {
                    Console.Error.WriteLine("Skippping Hydrogen mapping or This is not a single mapping case!");
                }
            }
        }

        private void SetTargetSingleAtomMap(bool removeHydrogen)
        {
            int counter = 0;
            BondEnergies be = BondEnergies.Instance;
            foreach (var targetAtom in target.Atoms)
            {
                if ((removeHydrogen && !targetAtom.Symbol.Equals("H")) || (!removeHydrogen))
                {
                    foreach (var sourceAtoms in source.Atoms)
                    {
                        IDictionary<IAtom, IAtom> mapAtoms = new Dictionary<IAtom, IAtom>();

                        if (string.Equals(targetAtom.Symbol, sourceAtoms.Symbol, StringComparison.OrdinalIgnoreCase))
                        {
                            mapAtoms[sourceAtoms] = targetAtom;
                            var bonds = source.GetConnectedBonds(sourceAtoms);

                            double totalOrder = 0;
                            foreach (var bond in bonds)
                            {
                                BondOrder order = bond.Order;
                                totalOrder += order.Numeric + be.GetEnergies(bond);
                            }
                            if (sourceAtoms.FormalCharge != targetAtom.FormalCharge)
                            {
                                totalOrder += 0.5;
                            }
                            connectedBondOrder[counter] = totalOrder;
                            mappings.Insert(counter++, mapAtoms);
                        }
                    }
                }
                else
                {
                    Console.Error.WriteLine("Skippping Hydrogen mapping or This is not a single mapping case!");
                }
            }
        }

        private void PostFilter()
        {
            List<IDictionary<IAtom, IAtom>> sortedMap = new List<IDictionary<IAtom, IAtom>>();
            foreach (var entry in SortByValue(connectedBondOrder))
            {
                IDictionary<IAtom, IAtom> mapToBeMoved = mappings[entry.Key];
                sortedMap.Add(mapToBeMoved);
            }
            mappings = sortedMap;
        }

        private IList<KeyValuePair<int, double>> SortByValue(IDictionary<int, double> map) 
        {
            var list = new List<KeyValuePair<int, double>>();
            foreach (var entry in map)
                list.Add(entry);
            list.Sort(delegate(KeyValuePair<int, double> x, KeyValuePair<int, double> y)
            {
                return x.Value.CompareTo(y.Value);
            });
            return list;
        }
    }
}

