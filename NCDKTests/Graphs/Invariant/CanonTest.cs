/*
 * Copyright (c) 2013 European Bioinformatics Institute (EMBL-EBI)
 *                    John May <jwmay@users.sf.net>
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
 * Any WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */
using NCDK.Common.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Smiles;
using static NCDK.Graphs.GraphUtil;

namespace NCDK.Graphs.Invariant
{
    /**
	 * @author John May
	 * @cdk.module test-standard
	 */
    [TestClass()]
    public class CanonTest
    {
        [TestMethod()]
        public void Phenol_symmetry()
        {
            IAtomContainer m = Smi("OC1=CC=CC=C1");
            long[] symmetry = Canon.Symmetry(m, GraphUtil.ToAdjList(m));
            Assert.IsTrue(Compares.AreEqual(new long[] { 1, 7, 5, 3, 2, 3, 5 }, symmetry));
        }

        [TestMethod()]
        public void Phenol_labelling()
        {
            IAtomContainer m = Smi("OC1=CC=CC=C1");
            long[] labels = Canon.Label(m, GraphUtil.ToAdjList(m));
            Assert.IsTrue(Compares.AreEqual(new long[] { 1, 7, 5, 3, 2, 4, 6 }, labels));
        }

        /**
		 * Ensure we consider the previous rank when we shatter ranks. This molecule
		 * has a carbons/sulphurs which experience the same environment. We must
		 * consider that they are different (due to their initial label) but not
		 * their environment.
		 *
		 * @cdk.inchi InChI=1/C2H4S5/c1-3-4-2-6-7-5-1/h1-2H2
		 */
        [TestMethod()]
        public void Lenthionine_symmetry()
        {
            IAtomContainer m = Smi("C1SSCSSS1");
            long[] labels = Canon.Symmetry(m, GraphUtil.ToAdjList(m));
            Assert.IsTrue(Compares.AreEqual(new long[] { 6, 4, 4, 6, 2, 1, 2 }, labels));
        }

        [TestMethod()]
        public void TestBasicInvariants_ethanol()
        {
            IAtomContainer m = Smi("CCO");
            long[] exp = new long[] { 1065731, 1082114, 541697 };
            long[] act = Canon.basicInvariants(m, ToAdjList(m));
            Assert.IsTrue(Compares.AreEqual(exp, act));
        }

        [TestMethod()]
        public void TestBasicInvariants_phenol()
        {
            IAtomContainer m = Smi("OC1=CC=CC=C1");
            long[] exp = new long[] { 541697, 836352, 819969, 819969, 819969, 819969, 819969 };
            long[] act = Canon.basicInvariants(m, ToAdjList(m));
            Assert.IsTrue(Compares.AreEqual(exp, act));
        }

        [TestMethod()]
        public void terminalExplicitHydrogensAreNotIncluded()
        {
            IAtomContainer m = Smi("C/C=C(/C)C[H]");
            bool[] mask = Canon.terminalHydrogens(m, GraphUtil.ToAdjList(m));
            Assert.IsTrue(Compares.AreEqual(new bool[] { false, false, false, false, false, true }, mask));
        }

        [TestMethod()]
        public void bridgingExplicitHydrogensAreIncluded()
        {
            IAtomContainer m = Smi("B1[H]B[H]1");
            bool[] mask = Canon.terminalHydrogens(m, GraphUtil.ToAdjList(m));
            Assert.IsTrue(Compares.AreEqual(new bool[] { false, false, false, false }, mask));
        }

        [TestMethod()]
        public void ExplicitHydrogensIonsAreIncluded()
        {
            IAtomContainer m = Smi("[H+]");
            bool[] mask = Canon.terminalHydrogens(m, GraphUtil.ToAdjList(m));
            Assert.IsTrue(Compares.AreEqual(new bool[] { false }, mask));
        }

        [TestMethod()]
        public void MolecularHydrogensAreNotIncluded()
        {
            IAtomContainer m = Smi("[H][H]");
            bool[] mask = Canon.terminalHydrogens(m, GraphUtil.ToAdjList(m));
            Assert.IsTrue(Compares.AreEqual(new bool[] { true, true }, mask));
        }

        [TestMethod()]
        public void ExplicitHydrogensOfEthanolHaveSymmetry()
        {
            IAtomContainer m = Smi("C([H])([H])C([H])([H])O");
            long[] symmetry = Canon.Symmetry(m, GraphUtil.ToAdjList(m));
            Assert.IsTrue(Compares.AreEqual(new long[] { 6, 1, 1, 7, 3, 3, 5 }, symmetry));
        }

        [TestMethod()]
        public void ExplicitHydrogensDoNotAffectHeavySymmetry()
        {
            IAtomContainer m = Smi("CC=C(C)C[H]");
            long[] symmetry = Canon.Symmetry(m, GraphUtil.ToAdjList(m));
            Assert.IsTrue(Compares.AreEqual(new long[] { 4, 2, 3, 5, 5, 1 }, symmetry));
        }

        [TestMethod()]
        public void CanonicallyLabelEthaneWithInConsistentHydrogenRepresentation()
        {
            IAtomContainer m = Smi("CC[H]");
            long[] labels = Canon.Label(m, GraphUtil.ToAdjList(m));
            Assert.IsTrue(Compares.AreEqual(new long[] { 2, 3, 1 }, labels));
        }

        [TestMethod()]
        public void CanonicallyLabelEthaneWithInConsistentHydrogenRepresentation2()
        {
            IAtomContainer m = Smi("CC([H])([H])");
            long[] labels = Canon.Label(m, GraphUtil.ToAdjList(m));
            Assert.IsTrue(Compares.AreEqual(new long[] { 3, 4, 1, 2 }, labels));
        }

        [TestMethod()]
        public void CanonicallyLabelCaffeineWithExplicitHydrogenRepresentation()
        {
            IAtomContainer m = Smi("[H]C1=NC2=C(N1C([H])([H])[H])C(=O)N(C(=O)N2C([H])([H])[H])C([H])([H])[H]");
            long[] labels = Canon.Label(m, GraphUtil.ToAdjList(m));
            Assert.IsTrue(Compares.AreEqual(
                new long[] { 1, 14, 13, 16, 18, 19, 22, 2, 3, 4, 15, 11, 20, 17, 12, 21, 24, 8, 9, 10, 23, 5, 6, 7 },
                labels));
        }

        static readonly SmilesParser sp = new SmilesParser(Silent.ChemObjectBuilder.Instance);

        static IAtomContainer Smi(string smi)
        {
            return sp.ParseSmiles(smi);
        }
    }
}
