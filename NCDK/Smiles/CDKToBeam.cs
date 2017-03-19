/*
 * Copyright (c) 2013 European Bioinformatics Institute (EMBL-EBI)
 *                    John May <jwmay@users.sf.net>
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
 * Any WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using System;
using System.Collections.Generic;
using System.Linq;
using NCDK.Beam;
using NCDK.Common.Collections;
using NCDK.Stereo;
using static NCDK.Common.Base.Preconditions;
using NCDK.Config;
using System.IO;

namespace NCDK.Smiles
{
    /// <summary>
    /// Convert a CDK <see cref="IAtomContainer"/> to a Beam graph object for generating
    /// SMILES. Once converted the Beam ChemicalGraph can be manipulated further to
    /// generate a standard-from SMILES and/or arrange the vertices in a canonical
    /// output order.
    /// </summary>
    /// <b>Important:</b> The conversion respects the implicit hydrogen count and if
    /// the number of implicit hydrogen ({@link IAtom#ImplicitHydrogenCount}) is
    /// null an exception will be thrown. To ensure correct conversion please ensure
    /// all atoms have their implicit hydrogen count set.
    ///
    /// <example><code>
    /// IAtomContainer m   = ...;
    ///
    /// // converter is thread-safe and can be used by multiple threads
    /// CDKToBeam      c2g = new CDKToBeam();
    /// ChemicalGraph  g   = c2g.ToBeamGraph(m);
    ///
    /// // get the SMILES notation from the Beam graph
    /// string         smi = g.ToSmiles():
    /// </code></example>
    /// <remarks>
    /// <a href="http://johnmay.github.io/Beam">Beam SMILES Toolkit</a>
    /// </remarks>
    // @author John May
    // @cdk.module smiles
    // @cdk.keyword SMILES
    internal sealed class CDKToBeam
    {
        /// <summary>
        /// Whether to convert the molecule with isotope and stereo information -
        /// Isomeric SMILES.
        /// </summary>
        private readonly bool isomeric;

        /// <summary>Use aromatic flags.</summary>
        private readonly bool aromatic;

        /// <summary>Set atom class data.</summary>
        private readonly bool atomClasses;

        /// <summary>Create a isomeric and aromatic converter.</summary>
        public CDKToBeam()
            : this(true, true)
        {
        }

        /// <summary>Create a aromatic converter specifying whether to be isomeric or not.</summary>
        public CDKToBeam(bool isomeric)
            : this(isomeric, true)
        {
        }

        /// <summary>
        /// Create a convert which will optionally convert isomeric and aromatic
        /// information from CDK data model.
        /// </summary>
        /// <param name="isomeric">convert isomeric information</param>
        /// <param name="aromatic">convert aromatic information</param>
        public CDKToBeam(bool isomeric, bool aromatic)
           : this(isomeric, aromatic, true)
        { }

        public CDKToBeam(bool isomeric, bool aromatic, bool atomClasses)
        {
            this.isomeric = isomeric;
            this.aromatic = aromatic;
            this.atomClasses = atomClasses;
        }

        /// <summary>
        /// Convert a CDK <see cref="IAtomContainer"/> to a Beam ChemicalGraph. The graph
        /// can when be written directly as to a SMILES or manipulated further (e.g
        /// canonical ordering/standard-form and other normalisations).
        /// </summary>
        /// <param name="ac">an atom container instance</param>
        /// <returns>the Beam ChemicalGraph for additional manipulation</returns>
        public Graph ToBeamGraph(IAtomContainer ac)
        {
            int order = ac.Atoms.Count;

            GraphBuilder gb = GraphBuilder.Create(order);
            IDictionary<IAtom, int> indices = new Dictionary<IAtom, int>(order);

            foreach (var a in ac.Atoms)
            {
                indices[a] = indices.Count;
                gb.Add(ToBeamAtom(a));
            }

            foreach (var b in ac.Bonds)
            {
                gb.Add(ToBeamEdge(b, indices));
            }

            // configure stereo-chemistry by encoding the stereo-elements
            if (isomeric)
            {
                foreach (var se in ac.StereoElements)
                {
                    if (se is ITetrahedralChirality)
                    {
                        AddTetrahedralConfiguration((ITetrahedralChirality)se, gb, indices);
                    }
                    else if (se is IDoubleBondStereochemistry)
                    {
                        AddGeometricConfiguration((IDoubleBondStereochemistry)se, gb, indices);
                    }
                    else if (se is ExtendedTetrahedral)
                    {
                        AddExtendedTetrahedralConfiguration((ExtendedTetrahedral)se, gb, indices);
                    }
                }
            }

            return gb.Build();
        }

        /// <summary>
        /// Convert an CDK <see cref="IAtom"/> to a Beam Atom. The symbol and implicit
        /// hydrogen count are not optional. If the symbol is not supported by the
        /// SMILES notation (e.g. 'R1') the element will automatically default to
        /// Unknown ('*').
        /// </summary>
        /// <param name="a">cdk Atom instance</param>
        /// <returns>a Beam atom</returns>
        /// <exception cref="NullReferenceException">the atom had an undefined symbol or implicit hydrogen count</exception>
        public Atom ToBeamAtom(IAtom a)
        {
            bool aromatic = this.aromatic && a.IsAromatic;
            int? charge = a.FormalCharge;
            string symbol = CheckNotNull(a.Symbol, "An atom had an undefined symbol");

            Element element = Element.OfSymbol(symbol);
            if (element == null) element = Element.Unknown;

            AtomBuilder ab = aromatic ? AtomBuilder.Aromatic(element) : AtomBuilder.Aliphatic(element);

            // CDK leaves nulls on pseudo atoms - we need to check this special case
            int? hCount = a.ImplicitHydrogenCount;
            if (element == Element.Unknown)
            {
                ab.NumOfHydrogens(hCount ?? 0);
            }
            else
            {
                ab.NumOfHydrogens(CheckNotNull(hCount, "One or more atoms had an undefined number of implicit hydrogens"));
            }

            if (charge.HasValue) ab.Charge(charge.Value);

            // use the mass number to specify isotope?
            if (isomeric)
            {
                int? massNumber = a.MassNumber;
                if (massNumber.HasValue)
                {
                    // XXX: likely causing some overhead but okay for now
                    try
                    {
                        IsotopeFactory isotopes = Isotopes.Instance;
                        IIsotope isotope = isotopes.GetMajorIsotope(a.Symbol);
                        if (isotope == null || !isotope.MassNumber.Equals(massNumber)) ab.Isotope(massNumber.Value);
                    }
                    catch (IOException e)
                    {
                        throw new ApplicationException("Isotope factory wouldn't load: " + e.Message);
                    }
                }
            }

            int? atomClass = a.GetProperty<int?>(CDKPropertyName.AtomAtomMapping);
            if (atomClasses && atomClass.HasValue)
            {
                ab.AtomClass(atomClass.Value);
            }

            return ab.Build();
        }

        /// <summary>
        /// Convert a CDK <see cref="IBond"/> to a Beam edge.
        /// </summary>
        /// <param name="b">the CDK bond</param>
        /// <param name="indices">map of atom indices</param>
        /// <returns>a Beam edge</returns>
        /// <exception cref="ArgumentException">the bond did not have 2 atoms or an unsupported order</exception>
        /// <exception cref="NullReferenceException">the bond order was undefined</exception>
        public Edge ToBeamEdge(IBond b, IDictionary<IAtom, int> indices)
        {
            CheckArgument(b.Atoms.Count == 2, "Invalid number of atoms on bond");

            int u = indices[b.Atoms[0]];
            int v = indices[b.Atoms[1]];

            return ToBeamEdgeLabel(b).CreateEdge(u, v);
        }

        /// <summary>
        /// Convert a CDK <see cref="IBond"/> to the Beam edge label type.
        /// </summary>
        /// <param name="b">cdk bond</param>
        /// <returns>the edge label for the Beam edge</returns>
        /// <exception cref="NullReferenceException">the bond order was null and the bond was not-aromatic</exception>
        /// <exception cref="ArgumentException">the bond order could not be converted</exception>
        private Bond ToBeamEdgeLabel(IBond b)
        {
            if (this.aromatic && b.IsAromatic) return Bond.Aromatic;

            if (b.Order.IsUnset) throw new CDKException("A bond had undefined order, possible query bond?");

            BondOrder order = b.Order;

            if (order == BondOrder.Single)
            {
                return Bond.Single;
            }
            else if (order == BondOrder.Double)
            {
                return Bond.Double;
            }
            else if (order == BondOrder.Triple)
            {
                return Bond.Triple;
            }
            else if (order == BondOrder.Quadruple)
            {
                return Bond.Quadruple;
            }
            else
                throw new CDKException("Unsupported bond order: " + order);
        }

        /// <summary>
        /// Add double-bond stereo configuration to the Beam GraphBuilder.
        /// </summary>
        /// <param name="dbs">stereo element specifying double-bond configuration</param>
        /// <param name="gb">the current graph builder</param>
        /// <param name="indices">atom indices</param>
        private void AddGeometricConfiguration(IDoubleBondStereochemistry dbs, GraphBuilder gb, IDictionary<IAtom, int> indices)
        {
            IBond db = dbs.StereoBond;
            var bs = dbs.Bonds;

            // don't try to set a configuration on aromatic bonds
            if (this.aromatic && db.IsAromatic) return;

            int u = indices[db.Atoms[0]];
            int v = indices[db.Atoms[1]];

            // is bs[0] always connected to db.Atom(0)?
            int x = indices[bs[0].GetConnectedAtom(db.Atoms[0])];
            int y = indices[bs[1].GetConnectedAtom(db.Atoms[1])];

            if (dbs.Stereo == DoubleBondConformation.Together)
            {
                gb.Geometric(u, v).Together(x, y);
            }
            else
            {
                gb.Geometric(u, v).Opposite(x, y);
            }
        }

        /// <summary>
        /// Add tetrahedral stereo configuration to the Beam GraphBuilder.
        /// </summary>
        /// <param name="tc">stereo element specifying tetrahedral configuration</param>
        /// <param name="gb">the current graph builder</param>
        /// <param name="indices">atom indices</param>
        private void AddTetrahedralConfiguration(ITetrahedralChirality tc, GraphBuilder gb, IDictionary<IAtom, int> indices)
        {
            var ligands = tc.Ligands;

            int u = indices[tc.ChiralAtom];
            int[] vs = new int[] {
                indices[ligands[0]],
                indices[ligands[1]],
                indices[ligands[2]],
                indices[ligands[3]], };

            gb.CreateTetrahedral(u).LookingFrom(vs[0]).Neighbors(vs[1], vs[2], vs[3])
                    .Winding(tc.Stereo == TetrahedralStereo.Clockwise ? Configuration.Clockwise : Configuration.AntiClockwise).Build();
        }

        /// <summary>
        /// Add extended tetrahedral stereo configuration to the Beam GraphBuilder.
        /// </summary>
        /// <param name="et">stereo element specifying tetrahedral configuration</param>
        /// <param name="gb">the current graph builder</param>
        /// <param name="indices">atom indices</param>
        private void AddExtendedTetrahedralConfiguration(ExtendedTetrahedral et, GraphBuilder gb, IDictionary<IAtom, int> indices)
        {
            IAtom[] ligands = et.Peripherals;

            int u = indices[et.Focus];
            int[] vs = new int[]{
                indices[ligands[0]],
                indices[ligands[1]],
                indices[ligands[2]],
                indices[ligands[3]], };

            gb.CreateExtendedTetrahedral(u).LookingFrom(vs[0]).Neighbors(vs[1], vs[2], vs[3])
                    .Winding(et.Winding == TetrahedralStereo.Clockwise ? Configuration.Clockwise : Configuration.AntiClockwise).Build();
        }
    }
}
