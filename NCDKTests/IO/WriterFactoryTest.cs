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
using NCDK.IO.Formats;
using NCDK.Tools;

namespace NCDK.IO
{
    /// <summary>
    /// TestCase for the writing files.
    /// </summary>
    // @cdk.module test-io
    [TestClass()]
    public class WriterFactoryTest : CDKTestCase
    {
        private WriterFactory factory = new WriterFactory();

        [TestMethod()]
        public void TestFormatCount()
        {
            Assert.IsTrue(factory.FormatCount > 0);
        }

        [TestMethod()]
        public void TestFindChemFormats()
        {
            IChemFormat[] formats = factory.FindChemFormats(DataFeatures.HAS_3D_COORDINATES);
            Assert.IsNotNull(formats);
            Assert.IsTrue(formats.Length > 0);
        }

        [TestMethod()]
        public void TestCreateWriter_IChemFormat()
        {
            IChemFormat format = (IChemFormat)XYZFormat.Instance;
            IChemObjectWriter writer = factory.CreateWriter(format);
            Assert.IsNotNull(writer);
            Assert.AreEqual(format.FormatName, writer.Format.FormatName);
        }

        [TestMethod()]
        public void TestCustomWriter()
        {
            WriterFactory factory = new WriterFactory();
            factory.RegisterWriter(typeof(CustomWriter));
            IChemObjectWriter writer = factory.CreateWriter(new CustomFormat());
            Assert.IsNotNull(writer);
            Assert.AreEqual(new CustomWriter().GetType().Name, writer.GetType().Name);
        }
    }
}
