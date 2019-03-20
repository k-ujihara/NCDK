/*
 * Copyright (c) 2014 European Bioinformatics Institute (EMBL-EBI)
 *                    John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 2.1 of the License, or (at
 * your option) any later version. All we ask is that proper credit is given
 * for our work, which includes - but is not limited to - adding the above
 * copyright notice to the beginning of your source code files, and to any
 * copyright notice that you may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NCDK.Common.Base;
using NCDK.Smiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Isomorphisms
{
    // @author John May
    // @cdk.module test-smarts
    [TestClass()]
    public class MappingsTest
    {
        [TestMethod()]
        public void Filter()
        {
            var m_iterable = new Mock<IEnumerable<int[]>>(); var iterable = m_iterable.Object;
            var m_iterator = new Mock<IEnumerator<int[]>>(); var iterator = m_iterator.Object;
            m_iterable.Setup(n => n.GetEnumerator()).Returns(iterator);

            int[] p1 = { 0, 1, 2 };
            int[] p2 = { 0, 2, 1 };
            int[] p3 = { 0, 3, 4 };
            int[] p4 = { 0, 4, 3 };

            m_iterator.SetupSequence(n => n.MoveNext()).Returns(true).Returns(true).Returns(true).Returns(true).Returns(false);
            m_iterator.SetupSequence(n => n.Current).Returns(p1).Returns(p2).Returns(p3).Returns(p4);

            Mappings ms = new Mappings(new Mock<IAtomContainer>().Object, new Mock<IAtomContainer>().Object, iterable);

            bool f(int[] n)
            {
                if (n == p1) return false;
                if (n == p2) return true;
                if (n == p3) return false;
                if (n == p4) return true;
                throw new InvalidOperationException();
            }

            Assert.IsTrue(Compares.AreDeepEqual(new int[][] { p2, p4 }, ms.Filter(f).ToArray()));
        }

        [TestMethod()]
        public void Map()
        {
            var m_iterable = new Mock<IEnumerable<int[]>>(); var iterable = m_iterable.Object;
            var m_iterator = new Mock<IEnumerator<int[]>>(); var iterator = m_iterator.Object;
            m_iterable.Setup(n => n.GetEnumerator()).Returns(iterator);

            int[] p1 = { 0, 1, 2 };
            int[] p2 = { 0, 2, 1 };
            int[] p3 = { 0, 3, 4 };
            int[] p4 = { 0, 4, 3 };

            m_iterator.SetupSequence(n => n.MoveNext()).Returns(true).Returns(true).Returns(true).Returns(true).Returns(false);
            m_iterator.SetupSequence(n => n.Current).Returns(p1).Returns(p2).Returns(p3).Returns(p4);

            Mappings ms = new Mappings(new Mock<IAtomContainer>().Object, new Mock<IAtomContainer>().Object, iterable);

            int nCalls = 0;
            var strings = ms.GetMapping(
                n =>
                {
                    nCalls++;
                    if (n == p1) return "p1";
                    if (n == p2) return "p2";
                    if (n == p3) return "p3";
                    if (n == p4) return "p4";
                    return null;
                });
            var stringIt = strings.GetEnumerator();

            Assert.AreEqual(0, nCalls);

            Assert.IsTrue(stringIt.MoveNext());
            Assert.AreEqual(stringIt.Current, "p1");
            Assert.IsTrue(stringIt.MoveNext());
            Assert.AreEqual(stringIt.Current, "p2");
            Assert.IsTrue(stringIt.MoveNext());
            Assert.AreEqual(stringIt.Current, "p3");
            Assert.IsTrue(stringIt.MoveNext());
            Assert.AreEqual(stringIt.Current, "p4");
            Assert.IsFalse(stringIt.MoveNext());

            Assert.AreEqual(4, nCalls);
        }

        [TestMethod()]
        public void Limit()
        {
            var m_iterable = new Mock<IEnumerable<int[]>>(); var iterable = m_iterable.Object;
            var m_iterator = new Mock<IEnumerator<int[]>>(); var iterator = m_iterator.Object;
            m_iterable.Setup(n => n.GetEnumerator()).Returns(iterator);
            m_iterator.SetupSequence(n => n.MoveNext()).Returns(true).Returns(true).Returns(true).Returns(true).Returns(true).Returns(false);
            m_iterator.SetupGet(n => n.Current).Returns(Array.Empty<int>());

            var ms = new Mappings(new Mock<IAtomContainer>().Object, new Mock<IAtomContainer>().Object, iterable);
            Assert.AreEqual(2, ms.Limit(2).Count());
            m_iterator.Verify(n => n.Current, Times.AtMost(2)); // was only called twice
        }

        [TestMethod()]
        public void Stereochemistry()
        {
            // tested by Filter() + StereoMatchTest
        }

        [TestMethod()]
        public void UniqueAtoms()
        {
            // tested by Filter() + MappingPredicatesTest
        }

        [TestMethod()]
        public void UniqueBonds()
        {
            // tested by Filter() + MappingPredicatesTest
        }

        [TestMethod()]
        public void ToArray()
        {
            var m_iterable = new Mock<IEnumerable<int[]>>(); var iterable = m_iterable.Object;
            var m_iterator = new Mock<IEnumerator<int[]>>(); var iterator = m_iterator.Object;
            m_iterable.Setup(n => n.GetEnumerator()).Returns(iterator);
            m_iterator.SetupSequence(n => n.MoveNext()).Returns(true).Returns(true).Returns(true).Returns(true).Returns(false);

            int[] p1 = { 0, 1, 2 };
            int[] p2 = { 0, 2, 1 };
            int[] p3 = { 0, 3, 4 };
            int[] p4 = { 0, 4, 3 };

            m_iterator.SetupSequence(n => n.Current).Returns(p1).Returns(p2).Returns(p3).Returns(p4);

            Mappings ms = new Mappings(new Mock<IAtomContainer>().Object, new Mock<IAtomContainer>().Object, iterable);
            Assert.IsTrue(Compares.AreDeepEqual(new int[][] { p1, p2, p3, p4 }, ms.ToArray()));
        }

        [TestMethod()]
        public void ToAtomMap()
        {
            IAtomContainer query = Smi("CC");
            IAtomContainer target = Smi("CC");

            var iterable = Pattern.FindIdentical(query).MatchAll(target).ToAtomMaps();
            var iterator = iterable.GetEnumerator();

            Assert.IsTrue(iterator.MoveNext());
            var m1 = iterator.Current;
            Assert.AreEqual(m1[query.Atoms[0]], target.Atoms[0]);
            Assert.AreEqual(m1[query.Atoms[1]], target.Atoms[1]);
            Assert.IsTrue(iterator.MoveNext());
            var m2 = iterator.Current;
            Assert.AreEqual(m2[query.Atoms[0]], target.Atoms[1]);
            Assert.AreEqual(m2[query.Atoms[1]], target.Atoms[0]);
            Assert.IsFalse(iterator.MoveNext());
        }

        [TestMethod()]
        public void ToBondMap()
        {
            IAtomContainer query = Smi("CCC");
            IAtomContainer target = Smi("CCC");

            var iterable = Pattern.FindIdentical(query).MatchAll(target).ToBondMaps();
            var iterator = iterable.GetEnumerator();

            Assert.IsTrue(iterator.MoveNext());
            var m1 = iterator.Current;
            Assert.AreEqual(m1[query.Bonds[0]], target.Bonds[0]);
            Assert.AreEqual(m1[query.Bonds[1]], target.Bonds[1]);
            Assert.IsTrue(iterator.MoveNext());
            var m2 = iterator.Current;
            Assert.AreEqual(m2[query.Bonds[0]], target.Bonds[1]);
            Assert.AreEqual(m2[query.Bonds[1]], target.Bonds[0]);
            Assert.IsFalse(iterator.MoveNext());
        }

        [TestMethod()]
        public void ToAtomBondMap()
        {
            IAtomContainer query = Smi("CCC");
            IAtomContainer target = Smi("CCC");

            var iterable = Pattern.FindIdentical(query).MatchAll(target).ToAtomBondMaps();
            var iterator = iterable.GetEnumerator();

            Assert.IsTrue(iterator.MoveNext());
            var m1 = iterator.Current;
            Assert.AreEqual(m1[query.Atoms[0]], (IChemObject)target.Atoms[0]);
            Assert.AreEqual(m1[query.Atoms[1]], (IChemObject)target.Atoms[1]);
            Assert.AreEqual(m1[query.Atoms[2]], (IChemObject)target.Atoms[2]);
            Assert.AreEqual(m1[query.Bonds[0]], (IChemObject)target.Bonds[0]);
            Assert.AreEqual(m1[query.Bonds[1]], (IChemObject)target.Bonds[1]);
            Assert.IsTrue(iterator.MoveNext());
            var m2 = iterator.Current;
            Assert.AreEqual(m2[query.Atoms[0]], (IChemObject)target.Atoms[2]);
            Assert.AreEqual(m2[query.Atoms[1]], (IChemObject)target.Atoms[1]);
            Assert.AreEqual(m2[query.Atoms[2]], (IChemObject)target.Atoms[0]);
            Assert.AreEqual(m2[query.Bonds[0]], (IChemObject)target.Bonds[1]);
            Assert.AreEqual(m2[query.Bonds[1]], (IChemObject)target.Bonds[0]);
            Assert.IsFalse(iterator.MoveNext());
        }

        [TestMethod()]
        public void ToSubstructures()
        {
            IAtomContainer query = Smi("O1CC1");
            IAtomContainer target = Smi("C1OC1CCC");

            var iterable = Pattern.FindSubstructure(query)
                                                       .MatchAll(target)
                                                       .GetUniqueAtoms()
                                                       .ToSubstructures();
            var iterator = iterable.GetEnumerator();

            Assert.IsTrue(iterator.MoveNext());
            IAtomContainer submol = iterator.Current;
            Assert.AreNotEqual(query, submol);
            // note that indices are mapped from query to target
            Assert.AreEqual(submol.Atoms[0], target.Atoms[1]); // oxygen
            Assert.AreEqual(submol.Atoms[1], target.Atoms[0]); // C
            Assert.AreEqual(submol.Atoms[2], target.Atoms[2]); // C
            Assert.AreEqual(submol.Bonds[0], target.Bonds[0]); // C-O bond
            Assert.AreEqual(submol.Bonds[1], target.Bonds[2]); // O-C bond
            Assert.AreEqual(submol.Bonds[2], target.Bonds[1]); // C-C bond
            Assert.IsFalse(iterator.MoveNext());
        }

        [TestMethod()]
        public void AtLeast()
        {
            var m_iterable = new Mock<IEnumerable<int[]>>(); var iterable = m_iterable.Object;
            var m_iterator = new Mock<IEnumerator<int[]>>(); var iterator = m_iterator.Object;
            m_iterable.Setup(n => n.GetEnumerator()).Returns(iterator);
            m_iterator.SetupSequence(n => n.MoveNext()).Returns(true).Returns(true).Returns(true).Returns(true).Returns(true).Returns(false);
            m_iterator.SetupGet(n => n.Current).Returns(Array.Empty<int>());

            Mappings ms = new Mappings(new Mock<IAtomContainer>().Object, new Mock<IAtomContainer>().Object, iterable);

            Assert.IsTrue(ms.AtLeast(2));
            m_iterator.Verify(n => n.Current, Times.AtMost(2)); // was only called twice
        }

        [TestMethod()]
        public void First()
        {
            var m_iterable = new Mock<IEnumerable<int[]>>(); var iterable = m_iterable.Object;
            var m_iterator = new Mock<IEnumerator<int[]>>(); var iterator = m_iterator.Object;
            m_iterable.Setup(n => n.GetEnumerator()).Returns(iterator);
            m_iterator.SetupSequence(n => n.MoveNext()).Returns(true).Returns(true).Returns(false);

            int[] p1 = Array.Empty<int>();
            int[] p2 = Array.Empty<int>();

            m_iterator.SetupSequence(n => n.Current).Returns(p1).Returns(p2);

            Mappings ms = new Mappings(new Mock<IAtomContainer>().Object, new Mock<IAtomContainer>().Object, iterable);
            Assert.AreSame(p1, ms.First());
        }

        [TestMethod()]
        public void Count()
        {
            var m_iterable = new Mock<IEnumerable<int[]>>(); var iterable = m_iterable.Object;
            var m_iterator = new Mock<IEnumerator<int[]>>(); var iterator = m_iterator.Object;
            m_iterable.Setup(n => n.GetEnumerator()).Returns(iterator);
            m_iterator.SetupSequence(n => n.MoveNext()).Returns(true).Returns(true).Returns(true).Returns(true).Returns(true).Returns(false);
            m_iterator.SetupSequence(n => n.Current).Returns(Array.Empty<int>());

            Mappings ms = new Mappings(new Mock<IAtomContainer>().Object, new Mock<IAtomContainer>().Object, iterable);
            Assert.AreEqual(5, ms.Count());
        }

        [TestMethod()]
        public void CountUnique()
        {
            var m_iterable = new Mock<IEnumerable<int[]>>(); var iterable = m_iterable.Object;
            var m_iterator = new Mock<IEnumerator<int[]>>(); var iterator = m_iterator.Object;
            m_iterable.Setup(n => n.GetEnumerator()).Returns(iterator);
            m_iterator.SetupSequence(n => n.MoveNext()).Returns(true).Returns(true).Returns(true).Returns(true).Returns(false);

            int[] p1 = { 0, 1, 2 };
            int[] p2 = { 0, 2, 1 };
            int[] p3 = { 0, 3, 4 };
            int[] p4 = { 0, 4, 3 };

            m_iterator.SetupSequence(n => n.Current).Returns(p1).Returns(p2).Returns(p3).Returns(p4);

            Mappings ms = new Mappings(new Mock<IAtomContainer>().Object, new Mock<IAtomContainer>().Object, iterable);
            Assert.AreEqual(2, ms.CountUnique());
        }

        [TestMethod()]
        public void Iterator()
        {
            var m_iterable = new Mock<IEnumerable<int[]>>(); var iterable = m_iterable.Object;
            var m_iterator = new Mock<IEnumerator<int[]>>(); var iterator = m_iterator.Object;
            m_iterable.Setup(n => n.GetEnumerator()).Returns(iterator);
            Mappings ms = new Mappings(new Mock<IAtomContainer>().Object, new Mock<IAtomContainer>().Object, iterable);
            Assert.AreSame(iterator, ms.GetEnumerator());
        }

        SmilesParser smipar = CDK.SmilesParser;

        IAtomContainer Smi(string smi)
        {
            return smipar.ParseSmiles(smi);
        }
    }
}
