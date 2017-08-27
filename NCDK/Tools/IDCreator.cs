/* Copyright (C) 2003-2007  Egon Willighagen <egonw@users.sf.net>
 *                    2008  Aleksey Tarkhov <bayern7105@yahoo.de>
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
 */
using NCDK.Tools.Manipulator;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Tools
{
    /// <summary>
    /// Class that provides methods to give unique IDs to ChemObjects.
    /// Methods are implemented for Atom, Bond, AtomContainer, AtomContainerSet
    /// and Reaction. It will only create missing IDs. If you want to create new
    /// IDs for all ChemObjects, you need to delete them first.
    /// </summary>
    // @cdk.module standard
    // @cdk.githash
    // @author   Egon Willighagen
    // @cdk.created  2003-04-01
    // @cdk.keyword  id, creation
    public abstract class IDCreator
    {
        // counters for generated in current session IDs
        private static int reactionCount = 0;
        private static int atomCount = 0;
        private static int bondCount = 0;
        private static int atomContainerCount = 0;
        private static int atomContainerSetCount = 0;
        private static int reactionSetCount = 0;
        private static int chemModelCount = 0;
        private static int chemSequenceCount = 0;
        private static int chemFileCount = 0;

        // prefix to prepend every individual IDs
        private const string REACTION_PREFIX = "r";
        private const string ATOM_PREFIX = "a";
        private const string BOND_PREFIX = "b";
        private const string ATOMCONTAINER_PREFIX = "m";
        private const string ATOMCONTAINERSET_PREFIX = "molSet";
        private const string REACTIONSET_PREFIX = "rset";
        private const string CHEMMODEL_PREFIX = "model";
        private const string CHEMSEQUENCE_PREFIX = "seq";
        private const string CHEMFILE_PREFIX = "file";

        /// <summary>
        /// Old ID generation policy - to generate IDs unique over the entire set
        /// </summary>
        public const int SET_UNIQUE_POLICY = 0;

        /// <summary>
        /// New ID generation policy - to generate IDs unique only in a molecule
        /// </summary>
        public const int OBJECT_UNIQUE_POLICY = 1;

        /// <summary>
        /// Internal flag identifying the IDs generation policy. The old policy
        /// is to generate IDs so that in a sequence of several molecules all the
        /// atoms and bonds will receive the unique IDs even across molecules, i.e.
        /// in a set of 2 molecules the first atom of the first molecule will be "a1"
        /// while the first atom of the second molecule will be "aX" where X equals
        /// to the number of atoms in the first molecule plus 1.
        /// </summary>
        /// <remarks>
        /// The new policy is to keep the singularity of IDs only within a single
        /// molecule, i.e. in a set of two molecules first atoms of each will be "a1".
        /// </remarks>
        private static int policy = SET_UNIQUE_POLICY;

        /// <summary>
        /// Alters the policy of ID generation. The IDCreator should in any case
        /// preserve the already existing IDs therefore if one of objects already
        /// has an ID set, this ID will be skipped in all the cases when attempting to
        /// generate a new ID value
        /// <param name="policy">new policy to be used</param>
        /// <seealso cref="OBJECT_UNIQUE_POLICY"/>
        /// <seealso cref="SET_UNIQUE_POLICY"/>
        /// </summary>
        public static void SetIDPolicy(int policy)
        {
            IDCreator.policy = policy;
        }

        /// <summary>
        /// Labels the Atom's and Bond's in the AtomContainer using the a1, a2, b1, b2
        /// scheme often used in CML. Supports IAtomContainer, IAtomContainerSet,
        /// IChemFile, IChemModel, IChemSequence, IReaction, IReactionSet,
        /// and derived interfaces.
        /// </summary>
        /// <param name="chemObject">IChemObject to create IDs for.</param>
        public static void CreateIDs(IChemObject chemObject)
        {
            if (chemObject == null) return;

            ReSetCounters();

            if (chemObject is IAtomContainer)
            {
                CreateIDsForAtomContainer((IAtomContainer)chemObject, null);
            }
            else if (chemObject is IChemObjectSet<IAtomContainer>)
            {
                CreateIDsForAtomContainerSet((IChemObjectSet<IAtomContainer>)chemObject, null);
            }
            else if (chemObject is IReaction)
            {
                CreateIDsForReaction((IReaction)chemObject, null);
            }
            else if (chemObject is IReactionSet)
            {
                CreateIDsForReactionSet((IReactionSet)chemObject, null);
            }
            else if (chemObject is IChemFile)
            {
                CreateIDsForChemFile((IChemFile)chemObject, null);
            }
            else if (chemObject is IChemSequence)
            {
                CreateIDsForChemSequence((IChemSequence)chemObject, null);
            }
            else if (chemObject is IChemModel)
            {
                CreateIDsForChemModel((IChemModel)chemObject, null);
            }
        }

        /// <summary>
        /// Reset the counters so that we keep generating simple IDs within
        /// single chem object or a set of them
        /// </summary>
        private static void ReSetCounters()
        {
            atomCount = 0;
            bondCount = 0;
            atomContainerCount = 0;
            atomContainerSetCount = 0;
            reactionCount = 0;
            reactionSetCount = 0;
            chemModelCount = 0;
            chemSequenceCount = 0;
            chemFileCount = 0;
        }

        /// <summary>
        /// Sets the ID on the object and adds it to the tabu list.
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="identifier"></param>
        /// <param name="obj">IChemObject to set the ID for</param>
        /// <param name="tabuList">Tabu list to add the ID to</param>
        private static int SetId(string prefix, int identifier, IChemObject obj, IList<string> tabuList)
        {
            identifier += 1;
            while (tabuList.Contains(prefix + identifier))
            {
                identifier += 1;
            }
            obj.Id = prefix + identifier;
            tabuList.Add(prefix + identifier);
            return identifier;
        }

        /// <summary>
        /// Labels the Atom's and Bond's in the AtomContainer using the a1, a2, b1, b2
        /// scheme often used in CML.
        /// </summary>
        /// <seealso cref="CreateIDs(IChemObject)"/>
        private static void CreateIDsForAtomContainer(IAtomContainer container, IList<string> tabuList)
        {
            if (tabuList == null) tabuList = AtomContainerManipulator.GetAllIDs(container);

            if (null == container.Id)
            {
                // generate new ID and remember it
                atomContainerCount = SetId(ATOMCONTAINER_PREFIX, atomContainerCount, container, tabuList);
            }

            // the tabu list for the container should force singularity
            // within a container only!
            IList<string> internalTabuList = AtomContainerManipulator.GetAllIDs(container);
            if (policy == OBJECT_UNIQUE_POLICY)
            {
                // start atom and bond indices within a container set always from 1
                atomCount = 0;
                bondCount = 0;
            }
            else
            {
                internalTabuList = tabuList;
            }

            foreach (var atom in container.Atoms)
            {
                if (null == atom.Id)
                {
                    atomCount = SetId(ATOM_PREFIX, atomCount, atom, internalTabuList);
                }
            }

            foreach (var bond in container.Bonds)
            {
                if (null == bond.Id)
                {
                    bondCount = SetId(BOND_PREFIX, bondCount, bond, internalTabuList);
                }
            }
        }

        /// <summary>
        /// Labels the Atom's and Bond's in each AtomContainer using the a1, a2, b1, b2
        /// scheme often used in CML. It will also set id's for all AtomContainers, naming
        /// them m1, m2, etc.
        /// It will not the AtomContainerSet itself.
        /// </summary>
        private static void CreateIDsForAtomContainerSet(IChemObjectSet<IAtomContainer> containerSet, IList<string> tabuList)
        {
            if (tabuList == null) tabuList = AtomContainerSetManipulator.GetAllIDs(containerSet).ToList();

            if (null == containerSet.Id)
            {
                atomContainerSetCount = SetId(ATOMCONTAINERSET_PREFIX, atomContainerSetCount, containerSet, tabuList);
            }

            if (policy == OBJECT_UNIQUE_POLICY)
            {
                // start atom and bond indices within a container set always from 1
                atomCount = 0;
                bondCount = 0;
            }

            foreach (var ac in containerSet)
            {
                CreateIDsForAtomContainer(ac, tabuList);
            }
        }

        /// <summary>
        /// Labels the reactants and products in the Reaction m1, m2, etc, and the atoms
        /// accordingly, when no ID is given.
        /// </summary>
        private static void CreateIDsForReaction(IReaction reaction, IList<string> tabuList)
        {
            if (tabuList == null)
                tabuList = ReactionManipulator.GetAllIDs(reaction).ToList();

            if (null == reaction.Id)
            {
                // generate new ID
                reactionCount = SetId(REACTION_PREFIX, reactionCount, reaction, tabuList);
            }

            if (policy == OBJECT_UNIQUE_POLICY)
            {
                // start atom and bond indices within a reaction set always from 1
                atomCount = 0;
                bondCount = 0;
            }

            foreach (var reactant in reaction.Reactants)
            {
                CreateIDsForAtomContainer(reactant, tabuList);
            }
            foreach (var product in reaction.Reactants)
            {
                CreateIDsForAtomContainer(product, tabuList);
            }
            foreach (var agent in reaction.Agents)
            {
                CreateIDsForAtomContainer(agent, tabuList);
            }
        }

        private static void CreateIDsForReactionSet(IReactionSet reactionSet, IList<string> tabuList)
        {
            if (tabuList == null) tabuList = ReactionSetManipulator.GetAllIDs(reactionSet);

            if (null == reactionSet.Id)
            {
                // generate new ID for the set
                reactionSetCount = SetId(REACTIONSET_PREFIX, reactionSetCount, reactionSet, tabuList);
            }

            foreach (var reaction in reactionSet)
            {
                CreateIDsForReaction(reaction, tabuList);
            }
        }

        private static void CreateIDsForChemFile(IChemFile file, IList<string> tabuList)
        {
            if (tabuList == null) tabuList = ChemFileManipulator.GetAllIDs(file).ToList();

            if (null == file.Id)
            {
                chemFileCount = SetId(CHEMFILE_PREFIX, chemFileCount, file, tabuList);
            }

            if (policy == OBJECT_UNIQUE_POLICY)
            {
                // start indices within a chem file always from 1
                chemSequenceCount = 0;
            }

            foreach (var chemSequence in file)
            {
                CreateIDsForChemSequence(chemSequence, tabuList);
            }
        }

        private static void CreateIDsForChemSequence(IChemSequence sequence, IList<string> tabuList)
        {
            if (tabuList == null) tabuList = ChemSequenceManipulator.GetAllIDs(sequence);

            if (null == sequence.Id)
            {
                chemSequenceCount = SetId(CHEMSEQUENCE_PREFIX, chemSequenceCount, sequence, tabuList);
            }

            if (policy == OBJECT_UNIQUE_POLICY)
            {
                // start indices within a chem file always from 1
                chemSequenceCount = 0;
            }

            foreach (var chemModel in sequence)
            {
                CreateIDsForChemModel(chemModel, tabuList);
            }
        }

        private static void CreateIDsForChemModel(IChemModel model, IList<string> tabuList)
        {
            if (tabuList == null) tabuList = ChemModelManipulator.GetAllIDs(model);

            if (null == model.Id)
            {
                chemModelCount = SetId(CHEMMODEL_PREFIX, chemModelCount, model, tabuList);
            }

            ICrystal crystal = model.Crystal;
            if (crystal != null)
            {
                if (policy == OBJECT_UNIQUE_POLICY)
                {
                    atomCount = 0;
                    bondCount = 0;
                }
                CreateIDsForAtomContainer(crystal, tabuList);
            }

            IChemObjectSet<IAtomContainer> moleculeSet = model.MoleculeSet;
            if (moleculeSet != null)
            {
                if (policy == OBJECT_UNIQUE_POLICY)
                {
                    atomContainerSetCount = 0;
                    atomContainerCount = 0;
                }
                CreateIDsForAtomContainerSet(moleculeSet, tabuList);
            }

            IReactionSet reactionSet = model.ReactionSet;
            if (reactionSet != null)
            {
                if (policy == OBJECT_UNIQUE_POLICY)
                {
                    reactionSetCount = 0;
                    reactionCount = 0;
                }
                CreateIDsForReactionSet(reactionSet, tabuList);
            }
        }
    }
}
