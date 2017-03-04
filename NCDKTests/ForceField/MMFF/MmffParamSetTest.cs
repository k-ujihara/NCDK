/*
 * Copyright (c) 2015 John May <jwmay@users.sf.net>
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

namespace NCDK.ForceField.MMFF
{
    /// <summary>
    // @author John May
    /// </summary>
    [TestClass()]
    public class MmffParamSetTest
    {
        internal static readonly MmffParamSet mmffParams = MmffParamSet.Instance;

        [TestMethod()]
        public void FormalCharge()
        {
            Assert.AreEqual(0.500M, mmffParams.GetFormalCharge("NCN+"));
        }

        [TestMethod()]
        public void FormalChargeAdjustment()
        {
            Assert.AreEqual(0.500M, mmffParams.GetFormalChargeAdjustment(32));
        }

        [TestMethod()]
        public void Crd()
        {
            Assert.AreEqual(1, mmffParams.GetCrd(32));
        }

        [TestMethod()]
        public void BciBetween1And18()
        {
            Assert.AreEqual(-0.1052M, mmffParams.GetBondChargeIncrement(0, 1, 18));
        }

        [TestMethod()]
        public void BciBetween18And1()
        {
            Assert.AreEqual(0.1052M, mmffParams.GetBondChargeIncrement(0, 18, 1));
        }

        [TestMethod()]
        public void BciBetween37And63WithBondClass()
        {
            Assert.AreEqual(0.0000M, mmffParams.GetBondChargeIncrement(0, 37, 63));
            Assert.AreEqual(-0.0530M, mmffParams.GetBondChargeIncrement(1, 37, 63));
        }
    }
}
