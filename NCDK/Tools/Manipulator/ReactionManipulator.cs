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
using System.Collections.Generic;

namespace NCDK.Tools.Manipulator
{
    /// <summary>
    // @cdk.module standard
    // @cdk.githash
    ///
    /// <seealso cref="ChemModelManipulator"/>
    /// </summary>
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
                    mol.Remove(electrons);
                }
            }
            IAtomContainerSet<IAtomContainer> products = reaction.Products;
            for (int i = 0; i < products.Count; i++)
            {
                IAtomContainer mol = products[i];
                if (mol.Contains(electrons))
                {
                    mol.Remove(electrons);
                }
            }
        }

        /// <summary>
        /// Get all molecule of a <see cref="IReaction"/>: reactants + products.
        ///
        /// <param name="reaction">The IReaction</param>
        /// <returns>The IAtomContainerSet</returns>
        /// </summary>
        public static IAtomContainerSet<IAtomContainer> GetAllMolecules(IReaction reaction)
        {
            IAtomContainerSet<IAtomContainer> moleculeSet = reaction.Builder.CreateAtomContainerSet();

            moleculeSet.AddRange(GetAllReactants(reaction));
            moleculeSet.AddRange(GetAllProducts(reaction));

            return moleculeSet;
        }

        /// <summary>
        /// get all products of a IReaction
        ///
        /// <param name="reaction">The IReaction</param>
        /// <returns>The IAtomContainerSet</returns>
        /// </summary>
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
        ///
        /// <param name="reaction">The IReaction</param>
        /// <returns>The IAtomContainerSet</returns>
        /// </summary>
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

        /// <summary>
        /// Returns a new Reaction object which is the reverse of the given
        /// Reaction.
        /// <param name="reaction">the reaction being considered</param>
        /// <returns>the reverse reaction</returns>
        /// </summary>
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
        /// <param name="reaction">The reaction being considered</param>
        /// <returns>a list of the IAtomContainer objects comprising the reaction</returns>
        /// </summary>
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
        ///
        /// <param name="reaction">The IReaction which contains the mapping</param>
        /// <param name="chemObject">The IChemObject which will be searched its mapped IChemObject</param>
        /// <returns>The mapped IChemObject</returns>
        /// </summary>
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
    }
}
