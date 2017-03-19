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
using NCDK.Common.Primitives;
using NCDK.Geometries;
using NCDK.Stereo;
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NCDK.Numerics;
using System.Xml.Linq;
using NCDK.Dict;

namespace NCDK.IO.CML
{
    /// <summary>
    /// Core CML 1.X and 2.X elements are parsed by this class.
    ///
    /// <para>Please file a bug report if this parser fails to parse
    /// a certain element or attribute value in a valid CML document.</para>
    /// </summary>
    // @cdk.module io
    // @cdk.githash
    // @author Egon Willighagen <egonw@sci.kun.nl>
    public class CMLCoreModule : ICMLModule
    {
        protected const string SYSTEMID = "CML-1999-05-15";
        //    protected IChemicalDocumentObject cdo;

        // data model to store things into
        protected IChemFile currentChemFile;

        protected IAtomContainer currentMolecule;
        protected IAtomContainerSet<IAtomContainer> currentMoleculeSet;
        protected IChemModel currentChemModel;
        protected IChemSequence currentChemSequence;
        protected IReactionSet currentReactionSet;
        protected IReaction currentReaction;
        protected IAtom currentAtom;
        protected IBond currentBond;
        protected IStrand currentStrand;
        protected IMonomer currentMonomer;
        protected IDictionary<string, IAtom> atomEnumeration;
        protected IList<string> moleculeCustomProperty;

        // helper fields
        protected int formulaCounter;
        protected int atomCounter;
        protected IList<string> elsym;
        protected IList<string> eltitles;
        protected IList<string> elid;
        protected IList<string> formula;
        protected IList<string> formalCharges;
        protected IList<string> partialCharges;
        protected IList<string> isotope;
        protected IList<string> atomicNumbers;
        protected IList<string> exactMasses;
        protected IList<string> x3;
        protected IList<string> y3;
        protected IList<string> z3;
        protected IList<string> x2;
        protected IList<string> y2;
        protected IList<string> xfract;
        protected IList<string> yfract;
        protected IList<string> zfract;
        protected IList<string> hCounts;
        protected IList<string> atomParities;
        protected IList<string> parityARef1;
        protected IList<string> parityARef2;
        protected IList<string> parityARef3;
        protected IList<string> parityARef4;
        protected IList<string> atomDictRefs;
        protected IList<string> atomAromaticities;
        protected IList<string> spinMultiplicities;
        protected IList<string> occupancies;
        protected IDictionary<int, IList<string>> atomCustomProperty;
        protected bool parityAtomsGiven;
        protected bool parityGiven;

        protected int bondCounter;
        protected IList<string> bondid;
        protected IList<string> bondARef1;
        protected IList<string> bondARef2;
        protected IList<string> order;
        protected IList<string> bondStereo;
        protected IList<string> bondDictRefs;
        protected IList<string> bondElid;
        protected List<bool?> bondAromaticity;
        protected IDictionary<string, IDictionary<string, string>> bondCustomProperty;
        protected bool stereoGiven;
        protected string inchi;
        protected int curRef;
        protected int CurrentElement;
        protected string BUILTIN;
        protected string DICTREF;
        protected string elementTitle;

        protected double[] unitcellparams;
        protected int crystalScalar;

        public CMLCoreModule(IChemFile chemFile)
        {
            this.currentChemFile = chemFile;
        }

        public CMLCoreModule(ICMLModule conv)
        {
            Inherit(conv);
        }

        public void Inherit(ICMLModule convention)
        {
            if (convention is CMLCoreModule)
            {
                CMLCoreModule conv = (CMLCoreModule)convention;

                // copy the data model
                this.currentChemFile = conv.currentChemFile;
                this.currentMolecule = conv.currentMolecule;
                this.currentMoleculeSet = conv.currentMoleculeSet;
                this.currentChemModel = conv.currentChemModel;
                this.currentChemSequence = conv.currentChemSequence;
                this.currentReactionSet = conv.currentReactionSet;
                this.currentReaction = conv.currentReaction;
                this.currentAtom = conv.currentAtom;
                this.currentStrand = conv.currentStrand;
                this.currentMonomer = conv.currentMonomer;
                this.atomEnumeration = conv.atomEnumeration;
                this.moleculeCustomProperty = conv.moleculeCustomProperty;

                // copy the intermediate fields
                this.BUILTIN = conv.BUILTIN;
                this.atomCounter = conv.atomCounter;
                this.formulaCounter = conv.formulaCounter;
                this.elsym = conv.elsym;
                this.eltitles = conv.eltitles;
                this.elid = conv.elid;
                this.formalCharges = conv.formalCharges;
                this.partialCharges = conv.partialCharges;
                this.isotope = conv.isotope;
                this.atomicNumbers = conv.atomicNumbers;
                this.exactMasses = conv.exactMasses;
                this.x3 = conv.x3;
                this.y3 = conv.y3;
                this.z3 = conv.z3;
                this.x2 = conv.x2;
                this.y2 = conv.y2;
                this.xfract = conv.xfract;
                this.yfract = conv.yfract;
                this.zfract = conv.zfract;
                this.hCounts = conv.hCounts;
                this.atomParities = conv.atomParities;
                this.parityARef1 = conv.parityARef1;
                this.parityARef2 = conv.parityARef2;
                this.parityARef3 = conv.parityARef3;
                this.parityARef4 = conv.parityARef4;
                this.atomDictRefs = conv.atomDictRefs;
                this.atomAromaticities = conv.atomAromaticities;
                this.spinMultiplicities = conv.spinMultiplicities;
                this.occupancies = conv.occupancies;
                this.bondCounter = conv.bondCounter;
                this.bondid = conv.bondid;
                this.bondARef1 = conv.bondARef1;
                this.bondARef2 = conv.bondARef2;
                this.order = conv.order;
                this.bondStereo = conv.bondStereo;
                this.bondCustomProperty = conv.bondCustomProperty;
                this.atomCustomProperty = conv.atomCustomProperty;
                this.bondDictRefs = conv.bondDictRefs;
                this.bondAromaticity = conv.bondAromaticity;
                this.curRef = conv.curRef;
                this.unitcellparams = conv.unitcellparams;
                this.inchi = conv.inchi;
            }
            else
            {
                Trace.TraceWarning("Cannot inherit information from module: ", convention.GetType().Name);
            }
        }

        public IChemFile ReturnChemFile()
        {
            return currentChemFile;
        }

        /// <summary>
        /// Clean all data about parsed data.
        /// </summary>
        protected void NewMolecule()
        {
            NewMoleculeData();
            NewAtomData();
            NewBondData();
            NewCrystalData();
            NewFormulaData();
        }

        /// <summary>
        /// Clean all data about the molecule itself.
        /// </summary>
        protected void NewMoleculeData()
        {
            this.inchi = null;
        }

        /// <summary>
        /// Clean all data about read formulas.
        /// </summary>
        protected void NewFormulaData()
        {
            formulaCounter = 0;
            formula = new List<string>();
        }

        /// <summary>
        /// Clean all data about read atoms.
        /// </summary>
        protected void NewAtomData()
        {
            atomCounter = 0;
            elsym = new List<string>();
            elid = new List<string>();
            eltitles = new List<string>();
            formalCharges = new List<string>();
            partialCharges = new List<string>();
            isotope = new List<string>();
            atomicNumbers = new List<string>();
            exactMasses = new List<string>();
            x3 = new List<string>();
            y3 = new List<string>();
            z3 = new List<string>();
            x2 = new List<string>();
            y2 = new List<string>();
            xfract = new List<string>();
            yfract = new List<string>();
            zfract = new List<string>();
            hCounts = new List<string>();
            atomParities = new List<string>();
            parityARef1 = new List<string>();
            parityARef2 = new List<string>();
            parityARef3 = new List<string>();
            parityARef4 = new List<string>();
            atomAromaticities = new List<string>();
            atomDictRefs = new List<string>();
            spinMultiplicities = new List<string>();
            occupancies = new List<string>();
            atomCustomProperty = new Dictionary<int, IList<string>>();
        }

