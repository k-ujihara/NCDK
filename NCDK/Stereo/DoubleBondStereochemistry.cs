/* Copyright (C) 2012  Egon Willighagen <egonw@users.sf.net>
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
using System.Collections.ObjectModel;
using System.Linq;

namespace NCDK.Stereo
{
    /// <summary>
    /// Stereochemistry specification for double bonds. See <see cref="IDoubleBondStereochemistry"/> for
    /// further details.
    /// </summary>
    /// <seealso cref="IDoubleBondStereochemistry"/>
    // @cdk.module core
    // @cdk.githash
    public class DoubleBondStereochemistry
        : IDoubleBondStereochemistry
    {
        private DoubleBondConformation stereo;
        private List<IBond> ligandBonds;
        private IBond stereoBond;
        public IChemObjectBuilder Builder { get; set; }

        /// <summary>
        /// Creates a new double bond stereo chemistry. The path of length three is defined by
        /// <c>ligandBonds[0]</c>, <c>stereoBonds</c>, and <c>ligandBonds[1]</c>.
        /// </summary>
        public DoubleBondStereochemistry(IBond stereoBond, IEnumerable<IBond> ligandBonds, DoubleBondConformation stereo)
        {
            if (ligandBonds.Count() > 2) throw new ArgumentException("expected two ligand bonds");
            this.stereoBond = stereoBond;
            this.ligandBonds = new List<IBond>(ligandBonds);
            this.stereo = stereo;
        }

        public virtual IReadOnlyList<IBond> Bonds => new ReadOnlyCollection<IBond>(ligandBonds);
        public virtual IBond StereoBond => stereoBond;
        public virtual DoubleBondConformation Stereo => stereo;

        public virtual bool Contains(IAtom atom)
        {
            return stereoBond.Contains(atom) || ligandBonds.Any(bond => bond.Contains(atom));
        }

        public virtual object Clone()
        {
            return Clone(new CDKObjectMap());
        }

        public virtual ICDKObject Clone(CDKObjectMap map)
        {
            var clone = (DoubleBondStereochemistry)base.MemberwiseClone();
            clone.stereo = stereo;
            clone.stereoBond = (IBond)stereoBond?.Clone(map);
            clone.ligandBonds = new List<IBond>();
            foreach (var bond in ligandBonds)
                clone.ligandBonds.Add((IBond)bond.Clone(map));
            return clone;
        }
    }
}
