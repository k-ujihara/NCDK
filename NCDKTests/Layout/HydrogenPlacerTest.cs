/* Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
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
using NCDK.Common.Mathematics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Silent;
using NCDK.Geometries;
using NCDK.IO;
using System;
using System.Diagnostics;
using System.IO;
using NCDK.Numerics;

namespace NCDK.Layout
{
    // @cdk.module test-sdg
    [TestClass()]
    public class HydrogenPlacerTest : CDKTestCase
    {
        public bool standAlone = false;

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAtomWithoutCoordinates()
        {
            HydrogenPlacer hydrogenPlacer = new HydrogenPlacer();
            hydrogenPlacer.PlaceHydrogens2D(new AtomContainer(), new Atom(), 1.5);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestNullContainer()
        {
            HydrogenPlacer hydrogenPlacer = new HydrogenPlacer();
            hydrogenPlacer.PlaceHydrogens2D(null, new Atom(), 1.5);
        }

        [TestMethod()]
        public void TestNoConnections()
        {
            HydrogenPlacer hydrogenPlacer = new HydrogenPlacer();
            AtomContainer container = new AtomContainer();
            Atom atom = new Atom("C", new Vector2(0, 0));
            container.Atoms.Add(atom);
            hydrogenPlacer.PlaceHydrogens2D(container, atom, 1.5);
        }

        /// <summary>@cdk.bug 1269</summary>
        [TestMethod()]
        public void TestH2()
        {
            HydrogenPlacer hydrogenPlacer = new HydrogenPlacer();

            // h1 has no coordinates
            IAtom h1 = new Atom("H");
            IAtom h2 = new Atom("H", Vector2.Zero);
            var m = new AtomContainer();
            m.Atoms.Add(h1);
            m.Atoms.Add(h2);
            m.Bonds.Add(new Bond(h1, h2));
            hydrogenPlacer.PlaceHydrogens2D(m, 1.5);
            Assert.IsNotNull(h1.Point2D);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void UnPlacedNonHydrogen()
        {
            HydrogenPlacer hydrogenPlacer = new HydrogenPlacer();

            // c2 is unplaced
            IAtom c1 = new Atom("C", Vector2.Zero);
            IAtom c2 = new Atom("C");
            var m = new AtomContainer();
            m.Atoms.Add(c1);
            m.Atoms.Add(c2);
            m.Bonds.Add(new Bond(c1, c2));
            hydrogenPlacer.PlaceHydrogens2D(m, 1.5);
        }

        /// <summary>@cdk.bug 933572</summary>
        [TestMethod()]
        public void TestBug933572()
        {
            var ac = new AtomContainer();
            ac.Atoms.Add(new Atom("H"));
            ac.Atoms[0].Point2D = Vector2.Zero;
            AddExplicitHydrogens(ac);
            var hPlacer = new HydrogenPlacer();
            hPlacer.PlaceHydrogens2D(ac, 36);
            for (int i = 0; i < ac.Atoms.Count; i++)
            {
                Assert.IsNotNull(ac.Atoms[i].Point2D);
            }
        }

        [TestMethod()]
        public void TestPlaceHydrogens2D()
        {
            HydrogenPlacer hydrogenPlacer = new HydrogenPlacer();
            IAtomContainer dichloromethane = new AtomContainer();
            Atom carbon = new Atom("C");
            Vector2 carbonPos = new Vector2(0.0, 0.0);
            carbon.Point2D = carbonPos;
            Atom h1 = new Atom("H");
            Atom h2 = new Atom("H");
            Atom cl1 = new Atom("Cl");
            Vector2 cl1Pos = new Vector2(0.0, -1.0);
            cl1.Point2D = cl1Pos;
            Atom cl2 = new Atom("Cl");
            Vector2 cl2Pos = new Vector2(-1.0, 0.0);
            cl2.Point2D = cl2Pos;
            dichloromethane.Atoms.Add(carbon);
            dichloromethane.Atoms.Add(h1);
            dichloromethane.Atoms.Add(h2);
            dichloromethane.Atoms.Add(cl1);
            dichloromethane.Atoms.Add(cl2);
            dichloromethane.Bonds.Add(new Bond(carbon, h1));
            dichloromethane.Bonds.Add(new Bond(carbon, h2));
            dichloromethane.Bonds.Add(new Bond(carbon, cl1));
            dichloromethane.Bonds.Add(new Bond(carbon, cl2));

            Assert.IsNull(h1.Point2D);
            Assert.IsNull(h2.Point2D);

            // generate new coords
            hydrogenPlacer.PlaceHydrogens2D(dichloromethane, carbon);
            // check that previously set coordinates are kept
            Assert.IsTrue(Vector2.Distance(carbonPos, carbon.Point2D.Value) < 0.01);
            Assert.IsTrue(Vector2.Distance(cl1Pos, cl1.Point2D.Value) < 0.01);
            Assert.IsTrue(Vector2.Distance(cl2Pos, cl2.Point2D.Value) < 0.01);
            Assert.IsNotNull(h1.Point2D);
            Assert.IsNotNull(h2.Point2D);
        }

        /// <summary>
        /// This one tests adding hydrogens to all atoms of a molecule and doing the
        /// layout for them. It is intended for visually checking the work of
        /// HydrogenPlacer, not to be run as a JUnit test. Thus the name without
        /// "test".
        /// </summary>
        public void VisualFullMolecule2DEvaluation()
        {
            HydrogenPlacer hydrogenPlacer = new HydrogenPlacer();
            string filename = "NCDK.Data.MDL.reserpine.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLReader reader = new MDLReader(ins, ChemObjectReaderMode.Strict);
            ChemFile chemFile = (ChemFile)reader.Read((ChemObject)new ChemFile());
            IChemSequence seq = chemFile[0];
            IChemModel model = seq[0];
            IAtomContainer mol = model.MoleculeSet[0];
            double bondLength = GeometryUtil.GetBondLengthAverage(mol);
            Debug.WriteLine("Read Reserpine");
            Debug.WriteLine("Starting addition of H's");
            AddExplicitHydrogens(mol);
            Debug.WriteLine("ended addition of H's");
            hydrogenPlacer.PlaceHydrogens2D(mol, bondLength);
        }
    }
}
