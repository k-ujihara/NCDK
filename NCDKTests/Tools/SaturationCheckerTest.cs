/* Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT Any WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Silent;

namespace NCDK.Tools
{
    // @cdk.module test-valencycheck
    // @author     steinbeck
    // @cdk.created    2003-02-20
    [TestClass()]
    public class SaturationCheckerTest
        : CDKTestCase
    {
        SaturationChecker satcheck = new SaturationChecker();

        [TestMethod()]
        public void TestAllSaturated()
        {
            // test methane with explicit hydrogen
            IAtomContainer m = new AtomContainer();
            Atom c = new Atom("C");
            Atom h1 = new Atom("H");
            Atom h2 = new Atom("H");
            Atom h3 = new Atom("H");
            Atom h4 = new Atom("H");
            m.Atoms.Add(c);
            m.Atoms.Add(h1);
            m.Atoms.Add(h2);
            m.Atoms.Add(h3);
            m.Atoms.Add(h4);
            m.Bonds.Add(new Bond(c, h1));
            m.Bonds.Add(new Bond(c, h2));
            m.Bonds.Add(new Bond(c, h3));
            m.Bonds.Add(new Bond(c, h4));
            Assert.IsTrue(satcheck.IsSaturated(m));

            // test methane with implicit hydrogen
            m = new AtomContainer();
            c = new Atom("C") { ImplicitHydrogenCount = 4 };
            m.Atoms.Add(c);
            Assert.IsTrue(satcheck.IsSaturated(m));
        }

        [TestMethod()]
        public void TestIsSaturated()
        {
            // test methane with explicit hydrogen
            IAtomContainer m = new AtomContainer();
            Atom c = new Atom("C");
            Atom h1 = new Atom("H");
            Atom h2 = new Atom("H");
            Atom h3 = new Atom("H");
            Atom h4 = new Atom("H");
            m.Atoms.Add(c);
            m.Atoms.Add(h1);
            m.Atoms.Add(h2);
            m.Atoms.Add(h3);
            m.Atoms.Add(h4);
            m.Bonds.Add(new Bond(c, h1));
            m.Bonds.Add(new Bond(c, h2));
            m.Bonds.Add(new Bond(c, h3));
            m.Bonds.Add(new Bond(c, h4));
            Assert.IsTrue(satcheck.IsSaturated(c, m));
            Assert.IsTrue(satcheck.IsSaturated(h1, m));
            Assert.IsTrue(satcheck.IsSaturated(h2, m));
            Assert.IsTrue(satcheck.IsSaturated(h3, m));
            Assert.IsTrue(satcheck.IsSaturated(h4, m));
        }

        /// <summary>
        /// Tests whether the saturation checker considers negative charges.
        /// </summary>
        [TestMethod()]
        public void TestIsSaturated_NegativelyChargedOxygen()
        {
            // test methane with explicit hydrogen
            IAtomContainer m = new AtomContainer();
            Atom c = new Atom("C");
            Atom h1 = new Atom("H");
            Atom h2 = new Atom("H");
            Atom h3 = new Atom("H");
            Atom o = new Atom("O") { FormalCharge = -1 };
            m.Atoms.Add(c);
            m.Atoms.Add(h1);
            m.Atoms.Add(h2);
            m.Atoms.Add(h3);
            m.Atoms.Add(o);
            m.Bonds.Add(new Bond(c, h1));
            m.Bonds.Add(new Bond(c, h2));
            m.Bonds.Add(new Bond(c, h3));
            m.Bonds.Add(new Bond(c, o));
            Assert.IsTrue(satcheck.IsSaturated(c, m));
            Assert.IsTrue(satcheck.IsSaturated(h1, m));
            Assert.IsTrue(satcheck.IsSaturated(h2, m));
            Assert.IsTrue(satcheck.IsSaturated(h3, m));
            Assert.IsTrue(satcheck.IsSaturated(o, m));
        }

        /// <summary>
        /// Tests whether the saturation checker considers positive charges.
        /// </summary>
        [TestMethod()]
        public void TestIsSaturated_PositivelyChargedNitrogen()
        {
            // test methane with explicit hydrogen
            IAtomContainer m = new AtomContainer();
            Atom n = new Atom("N");
            Atom h1 = new Atom("H");
            Atom h2 = new Atom("H");
            Atom h3 = new Atom("H");
            Atom h4 = new Atom("H");
            n.FormalCharge = +1;
            m.Atoms.Add(n);
            m.Atoms.Add(h1);
            m.Atoms.Add(h2);
            m.Atoms.Add(h3);
            m.Atoms.Add(h4);
            m.Bonds.Add(new Bond(n, h1));
            m.Bonds.Add(new Bond(n, h2));
            m.Bonds.Add(new Bond(n, h3));
            m.Bonds.Add(new Bond(n, h4));
            Assert.IsTrue(satcheck.IsSaturated(n, m));
            Assert.IsTrue(satcheck.IsSaturated(h1, m));
            Assert.IsTrue(satcheck.IsSaturated(h2, m));
            Assert.IsTrue(satcheck.IsSaturated(h3, m));
            Assert.IsTrue(satcheck.IsSaturated(h4, m));
        }

        [TestMethod()]
        public void TestSaturate()
        {
            // test ethene
            Atom c1 = new Atom("C") { ImplicitHydrogenCount = 2 };
            Atom c2 = new Atom("C") { ImplicitHydrogenCount = 2 };
            Bond b = new Bond(c1, c2, BondOrder.Single);
            // force single bond, Saturate() must fix that
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(c1);
            m.Atoms.Add(c2);
            m.Bonds.Add(b);
            satcheck.Saturate(m);
            Assert.AreEqual(BondOrder.Double, b.Order);
        }

        [TestMethod()]
        public void TestSaturate_Butene()
        {
            // test ethene
            Atom c1 = new Atom("C") { ImplicitHydrogenCount = 2 };
            Atom c2 = new Atom("C") { ImplicitHydrogenCount = 1 };
            Atom c3 = new Atom("C") { ImplicitHydrogenCount = 1 };
            Atom c4 = new Atom("C") { ImplicitHydrogenCount = 2 };
            Bond b1 = new Bond(c1, c2, BondOrder.Single);
            Bond b2 = new Bond(c3, c2, BondOrder.Single);
            Bond b3 = new Bond(c3, c4, BondOrder.Single);
            // force single bond, Saturate() must fix that
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(c1);
            m.Atoms.Add(c2);
            m.Atoms.Add(c3);
            m.Atoms.Add(c4);
            m.Bonds.Add(b1);
            m.Bonds.Add(b2);
            m.Bonds.Add(b3);
            satcheck.Saturate(m);
            Assert.AreEqual(BondOrder.Double, b1.Order);
            Assert.AreEqual(BondOrder.Single, b2.Order);
            Assert.AreEqual(BondOrder.Double, b3.Order);
        }

        [TestMethod()]
        public void TestSaturate_ParaDiOxygenBenzene()
        {
            IAtomContainer mol = new AtomContainer();
            Atom a1 = new Atom("C");
            mol.Atoms.Add(a1);
            Atom a2 = new Atom("O");
            mol.Atoms.Add(a2);
            Atom a3 = new Atom("C");
            mol.Atoms.Add(a3);
            Atom a4 = new Atom("C");
            mol.Atoms.Add(a4);
            Atom a5 = new Atom("H");
            mol.Atoms.Add(a5);
            Atom a6 = new Atom("C");
            mol.Atoms.Add(a6);
            Atom a7 = new Atom("H");
            mol.Atoms.Add(a7);
            Atom a8 = new Atom("C");
            mol.Atoms.Add(a8);
            Atom a9 = new Atom("H");
            mol.Atoms.Add(a9);
            Atom a10 = new Atom("C");
            mol.Atoms.Add(a10);
            Atom a11 = new Atom("H");
            mol.Atoms.Add(a11);
            Atom a12 = new Atom("O");
            mol.Atoms.Add(a12);
            Bond b1 = new Bond(a2, a1, BondOrder.Single);
            mol.Bonds.Add(b1);
            Bond b2 = new Bond(a1, a3, BondOrder.Single);
            mol.Bonds.Add(b2);
            Bond b3 = new Bond(a1, a4, BondOrder.Single);
            mol.Bonds.Add(b3);
            Bond b4 = new Bond(a5, a3, BondOrder.Single);
            mol.Bonds.Add(b4);
            Bond b5 = new Bond(a3, a6, BondOrder.Single);
            mol.Bonds.Add(b5);
            Bond b6 = new Bond(a7, a4, BondOrder.Single);
            mol.Bonds.Add(b6);
            Bond b7 = new Bond(a4, a8, BondOrder.Single);
            mol.Bonds.Add(b7);
            Bond b8 = new Bond(a6, a9, BondOrder.Single);
            mol.Bonds.Add(b8);
            Bond b9 = new Bond(a6, a10, BondOrder.Single);
            mol.Bonds.Add(b9);
            Bond b10 = new Bond(a8, a10, BondOrder.Single);
            mol.Bonds.Add(b10);
            Bond b11 = new Bond(a8, a11, BondOrder.Single);
            mol.Bonds.Add(b11);
            Bond b12 = new Bond(a10, a12, BondOrder.Single);
            mol.Bonds.Add(b12);
            satcheck.Saturate(mol);
            Assert.AreEqual(BondOrder.Double, b1.Order);
            Assert.AreEqual(BondOrder.Single, b2.Order);
            Assert.AreEqual(BondOrder.Single, b3.Order);
            Assert.AreEqual(BondOrder.Double, b5.Order);
            Assert.AreEqual(BondOrder.Double, b7.Order);
            Assert.AreEqual(BondOrder.Single, b9.Order);
            Assert.AreEqual(BondOrder.Single, b10.Order);
            Assert.AreEqual(BondOrder.Double, b12.Order);
        }

        [TestMethod()]
        public void TestBug772316()
        {
            // test methane with explicit hydrogen
            IAtomContainer m = new AtomContainer();
            Atom sulphur = new Atom("S");
            Atom o1 = new Atom("O");
            Atom o2 = new Atom("O");
            Atom o3 = new Atom("O");
            Atom o4 = new Atom("O");
            Atom h1 = new Atom("H");
            Atom h2 = new Atom("H");
            m.Atoms.Add(sulphur);
            m.Atoms.Add(o1);
            m.Atoms.Add(o2);
            m.Atoms.Add(o3);
            m.Atoms.Add(o4);
            m.Atoms.Add(h1);
            m.Atoms.Add(h2);
            m.Bonds.Add(new Bond(sulphur, o1, BondOrder.Double));
            m.Bonds.Add(new Bond(sulphur, o2, BondOrder.Double));
            m.Bonds.Add(new Bond(sulphur, o3, BondOrder.Single));
            m.Bonds.Add(new Bond(sulphur, o4, BondOrder.Single));
            m.Bonds.Add(new Bond(h1, o3, BondOrder.Single));
            m.Bonds.Add(new Bond(h2, o4, BondOrder.Single));
            Assert.IsTrue(satcheck.IsSaturated(sulphur, m));
            Assert.IsTrue(satcheck.IsSaturated(o1, m));
            Assert.IsTrue(satcheck.IsSaturated(o2, m));
            Assert.IsTrue(satcheck.IsSaturated(o3, m));
            Assert.IsTrue(satcheck.IsSaturated(o4, m));
            Assert.IsTrue(satcheck.IsSaturated(h1, m));
            Assert.IsTrue(satcheck.IsSaturated(h2, m));
        }

        [TestMethod()]
        public void TestBug777529()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(new Atom("C"));
            m.Atoms.Add(new Atom("C"));
            m.Atoms.Add(new Atom("C"));
            m.Atoms.Add(new Atom("C"));
            m.Atoms.Add(new Atom("C"));
            m.Atoms.Add(new Atom("C"));
            m.Atoms.Add(new Atom("C"));
            m.Atoms.Add(new Atom("C"));
            m.Atoms.Add(new Atom("C"));
            m.Atoms.Add(new Atom("C"));
            m.Atoms.Add(new Atom("C"));
            m.Atoms.Add(new Atom("C"));
            m.Atoms.Add(new Atom("C"));
            m.Atoms.Add(new Atom("C"));
            m.Atoms.Add(new Atom("C"));
            m.Atoms.Add(new Atom("C"));
            m.Atoms.Add(new Atom("C"));
            m.Atoms.Add(new Atom("C"));
            m.Atoms.Add(new Atom("C"));
            m.Atoms.Add(new Atom("C"));
            m.Atoms.Add(new Atom("O"));
            m.Atoms.Add(new Atom("O"));
            m.Atoms.Add(new Atom("F"));
            m.Atoms[0].ImplicitHydrogenCount = 1;
            m.Atoms[2].ImplicitHydrogenCount = 1;
            m.Atoms[3].ImplicitHydrogenCount = 1;
            m.Atoms[6].ImplicitHydrogenCount = 1;
            m.Atoms[7].ImplicitHydrogenCount = 1;
            m.Atoms[8].ImplicitHydrogenCount = 1;
            m.Atoms[9].ImplicitHydrogenCount = 1;
            //m.GetAtomAt(10).SetHydrogenCount(1);
            //m.GetAtomAt(12).SetHydrogenCount(1);
            m.Atoms[14].ImplicitHydrogenCount = 1;
            m.Atoms[15].ImplicitHydrogenCount = 1;
            m.Atoms[17].ImplicitHydrogenCount = 1;
            m.Atoms[18].ImplicitHydrogenCount = 1;
            m.Atoms[19].ImplicitHydrogenCount = 3;
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Single);
            m.AddBond(m.Atoms[3], m.Atoms[4], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[5], BondOrder.Single);
            m.AddBond(m.Atoms[5], m.Atoms[6], BondOrder.Single);
            m.AddBond(m.Atoms[6], m.Atoms[7], BondOrder.Single);
            m.AddBond(m.Atoms[7], m.Atoms[8], BondOrder.Single);
            m.AddBond(m.Atoms[8], m.Atoms[9], BondOrder.Single);
            m.AddBond(m.Atoms[5], m.Atoms[10], BondOrder.Single);
            m.AddBond(m.Atoms[9], m.Atoms[10], BondOrder.Single);
            m.AddBond(m.Atoms[10], m.Atoms[11], BondOrder.Single);
            m.AddBond(m.Atoms[0], m.Atoms[12], BondOrder.Single);
            m.AddBond(m.Atoms[4], m.Atoms[12], BondOrder.Single);
            m.AddBond(m.Atoms[11], m.Atoms[12], BondOrder.Single);
            m.AddBond(m.Atoms[11], m.Atoms[13], BondOrder.Single);
            m.AddBond(m.Atoms[13], m.Atoms[14], BondOrder.Single);
            m.AddBond(m.Atoms[14], m.Atoms[15], BondOrder.Single);
            m.AddBond(m.Atoms[15], m.Atoms[16], BondOrder.Single);
            m.AddBond(m.Atoms[16], m.Atoms[17], BondOrder.Single);
            m.AddBond(m.Atoms[13], m.Atoms[18], BondOrder.Single);
            m.AddBond(m.Atoms[17], m.Atoms[18], BondOrder.Single);
            m.AddBond(m.Atoms[20], m.Atoms[16], BondOrder.Single);
            m.AddBond(m.Atoms[11], m.Atoms[21], BondOrder.Single);
            m.AddBond(m.Atoms[22], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[20], m.Atoms[19], BondOrder.Single);
            m.Atoms[0].IsAromatic = true;
            m.Atoms[1].IsAromatic = true;
            m.Atoms[2].IsAromatic = true;
            m.Atoms[3].IsAromatic = true;
            m.Atoms[4].IsAromatic = true;
            m.Atoms[12].IsAromatic = true;
            m.Atoms[5].IsAromatic = true;
            m.Atoms[6].IsAromatic = true;
            m.Atoms[7].IsAromatic = true;
            m.Atoms[8].IsAromatic = true;
            m.Atoms[9].IsAromatic = true;
            m.Atoms[10].IsAromatic = true;
            m.Bonds[0].IsAromatic = true;
            m.Bonds[1].IsAromatic = true;
            m.Bonds[2].IsAromatic = true;
            m.Bonds[3].IsAromatic = true;
            m.Bonds[5].IsAromatic = true;
            m.Bonds[6].IsAromatic = true;
            m.Bonds[7].IsAromatic = true;
            m.Bonds[8].IsAromatic = true;
            m.Bonds[9].IsAromatic = true;
            m.Bonds[10].IsAromatic = true;
            m.Bonds[12].IsAromatic = true;
            m.Bonds[13].IsAromatic = true;
            satcheck.Saturate(m);
            Assert.IsTrue(m.Bonds[4].Order == BondOrder.Single);
            Assert.IsTrue(m.Bonds[9].Order == BondOrder.Double ^ m.Bonds[5].Order == BondOrder.Double);
            Assert.IsTrue(m.Bonds[13].Order == BondOrder.Double
                    ^ m.Bonds[3].Order == BondOrder.Double);
        }

        [TestMethod()]
        public void TestCalculateNumberOfImplicitHydrogens()
        {
            IChemObjectBuilder builder = ChemObjectBuilder.Instance;

            IAtomContainer proton = builder.NewAtomContainer();
            IAtom hplus = builder.NewAtom("H");
            hplus.FormalCharge = 1;
            proton.Atoms.Add(hplus);
            Assert.AreEqual(0, satcheck.CalculateNumberOfImplicitHydrogens(hplus, proton));

            IAtomContainer hydrogenRadical = builder.NewAtomContainer();
            IAtom hradical = builder.NewAtom("H");
            hydrogenRadical.Atoms.Add(hradical);
            hydrogenRadical.SingleElectrons.Add(builder.NewSingleElectron(hradical));
            Assert.AreEqual(0, satcheck.CalculateNumberOfImplicitHydrogens(hradical, hydrogenRadical));

            IAtomContainer hydrogen = builder.NewAtomContainer();
            IAtom h = builder.NewAtom("H");
            hydrogen.Atoms.Add(h);
            Assert.AreEqual(1, satcheck.CalculateNumberOfImplicitHydrogens(h, hydrogen));

            IAtomContainer coRad = builder.NewAtomContainer();
            IAtom c = builder.NewAtom("C");
            IAtom o = builder.NewAtom("O");
            IBond bond = builder.NewBond(c, o, BondOrder.Double);
            coRad.Atoms.Add(c);
            coRad.Atoms.Add(o);
            coRad.Bonds.Add(bond);
            coRad.SingleElectrons.Add(builder.NewSingleElectron(c));
            Assert.AreEqual(1, satcheck.CalculateNumberOfImplicitHydrogens(c, coRad));
        }
    }
}
