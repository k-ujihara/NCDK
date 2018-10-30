/* Copyright (C) 2011  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Smiles;
using NCDK.Tools.Manipulator;

namespace NCDK.Geometries.Volume
{
    /// <summary>
    /// Values in the paper are inaccurate. The spreadsheet from the SI is better.
    /// </summary>
    // @cdk.module test-standard
    [TestClass()]
    public class VABCVolumeTest
    {
        private static SmilesParser smilesParser = CDK.SilentSmilesParser;

        [TestMethod()]
        public void TestMethane()
        {
            var methane = smilesParser.ParseSmiles("C");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(methane);
            double volume = VABCVolume.Calculate(methane);
            Assert.AreEqual(25.8524433266667, volume, 0.01);
        }

        [TestMethod()]
        [ExpectedException(typeof(CDKException))]
        public void TestIronChloride()
        {
            var methane = smilesParser.ParseSmiles("Cl[Fe]Cl");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(methane);
            VABCVolume.Calculate(methane);
        }

        [TestMethod()]
        public void TestOmeprazol()
        {
            var methane = smilesParser.ParseSmiles("COc2ccc1[nH]c(nc1c2)S(=O)Cc3ncc(C)c(OC)c3C");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(methane);
            double volume = VABCVolume.Calculate(methane);
            Assert.AreEqual(292.23, volume, 0.01);
        }

        [TestMethod()]
        public void TestSaccharin()
        {
            var methane = smilesParser.ParseSmiles("O=C1NS(=O)(=O)c2ccccc12");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(methane);
            double volume = VABCVolume.Calculate(methane);
            Assert.AreEqual(139.35, volume, 0.01);
        }

        [TestMethod()]
        public void TestAdeforir()
        {
            var methane = smilesParser.ParseSmiles("Nc1ncnc2n(CCOCP(=O)(O)O)cnc12");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(methane);
            double volume = VABCVolume.Calculate(methane);
            Assert.AreEqual(199.84, volume, 0.01);
        }

        [TestMethod()]
        public void TestMethaneWithExplicitHydrogens()
        {
            var methane = smilesParser.ParseSmiles("[H]C([H])([H])[H]");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(methane);
            double volume = VABCVolume.Calculate(methane);
            Assert.AreEqual(25.8524433266667, volume, 0.01);
        }

        [TestMethod()]
        public void TestEthane()
        {
            var methane = smilesParser.ParseSmiles("CC");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(methane);
            double volume = VABCVolume.Calculate(methane);
            Assert.AreEqual(43.1484279525333, volume, 0.01);
        }

        [TestMethod()]
        public void TestButane()
        {
            var methane = smilesParser.ParseSmiles("CCCC");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(methane);
            double volume = VABCVolume.Calculate(methane);
            Assert.AreEqual(77.7403972042667, volume, 0.01);
        }

        [TestMethod()]
        public void TestAcetonitrile()
        {
            var methane = smilesParser.ParseSmiles("CC#N");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(methane);
            double volume = VABCVolume.Calculate(methane);
            Assert.AreEqual(48.8722707591, volume, 0.01);
        }

        [TestMethod()]
        public void TestAceticAcid()
        {
            var methane = smilesParser.ParseSmiles("CC(=O)O");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(methane);
            double volume = VABCVolume.Calculate(methane);
            Assert.AreEqual(58.0924226528555, volume, 0.01);
        }

        [TestMethod()]
        public void TestChloroFluoro()
        {
            var methane = smilesParser.ParseSmiles("CC(F)(F)Cl");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(methane);
            double volume = VABCVolume.Calculate(methane);
            Assert.AreEqual(70.4946134235795, volume, 0.01);
        }

        [TestMethod()]
        public void TestCS2()
        {
            var methane = smilesParser.ParseSmiles("S=C=S");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(methane);
            double volume = VABCVolume.Calculate(methane);
            Assert.AreEqual(57.5975740402667, volume, 0.01);
        }

        [TestMethod()]
        public void TestTriEthylPhosphite()
        {
            var methane = smilesParser.ParseSmiles("CCOP(=O)(OCC)OCC");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(methane);
            double volume = VABCVolume.Calculate(methane);
            Assert.AreEqual(167.320526666244, volume, 0.01);
        }

        [TestMethod()]
        public void TestBenzene()
        {
            var methane = smilesParser.ParseSmiles("c1ccccc1");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(methane);
            double volume = VABCVolume.Calculate(methane);
            Assert.AreEqual(81.1665316528, volume, 0.01);
        }

        [TestMethod()]
        public void TestPyrene()
        {
            var methane = smilesParser.ParseSmiles("c1cc2ccc3cccc4ccc(c1)c2c34");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(methane);
            double volume = VABCVolume.Calculate(methane);
            Assert.AreEqual(171.174708305067, volume, 0.01);
        }

        [TestMethod()]
        public void TestNicotine()
        {
            var methane = smilesParser.ParseSmiles("CN1CCCC1c2cccnc2");
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(methane);
            double volume = VABCVolume.Calculate(methane);
            Assert.AreEqual(159.9875318718, volume, 0.01);
        }
    }
}
