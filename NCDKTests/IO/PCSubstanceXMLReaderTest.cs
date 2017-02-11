/* Copyright (C) 2006-2007  Egon Willighagen <egonw@users.sf.net>
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
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using System.Diagnostics;

namespace NCDK.IO
{
    // @cdk.module test-io
    [TestClass()]
    public class PCSubstanceXMLReaderTest : SimpleChemObjectReaderTest
    {
        protected override string testFile => "NCDK.Data.ASN.PubChem.sid577309.xml";
        static readonly PCSubstanceXMLReader simpleReader = new PCSubstanceXMLReader();
        protected override IChemObjectIO ChemObjectIOToTest => simpleReader;

        [TestMethod()]
        public void TestAccepts()
        {
            PCSubstanceXMLReader reader = new PCSubstanceXMLReader();
            Assert.IsTrue(reader.Accepts(typeof(AtomContainer)));
        }

        [TestMethod()]
        public void TestReading()
        {
            string filename = "NCDK.Data.ASN.PubChem.sid577309.xml";
            Trace.TraceInformation("Testing: " + filename);
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            PCSubstanceXMLReader reader = new PCSubstanceXMLReader(ins);
            IAtomContainer molecule = reader.Read(new AtomContainer());
            Assert.IsNotNull(molecule);

            // check atom stuff
            Assert.AreEqual(19, molecule.Atoms.Count);
            Assert.IsTrue(molecule.Atoms[0] is IPseudoAtom);

            // check bond stuff
            Assert.AreEqual(19, molecule.Bonds.Count);
            Assert.IsNotNull(molecule.Bonds[3]);
        }
    }
}
