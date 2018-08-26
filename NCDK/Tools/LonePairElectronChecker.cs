/* Copyright (C) 2006-2007  Miguel Rojas <miguel.rojas@uni-koeln.de>
 *                    2007  Egon Willighagen <egonw@users.sf.net>
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

using NCDK.Config;
using System.Diagnostics;
using System.Linq;

namespace NCDK.Tools
{
    /// <summary>
    /// Provides methods for checking whether an atoms lone pair electrons are saturated
    /// with respect to a particular atom type.
    /// </summary>
    // @author         Miguel Rojas
    // @cdk.githash
    // @cdk.created    2006-04-01
    // @cdk.keyword    saturation
    // @cdk.keyword    atom, valency
    // @cdk.module     standard
    public static class LonePairElectronChecker
    {
        private static AtomTypeFactory factory = AtomTypeFactory.GetInstance("NCDK.Dict.Data.cdk-atom-types.owl", Silent.ChemObjectBuilder.Instance);

        /// <summary>
        /// Determines of all atoms on the AtomContainer have the
        /// right number the lone pair electrons.
        /// </summary>
        public static bool IsSaturated(IAtomContainer container)
        {
            return AllSaturated(container);
        }

        /// <summary>
        /// Determines of all atoms on the AtomContainer have
        /// the right number the lone pair electrons.
        /// </summary>
        public static bool AllSaturated(IAtomContainer ac)
        {
            Debug.WriteLine("Are all atoms saturated?");
            for (int f = 0; f < ac.Atoms.Count; f++)
            {
                if (!IsSaturated(ac.Atoms[f], ac))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if an Atom is saturated their lone pair electrons
        /// by comparing it with known AtomTypes.
        /// </summary>
        /// <returns>True, if it's right saturated</returns>
        public static bool IsSaturated(IAtom atom, IAtomContainer ac)
        {
            IAtomType atomType = factory.GetAtomType(atom.AtomTypeName);
            int lpCount = atomType.GetProperty<int>(CDKPropertyName.LonePairCount);
            int foundLPCount = ac.GetConnectedLonePairs(atom).Count();
            return foundLPCount >= lpCount;
        }

        /// <summary>
        /// Saturates a molecule by setting appropriate number lone pair electrons.
        /// </summary>
        public static void Saturate(IAtomContainer atomContainer)
        {
            Trace.TraceInformation("Saturating atomContainer by adjusting lone pair electrons...");
            bool allSaturated = AllSaturated(atomContainer);
            if (!allSaturated)
            {
                for (int i = 0; i < atomContainer.Atoms.Count; i++)
                {
                    Saturate(atomContainer.Atoms[i], atomContainer);
                }
            }
        }

        /// <summary>
        /// Saturates an IAtom by adding the appropriate number lone pairs.
        /// </summary>
        public static void Saturate(IAtom atom, IAtomContainer ac)
        {
            Trace.TraceInformation("Saturating atom by adjusting lone pair electrons...");
            IAtomType atomType = factory.GetAtomType(atom.AtomTypeName);
            int lpCount = atomType.GetProperty<int>(CDKPropertyName.LonePairCount);
            int missingLPs = lpCount - ac.GetConnectedLonePairs(atom).Count();

            for (int j = 0; j < missingLPs; j++)
            {
                ILonePair lp = atom.Builder.NewLonePair(atom);
                ac.LonePairs.Add(lp);
            }
        }
    }
}
