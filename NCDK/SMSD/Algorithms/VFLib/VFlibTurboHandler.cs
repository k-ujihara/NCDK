/* Copyright (C) 2009-2010  Syed Asad Rahman <asad@ebi.ac.uk>
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
using NCDK.SMSD.Algorithms.McGregors;

using NCDK.SMSD.Algorithms.VFLib.Map;
using NCDK.SMSD.Algorithms.VFLib.Query;
using NCDK.SMSD.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NCDK.SMSD.Algorithms.VFLib
{
    /// <summary>
    /// This is an ultra fast method to report if query
    /// is a substructure for target molecule. If this case is true
    /// then it returns only one mapping.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is much faster than <see cref="VFlibMCSHandler"/> class
    /// as it only reports first match and backtracks.
    /// </para>
    /// <para>
    /// This class should only be used to report if a query
    /// graph is a substructure of the target graph.
    /// </para>
    /// </remarks>
    // @cdk.module smsd
    // @cdk.githash
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    [Obsolete("SMSD has been deprecated from the CDK with a newer, more recent version of SMSD is available at http://github.com/asad/smsd .")]
    public class VFlibTurboHandler : AbstractSubGraph, IMCSBase
    {
        private List<IDictionary<IAtom, IAtom>> allAtomMCS = null;
        private IDictionary<IAtom, IAtom> atomsMCS = null;
        private List<IDictionary<IAtom, IAtom>> allAtomMCSCopy = null;
        private IDictionary<int, int> firstMCS = null;
        private List<IDictionary<int, int>> allMCS = null;
        private List<IDictionary<int, int>> allMCSCopy = null;
        private IQueryAtomContainer queryMol = null;
        private IAtomContainer mol1 = null;
        private IAtomContainer mol2 = null;
        private IDictionary<INode, IAtom> vfLibSolutions = null;
        private int vfMCSSize = -1;
        private bool bondMatchFlag = false;

        /// <summary>
        /// Constructor for an extended VF Algorithm for the MCS search
        /// </summary>
        public VFlibTurboHandler()
        {
            allAtomMCS = new List<IDictionary<IAtom, IAtom>>();
            allAtomMCSCopy = new List<IDictionary<IAtom, IAtom>>();
            atomsMCS = new Dictionary<IAtom, IAtom>();
            firstMCS = new SortedDictionary<int, int>();
            allMCS = new List<IDictionary<int, int>>();
            allMCSCopy = new List<IDictionary<int, int>>();
        }

        private void SetFirstMappings()
        {
            if (allAtomMCS.Count != 0)
            {
                foreach (var e in allAtomMCS[0])
                    atomsMCS[e.Key] = e.Value;
                foreach (var e in allMCS[0])
                    firstMCS[e.Key] = e.Value;
            }
        }

        private bool McGregorFlag()
        {
            int commonAtomCount = CheckCommonAtomCount(GetReactantMol(), GetProductMol());
            if (commonAtomCount > vfMCSSize && commonAtomCount > vfMCSSize)
            {
                return true;
            }
            return false;
        }

        /// <summary> 
        /// Set the VFLib MCS software
        /// </summary>
        public void Set(MolHandler reactant, MolHandler product)
        {
            mol1 = reactant.Molecule;
            mol2 = product.Molecule;
        }

        public void Set(IQueryAtomContainer source, IAtomContainer target)
        {
            queryMol = source;
            mol2 = target;
        }

        private bool HasMap(IDictionary<int, int> map, List<IDictionary<int, int>> mapGlobal)
        {
            foreach (var test in mapGlobal)
            {
                if (test.Equals(map))
                {
                    return true;
                }
            }
            return false;
        }

        public IList<IDictionary<IAtom, IAtom>> GetAllAtomMapping()
        {
            return new ReadOnlyCollection<IDictionary<IAtom, IAtom>>(allAtomMCS);
        }

        public IList<IDictionary<int, int>> GetAllMapping()
        {
            return new ReadOnlyCollection<IDictionary<int, int>>(allMCS);
        }

        public IDictionary<IAtom, IAtom> GetFirstAtomMapping()
        {
            return new ReadOnlyDictionary<IAtom, IAtom>(atomsMCS);
        }

        public IDictionary<int, int> GetFirstMapping()
        {
            return new ReadOnlyDictionary<int, int>(firstMCS);
        }

        private int CheckCommonAtomCount(IAtomContainer reactantMolecule, IAtomContainer productMolecule)
        {
            List<string> atoms = new List<string>();
            for (int i = 0; i < reactantMolecule.Atoms.Count; i++)
            {
                atoms.Add(reactantMolecule.Atoms[i].Symbol);
            }
            int common = 0;
            for (int i = 0; i < productMolecule.Atoms.Count; i++)
            {
                string symbol = productMolecule.Atoms[i].Symbol;
                if (atoms.Contains(symbol))
                {
                    atoms.Remove(symbol);
                    common++;
                }
            }
            return common;
        }

        private void SearchVFMappings()
        {
            //        Console.Out.WriteLine("searchVFMappings ");
            IQuery query = null;
            IMapper mapper = null;
            vfLibSolutions = new Dictionary<INode, IAtom>();
            if (queryMol != null)
            {
                query = new QueryCompiler(queryMol).Compile();
                mapper = new VFMapper(query);
                if (mapper.HasMap(GetProductMol()))
                {
                    IDictionary<INode, IAtom> map = mapper.GetFirstMap(GetProductMol());
                    if (map != null)
                    {
                        foreach (var e in map)
                            vfLibSolutions[e.Key] = e.Value;
                    }
                }
                SetVFMappings(true, query);
            }
            else if (GetReactantMol().Atoms.Count <= GetProductMol().Atoms.Count)
            {
                query = new QueryCompiler(mol1, IsBondMatchFlag).Compile();
                mapper = new VFMapper(query);
                if (mapper.HasMap(GetProductMol()))
                {
                    IDictionary<INode, IAtom> map = mapper.GetFirstMap(GetProductMol());
                    if (map != null)
                    {
                        foreach (var e in map)
                            vfLibSolutions[e.Key] = e.Value;
                    }
                }
                SetVFMappings(true, query);
            }
            else
            {
                query = new QueryCompiler(GetProductMol(), IsBondMatchFlag).Compile();
                mapper = new VFMapper(query);
                if (mapper.HasMap(GetReactantMol()))
                {
                    IDictionary<INode, IAtom> map = mapper.GetFirstMap(GetReactantMol());
                    if (map != null)
                    {
                        foreach (var e in map)
                            vfLibSolutions[e.Key] = e.Value;
                    }
                }
                SetVFMappings(false, query);
            }
        }

        private void SearchMcGregorMapping()
        {
            IList<IList<int>> mappings = new List<IList<int>>();
            foreach (var firstPassMappings in allMCSCopy)
            {
                McGregor mgit = new McGregor(GetReactantMol(), GetProductMol(), mappings, IsBondMatchFlag);
                mgit.StartMcGregorIteration(mgit.MCSSize, firstPassMappings); //Start McGregor search
                mappings = mgit.Mappings;
                mgit = null;
            }
            //        Console.Out.WriteLine("\nSol count after MG" + mappings.Count);
            SetMcGregorMappings(mappings);
            vfMCSSize = vfMCSSize / 2;
            //        Console.Out.WriteLine("After set Sol count MG" + allMCS.Count);
            //        Console.Out.WriteLine("MCSSize " + vfMCSSize + "\n");
        }

        private void SetVFMappings(bool ronp, IQuery query)
        {
            int counter = 0;

            IDictionary<IAtom, IAtom> atomatomMapping = new Dictionary<IAtom, IAtom>();
            IDictionary<int, int> indexindexMapping = new SortedDictionary<int, int>();
            if (vfLibSolutions.Count > vfMCSSize)
            {
                this.vfMCSSize = vfLibSolutions.Count;
                allAtomMCSCopy.Clear();
                allMCSCopy.Clear();
                counter = 0;
            }
            foreach (var mapping in vfLibSolutions)
            {
                IAtom qAtom = null;
                IAtom tAtom = null;
                if (ronp)
                {
                    qAtom = query.GetAtom(mapping.Key);
                    tAtom = mapping.Value;
                }
                else
                {
                    tAtom = query.GetAtom(mapping.Key);
                    qAtom = mapping.Value;
                }
                int qIndex = GetReactantMol().Atoms.IndexOf(qAtom);
                int tIndex = GetProductMol().Atoms.IndexOf(tAtom);
                atomatomMapping[qAtom] = tAtom;
                indexindexMapping[qIndex] = tIndex;
            }
            //            Console.Out.WriteLine("indexindexMapping " + indexindexMapping.Count);
            //            Console.Out.WriteLine("MCS Size " + vfMCSSize);
            if (atomatomMapping.Count != 0 && !HasMap(indexindexMapping, allMCSCopy)
                    && indexindexMapping.Count == vfMCSSize)
            {
                allAtomMCSCopy.Insert(counter, atomatomMapping);
                allMCSCopy.Insert(counter, indexindexMapping);
                counter++;
            }
            //        Console.Out.WriteLine("allMCSCopy " + allMCSCopy.Count);
        }

        private void SetMcGregorMappings(IList<IList<int>> mappings)
        {
            int counter = 0;
            this.vfMCSSize = 0;
            foreach (var mapping in mappings)
            {
                if (mapping.Count > vfMCSSize)
                {
                    vfMCSSize = (mapping.Count / 2);
                    allAtomMCS.Clear();
                    allMCS.Clear();
                    counter = 0;
                }
                IDictionary<IAtom, IAtom> atomatomMapping = new Dictionary<IAtom, IAtom>();
                IDictionary<int, int> indexindexMapping = new SortedDictionary<int, int>();
                for (int index = 0; index < mapping.Count; index += 2)
                {
                    IAtom qAtom = null;
                    IAtom tAtom = null;

                    qAtom = GetReactantMol().Atoms[mapping[index]];
                    tAtom = GetProductMol().Atoms[mapping[index + 1]];

                    int qIndex = mapping[index];
                    int tIndex = mapping[index + 1];

                    if (qIndex != -1 && tIndex != -1)
                    {
                        atomatomMapping[qAtom] = tAtom;
                        indexindexMapping[qIndex] = tIndex;
                    }
                    else
                    {
                        throw new CDKException("Atom index pointing to NULL");
                    }
                }
                if (atomatomMapping.Count != 0 && !HasMap(indexindexMapping, allMCS)
                        && (indexindexMapping.Count) == vfMCSSize)
                {
                    allAtomMCS.Insert(counter, atomatomMapping);
                    allMCS.Insert(counter, indexindexMapping);
                    counter++;
                }
            }
        }

        public override bool IsSubgraph(bool shouldMatchBonds)
        {
            IsBondMatchFlag = shouldMatchBonds;
            SearchVFMappings();
            //        bool flag = McGregorFlag();
            //        if (flag && vfLibSolutions.Count != 0) {
            //            try {
            //                SearchMcGregorMapping();
            //            } catch (CDKException ex) {
            //                Trace.TraceError(Level.SEVERE, null, ex);
            //            } catch (IOException ex) {
            //                Trace.TraceError(Level.SEVERE, null, ex);
            //            }
            //
            //        } else

            if (allAtomMCSCopy.Count != 0)
            {
                allAtomMCS.AddRange(allAtomMCSCopy);
                allMCS.AddRange(allMCSCopy);
            }
            SetFirstMappings();
            return (allMCS.Count != 0 && allMCS.First().Count == GetReactantMol().Atoms.Count) ? true : false;
        }

        public bool IsBondMatchFlag
        {
            get
            {
                return bondMatchFlag;
            }
            set
            {
                this.bondMatchFlag = value;
            }
        }

        private IAtomContainer GetReactantMol()
        {
            return queryMol ?? mol1;
        }

        private IAtomContainer GetProductMol()
        {
            return mol2;
        }
    }
}
