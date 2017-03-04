/*
 * Copyright (c) 2014 European Bioinformatics Institute (EMBL-EBI)
 *                    John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 2.1 of the License, or (at
 * your option) any later version. All we ask is that proper credit is given
 * for our work, which includes - but is not limited to - adding the above
 * copyright notice to the beginning of your source code files, and to any
 * copyright notice that you may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using NCDK.Common.Mathematics;
using NCDK.Numerics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NCDK.Renderers
{
    /// <summary>
    /// Predicate that defines whether an atom symbol is displayed in a structure diagram.
    /// </summary>
    /// <example>
    /// <code>
    /// SymbolVisibility visibility = SymbolVisibility.IUPACRecommendations;
    /// </code>
    /// </example>
    // @author John May
    public abstract class SymbolVisibility
    {
        /// <summary>
        /// Determine if an atom with the specified bonds is visible.
        /// </summary>
        /// <param name="atom">an atom</param>
        /// <param name="neighbors">neighboring bonds</param>
        /// <returns>whether the atom symbol is visible</returns>
        public abstract bool Visible(IAtom atom, IEnumerable<IBond> neighbors, RendererModel model);

        /// <summary>
        /// All atom symbols are visible.
        /// </summary>
        /// <returns>visibility that displays all symbols</returns>
        public static SymbolVisibility All { get; } = new SymbolVisibility_All();

        class SymbolVisibility_All : SymbolVisibility
        {
            public override bool Visible(IAtom atom, IEnumerable<IBond> neighbors, RendererModel model) => true;
        }

        /// <summary>
        /// Displays a symbol based on the preferred representation from the IUPAC guidelines (GR-2.1.2)
        /// {@cdk.cite Brecher08}. Carbons are unlabeled unless they have abnormal valence, parallel
        /// bonds, or are terminal (i.e. methyl, methylene, etc).
        /// </summary>
        /// <returns>symbol visibility instance</returns>
        public static SymbolVisibility IUPACRecommendations { get; } = new IupacVisibility(true);

        /// <summary>
        /// Displays a symbol based on the acceptable representation from the IUPAC guidelines (GR-2.1.2)
        /// {@cdk.cite Brecher08}. Carbons are unlabeled unless they have abnormal valence, parallel
        /// bonds. The recommendations note that it is acceptable to leave methyl groups unlabelled.
        /// </summary>
        /// <returns>symbol visibility instance</returns>
        public static SymbolVisibility IUPACRecommendationsWithoutTerminalCarbon { get; } = new IupacVisibility(false);

        /// <summary>
        /// Visibility following IUPAC guidelines.
        /// </summary>
        private sealed class IupacVisibility : SymbolVisibility
        {
            private bool terminal = false;

            internal IupacVisibility(bool terminal)
            {
                this.terminal = terminal;
            }

            /// <inheritdoc/>
            public override bool Visible(IAtom atom, IEnumerable<IBond> bonds_, RendererModel model)
            {
                var bonds = bonds_.ToList();
                var element = Config.Elements.OfNumber(atom.AtomicNumber.Value);

                // all non-carbons are displayed
                if (element != Config.Elements.Carbon) return true;

                // methane
                if (bonds.Count == 0) return true;

                // methyl (optional)
                if (bonds.Count == 1 && terminal) return true;

                // abnormal valence, could be due to charge or unpaired electrons
                if (!IsFourValent(atom, bonds)) return true;

                // carbon isotopes are displayed
                var mass = atom.MassNumber;
                if (mass != null && !IsMajorIsotope(element.AtomicNumber, mass.Value)) return true;

                // no kink between bonds to imply the presence of a carbon and it must
                // be displayed
                if (HasParallelBonds(atom, bonds))
                {
                    // TODO only when both bonds are single?
                    return true;
                }

                // special case ethane
                if (bonds.Count == 1)
                {
                    var begHcnt = atom.ImplicitHydrogenCount;
                    IAtom end = bonds[0].GetConnectedAtom(atom);
                    var endHcnt = end.ImplicitHydrogenCount;
                    if (begHcnt != null && endHcnt != null && begHcnt == 3 && endHcnt == 3)
                        return true;
                }

                // ProblemMarker ?

                return false;
            }

            /// <summary>
            /// Determine if the specified mass is the major isotope for the given atomic number.
            ///
            /// <param name="number">atomic number</param>
            /// <param name="mass">atomic mass</param>
            /// <returns>the mass is the major mass for the atomic number</returns>
            /// </summary>
            private static bool IsMajorIsotope(int number, int mass)
            {
                try
                {
                    IIsotope isotope = Config.Isotopes.Instance.GetMajorIsotope(number);
                    return isotope != null && isotope.MassNumber.Equals(mass);
                }
                catch (IOException)
                {
                    return false;
                }
            }

            /// <summary>
            /// Check the valency of the atom.
            /// </summary>
            /// <param name="atom">an atom</param>
            /// <param name="bonds">bonds connected to the atom</param>
            /// <returns>whether the atom is four valent</returns>
            private static bool IsFourValent(IAtom atom, List<IBond> bonds)
            {
                var valence = atom.ImplicitHydrogenCount;
                if (valence == null) return true;
                foreach (var bond in bonds)
                {
                    valence += bond.Order.Numeric;
                }
                return valence == 4;
            }

            /// <summary>
            /// Check whether the atom has only two bonds connected and they are (or close to) parallel.
            /// </summary>
            /// <param name="atom">an atom</param>
            /// <param name="bonds">bonds connected to the atom</param>
            /// <returns>whether the atom has parallele bonds</returns>
            private static bool HasParallelBonds(IAtom atom, IList<IBond> bonds)
            {
                if (bonds.Count != 2) return false;
                var thetaInRad = GetAngle(atom, bonds[0], bonds[1]);
                var thetaInDeg = Common.Mathematics.Vectors.RadianToDegree(thetaInRad);
                var delta = Math.Abs(thetaInDeg - 180);
                return delta < 8;
            }

            /// <summary>
            /// Determine the angle between two bonds of one atom.
            /// </summary>
            /// <param name="atom">an atom</param>
            /// <param name="bond1">a bond connected to the atom</param>
            /// <param name="bond2">another bond connected to the atom</param>
            /// <returns>the angle (in radians)</returns>
            private static double GetAngle(IAtom atom, IBond bond1, IBond bond2)
            {
                var pA = atom.Point2D.Value;
                var pB = bond1.GetConnectedAtom(atom).Point2D.Value;
                var pC = bond2.GetConnectedAtom(atom).Point2D.Value;
                var u = new Vector2(pB.X - pA.X, pB.Y - pA.Y);
                var v = new Vector2(pC.X - pA.X, pC.Y - pA.Y);
                return Vectors.Angle(u, v);
            }
        }
    }
}
