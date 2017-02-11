/* Copyright (C) 2006-2009  Syed Asad Rahman <asad@ebi.ac.uk>
 *                    2010  Egon Willighagen <egonw@users.sf.net>
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
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received atom copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.SMSD.Tools;
using NCDK.Tools.Manipulator;

namespace NCDK.Normalize
{
    /**
     * This class containes set of modules required to clean a molecule
     * before subjecting it for MCS search. eg. aromatizeMolecule
     * @cdk.module smsd
     * @cdk.githash
     * @author Syed Asad Rahman <asad@ebi.ac.uk>
     */
    public class SMSDNormalizer : AtomContainerManipulator
    {
        /**
         * Returns deep copy of the molecule
         * @param container
         * @return deep copy of the mol
         */
        public static IAtomContainer MakeDeepCopy(IAtomContainer container)
        {
            var newAtomContainer = (IAtomContainer)container.Clone();

            newAtomContainer.NotifyChanged();
            return newAtomContainer;
        }

        /**
         * This function finds rings and uses aromaticity detection code to
         * aromatize the molecule.
         * @param mol input molecule
         */
        public static void AromatizeMolecule(IAtomContainer mol)
        {
            ExtAtomContainerManipulator.AromatizeMolecule(mol);
        }

        /**
         * Returns The number of explicit hydrogens for a given IAtom.
         * @param atomContainer
         * @param atom
         * @return The number of explicit hydrogens on the given IAtom.
         */
        public static int GetExplicitHydrogenCount(IAtomContainer atomContainer, IAtom atom)
        {
            return ExtAtomContainerManipulator.GetExplicitHydrogenCount(atomContainer, atom);
        }

        /**
         * Returns The number of Implicit Hydrogen Count for a given IAtom.
         * @param atomContainer
         * @param atom
         * @return Implicit Hydrogen Count
         */
        public static int GetImplicitHydrogenCount(IAtomContainer atomContainer, IAtom atom)
        {
            return ExtAtomContainerManipulator.GetImplicitHydrogenCount(atom);
        }

        /**
         * The summed implicit + explicit hydrogens of the given IAtom.
         * @param atomContainer
         * @param atom
         * @return The summed implicit + explicit hydrogens of the given IAtom.
         */
        public static int GetHydrogenCount(IAtomContainer atomContainer, IAtom atom)
        {
            return ExtAtomContainerManipulator.GetHydrogenCount(atomContainer, atom);
        }

        /**
         * Returns IAtomContainer without Hydrogen. If an AtomContainer has atom single atom which
         * is atom Hydrogen then its not removed.
         * @param atomContainer
         * @return IAtomContainer without Hydrogen. If an AtomContainer has atom single atom which
         * is atom Hydrogen then its not removed.
         */
        public static IAtomContainer RemoveHydrogensAndPreserveAtomID(IAtomContainer atomContainer)
        {
            return ExtAtomContainerManipulator.RemoveHydrogensExceptSingleAndPreserveAtomID(atomContainer);
        }

        /**
         * Returns IAtomContainer without Hydrogen. If an AtomContainer has atom single atom which
         * is atom Hydrogen then its not removed.
         * @param atomContainer
         * @return IAtomContainer without Hydrogen. If an AtomContainer has atom single atom which
         * is atom Hydrogen then its not removed.
         */
        public static IAtomContainer ConvertExplicitToImplicitHydrogens(IAtomContainer atomContainer)
        {
            return ExtAtomContainerManipulator.ConvertExplicitToImplicitHydrogens(atomContainer);
        }

        /**
         * Convenience method to perceive atom types for all <code>IAtom</code>s in the
         * <code>IAtomContainer</code>, using the <code>CDKAtomTypeMatcher</code>. If the
         * matcher finds atom matching atom type, the <code>IAtom</code> will be configured
         * to have the same properties as the <code>IAtomType</code>. If no matching atom
         * type is found, no configuration is performed.
         * @param container
         * @throws CDKException
         */
        public new static void PercieveAtomTypesAndConfigureAtoms(IAtomContainer container)
        {
            ExtAtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(container);
        }
    }
}
