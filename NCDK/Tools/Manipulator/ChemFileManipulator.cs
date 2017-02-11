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
 */

using System.Collections.Generic;

namespace NCDK.Tools.Manipulator
{
    /**
     * Class with convenience methods that provide methods from
     * methods from ChemObjects within the ChemFile.
     *
     * @see org.openscience.cdk.AtomContainer#RemoveAtomAndConnectedElectronContainers(IAtom)
     *
     * @cdk.module standard
     * @cdk.githash
     */
    public class ChemFileManipulator
    {
        /**
         * Get the total number of atoms inside an IChemFile.
         *
         * @param file       The IChemFile object.
         * @return           The number of Atom object inside.
         */

        public static int GetAtomCount(IChemFile file)
        {
            int count = 0;
            for (int i = 0; i < file.Count; i++)
            {
                count += ChemSequenceManipulator.GetAtomCount(file[i]);
            }
            return count;
        }

        /**
         * Get the total number of bonds inside an IChemFile.
         *
         * @param file       The IChemFile object.
         * @return           The number of Bond object inside.
         */
        public static int GetBondCount(IChemFile file)
        {
            int count = 0;
            for (int i = 0; i < file.Count; i++)
            {
                count += ChemSequenceManipulator.GetBondCount(file[i]);
            }
            return count;
        }

        /**
         * Returns a List of all IChemObject inside a ChemFile.
         *
         * @return  A list of all ChemObjects
         */
        public static IEnumerable<IChemObject> GetAllChemObjects(IChemFile file)
        {
            //list.Add(file); // should not add the original file
            foreach (var sequence in file)
            {
                yield return sequence;
                foreach (var o in ChemSequenceManipulator.GetAllChemObjects(sequence))
                    yield return o;
            }
            yield break;
        }

        public static IEnumerable<string> GetAllIDs(IChemFile file)
        {
            if (file.Id != null)
                yield return file.Id;
            foreach (var sequence in file)
                foreach (var id in ChemSequenceManipulator.GetAllIDs(sequence))
                    yield return id;
            yield break;
        }

        /**
         * Returns all the AtomContainer's of a ChemFile.
         */
        public static IEnumerable<IAtomContainer> GetAllAtomContainers(IChemFile file)
        {
            foreach (var sequence in file)
                foreach (var ac in ChemSequenceManipulator.GetAllAtomContainers(sequence))
                    yield return ac;
            yield break;
        }

        /**
         * Get a list of all ChemModels inside an IChemFile.
         *
         * @param file  The IChemFile object.
         * @return      The List of IChemModel objects inside.
         */
        public static IEnumerable<IChemModel> GetAllChemModels(IChemFile file)
        {
            foreach (var models in file)
                foreach (var model in models)
                    yield return model;
            yield break;
        }

        /**
         * Get a list of all IReaction inside an IChemFile.
         *
         * @param file  The IChemFile object.
         * @return      The List of IReaction objects inside.
         */
        public static IEnumerable<IReaction> GetAllReactions(IChemFile file)
        {
            foreach (var model in GetAllChemModels(file))
                foreach (var reaction in model.ReactionSet)
                    yield return reaction;
            yield break;
        }
    }
}

