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
    public class TestNInchiBond
    {
        private static NInchiAtom atO = new NInchiAtom(0, 0, 0, "O");
        private static NInchiAtom atT = new NInchiAtom(1.21, 0, 0, "O");

        protected internal static NInchiBond GetTestBond()
        {
            return new NInchiBond(atO, atT, INCHI_BOND_TYPE.Double);
        }

        /*
         * Test method for 'net.sf.jniinchi.JniInchiBond.JniInchiBond(JniInchiAtom, JniInchiAtom, INCHI_BOND_TYPE, INCHI_BOND_STEREO)'
         */
        [TestMethod()]
        public void TestJniInchiBondJniInchiAtomJniInchiAtomINCHI_BOND_TYPEINCHI_BOND_STEREO()
        {
            NInchiBond bond = new NInchiBond(atO, atT, INCHI_BOND_TYPE.Double, INCHI_BOND_STEREO.DoubleEither);
            Assert.AreEqual(atO, bond.OriginAtom);
            Assert.AreEqual(atT, bond.TargetAtom);
            Assert.AreEqual(INCHI_BOND_TYPE.Double, bond.BondType);
            Assert.AreEqual(INCHI_BOND_STEREO.DoubleEither, bond.BondStereo);
        }

        /*
         * Test method for 'net.sf.jniinchi.JniInchiBond.JniInchiBond(JniInchiAtom, JniInchiAtom, INCHI_BOND_TYPE)'
         */
        [TestMethod()]
        public void TestJniInchiBondJniInchiAtomJniInchiAtomINCHI_BOND_TYPE()
        {
            NInchiBond bond = new NInchiBond(atO, atT, INCHI_BOND_TYPE.Double);
            Assert.AreEqual(atO, bond.OriginAtom);
            Assert.AreEqual(atT, bond.TargetAtom);
            Assert.AreEqual(INCHI_BOND_TYPE.Double, bond.BondType);
            Assert.AreEqual(INCHI_BOND_STEREO.None, bond.BondStereo);
        }

        /*
         * Test method for 'net.sf.jniinchi.JniInchiBond.setStereoDefinition(INCHI_BOND_STEREO)'
         */
        [TestMethod()]
        public void TestSetStereoDefinition()
        {
            NInchiBond bond = new NInchiBond(atO, atT, INCHI_BOND_TYPE.Double);
            Assert.AreEqual(INCHI_BOND_STEREO.None, bond.BondStereo);
            bond.BondStereo = INCHI_BOND_STEREO.DoubleEither;
            Assert.AreEqual(INCHI_BOND_STEREO.DoubleEither, bond.BondStereo);
        }

        /*
         * Test method for 'net.sf.jniinchi.JniInchiBond.OriginAtom'
         */
        [TestMethod()]
        public void TestGetOriginAtom()
        {
            NInchiBond bond = new NInchiBond(atO, atT, INCHI_BOND_TYPE.Double);
            Assert.AreEqual(atO, bond.OriginAtom);
            bond = new NInchiBond(atT, atO, INCHI_BOND_TYPE.Double);
            Assert.AreEqual(atT, bond.OriginAtom);
        }

        /*
         * Test method for 'net.sf.jniinchi.JniInchiBond.TargetAtom'
         */
        [TestMethod()]
        public void TestGetTargetAtom()
        {
            NInchiBond bond = new NInchiBond(atO, atT, INCHI_BOND_TYPE.Double);
            Assert.AreEqual(atT, bond.TargetAtom);
            bond = new NInchiBond(atT, atO, INCHI_BOND_TYPE.Double);
            Assert.AreEqual(atO, bond.TargetAtom);
        }

        /*
         * Test method for 'net.sf.jniinchi.JniInchiBond.BondType'
         */
        [TestMethod()]
        public void TestGetBondType()
        {
            NInchiBond bond = new NInchiBond(atO, atT, INCHI_BOND_TYPE.Double);
            Assert.AreEqual(INCHI_BOND_TYPE.Double, bond.BondType);
            bond = new NInchiBond(atO, atT, INCHI_BOND_TYPE.Single);
            Assert.AreEqual(INCHI_BOND_TYPE.Single, bond.BondType);
        }

        /*
         * Test method for 'net.sf.jniinchi.JniInchiBond.BondStereo'
         */
        [TestMethod()]
        public void TestGetBondStereo()
        {
            NInchiBond bond = new NInchiBond(atO, atT, INCHI_BOND_TYPE.Double, INCHI_BOND_STEREO.DoubleEither);
            Assert.AreEqual(INCHI_BOND_STEREO.DoubleEither, bond.BondStereo);
            bond = new NInchiBond(atO, atT, INCHI_BOND_TYPE.Single, INCHI_BOND_STEREO.Single1Up);
            Assert.AreEqual(INCHI_BOND_STEREO.Single1Up, bond.BondStereo);
        }
    }
}
