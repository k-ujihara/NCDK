/*  Copyright (C) 2004-2008  The Chemistry Development Kit (CDK) project
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT Any WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Isomorphisms.Matchers.SMARTS;
using System.Collections.Generic;

namespace NCDK.Isomorphisms.Matchers
{
    /// <summary>
    ///@cdk.module   isomorphism
    // @cdk.githash
    /// </summary>
    public class QueryAtomContainerCreator
    {

        /// <summary>
        /// Creates a QueryAtomContainer with SymbolQueryAtom's, AromaticQueryBond's and
        /// OrderQueryBond's. If a IBond of the input <code>container</code> is flagged
        /// aromatic, then it disregards bond order information and only match against
        /// an aromatic target atom instead.
        ///
        /// <param name="container">The AtomContainer that stands as model</param>
        /// <returns>The new QueryAtomContainer created from container.</returns>
        /// </summary>
        public static QueryAtomContainer CreateBasicQueryContainer(IAtomContainer container)
        {
            QueryAtomContainer queryContainer = new QueryAtomContainer(container.Builder);
            for (int i = 0; i < container.Atoms.Count; i++)
            {
                queryContainer.Atoms.Add(new SymbolQueryAtom(container.Atoms[i]));
            }
            foreach (var bond in container.Bonds)
            {
                int index1 = container.Atoms.IndexOf(bond.Atoms[0]);
                int index2 = container.Atoms.IndexOf(bond.Atoms[1]);
                if (bond.IsAromatic)
                {
                    queryContainer.Bonds.Add(new AromaticQueryBond((IQueryAtom)queryContainer.Atoms[index1],
                        (IQueryAtom)queryContainer.Atoms[index2], container.Builder));
                }
                else
                {
                    queryContainer.Bonds.Add(new OrderQueryBond((IQueryAtom)queryContainer.Atoms[index1],
                            (IQueryAtom)queryContainer.Atoms[index2], bond.Order, container.Builder));
                }
            }
            return queryContainer;
        }

        /// <summary>
        /// Creates a QueryAtomContainer with SymbolQueryAtom's and OrderQueryBond's. Unlike
        /// <code>createBasicQueryContainer</code>, it disregards aromaticity flags.
        ///
        /// <param name="container">The AtomContainer that stands as model</param>
        /// <returns>The new QueryAtomContainer created from container.</returns>
        /// </summary>
        public static QueryAtomContainer CreateSymbolAndBondOrderQueryContainer(IAtomContainer container)
        {
            QueryAtomContainer queryContainer = new QueryAtomContainer(container.Builder);
            for (int i = 0; i < container.Atoms.Count; i++)
            {
                queryContainer.Atoms.Add(new SymbolQueryAtom(container.Atoms[i]));
            }
            foreach (var bond in container.Bonds)
            {
                int index1 = container.Atoms.IndexOf(bond.Atoms[0]);
                int index2 = container.Atoms.IndexOf(bond.Atoms[1]);
                queryContainer.Bonds.Add(new OrderQueryBondOrderOnly((IQueryAtom)queryContainer.Atoms[index1],
                        (IQueryAtom)queryContainer.Atoms[index2], bond.Order, container.Builder));
            }
            return queryContainer;
        }

        /// <summary>
        ///  Creates a QueryAtomContainer with SymbolAncChargeQueryAtom's and
        ///  OrderQueryBond's.
        ///
        /// <param name="container">The AtomContainer that stands as model</param>
        /// <returns>The new QueryAtomContainer created from container.</returns>
        /// </summary>
        public static QueryAtomContainer CreateSymbolAndChargeQueryContainer(IAtomContainer container)
        {
            QueryAtomContainer queryContainer = new QueryAtomContainer(container.Builder);
            for (int i = 0; i < container.Atoms.Count; i++)
            {
                queryContainer.Atoms.Add(new SymbolAndChargeQueryAtom(container.Atoms[i]));
            }
            foreach (var bond in container.Bonds)
            {
                int index1 = container.Atoms.IndexOf(bond.Atoms[0]);
                int index2 = container.Atoms.IndexOf(bond.Atoms[1]);
                if (bond.IsAromatic)
                {
                    queryContainer.Bonds.Add(new AromaticQueryBond((IQueryAtom)queryContainer.Atoms[index1],
                            (IQueryAtom)queryContainer.Atoms[index2], container.Builder));
                }
                else
                {
                    queryContainer.Bonds.Add(new OrderQueryBond((IQueryAtom)queryContainer.Atoms[index1],
                            (IQueryAtom)queryContainer.Atoms[index2], bond.Order, container.Builder));
                }
            }
            return queryContainer;
        }

        public static QueryAtomContainer CreateSymbolChargeIDQueryContainer(IAtomContainer container)
        {
            QueryAtomContainer queryContainer = new QueryAtomContainer(container.Builder);
            for (int i = 0; i < container.Atoms.Count; i++)
            {
                queryContainer.Atoms.Add(new SymbolChargeIDQueryAtom(container.Atoms[i]));
            }
            foreach (var bond in container.Bonds)
            {
                int index1 = container.Atoms.IndexOf(bond.Atoms[0]);
                int index2 = container.Atoms.IndexOf(bond.Atoms[1]);
                if (bond.IsAromatic)
                {
                    queryContainer.Bonds.Add(new AromaticQueryBond((IQueryAtom)queryContainer.Atoms[index1],
                            (IQueryAtom)queryContainer.Atoms[index2], container.Builder));
                }
                else
                {
                    queryContainer.Bonds.Add(new OrderQueryBond((IQueryAtom)queryContainer.Atoms[index1],
                            (IQueryAtom)queryContainer.Atoms[index2], bond.Order, container.Builder));
                }
            }
            return queryContainer;
        }

        /// <summary>
        ///  Creates a QueryAtomContainer with AnyAtoms / Aromatic Atoms and OrderQueryBonds / AromaticQueryBonds.
        ///  It uses the CDKConstants.ISAROMATIC flag to determine the aromaticity of container.
        ///
        /// <param name="container">The AtomContainer that stands as model</param>
        /// <param name="aromaticity">True = use aromaticity flags to create AtomaticAtoms and AromaticQueryBonds</param>
        /// <returns>The new QueryAtomContainer created from container</returns>
        /// </summary>
        public static QueryAtomContainer CreateAnyAtomContainer(IAtomContainer container, bool aromaticity)
        {
            QueryAtomContainer queryContainer = new QueryAtomContainer(container.Builder);

            for (int i = 0; i < container.Atoms.Count; i++)
            {
                if (aromaticity && container.Atoms[i].IsAromatic)
                {
                    queryContainer.Atoms.Add(new AromaticAtom(container.Builder));
                }
                else
                {
                    queryContainer.Atoms.Add(new AnyAtom(container.Builder));
                }
            }

            foreach (var bond in container.Bonds)
            {
                int index1 = container.Atoms.IndexOf(bond.Atoms[0]);
                int index2 = container.Atoms.IndexOf(bond.Atoms[1]);
                if (aromaticity && bond.IsAromatic)
                {
                    queryContainer.Bonds.Add(new AromaticQueryBond((IQueryAtom)queryContainer.Atoms[index1],
                            (IQueryAtom)queryContainer.Atoms[index2], container.Builder));
                }
                else
                {
                    queryContainer.Bonds.Add(new OrderQueryBond((IQueryAtom)queryContainer.Atoms[index1],
                            (IQueryAtom)queryContainer.Atoms[index2], bond.Order, container.Builder));
                }
            }
            return queryContainer;
        }

        /// <summary>
        /// Creates a QueryAtomContainer with wildcard atoms and wildcard bonds.
        /// <p/>
        /// This method thus allows the user to search based only on connectivity.
        ///
        /// <param name="container">The AtomContainer that stands as the model</param>
        /// <param name="aromaticity">If True, aromaticity flags are checked to create AromaticAtoms</param>
        ///                    and AromaticQueryBonds
        /// <returns>The new QueryAtomContainer</returns>
        /// </summary>
        public static QueryAtomContainer CreateAnyAtomAnyBondContainer(IAtomContainer container, bool aromaticity)
        {
            QueryAtomContainer queryContainer = new QueryAtomContainer(container.Builder);

            for (int i = 0; i < container.Atoms.Count; i++)
            {
                if (aromaticity && container.Atoms[i].IsAromatic)
                {
                    queryContainer.Atoms.Add(new AromaticAtom(container.Builder));
                }
                else
                {
                    queryContainer.Atoms.Add(new AnyAtom(container.Builder));
                }
            }

            foreach (var bond in container.Bonds)
            {
                int index1 = container.Atoms.IndexOf(bond.Atoms[0]);
                int index2 = container.Atoms.IndexOf(bond.Atoms[1]);
                queryContainer.Bonds.Add(new AnyOrderBond(queryContainer.Atoms[index1], queryContainer.Atoms[index2],
                        container.Builder));
            }
            return queryContainer;
        }

        /// <summary>
        ///  Creates a QueryAtomContainer with SymbolQueryAtom's and
        ///  OrderQueryBond's. Each PseudoAtom will be replaced by a
        ///  AnyAtom
        ///
        /// <param name="container">The AtomContainer that stands as model</param>
        /// <returns>The new QueryAtomContainer created from container.</returns>
        /// </summary>
        public static QueryAtomContainer CreateAnyAtomForPseudoAtomQueryContainer(IAtomContainer container)
        {
            QueryAtomContainer queryContainer = new QueryAtomContainer(container.Builder);
            for (int i = 0; i < container.Atoms.Count; i++)
            {
                if (container.Atoms[i] is IPseudoAtom)
                {
                    queryContainer.Atoms.Add(new AnyAtom(container.Builder));
                }
                else
                {
                    queryContainer.Atoms.Add(new SymbolQueryAtom(container.Atoms[i]));
                }

            }
            foreach (var bond in container.Bonds)
            {
                int index1 = container.Atoms.IndexOf(bond.Atoms[0]);
                int index2 = container.Atoms.IndexOf(bond.Atoms[1]);
                if (bond.IsAromatic)
                {
                    queryContainer.Bonds.Add(new AromaticQueryBond((IQueryAtom)queryContainer.Atoms[index1],
                            (IQueryAtom)queryContainer.Atoms[index2], container.Builder));
                }
                else
                {
                    queryContainer.Bonds.Add(new OrderQueryBond((IQueryAtom)queryContainer.Atoms[index1],
                            (IQueryAtom)queryContainer.Atoms[index2], bond.Order, container.Builder));
                }
            }
            return queryContainer;
        }

        /// <summary>Match any atom.</summary>
        private sealed class AnyAtom : QueryAtom
        {
            public AnyAtom(IChemObjectBuilder builder)
                : base(builder)
            { }

            /// <inheritdoc/>
            public override bool Matches(IAtom atom)
            {
                return true;
            }
        }

        /// <summary>Match any aromatic atom.</summary>
        private sealed class AromaticAtom : QueryAtom
        {
            public AromaticAtom(IChemObjectBuilder builder)
                    : base(builder)
            { }

            /// <inheritdoc/>

            public override bool Matches(IAtom atom)
            {
                return atom.IsAromatic;
            }
        }

        /// <summary>Match any bond which doesn't have a null or unset bond order.</summary>
        private sealed class AnyOrderBond : QueryBond
        {
            public AnyOrderBond(IAtom either, IAtom other, IChemObjectBuilder builder)
                : base(either, other, BondOrder.Unset, builder)
            { }

            /// <inheritdoc/>
            public override bool Matches(IBond bond)
            {
                return bond != null && bond.Order != BondOrder.Unset;
            }
        }

        /// <summary>Match any aromatic bond.</summary>
        private sealed class AromaticQueryBond : QueryBond
        {
            public AromaticQueryBond(IAtom either, IAtom other, IChemObjectBuilder builder)
                : base(either, other, BondOrder.Unset, builder)
            { }

            /// <inheritdoc/>
            public override bool Matches(IBond bond)
            {
                return bond.IsAromatic;
            }
        }
    }
}
