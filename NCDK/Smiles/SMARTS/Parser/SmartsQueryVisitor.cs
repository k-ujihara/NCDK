#pragma warning disable 0162

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

using NCDK.Common.Collections;
using NCDK.Isomorphisms;
using NCDK.Isomorphisms.Matchers;
using NCDK.Isomorphisms.Matchers.SMARTS;
using NCDK.Stereo;
using NCDK.Tools.Manipulator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Smarts = NCDK.Isomorphisms.Matchers.SMARTS;

namespace NCDK.Smiles.SMARTS.Parser
{
    /// <summary>
    /// An AST tree visitor. It builds an instance of <see cref="QueryAtomContainer"/> 
    /// from the AST tree.
    /// </summary>
    /// <example>
    /// To use this visitor:
    /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Smiles.SMARTS.Parser.SmartsQueryVisitor_Example.cs+1"]/*' />
    /// </example>
    // @author Dazhi Jiao
    // @cdk.created 2007-04-24
    // @cdk.module smarts
    // @cdk.githash
    // @cdk.keyword SMARTS AST
    internal class SmartsQueryVisitor : ISMARTSParserVisitor
    {
        // current atoms with a ring identifier
        private RingIdentifierAtom[] ringAtoms;

        // query
        private IQueryAtomContainer query;

        private readonly IChemObjectBuilder builder;

        /// <summary>
        /// Maintain order of neighboring atoms - required for atom-based
        /// stereochemistry.
        /// </summary>
        private IDictionary<IAtom, IList<IAtom>> neighbors = new Dictionary<IAtom, IList<IAtom>>();

        /// <summary>
        /// Lookup of atom indices.
        /// </summary>
        private readonly BitArray tetrahedral = new BitArray(0);

        /// <summary>
        /// Stores the directional '/' or '\' bonds. Speeds up looking for double
        /// bond configurations.
        /// </summary>
        private List<IBond> stereoBonds = new List<IBond>();

        /// <summary>
        /// Stores the double bonds in the query.
        /// </summary>
        private List<IBond> doubleBonds = new List<IBond>();

        public SmartsQueryVisitor(IChemObjectBuilder builder)
        {
            this.builder = builder;
        }

        public object Visit(ASTRingIdentifier node, object data)
        {
            IQueryAtom atom = (IQueryAtom)data;
            RingIdentifierAtom ringIdAtom = new RingIdentifierAtom(builder)
            {
                Atom = atom
            };
            IQueryBond bond;
            if (node.JjtGetNumChildren() == 0)
            { 
                // implicit bond
                bond = null;
            }
            else
            {
                bond = (IQueryBond)node.JjtGetChild(0).JjtAccept(this, data);
            }
            ringIdAtom.RingBond = bond;
            return ringIdAtom;
        }

        public object Visit(ASTAtom node, object data)
        {
            IQueryAtom atom = (IQueryAtom)node.JjtGetChild(0).JjtAccept(this, data);
            for (int i = 1; i < node.JjtGetNumChildren(); i++)
            {
                // if there are ring identifiers
                throw new InvalidOperationException();
            }
            return atom;
        }

        private void HandleRingClosure(IQueryAtom atom, ASTRingIdentifier ringIdentifier)
        {
            RingIdentifierAtom ringIdAtom = (RingIdentifierAtom)ringIdentifier.JjtAccept(this, atom);

            // if there is already a RingIdentifierAtom, create a bond between
            // them and add the bond to the query
            int ringId = ringIdentifier.RingId;

            // ring digit > 9 - expand capacity
            if (ringId >= ringAtoms.Length) ringAtoms = Arrays.CopyOf(ringAtoms, 100);

            // Ring Open
            if (ringAtoms[ringId] == null)
            {
                ringAtoms[ringId] = ringIdAtom;
                if (neighbors.ContainsKey(atom))
                {
                    neighbors[atom].Add(ringIdAtom);
                }
            }

            // Ring Close
            else
            {
                IQueryBond ringBond;
                // first check if the two bonds ma
                if (ringAtoms[ringId].RingBond == null)
                {
                    if (ringIdAtom.RingBond == null)
                    {
                        if (atom is AromaticSymbolAtom
                                && ringAtoms[ringId].Atom is AromaticSymbolAtom)
                        {
                            ringBond = new AromaticQueryBond(builder);
                        }
                        else
                        {
                            ringBond = new RingBond(builder);
                        }
                    }
                    else
                    {
                        ringBond = ringIdAtom.RingBond;
                    }
                }
                else
                {
                    // Here I assume the bond are always same. This should be checked by the parser already
                    ringBond = ringAtoms[ringId].RingBond;
                }
                ((IBond)ringBond).SetAtoms(new[] { ringAtoms[ringId].Atom, atom });
                query.Bonds.Add((IBond)ringBond);

                // if the connected atoms was tracking neighbors, replace the
                // placeholder reference
                if (neighbors.ContainsKey(ringAtoms[ringId].Atom))
                {
                    IList<IAtom> localNeighbors = neighbors[ringAtoms[ringId].Atom];
                    localNeighbors[localNeighbors.IndexOf(ringAtoms[ringId])] = atom;
                }
                if (neighbors.ContainsKey(atom))
                {
                    neighbors[atom].Add(ringAtoms[ringId].Atom);
                }
                ringAtoms[ringId] = null;
            }
        }

