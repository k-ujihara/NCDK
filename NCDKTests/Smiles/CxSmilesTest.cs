/*
 * Copyright (c) 2016 John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 2.1 of the License, or (at
 * your option) any later version. All we ask is that proper credit is given
 * for our work, which includes - but is not limited to - adding the above
 * copyright notice to the beginning of your source code files, and to any
 * copyright notice that you may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * Any WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */
using NCDK.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Sgroups;
using System.Collections.Generic;
using System.Linq;
using NCDK.Silent;

namespace NCDK.Smiles
{
    [TestClass()]
    public class CxSmilesTest
    {
        private readonly SmilesParser smipar = new SmilesParser(Silent.ChemObjectBuilder.Instance);

        [TestMethod()]
        public void FragmentGroupingReactants()
        {
            IReaction reaction = smipar.ParseReactionSmiles("CC1=NC2=C(O)C=CC=C2C=C1.CC(Cl)=O.[Al+3].[Cl-].[Cl-].[Cl-]>[O-][N+](=O)C1=CC=CC=C1>CC(=O)C1=C2C=CC(C)=NC2=C(O)C=C1 |f:2.3.4.5|");
            Assert.AreEqual(3, reaction.Reactants.Count);
            Assert.AreEqual(1, reaction.Agents.Count);
            Assert.AreEqual(1, reaction.Products.Count);
            Assert.AreEqual("", reaction.GetProperty<string>(CDKPropertyName.Title));
        }

        // grouping is invalid as we group 4 in two separate fragments
        [TestMethod()]
        public void FragmentGroupingInvalid()
        {
            IReaction reaction = smipar.ParseReactionSmiles("CC1=NC2=C(O)C=CC=C2C=C1.CC(Cl)=O.[Al+3].[Cl-].[Cl-].[Cl-]>[O-][N+](=O)C1=CC=CC=C1>CC(=O)C1=C2C=CC(C)=NC2=C(O)C=C1 |f:2.3.4.5,4.6|");
            Assert.AreEqual(6, reaction.Reactants.Count);
            Assert.AreEqual(1, reaction.Agents.Count);
            Assert.AreEqual(1, reaction.Products.Count);
            Assert.AreEqual("", reaction.GetProperty<string>(CDKPropertyName.Title));
        }

        [TestMethod()]
        public void FragmentGroupingAgents()
        {
            IReaction reaction = smipar.ParseReactionSmiles("CC1=NC2=C(O)C=CC=C2C=C1.CC(Cl)=O>[Al+3].[Cl-].[Cl-].[Cl-].[O-][N+](=O)C1=CC=CC=C1>CC(=O)C1=C2C=CC(C)=NC2=C(O)C=C1 |f:2.3.4.5|");
            Assert.AreEqual(2, reaction.Reactants.Count);
            Assert.AreEqual(2, reaction.Agents.Count);
            Assert.AreEqual(1, reaction.Products.Count);
            Assert.AreEqual("", reaction.GetProperty<string>(CDKPropertyName.Title));
        }

        [TestMethod()]
        public void EmptyCXSMILES()
        {
            IReaction reaction = smipar.ParseReactionSmiles("CC1=NC2=C(O)C=CC=C2C=C1.CC(Cl)=O>[Al+3].[Cl-].[Cl-].[Cl-].[O-][N+](=O)C1=CC=CC=C1>CC(=O)C1=C2C=CC(C)=NC2=C(O)C=C1 ||");
            Assert.AreEqual("", reaction.GetProperty<string>(CDKPropertyName.Title));
        }

