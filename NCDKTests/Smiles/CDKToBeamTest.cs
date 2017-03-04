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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Moq;
using NCDK.Stereo;
using NCDK.Templates;
using NCDK.Tools.Manipulator;
using NCDK.Tools;
using NCDK.Aromaticities;

namespace NCDK.Smiles
{
    /// <summary>
    /// Unit tests for converting CDK IAtomContainer's to the grins object module.
    /// For clarity often the SMILES output is verified if a test fails it could be
    /// the Grins output changed and there was not a problem with the conversion.
    ///
    // @author John May
    // @cdk.module test-smiles
    /// </summary>
    [TestClass()]
    public class CDKToBeamTest
    {

        [TestMethod()]
        [ExpectedException(typeof(NullReferenceException))]
        public void NoImplicitHCount()
        {
            new CDKToBeam().ToBeamAtom(new Default.Atom("C"));
        }

        [TestMethod()]
        [ExpectedException(typeof(NullReferenceException))]
        public void NoSymbol()
        {
            new CDKToBeam().ToBeamAtom(new Default.Atom());
        }

        [TestMethod()]
        public void UnknownSymbol()
        {
            IAtom a = new Default.Atom("ALA");
            a.ImplicitHydrogenCount = 0;
            Assert.AreEqual(Beam.Element.Unknown, new CDKToBeam().ToBeamAtom(a).Element);
        }

        [TestMethod()]
        public void UnknownSymbol_Pseudo()
        {
            IAtom a = new Default.PseudoAtom("R1");
            a.ImplicitHydrogenCount = 0;
            Assert.AreEqual(Beam.Element.Unknown, new CDKToBeam().ToBeamAtom(a).Element);
        }

        [TestMethod()]
        public void Methane_Atom()
        {
            IAtom a = new Default.Atom("C");
            a.ImplicitHydrogenCount = 4;
            Assert.AreEqual(Beam.Element.Carbon, new CDKToBeam().ToBeamAtom(a).Element);
            Assert.AreEqual(4, new CDKToBeam().ToBeamAtom(a).NumOfHydrogens);
        }

        [TestMethod()]
        public void Water_Atom()
        {
            IAtom a = new Default.Atom("O");
            a.ImplicitHydrogenCount = 2;
            Assert.AreEqual(Beam.Element.Oxygen, new CDKToBeam().ToBeamAtom(a).Element);
            Assert.AreEqual(2, new CDKToBeam().ToBeamAtom(a).NumOfHydrogens);
        }

        [TestMethod()]
        public void ChargedAtom()
        {
            IAtom a = new Default.Atom("C");
            a.ImplicitHydrogenCount = 0;
            for (int chg = -10; chg < 10; chg++)
            {
                a.FormalCharge = chg;
                Assert.AreEqual(chg, new CDKToBeam().ToBeamAtom(a).Charge);
            }
        }

        [TestMethod()]
        public void AliphaticAtom()
        {
            IAtom a = new Default.Atom("C");
            a.ImplicitHydrogenCount = 0;
            Assert.IsFalse(new CDKToBeam().ToBeamAtom(a).IsAromatic());
        }

        [TestMethod()]
        public void AromaticAtom()
        {
            IAtom a = new Default.Atom("C");
            a.ImplicitHydrogenCount = 0;
            a.IsAromatic = true;
            Assert.IsTrue(new CDKToBeam().ToBeamAtom(a).IsAromatic());
        }

        [TestMethod()]
        public void UnspecifiedIsotope()
        {
            IAtom a = new Default.Atom("C");
            a.ImplicitHydrogenCount = 0;
            Assert.AreEqual(-1, new CDKToBeam().ToBeamAtom(a).Isotope);
        }

        [TestMethod()]
        public void SpecifiedIsotope()
        {
            IAtom a = new Default.Atom("C");
            a.ImplicitHydrogenCount = 0;
            a.MassNumber = 13;
            Assert.AreEqual(13, new CDKToBeam().ToBeamAtom(a).Isotope);
        }

