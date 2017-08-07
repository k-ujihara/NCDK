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
using System;
using System.IO;

namespace NCDK.IO
{
    /// <summary>
    /// TestCase for CDK IO classes.
    /// </summary>
    // @cdk.module test-io
    [TestClass()]
    public abstract class ChemObjectReaderTest : ChemObjectIOTest
    {
        protected abstract string TestFile { get; }

        protected IChemObjectReader CreateChemObjectReader(TextReader reader)
        {
            return (IChemObjectReader)ChemObjectIOToTestType.GetConstructor(new Type[] { typeof(TextReader) }).Invoke(new object[] { reader });
        }

        protected IChemObjectReader CreateChemObjectReader(Stream stream)
        {
            return (IChemObjectReader)CreateChemObjectIO(stream);
        }

        protected override IChemObjectIO ChemObjectIOToTest
        {
            get
            {
                try
                {
                    return base.ChemObjectIOToTest;
                }
                catch (Exception)
                {
                    chemObjectIOToTest = CreateChemObjectReader(new StringReader(""));
                }
                return chemObjectIOToTest;
            }
        }

        [TestMethod()]
        public virtual void TestSetReader_InputStream()
        {
            var ctor = ChemObjectIOToTestType.GetConstructor(new Type[] { typeof(Stream) });
            if (ctor != null)
            {
                Assert.IsNotNull(TestFile, "No test file has been set!");
                var ins = ResourceLoader.GetAsStream(TestFile);
                CreateChemObjectReader(ins);
            }
        }

        [TestMethod()]
        public virtual void TestSetReader_Reader()
        {
            var ctor = ChemObjectIOToTestType.GetConstructor(new Type[] { typeof(TextReader) });

            if (ctor != null)
            {
                Assert.IsNotNull(TestFile, "No test file has been set!");
                var ins = ResourceLoader.GetAsStream(TestFile);
                CreateChemObjectReader(new StreamReader(ins));
            }
        }
    }
}
