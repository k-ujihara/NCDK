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

namespace NCDK.Beam
{
    /// <author>John May </author>
    [TestClass()]
    public class ToTrigonalTopologyTest
    {
        [TestMethod()]
        public void e_difluoroethene_impl1()
        {
            Transform("F/C=C/F", "F[C@H]=[C@@H]F");
        }

        [TestMethod()]
        public void z_difluoroethene_impl2()
        {
            Transform("F/C=C\\F", "F[C@H]=[C@H]F");
        }

        [TestMethod()]
        public void e_difluoroethene_impl3()
        {
            Transform("F\\C=C\\F", "F[C@@H]=[C@H]F");
        }

        [TestMethod()]
        public void z_difluoroethene_impl4()
        {
            Transform("F\\C=C/F", "F[C@@H]=[C@@H]F");
        }

        [TestMethod()]
        public void e_difluoroethene_exp1()
        {
            Transform("F/C([H])=C([H])/F", "F[C@]([H])=[C@@]([H])F");
        }

        [TestMethod()]
        public void z_difluoroethene_exp2()
        {
            Transform("F/C([H])=C([H])\\F", "F[C@]([H])=[C@]([H])F");
        }

        [TestMethod()]
        public void e_difluoroethene_exp3()
        {
            Transform("F\\C([H])=C([H])\\F", "F[C@@]([H])=[C@]([H])F");
        }

        [TestMethod()]
        public void z_difluoroethene_exp4()
        {
            Transform("F\\C([H])=C([H])/F", "F[C@@]([H])=[C@@]([H])F");
        }

        [TestMethod()]
        public void z_difluoroethene_exp5()
        {
            Transform("FC(\\[H])=C([H])/F", "F[C@@]([H])=[C@@]([H])F");
            Transform("FC(\\[H])=C(\\[H])F", "F[C@@]([H])=[C@@]([H])F");
            Transform("F\\C([H])=C(\\[H])F", "F[C@@]([H])=[C@@]([H])F");
        }

        [TestMethod()]
        public void e_difluoroethene_exp6()
        {
            Transform("FC(\\[H])=C([H])\\F", "F[C@@]([H])=[C@]([H])F");
            Transform("FC(\\[H])=C(/[H])F", "F[C@@]([H])=[C@]([H])F");
            Transform("F\\C([H])=C(/[H])F", "F[C@@]([H])=[C@]([H])F");
        }

        [TestMethod()]
        public void z_difluoroethene_exp7()
        {
            Transform("FC(/[H])=C([H])\\F", "F[C@]([H])=[C@]([H])F");
            Transform("FC(/[H])=C(/[H])F", "F[C@]([H])=[C@]([H])F");
            Transform("F/C([H])=C(/[H])F", "F[C@]([H])=[C@]([H])F");
        }

        [TestMethod()]
        public void e_difluoroethene_exp8()
        {
            Transform("FC(/[H])=C([H])/F", "F[C@]([H])=[C@@]([H])F");
            Transform("FC(/[H])=C(\\[H])F", "F[C@]([H])=[C@@]([H])F");
            Transform("F/C([H])=C(\\[H])F", "F[C@]([H])=[C@@]([H])F");
        }

        [TestMethod()]
        public void e_difluoroethene_explicitH_9()
        {
            Transform("C(\\F)([H])=C([H])/F",
                      "[C@](F)([H])=[C@@]([H])F");
        }

        [TestMethod()]
        public void e_difluoroethene_permuted()
        {
            Transform("F/C=C/F",
                      new int[] { 1, 0, 2, 3 },
                      "[C@@H](F)=[C@@H]F");
        }

        [TestMethod()]
        public void e_difluoroethene_explicitH_permutation_1()
        {
            Transform("F/C([H])=C([H])/F",
                      new int[] { 1, 0, 2, 3, 4, 5 },
                      "[C@](F)([H])=[C@@]([H])F");
        }

        [TestMethod()]
        public void e_difluoroethene_explicitH_permutation_2()
        {
            Transform("F/C([H])=C([H])/F",
                      new int[] { 2, 0, 1, 3, 4, 5 },
                      "[C@@]([H])(F)=[C@@]([H])F");
        }

        [TestMethod()]
        public void e_difluoroethene_explicitH_permutation_3()
        {
            Transform("F/C([H])=C([H])/F",
                      new int[] { 2, 0, 1, 3, 5, 4 },
                      "[C@@]([H])(F)=[C@](F)[H]");
        }

        [TestMethod()]
        public void Cyclooctatetraene()
        {
            Transform("C/1=C/C=C\\C=C/C=C1",
                      "[C@H]1=[C@@H][C@H]=[C@H][C@@H]=[C@@H][C@H]=[C@H]1");
        }

        [TestMethod()]
        public void Unspecified()
        {
            Transform("FC=CF",
                      "FC=CF");
        }

        static void Transform(string smi, string exp)
        {
            ImplicitToExplicit ite = new ImplicitToExplicit();
            ToTrigonalTopology ttt = new ToTrigonalTopology();
            ExplicitToImplicit eti = new ExplicitToImplicit();
            Assert.AreEqual(exp, Generator
                                      .Generate(eti.Apply(ttt.Apply(ite.Apply(Parser.Parse(smi))))));
        }

        static void Transform(string smi, int[] p, string exp)
        {
            ImplicitToExplicit ite = new ImplicitToExplicit();
            ToTrigonalTopology ttt = new ToTrigonalTopology();
            ExplicitToImplicit eti = new ExplicitToImplicit();
            Assert.AreEqual(exp, Generator
                                      .Generate(eti.Apply(ttt.Apply(ite.Apply(Parser.Parse(smi)
                                                                                        .Permute(p))))));
        }
    }
}