        [TestMethod()]
        public void DefaultIsotope()
        {
            IAtom a = new Default.Atom("C");
            a.ImplicitHydrogenCount = 0;
            a.MassNumber = 12;
            Assert.AreEqual(-1, new CDKToBeam().ToBeamAtom(a).Isotope);
        }

        // special check that a CDK pseudo atom will default to 0 hydrogens if
        // the hydrogens are set to null
        [TestMethod()]
        public void PseuDoAtom_nullH()
        {
            Assert.AreEqual(0, new CDKToBeam().ToBeamAtom(new Default.Atom("R")).NumOfHydrogens);
            Assert.AreEqual(0, new CDKToBeam().ToBeamAtom(new Default.Atom("*")).NumOfHydrogens);
            Assert.AreEqual(0, new CDKToBeam().ToBeamAtom(new Default.Atom("R1")).NumOfHydrogens);
        }

        [TestMethod()]
        [ExpectedException(typeof(CDKException))]
        public void UnSetBondOrder()
        {
            var mock_u = new Mock<IAtom>(); var u = mock_u.Object;
            var mock_v = new Mock<IAtom>(); var v = mock_v.Object;
            IBond b = new Default.Bond(u, v, BondOrder.Unset);
            var mock = new Mock<IDictionary<IAtom, int>>();
            mock.SetupGet(n => n[u]).Returns(0);
            mock.SetupGet(n => n[v]).Returns(1);
            new CDKToBeam().ToBeamEdge(b, mock.Object);
        }

