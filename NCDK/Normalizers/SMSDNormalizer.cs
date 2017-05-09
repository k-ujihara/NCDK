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
    /// <summary>
    /// This class containes set of modules required to clean a molecule
    /// before subjecting it for MCS search. eg. aromatizeMolecule
    /// </summary>
    // @cdk.githash
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    public class SMSDNormalizer : AtomContainerManipulator
    {
        /// <summary>
        /// Returns deep copy of the molecule
        /// </summary>
        /// <param name="container"></param>
        /// <returns>deep copy of the mol</returns>
        public static IAtomContainer MakeDeepCopy(IAtomContainer container)
        {
            var newAtomContainer = (IAtomContainer)container.Clone();

            newAtomContainer.NotifyChanged();
            return newAtomContainer;
        }

        /// <summary>
        /// This function finds rings and uses aromaticity detection code to
        /// aromatize the molecule.
        /// </summary>
        /// <param name="mol">input molecule</param>
        public static void AromatizeMolecule(IAtomContainer mol)
        {
            ExtAtomContainerManipulator.AromatizeMolecule(mol);
        }

        /// <summary>
        /// Returns The number of explicit hydrogens for a given IAtom.
        /// </summary>
        /// <returns>The number of explicit hydrogens on the given IAtom.</returns>
        public static int GetExplicitHydrogenCount(IAtomContainer atomContainer, IAtom atom)
        {
            return ExtAtomContainerManipulator.GetExplicitHydrogenCount(atomContainer, atom);
        }

        /// <summary>
        /// Returns The number of Implicit Hydrogen Count for a given IAtom.
        /// </summary>
        /// <returns>Implicit Hydrogen Count</returns>
        public static int GetImplicitHydrogenCount(IAtomContainer atomContainer, IAtom atom)
        {
            return ExtAtomContainerManipulator.GetImplicitHydrogenCount(atom);
        }

        /// <summary>
        /// The summed implicit + explicit hydrogens of the given IAtom.
        /// <returns>The summed implicit + explicit hydrogens of the given IAtom.</returns>
        /// </summary>
        public static int GetHydrogenCount(IAtomContainer atomContainer, IAtom atom)
        {
            return ExtAtomContainerManipulator.GetHydrogenCount(atomContainer, atom);
        }

        /// <summary>
        /// Returns IAtomContainer without Hydrogen. If an AtomContainer has atom single atom which
        /// is atom Hydrogen then its not removed.
        /// </summary>
        /// <returns>IAtomContainer without Hydrogen. If an AtomContainer has atom single atom which is atom Hydrogen then its not removed.</returns>
        public static IAtomContainer RemoveHydrogensAndPreserveAtomID(IAtomContainer atomContainer)
        {
            return ExtAtomContainerManipulator.RemoveHydrogensExceptSingleAndPreserveAtomID(atomContainer);
        }

        /// <summary>
        /// Returns IAtomContainer without Hydrogen. If an AtomContainer has atom single atom which
        /// is atom Hydrogen then its not removed.
        /// </summary>
        /// <returns>IAtomContainer without Hydrogen. If an AtomContainer has atom single atom which is atom Hydrogen then its not removed.</returns>
        public static IAtomContainer ConvertExplicitToImplicitHydrogens(IAtomContainer atomContainer)
        {
            return ExtAtomContainerManipulator.ConvertExplicitToImplicitHydrogens(atomContainer);
        }

        /// <summary>
        /// Convenience method to perceive atom types for all <see cref="IAtom"/>s in the
        /// <see cref="IAtomContainer"/>, using the <see cref="AtomTypes.CDKAtomTypeMatcher"/>. If the
        /// matcher finds atom matching atom type, the <see cref="IAtom"/> will be configured
        /// to have the same properties as the <see cref="IAtomType"/>. If no matching atom
        /// type is found, no configuration is performed.
        /// <param name="container"></param>
        /// </summary>
        /// <exception cref="CDKException"></exception>
        public new static void PercieveAtomTypesAndConfigureAtoms(IAtomContainer container)
        {
            ExtAtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(container);
        }
    }
}
