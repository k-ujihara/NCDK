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
    /// </summary>
    /// <remarks>
    /// <para>This is a lousy implementation, though. Problems that will arise:
    /// <list type="bullet">
    ///   <item>when this new convention is adopted in the root element no
    ///     currentFrame was set. This is done when &lt;list sequence=""&gt; is found</item>
    ///   <item>multiple sequences are not yet supported</item>
    ///   <item>the frame is now added when the doc is ended, which will result in problems
    ///     but work for one sequence files made by PDB2CML v.??</item>
    /// </list>
    /// </para>
    /// <para>What is does:
    /// <list type="bullet">
    ///   <item>work for now</item>
    ///   <item>give an idea on the API of the plugable CML import filter
    ///     (a real one will be made)</item>
    ///   <item>read CML files generated with Steve Zara's PDB 2 CML converter
    ///     (of which version 1999 produces invalid CML 1.0)</item>
    /// </list>
    /// </para>
    /// </remarks>
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
                            ElementTitle = atti.Value;
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
                        CurrentStrand = CurrentChemFile.Builder.NewStrand();
                        CurrentStrand.StrandName = "A";
                        CurrentMolecule = CurrentChemFile.Builder.NewPDBPolymer();
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
                                CurrentMolecule.Id = atti.Value;
                            }
                            else if (atti.Name.LocalName.Equals("dictRef"))
                            {
                                //                            cdo.SetObjectProperty("Molecule", "dictRef", atts.GetValue(i));
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
                    //                cdo.EndObject("Molecule");
                    if (CurrentMolecule is IAtomContainer)
                    {
                        Debug.WriteLine("Adding molecule to set");
                        CurrentMoleculeSet.Add(CurrentMolecule);
                        Debug.WriteLine("#mols in set: " + CurrentMoleculeSet.Count);
                    }
                    else if (CurrentMolecule is ICrystal)
                    {
                        Debug.WriteLine("Adding crystal to chemModel");
                        CurrentChemModel.Crystal = (ICrystal)CurrentMolecule;
                        CurrentChemSequence.Add(CurrentChemModel);
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
                ElSym.Add(s);
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
                            CurrentBond = CurrentMolecule.Builder.NewBond(CurrentMolecule.Atoms[int.Parse(connect_root) - 1],
                                    CurrentMolecule.Atoms[int.Parse(atom) - 1], BondOrder.Single);
                            CurrentMolecule.Bonds.Add(CurrentBond);
                        }
                    }
                }
            }
        }

        protected override void StoreData()
        {
            if (InChIString != null)
            {
                //            cdo.SetObjectProperty("Molecule", "inchi", inchi);
                CurrentMolecule.SetProperty(CDKPropertyName.InChI, InChIString);
            }
            StoreAtomData();
            StoreBondData();
        }

        protected override void StoreAtomData()
        {
            Debug.WriteLine("No atoms: ", AtomCounter);
            if (AtomCounter == 0)
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

            if (ElId.Count == AtomCounter)
            {
                hasID = true;
            }
            else
            {
                Debug.WriteLine("No atom ids: " + ElId.Count, " != " + AtomCounter);
            }

            if (ElSym.Count == AtomCounter)
            {
                hasSymbols = true;
            }
            else
            {
                Debug.WriteLine("No atom symbols: " + ElSym.Count, " != " + AtomCounter);
            }

            if (ElTitles.Count == AtomCounter)
            {
                hasTitles = true;
            }
            else
            {
                Debug.WriteLine("No atom titles: " + ElTitles.Count, " != " + AtomCounter);
            }

            if ((X3.Count == AtomCounter) && (Y3.Count == AtomCounter) && (Z3.Count == AtomCounter))
            {
                has3D = true;
            }
            else
            {
                Debug.WriteLine("No 3D info: " + X3.Count, " " + Y3.Count, " " + Z3.Count, " != " + AtomCounter);
            }

            if ((XFract.Count == AtomCounter) && (YFract.Count == AtomCounter) && (ZFract.Count == AtomCounter))
            {
                has3Dfract = true;
            }
            else
            {
                Debug.WriteLine("No 3D fractional info: " + XFract.Count, " " + YFract.Count, " " + ZFract.Count, " != "
                        + AtomCounter);
            }

            if ((X2.Count == AtomCounter) && (Y2.Count == AtomCounter))
            {
                has2D = true;
            }
            else
            {
                Debug.WriteLine("No 2D info: " + X2.Count, " " + Y2.Count, " != " + AtomCounter);
            }

            if (FormalCharges.Count == AtomCounter)
            {
                hasFormalCharge = true;
            }
            else
            {
                Debug.WriteLine("No formal Charge info: " + FormalCharges.Count, " != " + AtomCounter);
            }

            if (PartialCharges.Count == AtomCounter)
            {
                hasPartialCharge = true;
            }
            else
            {
                Debug.WriteLine("No partial Charge info: " + PartialCharges.Count, " != " + AtomCounter);
            }

            if (HCounts.Count == AtomCounter)
            {
                hasHCounts = true;
            }
            else
            {
                Debug.WriteLine("No hydrogen Count info: " + HCounts.Count, " != " + AtomCounter);
            }

            if (SpinMultiplicities.Count == AtomCounter)
            {
                hasSpinMultiplicities = true;
            }
            else
            {
                Debug.WriteLine("No spinMultiplicity info: " + SpinMultiplicities.Count, " != " + AtomCounter);
            }

            if (Occupancies.Count == AtomCounter)
            {
                hasOccupancies = true;
            }
            else
            {
                Debug.WriteLine("No occupancy info: " + Occupancies.Count, " != " + AtomCounter);
            }

            if (AtomDictRefs.Count == AtomCounter)
            {
                hasDictRefs = true;
            }
            else
            {
                Debug.WriteLine("No dictRef info: " + AtomDictRefs.Count, " != " + AtomCounter);
            }

            if (Isotope.Count == AtomCounter)
            {
                hasIsotopes = true;
            }
            else
            {
                Debug.WriteLine("No isotope info: " + Isotope.Count, " != " + AtomCounter);
            }
            if (AtomCounter > 0)
            {
                //            cdo.StartObject("PDBMonomer");
                CurrentMonomer = CurrentChemFile.Builder.NewPDBMonomer();
            }

            for (int i = 0; i < AtomCounter; i++)
            {
                Trace.TraceInformation("Storing atom: ", i);
                //            cdo.StartObject("PDBAtom");
                CurrentAtom = CurrentChemFile.Builder.NewPDBAtom("H");
                if (hasID)
                {
                    //                cdo.SetObjectProperty("Atom", "id", (string)elid[i]);
                    CurrentAtom.Id = (string)ElId[i];
                }
                if (hasTitles)
                {
                    if (hasSymbols)
                    {
                        string symbol = (string)ElSym[i];
                        if (symbol.Equals("Du") || symbol.Equals("Dummy"))
                        {
                            //                        cdo.SetObjectProperty("PseudoAtom", "label", (string)eltitles[i]);
                            if (!(CurrentAtom is IPseudoAtom))
                            {
                                CurrentAtom = CurrentChemFile.Builder.NewPseudoAtom(CurrentAtom);
                            }
                            ((IPseudoAtom)CurrentAtom).Label = (string)ElTitles[i];
                        }
                        else
                        {
                            //                        cdo.SetObjectProperty("Atom", "title", (string)eltitles[i]);
                            // FIXME: is a guess, Atom.title is not found in ChemFileCDO
                            CurrentAtom.SetProperty(CDKPropertyName.Title, (string)ElTitles[i]);
                        }
                    }
                    else
                    {
                        //                    cdo.SetObjectProperty("Atom", "title", (string)eltitles[i]);
                        //                    FIXME: is a guess, Atom.title is not found in ChemFileCDO
                        CurrentAtom.SetProperty(CDKPropertyName.Title, (string)ElTitles[i]);
                    }
                }

                // store optional atom properties
                if (hasSymbols)
                {
                    string symbol = (string)ElSym[i];
                    if (symbol.Equals("Du") || symbol.Equals("Dummy"))
                    {
                        symbol = "R";
                    }
                    //                cdo.SetObjectProperty("Atom", "type", symbol);
                    if (symbol.Equals("R") && !(CurrentAtom is IPseudoAtom))
                    {
                        CurrentAtom = CurrentChemFile.Builder.NewPseudoAtom(CurrentAtom);
                    }
                    CurrentAtom.Symbol = symbol;
                    try
                    {
                        Isotopes.Instance.Configure(CurrentAtom);
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError("Could not configure atom: " + CurrentAtom);
                        Debug.WriteLine(e);
                    }
                }

                if (has3D)
                {
                    //                cdo.SetObjectProperty("Atom", "x3", (string)x3[i]);
                    //                cdo.SetObjectProperty("Atom", "y3", (string)y3[i]);
                    //                cdo.SetObjectProperty("Atom", "z3", (string)z3[i]);
                    CurrentAtom.Point3D = new Vector3(
                        double.Parse((string)X3[i]),
                        double.Parse((string)Y3[i]),
                        double.Parse((string)Z3[i]));
                }

                if (has3Dfract)
                {
                    // ok, need to convert fractional into eucledian coordinates
                    //                cdo.SetObjectProperty("Atom", "xFract", (string)xfract[i]);
                    //                cdo.SetObjectProperty("Atom", "yFract", (string)yfract[i]);
                    //                cdo.SetObjectProperty("Atom", "zFract", (string)zfract[i]);
                    CurrentAtom.FractionalPoint3D = new Vector3(
                        double.Parse((string)XFract[i]),
                        double.Parse((string)YFract[i]),
                        double.Parse((string)ZFract[i]));
                }

                if (hasFormalCharge)
                {
                    //              cdo.SetObjectProperty("Atom", "formalCharge",
                    //                                    (string)formalCharges[i]);
                    CurrentAtom.FormalCharge = int.Parse((string)FormalCharges[i]);
                }

                if (hasPartialCharge)
                {
                    Debug.WriteLine("Storing partial atomic charge...");
                    //              cdo.SetObjectProperty("Atom", "partialCharge",
                    //              (string)partialCharges[i]);
                    CurrentAtom.Charge = double.Parse((string)PartialCharges[i]);
                }

                if (hasHCounts)
                {
                    //              cdo.SetObjectProperty("Atom", "hydrogenCount", (string)hCounts[i]);
                    // FIXME: the hCount in CML is the total of implicit *and* explicit
                    CurrentAtom.ImplicitHydrogenCount = int.Parse((string)HCounts[i]);
                }

                if (has2D)
                {
                    if (X2[i] != null && Y2[i] != null)
                    {
                        //                    cdo.SetObjectProperty("Atom", "x2", (string)x2[i]);
                        //                    cdo.SetObjectProperty("Atom", "y2", (string)y2[i]);
                        CurrentAtom.Point2D = new Vector2(
                            double.Parse((string)X2[i]),
                            double.Parse((string)Y2[i]));
                    }
                }

                if (hasDictRefs)
                {
                    //                cdo.SetObjectProperty("Atom", "dictRef", (string)atomDictRefs[i]);
                    CurrentAtom.SetProperty("org.openscience.cdk.dict", (string)AtomDictRefs[i]);
                }

                if (hasSpinMultiplicities && SpinMultiplicities[i] != null)
                {
                    //                cdo.SetObjectProperty("Atom", "spinMultiplicity", (string)spinMultiplicities[i]);
                    int unpairedElectrons = int.Parse((string)SpinMultiplicities[i]) - 1;
                    for (int sm = 0; sm < unpairedElectrons; sm++)
                    {
                        CurrentMolecule.SingleElectrons.Add(CurrentChemFile.Builder.NewSingleElectron(CurrentAtom));
                    }
                }

                if (hasOccupancies && Occupancies[i] != null)
                {
                    //                cdo.SetObjectProperty("PDBAtom", "occupancy", (string)occupancies[i]);
                    double occ = double.Parse((string)Occupancies[i]);
                    if (occ >= 0.0) ((IPDBAtom)CurrentAtom).Occupancy = occ;
                }

                if (hasIsotopes)
                {
                    //              cdo.SetObjectProperty("Atom", "massNumber", (string)isotope[i]);
                    CurrentAtom.MassNumber = int.Parse((string)Isotope[i]);
                }

                if (hasScalar)
                {
                    IPDBAtom pdbAtom = (IPDBAtom)CurrentAtom;
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
                string cResidue = ((IPDBAtom)CurrentAtom).ResName + "A" + ((IPDBAtom)CurrentAtom).ResSeq;
                ((IPDBMonomer)CurrentMonomer).MonomerName = cResidue;
                ((IPDBMonomer)CurrentMonomer).MonomerType = ((IPDBAtom)CurrentAtom).ResName;
                ((IPDBMonomer)CurrentMonomer).ChainID = ((IPDBAtom)CurrentAtom).ChainID;
                ((IPDBMonomer)CurrentMonomer).ICode = ((IPDBAtom)CurrentAtom).ICode;
                ((IPDBPolymer)CurrentMolecule).AddAtom(((IPDBAtom)CurrentAtom), CurrentMonomer, CurrentStrand);
            }
            //        cdo.EndObject("PDBMonomer");
            // nothing done in the CDO for this event
            if (ElId.Count > 0)
            {
                // assume this is the current working list
                BondElId = ElId;
            }
            NewAtomData();
        }
    }
}
