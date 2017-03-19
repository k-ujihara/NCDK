/* Copyright (C) 2003-2007  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 */
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;

namespace NCDK.IO.CML
{
    // @author Egon Willighagen <elw38@cam.ac.uk>
    // @cdk.module io
    // @cdk.githash
    public class CMLReactionModule : CMLCoreModule
    {
#if DEBUG
        internal string objectType;
#endif

        public CMLReactionModule(IChemFile chemFile)
            : base(chemFile)
        { }

        public CMLReactionModule(ICMLModule conv)
            : base(conv)
        {
            Debug.WriteLine("New CML-Reaction Module!");
        }

        public override void StartElement(CMLStack xpath, XElement element)
        {
            if ("reaction".Equals(element.Name.LocalName))
            {
                //            cdo.StartObject("Reaction");
                if (currentReactionSet == null)
                    currentReactionSet = currentChemFile.Builder.CreateReactionSet();
                currentReaction = currentChemFile.Builder.CreateReaction();
                string id = AttGetValue(element.Attributes(), "id");
                if (id != null) currentReaction.Id = id;
                //                cdo.SetObjectProperty("Reaction", "id", id);
            }
            else if ("reactionList".Equals(element.Name.LocalName))
            {
                //            cdo.StartObject("ReactionSet");
                currentReactionSet = currentChemFile.Builder.CreateReactionSet();
                string id = AttGetValue(element.Attributes(), "id");
                if (id != null) currentReactionSet.Id = id;
                //                cdo.SetObjectProperty("reactionList", "id", id);
            }
            else if ("reactant".Equals(element.Name.LocalName))
            {
                //            cdo.StartObject("Reactant");
                if (currentReaction == null)
                {
                    if (currentReactionSet == null)
                        currentReactionSet = currentChemFile.Builder.CreateReactionSet();
                    currentReaction = currentChemFile.Builder.CreateReaction();
                }
                currentMolecule = currentChemFile.Builder.CreateAtomContainer();
#if DEBUG
                objectType = "Reactant";
#endif
                string id = AttGetValue(element.Attributes(), "id");
                if (id != null)
                    currentMolecule.Id = id;
                else
                {
                    string ref_ = AttGetValue(element.Attributes(), "ref");
                    if (ref_ != null) currentMolecule.Id = ref_;
                }
                //                cdo.SetObjectProperty("Reactant", "id", id);
            }
            else if ("product".Equals(element.Name.LocalName))
            {
                //            cdo.StartObject("Product");
                if (currentReaction == null)
                {
                    if (currentReactionSet == null)
                        currentReactionSet = currentChemFile.Builder.CreateReactionSet();
                    currentReaction = currentChemFile.Builder.CreateReaction();
                }
                currentMolecule = currentChemFile.Builder.CreateAtomContainer();
#if DEBUG
                objectType = "Product";
#endif
                string id = AttGetValue(element.Attributes(), "id");
                if (id != null)
                    currentMolecule.Id = id;
                else
                {
                    string ref_ = AttGetValue(element.Attributes(), "ref");
                    if (ref_ != null) currentMolecule.Id = ref_;
                }
                //                cdo.SetObjectProperty("Product", "id", id);
            }
            else if ("substance".Equals(element.Name.LocalName))
            {
                //            cdo.StartObject("Agent");
                if (currentReaction == null)
                {
                    if (currentReactionSet == null)
                        currentReactionSet = currentChemFile.Builder.CreateReactionSet();
                    currentReaction = currentChemFile.Builder.CreateReaction();
                }
                currentMolecule = currentChemFile.Builder.CreateAtomContainer();
#if DEBUG
                objectType = "Agent";
#endif
                string id = AttGetValue(element.Attributes(), "id");
                if (id != null)
                    currentMolecule.Id = id;
                else
                {
                    string ref_ = AttGetValue(element.Attributes(), "ref");
                    if (ref_ != null) currentMolecule.Id = ref_;
                }
                //                cdo.SetObjectProperty("Agent", "id", id);
            }
            else if ("molecule".Equals(element.Name.LocalName))
            {
                // clear existing molecule data
                base.NewMolecule();
                string id = AttGetValue(element.Attributes(), "id");
                if (id != null)
                {
                    // check for existing molecule of that id
                    IAtomContainer existing = GetMoleculeFromID(currentMoleculeSet, id);
                    if (existing != null)
                    {
                        currentMolecule = existing;
                    }
                    else
                    {
                        currentMolecule.Id = id;
                    }
                }
                else
                {
                    string ref_ = AttGetValue(element.Attributes(), "ref");
                    if (ref_ != null)
                    {
                        IAtomContainer atomC = GetMoleculeFromID(currentMoleculeSet, ref_);

                        // if there was no molecule create a new one for the reference. this
                        // happens when the reaction is defined before the molecule set
                        if (atomC == null)
                        {
                            atomC = currentChemFile.Builder.CreateAtomContainer();
                            atomC.Id = ref_;
                            currentMoleculeSet.Add(atomC);
                        }

                        base.currentMolecule = atomC;
                    }
                }
            }
            else
            {
                base.StartElement(xpath, element);
            }
        }

        public override void EndElement(CMLStack xpath, XElement element)
        {
            var local = element.Name.LocalName;

            if ("reaction".Equals(local))
            {
                //            cdo.EndObject("Reaction");
                currentReactionSet.Add(currentReaction);
                currentChemModel.ReactionSet = currentReactionSet;
            }
            else if ("reactionList".Equals(local))
            {
                //            cdo.EndObject("ReactionSet");
                currentChemModel.ReactionSet = currentReactionSet;
                /* FIXME: this should be when document is closed! */
            }
            else if ("reactant".Equals(local))
            {
                //            cdo.EndObject("Reactant");
                currentReaction.Reactants.Add(currentMolecule);
            }
            else if ("product".Equals(local))
            {
                //            cdo.EndObject("Product");
                currentReaction.Products.Add(currentMolecule);
            }
            else if ("substance".Equals(local))
            {
                //            cdo.EndObject("Agent");
                currentReaction.Agents.Add(currentMolecule);
            }
            else if ("molecule".Equals(local))
            {
                Debug.WriteLine("Storing Molecule");
                //if the current molecule exists in the currentMoleculeSet means that is a reference in these.
                if (currentMoleculeSet.GetMultiplier(currentMolecule) == -1) base.StoreData();
                // do nothing else but store atom/bond information
            }
            else
            {
                base.EndElement(xpath, element);
            }
        }

        /// <summary>
        /// Get the IAtomContainer contained in a IAtomContainerSet object with a ID.
        /// </summary>
        /// <param name="molSet">The IAtomContainerSet</param>
        /// <param name="id">The ID the look</param>
        /// <returns>The IAtomContainer with the ID</returns>
        private IAtomContainer GetMoleculeFromID(IAtomContainerSet<IAtomContainer> molSet, string id)
        {
            foreach (var mol in molSet)
            {
                if (mol.Id.Equals(id)) return mol;
            }
            return null;
        }
    }
}
