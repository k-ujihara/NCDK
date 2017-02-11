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
    public class TestNInchiInput
    {
        /*
         * Test method for 'net.sf.jniinchi.JniInchiInput.Options'
         */
        [TestMethod()]
        public void TestGetOptions()
        {
            NInchiInput input = new NInchiInput();
            input.Options = NInchiWrapper.FlagChar + "compress";
            Assert.AreEqual(NInchiWrapper.FlagChar + "compress", input.Options);
        }
    }
}
