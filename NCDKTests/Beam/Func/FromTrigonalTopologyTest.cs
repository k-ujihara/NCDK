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
    // @author John May 
    [TestClass()]
    public class FromTrigonalTopologyTest
    {

        [TestMethod()]
        public void Z_dichloroethene()
        {
            Transform("F[C@H]=[C@H]F", "F/C=C\\F");
        }

        [TestMethod()]
        public void Z_dichloroethene_alt()
        {
            Transform("F[C@@H]=[C@@H]F", "F\\C=C/F");
        }

        [TestMethod()]
        public void Z_dichloroethene_explicitH()
        {
            Transform("F[C@]([H])=[C@](F)[H]", "F/C(/[H])=C(\\F)/[H]");
        }

        [TestMethod()]
        public void Z_dichloroethene_alt_explicitH()
        {
            Transform("F[C@@]([H])=[C@@](F)[H]", "F\\C(\\[H])=C(/F)\\[H]");
        }

        [TestMethod()]
        public void E_dichloroethene()
        {
            Transform("F[C@H]=[C@@H]F", "F/C=C/F");
        }

        [TestMethod()]
        public void E_dichloroethene_alt()
        {
            Transform("F[C@@H]=[C@H]F", "F\\C=C\\F");
        }

        [TestMethod()]
        public void E_dichloroethene_explicitH()
        {
            Transform("F[C@]([H])=[C@@](F)[H]", "F/C(/[H])=C(/F)\\[H]");
        }

        [TestMethod()]
        public void E_dichloroethene_alt_explicitH()
        {
            Transform("F[C@@]([H])=[C@](F)[H]", "F\\C(\\[H])=C(\\F)/[H]");
        }

        [TestMethod()]
        public void Z_dichloroethene_permuted_1()
        {
            Transform("F[C@H]=[C@H]F", new int[] { 1, 0, 2, 3 }, "C(\\F)=C\\F");
        }

        [TestMethod()]
        public void Z_dichloroethene_permuted_2()
        {
            Transform("F[C@H]=[C@H]F", new int[] { 3, 2, 1, 0 }, "F\\C=C/F");
        }

        [TestMethod()]
        public void Z_dichloroethene_alt_permuted_1()
        {
            Transform("F[C@@H]=[C@@H]F", new int[] { 1, 0, 2, 3 }, "C(/F)=C/F");
        }

        [TestMethod()]
        public void Z_dichloroethene_alt_permuted_2()
        {
            Transform("F[C@@H]=[C@@H]F", new int[] { 3, 2, 1, 0 }, "F/C=C\\F");
        }

        [TestMethod()]
        public void E_dichloroethene_permuted_1()
        {
            Transform("F[C@H]=[C@@H]F", new int[] { 1, 0, 2, 3 }, "C(\\F)=C/F");
        }

        [TestMethod()]
        public void E_dichloroethene_permuted_2()
        {
            Transform("F[C@@H]=[C@H]F", new int[] { 3, 2, 1, 0 }, "F\\C=C\\F");
        }

        [TestMethod()]
        public void Conjugated()
        {
            Transform("F[C@H]=[C@@H][C@H]=[C@@H]F", "F/C=C/C=C/F");
        }

        /// <summary>Ensures that conflicting directional assignments are resolved.</summary>
        [TestMethod()]
        public void Conjugated_conflict()
        {
            Transform("F[C@H]=[C@@H][C@@H]=[C@H]F", "F/C=C/C=C/F");
        }



        [TestMethod()]
        public void Cyclooctatetraene_1()
        {
            Transform("[C@H]1=[C@@H][C@@H]=[C@@H][C@@H]=[C@@H][C@@H]=[C@@H]1",
                      "C\\1=C\\C=C/C=C\\C=C1");
        }

        [TestMethod()]
        public void Cyclooctatetraene_2()
        {
            Transform("[C@@H]1=[C@H][C@H]=[C@H][C@H]=[C@H][C@H]=[C@H]1",
                      "C/1=C/C=C\\C=C/C=C1");
        }

        //    [TestMethod()] public void Cyclooctatetraene_3() {
        //        Apply("[C@H]1=[C@@H][C@H]=[C@H][C@@H]=[C@@H][C@H]=[C@@H]1",
        //                  "C\\1=C/C=C\\C=C/C=C1");
        //    }

        static void Transform(string smi, string exp)
        {
            ImplicitToExplicit ite = new ImplicitToExplicit();
            FromTrigonalTopology ftt = new FromTrigonalTopology();
            ExplicitToImplicit eti = new ExplicitToImplicit();
            Assert.AreEqual(exp, Generator
                                      .Generate(eti.Apply(ftt.Apply(ite.Apply(Parser.Parse(smi))))));
        }

        static void Transform(string smi, int[] p, string exp)
        {
            ImplicitToExplicit ite = new ImplicitToExplicit();
            FromTrigonalTopology ftt = new FromTrigonalTopology();
            ExplicitToImplicit eti = new ExplicitToImplicit();
            Assert.AreEqual(exp, Generator
                                      .Generate(eti.Apply(ftt.Apply(ite.Apply(Parser.Parse(smi)
                                                                                    .Permute(p))))));
        }
    }
}
