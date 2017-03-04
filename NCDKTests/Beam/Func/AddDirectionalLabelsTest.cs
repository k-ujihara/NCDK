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
    public class AddDirectionalLabelsTest
    {
        [TestMethod()]
        public void E_butene_implH()
        {
            Transform("C\\C=C\\C",
                      "C\\C=C\\C");
            Transform("C/C=C/C",
                      "C/C=C/C");
            Transform("C\\C([H])=C\\C",
                      "C\\C(\\[H])=C\\C");
            Transform("C/C([H])=C/C",
                      "C/C(/[H])=C/C");
            Transform("C\\C([H])=C([H])\\C",
                      "C\\C(\\[H])=C(/[H])\\C");
            Transform("C/C([H])=C([H])/C",
                      "C/C(/[H])=C(\\[H])/C");
        }

        [TestMethod()]
        public void Z_butene_implH()
        {
            Transform("C\\C=C/C",
                      "C\\C=C/C");
            Transform("C/C=C\\C",
                      "C/C=C\\C");
            Transform("C\\C([H])=C/C",
                      "C\\C(\\[H])=C/C");
            Transform("C/C([H])=C\\C",
                      "C/C(/[H])=C\\C");
            Transform("C\\C([H])=C([H])/C",
                      "C\\C(\\[H])=C(\\[H])/C");
            Transform("C/C([H])=C(/[H])\\C",
                      "C/C(/[H])=C(/[H])\\C");
        }

        [TestMethod()]
        public void E_butene_expH()
        {
            Transform("C\\C([H])=C\\C",
                      "C\\C(\\[H])=C\\C");
            Transform("C/C([H])=C/C",
                      "C/C(/[H])=C/C");
            Transform("C\\C([H])=C([H])\\C",
                      "C\\C(\\[H])=C(/[H])\\C");
            Transform("C/C([H])=C([H])/C",
                      "C/C(/[H])=C(\\[H])/C");
        }

        [TestMethod()]
        public void Z_butene_expH()
        {
            Transform("C\\C([H])=C/C",
                      "C\\C(\\[H])=C/C");
            Transform("C/C([H])=C\\C",
                      "C/C(/[H])=C\\C");
            Transform("C\\C([H])=C([H])/C",
                      "C\\C(\\[H])=C(\\[H])/C");
            Transform("C/C([H])=C(/[H])\\C",
                      "C/C(/[H])=C(/[H])\\C");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidSmilesException))]
        public void Invalid()
        {
            Transform("C/C=C(/C)/([H])",
                      "n/a");
        }

        [TestMethod()]
        public void Conjugated2()
        {
            Transform("C/C=C(/C)C(\\[H])=C/C",
                      "C/C=C(/C)\\C(\\[H])=C/C");
        }

        [TestMethod()]
        public void E_e_hexadiene_implH()
        {
            Transform("C\\C=C\\C=C\\C",
                      "C\\C=C\\C=C\\C");
            Transform("C/C=C/C=C/C",
                      "C/C=C/C=C/C");
        }

        [TestMethod()]
        public void Z_z_hexadiene_implH()
        {
            Transform("C\\C=C/C=C\\C",
                      "C\\C=C/C=C\\C");
            Transform("C/C=C\\C=C/C",
                      "C/C=C\\C=C/C");
        }

        [TestMethod()]
        public void E_z_hexadiene_implH()
        {
            Transform("C\\C=C\\C=C/C",
                      "C\\C=C\\C=C/C");
            Transform("C/C=C/C=C\\C",
                      "C/C=C/C=C\\C");
        }

        [TestMethod()]
        public void Z_e_hexadiene_implH()
        {
            Transform("C/C=C\\C=C\\C",
                      "C/C=C\\C=C\\C");
            Transform("C\\C=C/C=C/C",
                      "C\\C=C/C=C/C");
        }

        [TestMethod()]
        public void E_e_hexadiene_expH()
        {
            Transform("C\\C([H])=C([H])\\C([H])=C([H])\\C",
                      "C\\C(\\[H])=C(/[H])\\C(\\[H])=C(/[H])\\C");
            Transform("C/C([H])=C([H])/C([H])=C([H])/C",
                      "C/C(/[H])=C(\\[H])/C(/[H])=C(\\[H])/C");
        }

        [TestMethod()]
        public void Z_z_hexadiene_expH()
        {
            Transform("C\\C([H])=C([H])/C([H])=C([H])\\C",
                      "C\\C(\\[H])=C(\\[H])/C(/[H])=C(/[H])\\C");
            Transform("C/C([H])=C([H])\\C([H])=C([H])/C",
                      "C/C(/[H])=C(/[H])\\C(\\[H])=C(\\[H])/C");
        }

        [TestMethod()]
        public void E_z_hexadiene_expH()
        {
            Transform("C\\C([H])=C([H])\\C([H])=C([H])/C",
                      "C\\C(\\[H])=C(/[H])\\C(\\[H])=C(\\[H])/C");
            Transform("C/C([H])=C([H])/C([H])=C([H])\\C",
                      "C/C(/[H])=C(\\[H])/C(/[H])=C(/[H])\\C");
        }

        [TestMethod()]
        public void Z_e_hexadiene_expH()
        {
            Transform("C/C(/[H])=C(/[H])\\C(\\[H])=C(/[H])\\C",
                      "C/C(/[H])=C(/[H])\\C(\\[H])=C(/[H])\\C");
            Transform("C\\C(\\[H])=C(\\[H])/C(/[H])=C(\\[H])/C",
                      "C\\C(\\[H])=C(\\[H])/C(/[H])=C(\\[H])/C");
        }

        /// <summary>Ensure cumulated double bonds don't break the transformation</summary>
        [TestMethod()]
        public void Allene()
        {
            Transform("CC=C=CC",
                      "CC=C=CC");
        }

        /// <summary>Ensure cumulated double bonds don't break the transformation</summary>
        [TestMethod()]
        public void Cumullene()
        {
            Transform("CC=C=C=CC",
                      "CC=C=C=CC");
        }

        // TODO: [TestMethod()] 
        public void InvalidConjugated()
        {
            Transform("F/C=C(/F)C(/F)=C/F",
                      "F/C=C(/F)\\C(\\F)=C\\F");
        }

        /// <summary>
        /// One double bond has an end point with no directional bonds. Fully
        /// specifying the other double bond adds one directional bond giving it a
        /// readable configuration.
        /// </summary>
        [TestMethod()]
        public void Partial1()
        {
            Transform("[H]\\C(C([H])=C(\\C)[H])=C(/C)[H]",
                      "[H]\\C(\\C(\\[H])=C(\\C)/[H])=C(/C)\\[H]");
        }


        /// <summary>The directional label between two double bonds is missing.</summary>
        [TestMethod()]
        public void Middle()
        {
            Transform("C(\\[H])(=C(\\C)[H])C(=C(/C)[H])/[H]",
                      "C(\\[H])(=C(\\C)/[H])/C(=C(/C)\\[H])/[H]");
        }

        static void Transform(string smi, string exp)
        {
            Assert.AreEqual(
                exp,
                Generator.Generate(new AddDirectionalLabels()
                                                         .Apply(Parser.Parse(smi))));

        }
    }
}
