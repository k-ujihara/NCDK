/* Copyright (C) 2009  The Chemistry Development Kit (CDK) project
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
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System;

namespace NCDK.IO.RDF
{
    // @cdk.module test-iordf
    [TestClass()]
    public class CDKOWLReaderTest 
        : SimpleChemObjectReaderTest
    {
        private readonly IChemObjectBuilder builder = CDK.Builder;
        protected override string TestFile => "NCDK.Data.OWL.molecule.n3";
        protected override Type ChemObjectIOToTestType => typeof(CDKOWLReader);

        [TestMethod()]
        public void TestAccepts()
        {
            Assert.IsTrue(ChemObjectIOToTest.Accepts(typeof(IAtomContainer)));
        }

        [TestMethod()]
        public void TestMolecule()
        {
            var filename = "NCDK.Data.OWL.molecule.n3";
            Trace.TraceInformation($"Testing: {filename}");
            var ins = ResourceLoader.GetAsStream(filename);
            IAtomContainer mol;
            using (var reader = new CDKOWLReader(new StreamReader(ins)))
            {
                mol = reader.Read(builder.NewAtomContainer());
            }
            Assert.IsNotNull(mol);
            Assert.AreEqual(2, mol.Atoms.Count);
            Assert.AreEqual(1, mol.Bonds.Count);
        }
    }
}
