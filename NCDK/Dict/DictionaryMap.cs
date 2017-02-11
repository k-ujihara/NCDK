/* Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
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
 *
 */
using NCDK.Util.Xml;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace NCDK.Dict
{
    /// <summary>
    /// Dictionary with entries.
    /// <para>
    /// FIXME: this should be replaced by an uptodate Dictionary Schema DOM type thing.
    /// </para>
    /// </summary>
	// @author     Egon Willighagen
    // @cdk.githash
	// @cdk.created    2003-08-23
	// @cdk.keyword dictionary
	// @cdk.module dict
    public class DictionaryMap
    {
        private IDictionary<string, Entry> entries;
        private XNamespace ownNS = null;

        public DictionaryMap()
        {
            entries = new Dictionary<string, Entry>();
        }

        public static DictionaryMap Unmarshal(TextReader reader)
        {
            DictionaryHandler handler = new DictionaryHandler();
            var r = new XReader();
            r.Handler = handler;
            var doc = XDocument.Load(reader);
            r.Read(doc);
            return handler.Dictionary;
        }

        public void AddEntry(Entry entry)
        {
            entries.Add(entry.Id.ToLowerInvariant(), entry);
        }

        public IEnumerable<Entry> GetEntries()
        {
            return entries.Values;
        }

        public bool HasEntry(string identifier)
        {
            return entries.ContainsKey(identifier);
        }

        public Entry GetEntry(string identifier)
        {
            return entries[identifier];
        }

        public int Count => entries.Count;

        public XNamespace NS
        {
            get { return ownNS; }
            set { ownNS = value; }
        }
    }
}

