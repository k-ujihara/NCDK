/*
 * Copyright (c) 2013 European Bioinformatics Institute (EMBL-EBI)
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
using NCDK.Default;
using NCDK.Graphs;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace NCDK.Isomorphisms
{
    /**
     * @author John May
     * @cdk.module test-isomorphism
     */
    [TestClass()]
    public class ComponentGroupingTest
    {

        // mock matching [#8].[#8]
        [TestMethod()]
        public void Ungrouped()
        {
            Assert.IsTrue(Create(null, oxidanone()).Apply(new int[] { 0, 1 }));
            Assert.IsTrue(Create(null, oxidanone()).Apply(new int[] { 1, 0 }));
            Assert.IsTrue(Create(null, EthyleneGlycol()).Apply(new int[] { 0, 3 }));
            Assert.IsTrue(Create(null, EthyleneGlycol()).Apply(new int[] { 3, 0 }));
            Assert.IsTrue(Create(null, EthylAlcoholHydrate()).Apply(new int[] { 0, 3 }));
            Assert.IsTrue(Create(null, EthylAlcoholHydrate()).Apply(new int[] { 3, 0 }));
        }

        // mock matching ([#8].[#8])
        [TestMethod()]
        public void Grouped()
        {
            int[] grouping = { 1, 1, 1 };
            Assert.IsTrue(Create(grouping, oxidanone()).Apply(new int[] { 0, 1 }));
            Assert.IsTrue(Create(grouping, oxidanone()).Apply(new int[] { 1, 0 }));
            Assert.IsTrue(Create(grouping, EthyleneGlycol()).Apply(new int[] { 0, 3 }));
            Assert.IsTrue(Create(grouping, EthyleneGlycol()).Apply(new int[] { 3, 0 }));
            Assert.IsFalse(Create(grouping, EthylAlcoholHydrate()).Apply(new int[] { 0, 3 }));
            Assert.IsFalse(Create(grouping, EthylAlcoholHydrate()).Apply(new int[] { 3, 0 }));
        }

        // mock matching ([#8]).([#8])
        [TestMethod()]
        public void MultipleGroups()
        {
            int[] grouping = { 1, 2, 2 };
            Assert.IsFalse(Create(grouping, oxidanone()).Apply(new int[] { 0, 1 }));
            Assert.IsFalse(Create(grouping, oxidanone()).Apply(new int[] { 1, 0 }));
            Assert.IsFalse(Create(grouping, EthyleneGlycol()).Apply(new int[] { 0, 3 }));
            Assert.IsFalse(Create(grouping, EthyleneGlycol()).Apply(new int[] { 3, 0 }));
            Assert.IsTrue(Create(grouping, EthylAlcoholHydrate()).Apply(new int[] { 0, 3 }));
            Assert.IsTrue(Create(grouping, EthylAlcoholHydrate()).Apply(new int[] { 3, 0 }));
        }

        /// <summary>@cdk.inchi InChI=1/O2/c1-2</summary>
        static IAtomContainer oxidanone()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(new Atom("O"));
            m.Atoms.Add(new Atom("O"));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Double);
            return m;
        }

        /// <summary>@cdk.inchi InChI=1/C2H6O2/c3-1-2-4/h3-4H,1-2H2</summary>
        static IAtomContainer EthyleneGlycol()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(new Atom("O"));
            m.Atoms.Add(new Atom("C"));
            m.Atoms.Add(new Atom("C"));
            m.Atoms.Add(new Atom("O"));
            m.AddBond(m.Atoms[0], m.Atoms[1], BondOrder.Single);
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Single);
            return m;
        }

        /// <summary>InChI=1/C2H6O.H2O/c1-2-3;/h3H,2H2,1H3;1H2</summary>
        static IAtomContainer EthylAlcoholHydrate()
        {
            IAtomContainer m = new AtomContainer();
            m.Atoms.Add(new Atom("O"));
            m.Atoms.Add(new Atom("C"));
            m.Atoms.Add(new Atom("C"));
            m.Atoms.Add(new Atom("O"));
            m.AddBond(m.Atoms[1], m.Atoms[2], BondOrder.Single);
            m.AddBond(m.Atoms[2], m.Atoms[3], BondOrder.Single);
            return m;
        }

        static ComponentGrouping Create(int[] grouping, IAtomContainer container)
        {
            return new ComponentGrouping(grouping, new ConnectedComponents(GraphUtil.ToAdjList(container)));
        }
    }
}
