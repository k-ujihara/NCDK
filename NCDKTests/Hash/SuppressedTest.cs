/*
 * Copyright (c) 2013 European Bioinformatics Institute (EMBL-EBI)
 *                    John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 2.1 of the License, or (at
 * your option) any later version. All we ask is that proper credit is given
 * for our work, which includes - but is not limited to - adding the above
 * copyright notice to the beginning of your source code files, and to any
 * copyright notice that you may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;

namespace NCDK.Hash
{
    /**
     * @author John May
     * @cdk.module test-hash
     */
    [TestClass()]
    public class SuppressedTest
    {

        [TestMethod()]
        public void none()
        {
            Suppressed suppressed = Suppressed.None;
            for (int i = 0; i < 1000; i++)
            {
                Assert.IsFalse(suppressed.Contains(i));
            }
        }

        [TestMethod()]
        public void Bitset()
        {
            BitArray set = new BitArray(48);
            set.Set(2, true);
            set.Set(3, true);
            set.Set(5, true);
            set.Set(7, true);
            set.Set(11, true);
            set.Set(42, true);
            Suppressed suppressed = Suppressed.FromBitSet(set);

            Assert.IsTrue(suppressed.Contains(2));
            Assert.IsTrue(suppressed.Contains(3));
            Assert.IsTrue(suppressed.Contains(5));
            Assert.IsTrue(suppressed.Contains(7));
            Assert.IsTrue(suppressed.Contains(11));
            Assert.IsTrue(suppressed.Contains(42));

            Assert.IsFalse(suppressed.Contains(0));
            Assert.IsFalse(suppressed.Contains(1));
            Assert.IsFalse(suppressed.Contains(4));
            Assert.IsFalse(suppressed.Contains(6));
            Assert.IsFalse(suppressed.Contains(8));
            Assert.IsFalse(suppressed.Contains(9));
            Assert.IsFalse(suppressed.Contains(10));
            Assert.IsFalse(suppressed.Contains(12));
            Assert.IsFalse(suppressed.Contains(13));
            Assert.IsFalse(suppressed.Contains(14));
        }
    }
}
