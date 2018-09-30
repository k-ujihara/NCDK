/* Copyright (C) 2004-2007  Miguel Rojas <miguel.rojas@uni-koeln.de>
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
using NCDK.Numerics;

namespace NCDK
{
    /// <summary>
    /// Checks the functionality of <see cref="IPDBAtom"/> implementations.
    /// </summary>
    // @cdk.module test-interfaces
    [TestClass()]
    public abstract class AbstractPDBAtomTest : AbstractAtomTest
    {
        /// <summary>
        /// Method to test the SetFractional3D() methods.
        /// </summary>
        [TestMethod()]
        public override void TestSetFractionalPoint3dPoint3d()
        {
            IPDBAtom a = (IPDBAtom)NewChemObject();
            a.Symbol = "C";
            a.FractionalPoint3D = new Vector3(0.5, 0.5, 0.5);
            Vector3 fract = a.FractionalPoint3D.Value;
            Assert.IsNotNull(fract);
            Assert.AreEqual(0.5, fract.X, 0.001);
            Assert.AreEqual(0.5, fract.Y, 0.001);
            Assert.AreEqual(0.5, fract.Z, 0.001);
        }

        [TestMethod()]
        public override void TestGetFractionalPoint3d()
        {
            TestSetFractionalPoint3dPoint3d();
        }

        [TestMethod()]
        public override void TestGetPoint3d()
        {
            Vector3 point3d = new Vector3(1, 2, 3);

            IPDBAtom a = (IPDBAtom)NewChemObject();
            a.Point3D = point3d;
            Assert.IsNotNull(a.Point3D);
            AssertAreEqual(point3d, a.Point3D.Value, 0.001);
        }

        [TestMethod()]
        public override void TestSetPoint3dPoint3d()
        {
            Vector3 point3d = new Vector3(1, 2, 3);

            IPDBAtom a = (IPDBAtom)NewChemObject();
            a.Symbol = "C";
            a.Point3D = point3d;
            Assert.AreEqual(point3d, a.Point3D);
        }

        /// <summary>
        /// Method to test the Clone() method
        /// </summary>
        [TestMethod()]
        public override void TestClone()
        {
            IPDBAtom atom = (IPDBAtom)NewChemObject();
            atom.Symbol = "C";
            object clone = atom.Clone();
            Assert.IsTrue(clone is IAtom);
        }

        /// <summary>
        /// Method to test the Clone() method
        /// </summary>
        [TestMethod()]
        public override void TestClonePoint3d()
        {
            IPDBAtom atom = (IPDBAtom)NewChemObject();
            atom.Symbol = "C";
            atom.Point3D = new Vector3(2, 3, 4);
            IAtom clone = (IAtom)atom.Clone();
            Assert.AreEqual(clone.Point3D.Value.X, 2.0, 0.001);
        }

        /// <summary>
        /// Method to test the Clone() method
        /// </summary>
        [TestMethod()]
        public override void TestCloneFractionalPoint3d()
        {
            IPDBAtom atom = (IPDBAtom)NewChemObject();
            atom.Symbol = "C";
            atom.FractionalPoint3D = new Vector3(2, 3, 4);
            IAtom clone = (IAtom)atom.Clone();
            Assert.AreEqual(clone.FractionalPoint3D.Value.X, 2.0, 0.001);
        }

        /// <summary>
        /// Method to test whether the class complies with RFC #9.
        /// </summary>
        [TestMethod()]
        public override void TestToString()
        {
            IPDBAtom atom = (IPDBAtom)NewChemObject();
            atom.Symbol = "C";
            string description = atom.ToString();
            for (int i = 0; i < description.Length; i++)
            {
                Assert.IsTrue('\n' != description[i]);
                Assert.IsTrue('\r' != description[i]);
            }
        }

        /// <summary>
        /// Checks that the default charge is set to NaN
        /// </summary>
        [TestMethod()]
        public override void TestDefaultChargeValue()
        {
            IPDBAtom atom = (IPDBAtom)NewChemObject();
            atom.Symbol = "C";
            Assert.AreEqual(0.00, atom.Charge.Value, 0.00000001);
        }

        [TestMethod()]
        public virtual void TestGetRecord()
        {
            IPDBAtom atom = (IPDBAtom)NewChemObject();
            atom.Symbol = "C";
            atom.Record = "ATOM 1635 N PHE 105 -3.504 9.019 -14.276 1.00 0.00 N";
            Assert.AreEqual("ATOM 1635 N PHE 105 -3.504 9.019 -14.276 1.00 0.00 N", atom.Record);
        }

        [TestMethod()]
        public virtual void TestSetRecord_String()
        {
            TestGetRecord();
        }

        [TestMethod()]
        public virtual void TestGetTempFactor()
        {
            IPDBAtom atom = (IPDBAtom)NewChemObject();
            atom.Symbol = "C";
            atom.TempFactor = 0.0;
            Assert.AreEqual(atom.TempFactor.Value, 0.0, 001);
        }

        [TestMethod()]
        public virtual void TestSetTempFactor_Double()
        {
            TestGetTempFactor();
        }

        [TestMethod()]
        public virtual void TestSetResName_String()
        {
            TestGetResName();
        }

        [TestMethod()]
        public virtual void TestGetResName()
        {
            IPDBAtom atom = (IPDBAtom)NewChemObject();
            atom.Symbol = "C";
            atom.ResName = "PHE";
            Assert.AreEqual("PHE", atom.ResName);
        }

        [TestMethod()]
        public virtual void TestSetICode_String()
        {
            TestGetICode();
        }

        [TestMethod()]
        public virtual void TestGetICode()
        {
            IPDBAtom atom = (IPDBAtom)NewChemObject();
            atom.Symbol = "C";
            atom.ICode = "123";
            Assert.AreEqual("123", atom.ICode);
        }

        [TestMethod()]
        public virtual void TestSetChainID_String()
        {
            TestGetChainID();
        }

        [TestMethod()]
        public virtual void TestGetChainID()
        {
            IPDBAtom atom = (IPDBAtom)NewChemObject();
            atom.Symbol = "C";
            atom.ChainID = "123";
            Assert.AreEqual("123", atom.ChainID);
        }

        [TestMethod()]
        public virtual void TestSetAltLoc_String()
        {
            TestGetAltLoc();
        }

        [TestMethod()]
        public virtual void TestGetAltLoc()
        {
            IPDBAtom atom = (IPDBAtom)NewChemObject();
            atom.Symbol = "C";
            atom.AltLoc = "123";
            Assert.AreEqual("123", atom.AltLoc);
        }

        [TestMethod()]
        public virtual void TestSetSegID_String()
        {
            TestGetSegID();
        }

        [TestMethod()]
        public virtual void TestGetSegID()
        {
            IPDBAtom atom = (IPDBAtom)NewChemObject();
            atom.Symbol = "C";
            atom.SegID = "123";
            Assert.AreEqual("123", atom.SegID);
        }

        [TestMethod()]
        public virtual void TestSetSerial_Integer()
        {
            TestGetSerial();
        }

        [TestMethod()]
        public virtual void TestGetSerial()
        {
            IPDBAtom atom = (IPDBAtom)NewChemObject();
            atom.Symbol = "C";
            atom.Serial = 123;
            Assert.AreEqual(123, atom.Serial.Value);
        }

        [TestMethod()]
        public virtual void TestSetResSeq_String()
        {
            TestGetResSeq();
        }

        [TestMethod()]
        public virtual void TestGetResSeq()
        {
            IPDBAtom atom = (IPDBAtom)NewChemObject();
            atom.Symbol = "C";
            atom.ResSeq = "123";
            Assert.AreEqual("123", atom.ResSeq);
        }

        [TestMethod()]
        public virtual void TestSetOxt_Boolean()
        {
            TestGetOxt();
        }

        [TestMethod()]
        public virtual void TestGetOxt()
        {
            IPDBAtom atom = (IPDBAtom)NewChemObject();
            atom.Symbol = "C";
            atom.Oxt = true;
            Assert.IsTrue(atom.Oxt);
        }

        [TestMethod()]
        public virtual void TestSetHetAtom_Boolean()
        {
            TestGetHetAtom();
        }

        [TestMethod()]
        public virtual void TestGetHetAtom()
        {
            IPDBAtom atom = (IPDBAtom)NewChemObject();
            atom.Symbol = "C";
            atom.HetAtom = true;
            Assert.IsTrue(atom.HetAtom.Value);
        }

        [TestMethod()]
        public virtual void TestSetOccupancy_Double()
        {
            TestGetOccupancy();
        }

        [TestMethod()]
        public virtual void TestGetOccupancy()
        {
            IPDBAtom atom = (IPDBAtom)NewChemObject();
            atom.Symbol = "C";
            atom.Occupancy = 1.0;
            Assert.AreEqual(atom.Occupancy.Value, 1.0, 0.01);
        }

        [TestMethod()]
        public virtual void TestGetName()
        {
            IPDBAtom atom = (IPDBAtom)NewChemObject();
            atom.Symbol = "C";
            atom.Name = "123";
            Assert.AreEqual("123", atom.Name);
        }

        [TestMethod()]
        public virtual void TestSetName_String()
        {
            TestGetName();
        }
    }
}
