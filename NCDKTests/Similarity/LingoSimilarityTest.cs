/*
 *
 * Copyright (C) 2010  Rajarshi Guha <rajarshi.guha@gmail.com>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All I ask is that proper credit is given for my work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
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
using NCDK.Fingerprints;
using NCDK.Templates;
using NCDK.Tools.Manipulator;
using System.Collections.Generic;

namespace NCDK.Similarity
{
    /// <summary>
    // @cdk.module test-fingerprint
    /// </summary>
    [TestClass()]
    public class LingoSimilarityTest : CDKTestCase
    {
        [TestMethod()]
        public void TestLingoSim()
        {
            IAtomContainer mol1 = TestMoleculeFactory.MakeIndole();
            IAtomContainer mol2 = TestMoleculeFactory.MakeIndole();
            AddImplicitHydrogens(mol1);
            AddImplicitHydrogens(mol2);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol1);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol2);
            LingoFingerprinter fingerprinter = new LingoFingerprinter();
            IDictionary<string, int> bs1 = fingerprinter.GetRawFingerprint(mol1);
            IDictionary<string, int> bs2 = fingerprinter.GetRawFingerprint(mol2);
            var lingosim = LingoSimilarity.Calculate(bs1, bs2);
            Assert.AreEqual(1.0, lingosim, 0.01);
        }
    }
}
