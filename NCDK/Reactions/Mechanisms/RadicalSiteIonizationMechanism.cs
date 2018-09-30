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
using System;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Reactions.Mechanisms
{
    /// <summary>
    /// <para>This mechanism extracts an atom because of the stabilization of a radical.
    /// It returns the reaction mechanism which has been cloned the IAtomContainer.</para>
    /// <para>This reaction could be represented as Y-B-[C*] =&gt; [Y*] + B=C</para>
    /// </summary>
    // @author         miguelrojasch
    // @cdk.created    2008-02-10
    // @cdk.module     reaction
    // @cdk.githash
    public class RadicalSiteIonizationMechanism : IReactionMechanism
    {

        /// <summary>
        /// Initiates the process for the given mechanism. The atoms to apply are mapped between
        /// reactants and products.
        /// </summary>
        /// <param name="atomContainerSet"></param>
        /// <param name="atomList">
        /// The list of atoms taking part in the mechanism. Only allowed two atoms.
        ///                    The first atom is the atom which contains the ISingleElectron and the second
        ///                    third is the atom which will be removed
        ///                    the first atom</param>
        /// <param name="bondList">The list of bonds taking part in the mechanism. Only allowed one bond.
        ///                       It is the bond which is moved</param>
        /// <returns>The Reaction mechanism</returns>
        public IReaction Initiate(IChemObjectSet<IAtomContainer> atomContainerSet, IList<IAtom> atomList, IList<IBond> bondList)
        {
            CDKAtomTypeMatcher atMatcher = CDKAtomTypeMatcher.GetInstance(atomContainerSet.Builder);
            if (atomContainerSet.Count != 1)
            {
                throw new CDKException("RadicalSiteIonizationMechanism only expects one IAtomContainer");
            }
            if (atomList.Count != 3)
            {
                throw new CDKException("RadicalSiteIonizationMechanism expects three atoms in the List");
            }
            if (bondList.Count != 2)
            {
                throw new CDKException("RadicalSiteIonizationMechanism only expect one bond in the List");
            }
            IAtomContainer molecule = atomContainerSet[0];
            IAtomContainer reactantCloned;
            reactantCloned = (IAtomContainer)molecule.Clone();
            IAtom atom1 = atomList[0];// Atom containing the ISingleElectron
            IAtom atom1C = reactantCloned.Atoms[molecule.Atoms.IndexOf(atom1)];
            IAtom atom2 = atomList[1];// Atom
            IAtom atom2C = reactantCloned.Atoms[molecule.Atoms.IndexOf(atom2)];
            IAtom atom3 = atomList[2];// Atom to be saved
            IAtom atom3C = reactantCloned.Atoms[molecule.Atoms.IndexOf(atom3)];
            IBond bond1 = bondList[0];// Bond to increase the order
            int posBond1 = molecule.Bonds.IndexOf(bond1);
            IBond bond2 = bondList[1];// Bond to remove
            int posBond2 = molecule.Bonds.IndexOf(bond2);

            BondManipulator.IncreaseBondOrder(reactantCloned.Bonds[posBond1]);
            reactantCloned.Bonds.Remove(reactantCloned.Bonds[posBond2]);

            var selectron = reactantCloned.GetConnectedSingleElectrons(atom1C);
            reactantCloned.SingleElectrons.Remove(selectron.Last());
            atom1C.Hybridization = Hybridization.Unset;
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(reactantCloned);
            IAtomType type = atMatcher.FindMatchingAtomType(reactantCloned, atom1C);
            if (type == null || type.AtomTypeName.Equals("X", StringComparison.Ordinal))
                return null;

            atom2C.Hybridization = Hybridization.Unset;
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(reactantCloned);
            type = atMatcher.FindMatchingAtomType(reactantCloned, atom2C);
            if (type == null || type.AtomTypeName.Equals("X", StringComparison.Ordinal))
                return null;

            reactantCloned.SingleElectrons.Add(atom2C.Builder.NewSingleElectron(atom3C));
            atom3C.Hybridization = Hybridization.Unset;
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(reactantCloned);
            type = atMatcher.FindMatchingAtomType(reactantCloned, atom3C);
            if (type == null || type.AtomTypeName.Equals("X", StringComparison.Ordinal))
                return null;

            IReaction reaction = atom2C.Builder.NewReaction();
            reaction.Reactants.Add(molecule);

            /* mapping */
            foreach (var atom in molecule.Atoms)
            {
                IMapping mapping = atom2C.Builder.NewMapping(atom,
                        reactantCloned.Atoms[molecule.Atoms.IndexOf(atom)]);
                reaction.Mappings.Add(mapping);
            }

            IChemObjectSet<IAtomContainer> moleculeSetP = ConnectivityChecker.PartitionIntoMolecules(reactantCloned);
            for (int z = 0; z < moleculeSetP.Count(); z++)
                reaction.Products.Add((IAtomContainer)moleculeSetP[z]);

            return reaction;
        }
    }
}