        public object Visit(SimpleNode node, object data)
        {
            return null;
        }

        public object Visit(ASTStart node, object data)
        {
            return node.JjtGetChild(0).JjtAccept(this, data);
        }

        public object Visit(ASTReaction node, object data)
        {
            IAtomContainer query = new QueryAtomContainer(builder);
            for (int grpIdx = 0; grpIdx < node.JjtGetNumChildren(); grpIdx++)
            {

                int rollback = query.Atoms.Count;

                ASTGroup group = (ASTGroup)node.JjtGetChild(grpIdx);
                group.JjtAccept(this, query);

                // fill in the roles for newly create atoms
                if (group.Role != ReactionRoles.Any)
                {
                    IQueryAtom roleQueryAtom = null;
                    ReactionRole? role = null;

                    // use single instances
                    switch (group.Role)
                    {
                        case ReactionRoles.Reactant:
                            roleQueryAtom = ReactionRoleQueryAtom.RoleReactant;
                            role = ReactionRole.Reactant;
                            break;
                        case ReactionRoles.Agent:
                            roleQueryAtom = ReactionRoleQueryAtom.RoleAgent;
                            role = ReactionRole.Agent;
                            break;
                        case ReactionRoles.Product:
                            roleQueryAtom = ReactionRoleQueryAtom.RoleProduct;
                            role = ReactionRole.Product;
                            break;
                    }

                    if (roleQueryAtom != null)
                    {
                        while (rollback < query.Atoms.Count)
                        {
                            IAtom org = query.Atoms[rollback];
                            IAtom rep = LogicalOperatorAtom.And(roleQueryAtom, (IQueryAtom)org);
                            // ensure AAM is propagated
                            rep.SetProperty(CDKPropertyName.AtomAtomMapping, org.GetProperty<int?>(CDKPropertyName.AtomAtomMapping));
                            rep.SetProperty(CDKPropertyName.ReactionRole, role);
                            AtomContainerManipulator.ReplaceAtomByAtom(query, org, rep);
                            rollback++;
                        }
                    }
                }
            }
            return query;
        }

        public object Visit(ASTGroup node, object data)
        {
            IAtomContainer fullQuery = (IAtomContainer)data;

            if (fullQuery == null)
                fullQuery = new QueryAtomContainer(builder);

            // keeps track of component grouping
            int[] components = fullQuery.GetProperty<int[]>(ComponentGrouping.Key, Array.Empty<int>());
            int maxId = 0;
            if (components.Length > 0)
            {
                foreach (int id in components)
                    if (id > maxId) maxId = id;
            }

            for (int i = 0; i < node.JjtGetNumChildren(); i++)
            {
                ASTSmarts smarts = (ASTSmarts)node.JjtGetChild(i);
                ringAtoms = new RingIdentifierAtom[10];
                query = new QueryAtomContainer(builder);

                smarts.JjtAccept(this, null);

                // update component info
                if (components.Length > 0 || smarts.ComponentId > 0)
                {
                    components = Arrays.CopyOf(components, 1 + fullQuery.Atoms.Count + query.Atoms.Count);
                    int id = smarts.ComponentId;
                    Arrays.Fill(components, fullQuery.Atoms.Count, components.Length, id);
                    if (id > maxId) maxId = id;
                }

                fullQuery.Add(query);
            }

