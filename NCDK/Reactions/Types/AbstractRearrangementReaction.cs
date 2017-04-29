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
using System.Linq;

namespace NCDK.Reactions.Types
{
    // @author         Miguel Rojas
    // @cdk.created    2006-05-05
    // @cdk.module     reaction
    // @cdk.githash
    public abstract class AbstractRearrangementReaction : ReactionEngine, IReactionProcess
    {
        public AbstractRearrangementReaction() { }

        /// <inheritdoc/>
        public abstract ReactionSpecification Specification { get; }

        /// <inheritdoc/>
        public abstract IReactionSet Initiate(IAtomContainerSet<IAtomContainer> reactants, IAtomContainerSet<IAtomContainer> agents);

        internal delegate bool CheckReactantAtom(IAtomContainer mol, IAtom atom);
        internal delegate bool CheckAtom(IAtom atom);
        
        internal IReactionSet Initiate(IAtomContainerSet<IAtomContainer> reactants, IAtomContainerSet<IAtomContainer> agents, CheckReactantAtom checkReatantAtom, CheckAtom checkAtom)
        {
            CheckInitiateParams(reactants, agents);

            IReactionSet setOfReactions = reactants.Builder.CreateReactionSet();
            IAtomContainer reactant = reactants[0];

            // if the parameter hasActiveCenter is not fixed yet, set the active centers
            IParameterReaction ipr = base.GetParameterClass(typeof(SetReactionCenter));
            if (ipr != null && !ipr.IsSetParameter) SetActiveCenters(reactant, checkReatantAtom, checkAtom);

            foreach (var atomi in reactant.Atoms)
            {
                if (atomi.IsReactiveCenter && checkReatantAtom(reactant, atomi))
                {
                    foreach (var bondi in reactant.GetConnectedBonds(atomi))
                    {
                        if (bondi.IsReactiveCenter && bondi.Order == BondOrder.Single)
                        {
                            IAtom atomj = bondi.GetConnectedAtom(atomi);
                            if (atomj.IsReactiveCenter
                                    && (atomj.FormalCharge ?? 0) == 0
                                    && !reactant.GetConnectedSingleElectrons(atomj).Any())
                            {
                                foreach (var bondj in reactant.GetConnectedBonds(atomj))
                                {
                                    if (bondj.Equals(bondi)) continue;

                                    if (bondj.IsReactiveCenter
                                            && bondj.Order == BondOrder.Double)
                                    {
                                        IAtom atomk = bondj.GetConnectedAtom(atomj);
                                        if (atomk.IsReactiveCenter
                                                && checkAtom(atomk)
                                                && !reactant.GetConnectedSingleElectrons(atomk).Any())
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
        
        private void SetActiveCenters(IAtomContainer reactant, CheckReactantAtom checkReatantAtom, CheckAtom checkAtom)
        {
            foreach (var atomi in reactant.Atoms)
            {
                if (checkReatantAtom(reactant, atomi))
                {
                    foreach (var bondi in reactant.GetConnectedBonds(atomi))
                    {
                        if (bondi.Order == BondOrder.Single)
                        {
                            IAtom atomj = bondi.GetConnectedAtom(atomi);
                            if ((atomj.FormalCharge ?? 0) == 0
                                    && !reactant.GetConnectedSingleElectrons(atomj).Any())
                            {
                                foreach (var bondj in reactant.GetConnectedBonds(atomj))
                                {
                                    if (bondj.Equals(bondi)) continue;

                                    if (bondj.Order == BondOrder.Double)
                                    {
                                        IAtom atomk = bondj.GetConnectedAtom(atomj);
                                        if (checkAtom(atomk)
											&& !reactant.GetConnectedSingleElectrons(atomk).Any())
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
