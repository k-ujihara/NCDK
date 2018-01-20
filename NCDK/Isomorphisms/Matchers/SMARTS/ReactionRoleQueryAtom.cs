/*
 * Copyright (c) 2016 John May <jwmay@users.sf.net>
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

using System.Text;

namespace NCDK.Isomorphisms.Matchers.SMARTS
{
    /// <summary>
    /// Matches atoms with a particular role in a reaction.
    /// </summary>
    public class ReactionRoleQueryAtom : SMARTSAtom
    {
        public const int ROLE_REACTANT = 0x1;
        public const int ROLE_AGENT = 0x2;
        public const int ROLE_PRODUCT = 0x4;
        public const int ROLE_ANY = ROLE_REACTANT | ROLE_PRODUCT | ROLE_AGENT;

        private readonly int role;

        public readonly static ReactionRoleQueryAtom RoleReactant = new ReactionRoleQueryAtom(null, ROLE_REACTANT);
        public readonly static ReactionRoleQueryAtom RoleAgent = new ReactionRoleQueryAtom(null, ROLE_AGENT);
        public readonly static ReactionRoleQueryAtom RoleProduct = new ReactionRoleQueryAtom(null, ROLE_PRODUCT);

        public ReactionRoleQueryAtom(IChemObjectBuilder builder, int role)
            : base(builder)
        {
            this.role = role;
        }

        public override bool Matches(IAtom atom)
        {
            ReactionRole? atomRole = atom.GetProperty<ReactionRole?>(CDKPropertyName.ReactionRole);
            if (atomRole == null)
                return this.role == ROLE_ANY;
            switch (atomRole.Value)
            {
                case ReactionRole.Reactant:
                    return (this.role & ROLE_REACTANT) != 0;
                case ReactionRole.Agent:
                    return (this.role & ROLE_AGENT) != 0;
                case ReactionRole.Product:
                    return (this.role & ROLE_PRODUCT) != 0;
                default:
                    return false;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if ((role & ROLE_REACTANT) != 0)
                sb.Append("Reactant");
            if ((role & ROLE_AGENT) != 0)
                sb.Append("Agent");
            if ((role & ROLE_PRODUCT) != 0)
                sb.Append("Product");
            return "ReactionRole(" + sb.ToString() + ")";
        }
    }
}
