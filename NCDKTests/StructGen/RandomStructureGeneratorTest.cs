/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
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
using NCDK.Graphs;
using NCDK.Layout;
using NCDK.Numerics;
using NCDK.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.StructGen
{
    // @cdk.module test-structgen
    [TestClass()]
    public class RandomStructureGeneratorTest : CDKTestCase
    {
        public bool debug = false;
        bool standAlone = false;

        public void SetStandAlone(bool standAlone)
        {
            this.standAlone = standAlone;
        }

        [TestMethod()]
        public void TestTwentyRandomStructures()
        {
            IAtomContainer molecule = TestMoleculeFactory.MakeAlphaPinene();
            RandomGenerator rg = new RandomGenerator(molecule);
            IAtomContainer result = null;
            for (int f = 0; f < 50; f++)
            {
                result = rg.ProposeStructure();
                Assert.AreEqual(molecule.Atoms.Count, result.Atoms.Count);
                Assert.AreEqual(1, ConnectivityChecker.PartitionIntoMolecules(result).Count);
            }
        }

        private bool everythingOk(IEnumerable<IAtomContainer> structures)
        {
            StructureDiagramGenerator sdg;
            if (debug) Console.Out.WriteLine("number of structures in vector: " + structures.Count());
            foreach (var mol in structures)
            {
                sdg = new StructureDiagramGenerator { Molecule = mol };
                sdg.GenerateCoordinates(new Vector2(0, 1));
            }
            return true;
        }
    }
}