        [TestMethod()]
        public void FragmentGroupingProducts()
        {
            IReaction reaction = smipar.ParseReactionSmiles("CC1=NC2=C(O)C=CC=C2C=C1.CC(Cl)=O>[O-][N+](=O)C1=CC=CC=C1>CC(=O)C1=C2C=CC(C)=NC2=C(O)C=C1.[Al+3].[Cl-].[Cl-].[Cl-] |f:3.4.5.6|");
            Assert.AreEqual(2, reaction.Reactants.Count);
            Assert.AreEqual(1, reaction.Agents.Count);
            Assert.AreEqual(2, reaction.Products.Count);
            Assert.AreEqual("", reaction.GetProperty<string>(CDKPropertyName.Title));
        }

        [TestMethod()]
        public void NonCXSMILESLayer()
        {
            IAtomContainer mol = smipar.ParseSmiles("c1ccccc1 |<benzene>|");
            Assert.IsNotNull(mol);
            Assert.AreEqual("|<benzene>|", (string)mol.Title);
        }

        [TestMethod()]
        public void TruncatedCXSMILES()
        {
            IAtomContainer mol = smipar.ParseSmiles("c1ccccc1 |");
            Assert.IsNotNull(mol);
            Assert.AreEqual("|", (string)mol.Title);
        }

        [TestMethod()]
        public void CorrectTitle()
        {
            IAtomContainer mol = smipar.ParseSmiles("c1ccccc1 |c:1,3,4| benzene");
            Assert.IsNotNull(mol);
            Assert.AreEqual("benzene", (string)mol.Title);
        }

        [TestMethod()]
        public void AtomLabels()
        {
            IAtomContainer mol = smipar.ParseSmiles("**.c1ccccc1CC |$R'$|");
            Assert.IsInstanceOfType(mol.Atoms[0], typeof(IPseudoAtom));
            Assert.AreEqual("R'", ((IPseudoAtom)mol.Atoms[0]).Label);
        }

        [TestMethod()]
        public void AttachPoints()
        {
            IAtomContainer mol = smipar.ParseSmiles("**.c1ccccc1CC |$;;;;;;;;;_AP1$|");
            Assert.IsInstanceOfType(mol.Atoms[9], typeof(IPseudoAtom));
            Assert.AreEqual(1, ((IPseudoAtom)mol.Atoms[9]).AttachPointNum);
        }

        [TestMethod()]
        public void PositionalVariation()
        {
            var mol = smipar.ParseSmiles("**.c1ccccc1CC |m:1:2.3.4.5.6.7|");
            var sgroups = mol.GetCtabSgroups();
            Assert.AreEqual(1, sgroups.Count);
            Assert.AreEqual(SgroupType.ExtMulticenter, sgroups[0].Type);
            Assert.AreEqual(7, sgroups[0].Atoms.Count);
            Assert.AreEqual(1, sgroups[0].Bonds.Count);
        }

        [TestMethod()]
        public void StructuralRepeatUnit()
        {
            var mol = smipar.ParseSmiles("**.c1ccccc1CC |Sg:n:8:m:ht|");
            var sgroups = mol.GetCtabSgroups();
            Assert.AreEqual(1, sgroups.Count);
            Assert.AreEqual(SgroupType.CtabStructureRepeatUnit, sgroups[0].Type);
            Assert.AreEqual("m", sgroups[0].Subscript);
            Assert.AreEqual("ht", sgroups[0].GetValue(SgroupKey.CtabConnectivity));
            Assert.AreEqual(1, sgroups[0].Atoms.Count);
            Assert.AreEqual(2, sgroups[0].Bonds.Count);
        }

        [TestMethod()]
        public void MarkushFragment()
        {
            var mol = smipar.ParseSmiles("**.c1ccccc1CC |m:1:2.3.4.5.6.7,Sg:n:8:m:ht,$R';;;;;;;;;_AP1$|");
            var sgroups = mol.GetCtabSgroups();
            // P-var and F-var
            Assert.AreEqual(2, sgroups.Count);
            // atom-labels
            Assert.IsInstanceOfType(mol.Atoms[0], typeof(IPseudoAtom));
            Assert.AreEqual("R'", ((IPseudoAtom)mol.Atoms[0]).Label);
            // attach-points
            Assert.IsInstanceOfType(mol.Atoms[9], typeof(IPseudoAtom));
            Assert.AreEqual(1, ((IPseudoAtom)mol.Atoms[9]).AttachPointNum);
            Assert.AreEqual("", (string)mol.Title);
        }

