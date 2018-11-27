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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCDK.Isomorphisms.Matchers
{
    // @cdk.module  isomorphism
    // @cdk.githash
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

        private readonly IChemObjectBuilder builder;

        /// <inheritdoc/>
        public override IChemObjectBuilder Builder => builder;

        /// <summary>
        /// Constructs an AtomContainer with a copy of the atoms and electronContainers
        /// of another AtomContainer (A shallow copy, i.e., with the same objects as in
        /// the original AtomContainer).
        /// </summary>
        /// <param name="container">An AtomContainer to copy the atoms and electronContainers from</param>
        /// <param name="builder"></param>
        public QueryAtomContainer(IAtomContainer container, IChemObjectBuilder builder)
            : base(container)
        {
            this.builder = builder;
        }

        /// <summary>
        /// Constructs an empty AtomContainer that will contain a certain number of
        /// atoms and electronContainers. It will set the starting array lengths to the
        /// defined values, but will not create any Atom or ElectronContainer's.
        /// </summary>
        public QueryAtomContainer(IChemObjectBuilder builder)
            : base()
        {
            this.builder = builder;
        }

        internal object ToArray()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create a query from a molecule and a provided set of expressions. The
        /// molecule is converted and any features specified in the <paramref name="opts"/>
        /// will be matched. 
        /// </summary>
        /// <remarks>
        /// A good starting point is the following options:
        /// <code>
        /// // [nH]1ccc(=O)cc1 => n1:c:c:c(=O):c:c:1
        /// QueryAtomContainer.create(mol, ALIPHATIC_ELEMENT,
        ///                                AROMATIC_ELEMENT,
        ///                                SingleOrAromatic,
        ///                                ALIPHATIC_ORDER,
        ///                                STEREOCHEMISTRY);
        /// </code>
        /// Specifying <see cref="ExprType.Degree"/> (or <see cref="ExprType.TotalDegree"/> +
        /// <see cref="ExprType.ImplicitHCount"/>) means the molecule will not match as a
        /// substructure.
        /// <code>
        /// // [nH]1ccc(=O)cc1 => [nD2]1:[cD2]:[cD2]:[cD2](=[OD1]):[cD2]:[cD2]:1
        /// QueryAtomContainer.create(mol, AliphaticElement,
        ///                                AromaticElement,
        ///                                Degree,
        ///                                SingleOrAromatic,
        ///                                AromaticOrder);
        /// </code>
        /// The <see cref="ExprType.RingBondCount"/> property is useful for locking in
        /// ring systems. Specifying the ring bond count on benzene means it will
        /// not match larger ring systems (e.g. naphthalenee) but can still be
        /// substituted.
        /// <code>
        /// // [nH]1ccc(=O)cc1 =>
        /// //   [nx2+0]1:[cx2+0]:[cx2+0]:[cx2+0](=[O&amp;x0+0]):[cx2+0]:[cx2+0]:1
        /// // IMPORTANT! use Cycles.markRingAtomsAndBonds(mol) to set ring status
        /// QueryAtomContainer.create(mol, AliphaticElement,
        ///                                AromaticElement,
        ///                                FormalCharge,
        ///                                Isotope,
        ///                                RingBondCount,
        ///                                SingleOrAromatic,
        ///                                AliphaticOrder);
        /// </code>
        /// Note that <see cref="ExprType.FormalCharge"/>,
        /// <see cref="ExprType.ImplicitHCount"/>, and <see cref="ExprType.Isotope"/> are ignored
        /// if <see langword="null"/>. Explicitly setting these to zero (only required for Isotope from
        /// SMILES) forces their inclusion.
        /// <code>
        /// // [nH]1ccc(=O)cc1 =>
        /// //   [0n+0]1:[0c+0]:[0c+0]:[0c+0](=[O+0]):[0c+0]:[0c+0]:1
        /// QueryAtomContainer.create(mol, AliphaticElement,
        ///                                AromaticElement,
        ///                                FormalCharge,
        ///                                Isotope,
        ///                                RingBondCount,
        ///                                SingleOrAromatic,
        ///                                AliphaticOrder);
        /// </code>
        /// Please note not all <see cref="ExprType"/>s are currently supported, if you
        /// require a specific type that you think is useful please open an issue.
        /// </remarks>
        /// <param name="mol">the molecule</param>
        /// <param name="opts">set of the expr types to match</param>
        /// <returns>the query molecule</returns>
        public static QueryAtomContainer Create(IAtomContainer mol, params ExprType[] opts)
        {
            var optset = new HashSet<ExprType>(opts);
            var query = new QueryAtomContainer(mol.Builder);
            var mapping = new CDKObjectMap();
            var stereos = new Dictionary<IChemObject, IStereoElement<IChemObject, IChemObject>>();
            foreach (var se in mol.StereoElements)
                stereos[se.Focus] = se;
            var qstereo = new List<IStereoElement<IChemObject, IChemObject>>();
            foreach (var atom in mol.Atoms)
            {
                var expr = new Expr();
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
                    expr.And(new Expr(ExprType.Degree,
                                      atom.Bonds.Count + atom.ImplicitHydrogenCount.Value));
                if (optset.Contains(ExprType.IsInRing) ||
                    optset.Contains(ExprType.IsInChain))
                    expr.And(new Expr(atom.IsInRing ? ExprType.IsInRing : ExprType.IsInChain));
                if (optset.Contains(ExprType.ImplicitHCount))
                    expr.And(new Expr(ExprType.ImplicitHCount));
                if (optset.Contains(ExprType.RingBondCount))
                {
                    int rbonds = 0;
                    foreach (var bond in mol.GetConnectedBonds(atom))
                        if (bond.IsInRing)
                            rbonds++;
                    expr.And(new Expr(ExprType.RingBondCount, rbonds));
                }
                if (optset.Contains(ExprType.FormalCharge) && atom.FormalCharge != null)
                    expr.And(new Expr(ExprType.FormalCharge, atom.FormalCharge.Value));
                if (stereos.TryGetValue(atom, out IStereoElement<IChemObject, IChemObject> se)
                 && se.Class == StereoClass.Tetrahedral 
                 && optset.Contains(ExprType.Stereochemistry))
                {
                    expr.And(new Expr(ExprType.Stereochemistry, (int)se.Configure));
                    qstereo.Add(se);
                }
                QueryAtom qatom = new QueryAtom(expr);
                // backward compatibility for naughty methods that are expecting
                // these to be set for a query!
                if (optset.Contains(ExprType.Element) ||
                    optset.Contains(ExprType.AromaticElement) ||
                    optset.Contains(ExprType.AliphaticElement))
                    qatom.Symbol = atom.Symbol;
                if (optset.Contains(ExprType.AromaticElement) ||
                    optset.Contains(ExprType.IsAromatic))
                    qatom.IsAromatic = atom.IsAromatic;
                mapping.Add(atom, qatom);
                query.Atoms.Add(qatom);
            }
            foreach (var bond in mol.Bonds)
            {
                Expr expr = new Expr();
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
                if (stereos.TryGetValue(bond, out IStereoElement<IChemObject, IChemObject> se)
                 && optset.Contains(ExprType.Stereochemistry))
                {
                    expr.And(new Expr(ExprType.Stereochemistry, (int)se.Configure));
                    qstereo.Add(se);
                }
                var qbond = new QueryBond(mapping.Get(bond.Begin), mapping.Get(bond.End), expr);
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
                query.Bonds.Add(qbond);
            }
            foreach (var se in qstereo)
            {
                query.StereoElements.Add((IStereoElement<IChemObject, IChemObject>)se.Clone(mapping));
            }
            return query;
        }
    }
}
