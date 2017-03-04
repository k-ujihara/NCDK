/* Copyright (C) 2007  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Smiles;

namespace NCDK.Tools.Manipulator
{
    /// <summary>
    // @cdk.module test-standard
    /// </summary>
    [TestClass()]
    public class AminoAcidManipulatorTest : CDKTestCase
    {
        private static readonly IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;

        public AminoAcidManipulatorTest()
            : base()
        { }

        [TestMethod()]
        public void TestRemoveAcidicOxygen_IAminoAcid()
        {
            IAminoAcid glycine = builder.CreateAminoAcid();
            glycine.Add(new SmilesParser(builder).ParseSmiles("C(C(=O)O)N"));
            Assert.AreEqual(5, glycine.Atoms.Count);
            glycine.AddCTerminus(glycine.Atoms[1]);
            AminoAcidManipulator.RemoveAcidicOxygen(glycine);
            Assert.AreEqual(4, glycine.Atoms.Count);
        }

        /// <summary>
        // @cdk.bug 1646861
        /// </summary>
        [TestMethod()]
        public void TestAddAcidicOxygen_IAminoAcid()
        {
            // FIXME: I think this is the proper test, but it currently fails
            IAminoAcid glycine = builder.CreateAminoAcid();
            glycine.Add(new SmilesParser(builder).ParseSmiles("C(C=O)N"));
            Assert.AreEqual(4, glycine.Atoms.Count);
            glycine.AddCTerminus(glycine.Atoms[1]);
            AminoAcidManipulator.AddAcidicOxygen(glycine);
            Assert.AreEqual(5, glycine.Atoms.Count);
        }
    }
}