        [TestMethod()]
        public void AtomCoordinates2D()
        {
            IAtomContainer mol = smipar.ParseSmiles("CCC |(0,1,;0,2,;0,3,)|");
            Assert.AreEqual(new Vector2(0, 1), mol.Atoms[0].Point2D);
            Assert.AreEqual(new Vector2(0, 2), mol.Atoms[1].Point2D);
            Assert.AreEqual(new Vector2(0, 3), mol.Atoms[2].Point2D);
        }

        [TestMethod()]
        public void AtomCoordinates3D()
        {
            IAtomContainer mol = smipar.ParseSmiles("CCC |(0,1,1;0,2,1;0,3,1)|");
            Assert.AreEqual(new Vector3(0, 1, 1), mol.Atoms[0].Point3D);
            Assert.AreEqual(new Vector3(0, 2, 1), mol.Atoms[1].Point3D);
            Assert.AreEqual(new Vector3(0, 3, 1), mol.Atoms[2].Point3D);
        }

        [TestMethod()]
        public void AtomValues()
        {
            IAtomContainer mol = smipar.ParseSmiles("N1CN=CC1 |$_AV:HydDonor;;HydAcceptor$|");
            Assert.AreEqual("HydDonor", (string)mol.Atoms[0].GetProperty<string>(CDKPropertyName.Comment));
            Assert.AreEqual("HydAcceptor", (string)mol.Atoms[2].GetProperty<string>(CDKPropertyName.Comment));
        }

        [TestMethod()]
        public void MonovalentRadical()
        {
            IAtomContainer mol = smipar.ParseSmiles("[N]1C=CC=C1 |c:1,3,^1:0|");
            Assert.AreEqual(1, mol.GetConnectedSingleElectrons(mol.Atoms[0]).Count());
        }

        [TestMethod()]
        public void DivalentRadical()
        {
            IAtomContainer mol = smipar.ParseSmiles("[C]1C2=CC=CC=C2C2=CC=CC=C12 |c:3,5,10,t:1,8,12,^3:0|");
            Assert.AreEqual(2, mol.GetConnectedSingleElectrons(mol.Atoms[0]).Count());
        }

        [TestMethod()]
        public void GenericReaction()
        {
            IReaction rxn = smipar.ParseReactionSmiles("C1=CC(=CC=C1)C(CC(N)=O)=O.*C>C1(=CC=CC=C1)N.C*.C1=CC(=CC=C1)C=2C=C(C3=C(N2)C=CC=C3)O.C*.C*> |$;;;;;;;;;;;;R22;;;;;;;;;;;;;;;;;;;;;;;;;;;;;R22$,f:0.1,2.3,4.5.6,m:13:0.1.2.3.4.5,21:14.15.16.17.18.19,40:23.24.25.26.27.28,42:29.30.31.32.33.34.35.36.37.38|");
        }

