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

namespace NCDK.Isomorphisms.Matchers.SMARTS
{
    /// <summary>
    /// This class matches a logical operator that connects two query atoms. Logical
    /// matchers are created with,  <see cref="And(IQueryAtom, IQueryAtom)"/>, 
    /// <see cref="Not(IQueryAtom)"/> and <see cref="Or(IQueryAtom, IQueryAtom)"/>.
    /// </summary>
    // @cdk.module  smarts
    // @cdk.keyword SMARTS
    [Obsolete]
    public class LogicalOperatorAtom : SMARTSAtom
    {
        /// <summary>Left child</summary>
        private IQueryAtom left;

        /// <summary>Name of operator</summary>
        private string operator_;

        /// <summary>Right child</summary>
        private IQueryAtom right;

        public LogicalOperatorAtom()
            : base()
        {
        }

        [Obsolete("use static utility methods to create logical atom matcher, " + nameof(And) + ", " + nameof(Or) + " or " + nameof(Not) + ".")]
        public virtual IQueryAtom Left
        {
            get { return left; }
            set { left = value; }
        }

        [Obsolete("use static utility methods to create logical atom matcher, " + nameof(And) + ", " + nameof(Or) + " or " + nameof(Not) + ".")]
#pragma warning disable CA1716 // Identifiers should not match keywords
        public virtual string Operator
#pragma warning restore CA1716 // Identifiers should not match keywords
        {
            get { return operator_; }
            set { operator_ = value; }
        }

        [Obsolete("use static utility methods to create logical atom matcher, " + nameof(And) + ", " + nameof(Or) + " or " + nameof(Not) + ".")]
        public virtual IQueryAtom Right
        {
            get { return right; }
            set { right = value; }
        }

        /// <summary>
        /// </summary>
        /// <param name="atom"></param>
        /// <returns></returns>
        /// <see cref="Isomorphisms.Matchers.SMARTS.SMARTSAtom.Matches(IAtom)"/>
        public override bool Matches(IAtom atom)
        {
            bool val = false;
            bool matchesLeft = left.Matches(atom);
            if (right != null)
            {
                switch (operator_)
                {
                    case "and":
                        {
                            bool matchesRight = right.Matches(atom);
                            val = matchesLeft && matchesRight;
                        }
                        break;
                    case "or":
                        {
                            bool matchesRight = right.Matches(atom);
                            val = matchesLeft || matchesRight;
                        }
                        break;
                }
            }
            else
            {
                switch (operator_)
                {
                    case "not":
                        val = (!matchesLeft);
                        break;
                    default:
                        val = matchesLeft;
                        break;
                }
            }
            return val;
        }

        /// <summary>
        /// Conjunction the provided expressions.
        /// </summary>
        /// <param name="left">expression</param>
        /// <param name="right">expression</param>
        /// <returns>conjunction of the left and right expressions</returns>
        public static SMARTSAtom And(IQueryAtom left, IQueryAtom right)
        {
            return new Conjunction(left, right);
        }

        /// <summary>
        /// Disjunction the provided expressions.
        /// </summary>
        /// <param name="left">expression</param>
        /// <param name="right">expression</param>
        /// <returns>disjunction of the left and right expressions</returns>
        public static SMARTSAtom Or(IQueryAtom left, IQueryAtom right)
        {
            return new Disjunction(left, right);
        }

        /// <summary>
        /// Negate the provided expression.
        /// </summary>
        /// <param name="expr">expression to negate</param>
        /// <returns>a SMARTS atom which is the negation of the expression</returns>
        public static SMARTSAtom Not(IQueryAtom expr)
        {
            return new Negation(expr);
        }

        /// <summary>Defines a conjunction (AND) between two query atoms.</summary>
        private class Conjunction : LogicalOperatorAtom
        {
            /// <summary>left and right of the operator.</summary>
            private new SMARTSAtom left, right;

            /// <summary>
            /// Create a disjunction of <see cref="Left"/> or <see cref="right"/>.
            /// </summary>
            /// <param name="left">the expression to negate</param>
            /// <param name="right">the expression to negate</param>
            public Conjunction(IQueryAtom left, IQueryAtom right)
                : base()
            {
                this.left = (SMARTSAtom)left;
                this.right = (SMARTSAtom)right;

                base.Left = left;
                base.Right = right;
                base.Operator = "and";
            }

