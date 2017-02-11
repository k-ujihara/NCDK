/*
 * Copyright (c) 2013, European Bioinformatics Institute (EMBL-EBI)
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 * 1. Redistributions of source code must retain the above copyright notice, this
 *    list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright notice,
 *    this list of conditions and the following disclaimer in the documentation
 *    and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
 * ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 * The views and conclusions contained in the software and documentation are those
 * of the authors and should not be interpreted as representing official policies,
 * either expressed or implied, of the FreeBSD Project.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace NCDK.Beam
{
    /// <author>John May </author>
    [TestClass()]
    public class RemoveUpDownBondsTest
    {
        [TestMethod()]
        public void e_butene_expH()
        {
            Transform("C\\C(\\[H])=C\\C",
                      "C\\C([H])=C\\C",
                      "CC(\\[H])=C\\C");
            Transform("C/C(/[H])=C/C",
                      "C/C([H])=C/C",
                      "CC(/[H])=C/C");
            Transform("C\\C(\\[H])=C(/[H])\\C",
                      "C\\C([H])=C(/[H])C",
                      "CC(\\[H])=C([H])\\C");
            Transform("C/C(/[H])=C(\\[H])/C",
                      "C/C([H])=C(\\[H])C",
                      "CC(/[H])=C([H])/C");
        }

        [TestMethod()]
        public void z_butene_expH()
        {
            Transform("C\\C(\\[H])=C/C",
                      "C\\C([H])=C/C",
                      "CC(\\[H])=C/C");
            Transform("C/C(/[H])=C\\C",
                      "C/C([H])=C\\C",
                      "CC(/[H])=C\\C");
            Transform("C\\C(\\[H])=C(\\[H])/C",
                      "C\\C([H])=C(\\[H])C",
                      "CC(\\[H])=C([H])/C");
            Transform("C/C(/[H])=C(/[H])\\C",
                      "C/C([H])=C(/[H])C",
                      "CC(/[H])=C([H])\\C");
        }

        [TestMethod()]
        public void e_e_hexadiene_expH()
        {
            Transform("C\\C(\\[H])=C(/[H])\\C(\\[H])=C(/[H])\\C",
                      "C\\C([H])=C(/[H])\\C([H])=C(/[H])C",
                      "CC(\\[H])=C([H])\\C(\\[H])=C([H])\\C");
        }

        [TestMethod()]
        public void e_e_hexadiene_expH2()
        {
            Transform("[H]\\C(\\C(\\[H])=C(\\C)/[H])=C(/C)\\[H]",
                      "[H]\\C(\\C([H])=C(\\C)[H])=C(/C)[H]",
                      "[H]C(\\C(\\[H])=C(C)/[H])=C(C)\\[H]");
        }

        [TestMethod()]
        public void e_e_hexadiene_expH3()
        {
            Transform("[H]/C(/C(=C(/C)\\[H])[H])=C(/[H])\\C",
                      "[H]/C(/C(=C(/C)[H])[H])=C(/[H])C",
                      "[H]C(/C(=C(C)\\[H])[H])=C([H])\\C");
        }

        [TestMethod()]
        public void e_e_hexadiene_expH4()
        {
            Transform("C\\C(\\[H])=C(/[H])\\C(=C(/[H])\\C)\\[H]",
                      "C\\C([H])=C(/[H])\\C(=C(/[H])C)[H]",
                      "CC(\\[H])=C([H])\\C(=C([H])\\C)\\[H]");
        }

        [TestMethod()]
        public void e_e_hexadiene_expH5()
        {
            Transform("[H]/C(/C)=C(/[H])\\C(\\[H])=C(/[H])\\C",
                      "[H]/C(C)=C(/[H])\\C([H])=C(/[H])C",
                      "[H]C(/C)=C([H])\\C(\\[H])=C([H])\\C");
        }

        [TestMethod()]
        public void e_e_hexadiene_permute()
        {
            string input = "C\\C(=C(\\C(=C(/[H])\\C)\\[H])/[H])\\[H]";
            int[] p = new int[] { 7, 2, 4, 1, 3, 6, 8, 9, 0, 5 };
            Graph g = Parser.Parse(input);
            Assert.AreEqual("[H]\\C(\\C(=C(/[H])\\C)\\[H])=C(\\[H])/C",
                Generator.Generate(g.Permute(p)));
            Assert.IsTrue(
                new[] {
                    "[H]\\C(\\C(=C(/[H])C)[H])=C(\\[H])C",
                    "[H]C(\\C(=C([H])\\C)\\[H])=C([H])/C"
                }.Contains(
                Generator.Generate(new RemoveUpDownBonds().Apply(g.Permute(p)))));
        }

        static void Transform(string smi, params string[] exps)
        {
			var actual = Generator.Generate(new RemoveUpDownBonds()
                                                         .Apply(Parser.Parse(smi)));
            Assert.IsTrue(exps.Contains(actual));
        }
    }
}