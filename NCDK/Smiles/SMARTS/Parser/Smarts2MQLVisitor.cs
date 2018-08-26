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

namespace NCDK.Smiles.SMARTS.Parser
{
    /// <summary>
    /// An AST tree visitor. It is a prototype that translate Smarts to MQL.
    /// It is far from fully functioning.
    /// </summary>
    // @author Dazhi Jiao
    // @cdk.created 2007-04-24
    // @cdk.module smarts
    // @cdk.githash
    // @cdk.keyword SMARTS AST
    internal class Smarts2MQLVisitor : ISMARTSParserVisitor
    {
        public object Visit(ASTRingIdentifier node, object data)
        {
            // TODO Auto-generated method stub
            return null;
        }

        string symbolIdentified = "";
        public bool Not { get; set; } = false;

        public object Visit(ASTAtom node, object data)
        {
            // TODO Auto-generated method stub
            return null;
        }

        public object Visit(SimpleNode node, object data)
        {
            return node.ChildrenAccept(this, data);
        }

        public object Visit(ASTStart node, object data)
        {
            return node.JjtGetChild(0).JjtAccept(this, data);
        }

        public object Visit(ASTReaction node, object data)
        {
            return node.JjtGetChild(0).JjtAccept(this, data);
        }

        public object Visit(ASTGroup node, object data)
        {
            return node.JjtGetChild(0).JjtAccept(this, data);
        }

        public object Visit(ASTSmarts node, object data)
        {
            string local = "";
            for (int i = 0; i < node.JjtGetNumChildren(); i++)
            {
                INode child = node.JjtGetChild(i);
                if (child is ASTAtom)
                {
                    local = (string)child.JjtAccept(this, local);
                }
                else if (child is ASTLowAndBond)
                {
                    i++;
                    INode nextChild = node.JjtGetChild(i); // the next child should
                                                          // be another smarts
                    string bond = (string)child.JjtAccept(this, local);
                    local = local + bond;
                    local = (string)nextChild.JjtAccept(this, local);
                }
                else if (child is ASTSmarts)
                { // implicit single bond
                    if (local.Length != 0)
                        local = local + "-";
                    local = (string)child.JjtAccept(this, local);
                }
                else if (child is ASTExplicitAtom)
                {
                    if (local.Length != 0)
                        local = local + "-";
                    local = (string)child.JjtAccept(this, local);
                }
            }
            return data + local;
        }

        // TODO: Accept only one bond. Need to find out whether MQL supports
        // logical bonds
        public object Visit(ASTLowAndBond node, object data)
        {
            return node.JjtGetChild(0).JjtAccept(this, data);
        }

        // TODO: Accept only one bond. Need to find out whether MQL supports
        // logical bonds
        public object Visit(ASTOrBond node, object data)
        {
            return node.JjtGetChild(0).JjtAccept(this, data);
        }

        // TODO: Accept only one bond. Need to find out whether MQL supports
        // logical bonds
        public object Visit(ASTExplicitHighAndBond node, object data)
        {
            return node.JjtGetChild(0).JjtAccept(this, data);
        }

        // TODO: Accept only one bond. Need to find out whether MQL supports
        // logical bonds
        public object Visit(ASTImplicitHighAndBond node, object data)
        {
            return node.JjtGetChild(0).JjtAccept(this, data);
        }

        // TODO: Accept only one bond. Need to find out whether MQL supports
        // logical bonds
        public object Visit(ASTNotBond node, object data)
        {
            return node.JjtGetChild(0).JjtAccept(this, data);
        }

        public object Visit(ASTSimpleBond node, object data)
        {
            string bond = "";
            int bondType = node.BondType;
            switch (bondType)
            {
                case SMARTSParserConstants.ANY_BOND:
                    bond = "~";
                    break;
                case SMARTSParserConstants.S_BOND:
                    bond = "-";
                    break;
                case SMARTSParserConstants.D_BOND:
                    bond = "=";
                    break;
                case SMARTSParserConstants.T_BOND:
                    bond = "#";
                    break;
                case SMARTSParserConstants.AR_BOND:
                    bond = ":";
                    break;
                case SMARTSParserConstants.R_BOND:
                    bond = "$~1"; // TODO: only one ring is assumed here. Should handle more
                    break;
                case SMARTSParserConstants.UP_S_BOND:
                case SMARTSParserConstants.DN_S_BOND:
                case SMARTSParserConstants.UP_OR_UNSPECIFIED_S_BOND:
                case SMARTSParserConstants.DN_OR_UNSPECIFIED_S_BOND:
                    bond = "-";
                    break;
            }
            return bond;
        }

        public object Visit(ASTExplicitAtom node, object data)
        {
            data = data + node.Symbol; // TODO: ring handling!
            return data;
        }

