/* Copyright (C) 2003-2007  The Jmol Development Team
 *                    2010  Egon Willighagen <egonw@users.sf.net>
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
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace NCDK.IO.Iterator
{
    /// <summary>
    /// Abstract class that IteratingChemObjectReader's can implement to have it
    /// take care of basic stuff, like managing the ReaderListeners.
    ///
    // @cdk.module io
    // @cdk.githash
    /// </summary>
    public abstract class DefaultIteratingChemObjectReader<T> : ChemObjectIO,
            IIteratingChemObjectReader<T> where T : IChemObject
    {
        protected ChemObjectReaderModes mode = ChemObjectReaderModes.Relaxed;
        protected IChemObjectReaderErrorHandler errorHandler = null;

        public override bool Accepts(Type type)
        {
            return false; // it's an iterator, idiot.
        }

        /* Extra convenience methods */

        /// <summary>
        /// File IO generally does not support removing of entries.
        /// </summary>
        public virtual void Remove()
        {
            throw new NotSupportedException();
        }

        public ChemObjectReaderModes ReaderMode
        {
            set
            {
                this.mode = value;
            }
        }

        /// <inheritdoc/>
        public IChemObjectReaderErrorHandler ErrorHandler
        {
            set
            {
                this.errorHandler = value;
            }
        }

        /// <inheritdoc/>
        public void HandleError(string message)
        {
            if (this.errorHandler != null) this.errorHandler.HandleError(message);
            if (this.mode == ChemObjectReaderModes.Strict) throw new CDKException(message);
        }

        /// <inheritdoc/>
        public void HandleError(string message, Exception exception)
        {
            if (this.errorHandler != null) this.errorHandler.HandleError(message, exception);
            if (this.mode == ChemObjectReaderModes.Strict)
            {
                throw new CDKException(message, exception);
            }
        }

        /// <inheritdoc/>
        public void HandleError(string message, int row, int colStart, int colEnd)
        {
            if (this.errorHandler != null) this.errorHandler.HandleError(message, row, colStart, colEnd);
            if (this.mode == ChemObjectReaderModes.Strict) throw new CDKException(message);
        }

        /// <inheritdoc/>
        public void HandleError(string message, int row, int colStart, int colEnd, Exception exception)
        {
            if (this.errorHandler != null) this.errorHandler.HandleError(message, row, colStart, colEnd, exception);
            if (this.mode == ChemObjectReaderModes.Strict)
            {
                throw new CDKException(message, exception);
            }
        }

        public abstract void SetReader(TextReader reader);
        public abstract void SetReader(Stream reader);
        public abstract IEnumerator<T> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
