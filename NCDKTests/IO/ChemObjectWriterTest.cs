/* Copyright (C) 2009  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@slists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License as published by the Free
 * Software Foundation; either version 2.1 of the License, or (at your option)
 * any later version. All we ask is that proper credit is given for our work,
 * which includes - but is not limited to - adding the above copyright notice to
 * the beginning of your source code files, and to any copyright notice that you
 * may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * Any WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
 * details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software Foundation, Inc.,
 * 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Silent;
using System;
using System.IO;

namespace NCDK.IO
{
    /// <summary>
    /// TestCase for <see cref="IChemObjectWriter"/> implementations.
    /// </summary>
    // @cdk.module test-io
    [TestClass()]
    public abstract class ChemObjectWriterTest : ChemObjectIOTest
    {
        protected IChemObjectWriter CreateChemObjectWriter(TextWriter writer)
        {
            return (IChemObjectWriter)ChemObjectIOToTestType.GetConstructor(new Type[] { typeof(TextWriter) }).Invoke(new object[] { new StringWriter() });
        }

        protected IChemObjectWriter CreateChemObjectWriter(Stream stream)
        {
            return (IChemObjectWriter)CreateChemObjectIO(stream);
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
                    chemObjectIOToTest = CreateChemObjectWriter(new StringWriter());
                }
                return chemObjectIOToTest;
            }
        }

        private static IChemObject[] allChemObjectsTypes = {
            new ChemFile(), new ChemModel(), new Reaction(),
            new ChemObjectSet<IAtomContainer>(), new AtomContainer(), };

        /// <summary>
        /// Unit tests that iterates over all common objects that can be
        /// serialized and tests that if it is marked as accepted with
        /// <see cref="IChemObjectIO.Accepts(Type)"/>, that it can actually be written too.
        /// </summary>
        [TestMethod()]
        public void TestAcceptsWriteConsistency()
        {
            var io = CreateChemObjectWriter(new StringWriter());
            Assert.IsNotNull(io, "The IChemObjectWriter is not set.");
            foreach (var obj in allChemObjectsTypes)
            {
                if (io.Accepts(obj.GetType()))
                {
                    StringWriter writer = new StringWriter();
                    io = CreateChemObjectWriter(writer);
                    try
                    {
                        io.Write(obj);
                    }
                    catch (CDKException exception)
                    {
                        if (exception.Message.Contains("Only supported"))
                        {
                            Assert.Fail("IChemObject of type " + obj.GetType().Name + " is marked as "
                                    + "accepted, but failed to be written.");
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }
        }

        [TestMethod()]
        public void TestSetWriter_Writer()
        {
            var ctor = ChemObjectIOToTestType.GetConstructor(new Type[] { typeof(TextWriter) });
            if (ctor != null)
            {
                StringWriter testWriter = new StringWriter();
                var io = CreateChemObjectWriter(new StringWriter());
                Assert.IsNotNull(io, "No IChemObjectWriter has been set!");
            }
        }

        [TestMethod()]
        public void TestSetWriter_OutputStream()
        {
            var ctor = ChemObjectIOToTestType.GetConstructor(new Type[] { typeof(Stream) });
            if (ctor != null)
            {
                MemoryStream testStream = new MemoryStream();
                var io = CreateChemObjectWriter(testStream);
                Assert.IsNotNull(io, "No IChemObjectWriter has been set!");
            }
        }
    }
}
