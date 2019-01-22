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
using System.Linq;

namespace NCDK.Pharmacophore
{
    // @cdk.module test-pcore
    [TestClass()]
    public class PharmacophoreMatcherTest
    {
        public static ConformerContainer conformers = null;

        static PharmacophoreMatcherTest()
        {
            var filename = "NCDK.Data.MDL.pcoretest1.sdf";
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new EnumerableMDLConformerReader(ins, CDK.Builder).GetEnumerator();
            if (reader.MoveNext())
                conformers = (ConformerContainer)reader.Current;
        }

        [TestMethod()]
        public void TestMatcherQuery1()
        {
            Assert.IsNotNull(conformers);

            // make a query
            var query = new PharmacophoreQuery();

            var o = new PharmacophoreQueryAtom("D", "[OX1]");
            var n1 = new PharmacophoreQueryAtom("A", "[N]");
            var n2 = new PharmacophoreQueryAtom("A", "[N]");

            query.Atoms.Add(o);
            query.Atoms.Add(n1);
            query.Atoms.Add(n2);

            var b1 = new PharmacophoreQueryBond(o, n1, 4.0, 4.5);
            var b2 = new PharmacophoreQueryBond(o, n2, 4.0, 5.0);
            var b3 = new PharmacophoreQueryBond(n1, n2, 5.4, 5.8);

            query.Bonds.Add(b1);
            query.Bonds.Add(b2);
            query.Bonds.Add(b3);

            var matcher = new PharmacophoreMatcher(query);

            bool firstTime = true;
            int i = 0;
            var statuses = new bool[100];
            foreach (var conf in conformers)
            {
                if (firstTime)
                {
                    statuses[i] = matcher.Matches(conf, true);
                    firstTime = false;
                }
                else
                    statuses[i] = matcher.Matches(conf, false);
                i++;
            }

            var hits = new int[18];
            int idx = 0;
            for (i = 0; i < statuses.Length; i++)
            {
                if (statuses[i])
                    hits[idx++] = i;
            }

            int[] expected = { 0, 1, 2, 5, 6, 7, 8, 9, 10, 20, 23, 48, 62, 64, 66, 70, 76, 87 };
            for (i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], hits[i], $"Hit {i} didn't match");
            }
        }

        [TestMethod()]
        public void TestMatchedAtoms()
        {
            Assert.IsNotNull(conformers);

            // make a query
            var query = new PharmacophoreQuery();

            var o = new PharmacophoreQueryAtom("D", "[OX1]");
            var n1 = new PharmacophoreQueryAtom("A", "[N]");
            var n2 = new PharmacophoreQueryAtom("A", "[N]");

            query.Atoms.Add(o);
            query.Atoms.Add(n1);
            query.Atoms.Add(n2);

            var b1 = new PharmacophoreQueryBond(o, n1, 4.0, 4.5);
            var b2 = new PharmacophoreQueryBond(o, n2, 4.0, 5.0);
            var b3 = new PharmacophoreQueryBond(n1, n2, 5.4, 5.8);

            query.Bonds.Add(b1);
            query.Bonds.Add(b2);
            query.Bonds.Add(b3);

            var conf1 = conformers[0];
            var matcher = new PharmacophoreMatcher(query);
            bool status = matcher.Matches(conf1);
            Assert.IsTrue(status);

            var pmatches = matcher.GetMatchingPharmacophoreAtoms();
            Assert.AreEqual(2, pmatches.Count);

            var upmatches = matcher.GetUniqueMatchingPharmacophoreAtoms();
            Assert.AreEqual(1, upmatches.Count);
        }

        [TestMethod()]
        public void TestMatchedBonds()
        {
            Assert.IsNotNull(conformers);

            // make a query
            var query = new PharmacophoreQuery();

            var o = new PharmacophoreQueryAtom("D", "[OX1]");
            var n1 = new PharmacophoreQueryAtom("A", "[N]");
            var n2 = new PharmacophoreQueryAtom("A", "[N]");

            query.Atoms.Add(o);
            query.Atoms.Add(n1);
            query.Atoms.Add(n2);

            var b1 = new PharmacophoreQueryBond(o, n1, 4.0, 4.5);
            var b2 = new PharmacophoreQueryBond(o, n2, 4.0, 5.0);
            var b3 = new PharmacophoreQueryBond(n1, n2, 5.4, 5.8);

            query.Bonds.Add(b1);
            query.Bonds.Add(b2);
            query.Bonds.Add(b3);

            var conf1 = conformers[0];
            var matcher = new PharmacophoreMatcher(query);
            bool status = matcher.Matches(conf1);
            Assert.IsTrue(status);

            var bMatches = matcher.GetMatchingPharmacophoreBonds();
            Assert.AreEqual(2, bMatches.Count); // 2 since we haven't gotten a unique set
            Assert.AreEqual(3, bMatches[0].Count);

            var pbond = (PharmacophoreBond)BondRef.Deref(bMatches[0][0]);
            var patom1 = (PharmacophoreAtom)AtomRef.Deref(pbond.Begin);
            var patom2 = (PharmacophoreAtom)AtomRef.Deref(pbond.End);
            Assert.AreEqual("D", patom1.Symbol);
            Assert.AreEqual("A", patom2.Symbol);

            var bondMap = matcher.GetTargetQueryBondMappings();
            Assert.AreEqual(2, bondMap.Count);
            var mapping = bondMap[0];

            // get the 'BondRef' for lookup
            var value = mapping[bMatches[0][0]];
            Assert.AreEqual(b1, value);
        }

        [TestMethod()]
        [ExpectedException(typeof(CDKException))]
        public void TestInvalidQuery()
        {
            var query = new PharmacophoreQuery();
            var o = new PharmacophoreQueryAtom("D", "[OX1]");
            var n1 = new PharmacophoreQueryAtom("A", "[N]");
            var n2 = new PharmacophoreQueryAtom("A", "[NX3]");

            query.Atoms.Add(o);
            query.Atoms.Add(n1);
            query.Atoms.Add(n2);

            var b1 = new PharmacophoreQueryBond(o, n1, 4.0, 4.5);
            var b2 = new PharmacophoreQueryBond(o, n2, 4.0, 5.0);
            var b3 = new PharmacophoreQueryBond(n1, n2, 5.4, 5.8);

            query.Bonds.Add(b1);
            query.Bonds.Add(b2);
            query.Bonds.Add(b3);

            var matcher = new PharmacophoreMatcher(query);
            matcher.Matches(conformers[0]);
        }

        [TestMethod()]
        public void TestCNSPcore()
        {
            var filename = "NCDK.Data.MDL.cnssmarts.sdf";
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new EnumerableSDFReader(ins, CDK.Builder);

            var query = new PharmacophoreQuery();
            var arom = new PharmacophoreQueryAtom("A", "c1ccccc1");
            var n1 = new PharmacophoreQueryAtom("BasicAmine", "[NX3;h2,h1,H1,H2;!$(NC=O)]");
            var b1 = new PharmacophoreQueryBond(arom, n1, 5.0, 7.0);
            query.Atoms.Add(arom);
            query.Atoms.Add(n1);
            query.Bonds.Add(b1);

            var mol = reader.First();
            reader.Close();

            var matcher = new PharmacophoreMatcher(query);
            var status = matcher.Matches(mol);
            Assert.IsTrue(status);

            var pmatches = matcher.GetMatchingPharmacophoreAtoms();
            Assert.AreEqual(1, pmatches.Count);

            var upmatches = matcher.GetUniqueMatchingPharmacophoreAtoms();
            Assert.AreEqual(1, upmatches.Count);
        }

        [TestMethod()]
        public void TestMatchingBonds()
        {
            var filename = "NCDK.Data.MDL.cnssmarts.sdf";
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new EnumerableSDFReader(ins, CDK.Builder);

            var query = new PharmacophoreQuery();
            var arom = new PharmacophoreQueryAtom("A", "c1ccccc1");
            var n1 = new PharmacophoreQueryAtom("BasicAmine", "[NX3;h2,h1,H1,H2;!$(NC=O)]");
            var b1 = new PharmacophoreQueryBond(arom, n1, 5.0, 7.0);
            query.Atoms.Add(arom);
            query.Atoms.Add(n1);
            query.Bonds.Add(b1);

            var mol = reader.First();
            reader.Close();

            var matcher = new PharmacophoreMatcher(query);
            bool status = matcher.Matches(mol);
            Assert.IsTrue(status);

            var pmatches = matcher.GetMatchingPharmacophoreAtoms();
            Assert.AreEqual(1, pmatches.Count);

            var upmatches = matcher.GetUniqueMatchingPharmacophoreAtoms();
            Assert.AreEqual(1, upmatches.Count);

            var bmatches = matcher.GetMatchingPharmacophoreBonds();
            Assert.AreEqual(1, bmatches.Count);
            var bmatch = bmatches[0];
            Assert.AreEqual(1, bmatch.Count);
            var pbond = (PharmacophoreBond)BondRef.Deref(bmatch[0]);
            Assert.AreEqual(5.63, pbond.BondLength, 0.01);
        }

        [TestMethod()]
        public void TestAngleMatch1()
        {
            var filename = "NCDK.Data.MDL.cnssmarts.sdf";
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new EnumerableSDFReader(ins, CDK.Builder);

            var query = new PharmacophoreQuery();
            var n1 = new PharmacophoreQueryAtom("BasicAmine", "[NX3;h2,h1,H1,H2;!$(NC=O)]");
            var n2 = new PharmacophoreQueryAtom("BasicAmine", "[NX3;h2,h1,H1,H2;!$(NC=O)]");
            var n3 = new PharmacophoreQueryAtom("BasicAmine", "[NX3;h2,h1,H1,H2;!$(NC=O)]");
            var b1 = new PharmacophoreQueryAngleBond(n1, n2, n3, 85, 90);
            query.Atoms.Add(n1);
            query.Atoms.Add(n2);
            query.Atoms.Add(n3);
            query.Bonds.Add(b1);

            var mol = reader.First();
            reader.Close();

            var matcher = new PharmacophoreMatcher(query);
            var status = matcher.Matches(mol);
            Assert.IsTrue(status);
        }

        [TestMethod()]
        public void TestAngleMatch2()
        {
            var filename = "NCDK.Data.MDL.cnssmarts.sdf";
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new EnumerableSDFReader(ins, CDK.Builder);

            var query = new PharmacophoreQuery();
            var n1 = new PharmacophoreQueryAtom("BasicAmine", "[NX3;h2,h1,H1,H2;!$(NC=O)]");
            var n2 = new PharmacophoreQueryAtom("BasicAmine", "[NX3;h2,h1,H1,H2;!$(NC=O)]");
            var n3 = new PharmacophoreQueryAtom("BasicAmine", "[NX3;h2,h1,H1,H2;!$(NC=O)]");
            var b1 = new PharmacophoreQueryAngleBond(n1, n2, n3, 89.14);
            query.Atoms.Add(n1);
            query.Atoms.Add(n2);
            query.Atoms.Add(n3);
            query.Bonds.Add(b1);

            var mol = reader.First();
            reader.Close();

            var matcher = new PharmacophoreMatcher(query);
            var status = matcher.Matches(mol);
            Assert.IsTrue(status);
        }

        [TestMethod()]
        public void TestAngleMatch3()
        {
            Assert.IsNotNull(conformers);

            // make a query
            var query = new PharmacophoreQuery();

            var o = new PharmacophoreQueryAtom("D", "[OX1]");
            var n1 = new PharmacophoreQueryAtom("A", "[N]");
            var n2 = new PharmacophoreQueryAtom("A", "[N]");

            query.Atoms.Add(o);
            query.Atoms.Add(n1);
            query.Atoms.Add(n2);
            var b1 = new PharmacophoreQueryAngleBond(o, n1, n2, 43, 47);
            query.Bonds.Add(b1);

            var matcher = new PharmacophoreMatcher(query);

            bool firstTime = true;
            int i = 0;
            var statuses = new bool[100];
            foreach (var conf in conformers)
            {
                if (firstTime)
                {
                    statuses[i] = matcher.Matches(conf, true);
                    firstTime = false;
                }
                else
                    statuses[i] = matcher.Matches(conf, false);
                i++;
            }
            Assert.AreEqual(100, statuses.Length);

            var hits = new int[9];
            int idx = 0;
            for (i = 0; i < statuses.Length; i++)
            {
                if (statuses[i]) hits[idx++] = i;
            }

            int[] expected = { 0, 6, 32, 33, 48, 54, 60, 62, 69 };
            for (i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], hits[i], $"Hit {i} didn't match");
            }
        }

        [TestMethod()]
        public void TestGetterSetter()
        {
            var query = new PharmacophoreQuery();
            var arom = new PharmacophoreQueryAtom("A", "c1ccccc1");
            var n1 = new PharmacophoreQueryAtom("BasicAmine", "[NX3;h2,h1,H1,H2;!$(NC=O)]");
            var b1 = new PharmacophoreQueryBond(arom, n1, 5.0, 7.0);
            query.Atoms.Add(arom);
            query.Atoms.Add(n1);
            query.Bonds.Add(b1);

            var matcher = new PharmacophoreMatcher();
            matcher.SetPharmacophoreQuery(query);
            var retQuery = matcher.GetPharmacophoreQuery();
            Assert.AreEqual(2, retQuery.Atoms.Count);
            Assert.AreEqual(1, retQuery.Bonds.Count);
        }

        [TestMethod()]
        public void MultiSmartsQuery()
        {
            var query = new PharmacophoreQuery();
            var rings = new PharmacophoreQueryAtom("A", "c1ccccc1|C1CCCC1");
            var o1 = new PharmacophoreQueryAtom("Hd", "[OX1]");
            var b1 = new PharmacophoreQueryBond(rings, o1, 3.5, 5.8);
            query.Atoms.Add(rings);
            query.Atoms.Add(o1);
            query.Bonds.Add(b1);

            var matcher = new PharmacophoreMatcher();
            matcher.SetPharmacophoreQuery(query);

            var filename = "NCDK.Data.PCore.multismartpcore.sdf";
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new EnumerableSDFReader(ins, CDK.Builder);
            var enumerator = reader.GetEnumerator();

            enumerator.MoveNext();
            var mol = enumerator.Current;
            Assert.IsTrue(matcher.Matches(mol));
            Assert.AreEqual(1, matcher.GetUniqueMatchingPharmacophoreAtoms().Count);
            Assert.AreEqual(2, matcher.GetUniqueMatchingPharmacophoreAtoms()[0].Count);

            enumerator.MoveNext();
            mol = enumerator.Current;
            Assert.IsTrue(matcher.Matches(mol));
            Assert.AreEqual(2, matcher.GetUniqueMatchingPharmacophoreAtoms().Count);
            Assert.AreEqual(2, matcher.GetUniqueMatchingPharmacophoreAtoms()[0].Count);
            Assert.AreEqual(2, matcher.GetUniqueMatchingPharmacophoreAtoms()[1].Count);

            enumerator.MoveNext();
            mol = enumerator.Current;
            reader.Close();
            Assert.IsFalse(matcher.Matches(mol));
        }
    }
}
