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
    /**
     * @cdk.module standard
     * @cdk.githash
     *
     * @see ChemModelManipulator
     */
    public class ReactionSetManipulator
    {
        public static int GetAtomCount(IReactionSet set)
        {
            int count = 0;
            foreach (var iReaction in set)
            {
                count += ReactionManipulator.GetAtomCount(iReaction);
            }
            return count;
        }

        public static int GetBondCount(IReactionSet set)
        {
            int count = 0;
            foreach (var iReaction in set)
            {
                count += ReactionManipulator.GetBondCount(iReaction);
            }
            return count;
        }

        public static void RemoveAtomAndConnectedElectronContainers(IReactionSet set, IAtom atom)
        {
            foreach (var reaction in set)
            {
                ReactionManipulator.RemoveAtomAndConnectedElectronContainers(reaction, atom);
            }
        }

        public static void RemoveElectronContainer(IReactionSet set, IElectronContainer electrons)
        {
            foreach (var reaction in set)
            {
                ReactionManipulator.RemoveElectronContainer(reaction, electrons);
            }
        }

        /**
         * get all Molecules object from a set of Reactions.
         *
         * @param set The set of reaction to inspect
         * @return    The IAtomContanerSet
         */
        public static IAtomContainerSet<IAtomContainer> GetAllMolecules(IReactionSet set)
        {
            IAtomContainerSet<IAtomContainer> moleculeSet = set.Builder.CreateAtomContainerSet();
            foreach (var reaction in set)
            {
                IAtomContainerSet<IAtomContainer> molecules = ReactionManipulator.GetAllMolecules(reaction);
                foreach (var ac in molecules)
                {
                    bool contain = false;
                    foreach (var atomContainer in moleculeSet)
                    {
                        if (atomContainer.Equals(ac))
                        {
                            contain = true;
                            break;
                        }
                    }
                    if (!contain) moleculeSet.Add(ac);

                }
            }
            return moleculeSet;
        }

        public static IList<string> GetAllIDs(IReactionSet set)
        {
            List<string> IDlist = new List<string>();
            if (set.Id != null) IDlist.Add(set.Id);
            foreach (var reaction in set)
            {
                IDlist.AddRange(ReactionManipulator.GetAllIDs(reaction));
            }
            return IDlist;
        }

        /**
         * Returns all the AtomContainer's of a Reaction.
         * @param set  the reaction set to get the molecules from
         * @return  a List containing the IAtomContainer objects in the IReactionSet
         */
        public static IEnumerable<IAtomContainer> GetAllAtomContainers(IReactionSet set)
        {

            return MoleculeSetManipulator.GetAllAtomContainers(GetAllMolecules(set));
        }

        public static IReaction GetRelevantReaction(IReactionSet set, IAtom atom)
        {
            foreach (var reaction in set)
            {
                IAtomContainer container = ReactionManipulator.GetRelevantAtomContainer(reaction, atom);
                if (container != null)
                { // a match!
                    return reaction;
                }
            }
            return null;
        }

        public static IReaction GetRelevantReaction(IReactionSet set, IBond bond)
        {
            foreach (var reaction in set)
            {
                IAtomContainer container = ReactionManipulator.GetRelevantAtomContainer(reaction, bond);
                if (container != null)
                { // a match!
                    return reaction;
                }
            }
            return null;
        }

        /**
         * Get all Reactions object containing a Molecule from a set of Reactions.
         *
         * @param reactSet The set of reaction to inspect
         * @param molecule The molecule to find
         * @return         The IReactionSet
         */
        public static IReactionSet GetRelevantReactions(IReactionSet reactSet, IAtomContainer molecule)
        {
            IReactionSet newReactSet = reactSet.Builder.CreateReactionSet();
            IReactionSet reactSetProd = GetRelevantReactionsAsProduct(reactSet, molecule);
            foreach (var reaction in reactSetProd)
                newReactSet.Add(reaction);
            IReactionSet reactSetReact = GetRelevantReactionsAsReactant(reactSet, molecule);
            foreach (var reaction in reactSetReact)
                newReactSet.Add(reaction);
            return newReactSet;
        }

        /**
         * Get all Reactions object containing a Molecule as a Reactant from a set
         * of Reactions.
         *
         * @param reactSet The set of reaction to inspect
         * @param molecule The molecule to find as a reactant
         * @return         The IReactionSet
         */
        public static IReactionSet GetRelevantReactionsAsReactant(IReactionSet reactSet, IAtomContainer molecule)
        {
            IReactionSet newReactSet = reactSet.Builder.CreateReactionSet();
            foreach (var reaction in reactSet)
            {
                foreach (var atomContainer in reaction.Reactants)
                    if (atomContainer.Equals(molecule)) newReactSet.Add(reaction);
            }
            return newReactSet;
        }

        /**
         * Get all Reactions object containing a Molecule as a Product from a set of
         * Reactions.
         *
         * @param reactSet The set of reaction to inspect
         * @param molecule The molecule to find as a product
         * @return         The IReactionSet
         */
        public static IReactionSet GetRelevantReactionsAsProduct(IReactionSet reactSet, IAtomContainer molecule)
        {
            IReactionSet newReactSet = reactSet.Builder.CreateReactionSet();
            foreach (var reaction in reactSet)
            {
                foreach (var atomContainer in reaction.Products)
                    if (atomContainer.Equals(molecule)) newReactSet.Add(reaction);
            }
            return newReactSet;
        }

        public static IAtomContainer GetRelevantAtomContainer(IReactionSet set, IAtom atom)
        {
            foreach (var reaction in set)
            {
                IAtomContainer container = ReactionManipulator.GetRelevantAtomContainer(reaction, atom);
                if (container != null)
                { // a match!
                    return container;
                }
            }
            return null;
        }

        public static IAtomContainer GetRelevantAtomContainer(IReactionSet set, IBond bond)
        {
            foreach (var reaction in set)
            {
                IAtomContainer container = ReactionManipulator.GetRelevantAtomContainer(reaction, bond);
                if (container != null)
                { // a match!
                    return container;
                }
            }
            return null;
        }

        public static void SetAtomProperties(IReactionSet set, string propKey, object propVal)
        {
            foreach (var reaction in set)
            {
                ReactionManipulator.SetAtomProperties(reaction, propKey, propVal);
            }
        }

        public static List<IChemObject> GetAllChemObjects(IReactionSet set)
        {
            List<IChemObject> list = new List<IChemObject>();
            list.Add(set);
            foreach (var reaction in set)
            {
                list.AddRange(ReactionManipulator.GetAllChemObjects(reaction));
            }
            return list;
        }

        /**
         * Gets a reaction from a ReactionSet by ID of any product or reactant. If several exist,
         * only the first one will be returned.
         *
         * @param reactionSet The reactionSet to search in
         * @param id The id to search for.
         * @return The Reaction or null;
         */
        public static IReaction GetReactionByAtomContainerID(IReactionSet reactionSet, string id)
        {
            foreach (var reaction in reactionSet)
            {
                if (AtomContainerSetManipulator.ContainsByID(reaction.Products, id)) return reaction;
            }
            foreach (var reaction in reactionSet)
            {
                if (AtomContainerSetManipulator.ContainsByID(reaction.Reactants, id)) return reaction;
            }
            return null;
        }

        /**
         * Gets a reaction from a ReactionSet by ID. If several exist,
         * only the first one will be returned.
         *
         * @param reactionSet The reactionSet to search in
         * @param id The id to search for.
         * @return The Reaction or null;
         */
        public static IReaction GetReactionByReactionID(IReactionSet reactionSet, string id)
        {
            foreach (var reaction in reactionSet)
            {
                if (reaction.Id != null && reaction.Id.Equals(id))
                {
                    return reaction;
                }
            }
            return null;
        }
    }
}
