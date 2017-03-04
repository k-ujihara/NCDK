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
using NCDK.Beam;
using static NCDK.Beam.Configuration.Types;
using NCDK.Common.Collections;
using NCDK.Stereo;

namespace NCDK.Smiles
{
    /// <summary>
    /// Convert the Beam toolkit object model to the CDK. Currently the aromatic
    /// bonds from SMILES are loaded as singly bonded <see cref="IBond"/>s with the {@link
    /// org.openscience.cdk.CDKConstants#ISAROMATIC} flag set.
    /// </summary>
    /// <example><code>
    /// IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;
    /// ChemicalGraph      g       = ChemicalGraph.FromSmiles("CCO");
    ///
    /// BeamToCDK          g2c     = new BeamToCDK(builder);
    ///
    /// // make sure the Beam notation is expanded - this converts organic
    /// // subset atoms with inferred hydrogen counts to atoms with a
    /// // set implicit hydrogen property
    /// IAtomContainer    ac       = g2c.ToAtomContainer(Functions.Expand(g));
    /// </code></example>
    /// <remarks><a href="http://johnmay.github.io/beam">Beam SMILES Toolkit</a></remarks>
    // @author John May
    // @cdk.module smiles
    internal sealed class BeamToCDK
    {
        /// <summary>The builder used to create the CDK objects.</summary>
        private readonly IChemObjectBuilder builder;

        /// <summary> Base atom objects for cloning - SMILES is very efficient and noticeable
        ///  lag is seen using the IChemObjectBuilders./// </summary>
        private readonly IAtom templateAtom;

        /// <summary> Base atom objects for cloning - SMILES is very efficient and noticeable
        ///  lag is seen using the IChemObjectBuilders./// </summary>
        private readonly IBond templateBond;

        /// <summary>
        /// Base atom container for cloning - SMILES is very efficient and noticeable
        /// lag is seen using the IChemObjectBuilders.
        /// </summary>
        private readonly IAtomContainer emptyContainer;

        /// <summary>
        /// Create a new converter for the Beam SMILES toolkit. The converter needs
        /// an <see cref="IChemObjectBuilder"/>. Currently the 'cdk-silent' builder will
        /// give the best performance.
        ///
        // @param builder chem object builder
        /// </summary>
        public BeamToCDK(IChemObjectBuilder builder)
        {
            this.builder = builder;
            this.templateAtom = builder.CreateAtom();
            this.templateBond = builder.CreateBond();
            this.emptyContainer = builder.CreateAtomContainer();
        }

        /// <summary>
        /// Convert a Beam ChemicalGraph to a CDK IAtomContainer.
        /// </summary>
        /// <param name="g">Beam graph instance</param>
        /// <returns>the CDK <see cref="IAtomContainer"/> for the input</returns>
        /// <exception cref="ArgumentException">the Beam graph was not 'expanded' - and
        /// contained organic subset atoms. If this happens use the Beam Functions.Expand() to</exception>
        public IAtomContainer ToAtomContainer(Graph g)
        {
            IAtomContainer ac = CreateEmptyContainer();
            IAtom[] atoms = new IAtom[g.Order];
            IBond[] bonds = new IBond[g.Size];

            int j = 0; // bond index

            for (int i = 0; i < g.Order; i++)
                atoms[i] = ToCDKAtom(g.GetAtom(i), g.ImplHCount(i));
            foreach (var e in g.Edges)
                bonds[j++] = ToCDKBond(e, atoms);

            // atom-centric stereo-specification (only tetrahedral ATM)
            for (int u = 0; u < g.Order; u++)
            {

                Configuration c = g.ConfigurationOf(u);
                if (c.Type == Tetrahedral)
                {
                    IStereoElement se = NewTetrahedral(u, g.Neighbors(u), atoms, c);

                    if (se != null) ac.Add(se);
                }
                else if (c.Type == Configuration.Types.ExtendedTetrahedral)
                {
                    IStereoElement se = NewExtendedTetrahedral(u, g, atoms);

                    if (se != null) ac.Add(se);
                }
            }

            ac.SetAtoms(atoms);
            ac.SetBonds(bonds);

            // use directional bonds to assign bond-based stereo-specification
            AddDoubleBondStereochemistry(g, ac);

            // title suffix
            ac.SetProperty(CDKPropertyName.TITLE, g.Title);

            return ac;
        }

