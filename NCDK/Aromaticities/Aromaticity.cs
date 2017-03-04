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

using NCDK.Common.Collections;
using NCDK.Graphs;
using NCDK.RingSearches;
using System.Collections.Generic;
using static NCDK.Graphs.GraphUtil;
using static NCDK.Common.Base.Preconditions;

namespace NCDK.Aromaticities
{
    /// <summary>
    /// A configurable model to perceive aromatic systems. Aromaticity is useful as
    /// both a chemical property indicating stronger stabilisation and as a way to
    /// treat different resonance forms as equivalent. Each has its own implications
    /// the first in physicochemical attributes and the second in similarity,
    /// depiction and storage.
    /// <para>
    /// To address the resonance forms, several simplified (sometimes conflicting)
    /// models have arisen. Generally the models <b>loosely</b> follow
    /// <a href="http://en.wikipedia.org/wiki/H%C3%BCckel's_rule">Hückel's rule</a>
    /// for determining aromaticity. A common omission being that planarity is not
    /// tested and chemical compounds which are non-planar can be perceived
    /// as aromatic. An example of one such compound is, cyclodeca-1,3,5,7,9-pentaene.
    /// </para>
    /// <para>
    /// Although there is not a single universally accepted model there are models
    /// which may better suited for a specific use (<a href="http://www.slideshare.net/NextMoveSoftware/cheminformatics-toolkits-a-personal-perspective">Cheminformatics Toolkits: A Personal Perspective, Roger Sayle</a>).
    /// The different models are often ill-defined or unpublished but it is important
    /// to acknowledge that there are differences (see. <a href="http://blueobelisk.shapado.com/questions/aromaticity-perception-differences">Aromaticity Perception Differences, Blue Obelisk</a>).
    /// </para>
    /// <para>
    /// Although models may get more complicated (e.g. considering tautomers)
    /// normally the reasons for differences are:
    /// <list type="bullet">
    ///     <item>the atoms allowed and how many electrons each contributes</item>
    ///     <item>the rings/cycles are tested</item>
    /// </list>
    /// </para>
    /// <para>
    /// This implementation allows configuration of these via an <see cref="ElectronDonation"/> model and <see cref="CycleFinder"/>. To obtain an instance
    /// of the electron donation model use one of the factory methods,
    /// <see cref="ElectronDonation.Cdk"/>, <see cref="ElectronDonation.CdkAllowingExocyclic"/>,
    /// <see cref="ElectronDonation.Daylight"/> or <see cref="ElectronDonation.PiBonds"/>.
    /// </para>
    /// </summary>
    /// <example>
    /// <code>
    /// // mimics the old CDKHuckelAromaticityDetector which uses the CDK atom types
    /// ElectronDonation model       = ElectronDonation.CDK();
    /// CycleFinder      cycles      = Cycles.CDKAromaticSet;
    /// Aromaticity      aromaticity = new Aromaticity(model, cycles);
    /// // apply our configured model to each molecule, the CDK model
    /// // requires that atom types are perceived
    /// foreach (var molecule in molecules) {
    ///     AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
    ///     aromaticity.Apply(molecule);
    /// }
    /// </code>
    /// </example>
    /// <remarks>
    /// <list type="bullet">
    /// <item><a href="http://en.wikipedia.org/wiki/H%C3%BCckel's_rule">Hückel's rule</a></item>
    /// <item><a href="http://www.slideshare.net/NextMoveSoftware/cheminformatics-toolkits-a-personal-perspective">Cheminformatics Toolkits: A Personal Perspective, Roger Sayle</a></item>
    /// <item><a href="http://blueobelisk.shapado.com/questions/aromaticity-perception-differences">Aromaticity Perception Differences, Blue Obelisk</a></item>
    /// </list>
    /// </remarks>
    // @author John May
    // @cdk.module standard
    // @cdk.githash
    public sealed class Aromaticity
    {
        /// <summary>Find how many electrons each atom contributes.</summary>
        private readonly ElectronDonation model;

        /// <summary>The method to find cycles which will be tested for aromaticity.</summary>
        private readonly CycleFinder cycles;