        public object Visit(ASTLowAndExpression node, object data)
        {
            string left = (string)node.JjtGetChild(0).JjtAccept(this, data);
            if (node.JjtGetNumChildren() == 1)
            {
                return left;
            }
            string right = (string)node.JjtGetChild(1).JjtAccept(this, data);
            if (left.Length == 0)
            {
                return right;
            }
            else if (right.Length == 0)
            {
                return left;
            }
            else
            {
                return left + "&" + right;
            }
        }

        public object Visit(ASTOrExpression node, object data)
        {
            string left = (string)node.JjtGetChild(0).JjtAccept(this, data);
            if (node.JjtGetNumChildren() == 1)
            {
                return left;
            }
            string right = (string)node.JjtGetChild(1).JjtAccept(this, data);
            if (left.Length == 0)
            {
                return right;
            }
            else if (right.Length == 0)
            {
                return left;
            }
            else
            {
                return left + "|" + right;
            }
        }

        // TODO: the precedence needs to be addressed
        public object Visit(ASTExplicitHighAndExpression node, object data)
        {
            string left = (string)node.JjtGetChild(0).JjtAccept(this, data);
            if (node.JjtGetNumChildren() == 1)
            {
                return left;
            }
            string right = (string)node.JjtGetChild(1).JjtAccept(this, data);
            if (left.Length == 0)
            {
                return right;
            }
            else if (right.Length == 0)
            {
                return left;
            }
            else
            {
                return left + "&" + right;
            }
        }

        //  TODO: the precedence needs to be addressed
        public object Visit(ASTImplicitHighAndExpression node, object data)
        {
            string left = (string)node.JjtGetChild(0).JjtAccept(this, data);
            if (node.JjtGetNumChildren() == 1)
            {
                return left;
            }
            string right = (string)node.JjtGetChild(1).JjtAccept(this, data);
            if (left.Length == 0)
            {
                return right;
            }
            else if (right.Length == 0)
            {
                return left;
            }
            else
            {
                return left + "&" + right;
            }
        }

        public object Visit(ASTNotExpression node, object data)
        {
            // well, I know there is a not in MQL :)
            if (node.Type == SMARTSParserConstants.NOT)
            {
                Not = true;
            }
            else
            {
                Not = false;
            }
            string str = "";
            for (int i = 0; i < node.JjtGetNumChildren(); i++)
            {
                str += node.JjtGetChild(i).JjtAccept(this, data);
            }
            return str;
        }

        // TODO: I don't think this is implemented in MQL. Throw an exception/warning?
        public object Visit(ASTRecursiveSmartsExpression node, object data)
        {
            return null;
        }

        public object Visit(ASTTotalHCount node, object data)
        {
            //      TODO: a property? not sure. just making things up here :)
            return data;
        }

        public object Visit(ASTImplicitHCount node, object data)
        {
            // TODO: a property? not sure. just making things up here :)
            return data;
        }

        public object Visit(ASTExplicitConnectivity node, object data)
        {
            //      TODO: a property? not sure. just making things up here :)
            return data;
        }

        public object Visit(ASTAtomicNumber node, object data)
        {
            //      TODO: a property? not sure. just making things up here :)
            return data;
        }

        public object Visit(ASTHybrdizationNumber node, object data)
        {
            return data;
        }

        public object Visit(ASTCharge node, object data)
        {
            //      TODO: a property? not sure. just making things up here :)
            return data;
        }

        public object Visit(ASTRingConnectivity node, object data)
        {

            return data;
        }

        public object Visit(ASTPeriodicGroupNumber node, object data)
        {
            return data;
        }

        public object Visit(ASTTotalConnectivity node, object data)
        {
            //      TODO: a property? not sure. just making things up here :)
            return data;
        }

        public object Visit(ASTValence node, object data)
        {
            //      TODO: a property? not sure. just making things up here :)
            return data;
        }

        public object Visit(ASTRingMembership node, object data)
        {
            //      TODO: "ring" is a property, but how about the number of rings?
            return "ring";
        }

        public object Visit(ASTSmallestRingSize node, object data)
        {
            return data;
        }

        public object Visit(ASTAliphatic node, object data)
        {
            return data;
        }

        public object Visit(ASTNonCHHeavyAtom node, object data)
        {
            return data;
        }

        public object Visit(ASTAromatic node, object data)
        {
            return data;
        }

        public object Visit(ASTAnyAtom node, object data)
        {
            return data;
        }

        public object Visit(ASTAtomicMass node, object data)
        {
            return data;
        }

        public object Visit(ASTChirality node, object data)
        {
            return data;
        }

        public object Visit(ASTElement node, object data)
        {
            symbolIdentified = node.Symbol;
            return "";
        }
    }
}
