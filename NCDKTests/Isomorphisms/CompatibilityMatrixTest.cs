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

using NCDK.Common.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
namespace NCDK.Isomorphisms
{
    /// <summary>
    // @author John May
    // @cdk.module test-isomorphism
    /// </summary>
    [TestClass()]
    public class CompatibilityMatrixTest
    {

        [TestMethod()]
        public void AccessAndModify()
        {
            CompatibilityMatrix m = new CompatibilityMatrix(5, 5);
            Assert.IsFalse(m.Get1(0, 1));
            Assert.IsFalse(m.Get1(0, 4));
            Assert.IsFalse(m.Get1(1, 0));
            Assert.IsFalse(m.Get1(1, 3));
            m.Set1(0, 1);
            m.Set1(0, 4);
            m.Set1(1, 0);
            m.Set1(1, 3);
            Assert.IsTrue(m.Get1(0, 1));
            Assert.IsTrue(m.Get1(0, 4));
            Assert.IsTrue(m.Get1(1, 0));
            Assert.IsTrue(m.Get1(1, 3));
        }

        [TestMethod()]
        public void Mark()
        {
            CompatibilityMatrix m = new CompatibilityMatrix(5, 5);
            m.Set1(0, 1);
            m.Set1(0, 2);
            m.Set1(0, 4);
            m.Set1(1, 0);
            m.Set1(1, 3);
            Assert.IsTrue(m.Get1(0, 1));
            Assert.IsTrue(m.Get1(0, 4));
            m.Mark(0, 1, -1);
            m.Mark(0, 4, -4);
            m.Mark(1, 3, -6);
            Assert.IsFalse(m.Get1(0, 1));
            Assert.IsFalse(m.Get1(0, 4));
            Assert.IsFalse(m.Get1(1, 3));
            m.ResetRows(0, -1);
            Assert.IsTrue(m.Get1(0, 1));
            Assert.IsFalse(m.Get1(0, 4));
            Assert.IsFalse(m.Get1(1, 3));
            m.ResetRows(0, -4);
            Assert.IsTrue(m.Get1(0, 1));
            Assert.IsTrue(m.Get1(0, 4));
            Assert.IsFalse(m.Get1(1, 3));
            m.ResetRows(0, -6);
            Assert.IsTrue(m.Get1(0, 1));
            Assert.IsTrue(m.Get1(0, 4));
            Assert.IsTrue(m.Get1(1, 3));
        }

        [TestMethod()]
        public void MarkRow()
        {
            CompatibilityMatrix m = new CompatibilityMatrix(5, 5);
            m.Set1(0, 1);
            m.Set1(0, 2);
            m.Set1(0, 4);
            Assert.IsTrue(Compares.AreEqual(new int[] { 0, 1, 1, 0, 1 }, m.Fix()[0]));
            m.MarkRow(0, -1);
            Assert.IsTrue(Compares.AreEqual(new int[] { 0, -1, -1, 0, -1 }, m.Fix()[0]));
        }

        [TestMethod()]
        public void Fix()
        {
            CompatibilityMatrix m = new CompatibilityMatrix(5, 5);
            m.Set1(0, 1);
            m.Set1(0, 2);
            m.Set1(0, 4);
            m.Set1(1, 0);
            m.Set1(1, 3);
            m.Set1(2, 4);
            Assert.IsTrue(Compares.AreDeepEqual(new int[][] {
                new[] {0, 1, 1, 0, 1},
                new[] {1, 0, 0, 1, 0},
                new[] {0, 0, 0, 0, 1},
                new[] {0, 0, 0, 0, 0},
                new[] {0, 0, 0, 0, 0},}, m.Fix()));
        }
    }
}
