/*
 * Copyright (c) 2013. John May <jwmay@users.sf.net>
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
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.IO.Formats;
using System.IO;

namespace NCDK.IO
{
    /// <summary>
    /// TestCase for the writing files.
    /// </summary>
    // @cdk.module test-libiocml
    [TestClass()]
    public class CMLWriterFactoryTest
    {
        private readonly WriterFactory factory = new WriterFactory();

        [TestMethod()]
        public void TestCMLWriter()
        {
            WriterFactory factory = new WriterFactory();
            WriterFactory.RegisterWriter(typeof(CMLWriter));
            IChemObjectWriter writer = factory.CreateWriter((IChemFormat)CMLFormat.Instance, new StringWriter());
            Assert.IsNotNull(writer);
            Assert.AreEqual(new CMLWriter(new StringWriter()).GetType().Name, writer.GetType().Name);
        }
    }
}
