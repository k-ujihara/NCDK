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
namespace NCDK.Smiles.SMARTS.Parser
{

    /// <summary>
    /// An AST node. It represents one type of atomic primitive notation in smarts.
    ///
    // @author Dazhi Jiao
    // @cdk.created 2007-04-24
    // @cdk.module smarts
    // @cdk.githash
    // @cdk.keyword SMARTS
    /// </summary>
    public
    class ASTPrimitiveAtomExpression : SimpleNode
    {

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public ASTPrimitiveAtomExpression(int id)
            : base(id)
        {
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public ASTPrimitiveAtomExpression(SMARTSParser p, int id)
            : base(p, id)
        {
        }

        public override object JJTAccept(SMARTSParserVisitor visitor, object data)
        {
            return visitor.Visit(this, data);
        }
    }
}
