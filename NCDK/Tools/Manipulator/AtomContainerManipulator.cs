/* 
 * Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
 *                    2014  Mark B Vine (orcid:0000-0002-7794-0426)
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
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *  */
using NCDK.AtomTypes;
using NCDK.Config;
using NCDK.Graphs;
using NCDK.RingSearches;
using NCDK.SGroups;
using NCDK.Stereo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace NCDK.Tools.Manipulator
{
    /// <summary>
    /// Class with convenience methods that provide methods to manipulate
    /// AtomContainer's. 
    /// </summary>
    /// <example>
    /// For example:
    /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Tools.AtomContainerManipulator_Example.cs+1"]/*' />
    /// will replace the Atom in the AtomContainer, but in all the ElectronContainer's
    /// it participates too.
    /// </example>
    // @cdk.module standard
    // @cdk.githash
    // @author  Egon Willighagen
    // @cdk.created 2003-08-07
    public class AtomContainerManipulator
    {
        /// <summary>
        /// Extract a substructure from an atom container, in the form of a new
        /// cloned atom container with only the atoms with indices in atomIndices and
        /// bonds that connect these atoms.
        /// </summary>
        /// <remarks>
        /// Note that this may result in a disconnected atom container.
        /// </remarks>
        /// <param name="atomContainer">the source container to extract from</param>
        /// <param name="atomIndices">the indices of the substructure</param>
        /// <returns>a cloned atom container with a substructure of the source</returns>
        public static IAtomContainer ExtractSubstructure(IAtomContainer atomContainer, params int[] atomIndices)
        {
            IAtomContainer substructure = (IAtomContainer)atomContainer.Clone();
            int numberOfAtoms = substructure.Atoms.Count;
            IAtom[] atoms = new IAtom[numberOfAtoms];
            for (int atomIndex = 0; atomIndex < numberOfAtoms; atomIndex++)
            {
                atoms[atomIndex] = substructure.Atoms[atomIndex];
            }
            Array.Sort(atomIndices);
            for (int index = 0; index < numberOfAtoms; index++)
            {
                if (Array.BinarySearch(atomIndices, index) < 0)
                {
                    IAtom atom = atoms[index];
                    substructure.RemoveAtom(atom);
                }
            }

            return substructure;
        }

        /// <summary>
        /// Returns an atom in an atomcontainer identified by id
        /// </summary>
        /// <param name="ac">The AtomContainer to search in</param>
        /// <param name="id">The id to search for</param>
        /// <returns>An atom having id id</returns>
        /// <exception cref="CDKException">There is no such atom</exception>
        public static IAtom GetAtomById(IAtomContainer ac, string id)
        {
            for (int i = 0; i < ac.Atoms.Count; i++)
            {
                if (ac.Atoms[i].Id != null && ac.Atoms[i].Id.Equals(id)) return ac.Atoms[i];
            }
            throw new CDKException("no suc atom");
        }

        /// <summary>
        /// Substitute one atom in a container for another adjusting bonds, single electrons, lone pairs, and stereochemistry
        /// as required.
        /// </summary>
        /// <param name="container">the container to replace the atom of</param>
        /// <param name="oldAtom">the atom to replace</param>
        /// <param name="newAtom">the atom to insert</param>
        /// <returns>whether replacement was made</returns>
        public static bool ReplaceAtomByAtom(IAtomContainer container, IAtom oldAtom, IAtom newAtom)
        {
            if (oldAtom == null)
                throw new ArgumentNullException(nameof(oldAtom), "Atom to be replaced was null!");
            if (newAtom == null)
                throw new ArgumentNullException(nameof(newAtom), "Replacement atom was null!");
             int idx = container.Atoms.IndexOf(oldAtom);
            if (idx < 0)
                return false;
            container.Atoms[idx]  = newAtom;
            var sgrougs = container.GetProperty<IList<Sgroup>>(CDKPropertyName.CtabSgroups);
            if (sgrougs != null)
            {
                bool updated = false;
                List<Sgroup> replaced = new List<Sgroup>();
                foreach (Sgroup org in sgrougs)
                {
                    if (org.Atoms.Contains(oldAtom))
                    {
                        updated = true;
                        Sgroup cpy = new Sgroup();
                        foreach (IAtom atom in org.Atoms)
                        {
                            if (!oldAtom.Equals(atom))
                                cpy.Atoms.Add(atom);
                            else
                                cpy.Atoms.Add(newAtom);
                        }
                        foreach (IBond bond in org.Bonds)
                            cpy.Bonds.Add(bond);
                        foreach (Sgroup parent in org.Parents)
                            cpy.AddParent(parent);
                        foreach (SgroupKey key in org.AttributeKeys)
                            cpy.PutValue(key, org.GetValue(key));
                        replaced.Add(cpy);
                    }
                    else
                    {
                        replaced.Add(org);
                    }
                }
                if (updated)
                {
                    container.SetProperty(CDKPropertyName.CtabSgroups,
                        new ReadOnlyCollection<Sgroup>(replaced));
                }
            }

            return true;
        }

        /// <summary>
        /// Get the summed charge of all atoms in an AtomContainer
        /// </summary>
        /// <param name="atomContainer">The IAtomContainer to manipulate</param>
        /// <returns>The summed charges of all atoms in this AtomContainer.</returns>
        public static double GetTotalCharge(IAtomContainer atomContainer)
        {
            double charge = 0.0;
            foreach (var atom in atomContainer.Atoms)
            {
                // we assume CDKConstant.Unset is equal to 0
                double? thisCharge = atom.Charge;
                if (thisCharge.HasValue) charge += thisCharge.Value;
            }
            return charge;
        }

        /// <summary>
        /// Get the summed exact mass of all atoms in an AtomContainer. It
        /// requires isotope information for all atoms to be set. Either set
        /// this information using the <see cref="IsotopeFactory"/>, or use the
        /// <see cref="MolecularFormulaManipulator.GetMajorIsotopeMass(IMolecularFormula)"/> 
        /// method, after converting the <see cref="IAtomContainer"/> to a
        /// <see cref="IMolecularFormula"/> with the <see cref="MolecularFormulaManipulator"/>.
        /// </summary>
        /// <param name="atomContainer">The IAtomContainer to manipulate</param>
        /// <returns>The summed exact mass of all atoms in this AtomContainer.</returns>
        public static double GetTotalExactMass(IAtomContainer atomContainer)
        {
            try
            {
                Isotopes isotopes = Isotopes.Instance;
                double mass = 0.0;
                double hExactMass = isotopes.GetMajorIsotope(1).ExactMass.Value;
                foreach (var atom in atomContainer.Atoms)
                {
                    if (!atom.ImplicitHydrogenCount.HasValue)
                        throw new ArgumentException("an atom had with unknown (null) implicit hydrogens");
                    mass += atom.ExactMass.Value;
                    mass += atom.ImplicitHydrogenCount.Value * hExactMass;
                }
                return mass;
            }
            catch (IOException e)
            {
                throw new IOException("Isotopes definitions could not be loaded", e);
            }
        }

        /// <summary>
        /// Returns the molecular mass of the IAtomContainer. For the calculation it
        /// uses the masses of the isotope mixture using natural abundances.
        /// </summary>
        /// <param name="atomContainer"></param>
        // @cdk.keyword mass, molecular
        public static double GetNaturalExactMass(IAtomContainer atomContainer)
        {
            try
            {
                Isotopes isotopes = Isotopes.Instance;
                double hydgrogenMass = isotopes.GetNaturalMass(Elements.Hydrogen.ToIElement());

                double mass = 0.0;
                foreach (var atom in atomContainer.Atoms)
                {

                    if (atom.AtomicNumber == null)
                        throw new ArgumentException("an atom had with unknown (null) atomic number");
                    if (atom.ImplicitHydrogenCount == null)
                        throw new ArgumentException("an atom had with unknown (null) implicit hydrogens");

                    mass += isotopes.GetNaturalMass(Elements.OfNumber(atom.AtomicNumber.Value).ToIElement());
                    mass += hydgrogenMass * atom.ImplicitHydrogenCount.Value;
                }
                return mass;

            }
            catch (IOException e)
            {
                throw new IOException("Isotopes definitions could not be loaded", e);
            }
        }

        /// <summary>
        /// Get the summed natural abundance of all atoms in an AtomContainer
        /// </summary>
        /// <param name="atomContainer">The IAtomContainer to manipulate</param>
        /// <returns>The summed natural abundance of all atoms in this AtomContainer.</returns>
        public static double GetTotalNaturalAbundance(IAtomContainer atomContainer)
        {
            try
            {
                Isotopes isotopes = Isotopes.Instance;
                double abundance = 1.0;
                double hAbundance = isotopes.GetMajorIsotope(1).NaturalAbundance.Value;

                int nImplH = 0;

                foreach (var atom in atomContainer.Atoms)
                {
                    if (!atom.ImplicitHydrogenCount.HasValue)
                        throw new ArgumentException("an atom had with unknown (null) implicit hydrogens");
                    abundance *= atom.NaturalAbundance.Value;
                    for (int h = 0; h < atom.ImplicitHydrogenCount.Value; h++)
                        abundance *= hAbundance;
                    nImplH += atom.ImplicitHydrogenCount.Value;
                }
                return abundance / Math.Pow(100, nImplH + atomContainer.Atoms.Count);
            }
            catch (IOException e)
            {
                throw new IOException("Isotopes definitions could not be loaded", e);
            }
        }

        /// <summary>
        /// Get the total formal charge on a molecule.
        /// </summary>
        /// <param name="atomContainer">the atom container to consider</param>
        /// <returns>The summed formal charges of all atoms in this AtomContainer.</returns>
        public static int GetTotalFormalCharge(IAtomContainer atomContainer)
        {
            int chargeP = GetTotalNegativeFormalCharge(atomContainer);
            int chargeN = GetTotalPositiveFormalCharge(atomContainer);

            return chargeP + chargeN;
        }

        /// <summary>
        /// Get the total formal negative charge on a molecule.
        /// </summary>
        /// <param name="atomContainer">the atom container to consider</param>
        /// <returns>The summed negative formal charges of all atoms in this AtomContainer.</returns>
        public static int GetTotalNegativeFormalCharge(IAtomContainer atomContainer)
        {
            int charge = 0;
            for (int i = 0; i < atomContainer.Atoms.Count; i++)
            {
                int chargeI = atomContainer.Atoms[i].FormalCharge.Value;
                if (chargeI < 0) charge += chargeI;
            }
            return charge;
        }

        /// <summary>
        /// Get the total positive formal charge on a molecule.
        /// </summary>
        /// <param name="atomContainer">the atom container to consider</param>
        /// <returns>The summed positive formal charges of all atoms in this AtomContainer.</returns>
        public static int GetTotalPositiveFormalCharge(IAtomContainer atomContainer)
        {
            int charge = 0;
            for (int i = 0; i < atomContainer.Atoms.Count; i++)
            {
                int chargeI = atomContainer.Atoms[i].FormalCharge.Value;
                if (chargeI > 0) charge += chargeI;
            }
            return charge;
        }

        /// <summary>
        /// Counts the number of hydrogens on the provided IAtomContainer. As this
        /// method will sum all implicit hydrogens on each atom it is important to
        /// ensure the atoms have already been perceived (and thus have an implicit
        /// hydrogen count) (see. <see cref="PercieveAtomTypesAndConfigureAtoms(IAtomContainer)"/>).
        /// </summary>
        /// <param name="container">the container to count the hydrogens on</param>
        /// <returns>the total number of hydrogens</returns>
        /// <seealso cref="IAtom.ImplicitHydrogenCount"/>
        /// <seealso cref="PercieveAtomTypesAndConfigureAtoms"/>
        /// <exception cref="ArgumentNullException">if the provided container was null</exception>
        public static int GetTotalHydrogenCount(IAtomContainer container)
        {
            if (container == null) throw new ArgumentNullException("null container provided");
            int hydrogens = 0;
            foreach (var atom in container.Atoms)
            {
                if (Elements.Hydrogen.Symbol.Equals(atom.Symbol))
                {
                    hydrogens++;
                }

                // rare but a hydrogen may have an implicit hydrogen so we don't use 'else'
                int? implicit_ = atom.ImplicitHydrogenCount;
                if (implicit_.HasValue)
                {
                    hydrogens += implicit_.Value;
                }
            }
            return hydrogens;
        }

        /// <summary>
        /// Counts the number of implicit hydrogens on the provided IAtomContainer.
        /// As this method will sum all implicit hydrogens on each atom it is
        /// important to ensure the atoms have already been perceived (and thus have
        /// an implicit hydrogen count) (see. <see cref="PercieveAtomTypesAndConfigureAtoms(IAtomContainer)"/>.
        /// </summary>
        /// <param name="container">the container to count the implicit hydrogens on</param>
        /// <returns>the total number of implicit hydrogens</returns>
        /// <seealso cref="IAtom.ImplicitHydrogenCount"/>
        /// <seealso cref="PercieveAtomTypesAndConfigureAtoms"/>
        /// <exception cref="ArgumentNullException">if the provided container was null</exception>
        public static int GetImplicitHydrogenCount(IAtomContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container), "null container provided");
            int count = 0;
            foreach (var atom in container.Atoms)
            {
                int? implicit_ = atom.ImplicitHydrogenCount;
                if (implicit_.HasValue)
                {
                    count += implicit_.Value;
                }
            }
            return count;
        }

        /// <summary>
        /// Count explicit hydrogens.
        /// </summary>
        /// <param name="atomContainer">the atom container to consider</param>
        /// <returns>The number of explicit hydrogens on the given IAtom.</returns>
        /// <exception cref="ArgumentNullException">if the provided container was null</exception>
        public static int CountExplicitHydrogens(IAtomContainer atomContainer, IAtom atom)
        {
            if (atomContainer == null || atom == null)
                throw new ArgumentException("null container or atom provided");
            int hCount = 0;
            foreach (var connected in atomContainer.GetConnectedAtoms(atom))
            {
                if (Elements.Hydrogen.Symbol.Equals(connected.Symbol))
                {
                    hCount++;
                }
            }
            return hCount;
        }

        /// <summary>
        /// Adds explicit hydrogens (without coordinates) to the IAtomContainer,
        /// equaling the number of set implicit hydrogens.
        /// </summary>
        /// <param name="atomContainer">the atom container to consider</param>
        // @cdk.keyword hydrogens, adding
        public static void ConvertImplicitToExplicitHydrogens(IAtomContainer atomContainer)
        {
            IList<IAtom> hydrogens = new List<IAtom>();
            IList<IBond> newBonds = new List<IBond>();

            // store a single explicit hydrogen of each original neighbor
            IDictionary<IAtom, IAtom> hNeighbor = new Dictionary<IAtom, IAtom>();

            foreach (var atom in atomContainer.Atoms)
            {
                if (!atom.Symbol.Equals("H"))
                {
                    int? hCount = atom.ImplicitHydrogenCount;
                    if (hCount != null)
                    {
                        for (int i = 0; i < hCount; i++)
                        {
                            IAtom hydrogen = atom.Builder.CreateAtom("H");
                            hydrogen.AtomTypeName = "H";
                            hydrogen.ImplicitHydrogenCount = 0;
                            hydrogens.Add(hydrogen);
                            newBonds.Add(atom.Builder.CreateBond(atom, hydrogen, BondOrder.Single));
                            if (!hNeighbor.ContainsKey(atom)) hNeighbor.Add(atom, hydrogen);
                        }
                        atom.ImplicitHydrogenCount = 0;
                    }
                }
            }
            foreach (var atom in hydrogens)
                atomContainer.Atoms.Add(atom);
            foreach (var bond in newBonds)
                atomContainer.Bonds.Add(bond);

            // update tetrahedral elements with an implicit part
            List<IStereoElement> stereos = new List<IStereoElement>();
            foreach (IStereoElement stereo in atomContainer.StereoElements)
            {
                if (stereo is ITetrahedralChirality)
                {
                    ITetrahedralChirality tc = (ITetrahedralChirality)stereo;

                    IAtom focus = tc.ChiralAtom;
                    var neighbors = tc.Ligands;
                    IAtom hydrogen;

                    // in sulfoxide - the implicit part of the tetrahedral centre
                    // is a lone pair

                    if (hNeighbor.TryGetValue(focus, out hydrogen))
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (neighbors[i] == focus)
                            {
                                neighbors[i] = hydrogen;
                                break;
                            }
                        }
                        // neighbors is a copy so need to create a new stereocenter
                        stereos.Add(new TetrahedralChirality(focus, neighbors, tc.Stereo));
                    }
                    else
                    {
                        stereos.Add(stereo);
                    }
                }
                else
                {
                    stereos.Add(stereo);
                }
            }
            atomContainer.SetStereoElements(stereos);
        }

        /// <returns>The summed implicit + explicit hydrogens of the given IAtom.</returns>
        public static int CountHydrogens(IAtomContainer atomContainer, IAtom atom)
        {
            int hCount = atom.ImplicitHydrogenCount ?? 0;
            hCount += CountExplicitHydrogens(atomContainer, atom);
            return hCount;
        }

        public static IList<string> GetAllIDs(IAtomContainer mol)
        {
            IList<string> idList = new List<string>();
            if (mol != null)
            {
                if (mol.Id != null) idList.Add(mol.Id);
                foreach (var atom in mol.Atoms)
                {
                    if (atom.Id != null) idList.Add(atom.Id);
                }

                foreach (var bond in mol.Bonds)
                {
                    if (bond.Id != null) idList.Add(bond.Id);
                }
            }
            return idList;
        }

        /// <summary>
        /// Produces an AtomContainer without explicit non stereo-relevant Hs but with H count from one with Hs.
        /// The new molecule is a deep copy.
        /// </summary>
        /// <param name="org">The AtomContainer from which to remove the hydrogens</param>
        /// <returns>The molecule without non stereo-relevant Hs.</returns>
        // @cdk.keyword         hydrogens, removal
        public static IAtomContainer RemoveNonChiralHydrogens(IAtomContainer org)
        {
            IDictionary<IAtom, IAtom> map = new Dictionary<IAtom, IAtom>(); // maps original atoms to clones.
            IList<IAtom> remove = new List<IAtom>(); // lists removed Hs.

            // Clone atoms except those to be removed.
            IAtomContainer cpy = org.Builder.CreateAtomContainer();
            int count = org.Atoms.Count;

            for (int i = 0; i < count; i++)
            {
                // Clone/remove this atom?
                IAtom atom = org.Atoms[i];
                bool addToRemove = false;
                if (SuppressibleHydrogen(org, atom))
                {
                    // test whether connected to a single hetero atom only, otherwise keep
                    if (org.GetConnectedAtoms(atom).Count() == 1)
                    {
                        IAtom neighbour = org.GetConnectedAtoms(atom).ElementAt(0);
                        // keep if the neighbouring hetero atom has stereo information, otherwise continue checking
                        int? stereoParity = neighbour.StereoParity;
                        if (stereoParity == null || stereoParity == 0)
                        {
                            addToRemove = true;
                            // keep if any of the bonds of the hetero atom have stereo information
                            foreach (var bond in org.GetConnectedBonds(neighbour))
                            {
                                BondStereo bondStereo = bond.Stereo;
                                if (bondStereo != BondStereo.None) addToRemove = false;
                                IAtom neighboursNeighbour = bond.GetOther(neighbour);
                                // remove in any case if the hetero atom is connected to more than one hydrogen
                                if (neighboursNeighbour.Symbol.Equals("H") && neighboursNeighbour != atom)
                                {
                                    addToRemove = true;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (addToRemove)
                    remove.Add(atom);
                else
                    AddClone(atom, cpy, map);
            }

            // rescue any false positives, i.e., hydrogens that are stereo-relevant
            // the use of IStereoElement is not fully integrated yet to describe stereo information
            foreach (var stereoElement in org.StereoElements)
            {
                if (stereoElement is ITetrahedralChirality)
                {
                    ITetrahedralChirality tetChirality = (ITetrahedralChirality)stereoElement;
                    foreach (var atom in tetChirality.Ligands)
                    {
                        if (atom.Symbol.Equals("H") && remove.Contains(atom))
                        {
                            remove.Remove(atom);
                            AddClone(atom, cpy, map);
                        }
                    }
                }
                else if (stereoElement is IDoubleBondStereochemistry)
                {
                    IDoubleBondStereochemistry dbs = (IDoubleBondStereochemistry)stereoElement;
                    IBond stereoBond = dbs.StereoBond;
                    foreach (var neighbor in org.GetConnectedAtoms(stereoBond.Begin))
                    {
                        if (remove.Remove(neighbor)) AddClone(neighbor, cpy, map);
                    }
                    foreach (var neighbor in org.GetConnectedAtoms(stereoBond.End))
                    {
                        if (remove.Remove(neighbor)) AddClone(neighbor, cpy, map);
                    }
                }
            }

            // Clone bonds except those involving removed atoms.
            count = org.Bonds.Count;
            for (int i = 0; i < count; i++)
            {
                // Check bond.
                IBond bond = org.Bonds[i];
                bool removedBond = false;
                int length = bond.Atoms.Count;
                for (int k = 0; k < length; k++)
                {
                    if (remove.Contains(bond.Atoms[k]))
                    {
                        removedBond = true;
                        break;
                    }
                }

                // Clone/remove this bond?
                if (!removedBond)
                {
                    IBond clone = null;
                    clone = (IBond)org.Bonds[i].Clone();
                    Trace.Assert(clone != null);
                    clone.SetAtoms(new[] { map[bond.Begin], map[bond.End] });
                    cpy.Bonds.Add(clone);
                }
            }

            // Recompute hydrogen counts of neighbours of removed Hydrogens.
            foreach (var aRemove in remove)
            {
                // Process neighbours.
                foreach (var iAtom in org.GetConnectedAtoms(aRemove))
                {

                    IAtom neighb;
                    if (!map.TryGetValue(iAtom, out neighb))
                        continue; // since for the case of H2, neight H has a heavy atom neighbor
                    neighb.ImplicitHydrogenCount = (neighb.ImplicitHydrogenCount ?? 0) + 1;
                }
            }
            foreach (var atom in cpy.Atoms)
            {
                if (atom.ImplicitHydrogenCount == null) atom.ImplicitHydrogenCount = 0;
            }
            cpy.AddProperties(org.GetProperties());
            ChemObjectFlagBag.Transfer(org, cpy);

            return (cpy);
        }

        private static void AddClone(IAtom atom, IAtomContainer mol, IDictionary<IAtom, IAtom> map)
        {
            IAtom clonedAtom = null;
            clonedAtom = (IAtom)atom.Clone();
            mol.Atoms.Add(clonedAtom);
            map.Add(atom, clonedAtom);
        }

        /// <summary>
        /// Copy the input container and suppress any explicit hydrogens. Only
        /// hydrogens that can be represented as a hydrogen count value on the atom
        /// are suppressed. If a copy is not needed please use <see cref="SuppressHydrogens"/>.
        /// </summary>
        /// <param name="org">the container from which to remove hydrogens</param>
        /// <returns>a copy of the input with suppressed hydrogens</returns>
        /// <seealso cref="SuppressHydrogens"/>
        public static IAtomContainer CopyAndSuppressedHydrogens(IAtomContainer org)
        {
            return SuppressHydrogens((IAtomContainer)org.Clone());
        }

        /// <summary>
        /// Suppress any explicit hydrogens in the provided container. Only hydrogens
        /// that can be represented as a hydrogen count value on the atom are
        /// suppressed. The container is updated and no elements are copied, please
        /// use either <see cref="CopyAndSuppressedHydrogens(IAtomContainer)"/> if you would to preserve
        /// the old instance.
        /// </summary>
        /// <param name="org">the container from which to remove hydrogens</param>
        /// <returns>the input for convenience</returns>
        /// <seealso cref="CopyAndSuppressedHydrogens"/>
        public static IAtomContainer SuppressHydrogens(IAtomContainer org)
        {
            bool anyHydrogenPresent = false;
            foreach (var atom in org.Atoms)
            {
                if ("H".Equals(atom.Symbol))
                {
                    anyHydrogenPresent = true;
                    break;
                }
            }

            if (!anyHydrogenPresent) return org;

            // we need fast adjacency checks (to check for suppression and
            // update hydrogen counts)
            GraphUtil.EdgeToBondMap bondmap = GraphUtil.EdgeToBondMap.WithSpaceFor(org);
            int[][] graph = GraphUtil.ToAdjList(org, bondmap);

            int nOrgAtoms = org.Atoms.Count;
            int nOrgBonds = org.Bonds.Count;

            int nCpyAtoms = 0;
            int nCpyBonds = 0;

            ICollection<IAtom> hydrogens = new HashSet<IAtom>();
            IAtom[] cpyAtoms = new IAtom[nOrgAtoms];

            // filter the original container atoms for those that can/can't
            // be suppressed
            for (int v = 0; v < nOrgAtoms; v++)
            {
                IAtom atom = org.Atoms[v];
                if (SuppressibleHydrogen(org, graph, bondmap, v))
                {
                    hydrogens.Add(atom);
                    IncrementImplHydrogenCount(org.Atoms[graph[v][0]]);
                }
                else
                {
                    cpyAtoms[nCpyAtoms++] = atom;
                }
            }

            // none of the hydrogens could be suppressed - no changes need to be made
            if (hydrogens.Count == 0) return org;

            org.SetAtoms(cpyAtoms.Take(nCpyAtoms));

            // we now update the bonds - we have auxiliary variable remaining that
            // bypasses the set membership checks if all suppressed bonds are found
            IBond[] cpyBonds = new IBond[nOrgBonds - hydrogens.Count()];
            int remaining = hydrogens.Count();

            foreach (var bond in org.Bonds)
            {
                if (remaining > 0 && (hydrogens.Contains(bond.Begin) || hydrogens.Contains(bond.End)))
                {
                    remaining--;
                    continue;
                }
                cpyBonds[nCpyBonds++] = bond;
            }

            // we know how many hydrogens we removed and we should have removed the
            // same number of bonds otherwise the containers is a strange
            if (nCpyBonds != cpyBonds.Count())
                throw new ArgumentException("number of removed bonds was less than the number of removed hydrogens");

            org.SetBonds(cpyBonds);

            IList<IStereoElement> elements = new List<IStereoElement>();

            foreach (var se in org.StereoElements)
            {
                if (se is ITetrahedralChirality)
                {
                    ITetrahedralChirality tc = (ITetrahedralChirality)se;
                    IAtom focus = tc.ChiralAtom;
                    IList<IAtom> neighbors = tc.Ligands;
                    bool updated = false;
                    for (int i = 0; i < neighbors.Count; i++)
                    {
                        if (hydrogens.Contains(neighbors[i]))
                        {
                            neighbors[i] = focus;
                            updated = true;
                        }
                    }

                    // no changes
                    if (!updated)
                    {
                        elements.Add(tc);
                    }
                    else
                    {
                        elements.Add(new TetrahedralChirality(focus, neighbors, tc.Stereo));
                    }
                }
                else if (se is IDoubleBondStereochemistry)
                {
                    IDoubleBondStereochemistry db = (IDoubleBondStereochemistry)se;
                    DoubleBondConformation conformation = db.Stereo;

                    IBond orgStereo = db.StereoBond;
                    IBond orgLeft = db.Bonds[0];
                    IBond orgRight = db.Bonds[1];

                    // we use the following variable names to refer to the
                    // double bond atoms and substituents
                    // x       y
                    //  \     /
                    //   u = v

                    IAtom u = orgStereo.Begin;
                    IAtom v = orgStereo.End;
                    IAtom x = orgLeft.GetOther(u);
                    IAtom y = orgRight.GetOther(v);

                    // if xNew == x and yNew == y we don't need to find the
                    // connecting bonds
                    IAtom xNew = x;
                    IAtom yNew = y;

                    if (hydrogens.Contains(x))
                    {
                        conformation = conformation.Invert();
                        xNew = FindOther(org, u, v, x);
                    }

                    if (hydrogens.Contains(y))
                    {
                        conformation = conformation.Invert();
                        yNew = FindOther(org, v, u, y);
                    }

                    // no other atoms connected, invalid double-bond configuration?
                    if (x == null || y == null) continue;

                    // no changes
                    if (x == xNew && y == yNew)
                    {
                        elements.Add(db);
                        continue;
                    }

                    // XXX: may perform slow operations but works for now
                    IBond cpyLeft = xNew != x ? org.GetBond(u, xNew) : orgLeft;
                    IBond cpyRight = yNew != y ? org.GetBond(v, yNew) : orgRight;

                    elements.Add(new DoubleBondStereochemistry(orgStereo, new IBond[] { cpyLeft, cpyRight }, conformation));
                }
            }

            org.SetStereoElements(elements);

            // single electron and lone pairs are not really used but we update
            // them just in-case but we just use the inefficient AtomContainer
            // methods

            if (org.SingleElectrons.Count > 0)
            {
                ICollection<ISingleElectron> remove = new HashSet<ISingleElectron>();
                foreach (var se in org.SingleElectrons)
                {
                    if (hydrogens.Contains(se.Atom)) remove.Add(se);
                }
                foreach (var se in remove)
                {
                    org.SingleElectrons.Remove(se);
                }
            }

            if (org.LonePairs.Count > 0)
            {
                ICollection<ILonePair> remove = new HashSet<ILonePair>();
                foreach (var lp in org.LonePairs)
                {
                    if (hydrogens.Contains(lp.Atom)) remove.Add(lp);
                }
                foreach (var lp in remove)
                {
                    org.LonePairs.Remove(lp);
                }
            }

            return org;
        }

        /// <summary>
        /// Create an copy of the <paramref name="org"/> structure with explicit hydrogens
        /// removed. Stereochemistry is updated but up and down bonds in a depiction
        /// may need to be recalculated (see. StructureDiagramGenerator).
        /// </summary>
        /// <param name="org">The AtomContainer from which to remove the hydrogens</param>
        /// <returns>The molecule without hydrogens.</returns>
        /// <seealso cref="CopyAndSuppressedHydrogens"/>
        // @cdk.keyword hydrogens, removal, suppress
        public static IAtomContainer RemoveHydrogens(IAtomContainer org)
        {
            return CopyAndSuppressedHydrogens(org);
        }

        /// <summary>
        /// Is the <paramref name="atom"/> a suppressible hydrogen and can be represented as
        /// implicit. A hydrogen is suppressible if it is not an ion, not the major
        /// isotope (i.e. it is a deuterium or tritium atom) and is not molecular
        /// hydrogen.
        /// </summary>
        /// <param name="container">the structure</param>
        /// <param name="atom">an atom in the structure</param>
        /// <returns>the atom is a hydrogen and it can be suppressed (implicit)</returns>
        private static bool SuppressibleHydrogen(IAtomContainer container, IAtom atom)
        {
            // is the atom a hydrogen
            if (!"H".Equals(atom.Symbol)) return false;
            // is the hydrogen an ion?
            if (atom.FormalCharge != null && atom.FormalCharge != 0) return false;
            // is the hydrogen deuterium / tritium?
            if (atom.MassNumber != null && atom.MassNumber != 1) return false;
            // molecule hydrogen with implicit H?
            if (atom.ImplicitHydrogenCount != null && atom.ImplicitHydrogenCount != 0) return false;
            // molecule hydrogen
            var neighbors = container.GetConnectedAtoms(atom).ToList();
            if (neighbors.Count == 1 && (neighbors[0].Symbol.Equals("H") ||
                                      neighbors[0] is IPseudoAtom)) return false;
            // what about bridging hydrogens?
            // hydrogens with atom-atom mapping?
            return true;
        }

        /// <summary>
        /// Increment the implicit hydrogen count of the provided atom. If the atom
        /// was a non-pseudo atom and had an unset hydrogen count an exception is
        /// thrown.
        /// </summary>
        /// <param name="atom">an atom to increment the hydrogen count of</param>
        private static void IncrementImplHydrogenCount(IAtom atom)
        {
            int? hCount = atom.ImplicitHydrogenCount;

            if (hCount == null)
            {
                if (!(atom is IPseudoAtom))
                    throw new ArgumentException("a non-pseudo atom had an unset hydrogen count");
                hCount = 0;
            }

            atom.ImplicitHydrogenCount = hCount + 1;
        }

        /// <summary>
        /// Is the 'atom' a suppressible hydrogen and can be represented as
        /// implicit. A hydrogen is suppressible if it is not an ion, not the major
        /// isotope (i.e. it is a deuterium or tritium atom) and is not molecular
        /// hydrogen.
        /// </summary>
        /// <param name="container">the structure</param>
        /// <param name="graph">adjacent list representation</param>
        /// <param name="v">vertex (atom index)</param>
        /// <returns>the atom is a hydrogen and it can be suppressed (implicit)</returns>
        private static bool SuppressibleHydrogen(IAtomContainer container, int[][] graph, GraphUtil.EdgeToBondMap bondmap, int v)
        {
            IAtom atom = container.Atoms[v];

            // is the atom a hydrogen
            if (!"H".Equals(atom.Symbol)) return false;
            // is the hydrogen an ion?
            if (atom.FormalCharge != null && atom.FormalCharge != 0) return false;
            // is the hydrogen deuterium / tritium?
            if (atom.MassNumber != null && atom.MassNumber != 1) return false;
            // hydrogen is either not attached to 0 or 2 neighbors
            if (graph[v].Length != 1) return false;
            // non-single bond
            if (bondmap[v, graph[v][0]].Order != BondOrder.Single) return false;

            // okay the hydrogen has one neighbor, if that neighbor is a
            // hydrogen (i.e. molecular hydrogen) then we can not suppress it
            if ("H".Equals(container.Atoms[graph[v][0]].Symbol))
                return false;
            // can not nicely suppress hydrogens on pseudo atoms
            if (container.Atoms[graph[v][0]] is IPseudoAtom)
                return false;
            return true;
        }

        /// <summary>
        /// Finds an neighbor connected to <paramref name="atom"/> which is not <paramref name="exclude1"/>
        /// or <paramref name="exclude2"/>. If no neighbor exists - null is returned.
        /// </summary>
        /// <param name="container">structure</param>
        /// <param name="atom">atom to find a neighbor of</param>
        /// <param name="exclude1">the neighbor should not be this atom</param>
        /// <param name="exclude2">the neighbor should also not be this atom</param>
        /// <returns>a neighbor of 'atom', null if not found</returns>
        private static IAtom FindOther(IAtomContainer container, IAtom atom, IAtom exclude1, IAtom exclude2)
        {
            foreach (var neighbor in container.GetConnectedAtoms(atom))
            {
                if (neighbor != exclude1 && neighbor != exclude2) return neighbor;
            }
            return null;
        }

        /// <summary>
        /// Produces an AtomContainer without explicit Hs but with H count from one with Hs.
        /// Hs bonded to more than one heavy atom are preserved.  The new molecule is a deep copy.
        /// </summary>
        /// <returns>The mol without Hs.</returns>
        // @cdk.keyword    hydrogens, removal
        [Obsolete(nameof(SuppressHydrogens) + "will now not removed bridging hydrogens by default")]
        public static IAtomContainer RemoveHydrogensPreserveMultiplyBonded(IAtomContainer ac)
        {
            return CopyAndSuppressedHydrogens(ac);
        }

        /// <summary>
        /// Produces an AtomContainer without explicit Hs (except those listed) but with H count from one with Hs.
        /// The new molecule is a deep copy.
        /// </summary>
        /// <param name="preserve">a list of H atoms to preserve.</param>
        /// <returns>The mol without Hs.</returns>
        // @cdk.keyword      hydrogens, removal
        [Obsolete("not used by the internal API " +  nameof(SuppressHydrogens) + "will now only suppress hydrogens that can be represent as a h count")]
        private static IAtomContainer RemoveHydrogens(IAtomContainer ac, List<IAtom> preserve)
        {
            IDictionary<IAtom, IAtom> map = new Dictionary<IAtom, IAtom>();
            // maps original atoms to clones.
            IList<IAtom> remove = new List<IAtom>();
            // lists removed Hs.

            // Clone atoms except those to be removed.
            IAtomContainer mol = ac.Builder.CreateAtomContainer();
            int count = ac.Atoms.Count;
            for (int i = 0; i < count; i++)
            {
                // Clone/remove this atom?
                IAtom atom = ac.Atoms[i];
                if (!SuppressibleHydrogen(ac, atom) || preserve.Contains(atom))
                {
                    IAtom a = null;
                    a = (IAtom)atom.Clone();
                    a.ImplicitHydrogenCount = 0;
                    mol.Atoms.Add(a);
                    map.Add(atom, a);
                }
                else
                {
                    remove.Add(atom);
                    // maintain list of removed H.
                }
            }

            // Clone bonds except those involving removed atoms.
            count = ac.Bonds.Count;
            for (int i = 0; i < count; i++)
            {
                // Check bond.
                IBond bond = ac.Bonds[i];
                IAtom atom0 = bond.Begin;
                IAtom atom1 = bond.End;
                bool remove_bond = false;
                foreach (var atom in bond.Atoms)
                {
                    if (remove.Contains(atom))
                    {
                        remove_bond = true;
                        break;
                    }
                }

                // Clone/remove this bond?
                if (!remove_bond)
                {
                    // if (!remove.Contains(atoms[0]) && !remove.Contains(atoms[1]))

                    IBond clone = (IBond)ac.Bonds[i].Clone();
                    clone.SetAtoms(new[] { map[atom0], map[atom1] });
                    mol.Bonds.Add(clone);
                }
            }

            // Recompute hydrogen counts of neighbours of removed Hydrogens.
            foreach (var Remove in remove)
            {
                // Process neighbours.
                foreach (var neighbor in ac.GetConnectedAtoms(Remove))
                {
                    IAtom neighb = map[neighbor];
                    neighb.ImplicitHydrogenCount = neighb.ImplicitHydrogenCount + 1;
                }
            }

            return (mol);
        }

        /// <summary>
        /// Sets a property on all <see cref="IAtom"/>s in the given container.
        /// </summary>
        public static void SetAtomProperties(IAtomContainer container, string propKey, object propVal)
        {
            if (container != null)
            {
                foreach (var atom in container.Atoms)
                {
                    atom.SetProperty(propKey, propVal);
                }
            }
        }

        /// <summary>
        ///  A method to remove ElectronContainerListeners.
        ///  ElectronContainerListeners are used to detect changes
        ///  in ElectronContainers (like bonds) and to notifiy
        ///  registered Listeners in the event of a change.
        ///  If an object looses interest in such changes, it should
        ///  unregister with this AtomContainer in order to improve
        ///  performance of this class.
        /// </summary>
        public static void UnregIsterElectronContainerListeners(IAtomContainer container)
        {
            foreach (var electronContainer in container.GetElectronContainers())
            {
                electronContainer.Listeners.Remove(container);
            }
        }
        /// <summary>
        ///  A method to remove AtomListeners.
        ///  AtomListeners are used to detect changes
        ///  in Atom objects within this AtomContainer and to notifiy
        ///  registered Listeners in the event of a change.
        ///  If an object looses interest in such changes, it should
        ///  unregister with this AtomContainer in order to improve
        ///  performance of this class.
        /// </summary>
        public static void UnregIsterAtomListeners(IAtomContainer container)
        {
            foreach (var atom in container.Atoms)
                atom.Listeners.Remove(container);
        }

        /// <summary>
        /// Compares this AtomContainer with another given AtomContainer and returns
        /// the Intersection between them.
        /// </summary>
        /// <remarks>
        /// <b>Important Note</b> : This is not the maximum common substructure.
        /// </remarks>
        /// <param name="container1">an AtomContainer object</param>
        /// <param name="container2">an AtomContainer object</param>
        /// <returns>An AtomContainer containing the intersection between <paramref name="container1"/> and <paramref name="container2"/></returns>
        public static IAtomContainer GetIntersection(IAtomContainer container1, IAtomContainer container2)
        {
            IAtomContainer intersection = container1.Builder.CreateAtomContainer();

            foreach (var atom1 in container1.Atoms)
                if (container2.Contains(atom1))
                    intersection.Atoms.Add(atom1);
            foreach (var electronContainer1 in container1.GetElectronContainers())
                if (container2.Contains(electronContainer1))
                    intersection.AddElectronContainer(electronContainer1);

            return intersection;
        }

        /// <summary>
        /// Constructs an array of Atom objects from an AtomContainer.
        /// </summary>
        /// <param name="container">The original AtomContainer.</param>
        /// <returns>The array of Atom objects.</returns>
        public static IAtom[] GetAtomArray(IAtomContainer container)
        {
            return container.Atoms.ToArray();
        }

        /// <summary>
        /// Constructs an array of Atom objects from a List of Atom objects.
        /// </summary>
        /// <param name="list">The original List.</param>
        /// <returns>The array of Atom objects.</returns>
        public static IAtom[] GetAtomArray(IEnumerable<IAtom> list)
        {
            return list.ToArray();
        }

        /// <summary>
        /// Constructs an array of Bond objects from an AtomContainer.
        /// </summary>
        /// <param name="container">The original AtomContainer.</param>
        /// <returns>The array of Bond objects.</returns>
        public static IBond[] GetBondArray(IAtomContainer container)
        {
            return container.Bonds.ToArray();
        }

        /// <summary>
        /// Constructs an array of Atom objects from a List of Atom objects.
        /// </summary>
        /// <param name="list">The original List.</param>
        /// <returns>The array of Atom objects.</returns>
        public static IBond[] GetBondArray(IEnumerable<IBond> list)
        {
            return list.ToArray();
        }

        /// <summary>
        /// Constructs an array of Bond objects from an AtomContainer.
        /// </summary>
        /// <param name="container">The original AtomContainer.</param>
        /// <returns>The array of Bond objects.</returns>
        public static IElectronContainer[] GetElectronContainerArray(IAtomContainer container)
        {
            return container.GetElectronContainers().ToArray();
        }

        /// <summary>
        /// Constructs an array of Atom objects from a List of Atom objects.
        /// </summary>
        /// <param name="list">The original List.</param>
        /// <returns>The array of Atom objects.</returns>
        public static IElectronContainer[] GetElectronContainerArray(IEnumerable<IElectronContainer> list)
        {
            return list.ToArray();
        }

        /// <summary>
        /// Convenience method to perceive atom types for all <see cref="IAtom"/>s in the
        /// <see cref="IAtomContainer"/>, using the <see cref="CDKAtomTypeMatcher"/>.If the
        /// matcher finds a matching atom type, the <see cref="IAtom"/> will be configured
        /// to have the same properties as the <see cref="IAtomType"/>. If no matching atom
        /// type is found, no configuration is performed.
        /// <b>This method overwrites existing values.</b>
        /// </summary>
        /// <param name="container"></param>
        public static void PercieveAtomTypesAndConfigureAtoms(IAtomContainer container)
        {
            CDKAtomTypeMatcher matcher = CDKAtomTypeMatcher.GetInstance(container.Builder);
            foreach (var atom in container.Atoms)
            {
                IAtomType matched = matcher.FindMatchingAtomType(container, atom);
                if (matched != null) AtomTypeManipulator.Configure(atom, matched);
            }
        }

        /// <summary>
        /// Convenience method to perceive atom types for all <see cref="IAtom"/>s in the
        /// <see cref="IAtomContainer"/>, using the <see cref="CDKAtomTypeMatcher"/>. If the
        /// matcher finds a matching atom type, the <see cref="IAtom"/> will be configured
        /// to have the same properties as the <see cref="IAtomType"/>. If no matching atom
        /// type is found, no configuration is performed.
        /// <b>This method overwrites existing values.</b>
        /// </summary>
        public static void PercieveAtomTypesAndConfigureUnsetProperties(IAtomContainer container)
        {
            CDKAtomTypeMatcher matcher = CDKAtomTypeMatcher.GetInstance(container.Builder);
            foreach (var atom in container.Atoms)
            {
                IAtomType matched = matcher.FindMatchingAtomType(container, atom);
                if (matched != null) AtomTypeManipulator.ConfigureUnsetProperties(atom, matched);
            }
        }

        /// <summary>
        /// This method will reset all atom configuration to unset.
        /// </summary>
        /// <remarks>
        /// This method is the reverse of <see cref="PercieveAtomTypesAndConfigureAtoms(IAtomContainer)"/> 
        /// and after a call to this method all atoms will be "unconfigured".
        /// <note type="note">
        /// Note that it is not a complete reversal of <see cref="PercieveAtomTypesAndConfigureAtoms(IAtomContainer)"/> 
        /// since the atomic symbol of the atoms remains unchanged. Also, all the flags that were set
        /// by the configuration method (such as <see cref="IAtomType.IsHydrogenBondAcceptor"/> or <see cref="IMolecularEntity.IsAromatic"/>) will be set to False.
        /// </note>
        /// </remarks>
        /// <param name="container">The molecule, whose atoms are to be unconfigured</param>
        /// <seealso cref="PercieveAtomTypesAndConfigureAtoms(IAtomContainer)"/>
        public static void ClearAtomConfigurations(IAtomContainer container)
        {
            foreach (var atom in container.Atoms)
            {
                atom.AtomTypeName = (string)null;
                atom.MaxBondOrder = BondOrder.Unset;
                atom.BondOrderSum = null;
                atom.CovalentRadius = null;
                atom.Valency = null;
                atom.FormalCharge = null;
                atom.Hybridization = Hybridization.Unset;
                atom.FormalNeighbourCount = null;
                atom.IsHydrogenBondAcceptor = false;
                atom.IsHydrogenBondDonor = false;
                atom.SetProperty(CDKPropertyName.ChemicalGroupConstant, null);
                atom.IsAromatic = false;
                atom.SetProperty(CDKPropertyName.Color, null);
                atom.ExactMass = null;
            }
        }

        /// <summary>
        /// Returns the sum of bond orders, where a single bond counts as one
        /// <i>single bond equivalent</i>, a double as two, etc.
        /// </summary>
        public static int GetSingleBondEquivalentSum(IAtomContainer container)
        {
            int sum = 0;
            foreach (var bond in container.Bonds)
            {
                BondOrder order = bond.Order;
                if (!order.IsUnset)
                {
                    sum += order.Numeric;
                }
            }
            return sum;
        }

        public static BondOrder GetMaximumBondOrder(IAtomContainer container)
        {
            return BondManipulator.GetMaximumBondOrder(container.Bonds);
        }

        /// <summary>
        /// Returns a set of nodes excluding all the hydrogens.
        /// </summary>
        /// <returns>The heavyAtoms value</returns>
        // @cdk.keyword    hydrogens, removal
        public static IList<IAtom> GetHeavyAtoms(IAtomContainer container)
        {
            List<IAtom> newAc = new List<IAtom>();
            for (int f = 0; f < container.Atoms.Count; f++)
            {
                if (!container.Atoms[f].Symbol.Equals("H"))
                {
                    newAc.Add(container.Atoms[f]);
                }
            }
            return newAc;
        }

        /// <summary>
        /// Generates a cloned atomcontainer with all atoms being carbon, all bonds
        /// being single non-aromatic
        /// </summary>
        /// <param name="atomContainer">The input atomcontainer</param>
        /// <returns>The new atomcontainer</returns>
        [Obsolete("not all attributes are removed producing unexpected results, use " + nameof(Anonymise))]
        public static IAtomContainer CreateAllCarbonAllSingleNonAromaticBondAtomContainer(IAtomContainer atomContainer)
        {
            IAtomContainer query = (IAtomContainer)atomContainer.Clone();
            for (int i = 0; i < query.Bonds.Count; i++)
            {
                query.Bonds[i].Order = BondOrder.Single;
                query.Bonds[i].IsAromatic = false;
                query.Bonds[i].IsSingleOrDouble = false;
                query.Bonds[i].Begin.Symbol = "C";
                query.Bonds[i].Begin.Hybridization = Hybridization.Unset;
                query.Bonds[i].End.Symbol = "C";
                query.Bonds[i].End.Hybridization = Hybridization.Unset;
                query.Bonds[i].Begin.IsAromatic = false;
                query.Bonds[i].End.IsAromatic = false;
            }
            return query;
        }

        /// <summary>
        /// Anonymise the provided container to single-bonded carbon atoms. No
        /// information other then the connectivity from the original container is
        /// retrained.
        /// </summary>
        /// <param name="src">an atom container</param>
        /// <returns>anonymised container</returns>
        public static IAtomContainer Anonymise(IAtomContainer src)
        {
            IChemObjectBuilder builder = src.Builder;

            IAtom[] atoms = new IAtom[src.Atoms.Count];
            IBond[] bonds = new IBond[src.Bonds.Count];

            for (int i = 0; i < atoms.Length; i++)
            {
                atoms[i] = builder.CreateAtom("C");
            }
            for (int i = 0; i < bonds.Length; i++)
            {
                IBond bond = src.Bonds[i];
                int u = src.Atoms.IndexOf(bond.Begin);
                int v = src.Atoms.IndexOf(bond.End);
                bonds[i] = builder.CreateBond(atoms[u], atoms[v]);
            }

            IAtomContainer dest = builder.CreateAtomContainer(atoms, bonds);
            return dest;
        }

        /// <summary>
        /// Create a skeleton copy of the provided structure. The skeleton copy is
        /// similar to an anonymous copy (<see cref="Anonymise(IAtomContainer)"/>) except that atom
        /// elements are preserved. All bonds are converted to single bonds and a
        /// 'clean' atom is created for the input elements. The 'clean' atom has
        /// unset charge, mass, and hydrogen count.
        /// </summary>
        /// <param name="src">input structure</param>
        /// <returns>the skeleton copy</returns>
        public static IAtomContainer Skeleton(IAtomContainer src)
        {
            IChemObjectBuilder builder = src.Builder;

            IAtom[] atoms = new IAtom[src.Atoms.Count];
            IBond[] bonds = new IBond[src.Bonds.Count];

            for (int i = 0; i < atoms.Length; i++)
            {
                atoms[i] = builder.CreateAtom(src.Atoms[i].AtomicNumber.Value);
            }
            for (int i = 0; i < bonds.Length; i++)
            {
                IBond bond = src.Bonds[i];
                int u = src.Atoms.IndexOf(bond.Begin);
                int v = src.Atoms.IndexOf(bond.End);
                bonds[i] = builder.CreateBond(atoms[u], atoms[v]);
            }

            IAtomContainer dest = builder.CreateAtomContainer(atoms, bonds);
            return dest;
        }

        /// <summary>
        /// Returns the sum of the bond order equivalents for a given IAtom. It
        /// considers single bonds as 1.0, double bonds as 2.0, triple bonds as 3.0,
        /// and quadruple bonds as 4.0.
        /// </summary>
        /// <param name="atom">The atom for which to calculate the bond order sum</param>
        /// <returns>The number of bond order equivalents for this atom</returns>
        public static double GetBondOrderSum(IAtomContainer container, IAtom atom)
        {
            double count = 0;
            foreach (var bond in container.GetConnectedBonds(atom))
            {
                BondOrder order = bond.Order;
                if (!order.IsUnset)
                {
                    count += order.Numeric;
                }
            }
            return count;
        }

        /// <summary>
        /// Assigns <see cref="IBond.IsSingleOrDouble"/> to the bonds of
        /// a container. The single or double flag indicates uncertainty of bond
        /// order and in this case is assigned to all aromatic bonds (and atoms)
        /// which occur in rings. If any such bonds are found the flag is also set
        /// on the container.
        /// </summary>
        /// <example>
        /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Tools.AtomContainerManipulator_Example.cs+SetSingleOrDoubleFlags"]/*' />
        /// </example>
        /// <param name="ac">container to which the flags are assigned</param>
        /// <returns>the input for convenience</returns>
        public static IAtomContainer SetSingleOrDoubleFlags(IAtomContainer ac)
        {
            // note - we could check for any aromatic bonds to avoid RingSearch but
            // RingSearch is fast enough it probably wouldn't do much to check
            // before hand
            RingSearch rs = new RingSearch(ac);
            bool singleOrDouble = false;
            foreach (var bond in rs.RingFragments().Bonds)
            {
                if (bond.IsAromatic)
                {
                    bond.IsSingleOrDouble = true;
                    bond.Begin.IsSingleOrDouble = true;
                    bond.End.IsSingleOrDouble = true;
                    singleOrDouble = singleOrDouble | true;
                }
            }
            if (singleOrDouble)
            {
                ac.IsSingleOrDouble = true;
            }

            return ac;
        }
    }
}
