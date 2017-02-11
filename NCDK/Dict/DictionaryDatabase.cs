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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace NCDK.Dict
{
    /**
	 * Database of dictionaries listing entries with compounds, fragments
	 * and entities.
	 *
	 * @author     Egon Willighagen
	 * @cdk.githash
	 * @cdk.created    2003-04-06
	 * @cdk.keyword    dictionary
	 * @cdk.module     dict
	 */
    public class DictionaryDatabase
    {
        public const string DICTREFPROPERTYNAME = "NCDK.Dict";

        private string[] dictionaryNames = {"chemical", "elements", "descriptor-algorithms",
                "reaction-processes"                        };
        private string[] dictionaryTypes = { "xml", "owl", "owl", "owl_React" };

        private IDictionary<string, DictionaryMap> dictionaries;

        public DictionaryDatabase()
        {
            // read dictionaries distributed with CDK
            dictionaries = new Dictionary<string, DictionaryMap>();
            for (int i = 0; i < dictionaryNames.Length; i++)
            {
                string name = dictionaryNames[i];
                string type = dictionaryTypes[i];
                DictionaryMap dictionary = ReadDictionary("NCDK.Dict.Data." + name, type);
                if (dictionary != null)
                {
                    dictionaries.Add(name.ToLowerInvariant(), dictionary);
                    Debug.WriteLine("Read dictionary: ", name);
                }
            }
        }

        private DictionaryMap ReadDictionary(string databaseLocator, string type)
        {
            DictionaryMap dictionary;
            // to distinguish between OWL: QSAR & REACT
            if (type.Contains("_React"))
                databaseLocator += "." + type.Substring(0, type.Length - 6);
            else
                databaseLocator += "." + type;
            Trace.TraceInformation("Reading dictionary from ", databaseLocator);
            try
            {
                var reader = new StreamReader(this.GetType().Assembly.GetManifestResourceStream(databaseLocator));
                if (type.Equals("owl"))
                {
                    dictionary = OWLFile.Unmarshal(reader);
                }
                else if (type.Equals("owl_React"))
                {
                    dictionary = OWLReact.Unmarshal(reader);
                }
                else
                { // assume XML using Castor
                    dictionary = DictionaryMap.Unmarshal(reader);
                }
            }
            catch (Exception exception)
            {
                dictionary = null;
                Trace.TraceError("Could not read dictionary ", databaseLocator);
                Debug.WriteLine(exception);
            }
            return dictionary;
        }

        /**
		 * Reads a custom dictionary into the database.
		 * @param reader The reader from which the dictionary data will be read
		 * @param name The name of the dictionary
		 */
        public void ReadDictionary(TextReader reader, string name)
        {
            name = name.ToLowerInvariant();
            Debug.WriteLine("Reading dictionary: ", name);
            if (!dictionaries.ContainsKey(name))
            {
                try
                {
                    DictionaryMap dictionary = DictionaryMap.Unmarshal(reader);
                    dictionaries.Add(name, dictionary);
                    Debug.WriteLine("  ... loaded and stored");
                }
                catch (Exception exception)
                {
                    Trace.TraceError("Could not read dictionary: ", name);
                    Debug.WriteLine(exception);
                }
            }
            else
            {
                Trace.TraceError("Dictionary already loaded: ", name);
            }
        }

        /**
		 * Returns a string[] with the names of the known dictionaries.
		 * @return The names of the dictionaries
		 */
        public string[] GetDictionaryNames()
        {
            return dictionaryNames;
        }

        public DictionaryMap GetDictionary(string dictionaryName)
        {
            return dictionaries[dictionaryName];
        }

        /**
		 * Returns a string[] with the id's of all entries in the specified database.
		 * @return The entry names for the specified dictionary
		 * @param dictionaryName The name of the dictionary
		 */
        public IEnumerable<string> GetDictionaryEntries(string dictionaryName)
        {
            DictionaryMap dictionary = GetDictionary(dictionaryName);
            if (dictionary == null)
            {
                Trace.TraceError("Cannot find requested dictionary");
            }
            else
            {
                // FIXME: dummy method that needs an implementation
                var entries = dictionary.GetEntries();
                foreach (var entry in entries)
                    yield return entry.Label;
            }
            yield break;
        }

        public IEnumerable<Entry> GetDictionaryEntry(string dictionaryName)
        {
            DictionaryMap dictionary = dictionaries[dictionaryName];
            return dictionary.GetEntries();
        }

        /**
		 * Returns true if the database contains the dictionary.
		 */
        public bool HasDictionary(string name)
        {
            return dictionaries.ContainsKey(name.ToLowerInvariant());
        }

        public IEnumerable<string> ListDictionaries()
        {
            return dictionaries.Keys;
        }

        /**
		 * Returns true if the given dictionary contains the given
		 * entry.
		 */
        public bool HasEntry(string dictName, string entryID)
        {
            if (HasDictionary(dictName))
            {
                DictionaryMap dictionary = dictionaries[dictName];
                return dictionary.HasEntry(entryID.ToLowerInvariant());
            }
            else
            {
                return false;
            }
        }
    }
}
