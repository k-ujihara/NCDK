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
using NCDK.Graphs;
using NCDK.Isomorphisms.Matchers;
using NCDK.SMSD.Helper;
using NCDK.SMSD.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace NCDK.SMSD.Algorithms.RGraph
{
    /// <summary>
    /// This class acts as a handler class for <see cref="CDKMCS"/> algorithm.
    ///
    // @cdk.module smsd
    // @cdk.githash
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    /// </summary>
    public class CDKSubGraphHandler : AbstractSubGraph, IMCSBase
    {

        //    //~--- fields -------------------------------------------------------------
        private IAtomContainer source;
        private IAtomContainer target;
        private bool rOnPFlag = false;
        private List<IDictionary<IAtom, IAtom>> allAtomMCS = null;
        private IDictionary<IAtom, IAtom> firstAtomMCS = null;
        private IDictionary<int, int> firstMCS = null;
        private List<IDictionary<int, int>> allMCS = null;

        //~--- constructors -------------------------------------------------------
        /// <summary>
        /// Creates a new instance of MappingHandler
        /// </summary>
        public CDKSubGraphHandler()
        {
            this.allAtomMCS = new List<IDictionary<IAtom, IAtom>>();
            this.firstAtomMCS = new Dictionary<IAtom, IAtom>();
            this.firstMCS = new SortedDictionary<int, int>();
            this.allMCS = new List<IDictionary<int, int>>();
        }

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

        public override bool IsSubgraph(bool shouldMatchBonds)
        {
            CDKRMapHandler rmap = new CDKRMapHandler();

            try
            {

                if ((source.Atoms.Count == target.Atoms.Count) && source.Bonds.Count == target.Bonds.Count)
                {
                    rOnPFlag = true;
                    rmap.CalculateIsomorphs(source, target, shouldMatchBonds);

                }
                else if (source.Atoms.Count > target.Atoms.Count && source.Bonds.Count != target.Bonds.Count)
                {
                    rOnPFlag = true;
                    rmap.CalculateSubGraphs(source, target, shouldMatchBonds);
                }
                else
                {
                    rOnPFlag = false;
                    rmap.CalculateSubGraphs(target, source, shouldMatchBonds);
                }

                SetAllMapping();
                SetAllAtomMapping();
                SetFirstMapping();
                SetFirstAtomMapping();
            }
            catch (CDKException)
            {
                rmap = null;
                //            Console.Error.WriteLine("WARNING: graphContainer: most probably time out error ");
            }

            return GetFirstMapping().Count != 0;
        }

        protected IAtomContainerSet GetUncommon(IAtomContainer mol, IAtomContainer mcss, bool shouldMatchBonds)
        {
            List<int> atomSerialsToDelete = new List<int>();

            var matches = CDKMCS.GetSubgraphAtomsMaps(mol, mcss, shouldMatchBonds);
            var mapList = matches[0];
            foreach (var o in mapList)
            {
                CDKRMap rmap = (CDKRMap)o;
                atomSerialsToDelete.Add(rmap.Id1);
            }

            // at this point we have the serial numbers of the bonds to delete
            // we should get the actual bonds rather than delete by serial numbers
            List<IAtom> atomsToDelete = new List<IAtom>();
            foreach (var serial in atomSerialsToDelete)
            {
                atomsToDelete.Add(mol.Atoms[serial]);
            }

            // now lets get rid of the bonds themselves
            foreach (var atom in atomsToDelete)
            {
                mol.RemoveAtomAndConnectedElectronContainers(atom);
            }

            // now we probably have a set of disconnected components
            // so lets get a set of individual atom containers for
            // corresponding to each component
            return ConnectivityChecker.PartitionIntoMolecules(mol);
        }

        //~--- get methods --------------------------------------------------------
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void SetAllMapping()
        {
            //int count_final_sol = 1;
            //Console.Out.WriteLine("Output of the final FinalMappings: ");
            try
            {
                var sol = FinalMappings.Instance.GetFinalMapping();
                int counter = 0;
                foreach (var finalSolution in sol)
                {
                    SortedDictionary<int, int> atomMappings = new SortedDictionary<int, int>();
                    foreach (var solutions in finalSolution)
                    {
                        int iIndex = solutions.Key;
                        int jIndex = solutions.Value;

                        if (rOnPFlag)
                        {
                            atomMappings[iIndex] = jIndex;
                        }
                        else
                        {
                            atomMappings[jIndex] = iIndex;
                        }
                    }
                    if (!allMCS.Contains(atomMappings))
                    {
                        allMCS.Insert(counter++, atomMappings);
                    }
                }

            }
            catch (Exception)
            {
                //ex.GetCause();
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void SetAllAtomMapping()
        {
            List<IDictionary<int, int>> sol = allMCS;

            int counter = 0;
            foreach (var finalSolution in sol)
            {
                IDictionary<IAtom, IAtom> atomMappings = new Dictionary<IAtom, IAtom>();
                foreach (var solutions in finalSolution)
                {
                    int iIndex = solutions.Key;
                    int jIndex = solutions.Value;

                    IAtom sourceAtom = null;
                    IAtom targetAtom = null;

                    sourceAtom = source.Atoms[iIndex];
                    targetAtom = target.Atoms[jIndex];
                    atomMappings[sourceAtom] = targetAtom;

                }
                allAtomMCS.Insert(counter++, atomMappings);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void SetFirstMapping()
        {
            if (allMCS.Count != 0)
            {
                firstMCS = new SortedDictionary<int, int>(allMCS[0]);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void SetFirstAtomMapping()
        {
            if (allAtomMCS.Count != 0)
            {
                firstAtomMCS = new Dictionary<IAtom, IAtom>(allAtomMCS[0]);
            }
        }

        public IList<IDictionary<int, int>> GetAllMapping()
        {
            return new ReadOnlyCollection<IDictionary<int, int>>(allMCS);
        }

        public IDictionary<int, int> GetFirstMapping()
        {
            return new ReadOnlyDictionary<int, int>(firstMCS);
        }

        public IList<IDictionary<IAtom, IAtom>> GetAllAtomMapping()
        {
            return new ReadOnlyCollection<IDictionary<IAtom, IAtom>>(allAtomMCS);
        }

        public IDictionary<IAtom, IAtom> GetFirstAtomMapping()
        {
            return new ReadOnlyDictionary<IAtom, IAtom>(firstAtomMCS);
        }
    }
}
