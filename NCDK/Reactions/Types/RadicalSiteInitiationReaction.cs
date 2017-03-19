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
    /// <para>IReactionProcess which participate mass spectrum process. Homolitic dissocitation.
    /// This reaction could be represented as A-B-[c*] => [A*] + B=C.</para>
    /// <para>Make sure that the molecule has the corresponend lone pair electrons
    /// for each atom. You can use the method: <see cref="Tools.LonePairElectronChecker"/></para>
    /// <para>It is processed by the RadicalSiteIonizationMechanism class</para>
    /// </summary>
    /// <example>
    /// <code>
    ///  IAtomContainerSet setOfReactants = Default.ChemObjectBuilder.Instance.NewAtomContainerSet();
    ///  setOfReactants.Add(new AtomContainer());
    ///  IReactionProcess type = new RadicalSiteInitiationReaction();
    ///  object[] parameters = {bool.FALSE};
    ///  type.Parameters = parameters;
    ///  IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
    ///  </code>
    ///
    /// <para>We have the possibility to localize the reactive center. Good method if you
    /// want to localize the reaction in a fixed point</para>
    /// <code>atoms[0].IsReactiveCenter = true;</code>
    /// <para>Moreover you must put the parameter true</para>
    /// <para>If the reactive center is not localized then the reaction process will
    /// try to find automatically the possible reactive center.</para>
    /// </example>
    /// <seealso cref="Mechanisms.RadicalSiteIonizationMechanism"/>
    // @author         Miguel Rojas
    // @cdk.created    2006-05-05
    // @cdk.module     reaction
    // @cdk.githash
    // @cdk.set        reaction-types
    public class RadicalSiteInitiationReaction : ReactionEngine, IReactionProcess
    {

        /// <summary>
        /// Constructor of the RadicalSiteInitiationReaction object
        /// </summary>
        public RadicalSiteInitiationReaction() { }

        /// <summary>
        ///  Gets the specification attribute of the RadicalSiteInitiationReaction object
        /// </summary>
        /// <returns>The specification value</returns>
        public ReactionSpecification Specification =>
            new ReactionSpecification(
                    "http://almost.cubic.uni-koeln.de/jrg/Members/mrc/reactionDict/reactionDict#RadicalSiteInitiation",
                    this.GetType().Name, "$Id$", "The Chemistry Development Kit");

        /// <summary>
        ///  Initiate process.
        /// </summary>
        /// <exception cref="CDKException"> Description of the Exception</exception>
        /// <param name="reactants">reactants of the reaction.</param>
        /// <param name="agents">agents of the reaction (Must be in this case null).</param>
        public IReactionSet Initiate(IAtomContainerSet<IAtomContainer> reactants, IAtomContainerSet<IAtomContainer> agents)
        {
            Debug.WriteLine("initiate reaction: RadicalSiteInitiationReaction");

            if (reactants.Count != 1)
            {
                throw new CDKException("RadicalSiteInitiationReaction only expects one reactant");
            }
            if (agents != null)
            {
                throw new CDKException("RadicalSiteInitiationReaction don't expects agents");
            }

            IReactionSet setOfReactions = reactants.Builder.CreateReactionSet();
            IAtomContainer reactant = reactants[0];

            // if the parameter hasActiveCenter is not fixed yet, set the active centers
            IParameterReact ipr = base.GetParameterClass(typeof(SetReactionCenter));
            if (ipr != null && !ipr.IsSetParameter) SetActiveCenters(reactant);

            foreach (var atomi in reactants[0].Atoms)
            {
                if (atomi.IsReactiveCenter && reactant.GetConnectedSingleElectrons(atomi).Count() == 1
                        && atomi.FormalCharge == 0)
                {

                    foreach (var bondi in reactant.GetConnectedBonds(atomi))
                    {
                        if (bondi.IsReactiveCenter && bondi.Order == BondOrder.Single)
                        {

                            IAtom atomj = bondi.GetConnectedAtom(atomi);
                            if (atomj.IsReactiveCenter && atomj.FormalCharge == 0)
                            {

                                foreach (var bondj in reactant.GetConnectedBonds(atomj))
                                {
                                    if (bondj.Equals(bondi)) continue;

                                    if (bondj.IsReactiveCenter
                                            && bondj.Order == BondOrder.Single)
                                    {

                                        IAtom atomk = bondj.GetConnectedAtom(atomj);
                                        if (atomk.IsReactiveCenter && atomk.Symbol.Equals("C")
                                                && atomk.FormalCharge == 0)
                                        {

                                            var atomList = new List<IAtom>();
                                            atomList.Add(atomi);
                                            atomList.Add(atomj);
                                            atomList.Add(atomk);
                                            var bondList = new List<IBond>();
                                            bondList.Add(bondi);
                                            bondList.Add(bondj);

                                            IAtomContainerSet<IAtomContainer> moleculeSet = reactant.Builder.CreateAtomContainerSet<IAtomContainer>();

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
                    }
                }
            }
            return setOfReactions;

        }

        /// <summary>
        /// set the active center for this molecule.
        /// The active center will be those which correspond with A-B-[C*].
        /// <code>
        /// A: Atom
        /// -: bond
        /// B: Atom
        /// -: bond
        /// C: Atom with single electron
        ///  </code>
        /// </summary>
        /// <param name="reactant">The molecule to set the activity</param>
        private void SetActiveCenters(IAtomContainer reactant)
        {
            foreach (var atomi in reactant.Atoms)
            {
                if (reactant.GetConnectedSingleElectrons(atomi).Count() == 1 && atomi.FormalCharge == 0)
                {
                    foreach (var bondi in reactant.GetConnectedBonds(atomi))
                    {
                        if (bondi.Order == BondOrder.Single)
                        {
                            IAtom atomj = bondi.GetConnectedAtom(atomi);
                            if (atomj.FormalCharge == 0)
                            {
                                foreach (var bondj in reactant.GetConnectedBonds(atomj))
                                {
                                    if (bondj.Equals(bondi)) continue;

                                    if (bondj.Order == BondOrder.Single)
                                    {
                                        IAtom atomk = bondj.GetConnectedAtom(atomj);
                                        if (atomk.Symbol.Equals("C") && atomk.FormalCharge == 0)
                                        {
                                            atomi.IsReactiveCenter = true;
                                            atomj.IsReactiveCenter = true;
                                            atomk.IsReactiveCenter = true;
                                            bondi.IsReactiveCenter = true;
                                            bondj.IsReactiveCenter = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