        /// <summary>
        /// Clean all data about read bonds.
        /// </summary>
        protected void NewBondData()
        {
            bondCounter = 0;
            bondid = new List<string>();
            bondARef1 = new List<string>();
            bondARef2 = new List<string>();
            order = new List<string>();
            bondStereo = new List<string>();
            bondCustomProperty = new Dictionary<string, IDictionary<string, string>>();
            bondDictRefs = new List<string>();
            bondElid = new List<string>();
            bondAromaticity = new List<bool?>();
        }

        /// <summary>
        /// Clean all data about read bonds.
        /// </summary>
        protected void NewCrystalData()
        {
            unitcellparams = new double[6];
            crystalScalar = 0;
            //        aAxis = new Vector3();
            //        bAxis = new Vector3();
            //        cAxis = new Vector3();
        }

        public virtual void StartDocument()
        {
            Trace.TraceInformation("Start XML Doc");
            // cdo.StartDocument();
            currentChemSequence = currentChemFile.Builder.CreateChemSequence();
            currentChemModel = currentChemFile.Builder.CreateChemModel();
            currentMoleculeSet = currentChemFile.Builder.CreateAtomContainerSet();
            currentMolecule = currentChemFile.Builder.CreateAtomContainer();
            atomEnumeration = new Dictionary<string, IAtom>();
            moleculeCustomProperty = new List<string>();

            NewMolecule();
            BUILTIN = "";
            curRef = 0;
        }

        public virtual void EndDocument()
        {
            //        cdo.EndDocument();
            if (currentReactionSet != null && currentReactionSet.Count == 0 && currentReaction != null)
            {
                Debug.WriteLine("Adding reaction to ReactionSet");
                currentReactionSet.Add(currentReaction);
            }
            if (currentReactionSet != null && currentChemModel.ReactionSet == null)
            {
                Debug.WriteLine("Adding SOR to ChemModel");
                currentChemModel.ReactionSet = currentReactionSet;
            }
            if (currentMoleculeSet != null && currentMoleculeSet.Count != 0)
            {
                Debug.WriteLine("Adding reaction to MoleculeSet");
                currentChemModel.MoleculeSet = currentMoleculeSet;
            }
            if (currentChemSequence.Count == 0)
            {
                Debug.WriteLine("Adding ChemModel to ChemSequence");
                currentChemSequence.Add(currentChemModel);
            }
            if (currentChemFile.Count == 0)
            {
                // assume there is one non-animation ChemSequence
                //            AddChemSequence(currentChemSequence);
                currentChemFile.Add(currentChemSequence);
            }

            Trace.TraceInformation("End XML Doc");
        }

        internal static string AttGetValue(IEnumerable<XAttribute> atts, string name)
        {
            XAttribute attribute = atts.Where(n => n.Name.LocalName == name).FirstOrDefault();
            return attribute == null ? null : attribute.Value;
        }

