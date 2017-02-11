/* Copyright (C) 2006-2007  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.Default;
using System.Diagnostics;

namespace NCDK.IO
{
    /// <summary>
    /// TestCase for the reading CTX files using a test file.
    /// </summary>
    /// <seealso cref="CrystClustReader"/>
    // @cdk.module test-io
	[TestClass()]
    public class CTXReaderTest : SimpleChemObjectReaderTest
    {
        protected override string testFile => "NCDK.Data.CTX.methanol_with_descriptors.ctx";
        static readonly CTXReader simpleReader = new CTXReader();
        protected override IChemObjectIO ChemObjectIOToTest => simpleReader;

        [TestMethod()]
        public void TestAccepts()
        {
            CTXReader reader = new CTXReader();
            Assert.IsTrue(reader.Accepts(typeof(ChemFile)));
        }

        [TestMethod()]
        public void TestMethanol()
        {
            string filename = "NCDK.Data.CTX.methanol_with_descriptors.ctx";
            Trace.TraceInformation("Testing: " + filename);
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            CTXReader reader = new CTXReader(ins);
            IChemFile chemFile = (ChemFile)reader.Read((ChemObject)new ChemFile());
            reader.Close();

            Assert.IsNotNull(chemFile);
            Assert.AreEqual(1, chemFile.Count);
            IChemSequence seq = chemFile[0];
            Assert.IsNotNull(seq);
            Assert.AreEqual(1, seq.Count);
            IChemModel model = seq[0];
            Assert.IsNotNull(model);

            var moleculeSet = model.MoleculeSet;
            Assert.IsNotNull(moleculeSet);
            Assert.AreEqual(1, moleculeSet.Count);

            IAtomContainer container = moleculeSet[0];
            Assert.IsNotNull(container);
            Assert.AreEqual(6, container.Atoms.Count, "Incorrect atom count.");
            Assert.AreEqual(5, container.Bonds.Count);

            Assert.AreEqual("Petra", container.Id);

            Assert.IsNotNull(container.GetProperty<string>(CDKPropertyName.TITLE));
            Assert.AreEqual("CH4O", container.GetProperty<string>(CDKPropertyName.TITLE));
        }
    }
}
