/* Copyright (C) 2011 Mark Rijnbeek <markr@ebi.ac.uk>
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
using NCDK.Common.Base;
using NCDK.Graphs.InChI;
using NCDK.SMSD;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace NCDK.Tautomers
{

    /// <summary>
    /// Creates tautomers for a given input molecule, based on the mobile H atoms listed in the InChI.
    /// Algorithm described in <token>cdk-cite-Thalheim2010</token>.
    /// <para>
    /// <b>Provide your input molecules in Kekule form, and make sure atom type are perceived.</b>
    /// </para>
    /// <para>
    /// When creating an input molecule by reading an MDL file, make sure to set implicit hydrogens. See the
    /// InChITautomerGeneratorTest test case.</para>
    /// </summary>
    // @author Mark Rijnbeek
    // @cdk.module tautomer
    // @cdk.githash
    public class InChITautomerGenerator
    {
        /// <summary>
        /// Public method to get tautomers for an input molecule, based on the InChI which will be calculated by jniinchi.
        /// </summary>
        /// <param name="molecule">molecule for which to generate tautomers</param>
        /// <returns>a list of tautomers, if any</returns>
        /// <exception cref="CDKException"></exception>
        public IList<IAtomContainer> GetTautomers(IAtomContainer molecule)
        {
            InChIGenerator gen = InChIGeneratorFactory.Instance.GetInChIGenerator(molecule);
            string inchi = gen.InChI;
            if (inchi == null)
                throw new CDKException(nameof(InChIGenerator)
                        + " failed to create an InChI for the provided molecule, InChI -> null.");
            return GetTautomers(molecule, inchi);
        }

        /// <summary>
        /// Overloaded <see cref="GetTautomers(IAtomContainer)"/> to get tautomers for an input molecule with the InChI already
        /// provided as input argument.
        /// </summary>
        /// <param name="inputMolecule">and input molecule for which to generate tautomers</param>
        /// <param name="inchi">InChI for the input molecule</param>
        /// <returns>a list of tautomers</returns>
        /// <exception cref="CDKException"></exception>
        public List<IAtomContainer> GetTautomers(IAtomContainer inputMolecule, string inchi)
        {
            //Initial checks
            if (inputMolecule == null || inchi == null)
                throw new CDKException("Please provide a valid input molecule and its corresponding InChI value.");

            List<IAtomContainer> tautomers = new List<IAtomContainer>();
            if (inchi.IndexOf("(H") == -1)
            { //No mobile H atoms according to InChI, so bail out.
                tautomers.Add(inputMolecule);
                return tautomers;
            }

            //Preparation: translate the InChi
            IDictionary<int, IAtom> inchiAtomsByPosition = GetElementsByPosition(inchi, inputMolecule);
            IAtomContainer inchiMolGraph = ConnectAtoms(inchi, inputMolecule, inchiAtomsByPosition);
            List<IAtomContainer> mappedContainers = MapInputMoleculeToInChIMolgraph(inchiMolGraph, inputMolecule);
            inchiMolGraph = mappedContainers[0];
            inputMolecule = mappedContainers[1];
            List<int> mobHydrAttachPositions = new List<int>();
            int totalMobHydrCount = ParseMobileHydrogens(mobHydrAttachPositions, inchi);

            tautomers = ConstructTautomers(inputMolecule, mobHydrAttachPositions, totalMobHydrCount);
            //Remove duplicates
            return RemoveDuplicates(tautomers);
        }

        /// <summary>
        /// Parses the InChI's formula (ignoring hydrogen) and returns a map
        /// with with a position for each atom, increasing in the order
        /// of the elements as listed in the formula.
        /// </summary>
        /// <param name="inputInchi">user input InChI</param>
        /// <param name="inputMolecule">user input molecule</param>
        /// <returns><see cref="IDictionary{TKey, TValue}"/> indicating position and atom</returns>
        private IDictionary<int, IAtom> GetElementsByPosition(string inputInchi, IAtomContainer inputMolecule)
        {
            IDictionary<int, IAtom> inchiAtomsByPosition = new Dictionary<int, IAtom>();
            int position = 0;
            string inchi = inputInchi;

            inchi = inchi.Substring(inchi.IndexOf('/') + 1);
            string formula = inchi.Substring(0, inchi.IndexOf('/'));

            // Test for dots in the formula. For now, bail out when encountered; it
            // would require more sophisticated InChI connection table parsing.
            // Example: what happened to the platinum connectivity below?
            // N.N.O=C1O[Pt]OC(=O)C12CCC2<br>
            // InChI=1S/C6H8O4.2H3N.Pt/c7-4(8)6(5(9)10
            // )2-1-3-6;;;/h1-3H2,(H,7,8)(H,9,10);2*1H3;/q;;;+2/p-2
            if (formula.Contains("."))
                throw new CDKException($"Cannot parse InChI, formula contains dot (unsupported feature). Input formula={formula}");

            Regex formulaPattern = new Regex("\\.?[0-9]*(?<symbol>[A-Z]{1}[a-z]?)(?<cnt>[0-9]*)", RegexOptions.Compiled);
            foreach (Match match in formulaPattern.Matches(formula))
            {
                string elementSymbol = match.Groups["symbol"].Value;
                if (!elementSymbol.Equals("H"))
                {
                    int elementCnt = 1;
                    {
                        string cnt = match.Groups["cnt"].Value;
                        if (cnt != "")
                            elementCnt = int.Parse(cnt);
                    }

                    for (int i = 0; i < elementCnt; i++)
                    {
                        position++;
                        IAtom atom = inputMolecule.Builder.CreateAtom(elementSymbol);

                        // This class uses the atom's ID attribute to keep track of
                        // atom positions defined in the InChi. So if for example
                        // atom.ID=14, it means this atom has position 14 in the
                        // InChI connection table.
                        atom.Id = position + "";
                        inchiAtomsByPosition[position] = atom;
                    }
                }
            }
            return inchiAtomsByPosition;
        }

        /// <summary>
        /// Pops and pushes its ways through the InChI connection table to build up a simple molecule.
        /// </summary>
        /// <param name="inputInchi">user input InChI</param>
        /// <param name="inputMolecule">user input molecule</param>
        /// <param name="inchiAtomsByPosition"></param>
        /// <returns>molecule with single bonds and no hydrogens.</returns>
        private IAtomContainer ConnectAtoms(string inputInchi, IAtomContainer inputMolecule, IDictionary<int, IAtom> inchiAtomsByPosition)
        {
            string inchi = inputInchi;
            inchi = inchi.Substring(inchi.IndexOf('/') + 1);
            inchi = inchi.Substring(inchi.IndexOf('/') + 1);
            string connections = inchi.Substring(1, inchi.IndexOf('/') - 1);
            Regex connectionPattern = new Regex("(-|\\(|\\)|,|([0-9])*)", RegexOptions.Compiled);
            var matches = connectionPattern.Matches(connections);
            var atomStack = new Stack<IAtom>();
            IAtomContainer inchiMolGraph = inputMolecule.Builder.CreateAtomContainer();
            bool pop = false;
            bool push = true;
            foreach (Match match in matches)
            {
                string group = match.Value;
                push = true;
                switch (group)
                {
                    case "":
                        break;
                    case "-":
                        pop = true;
                        push = true;
                        break;
                    case ",":
                        atomStack.Pop();
                        pop = false;
                        push = false;
                        break;
                    case "(":
                        pop = false;
                        push = true;
                        break;
                    case ")":
                        atomStack.Pop();
                        pop = true;
                        push = true;
                        break;
                    default:
                        int position;
                        if (int.TryParse(group, out position))
                        {
                            IAtom atom = inchiAtomsByPosition[position];
                            if (!inchiMolGraph.Contains(atom)) inchiMolGraph.Atoms.Add(atom);
                            IAtom prevAtom = null;
                            if (atomStack.Count != 0)
                            {
                                if (pop)
                                {
                                    prevAtom = atomStack.Pop();
                                }
                                else
                                {
                                    prevAtom = atomStack.Peek();
                                }
                                IBond bond = inputMolecule.Builder.CreateBond(prevAtom, atom,
                                        BondOrder.Single);
                                inchiMolGraph.Bonds.Add(bond);
                            }
                            if (push)
                            {
                                atomStack.Push(atom);
                            }
                        }
                        else
                        {
                            throw new CDKException("Unexpected token " + group + " in connection table encountered.");
                        }
                        break;
                }
            }
            //put any unconnected atoms in the output as well
            foreach (var at in inchiAtomsByPosition.Values)
            {
                if (!inchiMolGraph.Contains(at)) inchiMolGraph.Atoms.Add(at);
            }
            return inchiMolGraph;
        }

        /// <summary>
        /// Atom-atom mapping of the input molecule to the bare container constructed from the InChI connection table.
        /// This makes it possible to map the positions of the mobile hydrogens in the InChI back to the input molecule.
        /// </summary>
        /// <param name="inchiMolGraph">molecule (bare) as defined in InChI</param>
        /// <param name="inputMolecule">user input molecule</param>
        /// <exception cref="CDKException"></exception>
        private List<IAtomContainer> MapInputMoleculeToInChIMolgraph(IAtomContainer inchiMolGraph, IAtomContainer inputMolecule)
        {
            List<IAtomContainer> mappedContainers = new List<IAtomContainer>();
            Isomorphism isomorphism = new Isomorphism(Algorithm.TurboSubStructure, false);
            isomorphism.Init(inchiMolGraph, inputMolecule, true, false);
            isomorphism.SetChemFilters(true, true, true);
            IDictionary<IAtom, IAtom> mapping = isomorphism.GetFirstAtomMapping();
            inchiMolGraph = isomorphism.ReactantMolecule;
            inputMolecule = isomorphism.ProductMolecule;
            foreach (var inchiAtom in inchiMolGraph.Atoms)
            {
                string position = inchiAtom.Id;
                IAtom molAtom = mapping[inchiAtom];
                molAtom.Id = position;
                Debug.WriteLine($"Mapped InChI {inchiAtom.Symbol} {inchiAtom.Id} to {molAtom.Symbol} {molAtom.Id}");
            }
            mappedContainers.Add(inchiMolGraph);
            mappedContainers.Add(inputMolecule);
            return mappedContainers;
        }

        /// <summary>
        /// Parses mobile H Group(s) in an InChI string.
        /// <para>
        /// Multiple InChI sequences of mobile hydrogens are joined into a single sequence (list),
        /// see step 1 of algorithm in paper.
        /// </para>
        /// <para>
        /// Mobile H group has syntax (H[n][-[m]],a1,a2[,a3[,a4...]])
        /// Brackets [ ] surround optional terms.
        /// <list type="bullet">
        ///  <item>Term H[n] stands for 1 or, if the number n (n>1) is present, n mobile hydrogen atoms.</item>
        ///  <item>Term [-[m]], if present, stands for 1 or, if the number m (m>1) is present, m mobile negative charges.</item>
        ///  <item>a1,a2[,a3[,a4...]] are canonical numbers of atoms in the mobile H group.</item>
        ///  <item>no two mobile H groups may have an atom (a canonical number) in common.</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="mobHydrAttachPositions">list of positions where mobile H can attach</param>
        /// <param name="inputInchi">InChI input</param>
        /// <returns>overall count of hydrogens to be dispersed over the positions</returns>
        private int ParseMobileHydrogens(List<int> mobHydrAttachPositions, string inputInchi)
        {
            int totalMobHydrCount = 0;
            string hydrogens = "";
            string inchi = inputInchi;
            if (inchi.IndexOf("/h") != -1)
            {
                hydrogens = inchi.Substring(inchi.IndexOf("/h") + 2);
                if (hydrogens.IndexOf('/') != -1)
                {
                    hydrogens = hydrogens.Substring(0, hydrogens.IndexOf('/'));
                }
                string mobileHydrogens = hydrogens.Substring(hydrogens.IndexOf('('));
                foreach (Match match in mobileHydrPattern.Matches(mobileHydrogens))
                {
                    string mobileHGroup = match.Value;
                    int mobHCount = 0;
                    string head = mobileHGroup.Substring(0, mobileHGroup.IndexOf(',') + 1);
                    if (head.Contains("H,")) head = head.Replace("H,", "H1,");
                    if (head.Contains("-,")) head = head.Replace("-,", "-1,");
                    head = head.Substring(2);
                    
                    // Pragmatically, also add any delocalised neg charge to the
                    // mobile H count. Based on examples like:
                    // C[N+](C)(C)CCCCC\C=C(/NC(=O)C1CC1(Cl)Cl)\C(=O)O ->
                    // ...(H-,18,20,21,22)
                    // COc1cc(N)c(Cl)cc1C(=O)NC2C[N+]3(CCl)CCC2CC3 ->
                    // ...(H2-,19,20,22)
                    foreach (Match subMatch in subPattern.Matches(head))
                    {
                        if (!subMatch.Value.Equals(""))
                        {
                            mobHCount += int.Parse(subMatch.Value);
                        }
                    }
                    totalMobHydrCount += mobHCount;
                    mobileHGroup = mobileHGroup.Substring(mobileHGroup.IndexOf(',') + 1).Replace(")", "");
                    var tokens = mobileHGroup.Split(',');
                    foreach (var token in tokens)
                    {
                        var position = int.Parse(token);
                        mobHydrAttachPositions.Add(position);
                    }
                }
            }
            Debug.WriteLine($"#total mobile hydrogens: {totalMobHydrCount}");
            return totalMobHydrCount;
        }

        private static readonly Regex mobileHydrPattern = new Regex("\\((.)*?\\)", RegexOptions.Compiled);
        private static readonly Regex subPattern = new Regex("[0-9]*", RegexOptions.Compiled);

        /// <summary>
        /// Constructs tautomers following (most) steps of the algorithm in <token>cdk-cite-Thalheim2010</token>.
        /// </summary>
        /// <param name="inputMolecule">input molecule</param>
        /// <param name="mobHydrAttachPositions">mobile H positions</param>
        /// <param name="totalMobHydrCount">count of mobile hydrogens in molecule</param>
        /// <returns>tautomers</returns>
        private List<IAtomContainer> ConstructTautomers(IAtomContainer inputMolecule, List<int> mobHydrAttachPositions, int totalMobHydrCount)
        {
            List<IAtomContainer> tautomers = new List<IAtomContainer>();

            //Tautomeric skeleton generation
            IAtomContainer skeleton = (IAtomContainer)inputMolecule.Clone();

            bool atomsToRemove = true;
            List<IAtom> removedAtoms = new List<IAtom>();
            bool atomRemoved = false;
            while (atomsToRemove)
            {
                ATOMS: foreach (var atom in skeleton.Atoms)
                {
                    atomRemoved = false;
                    int position = int.Parse(atom.Id);
                    if (!mobHydrAttachPositions.Contains(position)
                            && atom.Hybridization.Equals(Hybridization.SP3))
                    {
                        skeleton.Atoms.Remove(atom);
                        removedAtoms.Add(atom);
                        atomRemoved = true;
                        goto break_ATOMS;
                    }
                    else
                    {
                        foreach (var bond in skeleton.Bonds)
                        {
                            if (bond.Contains(atom) && bond.Order.Equals(BondOrder.Triple))
                            {
                                skeleton.Atoms.Remove(atom);
                                removedAtoms.Add(atom);
                                atomRemoved = true;
                                goto break_ATOMS;
                            }
                        }
                    }
                }
            break_ATOMS:
                if (!atomRemoved) atomsToRemove = false;

            }
            bool bondsToRemove = true;
            bool bondRemoved = false;
            while (bondsToRemove)
            {
                BONDS: foreach (var bond in skeleton.Bonds)
                {
                    bondRemoved = false;
                    foreach (var removedAtom in removedAtoms)
                    {
                        if (bond.Contains(removedAtom))
                        {
                            IAtom other = bond.GetConnectedAtom(removedAtom);
                            int decValence = 0;
                            switch (bond.Order.Numeric)
                            {
                                case 1:
                                case 2:
                                case 3:
                                case 4:
                                    decValence = bond.Order.Numeric;
                                    break;
                                default:
                                    break;
                            }
                            other.Valency = other.Valency - decValence;
                            skeleton.Bonds.Remove(bond);
                            bondRemoved = true;
                            goto break_BONDS;
                        }
                    }
                }
                break_BONDS:
                if (!bondRemoved) bondsToRemove = false;

            }
            int doubleBondCount = 0;
            foreach (var bond in skeleton.Bonds)
            {
                if (bond.Order.Equals(BondOrder.Double))
                {
                    doubleBondCount++;
                }
            }

            foreach (var hPosition in mobHydrAttachPositions)
            {
                IAtom atom = FindAtomByPosition(skeleton, hPosition);
                atom.ImplicitHydrogenCount = 0;
            }

            foreach (var bond in skeleton.Bonds)
            {
                if (bond.Order.Equals(BondOrder.Double))
                {
                    bond.Order = BondOrder.Single;
                }
            }

            // Make combinations for mobile Hydrogen attachments
            var combinations = new List<IList<int>>();
            CombineHydrogenPositions(new List<int>(), combinations, skeleton, totalMobHydrCount,
                    mobHydrAttachPositions);

            Stack<object> solutions = new Stack<object>();
            foreach (var hPositions in combinations)
            {
                IAtomContainer tautomerSkeleton = (IAtomContainer)skeleton.Clone();

                foreach (var hPos in hPositions)
                {
                    IAtom atom = FindAtomByPosition(tautomerSkeleton, hPos);
                    atom.ImplicitHydrogenCount = atom.ImplicitHydrogenCount + 1;
                }
                List<IAtom> atomsInNeedOfFix = new List<IAtom>();
                foreach (var atom in tautomerSkeleton.Atoms)
                {
                    if (atom.Valency - atom.FormalCharge != atom.ImplicitHydrogenCount
                            + GetConnectivity(atom, tautomerSkeleton))
                        atomsInNeedOfFix.Add(atom);
                }
                var dblBondPositions = TryDoubleBondCombinations(tautomerSkeleton, 0, 0, doubleBondCount,
                              atomsInNeedOfFix);
                if (dblBondPositions != null)
                {
                    //Found a valid double bond combination for this mobile hydrogen configuration..
                    solutions.Push(dblBondPositions);
                    solutions.Push(tautomerSkeleton);
                }
            }
            Debug.WriteLine($"#possible solutions : {solutions.Count}");
            if (solutions.Count == 0)
            {
                Trace.TraceError("Could not generate any tautomers for the input. Is input in Kekule form? ");
                tautomers.Add(inputMolecule);
            }
            else
            {

                while (solutions.Count != 0)
                {
                    IAtomContainer tautomerSkeleton = (IAtomContainer)solutions.Pop();
                    var dblBondPositions = (List<int>)solutions.Pop();
                    IAtomContainer tautomer = (IAtomContainer)inputMolecule.Clone();
                    foreach (var skAtom1 in tautomerSkeleton.Atoms)
                    {
                        foreach (var atom1 in tautomer.Atoms)
                        {
                            if (atom1.Id.Equals(skAtom1.Id))
                            {
                                atom1.ImplicitHydrogenCount = skAtom1.ImplicitHydrogenCount;
                                for (int bondIdx = 0; bondIdx < tautomerSkeleton.Bonds.Count; bondIdx++)
                                {
                                    IBond skBond = tautomerSkeleton.Bonds[bondIdx];
                                    if (skBond.Contains(skAtom1))
                                    {
                                        IAtom skAtom2 = skBond.GetConnectedAtom(skAtom1);
                                        foreach (var atom2 in tautomer.Atoms)
                                        {
                                            if (atom2.Id.Equals(skAtom2.Id))
                                            {
                                                IBond tautBond = tautomer.GetBond(atom1, atom2);
                                                if (dblBondPositions.Contains(bondIdx))
                                                    tautBond.Order = BondOrder.Double;
                                                else
                                                    tautBond.Order = BondOrder.Single;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    foreach (var atom in tautomer.Atoms)
                    {
                        atom.IsAromatic = false;
                        atom.Valency = null;
                    }
                    foreach (var bond in tautomer.Bonds)
                        bond.IsAromatic = false;
                    tautomers.Add(tautomer);
                }
            }
            Debug.WriteLine($"# initial tautomers generated : {tautomers.Count}");
            return tautomers;
        }

        /// <summary>
        /// Removes duplicates from a molecule set. Uses SMSD to detect identical molecules.
        /// An example of pruning can be a case where double bonds are placed in different positions in
        /// an aromatic (Kekule) ring, which all amounts to one same aromatic ring.
        /// </summary>
        /// <param name="tautomers">molecule set of tautomers with possible duplicates</param>
        /// <returns>tautomers same set with duplicates removed</returns>
        /// <exception cref="CDKException"></exception>
        private List<IAtomContainer> RemoveDuplicates(List<IAtomContainer> tautomers)
        {
            List<IAtomContainer> unique = new List<IAtomContainer>();
            Isomorphism isomorphism = new Isomorphism(Algorithm.TurboSubStructure, true);
            BitArray removed = new BitArray(tautomers.Count);
            for (int idx1 = 0; idx1 < tautomers.Count; idx1++)
            {
                if (removed[idx1])
                {
                    continue;
                }
                IAtomContainer tautomer1 = tautomers[idx1];
                for (int idx2 = idx1 + 1; idx2 < tautomers.Count; idx2++)
                {
                    if (removed[idx2])
                    {
                        continue;
                    }
                    IAtomContainer tautomer2 = tautomers[idx2];
                    isomorphism.Init(tautomer1, tautomer2, false, false);
                    isomorphism.SetChemFilters(true, true, true);
                    if (isomorphism.IsSubgraph())
                    {
                        removed.Set(idx2, true);
                    }
                }
                unique.Add(tautomer1);
            }
            Debug.WriteLine($"# tautomers after clean up : {tautomers.Count}");
            return unique;
        }

        /// <summary>
        /// Makes combinations recursively of all possible mobile Hydrogen positions.
        /// </summary>
        /// <param name="taken">positions taken by hydrogen</param>
        /// <param name="combinations">combinations made so far</param>
        /// <param name="skeleton">container to work on</param>
        /// <param name="totalMobHydrCount"></param>
        /// <param name="mobHydrAttachPositions"></param>
        private void CombineHydrogenPositions(IList<int> taken, List<IList<int>> combinations, IAtomContainer skeleton, int totalMobHydrCount, IList<int> mobHydrAttachPositions)
        {
            if (taken.Count != totalMobHydrCount)
            {
                for (int i = 0; i < mobHydrAttachPositions.Count; i++)
                {
                    int pos = mobHydrAttachPositions[i];
                    IAtom atom = FindAtomByPosition(skeleton, pos);
                    int conn = GetConnectivity(atom, skeleton);
                    int hCnt = 0;
                    foreach (var t in taken)
                        if (t == pos) hCnt++;
                    if (atom.Valency - atom.FormalCharge > (hCnt + conn))
                    {
                        taken.Add(pos);
                        CombineHydrogenPositions(taken, combinations, skeleton, totalMobHydrCount, mobHydrAttachPositions);
                        taken.RemoveAt(taken.Count - 1);
                    }
                }
            }
            else
            {
                var addList = new List<int>(taken.Count);
                addList.AddRange(taken);
                addList.Sort();
                if (!combinations.Any(n => Compares.AreDeepEqual(n, addList)))
                {
                    combinations.Add(addList);
                }
            }
        }

        /// <summary>
        /// Helper method that locates an atom based on its InChI atom table
        /// position, which has been set as ID.
        /// </summary>
        /// <param name="container">input container</param>
        /// <param name="position">InChI atom table position</param>
        /// <returns>atom on the position</returns>
        private IAtom FindAtomByPosition(IAtomContainer container, int position)
        {
            string pos = position.ToString();
            foreach (var atom in container.Atoms)
            {
                if (atom.Id.Equals(pos)) return atom;
            }
            return null;
        }

        /// <summary>
        /// Tries double bond combinations for a certain input container of which the double bonds have been stripped
        /// around the mobile hydrogen positions. Recursively.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="dblBondsAdded">counts double bonds added so far</param>
        /// <param name="bondOffSet">offset for next double bond position to consider</param>
        /// <param name="doubleBondMax">maximum number of double bonds to add</param>
        /// <param name="atomsInNeedOfFix">atoms that require more bonds</param>
        /// <returns>a list of double bond positions (index) that make a valid combination, null if none found</returns>
        private List<int> TryDoubleBondCombinations(IAtomContainer container, int dblBondsAdded, int bondOffSet,
                int doubleBondMax, List<IAtom> atomsInNeedOfFix)
        {
            int offSet = bondOffSet;
            List<int> dblBondPositions = null;

            while (offSet < container.Bonds.Count && dblBondPositions == null)
            {
                IBond bond = container.Bonds[offSet];
                if (atomsInNeedOfFix.Contains(bond.Atoms[0]) && atomsInNeedOfFix.Contains(bond.Atoms[1]))
                {
                    bond.Order = BondOrder.Double;
                    dblBondsAdded = dblBondsAdded + 1;
                    if (dblBondsAdded == doubleBondMax)
                    {
                        bool validDoubleBondConfig = true;
                        CHECK: foreach (var atom in container.Atoms)
                        {
                            if (atom.Valency.Value != atom.ImplicitHydrogenCount + GetConnectivity(atom, container))
                            {
                                validDoubleBondConfig = false;
                                goto break_CHECK;
                            }
                        }
                        break_CHECK:
                        if (validDoubleBondConfig)
                        {
                            dblBondPositions = new List<int>();
                            for (int idx = 0; idx < container.Bonds.Count; idx++)
                            {
                                if (container.Bonds[idx].Order.Equals(BondOrder.Double))
                                    dblBondPositions.Add(idx);
                            }
                            return dblBondPositions;
                        }
                    }
                    else
                    {
                        dblBondPositions = TryDoubleBondCombinations(container, dblBondsAdded, offSet + 1, doubleBondMax,
                                atomsInNeedOfFix);
                    }

                    bond.Order = BondOrder.Single;
                    dblBondsAdded = dblBondsAdded - 1;
                }
                offSet++;
            }
            return dblBondPositions;
        }

        /// <summary>
        /// Sums the number of bonds (counting order) an atom is hooked up with.
        /// </summary>
        /// <param name="atom">an atom in the container</param>
        /// <param name="container">the container</param>
        /// <returns>valence (bond order sum) of the atom</returns>
        private int GetConnectivity(IAtom atom, IAtomContainer container)
        {
            int connectivity = 0;
            foreach (var bond in container.Bonds)
            {
                if (bond.Contains(atom))
                {
                    switch (bond.Order.Numeric)
                    {
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                            connectivity += bond.Order.Numeric;
                            break;
                        default:
                            connectivity += 10;
                            break;
                    }
                }
            }
            return connectivity;
        }
    }
}
