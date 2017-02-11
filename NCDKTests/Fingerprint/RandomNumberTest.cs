/*
 * Copyright (C) 2012 John May <jwmay@users.sf.net>
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

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Fingerprint
{
    /**
     * Unit tests for the {@link RandomNumber}.
     *
     * @author John May
     * @cdk.module test-fingerprint
     */
    [TestClass()]
    public class RandomNumberTest
    {
        private RandomNumber rn = new RandomNumber();

        /**
         * Tests the pseudorandom number generation to make sure we alway generate
         * the same "next" random number.
         */
        [TestMethod()]
        public void TestGenerateMersenneTwisterRandomNumber()
        {
            Assert.AreEqual(444,
                    rn.GenerateMersenneTwisterRandomNumber(1024, 42),
                    "Expected next random number to be 444");
            Assert.AreEqual(748,
                    rn.GenerateMersenneTwisterRandomNumber(1024, 444),
                    "Expected next random number to be 748");
        }
    }
}
