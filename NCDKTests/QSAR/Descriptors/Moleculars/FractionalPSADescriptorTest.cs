/* Copyright (c) 2014 Collaborative Drug Discovery, Inc. <alex@collaborativedrug.com>
 *
 * Implemented by Alex M. Clark, produced by Collaborative Drug Discovery, Inc.
 * Made available to the CDK community under the terms of the GNU LGPL.
 *
 *    http://collaborativedrug.com
 *
 * Contact: cdk-devel@lists.sourceforge.net
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
using NCDK.IO;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    // @cdk.module test-qsarmolecular
    [TestClass()]
    public class FractionalPSADescriptorTest : MolecularDescriptorTest<FractionalPSADescriptor>
    {
        [TestMethod()]
        public void TestDescriptors()
        {
            string fnmol = "NCDK.Data.CDD.pyridineacid.mol";
            IAtomContainer mol;
            using (var mdl = new MDLV2000Reader(ResourceLoader.GetAsStream(fnmol)))
            {
                mol = mdl.Read(CDK.Builder.NewAtomContainer());
            }

            var fpsa = CreateDescriptor();
            var results = fpsa.Calculate(mol);

            // note: test currently assumes that just one Descriptor is calculated
            var names = results.Keys.ToReadOnlyList();
            if (names.Count != 1 || !names[0].Equals("tpsaEfficiency"))
                throw new CDKException("Only expecting 'tpsaEfficiency'");
            var tpsaEfficiency = results.Value;
            double ANSWER = 0.4036, ANSWER_LO = ANSWER * 0.999, ANSWER_HI = ANSWER * 1.001; // (we can tolerate rounding errors)
            if (tpsaEfficiency < ANSWER_LO || tpsaEfficiency > ANSWER_HI)
            {
                throw new CDKException($"Got {tpsaEfficiency}, expected {ANSWER}");
            }
        }

        // included to shutdown the warning messages for not having tests for trivial methods
        [TestMethod()]
        public void Nop() { }
    }
}
