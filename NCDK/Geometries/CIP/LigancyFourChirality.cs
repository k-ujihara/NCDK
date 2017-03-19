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
using System;

namespace NCDK.Geometries.CIP
{
    /// <summary>
    /// Stereochemistry specification for quadrivalent atoms to be used for the CIP algorithm only.
    /// </summary>
    /// <remarks>
    /// The data model defines the central, chiral <see cref="IAtom"/>,
    /// and its four <see cref="ILigand"/>s, each of which has an ligand <see cref="IAtom"/>, directly bonded to the chiral atom via
    /// an <see cref="IBond"/>. The ordering of the four ligands is important, and defines together with the <see cref="Stereo"/>
    /// to spatial geometry around the chiral atom. The first ligand points towards to observer, and the three other
    /// ligands point away from the observer; the <see cref="Stereo"/> then defines the order of the second, third, and
    /// fourth ligand to be clockwise or anti-clockwise.
    /// </remarks>
    // @cdk.module cip
    // @cdk.githash
    public class LigancyFourChirality
    {
        private IAtom chiralAtom;
        private ILigand[] ligands;
        private TetrahedralStereo stereo;

        /// <summary>
        /// Creates a new data model for chirality for the CIP rules.
        /// </summary>
        /// <param name="chiralAtom">The <see cref="IAtom"/> that is actually chiral.</param>
        /// <param name="ligands">An array with exactly four <see cref="ILigand"/>s.</param>
        /// <param name="stereo">A indication of clockwise or anticlockwise orientation of the atoms.</param>
        /// <seealso cref="TetrahedralStereo"/>
        public LigancyFourChirality(IAtom chiralAtom, ILigand[] ligands, TetrahedralStereo stereo)
        {
            this.chiralAtom = chiralAtom;
            this.ligands = ligands;
            this.stereo = stereo;
        }

        /// <summary>
        /// Creates a new data model for chirality for the CIP rules based on a chirality definition
        /// in the CDK data model with <see cref="ITetrahedralChirality"/>.
        /// </summary>
        /// <param name="container"><see cref="IAtomContainer"/> to which the chiral atom belongs.</param>
        /// <param name="cdkChirality"><see cref="ITetrahedralChirality"/> object specifying the chirality.</param>
        public LigancyFourChirality(IAtomContainer container, ITetrahedralChirality cdkChirality)
        {
            this.chiralAtom = cdkChirality.ChiralAtom;
            var ligandAtoms = cdkChirality.Ligands;
            this.ligands = new ILigand[ligandAtoms.Count];
            VisitedAtoms visitedAtoms = new VisitedAtoms();
            for (int i = 0; i < ligandAtoms.Count; i++)
            {
                // ITetrahedralChirality stores a impl hydrogen as the central atom
                if (ligandAtoms[i] == chiralAtom)
                {
                    this.ligands[i] = new ImplicitHydrogenLigand(container, visitedAtoms, chiralAtom);
                }
                else
                {
                    this.ligands[i] = new Ligand(container, visitedAtoms, chiralAtom, ligandAtoms[i]);
                }
            }
            this.stereo = cdkChirality.Stereo;
        }

        /// <summary>
        /// Returns the four ligands for this chirality.
        /// </summary>
        /// <returns>An array of four <see cref="ILigand"/>s.</returns>
        public ILigand[] Ligands => ligands;

        /// <summary>
        /// Returns the chiral <see cref="IAtom"/> to which the four ligands are connected..
        /// </summary>
        /// <returns>The chiral <see cref="IAtom"/>.</returns>
        public IAtom ChiralAtom => chiralAtom;

        /// <summary>
        /// Returns the chirality value for this stereochemistry object.
        /// </summary>
        /// <returns>A <see cref="TetrahedralStereo"/> value.</returns>
        public TetrahedralStereo Stereo => stereo;

        /// <summary>
        /// Recalculates the <see cref="LigancyFourChirality"/> based on the new, given atom ordering.
        /// </summary>
        /// <param name="newOrder">new order of atoms</param>
        /// <returns>the chirality following the new atom order</returns>
        public LigancyFourChirality Project(ILigand[] newOrder)
        {
            TetrahedralStereo newStereo = this.stereo;
            // copy the current ordering, and work with that
            ILigand[] newAtoms = new ILigand[4];
            Array.Copy(this.ligands, 0, newAtoms, 0, 4);

            // now move atoms around to match the newOrder
            for (int i = 0; i < 3; i++)
            {
                if (newAtoms[i].GetLigandAtom() != newOrder[i].GetLigandAtom())
                {
                    // OK, not in the right position
                    // find the incorrect, old position
                    for (int j = i; j < 4; j++)
                    {
                        if (newAtoms[j].GetLigandAtom() == newOrder[i].GetLigandAtom())
                        {
                            // found the incorrect position
                            Swap(newAtoms, i, j);
                            // and swap the stereochemistry
                            if (newStereo == TetrahedralStereo.Clockwise)
                            {
                                newStereo = TetrahedralStereo.AntiClockwise;
                            }
                            else
                            {
                                newStereo = TetrahedralStereo.Clockwise;
                            }
                        }
                    }
                }
            }
            return new LigancyFourChirality(chiralAtom, newAtoms, newStereo);
        }

        private void Swap(ILigand[] ligands, int first, int second)
        {
            ILigand tmpLigand = ligands[first];
            ligands[first] = ligands[second];
            ligands[second] = tmpLigand;
        }
    }
}