            // only store if there was a component grouping
            if (maxId > 0)
            {
                components[components.Length - 1] = maxId; // we left space to store how many groups there were
                fullQuery.SetProperty(ComponentGrouping.Key, components);
            }

            // create tetrahedral elements
            foreach (var atom in neighbors.Keys)
            {
                IList<IAtom> localNeighbors = neighbors[atom];
                if (localNeighbors.Count == 4)
                {
                    fullQuery.StereoElements.Add(new TetrahedralChirality(atom, localNeighbors.ToArray(),
                        TetrahedralStereo.Clockwise)); // <- to be modified later
                }
                else if (localNeighbors.Count == 5)
                {
                    localNeighbors.Remove(atom); // remove central atom (which represented implicit part)
                    fullQuery.StereoElements.Add(new TetrahedralChirality(atom, localNeighbors.ToArray(),
                        TetrahedralStereo.Clockwise)); // <- to be modified later
                }
            }

            // for each double bond, find the stereo bonds. Currently doesn't
            // handle logical bonds i.e. C/C-,=C/C
            foreach (var bond in doubleBonds)
            {
                IAtom left = bond.Begin;
                IAtom right = bond.End;
                StereoBond leftBond = FindStereoBond(left);
                StereoBond rightBond = FindStereoBond(right);
                if (leftBond == null || rightBond == null) continue;
                DoubleBondConformation conformation = leftBond.GetDirection(left) == rightBond.GetDirection(right) ? DoubleBondConformation.Together
                       : DoubleBondConformation.Opposite;
                fullQuery.StereoElements.Add(new DoubleBondStereochemistry(bond, new IBond[] { leftBond, rightBond },
                        conformation));
            }

            return fullQuery;
        }

        /// <summary>
        /// Locate a stereo bond adjacent to the <paramref name="atom"/>.
        /// </summary>
        /// <param name="atom">an atom</param>
        /// <returns>a stereo bond or null if non found</returns>
        private StereoBond FindStereoBond(IAtom atom)
        {
            foreach (var bond in stereoBonds)
                if (bond.Contains(atom)) return (StereoBond)bond;
            return null;
        }

        public object Visit(ASTSmarts node, object data)
        {
            SMARTSAtom atom = null;
            SMARTSBond bond = null;

            ASTAtom first = (ASTAtom)node.JjtGetChild(0);
            atom = (SMARTSAtom)first.JjtAccept(this, null);
            if (data != null)
            { // this is a sub smarts
                bond = (SMARTSBond)((object[])data)[1];
                IAtom prev = (SMARTSAtom)((object[])data)[0];
                if (bond == null)
                { // since no bond was specified it could be aromatic or single
                    bond = new AromaticOrSingleQueryBond(builder);
                    bond.SetAtoms(new[] { prev, atom });
                }
                else
                {
                    bond.SetAtoms(new[] { prev, atom });
                }
                if (neighbors.ContainsKey(prev))
                {
                    neighbors[prev].Add(atom);
                }
                query.Bonds.Add(bond);
                bond = null;
            }
            // first ATOM in expression
            query.Atoms.Add(atom);

            if (BitArrays.GetValue(tetrahedral, query.Atoms.Count - 1))
            {
                List<IAtom> localNeighbors = new List<IAtom>(query.GetConnectedAtoms(atom))
                {
                    atom
                };
                neighbors[atom] = localNeighbors;
            }

