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

using System;
using System.Collections.Generic;

namespace NCDK.Tools.Manipulator
{
    /**
     * Class with convenience methods that provide methods from
     * methods from ChemObjects within the ChemModel. For example:
     * <pre>
     * ChemModelManipulator.RemoveAtomAndConnectedElectronContainers(chemModel, atom);
     * </pre>
     * will find the Atom in the model by traversing the ChemModel's
     * MoleculeSet, Crystal and ReactionSet fields and remove
     * it with the RemoveAtomAndConnectedElectronContainers(Atom) method.
     *
     * @cdk.module standard
     * @cdk.githash
     *
     * @see org.openscience.cdk.AtomContainer#RemoveAtomAndConnectedElectronContainers(IAtom)
     */
    public class ChemModelManipulator
    {

        /**
         * Get the total number of atoms inside an IChemModel.
         *
         * @param chemModel  The IChemModel object.
         * @return           The number of Atom object inside.
         */
        public static int GetAtomCount(IChemModel chemModel)
        {
            int count = 0;
            ICrystal crystal = chemModel.Crystal;
            if (crystal != null)
            {
                count += crystal.Atoms.Count;
            }
            IAtomContainerSet<IAtomContainer> moleculeSet = chemModel.MoleculeSet;
            if (moleculeSet != null)
            {
                count += MoleculeSetManipulator.GetAtomCount(moleculeSet);
            }
            IReactionSet reactionSet = chemModel.ReactionSet;
            if (reactionSet != null)
            {
                count += ReactionSetManipulator.GetAtomCount(reactionSet);
            }
            return count;
        }

        /**
         * Get the total number of bonds inside an IChemModel.
         *
         * @param chemModel  The IChemModel object.
         * @return           The number of Bond object inside.
         */
        public static int GetBondCount(IChemModel chemModel)
        {
            int count = 0;
            ICrystal crystal = chemModel.Crystal;
            if (crystal != null)
            {
                count += crystal.Bonds.Count;
            }
            IAtomContainerSet<IAtomContainer> moleculeSet = chemModel.MoleculeSet;
            if (moleculeSet != null)
            {
                count += MoleculeSetManipulator.GetBondCount(moleculeSet);
            }
            IReactionSet reactionSet = chemModel.ReactionSet;
            if (reactionSet != null)
            {
                count += ReactionSetManipulator.GetBondCount(reactionSet);
            }
            return count;
        }

        /**
         * Remove an Atom and the connected ElectronContainers from all AtomContainers
         * inside an IChemModel.
         *
         * @param chemModel  The IChemModel object.
         * @param atom       The Atom object to remove.
         */

        public static void RemoveAtomAndConnectedElectronContainers(IChemModel chemModel, IAtom atom)
        {
            ICrystal crystal = chemModel.Crystal;
            if (crystal != null)
            {
                if (crystal.Contains(atom))
                {
                    crystal.RemoveAtomAndConnectedElectronContainers(atom);
                }
                return;
            }
            IAtomContainerSet<IAtomContainer> moleculeSet = chemModel.MoleculeSet;
            if (moleculeSet != null)
            {
                MoleculeSetManipulator.RemoveAtomAndConnectedElectronContainers(moleculeSet, atom);
            }
            IReactionSet reactionSet = chemModel.ReactionSet;
            if (reactionSet != null)
            {
                ReactionSetManipulator.RemoveAtomAndConnectedElectronContainers(reactionSet, atom);
            }
        }

        /**
         * Remove an ElectronContainer from all AtomContainers
         * inside an IChemModel.
         *
         * @param chemModel  The IChemModel object.
         * @param electrons  The ElectronContainer to remove.
         */
        public static void RemoveElectronContainer(IChemModel chemModel, IElectronContainer electrons)
        {
            ICrystal crystal = chemModel.Crystal;
            if (crystal != null)
            {
                if (crystal.Contains(electrons))
                {
                    crystal.Remove(electrons);
                }
                return;
            }
            IAtomContainerSet<IAtomContainer> moleculeSet = chemModel.MoleculeSet;
            if (moleculeSet != null)
            {
                MoleculeSetManipulator.RemoveElectronContainer(moleculeSet, electrons);
            }
            IReactionSet reactionSet = chemModel.ReactionSet;
            if (reactionSet != null)
            {
                ReactionSetManipulator.RemoveElectronContainer(reactionSet, electrons);
            }
        }