        [TestMethod()]
        public void TrailingAtomLabelSemiColonAndAtomValues()
        {
            var mol = smipar.ParseSmiles("[H]C1=C([H])N2C(=O)C(=C([O-])[N+](CC3=CN=C(Cl)S3)=C2C(C)=C1[H])C1=CC(*)=CC=C1.** |$;;;;;;;;;;;;;;;;;;;;;;;;;;R;;;;RA;$,$_AV:;;;;;;;;;;;;;;;;;;;;;;;;2;;;4;5;6;;$,c:1,18,22,29,31,t:7,12,14,26,m:31:29.28.27.25.24.23|");
            var sgroups = mol.GetCtabSgroups();
            Assert.IsInstanceOfType(mol.Atoms[26], typeof(IPseudoAtom));
            Assert.IsInstanceOfType(mol.Atoms[30], typeof(IPseudoAtom));
            Assert.AreEqual("R", ((IPseudoAtom)mol.Atoms[26]).Label);
            Assert.AreEqual("RA", ((IPseudoAtom)mol.Atoms[30]).Label);
            Assert.AreEqual("2", mol.Atoms[24].GetProperty<string>(CDKPropertyName.Comment));
            Assert.AreEqual("4", mol.Atoms[27].GetProperty<string>(CDKPropertyName.Comment));
            Assert.AreEqual("5", mol.Atoms[28].GetProperty<string>(CDKPropertyName.Comment));
            Assert.AreEqual("6", mol.Atoms[29].GetProperty<string>(CDKPropertyName.Comment));
            Assert.AreEqual(1, sgroups.Count);
        }

        [TestMethod()]
        public void GenerateLabelledSmiles()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(new Atom("C"));
            mol.Atoms[0].ImplicitHydrogenCount = 3;
            mol.Atoms.Add(new Atom("C"));
            mol.Atoms[1].ImplicitHydrogenCount = 2;
            mol.Atoms.Add(new PseudoAtom("R1"));
            mol.Atoms[2].ImplicitHydrogenCount = 0;
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            SmilesGenerator smigen = new SmilesGenerator(SmiFlavors.CxAtomLabel);
            string smi = smigen.Create(mol);
            Assert.AreEqual("CC* |$;;R1$|", smi);
        }

        [TestMethod()]
        public void GenerateCanonLabelledSmiles()
        {
            IAtomContainer mol = new AtomContainer();
            mol.Atoms.Add(new Atom("C"));
            mol.Atoms[0].ImplicitHydrogenCount = 3;
            mol.Atoms.Add(new Atom("C"));
            mol.Atoms[1].ImplicitHydrogenCount = 2;
            mol.Atoms.Add(new PseudoAtom("R1"));
            mol.Atoms[2].ImplicitHydrogenCount = 0;
            mol.AddBond(mol.Atoms[0], mol.Atoms[1], BondOrder.Single);
            mol.AddBond(mol.Atoms[1], mol.Atoms[2], BondOrder.Single);
            SmilesGenerator smigen = new SmilesGenerator(SmiFlavors.Canonical |
                                                         SmiFlavors.CxAtomLabel);
            string smi = smigen.Create(mol);
            Assert.AreEqual("*CC |$R1$|", smi);
        }

        [TestMethod()]
        public void RoundTripMulticenter()
        {
            IChemObjectBuilder bldr = Silent.ChemObjectBuilder.Instance;
            SmilesParser smipar = new SmilesParser(bldr);
            IAtomContainer mol = smipar.ParseSmiles("c1ccccc1.*Cl |m:6:0.1.2.3.4.5|");
            SmilesGenerator smigen = new SmilesGenerator(SmiFlavors.UseAromaticSymbols |
                                                         SmiFlavors.CxMulticenter);
            string smi = smigen.Create(mol);
            Assert.AreEqual("c1ccccc1.*Cl |m:6:0.1.2.3.4.5|", smi);
        }

        [TestMethod()]
        public void CanonMulticenter()
        {
            IChemObjectBuilder bldr = Silent.ChemObjectBuilder.Instance;
            SmilesParser smipar = new SmilesParser(bldr);
            IAtomContainer mol = smipar.ParseSmiles("c1ccccc1.*Cl |m:6:0.1.2.3.4.5|");
            SmilesGenerator smigen = new SmilesGenerator(SmiFlavors.UseAromaticSymbols |
                                                         SmiFlavors.CxMulticenter |
                                                         SmiFlavors.Canonical);
            string smi = smigen.Create(mol);
            Assert.AreEqual("*Cl.c1ccccc1 |m:0:2.3.4.5.6.7|", smi);
        }


