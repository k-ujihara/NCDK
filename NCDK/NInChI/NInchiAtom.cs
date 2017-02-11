/*
 * Copyright 2006-2011 Sam Adams <sea36 at users.sourceforge.net>
 *
 * This file is part of JNI-InChI.
 *
 * JNI-InChI is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * JNI-InChI is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with JNI-InChI.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;

namespace NCDK.NInChI
{
    /**
     * Encapsulates properties of InChI Atom.  See <tt>inchi_api.h</tt>.
     * @author Sam Adams
     */
    public class NInchiAtom
    {
        /**
         * Indicates relative rather than absolute isotopic mass. Value
         * from inchi_api.h.
         */
#if TEST
        public
#else
        protected internal 
#endif
        const int ISOTOPIC_SHIFT_FLAG = 10000;

        /**
         * Atom x-coordinate.
         */
        public double X { get; private set; }

        /**
         * Atom y-coordinate.
         */
        public double Y { get; private set; }

        /**
         * Atom z-coordinate.
         */
        public double Z { get; private set; }

        /**
         * Chemical element symbol eg C, O, Fe, Hg.
         */
        public string ElementType { get; private set; }

        /**
         * Number of implicit hydrogens on atom. If set to -1, InChI will add
         * implicit H automatically.
         */
        public int ImplicitH { get; set; } = -1;

        /**
         * Number of implicit protiums (isotopic 1-H) on atom.
         */
        public int ImplicitProtium { get; set; } = 0;

        /**
         * Number of implicit deuteriums (isotopic 2-H) on atom.
         */
        public int ImplicitDeuterium { get; set; } = 0;

        /**
         * Number of implicit tritiums (isotopic 3-H) on atom.
         */
        public int ImplicitTritium { get; set; } = 0;

        /**
         * Mass of isotope. If set to 0, no isotopic mass set; otherwise, isotopic
         * mass, or ISOTOPIC_SHIFT_FLAG + (mass - average atomic mass).
         */
        public int IsotopicMass { get; set; } = 0;

        /**
         * Radical status of atom.
         */
        public INCHI_RADICAL Radical { get; set; } = INCHI_RADICAL.None;

        /**
         * Charge on atom.
         */
        public int Charge { get; set; } = 0;

        /**
         * <p>Create new atom.
         *
         * <p>Coordinates and element symbol must be set (unknown
         * coordinates/dimensions should be set to zero).  All other
         * parameters are initialised to default values:
         * <p>
         * <tt>
         *    Num Implicit H = 0<br>
         *    Num Implicit 1H = 0<br>
         *    Num Implicit 2H = 0<br>
         *    Num Implicit 3H = 0<br>
         *    Isotopic mass = 0 (non isotopic)<br>
         *    Radical status = None  (radical status not defined)
         * </tt>
         *
         * @param x     x-coordinate
         * @param y     y-coordinate
         * @param z     z-coordinate
         * @param el    Chemical element symbol
         * @ - if the element symbol is null.
         */
        public NInchiAtom(double x, double y, double z, string el)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;

            if (el == null)
            {
                throw new ArgumentNullException(nameof(el), "Chemical element must not be null");
            }
            this.ElementType = el;
        }


        /**
         * Convenience method to create a new atom with zero coordinates.
         * @param el
         */
        public NInchiAtom(string el)
            : this(0.0, 0.0, 0.0, el)
        { }

        /**
         * Sets isotopic mass, relative to standard mass.
         *
         * @param shift  Isotopic mass minus average atomic mass
         */
        public void SetIsotopicMassShift(int shift)
        {
            this.IsotopicMass = ISOTOPIC_SHIFT_FLAG + shift;
        }

        /**
         * Generates string representation of information on atom,
         * for debugging purposes.
         */
        public string ToDebugString()
        {
            return "InChI Atom: "
                + ElementType
                + " [" + ToString(X) + "," + ToString(Y) + "," + ToString(Z) + "] "
                + "Charge:" + Charge + " // "
                + "Iso Mass:" + IsotopicMass + " // "
                + "Implicit H:" + ImplicitH
                + " P:" + ImplicitProtium
                + " D:" + ImplicitDeuterium
                + " T:" + ImplicitTritium
                + " // Radical: " + Radical;
        }

        /// <summary>
        /// Java compatible <see cref="string.ToString()"/>
        /// </summary>
        private static string ToString(double x)
        {
            var s = x.ToString();
            return s.Contains(".") ? s : s + ".0";
        }

        /**
         * Outputs information on atom, for debugging purposes.
         */
        public void PrintDebug()
        {
            Console.Out.WriteLine(ToDebugString());
        }
    }
}