        /**
         * Adds a new Molecule to the MoleculeSet inside a given ChemModel.
         * Creates a MoleculeSet if none is contained.
         *
         * @param chemModel  The ChemModel object.
         * @return           The created Molecule object.
         */
        public static IAtomContainer CreateNewMolecule(IChemModel chemModel)
        {
            // Add a new molecule either the set of molecules
            IAtomContainer molecule = chemModel.Builder.CreateAtomContainer();
            if (chemModel.MoleculeSet != null)
            {
                IAtomContainerSet<IAtomContainer> moleculeSet = chemModel.MoleculeSet;
                for (int i = 0; i < moleculeSet.Count; i++)
                {
                    if (moleculeSet[i].Atoms.Count == 0)
                    {
                        moleculeSet.RemoveAt(i);
                        i--;
                    }
                }
                moleculeSet.Add(molecule);
            }
            else
            {
                IAtomContainerSet<IAtomContainer> moleculeSet = chemModel.Builder.CreateAtomContainerSet();
                moleculeSet.Add(molecule);
                chemModel.MoleculeSet = moleculeSet;
            }
            return molecule;
        }

        /**
         * Create a new ChemModel containing an IAtomContainer. It will create an
         * <see cref="IAtomContainer"/> from the passed IAtomContainer when needed, which may cause
         * information loss.
         *
         * @param  atomContainer  The AtomContainer to have inside the ChemModel.
         * @return                The new IChemModel object.
         */
        public static IChemModel NewChemModel(IAtomContainer atomContainer)
        {
            IChemModel model = atomContainer.Builder.CreateChemModel();
            IAtomContainerSet<IAtomContainer> moleculeSet = model.Builder.CreateAtomContainerSet();
            moleculeSet.Add(atomContainer);
            model.MoleculeSet = moleculeSet;
            return model;
        }

        /**
         * This badly named methods tries to determine which AtomContainer in the
         * ChemModel is best suited to contain added Atom's and Bond's.
         */
        public static IAtomContainer GetRelevantAtomContainer(IChemModel chemModel, IAtom atom)
        {
            IAtomContainer result = null;
            if (chemModel.MoleculeSet != null)
            {
                IAtomContainerSet<IAtomContainer> moleculeSet = chemModel.MoleculeSet;
                result = MoleculeSetManipulator.GetRelevantAtomContainer(moleculeSet, atom);
                if (result != null)
                {
                    return result;
                }
            }
            if (chemModel.ReactionSet != null)
            {
                IReactionSet reactionSet = chemModel.ReactionSet;
                return ReactionSetManipulator.GetRelevantAtomContainer(reactionSet, atom);
            }
            if (chemModel.Crystal != null && chemModel.Crystal.Contains(atom))
            {
                return chemModel.Crystal;
            }
            if (chemModel.RingSet != null)
            {
                return AtomContainerSetManipulator.GetRelevantAtomContainer(chemModel.RingSet, atom);
            }
            throw new ArgumentException("The provided atom is not part of this IChemModel.");
        }

        /**
         * Retrieves the first IAtomContainer containing a given IBond from an
         * IChemModel.
         *
         * @param chemModel  The IChemModel object.
         * @param bond       The IBond object to search.
         * @return           The IAtomContainer object found, null if none is found.
         */
        public static IAtomContainer GetRelevantAtomContainer(IChemModel chemModel, IBond bond)
        {
            IAtomContainer result = null;
            if (chemModel.MoleculeSet != null)
            {
                IAtomContainerSet<IAtomContainer> moleculeSet = chemModel.MoleculeSet;
                result = MoleculeSetManipulator.GetRelevantAtomContainer(moleculeSet, bond);
                if (result != null)
                {
                    return result;
                }
            }
            if (chemModel.ReactionSet != null)
            {
                IReactionSet reactionSet = chemModel.ReactionSet;
                return ReactionSetManipulator.GetRelevantAtomContainer(reactionSet, bond);
            }
            // This should never happen.
            return null;
        }

