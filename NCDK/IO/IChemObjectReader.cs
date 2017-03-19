/* Copyright (C) 2000-2007,2010  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.IO;
using System;
using System.IO;
using System.Linq;
using NCDK.Numerics;

namespace NCDK.IO
{
    public enum ChemObjectReaderModes
    {
        /// <summary>Only fail on serious format problems</summary>
        Relaxed,
        /// <summary>Fail on any format problem</summary>
        Strict
    }

    /// <summary>
    /// This interface specifies the common functionality all IO readers should provide.
    ///
    /// IO readers should not implement this interface directly, but rather implement
    /// one of its child interfaces: <see cref="ISimpleChemObjectReader"/> or <see cref="Iterator.IIteratingChemObjectReader{T}"/>.
    /// These sub-interfaces specify the information access methods:
    /// a simple Read() method for the <see cref="ISimpleChemObjectReader"/> and
    /// more advanced iterator based access for the <see cref="Iterator.IIteratingChemObjectReader{T}"/> (suitable for large files)
    /// </summary>
    /// <seealso cref="ISimpleChemObjectReader"/>
    /// <seealso cref="Iterator.IIteratingChemObjectReader{T}"/>
    // @cdk.module io
    // @cdk.githash
    // @author     Egon Willighagen <egonw@users.sf.net>
    public interface IChemObjectReader : IChemObjectIO
    {
        /// <summary>
        /// Sets the Reader from which this ChemObjectReader should read
        /// the contents.
        /// </summary>
         void SetReader(TextReader reader);

        /// <summary>
        /// Sets the Stream from which this ChemObjectReader should read
        /// the contents.
        /// </summary>
         void SetReader(Stream reader);

        /// <summary>
        /// The reader mode. If <see cref="ChemObjectReaderModes.Strict"/>, then the reader will fail on
        /// any problem in the format of the read file, instead of trying to
        /// recover from that.
        /// </summary>
        ChemObjectReaderModes ReaderMode { set; }

        /// <summary>
        /// An error handler that is sent events when file format issues occur.
        /// </summary>
        IChemObjectReaderErrorHandler ErrorHandler { set; }

        /// <summary>
        /// Redirects an error message to the <see cref="IChemObjectReaderErrorHandler"/>.
        /// Throws an <see cref="CDKException"/> when in Strict <see cref="ChemObjectReaderModes"/>.
        /// </summary>
        /// <param name="message">the error message.</param>
        void HandleError(string message);

        /// <summary>
        /// Redirects an error message to the <see cref="IChemObjectReaderErrorHandler"/>.
        /// Throws an <see cref="CDKException"/> when in Strict <see cref="ChemObjectReaderModes"/>.
        /// </summary>
        /// <param name="message">the error message.</param>
        /// <param name="exception">the corresponding <see cref="Exception"/>.</param>
        void HandleError(string message, Exception exception);

        /// <summary>
        /// Redirects an error message to the <see cref="IChemObjectReaderErrorHandler"/>.
        /// Throws an <see cref="CDKException"/> when in Strict <see cref="ChemObjectReaderModes"/>.
        /// </summary>
        /// <param name="message">the error message.</param>
        /// <param name="row">Row in the file where the error is found.</param>
        /// <param name="colStart">Start column in the file where the error is found.</param>
        /// <param name="colEnd">End column in the file where the error is found.</param>
        void HandleError(string message, int row, int colStart, int colEnd);

        /// <summary>
        /// Redirects an error message to the <see cref="IChemObjectReaderErrorHandler"/>.
        /// Throws an <see cref="CDKException"/> when in <see cref="ChemObjectReaderModes.Strict"/>.
        /// </summary>
        /// <param name="message">the error message.</param>
        /// <param name="exception">the corresponding <see cref="Exception"/>.</param>
        /// <param name="row">Row in the file where the error is found.</param>
        /// <param name="colStart">Start column in the file where the error is found.</param>
        /// <param name="colEnd">End column in the file where the error is found.</param>
        void HandleError(string message, int row, int colStart, int colEnd, Exception exception);
    }
}