            [Obsolete]
            public override IQueryAtom Left
            {
                get { return base.Left; }
                set { throw new InvalidOperationException("create a new logical atom"); }
            }

            [Obsolete]
            public override string Operator
            {
                get { return base.Operator; }
                set { throw new InvalidOperationException("create a new logical atom"); }
            }

            [Obsolete]
            public override IQueryAtom Right
            {
                get { return base.Right; }
                set { throw new InvalidOperationException("create a new logical atom"); }
            }

            /// <inheritdoc/>
            public override bool Matches(IAtom atom)
            {
                return left.Matches(atom) && right.Matches(atom);
            }

            /// <inheritdoc/>
            public override bool ChiralityMatches(IAtom target, int tParity, int permParity)
            {
                // contract dictates that left.Matches() & right.Matches() are known to be true
                return left.ChiralityMatches(target, tParity, permParity)
                        && right.ChiralityMatches(target, tParity, permParity);
            }
        }

        /// <summary>Defines a disjunction (or) between two query atoms.</summary>
        private class Disjunction : LogicalOperatorAtom
        {
            /// <summary>left of the operator.</summary>
            private new SMARTSAtom left;

            /// <summary>right of the operator.</summary>
            private new SMARTSAtom right;

            /// <summary>
            /// Create a disjunction of <see cref="Left"/> or <see cref="right"/>.
            /// </summary>
            /// <param name="left">the expression to negate</param>
            /// <param name="right">the expression to negate</param>
            public Disjunction(IQueryAtom left, IQueryAtom right)
                : base()
            {
                this.left = (SMARTSAtom)left;
                this.right = (SMARTSAtom)right;
                base.Left = left;
                base.Right = right;
                base.Operator = "or";
            }

            [Obsolete]
            public override IQueryAtom Left
            {
                get { return base.Left; }
                set { throw new InvalidOperationException("create a new logical atom"); }
            }

            [Obsolete]
            public override string Operator
            {
                get { return base.Operator; }
                set { throw new InvalidOperationException("create a new logical atom"); }
            }

            [Obsolete]
            public override IQueryAtom Right
            {
                get { return base.Right; }
                set { throw new InvalidOperationException("create a new logical atom"); }
            }

            /// <inheritdoc/>
            public override bool Matches(IAtom atom)
            {
                return left.Matches(atom) || right.Matches(atom);
            }

            /// <inheritdoc/>
            public override bool ChiralityMatches(IAtom target, int tParity, int permParity)
            {
                // we know the left or right was true, for each side which matched try to verify
                // the chirality
                return left.Matches(target) && left.ChiralityMatches(target, tParity, permParity) || right.Matches(target)
                        && right.ChiralityMatches(target, tParity, permParity);
            }
        }

        /// <summary>Defines a negation (not) of a query atom.</summary>
        private class Negation : LogicalOperatorAtom
        {
            /// <summary>Expression to negate.</summary>
            private SMARTSAtom expression;

            /// <summary>Is the expression chiral - if so, always true!</summary>
            private readonly bool chiral;

            /// <summary>
            /// Create a negation of <paramref name="expression"/>.
            /// </summary>
            /// <param name="expression">the expression to negate</param>
            public Negation(IQueryAtom expression)
                : base()
            {
                this.expression = (SMARTSAtom)expression;
                this.chiral = expression.GetType().Equals(typeof(ChiralityAtom));
                base.Left = expression;
                base.Operator = "not";
            }

            [Obsolete]
            public override IQueryAtom Left
            {
                get { return base.Left; }
                set { throw new InvalidOperationException("create a new logical atom"); }
            }

            [Obsolete]
            public override string Operator
            {
                get { return base.Operator; }
                set { throw new InvalidOperationException("create a new logical atom"); }
            }

            [Obsolete]
            public override IQueryAtom Right
            {
                get { return base.Right; }
                set { throw new InvalidOperationException("create a new logical atom"); }
            }

            /// <inheritdoc/>
            public override bool Matches(IAtom atom)
            {
                return chiral || !expression.Matches(atom);
            }

            /// <inheritdoc/>
            public override bool ChiralityMatches(IAtom target, int tParity, int permParity)
            {
                return !expression.ChiralityMatches(target, tParity, permParity);
            }
        }
    }
}
