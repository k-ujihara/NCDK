/* Copyright (C) 2006-2007  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
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
using System.Collections.Generic;

namespace NCDK
{
    /// <summary>
    /// The base class for all chemical objects in this cdk. It provides methods for
    /// adding listeners and for their notification of events, as well a a hash
    /// table for administration of physical or chemical properties
    /// </summary>
    // @author        egonw
    // @cdk.githash
    // @cdk.module    interfaces
    public partial interface IChemObject
        : ICDKObject
    {
        /// <summary>
        /// Deep comparator of <see cref="IChemObject"/>.  
        /// </summary>
        /// <param name="obj">Object to compare with.</param>
        /// <returns><see langword="true"/> if all properties of this object equals to <paramref name="obj"/>.</returns>
        bool Compare(object obj);

        /// <summary>
        /// <see cref="IChemObjectListener"/>s of this <see cref="IChemObject"/>.
        /// </summary>
        ICollection<IChemObjectListener> Listeners { get; }

        /// <summary>
        /// The flag that indicates whether notification messages are sent around.
        /// </summary>
        bool Notification { get; set; }

        /// <summary>
        /// This should be triggered by an method that changes the content of an object
        /// to that the registered listeners can react to it.
        /// </summary>
        void NotifyChanged();
        
        /// <summary>
        /// Identifier (ID) of this object.
        /// </summary>
        string Id { get; set; }

        /// <summary>Flag that is set if the <see cref="IChemObject"/> is placed (somewhere).</summary>
        bool IsPlaced { get; set; }

        /// <summary>Flag is set if <see cref="IChemObject"/> has been visited</summary>
        bool IsVisited { get; set; }
    }

    internal struct ChemObjectFlagBag
    {
        public static ChemObjectFlagBag Save(IChemObject source)
        {
            var bag = new ChemObjectFlagBag();
            return bag;
        }

        public static void Restore(IChemObject dest, ChemObjectFlagBag savedFlags)
        {
            dest.IsPlaced = savedFlags.IsPlaced;
            dest.IsVisited = savedFlags.IsVisited;
        }

        public static void Transfer(IChemObject src, IChemObject dest)
        {
            var fs = Save(src);
            Restore(src, fs);
        }

        bool IsPlaced { get; set; }
        bool IsVisited { get; set; }
    }
}
