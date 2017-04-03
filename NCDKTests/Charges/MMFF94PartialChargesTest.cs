/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Smiles;

namespace NCDK.Charges
{
    /// <summary>
    ///  TestSuite that runs a test for the MMFF94PartialCharges.
    /// </summary>
    // @cdk.module test-forcefield
    // @author        cubic
    // @cdk.created       2004-11-04
    [TestClass()]
    public class MMFF94PartialChargesTest : CDKTestCase
    {
        /// <summary>
        /// A unit test with beta-amino-acetic-acid
        /// </summary>
        [TestMethod()]
        public void TestMMFF94PartialCharges()
        {
            double[] testResult = { -0.99, 0.314, 0.66, -0.57, -0.65, 0.36, 0.36, 0, 0, 0.5 };
            SmilesParser sp = new SmilesParser(Default.ChemObjectBuilder.Instance);
            IAtomContainer ac = sp.ParseSmiles("NCC(=O)O");
            AddExplicitHydrogens(ac);
            MMFF94PartialCharges mmff = new MMFF94PartialCharges();
            mmff.AssignMMFF94PartialCharges(ac);
            for (int i = 0; i < ac.Atoms.Count; i++)
            {
                Assert.AreEqual(testResult[i], (ac.Atoms[i].GetProperty<double>("MMFF94charge")), 0.05);
                //Debug.WriteLine("CHARGE AT " + ac.GetAtomAt(i).Symbol + " " + ac.GetAtomAt(i).GetProperty("MMFF94charge"));
            }
        }
    }
}
