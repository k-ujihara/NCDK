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

namespace NCDK.Graphs.InChI
{
    [TestClass()]
    public class TestNInchiStructure
    {
        /// <summary>
        /// Test method for 'net.sf.jniinchi.JniInchiStructure.Atoms.Count'
        /// </summary>
        [TestMethod()]
        public void TestGetNumAtoms()
        {
            NInchiStructure structure = new NInchiStructure();
            Assert.AreEqual(0, structure.Atoms.Count);
            structure.Atoms.Add(TestNInchiAtom.GetNewTestAtom());
            Assert.AreEqual(1, structure.Atoms.Count);
        }

        /// <summary>
        /// Test method for 'net.sf.jniinchi.JniInchiStructure.Bonds.Count'
        /// </summary>
        [TestMethod()]
        public void TestGetNumBonds()
        {
            NInchiStructure structure = new NInchiStructure();
            Assert.AreEqual(0, structure.Bonds.Count);
            structure.Bonds.Add(TestNInchiBond.GetTestBond());
            Assert.AreEqual(1, structure.Bonds.Count);
        }

        /// <summary>
        /// Test method for 'net.sf.jniinchi.JniInchiStructure.Stereos.Count'
        /// </summary>
        [TestMethod()]
        public void TestGetNumStereo0D()
        {
            NInchiStructure structure = new NInchiStructure();
            Assert.AreEqual(0, structure.Stereos.Count);
            structure.Stereos.Add(TestNInchiStereo0D.GetTestStereo0D());
            Assert.AreEqual(1, structure.Stereos.Count);
        }

        /// <summary>
        /// Test method for 'net.sf.jniinchi.JniInchiStructure.Atoms.Add(JniInchiAtom)'
        /// </summary>
        [TestMethod()]
        public void TestAddAtom()
        {
            NInchiStructure structure = new NInchiStructure();
            Assert.AreEqual(0, structure.Atoms.Count);
            structure.Atoms.Add(TestNInchiAtom.GetNewTestAtom());
            Assert.AreEqual(1, structure.Atoms.Count);
        }

        /// <summary>
        /// Test method for 'net.sf.jniinchi.JniInchiStructure.Bonds.Add(JniInchiBond)'
        /// </summary>
        [TestMethod()]
        public void TestAddBond()
        {
            NInchiStructure structure = new NInchiStructure();
            Assert.AreEqual(0, structure.Bonds.Count);
            structure.Bonds.Add(TestNInchiBond.GetTestBond());
            Assert.AreEqual(1, structure.Bonds.Count);
        }

        /// <summary>
        /// Test method for 'net.sf.jniinchi.JniInchiStructure.addParity(JniInchiStereo0D)'
        /// </summary>
        [TestMethod()]
        public void TestAddStereo0D()
        {
            NInchiStructure structure = new NInchiStructure();
            Assert.AreEqual(0, structure.Stereos.Count);
            structure.Stereos.Add(TestNInchiStereo0D.GetTestStereo0D());
            Assert.AreEqual(1, structure.Stereos.Count);
        }

        /// <summary>
        /// Test method for 'net.sf.jniinchi.JniInchiStructure.Atoms[int]'
        /// </summary>
        [TestMethod()]
        public void TestGetAtom()
        {
            NInchiStructure structure = new NInchiStructure();
            NInchiAtom atom = TestNInchiAtom.GetNewTestAtom();
            structure.Atoms.Add(atom);
            Assert.AreEqual(atom, structure.Atoms[0]);
        }

        /// <summary>
        /// Test method for 'net.sf.jniinchi.JniInchiStructure.GetBond(int)'
        /// </summary>
        [TestMethod()]
        public void TestGetBond()
        {
            NInchiStructure structure = new NInchiStructure();
            NInchiBond bond = TestNInchiBond.GetTestBond();
            structure.Bonds.Add(bond);
            Assert.AreEqual(bond, structure.Bonds[0]);
        }

        /// <summary>
        /// Test method for 'net.sf.jniinchi.JniInchiStructure.GetStereo0D(int)'
        /// </summary>
        [TestMethod()]
        public void TestGetStereo0D()
        {
            NInchiStructure structure = new NInchiStructure();
            NInchiStereo0D stereo = TestNInchiStereo0D.GetTestStereo0D();
            structure.Stereos.Add(stereo);
            Assert.AreEqual(stereo, structure.Stereos[0]);
        }
    }
}
