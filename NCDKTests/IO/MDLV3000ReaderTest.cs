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
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Sgroups;
using NCDK.Silent;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace NCDK.IO
{
    /// <summary>
    /// TestCase for the reading MDL V3000 mol files using one test file.
    /// </summary>
    /// <seealso cref="MDLReader"/>
    /// <seealso cref="SDFReaderTest"/>
    // @cdk.module test-io
    [TestClass()]
    public class MDLV3000ReaderTest : SimpleChemObjectReaderTest
    {
        protected override string TestFile => "NCDK.Data.MDL.molV3000.mol";
        protected override Type ChemObjectIOToTestType => typeof(MDLV3000Reader);
        private static readonly IChemObjectBuilder builder = ChemObjectBuilder.Instance;
        private static readonly Type typeOfAtomContainer = builder.NewAtomContainer().GetType();

        [TestMethod()]
        public void TestAccepts()
        {
            MDLV3000Reader reader = new MDLV3000Reader(new StringReader(""));
            Assert.IsTrue(reader.Accepts(typeOfAtomContainer));
        }

        // @cdk.bug 1571207
        [TestMethod()]
        public void TestBug1571207()
        {
            var filename = "NCDK.Data.MDL.molV3000.mol";
            Trace.TraceInformation("Testing: " + filename);
            using (var ins = ResourceLoader.GetAsStream(filename))
            {
                MDLV3000Reader reader = new MDLV3000Reader(ins);
                IAtomContainer m = reader.Read(builder.NewAtomContainer());
                reader.Close();
                Assert.IsNotNull(m);
                Assert.AreEqual(31, m.Atoms.Count);
                Assert.AreEqual(34, m.Bonds.Count);

                IAtom atom = m.Atoms[0];
                Assert.IsNotNull(atom);
                Assert.IsNotNull(atom.Point2D);
                Assert.AreEqual(10.4341, atom.Point2D.Value.X, 0.0001);
                Assert.AreEqual(5.1053, atom.Point2D.Value.Y, 0.0001);
            }
        }

        [TestMethod()]
        public void TestEmptyString()
        {
            string emptyString = "";
            try
            {
                using (MDLV3000Reader reader = new MDLV3000Reader(new StringReader(emptyString)))
                {
                    reader.Read(builder.NewAtomContainer());
                    reader.Close();
                    Assert.Fail("Should have received a CDK Exception");
                }
            }
            catch (CDKException cdkEx)
            {
                Assert.AreEqual("Expected a header line, but found nothing.", cdkEx.Message);
            }
        }

        [TestMethod()]
        public void TestPseudoAtomLabels()
        {
            using (var ins = ResourceLoader.GetAsStream("NCDK.Data.MDL.pseudoatomsv3000.mol"))
            using (MDLV3000Reader reader = new MDLV3000Reader(ins))
            {
                IAtomContainer molecule = builder.NewAtomContainer();
                molecule = reader.Read(molecule);
                reader.Close();
                Assert.IsTrue(molecule.Atoms[9] is IPseudoAtom);
                Assert.AreEqual("R", molecule.Atoms[9].Symbol);
                IPseudoAtom pa = (IPseudoAtom)molecule.Atoms[9];
                Assert.AreEqual("Leu", pa.Label);
            }
        }

        [TestMethod()]
        public void PseuDoAtomReplacement()
        {
            using (MDLV3000Reader reader = new MDLV3000Reader(GetType().Assembly.GetManifestResourceStream(GetType(), "pseudoAtomReplacement.mol")))
            {
                IAtomContainer container = reader.Read(builder.NewAtomContainer());
                foreach (var atom in container.Bonds[9].Atoms)
                {
                    Assert.IsTrue(container.Contains(atom));
                }
            }
        }

        [TestMethod()]
        public void PositionalVariation()
        {
            using (MDLV3000Reader reader = new MDLV3000Reader(GetType().Assembly.GetManifestResourceStream(GetType(), "multicenterBond.mol")))
            {
                IAtomContainer container = reader.Read(builder.NewAtomContainer());
                Assert.AreEqual(8, container.Bonds.Count);
                var sgroups = container.GetCtabSgroups();
                Assert.IsNotNull(sgroups);
                Assert.AreEqual(1, sgroups.Count);
                Assert.AreEqual(SgroupType.ExtMulticenter, sgroups[0].Type);
            }
        }

        [TestMethod()]
        public void RadicalsInCH3()
        {
            using (MDLV3000Reader reader = new MDLV3000Reader(GetType().Assembly.GetManifestResourceStream(GetType(), "CH3.mol")))
            {
                IAtomContainer container = reader.Read(builder.NewAtomContainer());
                Assert.AreEqual(1, container.SingleElectrons.Count);
                Assert.AreEqual(3, container.Atoms[0].ImplicitHydrogenCount);
            }
        }

        [TestMethod()]
        public void Issue602()
        {
            using (var reader = new MDLV3000Reader(ResourceLoader.GetAsStream(GetType(), "issue602.mol")))
            {
                var mol = reader.Read(builder.NewAtomContainer());
                Assert.AreEqual(31, mol.Atoms.Count);
            }
        }
    }
}
