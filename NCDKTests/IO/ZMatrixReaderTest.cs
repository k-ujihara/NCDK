/* ZMatrixReaderTest.java
 *
 * Autor: Stephan Michels
 * EMail: stephan@vern.chem.tu-berlin.de
 * Datum: 20.7.2001
 *
 * Copyright (C) 2001-2007  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
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
    // @cdk.module test-extra
    [TestClass()]
    public class ZMatrixReaderTest : ChemObjectIOTest
    {
        protected override Type ChemObjectIOToTestType => typeof(ZMatrixReader);

        [TestMethod()]
        public void TestAccepts()
        {
            ZMatrixReader reader = new ZMatrixReader(new StringReader(""));
            Assert.IsTrue(reader.Accepts(typeof(ChemFile)));
        }
    }
}
