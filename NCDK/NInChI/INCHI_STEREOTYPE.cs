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

namespace NCDK.NInChI
{
    /**
     * Enumeration of InChI 0D parity types.
     * Corresponds to <tt>inchi_StereoType0D</tt> in <tt>inchi_api.h</tt>.
     * @author Sam Adams
     */
    public enum INCHI_STEREOTYPE
    {
        /**
         * None.
         */
        None = 0,

        /**
         * Stereogenic bond >A=B< or cumulene >A=C=C=B<.
         */
        DoubleBond = 1,

        /**
         * Tetrahedral atom.
         */
        Tetrahedral = 2,

        /**
         * Allene.
         */
        Allene = 3,
    }
}
