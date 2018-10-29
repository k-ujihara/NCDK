/* Copyright (C) 2012 Daniel Szisz
 *
 * Contact: orlando@caesar.elte.hu
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
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.RingSearches;
using NCDK.Silent;
using NCDK.Smiles;
using NCDK.Tools.Manipulator;
using System;
using System.Reflection;

namespace NCDK.Modelings.Builder3D
{
    /// <summary>
    /// Test class for <see cref="TemplateHandler3D"/>.
    /// </summary>
    // @author danielszisz
    // @created 05/14/2012
    // @cdk.module test-builder3d
    [TestClass()]
    public class FurtherTemplateHandler3DTest
    {
        [TestMethod()]
        public void TestLoadTemplates()
        {
            // test order is not guaranteed so the templates may have already been loaded,
            // to avoid this we create a new instance using reflection. This is a hack and
            // requires changing if the underlying class is modified
            var constructor = typeof(TemplateHandler3D).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, Type.EmptyTypes, null);

            var tmphandler3d = (TemplateHandler3D)constructor.Invoke(Array.Empty<object>());
            Assert.AreEqual(0, tmphandler3d.TemplateCount);
            //cannot test TemplateHandler3D#loadTemplates as it is a private method

            // but we can using reflection ...
            var loadTemplates = typeof(TemplateHandler3D).GetMethod("LoadTemplates", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
            loadTemplates.Invoke(tmphandler3d, Array.Empty<object>());
            Assert.AreEqual(10751, tmphandler3d.TemplateCount);
        }

        [TestCategory("SlowTest")]
        [TestMethod()]
        public void TestMapTemplatesCyclicMol1()
        {
            TemplateHandler3D tmphandler3d = TemplateHandler3D.Instance;
            string cyclicMolSmi = "O(CC(O)CN1CCN(CC1)CC(=O)Nc1c(cccc1C)C)c1c(cccc1)OC";
            IChemObjectBuilder builder = ChemObjectBuilder.Instance;
            SmilesParser smiparser = new SmilesParser(builder);
            var molecule = smiparser.ParseSmiles(cyclicMolSmi);
            ForceFieldConfigurator forcefconf = new ForceFieldConfigurator();
            forcefconf.SetForceFieldConfigurator("mmff94", builder);
            IRingSet rings = forcefconf.AssignAtomTyps(molecule);
            var ringSystems = RingPartitioner.PartitionRings(rings);
            IRingSet largestRingSet = RingSetManipulator.GetLargestRingSet(ringSystems);
            IAtomContainer allAtomsInOneContainer = RingSetManipulator.GetAllInOneContainer(largestRingSet);
            tmphandler3d.MapTemplates(allAtomsInOneContainer, allAtomsInOneContainer.Atoms.Count);
            for (int j = 0; j < allAtomsInOneContainer.Atoms.Count; j++)
            {
                Assert.IsNotNull(allAtomsInOneContainer.Atoms[j].Point3D);
            }
        }

        [TestCategory("SlowTest")]
        [TestMethod()]
        public void TestMapTemplatesCyclicMol2()
        {
            TemplateHandler3D tmphandler3d = TemplateHandler3D.Instance;
            string cyclicMolSmi = "CC(C)(C)NC(=O)C1CN(CCN1CC(CC(Cc1ccccc1)C(=O)NC1c2ccccc2CC1O)O)Cc1cccnc1";
            IChemObjectBuilder builder = ChemObjectBuilder.Instance;
            SmilesParser smiparser = new SmilesParser(builder);
            var molecule = smiparser.ParseSmiles(cyclicMolSmi);
            ForceFieldConfigurator forcefconf = new ForceFieldConfigurator();
            forcefconf.SetForceFieldConfigurator("mmff94", builder);
            IRingSet rings = forcefconf.AssignAtomTyps(molecule);
            var ringSystems = RingPartitioner.PartitionRings(rings);
            IRingSet largestRingSet = RingSetManipulator.GetLargestRingSet(ringSystems);
            IAtomContainer allAtomsInOneContainer = RingSetManipulator.GetAllInOneContainer(largestRingSet);
            tmphandler3d.MapTemplates(allAtomsInOneContainer, allAtomsInOneContainer.Atoms.Count);
            for (int j = 0; j < allAtomsInOneContainer.Atoms.Count; j++)
            {
                Assert.IsNotNull(allAtomsInOneContainer.Atoms[j].Point3D);
            }
        }
    }
}
