/* Copyright (C) 2009-2010 Syed Asad Rahman <asad@ebi.ac.uk>
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
using NCDK.SMSD.Algorithms.VFLib;
using System.Linq;

namespace NCDK.SMSD.Ring
{
    /// <summary>
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    ///
    // @cdk.module test-smsd
    // @cdk.require java1.6+
    /// </summary>
    [TestClass()]
    public class HanserRingFinderTest
    {
        /// <summary>
        /// Test of findRings method, of class HanserRingFinder.
        /// </summary>
        [TestMethod()]
        public void TestFindRings()
        {
            IAtomContainer molecule = null;
            HanserRingFinder instance = new HanserRingFinder();
            var result = instance.FindRings(molecule);
            Assert.IsNull(result);
        }

        private HanserRingFinder finder = new HanserRingFinder();

        [TestMethod()] // fixed CDK bug
        public void TestItShoudFindOneRingInBenzene()
        {
            IAtomContainer benzene = Molecules.CreateBenzene();
            var rings = finder.FindRings(benzene);

            Assert.AreEqual(1, rings.Count());
        }

        [TestMethod()] // fixed CDK bug
        public void TestItShouldFindThreeRingsInNaphthalene()
        {
            IAtomContainer naphthalene = Molecules.CreateNaphthalene();
            var rings = finder.FindRings(naphthalene);

            Assert.AreEqual(3, rings.Count());
        }

        [TestMethod()] // fixed CDK bug
        public void TestItShouldFind28RingsInCubane()
        {
            IAtomContainer cubane = Molecules.CreateCubane();
            var rings = finder.FindRings(cubane);

            Assert.AreEqual(28, rings.Count());
        }
    }
}