        /// <summary>
        /// Create an aromaticity model using the specified electron donation {@code
        /// model} which is tested on the <paramref name="cycles"/>. The <paramref name="model"/> defines
        /// how many π-electrons each atom may contribute to an aromatic system. The
        /// <paramref name="cycles"/> defines the <see cref="CycleFinder"/> which is used to find
        /// cycles in a molecule. The total electron donation from each atom in each
        /// cycle is counted and checked. If the electron contribution is equal to
        /// {@code 4n + 2} for a {@code n >= 0} then the cycle is considered
        /// aromatic. 
        /// </summary>
        /// <remarks>
        /// Changing the electron contribution model or which cycles
        /// are tested affects which atoms/bonds are found to be aromatic. There are
        /// several <see cref="ElectronDonation"/> models and <see cref="Cycles"/>
        /// available. A good choice for the cycles
        /// is to use <see cref="Cycles.All"/> falling back to
        /// <see cref="Cycles.Relevant"/> on failure. Finding all cycles is very
        /// fast but may produce an exponential number of cycles. It is therefore not
        /// feasible for complex fused systems and an exception is thrown.
        /// In such cases the aromaticity can either be skipped or a simpler
        /// polynomial cycle set <see cref="Cycles.Relevant"/> used.
        /// </remarks>
        /// <example>
        /// <code>
        /// // mimics the CDKHuckelAromaticityDetector
        /// Aromaticity aromaticity = new Aromaticity(ElectronDonation.CDK(), Cycles.CDKAromaticSet);
        /// // mimics the DoubleBondAcceptingAromaticityDetector
        /// Aromaticity aromaticity = new Aromaticity(ElectronDonation.CdkAllowingExocyclic(), Cycles.CDKAromaticSet);
        /// // a good model for writing SMILES
        /// Aromaticity aromaticity = new Aromaticity(ElectronDonation.Daylight(), Cycles.All());
        /// // a good model for writing MDL/Mol2
        /// Aromaticity aromaticity = new Aromaticity(ElectronDonation.PiBonds(), Cycles.All());
        /// </code>
        /// </example>
        /// <param name="model"></param>
        /// <param name="cycles"></param>
        /// <seealso cref="ElectronDonation"/>
        /// <seealso cref="Cycles"/>
        public Aromaticity(ElectronDonation model, CycleFinder cycles)
        {
            this.model = CheckNotNull(model);
            this.cycles = CheckNotNull(cycles);
        }

        /// <summary>
        /// Find the bonds of a <paramref name="molecule"/> which this model determined were aromatic.
        /// </summary>
        /// <example>
        /// <code>
        /// Aromaticity aromaticity = new Aromaticity(ElectronDonation.CDK(), Cycles.All());
        /// IAtomContainer container = ...;
        /// try {
        ///     ICollection&lt;IBond&gt;bonds = aromaticity.FindBonds(container);
        ///     int        nAromaticBonds = bonds.Count;
        /// } catch (CDKException e) {
        ///     // cycle computation was intractable
        /// }
        /// </code>
        /// </example>
        /// <param name="molecule">the molecule to apply the model to</param>
        /// <returns>the set of bonds which are aromatic</returns>
        /// <exception cref="">a problem occurred with the cycle perception - one can retry with a simpler cycle set</exception>
        public ICollection<IBond> FindBonds(IAtomContainer molecule)
        {
            // build graph data-structures for fast cycle perception
            EdgeToBondMap bondMap = EdgeToBondMap.WithSpaceFor(molecule);
            int[][] graph = GraphUtil.ToAdjList(molecule, bondMap);

            // initial ring/cycle search and get the contribution from each atom
            RingSearch ringSearch = new RingSearch(molecule, graph);
            int[] electrons = model.Contribution(molecule, ringSearch);

            ICollection<IBond> bonds = new List<IBond>();

            // obtain the subset of electron contributions which are >= 0 (i.e.
            // allowed to be aromatic) - we then find the cycles in this subgraph
            // and 'lift' the indices back to the original graph using the subset
            // as a lookup
            int[] subset = Subset(electrons);
            int[][] subgraph = GraphUtil.Subgraph(graph, subset);

            // for each cycle if the electron sum is valid add the bonds of the
            // cycle to the set or aromatic bonds
            foreach (var cycle in cycles.Find(molecule, subgraph, subgraph.Length).GetPaths())
            {
                if (CheckElectronSum(cycle, electrons, subset))
                {
                    for (int i = 1; i < cycle.Length; i++)
                    {
                        bonds.Add(bondMap[subset[cycle[i]], subset[cycle[i - 1]]]);
                    }
                }
            }

            return bonds;
        }

