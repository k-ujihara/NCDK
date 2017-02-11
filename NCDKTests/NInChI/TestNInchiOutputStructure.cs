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
    public class TestNInchiOutputStructure
    {

        /*
         * Test method for 'net.sf.jniinchi.JniInchiOutputStructure.ReturnStatus'
         */
        [TestMethod()]
        public void TestGetReturnStatus()
        {
            NInchiOutputStructure output = new NInchiOutputStructure(INCHI_RET.OKAY);
            Assert.AreEqual(INCHI_RET.OKAY, output.ReturnStatus);
        }

        /*
         * Test method for 'net.sf.jniinchi.JniInchiOutputStructure.Message'
         */
        [TestMethod()]
        public void TestGetMessage()
        {
            NInchiOutputStructure output = new NInchiOutputStructure(INCHI_RET.OKAY);
            output.Message = "Test message";
            Assert.AreEqual("Test message", output.Message);
        }

        /*
         * Test method for 'net.sf.jniinchi.JniInchiOutputStructure.Log'
         */
        [TestMethod()]
        public void TestGetLog()
        {
            NInchiOutputStructure output = new NInchiOutputStructure(INCHI_RET.OKAY);
            output.Log = "Test log";
            Assert.AreEqual("Test log", output.Log);
        }

        /*
         * Test method for 'net.sf.jniinchi.JniInchiOutputStructure.WarningFlags'
         */
        [TestMethod()]
        public void TestGetWarningFlags()
        {
            NInchiOutputStructure output = new NInchiOutputStructure(INCHI_RET.OKAY);
            ulong[,] flags = new ulong[,] { { 1, 2 }, { 3, 4 } };
            output.WarningFlags = flags;
            Assert.AreEqual(flags, output.WarningFlags);
        }
    }
}

