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
 * MERCHANTABILITY or FITNESS FOR sourceAtom PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Isomorphisms.Matchers;
using NCDK.SMSD.Filters;
using NCDK.SMSD.Helper;
using NCDK.SMSD.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NCDK.SMSD.Algorithms.MCSPluses
{
    /// <summary>
    /// This class acts as a handler class for MCSPlus algorithm.
    /// </summary>
    /// <seealso cref="MCSPlus"/>
    // @cdk.module smsd
    // @cdk.githash
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    [Obsolete("SMSD has been deprecated from the CDK with a newer, more recent version of SMSD is available at http://github.com/asad/smsd . ")]
    public class MCSPlusHandler : AbstractMCSAlgorithm, IMCSBase
    {
        private static List<IReadOnlyDictionary<IAtom, IAtom>> allAtomMCS = null;
        private static Dictionary<IAtom, IAtom> atomsMCS = null;
        private static SortedDictionary<int, int> firstMCS = null;
        private static List<IReadOnlyDictionary<int, int>> allMCS = null;
        private IAtomContainer source = null;
        private IAtomContainer target = null;
        private bool flagExchange = false;

        /// <summary>
        /// Constructor for the MCS Plus algorithm class
        /// </summary>
        public MCSPlusHandler()
        {
            allAtomMCS = new List<IReadOnlyDictionary<IAtom, IAtom>>();
            atomsMCS = new Dictionary<IAtom, IAtom>();
            firstMCS = new SortedDictionary<int, int>();
            allMCS = new List<IReadOnlyDictionary<int, int>>();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Set(MolHandler source, MolHandler target)
        {
            this.source = source.Molecule;
            this.target = target.Molecule;
        }

        public void Set(IQueryAtomContainer source, IAtomContainer target)
        {
            this.source = source;
            this.target = target;
        }

        /// <summary>
        /// Function is called by the main program and serves as a starting point for the comparison procedure.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void SearchMCS(bool shouldMatchBonds)
        {
            IList<IReadOnlyList<int>> mappings = null;
            try
            {
                if (source.Atoms.Count >= target.Atoms.Count)
                {
                    mappings = MCSPlus.GetOverlaps(source, target, shouldMatchBonds);
                }
                else
                {
                    flagExchange = true;
                    mappings = MCSPlus.GetOverlaps(target, source, shouldMatchBonds);
                }
                PostFilter.Filter(mappings);
                SetAllMapping();
                SetAllAtomMapping();
                SetFirstMapping();
                SetFirstAtomMapping();
            }
            catch (CDKException)
            {
                mappings = null;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void SetAllMapping()
        {
            try
            {
                var finalSolution = FinalMappings.Instance.GetFinalMapping();
                int counter = 0;
                foreach (var solution in finalSolution)
                {
                    var validSolution = new SortedDictionary<int, int>();
                    if (!flagExchange)
                    {
                        foreach (var map in solution)
                        {
                            validSolution[map.Key] = map.Value;
                        }
                    }
                    else
                    {
                        foreach (var map in solution)
                        {
                            validSolution[map.Value] = map.Key;
                        }
                    }
                    allMCS.Insert(counter++, validSolution);
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.StackTrace);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void SetAllAtomMapping()
        {
            try
            {
                int counter = 0;
                foreach (var solution in allMCS)
                {
                    var atomMappings = new Dictionary<IAtom, IAtom>();
                    foreach (var map in solution)
                    {
                        int iIndex = map.Key;
                        int jIndex = map.Value;

                        IAtom sourceAtom = null;
                        IAtom targetAtom = null;

                        sourceAtom = source.Atoms[iIndex];
                        targetAtom = target.Atoms[jIndex];
                        atomMappings[sourceAtom] = targetAtom;
                    }
                    allAtomMCS.Insert(counter++, atomMappings);
                }
            }
            catch (Exception)
            {
                //I.GetCause();
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void SetFirstMapping()
        {
            if (allMCS.Count != 0)
            {
                firstMCS = new SortedDictionary<int, int>();
                foreach (var e in allMCS.First())
                    firstMCS.Add(e.Key, e.Value);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void SetFirstAtomMapping()
        {
            if (allAtomMCS.Count != 0)
            {
                atomsMCS = new Dictionary<IAtom, IAtom>();
                foreach (var e in allAtomMCS.First())
                    atomsMCS.Add(e.Key, e.Value);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IReadOnlyList<IReadOnlyDictionary<int, int>> GetAllMapping()
        {
            return allMCS;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IReadOnlyDictionary<int, int> GetFirstMapping()
        {
            return firstMCS;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IReadOnlyList<IReadOnlyDictionary<IAtom, IAtom>> GetAllAtomMapping()
        {
            return allAtomMCS;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IReadOnlyDictionary<IAtom, IAtom> GetFirstAtomMapping()
        {
            return atomsMCS;
        }
    }
}
