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
using System;
using NCDK.Numerics;

namespace NCDK.Geometries
{
    /**
     * This class defines regression tests that should ensure that the source code
     * of the org.openscience.cdk.geometry.CrystalGeometryTools is not broken.
     * All methods that start with test are regression tests, e.g.
     * <code>TestNotionalToCartesian()</code>.
     *
     * @cdk.module test-standard
     *
     * @author     Egon Willighagen
     * @cdk.created    2003-08-19
     *
     * @see org.openscience.cdk.geometry.CrystalGeometryTools
     */
    [TestClass()]
    public class CrystalGeometryToolsTest : CDKTestCase
    {
        public CrystalGeometryToolsTest()
            : base()
        {
        }

        /**
         * This method tests the conversion of notional coordinates to
         * cartesian coordinates. The test assumes that the
         * <code>CrystalGeometryTools.NotionalToCartesian()</code> methods
         * places the a axis on the x axis and the b axis in the xy plane.
         */
        [TestMethod()]
        public void TestNotionalToCartesian_Double_double_double_double_double_double()
        {
            Vector3[] cardAxes = CrystalGeometryTools.NotionalToCartesian(1.0, 2.0, 3.0, 90.0, 90.0, 90.0);
            // the a axis
            Assert.AreEqual(1.0, cardAxes[0].X, 0.001);
            Assert.AreEqual(0.0, cardAxes[0].Y, 0.001);
            Assert.AreEqual(0.0, cardAxes[0].Z, 0.001);
            // the b axis
            Assert.AreEqual(0.0, cardAxes[1].X, 0.001);
            Assert.AreEqual(2.0, cardAxes[1].Y, 0.001);
            Assert.AreEqual(0.0, cardAxes[1].Z, 0.001);
            // the c axis
            Assert.AreEqual(0.0, cardAxes[2].X, 0.001);
            Assert.AreEqual(0.0, cardAxes[2].Y, 0.001);
            Assert.AreEqual(3.0, cardAxes[2].Z, 0.001);

            // some sanity checking: roundtripping
            cardAxes = CrystalGeometryTools.NotionalToCartesian(9.3323, 10.1989, 11.2477, 69.043, 74.441, 77.821);
            Vector3 a = cardAxes[0];
            Vector3 b = cardAxes[1];
            Vector3 c = cardAxes[2];
            Assert.AreEqual(69.043, Vectors.RadianToDegree(Vectors.Angle(b, c)), 0.001);
            Assert.AreEqual(74.441, Vectors.RadianToDegree(Vectors.Angle(a, c)), 0.001);
            Assert.AreEqual(77.821, Vectors.RadianToDegree(Vectors.Angle(b, a)), 0.001);
            Assert.AreEqual(9.3323, a.Length(), 0.0001);
            Assert.AreEqual(10.1989, b.Length(), 0.0001);
            Assert.AreEqual(11.2477, c.Length(), 0.0001);
        }

        /**
         * This method tests the conversion of cartesian coordinates to
         * notional coordinates.
         */
        [TestMethod()]
        public void TestCartesianToNotional_Vector3d_Vector3d_Vector3d()
        {
            Vector3 a = new Vector3(1.0, 0.0, 0.0);
            Vector3 b = new Vector3(0.0, 2.0, 0.0);
            Vector3 c = new Vector3(0.0, 0.0, 3.0);
            double[] notionalCoords = CrystalGeometryTools.CartesianToNotional(a, b, c);
            Assert.AreEqual(1.0, notionalCoords[0], 0.001);
            Assert.AreEqual(2.0, notionalCoords[1], 0.001);
            Assert.AreEqual(3.0, notionalCoords[2], 0.001);
            Assert.AreEqual(90.0, notionalCoords[3], 0.001);
            Assert.AreEqual(90.0, notionalCoords[4], 0.001);
            Assert.AreEqual(90.0, notionalCoords[5], 0.001);
        }

        /**
         * This method tests the conversion of atomic fractional coordinates to
         * cartesian coordinates.
         */
        [TestMethod()]
        public void TestFractionalToCartesian_Vector3d_Vector3d_Vector3d_Point3d()
        {
            Vector3 a = new Vector3(1.0, 0.0, 0.0);
            Vector3 b = new Vector3(0.0, 2.0, 0.0);
            Vector3 c = new Vector3(0.0, 0.0, 3.0);
            Vector3 fractCoord = new Vector3(0.25, 0.50, 0.75);
            Vector3 cartCoord = CrystalGeometryTools.FractionalToCartesian(a, b, c, fractCoord);
            Assert.AreEqual(0.25, cartCoord.X, 0.001);
            Assert.AreEqual(1.0, cartCoord.Y, 0.001);
            Assert.AreEqual(2.25, cartCoord.Z, 0.001);
        }

        /**
         * This method tests the conversion of atomic fractional coordinates to
         * cartesian coordinates. The specific numbers are taken from 9603.res.
         */
        [TestMethod()]
        public void TestFractionalToCartesian2()
        {
            Vector3[] cardAxes = CrystalGeometryTools
                    .NotionalToCartesian(9.3323, 10.1989, 11.2477, 69.043, 74.441, 77.821);
            Vector3 a = cardAxes[0];
            Vector3 b = cardAxes[1];
            Vector3 c = cardAxes[2];

            Vector3 cartCoords = CrystalGeometryTools.FractionalToCartesian(a, b, c, new Vector3(0.517879, 0.258121, 0.698477));
            Assert.AreEqual(7.495, cartCoords.X, 0.001);
            Assert.AreEqual(4.993, cartCoords.Y, 0.001);
            Assert.AreEqual(7.171, cartCoords.Z, 0.001);
        }