            // now process the rest of the bonds/atoms
            for (int i = 1; i < node.JjtGetNumChildren(); i++)
            {
                INode child = node.JjtGetChild(i);
                if (child is ASTLowAndBond)
                {
                    bond = (SMARTSBond)child.JjtAccept(this, data);
                }
                else if (child is ASTAtom)
                {
                    SMARTSAtom newAtom = (SMARTSAtom)child.JjtAccept(this, null);
                    if (bond == null)
                    { // since no bond was specified it could be aromatic or single
                        bond = new AromaticOrSingleQueryBond(builder);
                    }
                    bond.SetAtoms(new[] { atom, newAtom });
                    query.Bonds.Add(bond);
                    query.Atoms.Add(newAtom);

                    if (neighbors.ContainsKey(atom))
                    {
                        neighbors[atom].Add(newAtom);
                    }
                    if (BitArrays.GetValue(tetrahedral, query.Atoms.Count - 1))
                    {
                        List<IAtom> localNeighbors = new List<IAtom>(query.GetConnectedAtoms(newAtom))
                        {
                            newAtom
                        };
                        neighbors[newAtom] = localNeighbors;
                    }

                    atom = newAtom;
                    bond = null;
                }
                else if (child is ASTSmarts)
                { // another smarts
                    child.JjtAccept(this, new object[] { atom, bond });
                    bond = null;
                }
                else if (child is ASTRingIdentifier)
                {
                    HandleRingClosure(atom, (ASTRingIdentifier)child);
                }
                else
                {
                    throw new InvalidOperationException("Unhandled node type: " + child.GetType());
                }
            }

            return query;
        }

        public object Visit(ASTNotBond node, object data)
        {
            object left = node.JjtGetChild(0).JjtAccept(this, data);
            if (node.Type == SMARTSParserConstants.NOT)
            {
                LogicalOperatorBond bond = new LogicalOperatorBond(builder)
                {
                    Operator = "not",
                    Left = (IQueryBond)left
                };
                return bond;
            }
            else
            {
                return left;
            }
        }

        public object Visit(ASTImplicitHighAndBond node, object data)
        {
            object left = node.JjtGetChild(0).JjtAccept(this, data);
            if (node.JjtGetNumChildren() == 1)
            {
                return left;
            }
            LogicalOperatorBond bond = new LogicalOperatorBond(builder)
            {
                Operator = "and",
                Left = (IQueryBond)left
            };
            IQueryBond right = (IQueryBond)node.JjtGetChild(1).JjtAccept(this, data);
            bond.Right = right;
            return bond;
        }

        public object Visit(ASTLowAndBond node, object data)
        {
            object left = node.JjtGetChild(0).JjtAccept(this, data);
            if (node.JjtGetNumChildren() == 1)
            {
                return left;
            }
            LogicalOperatorBond bond = new LogicalOperatorBond(builder)
            {
                Operator = "and",
                Left = (IQueryBond)left
            };
            IQueryBond right = (IQueryBond)node.JjtGetChild(1).JjtAccept(this, data);
            bond.Right = right;
            return bond;
        }

        public object Visit(ASTOrBond node, object data)
        {
            object left = node.JjtGetChild(0).JjtAccept(this, data);
            if (node.JjtGetNumChildren() == 1)
            {
                return left;
            }
            LogicalOperatorBond bond = new LogicalOperatorBond(builder)
            {
                Operator = "or",
                Left = (IQueryBond)left
            };
            IQueryBond right = (IQueryBond)node.JjtGetChild(1).JjtAccept(this, data);
            bond.Right = right;
            return bond;
        }

        public object Visit(ASTExplicitHighAndBond node, object data)
        {
            object left = node.JjtGetChild(0).JjtAccept(this, data);
            if (node.JjtGetNumChildren() == 1)
            {
                return left;
            }
            LogicalOperatorBond bond = new LogicalOperatorBond(builder)
            {
                Operator = "and",
                Left = (IQueryBond)left
            };
            IQueryBond right = (IQueryBond)node.JjtGetChild(1).JjtAccept(this, data);
            bond.Right = right;
            return bond;
        }

