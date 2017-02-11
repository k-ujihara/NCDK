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

    /**
     * This interface specifies the common functionality all IO readers should provide.
     *
     * IO readers should not implement this interface directly, but rather implement
     * one of its child interfaces: {@link ISimpleChemObjectReader} or {@link IIteratingChemObjectReader}.
     * These sub-interfaces specify the information access methods:
     * a simple Read() method for the {@link ISimpleChemObjectReader} and
     * more advanced iterator based access for the {@link IIteratingChemObjectReader} (suitable for large files)
     *
     * @cdk.module io
     * @cdk.githash
     *
     * @author     Egon Willighagen <egonw@users.sf.net>
     * @see ISimpleChemObjectReader
     * @see IIteratingChemObjectReader
     **/
    public interface IChemObjectReader : IChemObjectIO
    {
        /**
         * Sets the Reader from which this ChemObjectReader should read
         * the contents.
         */
         void SetReader(TextReader reader);

        /**
         * Sets the Stream from which this ChemObjectReader should read
         * the contents.
         */
         void SetReader(Stream reader);

        /**
         * Sets the reader mode. If ChemObjectReaderModes.Strict, then the reader will fail on
         * any problem in the format of the read file, instead of trying to
         * recover from that.
         *
         * @param mode
         */
         ChemObjectReaderModes ReaderMode { set; }

        /**
         * Sets an error handler that is sent events when file format issues occur.
         *
         * @param handler {@link IChemObjectReaderErrorHandler} to send error
         *                messages to.
         */
        IChemObjectReaderErrorHandler ErrorHandler { set; }

        /**
         * Redirects an error message to the {@link IChemObjectReaderErrorHandler}.
         * Throws an {@link CDKException} when in Strict {@link Mode}.
         *
         * @param message the error message.
         */
         void HandleError(string message);

        /**
         * Redirects an error message to the {@link IChemObjectReaderErrorHandler}.
         * Throws an {@link CDKException} when in Strict {@link Mode}.
         *
         * @param message  the error message.
         * @param exception the corresponding {@link Exception}.
         */
         void HandleError(string message, Exception exception);

        /**
         * Redirects an error message to the {@link IChemObjectReaderErrorHandler}.
         * Throws an {@link CDKException} when in Strict {@link Mode}.
         *
         * @param message  the error message.
         * @param row      Row in the file where the error is found.
         * @param colStart Start column in the file where the error is found.
         * @param colEnd   End column in the file where the error is found.
         */
         void HandleError(string message, int row, int colStart, int colEnd);

        /**
         * Redirects an error message to the {@link IChemObjectReaderErrorHandler}.
         * Throws an {@link CDKException} when in Strict {@link Mode}.
         *
         * @param message  the error message.
         * @param exception the corresponding {@link Exception}.
         * @param row       Row in the file where the error is found.
         * @param colStart Start column in the file where the error is found.
         * @param colEnd   End column in the file where the error is found.
         */
         void HandleError(string message, int row, int colStart, int colEnd, Exception exception);
    }
}