        /// <summary>
        /// Adds double-bond conformations (<see cref="DoubleBondStereochemistry"/>) to the
        /// atom-container.
        /// </summary>
        /// <param name="g">Beam graph object (for directional bonds)</param>
        /// <param name="ac">The atom-container built from the Beam graph</param>
        private void AddDoubleBondStereochemistry(Graph g, IAtomContainer ac)
        {
            foreach (var e in g.Edges)
            {
                if (e.Bond != Bond.Double) continue;

                int u = e.Either();
                int v = e.Other(u);

                // find a directional bond for either end
                Edge first = FindDirectionalEdge(g, u);
                Edge second = FindDirectionalEdge(g, v);

                // if either atom is not incident to a directional label there
                // is no configuration
                if (first == null || second == null) continue;

                // if the directions (relative to the double bond) are the
                // same then they are on the same side - otherwise they
                // are opposite
                DoubleBondConformation conformation = first.GetBond(u) == second.GetBond(v) ? DoubleBondConformation.Together : DoubleBondConformation.Opposite;

                // get the stereo bond and build up the ligands for the
                // stereo-element - linear search could be improved with
                // map or API change to double bond element
                IBond db = ac.GetBond(ac.Atoms[u], ac.Atoms[v]);

                IBond[] ligands = new IBond[]{ac.GetBond(ac.Atoms[u], ac.Atoms[first.Other(u)]),
                        ac.GetBond(ac.Atoms[v], ac.Atoms[second.Other(v)])};

                ac.Add(new DoubleBondStereochemistry(db, ligands, conformation));
            }
        }

        /// <summary>
        /// Utility for find the first directional edge incident to a vertex. If
        /// there are no directional labels then null is returned.
        /// </summary>
        /// <param name="g">graph from Beam</param>
        /// <param name="u">the vertex for which to find</param>
        /// <returns>first directional edge (or <see langword="null"/> if none)</returns>
        private Edge FindDirectionalEdge(Graph g, int u)
        {
            foreach (var e in g.GetEdges(u))
            {
                Bond b = e.Bond;
                if (b == Bond.Up || b == Bond.Down) return e;
            }
            return null;
        }

        /// <summary>
        /// Creates a tetrahedral element for the given configuration. Currently only
        /// tetrahedral centres with 4 explicit atoms are handled.
        /// </summary>
        /// <param name="u">central atom</param>
        /// <param name="vs">neighboring atom indices (in order)</param>
        /// <param name="atoms">array of the CDK atoms (pre-converted)</param>
        /// <param name="c">the configuration of the neighbors (vs) for the order they are given</param>
        /// <returns>tetrahedral stereo element for addition to an atom container</returns>
        private IStereoElement NewTetrahedral(int u, int[] vs, IAtom[] atoms, Configuration c)
        {
            // no way to handle tetrahedral configurations with implicit
            // hydrogen or lone pair at the moment
            if (vs.Length != 4)
            {

                // sanity check
                if (vs.Length != 3) return null;

                // there is an implicit hydrogen (or lone-pair) we insert the
                // central atom in sorted position
                vs = Insert(u, vs);
            }

            // @TH1/@TH2 = anti-clockwise and clockwise respectively
            TetrahedralStereo stereo = c == Configuration.TH1 ? TetrahedralStereo.AntiClockwise : TetrahedralStereo.Clockwise;

            return new TetrahedralChirality(atoms[u], new IAtom[] { atoms[vs[0]], atoms[vs[1]], atoms[vs[2]], atoms[vs[3]] },
                    stereo);
        }

