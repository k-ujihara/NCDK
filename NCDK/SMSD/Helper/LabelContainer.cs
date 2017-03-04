/*
 *
 * Copyright (C) 2006-2010  Syed Asad Rahman <asad@ebi.ac.uk>
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
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
*/
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NCDK.SMSD.Helper
{
    /// <summary>
    /// Class that handles atoms and assignes an integer lable to them.
    // @cdk.module smsd
    // @cdk.githash
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    /// </summary>
    public class LabelContainer
    {
        private List<string> labelMap = null;
        private static LabelContainer instance = null;

        protected internal LabelContainer()
        {
            // Console.Error.WriteLine("List Initialized");
            labelMap = new List<string>();
            labelMap.Add("X");
            labelMap.Add("R");
        }

        /// <summary>
        /// Create ids from atom labels
        /// <returns>instance of this object</returns>
        /// </summary>
        public static LabelContainer Instance
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                if (instance == null)
                {
                    instance = new LabelContainer();
                }
                return instance;
            }
        }

        /// <summary>
        /// Add label if its not present
        /// <param name="label">/// </summary></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Add(string label)
        {
            if (!labelMap.Contains(label))
            {
                labelMap.Add(label);
            }
        }

        /// <summary>
        /// Returns label ID
        /// <param name="label">/// <returns>labelID</param></returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public int GetLabelID(string label)
        {
            Add(label);
            return labelMap.IndexOf(label);
        }

        /// <summary>
        /// Returns Label of a given ID
        /// <param name="labelID">/// <returns>label</param></returns>
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public string GetLabel(int labelID)
        {
            return labelMap[labelID];
        }

        /// <summary>
        /// Returns label count
        /// <returns>size of the labels</returns>
        /// </summary>
        public int Count => labelMap.Count;
    }
}
