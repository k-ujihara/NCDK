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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCDK.IO
{
    /**
     * TestCase for CDK IO classes.
     *
     * @cdk.module test-io
     */
    [TestClass()]
    public abstract class ChemObjectReaderTest : ChemObjectIOTest
    {
        protected IChemObjectReader ChemObjectReaderToTest => (IChemObjectReader)ChemObjectIOToTest;
        protected abstract string testFile { get; }

        [TestMethod()]
        public virtual void TestSetReader_InputStream()
        {
            Assert.IsNotNull(testFile, "No test file has been set!");
            var ins = typeof(ChemObjectReaderTest).Assembly.GetManifestResourceStream(testFile);
            ChemObjectReaderToTest.SetReader(ins);
        }

        [TestMethod()]
        public virtual void TestSetReader_Reader()
        {
            Assert.IsNotNull(testFile, "No test file has been set!");
            var ins = typeof(ChemObjectReaderTest).Assembly.GetManifestResourceStream(testFile);
            ChemObjectReaderToTest.SetReader(new StreamReader(ins));
        }
    }
}
