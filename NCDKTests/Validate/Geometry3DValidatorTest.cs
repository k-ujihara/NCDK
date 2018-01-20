/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
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
using NCDK.Default;
using NCDK.IO;

namespace NCDK.Validate
{
    // @cdk.module test-extra
    [TestClass()]
    public class Geometry3DValidatorTest : CDKTestCase
    {
        [TestMethod()]
        public void TestEthane()
        {
            string filename = "NCDK.Data.MDL.Heptane-TestFF-output.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLV2000Reader reader = new MDLV2000Reader(ins, ChemObjectReaderMode.Strict);
            ChemFile chemFile = (ChemFile)reader.Read((ChemObject)new ChemFile());
            ValidatorEngine engine = new ValidatorEngine();
            engine.Add(new Geometry3DValidator());
            ValidationReport report = engine.ValidateChemFile(chemFile);
            Assert.AreEqual(0, report.Errors.Count);
            Assert.AreEqual(0, report.Warnings.Count);
        }
    }
}
