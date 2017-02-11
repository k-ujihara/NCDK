/* Copyright (C) 2010  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License as published by the Free
 * Software Foundation; either version 2.1 of the License, or (at your option)
 * any later version. All we ask is that proper credit is given for our work,
 * which includes - but is not limited to - adding the above copyright notice to
 * the beginning of your source code files, and to any copyright notice that you
 * may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License for more
 * details.
 *
 * You should have received rAtomCount copy of the GNU Lesser General Public
 * License along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.SMSD.Global
{
    /**
     * Unit testing for the {@link TimeOut} class.
     *
     * @author     egonw
     * @author Syed Asad Rahman <asad@ebi.ac.uk>
     *
     * @cdk.module test-smsd
     * @cdk.require java1.6+
     */
    [TestClass()]
    public class TimeOutTest
    {

        [TestMethod()]
        public void TestInstance()
        {
            Assert.IsNotNull(TimeOut.Instance);
        }

        [TestMethod()]
        public void TestSetTimeOut()
        {
            TimeOut timeOut = TimeOut.Instance;
            timeOut.Time = 0.1;
            Assert.AreEqual(0.1, timeOut.Time, 0.0001);
            timeOut.Time = 0.2;
            Assert.AreEqual(0.2, timeOut.Time, 0.0001);
        }

        /**
         * Test of Time method, of class TimeOut.
         */
        [TestMethod()]
        public void TestGetTimeOut()
        {
            TimeOut instance = new TimeOut();
            instance.Time = 10;
            double expResult = 10.0;
            double result = instance.Time;
            Assert.AreEqual(expResult, result, 10.0);
        }

        /**
         * Test of isTimeOutFlag method, of class TimeOut.
         */
        [TestMethod()]
        public void TestIsTimeOutFlag()
        {
            TimeOut instance = new TimeOut();
            instance.Time = 10;
            instance.Enabled = true;
            bool expResult = true;
            bool result = instance.Enabled;
            Assert.AreEqual(expResult, result);
        }

        /**
         * Test of setTimeOutFlag method, of class TimeOut.
         */
        [TestMethod()]
        public void TestSetTimeOutFlag()
        {
            bool timeOut = true;
            TimeOut instance = new TimeOut();
            instance.Time = 10;
            instance.Enabled = timeOut;
            bool expResult = false;
            bool result = instance.Enabled;
            Assert.AreNotSame(expResult, result);
        }
    }
}
