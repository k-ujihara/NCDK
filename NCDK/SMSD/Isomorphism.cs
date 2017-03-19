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
 * of your sourceAtomCount code files, and to any copyright notice that you may distribute
 * with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received rBondCount copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Isomorphisms.Matchers;
using NCDK.SMSD.Algorithms.MCSPluses;
using NCDK.SMSD.Algorithms.RGraph;
using NCDK.SMSD.Algorithms.Single;
using NCDK.SMSD.Algorithms.VFLib;
using NCDK.SMSD.Filters;
using NCDK.SMSD.Global;
using NCDK.SMSD.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NCDK.SMSD
{
    /// <summary>
    ///  <para>This class implements the Isomorphism- a multipurpose structure comparison tool.
    ///  It allows users to, i) find the maximal common Substructure(s) (MCS);
    ///  ii) perform the mapping of a substructure in another structure, and;
    ///  iii) map two isomorphic structures.</para>
    ///  <para>It also comes with various published algorithms. The user is free to
    ///  choose his favorite algorithm to perform MCS or substructure search.
    ///  For example 0: Isomorphism algorithm, 1: MCSPlus, 2: VFLibMCS, 3: CDKMCS, 4:
    ///  Substructure</para>
    ///
    ///  <para>It also has a set of robust chemical filters (i.e. bond energy, fragment
    ///  count, stereo &amp; bond match) to sort the reported MCS solutions in a chemically
    ///  relevant manner. Each comparison can be made with or without using the bond
    ///  sensitive mode and with implicit or explicit hydrogens.</para>
    ///
    ///  <para>If you are using <b>Isomorphism, please cite Rahman <i>et.al. 2009</i></b>
    ///  {@cdk.cite SMSD2009}. The Isomorphism algorithm is described in this paper.
    ///  </para>
    /// </summary>
    /// 
    /// <example>
    /// <para>An example for <b>Substructure search</b>:</para>
    ///  <code>
    ///  SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
    ///  // Benzene
    ///  IAtomContainer A1 = sp.ParseSmiles("C1=CC=CC=C1");
    ///  // Napthalene
    ///  IAtomContainer A2 = sp.ParseSmiles("C1=CC2=C(C=C1)C=CC=C2");
    ///  //Turbo mode search
    ///  //Bond Sensitive is set true
    ///  Isomorphism comparison = new Isomorphism(Algorithm.SubStructure, true);
    ///  // set molecules, remove hydrogens, clean and configure molecule
    ///  comparison.Init(A1, A2, true, true);
    ///  // set chemical filter true
    ///  comparison.SetChemFilters(false, false, false);
    ///  if (comparison.IsSubgraph()) {
    ///  //Get similarity score
    ///   Console.Out.WriteLine("Tanimoto coefficient:  " + comparison.GetTanimotoSimilarity());
    ///   Console.Out.WriteLine("A1 is a subgraph of A2:  " + comparison.IsSubgraph());
    ///  //Get Modified AtomContainer
    ///   IAtomContainer Mol1 = comparison.ReactantMolecule;
    ///   IAtomContainer Mol2 = comparison.ProductMolecule;
    ///  // Print the mapping between molecules
    ///   Console.Out.WriteLine(" Mappings: ");
    ///   foreach (var mapping in comparison.GetFirstMapping().EntrySet()) {
    ///      Console.Out.WriteLine((mapping.Key + 1) + " " + (mapping.Value + 1));
    ///
    ///      IAtom eAtom = Mol1.Atoms[mapping.Key];
    ///      IAtom pAtom = Mol2.Atoms[mapping.Value];
    ///      Console.Out.WriteLine(eAtom.Symbol + " " + pAtom.Symbol);
    ///   }
    ///   Console.Out.WriteLine("");
    ///  }
    ///  </code>
    ///
    /// <para>An example for <b>MCS search</b>:</para>
    ///  <code>
    ///  SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
    ///  // Benzene
    ///  IAtomContainer A1 = sp.ParseSmiles("C1=CC=CC=C1");
    ///  // Napthalene
    ///  IAtomContainer A2 = sp.ParseSmiles("C1=CC2=C(C=C1)C=CC=C2");
    ///  //{ 0: Default Isomorphism Algorithm, 1: MCSPlus Algorithm, 2: VFLibMCS Algorithm, 3: CDKMCS Algorithm}
    ///  //Bond Sensitive is set true
    ///  Isomorphism comparison = new Isomorphism(Algorithm.Default, true);
    ///  // set molecules, remove hydrogens, clean and configure molecule
    ///  comparison.Init(A1, A2, true, true);
    ///  // set chemical filter true
    ///  comparison.SetChemFilters(true, true, true);
    ///
    ///  //Get similarity score
    ///  Console.Out.WriteLine("Tanimoto coefficient:  " + comparison.GetTanimotoSimilarity());
    ///  Console.Out.WriteLine("A1 is a subgraph of A2:  " + comparison.IsSubgraph());
    ///  //Get Modified AtomContainer
    ///  IAtomContainer Mol1 = comparison.ReactantMolecule;
    ///  IAtomContainer Mol2 = comparison.ProductMolecule;
    ///  // Print the mapping between molecules
    ///  Console.Out.WriteLine(" Mappings: ");
    ///  foreach (var mapping in comparison.GetFirstMapping().EntrySet()) {
    ///      Console.Out.WriteLine((mapping.Key + 1) + " " + (mapping.Value + 1));
    ///
    ///      IAtom eAtom = Mol1.Atoms[mapping.Key];
    ///      IAtom pAtom = Mol2.Atoms[mapping.Value];
    ///      Console.Out.WriteLine(eAtom.Symbol + " " + pAtom.Symbol);
    ///  }
    ///  Console.Out.WriteLine("");
    ///  </code>
    ///  </example>
    // @cdk.require java1.5+
    // @cdk.module smsd
    // @cdk.githash
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    [Serializable]
    public sealed class Isomorphism : AbstractMCS
    {
        private List<IDictionary<int, int>> allMCS = null;
        private IDictionary<int, int> firstSolution = null;
        private List<IDictionary<IAtom, IAtom>> allAtomMCS = null;
        private IDictionary<IAtom, IAtom> firstAtomMCS = null;
        private List<IDictionary<IBond, IBond>> allBondMCS = null;
        private IDictionary<IBond, IBond> firstBondMCS = null;
        private MolHandler rMol = null;
        private IQueryAtomContainer queryMol = null;
        private MolHandler pMol = null;
        private IAtomContainer pAC = null;
        private IList<double> stereoScore = null;
        private IList<int> fragmentSize = null;
        private IList<double> bEnergies = null;
        private Algorithm algorithmType;
        private bool removeHydrogen = false;
        private bool subGraph = false;

        /// <summary>
        /// This is the algorithm factory and entry port for all the MCS algorithm in the Isomorphism
        /// supported algorithm <see cref="Algorithm"/> types:
        /// <list type="bullet">
        /// <item>0: Default,</item>
        /// <item>1: MCSPlus,</item>
        /// <item>2: VFLibMCS,</item>
        /// <item>3: CDKMCS,</item>
        /// <item>4: SubStructure</item>
        /// </list> 
        /// </summary>
        /// <param name="algorithmType"><see cref="Algorithm"/></param>
        /// <param name="bondTypeFlag"></param>
        public Isomorphism(Algorithm algorithmType, bool bondTypeFlag)
        {
            this.algorithmType = algorithmType;
            firstSolution = new SortedDictionary<int, int>();
            allMCS = new List<IDictionary<int, int>>();
            allAtomMCS = new List<IDictionary<IAtom, IAtom>>();
            firstAtomMCS = new Dictionary<IAtom, IAtom>();
            allBondMCS = new List<IDictionary<IBond, IBond>>();
            firstBondMCS = new Dictionary<IBond, IBond>();

            SetTime(bondTypeFlag);
            IsMatchBonds = bondTypeFlag;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void MCSBuilder(MolHandler mol1, MolHandler mol2)
        {
            int rBondCount = mol1.Molecule.Bonds.Count;
            int pBondCount = mol2.Molecule.Bonds.Count;

            int rAtomCount = mol1.Molecule.Atoms.Count;
            int pAtomCount = mol2.Molecule.Atoms.Count;

            if ((rBondCount == 0 && rAtomCount > 0) || (pBondCount == 0 && pAtomCount > 0))
            {
                SingleMapping();
            }
            else
            {
                ChooseAlgorithm(rBondCount, pBondCount);
            }

            if (allAtomMCS.Count != 0 && firstAtomMCS.Count != 0 && firstAtomMCS.Count > 1)
            {
                AllBondMaps = MakeBondMapsOfAtomMaps(mol1.Molecule, mol2.Molecule, allAtomMCS);
                var firstMap = AllBondMaps.FirstOrDefault();
                if (firstMap != null)
                    SetFirstBondMap(firstMap);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void MCSBuilder(IQueryAtomContainer mol1, IAtomContainer mol2)
        {

            int rBondCount = mol1.Bonds.Count;
            int pBondCount = mol2.Bonds.Count;

            int rAtomCount = mol1.Atoms.Count;
            int pAtomCount = mol2.Atoms.Count;

            if ((rBondCount == 0 && rAtomCount > 0) || (pBondCount == 0 && pAtomCount > 0))
            {
                SingleMapping();
            }
            else
            {
                ChooseAlgorithm(rBondCount, pBondCount);
            }

            if (allAtomMCS.Count != 0 && firstAtomMCS.Count != 0 && firstAtomMCS.Count > 1)
            {
                AllBondMaps = MakeBondMapsOfAtomMaps(mol1, mol2, allAtomMCS);
                var firstMap = AllBondMaps.FirstOrDefault();
                if (firstMap != null)
                    SetFirstBondMap(firstMap);
            }
        }

        /// <summary>
        /// Returns bond maps between source and target molecules based on the atoms
        /// <param name="ac1">source molecule</param>
        /// <param name="ac2">target molecule</param>
        /// <param name="mappings">mappings between source and target molecule atoms</param>
        /// <returns>bond maps between source and target molecules based on the atoms</returns>
        /// </summary>
        public static List<IDictionary<IBond, IBond>> MakeBondMapsOfAtomMaps(IAtomContainer ac1, IAtomContainer ac2,
                List<IDictionary<IAtom, IAtom>> mappings)
        {
            List<IDictionary<IBond, IBond>> bondMaps = new List<IDictionary<IBond, IBond>>();
            foreach (var mapping in mappings)
            {
                bondMaps.Add(MakeBondMapOfAtomMap(ac1, ac2, mapping));
            }
            return bondMaps;
        }

        /// <summary>
        ///
        /// Returns bond map between source and target molecules based on the atoms
        /// <param name="ac1">source molecule</param>
        /// <param name="ac2">target molecule</param>
        /// <param name="mapping">mappings between source and target molecule atoms</param>
        /// <returns>bond map between source and target molecules based on the atoms</returns>
        /// </summary>
        public static IDictionary<IBond, IBond> MakeBondMapOfAtomMap(IAtomContainer ac1, IAtomContainer ac2,
                IDictionary<IAtom, IAtom> mapping)
        {
            IDictionary<IBond, IBond> maps = new Dictionary<IBond, IBond>();

            foreach (var mapS in mapping)
            {
                IAtom indexI = mapS.Key;
                IAtom indexJ = mapS.Value;

                foreach (var mapD in mapping)
                {
                    IAtom indexIPlus = mapD.Key;
                    IAtom indexJPlus = mapD.Value;

                    if (!indexI.Equals(indexIPlus) && !indexJ.Equals(indexJPlus))
                    {
                        IBond ac1Bond = ac1.GetBond(indexI, indexIPlus);
                        if (ac1Bond != null)
                        {
                            IBond ac2Bond = ac2.GetBond(indexJ, indexJPlus);
                            if (ac2Bond != null)
                            {
                                maps[ac1Bond] = ac2Bond;
                            }
                        }
                    }
                }
            }

            //        Console.Out.WriteLine("bond Map size:" + maps.Count);

            return maps;

        }

        private void ChooseAlgorithm(int rBondCount, int pBondCount)
        {
            switch (algorithmType.Ordinal)
            {
                case Algorithm.O.CDKMCS:
                    CDKMCSAlgorithm();
                    break;
                case Algorithm.O.Default:
                    DefaultMCSAlgorithm();
                    break;
                case Algorithm.O.MCSPlus:
                    MCSPlusAlgorithm();
                    break;
                case Algorithm.O.SubStructure:
                    SubStructureAlgorithm(rBondCount, pBondCount);
                    break;
                case Algorithm.O.VFLibMCS:
                    VFLibMCSAlgorithm();
                    break;
                case Algorithm.O.TurboSubStructure:
                    TurboSubStructureAlgorithm(rBondCount, pBondCount);
                    break;
                default:
                    throw new CDKException($"Unknown {nameof(Algorithm)}.");
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void CDKMCSAlgorithm()
        {
            CDKMCSHandler mcs = null;
            mcs = new CDKMCSHandler();

            if (queryMol == null)
            {
                mcs.Set(rMol, pMol);
            }
            else
            {
                mcs.Set(queryMol, pAC);
            }
            mcs.SearchMCS(IsMatchBonds);

            ClearMaps();

            foreach (var e in mcs.GetFirstMapping())
                firstSolution[e.Key] = e.Value;
            allMCS.AddRange(mcs.GetAllMapping());

            foreach (var e in mcs.GetFirstAtomMapping())
                firstAtomMCS[e.Key] = e.Value;
            allAtomMCS.AddRange(mcs.GetAllAtomMapping());

        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void CDKSubgraphAlgorithm()
        {
            CDKSubGraphHandler mcs = null;
            mcs = new CDKSubGraphHandler();

            if (queryMol == null)
            {
                mcs.Set(rMol, pMol);
            }
            else
            {
                mcs.Set(queryMol, pAC);
            }

            ClearMaps();

            if (mcs.IsSubgraph(IsMatchBonds))
            {
                foreach (var e in mcs.GetFirstMapping())
                    firstSolution[e.Key] = e.Value;
                allMCS.AddRange(mcs.GetAllMapping());

                foreach (var e in mcs.GetFirstAtomMapping())
                    firstAtomMCS[e.Key] = e.Value;
                allAtomMCS.AddRange(mcs.GetAllAtomMapping());
            }

        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void MCSPlusAlgorithm()
        {
            MCSPlusHandler mcs = null;
            mcs = new MCSPlusHandler();

            if (queryMol == null)
            {
                mcs.Set(rMol, pMol);
            }
            else
            {
                mcs.Set(queryMol, pAC);
            }
            mcs.SearchMCS(IsMatchBonds);

            ClearMaps();

            foreach (var e in mcs.GetFirstMapping())
                firstSolution[e.Key] = e.Value;
            allMCS.AddRange(mcs.GetAllMapping());

            foreach (var e in mcs.GetFirstAtomMapping())
                firstAtomMCS[e.Key] = e.Value;
            allAtomMCS.AddRange(mcs.GetAllAtomMapping());
        }

        private void VfLibMCS()
        {
            VFlibMCSHandler mcs = null;
            mcs = new VFlibMCSHandler();
            if (queryMol == null)
            {
                mcs.Set(rMol, pMol);
            }
            else
            {
                mcs.Set(queryMol, pAC);
            }
            mcs.SearchMCS(IsMatchBonds);

            ClearMaps();
            foreach (var e in mcs.GetFirstMapping())
                firstSolution[e.Key] = e.Value;
            allMCS.AddRange(mcs.GetAllMapping());

            foreach (var e in mcs.GetFirstAtomMapping())
                firstAtomMCS[e.Key] = e.Value;
            allAtomMCS.AddRange(mcs.GetAllAtomMapping());
        }

        private void SubStructureHandler()
        {
            VFlibSubStructureHandler subGraphTurboSearch = null;
            subGraphTurboSearch = new VFlibSubStructureHandler();
            if (queryMol == null)
            {
                subGraphTurboSearch.Set(rMol, pMol);
            }
            else
            {
                subGraphTurboSearch.Set(queryMol, pAC);
            }
            ClearMaps();
            subGraph = subGraphTurboSearch.IsSubgraph(IsMatchBonds);
            if (subGraph)
            {
                foreach (var e in subGraphTurboSearch.GetFirstMapping())
                    firstSolution[e.Key] = e.Value;
                allMCS.AddRange(subGraphTurboSearch.GetAllMapping());
                foreach (var e in subGraphTurboSearch.GetFirstAtomMapping())
                    firstAtomMCS[e.Key] = e.Value;
                allAtomMCS.AddRange(subGraphTurboSearch.GetAllAtomMapping());
            }
        }

        private void TurboSubStructureHandler()
        {
            VFlibTurboHandler subGraphTurboSearch = null;
            subGraphTurboSearch = new VFlibTurboHandler();
            if (queryMol == null)
            {
                subGraphTurboSearch.Set(rMol, pMol);
            }
            else
            {
                subGraphTurboSearch.Set(queryMol, pAC);
            }
            ClearMaps();
            subGraph = subGraphTurboSearch.IsSubgraph(IsMatchBonds);
            if (subGraph)
            {
                foreach (var e in subGraphTurboSearch.GetFirstMapping())
                    firstSolution[e.Key] = e.Value;
                allMCS.AddRange(subGraphTurboSearch.GetAllMapping());
                foreach (var e in subGraphTurboSearch.GetFirstAtomMapping())
                    firstAtomMCS[e.Key] = e.Value;
                allAtomMCS.AddRange(subGraphTurboSearch.GetAllAtomMapping());
            }
        }

        private void SingleMapping()
        {
            SingleMappingHandler mcs = null;

            mcs = new SingleMappingHandler(removeHydrogen);
            if (queryMol == null)
            {
                mcs.Set(rMol, pMol);
            }
            else
            {
                mcs.Set(queryMol, pAC);
            }
            mcs.SearchMCS(IsMatchBonds);

            ClearMaps();
            foreach (var e in mcs.GetFirstMapping())
                firstSolution[e.Key] = e.Value;
            allMCS.AddRange(mcs.GetAllMapping());

            foreach (var e in mcs.GetFirstAtomMapping())
                firstAtomMCS[e.Key] = e.Value;
            allAtomMCS.AddRange(mcs.GetAllAtomMapping());
        }

        private int GetHCount(IAtomContainer molecule)
        {
            int count = 0;
            foreach (var atom in molecule.Atoms)
            {
                if (string.Equals(atom.Symbol, "H", StringComparison.OrdinalIgnoreCase))
                {
                    ++count;
                }
            }
            return count;
        }

        private bool IsBondMatch(IAtomContainer reactant, IAtomContainer product)
        {
            int counter = 0;
            var ketSet = firstAtomMCS.Keys.ToArray();
            for (int i = 0; i < ketSet.Length; i++)
            {
                for (int j = i + 1; j < ketSet.Length; j++)
                {
                    IAtom indexI = (IAtom)ketSet[i];
                    IAtom indexJ = (IAtom)ketSet[j];
                    IBond rBond = reactant.GetBond(indexI, indexJ);
                    if (rBond != null)
                    {
                        counter++;
                    }
                }
            }

            var valueSet = firstAtomMCS.Values.ToArray();
            for (int i = 0; i < valueSet.Length; i++)
            {
                for (int j = i + 1; j < valueSet.Length; j++)
                {
                    IAtom indexI = (IAtom)valueSet[i];
                    IAtom indexJ = (IAtom)valueSet[j];
                    IBond pBond = product.GetBond(indexI, indexJ);
                    if (pBond != null)
                    {
                        counter--;
                    }
                }
            }
            return counter == 0 ? true : false;
        }

        private void DefaultMCSAlgorithm()
        {
            try
            {
                if (IsMatchBonds)
                {
                    CDKMCSAlgorithm();
                    if (GetFirstMapping() == null || IsTimeOut())
                    {
                        VfLibMCS();
                    }
                }
                else
                {
                    MCSPlusAlgorithm();
                    if (GetFirstMapping() == null || IsTimeOut())
                    {
                        VfLibMCS();
                    }
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.StackTrace);
            }
        }

        private void SubStructureAlgorithm(int rBondCount, int pBondCount)
        {
            if (rBondCount > 0 && pBondCount > 0)
            {
                CDKSubgraphAlgorithm();
                if (GetFirstMapping() == null || IsTimeOut())
                {
                    SubStructureHandler();
                }
            }
            else
            {
                SingleMapping();
            }
        }

        private void TurboSubStructureAlgorithm(int rBondCount, int pBondCount)
        {
            if (rBondCount > 0 && pBondCount > 0)
            {
                TurboSubStructureHandler();
            }
            else
            {
                SingleMapping();
            }
        }

        private void VFLibMCSAlgorithm()
        {
            VfLibMCS();
        }

        private void SetTime(bool bondTypeFlag)
        {
            if (bondTypeFlag)
            {
                TimeOut tmo = TimeOut.Instance;
                tmo.Time = BondSensitiveTimeOut;
            }
            else
            {
                TimeOut tmo = TimeOut.Instance;
                tmo.Time = BondInSensitiveTimeOut;
            }
        }

        public bool IsTimeOut()
        {
            return TimeOut.Instance.Enabled;
        }

        public void ReSetTimeOut()
        {
            TimeOut.Instance.Enabled = false;
        }

        private void ClearMaps()
        {
            this.firstSolution.Clear();
            this.allMCS.Clear();
            this.allAtomMCS.Clear();
            this.firstAtomMCS.Clear();
        }

        private void Init(MolHandler reactant, MolHandler product)
        {
            this.rMol = reactant;
            this.pMol = product;
            MCSBuilder(reactant, product);
        }

        public override void Init(IQueryAtomContainer reactant, IAtomContainer product)
        {
            this.queryMol = reactant;
            this.pAC = product;
            MCSBuilder(queryMol, pAC);
        }

        public override void Init(IAtomContainer reactant, IAtomContainer product, bool removeHydrogen,
                bool cleanAndConfigureMolecule)
        {
            this.removeHydrogen = removeHydrogen;
            Init(new MolHandler(reactant, removeHydrogen, cleanAndConfigureMolecule), new MolHandler(product,
                    removeHydrogen, cleanAndConfigureMolecule));
        }
        
        /// <summary>
        /// Initialize the query and targetAtomCount mol via mol files
        /// </summary>
        /// <param name="sourceMolFileName">source mol file name</param>
        /// <param name="targetMolFileName">target mol file name</param>
        /// <param name="removeHydrogen">set true to make hydrogens implicit before search</param>
        /// <param name="cleanAndConfigureMolecule"> eg: percieveAtomTypesAndConfigureAtoms, detect aromaticity etc</param>
        public void Init(string sourceMolFileName, string targetMolFileName, bool removeHydrogen,
                bool cleanAndConfigureMolecule)
        {
            this.removeHydrogen = removeHydrogen;
            Init(new MolHandler(sourceMolFileName, cleanAndConfigureMolecule, removeHydrogen), new MolHandler(
                    targetMolFileName, cleanAndConfigureMolecule, removeHydrogen));
        }

        public override void SetChemFilters(bool stereoFilter, bool fragmentFilter, bool energyFilter)
        {

            if (firstAtomMCS != null)
            {
                ChemicalFilters chemFilter = new ChemicalFilters(allMCS, allAtomMCS, firstSolution, firstAtomMCS,
                        ReactantMolecule, ProductMolecule);

                if (stereoFilter && firstAtomMCS.Count > 1)
                {
                    try
                    {
                        chemFilter.SortResultsByStereoAndBondMatch();
                        this.stereoScore = chemFilter.GetStereoMatches();
                    }
                    catch (CDKException ex)
                    {
                        Trace.TraceError(ex.Message);
                    }
                }
                if (fragmentFilter)
                {
                    chemFilter.SortResultsByFragments();
                    this.fragmentSize = chemFilter.GetSortedFragment();
                }
                if (energyFilter)
                {
                    try
                    {
                        chemFilter.SortResultsByEnergies();
                        this.bEnergies = chemFilter.GetSortedEnergy();
                    }
                    catch (CDKException ex)
                    {
                        Trace.TraceError(ex.Message);
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override int? GetFragmentSize(int key)
        {
            return (fragmentSize != null && fragmentSize.Count != 0) ? fragmentSize[key] : (int?)null;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override int? GetStereoScore(int key)
        {
            return (stereoScore != null && stereoScore.Count != 0) ? (int)stereoScore[key] : (int?)null;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override double? GetEnergyScore(int key)
        {
            return (bEnergies != null && bEnergies.Count != 0) ? bEnergies[key] : (double?)null;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override IDictionary<int, int> GetFirstMapping()
        {
            return firstSolution.Count == 0 ? null : firstSolution;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override IList<IDictionary<int, int>> GetAllMapping()
        {
            return allMCS.Count == 0 ? null : allMCS;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override IDictionary<IAtom, IAtom> GetFirstAtomMapping()
        {
            return firstAtomMCS.Count == 0 ? null : firstAtomMCS;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override IList<IDictionary<IAtom, IAtom>> GetAllAtomMapping()
        {
            return allAtomMCS.Count == 0 ? null : allAtomMCS;
        }

        public override IAtomContainer ReactantMolecule
        {
            get
            {
                return queryMol == null ? rMol.Molecule : queryMol;
            }
        }

        public override IAtomContainer ProductMolecule
        {
            get
            {
                return pAC == null ? pMol.Molecule : pAC;
            }
        }

        public override double GetTanimotoSimilarity()
        {
            double tanimoto = GetTanimotoAtomSimilarity() + GetTanimotoBondSimilarity();
            if (tanimoto > 0 && ReactantMolecule.Bonds.Count > 0 && ProductMolecule.Bonds.Count > 0)
            {
                tanimoto /= 2;
            }
            return tanimoto;
        }

        public double GetTanimotoAtomSimilarity()
        {
            int decimalPlaces = 4;
            int rAtomCount = 0;
            int pAtomCount = 0;
            double tanimotoAtom = 0.0;

            if (GetFirstMapping() != null && GetFirstMapping().Count != 0)
            {
                if (!removeHydrogen)
                {
                    rAtomCount = ReactantMolecule.Atoms.Count;
                    pAtomCount = ProductMolecule.Atoms.Count;
                }
                else
                {
                    rAtomCount = ReactantMolecule.Atoms.Count - GetHCount(ReactantMolecule);
                    pAtomCount = ProductMolecule.Atoms.Count - GetHCount(ProductMolecule);
                }
                double matchCount = GetFirstMapping().Count;
                tanimotoAtom = (matchCount) / (rAtomCount + pAtomCount - matchCount);
                decimal tan = new Decimal(tanimotoAtom);
                tan = decimal.Round(tan, decimalPlaces);
                tanimotoAtom = (double)tan;
            }
            return tanimotoAtom;
        }

        public double GetTanimotoBondSimilarity()
        {
            int decimalPlaces = 4;
            int rBondCount = 0;
            int pBondCount = 0;
            double tanimotoAtom = 0.0;

            if (FirstBondMap != null && FirstBondMap.Count != 0)
            {
                rBondCount = ReactantMolecule.Bonds.Count;
                pBondCount = ProductMolecule.Bonds.Count;

                double matchCount = FirstBondMap.Count;
                tanimotoAtom = (matchCount) / (rBondCount + pBondCount - matchCount);
                decimal tan = new Decimal(tanimotoAtom);
                tan = decimal.Round(tan, decimalPlaces);
                tanimotoAtom = (double)tan;
            }
            return tanimotoAtom;
        }

        public override bool IsStereoMisMatch()
        {
            bool flag = false;
            IAtomContainer reactant = ReactantMolecule;
            IAtomContainer product = ProductMolecule;
            int score = 0;

            foreach (var mappingI in firstAtomMCS)
            {
                IAtom indexI = mappingI.Key;
                IAtom indexJ = mappingI.Value;
                foreach (var mappingJ in firstAtomMCS)
                {

                    IAtom indexIPlus = mappingJ.Key;
                    IAtom indexJPlus = mappingJ.Value;
                    if (!indexI.Equals(indexIPlus) && !indexJ.Equals(indexJPlus))
                    {

                        IAtom sourceAtom1 = indexI;
                        IAtom sourceAtom2 = indexIPlus;

                        IBond rBond = reactant.GetBond(sourceAtom1, sourceAtom2);

                        IAtom targetAtom1 = indexJ;
                        IAtom targetAtom2 = indexJPlus;
                        IBond pBond = product.GetBond(targetAtom1, targetAtom2);

                        if ((rBond != null && pBond != null) && (rBond.Stereo != pBond.Stereo))
                        {
                            score++;
                        }
                    }
                }
            }
            if (score > 0)
            {
                flag = true;
            }
            return flag;
        }

        public override bool IsSubgraph()
        {

            IAtomContainer reactant = ReactantMolecule;
            IAtomContainer product = ProductMolecule;

            float mappingSize = 0;
            if (firstSolution != null && firstSolution.Count != 0)
            {
                mappingSize = firstSolution.Count;
            }
            else
            {
                return false;
            }
            int sourceAtomCount = reactant.Atoms.Count;
            int targetAtomCount = product.Atoms.Count;
            if (removeHydrogen)
            {
                sourceAtomCount -= GetHCount(reactant);
                targetAtomCount -= GetHCount(product);
            }
            if (mappingSize == sourceAtomCount && mappingSize <= targetAtomCount)
            {
                if (FirstBondMap.Count != 0 && FirstBondMap.Count == reactant.Bonds.Count)
                {
                    return true;
                }
                else if (mappingSize == 1)
                {
                    return true;
                }
            }
            return false;
        }

        public override double GetEuclideanDistance()
        {
            int decimalPlaces = 4;
            double source = 0;
            double target = 0;
            double euclidean = -1;

            if (GetFirstMapping() != null || GetFirstMapping().Count != 0)
            {
                if (!removeHydrogen)
                {
                    source = ReactantMolecule.Atoms.Count;
                    target = ProductMolecule.Atoms.Count;
                }
                else
                {
                    source = ReactantMolecule.Atoms.Count - GetHCount(ReactantMolecule);
                    target = ProductMolecule.Atoms.Count - GetHCount(ProductMolecule);
                }
                double common = GetFirstMapping().Count;
                euclidean = Math.Sqrt(source + target - 2 * common);
                decimal dist = new Decimal(euclidean);
                dist = decimal.Round(dist, decimalPlaces);
                euclidean = (double)dist;
            }
            return euclidean;
        }

        /// <summary>
        /// <inheritdoc/> (default 0.15 min)
        /// </summary>
        public override double BondSensitiveTimeOut { get; set; } = 0.15;  //mins

        /// <summary>
        /// <inheritdoc/> (default 1.00 min)
        /// </summary>
        public override double BondInSensitiveTimeOut { get; set; } = 1.00;    //mins                                      //mins

        public bool IsMatchBonds { get; set; }

        public List<IDictionary<IBond, IBond>> AllBondMaps
        {
            get
            {
                return allBondMCS;
            }
            set
            {
                this.allBondMCS = value;
            }
        }

        public IDictionary<IBond, IBond> FirstBondMap => firstBondMCS;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="firstBondMCS">The firstBondMCS to set</param>
        private void SetFirstBondMap(IDictionary<IBond, IBond> firstBondMCS)
        {
            this.firstBondMCS = firstBondMCS;
        }
    }
}
