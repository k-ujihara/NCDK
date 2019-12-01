/*
 * Copyright (c) 2015 John May <jwmay@users.sf.net>
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
using NCDK.Common.Base;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Renderers.Generators.Standards
{
    [TestClass()]
    public class AbbreviationLabelTest
    {
        [TestMethod()]
        public void CarboxylicAcid()
        {
            var tokens = new List<string>();
            Assert.IsTrue(AbbreviationLabel.Parse("COOH", tokens));
            Assert.AreEqual(4, tokens.Count);
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(new[] { "C", "O", "O", "H" }, tokens));
        }

        [TestMethod()]
        public void Carboxylate()
        {
            var tokens = new List<string>();
            Assert.IsTrue(AbbreviationLabel.Parse("COO-", tokens));
            Assert.AreEqual(4, tokens.Count);
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(new[] { "C", "O", "O", "-" }, tokens));
        }

        [TestMethod()]
        public void Trifluromethyl()
        {
            var tokens = new List<string>();
            Assert.IsTrue(AbbreviationLabel.Parse("CF3", tokens));
            Assert.AreEqual(2, tokens.Count);
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(new[] { "C", "F3" }, tokens));
        }

        [TestMethod()]
        public void Triphenylmethyl()
        {
            var tokens = new List<string>();
            Assert.IsTrue(AbbreviationLabel.Parse("CPh3", tokens));
            Assert.AreEqual(2, tokens.Count);
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(new[] { "C", "Ph3" }, tokens));
        }

        [TestMethod()]
        public void Tertbutyls()
        {
            var tokens = new List<string>(1);
            foreach (var str in new[] { "tBu", "tertBu", "t-Bu", "t-Butyl", "tertButyl" })
            {
                tokens.Clear();
                Assert.IsTrue(AbbreviationLabel.Parse(str, tokens), str);
                Assert.AreEqual(1, tokens.Count);
            }
        }

        [TestMethod()]
        public void Peglinker()
        {
            var tokens = new List<string>();
            Assert.IsTrue(AbbreviationLabel.Parse("CH2CH2OCH2CH2O", tokens));
            Assert.AreEqual(10, tokens.Count);
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(new[] { "C", "H2", "C", "H2", "O", "C", "H2", "C", "H2", "O" }, tokens));
        }

        [TestMethod()]
        public void ParseFeacac3()
        {
            var tokens = new List<string>();
            Assert.IsTrue(AbbreviationLabel.Parse("Fe(acac)3", tokens));
            Assert.AreEqual(5, tokens.Count);
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(new[] { "Fe", "(", "acac", ")", "3" }, tokens));
        }

        [TestMethod()]
        public void FormatFeacac3()
        {
            var tokens = new List<string>();
            Assert.IsTrue(AbbreviationLabel.Parse("Fe(acac)3", tokens));
            List<AbbreviationLabel.FormattedText> formatted = AbbreviationLabel.Format(tokens);
            Assert.AreEqual("Fe(acac)", formatted[0].Text);
            Assert.AreEqual(0, formatted[0].Style);
            Assert.AreEqual("3", formatted[1].Text);
            Assert.AreEqual(-1, formatted[1].Style);
        }

        [TestMethod()]
        public void FormatRubpy3Cl2()
        {
            var tokens = new List<string>();
            Assert.IsTrue(AbbreviationLabel.Parse("Ru(bpy)3Cl2", tokens));
            var formatted = AbbreviationLabel.Format(tokens);
            AbbreviationLabel.Reduce(formatted, 0, formatted.Count);
            Assert.AreEqual("Ru(bpy)", formatted[0].Text);
            Assert.AreEqual(0, formatted[0].Style);
            Assert.AreEqual("3", formatted[1].Text);
            Assert.AreEqual(-1, formatted[1].Style);
            Assert.AreEqual("Cl", formatted[2].Text);
            Assert.AreEqual(0, formatted[2].Style);
            Assert.AreEqual("2", formatted[3].Text);
            Assert.AreEqual(-1, formatted[3].Style);
        }

        [TestMethod()]
        public void CO2Et()
        {
            var tokens = new List<string>();
            Assert.IsTrue(AbbreviationLabel.Parse("CO2Et", tokens));
            Assert.AreEqual(3, tokens.Count);
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(new[] { "C", "O2", "Et" }, tokens));
        }

        [TestMethod()]
        public void ParseBrackets()
        {
            var tokens = new List<string>();
            Assert.IsTrue(AbbreviationLabel.Parse("N(CH2CH2O)CH2", tokens));
            Assert.AreEqual(10, tokens.Count);
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(new[] { "N", "(", "C", "H2", "C", "H2", "O", ")", "C", "H2" }, tokens));
        }

        [TestMethod()]
        public void ReversingBrackets()
        {
            var tokens = new List<string>();
            Assert.IsTrue(AbbreviationLabel.Parse("N(CH2CH2O)CH2", tokens));
            AbbreviationLabel.Reverse(tokens);
            Assert.AreEqual("H2C(OH2CH2C)N", string.Join("", tokens));
        }

        [TestMethod()]
        public void ReversingFormatPOOHOEt()
        {
            List<string> tokens = new List<string>();
            Assert.IsTrue(AbbreviationLabel.Parse("PO(OH)OEt", tokens));
            AbbreviationLabel.Reverse(tokens);
            AbbreviationLabel.Format(tokens);
            Assert.AreEqual("EtO(HO)OP", string.Join("", tokens));
        }

        [TestMethod()]
        public void ReversingBracketsWithNumbers()
        {
            List<string> tokens = new List<string>();
            Assert.IsTrue(AbbreviationLabel.Parse("B(OH)2", tokens));
            AbbreviationLabel.Reverse(tokens);
            Assert.AreEqual("(HO)2B", string.Join("", tokens));
        }

        [TestMethod()]
        public void NonAbbreviationLabel()
        {
            var tokens = new List<string>();
            Assert.IsFalse(AbbreviationLabel.Parse("A Random Label - Don't Reverse", tokens));
            Assert.AreEqual(1, tokens.Count);
        }

        [TestMethod()]
        public void FormatOPO3()
        {
            var tokens = new[] { "O", "P", "O3", "-2" };
            var texts = AbbreviationLabel.Format(tokens);
            Assert.AreEqual(3, texts.Count);
            Assert.AreEqual("OPO", texts[0].Text);
            Assert.AreEqual(AbbreviationLabel.STYLE_NORMAL, texts[0].Style);
            Assert.AreEqual("3", texts[1].Text);
            Assert.AreEqual(AbbreviationLabel.STYLE_SUBSCRIPT, texts[1].Style);
            Assert.AreEqual("2−", texts[2].Text);
            Assert.AreEqual(AbbreviationLabel.STYLE_SUPSCRIPT, texts[2].Style);
        }

        [TestMethod()]
        public void FormatTBu()
        {
            var tokens = new[] { "tBu" };
            List<AbbreviationLabel.FormattedText> texts = AbbreviationLabel.Format(tokens);
            Assert.AreEqual(2, texts.Count);
            Assert.AreEqual("t", texts[0].Text);
            Assert.AreEqual(AbbreviationLabel.STYLE_ITALIC, texts[0].Style);
            Assert.AreEqual("Bu", texts[1].Text);
            Assert.AreEqual(AbbreviationLabel.STYLE_NORMAL, texts[1].Style);
        }

        [TestMethod()]
        public void NEt3DotHCl()
        {
            List<string> tokens = new List<string>();
            Assert.IsTrue(AbbreviationLabel.Parse("NEt3·HCl", tokens));
            Assert.AreEqual(5, tokens.Count);
            Assert.AreEqual("N", tokens[0]);
            Assert.AreEqual("Et3", tokens[1]);
            Assert.AreEqual("·", tokens[2]);
            Assert.AreEqual("H", tokens[3]);
            Assert.AreEqual("Cl", tokens[4]);
            List<AbbreviationLabel.FormattedText> formatted = AbbreviationLabel.Format(tokens);
            AbbreviationLabel.Reduce(formatted, 0, formatted.Count);
            Assert.AreEqual(3, formatted.Count);
            Assert.AreEqual("NEt", formatted[0].Text);
            Assert.AreEqual(AbbreviationLabel.STYLE_NORMAL, formatted[0].Style);
            Assert.AreEqual("3", formatted[1].Text);
            Assert.AreEqual(AbbreviationLabel.STYLE_SUBSCRIPT, formatted[1].Style);
            Assert.AreEqual("·HCl", formatted[2].Text);
            Assert.AreEqual(AbbreviationLabel.STYLE_NORMAL, formatted[2].Style);
        }

        [TestMethod()]
        public void FormatOPO3H2()
        {
            var tokens = new[] { "O", "P", "O3", "H2" };
            List<AbbreviationLabel.FormattedText> texts = AbbreviationLabel.Format(tokens);
            Assert.AreEqual(4, texts.Count);
            Assert.AreEqual("OPO", texts[0].Text);
            Assert.AreEqual(0, texts[0].Style);
            Assert.AreEqual("3", texts[1].Text);
            Assert.AreEqual(-1, texts[1].Style);
            Assert.AreEqual("H", texts[2].Text);
            Assert.AreEqual(0, texts[2].Style);
            Assert.AreEqual("2", texts[3].Text);
            Assert.AreEqual(-1, texts[3].Style);
        }

        [TestMethod()]
        public void Hydrate()
        {
            var tokens = new List<string>();
            AbbreviationLabel.Parse("•H2O", tokens);
            Assert.IsTrue(tokens.SequenceEqual(new string[] { "•", "H2", "O" }));
        }

        [TestMethod()]
        public void Het()
        {
            // 'Het' not 'He'lium and 't'erts
            Assert.IsFalse(AbbreviationLabel.Parse("Het", new List<string>()));
        }

        [TestMethod()]
        public void ParseChargeOnly()
        {
            Assert.IsFalse(AbbreviationLabel.Parse("+", new List<string>()));
        }

        [TestMethod()]
        public void ParseNumberOnly()
        {
            Assert.IsFalse(AbbreviationLabel.Parse("1", new List<string>()));
        }

        [TestMethod()]
        public void NonAsciiLabel()
        {
            // phenyl
            Assert.IsFalse(AbbreviationLabel.Parse("苯基", new List<string>()));
        }
    }
}
