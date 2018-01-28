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
        : AbstractStereo<IBond, IBond>, IDoubleBondStereochemistry
    {
        /// <summary>
        /// Creates a new double bond stereo chemistry. The path of length three is defined by
        /// <paramref name="ligandBonds"/>[0], <paramref name="stereoBond"/>, and <paramref name="ligandBonds"/>[1].
        /// </summary>
        public DoubleBondStereochemistry(IBond stereoBond, IEnumerable<IBond> ligandBonds, DoubleBondConformation stereo) 
            : this(stereoBond, ligandBonds, stereo.ToConfiguration())
        {
        }

        public DoubleBondStereochemistry(IBond stereoBond, IEnumerable<IBond> ligandBonds, StereoElement.Configuration configure)
            : base(stereoBond, ligandBonds.ToList(), new StereoElement(StereoElement.Classes.CisTrans, configure))
        {
        }

        public DoubleBondStereochemistry(IBond stereoBond, IEnumerable<IBond> ligandBonds, StereoElement stereo)
            : this(stereoBond, ligandBonds, stereo.Configure)
        {
        }

        public virtual IReadOnlyList<IBond> Bonds => new ReadOnlyCollection<IBond>(new List<IBond>(Carriers));
        public virtual IBond StereoBond => Focus;
        public virtual DoubleBondConformation Stereo => Configure.ToConformation();

        protected override IStereoElement<IBond, IBond> Create(IBond focus, IList<IBond> carriers, StereoElement stereo)
        {
            return new DoubleBondStereochemistry(focus, carriers, stereo);
        }
    }
}
