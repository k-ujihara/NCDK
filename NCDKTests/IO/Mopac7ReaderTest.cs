/* Copyright (C) 2012  Egon Willighagen <egon.willighagen@gmail.com>
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
using System;
using System.IO;

namespace NCDK.IO
{
    // @cdk.module test-io
    [TestClass()]
    public class Mopac7ReaderTest : SimpleChemObjectReaderTest
    {
        protected override string TestFile => "NCDK.Data.MOPAC.ethylene.dat.out";
        protected override Type ChemObjectIOToTestType => typeof(Mopac7Reader);

        [TestMethod()]
        public void TestAccepts()
        {
            Mopac7Reader reader = new Mopac7Reader(new StringReader(""));
            reader.ReaderMode = ChemObjectReaderMode.Strict;
            Assert.IsTrue(reader.Accepts(typeof(AtomContainer)));
        }
    }
}
