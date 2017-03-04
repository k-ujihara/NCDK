/* Copyright (c) 2013. John May <jwmay@users.sf.net>
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
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Hash
{
    /// <summary>
    // @author John May
    // @cdk.module test-hash
    /// </summary>
    [TestClass()]
    public class XorshiftTest
    {

        private readonly Xorshift generator = new Xorshift();

        [TestMethod()]
        public void TestNext()
        {
            Assert.AreEqual(178258005L, generator.Next(5L));
            Assert.AreEqual(5651489766934405L, generator.Next(178258005L));
            Assert.AreEqual(-9127299601691290113L, generator.Next(5651489766934405L));
            Assert.AreEqual(146455018630021125L, generator.Next(-9127299601691290113L));
            Assert.AreEqual(2104002940825447L, generator.Next(146455018630021125L));
        }

        [TestMethod()]
        public void TestDistribution()
        {

            int[] values = new int[10];

            long x = System.DateTime.Now.Ticks;

            // fill the buckets (0..10)
            for (int i = 0; i < 1000000; i++)
            {
                // mask the sign bit and take the modulus, standard hash table
                values[(int)((0x7FFFFFFFFFFFL & (x = generator.Next(x))) % 10)]++;
            }

            foreach (var v in values)
            {
                Assert.IsTrue(99000 <= v && v <= 101000, v + " was not within 0.1 % of a uniform distribution");
            }
        }

        /// <summary>
        /// demonstrates a limitation of the xor-shift, 0 will always return 0
        /// </summary>
        [TestMethod()]
        public void DemonstrateZeroLimitation()
        {
            Assert.AreEqual(0L, new Xorshift().Next(0L));
        }
    }
}