        [TestMethod()]
        [ExpectedException(typeof(CDKException))]
        public void UndefBondOrder()
        {
            var mock_u = new Mock<IAtom>(); var u = mock_u.Object;
            var mock_v = new Mock<IAtom>(); var v = mock_v.Object;
            IBond b = new Default.Bond(u, v, BondOrder.Unset);
            var mock = new Mock<IDictionary<IAtom, int>>();
            mock.SetupGet(n => n[u]).Returns(0);
            mock.SetupGet(n => n[v]).Returns(1);
            new CDKToBeam().ToBeamEdge(b, mock.Object);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TooFewAtoms()
        {
            IBond b = new Default.Bond(new IAtom[] { new Mock<IAtom>().Object });
            new CDKToBeam().ToBeamEdge(b, new Mock<IDictionary<IAtom, int>>().Object);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TooManyAtoms()
        {
            IBond b = new Default.Bond(new IAtom[] { new Mock<IAtom>().Object, new Mock<IAtom>().Object, new Mock<IAtom>().Object });
            new CDKToBeam().ToBeamEdge(b, new Mock<IDictionary<IAtom, int>>().Object);
        }

        [TestMethod()]
        public void SingleBond()
        {
            var mock_u = new Mock<IAtom>(); var u = mock_u.Object;
            var mock_v = new Mock<IAtom>(); var v = mock_v.Object;
            IBond b = new Default.Bond(u, v);
            var mock = new Mock<IDictionary<IAtom, int>>();
            mock.SetupGet(n => n[u]).Returns(0);
            mock.SetupGet(n => n[v]).Returns(1);
            CDKToBeam c2g = new CDKToBeam();
            Assert.AreEqual(Beam.Bond.Single.CreateEdge(0, 1), c2g.ToBeamEdge(b, mock.Object));
        }

        [TestMethod()]
        public void AromaticBond()
        {
            var mock_u = new Mock<IAtom>(); var u = mock_u.Object;
            var mock_v = new Mock<IAtom>(); var v = mock_v.Object;
            IBond b = new Default.Bond(u, v);
            b.IsAromatic = true;
            var mock = new Mock<IDictionary<IAtom, int>>();
            mock.SetupGet(n => n[u]).Returns(0);
            mock.SetupGet(n => n[v]).Returns(1);
            CDKToBeam c2g = new CDKToBeam();
            Assert.AreEqual(Beam.Bond.Aromatic.CreateEdge(0, 1), c2g.ToBeamEdge(b, mock.Object));
        }

        [TestMethod()]
        public void DoubleBond()
        {
            var mock_u = new Mock<IAtom>(); var u = mock_u.Object;
            var mock_v = new Mock<IAtom>(); var v = mock_v.Object;
            IBond b = new Default.Bond(u, v, BondOrder.Double);
            var mock = new Mock<IDictionary<IAtom, int>>();
            mock.SetupGet(n => n[u]).Returns(0);
            mock.SetupGet(n => n[v]).Returns(1);
            CDKToBeam c2g = new CDKToBeam();
            Assert.AreEqual(Beam.Bond.Double.CreateEdge(0, 1), c2g.ToBeamEdge(b, mock.Object));
        }

        [TestMethod()]
        public void TripleBond()
        {
            var mock_u = new Mock<IAtom>(); var u = mock_u.Object;
            var mock_v = new Mock<IAtom>(); var v = mock_v.Object;
            IBond b = new Default.Bond(u, v, BondOrder.Triple);
            var mock = new Mock<IDictionary<IAtom, int>>();
            mock.SetupGet(n => n[u]).Returns(0);
            mock.SetupGet(n => n[v]).Returns(1);
            CDKToBeam c2g = new CDKToBeam();
            Assert.AreEqual(Beam.Bond.Triple.CreateEdge(0, 1), c2g.ToBeamEdge(b, mock.Object));
        }


        [TestMethod()]
        public void QuadrupleBond()
        {
            var mock_u = new Mock<IAtom>(); var u = mock_u.Object;
            var mock_v = new Mock<IAtom>(); var v = mock_v.Object;
            IBond b = new Default.Bond(u, v, BondOrder.Quadruple);
            var mock = new Mock<IDictionary<IAtom, int>>();
            mock.SetupGet(n => n[u]).Returns(0);
            mock.SetupGet(n => n[v]).Returns(1);
            CDKToBeam c2g = new CDKToBeam();
            Assert.AreEqual(Beam.Bond.Quadruple.CreateEdge(0, 1), c2g.ToBeamEdge(b, mock.Object));
        }

        [TestMethod()]
        public void Adeneine()
        {
            Beam.Graph g = Convert(TestMoleculeFactory.MakeAdenine());
            Assert.AreEqual("C12=C(N=CN=C1N)NC=N2", g.ToSmiles());
        }

        [TestMethod()]
        public void Benzene_kekule()
        {
            Beam.Graph g = Convert(TestMoleculeFactory.MakeBenzene());
            Assert.AreEqual("C=1C=CC=CC1", g.ToSmiles());
        }

        [TestMethod()]
        public void Benzene()
        {
            IAtomContainer ac = TestMoleculeFactory.MakeBenzene();
            Beam.Graph g = Convert(ac, true, true);
            Assert.AreEqual("c1ccccc1", g.ToSmiles());
        }

        [TestMethod()]
        public void Imidazole_kekule()
        {
            Beam.Graph g = Convert(TestMoleculeFactory.MakeImidazole(), false, true);
            Assert.AreEqual("C=1NC=NC1", g.ToSmiles());
        }

        [TestMethod()]
        public void Imidazole()
        {
            Beam.Graph g = Convert(TestMoleculeFactory.MakeImidazole(), true, true);
            Assert.AreEqual("c1[nH]cnc1", g.ToSmiles());
        }

        [TestMethod()]
        public void Imidazole_ignoreAromatic()
        {
            Beam.Graph g = Convert(TestMoleculeFactory.MakeImidazole(), true, true, false, true);
            Assert.AreEqual("C=1NC=NC1", g.ToSmiles());
        }

        [TestMethod()]
        public void C13_Isomeric()
        {
            IAtomContainer ac = new Default.AtomContainer();
            IAtom a = new Default.Atom("C");
            a.MassNumber = 13;
            ac.Atoms.Add(a);
            Beam.Graph g = Convert(ac);
            Assert.AreEqual(13, g.GetAtom(0).Isotope);
            Assert.AreEqual("[13CH4]", g.ToSmiles());
        }

        [TestMethod()]
        public void C13_nonIsomeric()
        {
            IAtomContainer ac = new Default.AtomContainer();
            IAtom a = new Default.Atom("C");
            a.MassNumber = 13;
            ac.Atoms.Add(a);
            Beam.Graph g = Convert(ac, false, false); // non-isomeric
            Assert.AreEqual(-1, g.GetAtom(0).Isotope);
            Assert.AreEqual("C", g.ToSmiles());
        }

        [TestMethod()]
        public void Azanium()
        {
            IAtomContainer ac = new Default.AtomContainer();
            IAtom a = new Default.Atom("N");
            a.FormalCharge = +1;
            ac.Atoms.Add(a);
            Beam.Graph g = Convert(ac);
            Assert.AreEqual(+1, g.GetAtom(0).Charge);
            Assert.AreEqual("[NH4+]", g.ToSmiles());
        }

        [TestMethod()]
        public void Oxidanide()
        {
            IAtomContainer ac = new Default.AtomContainer();
            IAtom a = new Default.Atom("O");
            a.FormalCharge = -1;
            ac.Atoms.Add(a);
            Beam.Graph g = Convert(ac);
            Assert.AreEqual(-1, g.GetAtom(0).Charge);
            Assert.AreEqual("[OH-]", g.ToSmiles());
        }

        [TestMethod()]
        public void Oxidandiide()
        {
            IAtomContainer ac = new Default.AtomContainer();
            IAtom a = new Default.Atom("O");
            a.FormalCharge = -2;
            ac.Atoms.Add(a);
            Beam.Graph g = Convert(ac);
            Assert.AreEqual(-2, g.GetAtom(0).Charge);
            Assert.AreEqual("[O-2]", g.ToSmiles());
        }

        /// <summary>
        /// (E)-1,2-difluoroethene
        ///
        // @cdk.inchi InChI=1/C2H2F2/c3-1-2-4/h1-2H/b2-1+
        /// </summary>
        [TestMethod()]
        public void E_1_2_difluoroethene()
        {

            IAtomContainer ac = new Default.AtomContainer();
            ac.Atoms.Add(new Default.Atom("F"));
            ac.Atoms.Add(new Default.Atom("C"));
            ac.Atoms.Add(new Default.Atom("C"));
            ac.Atoms.Add(new Default.Atom("F"));
            ac.AddBond(ac.Atoms[0], ac.Atoms[1], BondOrder.Single);
            ac.AddBond(ac.Atoms[1], ac.Atoms[2], BondOrder.Double);
            ac.AddBond(ac.Atoms[2], ac.Atoms[3], BondOrder.Single);

            ac.AddStereoElement(new DoubleBondStereochemistry(ac.Bonds[1], new IBond[] { ac.Bonds[0], ac.Bonds[2] },
                    DoubleBondConformation.Opposite));
            Beam.Graph g = Convert(ac);
            Assert.AreEqual("F/C=C/F", g.ToSmiles());
        }

        /// <summary>
        /// (Z)-1,2-difluoroethene
        ///
        // @cdk.inchi InChI=1/C2H2F2/c3-1-2-4/h1-2H/b2-1-
        /// </summary>
        [TestMethod()]
        public void Z_1_2_difluoroethene()
        {

            IAtomContainer ac = new Default.AtomContainer();
            ac.Atoms.Add(new Default.Atom("F"));
            ac.Atoms.Add(new Default.Atom("C"));
            ac.Atoms.Add(new Default.Atom("C"));
            ac.Atoms.Add(new Default.Atom("F"));
            ac.AddBond(ac.Atoms[0], ac.Atoms[1], BondOrder.Single);
            ac.AddBond(ac.Atoms[1], ac.Atoms[2], BondOrder.Double);
            ac.AddBond(ac.Atoms[2], ac.Atoms[3], BondOrder.Single);

            ac.AddStereoElement(new DoubleBondStereochemistry(ac.Bonds[1], new IBond[] { ac.Bonds[0], ac.Bonds[2] },
                    DoubleBondConformation.Together));
            Beam.Graph g = Convert(ac);
            Assert.AreEqual("F/C=C\\F", g.ToSmiles());
        }

        /// <summary>
        /// (2R)-butan-2-ol
        ///
        // @cdk.inchi InChI=1/C4H10O/c1-3-4(2)5/h4-5H,3H2,1-2H3/t4-/s2
        /// </summary>
        [TestMethod()]
        public void _2R_butan_2_ol()
        {
            IAtomContainer ac = new Default.AtomContainer();
            ac.Atoms.Add(new Default.Atom("C"));
            ac.Atoms.Add(new Default.Atom("C"));
            ac.Atoms.Add(new Default.Atom("C"));
            ac.Atoms.Add(new Default.Atom("C"));
            ac.Atoms.Add(new Default.Atom("O"));
            ac.Atoms.Add(new Default.Atom("H"));
            ac.AddBond(ac.Atoms[0], ac.Atoms[1], BondOrder.Single);
            ac.AddBond(ac.Atoms[1], ac.Atoms[2], BondOrder.Single);
            ac.AddBond(ac.Atoms[2], ac.Atoms[3], BondOrder.Single);
            ac.AddBond(ac.Atoms[2], ac.Atoms[4], BondOrder.Single);
            ac.AddBond(ac.Atoms[2], ac.Atoms[5], BondOrder.Single);

            ac.AddStereoElement(new TetrahedralChirality(ac.Atoms[2], new IAtom[]{ac.Atoms[1], // C-C
                        ac.Atoms[3], // C
                        ac.Atoms[4], // O
                        ac.Atoms[5], // H
                }, TetrahedralStereo.Clockwise));

            Beam.Graph g = Convert(ac);
            Assert.AreEqual("CC[C@@](C)(O)[H]", g.ToSmiles());
        }

        /// <summary>
        /// (2S)-butan-2-ol
        ///
        // @cdk.inchi InChI=1/C4H10O/c1-3-4(2)5/h4-5H,3H2,1-2H3/t4-/s2
        /// </summary>
        [TestMethod()]
        public void _2S_butan_2_ol()
        {

            IAtomContainer ac = new Default.AtomContainer();
            ac.Atoms.Add(new Default.Atom("C"));
            ac.Atoms.Add(new Default.Atom("C"));
            ac.Atoms.Add(new Default.Atom("C"));
            ac.Atoms.Add(new Default.Atom("C"));
            ac.Atoms.Add(new Default.Atom("O"));
            ac.Atoms.Add(new Default.Atom("H"));
            ac.AddBond(ac.Atoms[0], ac.Atoms[1], BondOrder.Single);
            ac.AddBond(ac.Atoms[1], ac.Atoms[2], BondOrder.Single);
            ac.AddBond(ac.Atoms[2], ac.Atoms[3], BondOrder.Single);
            ac.AddBond(ac.Atoms[2], ac.Atoms[4], BondOrder.Single);
            ac.AddBond(ac.Atoms[2], ac.Atoms[5], BondOrder.Single);

            ac.AddStereoElement(new TetrahedralChirality(ac.Atoms[2], new IAtom[]{ac.Atoms[1], // C-C
                        ac.Atoms[3], // C
                        ac.Atoms[4], // O
                        ac.Atoms[5], // H
                }, TetrahedralStereo.AntiClockwise));

            Beam.Graph g = Convert(ac);
            Assert.AreEqual("CC[C@](C)(O)[H]", g.ToSmiles());
        }

        /// <summary>
        /// This is a mock test where we don't want aromatic bonds to have a
        /// configuration. (Z)-1,2-difluoroethene is not aromatic but a 'real'
        /// example would be porphyrins.
        ///
        // @cdk.inchi InChI=1/C2H2F2/c3-1-2-4/h1-2H/b2-1-
        /// </summary>
        [TestMethod()]
        public void Z_1_2_difluoroethene_aromatic()
        {

            IAtomContainer ac = new Default.AtomContainer();
            ac.Atoms.Add(new Default.Atom("F"));
            ac.Atoms.Add(new Default.Atom("C"));
            ac.Atoms.Add(new Default.Atom("C"));
            ac.Atoms.Add(new Default.Atom("F"));
            ac.AddBond(ac.Atoms[0], ac.Atoms[1], BondOrder.Single);
            ac.AddBond(ac.Atoms[1], ac.Atoms[2], BondOrder.Double);
            ac.AddBond(ac.Atoms[2], ac.Atoms[3], BondOrder.Single);

            ac.Bonds[1].IsAromatic = true;

            ac.AddStereoElement(new DoubleBondStereochemistry(ac.Bonds[1], new IBond[] { ac.Bonds[0], ac.Bonds[2] },
                   DoubleBondConformation.Together));
            Beam.Graph g = Convert(ac);
            Assert.AreEqual("F[CH]:[CH]F", g.ToSmiles());
        }

        [TestMethod()]
        public void Propadiene()
        {

        }

        [TestMethod()]
        public void WriteAtomClass()
        {
            IAtomContainer ac = new Default.AtomContainer();
            ac.Atoms.Add(new Default.Atom("C"));
            ac.Atoms.Add(new Default.Atom("C"));
            ac.Atoms.Add(new Default.Atom("O"));
            ac.AddBond(ac.Atoms[0], ac.Atoms[1], BondOrder.Single);
            ac.AddBond(ac.Atoms[1], ac.Atoms[2], BondOrder.Single);
            ac.Atoms[0].SetProperty(CDKPropertyName.ATOM_ATOM_MAPPING, 3);
            ac.Atoms[1].SetProperty(CDKPropertyName.ATOM_ATOM_MAPPING, 1);
            ac.Atoms[2].SetProperty(CDKPropertyName.ATOM_ATOM_MAPPING, 2);
            Assert.AreEqual("[CH3:3][CH2:1][OH:2]", Convert(ac).ToSmiles());
        }

        [TestMethod()]
        public void R_penta_2_3_diene_impl_h()
        {
            IAtomContainer m = new Default.AtomContainer();
            m.Atoms.Add(new Default.Atom("C"));
            m.Atoms.Add(new Default.Atom("C"));
            m.Atoms.Add(new Default.Atom("C"));
            m.Atoms.Add(new Default.Atom("C"));
            m.Atoms.Add(new Default.Atom("C"));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Double);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Double);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);

