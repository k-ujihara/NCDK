/* Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@slists.sourceforge.net
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
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *  */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Templates;
using System.IO;

namespace NCDK.IO
{
    /// <summary>
    /// TestCase for the reading MDL mol files using one test file.
    /// A test case for SDF files is available as separate Class.
    /// </summary>
    /// <seealso cref="GaussianInputWriter"/>
    // @cdk.module test-io
    [TestClass()]
    public class GaussianInputWriterTest : ChemObjectIOTest
    {
        protected override IChemObjectIO ChemObjectIOToTest { get; } = new GaussianInputWriter();
        private static IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;

        [TestMethod()]
        public void TestAccepts()
        {
            GaussianInputWriter reader = new GaussianInputWriter();
            Assert.IsTrue(reader.Accepts(typeof(IAtomContainer)));
        }

        // @cdk.bug 2501715
        [TestMethod()]
        public void TestWrite()
        {
            IAtomContainer molecule = TestMoleculeFactory.MakeAlphaPinene();
            StringWriter writer = new StringWriter();
            GaussianInputWriter gaussianWriter = new GaussianInputWriter(writer);
            gaussianWriter.Write(molecule);
            gaussianWriter.Close();
            string output = writer.ToString();
            Assert.AreNotSame(0, output.Length);
        }
    }
}
