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
using NCDK.Graphs;
using NCDK.Tools.Manipulator;
using System.Collections.Generic;

namespace NCDK.Reactions.Mechanisms
{
    /// <summary>
    /// This mechanism extracts a single electron from a bonding orbital which located in
    /// an bond. It could have single, double as triple order. It returns the
    /// reaction mechanism which has been cloned the <see cref="IAtomContainer"/> with a decrease
    /// of the order of the bond and a ISingleElectron more.
    /// </summary>
    // @author         miguelrojasch
    // @cdk.created    2008-02-10
    // @cdk.module     reaction
    // @cdk.githash
    public class RemovingSEofBMechanism : IReactionMechanism
    {
        /// <summary>
        /// Initiates the process for the given mechanism. The atoms to apply are mapped between
        /// reactants and products.
        /// </summary>
        /// <param name="atomContainerSet"></param>
        /// <param name="atomList">The list of atoms taking part in the mechanism. Only allowed two atoms. The first atom receives the charge and the second the single electron</param>
        /// <param name="bondList">The list of bonds taking part in the mechanism. Only allowed one bond</param>
        /// <returns>The Reaction mechanism</returns>
        public IReaction Initiate(IChemObjectSet<IAtomContainer> atomContainerSet, IList<IAtom> atomList, IList<IBond> bondList)
        {
            CDKAtomTypeMatcher atMatcher = CDKAtomTypeMatcher.GetInstance(atomContainerSet.Builder,
                    CDKAtomTypeMatcher.RequireExplicitHydrogens);
            if (atomContainerSet.Count != 1)
            {
                throw new CDKException("RemovingSEofBMechanism only expects one IAtomContainer");
            }
            if (atomList.Count != 2)
            {
                throw new CDKException("RemovingSEofBMechanism expects two atoms in the List");
            }
            if (bondList.Count != 1)
            {
                throw new CDKException("RemovingSEofBMechanism only expects one bond in the List");
            }
            IAtomContainer molecule = atomContainerSet[0];
            IAtomContainer reactantCloned;
            reactantCloned = (IAtomContainer)molecule.Clone();
            IAtom atom1 = atomList[0];
            IAtom atom1C = reactantCloned.Atoms[molecule.Atoms.IndexOf(atom1)];
            IAtom atom2 = atomList[1];
            IAtom atom2C = reactantCloned.Atoms[molecule.Atoms.IndexOf(atom2)];
            IBond bond1 = bondList[0];
            int posBond1 = molecule.Bonds.IndexOf(bond1);

            if (bond1.Order == BondOrder.Single)
                reactantCloned.Bonds.Remove(reactantCloned.Bonds[posBond1]);
            else
                BondManipulator.DecreaseBondOrder(reactantCloned.Bonds[posBond1]);

            int charge = atom1C.FormalCharge.Value;
            atom1C.FormalCharge = charge + 1;
            reactantCloned.SingleElectrons.Add(atom1C.Builder.NewSingleElectron(atom2C));

            // check if resulting atom type is reasonable
            atom1C.Hybridization = Hybridization.Unset;
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(reactantCloned);
            IAtomType type = atMatcher.FindMatchingAtomType(reactantCloned, atom1C);
            if (type == null || type.AtomTypeName.Equals("X")) return null;
            atom2C.Hybridization = Hybridization.Unset;
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(reactantCloned);
            type = atMatcher.FindMatchingAtomType(reactantCloned, atom2C);
            if (type == null || type.AtomTypeName.Equals("X")) return null;

            IReaction reaction = atom1C.Builder.NewReaction();
            reaction.Reactants.Add(molecule);

            /* mapping */
            foreach (var atom in molecule.Atoms)
            {
                IMapping mapping = atom1C.Builder.NewMapping(atom,
                        reactantCloned.Atoms[molecule.Atoms.IndexOf(atom)]);
                reaction.Mappings.Add(mapping);
            }
            if (bond1.Order != BondOrder.Single)
            {
                reaction.Products.Add(reactantCloned);
            }
            else
            {
                IChemObjectSet<IAtomContainer> moleculeSetP = ConnectivityChecker.PartitionIntoMolecules(reactantCloned);
                for (int z = 0; z < moleculeSetP.Count; z++)
                {
                    reaction.Products.Add((IAtomContainer)moleculeSetP[z]);
                }
            }

            return reaction;
        }
    }
}
