/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
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
using NCDK;
using NCDK.Tools.Diff;
using System;

namespace NCDK
{
    /**
     * Checks the functionality of {@link IAtomType} implementations.
     *
     * @cdk.module test-interfaces
     */
    [TestClass()]
    public abstract class AbstractAtomTypeTest : AbstractIsotopeTest
    {
        [TestMethod()]
        public virtual void TestSetAtomTypeName_String()
        {
            IAtomType at = (IAtomType)NewChemObject();
            at.AtomTypeName = "C4";
            Assert.AreEqual("C4", at.AtomTypeName);
        }

        [TestMethod()]
        public virtual void TestGetAtomTypeName()
        {
            IAtomType at = (IAtomType)NewChemObject();
            at.AtomTypeName = "C4";
            Assert.AreEqual("C4", at.AtomTypeName);
        }

        [TestMethod()]
        public virtual void TestSetMaxBondOrder_BondOrder()
        {
            IAtomType at = (IAtomType)NewChemObject();
            at.MaxBondOrder = BondOrder.Triple;
            Assert.AreEqual(BondOrder.Triple, at.MaxBondOrder);
        }

        [TestMethod()]
        public virtual void TestGetMaxBondOrder()
        {
            TestSetMaxBondOrder_BondOrder();
        }

        [TestMethod()]
        public virtual void TestSetBondOrderSum_Double()
        {
            IAtomType at = (IAtomType)NewChemObject();
            at.BondOrderSum = 4.0;
            Assert.AreEqual(4.0, at.BondOrderSum.Value, 0.001);
        }

        [TestMethod()]
        public virtual void TestGetBondOrderSum()
        {
            TestSetBondOrderSum_Double();
        }

        [TestMethod()]
        public virtual void TestSetCovalentRadius_Double()
        {
            IAtomType at = (IAtomType)NewChemObject();
            at.CovalentRadius = 1.0;
            Assert.AreEqual(1.0, at.CovalentRadius.Value, 0.001);
        }

        [TestMethod()]
        public virtual void TestGetCovalentRadius()
        {
            TestSetCovalentRadius_Double();
        }

        [TestMethod()]
        public virtual void TestSetFormalCharge_Integer()
        {
            int charge = 1;

            IAtomType a = (IAtomType)NewChemObject();
            a.FormalCharge = charge;
            Assert.AreEqual(charge, a.FormalCharge.Value);
        }

        [TestMethod()]
        public virtual void TestGetFormalCharge()
        {
            TestSetFormalCharge_Integer();
        }

        /**
         * Method to test the get/SetValency() methods.
         */
        [TestMethod()]
        public virtual void TestSetValency_Integer()
        {
            int valency = 4;

            IAtomType a = (IAtomType)NewChemObject();
            a.Valency = valency;
            Assert.AreEqual(valency, (int)a.Valency);
        }

        [TestMethod()]
        public virtual void TestGetValency()
        {
            TestSetValency_Integer();
        }

        [TestMethod()]
        public virtual void TestSetFormalNeighbourCount_Integer()
        {
            int count = 4;

            IAtomType a = (IAtomType)NewChemObject();
            a.FormalNeighbourCount = count;
            Assert.AreEqual(count, (int)a.FormalNeighbourCount);
        }

        [TestMethod()]
        public virtual void TestGetFormalNeighbourCount()
        {
            TestSetFormalNeighbourCount_Integer();
        }

        [TestMethod()]
        public virtual void TestSetHybridization_IAtomType_Hybridization()
        {
            Hybridization hybridization = Hybridization.SP1;

            IAtomType atom = (IAtomType)NewChemObject();
            atom.Hybridization = hybridization;
            Assert.AreEqual(hybridization, atom.Hybridization);
        }

        [TestMethod()]
        public virtual void TestGetHybridization()
        {
            TestSetHybridization_IAtomType_Hybridization();
        }

        [TestMethod()]
        public virtual void TestSetHybridization_Null()
        {
            Hybridization hybridization = Hybridization.SP1;

            IAtomType atom = (IAtomType)NewChemObject();
            atom.Hybridization = hybridization;
            Assert.AreEqual(hybridization, atom.Hybridization);
            atom.Hybridization = Hybridization.Unset;
            Assert.IsTrue(atom.Hybridization.IsUnset);
        }

        [TestMethod()]
        public virtual void TestSetAcceptor_bool()
        {
            bool acceptor = true;
            IAtomType a = (IAtomType)NewChemObject();
            a.IsHydrogenBondAcceptor = acceptor;
            Assert.IsTrue(a.IsHydrogenBondAcceptor);
        }

        [TestMethod()]
        public virtual void TestGetAcceptor()
        {
            TestSetAcceptor_bool();
        }

        [TestMethod()]
        public virtual void TestSetDonor_bool()
        {
            bool donor = true;
            IAtomType a = (IAtomType)NewChemObject();
            a.IsHydrogenBondDonor = donor;

            Assert.IsTrue(a.IsHydrogenBondDonor);
        }

        [TestMethod()]
        public virtual void TestGetDonor()
        {
            TestSetDonor_bool();
        }

        [TestMethod()]
        public virtual void TestSetChemicalGroupConstant_int()
        {
            int benzol = 6;
            IAtomType a = (IAtomType)NewChemObject();
            a.SetProperty(CDKPropertyName.CHEMICAL_GROUP_CONSTANT, benzol);
            Assert.AreEqual(benzol, a.GetProperty<int>(CDKPropertyName.CHEMICAL_GROUP_CONSTANT));
        }

        [TestMethod()]
        public virtual void TestGetChemicalGroupConstant()
        {
            TestSetChemicalGroupConstant_int();
        }

