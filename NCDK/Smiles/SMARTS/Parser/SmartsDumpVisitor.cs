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
using System.Text;

namespace NCDK.Smiles.SMARTS.Parser
{

    /// <summary>
    /// An AST Tree visitor. It dumps the whole AST tree into console
    ///
    // @author Dazhi Jiao
    // @cdk.created 2007-04-24
    // @cdk.module smarts
    // @cdk.githash
    // @cdk.keyword SMARTS AST
    /// </summary>
    public class SmartsDumpVisitor : SMARTSParserVisitor
    {

        public object Visit(ASTRingIdentifier node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node);
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        public object Visit(ASTAtom node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node);
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        private int indent = 0;

        private string IndentString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < indent; ++i)
            {
                sb.Append("  ");
            }
            return sb.ToString();
        }

        public object Visit(SimpleNode node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node + ": acceptor not unimplemented in subclass?");
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        public object Visit(ASTStart node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node);
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        public object Visit(ASTReaction node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node);
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        public object Visit(ASTGroup node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node);
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        public object Visit(ASTSmarts node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node);
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        public object Visit(ASTNotBond node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node);
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        public object Visit(ASTSimpleBond node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node + " [" + node.BondType + "]");
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        public object Visit(ASTImplicitHighAndBond node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node);
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        public object Visit(ASTLowAndBond node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node);
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        public object Visit(ASTOrBond node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node);
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        public object Visit(ASTExplicitHighAndBond node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node);
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        public object Visit(ASTElement node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node + " " + node.Symbol);
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        public object Visit(ASTRecursiveSmartsExpression node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node);
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        public object Visit(ASTPrimitiveAtomExpression node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node);
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        public object Visit(ASTTotalHCount node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node + " " + node.Count);
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        public object Visit(ASTImplicitHCount node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node + " " + node.Count);
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        public object Visit(ASTExplicitConnectivity node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node + " " + node.NumOfConnection);
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        public object Visit(ASTAtomicNumber node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node + " " + node.Number);
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        public object Visit(ASTHybrdizationNumber node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node + " " + node.HybridizationNumber);
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        public object Visit(ASTCharge node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node + " " + node.Charge);
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        public object Visit(ASTRingConnectivity node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node + " " + node.NumOfConnection);
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        public object Visit(ASTPeriodicGroupNumber node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node + " " + node.GroupNumber);
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        public object Visit(ASTTotalConnectivity node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node + " " + node.NumOfConnection);
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        public object Visit(ASTValence node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node + " " + node.Order);
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        public object Visit(ASTRingMembership node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node + " " + node.NumOfMembership);
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        public object Visit(ASTSmallestRingSize node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node + " " + node.Size);
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        public object Visit(ASTAliphatic node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node);
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        public object Visit(ASTNonCHHeavyAtom node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node);
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        public object Visit(ASTAromatic node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node);
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        public object Visit(ASTAnyAtom node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node);
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        public object Visit(ASTAtomicMass node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node + " " + node.Mass);
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        public object Visit(ASTChirality node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node);
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        public object Visit(ASTLowAndExpression node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node);
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        public object Visit(ASTOrExpression node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node);
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        public object Visit(ASTNotExpression node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node);
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        public object Visit(ASTExplicitHighAndExpression node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node);
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        public object Visit(ASTImplicitHighAndExpression node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node);
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }

        public object Visit(ASTExplicitAtom node, object data)
        {
            System.Console.Out.WriteLine(IndentString() + node + " " + node.Symbol);
            ++indent;
            data = node.childrenAccept(this, data);
            --indent;
            return data;
        }
    }
}
