/* Copyright (C) 2006-2007  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
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
using NCDK.QSAR.Results;
using System.Collections.Generic;

namespace NCDK.QSAR
{
    /// <summary>
    /// Abstract atomic descriptor class with helper functions for descriptors
    /// that require the whole molecule to calculate the descriptor values,
    /// which in turn need to be cached for all atoms, so that they can be
    /// retrieved one by one.
    /// </summary>
    // @cdk.module qsar
    // @cdk.githash
    public abstract class AbstractAtomicDescriptor : AbstractDescriptor, IAtomicDescriptor
    {
        private const string PreviousAtomContainer = "previousAtomContainer";
        private IDictionary<object, object> cachedDescriptorValues = null;                   // FIXME: needs a better solution!

        /// <summary>
        /// Returns true if the cached IDescriptorResult's are for the given IAtomContainer.
        /// </summary>
        /// <param name="container"></param>
        /// <returns>false, if the cache is for a different IAtomContainer</returns>
        public bool IsCachedAtomContainer(IAtomContainer container)
        {
            if (cachedDescriptorValues == null) return false;
            return (cachedDescriptorValues[PreviousAtomContainer] == container);
        }

        /// <summary>
        /// Returns the cached DescriptorValue for the given IAtom.
        /// </summary>
        /// <param name="atom">the IAtom for which the DescriptorValue is requested</param>
        /// <returns>null, if no DescriptorValue was cached for the given IAtom</returns>
        public IDescriptorResult GetCachedDescriptorValue(IAtom atom)
        {
            if (cachedDescriptorValues == null) return null;
            return (IDescriptorResult)cachedDescriptorValues[atom];
        }

        /// <summary>
        /// Caches a DescriptorValue for a given IAtom. This method may only
        /// be called after SetNewContainer() is called.
        /// </summary>
        /// <param name="atom">IAtom to cache the value for</param>
        /// <param name="container"></param>
        /// <param name="value">DescriptorValue for the given IAtom</param>
        public void CacheDescriptorValue(IAtom atom, IAtomContainer container, IDescriptorResult value)
        {
            if (cachedDescriptorValues == null)
            {
                cachedDescriptorValues = new Dictionary<object, object>();
                cachedDescriptorValues[PreviousAtomContainer] = container;
            }
            else if (cachedDescriptorValues[PreviousAtomContainer] != container)
            {
                cachedDescriptorValues.Clear();
                cachedDescriptorValues[PreviousAtomContainer] = container;
            }
            cachedDescriptorValues[atom] = value;
        }

        public abstract DescriptorValue Calculate(IAtom atom, IAtomContainer container);
    }
}
