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
using NCDK.SGroups;
using System.Collections.Generic;
using System.Linq;

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
            Assert.AreEqual("", (string)reaction.GetProperty<string>(CDKPropertyName.TITLE));
        }

        // grouping is invalid as we group 4 in two separate fragments
        [TestMethod()]
        public void FragmentGroupingInvalid()
        {
            IReaction reaction = smipar.ParseReactionSmiles("CC1=NC2=C(O)C=CC=C2C=C1.CC(Cl)=O.[Al+3].[Cl-].[Cl-].[Cl-]>[O-][N+](=O)C1=CC=CC=C1>CC(=O)C1=C2C=CC(C)=NC2=C(O)C=C1 |f:2.3.4.5,4.6|");
            Assert.AreEqual(6, reaction.Reactants.Count);
            Assert.AreEqual(1, reaction.Agents.Count);
            Assert.AreEqual(1, reaction.Products.Count);
            Assert.AreEqual("", (string)reaction.GetProperty<string>(CDKPropertyName.TITLE));
        }

        [TestMethod()]
        public void FragmentGroupingAgents()
        {
            IReaction reaction = smipar.ParseReactionSmiles("CC1=NC2=C(O)C=CC=C2C=C1.CC(Cl)=O>[Al+3].[Cl-].[Cl-].[Cl-].[O-][N+](=O)C1=CC=CC=C1>CC(=O)C1=C2C=CC(C)=NC2=C(O)C=C1 |f:2.3.4.5|");
            Assert.AreEqual(2, reaction.Reactants.Count);
            Assert.AreEqual(2, reaction.Agents.Count);
            Assert.AreEqual(1, reaction.Products.Count);
            Assert.AreEqual("", (string)reaction.GetProperty<string>(CDKPropertyName.TITLE));
        }

        [TestMethod()]
        public void EmptyCXSMILES()
        {
            IReaction reaction = smipar.ParseReactionSmiles("CC1=NC2=C(O)C=CC=C2C=C1.CC(Cl)=O>[Al+3].[Cl-].[Cl-].[Cl-].[O-][N+](=O)C1=CC=CC=C1>CC(=O)C1=C2C=CC(C)=NC2=C(O)C=C1 ||");
            Assert.AreEqual("", (string)reaction.GetProperty<string>(CDKPropertyName.TITLE));
        }

        [TestMethod()]
        public void FragmentGroupingProducts()
        {
            IReaction reaction = smipar.ParseReactionSmiles("CC1=NC2=C(O)C=CC=C2C=C1.CC(Cl)=O>[O-][N+](=O)C1=CC=CC=C1>CC(=O)C1=C2C=CC(C)=NC2=C(O)C=C1.[Al+3].[Cl-].[Cl-].[Cl-] |f:3.4.5.6|");
            Assert.AreEqual(2, reaction.Reactants.Count);
            Assert.AreEqual(1, reaction.Agents.Count);
            Assert.AreEqual(2, reaction.Products.Count);
            Assert.AreEqual("", (string)reaction.GetProperty<string>(CDKPropertyName.TITLE));
        }

        [TestMethod()]
        public void NonCXSMILESLayer()
        {
            IAtomContainer mol = smipar.ParseSmiles("c1ccccc1 |<benzene>|");
            Assert.IsNotNull(mol);
            Assert.AreEqual("|<benzene>|", (string)mol.GetProperty<string>(CDKPropertyName.TITLE));
        }

        [TestMethod()]
        public void TruncatedCXSMILES()
        {
            IAtomContainer mol = smipar.ParseSmiles("c1ccccc1 |");
            Assert.IsNotNull(mol);
            Assert.AreEqual("|", (string)mol.GetProperty<string>(CDKPropertyName.TITLE));
        }

        [TestMethod()]
        public void CorrectTitle()
        {
            IAtomContainer mol = smipar.ParseSmiles("c1ccccc1 |c:1,3,4| benzene");
            Assert.IsNotNull(mol);
            Assert.AreEqual("benzene", (string)mol.GetProperty<string>(CDKPropertyName.TITLE));
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
            IAtomContainer mol = smipar.ParseSmiles("**.c1ccccc1CC |m:1:2.3.4.5.6.7|");
            IList<Sgroup> sgroups = mol.GetProperty<IList<Sgroup>>(CDKPropertyName.CTAB_SGROUPS);
            Assert.AreEqual(1, sgroups.Count);
            Assert.AreEqual(SgroupType.ExtMulticenter, sgroups[0].Type);
            Assert.AreEqual(7, sgroups[0].Atoms.Count);
            Assert.AreEqual(1, sgroups[0].Bonds.Count);
        }

        [TestMethod()]
        public void StructuralRepeatUnit()
        {
            IAtomContainer mol = smipar.ParseSmiles("**.c1ccccc1CC |Sg:n:8:m:ht|");
            IList<Sgroup> sgroups = mol.GetProperty<IList<Sgroup>>(CDKPropertyName.CTAB_SGROUPS);
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
            IAtomContainer mol = smipar.ParseSmiles("**.c1ccccc1CC |m:1:2.3.4.5.6.7,Sg:n:8:m:ht,$R';;;;;;;;;_AP1$|");
            IList<Sgroup> sgroups = mol.GetProperty<IList<Sgroup>>(CDKPropertyName.CTAB_SGROUPS);
            // P-var and F-var
            Assert.AreEqual(2, sgroups.Count);
            // atom-labels
            Assert.IsInstanceOfType(mol.Atoms[0], typeof(IPseudoAtom));
            Assert.AreEqual("R'", ((IPseudoAtom)mol.Atoms[0]).Label);
            // attach-points
            Assert.IsInstanceOfType(mol.Atoms[9], typeof(IPseudoAtom));
            Assert.AreEqual(1, ((IPseudoAtom)mol.Atoms[9]).AttachPointNum);
            Assert.AreEqual("", (string)mol.GetProperty<string>(CDKPropertyName.TITLE));
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
            Assert.AreEqual("HydDonor", (string)mol.Atoms[0].GetProperty<string>(CDKPropertyName.COMMENT));
            Assert.AreEqual("HydAcceptor", (string)mol.Atoms[2].GetProperty<string>(CDKPropertyName.COMMENT));
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
    }
}
