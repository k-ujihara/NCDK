/* Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
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
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *  */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.IO.Formats;

namespace NCDK.IO
{
    /// <summary>
    /// TestCase for the instantiation and functionality of the <see cref="ReaderFactory"/> for
    /// io classes currently in 'cdk-extra'.
    /// </summary>
    // @cdk.module test-extra
    [TestClass()]
    public class ExtraReaderFactoryTest : AbstractReaderFactoryTest
    {
        private ReaderFactory factory = new ReaderFactory();

        [TestMethod()]
        public void TestINChI()
        {
            ExpectReader("NCDK.Data.InChI.guanine.inchi.xml", INChIFormat.Instance, -1, -1);
        }

        [TestMethod()]
        public void TestINChIPlainText()
        {
            ExpectReader("NCDK.Data.InChI.guanine.inchi", INChIPlainTextFormat.Instance, -1, -1);
        }

        [TestMethod()]
        public void TestVASP()
        {
            ExpectReader("NCDK.Data.VASP.LiMoS2_optimisation_ISIF3.vasp", VASPFormat.Instance, -1, -1);
        }

        [TestMethod()]
        public void TestGamess()
        {
            ExpectReader("NCDK.Data.Gamess.ch3oh_gam.out", GamessFormat.Instance, -1, -1);
        }
    }
}
