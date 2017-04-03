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
        protected IChemFile CurrentChemFile{ get; set; }

        protected IAtomContainer CurrentMolecule{ get; set; }
        protected IAtomContainerSet<IAtomContainer> CurrentMoleculeSet{ get; set; }
        protected IChemModel CurrentChemModel{ get; set; }
        protected IChemSequence CurrentChemSequence{ get; set; }
        protected IReactionSet CurrentReactionSet{ get; set; }
        protected IReaction CurrentReaction{ get; set; }
        protected IAtom CurrentAtom{ get; set; }
        protected IBond CurrentBond{ get; set; }
        protected IStrand CurrentStrand{ get; set; }
        protected IMonomer CurrentMonomer{ get; set; }
        protected IDictionary<string, IAtom> AtomEnumeration{ get; set; }
        protected IList<string> MoleculeCustomProperty{ get; set; }

        // helper fields
        protected int FormulaCounter{ get; set; }
        protected int AtomCounter{ get; set; }
        protected IList<string> ElSym{ get; set; }
        protected IList<string> ElTitles{ get; set; }
        protected IList<string> ElId{ get; set; }
        protected IList<string> Formula{ get; set; }
        protected IList<string> FormalCharges{ get; set; }
        protected IList<string> PartialCharges{ get; set; }
        protected IList<string> Isotope{ get; set; }
        protected IList<string> AtomicNumbers{ get; set; }
        protected IList<string> ExactMasses{ get; set; }
        protected IList<string> X3{ get; set; }
        protected IList<string> Y3{ get; set; }
        protected IList<string> Z3{ get; set; }
        protected IList<string> X2{ get; set; }
        protected IList<string> Y2{ get; set; }
        protected IList<string> XFract{ get; set; }
        protected IList<string> YFract{ get; set; }
        protected IList<string> ZFract{ get; set; }
        protected IList<string> HCounts{ get; set; }
        protected IList<string> AtomParities{ get; set; }
        protected IList<string> ParityARef1{ get; set; }
        protected IList<string> ParityARef2{ get; set; }
        protected IList<string> ParityARef3{ get; set; }
        protected IList<string> ParityARef4{ get; set; }
        protected IList<string> AtomDictRefs{ get; set; }
        protected IList<string> AtomAromaticities{ get; set; }
        protected IList<string> SpinMultiplicities{ get; set; }
        protected IList<string> Occupancies{ get; set; }
        protected IDictionary<int, IList<string>> AtomCustomProperty{ get; set; }
        protected bool ParityAtomsGiven{ get; set; }
        protected bool ParityGiven{ get; set; }

        protected int BondCounter{ get; set; }
        protected IList<string> BondId{ get; set; }
        protected IList<string> BondARef1{ get; set; }
        protected IList<string> BondARef2{ get; set; }
        protected IList<string> Order{ get; set; }
        protected IList<string> BondStereo{ get; set; }
        protected IList<string> BondDictRefs{ get; set; }
        protected IList<string> BondElId{ get; set; }
        protected List<bool?> BondAromaticity{ get; set; }
        protected IDictionary<string, IDictionary<string, string>> BondCustomProperty{ get; set; }
        protected bool StereoGiven{ get; set; }
        protected string InChIString{ get; set; }
        protected int CurRef{ get; set; }
        protected int CurrentElement{ get; set; }
        protected string BUILTIN{ get; set; }
        protected string DICTREF{ get; set; }
        protected string ElementTitle{ get; set; }

        protected double[] UnitCellParams{ get; set; }
        protected int CrystalScalar{ get; set; }

        public CMLCoreModule(IChemFile chemFile)
        {
            this.CurrentChemFile = chemFile;
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
                this.CurrentChemFile = conv.CurrentChemFile;
                this.CurrentMolecule = conv.CurrentMolecule;
                this.CurrentMoleculeSet = conv.CurrentMoleculeSet;
                this.CurrentChemModel = conv.CurrentChemModel;
                this.CurrentChemSequence = conv.CurrentChemSequence;
                this.CurrentReactionSet = conv.CurrentReactionSet;
                this.CurrentReaction = conv.CurrentReaction;
                this.CurrentAtom = conv.CurrentAtom;
                this.CurrentStrand = conv.CurrentStrand;
                this.CurrentMonomer = conv.CurrentMonomer;
                this.AtomEnumeration = conv.AtomEnumeration;
                this.MoleculeCustomProperty = conv.MoleculeCustomProperty;

                // copy the intermediate fields
                this.BUILTIN = conv.BUILTIN;
                this.AtomCounter = conv.AtomCounter;
                this.FormulaCounter = conv.FormulaCounter;
                this.ElSym = conv.ElSym;
                this.ElTitles = conv.ElTitles;
                this.ElId = conv.ElId;
                this.FormalCharges = conv.FormalCharges;
                this.PartialCharges = conv.PartialCharges;
                this.Isotope = conv.Isotope;
                this.AtomicNumbers = conv.AtomicNumbers;
                this.ExactMasses = conv.ExactMasses;
                this.X3 = conv.X3;
                this.Y3 = conv.Y3;
                this.Z3 = conv.Z3;
                this.X2 = conv.X2;
                this.Y2 = conv.Y2;
                this.XFract = conv.XFract;
                this.YFract = conv.YFract;
                this.ZFract = conv.ZFract;
                this.HCounts = conv.HCounts;
                this.AtomParities = conv.AtomParities;
                this.ParityARef1 = conv.ParityARef1;
                this.ParityARef2 = conv.ParityARef2;
                this.ParityARef3 = conv.ParityARef3;
                this.ParityARef4 = conv.ParityARef4;
                this.AtomDictRefs = conv.AtomDictRefs;
                this.AtomAromaticities = conv.AtomAromaticities;
                this.SpinMultiplicities = conv.SpinMultiplicities;
                this.Occupancies = conv.Occupancies;
                this.BondCounter = conv.BondCounter;
                this.BondId = conv.BondId;
                this.BondARef1 = conv.BondARef1;
                this.BondARef2 = conv.BondARef2;
                this.Order = conv.Order;
                this.BondStereo = conv.BondStereo;
                this.BondCustomProperty = conv.BondCustomProperty;
                this.AtomCustomProperty = conv.AtomCustomProperty;
                this.BondDictRefs = conv.BondDictRefs;
                this.BondAromaticity = conv.BondAromaticity;
                this.CurRef = conv.CurRef;
                this.UnitCellParams = conv.UnitCellParams;
                this.InChIString = conv.InChIString;
            }
            else
            {
                Trace.TraceWarning("Cannot inherit information from module: ", convention.GetType().Name);
            }
        }

        public IChemFile ReturnChemFile()
        {
            return CurrentChemFile;
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
            this.InChIString = null;
        }

        /// <summary>
        /// Clean all data about read formulas.
        /// </summary>
        protected void NewFormulaData()
        {
            FormulaCounter = 0;
            Formula = new List<string>();
        }

        /// <summary>
        /// Clean all data about read atoms.
        /// </summary>
        protected void NewAtomData()
        {
            AtomCounter = 0;
            ElSym = new List<string>();
            ElId = new List<string>();
            ElTitles = new List<string>();
            FormalCharges = new List<string>();
            PartialCharges = new List<string>();
            Isotope = new List<string>();
            AtomicNumbers = new List<string>();
            ExactMasses = new List<string>();
            X3 = new List<string>();
            Y3 = new List<string>();
            Z3 = new List<string>();
            X2 = new List<string>();
            Y2 = new List<string>();
            XFract = new List<string>();
            YFract = new List<string>();
            ZFract = new List<string>();
            HCounts = new List<string>();
            AtomParities = new List<string>();
            ParityARef1 = new List<string>();
            ParityARef2 = new List<string>();
            ParityARef3 = new List<string>();
            ParityARef4 = new List<string>();
            AtomAromaticities = new List<string>();
            AtomDictRefs = new List<string>();
            SpinMultiplicities = new List<string>();
            Occupancies = new List<string>();
            AtomCustomProperty = new Dictionary<int, IList<string>>();
        }

        /// <summary>
        /// Clean all data about read bonds.
        /// </summary>
        protected void NewBondData()
        {
            BondCounter = 0;
            BondId = new List<string>();
            BondARef1 = new List<string>();
            BondARef2 = new List<string>();
            Order = new List<string>();
            BondStereo = new List<string>();
            BondCustomProperty = new Dictionary<string, IDictionary<string, string>>();
            BondDictRefs = new List<string>();
            BondElId = new List<string>();
            BondAromaticity = new List<bool?>();
        }

        /// <summary>
        /// Clean all data about read bonds.
        /// </summary>
        protected void NewCrystalData()
        {
            UnitCellParams = new double[6];
            CrystalScalar = 0;
            //        aAxis = new Vector3();
            //        bAxis = new Vector3();
            //        cAxis = new Vector3();
        }

        public virtual void StartDocument()
        {
            Trace.TraceInformation("Start XML Doc");
            // cdo.StartDocument();
            CurrentChemSequence = CurrentChemFile.Builder.CreateChemSequence();
            CurrentChemModel = CurrentChemFile.Builder.CreateChemModel();
            CurrentMoleculeSet = CurrentChemFile.Builder.CreateAtomContainerSet();
            CurrentMolecule = CurrentChemFile.Builder.CreateAtomContainer();
            AtomEnumeration = new Dictionary<string, IAtom>();
            MoleculeCustomProperty = new List<string>();

            NewMolecule();
            BUILTIN = "";
            CurRef = 0;
        }

        public virtual void EndDocument()
        {
            //        cdo.EndDocument();
            if (CurrentReactionSet != null && CurrentReactionSet.Count == 0 && CurrentReaction != null)
            {
                Debug.WriteLine("Adding reaction to ReactionSet");
                CurrentReactionSet.Add(CurrentReaction);
            }
            if (CurrentReactionSet != null && CurrentChemModel.ReactionSet == null)
            {
                Debug.WriteLine("Adding SOR to ChemModel");
                CurrentChemModel.ReactionSet = CurrentReactionSet;
            }
            if (CurrentMoleculeSet != null && CurrentMoleculeSet.Count != 0)
            {
                Debug.WriteLine("Adding reaction to MoleculeSet");
                CurrentChemModel.MoleculeSet = CurrentMoleculeSet;
            }
            if (CurrentChemSequence.Count == 0)
            {
                Debug.WriteLine("Adding ChemModel to ChemSequence");
                CurrentChemSequence.Add(CurrentChemModel);
            }
            if (CurrentChemFile.Count == 0)
            {
                // assume there is one non-animation ChemSequence
                //            AddChemSequence(currentChemSequence);
                CurrentChemFile.Add(CurrentChemSequence);
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
                    ElementTitle = att.Value;
                    Debug.WriteLine(name, "->TITLE found: ", att.Value);
                }
                else
                {
                    Debug.WriteLine("Qname: ", qname);
                }
            }

            if ("atom".Equals(name))
            {
                AtomCounter++;
                foreach (var atti in element.Attributes())
                {
                    string att = atti.Name.LocalName;
                    string value = atti.Value;

                    if (att.Equals("id"))
                    { // this is supported in CML 1.X
                        ElId.Add(value);
                    } // this is supported in CML 2.0
                    else if (att.Equals("elementType"))
                    {
                        ElSym.Add(value);
                    } // this is supported in CML 2.0
                    else if (att.Equals("title"))
                    {
                        ElTitles.Add(value);
                    } // this is supported in CML 2.0
                    else if (att.Equals("x2"))
                    {
                        X2.Add(value);
                    } // this is supported in CML 2.0
                    else if (att.Equals("xy2"))
                    {
                        var tokens = Strings.Tokenize(value);
                        X2.Add(tokens[0]);
                        Y2.Add(tokens[1]);
                    } // this is supported in CML 2.0
                    else if (att.Equals("xyzFract"))
                    {
                        var tokens = Strings.Tokenize(value);
                        XFract.Add(tokens[0]);
                        YFract.Add(tokens[1]);
                        ZFract.Add(tokens[2]);
                    } // this is supported in CML 2.0
                    else if (att.Equals("xyz3"))
                    {
                        var tokens = Strings.Tokenize(value);
                        X3.Add(tokens[0]);
                        Y3.Add(tokens[1]);
                        Z3.Add(tokens[2]);
                    } // this is supported in CML 2.0
                    else if (att.Equals("y2"))
                    {
                        Y2.Add(value);
                    } // this is supported in CML 2.0
                    else if (att.Equals("x3"))
                    {
                        X3.Add(value);
                    } // this is supported in CML 2.0
                    else if (att.Equals("y3"))
                    {
                        Y3.Add(value);
                    } // this is supported in CML 2.0
                    else if (att.Equals("z3"))
                    {
                        Z3.Add(value);
                    } // this is supported in CML 2.0
                    else if (att.Equals("xFract"))
                    {
                        XFract.Add(value);
                    } // this is supported in CML 2.0
                    else if (att.Equals("yFract"))
                    {
                        YFract.Add(value);
                    } // this is supported in CML 2.0
                    else if (att.Equals("zFract"))
                    {
                        ZFract.Add(value);
                    } // this is supported in CML 2.0
                    else if (att.Equals("formalCharge"))
                    {
                        FormalCharges.Add(value);
                    } // this is supported in CML 2.0
                    else if (att.Equals("hydrogenCount"))
                    {
                        HCounts.Add(value);
                    }
                    else if (att.Equals("isotopeNumber"))
                    {
                        Isotope.Add(value);
                    }
                    else if (att.Equals("dictRef"))
                    {
                        Debug.WriteLine("occupancy: " + value);
                        AtomDictRefs.Add(value);
                    }
                    else if (att.Equals("spinMultiplicity"))
                    {
                        SpinMultiplicities.Add(value);
                    }
                    else if (att.Equals("occupancy"))
                    {
                        Occupancies.Add(value);
                    }

                    else
                    {
                        Trace.TraceWarning("Unparsed attribute: " + att);
                    }

                    ParityAtomsGiven = false;
                    ParityGiven = false;
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
                        count = AddArrayElementsTo(ElId, attribute.Value);
                    }
                    else if (att.Equals("elementType"))
                    {
                        count = AddArrayElementsTo(ElSym, attribute.Value);
                    }
                    else if (att.Equals("x2"))
                    {
                        count = AddArrayElementsTo(X2, attribute.Value);
                    }
                    else if (att.Equals("y2"))
                    {
                        count = AddArrayElementsTo(Y2, attribute.Value);
                    }
                    else if (att.Equals("x3"))
                    {
                        count = AddArrayElementsTo(X3, attribute.Value);
                    }
                    else if (att.Equals("y3"))
                    {
                        count = AddArrayElementsTo(Y3, attribute.Value);
                    }
                    else if (att.Equals("z3"))
                    {
                        count = AddArrayElementsTo(Z3, attribute.Value);
                    }
                    else if (att.Equals("xFract"))
                    {
                        count = AddArrayElementsTo(XFract, attribute.Value);
                    }
                    else if (att.Equals("yFract"))
                    {
                        count = AddArrayElementsTo(YFract, attribute.Value);
                    }
                    else if (att.Equals("zFract"))
                    {
                        count = AddArrayElementsTo(ZFract, attribute.Value);
                    }
                    else
                    {
                        Trace.TraceWarning("Unparsed attribute: " + att);
                    }
                    if (!atomsCounted)
                    {
                        AtomCounter += count;
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
                    if (att.Equals("atomRefs4") && !ParityAtomsGiven)
                    {
                        //Expect exactly four references
                        try
                        {
                            var tokens = Strings.Tokenize(attribute.Value);
                            ParityARef1.Add(tokens[0]);
                            ParityARef2.Add(tokens[1]);
                            ParityARef3.Add(tokens[2]);
                            ParityARef4.Add(tokens[3]);
                            ParityAtomsGiven = true;
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
                BondCounter++;
                foreach (var attribute in element.Attributes())
                {
                    var xname = attribute.Name;
                    string att = xname.LocalName;
                    Debug.WriteLine("B2 ", att, "=", attribute.Value);

                    if (att.Equals("id"))
                    {
                        BondId.Add(attribute.Value);
                        Debug.WriteLine("B3 ", BondId);
                    }
                    else if (att.Equals("atomRefs") || // this is CML 1.X support
                          att.Equals("atomRefs2"))
                    { // this is CML 2.0 support

                        // expect exactly two references
                        try
                        {
                            var tokens = Strings.Tokenize(attribute.Value, ' ');
                            BondARef1.Add(tokens[0]);
                            BondARef2.Add(tokens[1]);
                        }
                        catch (Exception e)
                        {
                            Trace.TraceError("Error in CML file: ", e.Message);
                            Debug.WriteLine(e);
                        }
                    }
                    else if (att.Equals("order"))
                    { // this is CML 2.0 support
                        Order.Add(attribute.Value.Trim());
                    }
                    else if (att.Equals("dictRef"))
                    {
                        BondDictRefs.Add(attribute.Value.Trim());
                    }
                }

                StereoGiven = false;
                CurRef = 0;
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
                        count = AddArrayElementsTo(BondId, attribute.Value);
                    }
                    else if (att.Equals("atomRefs1"))
                    {
                        count = AddArrayElementsTo(BondARef1, attribute.Value);
                    }
                    else if (att.Equals("atomRefs2"))
                    {
                        count = AddArrayElementsTo(BondARef2, attribute.Value);
                    }
                    else if (att.Equals("atomRef1"))
                    {
                        count = AddArrayElementsTo(BondARef1, attribute.Value);
                    }
                    else if (att.Equals("atomRef2"))
                    {
                        count = AddArrayElementsTo(BondARef2, attribute.Value);
                    }
                    else if (att.Equals("order"))
                    {
                        count = AddArrayElementsTo(Order, attribute.Value);
                    }
                    else
                    {
                        Trace.TraceWarning("Unparsed attribute: " + att);
                    }
                    if (!bondsCounted)
                    {
                        BondCounter += count;
                        bondsCounted = true;
                    }
                }
                CurRef = 0;
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
                            BondStereo.Add(value.Substring(4));
                            StereoGiven = true;
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
                        if (attribute.Value.Equals("cdk:aromaticBond")) BondAromaticity.Add(true);
                    }
                }
            }
            else if ("molecule".Equals(name))
            {
                NewMolecule();
                BUILTIN = "";
                //            cdo.StartObject("Molecule");
                if (CurrentChemModel == null)
                    CurrentChemModel = CurrentChemFile.Builder.CreateChemModel();
                if (CurrentMoleculeSet == null)
                    CurrentMoleculeSet = CurrentChemFile.Builder.CreateAtomContainerSet();
                CurrentMolecule = CurrentChemFile.Builder.CreateAtomContainer();
                foreach (var attribute in element.Attributes())
                {
                    var xname = attribute.Name;
                    string att = xname.LocalName;
                    if (att.Equals("id"))
                    {
                        //                    cdo.SetObjectProperty("Molecule", "id", attribute.Value);
                        CurrentMolecule.Id = attribute.Value;
                    }
                    else if (att.Equals("dictRef"))
                    {
                        //                    cdo.SetObjectProperty("Molecule", "dictRef", attribute.Value);
                        CurrentMolecule.SetProperty(new DictRef(DICTREF, attribute.Value), attribute.Value);
                    }
                }
            }
            else if ("crystal".Equals(name))
            {
                NewCrystalData();
                //            cdo.StartObject("Crystal");
                CurrentMolecule = CurrentChemFile.Builder.CreateCrystal(CurrentMolecule);
                foreach (var attribute in element.Attributes())
                {
                    var xname = attribute.Name;
                    string att = xname.LocalName;
                    if (att.Equals("z"))
                    {
                        //                    cdo.SetObjectProperty("Crystal", "z", attribute.Value);
                        ((ICrystal)CurrentMolecule).Z = int.Parse(attribute.Value);
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
                        ((ICrystal)CurrentMolecule).SpaceGroup = attribute.Value;
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
                        CurrentMolecule.SetProperty(CDKPropertyName.InChI, element.Attribute("value").Value);
                }
            }
            else if ("scalar".Equals(name))
            {
                if (xpath.EndsWith("crystal", "scalar")) CrystalScalar++;
            }
            else if ("label".Equals(name))
            {
                if (xpath.EndsWith("atomType", "label"))
                {
                    //                cdo.SetObjectProperty("Atom", "atomTypeLabel", atts.GetValue("value"));
                    CurrentAtom.AtomTypeName = AttGetValue(element.Attributes(), "value");
                }
            }
            else if ("list".Equals(name))
            {
                //            cdo.StartObject("MoleculeSet");
                if (DICTREF.Equals("cdk:model"))
                {
                    CurrentChemModel = CurrentChemFile.Builder.CreateChemModel();
                    // see if there is an ID attribute
                    foreach (var attribute in element.Attributes())
                    {
                        var xname = attribute.Name;
                        string att = xname.LocalName;
                        if (att.Equals("id"))
                        {
                            CurrentChemModel.Id = attribute.Value;
                        }
                    }
                }
                else if (DICTREF.Equals("cdk:moleculeSet"))
                {
                    CurrentMoleculeSet = CurrentChemFile.Builder.CreateAtomContainerSet();
                    // see if there is an ID attribute
                    foreach (var attribute in element.Attributes())
                    {
                        var xname = attribute.Name;
                        string att = xname.LocalName;
                        if (att.Equals("id"))
                        {
                            CurrentMoleculeSet.Id = attribute.Value;
                        }
                    }
                    CurrentMolecule = CurrentChemFile.Builder.CreateAtomContainer();
                }
                else
                {
                    // the old default
                    CurrentMoleculeSet = CurrentChemFile.Builder.CreateAtomContainerSet();
                    // see if there is an ID attribute
                    foreach (var attribute in element.Attributes())
                    {
                        var xname = attribute.Name;
                        string att = xname.LocalName;
                        if (att.Equals("id"))
                        {
                            CurrentMoleculeSet.Id = attribute.Value;
                        }
                    }
                    CurrentMolecule = CurrentChemFile.Builder.CreateAtomContainer();
                }
            }
            else if ("formula".Equals(name))
            {
                FormulaCounter++;
                foreach (var attribute in element.Attributes())
                {
                    var xname = attribute.Name;
                    string att = xname.LocalName;
                    string value = attribute.Value;
                    if (att.Equals("concise"))
                    {
                        Formula.Add(value);
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
                if (!StereoGiven) BondStereo.Add("");
                if (BondCounter > BondDictRefs.Count) BondDictRefs.Add(null);
                if (BondCounter > BondAromaticity.Count) BondAromaticity.Add(null);
            }
            else if ("atom".Equals(name))
            {
                if (AtomCounter > ElTitles.Count)
                {
                    ElTitles.Add(null);
                }
                if (AtomCounter > HCounts.Count)
                {
                    HCounts.Add(null);
                }
                if (AtomCounter > AtomDictRefs.Count)
                {
                    AtomDictRefs.Add(null);
                }
                if (AtomCounter > AtomAromaticities.Count)
                {
                    AtomAromaticities.Add(null);
                }
                if (AtomCounter > Isotope.Count)
                {
                    Isotope.Add(null);
                }
                if (AtomCounter > AtomicNumbers.Count)
                {
                    AtomicNumbers.Add(null);
                }
                if (AtomCounter > ExactMasses.Count)
                {
                    ExactMasses.Add(null);
                }
                if (AtomCounter > SpinMultiplicities.Count)
                {
                    SpinMultiplicities.Add(null);
                }
                if (AtomCounter > Occupancies.Count)
                {
                    Occupancies.Add(null);
                }
                if (AtomCounter > FormalCharges.Count)
                {
                    // while strictly undefined, assume zero formal charge when no
                    // number is given
                    FormalCharges.Add("0");
                }
                if (!ParityGiven)
                {
                    AtomParities.Add("");
                }
                if (!ParityAtomsGiven)
                {
                    ParityARef1.Add("");
                    ParityARef2.Add("");
                    ParityARef3.Add("");
                    ParityARef4.Add("");
                }
                // It may happen that not all atoms have associated 2D or 3D
                // coordinates. accept that
                if (AtomCounter > X2.Count && X2.Count != 0)
                {
                    // apparently, the previous atoms had atomic coordinates, add
                    // 'null' for this atom
                    X2.Add(null);
                    Y2.Add(null);
                }
                if (AtomCounter > X3.Count && X3.Count != 0)
                {
                    // apparently, the previous atoms had atomic coordinates, add
                    // 'null' for this atom
                    X3.Add(null);
                    Y3.Add(null);
                    Z3.Add(null);
                }

                if (AtomCounter > XFract.Count && XFract.Count != 0)
                {
                    // apparently, the previous atoms had atomic coordinates, add
                    // 'null' for this atom
                    XFract.Add(null);
                    YFract.Add(null);
                    ZFract.Add(null);
                }
            }
            else if ("molecule".Equals(name))
            {
                StoreData();
                //            cdo.EndObject("Molecule");
                if (CurrentMolecule is ICrystal)
                {
                    Debug.WriteLine("Adding crystal to chemModel");
                    CurrentChemModel.Crystal = (ICrystal)CurrentMolecule;
                    CurrentChemSequence.Add(CurrentChemModel);
                }
                else if (CurrentMolecule is IAtomContainer)
                {
                    Debug.WriteLine("Adding molecule to set");
                    CurrentMoleculeSet.Add(CurrentMolecule);
                    Debug.WriteLine("#mols in set: " + CurrentMoleculeSet.Count());
                }
            }
            else if ("crystal".Equals(name))
            {
                if (CrystalScalar > 0)
                {
                    // convert unit cell parameters to cartesians
                    Vector3[] axes = CrystalGeometryTools.NotionalToCartesian(UnitCellParams[0], UnitCellParams[1],
                            UnitCellParams[2], UnitCellParams[3], UnitCellParams[4], UnitCellParams[5]);
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
                    ((ICrystal)CurrentMolecule).A = axes[0];
                    ((ICrystal)CurrentMolecule).B = axes[1];
                    ((ICrystal)CurrentMolecule).C = axes[2];
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
                if (CurrentChemModel.MoleculeSet != CurrentMoleculeSet)
                {
                    CurrentChemModel.MoleculeSet = CurrentMoleculeSet;
                    CurrentChemSequence.Add(CurrentChemModel);
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
                        X3.Add(tokens[0]);
                        Y3.Add(tokens[1]);
                        Z3.Add(tokens[2]);
                        Debug.WriteLine("coord3 x3.Length: ", X3.Count);
                        Debug.WriteLine("coord3 y3.Length: ", Y3.Count);
                        Debug.WriteLine("coord3 z3.Length: ", Z3.Count);
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
                    ElSym.Add(cData);
                }
                else if (BUILTIN.Equals("atomRef"))
                {
                    CurRef++;
                    Debug.WriteLine("Bond: ref #", CurRef);

                    if (CurRef == 1)
                    {
                        BondARef1.Add(cData.Trim());
                    }
                    else if (CurRef == 2)
                    {
                        BondARef2.Add(cData.Trim());
                    }
                }
                else if (BUILTIN.Equals("order"))
                {
                    Debug.WriteLine("Bond: order ", cData.Trim());
                    Order.Add(cData.Trim());
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
                    FormalCharges.Add(charge);
                }
            }
            else if ("bondStereo".Equals(name))
            {
                if (!string.IsNullOrEmpty(element.Value) && !StereoGiven)
                {
                    BondStereo.Add(element.Value);
                    StereoGiven = true;
                }
            }
            else if ("atomParity".Equals(name))
            {
                if (!string.IsNullOrEmpty(element.Value) && !ParityGiven && ParityAtomsGiven)
                {
                    AtomParities.Add(element.Value);
                    ParityGiven = true;
                }
            }
            else if ("float".Equals(name))
            {
                if (BUILTIN.Equals("x3"))
                {
                    X3.Add(cData.Trim());
                }
                else if (BUILTIN.Equals("y3"))
                {
                    Y3.Add(cData.Trim());
                }
                else if (BUILTIN.Equals("z3"))
                {
                    Z3.Add(cData.Trim());
                }
                else if (BUILTIN.Equals("x2"))
                {
                    X2.Add(cData.Trim());
                }
                else if (BUILTIN.Equals("y2"))
                {
                    Y2.Add(cData.Trim());
                }
                else if (BUILTIN.Equals("order"))
                {
                    // NOTE: this combination is in violation of the CML DTD!!!
                    Order.Add(cData.Trim());
                }
                else if (BUILTIN.Equals("charge") || BUILTIN.Equals("partialCharge"))
                {
                    PartialCharges.Add(cData.Trim());
                }
            }
            else if ("integer".Equals(name))
            {
                if (BUILTIN.Equals("formalCharge"))
                {
                    FormalCharges.Add(cData.Trim());
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
                        X2.Add(tokens[0]);
                        Y2.Add(tokens[1]);
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
                        bool countAtoms = (AtomCounter == 0) ? true : false;
                        var tokens = Strings.Tokenize(cData);
                        foreach (var token in tokens)
                        {
                            if (countAtoms)
                            {
                                AtomCounter++;
                            }
                            Debug.WriteLine("StringArray (Token): ", token);
                            ElId.Add(token);
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
                        bool countAtoms = (AtomCounter == 0) ? true : false;
                        var tokens = Strings.Tokenize(cData);

                        foreach (var token in tokens)
                        {
                            if (countAtoms)
                            {
                                AtomCounter++;
                            }
                            ElSym.Add(token);
                        }
                    }
                    catch (Exception e)
                    {
                        Notify("CMLParsing error: " + e, SYSTEMID, 194, 1);
                    }
                }
                else if (BUILTIN.Equals("atomRefs"))
                {
                    CurRef++;
                    Debug.WriteLine("New atomRefs found: ", CurRef);

                    try
                    {
                        bool countBonds = (BondCounter == 0) ? true : false;
                        var tokens = Strings.Tokenize(cData);

                        foreach (var token in tokens)
                        {
                            if (countBonds)
                            {
                                BondCounter++;
                            }
                            Debug.WriteLine("Token: ", token);

                            if (CurRef == 1)
                            {
                                BondARef1.Add(token);
                            }
                            else if (CurRef == 2)
                            {
                                BondARef2.Add(token);
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
                    CurRef++;
                    Debug.WriteLine("New atomRef found: ", CurRef); // this is CML1 stuff, we get things like:
                    // <bondArray> <stringArray builtin="atomRef">a2 a2 a2 a2 a3 a3
                    // a4 a4 a5 a6 a7 a9</stringArray> <stringArray
                    // builtin="atomRef">a9 a11 a12 a13 a5 a4 a6 a9 a7 a8 a8
                    // a10</stringArray> <stringArray builtin="order">1 1 1 1 2 1 2
                    // 1 1 1 2 2</stringArray> </bondArray>

                    try
                    {
                        bool countBonds = (BondCounter == 0) ? true : false;
                        var tokens = Strings.Tokenize(cData);

                        foreach (var token in tokens)
                        {
                            if (countBonds)
                            {
                                BondCounter++;
                            }
                            Debug.WriteLine("Token: ", token);

                            if (CurRef == 1)
                            {
                                BondARef1.Add(token);
                            }
                            else if (CurRef == 2)
                            {
                                BondARef2.Add(token);
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
                            Order.Add(token);
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
                            FormalCharges.Add(token);
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
                    Debug.WriteLine("Going to set a crystal parameter: " + CrystalScalar, " to ", cData);
                    try
                    {
                        UnitCellParams[CrystalScalar - 1] = double.Parse(cData.Trim());
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
                        BondStereo.Add(cData.Trim());
                        StereoGiven = true;
                    }
                    else
                    {
                        IDictionary<string, string> bp;
                        if (!BondCustomProperty.TryGetValue(BondId[BondId.Count - 1], out bp))
                        {
                            bp = new Dictionary<string, string>();
                            BondCustomProperty[BondId[BondId.Count - 1]] = bp;
                        }
                        bp[ElementTitle] = cData.Trim();
                    }
                }
                else if (xpath.EndsWith("atom", "scalar"))
                {
                    if (DICTREF.Equals("cdk:partialCharge"))
                    {
                        PartialCharges.Add(cData.Trim());
                    }
                    else if (DICTREF.Equals("cdk:atomicNumber"))
                    {
                        AtomicNumbers.Add(cData.Trim());
                    }
                    else if (DICTREF.Equals("cdk:aromaticAtom"))
                    {
                        AtomAromaticities.Add(cData.Trim());
                    }
                    else if (DICTREF.Equals("cdk:isotopicMass"))
                    {
                        ExactMasses.Add(cData.Trim());
                    }
                    else
                    {
                        if(!AtomCustomProperty.ContainsKey(AtomCounter - 1))
                            AtomCustomProperty[AtomCounter - 1] = new List<string>();
                        AtomCustomProperty[AtomCounter - 1].Add(ElementTitle);
                        AtomCustomProperty[AtomCounter - 1].Add(cData.Trim());
                    }
                }
                else if (xpath.EndsWith("molecule", "scalar"))
                {
                    if (DICTREF.Equals("pdb:id"))
                    {
                        //                    cdo.SetObjectProperty("Molecule", DICTREF, cData);
                        CurrentMolecule.SetProperty(new DictRef(DICTREF, cData), cData);
                    }
                    else if (DICTREF.Equals("cdk:molecularProperty"))
                    {
                        CurrentMolecule.SetProperty(ElementTitle, cData);
                    }
                    else
                    {
                        MoleculeCustomProperty.Add(ElementTitle);
                        MoleculeCustomProperty.Add(cData.Trim());
                    }
                }
                else if (xpath.EndsWith("reaction", "scalar"))
                {
                    if (DICTREF.Equals("cdk:reactionProperty"))
                    {
                        CurrentReaction.SetProperty(ElementTitle, cData);
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
                            X3.Add(token);
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
                            Y3.Add(token);
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
                            Z3.Add(token);
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
                            X2.Add(token);
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
                            Y2.Add(token);
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
                            PartialCharges.Add(token);
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
                this.InChIString = cData;
            }
            else if ("name".Equals(name))
            {
                if (xpath.EndsWith("molecule", "name"))
                {
                    if (DICTREF.Length > 0)
                    {
                        //                    cdo.SetObjectProperty("Molecule", DICTREF, cData);
                        CurrentMolecule.SetProperty(new DictRef(DICTREF, cData), cData);
                    }
                    else
                    {
                        //                    cdo.SetObjectProperty("Molecule", "Name", cData);
                        CurrentMolecule.SetProperty(CDKPropertyName.Title, cData);
                    }
                }
            }
            else if ("formula".Equals(name))
            {
                CurrentMolecule.SetProperty(CDKPropertyName.Formula, cData);
            }
            else
            {

                Trace.TraceWarning("Skipping element: " + name);
            }

            BUILTIN = "";
            ElementTitle = "";
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
            if (InChIString != null)
            {
                //            cdo.SetObjectProperty("Molecule", "inchi", inchi);
                CurrentMolecule.SetProperty(CDKPropertyName.InChI, InChIString);
            }
            if (Formula != null && Formula.Count > 0)
            {
                CurrentMolecule.SetProperty(CDKPropertyName.Formula, Formula);
            }
            IEnumerator<string> customs = MoleculeCustomProperty.GetEnumerator();

            while (customs.MoveNext())
            {
                string x = customs.Current;
                customs.MoveNext();
                string y = customs.Current;
                CurrentMolecule.SetProperty(x, y);
            }
            StoreAtomData();
            NewAtomData();
            StoreBondData();
            NewBondData();
            ConvertCMLToCDKHydrogenCounts();
        }

        private void ConvertCMLToCDKHydrogenCounts()
        {
            foreach (var atom in CurrentMolecule.Atoms)
            {
                if (atom.ImplicitHydrogenCount != null)
                {
                    int explicitHCount = AtomContainerManipulator.CountExplicitHydrogens(CurrentMolecule, atom);
                    if (explicitHCount != 0)
                    {
                        atom.ImplicitHydrogenCount = atom.ImplicitHydrogenCount - explicitHCount;
                    }
                }
            }
        }

        protected virtual void StoreAtomData()
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

            if (AtomAromaticities.Count == AtomCounter)
            {
                hasAtomAromaticities = true;
            }
            else
            {
                Debug.WriteLine("No aromatic atom info: " + AtomAromaticities.Count, " != " + AtomCounter);
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

            if (AtomParities.Count == AtomCounter)
            {
                hasAtomParities = true;
            }
            else
            {
                Debug.WriteLine("No atomParity info: " + SpinMultiplicities.Count, " != " + AtomCounter);
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
            if (AtomicNumbers.Count == AtomCounter)
            {
                hasAtomicNumbers = true;
            }
            else
            {
                Debug.WriteLine("No atomicNumbers info: " + AtomicNumbers.Count, " != " + AtomCounter);
            }
            if (ExactMasses.Count == AtomCounter)
            {
                hasExactMasses = true;
            }
            else
            {
                Debug.WriteLine("No atomicNumbers info: " + AtomicNumbers.Count, " != " + AtomCounter);
            }

            for (int i = 0; i < AtomCounter; i++)
            {
                Trace.TraceInformation("Storing atom: ", i);
                //            cdo.StartObject("Atom");
                CurrentAtom = CurrentChemFile.Builder.CreateAtom("H");
                Debug.WriteLine("Atom # " + AtomCounter);
                if (hasID)
                {
                    //                cdo.SetObjectProperty("Atom", "id", (string)elid[i]);
                    Debug.WriteLine("id: ", (string)ElId[i]);
                    CurrentAtom.Id = (string)ElId[i];
                    AtomEnumeration[(string)ElId[i]] = CurrentAtom;
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
                                CurrentAtom = CurrentChemFile.Builder.CreatePseudoAtom(CurrentAtom);
                                if (hasID) AtomEnumeration[(string)ElId[i]] = CurrentAtom;
                            }
                            ((IPseudoAtom)CurrentAtom).Label = (string)ElTitles[i];
                        }
                        else
                        {
                            //                        cdo.SetObjectProperty("Atom", "title", (string)eltitles[i]);
                            // FIXME: huh?
                            if (ElTitles[i] != null)
                                CurrentAtom.SetProperty(CDKPropertyName.Title, (string)ElTitles[i]);
                        }
                    }
                    else
                    {
                        //                    cdo.SetObjectProperty("Atom", "title", (string)eltitles[i]);
                        // FIXME: huh?
                        if (ElTitles[i] != null) CurrentAtom.SetProperty(CDKPropertyName.Title, (string)ElTitles[i]);
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
                        CurrentAtom = CurrentChemFile.Builder.CreatePseudoAtom(CurrentAtom);
                        ((IPseudoAtom)CurrentAtom).Label = "R";
                        if (hasID) AtomEnumeration[(string)ElId[i]] = CurrentAtom;
                    }
                    CurrentAtom.Symbol = symbol;
                    if (!hasAtomicNumbers || AtomicNumbers[i] == null)
                        CurrentAtom.AtomicNumber = PeriodicTable.GetAtomicNumber(symbol);
                }

                if (has3D)
                {
                    //                cdo.SetObjectProperty("Atom", "x3", (string)x3[i]);
                    //                cdo.SetObjectProperty("Atom", "y3", (string)y3[i]);
                    //                cdo.SetObjectProperty("Atom", "z3", (string)z3[i]);
                    if (X3[i] != null && Y3[i] != null && Z3[i] != null)
                    {
                        CurrentAtom.Point3D = new Vector3(
                            double.Parse((string)X3[i]),
                            double.Parse((string)Y3[i]),
                            double.Parse((string)Z3[i]));
                    }
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
                    //                cdo.SetObjectProperty("Atom", "formalCharge",
                    //                                      (string)formalCharges[i]);
                    CurrentAtom.FormalCharge = int.Parse((string)FormalCharges[i]);
                }

                if (hasAtomAromaticities)
                {
                    if (AtomAromaticities[i] != null) CurrentAtom.IsAromatic = true;
                }

                if (hasPartialCharge)
                {
                    Debug.WriteLine("Storing partial atomic charge...");
                    //                cdo.SetObjectProperty("Atom", "partialCharge",
                    //                                      (string)partialCharges[i]);
                    CurrentAtom.Charge = double.Parse((string)PartialCharges[i]);
                }

                if (hasHCounts)
                {
                    //                cdo.SetObjectProperty("Atom", "hydrogenCount", (string)hCounts[i]);
                    // ConvertCMLToCDKHydrogenCounts() is called to update hydrogen counts when molecule is stored
                    string hCount = HCounts[i];
                    if (hCount != null)
                    {
                        CurrentAtom.ImplicitHydrogenCount = int.Parse(hCount);
                    }
                    else
                    {
                        CurrentAtom.ImplicitHydrogenCount = null;
                    }
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
                    if (AtomDictRefs[i] != null)
                        CurrentAtom.SetProperty("org.openscience.cdk.dict", (string)AtomDictRefs[i]);
                }

                if (hasSpinMultiplicities && SpinMultiplicities[i] != null)
                {
                    //                cdo.SetObjectProperty("Atom", "spinMultiplicity", (string)spinMultiplicities[i]);
                    int unpairedElectrons = int.Parse((string)SpinMultiplicities[i]) - 1;
                    for (int sm = 0; sm < unpairedElectrons; sm++)
                    {
                        CurrentMolecule.SingleElectrons.Add(CurrentChemFile.Builder.CreateSingleElectron(CurrentAtom));
                    }
                }

                if (hasOccupancies && Occupancies[i] != null)
                {
                    //                cdo.SetObjectProperty("Atom", "occupanciy", (string)occupancies[i]);
                    // FIXME: this has no ChemFileCDO equivalent, not even if spelled correctly
                }

                if (hasIsotopes)
                {
                    //                cdo.SetObjectProperty("Atom", "massNumber", (string)isotope[i]);
                    if (Isotope[i] != null)
                        CurrentAtom.MassNumber = (int)double.Parse((string)Isotope[i]);
                }

                if (hasAtomicNumbers)
                {
                    if (AtomicNumbers[i] != null) CurrentAtom.AtomicNumber = int.Parse(AtomicNumbers[i]);
                }

                if (hasExactMasses)
                {
                    if (ExactMasses[i] != null) CurrentAtom.ExactMass = double.Parse(ExactMasses[i]);
                }

                IList<string> property;
                if (AtomCustomProperty.TryGetValue(i, out property))
                {
                    IEnumerator<string> it = property.GetEnumerator();
                    while (it.MoveNext())
                    {
                        var p1 = it.Current;
                        it.MoveNext();
                        var p2 = it.Current;
                        CurrentAtom.SetProperty(p1, p2);
                    }
                }

                //            cdo.EndObject("Atom");

                CurrentMolecule.Atoms.Add(CurrentAtom);
            }

            for (int i = 0; i < AtomCounter; i++)
            {
                if (hasAtomParities && AtomParities[i] != null)
                {
                    try
                    {
                        int parity = (int)Math.Round(double.Parse(AtomParities[i]));
                        //currentAtom.StereoParity = parity;
                        IAtom ligandAtom1 = AtomEnumeration[ParityARef1[i]];
                        IAtom ligandAtom2 = AtomEnumeration[ParityARef2[i]];
                        IAtom ligandAtom3 = AtomEnumeration[ParityARef3[i]];
                        IAtom ligandAtom4 = AtomEnumeration[ParityARef4[i]];
                        IAtom[] ligandAtoms = new IAtom[] { ligandAtom1, ligandAtom2, ligandAtom3, ligandAtom4 };
                        TetrahedralStereo stereo = (parity == 1 ? TetrahedralStereo.Clockwise : TetrahedralStereo.AntiClockwise);
                        TetrahedralChirality chirality = new TetrahedralChirality(CurrentMolecule.Atoms[i], ligandAtoms,
                                stereo);
                        CurrentMolecule.StereoElements.Add(chirality);
                    }
                    catch (FormatException e)
                    {
                        if (!e.Message.Equals("empty string"))
                        {
                            Trace.TraceWarning("Cannot interpret stereo information: " + AtomParities[i]);
                        }
                    }
                }
            }

            if (ElId.Count > 0)
            {
                // assume this is the current working list
                BondElId = ElId;
            }
        }

        protected virtual void StoreBondData()
        {
            Debug.WriteLine("Testing a1,a2,stereo,order = count: " + BondARef1.Count, "," + BondARef2.Count, ","
                    + BondStereo.Count, "," + Order.Count, "=" + BondCounter);

            if ((BondARef1.Count == BondCounter) && (BondARef2.Count == BondCounter))
            {
                Debug.WriteLine("About to add bond info...");

                IEnumerator<string> orders = Order.GetEnumerator();
                IEnumerator<string> ids = BondId.GetEnumerator();
                IEnumerator<string> bar1s = BondARef1.GetEnumerator();
                IEnumerator<string> bar2s = BondARef2.GetEnumerator();
                IEnumerator<string> stereos = BondStereo.GetEnumerator();
                IEnumerator<bool?> aroms = BondAromaticity.GetEnumerator();

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
                    IAtom a1 = (IAtom)AtomEnumeration[(string)bar1s.Current];
                    IAtom a2 = (IAtom)AtomEnumeration[(string)bar2s.Current];
                    CurrentBond = CurrentChemFile.Builder.CreateBond(a1, a2);
                    if (ids.MoveNext())
                    {
                        CurrentBond.Id = (string)ids.Current;
                    }

                    if (orders.MoveNext())
                    {
                        string bondOrder = (string)orders.Current;

                        if ("S".Equals(bondOrder))
                        {
                            //                        cdo.SetObjectProperty("Bond", "order", "1");
                            CurrentBond.Order = BondOrder.Single;
                        }
                        else if ("D".Equals(bondOrder))
                        {
                            //                        cdo.SetObjectProperty("Bond", "order", "2");
                            CurrentBond.Order = BondOrder.Double;
                        }
                        else if ("T".Equals(bondOrder))
                        {
                            //                        cdo.SetObjectProperty("Bond", "order", "3");
                            CurrentBond.Order = BondOrder.Triple;
                        }
                        else if ("A".Equals(bondOrder))
                        {
                            //                        cdo.SetObjectProperty("Bond", "order", "1.5");
                            CurrentBond.Order = BondOrder.Single;
                            CurrentBond.IsAromatic = true;
                        }
                        else
                        {
                            //                        cdo.SetObjectProperty("Bond", "order", bondOrder);
                            CurrentBond.Order = BondManipulator.CreateBondOrder(double.Parse(bondOrder));
                        }
                    }

                    if (stereos.MoveNext())
                    {
                        //                    cdo.SetObjectProperty("Bond", "stereo",
                        //                                          (string)stereos.Next());
                        string nextStereo = (string)stereos.Current;
                        if ("H".Equals(nextStereo))
                        {
                            CurrentBond.Stereo = NCDK.BondStereo.Down;
                        }
                        else if ("W".Equals(nextStereo))
                        {
                            CurrentBond.Stereo = NCDK.BondStereo.Up;
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
                            CurrentBond.IsAromatic = true;
                        }
                    }

                    if (CurrentBond.Id != null)
                    {
                        IDictionary<string, string> currentBondProperties;
                        if (BondCustomProperty.TryGetValue(CurrentBond.Id, out currentBondProperties))
                        {
                            foreach (var key in currentBondProperties.Keys)
                            {
                                CurrentBond.SetProperty(key, currentBondProperties[key]);
                            }
                            BondCustomProperty.Remove(CurrentBond.Id);
                        }
                    }

                    //                cdo.EndObject("Bond");
                    CurrentMolecule.Bonds.Add(CurrentBond);
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

