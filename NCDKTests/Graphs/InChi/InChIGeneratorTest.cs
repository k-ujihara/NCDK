/* Copyright (C) 2006-2007  Sam Adams <sea36@users.sf.net>
 *               2010,2012  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.IO;
using NCDK.Numerics;
using NCDK.Smiles;
using NCDK.Stereo;
using System;
using System.Collections.Generic;

namespace NCDK.Graphs.InChI
{
    /// <summary>
    /// TestCase for the InChIGenerator.
    /// </summary>
    // @cdk.module test-inchi
    // @see org.openscience.cdk.inchi.InChIGenerator
    [TestClass()]
    public class InChIGeneratorTest : CDKTestCase
    {
        private readonly IChemObjectBuilder builder = CDK.Builder;
        private readonly InChIGeneratorFactory factory = InChIGeneratorFactory.Instance;

        /// <summary>
        /// Tests element name is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetInChIFromChlorineAtom()
        {
            var ac = builder.NewAtomContainer();
            ac.Atoms.Add(builder.NewAtom("ClH"));
            var gen = factory.GetInChIGenerator(ac, "FixedH");
            Assert.AreEqual(gen.ReturnStatus, InChIReturnCode.Ok);
            Assert.AreEqual("InChI=1/ClH/h1H", gen.InChI);
        }

        [TestMethod()]
        public void TestGetLog()
        {
            var ac = builder.NewAtomContainer();
            ac.Atoms.Add(builder.NewAtom("Cl"));
            var gen = factory.GetInChIGenerator(ac, "FixedH");
            Assert.IsNotNull(gen.Log);
        }

        [TestMethod()]
        public void TestGetAuxInfo()
        {
            var ac = builder.NewAtomContainer();
            var a1 = builder.NewAtom("C");
            var a2 = builder.NewAtom("C");
            a1.ImplicitHydrogenCount = 3;
            a2.ImplicitHydrogenCount = 3;
            ac.Atoms.Add(a1);
            ac.Atoms.Add(a2);
            ac.Bonds.Add(builder.NewBond(a1, a2, BondOrder.Single));
            var gen = factory.GetInChIGenerator(ac, "");
            Assert.IsNotNull(gen.AuxInfo);
            Assert.IsTrue(gen.AuxInfo.StartsWith("AuxInfo="));
        }

        [TestMethod()]
        public void TestGetMessage()
        {
            var ac = builder.NewAtomContainer();
            ac.Atoms.Add(builder.NewAtom("Cl"));
            var gen = factory.GetInChIGenerator(ac, "FixedH");
            Assert.IsNull(gen.Message, "Because this generation should work, I expected a null message string.");
        }

        [TestMethod()]
        public void TestGetWarningMessage()
        {
            var ac = builder.NewAtomContainer();
            var cl = builder.NewAtom("Cl");
            var h = builder.NewAtom("H");
            ac.Atoms.Add(cl);
            ac.Atoms.Add(h);
            ac.AddBond(cl, h, BondOrder.Triple);
            var gen = factory.GetInChIGenerator(ac);
            Assert.IsNotNull(gen.Message);
            Assert.IsTrue(gen.Message.Contains("Accepted unusual valence"));
        }

        /// <summary>
        /// Tests charge is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetInChIFromLithiumIon()
        {
            var ac = builder.NewAtomContainer();
            var a = builder.NewAtom("Li");
            a.FormalCharge = +1;
            ac.Atoms.Add(a);
            var gen = factory.GetInChIGenerator(ac, "FixedH");
            Assert.AreEqual(gen.ReturnStatus, InChIReturnCode.Ok);
            Assert.AreEqual("InChI=1/Li/q+1", gen.InChI);
        }

        /// <summary>
        /// Tests isotopic mass is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetInChIFromChlorine37Atom()
        {
            var ac = builder.NewAtomContainer();
            var a = builder.NewAtom("ClH");
            a.MassNumber = 37;
            ac.Atoms.Add(a);
            var gen = factory.GetInChIGenerator(ac, "FixedH");
            Assert.AreEqual(gen.ReturnStatus, InChIReturnCode.Ok);
            Assert.AreEqual("InChI=1/ClH/h1H/i1+2", gen.InChI);
        }

        /// <summary>
        /// Tests implicit hydrogen count is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetInChIFromHydrogenChlorideImplicitH()
        {
            var ac = builder.NewAtomContainer();
            var a = builder.NewAtom("Cl");
            a.ImplicitHydrogenCount = 1;
            ac.Atoms.Add(a);
            var gen = factory.GetInChIGenerator(ac, "FixedH");
            Assert.AreEqual(gen.ReturnStatus, InChIReturnCode.Ok);
            Assert.AreEqual("InChI=1/ClH/h1H", gen.InChI);
        }

        /// <summary>
        /// Tests radical state is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetInChIFromMethylRadical()
        {
            var ac = builder.NewAtomContainer();
            var a = builder.NewAtom("C");
            a.ImplicitHydrogenCount = 3;
            ac.Atoms.Add(a);
            ac.SingleElectrons.Add(builder.NewSingleElectron(a));
            var gen = factory.GetInChIGenerator(ac, "FixedH");
            Assert.AreEqual(gen.ReturnStatus, InChIReturnCode.Ok);
            Assert.AreEqual("InChI=1/CH3/h1H3", gen.InChI);
        }

        /// <summary>
        /// Tests single bond is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetInChIFromEthane()
        {
            var ac = builder.NewAtomContainer();
            var a1 = builder.NewAtom("C");
            var a2 = builder.NewAtom("C");
            a1.ImplicitHydrogenCount = 3;
            a2.ImplicitHydrogenCount = 3;
            ac.Atoms.Add(a1);
            ac.Atoms.Add(a2);
            ac.Bonds.Add(builder.NewBond(a1, a2, BondOrder.Single));
            var gen = factory.GetInChIGenerator(ac, "FixedH");
            Assert.AreEqual(gen.ReturnStatus, InChIReturnCode.Ok);
            Assert.AreEqual("InChI=1/C2H6/c1-2/h1-2H3", gen.InChI);
            Assert.AreEqual("OTMSDBZUPAUEDD-UHFFFAOYNA-N", gen.GetInChIKey());
        }

        /// <summary>
        /// Test generation of non-standard InChIs.
        /// </summary>
        // @cdk.bug 1384
        [TestMethod()]
        public void NonStandardInChIWithEnumOptions()
        {
            var ac = builder.NewAtomContainer();
            var a1 = builder.NewAtom("C");
            var a2 = builder.NewAtom("C");
            a1.ImplicitHydrogenCount = 3;
            a2.ImplicitHydrogenCount = 3;
            ac.Atoms.Add(a1);
            ac.Atoms.Add(a2);
            ac.Bonds.Add(builder.NewBond(a1, a2, BondOrder.Single));
            var options = new List<InChIOption>
                {
                    InChIOption.FixedH,
                    InChIOption.SAbs,
                    InChIOption.SAsXYZ,
                    InChIOption.SPXYZ,
                    InChIOption.FixSp3Bug,
                    InChIOption.AuxNone
                };
            var gen = factory.GetInChIGenerator(ac, options);
            Assert.AreEqual(gen.ReturnStatus, InChIReturnCode.Ok);
            Assert.AreEqual("InChI=1/C2H6/c1-2/h1-2H3", gen.InChI);
            Assert.AreEqual("OTMSDBZUPAUEDD-UHFFFAOYNA-N", gen.GetInChIKey());
        }

        /// <summary>
        /// Tests double bond is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetInChIFromEthene()
        {
            var ac = builder.NewAtomContainer();
            var a1 = builder.NewAtom("C");
            var a2 = builder.NewAtom("C");
            a1.ImplicitHydrogenCount = 2;
            a2.ImplicitHydrogenCount = 2;
            ac.Atoms.Add(a1);
            ac.Atoms.Add(a2);
            ac.Bonds.Add(builder.NewBond(a1, a2, BondOrder.Double));
            var gen = factory.GetInChIGenerator(ac, "FixedH");
            Assert.AreEqual(gen.ReturnStatus, InChIReturnCode.Ok);
            Assert.AreEqual("InChI=1/C2H4/c1-2/h1-2H2", gen.InChI);
        }

        /// <summary>
        /// Tests triple bond is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetInChIFromEthyne()
        {
            var ac = builder.NewAtomContainer();
            var a1 = builder.NewAtom("C");
            var a2 = builder.NewAtom("C");
            a1.ImplicitHydrogenCount = 1;
            a2.ImplicitHydrogenCount = 1;
            ac.Atoms.Add(a1);
            ac.Atoms.Add(a2);
            ac.Bonds.Add(builder.NewBond(a1, a2, BondOrder.Triple));
            var gen = factory.GetInChIGenerator(ac, "FixedH");
            Assert.AreEqual(gen.ReturnStatus, InChIReturnCode.Ok);
            Assert.AreEqual("InChI=1/C2H2/c1-2/h1-2H", gen.InChI);
        }

        /// <summary>
        /// Tests 2D coordinates are correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetInChIEandZ12Dichloroethene2D()
        {
            // (E)-1,2-dichloroethene
            var acE = builder.NewAtomContainer();
            var a1E = builder.NewAtom("C", new Vector2(2.866, -0.250));
            var a2E = builder.NewAtom("C", new Vector2(3.732, 0.250));
            var a3E = builder.NewAtom("Cl", new Vector2(2.000, 2.500));
            var a4E = builder.NewAtom("Cl", new Vector2(4.598, -0.250));
            a1E.ImplicitHydrogenCount = 1;
            a2E.ImplicitHydrogenCount = 1;
            acE.Atoms.Add(a1E);
            acE.Atoms.Add(a2E);
            acE.Atoms.Add(a3E);
            acE.Atoms.Add(a4E);

            acE.Bonds.Add(builder.NewBond(a1E, a2E, BondOrder.Double));
            acE.Bonds.Add(builder.NewBond(a1E, a2E, BondOrder.Double));
            acE.Bonds.Add(builder.NewBond(a1E, a3E, BondOrder.Single));
            acE.Bonds.Add(builder.NewBond(a2E, a4E, BondOrder.Single));

            var genE = factory.GetInChIGenerator(acE, "FixedH");
            Assert.AreEqual(genE.ReturnStatus, InChIReturnCode.Ok);
            Assert.AreEqual("InChI=1/C2H2Cl2/c3-1-2-4/h1-2H/b2-1+", genE.InChI);

            // (Z)-1,2-dichloroethene
            var acZ = builder.NewAtomContainer();
            var a1Z = builder.NewAtom("C", new Vector2(2.866, -0.440));
            var a2Z = builder.NewAtom("C", new Vector2(3.732, 0.060));
            var a3Z = builder.NewAtom("Cl", new Vector2(2.000, 0.060));
            var a4Z = builder.NewAtom("Cl", new Vector2(3.732, 1.060));
            a1Z.ImplicitHydrogenCount = 1;
            a2Z.ImplicitHydrogenCount = 1;
            acZ.Atoms.Add(a1Z);
            acZ.Atoms.Add(a2Z);
            acZ.Atoms.Add(a3Z);
            acZ.Atoms.Add(a4Z);

            acZ.Bonds.Add(builder.NewBond(a1Z, a2Z, BondOrder.Double));
            acZ.Bonds.Add(builder.NewBond(a1Z, a2Z, BondOrder.Double));
            acZ.Bonds.Add(builder.NewBond(a1Z, a3Z, BondOrder.Single));
            acZ.Bonds.Add(builder.NewBond(a2Z, a4Z, BondOrder.Single));

            var genZ = factory.GetInChIGenerator(acZ, "FixedH");
            Assert.AreEqual(genZ.ReturnStatus, InChIReturnCode.Ok);
            Assert.AreEqual("InChI=1/C2H2Cl2/c3-1-2-4/h1-2H/b2-1-", genZ.InChI);
        }

        /// <summary>
        /// Tests 3D coordinates are correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetInChIFromLandDAlanine3D()
        {
            // L-Alanine
            var acL = builder.NewAtomContainer();
            var a1L = builder.NewAtom("C", new Vector3(-0.358, 0.819, 20.655));
            var a2L = builder.NewAtom("C", new Vector3(-1.598, -0.032, 20.905));
            var a3L = builder.NewAtom("N", new Vector3(-0.275, 2.014, 21.574));
            var a4L = builder.NewAtom("C", new Vector3(0.952, 0.043, 20.838));
            var a5L = builder.NewAtom("O", new Vector3(-2.678, 0.479, 21.093));
            var a6L = builder.NewAtom("O", new Vector3(-1.596, -1.239, 20.958));
            a1L.ImplicitHydrogenCount = 1;
            a3L.ImplicitHydrogenCount = 2;
            a4L.ImplicitHydrogenCount = 3;
            a5L.ImplicitHydrogenCount = 1;
            acL.Atoms.Add(a1L);
            acL.Atoms.Add(a2L);
            acL.Atoms.Add(a3L);
            acL.Atoms.Add(a4L);
            acL.Atoms.Add(a5L);
            acL.Atoms.Add(a6L);

            acL.Bonds.Add(builder.NewBond(a1L, a2L, BondOrder.Single));
            acL.Bonds.Add(builder.NewBond(a1L, a3L, BondOrder.Single));
            acL.Bonds.Add(builder.NewBond(a1L, a4L, BondOrder.Single));
            acL.Bonds.Add(builder.NewBond(a2L, a5L, BondOrder.Single));
            acL.Bonds.Add(builder.NewBond(a2L, a6L, BondOrder.Double));

            var genL = factory.GetInChIGenerator(acL, "FixedH");
            Assert.AreEqual(genL.ReturnStatus, InChIReturnCode.Ok);
            Assert.AreEqual("InChI=1/C3H7NO2/c1-2(4)3(5)6/h2H,4H2,1H3,(H,5,6)/t2-/m0/s1/f/h5H", genL.InChI);

            // D-Alanine
            var acD = builder.NewAtomContainer();
            var a1D = builder.NewAtom("C", new Vector3(0.358, 0.819, 20.655));
            var a2D = builder.NewAtom("C", new Vector3(1.598, -0.032, 20.905));
            var a3D = builder.NewAtom("N", new Vector3(0.275, 2.014, 21.574));
            var a4D = builder.NewAtom("C", new Vector3(-0.952, 0.043, 20.838));
            var a5D = builder.NewAtom("O", new Vector3(2.678, 0.479, 21.093));
            var a6D = builder.NewAtom("O", new Vector3(1.596, -1.239, 20.958));
            a1D.ImplicitHydrogenCount = 1;
            a3D.ImplicitHydrogenCount = 2;
            a4D.ImplicitHydrogenCount = 3;
            a5D.ImplicitHydrogenCount = 1;
            acD.Atoms.Add(a1D);
            acD.Atoms.Add(a2D);
            acD.Atoms.Add(a3D);
            acD.Atoms.Add(a4D);
            acD.Atoms.Add(a5D);
            acD.Atoms.Add(a6D);

            acD.Bonds.Add(builder.NewBond(a1D, a2D, BondOrder.Single));
            acD.Bonds.Add(builder.NewBond(a1D, a3D, BondOrder.Single));
            acD.Bonds.Add(builder.NewBond(a1D, a4D, BondOrder.Single));
            acD.Bonds.Add(builder.NewBond(a2D, a5D, BondOrder.Single));
            acD.Bonds.Add(builder.NewBond(a2D, a6D, BondOrder.Double));

            var genD = factory.GetInChIGenerator(acD, "FixedH");
            Assert.AreEqual(genD.ReturnStatus, InChIReturnCode.Ok);
            Assert.AreEqual("InChI=1/C3H7NO2/c1-2(4)3(5)6/h2H,4H2,1H3,(H,5,6)/t2-/m1/s1/f/h5H", genD.InChI);
        }

        // ensure only
        [TestMethod()]
        public void ZeroHydrogenCount()
        {
            var ac = builder.NewAtomContainer();
            ac.Atoms.Add(builder.NewAtom("O"));
            ac.Atoms[0].ImplicitHydrogenCount = 0;
            var gen = factory.GetInChIGenerator(ac);
            Assert.AreEqual(InChIReturnCode.Ok, gen.ReturnStatus);
            Assert.AreEqual("InChI=1S/O", gen.InChI);
        }

        /// <summary>
        /// Tests element name is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetStandardInChIFromChlorineAtom()
        {
            var ac = builder.NewAtomContainer();
            ac.Atoms.Add(builder.NewAtom("ClH"));
            var gen = factory.GetInChIGenerator(ac);
            Assert.AreEqual(InChIReturnCode.Ok, gen.ReturnStatus);
            Assert.AreEqual("InChI=1S/ClH/h1H", gen.InChI);
        }

        /// <summary>
        /// Tests charge is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetStandardInChIFromLithiumIon()
        {
            var ac = builder.NewAtomContainer();
            var a = builder.NewAtom("Li");
            a.FormalCharge = +1;
            ac.Atoms.Add(a);
            var gen = factory.GetInChIGenerator(ac);
            Assert.AreEqual(InChIReturnCode.Ok, gen.ReturnStatus);
            Assert.AreEqual("InChI=1S/Li/q+1", gen.InChI);
        }

        /// <summary>
        /// Tests isotopic mass is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetStandardInChIFromChlorine37Atom()
        {
            var ac = builder.NewAtomContainer();
            var a = builder.NewAtom("ClH");
            a.MassNumber = 37;
            ac.Atoms.Add(a);
            var gen = factory.GetInChIGenerator(ac);
            Assert.AreEqual(InChIReturnCode.Ok, gen.ReturnStatus);
            Assert.AreEqual("InChI=1S/ClH/h1H/i1+2", gen.InChI);
        }

        /// <summary>
        /// Tests implicit hydrogen count is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetStandardInChIFromHydrogenChlorideImplicitH()
        {
            var ac = builder.NewAtomContainer();
            var a = builder.NewAtom("Cl");
            a.ImplicitHydrogenCount = 1;
            ac.Atoms.Add(a);
            var gen = factory.GetInChIGenerator(ac);
            Assert.AreEqual(gen.ReturnStatus, InChIReturnCode.Ok);
            Assert.AreEqual("InChI=1S/ClH/h1H", gen.InChI);
        }

        /// <summary>
        /// Tests radical state is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetStandardInChIFromMethylRadical()
        {
            var ac = builder.NewAtomContainer();
            var a = builder.NewAtom("C");
            a.ImplicitHydrogenCount = 3;
            ac.Atoms.Add(a);
            ac.SingleElectrons.Add(builder.NewSingleElectron(a));
            var gen = factory.GetInChIGenerator(ac);
            Assert.AreEqual(InChIReturnCode.Ok, gen.ReturnStatus);
            Assert.AreEqual("InChI=1S/CH3/h1H3", gen.InChI);
        }

        /// <summary>
        /// Tests single bond is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetStandardInChIFromEthane()
        {
            var ac = builder.NewAtomContainer();
            var a1 = builder.NewAtom("C");
            var a2 = builder.NewAtom("C");
            a1.ImplicitHydrogenCount = 3;
            a2.ImplicitHydrogenCount = 3;
            ac.Atoms.Add(a1);
            ac.Atoms.Add(a2);
            ac.Bonds.Add(builder.NewBond(a1, a2, BondOrder.Single));
            var gen = factory.GetInChIGenerator(ac);
            Assert.AreEqual(InChIReturnCode.Ok, gen.ReturnStatus);
            Assert.AreEqual("InChI=1S/C2H6/c1-2/h1-2H3", gen.InChI);
            Assert.AreEqual("OTMSDBZUPAUEDD-UHFFFAOYSA-N", gen.GetInChIKey());
        }

        /// <summary>
        /// Tests double bond is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetStandardInChIFromEthene()
        {
            var ac = builder.NewAtomContainer();
            var a1 = builder.NewAtom("C");
            var a2 = builder.NewAtom("C");
            a1.ImplicitHydrogenCount = 2;
            a2.ImplicitHydrogenCount = 2;
            ac.Atoms.Add(a1);
            ac.Atoms.Add(a2);
            ac.Bonds.Add(builder.NewBond(a1, a2, BondOrder.Double));
            var gen = factory.GetInChIGenerator(ac);
            Assert.AreEqual(InChIReturnCode.Ok, gen.ReturnStatus);
            Assert.AreEqual("InChI=1S/C2H4/c1-2/h1-2H2", gen.InChI);
        }

        /// <summary>
        /// Tests triple bond is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetStandardInChIFromEthyne()
        {
            var ac = builder.NewAtomContainer();
            var a1 = builder.NewAtom("C");
            var a2 = builder.NewAtom("C");
            a1.ImplicitHydrogenCount = 1;
            a2.ImplicitHydrogenCount = 1;
            ac.Atoms.Add(a1);
            ac.Atoms.Add(a2);
            ac.Bonds.Add(builder.NewBond(a1, a2, BondOrder.Triple));
            var gen = factory.GetInChIGenerator(ac);
            Assert.AreEqual(InChIReturnCode.Ok, gen.ReturnStatus);
            Assert.AreEqual("InChI=1S/C2H2/c1-2/h1-2H", gen.InChI);
        }

        /// <summary>
        /// Tests 2D coordinates are correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetStandardInChIEandZ12Dichloroethene2D()
        {
            // (E)-1,2-dichloroethene
            var acE = builder.NewAtomContainer();
            var a1E = builder.NewAtom("C", new Vector2(2.866, -0.250));
            var a2E = builder.NewAtom("C", new Vector2(3.732, 0.250));
            var a3E = builder.NewAtom("Cl", new Vector2(2.000, 2.500));
            var a4E = builder.NewAtom("Cl", new Vector2(4.598, -0.250));
            a1E.ImplicitHydrogenCount = 1;
            a2E.ImplicitHydrogenCount = 1;
            acE.Atoms.Add(a1E);
            acE.Atoms.Add(a2E);
            acE.Atoms.Add(a3E);
            acE.Atoms.Add(a4E);

            acE.Bonds.Add(builder.NewBond(a1E, a2E, BondOrder.Double));
            acE.Bonds.Add(builder.NewBond(a1E, a2E, BondOrder.Double));
            acE.Bonds.Add(builder.NewBond(a1E, a3E, BondOrder.Single));
            acE.Bonds.Add(builder.NewBond(a2E, a4E, BondOrder.Single));

            var genE = factory.GetInChIGenerator(acE);
            Assert.AreEqual(InChIReturnCode.Ok, genE.ReturnStatus);
            Assert.AreEqual("InChI=1S/C2H2Cl2/c3-1-2-4/h1-2H/b2-1+", genE.InChI);

            // (Z)-1,2-dichloroethene
            var acZ = builder.NewAtomContainer();
            var a1Z = builder.NewAtom("C", new Vector2(2.866, -0.440));
            var a2Z = builder.NewAtom("C", new Vector2(3.732, 0.060));
            var a3Z = builder.NewAtom("Cl", new Vector2(2.000, 0.060));
            var a4Z = builder.NewAtom("Cl", new Vector2(3.732, 1.060));
            a1Z.ImplicitHydrogenCount = 1;
            a2Z.ImplicitHydrogenCount = 1;
            acZ.Atoms.Add(a1Z);
            acZ.Atoms.Add(a2Z);
            acZ.Atoms.Add(a3Z);
            acZ.Atoms.Add(a4Z);

            acZ.Bonds.Add(builder.NewBond(a1Z, a2Z, BondOrder.Double));
            acZ.Bonds.Add(builder.NewBond(a1Z, a2Z, BondOrder.Double));
            acZ.Bonds.Add(builder.NewBond(a1Z, a3Z, BondOrder.Single));
            acZ.Bonds.Add(builder.NewBond(a2Z, a4Z, BondOrder.Single));

            InChIGenerator genZ = factory.GetInChIGenerator(acZ);
            Assert.AreEqual(InChIReturnCode.Ok, genZ.ReturnStatus);
            Assert.AreEqual("InChI=1S/C2H2Cl2/c3-1-2-4/h1-2H/b2-1-", genZ.InChI);
        }

        /// <summary>
        /// Tests 3D coordinates are correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetStandardInChIFromLandDAlanine3D()
        {
            // L-Alanine
            var acL = builder.NewAtomContainer();
            var a1L = builder.NewAtom("C", new Vector3(-0.358, 0.819, 20.655));
            var a2L = builder.NewAtom("C", new Vector3(-1.598, -0.032, 20.905));
            var a3L = builder.NewAtom("N", new Vector3(-0.275, 2.014, 21.574));
            var a4L = builder.NewAtom("C", new Vector3(0.952, 0.043, 20.838));
            var a5L = builder.NewAtom("O", new Vector3(-2.678, 0.479, 21.093));
            var a6L = builder.NewAtom("O", new Vector3(-1.596, -1.239, 20.958));
            a1L.ImplicitHydrogenCount = 1;
            a3L.ImplicitHydrogenCount = 2;
            a4L.ImplicitHydrogenCount = 3;
            a5L.ImplicitHydrogenCount = 1;
            acL.Atoms.Add(a1L);
            acL.Atoms.Add(a2L);
            acL.Atoms.Add(a3L);
            acL.Atoms.Add(a4L);
            acL.Atoms.Add(a5L);
            acL.Atoms.Add(a6L);

            acL.Bonds.Add(builder.NewBond(a1L, a2L, BondOrder.Single));
            acL.Bonds.Add(builder.NewBond(a1L, a3L, BondOrder.Single));
            acL.Bonds.Add(builder.NewBond(a1L, a4L, BondOrder.Single));
            acL.Bonds.Add(builder.NewBond(a2L, a5L, BondOrder.Single));
            acL.Bonds.Add(builder.NewBond(a2L, a6L, BondOrder.Double));

            var genL = factory.GetInChIGenerator(acL);
            Assert.AreEqual(InChIReturnCode.Ok, genL.ReturnStatus);
            Assert.AreEqual("InChI=1S/C3H7NO2/c1-2(4)3(5)6/h2H,4H2,1H3,(H,5,6)/t2-/m0/s1", genL.InChI);

            // D-Alanine
            var acD = builder.NewAtomContainer();
            var a1D = builder.NewAtom("C", new Vector3(0.358, 0.819, 20.655));
            var a2D = builder.NewAtom("C", new Vector3(1.598, -0.032, 20.905));
            var a3D = builder.NewAtom("N", new Vector3(0.275, 2.014, 21.574));
            var a4D = builder.NewAtom("C", new Vector3(-0.952, 0.043, 20.838));
            var a5D = builder.NewAtom("O", new Vector3(2.678, 0.479, 21.093));
            var a6D = builder.NewAtom("O", new Vector3(1.596, -1.239, 20.958));
            a1D.ImplicitHydrogenCount = 1;
            a3D.ImplicitHydrogenCount = 2;
            a4D.ImplicitHydrogenCount = 3;
            a5D.ImplicitHydrogenCount = 1;
            acD.Atoms.Add(a1D);
            acD.Atoms.Add(a2D);
            acD.Atoms.Add(a3D);
            acD.Atoms.Add(a4D);
            acD.Atoms.Add(a5D);
            acD.Atoms.Add(a6D);

            acD.Bonds.Add(builder.NewBond(a1D, a2D, BondOrder.Single));
            acD.Bonds.Add(builder.NewBond(a1D, a3D, BondOrder.Single));
            acD.Bonds.Add(builder.NewBond(a1D, a4D, BondOrder.Single));
            acD.Bonds.Add(builder.NewBond(a2D, a5D, BondOrder.Single));
            acD.Bonds.Add(builder.NewBond(a2D, a6D, BondOrder.Double));

            var genD = factory.GetInChIGenerator(acD);
            Assert.AreEqual(InChIReturnCode.Ok, genD.ReturnStatus);
            Assert.AreEqual("InChI=1S/C3H7NO2/c1-2(4)3(5)6/h2H,4H2,1H3,(H,5,6)/t2-/m1/s1", genD.InChI);
        }

        [TestMethod()]
        public void TestTetrahedralStereo()
        {
            // L-Alanine
            var acL = builder.NewAtomContainer();
            var ligandAtoms = new IAtom[4];
            var a1 = builder.NewAtom("C");
            var a1H = builder.NewAtom("H");
            ligandAtoms[0] = a1H;
            var a2 = builder.NewAtom("C");
            ligandAtoms[1] = a2;
            var a3 = builder.NewAtom("N");
            ligandAtoms[2] = a3;
            var a4 = builder.NewAtom("C");
            ligandAtoms[3] = a4;
            var a5 = builder.NewAtom("O");
            var a6 = builder.NewAtom("O");
            a1.ImplicitHydrogenCount = 0;
            a3.ImplicitHydrogenCount = 2;
            a4.ImplicitHydrogenCount = 3;
            a5.ImplicitHydrogenCount = 1;
            acL.Atoms.Add(a1);
            acL.Atoms.Add(a1H);
            acL.Atoms.Add(a2);
            acL.Atoms.Add(a3);
            acL.Atoms.Add(a4);
            acL.Atoms.Add(a5);
            acL.Atoms.Add(a6);

            acL.Bonds.Add(builder.NewBond(a1, a1H, BondOrder.Single));
            acL.Bonds.Add(builder.NewBond(a1, a2, BondOrder.Single));
            acL.Bonds.Add(builder.NewBond(a1, a3, BondOrder.Single));
            acL.Bonds.Add(builder.NewBond(a1, a4, BondOrder.Single));
            acL.Bonds.Add(builder.NewBond(a2, a5, BondOrder.Single));
            acL.Bonds.Add(builder.NewBond(a2, a6, BondOrder.Double));

            var chirality = new TetrahedralChirality(a1, ligandAtoms, TetrahedralStereo.AntiClockwise);
            acL.StereoElements.Add(chirality);

            var genL = factory.GetInChIGenerator(acL);
            Assert.AreEqual(InChIReturnCode.Ok, genL.ReturnStatus);
            Assert.AreEqual("InChI=1S/C3H7NO2/c1-2(4)3(5)6/h2H,4H2,1H3,(H,5,6)/t2-/m0/s1", genL.InChI);
        }

        [TestMethod()]
        public void TestDoubleBondStereochemistry()
        {
            // (E)-1,2-dichloroethene
            var acE = builder.NewAtomContainer();
            var a1E = builder.NewAtom("C");
            var a2E = builder.NewAtom("C");
            var a3E = builder.NewAtom("Cl");
            var a4E = builder.NewAtom("Cl");
            a1E.ImplicitHydrogenCount = 1;
            a2E.ImplicitHydrogenCount = 1;
            acE.Atoms.Add(a1E);
            acE.Atoms.Add(a2E);
            acE.Atoms.Add(a3E);
            acE.Atoms.Add(a4E);

            acE.Bonds.Add(builder.NewBond(a1E, a2E, BondOrder.Double));
            acE.Bonds.Add(builder.NewBond(a1E, a3E, BondOrder.Single));
            acE.Bonds.Add(builder.NewBond(a2E, a4E, BondOrder.Single));

            var ligands = new IBond[2];
            ligands[0] = acE.Bonds[1];
            ligands[1] = acE.Bonds[2];
            var stereo = new DoubleBondStereochemistry(acE.Bonds[0], ligands, DoubleBondConformation.Opposite);
            acE.StereoElements.Add(stereo);

            var genE = factory.GetInChIGenerator(acE);
            Assert.AreEqual(InChIReturnCode.Ok, genE.ReturnStatus);
            Assert.AreEqual("InChI=1S/C2H2Cl2/c3-1-2-4/h1-2H/b2-1+", genE.InChI);
        }

        // @cdk.bug 1295
        [TestMethod()]
        public void Bug1295()
        {
            using (var reader = new MDLV2000Reader(ResourceLoader.GetAsStream("NCDK.Data.MDL.bug1295.mol")))
            {
                var container = reader.Read(builder.NewAtomContainer());
                var generator = factory.GetInChIGenerator(container);
                Assert.AreEqual("InChI=1S/C7H15NO/c1-4-7(3)6-8-9-5-2/h6-7H,4-5H2,1-3H3", generator.InChI);
            }
        }

        [TestMethod()]
        public void R_penta_2_3_diene_impl_h()
        {
            var m = builder.NewAtomContainer();
            m.Atoms.Add(builder.NewAtom("CH3"));
            m.Atoms.Add(builder.NewAtom("CH"));
            m.Atoms.Add(builder.NewAtom("C"));
            m.Atoms.Add(builder.NewAtom("CH"));
            m.Atoms.Add(builder.NewAtom("CH3"));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Double);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Double);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);

            var atoms = new int[][]
                {
                    new[] {0, 1, 3, 4}, new[] {1, 0, 3, 4},
                    new[] {1, 0, 4, 3}, new[] {0, 1, 4, 3},
                    new[] {4, 3, 1, 0}, new[] {4, 3, 0, 1},
                    new[] {3, 4, 0, 1}, new[] {3, 4, 1, 0},
                };
            var stereos = new TetrahedralStereo[]
                {
                    TetrahedralStereo.AntiClockwise, TetrahedralStereo.Clockwise, TetrahedralStereo.AntiClockwise, TetrahedralStereo.Clockwise,
                    TetrahedralStereo.AntiClockwise, TetrahedralStereo.Clockwise, TetrahedralStereo.AntiClockwise, TetrahedralStereo.Clockwise,
                };

            for (int i = 0; i < atoms.Length; i++)
            {
                var element = new ExtendedTetrahedral(
                    m.Atoms[2],
                    new IAtom[] {
                        m.Atoms[atoms[i][0]],
                        m.Atoms[atoms[i][1]],
                        m.Atoms[atoms[i][2]],
                        m.Atoms[atoms[i][3]]},
                    stereos[i]);
                m.SetStereoElements(new[] { element });

                var generator = factory.GetInChIGenerator(m);
                Assert.AreEqual("InChI=1S/C5H8/c1-3-5-4-2/h3-4H,1-2H3/t5-/m0/s1", generator.InChI);
            }
        }

        [TestMethod()]
        public void S_penta_2_3_diene_impl_h()
        {
            var m = builder.NewAtomContainer();
            m.Atoms.Add(builder.NewAtom("CH3"));
            m.Atoms.Add(builder.NewAtom("CH"));
            m.Atoms.Add(builder.NewAtom("C"));
            m.Atoms.Add(builder.NewAtom("CH"));
            m.Atoms.Add(builder.NewAtom("CH3"));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Double);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Double);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);

            var atoms = new int[][] {
                new[] {0, 1, 3, 4}, new[] {1, 0, 3, 4},
                new[] {1, 0, 4, 3}, new[] {0, 1, 4, 3},
                new[] {4, 3, 1, 0}, new[] {4, 3, 0, 1},
                new[] {3, 4, 0, 1}, new[] {3, 4, 1, 0}, };
            var stereos = new TetrahedralStereo[]
                {
                    TetrahedralStereo.Clockwise, TetrahedralStereo.AntiClockwise, TetrahedralStereo.Clockwise, TetrahedralStereo.AntiClockwise,
                    TetrahedralStereo.Clockwise, TetrahedralStereo.AntiClockwise, TetrahedralStereo.Clockwise, TetrahedralStereo.AntiClockwise,
                };

            for (int i = 0; i < atoms.Length; i++)
            {
                var element = new ExtendedTetrahedral(
                    m.Atoms[2],
                    new IAtom[] {
                        m.Atoms[atoms[i][0]],
                        m.Atoms[atoms[i][1]],
                        m.Atoms[atoms[i][2]],
                        m.Atoms[atoms[i][3]]},
                    stereos[i]);

                m.SetStereoElements(new[] { element });
                var generator = factory.GetInChIGenerator(m);
                Assert.AreEqual("InChI=1S/C5H8/c1-3-5-4-2/h3-4H,1-2H3/t5-/m1/s1", generator.InChI);
            }
        }

        // why it does not work? [TestMethod()]
        public void R_penta_2_3_diene_expl_h()
        {
            var m = builder.NewAtomContainer();
            m.Atoms.Add(builder.NewAtom("CH3"));
            m.Atoms.Add(builder.NewAtom("C"));
            m.Atoms.Add(builder.NewAtom("C"));
            m.Atoms.Add(builder.NewAtom("C"));
            m.Atoms.Add(builder.NewAtom("CH3"));
            m.Atoms.Add(builder.NewAtom("H"));
            m.Atoms.Add(builder.NewAtom("H"));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Double);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Double);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[6], BondOrder.Single);

            int[][] atoms = new int[][]{
                new[] {0, 5, 6, 4}, new[] {5, 0, 6, 4},
                new[] {5, 0, 4, 6}, new[] {0, 5, 4, 6},
                new[] {4, 6, 5, 0}, new[] {4, 6, 0, 5},
                new[] {6, 4, 0, 5} ,new[] {6, 4, 5, 0}, };
            var stereos = new TetrahedralStereo[]
                {
                    TetrahedralStereo.AntiClockwise, TetrahedralStereo.Clockwise, TetrahedralStereo.Clockwise, TetrahedralStereo.Clockwise,
                    TetrahedralStereo.Clockwise, TetrahedralStereo.Clockwise, TetrahedralStereo.AntiClockwise, TetrahedralStereo.Clockwise
                };

            for (int i = 0; i < atoms.Length; i++)
            {
                var element = new ExtendedTetrahedral(
                   m.Atoms[2],
                   new IAtom[] {
                        m.Atoms[atoms[i][0]],
                        m.Atoms[atoms[i][1]],
                        m.Atoms[atoms[i][2]],
                        m.Atoms[atoms[i][3]]},
                   stereos[i]);
                m.SetStereoElements(new[] { element });

                var generator = factory.GetInChIGenerator(m);
                Assert.AreEqual("InChI=1S/C5H8/c1-3-5-4-2/h3-4H,1-2H3/t5-/m0/s1", generator.InChI);
            }
        }

        // why it does not work? [TestMethod()]
        public void S_penta_2_3_diene_expl_h()
        {
            var m = builder.NewAtomContainer();
            m.Atoms.Add(builder.NewAtom("CH3"));
            m.Atoms.Add(builder.NewAtom("C"));
            m.Atoms.Add(builder.NewAtom("C"));
            m.Atoms.Add(builder.NewAtom("C"));
            m.Atoms.Add(builder.NewAtom("CH3"));
            m.Atoms.Add(builder.NewAtom("H"));
            m.Atoms.Add(builder.NewAtom("H"));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Double);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Double);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[6], BondOrder.Single);

            int[][] atoms = new int[][]
                {
                    new[] {0, 5, 6, 4}, new[] {5, 0, 6, 4},
                    new[] {5, 0, 4, 6}, new[] {0, 5, 4, 6},
                    new[] {4, 6, 5, 0}, new[] {4, 6, 0, 5},
                    new[] {6, 4, 0, 5}, new[] {6, 4, 5, 0},
                };
            var stereos = new TetrahedralStereo[]
                {
                    TetrahedralStereo.Clockwise, TetrahedralStereo.Clockwise, TetrahedralStereo.Clockwise, TetrahedralStereo.AntiClockwise,
                    TetrahedralStereo.Clockwise, TetrahedralStereo.AntiClockwise, TetrahedralStereo.Clockwise, TetrahedralStereo.Clockwise,
                };

            for (int i = 0; i < atoms.Length; i++)
            {
                var element = new ExtendedTetrahedral(
                   m.Atoms[2],
                   new IAtom[] {
                        m.Atoms[atoms[i][0]],
                        m.Atoms[atoms[i][1]],
                        m.Atoms[atoms[i][2]],
                        m.Atoms[atoms[i][3]]},
                   stereos[i]);
                m.SetStereoElements(new[] { element });

                var generator = factory.GetInChIGenerator(m);
                Assert.AreEqual("InChI=1S/C5H8/c1-3-5-4-2/h3-4H,1-2H3/t5-/m1/s1", generator.InChI);
            }
        }

        // if this test hits the timeout it's likely the users Locale is mixed, the
        // InChI library was loaded in one mode and java is in another, the issue
        // is InChI takes timeout in seconds and fractional seconds will be either
        // 0.1 or 0,1 depending on locale.
        [TestMethod()]
        [Timeout(500)]
        public void Timeout()
        {
            var smipar = new SmilesParser(builder);
            var smiles = "C(CCCNC(=N)N)(COCC(COP([O])(=O)OCCCCCCNC(NC1=CC(=C(C=C1)C2(C3=CC=C(C=C3OC=4C2=CC=C(C4)O)O)C)C(=O)[O])=S)OP(=O)([O])OCC(COCC(CCC/[NH]=C(\\[NH])/N)(CCCNC(=N)N)CCCNC(=N)N)OP(=O)([O])OCC(COCC(CCCNC(=N)N)(CCC/[NH]=C(\\[NH])/N)CCCNC(=N)N)OP(OCC(COCC(CCCNC(=N)N)(CCCNC(=N)N)CCC/[NH]=C(\\[NH])/N)OP(=O)([O])OCC(COCC(CCCNC(=N)N)(CCCNC(N)=N)CCC/[NH]=C(/N)\\[NH])OP([O])(=O)CCC(COCC(CCCNC(=N)N)(CCC/[NH]=C(\\[NH])/N)CCCNC(=N)N)OP([O])(=O)OCC(COCC(CCCNC(N)=N)(CCCNC(N)=N)CCC/[NH]=C(\\[NH])/N)OP(OCC(COCC(CCCNC(N)=N)(CCC/[NH]=C(/N)\\[NH])CCCNC(N)=N)O=P([O])(OCC(COP(=OC(COCC(CCC/[NH]=C(\\[NH])/N)(CCCNC(N)=N)CCCNC(N)=N)COP([O])(=O)OC(COP(OC(COCC(CCCNC(=N)N)(CCC/[NH]=C(\\[NH])/N)CCCNC(=N)N)COP(OC(COCC(CCCNC(=N)N)(CCC/[NH]=C(\\[NH])/N)CCCNC(=N)N)COP([O])(=O)OC(COP(OC(COP(OC(COP(=O)([O])OC(COCC(CCC/[NH]=C(/N)\\[NH])(CCCNC(N)=N)CCCNC(=N)N)COP([O])(=O)OCCCCCCNC(NC=5C=CC(=C(C5)C(=O)[O])C6(C7=CC=C(C=C7OC=8C6=CC=C(C8)O)O)C)=S)COCC(CCCNC(N)=N)(CCC/[NH]=C(\\[NH])/N)CCCNC(=N)N)([O])=O)COCC(CCCNC(=N)N)(CCC/[NH]=C(\\[NH])/N)CCCNC(=N)N)([O])=O)COCC(CCCNC(=N)N)(CCCNC(=N)N)CCC/[NH]=C(\\[NH])/N)([O])=O)([O])=O)COCC(CCC/[NH]=C(/N)\\[NH])(CCCNC(=N)N)CCCNC(=N)N)([O])[O])(C)COP(OCCCCCCO)(=O)[O])[O])(=O)[O])([O])=O)(CCC/[NH]=C(\\[NH])/[NH])CCCNC(=N)N";
            var mol = smipar.ParseSmiles(smiles);
            var inchiFact = InChIGeneratorFactory.Instance;
            var generator = inchiFact.GetInChIGenerator(mol, "W0.01");
            Assert.AreEqual(InChIReturnCode.Error, generator.ReturnStatus);
            Assert.IsTrue(
                generator.Log.Contains("Time limit exceeded")
             || generator.Log.Contains("Structure normalization timeout"));
        }

        /// <summary>
        /// Standard inchi for guanine.
        /// </summary>
        // @cdk.smiles NC1=NC2=C(N=CN2)C(=O)N1 
        [TestMethod()]
        public void Guanine_std()
        {
            var smipar = new SmilesParser(builder);
            var smiles = "NC1=NC2=C(N=CN2)C(=O)N1";
            var mol = smipar.ParseSmiles(smiles);
            var inchiFact = InChIGeneratorFactory.Instance;
            InChIGenerator inchigen = inchiFact.GetInChIGenerator(mol);
            Assert.AreEqual(InChIReturnCode.Ok, inchigen.ReturnStatus);
            Assert.AreEqual("InChI=1S/C5H5N5O/c6-5-9-3-2(4(11)10-5)7-1-8-3/h1H,(H4,6,7,8,9,10,11)", inchigen.InChI);
        }

        /// <summary>
        /// Ensures KET (Keto-enol) option can be passed to InChI for guanine.
        /// </summary>
        // @cdk.smiles NC1=NC2=C(N=CN2)C(=O)N1 
        [TestMethod()]
        public void Guanine_ket()
        {
            var smipar = new SmilesParser(builder);
            var smiles = "NC1=NC2=C(N=CN2)C(=O)N1";
            var mol = smipar.ParseSmiles(smiles);
            var inchiFact = InChIGeneratorFactory.Instance;
            var inchigen = inchiFact.GetInChIGenerator(mol, "KET");
            Assert.AreEqual(InChIReturnCode.Ok, inchigen.ReturnStatus);
            Assert.AreEqual("InChI=1/C5H5N5O/c6-5-9-3-2(4(11)10-5)7-1-8-3/h1H,(H4,2,6,7,8,9,10,11)", inchigen.InChI);
        }

        /// <summary>
        /// Standard test for aminopropenol.
        /// </summary>
        // @cdk.smiles N\C=C/C=O 
        [TestMethod()]
        public void Aminopropenol_std()
        {
            var smipar = new SmilesParser(builder);
            var smiles = "N\\C=C/C=O";
            var mol = smipar.ParseSmiles(smiles);
            var inchiFact = InChIGeneratorFactory.Instance;
            var stdinchi = inchiFact.GetInChIGenerator(mol);
            Assert.AreEqual(InChIReturnCode.Ok, stdinchi.ReturnStatus);
            Assert.AreEqual("InChI=1S/C3H5NO/c4-2-1-3-5/h1-3H,4H2/b2-1-", stdinchi.InChI);
            var inchigen = inchiFact.GetInChIGenerator(mol, "15T");
            Assert.AreEqual(InChIReturnCode.Ok, inchigen.ReturnStatus);
            Assert.AreEqual("InChI=1/C3H5NO/c4-2-1-3-5/h1-3H,(H2,4,5)", inchigen.InChI);
        }

        /// <summary>
        /// Ensures 15T (1,5-shifts) option can be passed to InChI for aminopropenol.
        /// </summary>
        // @cdk.smiles N\C=C/C=O 
        [TestMethod()]
        public void Aminopropenol_15T()
        {
            var builder = CDK.Builder;
            var smipar = new SmilesParser(builder);
            var smiles = "N\\C=C/C=O";
            var mol = smipar.ParseSmiles(smiles);
            var inchiFact = InChIGeneratorFactory.Instance;
            var inchigen = inchiFact.GetInChIGenerator(mol, "15T");
            Assert.AreEqual(InChIReturnCode.Ok, inchigen.ReturnStatus);
            Assert.AreEqual("InChI=1/C3H5NO/c4-2-1-3-5/h1-3H,(H2,4,5)", inchigen.InChI);
        }

        /// <summary>
        /// Ensures default timeout option is passed with proper switch character.
        /// </summary>
        [TestMethod()]
        public void TestFiveSecondTimeoutFlag()
        {
            var ac = builder.NewAtomContainer();
            ac.Atoms.Add(builder.NewAtom("C"));
            var factory = InChIGeneratorFactory.Instance;
            var generator = factory.GetInChIGenerator(ac);

            var flagChar = Environment.OSVersion.Platform < PlatformID.Unix ? "/" : "-";
            Assert.IsTrue(generator.Input.Options.Contains(flagChar + "W5"));
        }
    }
}
