/* Copyright (C) 2005-2007  Stefan Kuhn <shk3@users.sf.net>
 *                    2008  Aleksey Tarkhov <bayern7105@yahoo.de>
 *
 * Contact: jchempaint-devel@lists.sourceforge.net
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
 */
using NCDK.Dict;
using NCDK.Geometries;
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace NCDK.LibIO.CML
{
    /**
     * @cdk.module       libiocml
     * @cdk.githash
     * @cdk.keyword      CML
     * @cdk.keyword      class convertor
     * @cdk.require      java1.5+
     */
    public class Convertor
    {
        public const string NS_CML = "http://www.xml-cml.org/schema";

        private const string CUSTOMIZERS_LIST = "NCDK.libio-cml-customizers.set";
        private IDictionary<string, ICMLCustomizer> customizers = null;

        private bool useCMLIDs;
        private string prefix;

        /**
         * Constructs a CML convertor.
         *
         * @param useCMLIDs Uses object IDs like 'a1' instead of 'a&lt;hash>'.
         * @param prefix    Namespace prefix to use. If null, then no prefix is used;
         */
        public Convertor(bool useCMLIDs, string prefix)
        {
            this.useCMLIDs = useCMLIDs;
            this.prefix = prefix;
            SetupCustomizers();
        }

        public void RegisterCustomizer(ICMLCustomizer customizer)
        {
            if (customizers == null) customizers = new Dictionary<string, ICMLCustomizer>();

            if (!customizers.ContainsKey(customizer.GetType().Name))
            {
                customizers[customizer.GetType().Name] = customizer;
                Trace.TraceInformation("Loaded Customizer: ", customizer.GetType().Name);
            }
            else
            {
                Trace.TraceWarning("Duplicate attempt to register a customizer");
            }
        }

        private void SetupCustomizers()
        {
            if (customizers == null) customizers = new Dictionary<string, ICMLCustomizer>();

            try
            {
                Debug.WriteLine("Starting loading Customizers...");
                TextReader reader = new StreamReader(this.GetType().Assembly.GetManifestResourceStream(CUSTOMIZERS_LIST));
                int customizerCount = 0;
                string customizerName;
                while ((customizerName = reader.ReadLine()) != null)
                {
                    // load them one by one
                    customizerCount++;
                    if (customizers.ContainsKey(customizerName))
                    {
                        try
                        {
                            ICMLCustomizer customizer = (ICMLCustomizer)this.GetType().Assembly.CreateInstance(customizerName);
                            if (customizer == null)
                            {
                                Trace.TraceInformation("Could not find this Customizer: ", customizerName);
                            }
                            else
                            {
                                customizers[customizer.GetType().FullName] = customizer;
                                Trace.TraceInformation("Loaded Customizer: ", customizer.GetType().Name);
                            }
                        }
                        catch (IOException exception) 
                        {
                            Trace.TraceWarning("Could not load this Customizer: ", customizerName);
                            Trace.TraceWarning(exception.Message);
                            Debug.WriteLine(exception);
                        }
                        catch (Exception exception)
                        {
                            Trace.TraceWarning(exception.Message);
                            Debug.WriteLine(exception);
                        }
                    }
                    else
                    {
                        Trace.TraceWarning("Duplicate attempt to register a customizer");
                    }
                }
                Trace.TraceInformation("Number of loaded customizers: ", customizerCount);
            }
            catch (Exception exception)
            {
                Trace.TraceError("Could not load this list: ", CUSTOMIZERS_LIST);
                Debug.WriteLine(exception);
            }
        }


        public CMLCml CDKChemFileToCMLList(IChemFile file)
        {
            return CDKChemFileToCMLList(file, true);
        }

        private CMLCml CDKChemFileToCMLList(IChemFile file, bool setIDs)
        {
            CMLCml cmlList = new CMLCml();
            cmlList.Convention = "cdk:document";

            if (useCMLIDs && setIDs)
            {
                IDCreator.CreateIDs(file);
            }
            if (!string.IsNullOrEmpty(file.Id))
                cmlList.Id = file.Id;

            foreach (var sequence in file)
            {
                cmlList.Add(CDKChemSequenceToCMLList(sequence));
            }

            return cmlList;
        }

        public CMLList CDKChemSequenceToCMLList(IChemSequence sequence)
        {
            return CDKChemSequenceToCMLList(sequence, true);
        }

        private CMLList CDKChemSequenceToCMLList(IChemSequence sequence, bool setIDs)
        {
            CMLList cmlList = new CMLList();
            cmlList.Convention = "cdk:sequence";

            if (useCMLIDs && setIDs)
            {
                IDCreator.CreateIDs(sequence);
            }
            if (!string.IsNullOrEmpty(sequence.Id))
                cmlList.Id = sequence.Id;

            foreach (var chemModel in sequence)
            {
                cmlList.Add(CDKChemModelToCMLList(chemModel));
            }

            return cmlList;
        }

        public CMLList CDKChemModelToCMLList(IChemModel model)
        {
            return CDKChemModelToCMLList(model, true);
        }

        private CMLList CDKChemModelToCMLList(IChemModel model, bool setIDs)
        {
            CMLList cmlList = new CMLList();
            cmlList.Convention = "cdk:model";

            if (useCMLIDs && setIDs)
            {
                IDCreator.CreateIDs(model);
            }
            if (!string.IsNullOrEmpty(model.Id))
                cmlList.Id = model.Id;

            if (model.Crystal != null)
            {
                cmlList.Add(CDKCrystalToCMLMolecule(model.Crystal, false));
            }
            if (model.ReactionSet != null)
            {
                cmlList.Add(CDKReactionSetToCMLReactionList(model.ReactionSet, false));
            }
            if (model.MoleculeSet != null)
            {
                cmlList.Add(CDKAtomContainerSetToCMLList(model.MoleculeSet, false));
            }

            return cmlList;
        }

        public CMLCml CDKReactionSchemeToCMLReactionSchemeAndMoleculeList(IReactionScheme cdkScheme)
        {
            CMLCml cml = new CMLCml();
            cml.Add(CDKAtomContainerSetToCMLList(ReactionSchemeManipulator.GetAllAtomContainers(cdkScheme)));
            cml.Add(CDKReactionSchemeToCMLReactionScheme(cdkScheme, true));
            return cml;
        }

        public CMLReactionScheme CDKReactionSchemeToCMLReactionScheme(IReactionScheme cdkScheme)
        {
            return CDKReactionSchemeToCMLReactionScheme(cdkScheme, true);
        }

        private CMLReactionScheme CDKReactionSchemeToCMLReactionScheme(IReactionScheme cdkScheme, bool setIDs)
        {
            CMLReactionScheme reactionScheme = new CMLReactionScheme();

            if (useCMLIDs && setIDs)
            {
                IDCreator.CreateIDs(cdkScheme);
            }
            if (cdkScheme.Id != null && !cdkScheme.Id.Equals("")) reactionScheme.Id = cdkScheme.Id;

            foreach (var reaction in cdkScheme.Reactions)
                reactionScheme.Add(CDKReactionToCMLReaction(reaction));

            foreach (var intScheme in cdkScheme.Schemes)
                reactionScheme.Add(CDKReactionSchemeToCMLReactionScheme(intScheme));

            return reactionScheme;
        }

        public CMLReactionStep CDKReactionToCMLReactionStep(IReaction reaction)
        {
            return CDKReactionToCMLReactionStep(reaction, true);
        }

        private CMLReactionStep CDKReactionToCMLReactionStep(IReaction reaction, bool setIDs)
        {
            CMLReactionStep reactionStep = new CMLReactionStep();

            reactionStep.Add(CDKReactionToCMLReaction(reaction, true));

            return reactionStep;
        }

        public CMLReactionList CDKReactionSetToCMLReactionList(IReactionSet reactionSet)
        {
            return CDKReactionSetToCMLReactionList(reactionSet, true);
        }

        private CMLReactionList CDKReactionSetToCMLReactionList(IReactionSet reactionSet, bool setIDs)
        {
            CMLReactionList reactionList = new CMLReactionList();

            if (useCMLIDs && setIDs)
            {
                IDCreator.CreateIDs(reactionSet);
            }
            if (reactionSet.Id != null && !reactionSet.Id.Equals("")) reactionList.Id = reactionSet.Id;

            foreach (var reaction in reactionSet)
                reactionList.Add(CDKReactionToCMLReaction(reaction, false));

            return reactionList;
        }

        public CMLMoleculeList CDKAtomContainerSetToCMLList(IAtomContainerSet<IAtomContainer> moleculeSet)
        {
            return CDKAtomContainerSetToCMLList(moleculeSet, true);
        }

        private CMLMoleculeList CDKAtomContainerSetToCMLList(IAtomContainerSet<IAtomContainer> moleculeSet, bool setIDs)
        {
            CMLMoleculeList cmlList = new CMLMoleculeList();
            cmlList.Convention = "cdk:moleculeSet";

            if (useCMLIDs && setIDs)
            {
                IDCreator.CreateIDs(moleculeSet);
            }
            if (moleculeSet.Id != null && !moleculeSet.Id.Equals("")) cmlList.Id = moleculeSet.Id;

            foreach (var container in moleculeSet)
            {
                cmlList.Add(CDKAtomContainerToCMLMolecule(container, false, false));
            }
            return cmlList;
        }

        public CMLReaction CDKReactionToCMLReaction(IReaction reaction)
        {
            return CDKReactionToCMLReaction(reaction, true);
        }

        private CMLReaction CDKReactionToCMLReaction(IReaction reaction, bool setIDs)
        {
            CMLReaction cmlReaction = new CMLReaction();

            if (useCMLIDs && setIDs)
            {
                IDCreator.CreateIDs(reaction);
            }
            if (reaction.Id != null && !reaction.Id.Equals("")) cmlReaction.Id = reaction.Id;

            var props = reaction.GetProperties();
            foreach (var key in props.Keys)
            {
                var value = props[key];
                if (value is string)
                {
                    if (!key.ToString().Equals(CDKPropertyName.TITLE))
                    {
                        CMLScalar scalar = new CMLScalar();
                        this.CheckPrefix(scalar);
                        scalar.DictRef = "cdk:reactionProperty";
                        scalar.Title = key.ToString();
                        scalar.SetValue(value.ToString());
                        cmlReaction.Add(scalar);
                    }
                }
            }

            // reactants
            CMLReactantList cmlReactants = new CMLReactantList();
            foreach (var reactant in reaction.Reactants)
            {
                CMLReactant cmlReactant = new CMLReactant();
                cmlReactant.Add(CDKAtomContainerToCMLMolecule(reactant));
                cmlReactants.Add(cmlReactant);
            }

            // products
            CMLProductList cmlProducts = new CMLProductList();
            foreach (var product in reaction.Products)
            {
                CMLProduct cmlProduct = new CMLProduct();
                cmlProduct.Add(CDKAtomContainerToCMLMolecule(product));
                cmlProducts.Add(cmlProduct);
            }

            //      substance
            CMLSubstanceList cmlSubstances = new CMLSubstanceList();
            foreach (var agent in reaction.Agents)
            {
                CMLSubstance cmlSubstance = new CMLSubstance();
                cmlSubstance.Add(CDKAtomContainerToCMLMolecule(agent));
                cmlSubstances.Add(cmlSubstance);
            }
            if (reaction.Id != null) cmlReaction.Id = reaction.Id;

            cmlReaction.Add(cmlReactants);
            cmlReaction.Add(cmlProducts);
            cmlReaction.Add(cmlSubstances);
            return cmlReaction;
        }

        public CMLMolecule CDKCrystalToCMLMolecule(ICrystal crystal)
        {
            return CDKCrystalToCMLMolecule(crystal, true);
        }

        private CMLMolecule CDKCrystalToCMLMolecule(ICrystal crystal, bool setIDs)
        {
            CMLMolecule molecule = CDKAtomContainerToCMLMolecule(crystal, false, false);
            CMLCrystal cmlCrystal = new CMLCrystal();

            if (useCMLIDs && setIDs)
            {
                IDCreator.CreateIDs(crystal);
            }
            if (crystal.Id != null && !crystal.Id.Equals("")) cmlCrystal.Id = crystal.Id;

            this.CheckPrefix(cmlCrystal);
            cmlCrystal.Z = crystal.Z.Value;
            double[] params_ = CrystalGeometryTools.CartesianToNotional(crystal.A, crystal.B, crystal.C);
            Debug.WriteLine("Number of cell params: ", params_.Length);
            cmlCrystal.SetCellParameters(params_);
            molecule.Add(cmlCrystal);
            return molecule;
        }

        public CMLMolecule CDKPDBPolymerToCMLMolecule(IPDBPolymer pdbPolymer)
        {
            return CDKPDBPolymerToCMLMolecule(pdbPolymer, true);
        }

        private CMLMolecule CDKPDBPolymerToCMLMolecule(IPDBPolymer pdbPolymer, bool setIDs)
        {
            CMLMolecule cmlMolecule = new CMLMolecule();
            cmlMolecule.Convention = "PDB";
            cmlMolecule.DictRef = "pdb:model";

            IDictionary<string, IStrand> mapS = pdbPolymer.GetStrandMap();
            foreach (var key in mapS.Keys)
            {
                IStrand strand = mapS[key];
                List<string> monomerNames = new List<string>(strand.GetMonomerNames());
                monomerNames.Sort();
                foreach (var name in monomerNames)
                {
                    IMonomer monomer = strand.GetMonomer(name);
                    CMLMolecule clmono = CDKMonomerToCMLMolecule(monomer, true);
                    cmlMolecule.Add(clmono);
                }
            }

            return cmlMolecule;
        }

        public CMLMolecule CDKMonomerToCMLMolecule(IMonomer monomer)
        {
            return CDKMonomerToCMLMolecule(monomer, true);
        }

        private CMLMolecule CDKMonomerToCMLMolecule(IMonomer monomer, bool setIDs)
        {
            CMLMolecule cmlMolecule = new CMLMolecule();
            cmlMolecule.DictRef = "pdb:sequence";

            if (monomer.MonomerName != null && !monomer.MonomerName.Equals(""))
                cmlMolecule.Id = monomer.MonomerName;

            for (int i = 0; i < monomer.Atoms.Count; i++)
            {
                IAtom cdkAtom = monomer.Atoms[i];
                CMLAtom cmlAtom = CDKAtomToCMLAtom(monomer, cdkAtom);
                if (monomer.GetConnectedSingleElectrons(cdkAtom).Count() > 0)
                {
                    cmlAtom.SpinMultiplicity = monomer.GetConnectedSingleElectrons(cdkAtom).Count() + 1;
                }
                cmlMolecule.Add(cmlAtom);
            }
            return cmlMolecule;
        }

        public CMLMolecule CDKAtomContainerToCMLMolecule(IAtomContainer structure)
        {
            return CDKAtomContainerToCMLMolecule(structure, true, false);
        }

        private CMLMolecule CDKAtomContainerToCMLMolecule(IAtomContainer structure, bool setIDs, bool isRef)
        {
            CMLMolecule cmlMolecule = new CMLMolecule();

            if (useCMLIDs && setIDs)
            {
                IDCreator.CreateIDs(structure);
            }

            this.CheckPrefix(cmlMolecule);
            if (structure.Id != null && !structure.Id.Equals("")) if (!isRef)
                    cmlMolecule.Id = structure.Id;
                else
                    cmlMolecule.Ref = structure.Id;

            if (structure.GetProperty<string>(CDKPropertyName.TITLE) != null)
            {
                cmlMolecule.Title = structure.GetProperty<string>(CDKPropertyName.TITLE);
            }
            if (structure.GetProperty<string>(CDKPropertyName.INCHI) != null)
            {
                CMLIdentifier ident = new CMLIdentifier();
                ident.Convention = "iupac:inchi";
                ident.SetAttributeValue(CMLElement.Attribute_value,  structure.GetProperty<string>(CDKPropertyName.INCHI).ToString());
                cmlMolecule.Add(ident);
            }
            if (!isRef)
            {
                for (int i = 0; i < structure.Atoms.Count; i++)
                {
                    IAtom cdkAtom = structure.Atoms[i];
                    CMLAtom cmlAtom = CDKAtomToCMLAtom(structure, cdkAtom);
                    if (structure.GetConnectedSingleElectrons(cdkAtom).Count() > 0)
                    {
                        cmlAtom.SpinMultiplicity = structure.GetConnectedSingleElectrons(cdkAtom).Count() + 1;
                    }
                    cmlMolecule.Add(cmlAtom);
                }
                for (int i = 0; i < structure.Bonds.Count; i++)
                {
                    CMLBond cmlBond = CDKJBondToCMLBond(structure.Bonds[i]);
                    cmlMolecule.Add(cmlBond);
                }
            }

            // ok, output molecular properties, but not TITLE, INCHI, or DictRef's
            var props = structure.GetProperties();
            foreach (var key in props.Keys)
            {
                // but only if a string
                if (key is string && !isRef)
                {
                    object value = props[key];
                    if (!key.Equals(CDKPropertyName.TITLE) && !key.Equals(CDKPropertyName.INCHI))
                    {
                        // ok, should output this
                        CMLScalar scalar = new CMLScalar();
                        this.CheckPrefix(scalar);
                        scalar.DictRef = "cdk:molecularProperty";
                        scalar.Title = (string)key;
                        scalar.SetValue(value.ToString());
                        cmlMolecule.Add(scalar);
                    }
                }
                // FIXME: At the moment the order writing the formula is into properties
                // but it should be that IMolecularFormula is a extension of IAtomContainer
                if (key is string && !isRef && key.ToString().Equals(CDKPropertyName.FORMULA))
                {
                    if (props[key] is IMolecularFormula)
                    {
                        IMolecularFormula cdkFormula = (IMolecularFormula)props[key];

                        CMLFormula cmlFormula = new CMLFormula();
                        IList<IIsotope> isotopesList = MolecularFormulaManipulator.PutInOrder(
                                MolecularFormulaManipulator.OrderEle, cdkFormula);
                        foreach (var isotope in isotopesList)
                        {
                            cmlFormula.Add(isotope.Symbol, cdkFormula.GetCount(isotope));
                        }
                        cmlMolecule.Add(cmlFormula);
                    }
                    else if (props[key] is IMolecularFormulaSet)
                    {
                        IMolecularFormulaSet cdkFormulaSet = (IMolecularFormulaSet)props[key];
                        foreach (var cdkFormula in cdkFormulaSet)
                        {
                            var isotopesList = MolecularFormulaManipulator.PutInOrder(
                                    MolecularFormulaManipulator.OrderEle, cdkFormula);
                            CMLFormula cmlFormula = new CMLFormula();
                            cmlFormula.DictRef = "cdk:possibleMachts";
                            foreach (var isotope in isotopesList)
                            {
                                cmlFormula.Add(isotope.Symbol,
                                        cdkFormula.GetCount(isotope));
                            }
                            cmlMolecule.Add(cmlFormula);
                        }
                    }
                }
            }

            foreach (var element in customizers.Keys)
            {
                ICMLCustomizer customizer = customizers[element];
                try
                {
                    customizer.Customize(structure, cmlMolecule);
                }
                catch (Exception exception)
                {
                    Trace.TraceError("Error while customizing CML output with customizer: ", customizer.GetType().Name);
                    Debug.WriteLine(exception);
                }
            }
            return cmlMolecule;
        }

        private bool AddDictRef(IChemObject obj, CMLElement cmlElement)
        {
            var properties = obj.GetProperties();
            foreach (var key in properties.Keys)
            {
                if (key is string)
                {
                    string keyName = (string)key;
                    if (keyName.StartsWith(DictionaryDatabase.DICTREFPROPERTYNAME))
                    {
                        string dictRef = (string)properties[keyName];
                        cmlElement.SetAttributeValue("dictRef", dictRef);
                        return true;
                    }
                }
            }
            return false;
        }

        private bool AddAtomID(IAtom cdkAtom, CMLAtom cmlAtom)
        {
            if (cdkAtom.Id != null && !cdkAtom.Id.Equals(""))
            {
                cmlAtom.Id = cdkAtom.Id;
            }
            else
            {
                cmlAtom.Id = "a" + cdkAtom.GetHashCode().ToString();
            }
            return true;
        }

        public CMLAtom CDKAtomToCMLAtom(IAtom cdkAtom)
        {
            return CDKAtomToCMLAtom(null, cdkAtom);
        }

        public CMLAtom CDKAtomToCMLAtom(IAtomContainer container, IAtom cdkAtom)
        {
            CMLAtom cmlAtom = new CMLAtom();
            this.CheckPrefix(cmlAtom);
            AddAtomID(cdkAtom, cmlAtom);
            AddDictRef(cdkAtom, cmlAtom);
            cmlAtom.ElementType = cdkAtom.Symbol;
            if (cdkAtom is IPseudoAtom)
            {
                string label = ((IPseudoAtom)cdkAtom).Label;
                if (label != null) cmlAtom.Title = label;
                cmlAtom.ElementType = "Du";
            }
            Map2DCoordsToCML(cmlAtom, cdkAtom);
            Map3DCoordsToCML(cmlAtom, cdkAtom);
            MapFractionalCoordsToCML(cmlAtom, cdkAtom);

            int? formalCharge = cdkAtom.FormalCharge;
            if (formalCharge != null) cmlAtom.FormalCharge = formalCharge.Value;

            // CML's hydrogen count consists of the sum of implicit and explicit
            // hydrogens (see bug #1655045).
            int? totalHydrogen = cdkAtom.ImplicitHydrogenCount;
            if (totalHydrogen != null)
            {
                if (container != null)
                {
                    foreach (var bond in container.GetConnectedBonds(cdkAtom))
                    {
                        foreach (var atom in bond.Atoms)
                        {
                            if ("H".Equals(atom.Symbol) && atom != cdkAtom) totalHydrogen++;
                        }
                    }
                } // else: it is the implicit hydrogen count
                cmlAtom.HydrogenCount = totalHydrogen.Value;
            } // else: don't report it, people can count the explicit Hs themselves

            int? massNumber = cdkAtom.MassNumber;
            if (!(cdkAtom is IPseudoAtom))
            {
                if (massNumber != null)
                {
                    cmlAtom.IsotopeNumber = massNumber.Value;
                }
            }

            if (cdkAtom.Charge != null)
            {
                CMLScalar scalar = new CMLScalar();
                this.CheckPrefix(scalar);
                //            scalar.SetDataType("xsd:float");
                scalar.DictRef = "cdk:partialCharge";
                scalar.SetValue(cdkAtom.Charge.Value);
                cmlAtom.Add(scalar);
            }
            WriteProperties(cdkAtom, cmlAtom);

            if (cdkAtom.IsAromatic)
            {
                CMLScalar aromAtom = new CMLScalar();
                aromAtom.DictRef = "cdk:aromaticAtom";
                cmlAtom.Add(aromAtom);
            }

            foreach (var element in customizers.Keys)
            {
                ICMLCustomizer customizer = (ICMLCustomizer)customizers[element];
                try
                {
                    customizer.Customize(cdkAtom, cmlAtom);
                }
                catch (Exception exception)
                {
                    Trace.TraceError("Error while customizing CML output with customizer: ", customizer.GetType().Name);
                    Debug.WriteLine(exception);
                }
            }
            return cmlAtom;
        }

        public CMLBond CDKJBondToCMLBond(IBond cdkBond)
        {
            CMLBond cmlBond = new CMLBond();
            this.CheckPrefix(cmlBond);
            if (cdkBond.Id == null || cdkBond.Id.Length == 0)
            {
                cmlBond.Id = "b" + cdkBond.GetHashCode();
            }
            else
            {
                cmlBond.Id = cdkBond.Id;
            }

            string[] atomRefArray = new string[cdkBond.Atoms.Count];
            for (int i = 0; i < cdkBond.Atoms.Count; i++)
            {
                string atomID = cdkBond.Atoms[i].Id;
                if (atomID == null || atomID.Length == 0)
                {
                    atomRefArray[i] = "a" + cdkBond.Atoms[i].GetHashCode().ToString();
                }
                else
                {
                    atomRefArray[i] = atomID;
                }
            }
            if (atomRefArray.Length == 2)
            {
                cmlBond.AtomRefs2 = atomRefArray;
            }
            else
            {
                cmlBond.AtomRefs = atomRefArray;
            }

            BondOrder border = cdkBond.Order;
            if (border == BondOrder.Single)
            {
                cmlBond.Order = "S";
            }
            else if (border == BondOrder.Double)
            {
                cmlBond.Order = "D";
            }
            else if (border == BondOrder.Triple)
            {
                cmlBond.Order = "T";
            }
            else
            {
                CMLScalar scalar = new CMLScalar();
                this.CheckPrefix(scalar);
                //            scalar.SetDataType("xsd:float");
                scalar.DictRef = "cdk:bondOrder";
                scalar.Title = "order";
                scalar.SetValue(cdkBond.Order.Numeric);
                cmlBond.Add(scalar);
            }
            if (cdkBond.IsAromatic)
            {
                CMLBondType bType = new CMLBondType();
                bType.DictRef = "cdk:aromaticBond";
                cmlBond.Add(bType);
            }

            if (cdkBond.Stereo == BondStereo.Up || cdkBond.Stereo == BondStereo.Down)
            {
                CMLBondStereo bondStereo = new CMLBondStereo();
                this.CheckPrefix(bondStereo);
                if (cdkBond.Stereo == BondStereo.Up)
                {
                    bondStereo.DictRef = "cml:W";
                    bondStereo.Value = "W";
                }
                else
                {
                    bondStereo.DictRef = "cml:H";
                    bondStereo.Value = "H";
                }
                cmlBond.Add(bondStereo);
            }
            if (cdkBond.GetProperties().Count > 0) WriteProperties(cdkBond, cmlBond);

            foreach (var element in customizers.Keys)
            {
                ICMLCustomizer customizer = customizers[element];
                try
                {
                    customizer.Customize(cdkBond, cmlBond);
                }
                catch (Exception exception)
                {
                    Trace.TraceError("Error while customizing CML output with customizer: ", customizer.GetType().Name);
                    Debug.WriteLine(exception);
                }
            }

            return cmlBond;
        }

        private void WriteProperties(IChemObject obj, CMLElement cmlElement)
        {
            var props = obj.GetProperties();
            foreach (var key in props.Keys)
            {
                //            if (key is DictRef)
                //            {
                //                Object value = props[key];
                //                CMLScalar scalar = new CMLScalar();
                //                this.CheckPrefix(scalar);
                //                scalar.SetDictRef(((DictRef)key).GetType());
                //                scalar.SetValue(value.ToString());
                //                cmlElement.AppendChild(scalar);
                //            }
                //            else 
                //if (key is string)
                {
                    string stringKey = (string)key;
                    if (stringKey.Equals(CDKPropertyName.TITLE))
                    {
                        // don't output this one. It's covered by AddTitle()
                    }
                    else if (!(stringKey.StartsWith("org.openscience.cdk")))
                    {
                        object value = props[key];
                        CMLScalar scalar = new CMLScalar();
                        this.CheckPrefix(scalar);
                        scalar.Title = (string)key;
                        scalar.Value = value.ToString();
                        cmlElement.Add(scalar);
                    }
                }
            }
        }

        private void MapFractionalCoordsToCML(CMLAtom cmlAtom, IAtom cdkAtom)
        {
            if (cdkAtom.FractionalPoint3D != null)
            {
                cmlAtom.XFract = cdkAtom.FractionalPoint3D.Value.X;
                cmlAtom.YFract = cdkAtom.FractionalPoint3D.Value.Y;
                cmlAtom.ZFract = cdkAtom.FractionalPoint3D.Value.Z;
            }
        }

        private void Map3DCoordsToCML(CMLAtom cmlAtom, IAtom cdkAtom)
        {
            if (cdkAtom.Point3D != null)
            {
                cmlAtom.X3 = cdkAtom.Point3D.Value.X;
                cmlAtom.Y3 = cdkAtom.Point3D.Value.Y;
                cmlAtom.Z3 = cdkAtom.Point3D.Value.Z;
            }
        }

        private void Map2DCoordsToCML(CMLAtom cmlAtom, IAtom cdkAtom)
        {
            if (cdkAtom.Point2D != null)
            {
                cmlAtom.X2 = cdkAtom.Point2D.Value.X;
                cmlAtom.Y2 = cdkAtom.Point2D.Value.Y;
            }
        }

        private void CheckPrefix(CMLElement element)
        {
            if (this.prefix != null)
            {
                this.prefix.Trim();
                if (this.prefix.Length == 0) prefix = null;
            }
            if (this.prefix != null) element.Name = (XNamespace)(this.prefix) + element.Name.LocalName;
        }
    }
}
