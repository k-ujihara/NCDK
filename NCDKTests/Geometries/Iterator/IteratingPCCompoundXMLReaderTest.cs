/* Copyright (C) 2008  Egon Willighagen <egonw@users.sf.net>
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
 * 
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;

namespace NCDK.IO.Iterator
{
    // @cdk.module test-io
    [TestClass()]
    public class IteratingPCCompoundXMLReaderTest : CDKTestCase
    {
        [TestMethod()]
        public void TestList()
        {
            string filename = "NCDK.Data.ASN.PubChem.aceticAcids38.xml";
            Trace.TraceInformation("Testing: " + filename);
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            var reader = new IteratingPCCompoundXMLReader(new StreamReader(ins), Default.ChemObjectBuilder.Instance);

            int molCount = 0;
            var set = Default.ChemObjectBuilder.Instance.CreateAtomContainerSet();
            foreach (var obj in reader)
            {
                // Console.Out.WriteLine("next molecule found");
                Assert.IsNotNull(obj);
                Assert.IsTrue(obj is IAtomContainer);
                set.Add((IAtomContainer)obj);
                molCount++;
            }

            Assert.AreEqual(3, molCount);
            IAtomContainer first = set[0];
            Assert.AreEqual(8, first.Atoms.Count);
            Assert.AreEqual(7, first.Bonds.Count);
            Assert.IsNotNull(first.GetProperty<string>("IUPAC Name (Traditional)"));
            Assert.AreEqual("acetic acid", first.GetProperty<string>("IUPAC Name (Traditional)"));
            Assert.IsNotNull(first.GetProperty<string>("InChI"));
            Assert.AreEqual("InChI=1/C2H4O2/c1-2(3)4/h1H3,(H,3,4)/f/h3H", first.GetProperty<string>("InChI"));
            Assert.IsNotNull(first.GetProperty<string>("InChI"));
            Assert.AreEqual("176", first.GetProperty<string>("PubChem CID"));
        }
    }
}
