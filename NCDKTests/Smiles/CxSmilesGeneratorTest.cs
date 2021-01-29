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
using NCDK.IO;
using System;
using System.Collections.Generic;

namespace NCDK.Smiles
{
    public class CxSmilesGeneratorTest
    {
        private readonly IChemObjectBuilder builder = CDK.Builder;

        [TestMethod()]
        public void EmptyCXSMILES()
        {
            CxSmilesState state = new CxSmilesState();
            Assert.AreEqual("", CxSmilesGenerator.Generate(state, SmiFlavors.CxSmiles, Array.Empty<int>(), Array.Empty<int>()));
        }

        [TestMethod()]
        public void Multicenter()
        {
            var state = new CxSmilesState { positionVar = new SortedDictionary<int, IList<int>>() };
            state.positionVar[0] = new[] { 4, 5, 6, 7 };
            state.positionVar[2] = new[] { 4, 6, 5, 7 };
            Assert.AreEqual(" |m:5:0.1.2.3,7:0.1.2.3|", CxSmilesGenerator.Generate(state, SmiFlavors.CxMulticenter, Array.Empty<int>(), new int[] { 7, 6, 5, 4, 3, 2, 1, 0 }));
        }

        [TestMethod()]
        public void Coords2d()
        {
            CxSmilesState state = new CxSmilesState
            {
                atomCoords = new List<double[]>
                {
                    new double[] { 0, 1.5, 0 },
                    new double[] { 0, 3, 0 },
                    new double[] { 1.5, 1.5, 0 },
                }
            };
            Assert.AreEqual(" |(1.5,1.5,;,1.5,;,3,)|", CxSmilesGenerator.Generate(state, SmiFlavors.CxCoordinates, Array.Empty<int>(), new int[] { 1, 2, 0 }));
        }

        [TestMethod()]
        public void Sgroups()
        {
            CxSmilesState state = new CxSmilesState
            {
                mysgroups = new List<CxSmilesState.CxSgroup>(1)
                {
                    new CxSmilesState.CxPolymerSgroup("n", new[] { 2, 3 }, "n", "ht"),
                    new CxSmilesState.CxPolymerSgroup("n", new[] { 5 }, "m", "ht")
                }
            };
            Assert.AreEqual(" |Sg:n:2:m:ht,Sg:n:4,5:n:ht|", CxSmilesGenerator.Generate(state, SmiFlavors.CxPolymer, Array.Empty<int>(), new int[] { 7, 6, 5, 4, 3, 2, 1, 0 }));
        }

        [TestMethod()]
        public void Radicals()
        {
            CxSmilesState state = new CxSmilesState
            {
                atomRads = new SortedDictionary<int, CxSmilesState.Radical>
                {
                    [2] = CxSmilesState.Radical.Monovalent,
                    [6] = CxSmilesState.Radical.Monovalent,
                    [4] = CxSmilesState.Radical.Divalent
                }
            };
            Assert.AreEqual(" |^1:1,5,^2:3|", CxSmilesGenerator.Generate(state, SmiFlavors.CxSmiles, Array.Empty<int>(), new int[] { 7, 6, 5, 4, 3, 2, 1, 0 }));
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
                IAtomContainer mol = mdlr.Read(CDK.Builder.NewAtomContainer());
                SmilesGenerator smigen = new SmilesGenerator(SmiFlavors.CxSmiles | SmiFlavors.AtomicMassStrict);
                Assert.AreEqual("C(C(=O)OC)(C*)*C(C(C1=C(C(=C(C(=C1[2H])[2H])[2H])[2H])[2H])(*)[2H])([2H])[2H] |Sg:n:0,1,2,3,4,5:n:ht,Sg:n:8,9,10,11,12,13,14,15,16,17,18,19,20,22,23,24:m:ht|", smigen.Create(mol));
            }
        }

