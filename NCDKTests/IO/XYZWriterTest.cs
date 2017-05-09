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
using NCDK.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using System.IO;

namespace NCDK.IO
{
    /// <summary>
    /// TestCase for the writer XYZ files using one test file.
    /// </summary>
    /// <seealso cref="XYZWriter"/>
    // @cdk.module test-io
    [TestClass()]
    public class XYZWriterTest : ChemObjectIOTest
    {
        protected override IChemObjectIO ChemObjectIOToTest { get; } = new XYZWriter();

        [TestMethod()]
        public void TestAccepts()
        {
            XYZWriter reader = new XYZWriter();
            Assert.IsTrue(reader.Accepts(typeof(AtomContainer)));
        }

        [TestMethod()]
        public void TestWriting()
        {
            StringWriter writer = new StringWriter();
            IAtomContainer molecule = new AtomContainer();
            IAtom atom1 = new Atom("C");
            atom1.Point3D = new Vector3(1.0, 2.0, 3.0);
            IAtom atom2 = new Atom("C");
            atom2.Point3D = new Vector3(1.0, 2.0, 3.0);
            molecule.Atoms.Add(atom1);
            molecule.Atoms.Add(atom2);

            XYZWriter xyzWriter = new XYZWriter(writer);
            xyzWriter.Write(molecule);
            xyzWriter.Close();
            writer.Close();

            string output = writer.ToString();
            //        Debug.WriteLine(output);
            // count lines
            int lineCount = 0;
            var reader = new StringReader(output);
            while (reader.ReadLine() != null)
                lineCount++;
            Assert.AreEqual(4, lineCount);
        }

        // @cdk.bug 2215774
        [TestMethod()]
        public void TestWriting_Point2d()
        {
            StringWriter writer = new StringWriter();
            IAtomContainer molecule = new AtomContainer();
            IAtom atom1 = new Atom("C");
            atom1.Point2D = new Vector2(1.0, 2.0);
            molecule.Atoms.Add(atom1);

            XYZWriter xyzWriter = new XYZWriter(writer);
            xyzWriter.Write(molecule);
            xyzWriter.Close();
            writer.Close();

            string output = writer.ToString();
            Assert.IsTrue(output.Contains("0.000000\t 0.000000\t 0.000000"));
        }

        // @cdk.bug 2215775
        [TestMethod()]
        public void TestSixDecimalOuput()
        {
            StringWriter writer = new StringWriter();
            IAtomContainer molecule = new AtomContainer();
            IAtom atom1 = new Atom("C");
            atom1.Point3D = new Vector3(1.0, 2.0, 3.0);
            molecule.Atoms.Add(atom1);
            IAtom atom2 = new Atom("C");
            atom2.Point3D = new Vector3(-1.5, -2.0, 0.0);
            molecule.Atoms.Add(atom2);

            XYZWriter xyzWriter = new XYZWriter(writer);
            xyzWriter.Write(molecule);
            xyzWriter.Close();
            writer.Close();

            string output = writer.ToString();
            Assert.IsTrue(output.Contains("1.000000"));
            Assert.IsTrue(output.Contains("2.000000"));
            Assert.IsTrue(output.Contains("3.000000"));
        }
    }
}
