/* Copyright (C) 2008  Miguel Rojas <miguelrojasch@yahoo.es>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
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
using NCDK.AtomTypes;
using NCDK.Tools.Manipulator;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Reactions.Mechanisms
{
    /**
     * This mechanism extracts a single electron from a non-bonding orbital which located in
     * a ILonePair container. It returns the reaction mechanism which has been cloned the
     * <see cref="IAtomContainer"/> with an ILonPair electron less and an ISingleElectron more.
     *
     * @author         miguelrojasch
     * @cdk.created    2008-02-10
     * @cdk.module     reaction
     * @cdk.githash
     */
    public class RemovingSEofNBMechanism : IReactionMechanism
    {

        /**
         * Initiates the process for the given mechanism. The atoms to apply are mapped between
         * reactants and products.
         *
         *
         * @param atomContainerSet
         * @param atomList    The list of atoms taking part in the mechanism. Only allowed one atom
         * @param bondList    The list of bonds taking part in the mechanism. Only allowed one Bond
         * @return            The Reaction mechanism
         *
         */

        public IReaction Initiate(IChemObjectSet<IAtomContainer> atomContainerSet, IList<IAtom> atomList, IList<IBond> bondList)
        {
            CDKAtomTypeMatcher atMatcher = CDKAtomTypeMatcher.GetInstance(atomContainerSet.Builder);
            if (atomContainerSet.Count != 1)
            {
                throw new CDKException("RemovingSEofNBMechanism only expects one IAtomContainer");
            }
            if (atomList.Count != 1)
            {
                throw new CDKException("RemovingSEofNBMechanism only expects one atom in the List");
            }
            if (bondList != null)
            {
                throw new CDKException("RemovingSEofNBMechanism don't expect any bond in the List");
            }
            IAtomContainer molecule = atomContainerSet[0];
            IAtomContainer reactantCloned;
            reactantCloned = (IAtomContainer)molecule.Clone();

            // remove one lone pair electron and substitute with one single electron and charge 1.
            int posAtom = molecule.Atoms.IndexOf(atomList[0]);
            var lps = reactantCloned.GetConnectedLonePairs(reactantCloned.Atoms[posAtom]);
            reactantCloned.LonePairs.Remove(lps.Last());

            reactantCloned.SingleElectrons.Add(molecule.Builder.NewSingleElectron(reactantCloned.Atoms[posAtom]));
            int charge = reactantCloned.Atoms[posAtom].FormalCharge.Value;
            reactantCloned.Atoms[posAtom].FormalCharge = charge + 1;

            // check if resulting atom type is reasonable
            reactantCloned.Atoms[posAtom].Hybridization = Hybridization.Unset;
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(reactantCloned);
            IAtomType type = atMatcher.FindMatchingAtomType(reactantCloned, reactantCloned.Atoms[posAtom]);
            if (type == null || type.AtomTypeName.Equals("X")) return null;

            IReaction reaction = molecule.Builder.NewReaction();
            reaction.Reactants.Add(molecule);

            /* mapping */
            foreach (var atom in molecule.Atoms)
            {
                IMapping mapping = molecule.Builder.NewMapping(atom,
                        reactantCloned.Atoms[molecule.Atoms.IndexOf(atom)]);
                reaction.Mappings.Add(mapping);
            }
            reaction.Products.Add(reactantCloned);

            return reaction;
        }
    }
}
