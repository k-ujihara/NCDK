/*
 * Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
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
using NCDK.IO;
using NCDK.Tools.Manipulator;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class EccentricConnectivityIndexDescriptorTest : MolecularDescriptorTest<EccentricConnectivityIndexDescriptor>
    {
        IChemObjectBuilder builder = CDK.Builder;

        [TestMethod()]
        public void TestEhccentricConnectivityIndex()
        {
            string filename = "NCDK.Data.HIN.gravindex.hin";
            IChemFile content;
            using (var reader = new HINReader(ResourceLoader.GetAsStream(filename)))
            {
                content = reader.Read(builder.NewChemFile());
            }
            var cList = ChemFileManipulator.GetAllAtomContainers(content).ToReadOnlyList();
            var ac = cList[0];

            AddImplicitHydrogens(ac);

            var retval = CreateDescriptor(ac).Calculate().Value;

            Assert.AreEqual(254, retval, 0);
        }
    }
}
