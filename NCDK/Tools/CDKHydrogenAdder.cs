/* Copyright (C) 2007  Egon Willighagen
 *               2009  Mark Rijnbeek <markr@ebi.ac.uk>
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
using System;
using System.Collections.Generic;
using NCDK.Config;
using System.Linq;

namespace NCDK.Tools
{
    /// <summary>
    /// Adds implicit hydrogens based on atom type definitions. The class assumes
    /// that CDK atom types are already detected. 
    /// </summary>
    /// <example>
    /// A full code example is:
    /// <code>
    ///   IAtomContainer methane = new AtomContainer();
    ///   IAtom carbon = new Atom("C");
    ///   methane.Add(carbon);
    ///   CDKAtomTypeMatcher matcher = CDKAtomTypeMatcher.GetInstance(methane.GetNewBuilder());
    ///   foreach (var atom in methane.atoms) {
    ///     IAtomType type = matcher.FindMatchingAtomType(methane, atom);
    ///     AtomTypeManipulator.Configure(atom, type);
    ///   }
    ///   CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(methane.GetNewBuilder());
    ///   adder.AddImplicitHydrogens(methane);
    /// </code>
    ///
    /// If you want to add the hydrogens to a specific atom only,
    /// use this example:
    /// <code>
    ///   IAtomContainer ethane = new AtomContainer();
    ///   IAtom carbon1 = new Atom("C");
    ///   IAtom carbon2 = new Atom("C");
    ///   ethane.Add(carbon1);
    ///   ethane.Add(carbon2);
    ///   CDKAtomTypeMatcher matcher = CDKAtomTypeMatcher.GetInstance(ethane.GetNewBuilder());
    ///   IAtomType type = matcher.FindMatchingAtomType(ethane, carbon1);
    ///   AtomTypeManipulator.Configure(carbon1, type);
    ///   CDKHydrogenAdder adder = CDKHydrogenAdder.GetInstance(ethane.GetNewBuilder());
    ///   adder.AddImplicitHydrogens(ethane, carbon1);
    /// </code>
    /// </example>
    // @author     egonw
    // @cdk.module valencycheck
    // @cdk.githash
    public class CDKHydrogenAdder
    {
        private AtomTypeFactory atomTypeList;
        private readonly string ATOM_TYPE_LIST = "NCDK.Dict.Data.cdk-atom-types.owl";

        private static IDictionary<Type, CDKHydrogenAdder> tables = new Dictionary<Type, CDKHydrogenAdder>();

        private CDKHydrogenAdder(IChemObjectBuilder builder)
        {
            if (atomTypeList == null) atomTypeList = AtomTypeFactory.GetInstance(ATOM_TYPE_LIST, builder);
        }

        public static CDKHydrogenAdder GetInstance(IChemObjectBuilder builder)
        {
            CDKHydrogenAdder adder;
            if (!tables.TryGetValue(builder.GetType(), out adder))
            {
                adder = new CDKHydrogenAdder(builder);
                tables.Add(builder.GetType(), adder);
            }
            return adder;
        }

        /// <summary>
        /// Sets implicit hydrogen counts for all atoms in the given IAtomContainer.
        /// </summary>
        /// <param name="container">The molecule to which H's will be added</param>
        /// <exception cref="CDKException">if insufficient information is present</exception>
        // @cdk.keyword hydrogens, adding
        public void AddImplicitHydrogens(IAtomContainer container)
        {
            foreach (var atom in container.Atoms)
            {
                if (!(atom is IPseudoAtom))
                {
                    AddImplicitHydrogens(container, atom);
                }
            }
        }

        /// <summary>
        /// Sets the implicit hydrogen count for the indicated IAtom in the given IAtomContainer.
        /// If the atom type is "X", then the atom is assigned zero implicit hydrogens.
        /// </summary>
        /// <param name="container">The molecule to which H's will be added</param>
        /// <param name="atom">IAtom to set the implicit hydrogen count for</param>
        /// <exception cref="CDKException">if insufficient information is present</exception>
        public void AddImplicitHydrogens(IAtomContainer container, IAtom atom)
        {
            if (atom.AtomTypeName == null) throw new CDKException("IAtom is not typed! " + atom.Symbol);

            if ("X".Equals(atom.AtomTypeName))
            {
                if (atom.ImplicitHydrogenCount == null) atom.ImplicitHydrogenCount = 0;
                return;
            }

            IAtomType type = atomTypeList.GetAtomType(atom.AtomTypeName);
            if (type == null)
                throw new CDKException("Atom type is not a recognized CDK atom type: " + atom.AtomTypeName);

            if (type.FormalNeighbourCount == null)
                throw new CDKException(
                        "Atom type is too general; cannot decide the number of implicit hydrogen to add for: "
                                + atom.AtomTypeName);

            // very simply counting: each missing explicit neighbor is a missing hydrogen
            atom.ImplicitHydrogenCount = type.FormalNeighbourCount - container.GetConnectedAtoms(atom).Count();
        }
    }
}
