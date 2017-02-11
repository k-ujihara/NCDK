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
     * Encapsulates properties of InChI Bond.  See <tt>inchi_api.h</tt>.
     * @author Sam Adams
     */
    public class NInchiBond
    {
        /**
         * Origin atom in bond.
         */
        public NInchiAtom OriginAtom { get; set; }

        /**
         * Target atom in bond.
         */
        public NInchiAtom TargetAtom { get; set; }

        /**
         * Bond type.
         */
        public INCHI_BOND_TYPE BondType { get; set; } = INCHI_BOND_TYPE.None;

        /**
         * Bond 2D stereo definition.
         */
        public INCHI_BOND_STEREO BondStereo { get; set; } = INCHI_BOND_STEREO.None;

        /**
         * Create bond.
         *
         * @param atO        Origin atom
         * @param atT        Target atom
         * @param type        Bond type
         * @param stereo    Bond 2D stereo definition
         */
        public NInchiBond(NInchiAtom atO, NInchiAtom atT,
                INCHI_BOND_TYPE type, INCHI_BOND_STEREO stereo)
        {
            this.OriginAtom = atO;
            this.TargetAtom = atT;
            this.BondType = type;
            this.BondStereo = stereo;
        }


        NInchiBond(NInchiAtom atO, NInchiAtom atT,
                int type, int stereo)

            : this(atO, atT, (INCHI_BOND_TYPE)type, (INCHI_BOND_STEREO)stereo)
        {
        }

        /**
         * Create bond.
         *
         * @param atO        Origin atom
         * @param atT        Target atom
         * @param type        Bond type
         */
        public NInchiBond(NInchiAtom atO, NInchiAtom atT,
                INCHI_BOND_TYPE type)

            : this(atO, atT, type, INCHI_BOND_STEREO.None)
        { }

        /**
         * Generates string representation of information on bond,
         * for debugging purposes.
         */
        public string ToDebugString()
        {
            return ("InChI Bond: "
            + OriginAtom.ElementType
            + "-" + TargetAtom.ElementType
            + " // Type: " + BondType
            + " // Stereo: " + BondStereo
            );
        }

        /**
         * Outputs information on bond, for debugging purposes.
         */
        public void PrintDebug()
        {
            Console.Out.WriteLine(ToDebugString());
        }
    }
}
