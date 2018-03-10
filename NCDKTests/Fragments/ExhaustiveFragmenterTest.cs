/*
 * Copyright (C) 2010 Rajarshi Guha <rajarshi.guha@gmail.com>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Common.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Smiles;
using System.Linq;

namespace NCDK.Fragments
{
    /// <summary>
    /// Test exhaustive fragmenter.
    /// </summary>
    // @cdk.module test-fragment
    [TestClass()]
    public class ExhaustiveFragmenterTest : CDKTestCase
    {
        static ExhaustiveFragmenter fragmenter;
        static SmilesParser smilesParser;

        static ExhaustiveFragmenterTest()
        {
            fragmenter = new ExhaustiveFragmenter();
            smilesParser = new SmilesParser(Silent.ChemObjectBuilder.Instance);
        }

        [TestMethod()]
        public void TestEF1()
        {
            IAtomContainer mol = smilesParser.ParseSmiles("CCC");
            fragmenter.GenerateFragments(mol);
            var frags = fragmenter.GetFragments();
            Assert.AreEqual(0, frags.Count());
        }

        [TestMethod()]
        public void TestEF2()
        {
            IAtomContainer mol = smilesParser.ParseSmiles("C1CCCC1");
            fragmenter.GenerateFragments(mol);
            var frags = fragmenter.GetFragments();
            Assert.AreEqual(0, frags.Count());
        }

        [TestMethod()]
        public void TestEF3()
        {
            IAtomContainer mol = smilesParser.ParseSmiles("C1CCCCC1CC");
            fragmenter.GenerateFragments(mol);
            var frags = fragmenter.GetFragments();
            Assert.IsTrue(Compares.AreDeepEqual(new string[] { "C1CCCCC1" }, frags));
        }

        [TestMethod()]
        public void TestEF4()
        {
            IAtomContainer mol = smilesParser.ParseSmiles("c1ccccc1CC");
            fragmenter.GenerateFragments(mol);
            var frags = fragmenter.GetFragments();
            Assert.IsNotNull(frags);
            Assert.IsTrue(Compares.AreDeepEqual(new string[] { "c1ccccc1" }, frags));
        }

        [TestMethod()]
        public void TestEF5()
        {
            IAtomContainer mol = smilesParser.ParseSmiles("c1ccccc1Cc1ccccc1");
            fragmenter.GenerateFragments(mol);
            var frags = fragmenter.GetFragments();
            Assert.IsNotNull(frags);
            foreach (var s in new[] { "c1ccc(cc1)C", "c1ccccc1" })
                Assert.IsTrue(frags.Contains(s));
            Assert.IsNotNull(fragmenter.GetFragmentsAsContainers());
            Assert.AreEqual(2, fragmenter.GetFragmentsAsContainers().Count());
        }

        [TestMethod()]
        public void TestEF6()
        {
            IAtomContainer mol = smilesParser.ParseSmiles("c1ccccc1c1ccccc1");
            fragmenter.GenerateFragments(mol);
            var frags = fragmenter.GetFragments();
            Assert.IsNotNull(frags);
            Assert.IsTrue(Compares.AreDeepEqual(new string[] { "c1ccccc1" }, frags));

            Assert.IsNotNull(fragmenter.GetFragmentsAsContainers());
            Assert.AreEqual(1, fragmenter.GetFragmentsAsContainers().Count());
        }

        [TestMethod()]
        public void TestEF7()
        {
            IAtomContainer mol = smilesParser.ParseSmiles("C1(c2ccccc2)(CC(CC1)CCc1ccccc1)CC1C=CC=C1");
            fragmenter.GenerateFragments(mol);
            var frags = fragmenter.GetFragments().ToList();
            Assert.IsNotNull(frags);
            Assert.AreEqual(25, frags.Count);

            Assert.IsNotNull(fragmenter.GetFragmentsAsContainers());
            Assert.AreEqual(25, fragmenter.GetFragmentsAsContainers().Count());

            foreach (var s in new[] { "c1ccccc1", "c1ccc(cc1)C2(CCC(CC)C2)CC3C=CC=C3", "c1ccc(cc1)C2(C)CCC(C)C2" })
            {
                Assert.IsTrue(frags.Contains(s));
            }
        }

        [TestMethod()]
        public void TestMinSize()
        {
            IAtomContainer mol = smilesParser.ParseSmiles("C1CCCC1C2CCCCC2");
            fragmenter.MinimumFragmentSize = 6;
            fragmenter.GenerateFragments(mol);
            var frags = fragmenter.GetFragments();
            Assert.IsNotNull(frags);
            Assert.AreEqual(1, frags.Count());
            Assert.IsTrue(frags.First().Equals("C1CCCCC1"));
        }
    }
}