        /// <summary>
        /// Apply this aromaticity model to a molecule. Any existing aromaticity
        /// flags are removed - even if no aromatic bonds were found. This follows
        /// the idea of <i>applying</i> an aromaticity model to a molecule such that
        /// the result is the same irrespective of existing aromatic flags. If you
        /// require aromatic flags to be preserved the <see cref="FindBonds(IAtomContainer)"/>
        /// can be used to find bonds without setting any flags.
        /// <code>
        /// Aromaticity aromaticity = new Aromaticity(ElectronDonation.CDK(), Cycles.All());
        /// IAtomContainer container = ...;
        /// try {
        ///     if (aromaticity.Apply(container)) {
        ///         //
        ///     }
        /// } catch (CDKException e) {
        ///     // cycle computation was intractable
        /// }
        /// </code>
        /// </summary>
        /// <param name="molecule">the molecule to apply the model to</param>
        /// <returns>the model found the molecule was aromatic</returns>
        public bool Apply(IAtomContainer molecule)
        {
            ICollection<IBond> bonds = FindBonds(molecule);

            // clear existing flags
            molecule.IsAromatic = false;
            foreach (var bond in molecule.Bonds)
                bond.IsAromatic = false;
            foreach (var atom in molecule.Atoms)
                atom.IsAromatic = false;

            // set the new flags
            foreach (var bond in bonds)
            {
                bond.IsAromatic = true;
                bond.Atoms[0].IsAromatic = true;
                bond.Atoms[1].IsAromatic = true;
            }

            molecule.IsAromatic = bonds.Count != 0;
            
            return bonds.Count != 0;
        }

        /// <summary>
        /// Check if the number electrons in the <paramref name="cycle"/> could delocalise. The
        /// <paramref name="contributions"/> array indicates how many π-electrons each atom can
        /// contribute.
        /// </summary>
        /// <param name="cycle">closed walk (last and first vertex the same) of vertices which form a cycle</param>
        /// <param name="contributions">π-electron contribution from each atom</param>
        /// <returns>the number of electrons indicate they could delocalise</returns>
        private static bool CheckElectronSum(int[] cycle, int[] contributions, int[] subset)
        {
            return ValidSum(ElectronSum(cycle, contributions, subset));
        }

        /// <summary>
        /// Count the number electrons in the <paramref name="cycle"/>. The 
        /// <paramref name="contributions"/> array indicates how many π-electrons each atom can
        /// contribute. When the contribution of an atom is less than 0 the sum for
        /// the cycle is always 0.
        /// </summary>
        /// <param name="cycle">closed walk (last and first vertex the same) of vertices which form a cycle</param>
        /// <param name="contributions">π-electron contribution from each atom</param>
        /// <returns>the total sum of π-electrons contributed by the <paramref name="cycle"/></returns>
        internal static int ElectronSum(int[] cycle, int[] contributions, int[] subset)
        {
            int sum = 0;
            for (int i = 1; i < cycle.Length; i++)
                sum += contributions[subset[cycle[i]]];
            return sum;
        }

        /// <summary>
        /// Given the number of pi electrons verify that <c>sum = 4n + 2</c> for <c>n >= 0</c>.
        /// </summary>
        /// <param name="sum">π-electron sum</param>
        /// <returns>there is an <c>n</c> such that <c>sum = 4n + 2</c> is equal to the provided <c>sum</c>.</returns>
        internal static bool ValidSum(int sum)
        {
            return (sum - 2) % 4 == 0;
        }

        /// <summary>
        /// Obtain a subset of the vertices which can contribute <paramref name="electrons"/>
        /// and are allowed to be involved in an aromatic system.
        /// </summary>
        /// <param name="electrons">electron contribution</param>
        /// <returns>vertices which can be involved in an aromatic system</returns>
        private static int[] Subset(int[] electrons)
        {
            int[] vs = new int[electrons.Length];
            int n = 0;

            for (int i = 0; i < electrons.Length; i++)
                if (electrons[i] >= 0) vs[n++] = i;

            return Arrays.CopyOf(vs, n);
        }

        /// <summary>Replicates CDKHueckelAromaticityDetector.</summary>
        private static readonly Aromaticity CDK_LEGACY = new Aromaticity(ElectronDonation.Cdk(), Cycles.CDKAromaticSet);

        /// <summary>
        /// Access an aromaticity instance that replicates the previously utilised -
        /// CDKHueckelAromaticityDetector. It has the following configuration:
        /// <code>new Aromaticity(ElectronDonation.CDK(), Cycles.CDKAromaticSet);</code>
        /// <para>
        /// This model is not necessarily bad (or really considered legacy) but
        /// should <b>not</b> be considered a gold standard model that covers all
        /// possible cases. It was however the primary method used in previous
        /// versions of the CDK (1.4).
        /// </para>
        /// <para>
        /// This factory method is provided for convenience for
        /// those wishing to replicate aromaticity perception used in previous
        /// versions. The same electron donation model can be used to test
        /// aromaticity of more cycles. For instance, the following configuration
        /// will identify more bonds in a some structures as aromatic:
        /// <code>new Aromaticity(ElectronDonation.CDK(), Cycles.Or(Cycles.All(), Cycles.Relevant));</code>
        /// </para>
        /// </summary>
        public static Aromaticity CDKLegacy => CDK_LEGACY;
    }
}
