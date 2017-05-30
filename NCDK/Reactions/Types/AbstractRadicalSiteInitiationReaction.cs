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
    // @author         Miguel Rojas, Kazuya Ujihara
    // @cdk.created    2006-05-05
    // @cdk.module     reaction
    // @cdk.githash
    public abstract class AbstractRadicalSiteInitiationReaction : ReactionEngine, IReactionProcess
    {
        public AbstractRadicalSiteInitiationReaction() { }

        /// <inheritdoc/>
        public abstract ReactionSpecification Specification { get; }

        /// <inheritdoc/>
        public abstract IReactionSet Initiate(IAtomContainerSet<IAtomContainer> reactants, IAtomContainerSet<IAtomContainer> agents);
        
        internal IReactionSet Initiate(IAtomContainerSet<IAtomContainer> reactants, IAtomContainerSet<IAtomContainer> agents, string atomSymbol, int charge)
        {
            CheckInitiateParams(reactants, agents);

            IReactionSet setOfReactions = reactants.Builder.CreateReactionSet();
            IAtomContainer reactant = reactants[0];

            // if the parameter hasActiveCenter is not fixed yet, set the active centers
            IParameterReaction ipr = base.GetParameterClass(typeof(SetReactionCenter));
            if (ipr != null && !ipr.IsSetParameter) SetActiveCenters(reactant, atomSymbol, charge);

            foreach (var atomi in reactants[0].Atoms)
            {
                if (atomi.IsReactiveCenter && reactant.GetConnectedSingleElectrons(atomi).Count() == 1
                        && atomi.FormalCharge == charge)
                {
                    foreach (var bondi in reactant.GetConnectedBonds(atomi))
                    {
                        if (bondi.IsReactiveCenter && bondi.Order == BondOrder.Single)
                        {
                            IAtom atomj = bondi.GetOther(atomi);
                            if (atomj.IsReactiveCenter && atomj.FormalCharge == 0)
                            {
                                foreach (var bondj in reactant.GetConnectedBonds(atomj))
                                {
                                    if (bondj.Equals(bondi)) continue;

                                    if (bondj.IsReactiveCenter
                                            && bondj.Order == BondOrder.Single)
                                    {
                                        IAtom atomk = bondj.GetOther(atomj);
                                        if (atomk.IsReactiveCenter && atomk.Symbol.Equals(atomSymbol)
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
        
        private void SetActiveCenters(IAtomContainer reactant, string atomSymbol, int charge)
        {
            foreach (var atomi in reactant.Atoms)
            {
                if (reactant.GetConnectedSingleElectrons(atomi).Count() == 1 && atomi.FormalCharge == charge)
                {
                    foreach (var bondi in reactant.GetConnectedBonds(atomi))
                    {
                        if (bondi.Order == BondOrder.Single)
                        {
                            IAtom atomj = bondi.GetOther(atomi);
                            if (atomj.FormalCharge == 0)
                            {
                                foreach (var bondj in reactant.GetConnectedBonds(atomj))
                                {
                                    if (bondj.Equals(bondi)) continue;

                                    if (bondj.Order == BondOrder.Single)
                                    {
                                        IAtom atomk = bondj.GetOther(atomj);
                                        if (atomk.Symbol.Equals(atomSymbol) && atomk.FormalCharge == 0)
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
