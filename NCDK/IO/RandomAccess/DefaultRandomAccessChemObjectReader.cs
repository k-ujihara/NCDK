/* Copyright (C) 2003-2007  The Jmol Development Team
 *                    2009  Egon Willighagen <egonw@users.sf.net>
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

using NCDK.IO.Listener;
using NCDK.IO.Setting;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NCDK.IO.RandomAccess
{
    /// <summary>
    /// Abstract class for random readings.
    /// </summary>
    // @cdk.module  io
    // @cdk.githash
    public abstract class DefaultRandomAccessChemObjectReader : IList<IChemObject>
    {
        protected ChemObjectReaderMode ReaderMode { get; set; } = ChemObjectReaderMode.Relaxed;

        /// <summary>
        /// Holder of reader event listeners.
        /// </summary>
        private readonly List<IChemObjectIOListener> listenerList = new List<IChemObjectIOListener>();

        public virtual void AddChemObjectIOListener(IChemObjectIOListener listener)
        {
            listenerList.Add(listener);
        }

        public virtual void RemoveChemObjectIOListener(IChemObjectIOListener listener)
        {
            listenerList.Remove(listener);
        }

        /* Extra convenience methods */

        protected void FireIOSettingQuestion(IOSetting setting)
        {
            for (int i = 0; i < listenerList.Count; ++i)
            {
                IChemObjectIOListener listener = listenerList[i];
                listener.ProcessIOSettingQuestion(setting);
            }
        }

        public IOSetting[] IOSettings { get; } = Array.Empty<IOSetting>();
        public abstract int Count { get; }
        public abstract bool IsReadOnly { get; }

        public abstract IChemObject this[int index] { get; set; }

        public void SetReaderMode(ChemObjectReaderMode mode)
        {
            this.ReaderMode = mode;
        }

        public abstract int IndexOf(IChemObject item);
        public abstract void Insert(int index, IChemObject item);
        public abstract void RemoveAt(int index);
        public abstract void Add(IChemObject item);
        public abstract void Clear();
        public abstract bool Contains(IChemObject item);
        public abstract void CopyTo(IChemObject[] array, int arrayIndex);
        public abstract bool Remove(IChemObject item);
        public abstract IEnumerator<IChemObject> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

