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
using NCDK.IO;
using NCDK.Isomorphisms;
using NCDK.Numerics;
using NCDK.Tools.Diff;
using System;
using System.Collections.Generic;

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
        private readonly IChemObjectBuilder builder = CDK.Builder;

        [TestMethod()]
        public void TestHas2DCoordinatesIAtomContainer()
        {
            var atom1 = builder.NewAtom("C");
            atom1.Point2D = new Vector2(1, 1);
            var atom2 = builder.NewAtom("C");
            atom2.Point2D = new Vector2(1, 0);
            var container = builder.NewAtomContainer();
            container.Atoms.Add(atom1);
            container.Atoms.Add(atom2);
            Assert.IsTrue(GeometryUtil.Has2DCoordinates(container));

            atom1 = builder.NewAtom("C");
            atom1.Point3D = new Vector3(1, 1, 1);
            atom2 = builder.NewAtom("C");
            atom2.Point3D = new Vector3(1, 0, 5);
            container = builder.NewAtomContainer();
            container.Atoms.Add(atom1);
            container.Atoms.Add(atom2);
            Assert.IsFalse(GeometryUtil.Has2DCoordinates(container));
        }

        [TestMethod()]
        public void TestHas2DCoordinatesEmptyAtomContainer()
        {
            var container = builder.NewAtomContainer();
            Assert.IsFalse(GeometryUtil.Has2DCoordinates(container));
            Assert.IsFalse(GeometryUtil.Has2DCoordinates((IAtomContainer)null));
        }

        [TestMethod()]
        public void TestHas2DCoordinatesPartial()
        {
            var container = builder.NewAtomContainer();
            var atom1 = builder.NewAtom("C");
            var atom2 = builder.NewAtom("C");
            atom1.Point2D = new Vector2(1, 1);
            container.Atoms.Add(atom1);
            Assert.IsTrue(GeometryUtil.Has2DCoordinates(container));
            container.Atoms.Add(atom2);
            Assert.IsFalse(GeometryUtil.Has2DCoordinates(container));
        }

        // @cdk.bug 2936440
        [TestMethod()]
        public void TestHas2DCoordinatesWith000()
        {
            string filenameMol = "NCDK.Data.MDL.with000coordinate.mol";
            var ins = ResourceLoader.GetAsStream(filenameMol);
            IAtomContainer molOne = null;
            using (var reader = new MDLV2000Reader(ins, ChemObjectReaderMode.Strict))
            {
                molOne = reader.Read(builder.NewAtomContainer());
            }
            Assert.IsTrue(GeometryUtil.Has2DCoordinates(molOne));
        }

        [TestMethod()]
        public void Get2DCoordinateCoverageEmptyAtomContainer()
        {
            var container = builder.NewAtomContainer();
            Assert.AreEqual(GeometryUtil.CoordinateCoverage.None, GeometryUtil.Get2DCoordinateCoverage(container));
            Assert.AreEqual(GeometryUtil.CoordinateCoverage.None,
                                GeometryUtil.Get2DCoordinateCoverage((IAtomContainer)null));
        }

        [TestMethod()]
        public void Get2DCoordinateCoveragePartial()
        {
            var container = builder.NewAtomContainer();

            var atom1 = builder.NewAtom("C");
            var atom2 = builder.NewAtom("C");
            var atom3 = builder.NewAtom("C");

            atom1.Point2D = new Vector2(1, 1);
            atom3.Point2D = new Vector2(1, 1);

            container.Atoms.Add(atom1);
            container.Atoms.Add(atom2);
            container.Atoms.Add(atom3);

            Assert.AreEqual(GeometryUtil.CoordinateCoverage.Partial, GeometryUtil.Get2DCoordinateCoverage(container));
        }

        [TestMethod()]
        public void Get2DCoordinateCoverageFull()
        {
            var container = builder.NewAtomContainer();

            var atom1 = builder.NewAtom("C");
            var atom2 = builder.NewAtom("C");
            var atom3 = builder.NewAtom("C");

            atom1.Point2D = new Vector2(1, 1);
            atom2.Point2D = new Vector2(2, 1);
            atom3.Point2D = new Vector2(1, 2);

            container.Atoms.Add(atom1);
            container.Atoms.Add(atom2);
            container.Atoms.Add(atom3);

            Assert.AreEqual(GeometryUtil.CoordinateCoverage.Full, GeometryUtil.Get2DCoordinateCoverage(container));
        }

        [TestMethod()]
        public void Get2DCoordinateCoverageNone3D()
        {
            var container = builder.NewAtomContainer();

            var atom1 = builder.NewAtom("C");
            var atom2 = builder.NewAtom("C");
            var atom3 = builder.NewAtom("C");

            atom1.Point3D = new Vector3(1, 1, 0);
            atom2.Point3D = new Vector3(2, 1, 0);
            atom3.Point3D = new Vector3(1, 2, 0);

            container.Atoms.Add(atom1);
            container.Atoms.Add(atom2);
            container.Atoms.Add(atom3);

            Assert.AreEqual(GeometryUtil.CoordinateCoverage.None, GeometryUtil.Get2DCoordinateCoverage(container));
        }

        [TestMethod()]
        public void TestTranslateAllPositiveIAtomContainer()
        {
            var container = builder.NewAtomContainer();
            var atom = builder.NewAtom(ChemicalElement.C);
            atom.Point2D = new Vector2(-3, -2);
            container.Atoms.Add(atom);
            GeometryUtil.TranslateAllPositive(container);
            Assert.IsTrue(0 <= atom.Point2D.Value.X);
            Assert.IsTrue(0 <= atom.Point2D.Value.Y);
        }

        [TestMethod()]
        public void TestGetLength2DIBond()
        {
            var o = builder.NewAtom("O", new Vector2(0.0, 0.0));
            var c = builder.NewAtom("C", new Vector2(1.0, 0.0));
            var bond = builder.NewBond(c, o);

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
            var mappedAtoms = new Dictionary<int, int>();
            var reader = new MDLV2000Reader(ins, ChemObjectReaderMode.Strict);
            molOne = reader.Read(builder.NewAtomContainer());

            ins = ResourceLoader.GetAsStream(filenameMolTwo);
            reader = new MDLV2000Reader(ins, ChemObjectReaderMode.Strict);
            molTwo = reader.Read(builder.NewAtomContainer());

            AtomMappingTools.MapAtomsOfAlignedStructures(molOne, molTwo, mappedAtoms);
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
            var atom1 = builder.NewAtom("C");
            atom1.Point2D = new Vector2(1, 1);
            var atom2 = builder.NewAtom("C");
            atom2.Point2D = new Vector2(1, 0);
            var ac = builder.NewAtomContainer();
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
        public void TestGetMinMaxIAtomContainer()
        {
            var atom1 = builder.NewAtom("C");
            atom1.Point2D = new Vector2(1, 1);
            var atom2 = builder.NewAtom("C");
            atom2.Point2D = new Vector2(1, 0);
            var ac = builder.NewAtomContainer();
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
            var atom1 = builder.NewAtom("C");
            atom1.Point2D = new Vector2(-2, -1);
            var atom2 = builder.NewAtom("C");
            atom2.Point2D = new Vector2(-5, -1);
            var ac = builder.NewAtomContainer();
            ac.Atoms.Add(atom1);
            ac.Atoms.Add(atom2);
            double[] minmax = GeometryUtil.GetMinMax(ac);
            Assert.AreEqual(-5, minmax[0], .1);
            Assert.AreEqual(-1, minmax[1], .1);
            Assert.AreEqual(-2, minmax[2], .1);
            Assert.AreEqual(-1, minmax[3], .1);
        }

        [TestMethod()]
        public void TestRotateIAtomPoint3dPoint3dDouble()
        {
            var atom1 = builder.NewAtom("C");
            atom1.Point3D = new Vector3(1, 1, 0);
            GeometryUtil.Rotate(atom1, new Vector3(2, 0, 0), new Vector3(2, 2, 0), 90);
            AssertAreEqual(new Vector3(2.0, 1.0, 1.0), atom1.Point3D, 0.2);
        }

        [TestMethod()]
        public void TestNormalizePoint3d()
        {
            var p = new Vector3(1, 1, 0);
            p = Vector3.Normalize(p);
            Assert.AreEqual(p.X, 0.7, .1);
            Assert.AreEqual(p.Y, 0.7, .1);
            Assert.AreEqual(p.Z, 0.0, .1);
        }

        [TestMethod()]
        public void TestGet2DCenterIAtomContainer()
        {
            var atom1 = builder.NewAtom("C");
            atom1.Point2D = new Vector2(1, 1);
            var atom2 = builder.NewAtom("C");
            atom2.Point2D = new Vector2(1, 0);
            var ac = builder.NewAtomContainer();
            ac.Atoms.Add(atom1);
            ac.Atoms.Add(atom2);
            var p = GeometryUtil.Get2DCenter(ac);
            Assert.AreEqual(p.X, 1.0, .1);
            Assert.AreEqual(p.Y, 0.5, .1);
        }

        [TestMethod()]
        public void TestGet2DCenterOfMassIAtomContainer()
        {
            var atom1 = builder.NewAtom("C");
            atom1.Point2D = new Vector2(1, 1);
            atom1.ExactMass = 12.0;
            var atom2 = builder.NewAtom("C");
            atom2.Point2D = new Vector2(1, 0);
            atom2.ExactMass = 12.0;
            var ac = builder.NewAtomContainer();
            ac.Atoms.Add(atom1);
            ac.Atoms.Add(atom2);
            var p = GeometryUtil.Get2DCentreOfMass(ac);
            Assert.IsNotNull(p);
            Assert.AreEqual(p.Value.X, 1.0, .1);
            Assert.AreEqual(p.Value.Y, 0.5, .1);
        }

        [TestMethod()]
        public void TestGet2DCenterArrayIAtom()
        {
            var container = builder.NewAtomContainer();
            var atom1 = builder.NewAtom("C");
            atom1.Point2D = new Vector2(1, 1);
            var atom2 = builder.NewAtom("C");
            atom2.Point2D = new Vector2(1, 0);
            container.Atoms.Add(atom1);
            container.Atoms.Add(atom2);
            var p = GeometryUtil.Get2DCenter(container.Atoms);
            Assert.AreEqual(p.X, 1.0, .1);
            Assert.AreEqual(p.Y, 0.5, .1);
        }

        [TestMethod()]
        public void TestGet2DCenterIRingSet()
        {
            var atom1 = builder.NewAtom("C");
            atom1.Point2D = new Vector2(1, 1);
            var atom2 = builder.NewAtom("C");
            atom2.Point2D = new Vector2(1, 0);
            var ac = builder.NewRing();
            ac.Atoms.Add(atom1);
            ac.Atoms.Add(atom2);
            var ringset = builder.NewRingSet();
            ringset.Add(ac);
            var p = GeometryUtil.Get2DCenter(ac);
            Assert.AreEqual(p.X, 1.0, .1);
            Assert.AreEqual(p.Y, 0.5, .1);
        }

        [TestMethod()]
        public void TestGet2DCenterIterator()
        {
            var atom1 = builder.NewAtom("C");
            atom1.Point2D = new Vector2(1, 1);
            var atom2 = builder.NewAtom("C");
            atom2.Point2D = new Vector2(1, 0);
            var ac = builder.NewAtomContainer();
            ac.Atoms.Add(atom1);
            ac.Atoms.Add(atom2);
            var p = GeometryUtil.Get2DCenter(ac.Atoms);
            Assert.AreEqual(p.X, 1.0, .1);
            Assert.AreEqual(p.Y, 0.5, .1);
        }

        [TestMethod()]
        public void TestHas2DCoordinatesIAtom()
        {
            var atom1 = builder.NewAtom("C");
            atom1.Point2D = new Vector2(1, 1);
            Assert.IsTrue(GeometryUtil.Has2DCoordinates(atom1));

            atom1 = builder.NewAtom("C");
            atom1.Point3D = new Vector3(1, 1, 1);
            Assert.IsFalse(GeometryUtil.Has2DCoordinates(atom1));
        }

        [TestMethod()]
        public void TestHas2DCoordinatesIBond()
        {
            var atom1 = builder.NewAtom("C");
            atom1.Point2D = new Vector2(1, 1);
            var atom2 = builder.NewAtom("C");
            atom2.Point2D = new Vector2(1, 0);
            var bond = builder.NewBond(atom1, atom2);
            Assert.IsTrue(GeometryUtil.Has2DCoordinates(bond));

            atom1 = builder.NewAtom("C");
            atom1.Point3D = new Vector3(1, 1, 1);
            atom2 = builder.NewAtom("C");
            atom2.Point3D = new Vector3(1, 0, 5);
            bond = builder.NewBond(atom1, atom2);
            Assert.IsFalse(GeometryUtil.Has2DCoordinates(bond));
        }

        [TestMethod()]
        public void TestHas2DCoordinatesNewIAtomContainer()
        {
            var atom1 = builder.NewAtom("C");
            atom1.Point2D = new Vector2(1, 1);
            var atom2 = builder.NewAtom("C");
            atom2.Point2D = new Vector2(1, 0);
            var container = builder.NewAtomContainer();
            container.Atoms.Add(atom1);
            container.Atoms.Add(atom2);
            Assert.AreEqual(2, GeometryUtil.Has2DCoordinatesNew(container));

            atom1 = builder.NewAtom("C");
            atom1.Point2D = new Vector2(1, 1);
            atom2 = builder.NewAtom("C");
            atom2.Point3D = new Vector3(1, 0, 1);
            container = builder.NewAtomContainer();
            container.Atoms.Add(atom1);
            container.Atoms.Add(atom2);
            Assert.AreEqual(1, GeometryUtil.Has2DCoordinatesNew(container));

            atom1 = builder.NewAtom("C");
            atom1.Point3D = new Vector3(1, 1, 1);
            atom2 = builder.NewAtom("C");
            atom2.Point3D = new Vector3(1, 0, 5);
            container = builder.NewAtomContainer();
            container.Atoms.Add(atom1);
            container.Atoms.Add(atom2);
            Assert.AreEqual(0, GeometryUtil.Has2DCoordinatesNew(container));
        }

        [TestMethod()]
        public void TestHas3DCoordinatesIAtomContainer()
        {
            var atom1 = builder.NewAtom("C");
            atom1.Point2D = new Vector2(1, 1);
            var atom2 = builder.NewAtom("C");
            atom2.Point2D = new Vector2(1, 0);
            var container = builder.NewAtomContainer();
            container.Atoms.Add(atom1);
            container.Atoms.Add(atom2);
            Assert.IsFalse(GeometryUtil.Has3DCoordinates(container));

            atom1 = builder.NewAtom("C");
            atom1.Point3D = new Vector3(1, 1, 1);
            atom2 = builder.NewAtom("C");
            atom2.Point3D = new Vector3(1, 0, 5);
            container = builder.NewAtomContainer();
            container.Atoms.Add(atom1);
            container.Atoms.Add(atom2);
            Assert.IsTrue(GeometryUtil.Has3DCoordinates(container));
        }

        [TestMethod()]
        public void TestHas3DCoordinatesEmptyAtomContainer()
        {
            var container = builder.NewAtomContainer();
            Assert.IsFalse(GeometryUtil.Has3DCoordinates(container));
            Assert.IsFalse(GeometryUtil.Has3DCoordinates((IAtomContainer)null));
        }

        [TestMethod()]
        public void Get3DCoordinateCoverageEmptyAtomContainer()
        {
            var container = builder.NewAtomContainer();
            Assert.AreEqual(GeometryUtil.CoordinateCoverage.None, GeometryUtil.Get3DCoordinateCoverage(container));
            Assert.AreEqual(GeometryUtil.CoordinateCoverage.None, GeometryUtil.Get3DCoordinateCoverage((IAtomContainer)null));
        }

        [TestMethod()]
        public void Get3DCoordinateCoveragePartial()
        {
            var container = builder.NewAtomContainer();

            var atom1 = builder.NewAtom("C");
            var atom2 = builder.NewAtom("C");
            var atom3 = builder.NewAtom("C");

            atom1.Point3D = new Vector3(1, 1, 0);
            atom3.Point3D = new Vector3(1, 1, 0);

            container.Atoms.Add(atom1);
            container.Atoms.Add(atom2);
            container.Atoms.Add(atom3);

            Assert.AreEqual(GeometryUtil.CoordinateCoverage.Partial, GeometryUtil.Get3DCoordinateCoverage(container));
        }

        [TestMethod()]
        public void Get3DCoordinateCoverageFull()
        {
            var container = builder.NewAtomContainer();

            var atom1 = builder.NewAtom("C");
            var atom2 = builder.NewAtom("C");
            var atom3 = builder.NewAtom("C");

            atom1.Point3D = new Vector3(1, 1, 0);
            atom2.Point3D = new Vector3(2, 1, 0);
            atom3.Point3D = new Vector3(1, 2, 0);

            container.Atoms.Add(atom1);
            container.Atoms.Add(atom2);
            container.Atoms.Add(atom3);

            Assert.AreEqual(GeometryUtil.CoordinateCoverage.Full, GeometryUtil.Get3DCoordinateCoverage(container));
        }

        [TestMethod()]
        public void Get3DCoordinateCoverageNone2D()
        {
            var container = builder.NewAtomContainer();

            var atom1 = builder.NewAtom("C");
            var atom2 = builder.NewAtom("C");
            var atom3 = builder.NewAtom("C");

            atom1.Point2D = new Vector2(1, 1);
            atom2.Point2D = new Vector2(2, 1);
            atom3.Point2D = new Vector2(1, 2);

            container.Atoms.Add(atom1);
            container.Atoms.Add(atom2);
            container.Atoms.Add(atom3);

            Assert.AreEqual(GeometryUtil.CoordinateCoverage.None, GeometryUtil.Get3DCoordinateCoverage(container));
        }

        [TestMethod()]
        public void TestTranslateAllPositiveIAtomContainerHashMap()
        {
            var atom1 = builder.NewAtom("C");
            atom1.Point2D = new Vector2(-1, -1);
            var atom2 = builder.NewAtom("C");
            atom2.Point2D = new Vector2(1, 0);
            var ac = builder.NewAtomContainer();
            ac.Atoms.Add(atom1);
            ac.Atoms.Add(atom2);
            GeometryUtil.TranslateAllPositive(ac);
            Assert.AreEqual(atom1.Point2D.Value.X, 0.0, 0.01);
            Assert.AreEqual(atom1.Point2D.Value.Y, 0.0, 0.01);
            Assert.AreEqual(atom2.Point2D.Value.X, 2.0, 0.01);
            Assert.AreEqual(atom2.Point2D.Value.Y, 1.0, 0.01);
        }

        [TestMethod()]
        public void TestGetLength2DIBondHashMap()
        {
            var atom1 = builder.NewAtom("C");
            atom1.Point2D = new Vector2(-1, -1);
            var atom2 = builder.NewAtom("C");
            atom2.Point2D = new Vector2(1, 0);
            var bond = builder.NewBond(atom1, atom2);
            var ac = builder.NewAtomContainer();
            ac.Atoms.Add(atom1);
            ac.Atoms.Add(atom2);
            Assert.AreEqual(GeometryUtil.GetLength2D(bond), 2.23, 0.01);
        }

        [TestMethod()]
        public void TestGetClosestAtomMultiatom()
        {
            var atom1 = builder.NewAtom("C");
            atom1.Point2D = new Vector2(-1, -1);
            var atom2 = builder.NewAtom("C");
            atom2.Point2D = new Vector2(1, 0);
            var atom3 = builder.NewAtom("C");
            atom3.Point2D = new Vector2(5, 0);
            var acont = builder.NewAtomContainer();
            acont.Atoms.Add(atom1);
            acont.Atoms.Add(atom2);
            acont.Atoms.Add(atom3);
            Assert.AreEqual(atom2, GeometryUtil.GetClosestAtom(acont, atom1));
            Assert.AreEqual(atom1, GeometryUtil.GetClosestAtom(acont, atom2));
            Assert.AreEqual(atom2, GeometryUtil.GetClosestAtom(acont, atom3));
        }

        [TestMethod()]
        public void TestGetClosestAtomDoubleDoubleIAtomContainerIAtom()
        {
            var atom1 = builder.NewAtom("C");
            atom1.Point2D = new Vector2(1, 0);
            var atom2 = builder.NewAtom("C");
            atom2.Point2D = new Vector2(5, 0);
            var acont = builder.NewAtomContainer();
            acont.Atoms.Add(atom1);
            acont.Atoms.Add(atom2);
            Assert.AreEqual(atom2, GeometryUtil.GetClosestAtom(1.0, 0.0, acont, atom1));
            Assert.AreEqual(atom1, GeometryUtil.GetClosestAtom(1.0, 0.0, acont, null));
        }

        /// <summary>
        /// Tests if not the central atom is returned as closest atom.
        /// </summary>
        [TestMethod()]
        public void TestGetClosestAtomIAtomContainerIAtom()
        {
            var atom1 = builder.NewAtom("C");
            atom1.Point2D = new Vector2(-1, -1);
            var atom2 = builder.NewAtom("C");
            atom2.Point2D = new Vector2(1, 0);
            var acont = builder.NewAtomContainer();
            acont.Atoms.Add(atom1);
            acont.Atoms.Add(atom2);
            Assert.AreEqual(atom2, GeometryUtil.GetClosestAtom(acont, atom1));
            Assert.AreEqual(atom1, GeometryUtil.GetClosestAtom(acont, atom2));
        }

        [TestMethod()]
        public void TestShiftContainerHorizontalIAtomContainerRectangle2DRectangle2DDouble()
        {
            var atom1 = builder.NewAtom("C");
            atom1.Point2D = new Vector2(0, 1);
            var atom2 = builder.NewAtom("C");
            atom2.Point2D = new Vector2(1, 0);
            var react1 = builder.NewAtomContainer();
            react1.Atoms.Add(atom1);
            react1.Atoms.Add(atom2);
            var react2 = (IAtomContainer)react1.Clone();

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
        [TestMethod()]
        public void TestShiftContainerHorizontalTwoverticalmolecules()
        {
            var atom1 = builder.NewAtom("C");
            atom1.Point2D = Vector2.Zero;
            var atom2 = builder.NewAtom("C");
            atom2.Point2D = new Vector2(0, 1);
            var react1 = builder.NewAtomContainer();
            react1.Atoms.Add(atom1);
            react1.Atoms.Add(atom2);
            var react2 = (IAtomContainer)react1.Clone();

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
        public void TestGetBondLengthAverageIReaction()
        {
            var atom1 = builder.NewAtom("C");
            atom1.Point2D = Vector2.Zero;
            var atom2 = builder.NewAtom("C");
            atom2.Point2D = new Vector2(1, 0);
            var acont = builder.NewAtomContainer();
            var reaction = builder.NewReaction();
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
        public void TestGetBondLengthAverageMultiReaction()
        {
            var reaction = builder.NewReaction();

            // mol 1
            var atom1 = builder.NewAtom("C");
            atom1.Point2D = Vector2.Zero;
            var atom2 = builder.NewAtom("C");
            atom2.Point2D = new Vector2(1, 0);
            var acont = builder.NewAtomContainer();
            reaction.Reactants.Add(acont);
            acont.Atoms.Add(atom1);
            acont.Atoms.Add(atom2);
            acont.AddBond(acont.Atoms[0], acont.Atoms[1], BondOrder.Single);

            // mol 2
            atom1 = builder.NewAtom("C");
            atom1.Point2D = Vector2.Zero;
            atom2 = builder.NewAtom("C");
            atom2.Point2D = new Vector2(3, 0);
            acont = builder.NewAtomContainer();
            reaction.Products.Add(acont);
            acont.Atoms.Add(atom1);
            acont.Atoms.Add(atom2);
            acont.AddBond(acont.Atoms[0], acont.Atoms[1], BondOrder.Single);

            Assert.AreEqual(2.0, GeometryUtil.GetBondLengthAverage(reaction), 0.0);
        }

        [TestMethod()]
        public void TestShiftReactionVerticalIAtomContainerRectangle2DRectangle2DDouble()
        {
            var atom1 = builder.NewAtom("C");
            atom1.Point2D = new Vector2(0, 1);
            var atom2 = builder.NewAtom("C");
            atom2.Point2D = new Vector2(1, 0);
            var react1 = builder.NewAtomContainer();
            var reaction = builder.NewReaction();
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
        public void TestShiftReactionVerticalTwohorizontalmolecules()
        {
            var atom1 = builder.NewAtom("C");
            atom1.Point2D = Vector2.Zero;
            var atom2 = builder.NewAtom("C");
            atom2.Point2D = new Vector2(1, 0);
            var react1 = builder.NewAtomContainer();
            var reaction = builder.NewReaction();
            reaction.Reactants.Add(react1);
            react1.Atoms.Add(atom1);
            react1.Atoms.Add(atom2);
            react1.AddBond(react1.Atoms[0], react1.Atoms[1], BondOrder.Single);
            var reaction2 = (IReaction)reaction.Clone();
            var react2 = reaction2.Reactants[0];

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
            var TYPE = "C";
            var zero = builder.NewAtom("O");
            zero.Point2D = new Vector2();
            var pX = builder.NewAtom(TYPE);
            pX.Point2D = new Vector2(1, 0);
            var nX = builder.NewAtom(TYPE);
            nX.Point2D = new Vector2(-1, 0);
            var pY = builder.NewAtom(TYPE);
            pY.Point2D = new Vector2(0, 1);
            var nY = builder.NewAtom(TYPE);
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
            var container = builder.NewAtomContainer();
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
            var container = builder.NewAtomContainer();
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
            var container = builder.NewAtomContainer();
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
            var container = builder.NewAtomContainer();
            container.Atoms.Add(AtomAt(Vector2.Zero));
            container.Atoms.Add(AtomAt(new Vector2(0, 1.5)));
            container.AddBond(container.Atoms[0], container.Atoms[1], BondOrder.Single);
            Assert.AreEqual(1.5, GeometryUtil.GetBondLengthMedian(container));
        }

        [TestMethod()]
        public void MedianBondLengthWithZeroLengthBonds()
        {
            var container = builder.NewAtomContainer();
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
            var atom = builder.NewAtom("C");
            atom.Point2D = p;
            return atom;
        }

        private int AlignmentTestHelper(IAtom zero, params IAtom[] pos)
        {
            var mol = builder.NewAtomContainer();
            mol.Atoms.Add(zero);
            foreach (var atom in pos)
            {
                mol.Atoms.Add(atom);
                mol.Bonds.Add(builder.NewBond(zero, atom));
            }
            return GeometryUtil.GetBestAlignmentForLabelXY(mol, zero);
        }
    }
}
