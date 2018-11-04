/*
 * Copyright (c) 2016 John May <jwmay@users.sf.net>
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
using NCDK.Smiles;

namespace NCDK.SMARTS
{
    [TestClass()]
    public class SmartsFragmentExtractorTest
    {
        private static string Generate(string smi, SubstructureSelectionMode mode, int[] idxs)
        {
            var smipar = CDK.SmilesParser;
            var mol = smipar.ParseSmiles(smi);
            var subsmarts = new SmartsFragmentExtractor(mol);
            subsmarts.SetMode(mode);
            return subsmarts.Generate(idxs);
        }

        private static int[] MakeSeq(int beg, int to)
        {
            var a = new int[to - beg];
            for (int i = 0; i < a.Length; i++)
                a[i] = beg++;
            return a;
        }

        [TestMethod()]
        public void MethylExact()
        {
            var smarts = Generate("CC(C)CCC", SubstructureSelectionMode.ExactSmarts, MakeSeq(0, 1));
            Assert.AreEqual("[CH3v4X4+0]", smarts);
        }

        [TestMethod()]
        public void MethylForJCompoundMap()
        {
            var smarts = Generate("CC(C)CCC", SubstructureSelectionMode.JCompoundMapper, MakeSeq(0, 1));
            Assert.AreEqual("C*", smarts);
        }

        [TestMethod()]
        public void Indole()
        {
            var smarts = Generate("[nH]1ccc2c1cccc2", SubstructureSelectionMode.ExactSmarts, MakeSeq(0, 4));
            Assert.AreEqual("[nH1v3X3+0][cH1v4X3+0][cH1v4X3+0][cH0v4X3+0]", smarts);
        }

        [TestMethod()]
        public void IndoleForJCompoundMap()
        {
            var smarts = Generate("[nH]1ccc2c1cccc2", SubstructureSelectionMode.JCompoundMapper, MakeSeq(0, 4));
            Assert.AreEqual("n(ccc(a)a)a", smarts);
        }

        [TestMethod()]
        public void BiphenylIncludesSingleBond()
        {
            var smarts = Generate("c1ccccc1-c1ccccc1", SubstructureSelectionMode.ExactSmarts, MakeSeq(0, 12));
            Assert.IsTrue(smarts.Contains("-"));
        }

        [TestMethod()]
        public void FullereneC60()
        {
            var smarts = Generate("c12c3c4c5c1c1c6c7c2c2c8c3c3c9c4c4c%10c5c5c1c1c6c6c%11c7c2c2c7c8c3c3c8c9c4c4c9c%10c5c5c1c1c6c6c%11c2c2c7c3c3c8c4c4c9c5c1c1c6c2c3c41", SubstructureSelectionMode.ExactSmarts, MakeSeq(0, 60));
            Assert.AreEqual("[cH0v4X3+0]12[cH0v4X3+0]3[cH0v4X3+0]4[cH0v4X3+0]5[cH0v4X3+0]1[cH0v4X3+0]1[cH0v4X3+0]6[cH0v4X3+0]7[cH0v4X3+0]2[cH0v4X3+0]2[cH0v4X3+0]8[cH0v4X3+0]3[cH0v4X3+0]3[cH0v4X3+0]9[cH0v4X3+0]4[cH0v4X3+0]4[cH0v4X3+0]%10[cH0v4X3+0]5[cH0v4X3+0]5[cH0v4X3+0]1[cH0v4X3+0]1[cH0v4X3+0]6[cH0v4X3+0]6[cH0v4X3+0]%11[cH0v4X3+0]7[cH0v4X3+0]2[cH0v4X3+0]2[cH0v4X3+0]7[cH0v4X3+0]8[cH0v4X3+0]3[cH0v4X3+0]3[cH0v4X3+0]8[cH0v4X3+0]9[cH0v4X3+0]4[cH0v4X3+0]4[cH0v4X3+0]9[cH0v4X3+0]%10[cH0v4X3+0]5[cH0v4X3+0]5[cH0v4X3+0]1[cH0v4X3+0]1[cH0v4X3+0]6[cH0v4X3+0]6[cH0v4X3+0]%11[cH0v4X3+0]2[cH0v4X3+0]2[cH0v4X3+0]7[cH0v4X3+0]3[cH0v4X3+0]3[cH0v4X3+0]8[cH0v4X3+0]4[cH0v4X3+0]4[cH0v4X3+0]9[cH0v4X3+0]5[cH0v4X3+0]1[cH0v4X3+0]1[cH0v4X3+0]6[cH0v4X3+0]2[cH0v4X3+0]3[cH0v4X3+0]41", smarts);
        }
    }
}