        public object Visit(ASTSimpleBond node, object data)
        {
            SMARTSBond bond = null;
            switch (node.BondType)
            {
                case SMARTSParserConstants.S_BOND:
                    bond = new Smarts.OrderQueryBond(BondOrder.Single, builder);
                    break;
                case SMARTSParserConstants.D_BOND:
                    bond = new Smarts.OrderQueryBond(BondOrder.Double, builder);
                    doubleBonds.Add(bond);
                    break;
                case SMARTSParserConstants.T_BOND:
                    bond = new Smarts.OrderQueryBond(BondOrder.Triple, builder);
                    break;
                case SMARTSParserConstants.DOLLAR:
                    bond = new Smarts.OrderQueryBond(BondOrder.Quadruple, builder);
                    break;
                case SMARTSParserConstants.ANY_BOND:
                    bond = new Smarts.AnyOrderQueryBond(builder);
                    break;
                case SMARTSParserConstants.AR_BOND:
                    bond = new Smarts.AromaticQueryBond(builder);
                    break;
                case SMARTSParserConstants.R_BOND:
                    bond = new Smarts.RingBond(builder);
                    break;
                case SMARTSParserConstants.UP_S_BOND:
                    bond = new Smarts.StereoBond(builder, StereoBond.Direction.Up, false);
                    stereoBonds.Add(bond);
                    break;
                case SMARTSParserConstants.DN_S_BOND:
                    bond = new Smarts.StereoBond(builder, StereoBond.Direction.Down, false);
                    stereoBonds.Add(bond);
                    break;
                case SMARTSParserConstants.UP_OR_UNSPECIFIED_S_BOND:
                    bond = new Smarts.StereoBond(builder, StereoBond.Direction.Up, true);
                    stereoBonds.Add(bond);
                    break;
                case SMARTSParserConstants.DN_OR_UNSPECIFIED_S_BOND:
                    bond = new Smarts.StereoBond(builder, StereoBond.Direction.Down, true);
                    stereoBonds.Add(bond);
                    break;
                default:
                    Trace.TraceError("Unparsed bond: " + node.ToString());
                    break;
            }
            return bond;
        }

        public object Visit(ASTRecursiveSmartsExpression node, object data)
        {
            SmartsQueryVisitor recursiveVisitor = new SmartsQueryVisitor(builder)
            {
                query = new QueryAtomContainer(builder),
                ringAtoms = new RingIdentifierAtom[10]
            };
            return new RecursiveSmartsAtom((IQueryAtomContainer)node.JjtGetChild(0).JjtAccept(recursiveVisitor, null));
        }

        public ASTStart GetRoot(INode node)
        {
            if (node is ASTStart)
            {
                return (ASTStart)node;
            }
            return GetRoot(node.JjtGetParent());
        }

        public object Visit(ASTElement node, object data)
        {
            string symbol = node.Symbol;
            SMARTSAtom atom;
            switch (symbol)
            {
                case "o":
                case "n":
                case "c":
                case "s":
                case "p":
                case "as":
                case "se":
                    string atomSymbol = symbol.Substring(0, 1).ToUpperInvariant() + symbol.Substring(1);
                    atom = new AromaticSymbolAtom(atomSymbol, builder);
                    break;
                default:
                    atom = new AliphaticSymbolAtom(symbol, builder);
                    break;
            }
            return atom;
        }

        public object Visit(ASTTotalHCount node, object data)
        {
            return new TotalHCountAtom(node.Count, builder);
        }

        public object Visit(ASTImplicitHCount node, object data)
        {
            return new ImplicitHCountAtom(node.Count, builder);
        }

        public object Visit(ASTExplicitConnectivity node, object data)
        {
            return new ExplicitConnectionAtom(node.NumOfConnection, builder);
        }

        public object Visit(ASTAtomicNumber node, object data)
        {
            return new AtomicNumberAtom(node.Number, builder);
        }

        public object Visit(ASTHybrdizationNumber node, object data)
        {
            return new HybridizationNumberAtom(node.HybridizationNumber, builder);
        }

        public object Visit(ASTCharge node, object data)
        {
            if (node.IsPositive)
            {
                return new FormalChargeAtom(node.Charge, builder);
            }
            else
            {
                return new FormalChargeAtom(0 - node.Charge, builder);
            }
        }

        public object Visit(ASTRingConnectivity node, object data)
        {
            return new TotalRingConnectionAtom(node.NumOfConnection, builder);
        }

        public object Visit(ASTPeriodicGroupNumber node, object data)
        {
            return new PeriodicGroupNumberAtom(node.GroupNumber, builder);
        }

        public object Visit(ASTTotalConnectivity node, object data)
        {
            return new TotalConnectionAtom(node.NumOfConnection, builder);
        }

        public object Visit(ASTValence node, object data)
        {
            return new TotalValencyAtom(node.Order, builder);
        }

        public object Visit(ASTRingMembership node, object data)
        {
            return new RingMembershipAtom(node.NumOfMembership, builder);
        }

        public object Visit(ASTSmallestRingSize node, object data)
        {
            return new SmallestRingAtom(node.Size, builder);
        }

        public object Visit(ASTAliphatic node, object data)
        {
            return new AliphaticAtom(builder);
        }

