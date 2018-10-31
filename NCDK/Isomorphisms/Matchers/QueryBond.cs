/* Copyright (C) 2010  M.Rijnbeek <markr@ebi.ac.uk>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using System.Text;

namespace NCDK.Isomorphisms.Matchers
{
    /// <summary>
    /// Implements the concept of a "query bond" between two or more atoms.
    /// Query bonds can be used to capture types such as "Single or Double" or "Any".
    /// </summary>
    // @cdk.module isomorphism
    // @cdk.githash
    // @cdk.created 2010-12-16
    public class QueryBond : Silent.Bond, IQueryBond
    {
        private readonly IChemObjectBuilder builder;

        /// <inheritdoc/>
        public override IChemObjectBuilder Builder => builder;

        /// <summary>
        /// The bond expression for this query bond.
        /// </summary>
        public Expr Expression { get; set; } = new Expr(ExprType.True);

        /// <summary>
        /// Constructs an query bond from an expression.
        /// </summary>
        /// <example>
        /// <code>
        /// // pi-bond in a ring
        /// Expr e = new Expr(IS_IN_RING);
        /// e.And(new Expr(ALIPHATIC_ORDER, 2));
        /// new QueryBond(beg, end, e);
        /// </code>
        /// </example>
        /// <param name="expr">the expression</param>
        public QueryBond(IAtom beg, IAtom end, Expr expr)
            : this(beg, end, BondOrder.Unset, BondStereo.None, beg.Builder)
        {
            this.Expression.Set(expr);
            while (Atoms.Count < 2)
                Atoms.Add(null);
        }

        /// <summary>
        /// Constructs an query bond from an expression type.
        /// </summary>
        /// <example>
        /// <code>
        /// new QueryBond(beg, end, IS_IN_RING);
        /// </code>
        /// </example>
        /// <param name="type">the expression type</param>
        public QueryBond(IAtom beg, IAtom end, ExprType type)
            : this(beg, end, BondOrder.Unset, BondStereo.None, beg.Builder)
        {
            this.Expression.SetPrimitive(type);
            while (Atoms.Count < 2)
                Atoms.Add(null);
        }
        /// <summary>
        /// Constructs an query bond from an expression type and value.
        /// </summary>
        /// <example>
        /// <code>
        /// new QueryBond(beg, end, ALIPHATIC_ORDER, 8);
        /// </code></example>
        /// <param name="type">the expression type</param>
        /// <param name="val">the expression value</param>
        public QueryBond(IAtom beg, IAtom end, ExprType type, int val)
            : this(beg, end, BondOrder.Unset, BondStereo.None, beg.Builder)
        {
            this.Expression.SetPrimitive(type, val);
            while (Atoms.Count < 2)
                Atoms.Add(null);
        }

        /// <summary>
        /// Constructs an empty query bond.
        /// </summary>
        public QueryBond(IChemObjectBuilder builder)
        {
            this.builder = builder;
        }

        /// <summary>
        /// Constructs a query bond with a single query bond order..
        /// </summary>
        /// <param name="atom1">the first Atom in the query bond</param>
        /// <param name="atom2">the second Atom in the query bond</param>
        /// <param name="order"></param>
        /// <param name="builder"></param>
        public QueryBond(IAtom atom1, IAtom atom2, BondOrder order, IChemObjectBuilder builder)
            : base(atom1, atom2, order)
        {
            this.builder = builder;
        }

         /// <summary>
         /// Constructs a multi-center query bond, with undefined order and no stereo information.
         /// </summary>
         /// <param name="atoms">An array of IAtom containing the atoms constituting the query bond</param>
        public QueryBond(IAtom[] atoms, IChemObjectBuilder builder)
        {
            this.builder = builder;
            SetAtoms(atoms);
        }

         /// <summary>
         /// Constructs a multi-center query bond, with a specified order and no stereo information.
         /// </summary>
         /// <param name="atoms">An array of IAtom containing the atoms constituting the query bond</param>
         /// <param name="order">The order of the query bond</param>
        public QueryBond(IAtom[] atoms, BondOrder order, IChemObjectBuilder builder)
        {
            this.builder = builder;
            SetAtoms(atoms);
            this.order = order;
        }

        /// <summary>
        /// Constructs a query bond with a given order and stereo orientation from an array of atoms.
        /// </summary>
        /// <param name="atom1">the first Atom in the query bond</param>
        /// <param name="atom2">the second Atom in the query bond</param>
        /// <param name="order">the query bond order</param>
        /// <param name="stereo">a descriptor the stereochemical orientation of this query bond</param>
        public QueryBond(IAtom atom1, IAtom atom2, BondOrder order, BondStereo stereo, IChemObjectBuilder builder)
        {
            this.builder = builder;
            SetAtoms(new[] { atom1, atom2 });
            this.order = order;
            this.stereo = stereo;
        }

        public virtual bool Matches(IBond bond)
        {
            return Expression.Matches(bond);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(nameof(QueryBond) + "(");
            sb.Append(Expression == null ? "" : nameof(Expr) + ":" + Expression.ToString());
            sb.Append(", ").Append(base.ToString());
            sb.Append(')');
            return sb.ToString();
        }
    }
}
