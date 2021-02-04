/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
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

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCDK.Isomorphisms.Matchers
{
    // @cdk.module  isomorphism
    public class QueryAtomContainer : Silent.AtomContainer, IQueryAtomContainer
    {
        public override string ToString()
        {
            var s = new StringBuilder();
            s.Append("QueryAtomContainer(");
            s.Append(this.GetHashCode());
            s.Append(", #A:").Append(Atoms.Count);
            s.Append(", #EC:").Append(this.GetElectronContainers().Count());
            foreach (var atom in Atoms)
                s.Append(", ").Append(atom.ToString());
            foreach (var bond in Bonds)
                s.Append(", ").Append(bond.ToString());
            foreach (var lonePair in LonePairs)
                s.Append(", ").Append(lonePair.ToString());
            foreach (var singleElectron in SingleElectrons)
                s.Append(", ").Append(singleElectron.ToString());
            s.Append(')');
            return s.ToString();
        }

        /// <summary>
        /// Constructs an AtomContainer with a copy of the atoms and electronContainers
        /// of another AtomContainer (A shallow copy, i.e., with the same objects as in
        /// the original AtomContainer).
        /// </summary>
        /// <param name="container">An AtomContainer to copy the atoms and electronContainers from</param>
        public QueryAtomContainer(IAtomContainer container)
            : base(container)
        {
        }

        /// <summary>
        /// Constructs an empty AtomContainer that will contain a certain number of
        /// atoms and electronContainers. It will set the starting array lengths to the
        /// defined values, but will not create any Atom or ElectronContainer's.
        /// </summary>
        public QueryAtomContainer()
            : base()
        {
        }

        /// <summary>
        /// Create a query from a molecule and a provided set of expressions. The
        /// molecule is converted and any features specified in the <paramref name="opts"/>
        /// will be matched. 
        /// </summary>
        /// <remarks>
        /// A good starting point is the following options:
        /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Isomorphisms.Matchers.QueryAtomContainer_Example.cs+Create21"]/*' />
        /// Specifying <see cref="ExprType.Degree"/> (or <see cref="ExprType.TotalDegree"/> +
        /// <see cref="ExprType.ImplicitHCount"/>) means the molecule will not match as a
        /// substructure.
        /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Isomorphisms.Matchers.QueryAtomContainer_Example.cs+Create22"]/*' />
        /// The <see cref="ExprType.RingBondCount"/> property is useful for locking in
        /// ring systems. Specifying the ring bond count on benzene means it will
        /// not match larger ring systems (e.g. naphthalenee) but can still be
        /// substituted.
        /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Isomorphisms.Matchers.QueryAtomContainer_Example.cs+Create23"]/*' />
        /// Note that <see cref="ExprType.FormalCharge"/>,
        /// <see cref="ExprType.ImplicitHCount"/>, and <see cref="ExprType.Isotope"/> are ignored
        /// if <see langword="null"/>. Explicitly setting these to zero (only required for Isotope from
        /// SMILES) forces their inclusion.
        /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Isomorphisms.Matchers.QueryAtomContainer_Example.cs+Create24"]/*' />
        /// Please note not all <see cref="ExprType"/>s are currently supported, if you
        /// require a specific type that you think is useful please open an issue.
        /// </remarks>
        /// <param name="dst">the output destination</param>
        /// <param name="src">the input molecule</param>
        /// <param name="opts">set of the expr types to match</param>
        /// <returns>the query molecule</returns>
        public static void Create(IAtomContainer dst, IAtomContainer src, params ExprType[] opts)
        {
            var optset = new HashSet<ExprType>(opts);
            var mapping = new CDKObjectMap();
            var stereos = new Dictionary<IChemObject, IStereoElement<IChemObject, IChemObject>>();
            foreach (var se in src.StereoElements)
                stereos[se.Focus] = se;
            var qstereo = new List<IStereoElement<IChemObject, IChemObject>>();
            foreach (var atom in src.Atoms)
            {
                Expr expr;
                if (atom is IQueryAtom) 
                {
                    expr = ((QueryAtom)AtomRef.Deref(atom)).Expression;
                    if (stereos.TryGetValue(atom, out IStereoElement<IChemObject, IChemObject> se))
                        qstereo.Add(se);
                } 
                else
                {
                    expr = new Expr();

                    // isotope first
                    if (optset.Contains(ExprType.Isotope) && atom.MassNumber != null)
                        expr.And(new Expr(ExprType.Isotope, atom.MassNumber.Value));
                    if (atom.AtomicNumber != 0)
                    {
                        if (atom.IsAromatic)
                        {
                            if (optset.Contains(ExprType.AromaticElement))
                            {
                                expr.And(new Expr(ExprType.AromaticElement,
                                        atom.AtomicNumber));
                            }
                            else
                            {
                                if (optset.Contains(ExprType.IsAromatic))
                                {
                                    if (optset.Contains(ExprType.Element))
                                        expr.And(new Expr(ExprType.AromaticElement,
                                                atom.AtomicNumber));
                                    else
                                        expr.And(new Expr(ExprType.IsAromatic));
                                }
                                else if (optset.Contains(ExprType.Element))
                                {
                                    expr.And(new Expr(ExprType.Element,
                                            atom.AtomicNumber));
                                }
                            }
                        }
                        else
                        {
                            if (optset.Contains(ExprType.AliphaticElement))
                            {
                                expr.And(new Expr(ExprType.AliphaticElement,
                                        atom.AtomicNumber));
                            }
                            else
                            {
                                if (optset.Contains(ExprType.IsAliphatic))
                                {
                                    if (optset.Contains(ExprType.Element))
                                        expr.And(new Expr(ExprType.AliphaticElement,
                                                atom.AtomicNumber));
                                    else
                                        expr.And(new Expr(ExprType.IsAliphatic));
                                }
                                else if (optset.Contains(ExprType.Element))
                                {
                                    expr.And(new Expr(ExprType.Element,
                                            atom.AtomicNumber));
                                }
                            }
                        }
                    }
                    if (optset.Contains(ExprType.Degree))
                        expr.And(new Expr(ExprType.Degree,
                                atom.Bonds.Count));
                    if (optset.Contains(ExprType.TotalDegree))
                        expr.And(new Expr(ExprType.Degree, atom.Bonds.Count + atom.ImplicitHydrogenCount.Value));
                    if (optset.Contains(ExprType.IsInRing) && atom.IsInRing)
                        expr.And(new Expr(ExprType.IsInRing));
                    if (optset.Contains(ExprType.IsInChain) && !atom.IsInRing)
                        expr.And(new Expr(ExprType.IsInChain));
                    if (optset.Contains(ExprType.ImplicitHCount))
                        expr.And(new Expr(ExprType.ImplicitHCount));
                    if (optset.Contains(ExprType.RingBondCount))
                    {
                        int rbonds = 0;
                        foreach (var bond in src.GetConnectedBonds(atom))
                            if (bond.IsInRing)
                                rbonds++;
                        expr.And(new Expr(ExprType.RingBondCount, rbonds));
                    }
                    if (optset.Contains(ExprType.FormalCharge) && atom.FormalCharge != null)
                        expr.And(new Expr(ExprType.FormalCharge, atom.FormalCharge.Value));
                    if (stereos.TryGetValue(atom, out IStereoElement<IChemObject, IChemObject> se))
                    {
                        if (se.Class == StereoClass.Tetrahedral
                         && optset.Contains(ExprType.Stereochemistry))
                        {
                            expr.And(new Expr(ExprType.Stereochemistry, (int)se.Configure));
                            qstereo.Add(se);
                        }
                    }
                }
                var qatom = new QueryAtom(expr)
                {
                    IsInRing = atom.IsInRing
                };
                // backward compatibility for naughty methods that are expecting
                // these to be set for a query!
                if (optset.Contains(ExprType.Element) 
                 || optset.Contains(ExprType.AromaticElement) 
                 || optset.Contains(ExprType.AliphaticElement))
                {
                    qatom.Symbol = atom.Symbol;
                    qatom.AtomicNumber = atom.AtomicNumber;
                }
                if (optset.Contains(ExprType.AromaticElement) 
                 || optset.Contains(ExprType.IsAromatic))
                    qatom.IsAromatic = atom.IsAromatic;
                mapping.Add(atom, qatom);
                dst.Atoms.Add(qatom);
            }
            foreach (var bond in src.Bonds)
            {
                Expr expr;
                if (bond is IQueryBond) {
                    expr = ((QueryBond)BondRef.Deref(bond)).Expression;
                    var se = stereos[bond];
                    if (se != null) qstereo.Add(se);
                } else
                {
                    expr = new Expr();
                    if (bond.IsAromatic &&
                            (optset.Contains(ExprType.SingleOrAromatic) ||
                                    optset.Contains(ExprType.DoubleOrAromatic) ||
                                    optset.Contains(ExprType.IsAromatic)))
                        expr.And(new Expr(ExprType.IsAromatic));
                    else if ((optset.Contains(ExprType.SingleOrAromatic) ||
                            optset.Contains(ExprType.DoubleOrAromatic) ||
                            optset.Contains(ExprType.AliphaticOrder)) && !bond.IsAromatic)
                        expr.And(new Expr(ExprType.AliphaticOrder, bond.Order.Numeric()));
                    else if (bond.IsAromatic && optset.Contains(ExprType.IsAliphatic))
                        expr.And(new Expr(ExprType.IsAliphatic));
                    else if (optset.Contains(ExprType.Order))
                        expr.And(new Expr(ExprType.Order, bond.Order.Numeric()));
                    if (optset.Contains(ExprType.IsInRing) && bond.IsInRing)
                        expr.And(new Expr(ExprType.IsInRing));
                    else if (optset.Contains(ExprType.IsInChain) && !bond.IsInRing)
                        expr.And(new Expr(ExprType.IsInChain));
                    if (stereos.TryGetValue(bond, out IStereoElement<IChemObject, IChemObject> se))
                    {
                        if (optset.Contains(ExprType.Stereochemistry))
                        {
                            expr.And(new Expr(ExprType.Stereochemistry, (int)se.Configure));
                            qstereo.Add(se);
                        }
                    }
                }
                var qbond = new QueryBond(mapping.Get(bond.Begin), mapping.Get(bond.End), expr)
                {
                    IsInRing = bond.IsInRing
                };
                // backward compatibility for naughty methods that are expecting
                // these to be set for a query!
                if (optset.Contains(ExprType.AliphaticOrder) ||
                    optset.Contains(ExprType.Order))
                    qbond.Order = bond.Order;
                if (optset.Contains(ExprType.SingleOrAromatic) ||
                    optset.Contains(ExprType.DoubleOrAromatic) ||
                    optset.Contains(ExprType.IsAromatic))
                    qbond.IsAromatic = bond.IsAromatic;
                mapping.Add(bond, qbond);
                dst.Bonds.Add(qbond);
            }
            foreach (var se in qstereo)
            {
                dst.StereoElements.Add((IStereoElement<IChemObject, IChemObject>)se.Clone(mapping));
            }            
        }

        /// <summary>
        /// Create a query from a molecule and a provided set of expressions. The
        /// molecule is converted and any features specified in the <paramref name="opts"/>
        /// will be matched. 
        /// </summary>
        /// <remarks>
        /// A good starting point is the following options:
        /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Isomorphisms.Matchers.QueryAtomContainer_Example.cs+Create1"]/*' />
        /// Specifying <see cref="ExprType.Degree"/> (or <see cref="ExprType.TotalDegree"/> +
        /// <see cref="ExprType.ImplicitHCount"/>) means the molecule will not match as a
        /// substructure.
        /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Isomorphisms.Matchers.QueryAtomContainer_Example.cs+Create2"]/*' />
        /// The <see cref="ExprType.RingBondCount"/> property is useful for locking in
        /// ring systems. Specifying the ring bond count on benzene means it will
        /// not match larger ring systems (e.g. naphthalenee) but can still be
        /// substituted.
        /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Isomorphisms.Matchers.QueryAtomContainer_Example.cs+Create3"]/*' />
        /// Note that <see cref="ExprType.FormalCharge"/>,
        /// <see cref="ExprType.ImplicitHCount"/>, and <see cref="ExprType.Isotope"/> are ignored
        /// if <see langword="null"/>. Explicitly setting these to zero (only required for Isotope from
        /// SMILES) forces their inclusion.
        /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Isomorphisms.Matchers.QueryAtomContainer_Example.cs+Create4"]/*' />
        /// Please note not all <see cref="ExprType"/>s are currently supported, if you
        /// require a specific type that you think is useful please open an issue.
        /// </remarks>
        /// <param name="mol">the molecule</param>
        /// <param name="opts">set of the expr types to match</param>
        /// <returns>the query molecule</returns>
        public static QueryAtomContainer Create(IAtomContainer src, params ExprType[] opts)
        {
            var dst = new QueryAtomContainer();
            Create(dst, src, opts);
            return dst;
        }
    }
}
