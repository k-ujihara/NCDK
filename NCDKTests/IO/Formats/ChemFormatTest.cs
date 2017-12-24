/* Copyright (C) 2008  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.Tools;
using System;

namespace NCDK.IO.Formats
{
    // @cdk.module test-ioformats
    [TestClass()]
    abstract public class ChemFormatTest : ResourceFormatTest
    {
        private IChemFormat chemFormat;

        public void SetChemFormat(IChemFormat format)
        {
            base.SetResourceFormat(format);
            this.chemFormat = format;
        }

        [TestMethod()]
        public void TestChemFormatSet()
        {
            Assert.IsNotNull(chemFormat, $"You must use {nameof(SetChemFormat)}() to set the IChemFormat object.");
        }

        [TestMethod(), Ignore()] // Test cannot be run because it causes a circular dependency cycle
        public void TestGetReaderClassName()
        {
            // two valid output options: NULL and non-zero, existing class
            if (chemFormat.ReaderClassName != null)
            {
                string readerClass = chemFormat.ReaderClassName;
                Assert.AreNotSame(0, readerClass.Length, "Reader Class name string must be of non-zero length");
                Type reader = chemFormat.GetType().Assembly.GetType(readerClass);
                Assert.IsNotNull(reader);
            }
        }

        [TestMethod(), Ignore()] // Test cannot be run because it causes a circular dependency cycle
        public void TestGetWriterClassName()
        {
            // two valid output options: NULL and non-zero, existing class
            if (chemFormat.WriterClassName != null)
            {
                string writerClass = chemFormat.WriterClassName;
                Assert.AreNotSame(0, writerClass.Length, "Writer Class name string must be of non-zero length");
                Type writer = chemFormat.GetType().Assembly.GetType(writerClass);
                Assert.IsNotNull(writer);
            }
        }

        [TestMethod()]
        public void TestGetSupportedDataFeatures()
        {
            int supported = chemFormat.SupportedDataFeatures;
            Assert.IsTrue(supported >= DataFeatures.None);
            Assert.IsTrue(supported <= 1 << 13); // 13 features, so: all summed <= 1<<13
        }

        [TestMethod()]
        public void TestGetRequiredDataFeatures()
        {
            int required = chemFormat.RequiredDataFeatures;
            Assert.IsTrue(required >= DataFeatures.None);
            Assert.IsTrue(required <= 1 << 13); // 13 features, so: all summed <= 1<<13

            // test that the required features is a subset of the supported features
            int supported = chemFormat.SupportedDataFeatures;
            Assert.IsTrue(supported - required >= 0);
        }
    }
}
