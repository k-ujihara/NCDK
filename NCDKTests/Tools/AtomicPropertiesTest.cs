/* Copyright (C) 2011  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All I ask is that proper credit is given for my work, which includes
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

namespace NCDK.Tools
{
    // @cdk.module test-qsar
    [TestClass()]
    public class AtomicPropertiesTest : CDKTestCase
    {
        [TestMethod()]
        public void TestInstance()
        {
            AtomicProperties props = AtomicProperties.Instance;
            Assert.IsNotNull(props);
            // test singleton pattern
            AtomicProperties props2 = AtomicProperties.Instance;
            Assert.AreEqual(props2, props);
        }

        [TestMethod()]
        public void TestGetMass()
        {
            AtomicProperties props = AtomicProperties.Instance;
            double mass = props.GetMass("C");
            Assert.IsTrue(mass > 0);
        }

        [TestMethod()]
        public void TestGetNormalizedMass()
        {
            AtomicProperties props = AtomicProperties.Instance;
            double mass = props.GetNormalizedMass("C");
            Assert.IsTrue(mass > 0);
        }

        [TestMethod()]
        public void TestGetPolarizability()
        {
            AtomicProperties props = AtomicProperties.Instance;
            double polar = props.GetPolarizability("C");
            Assert.IsTrue(polar > 0);
        }

        [TestMethod()]
        public void TestGetNormalizedPolarizability()
        {
            AtomicProperties props = AtomicProperties.Instance;
            double polar = props.GetNormalizedPolarizability("C");
            Assert.IsTrue(polar > 0);
        }

        [TestMethod()]
        public void TestGetVdWVolume()
        {
            AtomicProperties props = AtomicProperties.Instance;
            double vol = props.GetVdWVolume("C");
            Assert.IsTrue(vol > 0);
        }

        [TestMethod()]
        public void TestGetNormalizedVdWVolume()
        {
            AtomicProperties props = AtomicProperties.Instance;
            double vol = props.GetNormalizedVdWVolume("C");
            Assert.IsTrue(vol > 0);
        }

        [TestMethod()]
        public void TestGetElectronegativity()
        {
            AtomicProperties props = AtomicProperties.Instance;
            double eneg = props.GetElectronegativity("C");
            Assert.IsTrue(eneg > 0);
        }

        [TestMethod()]
        public void TestGetNormalizedElectronegativity()
        {
            AtomicProperties props = AtomicProperties.Instance;
            double eneg = props.GetNormalizedElectronegativity("C");
            Assert.IsTrue(eneg > 0);
        }
    }
}
