/* Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
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
using NCDK.Common.Collections;
using NCDK.Stereo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Tools.Manipulator
{
    /// <summary>
    /// </summary>
    /// <seealso cref="ChemModelManipulator"/>
    // @cdk.module standard
    // @cdk.githash
    public class ReactionManipulator
    {
        public static int GetAtomCount(IReaction reaction)
        {
            int count = 0;
            IAtomContainerSet<IAtomContainer> reactants = reaction.Reactants;
            for (int i = 0; i < reactants.Count; i++)
            {
                count += reactants[i].Atoms.Count;
            }
            var agents = reaction.Agents;
            for (int i = 0; i < agents.Count; i++)
            {
                count += agents[i].Atoms.Count;
            }
            IAtomContainerSet<IAtomContainer> products = reaction.Products;
            for (int i = 0; i < products.Count; i++)
            {
                count += products[i].Atoms.Count;
            }
            return count;
        }

        public static int GetBondCount(IReaction reaction)
        {
            int count = 0;
            IAtomContainerSet<IAtomContainer> reactants = reaction.Reactants;
            for (int i = 0; i < reactants.Count; i++)
            {
                count += reactants[i].Bonds.Count;
            }
            var agents = reaction.Agents;
            for (int i = 0; i < agents.Count; i++)
            {
                count += agents[i].Bonds.Count;
            }
            IAtomContainerSet<IAtomContainer> products = reaction.Products;
            for (int i = 0; i < products.Count; i++)
            {
                count += products[i].Bonds.Count;
            }
            return count;
        }

        public static void RemoveAtomAndConnectedElectronContainers(IReaction reaction, IAtom atom)
        {
            IAtomContainerSet<IAtomContainer> reactants = reaction.Reactants;
            for (int i = 0; i < reactants.Count; i++)
            {
                IAtomContainer mol = reactants[i];
                if (mol.Contains(atom))
                {
                    mol.RemoveAtomAndConnectedElectronContainers(atom);
                }
            }
            var agents = reaction.Reactants;
            for (int i = 0; i < agents.Count; i++)
            {
                IAtomContainer mol = agents[i];
                if (mol.Contains(atom))
                {
                    mol.RemoveAtomAndConnectedElectronContainers(atom);
                }
            }
            IAtomContainerSet<IAtomContainer> products = reaction.Products;
            for (int i = 0; i < products.Count; i++)
            {
                IAtomContainer mol = products[i];
                if (mol.Contains(atom))
                {
                    mol.RemoveAtomAndConnectedElectronContainers(atom);
                }
            }
        }

        public static void RemoveElectronContainer(IReaction reaction, IElectronContainer electrons)
        {
            IAtomContainerSet<IAtomContainer> reactants = reaction.Reactants;
            for (int i = 0; i < reactants.Count; i++)
            {
                IAtomContainer mol = reactants[i];
                if (mol.Contains(electrons))
                {
                    mol.RemoveElectronContainer(electrons);
                }
            }
            var agents = reaction.Reactants;
            for (int i = 0; i < agents.Count; i++)
            {
                IAtomContainer mol = agents[i];
                if (mol.Contains(electrons))
                {
                    mol.RemoveElectronContainer(electrons);
                }
            }
            IAtomContainerSet<IAtomContainer> products = reaction.Products;
            for (int i = 0; i < products.Count; i++)
            {
                IAtomContainer mol = products[i];
                if (mol.Contains(electrons))
                {
                    mol.RemoveElectronContainer(electrons);
                }
            }
        }

        /// <summary>
        /// Get all molecule of a <see cref="IReaction"/>: reactants + products.
        /// </summary>
        /// <param name="reaction">The IReaction</param>
        /// <returns>The IAtomContainerSet</returns>
        public static IAtomContainerSet<IAtomContainer> GetAllMolecules(IReaction reaction)
        {
            IAtomContainerSet<IAtomContainer> moleculeSet = reaction.Builder.CreateAtomContainerSet();

            moleculeSet.AddRange(GetAllReactants(reaction));
            moleculeSet.AddRange(GetAllAgents(reaction));
            moleculeSet.AddRange(GetAllProducts(reaction));

            return moleculeSet;
        }

        /// <summary>
        /// get all products of a IReaction
        /// </summary>
        /// <param name="reaction">The IReaction</param>
        /// <returns>The IAtomContainerSet</returns>
        public static IAtomContainerSet<IAtomContainer> GetAllProducts(IReaction reaction)
        {
            IAtomContainerSet<IAtomContainer> moleculeSet = reaction.Builder.CreateAtomContainerSet();
            IAtomContainerSet<IAtomContainer> products = reaction.Products;
            for (int i = 0; i < products.Count; i++)
            {
                moleculeSet.Add(products[i]);
            }
            return moleculeSet;
        }

        /// <summary>
        /// get all reactants of a IReaction
        /// </summary>
        /// <param name="reaction">The IReaction</param>
        /// <returns>The IAtomContainerSet</returns>
        public static IAtomContainerSet<IAtomContainer> GetAllReactants(IReaction reaction)
        {
            IAtomContainerSet<IAtomContainer> moleculeSet = reaction.Builder.CreateAtomContainerSet();
            IAtomContainerSet<IAtomContainer> reactants = reaction.Reactants;
            for (int i = 0; i < reactants.Count; i++)
            {
                moleculeSet.Add(reactants[i]);
            }
            return moleculeSet;
        }

        public static IAtomContainerSet<IAtomContainer> GetAllAgents(IReaction reaction)
        {
            var moleculeSet = reaction.Builder.CreateAtomContainerSet();
            var agents = reaction.Agents;
            for (int i = 0; i < agents.Count; i++)
            {
                moleculeSet.Add(agents[i]);
            }
            return moleculeSet;
        }

        /// <summary>
        /// Returns a new Reaction object which is the reverse of the given
        /// Reaction.
        /// </summary>
        /// <param name="reaction">the reaction being considered</param>
        /// <returns>the reverse reaction</returns>
        public static IReaction Reverse(IReaction reaction)
        {
            IReaction reversedReaction = reaction.Builder.CreateReaction();
            if (reaction.Direction == ReactionDirection.Bidirectional)
            {
                reversedReaction.Direction = ReactionDirection.Bidirectional;
            }
            else if (reaction.Direction == ReactionDirection.Forward)
            {
                reversedReaction.Direction = ReactionDirection.Backward;
            }
            else if (reaction.Direction == ReactionDirection.Backward)
            {
                reversedReaction.Direction = ReactionDirection.Forward;
            }
            IAtomContainerSet<IAtomContainer> reactants = reaction.Reactants;
            for (int i = 0; i < reactants.Count; i++)
            {
                double coefficient = reaction.Reactants.GetMultiplier(reactants[i]).Value;
                reversedReaction.Products.Add(reactants[i], coefficient);
            }
            IAtomContainerSet<IAtomContainer> products = reaction.Products;
            for (int i = 0; i < products.Count; i++)
            {
                double coefficient = reaction.Products.GetMultiplier(products[i]).Value;
                reversedReaction.Reactants.Add(products[i], coefficient);
            }
            return reversedReaction;
        }

        /// <summary>
        /// Returns all the AtomContainer's of a Reaction.
        /// </summary>
        /// <param name="reaction">The reaction being considered</param>
        /// <returns>a list of the IAtomContainer objects comprising the reaction</returns>
        public static IEnumerable<IAtomContainer> GetAllAtomContainers(IReaction reaction)
        {
            return MoleculeSetManipulator.GetAllAtomContainers(GetAllMolecules(reaction));
        }

        public static IList<string> GetAllIDs(IReaction reaction)
        {
            List<string> idList = new List<string>();
            if (reaction.Id != null) idList.Add(reaction.Id);
            IAtomContainerSet<IAtomContainer> reactants = reaction.Reactants;
            for (int i = 0; i < reactants.Count; i++)
            {
                IAtomContainer mol = reactants[i];
                idList.AddRange(AtomContainerManipulator.GetAllIDs(mol));
            }
            IAtomContainerSet<IAtomContainer> products = reaction.Products;
            for (int i = 0; i < products.Count; i++)
            {
                IAtomContainer mol = products[i];
                idList.AddRange(AtomContainerManipulator.GetAllIDs(mol));
            }
            return idList;
        }

        public static IAtomContainer GetRelevantAtomContainer(IReaction reaction, IAtom atom)
        {
            IAtomContainer result = MoleculeSetManipulator.GetRelevantAtomContainer(reaction.Reactants, atom);
            if (result != null)
            {
                return result;
            }
            return MoleculeSetManipulator.GetRelevantAtomContainer(reaction.Products, atom);
        }

        public static IAtomContainer GetRelevantAtomContainer(IReaction reaction, IBond bond)
        {
            IAtomContainer result = MoleculeSetManipulator.GetRelevantAtomContainer(reaction.Reactants, bond);
            if (result != null)
            {
                return result;
            }
            return MoleculeSetManipulator.GetRelevantAtomContainer(reaction.Products, bond);
        }

        public static void SetAtomProperties(IReaction reaction, string propKey, object propVal)
        {
            IAtomContainerSet<IAtomContainer> reactants = reaction.Reactants;
            for (int j = 0; j < reactants.Count; j++)
            {
                AtomContainerManipulator.SetAtomProperties(reactants[j], propKey, propVal);
            }
            IAtomContainerSet<IAtomContainer> products = reaction.Products;
            for (int j = 0; j < products.Count; j++)
            {
                AtomContainerManipulator.SetAtomProperties(products[j], propKey, propVal);
            }
        }

        public static IList<IChemObject> GetAllChemObjects(IReaction reaction)
        {
            List<IChemObject> list = new List<IChemObject>();
            list.Add(reaction);
            IAtomContainerSet<IAtomContainer> reactants = reaction.Reactants;
            for (int i = 0; i < reactants.Count; i++)
            {
                list.Add(reactants[i]);
            }
            IAtomContainerSet<IAtomContainer> products = reaction.Products;
            for (int i = 0; i < products.Count; i++)
            {
                list.Add(products[i]);
            }
            return list;
        }

        /// <summary>
        /// get the IAtom which is mapped
        /// </summary>
        /// <param name="reaction">The IReaction which contains the mapping</param>
        /// <param name="chemObject">The IChemObject which will be searched its mapped IChemObject</param>
        /// <returns>The mapped IChemObject</returns>
        public static IChemObject GetMappedChemObject(IReaction reaction, IChemObject chemObject)
        {
            foreach (var mapping in reaction.Mappings)
            {
                if (mapping[0].Equals(chemObject))
                {
                    return mapping[1];
                }
                else if (mapping[1].Equals(chemObject)) return mapping[0];
            }
            return null;
        }

        /// <summary>
        /// Assigns a reaction role and group id to all atoms in a molecule.
        /// </summary>
        /// <param name="mol">molecule</param>
        /// <param name="role">role to assign</param>
        /// <param name="grpId">group id</param>
        private static void AssignRoleAndGrp(IAtomContainer mol, ReactionRole role, int grpId)
        {
            foreach (IAtom atom in mol.Atoms)
            {
                atom.SetProperty(CDKPropertyName.ReactionRole, role);
                atom.SetProperty(CDKPropertyName.ReactionGroup, grpId);
            }
        }

        /// <summary>
        /// <p>Converts a reaction to an 'inlined' reaction stored as a molecule. All
        /// reactants, agents, products are added to the molecule as disconnected
        /// components with atoms flagged as to their role <see cref="ReactionRole"/> and
        /// component group.</p>
        /// <p>
        /// The inlined reaction, stored in a molecule can be converted back to an explicit
        /// reaction with <see cref="toReaction"/>. Data stored on the individual components (e.g.
        /// titles is lost in the conversion).
        /// </p>
        /// </summary>
        /// <param name="rxn">reaction to convert</param>
        /// <returns>inlined reaction stored in a molecule</returns>
        /// <seealso cref="toReaction"/>
        public static IAtomContainer ToMolecule(IReaction rxn)
        {
            if (rxn == null)
                throw new ArgumentNullException(nameof(rxn), "Null reaction provided");
            IChemObjectBuilder bldr = rxn.Builder;
            IAtomContainer mol = bldr.CreateAtomContainer();
            mol.SetProperties(rxn.GetProperties());
            mol.Id = rxn.Id;
            int grpId = 0;
            foreach (IAtomContainer comp in rxn.Reactants)
            {
                AssignRoleAndGrp(comp, ReactionRole.Reactant, ++grpId);
                mol.Add(comp);
            }
            foreach (IAtomContainer comp in rxn.Agents)
            {
                AssignRoleAndGrp(comp, ReactionRole.Agent, ++grpId);
                mol.Add(comp);
            }
            foreach (IAtomContainer comp in rxn.Products)
            {
                AssignRoleAndGrp(comp, ReactionRole.Product, ++grpId);
                mol.Add(comp);
            }
            return mol;
        }

        /// <summary>
        /// <p>Converts an 'inlined' reaction stored in a molecule back to a reaction.</p>
        /// </summary>
        /// <param name="mol">molecule to convert</param>
        /// <returns>reaction</returns>
        /// <seealso cref="ToMolecule(IReaction)"/>
        public static IReaction ToReaction(IAtomContainer mol)
        {
            if (mol == null)
                throw new ArgumentException("Null molecule provided");
            IChemObjectBuilder bldr = mol.Builder;
            IReaction rxn = bldr.CreateReaction();
            rxn.SetProperties(mol.GetProperties());
            rxn.Id = mol.Id;

            IDictionary<int, IAtomContainer> components = new Dictionary<int, IAtomContainer>();

            // split atoms
            foreach (IAtom atom in mol.Atoms)
            {
                var role = atom.GetProperty<ReactionRole?>(CDKPropertyName.ReactionRole);
                var grpIdx = atom.GetProperty<int?>(CDKPropertyName.ReactionGroup);

                if (role == null || role.Value == ReactionRole.None)
                    throw new ArgumentException("Atom " + mol.Atoms.IndexOf(atom) + " had undefined role");
                if (grpIdx == null)
                    throw new ArgumentException("Atom " + mol.Atoms.IndexOf(atom) + " had no reaction group id");

                IAtomContainer comp;
                // new component, and add to appropriate role
                if (!components.TryGetValue(grpIdx.Value, out comp))
                {
                    components[grpIdx.Value] = comp = bldr.CreateAtomContainer();
                    switch (role)
                    {
                        case ReactionRole.Reactant:
                            rxn.Reactants.Add(comp);
                            break;
                        case ReactionRole.Product:
                            rxn.Products.Add(comp);
                            break;
                        case ReactionRole.Agent:
                            rxn.Agents.Add(comp);
                            break;
                    }
                }

                comp.Atoms.Add(atom);
            }

            // split bonds
            foreach (IBond bond in mol.Bonds)
            {
                IAtom beg = bond.Atoms[0];
                IAtom end = bond.Atoms[1];
                var begIdx = beg.GetProperty<int?>(CDKPropertyName.ReactionGroup);
                var endIdx = end.GetProperty<int?>(CDKPropertyName.ReactionGroup);
                if (begIdx == null || endIdx == null)
                    throw new ArgumentException("Bond " + mol.Bonds.IndexOf(bond) + " had atoms with no reaction group id");
                if (!begIdx.Equals(endIdx))
                    throw new ArgumentException("Bond " + mol.Bonds.IndexOf(bond) + " had atoms with different reaction group id");
                components[begIdx.Value].Bonds.Add(bond);
            }

            // split stereochemistry
            foreach (IStereoElement se in mol.StereoElements)
            {
                IAtom focus = null;
                if (se is ITetrahedralChirality)
                {
                    focus = ((ITetrahedralChirality)se).ChiralAtom;
                }
                else if (se is IDoubleBondStereochemistry)
                {
                    focus = ((IDoubleBondStereochemistry)se).StereoBond.Atoms[0];
                }
                else if (se is ExtendedTetrahedral)
                {
                    focus = ((ExtendedTetrahedral)se).Focus;
                }
                if (focus == null)
                    throw new ArgumentException("Stereochemistry had no focus");
                var grpIdx = focus.GetProperty<int>(CDKPropertyName.ReactionGroup);
                components[grpIdx].StereoElements.Add(se);
            }

            return rxn;
        }

        /// <summary>
        /// Bi-direction int-tuple for looking up bonds by index.
        /// </summary>
        private class IntTuple
        {
            private readonly int beg, end;

            public IntTuple(int beg, int end)
            {
                this.beg = beg;
                this.end = end;
            }

            public override bool Equals(Object o)
            {
                if (this == o) return true;
                if (o == null || GetType() != o.GetType()) return false;
                IntTuple that = (IntTuple)o;
                return (this.beg == that.beg && this.end == that.end) ||
                       (this.beg == that.end && this.end == that.beg);
            }

            public override int GetHashCode()
            {
                return beg ^ end;
            }
        }

        /// <summary>
        /// Collect the set of bonds that mapped in both a reactant and a product. The method uses
        /// the <see cref="CDKPropertyName.ATOM_ATOM_MAPPING"/> property of atoms.
        /// </summary>
        /// <param name="reaction">reaction</param>
        /// <returns>mapped bonds</returns>
        public static ISet<IBond> FindMappedBonds(IReaction reaction)
        {
            ISet<IBond> mapped = new HashSet<IBond>();

            // first we collect the occurrance of mapped bonds from reacants then products
            ISet<IntTuple> mappedReactantBonds = new HashSet<IntTuple>();
            ISet<IntTuple> mappedProductBonds = new HashSet<IntTuple>();
            foreach (IAtomContainer reactant in reaction.Reactants)
            {
                foreach (IBond bond in reactant.Bonds)
                {
                    var begidx = bond.Atoms[0].GetProperty<int?>(CDKPropertyName.AtomAtomMapping);
                    var endidx = bond.Atoms[1].GetProperty<int?>(CDKPropertyName.AtomAtomMapping);
                    if (begidx != null && endidx != null)
                        mappedReactantBonds.Add(new IntTuple(begidx.Value, endidx.Value));
                }
            }
            // fail fast
            if (!mappedReactantBonds.Any())
                return GenericCollections.GetEmptySet<IBond>();

            foreach (IAtomContainer product in reaction.Products)
            {
                foreach (IBond bond in product.Bonds)
                {
                    var begidx = bond.Atoms[0].GetProperty<int?>(CDKPropertyName.AtomAtomMapping);
                    var endidx = bond.Atoms[1].GetProperty<int?>(CDKPropertyName.AtomAtomMapping);
                    if (begidx != null && endidx != null)
                        mappedProductBonds.Add(new IntTuple(begidx.Value, endidx.Value));
                }
            }
            // fail fast
            if (!mappedProductBonds.Any())
                return GenericCollections.GetEmptySet<IBond>();

            // repeat above but now store any that are different or unmapped as being mapped
            foreach (IAtomContainer reactant in reaction.Reactants)
            {
                foreach (IBond bond in reactant.Bonds)
                {
                    var begidx = bond.Atoms[0].GetProperty<int?>(CDKPropertyName.AtomAtomMapping);
                    var endidx = bond.Atoms[1].GetProperty<int?>(CDKPropertyName.AtomAtomMapping);
                    if (begidx != null && endidx != null && mappedProductBonds.Contains(new IntTuple(begidx.Value, endidx.Value)))
                        mapped.Add(bond);
                }
            }
            foreach (IAtomContainer product in reaction.Products)
            {
                foreach (IBond bond in product.Bonds)
                {
                    var begidx = bond.Atoms[0].GetProperty<int?>(CDKPropertyName.AtomAtomMapping);
                    var endidx = bond.Atoms[1].GetProperty<int?>(CDKPropertyName.AtomAtomMapping);
                    if (begidx != null && endidx != null && mappedReactantBonds.Contains(new IntTuple(begidx.Value, endidx.Value)))
                        mapped.Add(bond);
                }
            }
            return mapped;
        }
    }
}
