/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 * (or see http://www.gnu.org/copyleft/lesser.html)
 */

using System;
using System.Collections.Generic;

namespace NCDK.Isomorphisms.Matchers.SMARTS
{
    /// <summary>
    /// This class matches a logical operator that connects two query bonds.
    /// </summary>
    // @cdk.module  smarts
    // @cdk.keyword SMARTS
    [Obsolete]
    public class LogicalOperatorBond : SMARTSBond
    {
        /// <summary>
        /// Left child
        /// </summary>
        private IQueryBond left;

        /// <summary>
        /// Name of the operator
        /// </summary>
        private string operator_;

        /// <summary>
        /// Right child
        /// </summary>
        private IQueryBond right;

        public LogicalOperatorBond()
            : base()
        {
        }

        public virtual IQueryBond Left
        {
            get { return left; }
            set { left = value; }
        }

        public virtual string Operator
        {
            get { return operator_; }
            set { operator_ = value; }
        }

        public virtual IQueryBond Right
        {
            get { return right; }
            set { right = value; }
        }

        public override bool Matches(IBond bond)
        {
            bool matchesLeft = left.Matches(bond);
            if (right != null)
            {
                bool matchesRight = right.Matches(bond);
                if (string.Equals("and", operator_, StringComparison.Ordinal))
                {
                    return matchesLeft && matchesRight;
                }
                else if (string.Equals("or", operator_, StringComparison.Ordinal))
                {
                    return matchesLeft || matchesRight;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (string.Equals("not", operator_, StringComparison.Ordinal))
                {
                    return (!matchesLeft);
                }
                else
                {
                    return matchesLeft;
                }
            }
        }

        public override void SetAtoms(IEnumerable<IAtom> atoms)
        {
            base.SetAtoms(atoms);
            left.SetAtoms(atoms);
            if (right != null) right.SetAtoms(atoms);
        }
    }
}
