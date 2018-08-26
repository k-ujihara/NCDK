/* Copyright (C) 2004-2008  Rajarshi Guha <rajarshi.guha@gmail.com>
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.IO.Iterator;
using NCDK.Silent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NCDK.Pharmacophore
{
    // @cdk.module test-pcore
    [TestClass()]
    public class PharmacophoreUtilityTest
    {
        public static ConformerContainer conformers = null;

        static PharmacophoreUtilityTest()
        {
            string filename = "NCDK.Data.MDL.pcoretest1.sdf";
            Stream ins = ResourceLoader.GetAsStream(filename);
            IEnumerableMDLConformerReader reader = new IEnumerableMDLConformerReader(ins, ChemObjectBuilder.Instance);
            conformers = reader.FirstOrDefault();
        }

        [TestMethod()]
        public void TestReadPcoreDef()
        {
            string filename = "NCDK.Data.PCore.pcore.xml";
            Stream ins = ResourceLoader.GetAsStream(filename);
            var defs = PharmacophoreUtils.ReadPharmacophoreDefinitions(ins);

            Assert.AreEqual(2, defs.Count);

            var def1 = defs[0];
            Assert.AreEqual(4, def1.Atoms.Count);
            Assert.AreEqual(2, def1.Bonds.Count);
            Assert.AreEqual("An imaginary pharmacophore definition", def1.GetProperty<string>("description"));
            Assert.AreEqual("Imaginary", def1.Title);

            var def2 = defs[1];
            Assert.AreEqual(3, def2.Atoms.Count);
            Assert.AreEqual(3, def2.Bonds.Count);
            Assert.IsNull(def2.Title);

            string[] ids = { "Aromatic", "Hydroxyl", "BasicAmine" };
            foreach (var atom in def2.Atoms)
            {
                string sym = atom.Symbol;
                bool found = false;
                foreach (var s in ids)
                {
                    if (sym.Equals(s))
                    {
                        found = true;
                        break;
                    }
                }
                Assert.IsTrue(found, "'" + sym + "' in pcore.xml is invalid");
            }
        }

        [TestMethod()]
        public void TestReadPcoreAngleDef()
        {
            string filename = "NCDK.Data.PCore.pcoreangle.xml";
            Stream ins = ResourceLoader.GetAsStream(filename);
            var defs = PharmacophoreUtils.ReadPharmacophoreDefinitions(ins);

            Assert.AreEqual(1, defs.Count);

            var def1 = defs[0];
            Assert.AreEqual(3, def1.Atoms.Count);
            Assert.AreEqual(2, def1.Bonds.Count);
            Assert.AreEqual("A modified definition for the D1 receptor", def1.GetProperty<string>("description"));

            string[] ids = { "Aromatic", "Hydroxyl", "BasicAmine" };
            foreach (var atom in def1.Atoms)
            {
                string sym = atom.Symbol;
                bool found = false;
                foreach (var s in ids)
                {
                    if (sym.Equals(s))
                    {
                        found = true;
                        break;
                    }
                }
                Assert.IsTrue(found, "'" + sym + "' in pcore.xml is invalid");
            }

            foreach (var bond in def1.Bonds)
            {
                IAtom[] a;
                switch (bond)
                {
                    case PharmacophoreQueryBond cons:
                        a = GetAtoms(cons);
                        Assert.AreEqual(2, a.Length);
                        break;
                    case PharmacophoreQueryAngleBond cons:
                        a = GetAtoms(cons);
                        Assert.AreEqual(3, a.Length);
                        break;
                }
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(CDKException))]
        public void TestInvalidPcoreXML()
        {
            string filename = "NCDK.Data.PCore.invalid1.xml";
            Stream ins = ResourceLoader.GetAsStream(filename);
            PharmacophoreUtils.ReadPharmacophoreDefinitions(ins);
        }

        [TestMethod()]
        public void TestPCoreWrite()
        {
            string filename = "NCDK.Data.PCore.pcore.xml";
            Stream ins = ResourceLoader.GetAsStream(filename);
            var defs = PharmacophoreUtils.ReadPharmacophoreDefinitions(ins);

            PharmacophoreQuery[] defarray = defs.ToArray();
            var baos = new MemoryStream();
            PharmacophoreUtils.WritePharmacophoreDefinition(defarray, baos);
            Assert.IsTrue(baos.ToArray().Length > 0);

            string[] lines;
            using (var sr = new StreamReader(new MemoryStream(baos.ToArray()), true))
            {
                var list = new List<string>();
                string line;
                while ((line = sr.ReadLine()) != null)
                    list.Add(line);
                lines = list.ToArray();
            }
            Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-8\"?>", lines[0].Trim());

            int ndef = 0;
            int ndist = 0;
            int nangle = 0;
            foreach (var line in lines)
            {
                if (line.IndexOf("</pharmacophore>") != -1) ndef++;
                if (line.IndexOf("</distanceConstraint>") != -1) ndist++;
                if (line.IndexOf("</angleConstraint>") != -1) nangle++;
            }
            Assert.AreEqual(2, ndef);
            Assert.AreEqual(5, ndist);
            Assert.AreEqual(0, nangle);
        }

        private static IAtom[] GetAtoms(IBond bond)
        {
            var alist = new List<IAtom>();
            foreach (var iAtom in bond.Atoms)
            {
                alist.Add(iAtom);
            }
            return alist.ToArray();
        }
    }
}
