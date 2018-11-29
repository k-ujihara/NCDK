/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
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
using System.IO;

namespace NCDK.IO
{
    /// <summary>
    /// TestCase for CDK IO classes.
    /// </summary>
    // @cdk.module test-io
    public abstract class SimpleChemObjectReaderTest 
        : ChemObjectReaderTest
    {
        protected ISimpleChemObjectReader CreateSimpleChemObjectReader(Stream stream) => (ISimpleChemObjectReader)CreateChemObjectIO(stream);
        protected ISimpleChemObjectReader CreateSimpleChemObjectReader(TextReader reader) => (ISimpleChemObjectReader)CreateChemObjectReader(reader);

        [TestMethod()]
        public virtual void TestRead_IChemObject()
        {
            Assert.IsNotNull(TestFile, "No test file has been set!");

            bool read = false;
            foreach (var obj in AcceptableChemObjects())
            {
                if (ChemObjectIOToTest.Accepts(obj.GetType()))
                {
                    var ins = ResourceLoader.GetAsStream(TestFile);
                    using (var reader = CreateSimpleChemObjectReader(ins))
                    {
                        IChemObject readObject = reader.Read(obj);
                        Assert.IsNotNull(readObject, "Failed attempt to read the file as " + obj.GetType().Name);
                        read = true;
                    }
                }
            }
            if (!read)
            {
                Assert.Fail("Reading an IChemObject from the Reader did not work properly.");
            }
        }
    }
}
