/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
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
using NCDK.Config;
using NCDK.Default;
using NCDK.IO;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.QSAR
{
    /// <summary>
    /// TestSuite that runs all tests for the DescriptorEngine. 
    /// </summary>
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class DescriptorNamesTest : CDKTestCase
    {
        public DescriptorNamesTest() { }

        [TestMethod()]
        public void CheckUniqueMolecularDescriptorNames()
        {
            DescriptorEngine engine = new DescriptorEngine(new string[] { typeof(IMolecularDescriptor).FullName },
                    Default.ChemObjectBuilder.Instance);
            var specs = engine.GetDescriptorSpecifications();

            // we work with a simple molecule with 3D coordinates
            string filename = "NCDK.Data.MDL.lobtest2.sdf";
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            ISimpleChemObjectReader reader = new MDLV2000Reader(ins);
            ChemFile content = (ChemFile)reader.Read(new ChemFile());
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToList();
            IAtomContainer ac = (IAtomContainer)cList[0];
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(ac);
            Isotopes.Instance.ConfigureAtoms(ac);
            engine.Process(ac);

            int ncalc = 0;
            List<string> descNames = new List<string>();
            foreach (var spec in specs)
            {
                DescriptorValue value = (DescriptorValue)ac.GetProperty<DescriptorValue>(spec);
                if (value == null) Assert.Fail(spec.ImplementationTitle + " was not calculated.");
                ncalc++;
                string[] names = value.Names;
                descNames.AddRange(names);
            }

            List<string> dups = new List<string>();
            var uniqueNames = new HashSet<string>();
            foreach (var name in descNames)
            {
                if (!uniqueNames.Add(name)) dups.Add(name);
            }
            Assert.AreEqual(specs.Count, ncalc);
            Assert.AreEqual(descNames.Count, uniqueNames.Count);
            if (dups.Count != 0)
            {
                Console.Out.WriteLine("Following names were duplicated");
                foreach (var dup in dups)
                {
                    Console.Out.WriteLine("dup = " + dup);
                }
            }
        }
    }
}