        [TestMethod()]
        public virtual void TestSetRingSize_int()
        {
            int five = 5;
            IAtomType a = (IAtomType)NewChemObject();
            a.SetProperty(CDKPropertyName.PART_OF_RING_OF_SIZE, five);
            Assert.AreEqual(five, a.GetProperty<int>(CDKPropertyName.PART_OF_RING_OF_SIZE));
        }

        [TestMethod()]
        public virtual void TestGetRingSize()
        {
            TestSetRingSize_int();
        }

        [TestMethod()]
        public virtual void TestSetIsAromatic_bool()
        {
            IAtomType a = (IAtomType)NewChemObject();
            a.IsAromatic = true;
            Assert.IsTrue(a.IsAromatic);
        }

        [TestMethod()]
        public virtual void TestGetIsAromatic()
        {
            TestSetIsAromatic_bool();
        }

        [TestMethod()]
        public virtual void TestSetSphericalMatcher_String()
        {
            string hoseCode = "C-4;HHHC(;///***)";
            IAtomType a = (IAtomType)NewChemObject();
            a.SetProperty(CDKPropertyName.CHEMICAL_GROUP_CONSTANT, hoseCode);
            Assert.AreEqual(hoseCode, a.GetProperty<string>(CDKPropertyName.CHEMICAL_GROUP_CONSTANT));
        }

        [TestMethod()]
        public virtual void TestGetSphericalMatcher()
        {
            TestSetSphericalMatcher_String();
        }

        /**
         * Test for bug #1309731.
         */
        [TestMethod()]
        public virtual void TestAtomTypeNameAndIDBug()
        {
            IAtomType a = (IAtomType)NewChemObject();
            a.Id = "carbon1";
            a.AtomTypeName = "C.sp3";
            Assert.AreEqual("carbon1", a.Id);
        }

        /**
         * Method to test the Clone() method
         */
        [TestMethod()]

        public override void TestClone()
        {
            IAtomType at = (IAtomType)NewChemObject();
            object clone = at.Clone();
            Assert.IsTrue(clone is IAtomType);

            // test that everything has been cloned properly
            string diff = AtomTypeDiff.Diff(at, (IAtomType)clone);
            Assert.IsNotNull(diff);
            Assert.AreEqual(0, diff.Length);
        }

        /**
         * Method to test the Clone() method
         */
        [TestMethod()]
        public virtual void TestClone_MaxBondOrder()
        {
            IAtomType at = (IAtomType)NewChemObject();
            at.MaxBondOrder = BondOrder.Single;
            IAtomType clone = (IAtomType)at.Clone();

            at.MaxBondOrder = BondOrder.Double;
            Assert.AreEqual(BondOrder.Single, clone.MaxBondOrder);
        }

        /**
         * Method to test the Clone() method
         */
        [TestMethod()]
        public virtual void TestClone_IBondOrderSum()
        {
            IAtomType at = (IAtomType)NewChemObject();
            at.BondOrderSum = 1.0;
            IAtomType clone = (IAtomType)at.Clone();

            at.BondOrderSum = 2.0;
            Assert.AreEqual(1.0, clone.BondOrderSum.Value, 0.001);
        }

        /**
         * Method to test the Clone() method
         */
        [TestMethod()]
        public virtual void TestClone_CovalentRadius()
        {
            IAtomType at = (IAtomType)NewChemObject();
            at.CovalentRadius = 1.0;
            IAtomType clone = (IAtomType)at.Clone();

            at.CovalentRadius = 2.0;
            Assert.AreEqual(1.0, clone.CovalentRadius.Value, 0.001);
        }

        /**
         * Method to test the Clone() method
         */
        [TestMethod()]
        public virtual void TestClone_FormalCharge()
        {
            IAtomType at = (IAtomType)NewChemObject();
            at.FormalCharge = 1;
            IAtomType clone = (IAtomType)at.Clone();

            at.FormalCharge = 2;
            Assert.AreEqual(1, clone.FormalCharge.Value);
        }

        /**
         * Method to test the Clone() method
         */
        [TestMethod()]
        public virtual void TestClone_FormalNeighbourCount()
        {
            IAtomType at = (IAtomType)NewChemObject();
            at.FormalNeighbourCount = 1;
            IAtomType clone = (IAtomType)at.Clone();

            at.FormalNeighbourCount = 2;
            Assert.AreEqual(1, (int)clone.FormalNeighbourCount);
        }

        /**
         * Method to test the Clone() method
         */
        [TestMethod()]
        public virtual void TestClone_Hybridization()
        {
            IAtomType at = (IAtomType)NewChemObject();
            at.Hybridization = Hybridization.Planar3;
            IAtomType clone = (IAtomType)at.Clone();

            at.Hybridization = Hybridization.SP1;
            Assert.AreEqual(Hybridization.Planar3, clone.Hybridization);
        }

        /**
         * Method to test whether the class complies with RFC #9.
         */
        [TestMethod()]

        public override void TestToString()
        {
            IAtomType at = (IAtomType)NewChemObject();
            string description = at.ToString();
            for (int i = 0; i < description.Length; i++)
            {
                Assert.IsTrue(description[i] != '\n');
                Assert.IsTrue(description[i] != '\r');
            }
        }

        [TestMethod()]
        public virtual void TestToString_AtomTypeName()
        {
            IAtomType at = (IAtomType)NewChemObject();
            at.AtomTypeName = "N.sp2.3";
            string description = at.ToString();
            Console.Out.WriteLine(description);
            Assert.IsTrue(description.Contains("N.sp2.3"));
        }

        [TestMethod()]
        public virtual void TestDefaultFormalCharge()
        {
            IAtomType atomType = (IAtomType)NewChemObject();
            Assert.AreEqual(0, atomType.FormalCharge.Value);
        }
    }
}