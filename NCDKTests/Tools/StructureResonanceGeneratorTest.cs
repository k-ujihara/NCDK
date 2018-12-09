/* Copyright (C) 2004-2007  Miguel Rojas <miguel.rojas@uni-koeln.de>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Aromaticities;
using NCDK.Silent;
using NCDK.Isomorphisms;
using NCDK.Isomorphisms.Matchers;
using NCDK.Reactions;
using NCDK.Reactions.Types;
using NCDK.Reactions.Types.Parameters;
using NCDK.Templates;
using NCDK.Tools.Manipulator;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.Tools
{
    // @cdk.module test-reaction
    [TestClass()]
    public class StructureResonanceGeneratorTest : CDKTestCase
    {
        private readonly static IChemObjectBuilder builder = CDK.Builder;

        /// <summary>
        /// Constructor of the StructureResonanceGeneratorTest.
        /// </summary>
        public StructureResonanceGeneratorTest()
                : base()
        { }

        [TestMethod()]
        public void TestStructureResonanceGenerator()
        {
            Assert.IsNotNull(new StructureResonanceGenerator());
        }

        [TestMethod()]
        public void TestStructureResonanceGenerator_boolean()
        {
            Assert.IsNotNull(new StructureResonanceGenerator(true));
        }

        [TestMethod()]
        public void TestGetReactions()
        {
            Assert.IsNotNull(new StructureResonanceGenerator().Reactions);
        }

        [TestMethod()]
        public void TestSetDefaultReactions()
        {
            StructureResonanceGenerator sRG = new StructureResonanceGenerator();

            var reactionList = sRG.Reactions;
            Assert.IsNotNull(reactionList);

            Assert.AreEqual(6, reactionList.Count);

            SharingLonePairReaction slReaction = (SharingLonePairReaction)reactionList[0];
            Assert.AreEqual(1, slReaction.ParameterList.Count);
            var objects = slReaction.ParameterList;
            foreach (var obj in objects)
            {
                if (obj is SetReactionCenter) Assert.IsFalse(obj.IsSetParameter);
            }

            PiBondingMovementReaction pBReaction = (PiBondingMovementReaction)reactionList[1];
            Assert.AreEqual(1, pBReaction.ParameterList.Count);
            objects = pBReaction.ParameterList;
            foreach (var obj in objects)
            {
                if (obj is SetReactionCenter) Assert.IsFalse(obj.IsSetParameter);
            }

            RearrangementAnionReaction raReaction = (RearrangementAnionReaction)reactionList[2];
            Assert.AreEqual(1, raReaction.ParameterList.Count);
            objects = raReaction.ParameterList;
            foreach (var obj in objects)
            {
                if (obj is SetReactionCenter) Assert.IsFalse(obj.IsSetParameter);
            }

            RearrangementCationReaction rcReaction = (RearrangementCationReaction)reactionList[3];
            Assert.AreEqual(1, rcReaction.ParameterList.Count);
            objects = rcReaction.ParameterList;
            foreach (var obj in objects)
            {
                if (obj is SetReactionCenter) Assert.IsFalse(obj.IsSetParameter);
            }

            RearrangementLonePairReaction lnReaction = (RearrangementLonePairReaction)reactionList[4];
            Assert.AreEqual(1, lnReaction.ParameterList.Count);
            objects = lnReaction.ParameterList;
            foreach (var obj in objects)
            {
                if (obj is SetReactionCenter) Assert.IsFalse(obj.IsSetParameter);
            }

            RearrangementRadicalReaction rrReaction = (RearrangementRadicalReaction)reactionList[5];
            Assert.AreEqual(1, rrReaction.ParameterList.Count);
            objects = rrReaction.ParameterList;
            foreach (var obj in objects)
            {
                if (obj is SetReactionCenter) Assert.IsFalse(obj.IsSetParameter);
            }

        }

        [TestMethod()]
        public void TestSetReactions_List()
        {
            StructureResonanceGenerator sRG = new StructureResonanceGenerator();
            var reactionList = sRG.Reactions;
            Assert.IsNotNull(reactionList);

            Assert.AreEqual(6, reactionList.Count);

            // put only one reaction more.
            List<IReactionProcess> newReactionList = new List<IReactionProcess>();

            IReactionProcess reaction = new HyperconjugationReaction();
            newReactionList.Add(reaction);

            sRG.Reactions = newReactionList;

            Assert.AreEqual(1, sRG.Reactions.Count);

        }

        //
        //    /// <summary>
        //    /// <p>A unit test suite: Resonance - CC(=[O*+])C=O</p>
        //    /// <p>CC(=[O*+])C=O &lt;=&gt; C[C+]([O*])C=O &lt;=&gt; CC([O*])=CO &lt;=&gt; CC(=O)[C*][O+] &lt;=&gt; CC(=O)C=[O*+]</p>
        //    ///
        //    /// <returns>The test suite</returns>
        //    /// </summary>
        //    [TestMethod()] public void TestGetAllStructures_IAtomContainer() {
        //        IAtomContainer molecule = CDK.SilentSmilesParser.ParseSmiles("CC(=O)C=O");
        //        AddExplicitHydrogens(molecule);
        //        AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
        //        CDK.LonePairElectronChecker.Saturate(molecule);
        //
        //        IAtom atom =  molecule.Atoms[2];
        //        molecule.SingleElectrons.Add(new SingleElectron(atom));
        //        atom.FormalCharge = 1;
        //        List<ILonePair> selectron = molecule.GetConnectedLonePairsList(atom);
        //        molecule.RemoveLonePair(selectron[0]);
        //        SmilesGenerator sg = new SmilesGenerator();
        //        Console.Out.WriteLine("> "+sg.CreateSMILES(molecule));
        //        MakeSureAtomTypesAreRecognized(molecule);
        //
        //        StructureResonanceGenerator gRI = new StructureResonanceGenerator(true,true,true,true,false,false,-1);
        //        var setOfMolecules = gRI.GetAllStructures(molecule);
        //        For(int i = 0; i < setOfMolecules.Count; i++)
        //            Console.Out.WriteLine("> "+sg.CreateSMILES((IAtomContainer) setOfMolecules[i]));
        //
        //
        //        Iterator<IAtomContainer> containers = setOfMolecules.AtomContainers();
        //        SmilesGenerator smiGen = new SmilesGenerator();
        //        while (containers.HasNext()) {
        //            Console.Out.WriteLine(smiGen.CreateSMILES(new AtomContainer(containers.Next())));
        //        }
        //        Assert.AreEqual(8,setOfMolecules.Count);
        //
        //        /*1*/
        //        IAtomContainer molecule1 = CDK.SilentSmilesParser.ParseSmiles("C[C+](O)C=O");
        //        For(int i = 0; i < 4; i++)
        //            molecule1.Atoms.Add(new Atom("H"));
        //        molecule1.AddBond(molecule1.Atoms[0], molecule1.Atoms[5], BondOrder.Single);
        //        molecule1.AddBond(molecule1.Atoms[0], molecule1.Atoms[6], BondOrder.Single);
        //        molecule1.AddBond(molecule1.Atoms[0], molecule1.Atoms[7], BondOrder.Single);
        //        molecule1.AddBond(molecule1.Atoms[3], molecule1.Atoms[8], BondOrder.Single);
        //        CDK.LonePairElectronChecker.Saturate(molecule1);
        //        IAtom atom1 =  molecule1.Atoms[2];
        //        molecule1.SingleElectrons.Add(new SingleElectron(atom1));
        //        QueryAtomContainer qAC = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(molecule1);
        //        Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(setOfMolecules[1],qAC));
        //
        ////        /*2*/
        ////        Molecule molecule2 = (new SmilesParser()).ParseSmiles("CC(O)=CO");
        ////        For(int i = 0; i < 4; i++)
        ////            molecule2.Atoms.Add(new Atom("H"));
        ////        molecule2.AddBond(molecule2.Atoms[0], molecule2.Atoms[5], BondOrder.Single);
        ////        molecule2.AddBond(molecule2.Atoms[0], molecule2.Atoms[6], BondOrder.Single);
        ////        molecule2.AddBond(molecule2.Atoms[0], molecule2.Atoms[7], BondOrder.Single);
        ////        molecule2.AddBond(molecule2.Atoms[3], molecule2.Atoms[8], BondOrder.Single);
        ////        CDK.LonePairElectronChecker.NewSaturate(molecule2);
        ////        IAtom atom2a =  molecule2.Atoms[2];
        ////        molecule2.Add(new SingleElectron(atom2a));
        ////
        ////        IAtom atom2b =  molecule2.Atoms[4];
        ////        atom2b.SetHydrogenCount(0);
        ////        atom2b.FormalCharge = 1;
        ////
        ////        qAC = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(molecule2);
        ////        Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(setOfMolecules[3],qAC));
        //    }
        //
        //    private void MakeSureAtomTypesAreRecognized(IAtomContainer molecule)
        //            {
        //        Iterator<IAtom> atoms = molecule.Atoms();
        //        CDKAtomTypeMatcher matcher = CDK.CdkAtomTypeMatcher);
        //        while (atoms.HasNext()) {
        //            IAtom nextAtom = atoms.Next();
        //            Assert.IsNotNull(
        //                "Missing atom type for: " + nextAtom,
        //                matcher.FindMatchingAtomType(molecule, nextAtom)
        //            );
        //        }
        //    }
        //    /// <summary>
        //    /// A unit test suite: Resonance CC(=[O*+])C=O &lt;=&gt; CC(=O)C=[O*+]
        //    ///
        //    /// <returns>The test suite</returns>
        //    /// </summary>
        //    [TestMethod()] public void TestGetStructures_IAtomContainer() {
        //        IAtomContainer molecule = CDK.SilentSmilesParser.ParseSmiles("CC(=O)C=O");
        //        AddExplicitHydrogens(molecule);
        //        AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
        //        CDK.LonePairElectronChecker.Saturate(molecule);
        //
        //        IAtom atom =  molecule.Atoms[2];
        //        molecule.SingleElectrons.Add(new SingleElectron(atom));
        //        atom.FormalCharge = 1;
        //        List<ILonePair> selectron = molecule.GetConnectedLonePairsList(atom);
        //        molecule.RemoveLonePair(selectron[selectron.Count-1]);
        //        MakeSureAtomTypesAreRecognized(molecule);
        //
        //        StructureResonanceGenerator gRI = new StructureResonanceGenerator();
        //        var setOfMolecules = gRI.GetStructures(molecule);
        //
        //        Assert.AreEqual(2,setOfMolecules.Count);
        //
        //        IAtomContainer molecule1 = CDK.SilentSmilesParser.ParseSmiles("CC(=O)C=O");
        //        AddExplicitHydrogens(molecule1);
        //        CDK.LonePairElectronChecker.Saturate(molecule1);
        //        IAtom atom1 =  molecule1.Atoms[4];
        //        molecule1.SingleElectrons.Add(new SingleElectron(atom1));
        //        selectron = molecule1.GetConnectedLonePairsList(atom1);
        //        molecule1.RemoveLonePair((ILonePair)selectron[0]);
        //        atom1.FormalCharge = 1;
        //
        //
        //        QueryAtomContainer qAC = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(molecule1);
        //        Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(setOfMolecules[1],qAC));
        //
        //    }
        //    /// <summary>
        //    /// A unit test suite: Resonance CCC(=[O*+])C(C)=O &lt;=&gt; CCC(=O)C(C)=[O*+]
        //    ///
        //    /// <returns>The test suite</returns>
        //    /// </summary>
        //    [TestMethod()] public void TestGetStructures2() {
        //        IAtomContainer molecule = CDK.SilentSmilesParser.ParseSmiles("CCC(=O)C(C)=O");
        //        AddExplicitHydrogens(molecule);
        //        AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
        //        CDK.LonePairElectronChecker.Saturate(molecule);
        //
        //        IAtom atom =  molecule.Atoms[3];
        //        molecule.SingleElectrons.Add(new SingleElectron(atom));
        //        atom.FormalCharge = 1;
        //        List<ILonePair> selectron = molecule.GetConnectedLonePairsList(atom);
        //        molecule.RemoveLonePair(selectron[0]);
        //        MakeSureAtomTypesAreRecognized(molecule);
        //
        //        StructureResonanceGenerator gRI = new StructureResonanceGenerator();
        //        var setOfMolecules = gRI.GetStructures(molecule);
        //
        //        Assert.AreEqual(2,setOfMolecules.Count);
        //
        //        IAtomContainer molecule1 = CDK.SilentSmilesParser.ParseSmiles("CCC(=O)C(C)=O");
        //        AddExplicitHydrogens(molecule1);
        //        CDK.LonePairElectronChecker.Saturate(molecule1);
        //
        //        IAtom atom1 =  molecule1.Atoms[6];
        //        molecule1.SingleElectrons.Add(new SingleElectron(atom1));
        //        atom1.FormalCharge = 1;
        //        selectron = molecule.GetConnectedLonePairsList(atom);
        //        molecule.RemoveLonePair((ILonePair)selectron[0]);
        //
        //        QueryAtomContainer qAC = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(molecule1);
        //        Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(setOfMolecules[1],qAC));
        //
        //    }

        /// <summary>
        /// A unit test suite: Resonance C-C=C-[C+]-C-C=C-[C+] &lt;=&gt; C-[C+]-C=C-C-C=C-[C+] +
        /// C-C=C-[C+]-C-[C+]-C=C + C-[C+]-C=C-C-[C+]-C=C
        /// </summary>
        [TestMethod()]
        public void TestGetStructures_IAtomContainer()
        {
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(new Atom("C"));
            molecule.Atoms.Add(new Atom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(new Atom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);
            molecule.Atoms.Add(new Atom("C"));
            molecule.Atoms[3].FormalCharge = +1;
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);
            molecule.Atoms.Add(new Atom("C"));
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Single);
            molecule.Atoms.Add(new Atom("C"));
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Single);
            molecule.Atoms.Add(new Atom("C"));
            molecule.AddBond(molecule.Atoms[5], molecule.Atoms[6], BondOrder.Double);
            molecule.Atoms.Add(new Atom("C"));
            molecule.AddBond(molecule.Atoms[6], molecule.Atoms[7], BondOrder.Single);
            molecule.Atoms[7].FormalCharge = +1;
            AddExplicitHydrogens(molecule);

            StructureResonanceGenerator sRG = new StructureResonanceGenerator();
            var setOfMolecules = sRG.GetStructures(molecule);

            Assert.AreEqual(4, setOfMolecules.Count);

        }

        /// <summary>
        /// A unit test suite: Resonance C-C=C-[C+]-C-C=C-[C+] &lt;=&gt; C-[C+]-C=C-C-C=C-[C+]
        /// </summary>
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestFlagActiveCenter1()
        {
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(new Atom("C"));
            molecule.Atoms.Add(new Atom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(new Atom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);
            molecule.Atoms.Add(new Atom("C"));
            molecule.Atoms[3].FormalCharge = +1;
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);
            molecule.Atoms.Add(new Atom("C"));
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Single);
            molecule.Atoms.Add(new Atom("C"));
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Single);
            molecule.Atoms.Add(new Atom("C"));
            molecule.AddBond(molecule.Atoms[5], molecule.Atoms[6], BondOrder.Double);
            molecule.Atoms.Add(new Atom("C"));
            molecule.AddBond(molecule.Atoms[6], molecule.Atoms[7], BondOrder.Single);
            molecule.Atoms[7].FormalCharge = +1;
            AddExplicitHydrogens(molecule);

            molecule.Atoms[1].IsReactiveCenter = true;
            molecule.Bonds[1].IsReactiveCenter = true;
            molecule.Atoms[2].IsReactiveCenter = true;
            molecule.Bonds[2].IsReactiveCenter = true;
            molecule.Atoms[3].IsReactiveCenter = true;

            var paramList = new List<IParameterReaction>();
            IParameterReaction param = new SetReactionCenter { IsSetParameter = true };
            paramList.Add(param);

            StructureResonanceGenerator sRG = new StructureResonanceGenerator();
            foreach (var reaction in sRG.Reactions)
            {
                reaction.ParameterList = paramList;
            }

            var setOfMolecules = sRG.GetStructures(molecule);

            Assert.AreEqual(2, setOfMolecules.Count);

            IAtomContainer molecule2 = builder.NewAtomContainer();
            molecule2.Atoms.Add(new Atom("C"));
            molecule2.Atoms.Add(new Atom("C"));
            molecule2.Atoms[1].FormalCharge = +1;
            molecule2.AddBond(molecule2.Atoms[0], molecule2.Atoms[1], BondOrder.Single);
            molecule2.Atoms.Add(new Atom("C"));
            molecule2.AddBond(molecule2.Atoms[1], molecule2.Atoms[2], BondOrder.Single);
            molecule2.Atoms.Add(new Atom("C"));
            molecule2.AddBond(molecule2.Atoms[2], molecule2.Atoms[3], BondOrder.Double);
            molecule2.Atoms.Add(new Atom("C"));
            molecule2.AddBond(molecule2.Atoms[3], molecule2.Atoms[4], BondOrder.Single);
            molecule2.Atoms.Add(new Atom("C"));
            molecule2.AddBond(molecule2.Atoms[4], molecule2.Atoms[5], BondOrder.Single);
            molecule2.Atoms.Add(new Atom("C"));
            molecule2.AddBond(molecule2.Atoms[5], molecule2.Atoms[6], BondOrder.Double);
            molecule2.Atoms.Add(new Atom("C"));
            molecule2.AddBond(molecule2.Atoms[6], molecule2.Atoms[7], BondOrder.Single);
            molecule2.Atoms[7].FormalCharge = +1;
            AddExplicitHydrogens(molecule2);

            QueryAtomContainer qAC = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(molecule2);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(setOfMolecules[1], qAC));
        }

        /// <summary>
        /// A unit test suite: Resonance C-C=C-[C-] &lt;=&gt; C=C-[C-]-C
        /// </summary>
        [TestMethod()]
        public void TesttestGetStructures2()
        {
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);
            molecule.Atoms[3].FormalCharge = -1;
            molecule.LonePairs.Add(new LonePair(molecule.Atoms[3]));
            AddExplicitHydrogens(molecule);

            StructureResonanceGenerator gR = new StructureResonanceGenerator();
            var setOfMolecules = gR.GetStructures(molecule);

            Assert.AreEqual(2, setOfMolecules.Count);

            IAtomContainer molecule2 = builder.NewAtomContainer();
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.AddBond(molecule2.Atoms[0], molecule2.Atoms[1], BondOrder.Double);
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.AddBond(molecule2.Atoms[1], molecule2.Atoms[2], BondOrder.Single);
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.AddBond(molecule2.Atoms[2], molecule2.Atoms[3], BondOrder.Single);
            molecule2.Atoms[2].FormalCharge = -1;
            molecule2.LonePairs.Add(new LonePair(molecule2.Atoms[2]));
            AddExplicitHydrogens(molecule2);

            QueryAtomContainer qAC = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(molecule2);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(setOfMolecules[1], qAC));
        }

        /// <summary>
        /// A unit test suite: Resonance Formic acid  C(=O)O &lt;=&gt; [C+](-[O-])O &lt;=&gt; C([O-])=[O+]
        /// </summary>
        //  @cdk.inchi InChI=1/CH2O2/c2-1-3/h1H,(H,2,3)/f/h2H
        [TestMethod()]
        public void TestFormicAcid()
        {
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("O"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("O"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[2], BondOrder.Single);
            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            var gR = new StructureResonanceGenerator();
            var reactionList = gR.Reactions.ToList();
            reactionList.Add(new HeterolyticCleavagePBReaction());
            gR.Reactions = reactionList;
            var setOfMolecules = gR.GetStructures(molecule);

            Assert.AreEqual(3, setOfMolecules.Count);

            IAtomContainer molecule2 = builder.NewAtomContainer();
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.Atoms.Add(builder.NewAtom("O"));
            molecule2.Atoms[1].FormalCharge = -1;
            molecule2.AddBond(molecule2.Atoms[0], molecule2.Atoms[1], BondOrder.Single);
            molecule2.Atoms.Add(builder.NewAtom("O"));
            molecule2.Atoms[2].FormalCharge = 1;
            molecule2.AddBond(molecule2.Atoms[0], molecule2.Atoms[2], BondOrder.Double);
            AddExplicitHydrogens(molecule2);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule2);
            CDK.LonePairElectronChecker.Saturate(molecule2);

            QueryAtomContainer qAC = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(molecule2);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(setOfMolecules[1], qAC));
        }

        /// <summary>
        /// A unit test suite: Resonance Formic acid  F-C=C &lt;=&gt; [F+]=C-[C-]
        /// </summary>
        //  @cdk.inchi InChI=1/C2H3F/c1-2-3/h2H,1H2
        [TestMethod()]
        public void TestFluoroethene()
        {
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("F"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);
            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            StructureResonanceGenerator gR = new StructureResonanceGenerator();
            var setOfMolecules = gR.GetStructures(molecule);

            Assert.AreEqual(2, setOfMolecules.Count);

            IAtomContainer molecule1 = builder.NewAtomContainer();
            molecule1.Atoms.Add(builder.NewAtom("F"));
            molecule1.Atoms.Add(builder.NewAtom("C"));
            molecule1.AddBond(molecule1.Atoms[0], molecule1.Atoms[1], BondOrder.Double);
            molecule1.Atoms.Add(builder.NewAtom("C"));
            molecule1.AddBond(molecule1.Atoms[1], molecule1.Atoms[2], BondOrder.Single);
            molecule1.Atoms[0].FormalCharge = +1; // workaround for bug #1875949
            molecule1.Atoms[2].FormalCharge = -1;
            AddExplicitHydrogens(molecule1);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule1);
            CDK.LonePairElectronChecker.Saturate(molecule1);

            QueryAtomContainer qAC = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(molecule1);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(setOfMolecules[1], qAC));
        }

        /// <summary>
        /// A unit test suite: Resonance Fluorobenzene  Fc1ccccc1 &lt;=&gt; ...
        /// </summary>
        // @cdk.inchi InChI=1/C6H5F/c7-6-4-2-1-3-5-6/h1-5H
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestFluorobenzene()
        {
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("F"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[5], molecule.Atoms[6], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[6], molecule.Atoms[1], BondOrder.Single);

            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            StructureResonanceGenerator gRI = new StructureResonanceGenerator();
            var setOfMolecules = gRI.GetStructures(molecule);

            Assert.AreEqual(5, setOfMolecules.Count);

            IAtomContainer molecule1 = builder.NewAtomContainer();
            molecule1.Atoms.Add(builder.NewAtom("F"));
            molecule1.Atoms[0].FormalCharge = 1;
            molecule1.Atoms.Add(builder.NewAtom("C"));
            molecule1.AddBond(molecule1.Atoms[0], molecule1.Atoms[1], BondOrder.Double);
            molecule1.Atoms.Add(builder.NewAtom("C"));
            molecule1.Atoms[2].FormalCharge = -1;
            molecule1.AddBond(molecule1.Atoms[1], molecule1.Atoms[2], BondOrder.Single);
            molecule1.Atoms.Add(builder.NewAtom("C"));
            molecule1.AddBond(molecule1.Atoms[2], molecule1.Atoms[3], BondOrder.Single);
            molecule1.Atoms.Add(builder.NewAtom("C"));
            molecule1.AddBond(molecule1.Atoms[3], molecule1.Atoms[4], BondOrder.Double);
            molecule1.Atoms.Add(builder.NewAtom("C"));
            molecule1.AddBond(molecule1.Atoms[4], molecule1.Atoms[5], BondOrder.Single);
            molecule1.Atoms.Add(builder.NewAtom("C"));
            molecule1.AddBond(molecule1.Atoms[5], molecule1.Atoms[6], BondOrder.Double);
            molecule1.AddBond(molecule1.Atoms[6], molecule1.Atoms[1], BondOrder.Single);
            AddExplicitHydrogens(molecule1);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule1);
            CDK.LonePairElectronChecker.Saturate(molecule1);

            QueryAtomContainer qAC = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(molecule1);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(setOfMolecules[2], qAC));

            IAtomContainer molecule2 = builder.NewAtomContainer();
            molecule2.Atoms.Add(builder.NewAtom("F"));
            molecule2.Atoms[0].FormalCharge = 1;
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.AddBond(molecule2.Atoms[0], molecule2.Atoms[1], BondOrder.Double);
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.AddBond(molecule2.Atoms[1], molecule2.Atoms[2], BondOrder.Single);
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.AddBond(molecule2.Atoms[2], molecule2.Atoms[3], BondOrder.Double);
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.AddBond(molecule2.Atoms[3], molecule2.Atoms[4], BondOrder.Single);
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.Atoms[4].FormalCharge = -1;
            molecule2.AddBond(molecule2.Atoms[4], molecule2.Atoms[5], BondOrder.Single);
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.AddBond(molecule2.Atoms[5], molecule2.Atoms[6], BondOrder.Double);
            molecule2.AddBond(molecule2.Atoms[6], molecule2.Atoms[1], BondOrder.Single);
            AddExplicitHydrogens(molecule2);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule2);
            CDK.LonePairElectronChecker.Saturate(molecule2);

            IAtomContainer product2 = setOfMolecules[4];
            qAC = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(molecule2);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(product2, qAC));
        }

        /// <summary>
        /// A unit test suite: Resonance Fluorobenzene  Fc1ccccc1 &lt;=&gt; ...
        /// </summary>
        // @cdk.inchi InChI=1/C6H5F/c7-6-4-2-1-3-5-6/h1-5H
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestFluorobenzeneContainer()
        {
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("F"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[5], molecule.Atoms[6], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[6], molecule.Atoms[1], BondOrder.Single);

            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            StructureResonanceGenerator gRI = new StructureResonanceGenerator();
            IAtomContainer container = gRI.GetContainer(molecule, molecule.Atoms[0]);

            Assert.AreEqual(7, container.Atoms.Count);
        }

        /// <summary>
        /// A unit test suite: Resonance Fluorobenzene  Fc1ccccc1 &lt;=&gt; ...
        /// </summary>
        // @cdk.inchi InChI=1/C6H5F/c7-6-4-2-1-3-5-6/h1-5H
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestFluorobenzene_symm()
        {
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("F"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[5], molecule.Atoms[6], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[6], molecule.Atoms[1], BondOrder.Single);

            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            StructureResonanceGenerator gRI = new StructureResonanceGenerator(true);
            var setOfMolecules = gRI.GetStructures(molecule);

            Assert.AreEqual(3, setOfMolecules.Count);

            IAtomContainer molecule1 = builder.NewAtomContainer();
            molecule1.Atoms.Add(builder.NewAtom("F"));
            molecule1.Atoms[0].FormalCharge = 1;
            molecule1.Atoms.Add(builder.NewAtom("C"));
            molecule1.AddBond(molecule1.Atoms[0], molecule1.Atoms[1], BondOrder.Double);
            molecule1.Atoms.Add(builder.NewAtom("C"));
            molecule1.Atoms[2].FormalCharge = -1;
            molecule1.AddBond(molecule1.Atoms[1], molecule1.Atoms[2], BondOrder.Single);
            molecule1.Atoms.Add(builder.NewAtom("C"));
            molecule1.AddBond(molecule1.Atoms[2], molecule1.Atoms[3], BondOrder.Single);
            molecule1.Atoms.Add(builder.NewAtom("C"));
            molecule1.AddBond(molecule1.Atoms[3], molecule1.Atoms[4], BondOrder.Double);
            molecule1.Atoms.Add(builder.NewAtom("C"));
            molecule1.AddBond(molecule1.Atoms[4], molecule1.Atoms[5], BondOrder.Single);
            molecule1.Atoms.Add(builder.NewAtom("C"));
            molecule1.AddBond(molecule1.Atoms[5], molecule1.Atoms[6], BondOrder.Double);
            molecule1.AddBond(molecule1.Atoms[6], molecule1.Atoms[1], BondOrder.Single);
            AddExplicitHydrogens(molecule1);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule1);
            CDK.LonePairElectronChecker.Saturate(molecule1);

            QueryAtomContainer qAC = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(molecule1);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(setOfMolecules[1], qAC));

            IAtomContainer molecule2 = builder.NewAtomContainer();
            molecule2.Atoms.Add(builder.NewAtom("F"));
            molecule2.Atoms[0].FormalCharge = 1;
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.AddBond(molecule2.Atoms[0], molecule2.Atoms[1], BondOrder.Double);
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.AddBond(molecule2.Atoms[1], molecule2.Atoms[2], BondOrder.Single);
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.AddBond(molecule2.Atoms[2], molecule2.Atoms[3], BondOrder.Double);
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.AddBond(molecule2.Atoms[3], molecule2.Atoms[4], BondOrder.Single);
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.Atoms[4].FormalCharge = -1;
            molecule2.AddBond(molecule2.Atoms[4], molecule2.Atoms[5], BondOrder.Single);
            molecule2.Atoms.Add(builder.NewAtom("C"));
            molecule2.AddBond(molecule2.Atoms[5], molecule2.Atoms[6], BondOrder.Double);
            molecule2.AddBond(molecule2.Atoms[6], molecule2.Atoms[1], BondOrder.Single);
            AddExplicitHydrogens(molecule2);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule2);
            CDK.LonePairElectronChecker.Saturate(molecule2);

            IAtomContainer product2 = setOfMolecules[2];
            qAC = QueryAtomContainerCreator.CreateSymbolAndChargeQueryContainer(molecule2);
            Assert.IsTrue(new UniversalIsomorphismTester().IsIsomorph(product2, qAC));

        }

        /// <summary>
        /// A unit test suite: Resonance   n1ccccc1 &lt;=&gt; ...
        /// </summary>
        // @cdk.inchi InChI=1/C6H7N/c7-6-4-2-1-3-5-6/h1-5H,7H2
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestAniline()
        {
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("N"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[5], molecule.Atoms[6], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[6], molecule.Atoms[1], BondOrder.Single);
            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            StructureResonanceGenerator gRI = new StructureResonanceGenerator();
            var setOfMolecules = gRI.GetStructures(molecule);

            Assert.AreEqual(5, setOfMolecules.Count);
        }

        /// <summary>
        /// A unit test suite: Resonance   n1ccccc1 &lt;=&gt; ...
        /// </summary>
        // @cdk.inchi InChI=1/C6H7N/c7-6-4-2-1-3-5-6/h1-5H,7H2
        [TestMethod()]
        public void TestAniline_Symm()
        {
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("N"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[5], molecule.Atoms[6], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[6], molecule.Atoms[1], BondOrder.Single);
            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            StructureResonanceGenerator gRI = new StructureResonanceGenerator(true);
            var setOfMolecules = gRI.GetStructures(molecule);

            Assert.AreEqual(3, setOfMolecules.Count);
        }

        /// <summary>
        /// ClC([H])=C([H])[C+]([H])[H] => [H]C([H])=C([H])[C+](Cl)[H] +
        /// Cl=C([H])[C-]([H])[C+]([H])[H] + Cl=C([H])C([H])=C([H])[H]
        /// </summary>
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestAllyl()
        {
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("Cl")); // to remove symmetry :)
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);
            molecule.Atoms[0].FormalCharge = +1;
            AddExplicitHydrogens(molecule);
            Assert.AreEqual(8, molecule.Atoms.Count);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            StructureResonanceGenerator gRI = new StructureResonanceGenerator();
            var resonanceStructures = gRI.GetStructures(molecule);

            Assert.AreEqual(4, resonanceStructures.Count);
        }

        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestAllylRadical()
        {
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms[0].FormalCharge = 1;
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C")); // to remove symmetry :)
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);
            AddExplicitHydrogens(molecule);
            molecule.Atoms[0].FormalCharge = 0;
            molecule.SingleElectrons.Add(new SingleElectron(molecule.Atoms[0]));
            Assert.AreEqual(11, molecule.Atoms.Count);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);

            StructureResonanceGenerator gRI = new StructureResonanceGenerator();
            var resonanceStructures = gRI.GetStructures(molecule);
            Assert.AreEqual(2, resonanceStructures.Count);
        }

        /// <summary>
        /// [H]C([H])=C([H])[O-] => O=C([H])[C-]([H])[H]
        /// </summary>
        // @cdk.inchi InChI=1/C2H4O/c1-2-3/h2-3H,1H2/p-1/fC2H3O/h3h/q-1
        [TestMethod()]
        public void TestEthenolate()
        {
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("O"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);
            molecule.Atoms[0].FormalCharge = -1;
            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);
            Assert.AreEqual(6, molecule.Atoms.Count);

            StructureResonanceGenerator gRI = new StructureResonanceGenerator();
            var resonanceStructures = gRI.GetStructures(molecule);

            Assert.AreEqual(2, resonanceStructures.Count);
        }

        /// <summary>
        /// [H]N([H])C1=C([H])C([H])=C([H])C([H])=C1C([H])([H])[H] =>
        ///  + [H]C=1C([H])=C(C(=[N+]([H])[H])[C-]([H])C=1([H]))C([H])([H])[H]
        ///  + [H]C1=C([H])[C-]([H])C([H])=C(C1=[N+]([H])[H])C([H])([H])[H]
        ///  + [H]C=1C([H])=C([H])[C-](C(C=1([H]))=[N+]([H])[H])C([H])([H])[H]
        /// </summary>
        // @cdk.inchi InChI=1/C7H9N/c1-6-4-2-3-5-7(6)8/h2-5H,8H2,1H3
        [TestMethod()]
        public void Test2Methylaniline()
        {
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("N"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[5], molecule.Atoms[0], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[6], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[7], BondOrder.Single);
            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            Assert.AreEqual(17, molecule.Atoms.Count);

            StructureResonanceGenerator gRI = new StructureResonanceGenerator(true);
            var resonanceStructures = gRI.GetStructures(molecule);

            Assert.AreEqual(4, resonanceStructures.Count);
        }

        // @cdk.inchi InChI=1/C8H10/c1-7-5-3-4-6-8(7)2/h3-6H,1-2H3
        [TestMethod()]
        public void Test12DimethylBenzene()
        {
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[5], molecule.Atoms[0], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[6], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[7], BondOrder.Single);

            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            Assert.AreEqual(18, molecule.Atoms.Count);

            StructureResonanceGenerator gRI = new StructureResonanceGenerator();
            // put only one reaction more.
            List<IReactionProcess> newReactionList = new List<IReactionProcess>();
            IReactionProcess reaction = new PiBondingMovementReaction();
            newReactionList.Add(reaction);

            gRI.Reactions = newReactionList;

            var resonanceStructures = gRI.GetStructures(molecule);

            Assert.AreEqual(2, resonanceStructures.Count);
        }

        /// <summary>
        /// A unit test suite: Resonance Fluorobenzene  Fc1ccccc1 &lt;=&gt; ...
        /// </summary>
        // @cdk.inchi InChI=1/C6H5F/c7-6-4-2-1-3-5-6/h1-5H
        [TestMethod()]
        public void TestPreservingAromaticity()
        {
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("F"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[5], molecule.Atoms[6], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[6], molecule.Atoms[1], BondOrder.Single);

            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            bool isAromatic = Aromaticity.CDKLegacy.Apply(molecule);
            Assert.IsTrue(isAromatic, "Molecule is expected to be marked aromatic!");

            Assert.IsTrue(molecule.Bonds[1].IsAromatic, "Bond is expected to be marked aromatic!");
            Assert.IsTrue(molecule.Bonds[2].IsAromatic, "Bond is expected to be marked aromatic!");
            Assert.IsTrue(molecule.Bonds[3].IsAromatic, "Bond is expected to be marked aromatic!");
            Assert.IsTrue(molecule.Bonds[4].IsAromatic, "Bond is expected to be marked aromatic!");
            Assert.IsTrue(molecule.Bonds[5].IsAromatic, "Bond is expected to be marked aromatic!");
            Assert.IsTrue(molecule.Bonds[6].IsAromatic, "Bond is expected to be marked aromatic!");

            StructureResonanceGenerator gRI = new StructureResonanceGenerator(false);
            var setOfMolecules = gRI.GetStructures(molecule);

            Assert.AreEqual(5, setOfMolecules.Count);

            IAtomContainer prod1 = setOfMolecules[1];
            Assert.IsTrue(prod1.Bonds[1].IsAromatic, "Bond is expected to be marked aromatic!");
            Assert.IsTrue(prod1.Bonds[2].IsAromatic, "Bond is expected to be marked aromatic!");
            Assert.IsTrue(prod1.Bonds[3].IsAromatic, "Bond is expected to be marked aromatic!");
            Assert.IsTrue(prod1.Bonds[4].IsAromatic, "Bond is expected to be marked aromatic!");
            Assert.IsTrue(prod1.Bonds[5].IsAromatic, "Bond is expected to be marked aromatic!");
            Assert.IsTrue(prod1.Bonds[6].IsAromatic, "Bond is expected to be marked aromatic!");
            IAtomContainer prod2 = setOfMolecules[2];
            Assert.IsTrue(prod2.Bonds[1].IsAromatic, "Bond is expected to be marked aromatic!");
            Assert.IsTrue(prod2.Bonds[2].IsAromatic, "Bond is expected to be marked aromatic!");
            Assert.IsTrue(prod2.Bonds[3].IsAromatic, "Bond is expected to be marked aromatic!");
            Assert.IsTrue(prod2.Bonds[4].IsAromatic, "Bond is expected to be marked aromatic!");
            Assert.IsTrue(prod2.Bonds[5].IsAromatic, "Bond is expected to be marked aromatic!");
            Assert.IsTrue(prod2.Bonds[6].IsAromatic, "Bond is expected to be marked aromatic!");
            IAtomContainer prod3 = setOfMolecules[3];
            Assert.IsTrue(prod3.Bonds[1].IsAromatic, "Bond is expected to be marked aromatic!");
            Assert.IsTrue(prod3.Bonds[2].IsAromatic, "Bond is expected to be marked aromatic!");
            Assert.IsTrue(prod3.Bonds[3].IsAromatic, "Bond is expected to be marked aromatic!");
            Assert.IsTrue(prod3.Bonds[4].IsAromatic, "Bond is expected to be marked aromatic!");
            Assert.IsTrue(prod3.Bonds[5].IsAromatic, "Bond is expected to be marked aromatic!");
            Assert.IsTrue(prod3.Bonds[6].IsAromatic, "Bond is expected to be marked aromatic!");
        }

        [TestMethod()]
        public void TestCyclobutadiene()
        {
            // anti-aromatic
            IAtomContainer molecule = TestMoleculeFactory.MakeCyclobutadiene();
            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            StructureResonanceGenerator gRI = new StructureResonanceGenerator();
            var setOfMolecules = gRI.GetStructures(molecule);

            Assert.AreEqual(2, setOfMolecules.Count);
        }

        // @cdk.bug      1728830
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestBenzene()
        {
            IAtomContainer molecule = TestMoleculeFactory.MakeBenzene();
            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            StructureResonanceGenerator gRI = new StructureResonanceGenerator();
            var setOfMolecules = gRI.GetStructures(molecule);

            Assert.AreEqual(2, setOfMolecules.Count);
        }

        /// <summary>
        /// [H]C([H])=C([H])[O-] => OCC
        /// </summary>
        // @cdk.inchi InChI=1/C2H4O/c1-2-3/h2-3H,1H2/p-1/fC2H3O/h3h/q-1
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestGetContainers_IAtomContainer()
        {
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("O"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);
            molecule.Atoms[0].FormalCharge = -1;
            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);
            Assert.AreEqual(6, molecule.Atoms.Count);

            StructureResonanceGenerator gRI = new StructureResonanceGenerator();
            var containers = gRI.GetContainers(molecule);

            Assert.AreEqual(1, containers.Count);
            Assert.AreEqual(3, containers[0].Atoms.Count);
            Assert.AreEqual(2, containers[0].Bonds.Count);
        }

        /// <summary>
        /// A unit test suite: Resonance C-C=C-[C+]-C-C=C-[C+] &lt;=&gt; C-[C+]-C=C-C-C=C-[C+]
        /// </summary>
        [TestMethod()]
        public void TestGetContainers2Groups()
        {
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(new Atom("C"));
            molecule.Atoms.Add(new Atom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Bonds[0].Id = "bond_0";
            molecule.Atoms.Add(new Atom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);
            molecule.Bonds[1].Id = "bond_1";
            molecule.Atoms.Add(new Atom("C"));
            molecule.Atoms[3].FormalCharge = +1;
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);
            molecule.Bonds[2].Id = "bond_2";
            molecule.Atoms.Add(new Atom("C"));
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Single);
            molecule.Bonds[3].Id = "bond_3";
            molecule.Atoms.Add(new Atom("C"));
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Single);
            molecule.Bonds[4].Id = "bond_4";
            molecule.Atoms.Add(new Atom("C"));
            molecule.AddBond(molecule.Atoms[5], molecule.Atoms[6], BondOrder.Double);
            molecule.Bonds[5].Id = "bond_5";
            molecule.Atoms.Add(new Atom("C"));
            molecule.AddBond(molecule.Atoms[6], molecule.Atoms[7], BondOrder.Single);
            molecule.Bonds[6].Id = "bond_6";
            molecule.Atoms[7].FormalCharge = +1;
            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            StructureResonanceGenerator sRG = new StructureResonanceGenerator();
            var setOfContainers = sRG.GetContainers(molecule);

            Assert.AreEqual(2, setOfContainers.Count);
            for (int i = 0; i < 2; i++)
            {
                Assert.AreEqual(3, setOfContainers[i].Atoms.Count);
                Assert.AreEqual(2, setOfContainers[i].Bonds.Count);

            }
        }

        /// <summary>
        /// A unit test suite: Resonance C-C=C-[C+]-C-C=C-[C+] &lt;=&gt; C-[C+]-C=C-C-C=C-[C+]
        /// </summary>
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestGetContainer_IAtomContainer_IAtom()
        {
            IAtomContainer molecule = builder.NewAtomContainer();
            IAtom atom1 = builder.NewAtom("C");
            atom1.Id = "atom1";
            molecule.Atoms.Add(atom1);
            IAtom atom2 = builder.NewAtom("C");
            atom2.Id = "atom2";
            molecule.Atoms.Add(atom2);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            IAtom atom3 = builder.NewAtom("C");
            atom3.Id = "atom3";
            molecule.Atoms.Add(atom3);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);
            IAtom atom4 = builder.NewAtom("C");
            atom4.Id = "atom4";
            molecule.Atoms.Add(atom4);
            atom4.FormalCharge = +1;
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);
            molecule.Atoms.Add(new Atom("C"));
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Single);
            molecule.Atoms.Add(new Atom("C"));
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Single);
            molecule.Atoms.Add(new Atom("C"));
            molecule.AddBond(molecule.Atoms[5], molecule.Atoms[6], BondOrder.Double);
            molecule.Atoms.Add(new Atom("C"));
            molecule.Atoms[7].FormalCharge = +1;
            molecule.AddBond(molecule.Atoms[6], molecule.Atoms[7], BondOrder.Single);
            AddExplicitHydrogens(molecule);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            StructureResonanceGenerator sRG = new StructureResonanceGenerator();
            IAtomContainer container = sRG.GetContainer(molecule, atom4);

            Assert.AreEqual(3, container.Atoms.Count);
            Assert.AreEqual(2, container.Bonds.Count);
            Assert.IsTrue(container.Contains(atom4));

        }

        /// <summary>
        /// A unit test suite: Resonance C-C=C-[C+]-C-C=C-[C+] &lt;=&gt; C-[C+]-C=C-C-C=C-[C+]
        /// </summary>
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestGetContainer_IAtomContainer_IBond()
        {
            IAtomContainer molecule = builder.NewAtomContainer();
            IAtom atom1 = builder.NewAtom("C");
            atom1.Id = "atom1";
            molecule.Atoms.Add(atom1);
            IAtom atom2 = builder.NewAtom("C");
            atom2.Id = "atom2";
            molecule.Atoms.Add(atom2);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            IAtom atom3 = builder.NewAtom("C");
            atom3.Id = "atom3";
            molecule.Atoms.Add(atom3);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);
            IAtom atom4 = builder.NewAtom("C");
            atom4.Id = "atom4";
            molecule.Atoms.Add(atom4);
            atom4.FormalCharge = +1;
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);
            molecule.Atoms.Add(new Atom("C"));
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Single);
            molecule.Atoms.Add(new Atom("C"));
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Single);
            molecule.Atoms.Add(new Atom("C"));
            molecule.AddBond(molecule.Atoms[5], molecule.Atoms[6], BondOrder.Double);
            molecule.Atoms.Add(new Atom("C"));
            molecule.Atoms[7].FormalCharge = +1;
            molecule.AddBond(molecule.Atoms[6], molecule.Atoms[7], BondOrder.Single);
            AddExplicitHydrogens(molecule);

            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            StructureResonanceGenerator sRG = new StructureResonanceGenerator();
            IAtomContainer container = sRG.GetContainer(molecule, molecule.Bonds[1]);

            Assert.AreEqual(3, container.Atoms.Count);
            Assert.AreEqual(2, container.Bonds.Count);
            Assert.IsTrue(container.Contains(molecule.Bonds[1]));

        }

        /// <summary>
        /// A unit test suite: Resonance C-C=C-[C+]-C-C=C-[C+] &lt;=&gt; C-[C+]-C=C-C-C=C-[C+]
        /// </summary>
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestGetID()
        {
            IAtomContainer molecule = builder.NewAtomContainer();
            IAtom atom1 = builder.NewAtom("C");
            atom1.Id = "atom1";
            molecule.Atoms.Add(atom1);
            IAtom atom2 = builder.NewAtom("C");
            atom2.Id = "atom2";
            molecule.Atoms.Add(atom2);
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            IAtom atom3 = builder.NewAtom("C");
            atom3.Id = "atom3";
            molecule.Atoms.Add(atom3);
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);
            IAtom atom4 = builder.NewAtom("C");
            atom4.Id = "atom4";
            molecule.Atoms.Add(atom4);
            atom4.FormalCharge = +1;
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);
            IAtom atom5 = builder.NewAtom("C");
            atom5.Id = "atom5";
            molecule.Atoms.Add(atom5);
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Single);
            IAtom atom6 = builder.NewAtom("C");
            atom6.Id = "atom6";
            molecule.Atoms.Add(atom6);
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Single);
            IAtom atom7 = builder.NewAtom("C");
            atom7.Id = "atom7";
            molecule.Atoms.Add(atom7);
            molecule.AddBond(molecule.Atoms[5], molecule.Atoms[6], BondOrder.Double);
            IAtom atom8 = builder.NewAtom("C");
            atom8.Id = "atom8";
            molecule.Atoms.Add(atom8);
            atom8.FormalCharge = +1;
            molecule.AddBond(molecule.Atoms[6], molecule.Atoms[7], BondOrder.Single);
            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            StructureResonanceGenerator sRG = new StructureResonanceGenerator();
            IAtomContainer container = sRG.GetContainer(molecule, atom4);

            Assert.IsNotNull(atom2.Id);
            Assert.IsNotNull(atom3.Id);
            Assert.IsNotNull(atom4.Id);
            Assert.AreEqual(atom2.Id, container.Atoms[0].Id);
            Assert.AreEqual(atom3.Id, container.Atoms[1].Id);
            Assert.AreEqual(atom4.Id, container.Atoms[2].Id);

        }

        /// <summary>
        /// A unit test suite: Resonance 1-fluoro-2-methylbenzene  Fc1ccccc1C &lt;=&gt; Fc1ccccc1
        /// </summary>
        // @cdk.inchi  InChI=1/C7H7F/c1-6-4-2-3-5-7(6)8/h2-5H,1H3
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestGetContainersFluoromethylbenzene()
        {
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("F"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[5], molecule.Atoms[6], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[6], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[6], molecule.Atoms[7], BondOrder.Single);

            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            StructureResonanceGenerator gRI = new StructureResonanceGenerator();
            var setOfContainers = gRI.GetContainers(molecule);

            Assert.AreEqual(1, setOfContainers.Count);

            IAtomContainer container = setOfContainers[0];

            Assert.AreEqual(15, molecule.Atoms.Count);
            Assert.AreEqual(7, container.Atoms.Count);

            Assert.AreEqual(15, molecule.Bonds.Count);
            Assert.AreEqual(7, container.Bonds.Count);
        }

        /// <summary>
        /// A unit test suite: Resonance 1-fluoro-benzene  Fc1ccccc1C &lt;=&gt; Fc1ccccc1
        /// </summary>
        // @cdk.inchi InChI=1/C6H5F/c7-6-4-2-1-3-5-6/h1-5H
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestGetContainersFluorobenzene()
        {
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("F"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[5], molecule.Atoms[6], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[6], molecule.Atoms[1], BondOrder.Single);

            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            StructureResonanceGenerator gRI = new StructureResonanceGenerator();
            var setOfContainers = gRI.GetContainers(molecule);

            Assert.AreEqual(1, setOfContainers.Count);

            IAtomContainer container = setOfContainers[0];

            Assert.AreEqual(12, molecule.Atoms.Count);
            Assert.AreEqual(7, container.Atoms.Count);

            Assert.AreEqual(12, molecule.Bonds.Count);
            Assert.AreEqual(7, container.Bonds.Count);
        }

        /// <summary>
        /// A unit test suite: Resonance Formic acid  C-C(C)=C &lt;=&gt; [Cl+]=C(C)-[C-]
        /// </summary>
        // @cdk.inchi InChI=1/C3H5Cl/c1-3(2)4/h1H2,2H3
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void Test1Propene2chloro()
        {
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("Cl"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[3], BondOrder.Single);
            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            StructureResonanceGenerator gRI = new StructureResonanceGenerator();
            var setOfContainers = gRI.GetContainers(molecule);

            Assert.AreEqual(1, setOfContainers.Count);
            Assert.AreEqual(3, setOfContainers[0].Atoms.Count);
        }

        /// <summary>
        /// A unit test suite: COC1=CC=C(C=C1)Br
        /// </summary>
        // @cdk.inchi InChI=1/C7H7BrO/c1-9-7-4-2-6(8)3-5-7/h2-5H,1H3
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestBenzene1bromo4methoxy()
        {
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("F"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[5], molecule.Atoms[6], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[6], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("O"));
            molecule.AddBond(molecule.Atoms[6], molecule.Atoms[7], BondOrder.Single);

            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            StructureResonanceGenerator gRI = new StructureResonanceGenerator();
            var setOfContainers = gRI.GetContainers(molecule);

            Assert.AreEqual(1, setOfContainers.Count);
            Assert.AreEqual(8, setOfContainers[0].Atoms.Count);
        }

        /// <summary>
        /// A unit test suite: COC1=CC=C(C=C1)Br
        /// </summary>
        // @cdk.inchi InChI=1/C7H7BrO/c1-9-7-4-2-6(8)3-5-7/h2-5H,1H3
        [TestMethod()]
        public void TestBenzene1bromo4methoxy_with()
        {
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("Br"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[5], molecule.Atoms[6], BondOrder.Single);
            molecule.AddBond(molecule.Atoms[6], molecule.Atoms[1], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("O"));
            molecule.AddBond(molecule.Atoms[6], molecule.Atoms[7], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[7], molecule.Atoms[8], BondOrder.Single);

            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            StructureResonanceGenerator gRI = new StructureResonanceGenerator();
            var setOfContainers = gRI.GetContainers(molecule);

            Assert.AreEqual(1, setOfContainers.Count);
            Assert.AreEqual(8, setOfContainers[0].Atoms.Count);
        }

        [TestMethod()]
        public void TestGetMaximalStructures()
        {
            StructureResonanceGenerator gRI = new StructureResonanceGenerator();
            Assert.AreEqual(50, gRI.MaximalStructures);
        }

        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestSetMaximalStructures_int()
        {
            StructureResonanceGenerator gRI = new StructureResonanceGenerator();
            Assert.AreEqual(50, gRI.MaximalStructures);
            gRI.MaximalStructures = 1;
            Assert.AreEqual(1, gRI.MaximalStructures);

        }

        /// <summary>
        /// A unit test suite: c1ccccc1CN
        /// </summary>
        // @cdk.inchi InChI=1/C7H9N/c8-6-7-4-2-1-3-5-7/h1-5H,6,8H2
        [TestMethod()]
        public void TestBenzylamine()
        {
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[5], molecule.Atoms[6], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[6], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("N"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[7], BondOrder.Single);

            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            StructureResonanceGenerator gRI = new StructureResonanceGenerator();
            var setOfContainers = gRI.GetContainers(molecule);

            Assert.AreEqual(1, setOfContainers.Count);
            Assert.AreEqual(6, setOfContainers[0].Atoms.Count);
        }

        /// <summary>
        /// A unit test suite: c1ccccc1CN
        /// </summary>
        // @cdk.inchi InChI=1/C7H9N/c8-6-7-4-2-1-3-5-7/h1-5H,6,8H2
        // @cdk.bug 2014515
        [TestMethod()]
        public void TestBenzylamine_Aromatic()
        {
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[5], molecule.Atoms[6], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[6], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("N"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[7], BondOrder.Single);

            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            Assert.IsTrue(Aromaticity.CDKLegacy.Apply(molecule));

            StructureResonanceGenerator gRI = new StructureResonanceGenerator();
            var setOfContainers = gRI.GetContainers(molecule);

            Assert.IsNotNull(setOfContainers);
            Assert.AreEqual(1, setOfContainers.Count);
            Assert.AreEqual(6, setOfContainers[0].Atoms.Count);
        }

        /// <summary>
        /// A unit test suite: c1ccccc1CN
        /// </summary>
        // @cdk.inchi InChI=1/C7H9N/c8-6-7-4-2-1-3-5-7/h1-5H,6,8H2
        [TestMethod()]
        public void TestBenzylamine_Aromatic_lookingSymmetry()
        {
            IAtomContainer molecule = builder.NewAtomContainer();
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[1], molecule.Atoms[2], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[2], molecule.Atoms[3], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[3], molecule.Atoms[4], BondOrder.Double);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[4], molecule.Atoms[5], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("C"));
            molecule.AddBond(molecule.Atoms[5], molecule.Atoms[6], BondOrder.Double);
            molecule.AddBond(molecule.Atoms[6], molecule.Atoms[1], BondOrder.Single);
            molecule.Atoms.Add(builder.NewAtom("N"));
            molecule.AddBond(molecule.Atoms[0], molecule.Atoms[7], BondOrder.Single);

            AddExplicitHydrogens(molecule);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(molecule);
            CDK.LonePairElectronChecker.Saturate(molecule);

            Assert.IsTrue(Aromaticity.CDKLegacy.Apply(molecule));

            StructureResonanceGenerator gRI = new StructureResonanceGenerator(true);
            var setOfContainers = gRI.GetContainers(molecule);

            Assert.IsNull(setOfContainers);
        }
    }
}
