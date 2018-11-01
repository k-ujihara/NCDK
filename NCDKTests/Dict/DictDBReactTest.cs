/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Dict
{
    /// <summary>
    /// Checks the functionality of the dictionary reaction-processes class.
    /// </summary>
    /// <seealso cref="DictionaryDatabase"/>
    // @cdk.module test-dict
    [TestClass()]
    public class DictDBReactTest : CDKTestCase
    {
        [TestMethod()]
        public void TestDictDBReact()
        {
            DictionaryDatabase db = new DictionaryDatabase();
            Assert.IsTrue(db.HasDictionary("reaction-processes"));
        }

        [TestMethod()]
        public void TestCheckUniqueID()
        {
            DictionaryDatabase db = new DictionaryDatabase();
            var dict = db.GetDictionary("reaction-processes");
            var entries = dict.Entries.ToReadOnlyList();
            var idList = new List<string>();
            idList.Add(entries[0].Id);
            for (int i = 1; i < entries.Count; i++)
            {
                //            System.Console.Out.WriteLine(entries[i].Id);
                if (!idList.Contains(entries[i].Id))
                    idList.Add(entries[i].Id);
                else
                    Assert.IsFalse(idList.Contains(entries[i].Id), $"The entry is contained {entries[i]}  two times");
            }
        }
    }
}
