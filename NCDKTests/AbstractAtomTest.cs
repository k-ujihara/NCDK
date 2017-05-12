/* Copyright (C) 2002-2007  The Chemistry Development Kit (CDK) project
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
using NCDK.Tools.Diff;
using NCDK.Numerics;

namespace NCDK
{
    /// <summary>
    /// Checks the functionality of <see cref="IAtom"/> implementations.
    /// </summary>
    // @cdk.module test-interfaces
    [TestClass()]
    public abstract class AbstractAtomTest : AbstractAtomTypeTest
    {
        /// <summary>
        /// Method to test the get/SetCharge() methods.
        /// </summary>
        [TestMethod()]
        public virtual void TestSetCharge_Double()
        {
            double charge = 0.15;

            IAtom a = (IAtom)NewChemObject();
            a.Charge = charge;
            Assert.AreEqual(charge, a.Charge.Value, 0.001);
        }

        [TestMethod()]
        public virtual void TestGetCharge()
        {
            TestSetCharge_Double();
        }

        /// <summary>
        /// Method to test the get/SetHydrogenCount() methods.
        /// </summary>
        [TestMethod()]
        public virtual void TestSetImplicitHydrogenCount_Integer()
        {
            int count = 1;

            IAtom a = (IAtom)NewChemObject();
            a.ImplicitHydrogenCount = count;
            Assert.AreEqual(count, a.ImplicitHydrogenCount);
        }

        [TestMethod()]
        public virtual void TestGetImplicitHydrogenCount()
        {
            // should be null by default
            IAtom a = (IAtom)NewChemObject();
            Assert.IsNull(a.ImplicitHydrogenCount);
        }

        /// <summary>
        /// Method to test the SetFractional3D() methods.
        /// </summary>
        [TestMethod()]
        public virtual void TestSetFractionalPoint3d_Point3d()
        {
            IAtom a = (IAtom)NewChemObject();
            a.FractionalPoint3D = new Vector3(0.5, 0.5, 0.5);
            Vector3? fract = a.FractionalPoint3D;
            Assert.IsNotNull(fract);
            Assert.AreEqual(0.5, fract.Value.X, 0.001);
            Assert.AreEqual(0.5, fract.Value.Y, 0.001);
            Assert.AreEqual(0.5, fract.Value.Z, 0.001);
        }

        [TestMethod()]
        public virtual void TestGetFractionalPoint3d()
        {
            TestSetFractionalPoint3d_Point3d();
        }

        [TestMethod()]
        public virtual void TestGetPoint3d()
        {
            Vector3 point3d = new Vector3(1, 2, 3);

            IAtom a = (IAtom)NewChemObject();
            a.Point3D = point3d;
            Assert.IsNotNull(a.Point3D); 
            AssertAreEqual(point3d, a.Point3D.Value, 0.001);
        }

        [TestMethod()]
        public virtual void TestSetPoint3d_Point3d()
        {
            Vector3 point3d = new Vector3(1, 2, 3);

            IAtom a = (IAtom)NewChemObject();
            a.Point3D = point3d;
            Assert.AreEqual(point3d, a.Point3D);
        }

        [TestMethod()]
        public virtual void TestGetPoint2d()
        {
            Vector2 point2d = new Vector2(1, 2);

            IAtom a = (IAtom)NewChemObject();
            a.Point2D = point2d;
            Assert.IsNotNull(a.Point2D);
            Assert.AreEqual(point2d.X, a.Point2D.Value.X, 0.001);
            Assert.AreEqual(point2d.Y, a.Point2D.Value.Y, 0.001);
        }

        [TestMethod()]
        public virtual void TestSetPoint2d_Point2d()
        {
            Vector2 point2d = new Vector2(1, 2);

            IAtom a = (IAtom)NewChemObject();
            a.Point2D = point2d;
            Assert.AreEqual(point2d, a.Point2D);
        }

        /// <summary>
        /// Method to test the get/SetHydrogenCount() methods.
        /// </summary>
        [TestMethod()]
        public virtual void TestSetStereoParity_Integer()
        {
            int parity = CDKConstants.STEREO_ATOM_PARITY_PLUS;

            IAtom a = (IAtom)NewChemObject();
            a.StereoParity = parity;
            Assert.AreEqual(parity, a.StereoParity.Value);
        }

        [TestMethod()]
        public virtual void TestGetStereoParity()
        {
            TestSetStereoParity_Integer();
        }

        /// <summary>
        /// Method to test the Clone() method
        /// </summary>
        [TestMethod()]
        public override void TestClone()
        {
            IAtom atom = (IAtom)NewChemObject();
            object clone = atom.Clone();
            Assert.IsTrue(clone is IAtom);

            // test that everything has been cloned properly
            string diff = AtomDiff.Diff(atom, (IAtom)clone);
            Assert.IsNotNull(diff);
            Assert.AreEqual(0, diff.Length);
        }

        /// <summary>
        /// Method to test the Clone() method
        /// </summary>
        [TestMethod()]
        public virtual void TestClone_Point2d()
        {
            IAtom atom = (IAtom)NewChemObject();
            atom.Point2D = new Vector2(2, 3);
            IAtom clone = (IAtom)atom.Clone();
            Assert.AreEqual(clone.Point2D.Value.X, 2.0, 0.001);
        }

        /// <summary>
        /// Method to test the Clone() method
        /// </summary>
        [TestMethod()]
        public virtual void TestClone_Point3d()
        {
            IAtom atom = (IAtom)NewChemObject();
            atom.Point3D = new Vector3(2, 3, 4);
            IAtom clone = (IAtom)atom.Clone();
            Assert.AreEqual(clone.Point3D.Value.X, 2.0, 0.001);
        }

        /// <summary>
        /// Method to test the Clone() method
        /// </summary>
        [TestMethod()]
        public virtual void TestClone_FractionalPoint3d()
        {
            IAtom atom = (IAtom)NewChemObject();
            atom.FractionalPoint3D = new Vector3(2, 3, 4);
            IAtom clone = (IAtom)atom.Clone();
            Assert.AreEqual(clone.FractionalPoint3D.Value.X, 2.0, 0.001);
        }

        /// <summary>
        /// Method to test the Clone() method
        /// </summary>
        [TestMethod()]
        public virtual void TestClone_HydrogenCount()
        {
            IAtom atom = (IAtom)NewChemObject();
            atom.ImplicitHydrogenCount = 3;
            IAtom clone = (IAtom)atom.Clone();

            // test cloning
            atom.ImplicitHydrogenCount = 4;
            Assert.AreEqual(3, clone.ImplicitHydrogenCount.Value);
        }

        /// <summary>
        /// Method to test the Clone() method
        /// </summary>
        [TestMethod()]
        public virtual void TestClone_StereoParity()
        {
            IAtom atom = (IAtom)NewChemObject();
            atom.StereoParity = 3;
            IAtom clone = (IAtom)atom.Clone();

            // test cloning
            atom.StereoParity = 4;
            Assert.AreEqual(3, clone.StereoParity.Value);
        }

        /// <summary>
        /// Method to test the Clone() method
        /// </summary>
        [TestMethod()]
        public virtual void TestClone_Charge()
        {
            IAtom atom = (IAtom)NewChemObject();
            atom.Charge = 1.0;
            IAtom clone = (IAtom)atom.Clone();

            // test cloning
            atom.Charge = 5.0;
            Assert.AreEqual(1.0, clone.Charge.Value, 0.001);
        }

        /// <summary>
        /// Method to test whether the class complies with RFC #9.
        /// </summary>
        [TestMethod()]
        public override void TestToString()
        {
            IAtom atom = (IAtom)NewChemObject();
            string description = atom.ToString();
            for (int i = 0; i < description.Length; i++)
            {
                Assert.IsTrue('\n' != description[i]);
                Assert.IsTrue('\r' != description[i]);
            }
        }

        [TestMethod()]
        public virtual void TestToString_FractionalCoordinates()
        {
            IAtom atom = (IAtom)NewChemObject();
            atom.FractionalPoint3D = new Vector3(2, 3, 4);
            string description = atom.ToString();
            Assert.IsTrue(description.Contains("F3D"));
        }

        /// <summary>
        /// Checks that the default charge is set to NaN
        /// </summary>
        [TestMethod()]
        public virtual void TestDefaultChargeValue()
        {
            IAtom atom = (IAtom)NewChemObject();
            Assert.AreEqual(null, atom.Charge);
            //        Assert.AreEqual(0.0, atom.Charge, 0.00000001);
        }
    }
}