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
using Smarts = NCDK.Isomorphisms.Matchers.SMARTS;
using NCDK.Stereo;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NCDK.Smiles.SMARTS.Parser
{
    /**
     * An AST tree visitor. It builds an instance of <code>QueryAtomContainer</code>
     * from the AST tree.
     *
     * To use this visitor:
     * <pre>
     * SMARTSParser parser = new SMARTSParser(new java.io.StringReader("C*C"));
     * ASTStart ast = parser.Start();
     * SmartsQueryVisitor visitor = new SmartsQueryVisitor();
     * QueryAtomContainer query = visitor.Visit(ast, null);
     * </pre>
     *
     * @author Dazhi Jiao
     * @cdk.created 2007-04-24
     * @cdk.module smarts
     * @cdk.githash
     * @cdk.keyword SMARTS AST
     */
    public class SmartsQueryVisitor : SMARTSParserVisitor
    {
        // current atoms with a ring identifier
        private RingIdentifierAtom[] ringAtoms;

        private IMultiDictionary<IAtom, RingIdentifierAtom> ringAtomLookup = new MultiDictionary<IAtom, RingIdentifierAtom>();

        // query
        private IQueryAtomContainer query;

        private readonly IChemObjectBuilder builder;

        /**
         * Maintain order of neighboring atoms - required for atom-based
         * stereochemistry.
         */
        private IDictionary<IAtom, IList<IAtom>> neighbors = new Dictionary<IAtom, IList<IAtom>>();

        /**
         * Lookup of atom indices.
         */
        private BitArray tetrahedral = new BitArray(0);

        /**
         * Stores the directional '/' or '\' bonds. Speeds up looking for double
         * bond configurations.
         */
        private List<IBond> stereoBonds = new List<IBond>();

        /**
         * Stores the double bonds in the query.
         */
        private List<IBond> doubleBonds = new List<IBond>();

        public SmartsQueryVisitor(IChemObjectBuilder builder)
        {
            this.builder = builder;
        }

        public object Visit(ASTRingIdentifier node, object data)
        {
            IQueryAtom atom = (IQueryAtom)data;
            RingIdentifierAtom ringIdAtom = new RingIdentifierAtom(builder);
            ringIdAtom.Atom = atom;
            IQueryBond bond;
            if (node.jjtGetNumChildren() == 0)
            { // implicit bond
                bond = null;
            }
            else
            {
                bond = (IQueryBond)node.jjtGetChild(0).jjtAccept(this, data);
            }
            ringIdAtom.RingBond = bond;
            return ringIdAtom;
        }

        public object Visit(ASTAtom node, object data)
        {
            IQueryAtom atom = (IQueryAtom)node.jjtGetChild(0).jjtAccept(this, data);
            for (int i = 1; i < node.jjtGetNumChildren(); i++)
            { // if there are ring identifiers
                ASTRingIdentifier ringIdentifier = (ASTRingIdentifier)node.jjtGetChild(i);
                RingIdentifierAtom ringIdAtom = (RingIdentifierAtom)ringIdentifier.jjtAccept(this, atom);

                // if there is already a RingIdentifierAtom, create a bond between
                // them and add the bond to the query
                int ringId = ringIdentifier.RingId;

                // ring digit > 9 - expand capacity
                if (ringId >= ringAtoms.Length) ringAtoms = Arrays.CopyOf(ringAtoms, 100);

                // Ring Open
                if (ringAtoms[ringId] == null)
                {
                    ringAtoms[ringId] = ringIdAtom;
                    ringAtomLookup.Add(atom, ringIdAtom);
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

                    ringAtomLookup.Remove(ringAtoms[ringId].Atom, ringIdAtom);
                    ringAtoms[ringId] = null;
                }
            }
            return atom;
        }

        public object Visit(SimpleNode node, object data)
        {
            return null;
        }

        public object Visit(ASTStart node, object data)
        {
            return node.jjtGetChild(0).jjtAccept(this, data);
        }

        // TODO: No QueryReaction API
        public object Visit(ASTReaction node, object data)
        {
            return node.jjtGetChild(0).jjtAccept(this, data);
        }

        public object Visit(ASTGroup node, object data)
        {
            IAtomContainer fullQuery = new QueryAtomContainer(builder);

            // keeps track of component grouping
            int[] components = new int[0];
            int maxId = 0;

            for (int i = 0; i < node.jjtGetNumChildren(); i++)
            {
                ASTSmarts smarts = (ASTSmarts)node.jjtGetChild(i);
                ringAtoms = new RingIdentifierAtom[10];
                query = new QueryAtomContainer(builder);

                smarts.jjtAccept(this, null);

                // update component info
                if (smarts.ComponentId > 0)
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
                    fullQuery.AddStereoElement(new TetrahedralChirality(atom, localNeighbors.ToArray(),
                        TetrahedralStereo.Clockwise)); // <- to be modified later
                }
                else if (localNeighbors.Count == 5)
                {
                    localNeighbors.Remove(atom); // remove central atom (which represented implicit part)
                    fullQuery.AddStereoElement(new TetrahedralChirality(atom, localNeighbors.ToArray(),
                        TetrahedralStereo.Clockwise)); // <- to be modified later
                }
            }

            // for each double bond, find the stereo bonds. currently doesn't
            // handle logical bonds i.e. C/C-,=C/C
            foreach (var bond in doubleBonds)
            {
                IAtom left = bond.Atoms[0];
                IAtom right = bond.Atoms[1];
                StereoBond leftBond = FindStereoBond(left);
                StereoBond rightBond = FindStereoBond(right);
                if (leftBond == null || rightBond == null) continue;
                DoubleBondConformation conformation = leftBond.GetDirection(left) == rightBond.GetDirection(right) ? DoubleBondConformation.Together
                       : DoubleBondConformation.Opposite;
                fullQuery.AddStereoElement(new DoubleBondStereochemistry(bond, new IBond[] { leftBond, rightBond },
                        conformation));
            }

            return fullQuery;
        }

        /**
         * Locate a stereo bond adjacent to the {@code atom}.
         *
         * @param atom an atom
         * @return a stereo bond or null if non found
         */
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

            ASTAtom first = (ASTAtom)node.jjtGetChild(0);
            atom = (SMARTSAtom)first.jjtAccept(this, null);
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
            query.Atoms.Add(atom);

            if (BitArrays.GetValue(tetrahedral, query.Atoms.Count - 1))
            {
                List<IAtom> localNeighbors = new List<IAtom>(query.GetConnectedAtoms(atom));
                localNeighbors.Add(atom);
                // placeholders for ring closure
                foreach (var ringIdAtom in ringAtomLookup[atom])
                    localNeighbors.Add(ringIdAtom);
                neighbors[atom] = localNeighbors;
            }

            for (int i = 1; i < node.jjtGetNumChildren(); i++)
            {
                Node child = node.jjtGetChild(i);
                if (child is ASTLowAndBond)
                {
                    bond = (SMARTSBond)child.jjtAccept(this, data);
                }
                else if (child is ASTAtom)
                {
                    SMARTSAtom newAtom = (SMARTSAtom)child.jjtAccept(this, null);
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
                        List<IAtom> localNeighbors = new List<IAtom>(query.GetConnectedAtoms(newAtom));
                        localNeighbors.Add(newAtom);
                        // placeholders for ring closure
                        foreach (var ringIdAtom in ringAtomLookup[newAtom])
                            localNeighbors.Add(ringIdAtom);
                        neighbors[newAtom] = localNeighbors;
                    }

                    atom = newAtom;
                    bond = null;
                }
                else if (child is ASTSmarts)
                { // another smarts
                    child.jjtAccept(this, new object[] { atom, bond });
                    bond = null;
                }
            }

            return query;
        }

        public object Visit(ASTNotBond node, object data)
        {
            object left = node.jjtGetChild(0).jjtAccept(this, data);
            if (node.Type == SMARTSParserConstants.NOT)
            {
                LogicalOperatorBond bond = new LogicalOperatorBond(builder);
                bond.Operator = "not";
                bond.Left = (IQueryBond)left;
                return bond;
            }
            else
            {
                return left;
            }
        }

        public object Visit(ASTImplicitHighAndBond node, object data)
        {
            object left = node.jjtGetChild(0).jjtAccept(this, data);
            if (node.jjtGetNumChildren() == 1)
            {
                return left;
            }
            LogicalOperatorBond bond = new LogicalOperatorBond(builder);
            bond.Operator = "and";
            bond.Left = (IQueryBond)left;
            IQueryBond right = (IQueryBond)node.jjtGetChild(1).jjtAccept(this, data);
            bond.Right = right;
            return bond;
        }

        public object Visit(ASTLowAndBond node, object data)
        {
            object left = node.jjtGetChild(0).jjtAccept(this, data);
            if (node.jjtGetNumChildren() == 1)
            {
                return left;
            }
            LogicalOperatorBond bond = new LogicalOperatorBond(builder);
            bond.Operator = "and";
            bond.Left = (IQueryBond)left;
            IQueryBond right = (IQueryBond)node.jjtGetChild(1).jjtAccept(this, data);
            bond.Right = right;
            return bond;
        }

        public object Visit(ASTOrBond node, object data)
        {
            object left = node.jjtGetChild(0).jjtAccept(this, data);
            if (node.jjtGetNumChildren() == 1)
            {
                return left;
            }
            LogicalOperatorBond bond = new LogicalOperatorBond(builder);
            bond.Operator = "or";
            bond.Left = (IQueryBond)left;
            IQueryBond right = (IQueryBond)node.jjtGetChild(1).jjtAccept(this, data);
            bond.Right = right;
            return bond;
        }

        public object Visit(ASTExplicitHighAndBond node, object data)
        {
            object left = node.jjtGetChild(0).jjtAccept(this, data);
            if (node.jjtGetNumChildren() == 1)
            {
                return left;
            }
            LogicalOperatorBond bond = new LogicalOperatorBond(builder);
            bond.Operator = "and";
            bond.Left = (IQueryBond)left;
            IQueryBond right = (IQueryBond)node.jjtGetChild(1).jjtAccept(this, data);
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
                    Trace.TraceError("Un parsed bond: " + node.ToString());
                    break;
            }
            return bond;
        }

        public object Visit(ASTRecursiveSmartsExpression node, object data)
        {
            SmartsQueryVisitor recursiveVisitor = new SmartsQueryVisitor(builder);
            recursiveVisitor.query = new QueryAtomContainer(builder);
            recursiveVisitor.ringAtoms = new RingIdentifierAtom[10];
            return new RecursiveSmartsAtom((IQueryAtomContainer)node.jjtGetChild(0).jjtAccept(recursiveVisitor, null));
        }

        public ASTStart GetRoot(Node node)
        {
            if (node is ASTStart)
            {
                return (ASTStart)node;
            }
            return GetRoot(node.jjtGetParent());
        }

        public object Visit(ASTElement node, object data)
        {
            string symbol = node.Symbol;
            SMARTSAtom atom;
            if ("o".Equals(symbol) || "n".Equals(symbol) || "c".Equals(symbol) || "s".Equals(symbol) || "p".Equals(symbol)
                    || "as".Equals(symbol) || "se".Equals(symbol))
            {
                string atomSymbol = symbol.Substring(0, 1).ToUpperInvariant() + symbol.Substring(1);
                atom = new AromaticSymbolAtom(atomSymbol, builder);
            }
            else
            {
                atom = new AliphaticSymbolAtom(symbol, builder);
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
            ChiralityAtom atom = new ChiralityAtom(builder);
            atom.IsClockwise = node.IsClockwise;
            atom.IsUnspecified = node.IsUnspecified;
            BitArrays.SetValue(tetrahedral, query.Atoms.Count, true);
            return atom;
        }

        public object Visit(ASTLowAndExpression node, object data)
        {
            object left = node.jjtGetChild(0).jjtAccept(this, data);
            if (node.jjtGetNumChildren() == 1)
            {
                return left;
            }
            IQueryAtom right = (IQueryAtom)node.jjtGetChild(1).jjtAccept(this, data);
            return LogicalOperatorAtom.And((IQueryAtom)left, right);
        }

        public object Visit(ASTOrExpression node, object data)
        {
            object left = node.jjtGetChild(0).jjtAccept(this, data);
            if (node.jjtGetNumChildren() == 1)
            {
                return left;
            }
            IQueryAtom right = (IQueryAtom)node.jjtGetChild(1).jjtAccept(this, data);
            return LogicalOperatorAtom.Or((IQueryAtom)left, right);
        }

        public object Visit(ASTNotExpression node, object data)
        {
            object left = node.jjtGetChild(0).jjtAccept(this, data);
            if (node.Type == SMARTSParserConstants.NOT)
            {
                return LogicalOperatorAtom.Not((IQueryAtom)left);
            }
            return left;
        }

        public object Visit(ASTExplicitHighAndExpression node, object data)
        {
            object left = node.jjtGetChild(0).jjtAccept(this, data);
            if (node.jjtGetNumChildren() == 1)
            {
                return left;
            }
            IQueryAtom right = (IQueryAtom)node.jjtGetChild(1).jjtAccept(this, data);
            return LogicalOperatorAtom.And((IQueryAtom)left, right);
        }

        public object Visit(ASTImplicitHighAndExpression node, object data)
        {
            object left = node.jjtGetChild(0).jjtAccept(this, data);
            if (node.jjtGetNumChildren() == 1)
            {
                return left;
            }
            IQueryAtom right = (IQueryAtom)node.jjtGetChild(1).jjtAccept(this, data);
            return LogicalOperatorAtom.And((IQueryAtom)left, right);
        }

        public object Visit(ASTExplicitAtom node, object data)
        {
            IQueryAtom atom = null;
            string symbol = node.Symbol;
            if ("*".Equals(symbol))
            {
                atom = new AnyAtom(builder);
            }
            else if ("A".Equals(symbol))
            {
                atom = new AliphaticAtom(builder);
            }
            else if ("a".Equals(symbol))
            {
                atom = new AromaticAtom(builder);
            }
            else if ("o".Equals(symbol) || "n".Equals(symbol) || "c".Equals(symbol) || "s".Equals(symbol)
                  || "p".Equals(symbol) || "as".Equals(symbol) || "se".Equals(symbol))
            {
                string atomSymbol = symbol.Substring(0, 1).ToUpperInvariant() + symbol.Substring(1);
                atom = new AromaticSymbolAtom(atomSymbol, builder);
            }
            else if ("H".Equals(symbol))
            {
                atom = new HydrogenAtom(builder);
                atom.Symbol = symbol.ToUpperInvariant();
                atom.MassNumber = 1;
            }
            else if ("D".Equals(symbol))
            {
                atom = new HydrogenAtom(builder);
                atom.Symbol = symbol.ToUpperInvariant();
                atom.MassNumber = 2;
            }
            else if ("T".Equals(symbol))
            {
                atom = new HydrogenAtom(builder);
                atom.Symbol = symbol.ToUpperInvariant();
                atom.MassNumber = 3;
            }
            else
            {
                atom = new AliphaticSymbolAtom(symbol, builder);
            }
            return atom;
        }
    }
}
