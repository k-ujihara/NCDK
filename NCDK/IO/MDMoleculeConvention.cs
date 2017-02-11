/* Copyright (C) 2007  Ola Spjuth <ola.spjuth@farmbio.uu.se>
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
using NCDK.LibIO.CML;
using NCDK.LibIO.MD;
using System;
using System.Diagnostics;
using System.Xml.Linq;
using static NCDK.LibIO.CML.CMLElement;

namespace NCDK.IO.CML
{
    /// <summary>
    /// Implements a Convention for parsing an MDMolecule from CML.
    /// </summary>
    // @cdk.module libiomd
    // @cdk.githash
    // @author Ola Spjuth <ola.spjuth@farmbio.uu.se>
    public class MDMoleculeConvention : CMLCoreModule
    {
        private Residue currentResidue;
        private ChargeGroup currentChargeGroup;

        public MDMoleculeConvention(IChemFile chemFile)
                : base(chemFile)
        { }

        public MDMoleculeConvention(ICMLModule conv)
                : base(conv)
        { }
        
        /// <summary>
        /// Add parsing of elements in mdmolecule:
        /// <![CDATA[
        /// mdmolecule
        ///     chargeGroup
        ///         id
        ///         cgNumber
        ///         atomArray
        ///         switchingAtom
        ///     residue
        /// 			id
        /// 			title
        /// 			resNumber
        /// 			atomArray
        /// ]]>
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="element"></param>
        // @cdk.todo The JavaDoc of this class needs to be converted into HTML
        public override void StartElement(CMLStack xpath, XElement element)
        {
            // <molecule convention="md:mdMolecule"
            //	          xmlns="http://www.xml-cml.org/schema"
            //	          xmlns:md="http://www.bioclipse.org/mdmolecule">
            //	  <atomArray>
            //	    <atom id="a1" elementType="C"/>
            //	    <atom id="a2" elementType="C"/>
            //	  </atomArray>
            //	  <molecule dictRef="md:chargeGroup" id="cg1">
            //	    <scalar dictRef="md:cgNumber">5</scalar>
            //	    <atomArray>
            //	      <atom ref="a1"/>
            //	      <atom ref="a2"><scalar dictRef="md:switchingAtom"/></atom>
            //	    </atomArray>
            //	  </molecule>
            //	  <molecule dictRef="md:residue" id="r1" title="resName">
            //	    <scalar dictRef="md:resNumber">3</scalar>
            //	    <atomArray>
            //	      <atom ref="a1"/>
            //	      <atom ref="a2"/>
            //	    </atomArray>
            //	  </molecule>
            //	</molecule>

            // let the CMLCore convention deal with things first

            if (element.Name == XName_CML_molecule)
            {
                // the copy the parsed content into a new MDMolecule
                if (element.Attribute(Attribute_convention) != null && element.Attribute(Attribute_convention).Value.Equals("md:mdMolecule"))
                {
                    //				Console.Out.WriteLine("creating a MDMolecule");
                    base.StartElement(xpath, element);
                    currentMolecule = new MDMolecule(currentMolecule);
                }
                else
                {
                    DICTREF = element.Attribute(Attribute_dictRef) != null ? element.Attribute(Attribute_dictRef).Value : "";
                    //If residue or chargeGroup, set up a new one
                    if (DICTREF.Equals("md:chargeGroup"))
                    {
                        //					Console.Out.WriteLine("Creating a new charge group...");
                        currentChargeGroup = new ChargeGroup();
                    }
                    else if (DICTREF.Equals("md:residue"))
                    {
                        //					Console.Out.WriteLine("Creating a new residue group...");
                        currentResidue = new Residue();
                        if (element.Attribute(Attribute_title) != null) currentResidue.Name = element.Attribute(Attribute_title).Value;
                    }
                }
            }
            else
            //We have a scalar element. Now check who it belongs to
            if (element.Name == XName_CML_scalar)
            {
                DICTREF = element.Attribute(Attribute_dictRef).Value;
                //Switching Atom
                if ("md:switchingAtom".Equals(DICTREF))
                {
                    //Set current atom as switching atom
                    Console.Out.WriteLine("Adding Switching atom: " + currentAtom);
                    currentChargeGroup.SetSwitchingAtom(currentAtom);
                }
                else
                {
                    base.StartElement(xpath, element);
                }
            }
            else if (element.Name == XName_CML_atom)
            {
                if (currentChargeGroup != null)
                {
                    string id = element.Attribute(Attribute_ref).Value;
                    if (id != null)
                    {
                        // ok, an atom is referenced; look it up
                        currentAtom = null;
                        //					Console.Out.WriteLine("#atoms: " + currentMolecule.Atoms.Count);
                        foreach (var nextAtom in currentMolecule.Atoms)
                        {
                            if (nextAtom.Id.Equals(id))
                            {
                                currentAtom = nextAtom;
                            }
                        }
                        if (currentAtom == null)
                        {
                            Trace.TraceError($"Could not found the referenced atom '{id}' for this charge group!");
                        }
                        else
                        {
                            currentChargeGroup.Atoms.Add(currentAtom);
                        }
                    }
                }
                else if (currentResidue != null)
                {
                    string id = element.Attribute(Attribute_ref).Value;
                    if (id != null)
                    {
                        // ok, an atom is referenced; look it up
                        IAtom referencedAtom = null;
                        //					Console.Out.WriteLine("#atoms: " + currentMolecule.Atoms.Count);
                        foreach (var nextAtom in currentMolecule.Atoms)
                        {
                            if (nextAtom.Id.Equals(id))
                            {
                                referencedAtom = nextAtom;
                            }
                        }
                        if (referencedAtom == null)
                        {
                            Trace.TraceError($"Could not found the referenced atom '{id}' for this residue!");
                        }
                        else
                        {
                            currentResidue.Atoms.Add(referencedAtom);
                        }
                    }
                }
                else
                {
                    // ok, fine, just add it to the currentMolecule
                    base.StartElement(xpath, element);
                }
            }
            else
            {
                base.StartElement(xpath, element);
            }
        }

        /// <summary>
        /// Finish up parsing of elements in mdmolecule.
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="element"></param>
        public override void EndElement(CMLStack xpath, XElement element)
        {
            if (element.Name.Equals(XName_CML_molecule))
            {
                //			Console.Out.WriteLine("Ending element mdmolecule");
                // add chargeGroup, and then delete them
                if (currentChargeGroup != null)
                {
                    if (currentMolecule is MDMolecule)
                    {
                        ((MDMolecule)currentMolecule).AddChargeGroup(currentChargeGroup);
                    }
                    else
                    {
                        Trace.TraceError("Need to store a charge group, but the current molecule is not a MDMolecule!");
                    }
                    currentChargeGroup = null;
                }
                else

                // add chargeGroup, and then delete them
                if (currentResidue != null)
                {
                    if (currentMolecule is MDMolecule)
                    {
                        ((MDMolecule)currentMolecule).AddResidue(currentResidue);
                    }
                    else
                    {
                        Trace.TraceError("Need to store a residue group, but the current molecule is not a MDMolecule!");
                    }
                    currentResidue = null;
                }
                else
                {
                    //				Console.Out.WriteLine("OK, that was the last end mdmolecule");
                    base.EndElement(xpath, element);
                }
            }
            else if (element.Name == XName_CML_atomArray)
            {
                if (xpath.Count == 2 && xpath.EndsWith("molecule", "atomArray"))
                {
                    StoreAtomData();
                    NewAtomData();
                }
                else if (xpath.Count > 2 && xpath.EndsWith("cml", "molecule", "atomArray"))
                {
                    StoreAtomData();
                    NewAtomData();
                }
            }
            else if (element.Name == XName_CML_bondArray)
            {
                if (xpath.Count == 2 && xpath.EndsWith("molecule", "bondArray"))
                {
                    StoreBondData();
                    NewBondData();
                }
                else if (xpath.Count > 2 && xpath.EndsWith("cml", "molecule", "bondArray"))
                {
                    StoreBondData();
                    NewBondData();
                }
            }
            else if (element.Name == XName_CML_scalar)
            {
                //Residue number
                if ("md:resNumber".Equals(DICTREF))
                {
                    int myInt = int.Parse(element.Value);
                    currentResidue.SetNumber(myInt);
                }
                //ChargeGroup number
                else if ("md:cgNumber".Equals(DICTREF))
                {
                    int myInt = int.Parse(element.Value);
                    currentChargeGroup.SetNumber(myInt);
                }
            }
            else
            {
                base.EndElement(xpath, element);
            }
        }
    }
}
