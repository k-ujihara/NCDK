/* Copyright (C) 2008  Rajarshi Guha
 *
 * Contact: cdk-devel@slists.sourceforge.net
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
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *  */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Tools.Manipulator;
using System.Diagnostics;
using System.IO;

namespace NCDK.IO.Iterator
{
    /**
     * @cdk.module test-io
     */
     [TestClass()]
    public class IteratingPCSubstancesXMLReaderTest : CDKTestCase
    {
        [TestMethod()]
        public void TestTaxols()
        {
            string filename = "NCDK.Data.ASN.PubChem.taxols.xml";
            Trace.TraceInformation("Testing: " + filename);

            int modelCount = 0;
            IChemSequence set;
            using (var ins = this.GetType().Assembly.GetManifestResourceStream(filename))
            using (var sr = new StreamReader(ins))
            using (var reader = new IteratingPCSubstancesXMLReader(sr, Default.ChemObjectBuilder.Instance))
            {
                set = Default.ChemObjectBuilder.Instance.CreateChemSequence();
                foreach (var obj in reader)
                {
                    Assert.IsNotNull(obj);
                    Assert.IsTrue(obj is IChemModel);
                    set.Add((IChemModel)obj);
                    modelCount++;
                }
            }

            Assert.AreEqual(77, modelCount);
            IChemModel first = set[0];
            Assert.AreEqual(63, ChemModelManipulator.GetAtomCount(first));
            Assert.AreEqual(69, ChemModelManipulator.GetBondCount(first));
        }
    }
}
