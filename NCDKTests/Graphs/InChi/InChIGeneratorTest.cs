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
using NCDK.Default;
using NCDK.IO;
using NCDK.NInChI;
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
        protected static InChIGeneratorFactory Factory { get; set; } = null;

        protected InChIGeneratorFactory GetFactory()
        {
            if (Factory == null)
            {
                Factory = InChIGeneratorFactory.Instance;
            }
            return Factory;
        }

        /// <summary>
        /// Tests element name is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetInchiFromChlorineAtom()
        {
            IAtomContainer ac = new AtomContainer();
            ac.Atoms.Add(new Atom("ClH"));
            InChIGenerator gen = GetFactory().GetInChIGenerator(ac, "FixedH");
            Assert.AreEqual(gen.ReturnStatus, INCHI_RET.OKAY);
            Assert.AreEqual("InChI=1/ClH/h1H", gen.InChI);
        }

        [TestMethod()]
        public void TestGetLog()
        {
            IAtomContainer ac = new AtomContainer();
            ac.Atoms.Add(new Atom("Cl"));
            InChIGenerator gen = GetFactory().GetInChIGenerator(ac, "FixedH");
            Assert.IsNotNull(gen.Log);
        }

        [TestMethod()]
        public void TestGetAuxInfo()
        {
            IAtomContainer ac = new AtomContainer();
            IAtom a1 = new Atom("C");
            IAtom a2 = new Atom("C");
            a1.ImplicitHydrogenCount = 3;
            a2.ImplicitHydrogenCount = 3;
            ac.Atoms.Add(a1);
            ac.Atoms.Add(a2);
            ac.Bonds.Add(new Bond(a1, a2, BondOrder.Single));
            InChIGenerator gen = GetFactory().GetInChIGenerator(ac, "");
            Assert.IsNotNull(gen.AuxInfo);
            Assert.IsTrue(gen.AuxInfo.StartsWith("AuxInfo="));
        }

        [TestMethod()]
        public void TestGetMessage()
        {
            IAtomContainer ac = new AtomContainer();
            ac.Atoms.Add(new Atom("Cl"));
            InChIGenerator gen = GetFactory().GetInChIGenerator(ac, "FixedH");
            Assert.IsNull(gen.Message, "Because this generation should work, I expected a null message string.");
        }

        [TestMethod()]
        public void TestGetWarningMessage()
        {
            IAtomContainer ac = new AtomContainer();
            var cl = new Atom("Cl");
            var h = new Atom("H");
            ac.Atoms.Add(cl);
            ac.Atoms.Add(h);
            ac.AddBond(cl, h, BondOrder.Triple);
            InChIGenerator gen = GetFactory().GetInChIGenerator(ac);
            Assert.IsNotNull(gen.Message);
            Assert.IsTrue(gen.Message.Contains("Accepted unusual valence"));
        }

        /// <summary>
        /// Tests charge is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetInchiFromLithiumIon()
        {
            IAtomContainer ac = new AtomContainer();
            IAtom a = new Atom("Li");
            a.FormalCharge = +1;
            ac.Atoms.Add(a);
            InChIGenerator gen = GetFactory().GetInChIGenerator(ac, "FixedH");
            Assert.AreEqual(gen.ReturnStatus, INCHI_RET.OKAY);
            Assert.AreEqual("InChI=1/Li/q+1", gen.InChI);
        }

        /// <summary>
       /// Tests isotopic mass is correctly passed to InChI.
       /// </summary>
        [TestMethod()]
        public void TestGetInchiFromChlorine37Atom()
        {
            IAtomContainer ac = new AtomContainer();
            IAtom a = new Atom("ClH");
            a.MassNumber = 37;
            ac.Atoms.Add(a);
            InChIGenerator gen = GetFactory().GetInChIGenerator(ac, "FixedH");
            Assert.AreEqual(gen.ReturnStatus, INCHI_RET.OKAY);
            Assert.AreEqual("InChI=1/ClH/h1H/i1+2", gen.InChI);
        }

        /// <summary>
        /// Tests implicit hydrogen count is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetInchiFromHydrogenChlorideImplicitH()
        {
            IAtomContainer ac = new AtomContainer();
            IAtom a = new Atom("Cl");
            a.ImplicitHydrogenCount = 1;
            ac.Atoms.Add(a);
            InChIGenerator gen = GetFactory().GetInChIGenerator(ac, "FixedH");
            Assert.AreEqual(gen.ReturnStatus, INCHI_RET.OKAY);
            Assert.AreEqual("InChI=1/ClH/h1H", gen.InChI);
        }

        /// <summary>
        /// Tests radical state is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetInchiFromMethylRadical()
        {
            IAtomContainer ac = new AtomContainer();
            IAtom a = new Atom("C");
            a.ImplicitHydrogenCount = 3;
            ac.Atoms.Add(a);
            ac.SingleElectrons.Add(new SingleElectron(a));
            InChIGenerator gen = GetFactory().GetInChIGenerator(ac, "FixedH");
            Assert.AreEqual(gen.ReturnStatus, INCHI_RET.OKAY);
            Assert.AreEqual("InChI=1/CH3/h1H3", gen.InChI);
        }

        /// <summary>
        /// Tests single bond is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetInchiFromEthane()
        {
            IAtomContainer ac = new AtomContainer();
            IAtom a1 = new Atom("C");
            IAtom a2 = new Atom("C");
            a1.ImplicitHydrogenCount = 3;
            a2.ImplicitHydrogenCount = 3;
            ac.Atoms.Add(a1);
            ac.Atoms.Add(a2);
            ac.Bonds.Add(new Bond(a1, a2, BondOrder.Single));
            InChIGenerator gen = GetFactory().GetInChIGenerator(ac, "FixedH");
            Assert.AreEqual(gen.ReturnStatus, INCHI_RET.OKAY);
            Assert.AreEqual("InChI=1/C2H6/c1-2/h1-2H3", gen.InChI);
            Assert.AreEqual("OTMSDBZUPAUEDD-UHFFFAOYNA-N", gen.GetInChIKey());
        }

        /// <summary>
        /// Test generation of non-standard InChIs.
        /// </summary>
        /// <exception cref="Exception"></exception>
        /// <seealso cref=""/>
        // @cdk.bug 1384
        [TestMethod()]
        public void NonStandardInChIWithEnumOptions()
        {
            IAtomContainer ac = new AtomContainer();
            IAtom a1 = new Atom("C");
            IAtom a2 = new Atom("C");
            a1.ImplicitHydrogenCount = 3;
            a2.ImplicitHydrogenCount = 3;
            ac.Atoms.Add(a1);
            ac.Atoms.Add(a2);
            ac.Bonds.Add(new Bond(a1, a2, BondOrder.Single));
            List<INCHI_OPTION> options = new List<INCHI_OPTION>();
            options.Add(INCHI_OPTION.FixedH);
            options.Add(INCHI_OPTION.SAbs);
            options.Add(INCHI_OPTION.SAsXYZ);
            options.Add(INCHI_OPTION.SPXYZ);
            options.Add(INCHI_OPTION.FixSp3Bug);
            options.Add(INCHI_OPTION.AuxNone);
            InChIGenerator gen = GetFactory().GetInChIGenerator(ac, options);
            Assert.AreEqual(gen.ReturnStatus, INCHI_RET.OKAY);
            Assert.AreEqual("InChI=1/C2H6/c1-2/h1-2H3", gen.InChI);
            Assert.AreEqual("OTMSDBZUPAUEDD-UHFFFAOYNA-N", gen.GetInChIKey());
        }

        /// <summary>
        /// Tests double bond is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetInchiFromEthene()
        {
            IAtomContainer ac = new AtomContainer();
            IAtom a1 = new Atom("C");
            IAtom a2 = new Atom("C");
            a1.ImplicitHydrogenCount = 2;
            a2.ImplicitHydrogenCount = 2;
            ac.Atoms.Add(a1);
            ac.Atoms.Add(a2);
            ac.Bonds.Add(new Bond(a1, a2, BondOrder.Double));
            InChIGenerator gen = GetFactory().GetInChIGenerator(ac, "FixedH");
            Assert.AreEqual(gen.ReturnStatus, INCHI_RET.OKAY);
            Assert.AreEqual("InChI=1/C2H4/c1-2/h1-2H2", gen.InChI);
        }

        /// <summary>
        /// Tests triple bond is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetInchiFromEthyne()
        {
            IAtomContainer ac = new AtomContainer();
            IAtom a1 = new Atom("C");
            IAtom a2 = new Atom("C");
            a1.ImplicitHydrogenCount = 1;
            a2.ImplicitHydrogenCount = 1;
            ac.Atoms.Add(a1);
            ac.Atoms.Add(a2);
            ac.Bonds.Add(new Bond(a1, a2, BondOrder.Triple));
            InChIGenerator gen = GetFactory().GetInChIGenerator(ac, "FixedH");
            Assert.AreEqual(gen.ReturnStatus, INCHI_RET.OKAY);
            Assert.AreEqual("InChI=1/C2H2/c1-2/h1-2H", gen.InChI);
        }

        /// <summary>
        /// Tests 2D coordinates are correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetInchiEandZ12Dichloroethene2D()
        {
            // (E)-1,2-dichloroethene
            IAtomContainer acE = new AtomContainer();
            IAtom a1E = new Atom("C", new Vector2(2.866, -0.250));
            IAtom a2E = new Atom("C", new Vector2(3.732, 0.250));
            IAtom a3E = new Atom("Cl", new Vector2(2.000, 2.500));
            IAtom a4E = new Atom("Cl", new Vector2(4.598, -0.250));
            a1E.ImplicitHydrogenCount = 1;
            a2E.ImplicitHydrogenCount = 1;
            acE.Atoms.Add(a1E);
            acE.Atoms.Add(a2E);
            acE.Atoms.Add(a3E);
            acE.Atoms.Add(a4E);

            acE.Bonds.Add(new Bond(a1E, a2E, BondOrder.Double));
            acE.Bonds.Add(new Bond(a1E, a2E, BondOrder.Double));
            acE.Bonds.Add(new Bond(a1E, a3E, BondOrder.Single));
            acE.Bonds.Add(new Bond(a2E, a4E, BondOrder.Single));

            InChIGenerator genE = GetFactory().GetInChIGenerator(acE, "FixedH");
            Assert.AreEqual(genE.ReturnStatus, INCHI_RET.OKAY);
            Assert.AreEqual("InChI=1/C2H2Cl2/c3-1-2-4/h1-2H/b2-1+", genE.InChI);

            // (Z)-1,2-dichloroethene
            IAtomContainer acZ = new AtomContainer();
            IAtom a1Z = new Atom("C", new Vector2(2.866, -0.440));
            IAtom a2Z = new Atom("C", new Vector2(3.732, 0.060));
            IAtom a3Z = new Atom("Cl", new Vector2(2.000, 0.060));
            IAtom a4Z = new Atom("Cl", new Vector2(3.732, 1.060));
            a1Z.ImplicitHydrogenCount = 1;
            a2Z.ImplicitHydrogenCount = 1;
            acZ.Atoms.Add(a1Z);
            acZ.Atoms.Add(a2Z);
            acZ.Atoms.Add(a3Z);
            acZ.Atoms.Add(a4Z);

            acZ.Bonds.Add(new Bond(a1Z, a2Z, BondOrder.Double));
            acZ.Bonds.Add(new Bond(a1Z, a2Z, BondOrder.Double));
            acZ.Bonds.Add(new Bond(a1Z, a3Z, BondOrder.Single));
            acZ.Bonds.Add(new Bond(a2Z, a4Z, BondOrder.Single));

            InChIGenerator genZ = GetFactory().GetInChIGenerator(acZ, "FixedH");
            Assert.AreEqual(genZ.ReturnStatus, INCHI_RET.OKAY);
            Assert.AreEqual("InChI=1/C2H2Cl2/c3-1-2-4/h1-2H/b2-1-", genZ.InChI);
        }

        /// <summary>
        /// Tests 3D coordinates are correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetInchiFromLandDAlanine3D()
        {
            // L-Alanine
            IAtomContainer acL = new AtomContainer();
            IAtom a1L = new Atom("C", new Vector3(-0.358, 0.819, 20.655));
            IAtom a2L = new Atom("C", new Vector3(-1.598, -0.032, 20.905));
            IAtom a3L = new Atom("N", new Vector3(-0.275, 2.014, 21.574));
            IAtom a4L = new Atom("C", new Vector3(0.952, 0.043, 20.838));
            IAtom a5L = new Atom("O", new Vector3(-2.678, 0.479, 21.093));
            IAtom a6L = new Atom("O", new Vector3(-1.596, -1.239, 20.958));
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

            acL.Bonds.Add(new Bond(a1L, a2L, BondOrder.Single));
            acL.Bonds.Add(new Bond(a1L, a3L, BondOrder.Single));
            acL.Bonds.Add(new Bond(a1L, a4L, BondOrder.Single));
            acL.Bonds.Add(new Bond(a2L, a5L, BondOrder.Single));
            acL.Bonds.Add(new Bond(a2L, a6L, BondOrder.Double));

            InChIGenerator genL = GetFactory().GetInChIGenerator(acL, "FixedH");
            Assert.AreEqual(genL.ReturnStatus, INCHI_RET.OKAY);
            Assert.AreEqual("InChI=1/C3H7NO2/c1-2(4)3(5)6/h2H,4H2,1H3,(H,5,6)/t2-/m0/s1/f/h5H", genL.InChI);

            // D-Alanine
            IAtomContainer acD = new AtomContainer();
            IAtom a1D = new Atom("C", new Vector3(0.358, 0.819, 20.655));
            IAtom a2D = new Atom("C", new Vector3(1.598, -0.032, 20.905));
            IAtom a3D = new Atom("N", new Vector3(0.275, 2.014, 21.574));
            IAtom a4D = new Atom("C", new Vector3(-0.952, 0.043, 20.838));
            IAtom a5D = new Atom("O", new Vector3(2.678, 0.479, 21.093));
            IAtom a6D = new Atom("O", new Vector3(1.596, -1.239, 20.958));
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

            acD.Bonds.Add(new Bond(a1D, a2D, BondOrder.Single));
            acD.Bonds.Add(new Bond(a1D, a3D, BondOrder.Single));
            acD.Bonds.Add(new Bond(a1D, a4D, BondOrder.Single));
            acD.Bonds.Add(new Bond(a2D, a5D, BondOrder.Single));
            acD.Bonds.Add(new Bond(a2D, a6D, BondOrder.Double));

            InChIGenerator genD = GetFactory().GetInChIGenerator(acD, "FixedH");
            Assert.AreEqual(genD.ReturnStatus, INCHI_RET.OKAY);
            Assert.AreEqual("InChI=1/C3H7NO2/c1-2(4)3(5)6/h2H,4H2,1H3,(H,5,6)/t2-/m1/s1/f/h5H", genD.InChI);
        }

        // ensure only
        [TestMethod()]
        public void ZeroHydrogenCount()
        {
            IAtomContainer ac = new AtomContainer();
            ac.Atoms.Add(new Atom("O"));
            ac.Atoms[0].ImplicitHydrogenCount = 0;
            InChIGenerator gen = GetFactory().GetInChIGenerator(ac);
            Assert.AreEqual(INCHI_RET.OKAY, gen.ReturnStatus);
            Assert.AreEqual("InChI=1S/O", gen.InChI);
        }

        /// <summary>
        /// Tests element name is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetStandardInchiFromChlorineAtom()
        {
            IAtomContainer ac = new AtomContainer();
            ac.Atoms.Add(new Atom("ClH"));
            InChIGenerator gen = GetFactory().GetInChIGenerator(ac);
            Assert.AreEqual(INCHI_RET.OKAY, gen.ReturnStatus);
            Assert.AreEqual("InChI=1S/ClH/h1H", gen.InChI);
        }

        /// <summary>
        /// Tests charge is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetStandardInchiFromLithiumIon()
        {
            IAtomContainer ac = new AtomContainer();
            IAtom a = new Atom("Li");
            a.FormalCharge = +1;
            ac.Atoms.Add(a);
            InChIGenerator gen = GetFactory().GetInChIGenerator(ac);
            Assert.AreEqual(INCHI_RET.OKAY, gen.ReturnStatus);
            Assert.AreEqual("InChI=1S/Li/q+1", gen.InChI);
        }

        /// <summary>
       /// Tests isotopic mass is correctly passed to InChI.
       /// </summary>
        [TestMethod()]
        public void TestGetStandardInchiFromChlorine37Atom()
        {
            IAtomContainer ac = new AtomContainer();
            IAtom a = new Atom("ClH");
            a.MassNumber = 37;
            ac.Atoms.Add(a);
            InChIGenerator gen = GetFactory().GetInChIGenerator(ac);
            Assert.AreEqual(INCHI_RET.OKAY, gen.ReturnStatus);
            Assert.AreEqual("InChI=1S/ClH/h1H/i1+2", gen.InChI);
        }

        /// <summary>
        /// Tests implicit hydrogen count is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetStandardInchiFromHydrogenChlorideImplicitH()
        {
            IAtomContainer ac = new AtomContainer();
            IAtom a = new Atom("Cl");
            a.ImplicitHydrogenCount = 1;
            ac.Atoms.Add(a);
            InChIGenerator gen = GetFactory().GetInChIGenerator(ac);
            Assert.AreEqual(gen.ReturnStatus, INCHI_RET.OKAY);
            Assert.AreEqual("InChI=1S/ClH/h1H", gen.InChI);
        }

        /// <summary>
        /// Tests radical state is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetStandardInchiFromMethylRadical()
        {
            IAtomContainer ac = new AtomContainer();
            IAtom a = new Atom("C");
            a.ImplicitHydrogenCount = 3;
            ac.Atoms.Add(a);
            ac.SingleElectrons.Add(new SingleElectron(a));
            InChIGenerator gen = GetFactory().GetInChIGenerator(ac);
            Assert.AreEqual(INCHI_RET.OKAY, gen.ReturnStatus);
            Assert.AreEqual("InChI=1S/CH3/h1H3", gen.InChI);
        }

        /// <summary>
        /// Tests single bond is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetStandardInchiFromEthane()
        {
            IAtomContainer ac = new AtomContainer();
            IAtom a1 = new Atom("C");
            IAtom a2 = new Atom("C");
            a1.ImplicitHydrogenCount = 3;
            a2.ImplicitHydrogenCount = 3;
            ac.Atoms.Add(a1);
            ac.Atoms.Add(a2);
            ac.Bonds.Add(new Bond(a1, a2, BondOrder.Single));
            InChIGenerator gen = GetFactory().GetInChIGenerator(ac);
            Assert.AreEqual(INCHI_RET.OKAY, gen.ReturnStatus);
            Assert.AreEqual("InChI=1S/C2H6/c1-2/h1-2H3", gen.InChI);
            Assert.AreEqual("OTMSDBZUPAUEDD-UHFFFAOYSA-N", gen.GetInChIKey());
        }

        /// <summary>
        /// Tests double bond is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetStandardInchiFromEthene()
        {
            IAtomContainer ac = new AtomContainer();
            IAtom a1 = new Atom("C");
            IAtom a2 = new Atom("C");
            a1.ImplicitHydrogenCount = 2;
            a2.ImplicitHydrogenCount = 2;
            ac.Atoms.Add(a1);
            ac.Atoms.Add(a2);
            ac.Bonds.Add(new Bond(a1, a2, BondOrder.Double));
            InChIGenerator gen = GetFactory().GetInChIGenerator(ac);
            Assert.AreEqual(INCHI_RET.OKAY, gen.ReturnStatus);
            Assert.AreEqual("InChI=1S/C2H4/c1-2/h1-2H2", gen.InChI);
        }

        /// <summary>
        /// Tests triple bond is correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetStandardInchiFromEthyne()
        {
            IAtomContainer ac = new AtomContainer();
            IAtom a1 = new Atom("C");
            IAtom a2 = new Atom("C");
            a1.ImplicitHydrogenCount = 1;
            a2.ImplicitHydrogenCount = 1;
            ac.Atoms.Add(a1);
            ac.Atoms.Add(a2);
            ac.Bonds.Add(new Bond(a1, a2, BondOrder.Triple));
            InChIGenerator gen = GetFactory().GetInChIGenerator(ac);
            Assert.AreEqual(INCHI_RET.OKAY, gen.ReturnStatus);
            Assert.AreEqual("InChI=1S/C2H2/c1-2/h1-2H", gen.InChI);
        }

        /// <summary>
        /// Tests 2D coordinates are correctly passed to InChI.
        /// </summary>
        [TestMethod()]
        public void TestGetStandardInchiEandZ12Dichloroethene2D()
        {

            // (E)-1,2-dichloroethene
            IAtomContainer acE = new AtomContainer();
            IAtom a1E = new Atom("C", new Vector2(2.866, -0.250));
            IAtom a2E = new Atom("C", new Vector2(3.732, 0.250));
            IAtom a3E = new Atom("Cl", new Vector2(2.000, 2.500));
            IAtom a4E = new Atom("Cl", new Vector2(4.598, -0.250));
            a1E.ImplicitHydrogenCount = 1;
            a2E.ImplicitHydrogenCount = 1;
            acE.Atoms.Add(a1E);
            acE.Atoms.Add(a2E);
            acE.Atoms.Add(a3E);
            acE.Atoms.Add(a4E);

            acE.Bonds.Add(new Bond(a1E, a2E, BondOrder.Double));
            acE.Bonds.Add(new Bond(a1E, a2E, BondOrder.Double));
            acE.Bonds.Add(new Bond(a1E, a3E, BondOrder.Single));
            acE.Bonds.Add(new Bond(a2E, a4E, BondOrder.Single));

            InChIGenerator genE = GetFactory().GetInChIGenerator(acE);
            Assert.AreEqual(INCHI_RET.OKAY, genE.ReturnStatus);
            Assert.AreEqual("InChI=1S/C2H2Cl2/c3-1-2-4/h1-2H/b2-1+", genE.InChI);

            // (Z)-1,2-dichloroethene
            IAtomContainer acZ = new AtomContainer();
            IAtom a1Z = new Atom("C", new Vector2(2.866, -0.440));
            IAtom a2Z = new Atom("C", new Vector2(3.732, 0.060));
            IAtom a3Z = new Atom("Cl", new Vector2(2.000, 0.060));
            IAtom a4Z = new Atom("Cl", new Vector2(3.732, 1.060));
            a1Z.ImplicitHydrogenCount = 1;
            a2Z.ImplicitHydrogenCount = 1;
            acZ.Atoms.Add(a1Z);
            acZ.Atoms.Add(a2Z);
            acZ.Atoms.Add(a3Z);
            acZ.Atoms.Add(a4Z);

            acZ.Bonds.Add(new Bond(a1Z, a2Z, BondOrder.Double));
            acZ.Bonds.Add(new Bond(a1Z, a2Z, BondOrder.Double));
            acZ.Bonds.Add(new Bond(a1Z, a3Z, BondOrder.Single));
            acZ.Bonds.Add(new Bond(a2Z, a4Z, BondOrder.Single));

            InChIGenerator genZ = GetFactory().GetInChIGenerator(acZ);
            Assert.AreEqual(INCHI_RET.OKAY, genZ.ReturnStatus);
            Assert.AreEqual("InChI=1S/C2H2Cl2/c3-1-2-4/h1-2H/b2-1-", genZ.InChI);
        }

        /// <summary>
        /// Tests 3D coordinates are correctly passed to InChI.
        ///
        // @
        /// </summary>
        [TestMethod()]
        public void TestGetStandardInchiFromLandDAlanine3D()
        {

            // L-Alanine
            IAtomContainer acL = new AtomContainer();
            IAtom a1L = new Atom("C", new Vector3(-0.358, 0.819, 20.655));
            IAtom a2L = new Atom("C", new Vector3(-1.598, -0.032, 20.905));
            IAtom a3L = new Atom("N", new Vector3(-0.275, 2.014, 21.574));
            IAtom a4L = new Atom("C", new Vector3(0.952, 0.043, 20.838));
            IAtom a5L = new Atom("O", new Vector3(-2.678, 0.479, 21.093));
            IAtom a6L = new Atom("O", new Vector3(-1.596, -1.239, 20.958));
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

            acL.Bonds.Add(new Bond(a1L, a2L, BondOrder.Single));
            acL.Bonds.Add(new Bond(a1L, a3L, BondOrder.Single));
            acL.Bonds.Add(new Bond(a1L, a4L, BondOrder.Single));
            acL.Bonds.Add(new Bond(a2L, a5L, BondOrder.Single));
            acL.Bonds.Add(new Bond(a2L, a6L, BondOrder.Double));

            InChIGenerator genL = GetFactory().GetInChIGenerator(acL);
            Assert.AreEqual(INCHI_RET.OKAY, genL.ReturnStatus);
            Assert.AreEqual("InChI=1S/C3H7NO2/c1-2(4)3(5)6/h2H,4H2,1H3,(H,5,6)/t2-/m0/s1", genL.InChI);

            // D-Alanine
            IAtomContainer acD = new AtomContainer();
            IAtom a1D = new Atom("C", new Vector3(0.358, 0.819, 20.655));
            IAtom a2D = new Atom("C", new Vector3(1.598, -0.032, 20.905));
            IAtom a3D = new Atom("N", new Vector3(0.275, 2.014, 21.574));
            IAtom a4D = new Atom("C", new Vector3(-0.952, 0.043, 20.838));
            IAtom a5D = new Atom("O", new Vector3(2.678, 0.479, 21.093));
            IAtom a6D = new Atom("O", new Vector3(1.596, -1.239, 20.958));
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

            acD.Bonds.Add(new Bond(a1D, a2D, BondOrder.Single));
            acD.Bonds.Add(new Bond(a1D, a3D, BondOrder.Single));
            acD.Bonds.Add(new Bond(a1D, a4D, BondOrder.Single));
            acD.Bonds.Add(new Bond(a2D, a5D, BondOrder.Single));
            acD.Bonds.Add(new Bond(a2D, a6D, BondOrder.Double));

            InChIGenerator genD = GetFactory().GetInChIGenerator(acD);
            Assert.AreEqual(INCHI_RET.OKAY, genD.ReturnStatus);
            Assert.AreEqual("InChI=1S/C3H7NO2/c1-2(4)3(5)6/h2H,4H2,1H3,(H,5,6)/t2-/m1/s1", genD.InChI);
        }

        [TestMethod()]
        public void TestTetrahedralStereo()
        {
            // L-Alanine
            IAtomContainer acL = new AtomContainer();
            IAtom[] ligandAtoms = new IAtom[4];
            IAtom a1 = new Atom("C");
            IAtom a1H = new Atom("H");
            ligandAtoms[0] = a1H;
            IAtom a2 = new Atom("C");
            ligandAtoms[1] = a2;
            IAtom a3 = new Atom("N");
            ligandAtoms[2] = a3;
            IAtom a4 = new Atom("C");
            ligandAtoms[3] = a4;
            IAtom a5 = new Atom("O");
            IAtom a6 = new Atom("O");
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

            acL.Bonds.Add(new Bond(a1, a1H, BondOrder.Single));
            acL.Bonds.Add(new Bond(a1, a2, BondOrder.Single));
            acL.Bonds.Add(new Bond(a1, a3, BondOrder.Single));
            acL.Bonds.Add(new Bond(a1, a4, BondOrder.Single));
            acL.Bonds.Add(new Bond(a2, a5, BondOrder.Single));
            acL.Bonds.Add(new Bond(a2, a6, BondOrder.Double));

            ITetrahedralChirality chirality = new TetrahedralChirality(a1, ligandAtoms, TetrahedralStereo.AntiClockwise);
            acL.StereoElements.Add(chirality);

            InChIGenerator genL = GetFactory().GetInChIGenerator(acL);
            Assert.AreEqual(INCHI_RET.OKAY, genL.ReturnStatus);
            Assert.AreEqual("InChI=1S/C3H7NO2/c1-2(4)3(5)6/h2H,4H2,1H3,(H,5,6)/t2-/m0/s1", genL.InChI);
        }

        [TestMethod()]
        public void TestDoubleBondStereochemistry()
        {
            // (E)-1,2-dichloroethene
            IAtomContainer acE = new AtomContainer();
            IAtom a1E = new Atom("C");
            IAtom a2E = new Atom("C");
            IAtom a3E = new Atom("Cl");
            IAtom a4E = new Atom("Cl");
            a1E.ImplicitHydrogenCount = 1;
            a2E.ImplicitHydrogenCount = 1;
            acE.Atoms.Add(a1E);
            acE.Atoms.Add(a2E);
            acE.Atoms.Add(a3E);
            acE.Atoms.Add(a4E);

            acE.Bonds.Add(new Bond(a1E, a2E, BondOrder.Double));
            acE.Bonds.Add(new Bond(a1E, a3E, BondOrder.Single));
            acE.Bonds.Add(new Bond(a2E, a4E, BondOrder.Single));

            IBond[] ligands = new IBond[2];
            ligands[0] = acE.Bonds[1];
            ligands[1] = acE.Bonds[2];
            IDoubleBondStereochemistry stereo = new DoubleBondStereochemistry(acE.Bonds[0], ligands,
                    DoubleBondConformation.Opposite);
            acE.StereoElements.Add(stereo);

            InChIGenerator genE = GetFactory().GetInChIGenerator(acE);
            Assert.AreEqual(INCHI_RET.OKAY, genE.ReturnStatus);
            Assert.AreEqual("InChI=1S/C2H2Cl2/c3-1-2-4/h1-2H/b2-1+", genE.InChI);
        }

        /// <summary>
        // @cdk.bug 1295
        /// </summary>
        [TestMethod()]
        public void Bug1295()
        {
            MDLV2000Reader reader = new MDLV2000Reader(ResourceLoader.GetAsStream("NCDK.Data.MDL.bug1295.mol"));
            try
            {
                IAtomContainer container = reader.Read(new AtomContainer());
                InChIGenerator generator = GetFactory().GetInChIGenerator(container);
                Assert.AreEqual("InChI=1S/C7H15NO/c1-4-7(3)6-8-9-5-2/h6-7H,4-5H2,1-3H3", generator.InChI);
            }
            finally
            {
                reader.Close();
            }
        }

        [TestMethod()]
        public void R_penta_2_3_diene_impl_h()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(new Atom("CH3"));
            m.Atoms.Add(new Atom("CH"));
            m.Atoms.Add(new Atom("C"));
            m.Atoms.Add(new Atom("CH"));
            m.Atoms.Add(new Atom("CH3"));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Double);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Double);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);

            int[][] atoms = new int[][]{
                new[] {0, 1, 3, 4}, new[] {1, 0, 3, 4},
                new[] {1, 0, 4, 3}, new[] {0, 1, 4, 3},
                new[] {4, 3, 1, 0}, new[] {4, 3, 0, 1},
                new[] {3, 4, 0, 1}, new[] {3, 4, 1, 0},};
            TetrahedralStereo[] stereos = new TetrahedralStereo[]{TetrahedralStereo.AntiClockwise, TetrahedralStereo.Clockwise, TetrahedralStereo.AntiClockwise,
                TetrahedralStereo.Clockwise, TetrahedralStereo.AntiClockwise, TetrahedralStereo.Clockwise, TetrahedralStereo.AntiClockwise, TetrahedralStereo.Clockwise};

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

                InChIGenerator generator = GetFactory().GetInChIGenerator(m);
                Assert.AreEqual("InChI=1S/C5H8/c1-3-5-4-2/h3-4H,1-2H3/t5-/m0/s1", generator.InChI);
            }
        }

        [TestMethod()]
        public void S_penta_2_3_diene_impl_h()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(new Atom("CH3"));
            m.Atoms.Add(new Atom("CH"));
            m.Atoms.Add(new Atom("C"));
            m.Atoms.Add(new Atom("CH"));
            m.Atoms.Add(new Atom("CH3"));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Double);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Double);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);

            int[][] atoms = new int[][] {
                new[] {0, 1, 3, 4}, new[] {1, 0, 3, 4},
                new[] {1, 0, 4, 3}, new[] {0, 1, 4, 3},
                new[] {4, 3, 1, 0}, new[] {4, 3, 0, 1},
                new[] {3, 4, 0, 1}, new[] {3, 4, 1, 0}, };
            TetrahedralStereo[] stereos = new TetrahedralStereo[]{TetrahedralStereo.Clockwise, TetrahedralStereo.AntiClockwise, TetrahedralStereo.Clockwise,
                TetrahedralStereo.AntiClockwise, TetrahedralStereo.Clockwise, TetrahedralStereo.AntiClockwise, TetrahedralStereo.Clockwise, TetrahedralStereo.AntiClockwise};

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
                InChIGenerator generator = GetFactory().GetInChIGenerator(m);
                Assert.AreEqual("InChI=1S/C5H8/c1-3-5-4-2/h3-4H,1-2H3/t5-/m1/s1", generator.InChI);
            }
        }

        // why it does not work? [TestMethod()]
        public void R_penta_2_3_diene_expl_h()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(new Atom("CH3"));
            m.Atoms.Add(new Atom("C"));
            m.Atoms.Add(new Atom("C"));
            m.Atoms.Add(new Atom("C"));
            m.Atoms.Add(new Atom("CH3"));
            m.Atoms.Add(new Atom("H"));
            m.Atoms.Add(new Atom("H"));
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
            TetrahedralStereo[] stereos = new TetrahedralStereo[]{TetrahedralStereo.AntiClockwise, TetrahedralStereo.Clockwise, TetrahedralStereo.Clockwise,
                TetrahedralStereo.Clockwise, TetrahedralStereo.Clockwise, TetrahedralStereo.Clockwise, TetrahedralStereo.AntiClockwise, TetrahedralStereo.Clockwise};

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

                InChIGenerator generator = GetFactory().GetInChIGenerator(m);
                Assert.AreEqual("InChI=1S/C5H8/c1-3-5-4-2/h3-4H,1-2H3/t5-/m0/s1", generator.InChI);
            }
        }

        // why it does not work? [TestMethod()]
        public void S_penta_2_3_diene_expl_h()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(new Atom("CH3"));
            m.Atoms.Add(new Atom("C"));
            m.Atoms.Add(new Atom("C"));
            m.Atoms.Add(new Atom("C"));
            m.Atoms.Add(new Atom("CH3"));
            m.Atoms.Add(new Atom("H"));
            m.Atoms.Add(new Atom("H"));
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
                new[] {6, 4, 0, 5}, new[] {6, 4, 5, 0}, };
            TetrahedralStereo[] stereos = new TetrahedralStereo[]{TetrahedralStereo.Clockwise, TetrahedralStereo.Clockwise, TetrahedralStereo.Clockwise,
                TetrahedralStereo.AntiClockwise, TetrahedralStereo.Clockwise, TetrahedralStereo.AntiClockwise, TetrahedralStereo.Clockwise, TetrahedralStereo.Clockwise};

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

                InChIGenerator generator = GetFactory().GetInChIGenerator(m);
                Assert.AreEqual("InChI=1S/C5H8/c1-3-5-4-2/h3-4H,1-2H3/t5-/m1/s1", generator.InChI);
            }
        }

        [TestMethod()]
        public void Timeout()
        {
            IChemObjectBuilder bldr = Silent.ChemObjectBuilder.Instance;
            SmilesParser smipar = new SmilesParser(bldr);
            string smiles = "C(CCCNC(=N)N)(COCC(COP([O])(=O)OCCCCCCNC(NC1=CC(=C(C=C1)C2(C3=CC=C(C=C3OC=4C2=CC=C(C4)O)O)C)C(=O)[O])=S)OP(=O)([O])OCC(COCC(CCC/[NH]=C(\\[NH])/N)(CCCNC(=N)N)CCCNC(=N)N)OP(=O)([O])OCC(COCC(CCCNC(=N)N)(CCC/[NH]=C(\\[NH])/N)CCCNC(=N)N)OP(OCC(COCC(CCCNC(=N)N)(CCCNC(=N)N)CCC/[NH]=C(\\[NH])/N)OP(=O)([O])OCC(COCC(CCCNC(=N)N)(CCCNC(N)=N)CCC/[NH]=C(/N)\\[NH])OP([O])(=O)CCC(COCC(CCCNC(=N)N)(CCC/[NH]=C(\\[NH])/N)CCCNC(=N)N)OP([O])(=O)OCC(COCC(CCCNC(N)=N)(CCCNC(N)=N)CCC/[NH]=C(\\[NH])/N)OP(OCC(COCC(CCCNC(N)=N)(CCC/[NH]=C(/N)\\[NH])CCCNC(N)=N)O=P([O])(OCC(COP(=OC(COCC(CCC/[NH]=C(\\[NH])/N)(CCCNC(N)=N)CCCNC(N)=N)COP([O])(=O)OC(COP(OC(COCC(CCCNC(=N)N)(CCC/[NH]=C(\\[NH])/N)CCCNC(=N)N)COP(OC(COCC(CCCNC(=N)N)(CCC/[NH]=C(\\[NH])/N)CCCNC(=N)N)COP([O])(=O)OC(COP(OC(COP(OC(COP(=O)([O])OC(COCC(CCC/[NH]=C(/N)\\[NH])(CCCNC(N)=N)CCCNC(=N)N)COP([O])(=O)OCCCCCCNC(NC=5C=CC(=C(C5)C(=O)[O])C6(C7=CC=C(C=C7OC=8C6=CC=C(C8)O)O)C)=S)COCC(CCCNC(N)=N)(CCC/[NH]=C(\\[NH])/N)CCCNC(=N)N)([O])=O)COCC(CCCNC(=N)N)(CCC/[NH]=C(\\[NH])/N)CCCNC(=N)N)([O])=O)COCC(CCCNC(=N)N)(CCCNC(=N)N)CCC/[NH]=C(\\[NH])/N)([O])=O)([O])=O)COCC(CCC/[NH]=C(/N)\\[NH])(CCCNC(=N)N)CCCNC(=N)N)([O])[O])(C)COP(OCCCCCCO)(=O)[O])[O])(=O)[O])([O])=O)(CCC/[NH]=C(\\[NH])/[NH])CCCNC(=N)N";
            IAtomContainer mol = smipar.ParseSmiles(smiles);
            InChIGeneratorFactory inchiFact = InChIGeneratorFactory.Instance;
            InChIGenerator generator = inchiFact.GetInChIGenerator(mol, "W0.01");
            Assert.AreEqual(INCHI_RET.ERROR, generator.ReturnStatus);
            Assert.IsTrue(generator.Log.Contains("Time limit exceeded"));
        }

        /// <summary>
        /// Standard inchi for guanine.
        /// </summary>
        // @cdk.smiles NC1=NC2=C(N=CN2)C(=O)N1 
        [TestMethod()]
        public void Guanine_std()
        {
            IChemObjectBuilder bldr = Silent.ChemObjectBuilder.Instance;
            SmilesParser smipar = new SmilesParser(bldr);
            string smiles = "NC1=NC2=C(N=CN2)C(=O)N1";
            IAtomContainer mol = smipar.ParseSmiles(smiles);
            InChIGeneratorFactory inchiFact = InChIGeneratorFactory.Instance;
            InChIGenerator inchigen = inchiFact.GetInChIGenerator(mol);
            Assert.AreEqual(INCHI_RET.OKAY, inchigen.ReturnStatus);
            Assert.AreEqual("InChI=1S/C5H5N5O/c6-5-9-3-2(4(11)10-5)7-1-8-3/h1H,(H4,6,7,8,9,10,11)", inchigen.InChI);
        }

        /// <summary>
        /// Ensures KET (Keto-enol) option can be passed to InChI for guanine.
        /// </summary>
        // @cdk.smiles NC1=NC2=C(N=CN2)C(=O)N1 
        [TestMethod()]
        public void Guanine_ket()
        {
            IChemObjectBuilder bldr = Silent.ChemObjectBuilder.Instance;
            SmilesParser smipar = new SmilesParser(bldr);
            string smiles = "NC1=NC2=C(N=CN2)C(=O)N1";
            IAtomContainer mol = smipar.ParseSmiles(smiles);
            InChIGeneratorFactory inchiFact = InChIGeneratorFactory.Instance;
            InChIGenerator inchigen = inchiFact.GetInChIGenerator(mol, "KET");
            Assert.AreEqual(INCHI_RET.OKAY, inchigen.ReturnStatus);
            Assert.AreEqual("InChI=1/C5H5N5O/c6-5-9-3-2(4(11)10-5)7-1-8-3/h1H,(H4,2,6,7,8,9,10,11)", inchigen.InChI);
        }

        /// <summary>
        /// Standard test for aminopropenol.
        /// </summary>
        // @cdk.smiles N\C=C/C=O 
        [TestMethod()]
        public void Aminopropenol_std()
        {
            IChemObjectBuilder bldr = Silent.ChemObjectBuilder.Instance;
            SmilesParser smipar = new SmilesParser(bldr);
            string smiles = "N\\C=C/C=O";
            IAtomContainer mol = smipar.ParseSmiles(smiles);
            InChIGeneratorFactory inchiFact = InChIGeneratorFactory.Instance;
            InChIGenerator stdinchi = inchiFact.GetInChIGenerator(mol);
            Assert.AreEqual(INCHI_RET.OKAY, stdinchi.ReturnStatus);
            Assert.AreEqual("InChI=1S/C3H5NO/c4-2-1-3-5/h1-3H,4H2/b2-1-", stdinchi.InChI);
            InChIGenerator inchigen = inchiFact.GetInChIGenerator(mol, "15T");
            Assert.AreEqual(INCHI_RET.OKAY, inchigen.ReturnStatus);
            Assert.AreEqual("InChI=1/C3H5NO/c4-2-1-3-5/h1-3H,(H2,4,5)", inchigen.InChI);
        }

        /// <summary>
        /// Ensures 15T (1,5-shifts) option can be passed to InChI for aminopropenol.
        /// </summary>
        // @cdk.smiles N\C=C/C=O 
        [TestMethod()]
        public void Aminopropenol_15T()
        {
            IChemObjectBuilder bldr = Silent.ChemObjectBuilder.Instance;
            SmilesParser smipar = new SmilesParser(bldr);
            string smiles = "N\\C=C/C=O";
            IAtomContainer mol = smipar.ParseSmiles(smiles);
            InChIGeneratorFactory inchiFact = InChIGeneratorFactory.Instance;
            InChIGenerator inchigen = inchiFact.GetInChIGenerator(mol, "15T");
            Assert.AreEqual(INCHI_RET.OKAY, inchigen.ReturnStatus);
            Assert.AreEqual("InChI=1/C3H5NO/c4-2-1-3-5/h1-3H,(H2,4,5)", inchigen.InChI);
        }

        /// <summary>
        /// Ensures default timeout option is passed with proper switch character.
        /// </summary>
        [TestMethod()]
        public void TestFiveSecondTimeoutFlag()
        {
            IAtomContainer ac = new AtomContainer();
            ac.Atoms.Add(new Atom("C"));
            InChIGeneratorFactory factory = InChIGeneratorFactory.Instance;
            InChIGenerator generator = factory.GetInChIGenerator(ac);

            string flagChar = Environment.OSVersion.Platform < PlatformID.Unix ? "/" : "-";
            Assert.IsTrue(generator.Input.Options.Contains(flagChar + "W5"));
        }
    }
}
