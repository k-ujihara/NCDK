/* Copyright (C) 2008 Egon Willighagen <egonw@users.sf.net>
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

namespace NCDK.Isomorphisms.Matchers
{
    /// <summary>
    /// Checks the functionality of the <see cref="QueryAtomContainerCreator"/> .
    /// </summary>
    // @cdk.module test-isomorphism
    [TestClass()]
    public class QueryAtomContainerCreatorTest : CDKTestCase
    {
        // @cdk.inchi InChI=1/C8H10/c1-7-5-3-4-6-8(7)2/h3-6H,1-2H3
        [TestMethod()]
        public void Test12DimethylBenzene()
        {
            var builder = CDK.Builder;
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[5], molecule.Atoms[0], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[6], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[7], BondOrder.Single);

            // 2,3-dimethyl-1,3-butadiene matches
            IAtomContainer query1 = builder.NewAtomContainer();
            query1.Atoms.Add(builder.NewAtom("C"));
            query1.Atoms.Add(builder.NewAtom("C"));
            query1.Atoms.Add(builder.NewAtom("C"));
            query1.Atoms.Add(builder.NewAtom("C"));
            query1.Atoms.Add(builder.NewAtom("C"));
            query1.Atoms.Add(builder.NewAtom("C"));
            query1.AddBond(query1.Atoms[0], query1.Atoms[1], BondOrder.Single);
            query1.AddBond(query1.Atoms[1], query1.Atoms[2], BondOrder.Double);
            query1.AddBond(query1.Atoms[3], query1.Atoms[0], BondOrder.Double);
            query1.AddBond(query1.Atoms[0], query1.Atoms[4], BondOrder.Single);
            query1.AddBond(query1.Atoms[1], query1.Atoms[5], BondOrder.Single);
            QueryAtomContainer queryContainer1 = QueryAtomContainerCreator.CreateSymbolAndBondOrderQueryContainer(query1);
            Assert.IsTrue(new UniversalIsomorphismTester().IsSubgraph(molecule, queryContainer1));

            // 2,3-dimethyl-2-butene does not match
            IAtomContainer query2 = builder.NewAtomContainer();
            query2.Atoms.Add(builder.NewAtom("C"));
            query2.Atoms.Add(builder.NewAtom("C"));
            query2.Atoms.Add(builder.NewAtom("C"));
            query2.Atoms.Add(builder.NewAtom("C"));
            query2.Atoms.Add(builder.NewAtom("C"));
            query2.Atoms.Add(builder.NewAtom("C"));
            query2.AddBond(query2.Atoms[0], query2.Atoms[1], BondOrder.Double);
            query2.AddBond(query2.Atoms[1], query2.Atoms[2], BondOrder.Single);
            query2.AddBond(query2.Atoms[3], query2.Atoms[0], BondOrder.Single);
            query2.AddBond(query2.Atoms[0], query2.Atoms[4], BondOrder.Single);
            query2.AddBond(query2.Atoms[1], query2.Atoms[5], BondOrder.Single);
            QueryAtomContainer queryContainer2 = QueryAtomContainerCreator.CreateSymbolAndBondOrderQueryContainer(query2);
            Assert.IsFalse(new UniversalIsomorphismTester().IsSubgraph(molecule, queryContainer2));
        }
    }
}
