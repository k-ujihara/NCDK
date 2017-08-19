/*  Copyright (C) 1997-2008  The Chemistry Development Kit (CDK) project
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *  All we ask is that proper credit is given for our work, which includes
 *  - but is not limited to - adding the above copyright notice to the beginning
 *  of your source code files, and to any copyright notice that you may distribute
 *  with programs based on this work.
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
using NCDK.Config;
using NCDK.Default;
using NCDK.IO;
using NCDK.Isomorphisms;
using NCDK.Tools.Diff;
using System;
using System.Collections.Generic;
using NCDK.Numerics;

namespace NCDK.Geometries
{
    /// <summary>
    /// This class defines regression tests that should ensure that the source code
    /// of the <see cref="GeometryUtil"/> is not broken.
    /// </summary>
    /// <seealso cref="GeometryUtil"/>
    // @cdk.module test-standard
    // @author     Egon Willighagen
    // @cdk.created    2004-01-30
    [TestClass()]
    public class GeometryUtilTest : CDKTestCase
    {
        [TestMethod()]
        public void TestHas2DCoordinates_IAtomContainer()
        {
            Atom atom1 = new Atom("C");
            atom1.Point2D = new Vector2(1, 1);
            Atom atom2 = new Atom("C");
            atom2.Point2D = new Vector2(1, 0);
            IAtomContainer container = new AtomContainer();
            container.Atoms.Add(atom1);
            container.Atoms.Add(atom2);
            Assert.IsTrue(GeometryUtil.Has2DCoordinates(container));

            atom1 = new Atom("C");
            atom1.Point3D = new Vector3(1, 1, 1);
            atom2 = new Atom("C");
            atom2.Point3D = new Vector3(1, 0, 5);
            container = new AtomContainer();
            container.Atoms.Add(atom1);
            container.Atoms.Add(atom2);
            Assert.IsFalse(GeometryUtil.Has2DCoordinates(container));
        }

        [TestMethod()]
        public void TestHas2DCoordinates_EmptyAtomContainer()
        {
            IAtomContainer container = new AtomContainer();
            Assert.IsFalse(GeometryUtil.Has2DCoordinates(container));
            Assert.IsFalse(GeometryUtil.Has2DCoordinates((IAtomContainer)null));
        }

        [TestMethod()]
        public void TestHas2DCoordinates_Partial()
        {
            IAtomContainer container = new AtomContainer();
            Atom atom1 = new Atom("C");
            Atom atom2 = new Atom("C");
            atom1.Point2D = new Vector2(1, 1);
            container.Atoms.Add(atom1);
            Assert.IsTrue(GeometryUtil.Has2DCoordinates(container));
            container.Atoms.Add(atom2);
            Assert.IsFalse(GeometryUtil.Has2DCoordinates(container));
        }

        // @cdk.bug 2936440
        [TestMethod()]
        public void TestHas2DCoordinates_With000()
        {
            string filenameMol = "NCDK.Data.MDL.with000coordinate.mol";
            var ins = ResourceLoader.GetAsStream(filenameMol);
            IAtomContainer molOne = null;
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
            molOne = (IAtomContainer) reader.Read(new AtomContainer());
            Assert.IsTrue(GeometryUtil.Has2DCoordinates(molOne));
        }

        [TestMethod()]
        public void Get2DCoordinateCoverage_EmptyAtomContainer()
        {
            IAtomContainer container = new AtomContainer();
            Assert.AreEqual(GeometryUtil.CoordinateCoverages.None, GeometryUtil.Get2DCoordinateCoverage(container));
            Assert.AreEqual(GeometryUtil.CoordinateCoverages.None,
                                GeometryUtil.Get2DCoordinateCoverage((IAtomContainer)null));
        }

        [TestMethod()]
        public void Get2DCoordinateCoverage_Partial()
        {
            IAtomContainer container = new AtomContainer();

            IAtom atom1 = new Atom("C");
            IAtom atom2 = new Atom("C");
            IAtom atom3 = new Atom("C");

            atom1.Point2D = new Vector2(1, 1);
            atom3.Point2D = new Vector2(1, 1);

            container.Atoms.Add(atom1);
            container.Atoms.Add(atom2);
            container.Atoms.Add(atom3);

            Assert.AreEqual(GeometryUtil.CoordinateCoverages.Partial, GeometryUtil.Get2DCoordinateCoverage(container));
        }

        [TestMethod()]
        public void Get2DCoordinateCoverage_Full()
        {
            IAtomContainer container = new AtomContainer();

            IAtom atom1 = new Atom("C");
            IAtom atom2 = new Atom("C");
            IAtom atom3 = new Atom("C");

            atom1.Point2D = new Vector2(1, 1);
            atom2.Point2D = new Vector2(2, 1);
            atom3.Point2D = new Vector2(1, 2);

            container.Atoms.Add(atom1);
            container.Atoms.Add(atom2);
            container.Atoms.Add(atom3);

            Assert.AreEqual(GeometryUtil.CoordinateCoverages.Full, GeometryUtil.Get2DCoordinateCoverage(container));
        }

        [TestMethod()]
        public void Get2DCoordinateCoverage_None_3D()
        {
            IAtomContainer container = new AtomContainer();

            IAtom atom1 = new Atom("C");
            IAtom atom2 = new Atom("C");
            IAtom atom3 = new Atom("C");

            atom1.Point3D = new Vector3(1, 1, 0);
            atom2.Point3D = new Vector3(2, 1, 0);
            atom3.Point3D = new Vector3(1, 2, 0);

            container.Atoms.Add(atom1);
            container.Atoms.Add(atom2);
            container.Atoms.Add(atom3);

            Assert.AreEqual(GeometryUtil.CoordinateCoverages.None, GeometryUtil.Get2DCoordinateCoverage(container));
        }

        [TestMethod()]
        public void TestTranslateAllPositive_IAtomContainer()
        {
            IAtomContainer container = new AtomContainer();
            IAtom atom = new Atom(Elements.Carbon.ToIElement());
            atom.Point2D = new Vector2(-3, -2);
            container.Atoms.Add(atom);
            GeometryUtil.TranslateAllPositive(container);
            Assert.IsTrue(0 <= atom.Point2D.Value.X);
            Assert.IsTrue(0 <= atom.Point2D.Value.Y);
        }

        [TestMethod()]
        public void TestGetLength2D_IBond()
        {
            Atom o = new Atom("O", new Vector2(0.0, 0.0));
            Atom c = new Atom("C", new Vector2(1.0, 0.0));
            Bond bond = new Bond(c, o);

            Assert.AreEqual(1.0, GeometryUtil.GetLength2D(bond), 0.001);
        }

        [TestMethod()]
        public void TestMapAtomsOfAlignedStructures()
        {
            string filenameMolOne = "NCDK.Data.MDL.murckoTest6_3d_2.mol";
            string filenameMolTwo = "NCDK.Data.MDL.murckoTest6_3d.mol";
            //string filenameMolTwo = "NCDK.Data.MDL.murckoTest6_3d_2.mol";
            var ins = ResourceLoader.GetAsStream(filenameMolOne);
            IAtomContainer molOne;
            IAtomContainer molTwo;
            IDictionary<int, int> mappedAtoms = new Dictionary<int, int>();
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
            molOne = reader.Read(new AtomContainer());

            ins = ResourceLoader.GetAsStream(filenameMolTwo);
            reader = new MDLV2000Reader(ins, ChemObjectReaderModes.Strict);
            molTwo = reader.Read(new AtomContainer());

            mappedAtoms = AtomMappingTools.MapAtomsOfAlignedStructures(molOne, molTwo, mappedAtoms);
            //Debug.WriteLine("mappedAtoms:"+mappedAtoms.ToString());
            //Debug.WriteLine("***** ANGLE VARIATIONS *****");
            double AngleRMSD = GeometryUtil.GetAngleRMSD(molOne, molTwo, mappedAtoms);
            //Debug.WriteLine("The Angle RMSD between the first and the second structure is :"+AngleRMSD);
            //Debug.WriteLine("***** ALL ATOMS RMSD *****");
            Assert.AreEqual(0.2, AngleRMSD, 0.1);
            double AllRMSD = GeometryUtil.GetAllAtomRMSD(molOne, molTwo, mappedAtoms, true);
            //Debug.WriteLine("The RMSD between the first and the second structure is :"+AllRMSD);
            Assert.AreEqual(0.242, AllRMSD, 0.001);
            //Debug.WriteLine("***** BOND LENGTH RMSD *****");
            double BondLengthRMSD = GeometryUtil.GetBondLengthRMSD(molOne, molTwo, mappedAtoms, true);
            //Debug.WriteLine("The Bond length RMSD between the first and the second structure is :"+BondLengthRMSD);
            Assert.AreEqual(0.2, BondLengthRMSD, 0.1);
        }

        // @cdk.bug 1649007
        [TestMethod()]
        public void TestRotate_IAtomContainer_Point2d_Double()
        {
            Atom atom1 = new Atom("C");
            atom1.Point2D = new Vector2(1, 1);
            Atom atom2 = new Atom("C");
            atom2.Point2D = new Vector2(1, 0);
            IAtomContainer ac = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            ac.Atoms.Add(atom1);
            ac.Atoms.Add(atom2);
            GeometryUtil.Rotate(ac, Vector2.Zero, Math.PI / 2);
            Assert.AreEqual(atom1.Point2D.Value.X, -1, .2);
            Assert.AreEqual(atom1.Point2D.Value.Y, 1, .2);
            Assert.AreEqual(atom2.Point2D.Value.X, 0, .2);
            Assert.AreEqual(atom2.Point2D.Value.Y, 1, .2);
            atom2.Point2D = Vector2.Zero;
            GeometryUtil.Rotate(ac, Vector2.Zero, Math.PI);
            Assert.IsFalse(double.IsNaN(atom2.Point2D.Value.X));
            Assert.IsFalse(double.IsNaN(atom2.Point2D.Value.Y));
        }

        [TestMethod()]
        public void TestGetMinMax_IAtomContainer()
        {
            Atom atom1 = new Atom("C");
            atom1.Point2D = new Vector2(1, 1);
            Atom atom2 = new Atom("C");
            atom2.Point2D = new Vector2(1, 0);
            IAtomContainer ac = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            ac.Atoms.Add(atom1);
            ac.Atoms.Add(atom2);
            double[] minmax = GeometryUtil.GetMinMax(ac);
            Assert.AreEqual(minmax[0], 1d, .1);
            Assert.AreEqual(minmax[1], 0d, .1);
            Assert.AreEqual(minmax[2], 1d, .1);
            Assert.AreEqual(minmax[3], 1d, .1);
        }

        // @cdk.bug 2094881
        [TestMethod()]
        public void TestGetMinMax2()
        {
            Atom atom1 = new Atom("C");
            atom1.Point2D = new Vector2(-2, -1);
            Atom atom2 = new Atom("C");
            atom2.Point2D = new Vector2(-5, -1);
            IAtomContainer ac = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            ac.Atoms.Add(atom1);
            ac.Atoms.Add(atom2);
            double[] minmax = GeometryUtil.GetMinMax(ac);
            Assert.AreEqual(-5, minmax[0], .1);
            Assert.AreEqual(-1, minmax[1], .1);
            Assert.AreEqual(-2, minmax[2], .1);
            Assert.AreEqual(-1, minmax[3], .1);
        }

        [TestMethod()]
        public void TestRotate_IAtom_Point3d_Point3d_Double()
        {
            Atom atom1 = new Atom("C");
            atom1.Point3D = new Vector3(1, 1, 0);
            GeometryUtil.Rotate(atom1, new Vector3(2, 0, 0), new Vector3(2, 2, 0), 90);
            AssertAreEqual(new Vector3(2.0, 1.0, 1.0), atom1.Point3D, 0.2);
        }

        [TestMethod()]
        public void TestNormalize_Point3d()
        {
            Vector3 p = new Vector3(1, 1, 0);
            p = Vector3.Normalize(p);
            Assert.AreEqual(p.X, 0.7, .1);
            Assert.AreEqual(p.Y, 0.7, .1);
            Assert.AreEqual(p.Z, 0.0, .1);
        }

        [TestMethod()]
        public void TestGet2DCenter_IAtomContainer()
        {
            Atom atom1 = new Atom("C");
            atom1.Point2D = new Vector2(1, 1);
            Atom atom2 = new Atom("C");
            atom2.Point2D = new Vector2(1, 0);
            IAtomContainer ac = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            ac.Atoms.Add(atom1);
            ac.Atoms.Add(atom2);
            Vector2 p = GeometryUtil.Get2DCenter(ac);
            Assert.AreEqual(p.X, 1.0, .1);
            Assert.AreEqual(p.Y, 0.5, .1);
        }

        [TestMethod()]
        public void TestGet2DCenterOfMass_IAtomContainer()
        {
            Atom atom1 = new Atom("C");
            atom1.Point2D = new Vector2(1, 1);
            atom1.ExactMass = 12.0;
            Atom atom2 = new Atom("C");
            atom2.Point2D = new Vector2(1, 0);
            atom2.ExactMass = 12.0;
            IAtomContainer ac = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            ac.Atoms.Add(atom1);
            ac.Atoms.Add(atom2);
            Vector2? p = GeometryUtil.Get2DCentreOfMass(ac);
            Assert.IsNotNull(p);
            Assert.AreEqual(p.Value.X, 1.0, .1);
            Assert.AreEqual(p.Value.Y, 0.5, .1);
        }

        [TestMethod()]
        public void TestGet2DCenter_arrayIAtom()
        {
            IAtomContainer container = new AtomContainer();
            Atom atom1 = new Atom("C");
            atom1.Point2D = new Vector2(1, 1);
            Atom atom2 = new Atom("C");
            atom2.Point2D = new Vector2(1, 0);
            container.Atoms.Add(atom1);
            container.Atoms.Add(atom2);
            Vector2 p = GeometryUtil.Get2DCenter(container.Atoms);
            Assert.AreEqual(p.X, 1.0, .1);
            Assert.AreEqual(p.Y, 0.5, .1);
        }

        [TestMethod()]
        public void TestGet2DCenter_IRingSet()
        {
            Atom atom1 = new Atom("C");
            atom1.Point2D = new Vector2(1, 1);
            Atom atom2 = new Atom("C");
            atom2.Point2D = new Vector2(1, 0);
            IRing ac = Default.ChemObjectBuilder.Instance.NewRing();
            ac.Atoms.Add(atom1);
            ac.Atoms.Add(atom2);
            IRingSet ringset = Default.ChemObjectBuilder.Instance.NewRingSet();
            ringset.Add(ac);
            Vector2 p = GeometryUtil.Get2DCenter(ac);
            Assert.AreEqual(p.X, 1.0, .1);
            Assert.AreEqual(p.Y, 0.5, .1);
        }

        [TestMethod()]
        public void TestGet2DCenter_Iterator()
        {
            Atom atom1 = new Atom("C");
            atom1.Point2D = new Vector2(1, 1);
            Atom atom2 = new Atom("C");
            atom2.Point2D = new Vector2(1, 0);
            IAtomContainer ac = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            ac.Atoms.Add(atom1);
            ac.Atoms.Add(atom2);
            Vector2 p = GeometryUtil.Get2DCenter(ac.Atoms);
            Assert.AreEqual(p.X, 1.0, .1);
            Assert.AreEqual(p.Y, 0.5, .1);
        }

        [TestMethod()]
        public void TestHas2DCoordinates_IAtom()
        {
            Atom atom1 = new Atom("C");
            atom1.Point2D = new Vector2(1, 1);
            Assert.IsTrue(GeometryUtil.Has2DCoordinates(atom1));

            atom1 = new Atom("C");
            atom1.Point3D = new Vector3(1, 1, 1);
            Assert.IsFalse(GeometryUtil.Has2DCoordinates(atom1));
        }

        [TestMethod()]
        public void TestHas2DCoordinates_IBond()
        {
            Atom atom1 = new Atom("C");
            atom1.Point2D = new Vector2(1, 1);
            Atom atom2 = new Atom("C");
            atom2.Point2D = new Vector2(1, 0);
            IBond bond = new Bond(atom1, atom2);
            Assert.IsTrue(GeometryUtil.Has2DCoordinates(bond));

            atom1 = new Atom("C");
            atom1.Point3D = new Vector3(1, 1, 1);
            atom2 = new Atom("C");
            atom2.Point3D = new Vector3(1, 0, 5);
            bond = new Bond(atom1, atom2);
            Assert.IsFalse(GeometryUtil.Has2DCoordinates(bond));
        }

        [TestMethod()]
        public void TestHas2DCoordinatesNew_IAtomContainer()
        {
            Atom atom1 = new Atom("C");
            atom1.Point2D = new Vector2(1, 1);
            Atom atom2 = new Atom("C");
            atom2.Point2D = new Vector2(1, 0);
            IAtomContainer container = new AtomContainer();
            container.Atoms.Add(atom1);
            container.Atoms.Add(atom2);
            Assert.AreEqual(2, GeometryUtil.Has2DCoordinatesNew(container));

            atom1 = new Atom("C");
            atom1.Point2D = new Vector2(1, 1);
            atom2 = new Atom("C");
            atom2.Point3D = new Vector3(1, 0, 1);
            container = new AtomContainer();
            container.Atoms.Add(atom1);
            container.Atoms.Add(atom2);
            Assert.AreEqual(1, GeometryUtil.Has2DCoordinatesNew(container));

            atom1 = new Atom("C");
            atom1.Point3D = new Vector3(1, 1, 1);
            atom2 = new Atom("C");
            atom2.Point3D = new Vector3(1, 0, 5);
            container = new AtomContainer();
            container.Atoms.Add(atom1);
            container.Atoms.Add(atom2);
            Assert.AreEqual(0, GeometryUtil.Has2DCoordinatesNew(container));
        }

        [TestMethod()]
        public void TestHas3DCoordinates_IAtomContainer()
        {
            Atom atom1 = new Atom("C");
            atom1.Point2D = new Vector2(1, 1);
            Atom atom2 = new Atom("C");
            atom2.Point2D = new Vector2(1, 0);
            IAtomContainer container = new AtomContainer();
            container.Atoms.Add(atom1);
            container.Atoms.Add(atom2);
            Assert.IsFalse(GeometryUtil.Has3DCoordinates(container));

            atom1 = new Atom("C");
            atom1.Point3D = new Vector3(1, 1, 1);
            atom2 = new Atom("C");
            atom2.Point3D = new Vector3(1, 0, 5);
            container = new AtomContainer();
            container.Atoms.Add(atom1);
            container.Atoms.Add(atom2);
            Assert.IsTrue(GeometryUtil.Has3DCoordinates(container));
        }

        [TestMethod()]
        public void TestHas3DCoordinates_EmptyAtomContainer()
        {
            IAtomContainer container = new AtomContainer();
            Assert.IsFalse(GeometryUtil.Has3DCoordinates(container));
            Assert.IsFalse(GeometryUtil.Has3DCoordinates((IAtomContainer)null));
        }

        [TestMethod()]
        public void Get3DCoordinateCoverage_EmptyAtomContainer()
        {
            IAtomContainer container = new AtomContainer();
            Assert.AreEqual(GeometryUtil.CoordinateCoverages.None, GeometryUtil.Get3DCoordinateCoverage(container));
            Assert.AreEqual(GeometryUtil.CoordinateCoverages.None,
                                GeometryUtil.Get3DCoordinateCoverage((IAtomContainer)null));
        }

        [TestMethod()]
        public void Get3DCoordinateCoverage_Partial()
        {
            IAtomContainer container = new AtomContainer();

            IAtom atom1 = new Atom("C");
            IAtom atom2 = new Atom("C");
            IAtom atom3 = new Atom("C");

            atom1.Point3D = new Vector3(1, 1, 0);
            atom3.Point3D = new Vector3(1, 1, 0);

            container.Atoms.Add(atom1);
            container.Atoms.Add(atom2);
            container.Atoms.Add(atom3);

            Assert.AreEqual(GeometryUtil.CoordinateCoverages.Partial, GeometryUtil.Get3DCoordinateCoverage(container));
        }

        [TestMethod()]
        public void Get3DCoordinateCoverage_Full()
        {
            IAtomContainer container = new AtomContainer();

            IAtom atom1 = new Atom("C");
            IAtom atom2 = new Atom("C");
            IAtom atom3 = new Atom("C");

            atom1.Point3D = new Vector3(1, 1, 0);
            atom2.Point3D = new Vector3(2, 1, 0);
            atom3.Point3D = new Vector3(1, 2, 0);

            container.Atoms.Add(atom1);
            container.Atoms.Add(atom2);
            container.Atoms.Add(atom3);

            Assert.AreEqual(GeometryUtil.CoordinateCoverages.Full, GeometryUtil.Get3DCoordinateCoverage(container));
        }

        [TestMethod()]
        public void Get3DCoordinateCoverage_None_2D()
        {
            IAtomContainer container = new AtomContainer();

            IAtom atom1 = new Atom("C");
            IAtom atom2 = new Atom("C");
            IAtom atom3 = new Atom("C");

            atom1.Point2D = new Vector2(1, 1);
            atom2.Point2D = new Vector2(2, 1);
            atom3.Point2D = new Vector2(1, 2);

            container.Atoms.Add(atom1);
            container.Atoms.Add(atom2);
            container.Atoms.Add(atom3);

            Assert.AreEqual(GeometryUtil.CoordinateCoverages.None, GeometryUtil.Get3DCoordinateCoverage(container));
        }

        [TestMethod()]
        public void TestTranslateAllPositive_IAtomContainer_HashMap()
        {
            Atom atom1 = new Atom("C");
            atom1.Point2D = new Vector2(-1, -1);
            Atom atom2 = new Atom("C");
            atom2.Point2D = new Vector2(1, 0);
            IAtomContainer ac = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            ac.Atoms.Add(atom1);
            ac.Atoms.Add(atom2);
            GeometryUtil.TranslateAllPositive(ac);
            Assert.AreEqual(atom1.Point2D.Value.X, 0.0, 0.01);
            Assert.AreEqual(atom1.Point2D.Value.Y, 0.0, 0.01);
            Assert.AreEqual(atom2.Point2D.Value.X, 2.0, 0.01);
            Assert.AreEqual(atom2.Point2D.Value.Y, 1.0, 0.01);
        }

        [TestMethod()]
        public void TestGetLength2D_IBond_HashMap()
        {
            Atom atom1 = new Atom("C");
            atom1.Point2D = new Vector2(-1, -1);
            Atom atom2 = new Atom("C");
            atom2.Point2D = new Vector2(1, 0);
            IBond bond = new Bond(atom1, atom2);
            IAtomContainer ac = Default.ChemObjectBuilder.Instance.NewAtomContainer();
            ac.Atoms.Add(atom1);
            ac.Atoms.Add(atom2);
            Assert.AreEqual(GeometryUtil.GetLength2D(bond), 2.23, 0.01);
        }

        [TestMethod()]
        public void TestGetClosestAtom_Multiatom()
        {
            IAtom atom1 = new Atom("C");
            atom1.Point2D = new Vector2(-1, -1);
            IAtom atom2 = new Atom("C");
            atom2.Point2D = new Vector2(1, 0);
            IAtom atom3 = new Atom("C");
            atom3.Point2D = new Vector2(5, 0);
            IAtomContainer acont = new AtomContainer();
            acont.Atoms.Add(atom1);
            acont.Atoms.Add(atom2);
            acont.Atoms.Add(atom3);
            Assert.AreEqual(atom2, GeometryUtil.GetClosestAtom(acont, atom1));
            Assert.AreEqual(atom1, GeometryUtil.GetClosestAtom(acont, atom2));
            Assert.AreEqual(atom2, GeometryUtil.GetClosestAtom(acont, atom3));
        }

        [TestMethod()]
        public void TestGetClosestAtom_Double_Double_IAtomContainer_IAtom()
        {
            IAtom atom1 = new Atom("C");
            atom1.Point2D = new Vector2(1, 0);
            IAtom atom2 = new Atom("C");
            atom2.Point2D = new Vector2(5, 0);
            IAtomContainer acont = new AtomContainer();
            acont.Atoms.Add(atom1);
            acont.Atoms.Add(atom2);
            Assert.AreEqual(atom2, GeometryUtil.GetClosestAtom(1.0, 0.0, acont, atom1));
            Assert.AreEqual(atom1, GeometryUtil.GetClosestAtom(1.0, 0.0, acont, null));
        }

        /// <summary>
        /// Tests if not the central atom is returned as closest atom.
        /// </summary>
        [TestMethod()]
        public void TestGetClosestAtom_IAtomContainer_IAtom()
        {
            IAtom atom1 = new Atom("C");
            atom1.Point2D = new Vector2(-1, -1);
            IAtom atom2 = new Atom("C");
            atom2.Point2D = new Vector2(1, 0);
            IAtomContainer acont = new AtomContainer();
            acont.Atoms.Add(atom1);
            acont.Atoms.Add(atom2);
            Assert.AreEqual(atom2, GeometryUtil.GetClosestAtom(acont, atom1));
            Assert.AreEqual(atom1, GeometryUtil.GetClosestAtom(acont, atom2));
        }

        [TestMethod()]
        public void TestShiftContainerHorizontal_IAtomContainer_Rectangle2D_Rectangle2D_Double()
        {
            IAtom atom1 = new Atom("C");
            atom1.Point2D = new Vector2(0, 1);
            IAtom atom2 = new Atom("C");
            atom2.Point2D = new Vector2(1, 0);
            IAtomContainer react1 = new AtomContainer();
            react1.Atoms.Add(atom1);
            react1.Atoms.Add(atom2);
            IAtomContainer react2 = (IAtomContainer)react1.Clone();

            // shift the second molecule right
            GeometryUtil.ShiftContainer(react2, GeometryUtil.GetMinMax(react2), GeometryUtil.GetMinMax(react1), 1.0);
            // assert all coordinates of the second molecule moved right
            AtomContainerDiff.Diff(react1, react2);
            for (int i = 0; i < 2; i++)
            {
                atom1 = react1.Atoms[0];
                atom2 = react2.Atoms[0];
                // so, y coordinates should be the same
                Assert.AreEqual(atom1.Point2D.Value.Y, atom2.Point2D.Value.Y, 0.0);
                // but, x coordinates should not
                Assert.IsTrue(atom1.Point2D.Value.X < atom2.Point2D.Value.X);
            }
        }

        /// <summary>
        /// Unit tests that tests the situation where two vertical two-atom
        /// molecules are with the same x coordinates.
        /// </summary>
        // @ Thrown when the cloning failed.
        [TestMethod()]
        public void TestShiftContainerHorizontal_Two_vertical_molecules()
        {
            IAtom atom1 = new Atom("C");
            atom1.Point2D = Vector2.Zero;
            IAtom atom2 = new Atom("C");
            atom2.Point2D = new Vector2(0, 1);
            IAtomContainer react1 = new AtomContainer();
            react1.Atoms.Add(atom1);
            react1.Atoms.Add(atom2);
            IAtomContainer react2 = (IAtomContainer)react1.Clone();

            // shift the second molecule right
            GeometryUtil.ShiftContainer(react2, GeometryUtil.GetMinMax(react2), GeometryUtil.GetMinMax(react1), 1.0);
            // assert all coordinates of the second molecule moved right
            AtomContainerDiff.Diff(react1, react2);
            for (int i = 0; i < 2; i++)
            {
                atom1 = react1.Atoms[0];
                atom2 = react2.Atoms[0];
                // so, y coordinates should be the same
                Assert.AreEqual(atom1.Point2D.Value.Y, atom2.Point2D.Value.Y, 0.0);
                // but, x coordinates should not
                Assert.IsTrue(atom1.Point2D.Value.X < atom2.Point2D.Value.X);
            }
        }

        [TestMethod()]
        public void TestGetBondLengthAverage_IReaction()
        {
            IAtom atom1 = new Atom("C");
            atom1.Point2D = Vector2.Zero;
            IAtom atom2 = new Atom("C");
            atom2.Point2D = new Vector2(1, 0);
            IAtomContainer acont = new AtomContainer();
            IReaction reaction = new Reaction();
            reaction.Reactants.Add(acont);
            acont.Atoms.Add(atom1);
            acont.Atoms.Add(atom2);
            acont.AddBond(acont.Atoms[0], acont.Atoms[1], BondOrder.Single);
            Assert.AreEqual(1.0, GeometryUtil.GetBondLengthAverage(reaction), 0.0);
        }

        /// <summary>
        /// Tests if the bond length average is calculated based on all
        /// <see cref="IAtomContainer"/>s in the IReaction.
        /// </summary>
        [TestMethod()]
        public void TestGetBondLengthAverage_MultiReaction()
        {
            IReaction reaction = new Reaction();

            // mol 1
            IAtom atom1 = new Atom("C");
            atom1.Point2D = Vector2.Zero;
            IAtom atom2 = new Atom("C");
            atom2.Point2D = new Vector2(1, 0);
            IAtomContainer acont = new AtomContainer();
            reaction.Reactants.Add(acont);
            acont.Atoms.Add(atom1);
            acont.Atoms.Add(atom2);
            acont.AddBond(acont.Atoms[0], acont.Atoms[1], BondOrder.Single);

            // mol 2
            atom1 = new Atom("C");
            atom1.Point2D = Vector2.Zero;
            atom2 = new Atom("C");
            atom2.Point2D = new Vector2(3, 0);
            acont = new AtomContainer();
            reaction.Products.Add(acont);
            acont.Atoms.Add(atom1);
            acont.Atoms.Add(atom2);
            acont.AddBond(acont.Atoms[0], acont.Atoms[1], BondOrder.Single);

            Assert.AreEqual(2.0, GeometryUtil.GetBondLengthAverage(reaction), 0.0);
        }

        [TestMethod()]
        public void TestShiftReactionVertical_IAtomContainer_Rectangle2D_Rectangle2D_Double()
        {
            IAtom atom1 = new Atom("C");
            atom1.Point2D = new Vector2(0, 1);
            IAtom atom2 = new Atom("C");
            atom2.Point2D = new Vector2(1, 0);
            IAtomContainer react1 = new AtomContainer();
            IReaction reaction = new Reaction();
            reaction.Reactants.Add(react1);
            react1.Atoms.Add(atom1);
            react1.Atoms.Add(atom2);
            react1.AddBond(react1.Atoms[0], react1.Atoms[1], BondOrder.Single);
            IReaction reaction2 = (IReaction)reaction.Clone();
            IAtomContainer react2 = reaction2.Reactants[0];

            // shift the second reaction up
            GeometryUtil.ShiftReactionVertical(reaction2, GeometryUtil.GetMinMax(react2), GeometryUtil.GetMinMax(react1),
                                               1.0);
            // assert all coordinates of the second reaction moved up
            AtomContainerDiff.Diff(react1, react2);
            for (int i = 0; i < 2; i++)
            {
                atom1 = react1.Atoms[0];
                atom2 = react2.Atoms[0];
                // so, x coordinates should be the same
                Assert.AreEqual(atom1.Point2D.Value.X, atom2.Point2D.Value.X, 0.0);
                // but, y coordinates should not
                Assert.IsTrue(atom1.Point2D.Value.Y < atom2.Point2D.Value.Y);
            }
        }

        /// <summary>
        /// Unit tests that tests the situation where two horizontal two-atom
        /// molecules are with the same y coordinates.
        /// </summary>
        // @ Thrown when the cloning failed.
        [TestMethod()]
        public void TestShiftReactionVertical_Two_horizontal_molecules()
        {
            IAtom atom1 = new Atom("C");
            atom1.Point2D = Vector2.Zero;
            IAtom atom2 = new Atom("C");
            atom2.Point2D = new Vector2(1, 0);
            IAtomContainer react1 = new AtomContainer();
            IReaction reaction = new Reaction();
            reaction.Reactants.Add(react1);
            react1.Atoms.Add(atom1);
            react1.Atoms.Add(atom2);
            react1.AddBond(react1.Atoms[0], react1.Atoms[1], BondOrder.Single);
            IReaction reaction2 = (IReaction)reaction.Clone();
            IAtomContainer react2 = reaction2.Reactants[0];

            // shift the second reaction up
            GeometryUtil.ShiftReactionVertical(reaction2, GeometryUtil.GetMinMax(react2), GeometryUtil.GetMinMax(react1),
                                               1.0);
            // assert all coordinates of the second reaction moved up
            AtomContainerDiff.Diff(react1, react2);
            for (int i = 0; i < 2; i++)
            {
                atom1 = react1.Atoms[0];
                atom2 = react2.Atoms[0];
                // so, x coordinates should be the same
                Assert.AreEqual(atom1.Point2D.Value.X, atom2.Point2D.Value.X, 0.0);
                // but, y coordinates should not
                Assert.IsTrue(atom1.Point2D.Value.Y < atom2.Point2D.Value.Y);
            }
        }

        [TestMethod()]
        public void TestGetBestAlignmentForLabelXY()
        {
            string TYPE = "C";
            IAtom zero = new Atom("O");
            zero.Point2D = new Vector2();
            IAtom pX = new Atom(TYPE);
            pX.Point2D = new Vector2(1, 0);
            IAtom nX = new Atom(TYPE);
            nX.Point2D = new Vector2(-1, 0);
            IAtom pY = new Atom(TYPE);
            pY.Point2D = new Vector2(0, 1);
            IAtom nY = new Atom(TYPE);
            nY.Point2D = new Vector2(0, -1);

            Assert.AreEqual(-1, AlignmentTestHelper(zero, pX));
            Assert.AreEqual(1, AlignmentTestHelper(zero, nX));
            Assert.AreEqual(-2, AlignmentTestHelper(zero, pY));
            Assert.AreEqual(2, AlignmentTestHelper(zero, nY));

            Assert.AreEqual(1, AlignmentTestHelper(zero, pY, nY));
        }

        [TestMethod()]
        public void MedianBondLength()
        {
            IAtomContainer container = new AtomContainer();
            container.Atoms.Add(AtomAt(Vector2.Zero));
            container.Atoms.Add(AtomAt(new Vector2(0, 1.5)));
            container.Atoms.Add(AtomAt(new Vector2(0, -1.5)));
            container.Atoms.Add(AtomAt(new Vector2(0, 5)));
            container.AddBond(container.Atoms[0], container.Atoms[1], BondOrder.Single);
            container.AddBond(container.Atoms[0], container.Atoms[2], BondOrder.Single);
            container.AddBond(container.Atoms[0], container.Atoms[3], BondOrder.Single);
            Assert.AreEqual(1.5, GeometryUtil.GetBondLengthMedian(container));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void MedianBondLengthNoBonds()
        {
            IAtomContainer container = new AtomContainer();
            container.Atoms.Add(AtomAt(Vector2.Zero));
            container.Atoms.Add(AtomAt(new Vector2(0, 1.5)));
            container.Atoms.Add(AtomAt(new Vector2(0, -1.5)));
            container.Atoms.Add(AtomAt(new Vector2(0, 5)));
            GeometryUtil.GetBondLengthMedian(container);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void MedianBondLengthNoPoints()
        {
            IAtomContainer container = new AtomContainer();
            container.Atoms.Add(AtomAt(Vector2.Zero));
            container.Atoms.Add(AtomAt(new Vector2(0, 1.5)));
            container.Atoms.Add(AtomAt(null));
            container.Atoms.Add(AtomAt(new Vector2(0, 5)));
            container.AddBond(container.Atoms[0], container.Atoms[1], BondOrder.Single);
            container.AddBond(container.Atoms[0], container.Atoms[2], BondOrder.Single);
            container.AddBond(container.Atoms[0], container.Atoms[3], BondOrder.Single);
            GeometryUtil.GetBondLengthMedian(container);
        }

        [TestMethod()]
        public void MedianBondLengthOneBond()
        {
            IAtomContainer container = new AtomContainer();
            container.Atoms.Add(AtomAt(Vector2.Zero));
            container.Atoms.Add(AtomAt(new Vector2(0, 1.5)));
            container.AddBond(container.Atoms[0], container.Atoms[1], BondOrder.Single);
            Assert.AreEqual(1.5, GeometryUtil.GetBondLengthMedian(container));
        }

        [TestMethod()]
        public void MedianBondLengthWithZeroLengthBonds()
        {
            IAtomContainer container = new AtomContainer();
            container.Atoms.Add(AtomAt(Vector2.Zero));
            container.Atoms.Add(AtomAt(Vector2.Zero));
            container.Atoms.Add(AtomAt(Vector2.Zero));
            container.Atoms.Add(AtomAt(Vector2.Zero));
            container.Atoms.Add(AtomAt(Vector2.Zero));
            container.Atoms.Add(AtomAt(Vector2.Zero));
            container.Atoms.Add(AtomAt(Vector2.Zero));
            container.Atoms.Add(AtomAt(Vector2.Zero));
            container.Atoms.Add(AtomAt(Vector2.Zero));
            container.Atoms.Add(AtomAt(Vector2.Zero));
            container.Atoms.Add(AtomAt(Vector2.Zero));
            container.Atoms.Add(AtomAt(new Vector2(0, 1)));
            container.Atoms.Add(AtomAt(new Vector2(0, 2)));
            container.Atoms.Add(AtomAt(new Vector2(0, 3)));
            for (int i = 0; i < container.Atoms.Count - 1; i++)
            {
                container.AddBond(container.Atoms[i], container.Atoms[i + 1], BondOrder.Single);
            }
            Assert.AreEqual(1d, GeometryUtil.GetBondLengthMedian(container));
        }

        private IAtom AtomAt(Vector2? p)
        {
            IAtom atom = new Atom("C");
            atom.Point2D = p;
            return atom;
        }

        private int AlignmentTestHelper(IAtom zero, params IAtom[] pos)
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(zero);
            foreach (var atom in pos)
            {
                mol.Atoms.Add(atom);
                mol.Bonds.Add(new Bond(zero, atom));
            }
            return GeometryUtil.GetBestAlignmentForLabelXY(mol, zero);
        }
    }
}