        [TestMethod()]
        public void Chembl367774()
        {
            using (MDLV2000Reader mdlr = new MDLV2000Reader(GetType().Assembly.GetManifestResourceStream(GetType(), "CHEMBL367774.mol")))
            {
                IAtomContainer container = mdlr.Read(builder.NewAtomContainer());
                SmilesGenerator smigen = new SmilesGenerator(SmiFlavors.CxSmiles);
                Assert.AreEqual("OC(=O)C1=CC(F)=CC=2NC(=NC12)C3=CC=C(C=C3F)C4=CC=CC=C4", smigen.Create(container));
            }
        }

        [TestMethod()]
        public void RadicalCanon()
        {
            var builder = CDK.Builder;

            IAtomContainer mola = builder.NewAtomContainer();
            mola.Atoms.Add(builder.NewAtom("CH3"));
            mola.Atoms.Add(builder.NewAtom("CH2"));
            mola.Atoms.Add(builder.NewAtom("CH2"));
            mola.Atoms.Add(builder.NewAtom("CH2"));
            mola.Atoms.Add(builder.NewAtom("CH2"));
            mola.Atoms.Add(builder.NewAtom("CH1"));
            mola.Atoms.Add(builder.NewAtom("CH3"));
            mola.AddBond(mola.Atoms[1], mola.Atoms[2], BondOrder.Single);
            mola.AddBond(mola.Atoms[2], mola.Atoms[3], BondOrder.Single);
            mola.AddBond(mola.Atoms[3], mola.Atoms[4], BondOrder.Single);
            mola.AddBond(mola.Atoms[4], mola.Atoms[5], BondOrder.Single);
            mola.AddBond(mola.Atoms[5], mola.Atoms[6], BondOrder.Single);
            mola.AddBond(mola.Atoms[0], mola.Atoms[5], BondOrder.Single);
            mola.AddSingleElectronTo(mola.Atoms[1]);

            SmilesParser smipar = new SmilesParser(builder);
            var molb = smipar.ParseSmiles("CC(CCC[CH2])C |^1:5|");
            SmilesGenerator smigen = new SmilesGenerator(SmiFlavors.Canonical | SmiFlavors.CxRadical);
            Assert.AreEqual(smigen.Create(molb), smigen.Create(mola));
        }

        [TestMethod()]
        public void RoundTripLigandOrdering()
        {
            var mol = CDK.SmilesParser.ParseSmiles("Cl[*](Br)I |$;_R1;;$,LO:1:0.2.3|");
            var smigen = new SmilesGenerator(SmiFlavors.CxSmiles);
            Assert.AreEqual("Cl*(Br)I |$;R1$,LO:1:0.2.3|", smigen.Create(mol));
        }

        [TestMethod()]
        public void CanonLigandOrdering()
        {
            var mol = CDK.SmilesParser.ParseSmiles("Cl[*](I)Br |$;_R1;;$,LO:1:0.2.3|");
            var smigen = new SmilesGenerator(SmiFlavors.Canonical | SmiFlavors.CxSmiles);
            Assert.AreEqual("Cl*(Br)I |$;R1$,LO:1:0.3.2|", smigen.Create(mol));
        }

        [TestMethod()]
        public void RoundTripSgroupParents()
        {
            var mol = CDK.SmilesParser.ParseSmiles("CN1CCCCC1.CO.O |Sg:c:0,1,2,3,4,5,6::,Sg:c:7,8::,Sg:c:9::,Sg:mix:0,1,2,3,4,5,6,7,8,9::,Sg:mix:7,8,9::,SgH:3:4.0,4:2.1|");
            var smigen = new SmilesGenerator(SmiFlavors.CxSmiles);
            Assert.AreEqual("CN1CCCCC1.CO.O |Sg:c:0,1,2,3,4,5,6:c:,Sg:c:7,8:c:,Sg:c:9:c:,Sg:mix:0,1,2,3,4,5,6,7,8,9:mix:,Sg:mix:7,8,9:mix:,SgH:3:0.4,4:1.2|", smigen.Create(mol));
        }
    }
}
