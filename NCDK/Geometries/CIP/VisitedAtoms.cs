/* Copyright (C) 2010  Egon Willighagen <egonw@users.sf.net>
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
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using System.Collections.Generic;

namespace NCDK.Geometries.CIP
{
    /**
     * Helper class for the {@link CIPTool} to keep track of which atoms have
     * already been visited.
     *
     * @cdk.module cip
     * @cdk.githash
     */
    public class VisitedAtoms
    {
        /**
         * {@link List} to hold the visited {@link IAtom}s.
         */
        private List<IAtom> visitedItems;

        /**
         * Creates a new empty list of visited {@link IAtom}s.
         */
        public VisitedAtoms()
        {
            visitedItems = new List<IAtom>();
        }

        /**
         * Returns true if the given atom already has been visited.
         *
         * @param  atom {@link IAtom} which may have been visited
         * @return      true if the {@link IAtom} was visited
         */
        public bool IsVisited(IAtom atom)
        {
            return visitedItems.Contains(atom);
        }

        /**
         * Marks the given atom as visited.
         *
         * @param atom {@link IAtom} that is now marked as visited
         */
        public void Visited(IAtom atom)
        {
            visitedItems.Add(atom);
        }

        /**
         * Adds all atoms from the <code>visitedAtoms</code> list to the current
         * list.
         *
         * @param visitedAtoms the {@link VisitedAtoms} from which all atoms are
         *                     added
         */
        public void Visited(VisitedAtoms visitedAtoms)
        {
            visitedItems.AddRange(visitedAtoms.visitedItems);
        }
    }
}
