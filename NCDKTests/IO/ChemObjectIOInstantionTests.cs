/* Copyright (C) 2001-2003  Jmol Project
 * Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 *  This library is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public
 *  License as published by the Free Software Foundation; either
 *  version 2.1 of the License, or (at your option) any later version.
 *
 *  This library is distributed in the hope that it will be useful,
 *  but WITHOUT Any WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 *  Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public
 *  License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.IO.Formats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace NCDK.IO
{
    /// <summary>
    /// Tests whether all Reader and Writer classes can be instantiated.
    /// </summary>
    // @cdk.module test-io
    // @author  Egon Willighagen <egonw@sci.kun.nl>
    [TestClass()]
    public class ChemObjectIOInstantionTests : CDKTestCase
    {
        private const string IO_FORMATS_LIST = "NCDK." + "io-formats.set";
        private static List<IChemFormat> formats = null;

        private void LoadFormats()
        {
            if (formats == null)
            {
                formats = new List<IChemFormat>();
                try
                {
                    Debug.WriteLine("Starting loading Formats...");
                    var reader = new StreamReader(ResourceLoader.GetAsStream(typeof(IResourceFormat).Assembly,  IO_FORMATS_LIST));
                    int formatCount = 0;
                    string formatName;

                    while ((formatName = reader.ReadLine()) != null)
                    {
                        // load them one by one
                        formatCount++;
                        try
                        {
                            var cls = typeof(IResourceFormat).Assembly.GetType(formatName);
                            if (cls != null)
                            {
                                IResourceFormat format = (IResourceFormat)cls.GetConstructor(Type.EmptyTypes).Invoke(Array.Empty<object>());
                                if (format is IChemFormat)
                                {
                                    formats.Add((IChemFormat)format);
                                    Trace.TraceInformation("Loaded IChemFormat: " + format.GetType().Name);
                                }
                            }
                        }
                        catch (ArgumentException exception)
                        {
                            Trace.TraceError("Could not find this IResourceFormat: ", formatName);
                            Debug.WriteLine(exception);
                        }
                        catch (IOException exception)
                        {
                            Trace.TraceError("Could not load this IResourceFormat: ", formatName);
                            Debug.WriteLine(exception);
                        }
                    }
                    Trace.TraceInformation("Number of loaded formats used in detection: ", formatCount);
                }
                catch (Exception exception)
                {
                    Trace.TraceError("Could not load this io format list: ", IO_FORMATS_LIST);
                    Debug.WriteLine(exception);
                }
            }
        }

        [TestMethod()]
        public void TestInstantion()
        {
            LoadFormats();

            foreach (var format in formats)
            {
                if (format.ReaderClassName != null)
                {
                    TryToInstantiate(format.ReaderClassName);
                }
                if (format.WriterClassName != null)
                {
                    TryToInstantiate(format.WriterClassName);
                }
            }
        }

        private void TryToInstantiate(string className)
        {
            try
            {
                // make a new instance of this class
                Type type = typeof(ChemObjectIO).Assembly.GetType(className);
                if (type == null)
                    throw new ArgumentException();
                ConstructorInfo ctor;
                object instance = null;
                if ((ctor = type.GetConstructor(new[] { typeof(Stream) })) != null)
                    instance = ctor.Invoke(new[] { new MemoryStream() });
                else if ((ctor = type.GetConstructor(new[] { typeof(TextReader) })) != null)
                    instance = ctor.Invoke(new[] { new StringReader("") });
                else if ((ctor = type.GetConstructor(new[] { typeof(TextWriter) })) != null)
                    instance = ctor.Invoke(new[] { new StringWriter() });
                Assert.IsNotNull(instance);
                Assert.AreEqual(className, instance.GetType().FullName);
            }
            catch (ArgumentException)
            {
                Debug.WriteLine($"Could not find this class: {className}");
                // but that's not error, it can mean that it is a Jmol based IO class, and no Jmol is in the classpath
            }
            catch (IOException exception)
            {
                Debug.WriteLine(exception);
                Assert.Fail("Could not instantiate this class: " + className);
            }
        }
    }
}