        public virtual void StartElement(CMLStack xpath, XElement element)
        {
            string name = element.Name.LocalName;
            Debug.WriteLine("StartElement");

            BUILTIN = "";
            DICTREF = "";

            foreach (var att in element.Attributes())
            {
                var qname = att.Name.LocalName;
                if (qname.Equals("builtin"))
                {
                    BUILTIN = att.Value;
                    Debug.WriteLine(name, "->BUILTIN found: ", att.Value);
                }
                else if (qname.Equals("dictRef"))
                {
                    DICTREF = att.Value;
                    Debug.WriteLine(name, "->DICTREF found: ", att.Value);
                }
                else if (qname.Equals("title"))
                {
                    elementTitle = att.Value;
                    Debug.WriteLine(name, "->TITLE found: ", att.Value);
                }
                else
                {
                    Debug.WriteLine("Qname: ", qname);
                }
            }

            if ("atom".Equals(name))
            {
                atomCounter++;
                foreach (var atti in element.Attributes())
                {
                    string att = atti.Name.LocalName;
                    string value = atti.Value;

                    if (att.Equals("id"))
                    { // this is supported in CML 1.X
                        elid.Add(value);
                    } // this is supported in CML 2.0
                    else if (att.Equals("elementType"))
                    {
                        elsym.Add(value);
                    } // this is supported in CML 2.0
                    else if (att.Equals("title"))
                    {
                        eltitles.Add(value);
                    } // this is supported in CML 2.0
                    else if (att.Equals("x2"))
                    {
                        x2.Add(value);
                    } // this is supported in CML 2.0
                    else if (att.Equals("xy2"))
                    {
                        var tokens = Strings.Tokenize(value);
                        x2.Add(tokens[0]);
                        y2.Add(tokens[1]);
                    } // this is supported in CML 2.0
                    else if (att.Equals("xyzFract"))
                    {
                        var tokens = Strings.Tokenize(value);
                        xfract.Add(tokens[0]);
                        yfract.Add(tokens[1]);
                        zfract.Add(tokens[2]);
                    } // this is supported in CML 2.0
                    else if (att.Equals("xyz3"))
                    {
                        var tokens = Strings.Tokenize(value);
                        x3.Add(tokens[0]);
                        y3.Add(tokens[1]);
                        z3.Add(tokens[2]);
                    } // this is supported in CML 2.0
                    else if (att.Equals("y2"))
                    {
                        y2.Add(value);
                    } // this is supported in CML 2.0
                    else if (att.Equals("x3"))
                    {
                        x3.Add(value);
                    } // this is supported in CML 2.0
                    else if (att.Equals("y3"))
                    {
                        y3.Add(value);
                    } // this is supported in CML 2.0
                    else if (att.Equals("z3"))
                    {
                        z3.Add(value);
                    } // this is supported in CML 2.0
                    else if (att.Equals("xFract"))
                    {
                        xfract.Add(value);
                    } // this is supported in CML 2.0
                    else if (att.Equals("yFract"))
                    {
                        yfract.Add(value);
                    } // this is supported in CML 2.0
                    else if (att.Equals("zFract"))
                    {
                        zfract.Add(value);
                    } // this is supported in CML 2.0
                    else if (att.Equals("formalCharge"))
                    {
                        formalCharges.Add(value);
                    } // this is supported in CML 2.0
                    else if (att.Equals("hydrogenCount"))
                    {
                        hCounts.Add(value);
                    }
                    else if (att.Equals("isotopeNumber"))
                    {
                        isotope.Add(value);
                    }
                    else if (att.Equals("dictRef"))
                    {
                        Debug.WriteLine("occupancy: " + value);
                        atomDictRefs.Add(value);
                    }
                    else if (att.Equals("spinMultiplicity"))
                    {
                        spinMultiplicities.Add(value);
                    }
                    else if (att.Equals("occupancy"))
                    {
                        occupancies.Add(value);
                    }

                    else
                    {
                        Trace.TraceWarning("Unparsed attribute: " + att);
                    }

                    parityAtomsGiven = false;
                    parityGiven = false;
                }
            }
            else if ("atomArray".Equals(name) && !xpath.EndsWith("formula", "atomArray"))
            {
                bool atomsCounted = false;
                foreach (var attribute in element.Attributes())
                {
                    var xname = attribute.Name;
                    string att = xname.LocalName;
                    int count = 0;
                    if (att.Equals("atomID"))
                    {
                        count = AddArrayElementsTo(elid, attribute.Value);
                    }
                    else if (att.Equals("elementType"))
                    {
                        count = AddArrayElementsTo(elsym, attribute.Value);
                    }
                    else if (att.Equals("x2"))
                    {
                        count = AddArrayElementsTo(x2, attribute.Value);
                    }
                    else if (att.Equals("y2"))
                    {
                        count = AddArrayElementsTo(y2, attribute.Value);
                    }
                    else if (att.Equals("x3"))
                    {
                        count = AddArrayElementsTo(x3, attribute.Value);
                    }
                    else if (att.Equals("y3"))
                    {
                        count = AddArrayElementsTo(y3, attribute.Value);
                    }
                    else if (att.Equals("z3"))
                    {
                        count = AddArrayElementsTo(z3, attribute.Value);
                    }
                    else if (att.Equals("xFract"))
                    {
                        count = AddArrayElementsTo(xfract, attribute.Value);
                    }
                    else if (att.Equals("yFract"))
                    {
                        count = AddArrayElementsTo(yfract, attribute.Value);
                    }
                    else if (att.Equals("zFract"))
                    {
                        count = AddArrayElementsTo(zfract, attribute.Value);
                    }
                    else
                    {
                        Trace.TraceWarning("Unparsed attribute: " + att);
                    }
                    if (!atomsCounted)
                    {
                        atomCounter += count;
                        atomsCounted = true;
                    }
                }
            }
            else if ("atomParity".Equals(name))
            {
                foreach (var attribute in element.Attributes())
                {
                    var xname = attribute.Name;
                    string att = xname.LocalName;
                    if (att.Equals("atomRefs4") && !parityAtomsGiven)
                    {
                        //Expect exactly four references
                        try
                        {
                            var tokens = Strings.Tokenize(attribute.Value);
                            parityARef1.Add(tokens[0]);
                            parityARef2.Add(tokens[1]);
                            parityARef3.Add(tokens[2]);
                            parityARef4.Add(tokens[3]);
                            parityAtomsGiven = true;
                        }
                        catch (Exception e)
                        {
                            Trace.TraceError("Error in CML file: ", e.Message);
                            Debug.WriteLine(e);
                        }
                    }
                }
            }
            else if ("bond".Equals(name))
            {
                bondCounter++;
                foreach (var attribute in element.Attributes())
                {
                    var xname = attribute.Name;
                    string att = xname.LocalName;
                    Debug.WriteLine("B2 ", att, "=", attribute.Value);

                    if (att.Equals("id"))
                    {
                        bondid.Add(attribute.Value);
                        Debug.WriteLine("B3 ", bondid);
                    }
                    else if (att.Equals("atomRefs") || // this is CML 1.X support
                          att.Equals("atomRefs2"))
                    { // this is CML 2.0 support

                        // expect exactly two references
                        try
                        {
                            var tokens = Strings.Tokenize(attribute.Value, ' ');
                            bondARef1.Add(tokens[0]);
                            bondARef2.Add(tokens[1]);
                        }
                        catch (Exception e)
                        {
                            Trace.TraceError("Error in CML file: ", e.Message);
                            Debug.WriteLine(e);
                        }
                    }
                    else if (att.Equals("order"))
                    { // this is CML 2.0 support
                        order.Add(attribute.Value.Trim());
                    }
                    else if (att.Equals("dictRef"))
                    {
                        bondDictRefs.Add(attribute.Value.Trim());
                    }
                }

                stereoGiven = false;
                curRef = 0;
            }
            else if ("bondArray".Equals(name))
            {
                bool bondsCounted = false;
                foreach (var attribute in element.Attributes())
                {
                    var xname = attribute.Name;
                    string att = xname.LocalName;
                    int count = 0;
                    if (att.Equals("bondID"))
                    {
                        count = AddArrayElementsTo(bondid, attribute.Value);
                    }
                    else if (att.Equals("atomRefs1"))
                    {
                        count = AddArrayElementsTo(bondARef1, attribute.Value);
                    }
                    else if (att.Equals("atomRefs2"))
                    {
                        count = AddArrayElementsTo(bondARef2, attribute.Value);
                    }
                    else if (att.Equals("atomRef1"))
                    {
                        count = AddArrayElementsTo(bondARef1, attribute.Value);
                    }
                    else if (att.Equals("atomRef2"))
                    {
                        count = AddArrayElementsTo(bondARef2, attribute.Value);
                    }
                    else if (att.Equals("order"))
                    {
                        count = AddArrayElementsTo(order, attribute.Value);
                    }
                    else
                    {
                        Trace.TraceWarning("Unparsed attribute: " + att);
                    }
                    if (!bondsCounted)
                    {
                        bondCounter += count;
                        bondsCounted = true;
                    }
                }
                curRef = 0;
            }
            else if ("bondStereo".Equals(name))
            {
                foreach (var attribute in element.Attributes())
                {
                    var xname = attribute.Name;
                    string att = xname.LocalName;
                    if (att.Equals("dictRef"))
                    {
                        string value = attribute.Value;
                        if (value.StartsWith("cml:") && value.Length > 4)
                        {
                            bondStereo.Add(value.Substring(4));
                            stereoGiven = true;
                        }
                    }
                }
            }
            else if ("bondType".Equals(name))
            {
                foreach (var attribute in element.Attributes())
                {
                    var xname = attribute.Name;
                    string att = xname.LocalName;
                    if (att.Equals("dictRef"))
                    {
                        if (attribute.Value.Equals("cdk:aromaticBond")) bondAromaticity.Add(true);
                    }
                }
            }
            else if ("molecule".Equals(name))
            {
                NewMolecule();
                BUILTIN = "";
                //            cdo.StartObject("Molecule");
                if (currentChemModel == null)
                    currentChemModel = currentChemFile.Builder.CreateChemModel();
                if (currentMoleculeSet == null)
                    currentMoleculeSet = currentChemFile.Builder.CreateAtomContainerSet();
                currentMolecule = currentChemFile.Builder.CreateAtomContainer();
                foreach (var attribute in element.Attributes())
                {
                    var xname = attribute.Name;
                    string att = xname.LocalName;
                    if (att.Equals("id"))
                    {
                        //                    cdo.SetObjectProperty("Molecule", "id", attribute.Value);
                        currentMolecule.Id = attribute.Value;
                    }
                    else if (att.Equals("dictRef"))
                    {
                        //                    cdo.SetObjectProperty("Molecule", "dictRef", attribute.Value);
                        currentMolecule.SetProperty(new DictRef(DICTREF, attribute.Value), attribute.Value);
                    }
                }
            }
            else if ("crystal".Equals(name))
            {
                NewCrystalData();
                //            cdo.StartObject("Crystal");
                currentMolecule = currentChemFile.Builder.CreateCrystal(currentMolecule);
                foreach (var attribute in element.Attributes())
                {
                    var xname = attribute.Name;
                    string att = xname.LocalName;
                    if (att.Equals("z"))
                    {
                        //                    cdo.SetObjectProperty("Crystal", "z", attribute.Value);
                        ((ICrystal)currentMolecule).Z = int.Parse(attribute.Value);
                    }
                }
            }
            else if ("symmetry".Equals(name))
            {
                foreach (var attribute in element.Attributes())
                {
                    var xname = attribute.Name;
                    string att = xname.LocalName;
                    if (att.Equals("spaceGroup"))
                    {
                        //                    cdo.SetObjectProperty("Crystal", "spacegroup", attribute.Value);
                        ((ICrystal)currentMolecule).SpaceGroup = attribute.Value;
                    }
                }
            }
            else if ("identifier".Equals(name))
            {
                var convention_value = AttGetValue(element.Attributes(), "convention");
                if ("iupac:inchi".Equals(convention_value))
                {
                    //                cdo.SetObjectProperty("Molecule", "inchi", atts.GetValue("value"));
                    if (element.Attribute("value") != null)
                        currentMolecule.SetProperty(CDKPropertyName.InChI, element.Attribute("value").Value);
                }
            }
            else if ("scalar".Equals(name))
            {
                if (xpath.EndsWith("crystal", "scalar")) crystalScalar++;
            }
            else if ("label".Equals(name))
            {
                if (xpath.EndsWith("atomType", "label"))
                {
                    //                cdo.SetObjectProperty("Atom", "atomTypeLabel", atts.GetValue("value"));
                    currentAtom.AtomTypeName = AttGetValue(element.Attributes(), "value");
                }
            }
            else if ("list".Equals(name))
            {
                //            cdo.StartObject("MoleculeSet");
                if (DICTREF.Equals("cdk:model"))
                {
                    currentChemModel = currentChemFile.Builder.CreateChemModel();
                    // see if there is an ID attribute
                    foreach (var attribute in element.Attributes())
                    {
                        var xname = attribute.Name;
                        string att = xname.LocalName;
                        if (att.Equals("id"))
                        {
                            currentChemModel.Id = attribute.Value;
                        }
                    }
                }
                else if (DICTREF.Equals("cdk:moleculeSet"))
                {
                    currentMoleculeSet = currentChemFile.Builder.CreateAtomContainerSet();
                    // see if there is an ID attribute
                    foreach (var attribute in element.Attributes())
                    {
                        var xname = attribute.Name;
                        string att = xname.LocalName;
                        if (att.Equals("id"))
                        {
                            currentMoleculeSet.Id = attribute.Value;
                        }
                    }
                    currentMolecule = currentChemFile.Builder.CreateAtomContainer();
                }
                else
                {
                    // the old default
                    currentMoleculeSet = currentChemFile.Builder.CreateAtomContainerSet();
                    // see if there is an ID attribute
                    foreach (var attribute in element.Attributes())
                    {
                        var xname = attribute.Name;
                        string att = xname.LocalName;
                        if (att.Equals("id"))
                        {
                            currentMoleculeSet.Id = attribute.Value;
                        }
                    }
                    currentMolecule = currentChemFile.Builder.CreateAtomContainer();
                }
            }
            else if ("formula".Equals(name))
            {
                formulaCounter++;
                foreach (var attribute in element.Attributes())
                {
                    var xname = attribute.Name;
                    string att = xname.LocalName;
                    string value = attribute.Value;
                    if (att.Equals("concise"))
                    {
                        formula.Add(value);
                    }
                }
            }
        }

