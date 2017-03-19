/* Copyright (C) 2008 Miguel Rojas <miguelrojasch@users.sf.net>
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT Any WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Reactions.Types.Parameters;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NCDK.Reactions.Types
{
    /// <summary>
    /// <para>IReactionProcess which participate in movement resonance.
    /// This reaction could be represented as [A+]-B| => A=[B+]. Due to
    /// deficiency of charge of the atom A, the lone pair electron of the atom A is
    /// desplaced.</para>
    /// <para>Make sure that the molecule has the correspond lone pair electrons
    /// for each atom. You can use the method: <see cref="Tools.LonePairElectronChecker"/></para>
    /// </summary>
    /// <example>
    /// <code>
    ///  IAtomContainerSet setOfReactants = Default.ChemObjectBuilder.Instance.NewAtomContainerSet();
    ///  setOfReactants.Add(new AtomContainer());
    ///  IReactionProcess type = new SharingLonePairReaction();
    ///  object[] parameters = {bool.FALSE};
    ///  type.Parameters = parameters;
    ///  IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    /// </example>
    /// <remarks>
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</para>
    /// <code>atoms[0].IsReactiveCenter = true;</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</para>
    /// </remarks>
    // @author         Miguel Rojas
    // @cdk.created    2006-05-05
    // @cdk.module     reaction
    // @cdk.githash
    // @cdk.set        reaction-types
    public class SharingLonePairReaction : ReactionEngine, IReactionProcess
    {
        /// <summary>
        /// Constructor of the SharingLonePairReaction object.
        /// </summary>
        public SharingLonePairReaction() { }

        /// <summary>
        ///  Gets the specification attribute of the SharingLonePairReaction object.
        /// </summary>
        /// <returns>The specification value</returns>
        public ReactionSpecification Specification =>
            new ReactionSpecification(
                    "http://almost.cubic.uni-koeln.de/jrg/Members/mrc/reactionDict/reactionDict#SharingLonePair", this
                            .GetType().Name, "$Id$", "The Chemistry Development Kit");

        /// <summary>
        ///  Initiate process.
        ///  It is needed to call the addExplicitHydrogensToSatisfyValency
        ///  from the class tools.HydrogenAdder.
        /// </summary>
        /// <exception cref="CDKException"> Description of the Exception</exception>
        /// <param name="reactants">reactants of the reaction.</param>
        /// <param name="agents">agents of the reaction (Must be in this case null).</param>
        public IReactionSet Initiate(IAtomContainerSet<IAtomContainer> reactants, IAtomContainerSet<IAtomContainer> agents)
        {
            Debug.WriteLine("initiate reaction: SharingLonePairReaction");

            if (reactants.Count != 1)
            {
                throw new CDKException("SharingLonePairReaction only expects one reactant");
            }
            if (agents != null)
            {
                throw new CDKException("SharingLonePairReaction don't expects agents");
            }

            IReactionSet setOfReactions = reactants.Builder.CreateReactionSet();
            IAtomContainer reactant = reactants[0];

            // if the parameter hasActiveCenter is not fixed yet, set the active centers
            IParameterReact ipr = base.GetParameterClass(typeof(SetReactionCenter));
            if (ipr != null && !ipr.IsSetParameter) SetActiveCenters(reactant);

            foreach (var atomi in reactant.Atoms)
            {
                if (atomi.IsReactiveCenter && atomi.FormalCharge == 0
                        && !reactant.GetConnectedSingleElectrons(atomi).Any()
                        && reactant.GetConnectedLonePairs(atomi).Any())
                {

                    foreach (var bondi in reactant.GetConnectedBonds(atomi))
                    {
                        if (bondi.IsReactiveCenter && bondi.Order == BondOrder.Single)
                        {
                            IAtom atomj = bondi.GetConnectedAtom(atomi);
                            if (atomj.IsReactiveCenter && atomj.FormalCharge == 1
                                    && !reactant.GetConnectedSingleElectrons(atomj).Any())
                            {

                                var atomList = new List<IAtom>();
                                atomList.Add(atomi);
                                atomList.Add(atomj);
                                var bondList = new List<IBond>();
                                bondList.Add(bondi);

                                IAtomContainerSet<IAtomContainer> moleculeSet = reactant.Builder.CreateAtomContainerSet();
                                moleculeSet.Add(reactant);
                                IReaction reaction = Mechanism.Initiate(moleculeSet, atomList, bondList);
                                if (reaction == null)
                                    continue;
                                else
                                    setOfReactions.Add(reaction);
                            }
                        }
                    }
                }
            }

            return setOfReactions;

        }

        /// <summary>
        /// set the active center for this molecule.
        /// The active center will be those which correspond with [A+]-B|.
        /// <code>
        /// A: Atom with positive charge
        /// -: Single bond
        /// B: Atom with lone pair electrons
        ///  </code>
        /// </summary>
        /// <param name="reactant">The molecule to set the activity</param>
        private void SetActiveCenters(IAtomContainer reactant)
        {
            foreach (var atomi in reactant.Atoms)
            {
                if (atomi.FormalCharge == 0 && !reactant.GetConnectedSingleElectrons(atomi).Any()
                        && reactant.GetConnectedLonePairs(atomi).Any())
                {

                    foreach (var bondi in reactant.GetConnectedBonds(atomi))
                    {
                        if (bondi.Order == BondOrder.Single)
                        {
                            IAtom atomj = bondi.GetConnectedAtom(atomi);
                            if (atomj.FormalCharge == 1 && !reactant.GetConnectedSingleElectrons(atomj).Any())
                            {
                                atomi.IsReactiveCenter = true;
                                atomj.IsReactiveCenter = true;
                                bondi.IsReactiveCenter = true;
                            }
                        }
                    }
                }
            }
        }
    }
}
