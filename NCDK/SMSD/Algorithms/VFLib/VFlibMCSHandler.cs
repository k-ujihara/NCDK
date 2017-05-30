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
 * You should have received commonAtomList copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using NCDK.Isomorphisms.Matchers;
using NCDK.SMSD.Algorithms.McGregors;

using NCDK.SMSD.Algorithms.VFLib.Map;
using NCDK.SMSD.Algorithms.VFLib.Query;
using NCDK.SMSD.Tools;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace NCDK.SMSD.Algorithms.VFLib
{
    /// <summary>
    /// This class should be used to find MCS between query
    /// graph and target graph.
    ///
    /// First the algorithm runs VF lib <see cref="VFMCSMapper"/>
    /// and reports MCS between
    /// run query and target graphs. Then these solutions are extended
    /// using McGregor <see cref="McGregors.McGregor"/>
    /// algorithm where ever required.
    /// </summary>
    // @cdk.module smsd
    // @cdk.githash
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    [Obsolete("SMSD has been deprecated from the CDK with a newer, more recent version of SMSD is available at http://github.com/asad/smsd .")]
    public class VFlibMCSHandler : AbstractMCSAlgorithm, IMCSBase
    {
        private static List<IDictionary<IAtom, IAtom>> allAtomMCS = null;
        private static IDictionary<IAtom, IAtom> atomsMCS = null;
        private static List<IDictionary<IAtom, IAtom>> allAtomMCSCopy = null;
        private static IDictionary<int, int> firstMCS = null;
        private static List<IDictionary<int, int>> allMCS = null;
        private static List<IDictionary<int, int>> allMCSCopy = null;
        private List<IDictionary<INode, IAtom>> vfLibSolutions = null;
        private IQueryAtomContainer queryMol = null;
        private IAtomContainer mol1 = null;
        private IAtomContainer mol2 = null;
        private int vfMCSSize = -1;
        private bool bondMatchFlag = false;
        private int countR = 0;
        private int countP = 0;

        /// <summary>
        /// Constructor for an extended VF Algorithm for the MCS search
        /// </summary>
        public VFlibMCSHandler()
        {
            allAtomMCS = new List<IDictionary<IAtom, IAtom>>();
            allAtomMCSCopy = new List<IDictionary<IAtom, IAtom>>();
            atomsMCS = new Dictionary<IAtom, IAtom>();
            firstMCS = new SortedDictionary<int, int>();
            allMCS = new List<IDictionary<int, int>>();
            allMCSCopy = new List<IDictionary<int, int>>();
        }

        public override void SearchMCS(bool bondTypeMatch)
        {
            IsBondMatchFlag = bondTypeMatch;
            SearchVFMCSMappings();
            bool flag = McGregorFlag();
            if (flag && vfLibSolutions.Count != 0)
            {
                try
                {
                    SearchMcGregorMapping();
                }
                catch (CDKException ex)
                {
                    Trace.TraceError(ex.Message);
                }
                catch (IOException ex)
                {
                    Trace.TraceError(ex.Message);
                }
            }
            else if (allAtomMCSCopy.Count != 0)
            {
                allAtomMCS.AddRange(allAtomMCSCopy);
                allMCS.AddRange(allMCSCopy);
            }
            SetFirstMappings();
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

        /// <inheritdoc/>
        public void Set(IQueryAtomContainer source, IAtomContainer target)
        {
            queryMol = source;
            mol2 = target;
        }

        private bool HasMap(IDictionary<int, int> maps, List<IDictionary<int, int>> mapGlobal)
        {
            foreach (var test in mapGlobal)
            {
                if (test.Equals(maps))
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

        private void SearchVFMCSMappings()
        {
            //        Console.Out.WriteLine("searchVFMCSMappings ");
            IQuery query = null;
            IMapper mapper = null;

            if (queryMol == null)
            {
                countR = GetReactantMol().Atoms.Count
                        + AtomContainerManipulator.GetSingleBondEquivalentSum(GetReactantMol());
                countP = GetProductMol().Atoms.Count
                        + AtomContainerManipulator.GetSingleBondEquivalentSum(GetProductMol());
            }
            vfLibSolutions = new List<IDictionary<INode, IAtom>>();
            if (queryMol != null)
            {
                query = new QueryCompiler(queryMol).Compile();
                mapper = new VFMCSMapper(query);
                var maps = mapper.GetMaps(GetProductMol());
                if (maps != null)
                {
                    vfLibSolutions.AddRange(maps);
                }
                SetVFMappings(true, query);
            }
            else if (countR <= countP)
            {
                query = new QueryCompiler(mol1, IsBondMatchFlag).Compile();
                mapper = new VFMCSMapper(query);
                var maps = mapper.GetMaps(GetProductMol());
                if (maps != null)
                {
                    vfLibSolutions.AddRange(maps);
                }
                SetVFMappings(true, query);
            }
            else
            {
                query = new QueryCompiler(GetProductMol(), IsBondMatchFlag).Compile();
                mapper = new VFMCSMapper(query);
                var maps = mapper.GetMaps(GetReactantMol());
                if (maps != null)
                {
                    vfLibSolutions.AddRange(maps);
                }
                SetVFMappings(false, query);
            }
            SetVFMappings(false, query);
            //        Console.Out.WriteLine("Sol count " + vfLibSolutions.Count);
            //        Console.Out.WriteLine("Sol size " + vfLibSolutions.Iterator().Next().Count);
            //        Console.Out.WriteLine("MCSSize " + vfMCSSize);
            //        Console.Out.WriteLine("After Sol count " + allMCSCopy.Count);

        }

        private void SearchMcGregorMapping()
        {
            IList<IList<int>> mappings = new List<IList<int>>();
            bool ropFlag = true;
            foreach (var firstPassMappings in allMCSCopy)
            {
                IDictionary<int, int> tMapping = new SortedDictionary<int, int>(firstPassMappings);
                McGregor mgit = null;
                if (queryMol != null)
                {
                    mgit = new McGregor(queryMol, mol2, mappings, IsBondMatchFlag);
                }
                else
                {
                    if (countR > countP)
                    {
                        mgit = new McGregor(mol1, mol2, mappings, IsBondMatchFlag);
                    }
                    else
                    {
                        tMapping.Clear();
                        mgit = new McGregor(mol2, mol1, mappings, IsBondMatchFlag);
                        ropFlag = false;
                        foreach (var map in firstPassMappings)
                        {
                            tMapping[map.Value] = map.Key;
                        }
                    }
                }
                mgit.StartMcGregorIteration(mgit.MCSSize, tMapping); //Start McGregor search
                mappings = mgit.Mappings;
                mgit = null;
            }
            //        Console.Out.WriteLine("\nSol count after MG" + mappings.Count);
            SetMcGregorMappings(ropFlag, mappings);
            vfMCSSize = vfMCSSize / 2;
            //        Console.Out.WriteLine("After set Sol count MG" + allMCS.Count);
            //        Console.Out.WriteLine("MCSSize " + vfMCSSize + "\n");
        }

        private void SetVFMappings(bool rONP, IQuery query)
        {
            int counter = 0;
            foreach (var solution in vfLibSolutions)
            {
                IDictionary<IAtom, IAtom> atomatomMapping = new Dictionary<IAtom, IAtom>();
                IDictionary<int, int> indexindexMapping = new SortedDictionary<int, int>();
                if (solution.Count > vfMCSSize)
                {
                    this.vfMCSSize = solution.Count;
                    allAtomMCSCopy.Clear();
                    allMCSCopy.Clear();
                    counter = 0;
                }
                foreach (var mapping in solution)
                {
                    IAtom qAtom = null;
                    IAtom tAtom = null;
                    int qIndex = 0;
                    int tIndex = 0;

                    if (rONP)
                    {
                        qAtom = query.GetAtom(mapping.Key);
                        tAtom = mapping.Value;
                        qIndex = GetReactantMol().Atoms.IndexOf(qAtom);
                        tIndex = GetProductMol().Atoms.IndexOf(tAtom);
                    }
                    else
                    {
                        tAtom = query.GetAtom(mapping.Key);
                        qAtom = mapping.Value;
                        qIndex = GetReactantMol().Atoms.IndexOf(qAtom);
                        tIndex = GetProductMol().Atoms.IndexOf(tAtom);
                    }

                    if (qIndex != -1 && tIndex != -1)
                    {
                        atomatomMapping[qAtom] = tAtom;
                        indexindexMapping[qIndex] = tIndex;
                    }
                    else
                    {
                        try
                        {
                            throw new CDKException("Atom index pointing to -1");
                        }
                        catch (CDKException ex)
                        {
                            Trace.TraceError(ex.Message);
                        }
                    }
                }
                if (atomatomMapping.Count != 0 && !HasMap(indexindexMapping, allMCSCopy)
                        && indexindexMapping.Count == vfMCSSize)
                {
                    allAtomMCSCopy.Insert(counter, atomatomMapping);
                    allMCSCopy.Insert(counter, indexindexMapping);
                    counter++;
                }
            }
        }

        private void SetMcGregorMappings(bool ronp, IList<IList<int>> mappings)
        {
            int counter = 0;
            foreach (var mapping in mappings)
            {
                if (mapping.Count > vfMCSSize)
                {
                    vfMCSSize = mapping.Count;
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
                    int qIndex = 0;
                    int tIndex = 0;

                    if (ronp)
                    {
                        qAtom = GetReactantMol().Atoms[mapping[index]];
                        tAtom = GetProductMol().Atoms[mapping[index + 1]];

                        qIndex = mapping[index];
                        tIndex = mapping[index + 1];
                    }
                    else
                    {
                        qAtom = GetReactantMol().Atoms[mapping[index + 1]];
                        tAtom = GetProductMol().Atoms[mapping[index]];
                        qIndex = mapping[index + 1];
                        tIndex = mapping[index];
                    }

                    //if (qIndex != null && tIndex != null) {
                    atomatomMapping[qAtom] = tAtom;
                    indexindexMapping[qIndex] = tIndex;
                    //} else {
                    //    throw new CDKException("Atom index pointing to NULL");
                    //}
                }
                if (atomatomMapping.Count != 0 && !HasMap(indexindexMapping, allMCS)
                        && (2 * indexindexMapping.Count) == vfMCSSize)
                {
                    allAtomMCS.Insert(counter, atomatomMapping);
                    allMCS.Insert(counter, indexindexMapping);
                    counter++;
                }
            }
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
            if (queryMol == null)
            {
                return mol1;
            }
            else
            {
                return queryMol;
            }
        }

        private IAtomContainer GetProductMol()
        {
            return mol2;
        }
    }
}