        public virtual void EndElement(CMLStack xpath, XElement element)
        {
            var name = element.Name.LocalName;

            Debug.WriteLine("EndElement: ", name);

            string cData = element.Value;
            if ("bond".Equals(name))
            {
                if (!stereoGiven) bondStereo.Add("");
                if (bondCounter > bondDictRefs.Count) bondDictRefs.Add(null);
                if (bondCounter > bondAromaticity.Count) bondAromaticity.Add(null);
            }
            else if ("atom".Equals(name))
            {
                if (atomCounter > eltitles.Count)
                {
                    eltitles.Add(null);
                }
                if (atomCounter > hCounts.Count)
                {
                    hCounts.Add(null);
                }
                if (atomCounter > atomDictRefs.Count)
                {
                    atomDictRefs.Add(null);
                }
                if (atomCounter > atomAromaticities.Count)
                {
                    atomAromaticities.Add(null);
                }
                if (atomCounter > isotope.Count)
                {
                    isotope.Add(null);
                }
                if (atomCounter > atomicNumbers.Count)
                {
                    atomicNumbers.Add(null);
                }
                if (atomCounter > exactMasses.Count)
                {
                    exactMasses.Add(null);
                }
                if (atomCounter > spinMultiplicities.Count)
                {
                    spinMultiplicities.Add(null);
                }
                if (atomCounter > occupancies.Count)
                {
                    occupancies.Add(null);
                }
                if (atomCounter > formalCharges.Count)
                {
                    // while strictly undefined, assume zero formal charge when no
                    // number is given
                    formalCharges.Add("0");
                }
                if (!parityGiven)
                {
                    atomParities.Add("");
                }
                if (!parityAtomsGiven)
                {
                    parityARef1.Add("");
                    parityARef2.Add("");
                    parityARef3.Add("");
                    parityARef4.Add("");
                }
                // It may happen that not all atoms have associated 2D or 3D
                // coordinates. accept that
                if (atomCounter > x2.Count && x2.Count != 0)
                {
                    // apparently, the previous atoms had atomic coordinates, add
                    // 'null' for this atom
                    x2.Add(null);
                    y2.Add(null);
                }
                if (atomCounter > x3.Count && x3.Count != 0)
                {
                    // apparently, the previous atoms had atomic coordinates, add
                    // 'null' for this atom
                    x3.Add(null);
                    y3.Add(null);
                    z3.Add(null);
                }

                if (atomCounter > xfract.Count && xfract.Count != 0)
                {
                    // apparently, the previous atoms had atomic coordinates, add
                    // 'null' for this atom
                    xfract.Add(null);
                    yfract.Add(null);
                    zfract.Add(null);
                }
            }
            else if ("molecule".Equals(name))
            {
                StoreData();
                //            cdo.EndObject("Molecule");
                if (currentMolecule is ICrystal)
                {
                    Debug.WriteLine("Adding crystal to chemModel");
                    currentChemModel.Crystal = (ICrystal)currentMolecule;
                    currentChemSequence.Add(currentChemModel);
                }
                else if (currentMolecule is IAtomContainer)
                {
                    Debug.WriteLine("Adding molecule to set");
                    currentMoleculeSet.Add(currentMolecule);
                    Debug.WriteLine("#mols in set: " + currentMoleculeSet.Count());
                }
            }
            else if ("crystal".Equals(name))
            {
                if (crystalScalar > 0)
                {
                    // convert unit cell parameters to cartesians
                    Vector3[] axes = CrystalGeometryTools.NotionalToCartesian(unitcellparams[0], unitcellparams[1],
                            unitcellparams[2], unitcellparams[3], unitcellparams[4], unitcellparams[5]);
                    //                cdo.StartObject("a-axis");
                    //                cdo.SetObjectProperty("a-axis", "x", new Double(aAxis.X).ToString());
                    //                cdo.SetObjectProperty("a-axis", "y", new Double(aAxis.Y).ToString());
                    //                cdo.SetObjectProperty("a-axis", "z", new Double(aAxis.Z).ToString());
                    //                cdo.EndObject("a-axis");
                    //                cdo.StartObject("b-axis");
                    //                cdo.SetObjectProperty("b-axis", "x", new Double(bAxis.X).ToString());
                    //                cdo.SetObjectProperty("b-axis", "y", new Double(bAxis.Y).ToString());
                    //                cdo.SetObjectProperty("b-axis", "z", new Double(bAxis.Z).ToString());
                    //                cdo.EndObject("b-axis");
                    //                cdo.StartObject("c-axis");
                    //                cdo.SetObjectProperty("c-axis", "x", new Double(cAxis.X).ToString());
                    //                cdo.SetObjectProperty("c-axis", "y", new Double(cAxis.Y).ToString());
                    //                cdo.SetObjectProperty("c-axis", "z", new Double(cAxis.Z).ToString());
                    //                cdo.EndObject("c-axis");
                    ((ICrystal)currentMolecule).A = axes[0];
                    ((ICrystal)currentMolecule).B = axes[1];
                    ((ICrystal)currentMolecule).C = axes[2];
                }
                else
                {
                    Trace.TraceError("Could not find crystal unit cell parameters");
                }
                //            cdo.EndObject("Crystal");
            }
            else if ("list".Equals(name))
            {
                //            cdo.EndObject("MoleculeSet");
                // FIXME: I really should check the DICTREF, but there is currently
                // no mechanism for storing these for use with EndTag() :(
                // So, instead, for now, just see if it already has done the setting
                // to work around duplication
                if (currentChemModel.MoleculeSet != currentMoleculeSet)
                {
                    currentChemModel.MoleculeSet = currentMoleculeSet;
                    currentChemSequence.Add(currentChemModel);
                }
            }
            else if ("coordinate3".Equals(name))
            {
                if (BUILTIN.Equals("xyz3"))
                {
                    Debug.WriteLine("New coord3 xyz3 found: ", element.Value);

                    try
                    {
                        var tokens = Strings.Tokenize(element.Value);
                        x3.Add(tokens[0]);
                        y3.Add(tokens[1]);
                        z3.Add(tokens[2]);
                        Debug.WriteLine("coord3 x3.Length: ", x3.Count);
                        Debug.WriteLine("coord3 y3.Length: ", y3.Count);
                        Debug.WriteLine("coord3 z3.Length: ", z3.Count);
                    }
                    catch (Exception exception)
                    {
                        Trace.TraceError("CMLParsing error while setting coordinate3!");
                        Debug.WriteLine(exception);
                    }
                }
                else
                {
                    Trace.TraceWarning("Unknown coordinate3 BUILTIN: " + BUILTIN);
                }
            }
            else if ("string".Equals(name))
            {
                if (BUILTIN.Equals("elementType"))
                {
                    Debug.WriteLine("Element: ", cData.Trim());
                    elsym.Add(cData);
                }
                else if (BUILTIN.Equals("atomRef"))
                {
                    curRef++;
                    Debug.WriteLine("Bond: ref #", curRef);

                    if (curRef == 1)
                    {
                        bondARef1.Add(cData.Trim());
                    }
                    else if (curRef == 2)
                    {
                        bondARef2.Add(cData.Trim());
                    }
                }
                else if (BUILTIN.Equals("order"))
                {
                    Debug.WriteLine("Bond: order ", cData.Trim());
                    order.Add(cData.Trim());
                }
                else if (BUILTIN.Equals("formalCharge"))
                {
                    // NOTE: this combination is in violation of the CML DTD!!!
                    Trace.TraceWarning("formalCharge BUILTIN accepted but violating CML DTD");
                    Debug.WriteLine("Charge: ", cData.Trim());
                    string charge = cData.Trim();
                    if (charge.StartsWith("+") && charge.Length > 1)
                    {
                        charge = charge.Substring(1);
                    }
                    formalCharges.Add(charge);
                }
            }
            else if ("bondStereo".Equals(name))
            {
                if (!string.IsNullOrEmpty(element.Value) && !stereoGiven)
                {
                    bondStereo.Add(element.Value);
                    stereoGiven = true;
                }
            }
            else if ("atomParity".Equals(name))
            {
                if (!string.IsNullOrEmpty(element.Value) && !parityGiven && parityAtomsGiven)
                {
                    atomParities.Add(element.Value);
                    parityGiven = true;
                }
            }
            else if ("float".Equals(name))
            {
                if (BUILTIN.Equals("x3"))
                {
                    x3.Add(cData.Trim());
                }
                else if (BUILTIN.Equals("y3"))
                {
                    y3.Add(cData.Trim());
                }
                else if (BUILTIN.Equals("z3"))
                {
                    z3.Add(cData.Trim());
                }
                else if (BUILTIN.Equals("x2"))
                {
                    x2.Add(cData.Trim());
                }
                else if (BUILTIN.Equals("y2"))
                {
                    y2.Add(cData.Trim());
                }
                else if (BUILTIN.Equals("order"))
                {
                    // NOTE: this combination is in violation of the CML DTD!!!
                    order.Add(cData.Trim());
                }
                else if (BUILTIN.Equals("charge") || BUILTIN.Equals("partialCharge"))
                {
                    partialCharges.Add(cData.Trim());
                }
            }
            else if ("integer".Equals(name))
            {
                if (BUILTIN.Equals("formalCharge"))
                {
                    formalCharges.Add(cData.Trim());
                }
            }
            else if ("coordinate2".Equals(name))
            {
                if (BUILTIN.Equals("xy2"))
                {
                    Debug.WriteLine("New coord2 xy2 found.", cData);

                    try
                    {
                        var tokens = Strings.Tokenize(cData);
                        x2.Add(tokens[0]);
                        y2.Add(tokens[1]);
                    }
                    catch (Exception e)
                    {
                        Notify("CMLParsing error: " + e, SYSTEMID, 175, 1);
                    }
                }
            }
            else if ("stringArray".Equals(name))
            {
                if (BUILTIN.Equals("id") || BUILTIN.Equals("atomId") || BUILTIN.Equals("atomID"))
                { // invalid according to CML1 DTD but found in OpenBabel 1.X output

                    try
                    {
                        bool countAtoms = (atomCounter == 0) ? true : false;
                        var tokens = Strings.Tokenize(cData);
                        foreach (var token in tokens)
                        {
                            if (countAtoms)
                            {
                                atomCounter++;
                            }
                            Debug.WriteLine("StringArray (Token): ", token);
                            elid.Add(token);
                        }
                    }
                    catch (Exception e)
                    {
                        Notify("CMLParsing error: " + e, SYSTEMID, 186, 1);
                    }
                }
                else if (BUILTIN.Equals("elementType"))
                {

                    try
                    {
                        bool countAtoms = (atomCounter == 0) ? true : false;
                        var tokens = Strings.Tokenize(cData);

                        foreach (var token in tokens)
                        {
                            if (countAtoms)
                            {
                                atomCounter++;
                            }
                            elsym.Add(token);
                        }
                    }
                    catch (Exception e)
                    {
                        Notify("CMLParsing error: " + e, SYSTEMID, 194, 1);
                    }
                }
                else if (BUILTIN.Equals("atomRefs"))
                {
                    curRef++;
                    Debug.WriteLine("New atomRefs found: ", curRef);

                    try
                    {
                        bool countBonds = (bondCounter == 0) ? true : false;
                        var tokens = Strings.Tokenize(cData);

                        foreach (var token in tokens)
                        {
                            if (countBonds)
                            {
                                bondCounter++;
                            }
                            Debug.WriteLine("Token: ", token);

                            if (curRef == 1)
                            {
                                bondARef1.Add(token);
                            }
                            else if (curRef == 2)
                            {
                                bondARef2.Add(token);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Notify("CMLParsing error: " + e, SYSTEMID, 194, 1);
                    }
                }
                else if (BUILTIN.Equals("atomRef"))
                {
                    curRef++;
                    Debug.WriteLine("New atomRef found: ", curRef); // this is CML1 stuff, we get things like:
                    // <bondArray> <stringArray builtin="atomRef">a2 a2 a2 a2 a3 a3
                    // a4 a4 a5 a6 a7 a9</stringArray> <stringArray
                    // builtin="atomRef">a9 a11 a12 a13 a5 a4 a6 a9 a7 a8 a8
                    // a10</stringArray> <stringArray builtin="order">1 1 1 1 2 1 2
                    // 1 1 1 2 2</stringArray> </bondArray>

                    try
                    {
                        bool countBonds = (bondCounter == 0) ? true : false;
                        var tokens = Strings.Tokenize(cData);

                        foreach (var token in tokens)
                        {
                            if (countBonds)
                            {
                                bondCounter++;
                            }
                            Debug.WriteLine("Token: ", token);

                            if (curRef == 1)
                            {
                                bondARef1.Add(token);
                            }
                            else if (curRef == 2)
                            {
                                bondARef2.Add(token);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Notify("CMLParsing error: " + e, SYSTEMID, 194, 1);
                    }
                }
                else if (BUILTIN.Equals("order"))
                {
                    Debug.WriteLine("New bond order found.");

                    try
                    {

                        var tokens = Strings.Tokenize(cData);

                        foreach (var token in tokens)
                        {
                            Debug.WriteLine("Token: ", token);
                            order.Add(token);
                        }
                    }
                    catch (Exception e)
                    {
                        Notify("CMLParsing error: " + e, SYSTEMID, 194, 1);
                    }
                }
            }
            else if ("integerArray".Equals(name))
            {
                Debug.WriteLine("IntegerArray: builtin = ", BUILTIN);

                if (BUILTIN.Equals("formalCharge"))
                {

                    try
                    {

                        var tokens = Strings.Tokenize(cData);

                        foreach (var token in tokens)
                        {
                            Debug.WriteLine("Charge added: ", token);
                            formalCharges.Add(token);
                        }
                    }
                    catch (Exception e)
                    {
                        Notify("CMLParsing error: " + e, SYSTEMID, 205, 1);
                    }
                }
            }
            else if ("scalar".Equals(name))
            {
                if (xpath.EndsWith("crystal", "scalar"))
                {
                    Debug.WriteLine("Going to set a crystal parameter: " + crystalScalar, " to ", cData);
                    try
                    {
                        unitcellparams[crystalScalar - 1] = double.Parse(cData.Trim());
                    }
                    catch (FormatException)
                    {
                        Trace.TraceError("Content must a float: " + cData);
                    }
                }
                else if (xpath.EndsWith("bond", "scalar"))
                {
                    if (DICTREF.Equals("mdl:stereo"))
                    {
                        bondStereo.Add(cData.Trim());
                        stereoGiven = true;
                    }
                    else
                    {
                        IDictionary<string, string> bp;
                        if (!bondCustomProperty.TryGetValue(bondid[bondid.Count - 1], out bp))
                        {
                            bp = new Dictionary<string, string>();
                            bondCustomProperty[bondid[bondid.Count - 1]] = bp;
                        }
                        bp[elementTitle] = cData.Trim();
                    }
                }
                else if (xpath.EndsWith("atom", "scalar"))
                {
                    if (DICTREF.Equals("cdk:partialCharge"))
                    {
                        partialCharges.Add(cData.Trim());
                    }
                    else if (DICTREF.Equals("cdk:atomicNumber"))
                    {
                        atomicNumbers.Add(cData.Trim());
                    }
                    else if (DICTREF.Equals("cdk:aromaticAtom"))
                    {
                        atomAromaticities.Add(cData.Trim());
                    }
                    else if (DICTREF.Equals("cdk:isotopicMass"))
                    {
                        exactMasses.Add(cData.Trim());
                    }
                    else
                    {
                        if(!atomCustomProperty.ContainsKey(atomCounter - 1))
                            atomCustomProperty[atomCounter - 1] = new List<string>();
                        atomCustomProperty[atomCounter - 1].Add(elementTitle);
                        atomCustomProperty[atomCounter - 1].Add(cData.Trim());
                    }
                }
                else if (xpath.EndsWith("molecule", "scalar"))
                {
                    if (DICTREF.Equals("pdb:id"))
                    {
                        //                    cdo.SetObjectProperty("Molecule", DICTREF, cData);
                        currentMolecule.SetProperty(new DictRef(DICTREF, cData), cData);
                    }
                    else if (DICTREF.Equals("cdk:molecularProperty"))
                    {
                        currentMolecule.SetProperty(elementTitle, cData);
                    }
                    else
                    {
                        moleculeCustomProperty.Add(elementTitle);
                        moleculeCustomProperty.Add(cData.Trim());
                    }
                }
                else if (xpath.EndsWith("reaction", "scalar"))
                {
                    if (DICTREF.Equals("cdk:reactionProperty"))
                    {
                        currentReaction.SetProperty(elementTitle, cData);
                    }
                }
                else
                {
                    Trace.TraceWarning("Ignoring scalar: " + xpath);
                }
            }
            else if ("floatArray".Equals(name))
            {
                if (BUILTIN.Equals("x3"))
                {

                    try
                    {
                        var tokens = Strings.Tokenize(cData);

                        foreach (var token in tokens)
                            x3.Add(token);
                    }
                    catch (Exception e)
                    {
                        Notify("CMLParsing error: " + e, SYSTEMID, 205, 1);
                    }
                }
                else if (BUILTIN.Equals("y3"))
                {

                    try
                    {

                        var tokens = Strings.Tokenize(cData);

                        foreach (var token in tokens)
                            y3.Add(token);
                    }
                    catch (Exception e)
                    {
                        Notify("CMLParsing error: " + e, SYSTEMID, 213, 1);
                    }
                }
                else if (BUILTIN.Equals("z3"))
                {

                    try
                    {

                        var tokens = Strings.Tokenize(cData);

                        foreach (var token in tokens)
                            z3.Add(token);
                    }
                    catch (Exception e)
                    {
                        Notify("CMLParsing error: " + e, SYSTEMID, 221, 1);
                    }
                }
                else if (BUILTIN.Equals("x2"))
                {
                    Debug.WriteLine("New floatArray found.");

                    try
                    {
                        var tokens = Strings.Tokenize(cData);

                        foreach (var token in tokens)
                            x2.Add(token);
                    }
                    catch (Exception e)
                    {
                        Notify("CMLParsing error: " + e, SYSTEMID, 205, 1);
                    }
                }
                else if (BUILTIN.Equals("y2"))
                {
                    Debug.WriteLine("New floatArray found.");

                    try
                    {

                        var tokens = Strings.Tokenize(cData);

                        foreach (var token in tokens)
                            y2.Add(token);
                    }
                    catch (Exception e)
                    {
                        Notify("CMLParsing error: " + e, SYSTEMID, 454, 1);
                    }
                }
                else if (BUILTIN.Equals("partialCharge"))
                {
                    Debug.WriteLine("New floatArray with partial charges found.");

                    try
                    {

                        var tokens = Strings.Tokenize(cData);

                        foreach (var token in tokens)
                            partialCharges.Add(token);
                    }
                    catch (Exception e)
                    {
                        Notify("CMLParsing error: " + e, SYSTEMID, 462, 1);
                    }
                }
            }
            else if ("basic".Equals(name))
            {
                // assuming this is the child element of <identifier>
                this.inchi = cData;
            }
            else if ("name".Equals(name))
            {
                if (xpath.EndsWith("molecule", "name"))
                {
                    if (DICTREF.Length > 0)
                    {
                        //                    cdo.SetObjectProperty("Molecule", DICTREF, cData);
                        currentMolecule.SetProperty(new DictRef(DICTREF, cData), cData);
                    }
                    else
                    {
                        //                    cdo.SetObjectProperty("Molecule", "Name", cData);
                        currentMolecule.SetProperty(CDKPropertyName.Title, cData);
                    }
                }
            }
            else if ("formula".Equals(name))
            {
                currentMolecule.SetProperty(CDKPropertyName.Formula, cData);
            }
            else
            {

                Trace.TraceWarning("Skipping element: " + name);
            }

            BUILTIN = "";
            elementTitle = "";
        }

        public virtual void CharacterData(CMLStack xpath, XElement element)
        {
            Debug.WriteLine($"CD: {element.Value}");
        }

        protected void Notify(string message, string systemId, int line, int column)
        {
            Debug.WriteLine("Message: ", message);
            Debug.WriteLine("SystemId: ", systemId);
            Debug.WriteLine("Line: ", line);
            Debug.WriteLine("Column: ", column);
        }

        protected virtual void StoreData()
        {
            if (inchi != null)
            {
                //            cdo.SetObjectProperty("Molecule", "inchi", inchi);
                currentMolecule.SetProperty(CDKPropertyName.InChI, inchi);
            }
            if (formula != null && formula.Count > 0)
            {
                currentMolecule.SetProperty(CDKPropertyName.Formula, formula);
            }
            IEnumerator<string> customs = moleculeCustomProperty.GetEnumerator();

            while (customs.MoveNext())
            {
                string x = customs.Current;
                customs.MoveNext();
                string y = customs.Current;
                currentMolecule.SetProperty(x, y);
            }
            StoreAtomData();
            NewAtomData();
            StoreBondData();
            NewBondData();
            ConvertCMLToCDKHydrogenCounts();
        }

        private void ConvertCMLToCDKHydrogenCounts()
        {
            foreach (var atom in currentMolecule.Atoms)
            {
                if (atom.ImplicitHydrogenCount != null)
                {
                    int explicitHCount = AtomContainerManipulator.CountExplicitHydrogens(currentMolecule, atom);
                    if (explicitHCount != 0)
                    {
                        atom.ImplicitHydrogenCount = atom.ImplicitHydrogenCount - explicitHCount;
                    }
                }
            }
        }

        protected virtual void StoreAtomData()
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
            bool hasAtomAromaticities = false;
            bool hasPartialCharge = false;
            bool hasHCounts = false;
            bool hasSymbols = false;
            bool hasTitles = false;
            bool hasIsotopes = false;
            bool hasAtomicNumbers = false;
            bool hasExactMasses = false;
            bool hasDictRefs = false;
            bool hasSpinMultiplicities = false;
            bool hasAtomParities = false;
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

            if (atomAromaticities.Count == atomCounter)
            {
                hasAtomAromaticities = true;
            }
            else
            {
                Debug.WriteLine("No aromatic atom info: " + atomAromaticities.Count, " != " + atomCounter);
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

            if (atomParities.Count == atomCounter)
            {
                hasAtomParities = true;
            }
            else
            {
                Debug.WriteLine("No atomParity info: " + spinMultiplicities.Count, " != " + atomCounter);
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
            if (atomicNumbers.Count == atomCounter)
            {
                hasAtomicNumbers = true;
            }
            else
            {
                Debug.WriteLine("No atomicNumbers info: " + atomicNumbers.Count, " != " + atomCounter);
            }
            if (exactMasses.Count == atomCounter)
            {
                hasExactMasses = true;
            }
            else
            {
                Debug.WriteLine("No atomicNumbers info: " + atomicNumbers.Count, " != " + atomCounter);
            }

            for (int i = 0; i < atomCounter; i++)
            {
                Trace.TraceInformation("Storing atom: ", i);
                //            cdo.StartObject("Atom");
                currentAtom = currentChemFile.Builder.CreateAtom("H");
                Debug.WriteLine("Atom # " + atomCounter);
                if (hasID)
                {
                    //                cdo.SetObjectProperty("Atom", "id", (string)elid[i]);
                    Debug.WriteLine("id: ", (string)elid[i]);
                    currentAtom.Id = (string)elid[i];
                    atomEnumeration[(string)elid[i]] = currentAtom;
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
                                if (hasID) atomEnumeration[(string)elid[i]] = currentAtom;
                            }
                            ((IPseudoAtom)currentAtom).Label = (string)eltitles[i];
                        }
                        else
                        {
                            //                        cdo.SetObjectProperty("Atom", "title", (string)eltitles[i]);
                            // FIXME: huh?
                            if (eltitles[i] != null)
                                currentAtom.SetProperty(CDKPropertyName.Title, (string)eltitles[i]);
                        }
                    }
                    else
                    {
                        //                    cdo.SetObjectProperty("Atom", "title", (string)eltitles[i]);
                        // FIXME: huh?
                        if (eltitles[i] != null) currentAtom.SetProperty(CDKPropertyName.Title, (string)eltitles[i]);
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
                        ((IPseudoAtom)currentAtom).Label = "R";
                        if (hasID) atomEnumeration[(string)elid[i]] = currentAtom;
                    }
                    currentAtom.Symbol = symbol;
                    if (!hasAtomicNumbers || atomicNumbers[i] == null)
                        currentAtom.AtomicNumber = PeriodicTable.GetAtomicNumber(symbol);
                }

                if (has3D)
                {
                    //                cdo.SetObjectProperty("Atom", "x3", (string)x3[i]);
                    //                cdo.SetObjectProperty("Atom", "y3", (string)y3[i]);
                    //                cdo.SetObjectProperty("Atom", "z3", (string)z3[i]);
                    if (x3[i] != null && y3[i] != null && z3[i] != null)
                    {
                        currentAtom.Point3D = new Vector3(
                            double.Parse((string)x3[i]),
                            double.Parse((string)y3[i]),
                            double.Parse((string)z3[i]));
                    }
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
                    //                cdo.SetObjectProperty("Atom", "formalCharge",
                    //                                      (string)formalCharges[i]);
                    currentAtom.FormalCharge = int.Parse((string)formalCharges[i]);
                }

                if (hasAtomAromaticities)
                {
                    if (atomAromaticities[i] != null) currentAtom.IsAromatic = true;
                }

                if (hasPartialCharge)
                {
                    Debug.WriteLine("Storing partial atomic charge...");
                    //                cdo.SetObjectProperty("Atom", "partialCharge",
                    //                                      (string)partialCharges[i]);
                    currentAtom.Charge = double.Parse((string)partialCharges[i]);
                }

                if (hasHCounts)
                {
                    //                cdo.SetObjectProperty("Atom", "hydrogenCount", (string)hCounts[i]);
                    // ConvertCMLToCDKHydrogenCounts() is called to update hydrogen counts when molecule is stored
                    string hCount = hCounts[i];
                    if (hCount != null)
                    {
                        currentAtom.ImplicitHydrogenCount = int.Parse(hCount);
                    }
                    else
                    {
                        currentAtom.ImplicitHydrogenCount = null;
                    }
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
                    if (atomDictRefs[i] != null)
                        currentAtom.SetProperty("org.openscience.cdk.dict", (string)atomDictRefs[i]);
                }

                if (hasSpinMultiplicities && spinMultiplicities[i] != null)
                {
                    //                cdo.SetObjectProperty("Atom", "spinMultiplicity", (string)spinMultiplicities[i]);
                    int unpairedElectrons = int.Parse((string)spinMultiplicities[i]) - 1;
                    for (int sm = 0; sm < unpairedElectrons; sm++)
                    {
                        currentMolecule.SingleElectrons.Add(currentChemFile.Builder.CreateSingleElectron(currentAtom));
                    }
                }

                if (hasOccupancies && occupancies[i] != null)
                {
                    //                cdo.SetObjectProperty("Atom", "occupanciy", (string)occupancies[i]);
                    // FIXME: this has no ChemFileCDO equivalent, not even if spelled correctly
                }

                if (hasIsotopes)
                {
                    //                cdo.SetObjectProperty("Atom", "massNumber", (string)isotope[i]);
                    if (isotope[i] != null)
                        currentAtom.MassNumber = (int)double.Parse((string)isotope[i]);
                }

                if (hasAtomicNumbers)
                {
                    if (atomicNumbers[i] != null) currentAtom.AtomicNumber = int.Parse(atomicNumbers[i]);
                }

                if (hasExactMasses)
                {
                    if (exactMasses[i] != null) currentAtom.ExactMass = double.Parse(exactMasses[i]);
                }

                IList<string> property;
                if (atomCustomProperty.TryGetValue(i, out property))
                {
                    IEnumerator<string> it = property.GetEnumerator();
                    while (it.MoveNext())
                    {
                        var p1 = it.Current;
                        it.MoveNext();
                        var p2 = it.Current;
                        currentAtom.SetProperty(p1, p2);
                    }
                }

                //            cdo.EndObject("Atom");

                currentMolecule.Atoms.Add(currentAtom);
            }

            for (int i = 0; i < atomCounter; i++)
            {
                if (hasAtomParities && atomParities[i] != null)
                {
                    try
                    {
                        int parity = (int)Math.Round(double.Parse(atomParities[i]));
                        //currentAtom.StereoParity = parity;
                        IAtom ligandAtom1 = atomEnumeration[parityARef1[i]];
                        IAtom ligandAtom2 = atomEnumeration[parityARef2[i]];
                        IAtom ligandAtom3 = atomEnumeration[parityARef3[i]];
                        IAtom ligandAtom4 = atomEnumeration[parityARef4[i]];
                        IAtom[] ligandAtoms = new IAtom[] { ligandAtom1, ligandAtom2, ligandAtom3, ligandAtom4 };
                        TetrahedralStereo stereo = (parity == 1 ? TetrahedralStereo.Clockwise : TetrahedralStereo.AntiClockwise);
                        TetrahedralChirality chirality = new TetrahedralChirality(currentMolecule.Atoms[i], ligandAtoms,
                                stereo);
                        currentMolecule.StereoElements.Add(chirality);
                    }
                    catch (FormatException e)
                    {
                        if (!e.Message.Equals("empty string"))
                        {
                            Trace.TraceWarning("Cannot interpret stereo information: " + atomParities[i]);
                        }
                    }
                }
            }

            if (elid.Count > 0)
            {
                // assume this is the current working list
                bondElid = elid;
            }
        }

        protected virtual void StoreBondData()
        {
            Debug.WriteLine("Testing a1,a2,stereo,order = count: " + bondARef1.Count, "," + bondARef2.Count, ","
                    + bondStereo.Count, "," + order.Count, "=" + bondCounter);

            if ((bondARef1.Count == bondCounter) && (bondARef2.Count == bondCounter))
            {
                Debug.WriteLine("About to add bond info...");

                IEnumerator<string> orders = order.GetEnumerator();
                IEnumerator<string> ids = bondid.GetEnumerator();
                IEnumerator<string> bar1s = bondARef1.GetEnumerator();
                IEnumerator<string> bar2s = bondARef2.GetEnumerator();
                IEnumerator<string> stereos = bondStereo.GetEnumerator();
                IEnumerator<bool?> aroms = bondAromaticity.GetEnumerator();

                while (bar1s.MoveNext())
                {
                    bar2s.MoveNext();
                    //                cdo.StartObject("Bond");
                    //                if (ids.HasNext()) {
                    //                    cdo.SetObjectProperty("Bond", "id", (string)ids.Next());
                    //                }
                    //                cdo.SetObjectProperty("Bond", "atom1",
                    //                                      int.Parse(bondElid.IndexOf(
                    //                                                          (string)bar1s.Next())).ToString());
                    //                cdo.SetObjectProperty("Bond", "atom2",
                    //                                      int.Parse(bondElid.IndexOf(
                    //                                                          (string)bar2s.Next())).ToString());
                    IAtom a1 = (IAtom)atomEnumeration[(string)bar1s.Current];
                    IAtom a2 = (IAtom)atomEnumeration[(string)bar2s.Current];
                    currentBond = currentChemFile.Builder.CreateBond(a1, a2);
                    if (ids.MoveNext())
                    {
                        currentBond.Id = (string)ids.Current;
                    }

                    if (orders.MoveNext())
                    {
                        string bondOrder = (string)orders.Current;

                        if ("S".Equals(bondOrder))
                        {
                            //                        cdo.SetObjectProperty("Bond", "order", "1");
                            currentBond.Order = BondOrder.Single;
                        }
                        else if ("D".Equals(bondOrder))
                        {
                            //                        cdo.SetObjectProperty("Bond", "order", "2");
                            currentBond.Order = BondOrder.Double;
                        }
                        else if ("T".Equals(bondOrder))
                        {
                            //                        cdo.SetObjectProperty("Bond", "order", "3");
                            currentBond.Order = BondOrder.Triple;
                        }
                        else if ("A".Equals(bondOrder))
                        {
                            //                        cdo.SetObjectProperty("Bond", "order", "1.5");
                            currentBond.Order = BondOrder.Single;
                            currentBond.IsAromatic = true;
                        }
                        else
                        {
                            //                        cdo.SetObjectProperty("Bond", "order", bondOrder);
                            currentBond.Order = BondManipulator.CreateBondOrder(double.Parse(bondOrder));
                        }
                    }

                    if (stereos.MoveNext())
                    {
                        //                    cdo.SetObjectProperty("Bond", "stereo",
                        //                                          (string)stereos.Next());
                        string nextStereo = (string)stereos.Current;
                        if ("H".Equals(nextStereo))
                        {
                            currentBond.Stereo = BondStereo.Down;
                        }
                        else if ("W".Equals(nextStereo))
                        {
                            currentBond.Stereo = BondStereo.Up;
                        }
                        else if (nextStereo != null)
                        {
                            Trace.TraceWarning("Cannot interpret stereo information: " + nextStereo);
                        }
                    }

                    if (aroms.MoveNext())
                    {
                        var nextArom = aroms.Current;
                        if (nextArom != null && nextArom == true)
                        {
                            currentBond.IsAromatic = true;
                        }
                    }

                    if (currentBond.Id != null)
                    {
                        IDictionary<string, string> currentBondProperties;
                        if (bondCustomProperty.TryGetValue(currentBond.Id, out currentBondProperties))
                        {
                            foreach (var key in currentBondProperties.Keys)
                            {
                                currentBond.SetProperty(key, currentBondProperties[key]);
                            }
                            bondCustomProperty.Remove(currentBond.Id);
                        }
                    }

                    //                cdo.EndObject("Bond");
                    currentMolecule.Bonds.Add(currentBond);
                }
            }
        }

        protected int AddArrayElementsTo(IList<string> toAddto, string array)
        {
            int i = 0;
            var tokens = Strings.Tokenize(array);
            foreach (var token in tokens)
            {
                toAddto.Add(token);
                i++;
            }
            return i;
        }
    }
}