        private IStereoElement NewExtendedTetrahedral(int u, Graph g, IAtom[] atoms)
        {
            int[] terminals = g.Neighbors(u);
            int[] xs = new int[] { -1, terminals[0], -1, terminals[1] };

            int n = 0;
            foreach (var e in g.GetEdges(terminals[0]))
            {
                if (e.Bond.Order == 1) xs[n++] = e.Other(terminals[0]);
            }
            n = 2;
            foreach (var e in g.GetEdges(terminals[1]))
            {
                if (e.Bond.Order == 1) xs[n++] = e.Other(terminals[1]);
            }

            Array.Sort(xs);

            TetrahedralStereo stereo = g.ConfigurationOf(u).Shorthand == Configuration.Clockwise ? TetrahedralStereo.Clockwise
                    : TetrahedralStereo.AntiClockwise;

            return new ExtendedTetrahedral(atoms[u], new IAtom[]{atoms[xs[0]], atoms[xs[1]],
                    atoms[xs[2]], atoms[xs[3]]}, stereo);
        }

        /// <summary>
        /// Insert the vertex 'v' into sorted position in the array 'vs'.
        /// </summary>
        /// <param name="v">a vertex (int id)</param>
        /// <param name="vs">array of vertices (int ids)</param>
        /// <returns>array with 'u' inserted in sorted order</returns>
        private static int[] Insert(int v, int[] vs)
        {
            int n = vs.Length;
            int[] ws = Arrays.CopyOf(vs, n + 1);
            ws[n] = v;

            // insert 'u' in to sorted position
            for (int i = n; i > 0 && ws[i] < ws[i - 1]; i--)
            {
                int tmp = ws[i];
                ws[i] = ws[i - 1];
                ws[i - 1] = tmp;
            }

            return ws;
        }

        /// <summary>
        /// Create a new CDK <see cref="IAtom"/> from the Beam Atom.
        /// </summary>
        /// <param name="beamAtom">an Atom from the Beam ChemicalGraph</param>
        /// <param name="hCount">hydrogen count for the atom</param>
        /// <returns>the CDK atom to have it's properties set</returns>
        public IAtom ToCDKAtom(Atom beamAtom, int hCount)
        {
            IAtom cdkAtom = NewCDKAtom(beamAtom);

            cdkAtom.ImplicitHydrogenCount = hCount;
            cdkAtom.FormalCharge = beamAtom.Charge;

            if (beamAtom.Isotope >= 0) cdkAtom.MassNumber = beamAtom.Isotope;

            if (beamAtom.IsAromatic()) cdkAtom.IsAromatic = true;

            if (beamAtom.AtomClass > 0) cdkAtom.SetProperty(CDKPropertyName.ATOM_ATOM_MAPPING, beamAtom.AtomClass);

            return cdkAtom;
        }

        /// <summary>
        /// Create a new CDK <see cref="IAtom"/> from the Beam Atom. If the element is
        /// unknown (i.e. '*') then an pseudo atom is created.
        /// </summary>
        /// <param name="atom">an Atom from the Beam Graph</param>
        /// <returns>the CDK atom to have it's properties set</returns>
        public IAtom NewCDKAtom(Atom atom)
        {
            Element element = atom.Element;
            bool unknown = element == Element.Unknown;
            if (unknown)
            {
                IPseudoAtom pseudoAtom = builder.CreatePseudoAtom(element.Symbol);
                pseudoAtom.Symbol = element.Symbol;
                pseudoAtom.Label = atom.Label;
                return pseudoAtom;
            }
            return CreateAtom(element);
        }