        public object Visit(ASTNonCHHeavyAtom node, object data)
        {
            return new NonCHHeavyAtom(builder);
        }

        public object Visit(ASTAromatic node, object data)
        {
            return new AromaticAtom(builder);
        }

        public object Visit(ASTAnyAtom node, object data)
        {
            return new AnyAtom(builder);
        }

        public object Visit(ASTAtomicMass node, object data)
        {
            return new MassAtom(node.Mass, builder);
        }

        public object Visit(ASTChirality node, object data)
        {
            ChiralityAtom atom = new ChiralityAtom(builder)
            {
                IsClockwise = node.IsClockwise,
                IsUnspecified = node.IsUnspecified
            };
            BitArrays.SetValue(tetrahedral, query.Atoms.Count, true);
            return atom;
        }

        public object Visit(ASTLowAndExpression node, object data)
        {
            IAtom expr = (IAtom)node.JjtGetChild(0).JjtAccept(this, data);
            if (node.JjtGetNumChildren() > 1)
            {
                IQueryAtom right = (IQueryAtom)node.JjtGetChild(1).JjtAccept(this, data);
                expr = LogicalOperatorAtom.And((IQueryAtom)expr, right);
            }
            if (node.GetMapIdx() > 0)
                expr.SetProperty(CDKPropertyName.AtomAtomMapping, node.GetMapIdx());
            return expr;
        }

        public object Visit(ASTOrExpression node, object data)
        {
            object left = node.JjtGetChild(0).JjtAccept(this, data);
            if (node.JjtGetNumChildren() == 1)
            {
                return left;
            }
            IQueryAtom right = (IQueryAtom)node.JjtGetChild(1).JjtAccept(this, data);
            return LogicalOperatorAtom.Or((IQueryAtom)left, right);
        }

        public object Visit(ASTNotExpression node, object data)
        {
            object left = node.JjtGetChild(0).JjtAccept(this, data);
            if (node.Type == SMARTSParserConstants.NOT)
            {
                return LogicalOperatorAtom.Not((IQueryAtom)left);
            }
            return left;
        }

        public object Visit(ASTExplicitHighAndExpression node, object data)
        {
            object left = node.JjtGetChild(0).JjtAccept(this, data);
            if (node.JjtGetNumChildren() == 1)
            {
                return left;
            }
            IQueryAtom right = (IQueryAtom)node.JjtGetChild(1).JjtAccept(this, data);
            return LogicalOperatorAtom.And((IQueryAtom)left, right);
        }

        public object Visit(ASTImplicitHighAndExpression node, object data)
        {
            object left = node.JjtGetChild(0).JjtAccept(this, data);
            if (node.JjtGetNumChildren() == 1)
            {
                return left;
            }
            IQueryAtom right = (IQueryAtom)node.JjtGetChild(1).JjtAccept(this, data);
            return LogicalOperatorAtom.And((IQueryAtom)left, right);
        }

        public object Visit(ASTExplicitAtom node, object data)
        {
            IQueryAtom atom = null;
            string symbol = node.Symbol;
            switch (symbol)
            {
                case "*":
                    atom = new AnyAtom(builder);
                    break;
                case "A":
                    atom = new AliphaticAtom(builder);
                    break;
                case "a":
                    atom = new AromaticAtom(builder);
                    break;
                case "o":
                case "n":
                case "c":
                case "s":
                case "p":
                case "as":
                case "se":
                    string atomSymbol = symbol.Substring(0, 1).ToUpperInvariant() + symbol.Substring(1);
                    atom = new AromaticSymbolAtom(atomSymbol, builder);
                    break;
                case "H":
                    atom = new HydrogenAtom(builder)
                    {
                        Symbol = symbol.ToUpperInvariant(),
                        MassNumber = 1
                    };
                    break;
                case "D":
                    atom = new HydrogenAtom(builder)
                    {
                        Symbol = symbol.ToUpperInvariant(),
                        MassNumber = 2
                    };
                    break;
                case "T":
                    atom = new HydrogenAtom(builder)
                    {
                        Symbol = symbol.ToUpperInvariant(),
                        MassNumber = 3
                    };
                    break;
                default:
                    atom = new AliphaticSymbolAtom(symbol, builder);
                    break;
            }
            return atom;
        }
    }
}
