/* Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
 *                    2014  Mark B Vine (orcid:0000-0002-7794-0426)
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
using NCDK.IO.Formats;
using NCDK.IO.Setting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace NCDK.IO.Iterator
{
    /// <summary>
    /// Iterating MDL SDF reader. It allows to iterate over all molecules
    /// in the SD file, without reading them into memory first. Suitable
    /// for (very) large SDF files. For parsing the molecules in the
    /// SD file, it uses the <code>MDLV2000Reader</code> or
    /// <code>MDLV3000Reader</code> reader; it does <b>not</b> work
    /// for SDF files with MDL formats prior to the V2000 format.
    ///
    /// <p>Example use:
    /// <code>
    /// File sdfFile = new File("../zinc-structures/ZINC_subset3_3D_charged_wH_maxmin1000.sdf");
    /// IteratingMDLReader reader = new IteratingMDLReader(
    ///   new FileInputStream(sdfFile), Default.ChemObjectBuilder.Instance
    /// );
    /// while (reader.HasNext()) {
    ///   IAtomContainer molecule = (IAtomContainer)reader.Next();
    /// }
    /// </code>
    ///
    // @cdk.module io
    // @cdk.githash
    ///
    // @see org.openscience.cdk.io.MDLV2000Reader
    // @see org.openscience.cdk.io.MDLV3000Reader
    ///
    // @author     Egon Willighagen <egonw@sci.kun.nl>
    // @cdk.created    2003-10-19
    ///
    // @cdk.keyword    file format, MDL molfile
    // @cdk.keyword    file format, SDF
    // @cdk.iooptions
    /// </summary>
    public class IteratingSDFReader : DefaultIteratingChemObjectReader<IAtomContainer>
    {

        private TextReader input;
        private string currentLine;
        private IChemFormat currentFormat;
        private readonly ReaderFactory factory = new ReaderFactory();
        private IChemObjectBuilder builder;
        private BooleanIOSetting forceReadAs3DCoords;

        // if an error is encountered the reader will skip over the error
        private bool skip = false;

        private static readonly string LINE_SEPARATOR = Environment.NewLine;

        // patterns to match
        private static Regex MDL_VERSION = new Regex("[vV](2000|3000)", RegexOptions.Compiled);
        private static Regex M_END = new Regex("M\\s\\sEND", RegexOptions.Compiled);
        private static Regex SDF_RECORD_SEPARATOR = new Regex("\\$\\$\\$\\$", RegexOptions.Compiled);
        private static Regex SDF_FIELD_START = new Regex("\\A>\\s", RegexOptions.Compiled);

        // map of MDL formats to their readers
        private readonly IDictionary<IChemFormat, ISimpleChemObjectReader> readerMap = new Dictionary<IChemFormat, ISimpleChemObjectReader>(
                                                                                             5);

        /// <summary>
        /// Constructs a new IteratingMDLReader that can read Molecule from a given Reader.
        ///
        /// <param name="in">The Reader to read from</param>
        /// <param name="builder">The builder</param>
        /// </summary>
        public IteratingSDFReader(TextReader in_, IChemObjectBuilder builder)
            : this(in_, builder, false)
        { }

        /// <summary>
        /// Constructs a new IteratingMDLReader that can read Molecule from a given Stream.
        ///
        /// <param name="in">The Stream to read from</param>
        /// <param name="builder">The builder</param>
        /// </summary>
        public IteratingSDFReader(Stream in_, IChemObjectBuilder builder)
            : this(new StreamReader(in_), builder)
        { }

        /// <summary>
        /// Constructs a new IteratingMDLReader that can read Molecule from a given a
        /// Stream. This constructor allows specification of whether the reader will
        /// skip 'null' molecules. If skip is set to false and a broken/corrupted molecule
        /// is read the iterating reader will stop at the broken molecule. However if
        /// skip is set to true then the reader will keep trying to read more molecules
        /// until the end of the file is reached.
        ///
        /// <param name="in">the <see cref="Stream"/> to read from</param>
        /// <param name="builder">builder to use</param>
        /// <param name="skip">whether to skip null molecules</param>
        /// </summary>
        public IteratingSDFReader(Stream in_, IChemObjectBuilder builder, bool skip)
            : this(new StreamReader(in_), builder, skip)
        { }

        /// <summary>
        /// Constructs a new IteratingMDLReader that can read Molecule from a given a
        /// Reader. This constructor allows specification of whether the reader will
        /// skip 'null' molecules. If skip is set to false and a broken/corrupted molecule
        /// is read the iterating reader will stop at the broken molecule. However if
        /// skip is set to true then the reader will keep trying to read more molecules
        /// until the end of the file is reached.
        ///
        /// <param name="in">the <see cref="Reader"/> to read from</param>
        /// <param name="builder">builder to use</param>
        /// <param name="skip">whether to skip null molecules</param>
        /// </summary>
        public IteratingSDFReader(TextReader in_, IChemObjectBuilder builder, bool skip)
        {
            this.builder = builder;
            SetReader(in_);
            InitIOSettings();
            Skip = skip;
        }

        public override IResourceFormat Format => currentFormat;

        /// <summary>
        ///                Method will return an appropriate reader for the provided format. Each reader is stored
        ///                in a map, if no reader is available for the specified format a new reader is created. The
        ///                {@see ISimpleChemObjectReadr#SetErrorHandler(IChemObjectReaderErrorHandler)} and
        ///                {@see ISimpleChemObjectReadr#SetReaderMode(DefaultIteratingChemObjectReader)}
        ///                methods are set.
        ///
        /// <param name="format">The format to obtain a reader for</param>
        /// <returns>instance of a reader appropriate for the provided format</returns>
        /// </summary>
        private ISimpleChemObjectReader GetReader(IChemFormat format)
        {
            // create a new reader if not mapped
            if (!readerMap.ContainsKey(format))
            {
                ISimpleChemObjectReader reader = factory.CreateReader(format);
                reader.ErrorHandler = this.errorHandler;
                reader.ReaderMode = this.mode;
                if (currentFormat is MDLV2000Format)
                {
                    reader.AddSettings(IOSettings.Settings);
                }
                readerMap[format] = reader;

            }

            return readerMap[format];
        }

        public override IEnumerator<IAtomContainer> GetEnumerator()
        {
            // buffer to store pre-read Mol records in
            StringBuilder buffer = new StringBuilder(10000);

            currentFormat = (IChemFormat)MDLFormat.Instance;

            while ((currentLine = input.ReadLine()) != null)
            {
                // still in a molecule
                buffer.Append(currentLine).Append(LINE_SEPARATOR);

                // do MDL molfile version checking
                var versionMatcher = MDL_VERSION.Match(currentLine);
                if (versionMatcher.Success)
                {
                    currentFormat = "2000".Equals(versionMatcher.Groups[1].Value) ? (IChemFormat)MDLV2000Format.Instance
                            : (IChemFormat)MDLV3000Format.Instance;
                }

                // un-trimmed line has already been stored in buffer
                currentLine = currentLine.Trim();

                if (M_END.IsMatch(currentLine))
                {
                    Debug.WriteLine("MDL file part read: ", buffer);

                    IAtomContainer molecule = null;

                    try
                    {
                        ISimpleChemObjectReader reader = GetReader(currentFormat);
                        using (var byteStream = new MemoryStream(Encoding.UTF8.GetBytes(buffer.ToString())))
                        {
                            reader.SetReader(byteStream);
                            molecule = (IAtomContainer)reader.Read(builder.CreateAtomContainer());
                        }
                    }
                    catch (Exception exception)
                    {
                        if (exception is CDKException | exception is ArgumentException | exception is IOException)
                        {
                            Trace.TraceError("Error while reading next molecule: " + exception.Message);
                            Debug.WriteLine(exception);
                        }
                        else
                            throw;
                    }

                    if (molecule != null)
                    {
                        ReadDataBlockInto(molecule);
                        yield return molecule;
                    }
                    else if (skip)
                    {
                        // null molecule and skip = true, eat up the rest of the entry until '$$$$'
                        string line;
                        while ((line = input.ReadLine()) != null && !SDF_RECORD_SEPARATOR.IsMatch(line))
                        {
                            buffer.Clear();
                        }
                    }
                    else
                    {
                        // don't yield
                    }

                    // empty the buffer
                    buffer.Clear();
                }

                // found SDF record separator ($$$$) without parsing a molecule (separator is detected
                // in ReadDataBlockInto()) the buffer is cleared and the iterator continues reading
                if (currentLine == null)
                    break;
                if (SDF_RECORD_SEPARATOR.IsMatch(currentLine))
                {
                    buffer.Clear();
                }
            }
            yield break;
        }

        private void ReadDataBlockInto(IAtomContainer m)
        {
            string fieldName = null;
            while ((currentLine = input.ReadLine()) != null && !SDF_RECORD_SEPARATOR.IsMatch(currentLine))
            {
                Debug.WriteLine("looking for data header: ", currentLine);
                string str = currentLine;
                if (SDF_FIELD_START.IsMatch(str))
                {
                    fieldName = ExtractFieldName(fieldName, str);
                    str = SkipOtherFieldHeaderLines(str);
                    string data = ExtractFieldData(str);
                    if (fieldName != null)
                    {
                        Trace.TraceInformation("fieldName, data: ", fieldName, ", ", data);
                        m.SetProperty(fieldName, data);
                    }
                }
            }
        }

        /// <summary>
        ///        Indicate whether the reader should skip over SDF records
        ///        that cause problems. If true the reader will fetch the next
        ///        molecule
        /// <param name="skip">ignore error molecules continue reading</param>
        /// </summary>
        public bool Skip
        {
            set
            {
                this.skip = value;
            }
        }

        private string ExtractFieldData(string str)
        {
            StringBuilder data = new StringBuilder();
            while (str.Trim().Length > 0)
            {
                Debug.WriteLine("data line: ", currentLine);
                if (data.Length > 0)
                {
                    str = Environment.NewLine + str;
                }
                data.Append(str);
                currentLine = input.ReadLine();
                str = currentLine.Trim();
            }
            return data.ToString();
        }

        private string SkipOtherFieldHeaderLines(string str)
        {
            while (str.StartsWith("> "))
            {
                Debug.WriteLine("data header line: ", currentLine);
                currentLine = input.ReadLine();
                str = currentLine;
            }
            return str;
        }

        private string ExtractFieldName(string fieldName, string str)
        {
            int index = str.IndexOf('<');
            if (index != -1)
            {
                int index2 = str.Substring(index).IndexOf('>');
                if (index2 != -1)
                {
                    fieldName = str.Substring(index + 1, index2 - 1);
                }
            }
            return fieldName;
        }

        public override void Close()
        {
            input.Close();
        }

        public override void SetReader(TextReader reader)
        {
            input = reader;
        }

        public override void SetReader(Stream reader)
        {
            SetReader(new StreamReader(reader));
        }

        private void InitIOSettings()
        {
            forceReadAs3DCoords = new BooleanIOSetting("ForceReadAs3DCoordinates", IOSetting.Importance.Low,
                    "Should coordinates always be read as 3D?", "false");
            Add(forceReadAs3DCoords);
        }

        public void CustomizeJob()
        {
            FireIOSettingQuestion(forceReadAs3DCoords);
        }

        public override void Dispose()
        {
            Close();
        }
    }
}
