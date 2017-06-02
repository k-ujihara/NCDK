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
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.IO;
using System.Collections.Generic;

namespace NCDK.Smiles
{
    public class CxSmilesGeneratorTest
    {
        [TestMethod()]
        public void EmptyCXSMILES()
        {
            CxSmilesState state = new CxSmilesState();
            Assert.AreEqual("", CxSmilesGenerator.Generate(state, SmiFlavor.CxSmiles, new int[0], new int[0]));
        }

        [TestMethod()]
        public void Multicenter()
        {
            CxSmilesState state = new CxSmilesState();
            state.positionVar = new Dictionary<int, IList<int>>();
            state.positionVar[0] = new[] { 4, 5, 6, 7 };
            state.positionVar[2] = new[] { 4, 6, 5, 7 };
            Assert.AreEqual(" |m:5:0.1.2.3,7:0.1.2.3|", CxSmilesGenerator.Generate(state, SmiFlavor.CxMulticenter, new int[0], new int[] { 7, 6, 5, 4, 3, 2, 1, 0 }));
        }

        [TestMethod()]
        public void Coords2d()
        {
            CxSmilesState state = new CxSmilesState();
            state.AtomCoords = new[]
            {
                new double[] { 0, 1.5, 0 },
                new double[] { 0, 3, 0 },
                new double[] { 1.5, 1.5, 0 },
            };
            Assert.AreEqual(" |(1.5,1.5,;,1.5,;,3,)|", CxSmilesGenerator.Generate(state, SmiFlavor.CxCoordinates, new int[0], new int[] { 1, 2, 0 }));
        }

        [TestMethod()]
        public void Sgroups()
        {
            CxSmilesState state = new CxSmilesState();
            state.sgroups = new List<CxSmilesState.PolymerSgroup>(1);
            state.sgroups.Add(new CxSmilesState.PolymerSgroup("n", new[] { 2, 3 }, "n", "ht"));
            state.sgroups.Add(new CxSmilesState.PolymerSgroup("n", new[] { 5 }, "m", "ht"));
            Assert.AreEqual(" |Sg:n:2:m:ht,Sg:n:4,5:n:ht|", CxSmilesGenerator.Generate(state, SmiFlavor.CxPolymer, new int[0], new int[] { 7, 6, 5, 4, 3, 2, 1, 0 }));
        }

        [TestMethod()]
        public void Radicals()
        {
            CxSmilesState state = new CxSmilesState();
            state.atomRads = new Dictionary<int, CxSmilesState.Radical>();
            state.atomRads[2] = CxSmilesState.Radical.Monovalent;
            state.atomRads[6] = CxSmilesState.Radical.Monovalent;
            state.atomRads[4] = CxSmilesState.Radical.Divalent;
            Assert.AreEqual(" |^1:1,5,^2:3|", CxSmilesGenerator.Generate(state, SmiFlavor.CxSmiles, new int[0], new int[] { 7, 6, 5, 4, 3, 2, 1, 0 }));
        }

        /// <summary>
        /// Integration - test used to fail because the D (pseudo) was swapped out with a 2H after Sgroups were
        /// initialized.
        /// </summary>
        [TestMethod()]
        public void Chebi53695()
        {
            using (var ins = GetType().Assembly.GetManifestResourceStream(GetType(), "CHEBI_53695.mol"))
            using (var mdlr = new MDLV2000Reader(ins))
            {
                IAtomContainer mol = mdlr.Read(Silent.ChemObjectBuilder.Instance.CreateAtomContainer());
                SmilesGenerator smigen = new SmilesGenerator(SmiFlavor.CxSmiles | SmiFlavor.AtomicMassStrict);
                Assert.AreEqual("C(C(=O)OC)(C*)*C(C(C1=C(C(=C(C(=C1[2H])[2H])[2H])[2H])[2H])(*)[2H])([2H])[2H] |Sg:n:0,1,2,3,4,5:n:ht,Sg:n:8,9,10,11,12,13,14,15,16,17,18,19,20,22,23,24:m:ht|", smigen.Create(mol));
            }
        }

        [TestMethod()]
        public void Chembl367774()
        {
            using (MDLV2000Reader mdlr = new MDLV2000Reader(GetType().Assembly.GetManifestResourceStream(GetType(), "CHEMBL367774.mol")))
            {
                IAtomContainer container = mdlr.Read(new AtomContainer());
                SmilesGenerator smigen = new SmilesGenerator(SmiFlavor.CxSmiles);
                Assert.AreEqual("OC(=O)C1=CC(F)=CC=2NC(=NC12)C3=CC=C(C=C3F)C4=CC=CC=C4", smigen.Create(container));
            }
        }

        [TestMethod()]
        public void RadicalCanon()
        {
            IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;

            IAtomContainer mola = builder.CreateAtomContainer();
            mola.Atoms.Add(builder.CreateAtom("CH3"));
            mola.Atoms.Add(builder.CreateAtom("CH2"));
            mola.Atoms.Add(builder.CreateAtom("CH2"));
            mola.Atoms.Add(builder.CreateAtom("CH2"));
            mola.Atoms.Add(builder.CreateAtom("CH2"));
            mola.Atoms.Add(builder.CreateAtom("CH1"));
            mola.Atoms.Add(builder.CreateAtom("CH3"));
            mola.AddBond(mola.Atoms[1], mola.Atoms[2], BondOrder.Single);
            mola.AddBond(mola.Atoms[2], mola.Atoms[3], BondOrder.Single);
            mola.AddBond(mola.Atoms[3], mola.Atoms[4], BondOrder.Single);
            mola.AddBond(mola.Atoms[4], mola.Atoms[5], BondOrder.Single);
            mola.AddBond(mola.Atoms[5], mola.Atoms[6], BondOrder.Single);
            mola.AddBond(mola.Atoms[0], mola.Atoms[5], BondOrder.Single);
            mola.AddSingleElectronTo(mola.Atoms[1]);

            SmilesParser smipar = new SmilesParser(builder);
            IAtomContainer molb = smipar.ParseSmiles("CC(CCC[CH2])C |^1:5|");
            SmilesGenerator smigen = new SmilesGenerator(SmiFlavor.Canonical | SmiFlavor.CxRadical);
            Assert.AreEqual(smigen.Create(molb), smigen.Create(mola));
        }
    }
}
