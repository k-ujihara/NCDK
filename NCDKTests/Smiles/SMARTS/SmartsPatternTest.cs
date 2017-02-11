/*
 * Copyright (c) 2014 European Bioinformatics Institute (EMBL-EBI)
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

namespace NCDK.Smiles.SMARTS
{
    /**
     * @author John May
     */
	 [TestClass()]
    public class SmartsPatternTest
    {
        IChemObjectBuilder bldr = Silent.ChemObjectBuilder.Instance;

        [TestMethod()]
        public void RingSizeOrNumber_membership()
        {
            Assert.IsFalse(SmartsPattern.RingSizeOrNumber("[R]"));
        }

        [TestMethod()]
        public void RingSizeOrNumber_ringConnectivity()
        {
            Assert.IsFalse(SmartsPattern.RingSizeOrNumber("[X2]"));
        }

        [TestMethod()]
        public void RingSizeOrNumber_elements()
        {
            Assert.IsFalse(SmartsPattern.RingSizeOrNumber("[Br]"));
            Assert.IsFalse(SmartsPattern.RingSizeOrNumber("[Cr]"));
            Assert.IsFalse(SmartsPattern.RingSizeOrNumber("[Fr]"));
            Assert.IsFalse(SmartsPattern.RingSizeOrNumber("[Sr]"));
            Assert.IsFalse(SmartsPattern.RingSizeOrNumber("[Ra]"));
            Assert.IsFalse(SmartsPattern.RingSizeOrNumber("[Re]"));
            Assert.IsFalse(SmartsPattern.RingSizeOrNumber("[Rf]"));
        }

        [TestMethod()]
        public void RingSizeOrNumber_negatedMembership()
        {
            Assert.IsTrue(SmartsPattern.RingSizeOrNumber("[!R]"));
        }

        [TestMethod()]
        public void RingSizeOrNumber_membershipZero()
        {
            Assert.IsTrue(SmartsPattern.RingSizeOrNumber("[R0]"));
        }

        [TestMethod()]
        public void RingSizeOrNumber_membershipTwo()
        {
            Assert.IsTrue(SmartsPattern.RingSizeOrNumber("[R2]"));
        }

        [TestMethod()]
        public void RingSizeOrNumber_ringSize()
        {
            Assert.IsTrue(SmartsPattern.RingSizeOrNumber("[r5]"));
        }

        [TestMethod()]
        public void Components()
        {
            Assert.IsTrue(SmartsPattern.Create("(O).(O)", bldr).Matches(Smi("O.O")));
            Assert.IsFalse(SmartsPattern.Create("(O).(O)", bldr).Matches(Smi("OO")));
        }

        [TestMethod()]
        public void Stereochemistry()
        {
            Assert.IsTrue(SmartsPattern.Create("C[C@H](O)CC", bldr).Matches(Smi("C[C@H](O)CC")));
            Assert.IsFalse(SmartsPattern.Create("C[C@H](O)CC", bldr).Matches(Smi("C[C@@H](O)CC")));
            Assert.IsFalse(SmartsPattern.Create("C[C@H](O)CC", bldr).Matches(Smi("CC(O)CC")));
        }

        IAtomContainer Smi(string smi)
        {
            return new SmilesParser(bldr).ParseSmiles(smi);
        }
    }
}
