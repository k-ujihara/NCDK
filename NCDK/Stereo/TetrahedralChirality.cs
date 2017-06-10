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
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using System.Linq;
using System.Collections.Generic;
using System.Text;
using System;

namespace NCDK.Stereo
{
    /// <summary>
    /// Stereochemistry specification for quadrivalent atoms. See <see cref="ITetrahedralChirality"/> for
    /// further details.
    /// </summary>
    /// <seealso cref="ITetrahedralChirality"/>
    // @cdk.module core
    // @cdk.githash
    public class TetrahedralChirality
        : ITetrahedralChirality
    {
        private IAtom chiralAtom;
        private IList<IAtom> ligandAtoms;
        private TetrahedralStereo stereo;
        public IChemObjectBuilder Builder { get; set; }

        /// <summary>
        /// Constructor to create a new <see cref="ITetrahedralChirality"/> implementation instance.
        /// </summary>
        /// <param name="chiralAtom">The chiral <see cref="IAtom"/>.</param>
        /// <param name="ligandAtoms">The ligand atoms around the chiral atom.</param>
        /// <param name="chirality">The <see cref="Stereo"/> chirality.</param>
        public TetrahedralChirality(IAtom chiralAtom, IEnumerable<IAtom> ligandAtoms, TetrahedralStereo chirality)
        {
            this.chiralAtom = chiralAtom;
            this.ligandAtoms = new List<IAtom>(ligandAtoms);
            this.stereo = chirality;
        }

        /// <summary>
        /// An array of ligand atoms around the chiral atom.
        /// </summary>
        public virtual IList<IAtom> Ligands => ligandAtoms;

        /// <summary>
        /// Atom that is the chirality center.
        /// </summary>
        public virtual IAtom ChiralAtom => chiralAtom;

        /// <summary>
        /// Defines the stereochemistry around the chiral atom. The value depends on the order of ligand atoms.
        /// </summary>
        public virtual TetrahedralStereo Stereo
        {
            get { return stereo; }
            set { stereo = value; }
        }

        public static object TetrahedralStereo { get; set; }

        public virtual bool Contains(IAtom atom)
        {
            if (chiralAtom == atom)
                return true;
            return ligandAtoms.Any(ligand => ligand == atom);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Tetrahedral{").Append(GetHashCode()).Append(", ");
            sb.Append(this.Stereo).Append(", ");
            sb.Append("c:").Append(this.ChiralAtom).Append(", ");
            var ligands = this.Ligands;
            for (int i = 0; i < ligands.Count; i++)
                sb.Append(i + 1).Append(':').Append(ligands[i]).Append(", ");
            sb.Append('}');
            return sb.ToString();
        }

        public virtual object Clone()
        {
            return Clone(new CDKObjectMap());
        }

        public ICDKObject Clone(CDKObjectMap map)
        {
            if (map == null)
                throw new ArgumentNullException(nameof(map));

            var clone = (TetrahedralChirality)MemberwiseClone();

            // convert the chiral atom and it's ligands to their equivalent
            IAtom chiral = chiralAtom;
            if (chiral != null && map.AtomMap.ContainsKey(chiralAtom))
                chiral = map.AtomMap[chiralAtom];
            clone.chiralAtom = chiral;
            clone.ligandAtoms = new List<IAtom>();
            foreach (var ligand in ligandAtoms)
            {
                IAtom atom;
                if (ligand == null || !map.AtomMap.TryGetValue(ligand, out atom))
                    atom = ligand;
                clone.ligandAtoms.Add(atom);
            }
            return clone;
        }
    }
}
