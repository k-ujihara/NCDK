/* Copyright (C) 2007  Egon Willighagen <egonw@users.sf.net>
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Smiles;

namespace NCDK.Graphs.Matrix
{
    // @cdk.module test-core
    [TestClass()]
    public class AdjacencyMatrixTest : CDKTestCase
    {
        private static SmilesParser sp = CDK.SmilesParser;

        [TestMethod()]
        public void TestGetMatrix_IAtomContainer()
        {
            var container = sp.ParseSmiles("C1CC1");
            int[][] matrix = AdjacencyMatrix.GetMatrix(container);
            Assert.AreEqual(3, matrix.Length);
            Assert.AreEqual(3, matrix[0].Length);
            Assert.AreEqual(0, matrix[0][0]);
            Assert.AreEqual(1, matrix[0][2]);
            Assert.AreEqual(1, matrix[0][1]);
        }
    }
}
