/* Copyright (C) 2010 Gilleain Torrance <gilleain.torrance@gmail.com>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
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
using NCDK.Common.Mathematics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.Numerics;
using static NCDK.Stereo.StereoTool;

namespace NCDK.Stereo
{
    /// <summary>
    // @author maclean
    // @cdk.module test-standard
    /// </summary>
    [TestClass()]
    public class StereoToolTest : CDKTestCase
    {

        private static readonly Vector3 ORIGIN = Vector3.Zero;
        private static readonly Vector3 XAXIS = new Vector3(1, 0, 0);
        private static readonly Vector3 YAXIS = new Vector3(0, 1, 0);
        private static readonly Vector3 ZAXIS = new Vector3(0, 0, 1);

        [TestMethod()]
        public void PositivePointPlaneDistanceTest()
        {
            // the normal for the Y-Z plane is X
            Vector3 planeNormal = XAXIS;
            Vector3.Normalize(planeNormal);

            // an arbitrary point in the Y-Z plane
            Vector3 pointInPlane = new Vector3(0, 1, 1);

            // make a positive point on the X axis = same direction as the normal
            Vector3 pointToMeasurePos = new Vector3(2, 0, 0);

            double distancePos = StereoTool.SignedDistanceToPlane(planeNormal, pointInPlane, pointToMeasurePos);
            Assert.AreEqual(2.0, distancePos, 0.1);
        }

        [TestMethod()]
        public void NegativePointPlaneDistanceTest()
        {
            // the normal for the Y-Z plane is X
            Vector3 planeNormal = XAXIS;
            Vector3.Normalize(planeNormal);

            // an arbitrary point in the Y-Z plane
            Vector3 pointInPlane = new Vector3(0, 1, 1);

            // make a negative point on the X axis = opposite direction to normal
            Vector3 pointToMeasureNeg = new Vector3(-2, 0, 0);

            double distance = StereoTool.SignedDistanceToPlane(planeNormal, pointInPlane, pointToMeasureNeg);
            Assert.AreEqual(-2.0, distance, 0.1);
        }

        [TestMethod()]
        public void GetNormalFromThreePoints()
        {
            // these are, of course, points on these axes, not the axis vectors
            Vector3 axisXPoint = XAXIS;
            Vector3 axisYPoint = YAXIS;

            // the normal of X and Y should be Z
            Vector3 normal = StereoTool.GetNormal(ORIGIN, axisXPoint, axisYPoint);
            Assert.IsTrue(Vector3.Distance(ZAXIS, normal) < 0.0001);
        }

        [TestMethod()]
        public void TetrahedralPlusAtomsAboveXYClockwiseTest()
        {
            // above the XY plane
            IAtom baseA = new Atom("C", new Vector3(0, 0, 1));
            IAtom baseB = new Atom("C", new Vector3(1, 0, 1));
            IAtom baseC = new Atom("C", new Vector3(1, 1, 1));

            IAtom positiveApex = new Atom("C", new Vector3(0.5, 0.5, 2));
            TetrahedralSign tetSign = StereoTool.GetHandedness(baseC, baseB, baseA, positiveApex);
            Assert.AreEqual(TetrahedralSign.MINUS, tetSign);
        }

        [TestMethod()]
        public void TetrahedralPlusAtomsAboveXYTest()
        {
            // above the XY plane
            IAtom baseA = new Atom("C", new Vector3(0, 0, 1));
            IAtom baseB = new Atom("C", new Vector3(1, 0, 1));
            IAtom baseC = new Atom("C", new Vector3(1, 1, 1));

            IAtom positiveApex = new Atom("C", new Vector3(0.5, 0.5, 2));
            TetrahedralSign tetSign = StereoTool.GetHandedness(baseA, baseB, baseC, positiveApex);
            Assert.AreEqual(TetrahedralSign.PLUS, tetSign);
        }

        [TestMethod()]
        public void TetrahedralMinusAtomsAboveXYTest()
        {
            // above the XY plane
            IAtom baseA = new Atom("C", new Vector3(0, 0, 1));
            IAtom baseB = new Atom("C", new Vector3(1, 0, 1));
            IAtom baseC = new Atom("C", new Vector3(1, 1, 1));

            IAtom negativeApex = new Atom("C", new Vector3(0.5, 0.5, -1));
            TetrahedralSign tetSign = StereoTool.GetHandedness(baseA, baseB, baseC, negativeApex);
            Assert.AreEqual(TetrahedralSign.MINUS, tetSign);
        }

        [TestMethod()]
        public void TetrahedralPlusAtomsBelowXYTest()
        {
            // below the XY plane
            IAtom baseA = new Atom("C", new Vector3(0, 0, -1));
            IAtom baseB = new Atom("C", new Vector3(1, 0, -1));
            IAtom baseC = new Atom("C", new Vector3(1, 1, -1));

            IAtom positiveApex = new Atom("C", new Vector3(0.5, 0.5, 0));
            TetrahedralSign tetSign = StereoTool.GetHandedness(baseA, baseB, baseC, positiveApex);
            Assert.AreEqual(TetrahedralSign.PLUS, tetSign);
        }

        [TestMethod()]
        public void TetrahedralMinusAtomsBelowXYTest()
        {
            // below the XY plane
            IAtom baseA = new Atom("C", new Vector3(0, 0, -1));
            IAtom baseB = new Atom("C", new Vector3(1, 0, -1));
            IAtom baseC = new Atom("C", new Vector3(1, 1, -1));

            IAtom negativeApex = new Atom("C", new Vector3(0.5, 0.5, -2));
            TetrahedralSign tetSign = StereoTool.GetHandedness(baseA, baseB, baseC, negativeApex);
            Assert.AreEqual(TetrahedralSign.MINUS, tetSign);
        }

        [TestMethod()]
        public void ColinearTestWithColinearPoints()
        {
            Vector3 pointA = new Vector3(1, 1, 1);
            Vector3 pointB = new Vector3(2, 2, 2);
            Vector3 pointC = new Vector3(3, 3, 3);

            Assert.IsTrue(StereoTool.IsColinear(pointA, pointB, pointC));
        }

        [TestMethod()]
        public void ColinearTestWithNearlyColinearPoints()
        {
            Vector3 pointA = new Vector3(1, 1, 1);
            Vector3 pointB = new Vector3(2, (float)2.001, 2);
            Vector3 pointC = new Vector3(3, 3, 3);

            Assert.IsTrue(StereoTool.IsColinear(pointA, pointB, pointC));
        }

        [TestMethod()]
        public void ColinearTestWithNonColinearPoints()
        {
            Vector3 pointA = new Vector3(1, 1, 1);
            Vector3 pointB = new Vector3(2, 3, 2);
            Vector3 pointC = new Vector3(3, 3, 3);

            Assert.IsFalse(StereoTool.IsColinear(pointA, pointB, pointC));
        }

        [TestMethod()]
        public void SquarePlanarUShapeTest()
        {
            // all points are in the XY plane
            IAtom atomA = new Atom("C", new Vector3(1, 2, 0));
            IAtom atomB = new Atom("C", new Vector3(1, 1, 0));
            IAtom atomC = new Atom("C", new Vector3(2, 1, 0));
            IAtom atomD = new Atom("C", new Vector3(2, 2, 0));

            SquarePlanarShape shape = StereoTool.GetSquarePlanarShape(atomA, atomB, atomC, atomD);
            Assert.AreEqual(SquarePlanarShape.U_SHAPE, shape);
        }

        [TestMethod()]
        public void SquarePlanar4ShapeTest()
        {
            // all points are in the XY plane
            IAtom atomA = new Atom("C", new Vector3(1, 2, 0));
            IAtom atomB = new Atom("C", new Vector3(2, 1, 0));
            IAtom atomC = new Atom("C", new Vector3(2, 2, 0));
            IAtom atomD = new Atom("C", new Vector3(1, 1, 0));

            SquarePlanarShape shape = StereoTool.GetSquarePlanarShape(atomA, atomB, atomC, atomD);
            Assert.AreEqual(SquarePlanarShape.FOUR_SHAPE, shape);
        }

        [TestMethod()]
        public void SquarePlanarZShapeTest()
        {
            // all points are in the XY plane
            IAtom atomA = new Atom("C", new Vector3(1, 2, 0));
            IAtom atomB = new Atom("C", new Vector3(1, 1, 0));
            IAtom atomC = new Atom("C", new Vector3(2, 2, 0));
            IAtom atomD = new Atom("C", new Vector3(2, 1, 0));

            SquarePlanarShape shape = StereoTool.GetSquarePlanarShape(atomA, atomB, atomC, atomD);
            Assert.AreEqual(SquarePlanarShape.Z_SHAPE, shape);
        }

        [TestMethod()]
        public void TrigonalBipyramidalTest()
        {
            IAtom atomA = new Atom("C", new Vector3(1, 1, 2)); // axis point 1
            IAtom atomB = new Atom("C", new Vector3(1, 1, 1)); // center of plane
            IAtom atomC = new Atom("C", new Vector3(0, 1, 1));
            IAtom atomD = new Atom("C", new Vector3(1, 0, 1));
            IAtom atomE = new Atom("C", new Vector3(2, 2, 1));
            IAtom atomF = new Atom("C", new Vector3(1, 1, 0)); // axis point 2
            Assert.IsTrue(StereoTool.IsTrigonalBipyramidal(atomA, atomB, atomC, atomD, atomE, atomF));
        }

        [TestMethod()]
        public void OctahedralTest()
        {
            IAtom atomA = new Atom("C", new Vector3(2, 2, 2)); // axis point 1
            IAtom atomB = new Atom("C", new Vector3(2, 2, 1)); // center of plane
            IAtom atomC = new Atom("C", new Vector3(1, 3, 1));
            IAtom atomD = new Atom("C", new Vector3(3, 3, 1));
            IAtom atomE = new Atom("C", new Vector3(3, 1, 1));
            IAtom atomF = new Atom("C", new Vector3(1, 3, 1));
            IAtom atomG = new Atom("C", new Vector3(2, 2, 0)); // axis point 2

            Assert.IsTrue(StereoTool.IsOctahedral(atomA, atomB, atomC, atomD, atomE, atomF, atomG));
        }

        [TestMethod()]
        public void SquarePlanarTest()
        {
            IAtom atomA = new Atom("C", new Vector3(1, 2, 0));
            IAtom atomB = new Atom("C", new Vector3(1, 1, 0));
            IAtom atomC = new Atom("C", new Vector3(2, 2, 0));
            IAtom atomD = new Atom("C", new Vector3(2, 1, 0));
            Assert.IsTrue(StereoTool.IsSquarePlanar(atomA, atomB, atomC, atomD));
        }

        [TestMethod()]
        public void AllCoplanarTest()
        {
            Vector3 pointA = new Vector3(1, 1, 0);
            Vector3 pointB = new Vector3(2, 1, 0);
            Vector3 pointC = new Vector3(1, 2, 0);
            Vector3 pointD = new Vector3(2, 2, 0);
            Vector3 pointE = new Vector3(3, 2, 0);
            Vector3 pointF = new Vector3(3, 3, 0);

            Vector3 normal = StereoTool.GetNormal(pointA, pointB, pointC);
            Assert.IsTrue(StereoTool.AllCoplanar(normal, pointA, pointB, pointC, pointD, pointE, pointF));
        }

        [TestMethod()]
        public void GetStereoACWTest()
        {
            IAtom closestAtomToViewer = new Atom("F", new Vector3(1, 1, 1));
            IAtom highestCIPPriority = new Atom("I", new Vector3(0, 1, 2));
            IAtom middleCIPPriority = new Atom("Br", Vector3.Zero);
            IAtom nearlylowestCIPPriority = new Atom("Cl", new Vector3(0, 2, 0));
            Assert.AreEqual(TetrahedralStereo.AntiClockwise, StereoTool.GetStereo(closestAtomToViewer, highestCIPPriority,
                    middleCIPPriority, nearlylowestCIPPriority));
        }

        [TestMethod()]
        public void GetStereoCWTest()
        {
            IAtom closestAtomToViewer = new Atom("F", new Vector3(1, 1, 1));
            IAtom highestCIPPriority = new Atom("I", new Vector3(0, 1, 2));
            IAtom middleCIPPriority = new Atom("Br", new Vector3(0, 2, 0));
            IAtom nearlylowestCIPPriority = new Atom("Cl", Vector3.Zero);
            Assert.AreEqual(TetrahedralStereo.Clockwise, StereoTool.GetStereo(closestAtomToViewer, highestCIPPriority,
                    middleCIPPriority, nearlylowestCIPPriority));
        }
    }
}
