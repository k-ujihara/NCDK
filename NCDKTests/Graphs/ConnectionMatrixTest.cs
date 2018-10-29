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
using NCDK.Common.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Silent;
using NCDK.Graphs.Matrix;
using NCDK.Smiles;

namespace NCDK.Graphs
{
    // @cdk.module test-standard
    [TestClass()]
    public class ConnectionMatrixTest : CDKTestCase
    {
        private readonly static SmilesParser sp = CDK.SilentSmilesParser;

        public ConnectionMatrixTest()
                : base()
        { }

        [TestMethod()]
        public void TestGetMatrix_IAtomContainer()
        {
            var container = sp.ParseSmiles("C1CC1");
            double[][] matrix = ConnectionMatrix.GetMatrix(container);
            Assert.AreEqual(3, matrix.Length);
            Assert.AreEqual(3, matrix[0].Length);
        }

        [TestMethod()]
        public void TestLonePairs()
        {
            IAtomContainer container = new AtomContainer();
            container.Atoms.Add(container.Builder.NewAtom("I"));
            container.LonePairs.Add(container.Builder.NewLonePair(container.Atoms[0]));
            container.Atoms.Add(container.Builder.NewAtom("H"));
            container.Builder.NewBond(container.Atoms[0], container.Atoms[1], BondOrder.Single);

            double[][] matrix = ConnectionMatrix.GetMatrix(container);
            Assert.AreEqual(2, matrix.Length);
            Assert.AreEqual(2, matrix[0].Length);
        }
    }
}
