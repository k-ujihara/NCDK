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
 * Any WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Numerics;
using NCDK.Sgroups;
using NCDK.Stereo;
using System.Collections.Generic;
using System.IO;

namespace NCDK.IO
{
    [TestClass()]
    public class MDLV3000WriterTest
    {
        private readonly IChemObjectBuilder builder = CDK.Builder;

        [TestMethod()]
        public void OutputValencyWhenNeeded()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("Na"));
            mol.Atoms.Add(builder.NewAtom("Na"));
            mol.Atoms[0].ImplicitHydrogenCount = 0; // Na metal
            mol.Atoms[1].ImplicitHydrogenCount = 1; // Na hydride
            string res = WriteToStr(mol);
            Assert.IsTrue(res.Contains("M  V30 1 Na 0 0 0 0 VAL=-1\n"));
            Assert.IsTrue(res.Contains("M  V30 2 Na 0 0 0 0\n"));
        }

        [TestMethod()]
        public void OutputFormalCharge()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("O"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.Atoms[0].ImplicitHydrogenCount = 0;
            mol.Atoms[0].FormalCharge = -1;
            mol.Atoms[1].ImplicitHydrogenCount = 3;
            string res = WriteToStr(mol);
            Assert.IsTrue(res.Contains("M  V30 1 O 0 0 0 0 CHG=-1\n"));
            Assert.IsTrue(res.Contains("M  V30 2 C 0 0 0 0\n"));
        }

        [TestMethod()]
        public void OutputMassNumber()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.Atoms[0].ImplicitHydrogenCount = 0;
            mol.Atoms[0].MassNumber = 2;
            mol.Atoms[1].ImplicitHydrogenCount = 3;
            string res = WriteToStr(mol);
            // H is pushed to back for compatibility
            Assert.IsTrue(res.Contains("M  V30 2 H 0 0 0 0 MASS=2\n"));
            Assert.IsTrue(res.Contains("M  V30 1 C 0 0 0 0\n"));
        }

        [TestMethod()]
        public void OutputRadical()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms[0].ImplicitHydrogenCount = 3;
            mol.AddSingleElectronTo(mol.Atoms[0]);
            string res = WriteToStr(mol);
            Assert.IsTrue(res.Contains("M  V30 1 C 0 0 0 0 RAD=2 VAL=3\n"));
        }

        [TestMethod()]
        [ExpectedException(typeof(CDKException))]
        public void NullBondOrder()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Bonds.Add(builder.NewBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Unset));
            mol.Atoms[0].ImplicitHydrogenCount = 0;
            mol.Atoms[0].MassNumber = 2;
            mol.Atoms[1].ImplicitHydrogenCount = 3;
            WriteToStr(mol);
        }

        [TestMethod()]
        [ExpectedException(typeof(CDKException))]
        public void UnSetBondOrder()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Unset);
            mol.Atoms[0].ImplicitHydrogenCount = 0;
            mol.Atoms[0].MassNumber = 2;
            mol.Atoms[1].ImplicitHydrogenCount = 3;
            WriteToStr(mol);
        }

        [TestMethod()]
        public void SolidWedgeBonds()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("O"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single, BondStereo.Up);
            mol.Atoms[0].ImplicitHydrogenCount = 3;
            mol.Atoms[1].ImplicitHydrogenCount = 1;
            string res = WriteToStr(mol);
            Assert.IsTrue(res.Contains("M  V30 1 1 1 2 CFG=1\n"));
        }

        [TestMethod()]
        public void HashedWedgeBonds()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("O"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single, BondStereo.Down);
            mol.Atoms[0].ImplicitHydrogenCount = 3;
            mol.Atoms[1].ImplicitHydrogenCount = 1;
            string res = WriteToStr(mol);
            Assert.IsTrue(res.Contains("M  V30 1 1 1 2 CFG=3\n"));
        }

        [TestMethod()]
        public void SolidWedgeInvBonds()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("O"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single, BondStereo.UpInverted);
            mol.Atoms[0].ImplicitHydrogenCount = 3;
            mol.Atoms[1].ImplicitHydrogenCount = 1;
            string res = WriteToStr(mol);
            Assert.IsTrue(res.Contains("M  V30 1 1 2 1 CFG=1\n"));
        }

        [TestMethod()]
        public void HashedWedgeInvBonds()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("O"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single, BondStereo.DownInverted);
            mol.Atoms[0].ImplicitHydrogenCount = 3;
            mol.Atoms[1].ImplicitHydrogenCount = 1;
            string res = WriteToStr(mol);
            Assert.IsTrue(res.Contains("M  V30 1 1 2 1 CFG=3\n"));
        }

        [TestMethod()]
        public void WriteLeadingZero()
        {
            var mol = builder.NewAtomContainer();
            var atom = builder.NewAtom("C"); ;
            atom.Point2D = new Vector2(0.5, 1.2);
            mol.Atoms.Add(atom);
            Assert.IsTrue(WriteToStr(mol).Contains("0.5 1.2"));
        }

        [TestMethod()]
        public void WriteParity()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("O"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[5], BondOrder.Single);
            mol.Atoms[0].ImplicitHydrogenCount = 1;
            mol.Atoms[1].ImplicitHydrogenCount = 0;
            mol.Atoms[2].ImplicitHydrogenCount = 2;
            mol.Atoms[3].ImplicitHydrogenCount = 3;
            mol.Atoms[4].ImplicitHydrogenCount = 3;
            mol.Atoms[5].ImplicitHydrogenCount = 0;
            mol.StereoElements.Add(
                new TetrahedralChirality(mol.Atoms[1],
                    new IAtom[]
                        {
                            mol.Atoms[0],  // oxygen (look from)
                            mol.Atoms[2],  // Et
                            mol.Atoms[4],  // Me
                            mol.Atoms[5],  // H
                        },
                    TetrahedralStereo.Clockwise));
            string res = WriteToStr(mol);
            Assert.IsTrue(res.Contains("M  V30 2 C 0 0 0 0 CFG=2\n"));
        }

        [TestMethod()]
        public void WriteParityHNotLast()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("O"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("H"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[5], BondOrder.Single);
            mol.Atoms[0].ImplicitHydrogenCount = 1;
            mol.Atoms[1].ImplicitHydrogenCount = 0;
            mol.Atoms[2].ImplicitHydrogenCount = 2;
            mol.Atoms[3].ImplicitHydrogenCount = 3;
            mol.Atoms[4].ImplicitHydrogenCount = 0;
            mol.Atoms[5].ImplicitHydrogenCount = 3;
            mol.StereoElements.Add(
                new TetrahedralChirality(
                    mol.Atoms[1],
                    new IAtom[]
                        {
                            mol.Atoms[0],  // oxygen (look from)
                            mol.Atoms[2],  // Et
                            mol.Atoms[4],  // H
                            mol.Atoms[5]}, // Me
                    TetrahedralStereo.Clockwise));
            string res = WriteToStr(mol);
            Assert.IsTrue(res.Contains("M  V30 2 C 0 0 0 0 CFG=1\n"));
            // H was moved to position 6 from 5
            Assert.IsTrue(res.Contains("M  V30 6 H 0 0 0 0\n"));
        }

        [TestMethod()]
        public void WriteParityImplH()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("O"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);
            mol.Atoms[0].ImplicitHydrogenCount = 1;
            mol.Atoms[1].ImplicitHydrogenCount = 1;
            mol.Atoms[2].ImplicitHydrogenCount = 2;
            mol.Atoms[3].ImplicitHydrogenCount = 3;
            mol.Atoms[4].ImplicitHydrogenCount = 3;
            mol.StereoElements.Add(new TetrahedralChirality(mol.Atoms[1],
                                                          new IAtom[]{mol.Atoms[0],  // oxygen (look from)
                                                                  mol.Atoms[2],  // Et
                                                                  mol.Atoms[1],  // H (implicit)
                                                                  mol.Atoms[4]}, // Me
                                                          TetrahedralStereo.Clockwise));
            string res = WriteToStr(mol);
            Assert.IsTrue(res.Contains("M  V30 2 C 0 0 0 0 CFG=1\n"));
        }

        [TestMethod()]
        public void WriteParityImplHInverted()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("O"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[4], BondOrder.Single);
            mol.Atoms[0].ImplicitHydrogenCount = 1;
            mol.Atoms[1].ImplicitHydrogenCount = 1;
            mol.Atoms[2].ImplicitHydrogenCount = 2;
            mol.Atoms[3].ImplicitHydrogenCount = 3;
            mol.Atoms[4].ImplicitHydrogenCount = 3;
            mol.StereoElements.Add(
                new TetrahedralChirality(
                    mol.Atoms[1],
                    new IAtom[]{
                        mol.Atoms[0],  // oxygen (look from)
                        mol.Atoms[1],  // H (implicit)
                        mol.Atoms[2],  // Et
                        mol.Atoms[4]}, // Me
                    TetrahedralStereo.Clockwise));
            string res = WriteToStr(mol);
            Assert.IsTrue(res.Contains("M  V30 2 C 0 0 0 0 CFG=2\n"));
        }

        [TestMethod()]
        public void WriteSRUs()
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("O"));
            mol.Atoms.Add(builder.NewAtom("O"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            mol.AddBond(mol.Atoms[2], mol.Atoms[3], BondOrder.Single);
            mol.Atoms[0].ImplicitHydrogenCount = 3;
            mol.Atoms[1].ImplicitHydrogenCount = 2;
            mol.Atoms[2].ImplicitHydrogenCount = 0;
            mol.Atoms[3].ImplicitHydrogenCount = 1;
            var sgroups = new List<Sgroup>();
            Sgroup sgroup = new Sgroup();
            sgroup.Atoms.Add(mol.Atoms[1]);
            sgroup.Atoms.Add(mol.Atoms[2]);
            sgroup.Bonds.Add(mol.Bonds[0]);
            sgroup.Bonds.Add(mol.Bonds[2]);
            sgroup.Type = SgroupType.CtabStructureRepeatUnit;
            sgroup.Subscript = "n";
            sgroup.PutValue(SgroupKey.CtabConnectivity, "HH");
            sgroups.Add(sgroup);
            mol.SetCtabSgroups(sgroups);
            string res = WriteToStr(mol);
            Assert.IsTrue(res.Contains("M  V30 1 SRU 0 ATOMS=(2 2 3) XBONDS=(2 1 3) LABEL=n CONNECT=HH\n"));
        }

        [TestMethod()]
        public void WriteMultipleGroup()
        {
            int repeatAtoms = 50;
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(builder.NewAtom("C"));
            for (int i = 0; i < repeatAtoms; i++)
                mol.Atoms.Add(builder.NewAtom("C"));
            mol.Atoms.Add(builder.NewAtom("O"));
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            for (int i = 0; i < repeatAtoms; i++)
                mol.AddBond(mol.Atoms[i + 1], mol.Atoms[i + 2], BondOrder.Single);
            mol.Atoms[0].ImplicitHydrogenCount = 3;
            for (int i = 0; i < repeatAtoms; i++)
                mol.Atoms[1 + i].ImplicitHydrogenCount = 2;
            mol.Atoms[mol.Atoms.Count - 1].ImplicitHydrogenCount = 1;

            var sgroups = new List<Sgroup>();
            Sgroup sgroup = new Sgroup();
            for (int i = 0; i < repeatAtoms; i++)
                sgroup.Atoms.Add(mol.Atoms[i + 1]);
            sgroup.Bonds.Add(mol.Bonds[0]);
            sgroup.Bonds.Add(mol.Bonds[mol.Bonds.Count - 1]);
            sgroup.PutValue(SgroupKey.CtabParentAtomList, new[] { mol.Atoms[1] });
            sgroup.Type = SgroupType.CtabMultipleGroup;
            sgroup.Subscript = repeatAtoms.ToString();
            sgroups.Add(sgroup);
            mol.SetCtabSgroups(sgroups);
            string res = WriteToStr(mol);
            Assert.IsTrue(res.Contains("M  V30 1 MUL 0 ATOMS=(50 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 2-\n"
                                                    + "M  V30 2 23 24 25 26 27 28 29 30 31 32 33 34 35 36 37 38 39 40 41 42 43 44 45 -\n"
                                                    + "M  V30 46 47 48 49 50 51) XBONDS=(2 1 51) MULT=50 PATOMS=(1 2)\n"));
        }

        [TestMethod()]
        public void RoundTripSRU()
        {
            using (MDLV2000Reader mdlr = new MDLV2000Reader(ResourceLoader.GetAsStream("NCDK.Data.MDL.sgroup-sru-bracketstyles.mol")))
            {
                IAtomContainer mol = mdlr.Read(builder.NewAtomContainer());
                string res = WriteToStr(mol);
                Assert.IsTrue(
                    res.Contains("M  V30 1 SRU 0 ATOMS=(1 2) XBONDS=(2 1 2) LABEL=n CONNECT=HT BRKXYZ=(9 -2.5742-\n"
                                    + "M  V30  4.207 0 -3.0692 3.3497 0 0 0 0) BRKXYZ=(9 -3.1626 3.3497 0 -3.6576 4.2-\n"
                                    + "M  V30 07 0 0 0 0) BRKTYP=PAREN\n"
                                    + "M  V30 2 SRU 0 ATOMS=(1 5) XBONDS=(2 3 4) LABEL=n CONNECT=HT BRKXYZ=(9 0.9542 -\n"
                                    + "M  V30 4.1874 0 0.4592 3.33 0 0 0 0) BRKXYZ=(9 0.3658 3.33 0 -0.1292 4.1874 0 -\n"
                                    + "M  V30 0 0 0) BRKTYP=PAREN"));
            }
        }

        [TestMethod()]
        public void RoundTripExpandedAbbrv()
        {
            using (MDLV2000Reader mdlr = new MDLV2000Reader(ResourceLoader.GetAsStream("NCDK.Data.MDL.triphenyl-phosphate-expanded.mol")))
            {
                IAtomContainer mol = mdlr.Read(builder.NewAtomContainer());
                string res = WriteToStr(mol);
                Assert.IsTrue(res.Contains(
                    "M  V30 1 SUP 0 ATOMS=(6 6 19 20 21 22 23) XBONDS=(1 5) ESTATE=E LABEL=Ph\n" +
                    "M  V30 2 SUP 0 ATOMS=(6 8 14 15 16 17 18) XBONDS=(1 7) ESTATE=E LABEL=Ph\n" +
                    "M  V30 3 SUP 0 ATOMS=(6 7 9 10 11 12 13) XBONDS=(1 6) ESTATE=E LABEL=Ph\n"));
            }
        }

        [TestMethod()]
        public void RoundTripOrderMixtures()
        {
            using (MDLV2000Reader mdlr = new MDLV2000Reader(ResourceLoader.GetAsStream("NCDK.Data.MDL.sgroup-ord-mixture.mol")))
            {
                IAtomContainer mol = mdlr.Read(builder.NewAtomContainer());
                string res = WriteToStr(mol);
                Assert.IsTrue(res.Contains(
                    "M  V30 1 FOR 0 ATOMS=(24 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21-\n" +
                    "M  V30  22 23 24) BRKXYZ=(9 -6.9786 -1.9329 0 -6.9786 4.5847 0 0 0 0) BRKXYZ=(-\n" +
                    "M  V30 9 1.9402 4.5847 0 1.9402 -1.9329 0 0 0 0)\n"));
                Assert.IsTrue(res.Contains(
                    "M  V30 2 COM 0 ATOMS=(13 1 2 3 4 5 6 7 8 9 10 11 12 13) PARENT=1 BRKXYZ=(9 -6.-\n" +
                    "M  V30 5661 -1.1668 0 -6.5661 3.7007 0 0 0 0) BRKXYZ=(9 -2.5532 3.7007 0 -2.55-\n" +
                    "M  V30 32 -1.1668 0 0 0 0) COMPNO=1\n"));
                Assert.IsTrue(res.Contains(
                    "M  V30 3 COM 0 ATOMS=(11 14 15 16 17 18 19 20 21 22 23 24) PARENT=1 BRKXYZ=(9 -\n" +
                    "M  V30 -1.8257 -1.5204 0 -1.8257 4.1722 0 0 0 0) BRKXYZ=(9 1.4727 4.1722 0 1.4-\n" +
                    "M  V30 727 -1.5204 0 0 0 0) COMPNO=2\n"));
            }
        }

        [TestMethod()]
        public void PositionalVariationRoundTrip()
        {
            using (MDLV3000Reader mdlr = new MDLV3000Reader(GetType().Assembly.GetManifestResourceStream(GetType(), "multicenterBond.mol")))
            {
                IAtomContainer mol = mdlr.Read(builder.NewAtomContainer());
                string res = WriteToStr(mol);
                Assert.IsTrue(res.Contains("M  V30 8 1 8 9 ATTACH=Any ENDPTS=(5 2 3 4 5 6)\n"));
            }
        }

        [TestMethod()]
        public void WriteDimensionField()
        {
            var builder = CDK.Builder;
            IAtomContainer mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom();
            atom.Symbol = "C";
            atom.ImplicitHydrogenCount = 4;
            atom.Point2D = new Vector2(0.5, 0.5);
            mol.Atoms.Add(atom);
            StringWriter sw = new StringWriter();
            using (MDLV3000Writer mdlw = new MDLV3000Writer(sw))
            {
                mdlw.Write(mol);
            }
            Assert.IsTrue(sw.ToString().Contains("2D"));
        }

        [TestMethod()]
        public void WriteDimensionField3D()
        {
            var builder = CDK.Builder;
            IAtomContainer mol = builder.NewAtomContainer();
            IAtom atom = builder.NewAtom();
            atom.Symbol = "C";
            atom.ImplicitHydrogenCount = 4;
            atom.Point3D = new Vector3(0.5, 0.5, 0.1);
            mol.Atoms.Add(atom);
            StringWriter sw = new StringWriter();
            using (MDLV3000Writer mdlw = new MDLV3000Writer(sw))
            {
                mdlw.Write(mol);
            }
            Assert.IsTrue(sw.ToString().Contains("3D"));
        }

        private string WriteToStr(IAtomContainer mol)
        {
            StringWriter sw = new StringWriter();
            using (MDLV3000Writer mdlw = new MDLV3000Writer(sw))
            {
                mdlw.Write(mol);
            }
            return sw.ToString();
        }
    }
}
