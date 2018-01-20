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

namespace NCDK.Geometries.CIP
{
    /// <summary>
    /// Concept of a ligand in CIP terms, reflecting a side chain of a central atom that can
    /// have precedence over another.
    /// </summary>
    // @cdk.module cip
    // @cdk.githash
    public class Ligand : ILigand
    {
        private IAtomContainer container;
        private IAtom centralAtom;
        private IAtom ligandAtom;
        private VisitedAtoms visitedAtoms;

        public Ligand(IAtomContainer container, VisitedAtoms visitedAtoms, IAtom centralAtom, IAtom ligandAtom)
        {
            this.container = container;
            this.centralAtom = centralAtom;
            this.ligandAtom = ligandAtom;
            this.visitedAtoms = new VisitedAtoms();
            this.visitedAtoms.Visited(visitedAtoms);
            this.visitedAtoms.Visited(centralAtom);
        }

        /// <summary>
        /// <see cref="IAtomContainer"/> of which this ligand is part.
        /// </summary>
        /// <returns>the IAtomContainer</returns>
        public IAtomContainer GetAtomContainer()
        {
            return container;
        }

        /// <summary>
        /// The central <see cref="IAtom"/> to which this ligand is connected via one <see cref="IBond"/>.
        /// </summary>
        /// <returns>the central atom</returns>
        public IAtom GetCentralAtom()
        {
            return centralAtom;
        }

        /// <summary>
        /// <see cref="IAtom"/> of the ligand that is connected to the chiral <see cref="IAtom"/> via
        /// one <see cref="IBond"/>.
        /// </summary>
        /// <returns>the ligand atom</returns>
        public IAtom GetLigandAtom()
        {
            return ligandAtom;
        }

        public VisitedAtoms GetVisitedAtoms()
        {
            return visitedAtoms;
        }

        public bool IsVisited(IAtom atom)
        {
            return visitedAtoms.IsVisited(atom);
        }
    }
}