        /// <summary>
        /// Convert an edge from the Beam Graph to a CDK bond. Note -
        /// currently aromatic bonds are set to Single and then.
        /// </summary>
        /// <param name="edge">the Beam edge to convert</param>
        /// <param name="atoms">the already converted atoms</param>
        /// <returns>new bond instance</returns>
        public IBond ToCDKBond(Edge edge, IAtom[] atoms)
        {
            int u = edge.Either();
            int v = edge.Other(u);

            IBond bond = CreateBond(atoms[u], atoms[v], ToCDKBondOrder(edge));

            // switch on the edge label to set aromatic flags
            var aa = edge.Bond;
            if (aa == Bond.Aromatic || aa == Bond.ImplicitAromatic || aa == Bond.DoubleAromatic)
            {
                bond.IsAromatic = true;
                atoms[u].IsAromatic = true;
                atoms[v].IsAromatic = true;
            }
            else if (aa == Bond.Implicit)
            {
                if (atoms[u].IsAromatic && atoms[v].IsAromatic)
                {
                    bond.IsAromatic = true;
                    atoms[u].IsAromatic = true;
                    atoms[v].IsAromatic = true;
                }
            }

            return bond;
        }

        /// <summary>
        /// Convert bond label on the edge to a CDK bond order - there is no aromatic
        /// bond order and as such this is currently set 'Single' with the aromatic
        /// flag to be set.
        /// </summary>
        /// <param name="edge">beam edge</param>
        /// <returns>CDK bond order for the edge type</returns>
        /// <exception cref="ArgumentException">the bond was a 'DOT' - should not be loaded but the exception is there in-case</exception>
        private BondOrder ToCDKBondOrder(Edge edge)
        {
            var aa = edge.Bond;
            if (aa == Bond.Single ||
                aa == Bond.Up ||
                aa == Bond.Down ||
                aa == Bond.Implicit || // single/aromatic - aromatic ~ single atm.
                aa == Bond.ImplicitAromatic ||
                aa == Bond.Aromatic)  // we will also set the flag
                return BondOrder.Single;
            if (aa == Bond.Double ||
                aa == Bond.DoubleAromatic)
                return BondOrder.Double;
            if (aa == Bond.Triple)
                return BondOrder.Triple;
            if (aa == Bond.Quadruple)
                return BondOrder.Quadruple;
            throw new ArgumentException("Edge label " + edge.Bond
               + "cannot be converted to a CDK bond order");
        }

        /// <summary>
        /// Create a new empty atom container instance.
        /// </summary>
        /// <returns>a new atom container instance</returns>
        private IAtomContainer CreateEmptyContainer()
        {
            try
            {
                return (IAtomContainer)emptyContainer.Clone();
            }
            catch (Exception)
            {
                return builder.CreateAtomContainer();
            }
        }

        /// <summary>
        /// Create a new atom for the provided symbol. The atom is created by cloning
        /// an existing 'template'. Unfortunately IChemObjectBuilders really show a
        /// slow down when SMILES processing.
        /// </summary>
        /// <param name="element">Beam element</param>
        /// <returns>new atom with configured symbol and atomic number</returns>
        private IAtom CreateAtom(Element element)
        {
            try
            {
                IAtom atom = (IAtom)templateAtom.Clone();
                atom.Symbol = element.Symbol;
                atom.AtomicNumber = element.AtomicNumber;
                return atom;
            }
            catch (Exception)
            {
                // clone is always supported if overridden but just in case :-)
                return builder.CreateAtom(element.Symbol);
            }
        }

        /// <summary>
        /// Create a new bond for the provided symbol. The bond is created by cloning
        /// an existing 'template'. Unfortunately IChemObjectBuilders really show a
        /// slow down when SMILES processing.
        /// </summary>
        /// <param name="either">an atom of the bond</param>
        /// <param name="other">another atom of the bond</param>
        /// <param name="order">the order of the bond</param>
        /// <returns>new bond instance</returns>
        private IBond CreateBond(IAtom either, IAtom other, BondOrder order)
        {
            IBond bond = (IBond)templateBond.Clone();
            bond.SetAtoms(new IAtom[] { either, other });
            bond.Order = order;
            return bond;
        }
    }
}

