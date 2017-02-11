/* Copyright (C) 2005-2007  Egon Willighagen <egonw@users.sf.net>
 * Copyright (C) 2011       Jonathan Alvarsson <jonalv@users.sf.net>
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
using System.Linq;
using System.Collections.Generic;

namespace NCDK.Fingerprint
{
    /**
    // Interface for fingerprint calculators.
     *
    // @author         egonw
    // @cdk.keyword    fingerprint
    // @cdk.module     core
    // @cdk.githash
     */
#if TEST
    public
#endif
    interface IFingerprinter
    {
        /**
        // Returns the bit fingerprint for the given <see cref="IAtomContainer"/>.
         *
        // @param  container <see cref="IAtomContainer"/> for which the fingerprint should be calculated.
        // @return           the bit fingerprint
        // @ may be thrown if there is an error during aromaticity detection
        // or (for key based fingerprints) if there is a SMARTS parsing error
        // @ if the Fingerprinter can not produce bit fingerprints
         */
        IBitFingerprint GetBitFingerprint(IAtomContainer container);

        /**
        // Returns the count fingerprint for the given <see cref="IAtomContainer"/>.
         *
        // @param container <see cref="IAtomContainer"/> for which the fingerprint should be calculated.
        // @return the count fingerprint
        // @ if there is an error during aromaticity detection
        // or (for key based fingerprints) if there is a SMARTS parsing error.
        // @ if the Fingerprinter can not produce count fingerprints
         */
        ICountFingerprint GetCountFingerprint(IAtomContainer container);

        /**
        // Returns the raw representation of the fingerprint for the given IAtomContainer. The raw representation contains
        // counts as well as the key strings.
         *
        // @param container IAtomContainer for which the fingerprint should be calculated.
        // @return the raw fingerprint
        // @
         */
        IDictionary<string, int> GetRawFingerprint(IAtomContainer container);

        /**
        // Returns the size of the fingerprints calculated.
         *
        // @return the size of the fingerprint
         */
        int Count { get; }
    }
}
