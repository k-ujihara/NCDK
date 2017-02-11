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

namespace NCDK.NInChI
{
	[TestClass()]
    public class TestNInchiStereo0D
    {
        private static NInchiAtom atC = new NInchiAtom(0, 0, 0, "C");
        private static NInchiAtom at0 = new NInchiAtom(0, 0, 0, "N");
        private static NInchiAtom at1 = new NInchiAtom(0, 0, 0, "O");
        private static NInchiAtom at2 = new NInchiAtom(0, 0, 0, "S");
        private static NInchiAtom at3 = new NInchiAtom(0, 0, 0, "F");

        protected internal static NInchiStereo0D GetTestStereo0D()
        {
            return new NInchiStereo0D(atC, at0, at1, at2, at3, INCHI_STEREOTYPE.Tetrahedral, INCHI_PARITY.Odd);
        }

        /*
         * Test method for 'net.sf.jniinchi.JniInchiStereo0D.JniInchiStereo0D(JniInchiAtom, JniInchiAtom, JniInchiAtom, JniInchiAtom, JniInchiAtom, INCHI_STEREOTYPE, INCHI_PARITY)'
         */
        [TestMethod()]
        public void TestJniInchiStereo0D()
        {
            NInchiStereo0D stereo = new NInchiStereo0D(atC, at0, at1, at2, at3, INCHI_STEREOTYPE.Tetrahedral, INCHI_PARITY.Odd);
            Assert.AreEqual(atC, stereo.CentralAtom);
            NInchiAtom[] neighbors = stereo.Neighbors;
            Assert.AreEqual(at0, neighbors[0]);
            Assert.AreEqual(at1, neighbors[1]);
            Assert.AreEqual(at2, neighbors[2]);
            Assert.AreEqual(at3, neighbors[3]);
            Assert.AreEqual(INCHI_STEREOTYPE.Tetrahedral, stereo.StereoType);
            Assert.AreEqual(INCHI_PARITY.Odd, stereo.Parity);
        }

        /*
         * Test method for 'net.sf.jniinchi.JniInchiStereo0D.setDisconnectedParity(INCHI_PARITY)'
         */
        [TestMethod()]
        public void TestSetDisconnectedParity()
        {
            NInchiStereo0D stereo = GetTestStereo0D();
            Assert.AreEqual(INCHI_PARITY.None, stereo.DisconnectedParity);
            stereo.DisconnectedParity = INCHI_PARITY.Even;
            Assert.AreEqual(INCHI_PARITY.Even, stereo.DisconnectedParity);
        }

        /*
         * Test method for 'net.sf.jniinchi.JniInchiStereo0D.CentralAtom'
         */
        [TestMethod()]
        public void TestGetCentralAtom()
        {
            NInchiStereo0D stereo = new NInchiStereo0D(null, null, null, null, null, INCHI_STEREOTYPE.None, INCHI_PARITY.None);
            Assert.IsNull(stereo.CentralAtom);
            stereo = new NInchiStereo0D(atC, null, null, null, null, INCHI_STEREOTYPE.None, INCHI_PARITY.None);
            Assert.AreEqual(atC, stereo.CentralAtom);
        }

        /*
         * Test method for 'net.sf.jniinchi.JniInchiStereo0D.Neighbors'
         */
        [TestMethod()]
        public void TestGetNeighbors()
        {
            NInchiStereo0D stereo = new NInchiStereo0D(atC, at0, at1, at2, at3, INCHI_STEREOTYPE.Tetrahedral, INCHI_PARITY.Even);
            NInchiAtom[] neighbours = { at0, at1, at2, at3 };
			for (int i = 0; i < neighbours.Length; i++)
	            Assert.AreEqual(neighbours[i], stereo.Neighbors[i]);
        }

        /*
         * Test method for 'net.sf.jniinchi.JniInchiStereo0D.Parity'
         */
        [TestMethod()]
        public void TestGetParity()
        {
            NInchiStereo0D stereo = new NInchiStereo0D(atC, at0, at1, at2, at3, INCHI_STEREOTYPE.Tetrahedral, INCHI_PARITY.Even);
            Assert.AreEqual(INCHI_PARITY.Even, stereo.Parity);
            stereo = new NInchiStereo0D(atC, at0, at1, at2, at3, INCHI_STEREOTYPE.Tetrahedral, INCHI_PARITY.Odd);
            Assert.AreEqual(INCHI_PARITY.Odd, stereo.Parity);
        }

        /*
         * Test method for 'net.sf.jniinchi.JniInchiStereo0D.StereoType'
         */
        [TestMethod()]
        public void TestGetStereoType()
        {
            NInchiStereo0D stereo = new NInchiStereo0D(atC, at0, at1, at2, at3, INCHI_STEREOTYPE.Tetrahedral, INCHI_PARITY.Even);
            Assert.AreEqual(INCHI_STEREOTYPE.Tetrahedral, stereo.StereoType);
            stereo = new NInchiStereo0D(atC, at0, at1, at2, at3, INCHI_STEREOTYPE.Allene, INCHI_PARITY.Even);
            Assert.AreEqual(INCHI_STEREOTYPE.Allene, stereo.StereoType);
        }
    }
}
