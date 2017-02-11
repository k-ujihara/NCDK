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
using System.Collections.Generic;

namespace NCDK.Isomorphisms.Matchers.SMARTS
{
    /**
	 * This class matches a logical operator that connects two query bonds.
	 *
	 * @cdk.module  smarts
	 * @cdk.githash
	 * @cdk.keyword SMARTS
	 */
    public class LogicalOperatorBond : SMARTSBond
    {
        /**
		 * Left child
		 */
        private IQueryBond left;

        /**
		 * Name of the operator
		 */
        private string operator_;

        /**
		 * Right child
		 */
        private IQueryBond right;

        public LogicalOperatorBond(IChemObjectBuilder builder)
            : base(builder)
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
                if ("and".Equals(operator_))
                {
                    return matchesLeft && matchesRight;
                }
                else if ("or".Equals(operator_))
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
                if ("not".Equals(operator_))
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

