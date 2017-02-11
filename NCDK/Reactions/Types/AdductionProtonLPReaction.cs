/* Copyright (C) 2008  Miguel Rojas <miguelrojasch@users.sf.net>
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
using NCDK.Tools.Manipulator;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NCDK.Reactions.Types
{
    /**
     * <p>IReactionProcess which produces a protonation.
     * As most commonly encountered, this reaction results in the formal migration
     * of a hydrogen atom or proton, accompanied by a switch of a single bond and adjacent double bond</p>
     *
     * <pre>[X-] + [H+] => X -H</pre>
     * <pre>|X + [H+]   => [X+]-H</pre>
     *
     * <p>Below you have an example how to initiate the mechanism.</p>
     * <p>It is processed by the AdductionLPMechanism class</p>
     * <pre>
     *  IAtomContainerSet setOfReactants = Default.ChemObjectBuilder.Instance.NewAtomContainerSet();
     *  setOfReactants.Add(new AtomContainer());
     *  IReactionProcess type = new AdductionProtonLPReaction();
     *  object[] parameters = {bool.FALSE};
        type.Parameters = parameters;
     *  IReactionSet setOfReactions = type.Initiate(setOfReactants, null);
     *  </pre>
     *
     * <p>We have the possibility to localize the reactive center. Good method if you
     * want to specify the reaction in a fixed point.</p>
     * <pre>atoms[0].SetFlag(CDKConstants.REACTIVE_CENTER,true);</pre>
     * <p>Moreover you must put the parameter true</p>
     * <p>If the reactive center is not specified then the reaction process will
     * try to find automatically the possible reaction centers.</p>
     *
     *
     * @author         Miguel Rojas
     *
     * @cdk.created    2008-02-11
     * @cdk.module     reaction
     * @cdk.set        reaction-types
     * @cdk.githash
     *
     * @see AdductionLPMechanism
     **/
    public class AdductionProtonLPReaction : ReactionEngine, IReactionProcess
    {
        /**
         * Constructor of the AdductionProtonLPReaction object.
         *
         */
        public AdductionProtonLPReaction() { }

        /**
         *  Gets the specification attribute of the AdductionProtonLPReaction object.
         *
         *@return    The specification value
         */

        public ReactionSpecification Specification =>
            new ReactionSpecification(
                    "http://almost.cubic.uni-koeln.de/jrg/Members/mrc/reactionDict/reactionDict#AdductionProtonLP",
                    this.GetType().Name, "$Id$", "The Chemistry Development Kit");

        /**
         *  Initiate process.
         *  It is needed to call the addExplicitHydrogensToSatisfyValency
         *  from the class tools.HydrogenAdder.
         *
         *
         *@exception  CDKException  Description of the Exception

         * @param  reactants         reactants of the reaction
        * @param  agents            agents of the reaction (Must be in this case null)
         */

        public IReactionSet Initiate(IAtomContainerSet<IAtomContainer> reactants, IAtomContainerSet<IAtomContainer> agents)
        {

            Debug.WriteLine("initiate reaction: AdductionProtonLPReaction");

            if (reactants.Count != 1)
            {
                throw new CDKException("AdductionProtonLPReaction only expects one reactant");
            }
            if (agents != null)
            {
                throw new CDKException("AdductionProtonLPReaction don't expects agents");
            }

            IReactionSet setOfReactions = reactants.Builder.CreateReactionSet();
            IAtomContainer reactant = reactants[0];

            IParameterReact ipr = base.GetParameterClass(typeof(SetReactionCenter));
            if (ipr != null && !ipr.IsSetParameter) SetActiveCenters(reactant);

            if (AtomContainerManipulator.GetTotalCharge(reactant) > 0) return setOfReactions;

            foreach (var atomi in reactant.Atoms)
            {
                if (atomi.IsReactiveCenter
   && (atomi.FormalCharge ?? 0) <= 0
   && reactant.GetConnectedLonePairs(atomi).Any()
   && !reactant.GetConnectedSingleElectrons(atomi).Any())
                {

                    var atomList = new List<IAtom>();
                    atomList.Add(atomi);
                    IAtom atomH = reactant.Builder.CreateAtom("H");
                    atomH.FormalCharge = 1;
                    atomList.Add(atomH);

                    var moleculeSet = reactant.Builder.CreateAtomContainerSet();
                    moleculeSet.Add(reactant);
                    IAtomContainer adduct = reactant.Builder.CreateAtomContainer();
                    adduct.Atoms.Add(atomH);
                    moleculeSet.Add(adduct);

                    IReaction reaction = Mechanism.Initiate(moleculeSet, atomList, null);
                    if (reaction == null)
                        continue;
                    else
                        setOfReactions.Add(reaction);

                }
            }

            return setOfReactions;
        }

        /**
         * set the active center for this molecule.
         * The active center will be those which correspond with X=Y-Z-H.
         * <pre>
         * [X-]
         *  </pre>
         *
         * @param reactant The molecule to set the activity
         * @
         */
        private void SetActiveCenters(IAtomContainer reactant)
        {
            if (AtomContainerManipulator.GetTotalCharge(reactant) > 0) return;

            foreach (var atomi in reactant.Atoms)
            {
                if ((atomi.FormalCharge ?? 0) <= 0
                        && reactant.GetConnectedLonePairs(atomi).Any()
                        && !reactant.GetConnectedSingleElectrons(atomi).Any())
                {
                    atomi.IsReactiveCenter = true;
                }
            }
        }
    }
}
