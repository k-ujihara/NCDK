/* Copyright (C) 1997-2007  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.Common.Mathematics;
using NCDK.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using NCDK.Numerics;
using System.Xml.Linq;

namespace NCDK.IO.CML
{
    /// <summary>
    /// Implements the PDB convention used by PDB2CML.
    /// 
    /// <para>This is a lousy implementation, though. Problems that will arise:
    /// <ul>
    ///   <li>when this new convention is adopted in the root element no
    ///     currentFrame was set. This is done when <list sequence=""> is found</li>
    ///   <li>multiple sequences are not yet supported</li>
    ///   <li>the frame is now added when the doc is ended, which will result in problems
    ///     but work for one sequence files made by PDB2CML v.??</li>
    /// <ul>
    /// </para>
    /// <para>What is does:
    /// <ul>
    ///   <li>work for now</li>
    ///   <li>give an idea on the API of the plugable CML import filter
    ///     (a real one will be made)</li>
    ///   <li>read CML files generated with Steve Zara's PDB 2 CML converter
    ///     (of which version 1999 produces invalid CML 1.0)</li>
    /// </ul>
    /// </para>
    /// </summary>
    // @cdk.module io
    // @cdk.githash
    // @author Egon Willighagen <egonw@sci.kun.nl>
    public class PDBConvention : CMLCoreModule
    {
        private bool connectionTable;
        private bool isELSYM;
        private bool isBond;
        private string connect_root;
        private bool hasScalar;
        private string idValue = "";
        private List<string> altLocV;
        private List<string> chainIDV;
        private List<string> hetAtomV;
        private List<string> iCodeV;
        private List<string> nameV;
        private List<string> oxtV;
        private List<string> recordV;
        private List<string> resNameV;
        private List<string> resSeqV;
        private List<string> segIDV;
        private List<string> serialV;
        private List<string> tempFactorV;

        public PDBConvention(IChemFile chemFile)
            : base(chemFile)
        { }

        public PDBConvention(ICMLModule conv)
            : base(conv)
        { }

        public override void EndDocument()
        {
            StoreData();
            base.EndDocument();
        }

        public override void StartElement(CMLStack xpath, XElement element)
        {
            string name = element.Name.LocalName;
            isELSYM = false;
            if ("molecule".Equals(name))
            {
                foreach (var attj in element.Attributes())
                {
                    Debug.WriteLine("StartElement");

                    BUILTIN = "";
                    DICTREF = "";

                    foreach (var atti in element.Attributes())
                    {
                        string qname = atti.Name.LocalName;
                        if (qname.Equals("builtin"))
                        {
                            BUILTIN = atti.Value;
                            Debug.WriteLine(name, "->BUILTIN found: ", atti.Value);
                        }
                        else if (qname.Equals("dictRef"))
                        {
                            DICTREF = atti.Value;
                            Debug.WriteLine(name, "->DICTREF found: ", atti.Value);
                        }
                        else if (qname.Equals("title"))
                        {
                            elementTitle = atti.Value;
                            Debug.WriteLine(name, "->TITLE found: ", atti.Value);
                        }
                        else
                        {
                            Debug.WriteLine("Qname: ", qname);
                        }
                    }
                    if (attj.Name.LocalName.Equals("convention") && attj.Value.Equals("PDB"))
                    {
                        //                    cdo.StartObject("PDBPolymer");
                        currentStrand = currentChemFile.Builder.CreateStrand();
                        currentStrand.StrandName = "A";
                        currentMolecule = currentChemFile.Builder.CreatePDBPolymer();
                    }
                    else if (attj.Name.LocalName.Equals("dictRef") && attj.Value.Equals("pdb:sequence"))
                    {

                        NewSequence();
                        BUILTIN = "";
                        foreach (var atti in element.Attributes())
                        {
                            if (atti.Name.LocalName.Equals("id"))
                            {
                                //                            cdo.SetObjectProperty("Molecule", "id", atts.GetValue(i));
                                currentMolecule.Id = atti.Value;
                            }
                            else if (atti.Name.LocalName.Equals("dictRef"))
                            {
                                //                        	cdo.SetObjectProperty("Molecule", "dictRef", atts.GetValue(i));
                                // FIXME: has no equivalent in ChemFileCDO
                            }
                        }

                    }
                    else if (attj.Name.LocalName.Equals("title") && attj.Value.Equals("connections"))
                    {
                        // assume that Atom's have been read
                        Debug.WriteLine("Assuming that Atom's have been read: storing them");
                        base.StoreAtomData();
                        connectionTable = true;
                        Debug.WriteLine("Start Connection Table");
                    }
                    else if (attj.Name.LocalName.Equals("title") && attj.Value.Equals("connect"))
                    {
                        Debug.WriteLine("New connection");
                        isBond = true;
                    }
                    else if (attj.Name.LocalName.Equals("id") && isBond)
                    {
                        connect_root = attj.Value;
                    }

                    // ignore other list items at this moment
                }
            }
            else if ("scalar".Equals(name))
            {
                hasScalar = true;
                foreach (var atti in element.Attributes())
                {
                    if (atti.Name.LocalName.Equals("dictRef")) idValue = atti.Value;
                }
            }
            else if ("label".Equals(name))
            {
                hasScalar = true;
                foreach (var atti in element.Attributes())
                {
                    if (atti.Name.LocalName.Equals("dictRef")) idValue = atti.Value;
                }
            }
            else if ("atom".Equals(name))
            {
                base.StartElement(xpath, element);
            }
        }

        public void NewSequence()
        {
            altLocV = new List<string>();
            chainIDV = new List<string>();
            hetAtomV = new List<string>();
            iCodeV = new List<string>();
            nameV = new List<string>();
            oxtV = new List<string>();
            recordV = new List<string>();
            resNameV = new List<string>();
            resSeqV = new List<string>();
            segIDV = new List<string>();
            serialV = new List<string>();
            tempFactorV = new List<string>();

        }

        public override void EndElement(CMLStack xpath, XElement element)
        {
            string name = element.Name.LocalName;

            // OLD +++++++++++++++++++++++++++++++++++++++++++++
            if (name.Equals("list") && connectionTable && !isBond)
            {
                Debug.WriteLine("End Connection Table");
                connectionTable = false;
                // OLD +++++++++++++++++++++++++++++++++++++++++++++

            }
            else if (name.Equals("molecule"))
            {
                StoreData();
                if (xpath.Count == 1)
                {
                    //	        	cdo.EndObject("Molecule");
                    if (currentMolecule is IAtomContainer)
                    {
                        Debug.WriteLine("Adding molecule to set");
                        currentMoleculeSet.Add(currentMolecule);
                        Debug.WriteLine("#mols in set: " + currentMoleculeSet.Count);
                    }
                    else if (currentMolecule is ICrystal)
                    {
                        Debug.WriteLine("Adding crystal to chemModel");
                        currentChemModel.Crystal = (ICrystal)currentMolecule;
                        currentChemSequence.Add(currentChemModel);
                    }
                }
            }
            isELSYM = false;
            isBond = false;

        }

        public override void CharacterData(CMLStack xpath, XElement element)
        {
            string s = element.Value.Trim();
            var st1tokens = s.Split(' ');
            string dictpdb = "";

            foreach (var st1token in st1tokens)
            {
                dictpdb += st1token;
                dictpdb += " ";
            }
            if (dictpdb.Length > 0 && !idValue.Equals("pdb:record"))
                dictpdb = dictpdb.Substring(0, dictpdb.Length - 1);

            if (idValue.Equals("pdb:altLoc"))
                altLocV.Add(dictpdb);
            else if (idValue.Equals("pdb:chainID"))
                chainIDV.Add(dictpdb);
            else if (idValue.Equals("pdb:hetAtom"))
                hetAtomV.Add(dictpdb);
            else if (idValue.Equals("pdb:iCode"))
                iCodeV.Add(dictpdb);
            else if (idValue.Equals("pdb:name"))
                nameV.Add(dictpdb);
            else if (idValue.Equals("pdb:oxt"))
                oxtV.Add(dictpdb);
            else if (idValue.Equals("pdb:record"))
                recordV.Add(dictpdb);
            else if (idValue.Equals("pdb:resName"))
                resNameV.Add(dictpdb);
            else if (idValue.Equals("pdb:resSeq"))
                resSeqV.Add(dictpdb);
            else if (idValue.Equals("pdb:segID"))
                segIDV.Add(dictpdb);
            else if (idValue.Equals("pdb:serial"))
                serialV.Add(dictpdb);
            else if (idValue.Equals("pdb:tempFactor")) tempFactorV.Add(dictpdb);

            idValue = "";

            if (isELSYM)
            {
                elsym.Add(s);
            }
            else if (isBond)
            {
                Debug.WriteLine("CD (bond): " + s);
                if (connect_root.Length > 0)
                {
                    var tokens = s.Split(' ');
                    foreach (var atom in tokens)
                    {
                        if (!atom.Equals("0"))
                        {
                            Debug.WriteLine("new bond: " + connect_root + "-" + atom);
                            //                        cdo.StartObject("Bond");
                            //                        int atom1 = int.Parse(connect_root) - 1;
                            //                        int atom2 = int.Parse(atom) - 1;
                            //                        cdo.SetObjectProperty("Bond", "atom1",
                            //                                (new Integer(atom1)).ToString());
                            //                        cdo.SetObjectProperty("Bond", "atom2",
                            //                                (new Integer(atom2)).ToString());
                            currentBond = currentMolecule.Builder.CreateBond(currentMolecule.Atoms[int.Parse(connect_root) - 1],
                                    currentMolecule.Atoms[int.Parse(atom) - 1], BondOrder.Single);
                            currentMolecule.Add(currentBond);
                        }
                    }
                }
            }
        }

        protected override void StoreData()
        {
            if (inchi != null)
            {
                //            cdo.SetObjectProperty("Molecule", "inchi", inchi);
                currentMolecule.SetProperty(CDKPropertyName.INCHI, inchi);
            }
            StoreAtomData();
            StoreBondData();
        }

        protected override void StoreAtomData()
        {
            Debug.WriteLine("No atoms: ", atomCounter);
            if (atomCounter == 0)
            {
                return;
            }

            bool hasID = false;
            bool has3D = false;
            bool has3Dfract = false;
            bool has2D = false;
            bool hasFormalCharge = false;
            bool hasPartialCharge = false;
            bool hasHCounts = false;
            bool hasSymbols = false;
            bool hasTitles = false;
            bool hasIsotopes = false;
            bool hasDictRefs = false;
            bool hasSpinMultiplicities = false;
            bool hasOccupancies = false;

            if (elid.Count == atomCounter)
            {
                hasID = true;
            }
            else
            {
                Debug.WriteLine("No atom ids: " + elid.Count, " != " + atomCounter);
            }

            if (elsym.Count == atomCounter)
            {
                hasSymbols = true;
            }
            else
            {
                Debug.WriteLine("No atom symbols: " + elsym.Count, " != " + atomCounter);
            }

            if (eltitles.Count == atomCounter)
            {
                hasTitles = true;
            }
            else
            {
                Debug.WriteLine("No atom titles: " + eltitles.Count, " != " + atomCounter);
            }

            if ((x3.Count == atomCounter) && (y3.Count == atomCounter) && (z3.Count == atomCounter))
            {
                has3D = true;
            }
            else
            {
                Debug.WriteLine("No 3D info: " + x3.Count, " " + y3.Count, " " + z3.Count, " != " + atomCounter);
            }

            if ((xfract.Count == atomCounter) && (yfract.Count == atomCounter) && (zfract.Count == atomCounter))
            {
                has3Dfract = true;
            }
            else
            {
                Debug.WriteLine("No 3D fractional info: " + xfract.Count, " " + yfract.Count, " " + zfract.Count, " != "
                        + atomCounter);
            }

            if ((x2.Count == atomCounter) && (y2.Count == atomCounter))
            {
                has2D = true;
            }
            else
            {
                Debug.WriteLine("No 2D info: " + x2.Count, " " + y2.Count, " != " + atomCounter);
            }

            if (formalCharges.Count == atomCounter)
            {
                hasFormalCharge = true;
            }
            else
            {
                Debug.WriteLine("No formal Charge info: " + formalCharges.Count, " != " + atomCounter);
            }

            if (partialCharges.Count == atomCounter)
            {
                hasPartialCharge = true;
            }
            else
            {
                Debug.WriteLine("No partial Charge info: " + partialCharges.Count, " != " + atomCounter);
            }

            if (hCounts.Count == atomCounter)
            {
                hasHCounts = true;
            }
            else
            {
                Debug.WriteLine("No hydrogen Count info: " + hCounts.Count, " != " + atomCounter);
            }

            if (spinMultiplicities.Count == atomCounter)
            {
                hasSpinMultiplicities = true;
            }
            else
            {
                Debug.WriteLine("No spinMultiplicity info: " + spinMultiplicities.Count, " != " + atomCounter);
            }

            if (occupancies.Count == atomCounter)
            {
                hasOccupancies = true;
            }
            else
            {
                Debug.WriteLine("No occupancy info: " + occupancies.Count, " != " + atomCounter);
            }

            if (atomDictRefs.Count == atomCounter)
            {
                hasDictRefs = true;
            }
            else
            {
                Debug.WriteLine("No dictRef info: " + atomDictRefs.Count, " != " + atomCounter);
            }

            if (isotope.Count == atomCounter)
            {
                hasIsotopes = true;
            }
            else
            {
                Debug.WriteLine("No isotope info: " + isotope.Count, " != " + atomCounter);
            }
            if (atomCounter > 0)
            {
                //        	cdo.StartObject("PDBMonomer");
                currentMonomer = currentChemFile.Builder.CreatePDBMonomer();
            }

            for (int i = 0; i < atomCounter; i++)
            {
                Trace.TraceInformation("Storing atom: ", i);
                //            cdo.StartObject("PDBAtom");
                currentAtom = currentChemFile.Builder.CreatePDBAtom("H");
                if (hasID)
                {
                    //                cdo.SetObjectProperty("Atom", "id", (string)elid[i]);
                    currentAtom.Id = (string)elid[i];
                }
                if (hasTitles)
                {
                    if (hasSymbols)
                    {
                        string symbol = (string)elsym[i];
                        if (symbol.Equals("Du") || symbol.Equals("Dummy"))
                        {
                            //                        cdo.SetObjectProperty("PseudoAtom", "label", (string)eltitles[i]);
                            if (!(currentAtom is IPseudoAtom))
                            {
                                currentAtom = currentChemFile.Builder.CreatePseudoAtom(currentAtom);
                            }
                            ((IPseudoAtom)currentAtom).Label = (string)eltitles[i];
                        }
                        else
                        {
                            //                        cdo.SetObjectProperty("Atom", "title", (string)eltitles[i]);
                            // FIXME: is a guess, Atom.title is not found in ChemFileCDO
                            currentAtom.SetProperty(CDKPropertyName.TITLE, (string)eltitles[i]);
                        }
                    }
                    else
                    {
                        //                    cdo.SetObjectProperty("Atom", "title", (string)eltitles[i]);
                        //               	 FIXME: is a guess, Atom.title is not found in ChemFileCDO
                        currentAtom.SetProperty(CDKPropertyName.TITLE, (string)eltitles[i]);
                    }
                }

                // store optional atom properties
                if (hasSymbols)
                {
                    string symbol = (string)elsym[i];
                    if (symbol.Equals("Du") || symbol.Equals("Dummy"))
                    {
                        symbol = "R";
                    }
                    //                cdo.SetObjectProperty("Atom", "type", symbol);
                    if (symbol.Equals("R") && !(currentAtom is IPseudoAtom))
                    {
                        currentAtom = currentChemFile.Builder.CreatePseudoAtom(currentAtom);
                    }
                    currentAtom.Symbol = symbol;
                    try
                    {
                        Isotopes.Instance.Configure(currentAtom);
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError("Could not configure atom: " + currentAtom);
                        Debug.WriteLine(e);
                    }
                }

                if (has3D)
                {
                    //                cdo.SetObjectProperty("Atom", "x3", (string)x3[i]);
                    //                cdo.SetObjectProperty("Atom", "y3", (string)y3[i]);
                    //                cdo.SetObjectProperty("Atom", "z3", (string)z3[i]);
                    currentAtom.Point3D = new Vector3(
                        double.Parse((string)x3[i]),
                        double.Parse((string)y3[i]),
                        double.Parse((string)z3[i]));
                }

                if (has3Dfract)
                {
                    // ok, need to convert fractional into eucledian coordinates
                    //                cdo.SetObjectProperty("Atom", "xFract", (string)xfract[i]);
                    //                cdo.SetObjectProperty("Atom", "yFract", (string)yfract[i]);
                    //                cdo.SetObjectProperty("Atom", "zFract", (string)zfract[i]);
                    currentAtom.FractionalPoint3D = new Vector3(
                        double.Parse((string)xfract[i]),
                        double.Parse((string)yfract[i]),
                        double.Parse((string)zfract[i]));
                }

                if (hasFormalCharge)
                {
                    //              cdo.SetObjectProperty("Atom", "formalCharge",
                    //                                    (string)formalCharges[i]);
                    currentAtom.FormalCharge = int.Parse((string)formalCharges[i]);
                }

                if (hasPartialCharge)
                {
                    Debug.WriteLine("Storing partial atomic charge...");
                    //          	cdo.SetObjectProperty("Atom", "partialCharge",
                    //          	(string)partialCharges[i]);
                    currentAtom.Charge = double.Parse((string)partialCharges[i]);
                }

                if (hasHCounts)
                {
                    //          	cdo.SetObjectProperty("Atom", "hydrogenCount", (string)hCounts[i]);
                    // FIXME: the hCount in CML is the total of implicit *and* explicit
                    currentAtom.ImplicitHydrogenCount = int.Parse((string)hCounts[i]);
                }

                if (has2D)
                {
                    if (x2[i] != null && y2[i] != null)
                    {
                        //                    cdo.SetObjectProperty("Atom", "x2", (string)x2[i]);
                        //                    cdo.SetObjectProperty("Atom", "y2", (string)y2[i]);
                        currentAtom.Point2D = new Vector2(
                            double.Parse((string)x2[i]),
                            double.Parse((string)y2[i]));
                    }
                }

                if (hasDictRefs)
                {
                    //                cdo.SetObjectProperty("Atom", "dictRef", (string)atomDictRefs[i]);
                    currentAtom.SetProperty("org.openscience.cdk.dict", (string)atomDictRefs[i]);
                }

                if (hasSpinMultiplicities && spinMultiplicities[i] != null)
                {
                    //                cdo.SetObjectProperty("Atom", "spinMultiplicity", (string)spinMultiplicities[i]);
                    int unpairedElectrons = int.Parse((string)spinMultiplicities[i]) - 1;
                    for (int sm = 0; sm < unpairedElectrons; sm++)
                    {
                        currentMolecule.Add(currentChemFile.Builder.CreateSingleElectron(currentAtom));
                    }
                }

                if (hasOccupancies && occupancies[i] != null)
                {
                    //                cdo.SetObjectProperty("PDBAtom", "occupancy", (string)occupancies[i]);
                    double occ = double.Parse((string)occupancies[i]);
                    if (occ >= 0.0) ((IPDBAtom)currentAtom).Occupancy = occ;
                }

                if (hasIsotopes)
                {
                    //              cdo.SetObjectProperty("Atom", "massNumber", (string)isotope[i]);
                    currentAtom.MassNumber = int.Parse((string)isotope[i]);
                }

                if (hasScalar)
                {
                    IPDBAtom pdbAtom = (IPDBAtom)currentAtom;
                    //                cdo.SetObjectProperty("PDBAtom", "altLoc", altLocV[i].ToString());
                    if (altLocV.Count > 0) pdbAtom.AltLoc = altLocV[i].ToString();
                    //                cdo.SetObjectProperty("PDBAtom", "chainID", chainIDV[i].ToString());
                    if (chainIDV.Count > 0) pdbAtom.ChainID = chainIDV[i].ToString();
                    //                cdo.SetObjectProperty("PDBAtom", "hetAtom", hetAtomV[i].ToString());
                    if (hetAtomV.Count > 0) pdbAtom.HetAtom = hetAtomV[i].ToString().Equals("true");
                    //                cdo.SetObjectProperty("PDBAtom", "iCode", iCodeV[i].ToString());
                    if (iCodeV.Count > 0) pdbAtom.ICode = iCodeV[i].ToString();
                    //                cdo.SetObjectProperty("PDBAtom", "name", nameV[i].ToString());
                    if (nameV.Count > 0) pdbAtom.Name = nameV[i].ToString();
                    //                cdo.SetObjectProperty("PDBAtom", "oxt", oxtV[i].ToString());
                    if (oxtV.Count > 0) pdbAtom.Oxt = oxtV[i].ToString().Equals("true");
                    //                cdo.SetObjectProperty("PDBAtom", "resSeq", resSeqV[i].ToString());
                    if (resSeqV.Count > 0) pdbAtom.ResSeq = resSeqV[i].ToString();
                    //                cdo.SetObjectProperty("PDBAtom", "record", recordV[i].ToString());
                    if (recordV.Count > 0) pdbAtom.Record = recordV[i].ToString();
                    //                cdo.SetObjectProperty("PDBAtom", "resName", resNameV[i].ToString());
                    if (resNameV.Count > 0) pdbAtom.ResName = resNameV[i].ToString();
                    //                cdo.SetObjectProperty("PDBAtom", "segID", segIDV[i].ToString());
                    if (segIDV.Count > 0) pdbAtom.SegID = segIDV[i].ToString();
                    //                cdo.SetObjectProperty("PDBAtom", "serial", serialV[i].ToString());
                    if (serialV.Count > 0) pdbAtom.Serial = int.Parse(serialV[i].ToString());
                    //                cdo.SetObjectProperty("PDBAtom", "tempFactor", tempFactorV[i].ToString());
                    if (tempFactorV.Count > 0) pdbAtom.TempFactor = double.Parse(tempFactorV[i].ToString());
                }

                //            cdo.EndObject("PDBAtom");
                string cResidue = ((IPDBAtom)currentAtom).ResName + "A" + ((IPDBAtom)currentAtom).ResSeq;
                ((IPDBMonomer)currentMonomer).MonomerName = cResidue;
                ((IPDBMonomer)currentMonomer).MonomerType = ((IPDBAtom)currentAtom).ResName;
                ((IPDBMonomer)currentMonomer).ChainID = ((IPDBAtom)currentAtom).ChainID;
                ((IPDBMonomer)currentMonomer).ICode = ((IPDBAtom)currentAtom).ICode;
                ((IPDBPolymer)currentMolecule).AddAtom(((IPDBAtom)currentAtom), currentMonomer, currentStrand);
            }
            //		cdo.EndObject("PDBMonomer");
            // nothing done in the CDO for this event
            if (elid.Count > 0)
            {
                // assume this is the current working list
                bondElid = elid;
            }
            NewAtomData();
        }
    }
}
