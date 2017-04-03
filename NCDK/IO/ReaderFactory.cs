/* Copyright (C) 2001-2007  Bradley A. Smith <bradley@baysmith.com>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Common.Util;
using NCDK.IO.Formats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace NCDK.IO
{
    /// <summary>
    /// A factory for creating ChemObjectReaders. The type of reader
    /// created is determined from the content of the input. Formats
    /// of GZiped files can be detected too.
    /// </summary>
    /// <example>    
    /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.IO.ReaderFactory_Example.cs"]/*' />
    /// </example>
    // @cdk.module io
    // @cdk.githash
    // @author  Egon Willighagen <egonw@sci.kun.nl>
    // @author  Bradley A. Smith <bradley@baysmith.com>
    public class ReaderFactory
    {
        private FormatFactory formatFactory = null;
        private int headerLength = 8192;

        /// <summary>
        /// Constructs a ReaderFactory which tries to detect the format in the
        /// first 65536 chars.
        /// </summary>
        public ReaderFactory()
            : this(8192)
        { }

        /// <summary>
        /// Constructs a ReaderFactory which tries to detect the format in the
        /// first given number of chars.
        /// </summary>
        /// <param name="headerLength">length of the header in number of chars</param>
        public ReaderFactory(int headerLength)
        {
            formatFactory = new FormatFactory(headerLength);
            this.headerLength = headerLength;
        }

        /// <summary>
        /// Registers a format for detection.
        /// </summary>
        public void RegisterFormat(IChemFormatMatcher format)
        {
            formatFactory.RegisterFormat(format);
        }

        public IList<IChemFormatMatcher> Formats => formatFactory.Formats;

        /// <summary>
        /// Detects the format of the Reader input, and if known, it will return
        /// a CDK Reader to read the format, or null when the reader is not
        /// implemented.
        /// </summary>
        /// <param name="input"></param>
        /// <returns><see langword="null"/> if CDK does not contain a reader for the detected format.</returns>
        /// <seealso cref="CreateReader(TextReader)"/>
        public ISimpleChemObjectReader CreateReader(Stream input)
        {
            IChemFormat format = null;
            ISimpleChemObjectReader reader = null;
            if (input is GZipStream)
            {
                var istreamToRead = new ReadSeekableStream(input, 65536);
                format = formatFactory.GuessFormat(istreamToRead);
                reader = CreateReader(format);
                if (reader != null)
                {
                    try
                    {
                        reader.SetReader(istreamToRead);
                    }
                    catch (CDKException e1)
                    {
                        IOException wrapper = new IOException("Exception while setting the Stream: " + e1.Message, e1);
                        throw wrapper;
                    }
                }
            }
            else
            {
                Stream bistream = input;
                Stream istreamToRead = bistream; // if gzip test fails, then take default
                                                 //                bistream.Mark(5);
                int countRead = 0;
                byte[] abMagic = new byte[4];
                countRead = bistream.Read(abMagic, 0, 4);
                bistream.Seek(0, SeekOrigin.Begin);
                if (countRead == 4)
                {
                    if (abMagic[0] == (byte)0x1F && abMagic[1] == (byte)0x8B)
                    {
                        istreamToRead = new GZipStream(bistream, CompressionMode.Decompress);
                        return CreateReader(istreamToRead);
                    }
                }
                format = formatFactory.GuessFormat(istreamToRead);
                reader = CreateReader(format);
                if (reader != null)
                {
                    try
                    {
                        reader.SetReader(istreamToRead);
                    }
                    catch (CDKException e1)
                    {
                        IOException wrapper = new IOException("Exception while setting the Stream: " + e1.Message, e1);
                        throw wrapper;
                    }
                }
            }
            return reader;
        }

        /// <summary>
        /// Creates a new IChemObjectReader based on the given <see cref="IChemFormat"/>.
        /// </summary>
        /// <seealso cref="CreateReader(Stream)"/>
        public ISimpleChemObjectReader CreateReader(IChemFormat format)
        {
            if (format != null)
            {
                string readerClassName = format.ReaderClassName;
                if (readerClassName != null)
                {
                    try
                    {
                        // make a new instance of this class
                        Type clazz = null;
                        clazz = this.GetType().Assembly.GetType(readerClassName);
                        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                        {
                            clazz = asm.GetType(readerClassName);
                            if (clazz != null)
                                break;
                        }
                        if (clazz == null)
                            clazz = this.GetType().Assembly.GetType(readerClassName, true);
                        return (ISimpleChemObjectReader)clazz.GetConstructor(Type.EmptyTypes).Invoke(Array.Empty<object>());
                    }
                    catch (Exception exception)
                    {
                        Trace.TraceError($"Could not create this ChemObjectReader: {readerClassName}");
                        Debug.WriteLine(exception);
                    }
                }
                else
                {
                    Trace.TraceWarning("ChemFormat is recognized, but no reader is available.");
                }
            }
            else
            {
                Trace.TraceWarning("ChemFormat is not recognized.");
            }
            return null;
        }

        /// <summary>
        /// Detects the format of the Reader input, and if known, it will return
        /// a CDK Reader to read the format. This method is not able to detect the
        /// format of gziped files. Use CreateReader(Stream) instead for such
        /// files.
        /// </summary>
        /// <seealso cref="CreateReader(Stream)"/>
        public ISimpleChemObjectReader CreateReader(TextReader input)
        {
            IChemFormat chemFormat = formatFactory.GuessFormat(input);
            ISimpleChemObjectReader coReader = CreateReader(chemFormat);
            try
            {
                coReader.SetReader(input);
            }
            catch (Exception exception)
            {
                Trace.TraceError("Could not set the Reader source: ", exception.Message);
                Debug.WriteLine(exception);
            }
            return coReader;
        }
    }
}
