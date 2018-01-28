/*
 * Copyright 2006-2011 Sam Adams <sea36 at users.sourceforge.net>
 *
 * This file is part of JNI-InChI.
 *
 * JNI-InChI is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * JNI-InChI is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with JNI-InChI.  If not, see <http://www.gnu.org/licenses/>.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace NCDK.Graphs.InChI
{
    [TestClass()]
    public class TestNInchiAtom
    {
        internal static NInchiAtom GetNewTestAtom()
        {
            return new NInchiAtom(1, 2, 3, "C");
        }

        /// <summary>
        /// Test JniInchiAtom constructor.
        ///
        /// </summary>
        [TestMethod()]
        public void TestJniInchiAtomConstructor()
        {
            NInchiAtom atom = GetNewTestAtom();
            // Check configured parameters
            Assert.AreEqual(1.0, atom.X, 1E-6);
            Assert.AreEqual(2.0, atom.Y, 1E-6);
            Assert.AreEqual(3.0, atom.Z, 1E-6);
            Assert.AreEqual("C", atom.ElementType);

            // Check default values set correctly
            Assert.AreEqual(0, atom.Charge);
            Assert.AreEqual(-1, atom.ImplicitH);
            Assert.AreEqual(0, atom.ImplicitProtium);
            Assert.AreEqual(0, atom.ImplicitDeuterium);
            Assert.AreEqual(0, atom.ImplicitTritium);
            Assert.AreEqual(0, atom.IsotopicMass);
            Assert.AreEqual(INCHI_RADICAL.None, atom.Radical);
        }

        /// <summary>
        /// Test setCharge.
        ///
        /// </summary>
        [TestMethod()]
        public void TestSetCharge()
        {
            NInchiAtom atom = GetNewTestAtom();
            atom.Charge = +1;
            Assert.AreEqual(+1, atom.Charge);
        }

        /// <summary>
        /// Test setRadical.
        ///
        /// </summary>
        [TestMethod()]
        public void TestSetRadical()
        {
            NInchiAtom atom = GetNewTestAtom();
            atom.Radical = INCHI_RADICAL.Doublet;
            Assert.AreEqual(INCHI_RADICAL.Doublet, atom.Radical);
        }

        /// <summary>
        /// Test setIsotopicMass.
        ///
        /// </summary>
        [TestMethod()]
        public void TestSetIsotopicMass()
        {
            NInchiAtom atom = GetNewTestAtom();
            atom.IsotopicMass = 13;
            Assert.AreEqual(13, atom.IsotopicMass);
        }

        /// <summary>
        /// Test setIsotopicMassShift.
        ///
        /// </summary>
        [TestMethod()]
        public void TestSetIsotopicMassShift()
        {
            NInchiAtom atom = GetNewTestAtom();
            atom.SetIsotopicMassShift(+1);
            Assert.AreEqual(NInchiAtom.ISOTOPIC_SHIFT_FLAG + 1, atom.IsotopicMass);
        }

        /// <summary>
        /// Test setImplicitH.
        ///
        /// </summary>
        [TestMethod()]
        public void TestSetImplictH()
        {
            NInchiAtom atom = GetNewTestAtom();
            atom.ImplicitH = 3;
            Assert.AreEqual(3, atom.ImplicitH);
        }

        /// <summary>
        /// Test setImplicitProtium.
        ///
        /// </summary>
        [TestMethod()]
        public void TestSetImplictProtium()
        {
            NInchiAtom atom = GetNewTestAtom();
            atom.ImplicitProtium = 2;
            Assert.AreEqual(2, atom.ImplicitProtium);
        }

        /// <summary>
        /// Test setImplicitDeuterium.
        ///
        /// </summary>
        [TestMethod()]
        public void TestSetImplictDeuterium()
        {
            NInchiAtom atom = GetNewTestAtom();
            atom.ImplicitDeuterium = 2;
            Assert.AreEqual(2, atom.ImplicitDeuterium);
        }

        /// <summary>
        /// Test setImplicitTritium.
        ///
        /// </summary>
        [TestMethod()]
        public void TestSetImplictTritium()
        {
            NInchiAtom atom = GetNewTestAtom();
            atom.ImplicitTritium = 2;
            Assert.AreEqual(2, atom.ImplicitTritium);
        }

        /// <summary>
        /// Test getElementType.
        ///
        /// </summary>
        [TestMethod()]
        public void TestGetElementType()
        {
            NInchiAtom atom = GetNewTestAtom();
            Assert.AreEqual("C", atom.ElementType);
        }

        /// <summary>
        /// Test getCharge.
        ///
        /// </summary>
        [TestMethod()]
        public void TestGetCharge()
        {
            NInchiAtom atom = GetNewTestAtom();
            atom.Charge = +1;
            Assert.AreEqual(+1, atom.Charge);
        }

        /// <summary>
        /// Test getRadical.
        ///
        /// </summary>
        [TestMethod()]
        public void TestGetRadical()
        {
            NInchiAtom atom = GetNewTestAtom();
            atom.Radical = INCHI_RADICAL.Triplet;
            Assert.AreEqual(INCHI_RADICAL.Triplet, atom.Radical);
        }

        /// <summary>
        /// Test getX.
        ///
        /// </summary>
        [TestMethod()]
        public void TestGetX()
        {
            NInchiAtom atom = GetNewTestAtom();
            Assert.AreEqual(1.0, atom.X, 1E-6);
        }

        /// <summary>
        /// Test getY.
        ///
        /// </summary>
        [TestMethod()]
        public void TestGetY()
        {
            NInchiAtom atom = GetNewTestAtom();
            Assert.AreEqual(2.0, atom.Y, 1E-6);
        }

        /// <summary>
        /// Test getZ.
        ///
        /// </summary>
        [TestMethod()]
        public void TestGetZ()
        {
            NInchiAtom atom = GetNewTestAtom();
            Assert.AreEqual(3.0, atom.Z, 1E-6);
        }

        /// <summary>
        /// Test getImplicitH.
        ///
        /// </summary>
        [TestMethod()]
        public void TestGetImplicitH()
        {
            NInchiAtom atom = GetNewTestAtom();
            Assert.AreEqual(-1, atom.ImplicitH);
            atom.ImplicitH = 3;
            Assert.AreEqual(3, atom.ImplicitH);
        }

        /// <summary>
        /// Test getImplicitProtium.
        ///
        /// </summary>
        [TestMethod()]
        public void TestGetImplicitProtium()
        {
            NInchiAtom atom = GetNewTestAtom();
            Assert.AreEqual(0, atom.ImplicitProtium);
            atom.ImplicitProtium = 2;
            Assert.AreEqual(2, atom.ImplicitProtium);
        }

        /// <summary>
        /// Test getImplicitDeuterium.
        ///
        /// </summary>
        [TestMethod()]
        public void TestGetImplicitDeuterium()
        {
            NInchiAtom atom = GetNewTestAtom();
            Assert.AreEqual(0, atom.ImplicitDeuterium);
            atom.ImplicitDeuterium = 2;
            Assert.AreEqual(2, atom.ImplicitDeuterium);
        }

        /// <summary>
        /// Test getImplicitTritium.
        ///
        /// </summary>
        [TestMethod()]
        public void TestGetImplicitTritium()
        {
            NInchiAtom atom = GetNewTestAtom();
            Assert.AreEqual(0, atom.ImplicitTritium);
            atom.ImplicitTritium = 2;
            Assert.AreEqual(2, atom.ImplicitTritium);
        }

        [TestMethod()]
        public void TestNullElementSymbol()
        {
            try
            {
                new NInchiAtom(0, 0, 0, null);
                Assert.Fail("Null element symbol");
            }
            catch (ArgumentNullException)
            {
                // pass
            }
        }
    }
}
