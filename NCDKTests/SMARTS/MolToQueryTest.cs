/*
 * Copyright (C) 2018  The Chemistry Development Kit (CDK) project
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
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Graphs;
using NCDK.Isomorphisms.Matchers;
using NCDK.Smiles;

namespace NCDK.SMARTS
{
    [TestClass()]
    public class MolToQueryTest
    {
        private readonly SmilesParser smipar = CDK.SmilesParser;

        private void Test(string expected, string smi, params ExprType[] opts)
        {
            var mol = smipar.ParseSmiles(smi);
            Cycles.MarkRingAtomsAndBonds(mol);
            var query = QueryAtomContainer.Create(mol, opts);
            var actual = Smarts.Generate(query);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void NoOptsSpecified()
        {
            Test("*1~*~*~*~*~*~1~*", "c1cccnc1C");
        }

        [TestMethod()]
        public void AromaticWithBonds()
        {
            Test("a1:a:a:a:a:a:1-A", "c1cccnc1C",
                 ExprType.IsAromatic,
                 ExprType.IsAliphatic,
                 ExprType.SingleOrAromatic);
        }

        [TestMethod()]
        public void AromaticElementWithBonds()
        {
            Test("c1:c:c:c:n:c:1-*", "c1cccnc1C",
                 ExprType.AromaticElement,
                 ExprType.SingleOrAromatic);
            Test("c1:c:c:c:n:c:1-[#6]", "c1cccnc1C",
                 ExprType.IsAromatic,
                 ExprType.Element,
                 ExprType.SingleOrAromatic);
        }

        [TestMethod()]
        public void PseudoAtoms()
        {
            Test("[#6]~[#6]~*", "CC*",
                 ExprType.Element);
        }

        [TestMethod()]
        public void ElementAndDegree()
        {
            Test("[#6D2]1~[#6D2]~[#6D2]~[#6D2]~[#7D2]~[#6D3]~1~[#6D]", "c1cccnc1C",
                 ExprType.Element, ExprType.Degree);
        }

        [TestMethod()]
        public void ComplexDocExample()
        {
            Test("[nx2+0]1:[cx2+0]:[cx2+0]:[cx2+0](=[O&x0+0]):[cx2+0]:[cx2+0]:1",
                 "[nH]1ccc(=O)cc1",
                 ExprType.AliphaticElement,
                 ExprType.AromaticElement,
                 ExprType.SingleOrAromatic,
                 ExprType.AliphaticOrder,
                 ExprType.Isotope,
                 ExprType.RingBondCount,
                 ExprType.FormalCharge);
            Test("[0n+0]1:[0c+0]:[0c+0]:[0c+0](=[O+0]):[0c+0]:[0c+0]:1",
                 "[0nH]1[0cH][0cH][0cH](=O)[0cH][0cH]1",
                 ExprType.AliphaticElement,
                 ExprType.AromaticElement,
                 ExprType.SingleOrAromatic,
                 ExprType.AliphaticOrder,
                 ExprType.Isotope,
                 ExprType.FormalCharge);
        }
    }
}
