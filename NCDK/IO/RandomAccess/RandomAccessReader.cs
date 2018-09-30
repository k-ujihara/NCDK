/* Copyright (C) 2005-2008   Nina Jeliazkova <nina@acad.bg>
 *                    2009   Egon Willighagen <egonw@users.sf.net>
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

using NCDK.Common.Collections;
using NCDK.IO.Listener;
using NCDK.IO.Setting;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace NCDK.IO.RandomAccess
{
    /// <summary>
    /// Random access to text files of compounds.
    /// Reads the file as a text and builds an index file, if the index file doesn't already exist.
    /// The index stores offset, length and a third field reserved for future use.
    /// Subsequent access for a record N uses this index to seek the record and return the molecule.
    /// Useful for very big files.
    /// </summary>
    // @author     Nina Jeliazkova <nina@acad.bg>
    // @cdk.module io
    // @cdk.githash
    public abstract class RandomAccessReader
        : DefaultRandomAccessChemObjectReader, IDisposable
    {
        private Stream raFile;
        private readonly string filename;
        private int indexVersion = 1;
        // index[record][0] - record offset in file index[record][1] - record length
        // index[record][2] - number of atoms (if available)
        private long[][] index = null;
        private int records;
        private int currentRecord = 0;
        private byte[] b;
        private bool indexCreated = false;

        /// <summary>
        /// Reads the file and builds an index file, if the index file doesn't already exist.
        /// </summary>
        /// <param name="file">the file object containing the molecules to be indexed</param>
        /// <param name="builder">a chem object builder</param>
        /// <exception cref="System.IO.IOException">if there is an error during reading</exception>
        protected RandomAccessReader(string file, IChemObjectBuilder builder)
            : this(file, builder, null)
        { }

        /// <summary>
        /// Reads the file and builds an index file, if the index file doesn't already exist.
        /// </summary>
        /// <param name="file">file the file object containing the molecules to be indexed</param>
        /// <param name="builder">builder a chem object builder</param>
        /// <param name="listener">listen for read event</param>
        /// <exception cref="System.IO.IOException">if there is an error during reading</exception>
        protected RandomAccessReader(string file, IChemObjectBuilder builder, IReaderListener listener)
            : base()
        {
            this.filename = Path.GetFullPath(file);
            this.Builder = builder;
            if (listener != null) AddChemObjectIOListener(listener);
            raFile = new FileStream(filename, FileMode.Open, FileAccess.Read);
            records = 0;
            SetIndexCreated(false);
            IndexTheFile();
        }

        /// <summary>
        /// Returns the object at given record No.
        /// </summary>
        /// <param name="record">Zero-based record number</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IChemObject ReadRecord(int record)
        {
            string buffer = ReadContent(record);
            ChemObjectReader = CreateChemObjectReader(new StringReader(buffer));
            currentRecord = record;
            return ProcessContent();
        }

        /// <summary>
        /// Reads the record text content into a string.
        /// </summary>
        /// <param name="record">The record number</param>
        /// <returns>A string representation of the record</returns>
        /// <exception cref="IOException">if error occurs during reading</exception>
        /// <exception cref="CDKException">if the record number is invalid</exception>
        protected string ReadContent(int record)
        {
            Debug.WriteLine($"Current record {record}");

            if ((record < 0) || (record >= records))
            {
                throw new CDKException($"No such record {record}");
            }

            raFile.Seek(index[record][0], SeekOrigin.Begin);
            int length = (int)index[record][1];
            raFile.Read(b, 0, length);
            return Encoding.UTF8.GetString(b, 0, length);
        }

        /// <summary>
        /// The reader is already set to read the record buffer.
        /// </summary>
        /// <returns>the read IChemObject</returns>
        /// <exception cref="System.IO.IOException">an error occurred whilst reading the file</exception>
        protected virtual IChemObject ProcessContent()
        {
            return ChemObjectReader.Read(Builder.NewChemFile());
        }

        internal static long[][] Resize(long[][] index, int newLength)
        {
            long[][] newIndex = Arrays.CreateJagged<long>(newLength, 3);
            for (int i = 0; i < index.Length; i++)
            {
                newIndex[i][0] = index[i][0];
                newIndex[i][1] = index[i][1];
                newIndex[i][2] = index[i][2];
            }
            return newIndex;
        }

        protected abstract bool IsRecordEnd(string line);

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected void SaveIndex(string file)
        {
            if (records == 0)
            {
                File.Delete(file);
                return;
            }
            using (var o = new StreamWriter(file))
            {
                o.Write(indexVersion.ToString(NumberFormatInfo.InvariantInfo));
                o.Write('\n');
                o.Write(filename);
                o.Write('\n');
                o.Write(raFile.Length.ToString(NumberFormatInfo.InvariantInfo));
                o.Write('\n');
                o.Write(records.ToString(NumberFormatInfo.InvariantInfo));
                o.Write('\n');
                for (int i = 0; i < records; i++)
                {
                    o.Write(index[i][0].ToString(NumberFormatInfo.InvariantInfo));
                    o.Write("\t");
                    o.Write(index[i][1].ToString(NumberFormatInfo.InvariantInfo));
                    o.Write("\t");
                    o.Write(index[i][2].ToString(NumberFormatInfo.InvariantInfo));
                    o.Write("\t");
                }
                o.Write(records.ToString(NumberFormatInfo.InvariantInfo));
                o.Write('\n');
                o.Write(filename);
                o.Write('\n');
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected void LoadIndex(string file)
        {
            using (var ins = new StreamReader(file))
            {
                string version = ins.ReadLine();
                if (!int.TryParse(version, out int iv))
                    throw new Exception($"Invalid index version {version}");
                if (int.Parse(version, NumberFormatInfo.InvariantInfo) != indexVersion)
                    throw new Exception($"Expected index version {indexVersion} instead of {version}");

                string fileIndexed = ins.ReadLine();
                if (!string.Equals(filename, fileIndexed, StringComparison.Ordinal))
                    throw new Exception($"Index for {fileIndexed} found instead of {filename}. Creating new index.");

                string line = ins.ReadLine();
                int fileLength = int.Parse(line, NumberFormatInfo.InvariantInfo);
                if (fileLength != raFile.Length)
                    throw new Exception($"Index for file of size {fileLength} found instead of {raFile.Length}");

                line = ins.ReadLine();
                int indexLength = int.Parse(line, NumberFormatInfo.InvariantInfo);
                if (indexLength <= 0)
                    throw new Exception($"Index of zero length! {Path.GetFullPath(file)}");
                index = Arrays.CreateJagged<long>(indexLength, 3);
                records = 0;
                int maxRecordLength = 0;
                for (int i = 0; i < index.Length; i++)
                {
                    line = ins.ReadLine();
                    string[] result = line.Split('\t');
                    for (int j = 0; j < 3; j++)
                        try
                        {
                            index[i][j] = long.Parse(result[j], NumberFormatInfo.InvariantInfo);
                        }
                        catch (Exception x)
                        {
                            throw new Exception($"Error reading index! {result[j]}", x);
                        }

                    if (maxRecordLength < index[records][1]) maxRecordLength = (int)index[records][1];
                    records++;
                }

                line = ins.ReadLine();
                int indexLength2 = int.Parse(line, NumberFormatInfo.InvariantInfo);
                if (indexLength2 <= 0)
                {
                    throw new Exception("Index of zero length!");
                }
                if (indexLength2 != indexLength)
                {
                    throw new Exception("Wrong index length!");
                }
                line = ins.ReadLine();
                if (!string.Equals(line, filename, StringComparison.Ordinal))
                {
                    throw new Exception($"Index for {line} found instead of {filename}");
                }
                b = new byte[maxRecordLength];
            }
            //FireFrameRead();
        }

        /// <summary>
        /// The index file <see cref="GetIndexFile(string)"/> is loaded, if already exists, or created a new.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected void MakeIndex()
        {
            var indexFile = GetIndexFile(filename);
            if (File.Exists(indexFile))
                try
                {
                    LoadIndex(indexFile);
                    SetIndexCreated(true);
                    return;
                }
                catch (Exception x)
                {
                    Trace.TraceWarning(x.Message);
                }
            indexCreated = false;
            long now = DateTime.Now.Ticks;
            int recordLength = 1000;
            int maxRecords = 1;
            int maxRecordLength = 0;
            maxRecords = (int)raFile.Length / recordLength;
            if (maxRecords == 0) maxRecords = 1;
            index = Arrays.CreateJagged<long>(maxRecords, 3);

            string s = null;
            long start = 0;
            long end = 0;
            raFile.Seek(0, SeekOrigin.Begin);
            records = 0;
            recordLength = 0;
            while ((s = ReadLine(raFile)) != null)
            {
                if (start == -1) start = raFile.Position;
                if (IsRecordEnd(s))
                {
                    if (records >= maxRecords)
                    {
                        index = Resize(index,
                                records + (int)(records + (raFile.Length - records * raFile.Position) / recordLength));
                    }
                    end += 4;
                    index[records][0] = start;
                    index[records][1] = end - start;
                    index[records][2] = -1;
                    if (maxRecordLength < index[records][1])
                        maxRecordLength = (int)index[records][1];
                    records++;
                    recordLength += (int)(end - start);

                    start = raFile.Position;
                }
                else
                {
                    end = raFile.Position;
                }
            }
            b = new byte[maxRecordLength];

            Trace.TraceInformation($"Index created in {(DateTime.Now.Ticks - now) / 10000} ms.");
            try
            {
                SaveIndex(indexFile);
            }
            catch (Exception x)
            {
                Trace.TraceError(x.Message);
            }
        }

        static string ReadLine(Stream stream)
        {
            int c = stream.ReadByte();
            if (c == -1)
                return null;
            var sb = new StringBuilder();
            while (true)
            {
                if (c == '\n')
                {
                    if (sb.Length > 0 && sb[sb.Length - 1] == '\r')
                        sb.Remove(sb.Length - 1, 1);
                    break;
                }
                if (c == -1)
                    break;
                sb.Append((char)c);
                c = stream.ReadByte();
            }
            return sb.ToString();
        }

        /// <summary>
        /// Opens the file index "_cdk.index" in a temporary folder, as specified by <see cref="Path.GetTempPath()"/> property.
        /// </summary>
        /// <param name="filename">the name of the file for which the index was generated</param>
        /// <returns>a file object representing the index file</returns>
        public static string GetIndexFile(string filename)
        {
            var tmpDir = Path.GetTempPath();
            var indexFile = Path.Combine(tmpDir, filename + "_cdk.index");
            return indexFile;
        }

        public abstract ISimpleChemObjectReader CreateChemObjectReader(TextReader reader);

        public bool HasNext()
        {
            return currentRecord < (records - 1);
        }

        public bool HasPrevious()
        {
            return currentRecord > 0;
        }

        public IChemObject First()
        {
            try
            {
                return ReadRecord(0);
            }
            catch (Exception x)
            {
                Trace.TraceError(x.Message);
                return null;
            }
        }

        public IChemObject Last()
        {
            try
            {
                return ReadRecord(records - 1);
            }
            catch (Exception x)
            {
                Trace.TraceError(x.Message);
                return null;
            }
        }

        public IChemObject Next()
        {
            try
            {
                return ReadRecord(currentRecord + 1);
            }
            catch (Exception x)
            {
                Trace.TraceError(x.Message);
                return null;
            }
        }

        public IChemObject Previous()
        {
            try
            {
                return ReadRecord(currentRecord - 1);
            }
            catch (Exception x)
            {
                Trace.TraceError(x.Message);
                return null;
            }
        }

        public override void Add(IChemObject arg0)
        { }

        public virtual int PreviousIndex()
        {
            return currentRecord - 1;
        }

        public virtual int NextIndex()
        {
            return currentRecord + 1;
        }

        public override int Count => records;

        protected ISimpleChemObjectReader ChemObjectReader { get; set; } = null;
        protected IChemObjectBuilder Builder { get; set; }

        public override void AddChemObjectIOListener(IChemObjectIOListener listener)
        {
            AddChemObjectIOListener(listener);
            if (ChemObjectReader != null) ChemObjectReader.Listeners.Add(listener);
        }

        public override void RemoveChemObjectIOListener(IChemObjectIOListener listener)
        {
            base.RemoveChemObjectIOListener(listener);
            if (ChemObjectReader != null) ChemObjectReader.Listeners.Remove(listener);
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        public int GetCurrentRecord()
        {
            return currentRecord;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool IsIndexCreated()
        {
            return indexCreated;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetIndexCreated(bool indexCreated)
        {
            this.indexCreated = indexCreated;
        }

        private void IndexTheFile()
        {
            try
            {
                SetIndexCreated(false);
                MakeIndex();
                currentRecord = 0;
                raFile.Seek(index[0][0], SeekOrigin.Begin);
                SetIndexCreated(true);
            }
            catch (Exception)
            {
                SetIndexCreated(true);
            }
        }

        public override string ToString()
        {
            return filename;
        }

        public void Close()
        {
            Dispose();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    raFile.Close();
                    //TODO
                    //RemoveChemObjectIOListener(listener)
                }

                raFile = null;

                disposedValue = true;
            }
        }

        ~RandomAccessReader()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }

    class RecordReaderEvent : ReaderEvent
    {
        protected int record = 0;

        public RecordReaderEvent(Object source, int record)
                : base(source)
        {
            this.record = record;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int GetRecord()
        {
            return record;
        }
    }
}
