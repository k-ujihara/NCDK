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
using NCDK.SMSD.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NCDK.SMSD.Algorithms.Singles
{
    /// <summary>
    /// This is a handler class for single atom mapping <see cref="SingleMapping"/>.
    /// </summary>
    // @cdk.module smsd
    // @cdk.githash
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    [Obsolete("This class is part of SMSD and either duplicates functionality elsewhere in the CDK or provides public access to internal implementation details. SMSD has been deprecated from the CDK and a newer, more recent version of SMSD is available at http://github.com/asad/smsd .")]
    public class SingleMappingHandler : AbstractMCSAlgorithm, IMCSBase
    {
        private static List<IReadOnlyDictionary<IAtom, IAtom>> allAtomMCS = null;
        private static Dictionary<IAtom, IAtom> atomsMCS = null;
        private static SortedDictionary<int, int> firstMCS = null;
        private static List<IReadOnlyDictionary<int, int>> allMCS = null;
        private IAtomContainer source = null;
        private IQueryAtomContainer smartSource = null;
        private IAtomContainer target = null;
        private readonly bool removeHydrogen = false;

        public SingleMappingHandler(bool removeH)
        {
            this.removeHydrogen = removeH;
            allAtomMCS = new List<IReadOnlyDictionary<IAtom, IAtom>>();
            atomsMCS = new Dictionary<IAtom, IAtom>();
            firstMCS = new SortedDictionary<int, int>();
            allMCS = new List<IReadOnlyDictionary<int, int>>();
        }

        public void Set(MolHandler source, MolHandler target)
        {
            this.source = source.Molecule;
            this.target = target.Molecule;
        }

        public void Set(IQueryAtomContainer source, IAtomContainer target)
        {
            this.smartSource = source;
            this.source = source;
            this.target = target;
        }

        //Function is called by the main program and serves as a starting point for the comparision procedure.

        public override void SearchMCS(bool bondTypeMatch)
        {
            SingleMapping singleMapping = new SingleMapping();
            IReadOnlyList<IReadOnlyDictionary<IAtom, IAtom>> mappings = null;
            try
            {
                if (this.smartSource == null)
                {
                    mappings = singleMapping.GetOverLaps(source, target, removeHydrogen);
                }
                else
                {
                    mappings = singleMapping.GetOverLaps(smartSource, target, removeHydrogen);
                }
            }
            catch (CDKException ex)
            {
                Trace.TraceError(ex.Message);
            }

            SetAllAtomMapping(mappings);
            SetAllMapping(mappings);
            SetFirstMapping();
            SetFirstAtomMapping();
            //SetStereoScore();
        }

        private void SetAllMapping(IReadOnlyList<IReadOnlyDictionary<IAtom, IAtom>> mappings)
        {
            try
            {
                int counter = 0;
                foreach (var solution in mappings)
                {
                    var atomMappings = new SortedDictionary<int, int>();
                    foreach (var map in solution)
                    {
                        IAtom sourceAtom = map.Key;
                        IAtom targetAtom = map.Value;
                        atomMappings[source.Atoms.IndexOf(sourceAtom)] = target.Atoms.IndexOf(targetAtom);
                    }
                    allMCS.Insert(counter++, atomMappings);
                }
            }
            catch (Exception)
            {
                //I.GetCause();
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void SetAllAtomMapping(IReadOnlyList<IReadOnlyDictionary<IAtom, IAtom>> mappings)
        {
            try
            {
                int counter = 0;
                foreach (var solution in mappings)
                {
                    var atomMappings = new Dictionary<IAtom, IAtom>();
                    foreach (var map in solution)
                    {
                        IAtom sourceAtom = map.Key;
                        IAtom targetAtom = map.Value;
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
            if (allMCS.Count > 0)
            {
                firstMCS = new SortedDictionary<int, int>();
                foreach (var e in allMCS.First())
                    firstMCS.Add(e.Key, e.Value);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void SetFirstAtomMapping()
        {
            if (allAtomMCS.Count > 0)
            {
                atomsMCS = new Dictionary<IAtom, IAtom>();
                foreach (var e in allAtomMCS.First())
                    atomsMCS.Add(e.Key, e.Value);
            }
        }

        public IReadOnlyList<IReadOnlyDictionary<int, int>> GetAllMapping()
        {
            return allMCS;
        }

        public IReadOnlyDictionary<int, int> GetFirstMapping()
        {
            return firstMCS;
        }

        public IReadOnlyList<IReadOnlyDictionary<IAtom, IAtom>> GetAllAtomMapping()
        {
            return allAtomMCS;
        }

        public IReadOnlyDictionary<IAtom, IAtom> GetFirstAtomMapping()
        {
            return atomsMCS;
        }
    }
}
