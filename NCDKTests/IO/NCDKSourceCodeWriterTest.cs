/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
 *                    2010  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.Default;
using System;
using System.IO;

namespace NCDK.IO
{
    /// <summary>
    /// TestCase for the writer CDK source code files using one test file.
    /// </summary>
    /// <seealso cref="NCDKSourceCodeWriter"/>
    // @cdk.module test-io
    [TestClass()]
    public class NCDKSourceCodeWriterTest : ChemObjectIOTest
    {
        protected override IChemObjectIO ChemObjectIOToTest { get; } = new NCDKSourceCodeWriter();

        [TestMethod()]
        public void TestAccepts()
        {
            Assert.IsTrue(ChemObjectIOToTest.Accepts(typeof(AtomContainer)));
        }

        [TestMethod()]
        public void TestOutput()
        {
            StringWriter writer = new StringWriter();
            IAtomContainer molecule = new AtomContainer();
            Atom atom = new Atom("C");
            atom.MassNumber = 14;
            molecule.Atoms.Add(atom);

            var sourceWriter = new NCDKSourceCodeWriter(writer);
            sourceWriter.Write(molecule);
            sourceWriter.Close();
            string output = writer.ToString();
            string newline = Environment.NewLine;
            Assert.AreEqual("{" + newline +
                    "  IChemObjectBuilder builder = Default.ChemObjectBuilder.Instance;" + newline +
                    "  IAtomContainer mol = builder.CreateAtomContainer();" + newline +
                    "  IAtom a1 = builder.CreateAtom(\"C\");" + newline +
                    "  a1.FormalCharge = 0;" + newline +
                    "  mol.Atoms.Add(a1);" + newline +
                    "}" + newline, output);
        }
    }
}
