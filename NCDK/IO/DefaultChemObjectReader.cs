/* Copyright (C) 2002-2007  The Jmol Development Team
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
using NCDK.IO.Listener;
using System;
using System.IO;

namespace NCDK.IO
{
    /// <summary>
    /// Abstract class that ChemObjectReader's can implement to have it
    /// take care of basic stuff, like managing the ReaderListeners.
    /// </summary>
    // @cdk.module io
    // @cdk.githash
    public abstract class DefaultChemObjectReader : ChemObjectIO, ISimpleChemObjectReader
    {
        /// <summary>
        /// An event to be sent to listeners when a frame is read.
        /// </summary>
        private ReaderEvent frameReadEvent = null;

        protected ChemObjectReaderModes mode = ChemObjectReaderModes.Relaxed;
        protected IChemObjectReaderErrorHandler errorHandler = null;

        /* Extra convenience methods */

        /// <summary>
        /// Sends a frame read event to the registered ReaderListeners.
        /// </summary>
        protected void FireFrameRead()
        {
            foreach (var listener in Listeners)
            {
                if (listener is IReaderListener)
                {
                    // Lazily create the event:
                    if (frameReadEvent == null)
                    {
                        frameReadEvent = new ReaderEvent(this);
                    }
                    ((IReaderListener)listener).FrameRead(frameReadEvent);
                }
            }
        }

        public ChemObjectReaderModes ReaderMode
        {
            set
            {
                this.mode = value;
            }
        }

        public IChemObjectReaderErrorHandler ErrorHandler
        {
            set
            {
                this.errorHandler = value;
            }
        }

        public void HandleError(string message)
        {
            if (this.errorHandler != null) this.errorHandler.HandleError(message);
            if (this.mode == ChemObjectReaderModes.Strict) throw new CDKException(message);
        }

        public void HandleError(string message, Exception exception)
        {
            if (this.errorHandler != null) this.errorHandler.HandleError(message, exception);
            if (this.mode == ChemObjectReaderModes.Strict)
            {
                throw new CDKException(message, exception);
            }
        }

        public void HandleError(string message, int row, int colStart, int colEnd)
        {
            if (this.errorHandler != null) this.errorHandler.HandleError(message, row, colStart, colEnd);
            if (this.mode == ChemObjectReaderModes.Strict) throw new CDKException(message);
        }

        public void HandleError(string message, int row, int colStart, int colEnd, Exception exception)
        {
            if (this.errorHandler != null) this.errorHandler.HandleError(message, row, colStart, colEnd, exception);
            if (this.mode == ChemObjectReaderModes.Strict)
            {
                throw new CDKException(message, exception);
            }
        }

        public abstract T Read<T>(T obj) where T : IChemObject;
        public abstract void SetReader(TextReader reader);
        public abstract void SetReader(Stream reader);
    }
}