            IStereoElement element = new ExtendedTetrahedral(m.Atoms[2], new IAtom[]{m.Atoms[0], m.Atoms[1],
                        m.Atoms[3], m.Atoms[4]}, TetrahedralStereo.AntiClockwise);
            m.SetStereoElements(new[] { element });

            Assert.AreEqual("CC=[C@]=CC", Convert(m).ToSmiles());
        }

        [TestMethod()]
        public void S_penta_2_3_diene_impl_h()
        {
            IAtomContainer m = new Default.AtomContainer();
            m.Atoms.Add(new Default.Atom("C"));
            m.Atoms.Add(new Default.Atom("C"));
            m.Atoms.Add(new Default.Atom("C"));
            m.Atoms.Add(new Default.Atom("C"));
            m.Atoms.Add(new Default.Atom("C"));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Double);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Double);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);

            IStereoElement element = new ExtendedTetrahedral(m.Atoms[2], new IAtom[]{m.Atoms[0], m.Atoms[1],
                        m.Atoms[3], m.Atoms[4]}, TetrahedralStereo.Clockwise);
            m.SetStereoElements(new[] { element });

            Assert.AreEqual("CC=[C@@]=CC", Convert(m).ToSmiles());
        }

        [TestMethod()]
        public void R_penta_2_3_diene_expl_h()
        {
            IAtomContainer m = new Default.AtomContainer();
            m.Atoms.Add(new Default.Atom("C"));
            m.Atoms.Add(new Default.Atom("C"));
            m.Atoms.Add(new Default.Atom("C"));
            m.Atoms.Add(new Default.Atom("C"));
            m.Atoms.Add(new Default.Atom("C"));
            m.Atoms.Add(new Default.Atom("H"));
            m.Atoms.Add(new Default.Atom("H"));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Double);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Double);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[6], BondOrder.Single);

            int[][] atoms = new int[][] { new[] { 0, 5, 6, 4 }, new[] { 5, 0, 6, 4 }, new[] { 5, 0, 4, 6 }, new[] { 0, 5, 4, 6 }, new[] { 4, 6, 5, 0 }, new[] { 4, 6, 0, 5 }, new[] { 6, 4, 0, 5 }, new[] { 6, 4, 5, 0 }, };
            TetrahedralStereo[] stereos = new TetrahedralStereo[]{TetrahedralStereo.AntiClockwise, TetrahedralStereo.Clockwise, TetrahedralStereo.AntiClockwise,
                        TetrahedralStereo.Clockwise, TetrahedralStereo.AntiClockwise, TetrahedralStereo.Clockwise, TetrahedralStereo.AntiClockwise, TetrahedralStereo.Clockwise};

            for (int i = 0; i < atoms.Length; i++)
            {
                IStereoElement element = new ExtendedTetrahedral(m.Atoms[2], new IAtom[]{m.Atoms[atoms[i][0]],
                            m.Atoms[atoms[i][1]], m.Atoms[atoms[i][2]], m.Atoms[atoms[i][3]]}, stereos[i]);
                m.SetStereoElements(new[] { element });

                Assert.AreEqual("CC(=[C@@]=C(C)[H])[H]", Convert(m).ToSmiles());
            }
        }

        [TestMethod()]
        public void S_penta_2_3_diene_expl_h()
        {
            IAtomContainer m = new Default.AtomContainer();
            m.Atoms.Add(new Default.Atom("C"));
            m.Atoms.Add(new Default.Atom("C"));
            m.Atoms.Add(new Default.Atom("C"));
            m.Atoms.Add(new Default.Atom("C"));
            m.Atoms.Add(new Default.Atom("C"));
            m.Atoms.Add(new Default.Atom("H"));
            m.Atoms.Add(new Default.Atom("H"));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Double);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Double);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[6], BondOrder.Single);

            int[][] atoms = new int[][] { new[] { 0, 5, 6, 4 }, new[] { 5, 0, 6, 4 }, new[] { 5, 0, 4, 6 }, new[] { 0, 5, 4, 6 }, new[] { 4, 6, 5, 0 }, new[] { 4, 6, 0, 5 }, new[] { 6, 4, 0, 5 }, new[] { 6, 4, 5, 0 }, };
            TetrahedralStereo[] stereos = new TetrahedralStereo[]{TetrahedralStereo.Clockwise, TetrahedralStereo.AntiClockwise, TetrahedralStereo.Clockwise,
                        TetrahedralStereo.AntiClockwise, TetrahedralStereo.Clockwise, TetrahedralStereo.AntiClockwise, TetrahedralStereo.Clockwise, TetrahedralStereo.AntiClockwise};

            for (int i = 0; i < atoms.Length; i++)
            {
                IStereoElement element = new ExtendedTetrahedral(m.Atoms[2], new IAtom[]{m.Atoms[atoms[i][0]],
                            m.Atoms[atoms[i][1]], m.Atoms[atoms[i][2]], m.Atoms[atoms[i][3]]}, stereos[i]);
                m.SetStereoElements(new[] { element });

                Assert.AreEqual("CC(=[C@]=C(C)[H])[H]", Convert(m).ToSmiles());

            }
        }

        static Beam.Graph Convert(IAtomContainer ac)
        {
            return Convert(ac, false, true);
        }

        static Beam.Graph Convert(IAtomContainer ac, bool perceiveAromaticity, bool isomeric)
        {
            return Convert(ac, perceiveAromaticity, isomeric, true, true);
        }

        static Beam.Graph Convert(IAtomContainer ac, bool perceiveAromaticity, bool isomeric, bool aromatic,
                bool atomClasses)
        {
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(ac);
            CDKHydrogenAdder.GetInstance(Silent.ChemObjectBuilder.Instance).AddImplicitHydrogens(ac);
            if (perceiveAromaticity) Aromaticity.CDKLegacy.Apply(ac);
            return new CDKToBeam(isomeric, aromatic, atomClasses).ToBeamGraph(ac);
        }
    }
}


