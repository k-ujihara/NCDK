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
using NCDK.Tools.Manipulator;
using System.Diagnostics;
using System.Linq;

namespace NCDK.IO
{
    // @cdk.module test-io
    [TestClass()]
    public class PCCompoundASNReaderTest : SimpleChemObjectReaderTest
    {
        protected override string testFile => "NCDK.Data.ASN.PubChem.cid1.asn";
        static readonly PCCompoundASNReader simpleReader = new PCCompoundASNReader();
        protected override IChemObjectIO ChemObjectIOToTest => simpleReader;

        [TestMethod()]
        public void TestAccepts()
        {
            PCCompoundASNReader reader = new PCCompoundASNReader();
            Assert.IsTrue(reader.Accepts(typeof(ChemFile)));
        }

        [TestMethod()]
        public void TestReading()
        {
            string filename = "NCDK.Data.ASN.PubChem.cid1.asn";
            Trace.TraceInformation("Testing: " + filename);
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            PCCompoundASNReader reader = new PCCompoundASNReader(ins);
            IChemFile cFile = (IChemFile)reader.Read(new ChemFile());
            reader.Close();
            var containers = ChemFileManipulator.GetAllAtomContainers(cFile).ToList();
            Assert.AreEqual(1, containers.Count);
            Assert.IsTrue(containers[0] is IAtomContainer);
            IAtomContainer molecule = containers[0];
            Assert.IsNotNull(molecule);

            // check atom stuff
            Assert.AreEqual(31, molecule.Atoms.Count);
            Assert.IsNotNull(molecule.Atoms[3]);
            Assert.AreEqual("O", molecule.Atoms[3].Symbol);
            Assert.IsNotNull(molecule.Atoms[4]);
            Assert.AreEqual("N", molecule.Atoms[4].Symbol);

            // check bond stuff
            Assert.AreEqual(30, molecule.Bonds.Count);
            Assert.IsNotNull(molecule.Bonds[3]);
            Assert.AreEqual(molecule.Atoms[2], molecule.Bonds[3].Atoms[0]);
            Assert.AreEqual(molecule.Atoms[11], molecule.Bonds[3].Atoms[1]);

            // some extracted props
            Assert.AreEqual("InChI=1/C9H17NO4/c1-7(11)14-8(5-9(12)13)6-10(2,3)4/h8H,5-6H2,1-4H3",
                    molecule.GetProperty<string>(CDKPropertyName.INCHI));
            Assert.AreEqual("CC(=O)OC(CC(=O)[O-])C[N+](C)(C)C", molecule.GetProperty<string>(CDKPropertyName.SMILES));
        }
    }
}