        /**
         * This method tests the conversion of atomic cartesian coordinates to
         * fractional coordinates.
         */
        [TestMethod()]
        public void TestCartesianToFractional_Vector3d_Vector3d_Vector3d_Point3d()
        {
            Vector3 a = new Vector3(1.0, 0.0, 0.0);
            Vector3 b = new Vector3(0.0, 2.0, 0.0);
            Vector3 c = new Vector3(0.0, 0.0, 3.0);
            Vector3 cartCoord = new Vector3(0.25, 1.0, 2.25);
            Vector3 fractCoord = CrystalGeometryTools.CartesianToFractional(a, b, c, cartCoord);
            Assert.AreEqual(0.25, fractCoord.X, 0.001);
            Assert.AreEqual(0.50, fractCoord.Y, 0.001);
            Assert.AreEqual(0.75, fractCoord.Z, 0.001);
        }

        /**
         * This method tests the calculation of axis lengths.
         */
        [TestMethod()]
        public void TestCalcAxisLength()
        {
            Vector3 a = new Vector3(1.0, 1.0, 1.0);
            double length = a.Length();
            Assert.AreEqual(Math.Sqrt(3.0), length, 0.001);
        }

        /**
         * This method tests the calculation of axis lengths too, like
         * <code>TestCalcAxisLength()</code>.
         */
        [TestMethod()]
        public void TestCalcAxisLength2()
        {
            Vector3 a = new Vector3(1.0, 0.0, 0.0);
            double length = a.Length();
            Assert.AreEqual(1.0, length, 0.001);
            Vector3 b = new Vector3(0.0, 1.0, 0.0);
            length = b.Length();
            Assert.AreEqual(1.0, length, 0.001);
            Vector3 c = new Vector3(0.0, 0.0, 1.0);
            length = c.Length();
            Assert.AreEqual(1.0, length, 0.001);
        }

        /**
         * This method tests the calculation of the angle between two axes.
         */
        [TestMethod()]
        public void TestCalcAngle()
        {
            Vector3 b = new Vector3(0.0, 2.0, 0.0);
            Vector3 c = new Vector3(0.0, 0.0, 3.0);
            double angle = Vectors.Angle(b, c) * 180.0 / Math.PI;
            Assert.AreEqual(90.0, angle, 0.001);
        }

        /**
         * This method tests the calculation of the angle between two axes too.
         */
        [TestMethod()]
        public void TestCalcAngle2()
        {
            Vector3 b = new Vector3(0.0, 1.0, 1.0);
            Vector3 c = new Vector3(0.0, 0.0, 1.0);
            double angle = Vectors.Angle(b, c) * 180.0 / Math.PI;
            Assert.AreEqual(45.0, angle, 0.001);
        }

        /**
         * This method tests the calculation of the angle between one axis
         * and itself, which should be zero by definition.
         */
        [TestMethod()]
        public void TestCalcAngle3()
        {
            Vector3 b = new Vector3(4.5, 3.1, 1.7);
            double angle = Vectors.Angle(b, b) * 180.0 / Math.PI;
            Assert.AreEqual(0.0, angle, 0.001);
        }

        /**
         * This method tests the conversion of notional coordinates to
         * cartesian and back to notional.
         */
        [TestMethod()]
        public void TestRoundTripUnitCellNotionalCoordinates()
        {
            Vector3[] cardAxes = CrystalGeometryTools.NotionalToCartesian(7.6, 3.9, 10.3, 67.0, 91.2, 110.5);
            Vector3 a = cardAxes[0];
            Vector3 b = cardAxes[1];
            Vector3 c = cardAxes[2];
            double[] notionalCoords = CrystalGeometryTools.CartesianToNotional(a, b, c);
            Assert.AreEqual(7.6, notionalCoords[0], 0.001);
            Assert.AreEqual(3.9, notionalCoords[1], 0.001);
            Assert.AreEqual(10.3, notionalCoords[2], 0.001);
            Assert.AreEqual(67.0, notionalCoords[3], 0.001);
            Assert.AreEqual(91.2, notionalCoords[4], 0.001);
            Assert.AreEqual(110.5, notionalCoords[5], 0.001);
        }

        /**
         * This method tests whether two times inversion of the axes
         * gives back the original axes.
         */
        [TestMethod()]
        public void TestCalcInvertedAxes_Vector3d_Vector3d_Vector3d()
        {
            Vector3 a = new Vector3(3.4, 7.6, 5.5);
            Vector3 b = new Vector3(2.8, 4.0, 6.3);
            Vector3 c = new Vector3(1.9, 3.9, 9.1);
            Vector3[] invertedAxes = CrystalGeometryTools.CalcInvertedAxes(a, b, c);
            Vector3 a2 = invertedAxes[0];
            Vector3 b2 = invertedAxes[1];
            Vector3 c2 = invertedAxes[2];
            Vector3[] doubleAxes = CrystalGeometryTools.CalcInvertedAxes(a2, b2, c2);
            Vector3 a3 = doubleAxes[0];
            Vector3 b3 = doubleAxes[1];
            Vector3 c3 = doubleAxes[2];
            Assert.AreEqual(a.X, a3.X, 0.001);
            Assert.AreEqual(a.Y, a3.Y, 0.001);
            Assert.AreEqual(a.Z, a3.Z, 0.001);
            Assert.AreEqual(b.X, b3.X, 0.001);
            Assert.AreEqual(b.Y, b3.Y, 0.001);
            Assert.AreEqual(b.Z, b3.Z, 0.001);
            Assert.AreEqual(c.X, c3.X, 0.001);
            Assert.AreEqual(c.Y, c3.Y, 0.001);
            Assert.AreEqual(c.Z, c3.Z, 0.001);
        }
    }
}
