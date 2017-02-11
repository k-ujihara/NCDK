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
     * Encapsulates properites of InChI Stereo Parity.  See <tt>inchi_api.h</tt>.
     * @author Sam Adams
     */
    public class NInchiStereo0D
    {
        /**
         * Indicates non-existent (central) atom. Value from inchi_api.h.
         */
        public const int NO_ATOM = -1;

        /**
         * Neighbouring atoms.
         */
        public NInchiAtom[] Neighbors { get; private set; } = new NInchiAtom[4];

        /**
         * Central atom.
         */
        public NInchiAtom CentralAtom { get; private set; }

        /**
         * Stereo parity type.
         */
        public INCHI_STEREOTYPE StereoType { get; private set; }

        /**
         * Parity.
         */
        public INCHI_PARITY Parity {
            get;
#if !DEBUG
            private 
#endif
            set; }

        /**
         * Second parity (for disconnected systems).
         */
        public INCHI_PARITY DisconnectedParity {
            get;
#if !TEST
            private 
#endif
            set;
        } = INCHI_PARITY.None;

        /**
         * Constructor.  See <tt>inchi_api.h</tt> for details of usage.
         *
         * @see CreateNewTetrahedralStereo0D()
         * @see CreateNewDoublebondStereo0D()
         *
         * @param atC    Central atom
         * @param at0    Neighbour atom 0
         * @param at1    Neighbour atom 1
         * @param at2    Neighbour atom 2
         * @param at3    Neighbour atom 3
         * @param type          Stereo parity type
         * @param parity    Parity
         */
        public NInchiStereo0D(NInchiAtom atC, NInchiAtom at0,
                 NInchiAtom at1, NInchiAtom at2, NInchiAtom at3,
                 INCHI_STEREOTYPE type, INCHI_PARITY parity)
        {
            CentralAtom = atC;
            Neighbors[0] = at0;
            Neighbors[1] = at1;
            Neighbors[2] = at2;
            Neighbors[3] = at3;

            this.StereoType = type;
            this.Parity = parity;
        }

        NInchiStereo0D(NInchiAtom atC, NInchiAtom at0,
                 NInchiAtom at1, NInchiAtom at2, NInchiAtom at3,
                 int type, int parity)
            : this(atC, at0, at1, at2, at3, (INCHI_STEREOTYPE)type, (INCHI_PARITY)parity)
        { }

        /**
         * Generates string representation of information on stereo parity,
         * for debugging purposes.
         */
        public string ToDebugString()
        {
            return ("InChI Stereo0D: "
                + (CentralAtom == null ? "-" : CentralAtom.ElementType)
                + " [" + Neighbors[0].ElementType + "," + Neighbors[1].ElementType
                + "," + Neighbors[2].ElementType + "," + Neighbors[3].ElementType + "] "
                + "Type::" + StereoType + " // "
                + "Parity:" + Parity
                );
        }

        /**
         * Outputs information on stereo parity, for debugging purposes.
         */
        public void PrintDebug()
        {
            Console.Out.WriteLine(ToDebugString());
        }

        /**
         * <p>Convenience method for generating 0D stereo parities at tetrahedral
         * atom centres.
         *
         * <p><b>Usage notes from <i>inchi_api.h</i>:</b>
         * <pre>
         *  4 neighbors
         *
         *           X                    neighbor[4] : {#W, #X, #Y, #Z}
         *           |                    central_atom: #A
         *        W--A--Y                 type        : INCHI_StereoType_Tetrahedral
         *           |
         *           Z
         *  parity: if (X,Y,Z) are clockwize when seen from W then parity is 'e' otherwise 'o'
         *  Example (see AXYZW above): if W is above the plane XYZ then parity = 'e'
         *
         *  3 neighbors
         *
         *             Y          Y       neighbor[4] : {#A, #X, #Y, #Z}
         *            /          /        central_atom: #A
         *        X--A  (e.g. O=S   )     type        : INCHI_StereoType_Tetrahedral
         *            \          \
         *             Z          Z
         *
         *  parity: if (X,Y,Z) are clockwize when seen from A then parity is 'e',
         *                                                         otherwise 'o'
         *  unknown parity = 'u'
         *  Example (see AXYZ above): if A is above the plane XYZ then parity = 'e'
         *  This approach may be used also in case of an implicit H attached to A.
         *
         *  ==============================================
         *  Note. Correspondence to CML 0D stereo parities
         *  ==============================================
         *  a list of 4 atoms corresponds to CML atomRefs4
         *
         *  tetrahedral atom
         *  ================
         *  CML atomParity > 0 <=> INCHI_PARITY_EVEN
         *  CML atomParity < 0 <=> INCHI_PARITY_ODD
         *
         *                               | 1   1   1   1  |  where xW is x-coordinate of
         *                               | xW  xX  xY  xZ |  atom W, etc. (xyz is a
         *  CML atomParity = determinant | yW  yX  yY  yZ |  'right-handed' Cartesian
         *                               | zW  zX  xY  zZ |  coordinate system)
         * </pre>
         *
         * @param atC    Central atom
         * @param at0    Neighbour atom 0
         * @param at1    Neighbour atom 1
         * @param at2    Neighbour atom 2
         * @param at3    Neighbour atom 3
         * @param parity Parity
         */
        public static NInchiStereo0D CreateNewTetrahedralStereo0D(NInchiAtom atC, NInchiAtom at0,
                 NInchiAtom at1, NInchiAtom at2, NInchiAtom at3,
                INCHI_PARITY parity)
        {
            NInchiStereo0D stereo = new NInchiStereo0D(atC, at0, at1, at2, at3, INCHI_STEREOTYPE.Tetrahedral, parity);
            return stereo;
        }

        /**
         * <p>Convenience method for generating 0D stereo parities at stereogenic
         * double bonds.
         *
         * <p><b>Usage notes from <i>inchi_api.h</i>:</b>
         * <pre>
         *  =============================================
         *  stereogenic bond >A=B< or cumulene >A=C=C=B<
         *  =============================================
         *
         *                              neighbor[4]  : {#X,#A,#B,#Y} in this order
         *  X                           central_atom : NO_ATOM
         *   \            X      Y      type         : INCHI_StereoType_DoubleBond
         *    A==B         \    /
         *        \         A==B
         *         Y
         *
         *  parity= 'e'    parity= 'o'   unknown parity = 'u'
         *
         *  ==============================================
         *  Note. Correspondence to CML 0D stereo parities
         *  ==============================================
         *
         *  stereogenic double bond and (not yet defined in CML) cumulenes
         *  ==============================================================
         *  CML 'C' (cis)      <=> INCHI_PARITY_ODD
         *  CML 'T' (trans)    <=> INCHI_PARITY_EVEN
         * </pre>
         *
         * @param at0    Neighbour atom 0
         * @param at1    Neighbour atom 1
         * @param at2    Neighbour atom 2
         * @param at3    Neighbour atom 3
         * @param parity Parity
         * @return
         */
        public static NInchiStereo0D CreateNewDoublebondStereo0D(NInchiAtom at0,
                 NInchiAtom at1, NInchiAtom at2, NInchiAtom at3,
                 INCHI_PARITY parity)
        {
            NInchiStereo0D stereo = new NInchiStereo0D(null, at0, at1, at2, at3, INCHI_STEREOTYPE.DoubleBond, parity);
            return stereo;
        }
    }
}