        /**
         * Retrieves the first IReaction containing a given IAtom from an
         * IChemModel.
         *
         * @param chemModel  The IChemModel object.
         * @param atom       The IAtom object to search.
         * @return           The IAtomContainer object found, null if none is found.
         */
        public static IReaction GetRelevantReaction(IChemModel chemModel, IAtom atom)
        {
            IReaction reaction = null;
            if (chemModel.ReactionSet != null)
            {
                IReactionSet reactionSet = chemModel.ReactionSet;
                reaction = ReactionSetManipulator.GetRelevantReaction(reactionSet, atom);
            }
            return reaction;
        }

        /**
         * Returns all the AtomContainer's of a ChemModel.
         */
        public static IEnumerable<IAtomContainer> GetAllAtomContainers(IChemModel chemModel)
        {
            IAtomContainerSet<IAtomContainer> moleculeSet = chemModel.Builder.CreateAtomContainerSet();
            if (chemModel.MoleculeSet != null)
            {
                moleculeSet.AddRange(chemModel.MoleculeSet);
            }
            if (chemModel.ReactionSet != null)
            {
                moleculeSet.AddRange(ReactionSetManipulator.GetAllMolecules(chemModel.ReactionSet));
            }
            return MoleculeSetManipulator.GetAllAtomContainers(moleculeSet);
        }

        /**
         * Sets the AtomProperties of all Atoms inside an IChemModel.
         *
         * @param chemModel  The IChemModel object.
         * @param propKey    The key of the property.
         * @param propVal    The value of the property.
         */
        public static void SetAtomProperties(IChemModel chemModel, string propKey, object propVal)
        {
            if (chemModel.MoleculeSet != null)
            {
                MoleculeSetManipulator.SetAtomProperties(chemModel.MoleculeSet, propKey, propVal);
            }
            if (chemModel.ReactionSet != null)
            {
                ReactionSetManipulator.SetAtomProperties(chemModel.ReactionSet, propKey, propVal);
            }
            if (chemModel.Crystal != null)
            {
                AtomContainerManipulator.SetAtomProperties(chemModel.Crystal, propKey, propVal);
            }
        }

        /**
         * Retrieve a List of all ChemObject objects within an IChemModel.
         *
         * @param chemModel  The IChemModel object.
         * @return           A List of all ChemObjects inside.
         */
        public static List<IChemObject> GetAllChemObjects(IChemModel chemModel)
        {
            List<IChemObject> list = new List<IChemObject>();
            // list.Add(chemModel); // only add ChemObjects contained within
            ICrystal crystal = chemModel.Crystal;
            if (crystal != null)
            {
                list.Add(crystal);
            }
            IAtomContainerSet<IAtomContainer> moleculeSet = chemModel.MoleculeSet;
            if (moleculeSet != null)
            {
                list.Add(moleculeSet);
                var current = MoleculeSetManipulator.GetAllChemObjects(moleculeSet);
                foreach (var chemObject in current)
                {
                    if (!list.Contains(chemObject)) list.Add(chemObject);
                }
            }
            IReactionSet reactionSet = chemModel.ReactionSet;
            if (reactionSet != null)
            {
                list.Add(reactionSet);
                List<IChemObject> current = ReactionSetManipulator.GetAllChemObjects(reactionSet);
                foreach (var chemObject in current)
                {
                    if (!list.Contains(chemObject)) list.Add(chemObject);
                }
            }
            return list;
        }

        public static List<string> GetAllIDs(IChemModel chemModel)
        {
            List<string> list = new List<string>();
            if (chemModel.Id != null) list.Add(chemModel.Id);
            ICrystal crystal = chemModel.Crystal;
            if (crystal != null)
            {
                list.AddRange(AtomContainerManipulator.GetAllIDs(crystal));
            }
            IAtomContainerSet<IAtomContainer> moleculeSet = chemModel.MoleculeSet;
            if (moleculeSet != null)
            {
                list.AddRange(MoleculeSetManipulator.GetAllIDs(moleculeSet));
            }
            IReactionSet reactionSet = chemModel.ReactionSet;
            if (reactionSet != null)
            {
                list.AddRange(ReactionSetManipulator.GetAllIDs(reactionSet));
            }
            return list;
        }
    }
}
