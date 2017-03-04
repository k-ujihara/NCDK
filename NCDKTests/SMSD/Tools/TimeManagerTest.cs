/* Copyright (C) 2009  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace NCDK.SMSD.Tools
{
    /// <summary>
    // @author Asad
    // @cdk.module test-smsd
    /// </summary>
    //[TestCategory("SlowTest")]
    // test uses Thread.sleep...
    [TestClass()]
    public class TimeManagerTest : CDKTestCase
    {

        [TestMethod()]
        public void TestTimeManager()
        {
            TimeManager tMan = new TimeManager();
            Assert.IsNotNull(tMan);
        }

        /// <summary>
        /// Test of getElapsedTimeInHours method, of class TimeManager.
        /// </summary>
        [TestMethod()]
        public void TestGetElapsedTimeInHours()
        {
            TimeManager instance = new TimeManager();
            double expResult = 0.0001;
            MyMethod(360);
            double result = instance.GetElapsedTimeInHours();
            Assert.AreEqual(expResult, result, 0.0001);
        }

        /// <summary>
        /// Test of getElapsedTimeInMinutes method, of class TimeManager.
        /// </summary>
        [TestMethod()]
        public void TestGetElapsedTimeInMinutes()
        {
            TimeManager instance = new TimeManager();
            double expResult = 0.006;
            MyMethod(360);
            double result = instance.GetElapsedTimeInMinutes();
            Assert.AreEqual(expResult, result, 0.006);
        }

        /// <summary>
        /// Test of getElapsedTimeInSeconds method, of class TimeManager.
        /// </summary>
        [TestMethod()]
        public void TestGetElapsedTimeInSeconds()
        {
            TimeManager instance = new TimeManager();
            double expResult = 0.36;
            MyMethod(360);
            double result = instance.GetElapsedTimeInSeconds();
            Assert.AreEqual(expResult, result, 0.36);
        }

        /// <summary>
        /// Test of getElapsedTimeInMilliSeconds method, of class TimeManager.
        /// </summary>
        [TestMethod()]
        public void TestGetElapsedTimeInMilliSeconds()
        {
            TimeManager instance = new TimeManager();
            double expResult = 360;
            MyMethod(360);
            double result = instance.GetElapsedTimeInMilliSeconds();
            Assert.AreEqual(expResult, result, 360);
        }

        public void MyMethod(long timeMillis)
        {
            Console.Out.WriteLine("Starting......");

            // pause for a while
            Thread.Sleep((int)timeMillis);
            Console.Out.WriteLine("Ending......");
        }
    }
}