        [TestMethod()]
        public void RoundTripPEGn()
        {
            IChemObjectBuilder bldr = Silent.ChemObjectBuilder.Instance;
            SmilesParser smipar = new SmilesParser(bldr);
            IAtomContainer mol = smipar.ParseSmiles("CCCOCCO |Sg:n:1,2,3::ht|");
            SmilesGenerator smigen = new SmilesGenerator(SmiFlavors.CxPolymer);
            string smi = smigen.Create(mol);
            Assert.AreEqual("CCCOCCO |Sg:n:1,2,3:n:ht|", smi);
        }

        [TestMethod()]
        public void CanonPEGn()
        {
            IChemObjectBuilder bldr = Silent.ChemObjectBuilder.Instance;
            SmilesParser smipar = new SmilesParser(bldr);
            IAtomContainer mol = smipar.ParseSmiles("CCCOCCO |Sg:n:1,2,3::ht|");
            SmilesGenerator smigen = new SmilesGenerator(SmiFlavors.Canonical |
                                                         SmiFlavors.CxPolymer);
            string smi = smigen.Create(mol);
            Assert.AreEqual("OCCOCCC |Sg:n:3,4,5:n:ht|", smi);
        }

        [TestMethod()]
        public void CoordsEtOH()
        {
            IChemObjectBuilder bldr = Silent.ChemObjectBuilder.Instance;
            SmilesParser smipar = new SmilesParser(bldr);
            IAtomContainer mol = smipar.ParseSmiles("CCO |(,,;1,1,;2,2,)|");
            SmilesGenerator smigen = new SmilesGenerator(SmiFlavors.CxCoordinates);
            string smi = smigen.Create(mol);
            Assert.AreEqual("CCO |(,,;1,1,;2,2,)|", smi);
        }

        [TestMethod()]
        public void CanonCoordsEtOH()
        {
            IChemObjectBuilder bldr = Silent.ChemObjectBuilder.Instance;
            SmilesParser smipar = new SmilesParser(bldr);
            IAtomContainer mol = smipar.ParseSmiles("CCO |(,,;1,1,;2,2,)|");
            SmilesGenerator smigen = new SmilesGenerator(SmiFlavors.Canonical |
                                                         SmiFlavors.CxCoordinates);
            string smi = smigen.Create(mol);
            Assert.AreEqual("OCC |(2,2,;1,1,;,,)|", smi);
        }

        [TestMethod()]
        public void NoCoordsOptEtOH()
        {
            IChemObjectBuilder bldr = Silent.ChemObjectBuilder.Instance;
            SmilesParser smipar = new SmilesParser(bldr);
            IAtomContainer mol = smipar.ParseSmiles("CCO |(,,;1,1,;2,2,)|");
            SmilesGenerator smigen = new SmilesGenerator(0);
            string smi = smigen.Create(mol);
            Assert.AreEqual("CCO", smi);
        }

        [TestMethod()]
        public void NoCoordsInEtOH()
        {
            IChemObjectBuilder bldr = Silent.ChemObjectBuilder.Instance;
            SmilesParser smipar = new SmilesParser(bldr);
            IAtomContainer mol = smipar.ParseSmiles("CCO");
            SmilesGenerator smigen = new SmilesGenerator(SmiFlavors.CxCoordinates);
            string smi = smigen.Create(mol);
            Assert.AreEqual("CCO", smi);
        }

        [TestMethod()]
        public void RoundTripRadicals()
        {
            IChemObjectBuilder bldr = Silent.ChemObjectBuilder.Instance;
            SmilesParser smipar = new SmilesParser(bldr);
            IAtomContainer mol = smipar.ParseSmiles("[C]1C[CH][CH]OC1 |^1:2,3,^2:0|");
            SmilesGenerator smigen = new SmilesGenerator(SmiFlavors.CxRadical);
            string smi = smigen.Create(mol);
            Assert.AreEqual("[C]1C[CH][CH]OC1 |^1:2,3,^2:0|", smi);
        }

