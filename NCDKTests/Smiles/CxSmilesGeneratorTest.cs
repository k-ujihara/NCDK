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
    }
}
