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

using NCDK.Beam;
using NCDK.Common.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NCDK.Stereo;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NCDK.Smiles
{
    // @author John May
    // @cdk.module test-smiles
    [TestClass()]
    public class BeamToCDKTest
    {
        private static IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;
        private static BeamToCDK g2c = new BeamToCDK(builder);

        [TestMethod()]
        public void NewUnknownAtom()
        {
            IAtom a = g2c.NewCDKAtom(AtomBuilder.Aliphatic(Element.Unknown).Build());
            Assert.IsInstanceOfType(a, typeof(IPseudoAtom));
            Assert.AreEqual("*", ((IPseudoAtom)a).Label);
        }

        [TestMethod()]
        public void NewCarbonAtom()
        {
            IAtom a = g2c.NewCDKAtom(AtomBuilder.Aliphatic(Element.Carbon).Build());
            Assert.IsInstanceOfType(a, typeof(IAtom));
            Assert.IsNotInstanceOfType(a, typeof(IPseudoAtom));
            Assert.AreEqual("C", a.Symbol);
        }

        [TestMethod()]
        public void NewNitrogenAtom()
        {
            IAtom a = g2c.NewCDKAtom(AtomBuilder.Aliphatic(Element.Nitrogen).Build());
            Assert.IsInstanceOfType(a, typeof(IAtom));
            Assert.IsNotInstanceOfType(a, typeof(IPseudoAtom));
            Assert.AreEqual("N", a.Symbol);
        }

        [TestMethod()]
        public void MethaneAtom()
        {
            IAtom a = g2c.ToCDKAtom(AtomBuilder.Aliphatic(Element.Carbon).NumOfHydrogens(4).Build(), 4);
            Assert.AreEqual("C", a.Symbol);
            Assert.AreEqual(4, a.ImplicitHydrogenCount);
        }

        [TestMethod()]
        public void WaterAtom()
        {
            IAtom a = g2c.ToCDKAtom(AtomBuilder.Aliphatic(Element.Oxygen).NumOfHydrogens(2).Build(), 2);
            Assert.AreEqual("O", a.Symbol);
            Assert.AreEqual(2, a.ImplicitHydrogenCount);
        }

        [TestMethod()]
        public void Oxidanide()
        {
            IAtom a = g2c.ToCDKAtom(AtomBuilder.Aliphatic(Element.Oxygen).NumOfHydrogens(1).Anion.Build(), 1);
            Assert.AreEqual("O", a.Symbol);
            Assert.AreEqual(1, a.ImplicitHydrogenCount);
            Assert.AreEqual(-1, a.FormalCharge);
        }

        [TestMethod()]
        public void AzaniumAtom()
        {
            IAtom a = g2c.ToCDKAtom(AtomBuilder.Aliphatic(Element.Nitrogen).NumOfHydrogens(4).Cation.Build(), 4);
            Assert.AreEqual("N", a.Symbol);
            Assert.AreEqual(4, a.ImplicitHydrogenCount);
            Assert.AreEqual(+1, a.FormalCharge);
        }

        [TestMethod()]
        public void UnspecifiedMass()
        {
            IAtom a = g2c.ToCDKAtom(AtomBuilder.Aliphatic(Element.Carbon).NumOfHydrogens(4).Build(), 4);
            Assert.IsNull(a.MassNumber);
        }

        [TestMethod()]
        public void Carbon_12()
        {
            IAtom a = g2c.ToCDKAtom(AtomBuilder.Aliphatic(Element.Carbon).NumOfHydrogens(4).Isotope(12).Build(), 4);
            Assert.AreEqual(12, a.MassNumber);
        }

        [TestMethod()]
        public void Carbon_13()
        {
            IAtom a = g2c.ToCDKAtom(AtomBuilder.Aliphatic(Element.Carbon).NumOfHydrogens(4).Isotope(13).Build(), 4);
            Assert.AreEqual(13, a.MassNumber);
        }

        [TestMethod()]
        public void Carbon_14()
        {
            IAtom a = g2c.ToCDKAtom(AtomBuilder.Aliphatic(Element.Carbon).NumOfHydrogens(4).Isotope(14).Build(), 4);
            Assert.AreEqual(14, a.MassNumber);
        }

        [TestMethod()]
        public void Aromatic()
        {
            IAtom a = g2c.ToCDKAtom(AtomBuilder.Aromatic(Element.Carbon).Build(), 0);
            Assert.IsTrue(a.IsAromatic);
        }

        [TestMethod()]
        public void Benzene()
        {
            IAtomContainer ac = Convert("c1ccccc1");
            Assert.AreEqual(6, ac.Atoms.Count);
            Assert.AreEqual(6, ac.Bonds.Count);
            foreach (var a in ac.Atoms)
            {
                Assert.AreEqual("C", a.Symbol);
                Assert.IsTrue(a.IsAromatic);
                Assert.AreEqual(1, a.ImplicitHydrogenCount);
            }
            foreach (var b in ac.Bonds)
            {
                Assert.AreEqual(BondOrder.Unset, b.Order);
                Assert.IsTrue(b.IsAromatic);
            }
        }

        [TestMethod()]
        public void Benzene_kekule()
        {
            IAtomContainer ac = Convert("C=1C=CC=CC1");
            Assert.AreEqual(6, ac.Atoms.Count);
            Assert.AreEqual(6, ac.Bonds.Count);
            foreach (var a in ac.Atoms)
            {
                Assert.AreEqual("C", a.Symbol);
                Assert.AreEqual(1, a.ImplicitHydrogenCount);
            }

            Assert.AreEqual(BondOrder.Single, ac.GetBond(ac.Atoms[0], ac.Atoms[1]).Order);
            Assert.AreEqual(BondOrder.Double, ac.GetBond(ac.Atoms[1], ac.Atoms[2]).Order);
            Assert.AreEqual(BondOrder.Single, ac.GetBond(ac.Atoms[2], ac.Atoms[3]).Order);
            Assert.AreEqual(BondOrder.Double, ac.GetBond(ac.Atoms[3], ac.Atoms[4]).Order);
            Assert.AreEqual(BondOrder.Single, ac.GetBond(ac.Atoms[4], ac.Atoms[5]).Order);
            Assert.AreEqual(BondOrder.Double, ac.GetBond(ac.Atoms[5], ac.Atoms[0]).Order);

            Assert.IsFalse(ac.Bonds[0].IsAromatic);
            Assert.IsFalse(ac.Bonds[1].IsAromatic);
            Assert.IsFalse(ac.Bonds[2].IsAromatic);
            Assert.IsFalse(ac.Bonds[3].IsAromatic);
            Assert.IsFalse(ac.Bonds[4].IsAromatic);
            Assert.IsFalse(ac.Bonds[5].IsAromatic);
        }

        [TestMethod()]
        public void Imidazole()
        {

            IAtomContainer ac = Convert("c1[nH]cnc1");
            Assert.AreEqual(5, ac.Atoms.Count);
            Assert.AreEqual(5, ac.Bonds.Count);

            foreach (var a in ac.Atoms)
                Assert.IsTrue(a.IsAromatic);

            Assert.AreEqual("C", ac.Atoms[0].Symbol);
            Assert.AreEqual("N", ac.Atoms[1].Symbol);
            Assert.AreEqual("C", ac.Atoms[2].Symbol);
            Assert.AreEqual("N", ac.Atoms[3].Symbol);
            Assert.AreEqual("C", ac.Atoms[4].Symbol);

            Assert.AreEqual(1, ac.Atoms[0].ImplicitHydrogenCount);
            Assert.AreEqual(1, ac.Atoms[1].ImplicitHydrogenCount);
            Assert.AreEqual(1, ac.Atoms[2].ImplicitHydrogenCount);
            Assert.AreEqual(0, ac.Atoms[3].ImplicitHydrogenCount);
            Assert.AreEqual(1, ac.Atoms[4].ImplicitHydrogenCount);

            foreach (var a in ac.Atoms)
            {
                Assert.IsTrue(a.IsAromatic);
            }

            foreach (var b in ac.Bonds)
            {
                Assert.AreEqual(BondOrder.Unset, b.Order);
                Assert.IsTrue(b.IsAromatic);
            }
        }

        [TestMethod()]
        public void Imidazole_kekule()
        {

            IAtomContainer ac = Convert("N1C=CN=C1");
            Assert.AreEqual(5, ac.Atoms.Count);
            Assert.AreEqual(5, ac.Bonds.Count);

            foreach (var a in ac.Atoms)
                Assert.IsFalse(a.IsAromatic);

            Assert.AreEqual("N", ac.Atoms[0].Symbol);
            Assert.AreEqual("C", ac.Atoms[1].Symbol);
            Assert.AreEqual("C", ac.Atoms[2].Symbol);
            Assert.AreEqual("N", ac.Atoms[3].Symbol);
            Assert.AreEqual("C", ac.Atoms[4].Symbol);

            Assert.AreEqual(1, ac.Atoms[0].ImplicitHydrogenCount);
            Assert.AreEqual(1, ac.Atoms[1].ImplicitHydrogenCount);
            Assert.AreEqual(1, ac.Atoms[2].ImplicitHydrogenCount);
            Assert.AreEqual(0, ac.Atoms[3].ImplicitHydrogenCount);
            Assert.AreEqual(1, ac.Atoms[4].ImplicitHydrogenCount);

            foreach (var a in ac.Atoms)
            {
                Assert.IsFalse(a.IsAromatic);
            }

            Assert.AreEqual(BondOrder.Single, ac.GetBond(ac.Atoms[0], ac.Atoms[1]).Order);
            Assert.AreEqual(BondOrder.Double, ac.GetBond(ac.Atoms[1], ac.Atoms[2]).Order);
            Assert.AreEqual(BondOrder.Single, ac.GetBond(ac.Atoms[2], ac.Atoms[3]).Order);
            Assert.AreEqual(BondOrder.Double, ac.GetBond(ac.Atoms[3], ac.Atoms[4]).Order);
            Assert.AreEqual(BondOrder.Single, ac.GetBond(ac.Atoms[4], ac.Atoms[0]).Order);

            foreach (var b in ac.Bonds)
            {
                Assert.IsFalse(b.IsAromatic);
            }
        }

        /// <summary>
        /// (2R)-butan-2-ol
        /// </summary>
        // @cdk.inchi InChI=1/C4H10O/c1-3-4(2)5/h4-5H,3H2,1-2H3/t4-/s2
        [TestMethod()]
        public void Test_2R_butan_2_ol()
        {
            IAtomContainer ac = Convert("CC[C@@](C)(O)[H]");

            IStereoElement se = ac.StereoElements.First();

            Assert.IsInstanceOfType(se, typeof(ITetrahedralChirality));

            ITetrahedralChirality tc = (ITetrahedralChirality)se;

            Assert.AreEqual(ac.Atoms[2], tc.ChiralAtom);
            Assert.IsTrue(Compares.AreEqual(new IAtom[] { ac.Atoms[1], ac.Atoms[3], ac.Atoms[4], ac.Atoms[5] }, tc.Ligands));
            Assert.AreEqual(TetrahedralStereo.Clockwise, tc.Stereo);
        }

        /// <summary>
        /// (2S)-butan-2-ol
        /// </summary>
        // @cdk.inchi InChI=1/C4H10O/c1-3-4(2)5/h4-5H,3H2,1-2H3/t4-/s2
        [TestMethod()]
        public void Test_2S_butan_2_ol()
        {
            IAtomContainer ac = Convert("CC[C@](C)(O)[H]");

            IStereoElement se = ac.StereoElements.First();

            Assert.IsInstanceOfType(se, typeof(ITetrahedralChirality));

            ITetrahedralChirality tc = (ITetrahedralChirality)se;

            Assert.AreEqual(ac.Atoms[2], tc.ChiralAtom);
            Assert.IsTrue(Compares.AreEqual(new IAtom[] { ac.Atoms[1], ac.Atoms[3], ac.Atoms[4], ac.Atoms[5] }, tc.Ligands));
            Assert.AreEqual(TetrahedralStereo.AntiClockwise, tc.Stereo);
        }

        /// <summary>
        /// (4as,8as)-decahydronaphthalene-4a,8a-diol
        /// </summary>
        // @cdk.inchi InChI=1/C10H18O2/c11-9-5-1-2-6-10(9,12)8-4-3-7-9/h11-12H,1-8H2/t9-,10+
        [TestMethod()]
        public void TetrahedralRingClosure()
        {
            IAtomContainer ac = Convert("O[C@]12CCCC[C@@]1(O)CCCC2");

            IStereoElement[] ses = ac.StereoElements.ToArray();

            Assert.AreEqual(2, ses.Length);
            Assert.IsInstanceOfType(ses[0], typeof(ITetrahedralChirality));
            Assert.IsInstanceOfType(ses[1], typeof(ITetrahedralChirality));

            ITetrahedralChirality tc1 = (ITetrahedralChirality)ses[0];
            ITetrahedralChirality tc2 = (ITetrahedralChirality)ses[1];

            // we want the second atom stereo as tc1
            if (ac.Atoms.IndexOf(tc1.ChiralAtom) > ac.Atoms.IndexOf(tc2.ChiralAtom))
            {
                ITetrahedralChirality swap = tc1;
                tc1 = tc2;
                tc2 = swap;
            }

            Assert.AreEqual(ac.Atoms[1], tc1.ChiralAtom);
            Assert.IsTrue(Compares.AreEqual(new IAtom[] { ac.Atoms[0], ac.Atoms[2], ac.Atoms[6], ac.Atoms[11] }, tc1.Ligands));
            Assert.AreEqual(TetrahedralStereo.AntiClockwise, tc1.Stereo);

            // the configuration around atom 6 flips as the ring closure '[C@@]1'
            // is the first atom (when ordered) but not in the configuration - when
            // we order the atoms by their index the tetrahedral configuration goes
            // from clockwise in the SMILES to anti-clockwise ('@'). Writing out the
            // SMILES again one can see it will flip back clockwise ('@@').

            Assert.AreEqual(ac.Atoms[6], tc2.ChiralAtom);
            Assert.IsTrue(Compares.AreEqual(new IAtom[] { ac.Atoms[1], ac.Atoms[5], ac.Atoms[7], ac.Atoms[8] }, tc2.Ligands));
            Assert.AreEqual(TetrahedralStereo.AntiClockwise, tc2.Stereo);
        }

        /// <summary>
        /// (E)-1,2-difluoroethene
        /// </summary>
        // @cdk.inchi InChI=1/C2H2F2/c3-1-2-4/h1-2H/b2-1+
        [TestMethod()]
        public void E_1_2_difluroethene()
        {
            IAtomContainer ac = Convert("F/C=C/F");

            IStereoElement se = ac.StereoElements.First();

            Assert.IsInstanceOfType(se, typeof(IDoubleBondStereochemistry));

            IDoubleBondStereochemistry dbs = (IDoubleBondStereochemistry)se;
            Assert.AreEqual(ac.GetBond(ac.Atoms[1], ac.Atoms[2]), dbs.StereoBond);
            Assert.IsTrue(Compares.AreDeepEqual(new IBond[] { ac.GetBond(ac.Atoms[0], ac.Atoms[1]), ac.GetBond(ac.Atoms[2], ac.Atoms[3]) }, dbs.Bonds));
            Assert.AreEqual(DoubleBondConformation.Opposite, dbs.Stereo);
        }

        /// <summary>
        /// (Z)-1,2-difluoroethene
        /// </summary>
        // @cdk.inchi InChI=1/C2H2F2/c3-1-2-4/h1-2H/b2-1+
        [TestMethod()]
        public void Z_1_2_difluroethene()
        {

            IAtomContainer ac = Convert("F/C=C\\F");

            IStereoElement se = ac.StereoElements.First();

            Assert.IsInstanceOfType(se, typeof(IDoubleBondStereochemistry));

            IDoubleBondStereochemistry dbs = (IDoubleBondStereochemistry)se;
            Assert.AreEqual(ac.GetBond(ac.Atoms[1], ac.Atoms[2]), dbs.StereoBond);
            Assert.IsTrue(Compares.AreDeepEqual(new IBond[] { ac.GetBond(ac.Atoms[0], ac.Atoms[1]), ac.GetBond(ac.Atoms[2], ac.Atoms[3]) }, dbs.Bonds));
            Assert.AreEqual(DoubleBondConformation.Together, dbs.Stereo);
        }

        /// <summary>
        /// (E)-1,2-difluoroethene
        /// </summary>
        // @cdk.inchi InChI=1/C2H2F2/c3-1-2-4/h1-2H/b2-1+
        [TestMethod()]
        public void E_1_2_difluroethene_explicit()
        {
            IAtomContainer ac = Convert("F/C([H])=C(\\[H])F");

            IStereoElement se = ac.StereoElements.First();

            Assert.IsInstanceOfType(se, typeof(IDoubleBondStereochemistry));

            IDoubleBondStereochemistry dbs = (IDoubleBondStereochemistry)se;
            Assert.AreEqual(ac.GetBond(ac.Atoms[1], ac.Atoms[3]), dbs.StereoBond);
            Assert.IsTrue(Compares.AreDeepEqual(new IBond[] { ac.GetBond(ac.Atoms[0], ac.Atoms[1]), ac.GetBond(ac.Atoms[3], ac.Atoms[4]) }, dbs.Bonds));
            // the two 'F' are opposite but we use a H so they are 'together'
            Assert.AreEqual(DoubleBondConformation.Together, dbs.Stereo);
        }

        /// <summary>
        /// (Z)-1,2-difluoroethene
        /// </summary>
        // @cdk.inchi InChI=1/C2H2F2/c3-1-2-4/h1-2H/b2-1-
        [TestMethod()]
        public void Z_1_2_difluroethene_explicit()
        {
            IAtomContainer ac = Convert("FC(\\[H])=C([H])/F");

            IStereoElement se = ac.StereoElements.First();

            Assert.IsInstanceOfType(se, typeof(IDoubleBondStereochemistry));

            IDoubleBondStereochemistry dbs = (IDoubleBondStereochemistry)se;
            Assert.AreEqual(ac.GetBond(ac.Atoms[1], ac.Atoms[3]), dbs.StereoBond);
            Assert.IsTrue(Compares.AreDeepEqual(new IBond[] { ac.GetBond(ac.Atoms[1], ac.Atoms[2]), ac.GetBond(ac.Atoms[3], ac.Atoms[5]) }, dbs.Bonds));
            // the two 'F' are together but we use a H so they are 'opposite'
            Assert.AreEqual(DoubleBondConformation.Opposite, dbs.Stereo);
        }

        [TestMethod()]
        public void ReadAtomClass()
        {
            IAtomContainer ac = Convert("CC[C:2]C");
            object actual = ac.Atoms[2].GetProperty<int>(CDKPropertyName.AtomAtomMapping);
            Assert.IsNotNull(actual);
            Assert.AreEqual(2, (int)actual);
        }

        [TestMethod()]
        public void ErroneousLabels_tRNA()
        {
            IAtomContainer ac = Convert("[tRNA]CC");
            Assert.AreEqual("*", ac.Atoms[0].Symbol);
            Assert.IsInstanceOfType(ac.Atoms[0], typeof(IPseudoAtom));
            Assert.AreEqual("tRNA", ((IPseudoAtom)ac.Atoms[0]).Label);
        }

        // believe it or not there are cases of this in the wild -checkout some
        // acyl-carrier-protein SMILES in MetaCyc
        [TestMethod()]
        public void ErroneousLabels_nested()
        {
            IAtomContainer ac = Convert("[now-[this]-is-mean]CC");
            Assert.AreEqual("*", ac.Atoms[0].Symbol);
            Assert.IsInstanceOfType(ac.Atoms[0], typeof(IPseudoAtom));

            Assert.AreEqual("now-[this]-is-mean", ((IPseudoAtom)ac.Atoms[0]).Label);
        }

        [TestMethod()]
        [ExpectedException(typeof(IOException), AllowDerivedTypes = true)]
        public void ErroneousLabels_bad1()
        {
            Convert("[this]-is-not-okay]CC");
        }

        [TestMethod()]
        [ExpectedException(typeof(IOException), AllowDerivedTypes = true)]
        public void ErroneousLabels_bad2()
        {
            Convert("[this-[is-not-okay]CC");
        }

        [TestMethod()]
        [ExpectedException(typeof(IOException), AllowDerivedTypes = true)]
        public void ErroneousLabels_bad3()
        {
            Convert("[this-[is]-not]-okay]CC");
        }

        [TestMethod()]
        public void ExtendedTetrahedral_ccw()
        {
            IAtomContainer ac = Convert("CC=[C@]=CC");
            IEnumerator<IStereoElement> elements = ac.StereoElements.GetEnumerator();
            Assert.IsTrue(elements.MoveNext());
            IStereoElement element = elements.Current;
            Assert.IsInstanceOfType(element, typeof(ExtendedTetrahedral));
            ExtendedTetrahedral extendedTetrahedral = (ExtendedTetrahedral)element;
            Assert.AreEqual(TetrahedralStereo.AntiClockwise, extendedTetrahedral.Winding);
            Assert.AreEqual(ac.Atoms[2], extendedTetrahedral.Focus);
            Assert.IsTrue(Compares.AreEqual(
                new IAtom[] { ac.Atoms[0], ac.Atoms[1], ac.Atoms[3], ac.Atoms[4] },
                extendedTetrahedral.Peripherals));
        }

        [TestMethod()]
        public void ExtendedTetrahedral_cw()
        {
            IAtomContainer ac = Convert("CC=[C@@]=CC");
            IEnumerator<IStereoElement> elements = ac.StereoElements.GetEnumerator();
            Assert.IsTrue(elements.MoveNext());
            IStereoElement element = elements.Current;
            Assert.IsInstanceOfType(element, typeof(ExtendedTetrahedral));
            ExtendedTetrahedral extendedTetrahedral = (ExtendedTetrahedral)element;
            Assert.AreEqual(TetrahedralStereo.Clockwise, extendedTetrahedral.Winding);
            Assert.AreEqual(ac.Atoms[2], extendedTetrahedral.Focus);
            Assert.IsTrue(Compares.AreEqual(
                new IAtom[] { ac.Atoms[0], ac.Atoms[1], ac.Atoms[3], ac.Atoms[4] },
                extendedTetrahedral.Peripherals));
        }

        [TestMethod()]
        public void TitleWithTab()
        {
            Assert.AreEqual(Convert("CN1C=NC2=C1C(=O)N(C(=O)N2C)C\tcaffeine").GetProperty<string>(CDKPropertyName.Title),
                         "caffeine");
        }

        [TestMethod()]
        public void TitleWithSpace()
        {
            Assert.AreEqual(Convert("CN1C=NC2=C1C(=O)N(C(=O)N2C)C caffeine").GetProperty<string>(CDKPropertyName.Title),
                 "caffeine");
        }

        [TestMethod()]
        public void TitleWithMultipleSpace()
        {
            Assert.AreEqual(Convert("CN1C=NC2=C1C(=O)N(C(=O)N2C)C caffeine compound").GetProperty<string>(CDKPropertyName.Title),
                 "caffeine compound");
        }

        IAtomContainer Convert(string smi)
        {
            BeamToCDK g2c = new BeamToCDK(Silent.ChemObjectBuilder.Instance);
            Graph g = Graph.FromSmiles(smi);
            return g2c.ToAtomContainer(g, false);
        }
    }
}
