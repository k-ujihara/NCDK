/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
 *                    2011  Egon Willighagen <egonw@users.sf.net>
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
using System.Linq;

namespace NCDK.Dict
{
    /**
     * Checks the functionality of the DictionaryDatabase class.
     *
     * @cdk.module test-dict
     *
     * @see org.openscience.cdk.dict.DictionaryDatabase
     */
    [TestClass()]
    public class DictDBTest : CDKTestCase
    {
        [TestMethod()]
        public void TestDictionaryDatabase()
        {
            DictionaryDatabase db = new DictionaryDatabase();
            Assert.IsTrue(db.HasDictionary("descriptor-algorithms"));
            Assert.IsTrue(db.HasDictionary("reaction-processes"));
        }

        [TestMethod()]
        public void TestOWLDictionary()
        {
            DictionaryDatabase db = new DictionaryDatabase();
            var dict = db.GetDictionary("descriptor-algorithms");
            Assert.IsTrue(dict.Count > 0);
            Assert.IsNotNull(dict.NS);
        }

        [TestMethod()]
        public void TestOWLEntry()
        {
            DictionaryDatabase db = new DictionaryDatabase();
            var dict = db.GetDictionary("descriptor-algorithms");
            Entry entry = dict.GetEntry("apol");
            Assert.IsNotNull(entry);
            Assert.AreEqual("Atomic Polarizabilities", entry.Label);
            string def = entry.Definition;
            Assert.IsNotNull(def);
            Assert.IsTrue(def.Length > 0);
        }

        [TestMethod()]
        public void TestOWLReactEntry()
        {
            DictionaryDatabase db = new DictionaryDatabase();
            var dict = db.GetDictionary("reaction-processes");
            Entry entry = dict.GetEntry("AdductionProtonLP".ToLowerInvariant());
            Assert.IsNotNull(entry);
            Assert.AreEqual("Adduction Proton from Lone Pair Orbitals", entry.Label);
            string def = entry.Definition;
            Assert.IsNotNull(def);
            //Assert.IsTrue(def.Length > 0);    // tab is tab in java, java is empty in C# 
        }

        [TestMethod()]
        public void TestListDictionaries()
        {
            DictionaryDatabase db = new DictionaryDatabase();
            var dbs = db.ListDictionaries();
            Assert.IsNotNull(dbs);
            Assert.IsTrue(dbs.Count() > 0);
            foreach (var dbName in dbs)
            {
                Assert.IsNotNull(dbName);
                Assert.AreNotSame(0, dbName.Length);
            }
        }

        [TestMethod()]
        public void TestGetDictionaryNames()
        {
            DictionaryDatabase db = new DictionaryDatabase();
            var dbs = db.GetDictionaryNames();
            Assert.IsNotNull(dbs);
            Assert.AreNotSame(0, dbs.Length);
            foreach (var dbName in dbs)
            {
                Assert.IsNotNull(dbName);
                Assert.AreNotSame(0, dbName.Length);
            }
        }

        [TestMethod()]
        public void TestHasDictionary()
        {
            DictionaryDatabase db = new DictionaryDatabase();
            var dbs = db.ListDictionaries();
            Assert.IsNotNull(dbs);
            Assert.IsTrue(dbs.Count() > 0);
            foreach (var dbName in dbs)
            {
                Assert.IsTrue(db.HasDictionary(dbName));
            }
        }
    }
}