        [TestMethod()]
        public void CanonRadicals()
        {
            IChemObjectBuilder bldr = Silent.ChemObjectBuilder.Instance;
            SmilesParser smipar = new SmilesParser(bldr);
            IAtomContainer mol = smipar.ParseSmiles("[C]1C[CH][CH]OC1 |^1:2,3,^2:0|");
            SmilesGenerator smigen = new SmilesGenerator(SmiFlavors.CxRadical |
                                                         SmiFlavors.Canonical);
            string smi = smigen.Create(mol);
            Assert.AreEqual("[C]1CO[CH][CH]C1 |^1:3,4,^2:0|", smi);
        }

        [TestMethod()]
        public void RoundTripReactionAtomLabelsAndFragmentGroups()
        {
            IChemObjectBuilder bldr = Silent.ChemObjectBuilder.Instance;
            SmilesParser smipar = new SmilesParser(bldr);
            IReaction rxn = smipar.ParseReactionSmiles("CC(C)c1ccccc1.ClC([*])=O>ClCCl.[Al+3].[Cl-].[Cl-].[Cl-]>CC(C)c1ccc(cc1)C([*])=O |$;;;;;;;;;;;R1;;;;;;;;;;;;;;;;;;;R1;$,f:3.4.5.6|");
            SmilesGenerator smigen = new SmilesGenerator(SmiFlavors.CxAtomLabel |
                                                         SmiFlavors.CxFragmentGroup);
            Assert.AreEqual("CC(C)C1=CC=CC=C1.ClC(*)=O>ClCCl.[Al+3].[Cl-].[Cl-].[Cl-]>CC(C)C1=CC=C(C=C1)C(*)=O |f:3.4.5.6,$;;;;;;;;;;;R1;;;;;;;;;;;;;;;;;;;R1$|", smigen.Create(rxn));
        }

        [TestMethod()]
        public void CanonicalReactionAtomLabelsAndFragmentGroups()
        {
            IChemObjectBuilder bldr = Silent.ChemObjectBuilder.Instance;
            SmilesParser smipar = new SmilesParser(bldr);
            IReaction rxn1 = smipar.ParseReactionSmiles("CC(C)c1ccccc1.ClC([*])=O>[Al+3].[Cl-].[Cl-].[Cl-].ClCCl>CC(C)c1ccc(cc1)C([*])=O |$;;;;;;;;;;;R1;;;;;;;;;;;;;;;;;;;R1;$,f:2.3.4.5|");
            IReaction rxn2 = smipar.ParseReactionSmiles("ClC([*])=O.CC(C)c1ccccc1>[Al+3].[Cl-].[Cl-].[Cl-].ClCCl>CC(C)c1ccc(cc1)C([*])=O |$;;R1;;;;;;;;;;;;;;;;;;;;;;;;;;;;R1;$,f:2.3.5.4|");
            SmilesGenerator smigen = new SmilesGenerator(SmiFlavors.CxAtomLabel |
                                                         SmiFlavors.CxFragmentGroup |
                                                         SmiFlavors.Canonical);
            Assert.AreEqual(smigen.Create(rxn2), smigen.Create(rxn1));
        }

        [TestMethod()]
        public void CanonAtomLabels()
        {
            IChemObjectBuilder bldr = Silent.ChemObjectBuilder.Instance;
            SmilesParser smipar = new SmilesParser(bldr);
            IAtomContainer mol = smipar.ParseSmiles("c1ccccc1O |$_AV:0;1;2;3;4;5;6$|");
            SmilesGenerator smigen = new SmilesGenerator(SmiFlavors.Canonical | SmiFlavors.CxAtomValue);
            Assert.AreEqual("OC=1C=CC=CC1 |$_AV:6;5;0;1;2;3;4$|", smigen.Create(mol));
        }
    }
}
