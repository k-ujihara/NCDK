/* Copyright (C) 2006-2007  Egon Willighagen <ewilligh@uni-koeln.de>
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
using NCDK.AtomTypes;
using NCDK.IO;
using NCDK.Silent;
using NCDK.Smiles;
using NCDK.Tools;
using NCDK.Tools.Manipulator;
using System.IO;
using System.Text;

namespace NCDK.Graphs.Invariant
{
    /// <summary>
    /// Checks the functionality of the CanonicalLabeler.
    /// </summary>
    // @cdk.module test-standard
    // CanonicalLabeler is deprecated (slow)
    [TestClass()]
    public class CanonicalLabelerTest : CDKTestCase
    {
        private SmilesParser parser = CDK.SmilesParser;
        private CanonicalLabeler labeler = new CanonicalLabeler();

        public CanonicalLabelerTest()
            : base()
        { }

        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestCanonicalLabeler()
        {
            // assume setup worked
            Assert.IsNotNull(labeler);
        }

        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestCanonLabel_IAtomContainer()
        {
            var molecule = parser.ParseSmiles("CC(=O)CBr");

            labeler.CanonLabel(molecule);
            foreach (var atom in molecule.Atoms)
            {
                Assert.IsNotNull(atom.GetProperty<long?>(InvPair.CanonicalLabelPropertyKey));
            }

            Assert.AreEqual(3, molecule.Atoms[0].GetProperty<long>(InvPair.CanonicalLabelPropertyKey));
            Assert.AreEqual(2, molecule.Atoms[1].GetProperty<long>(InvPair.CanonicalLabelPropertyKey));
            Assert.AreEqual(1, molecule.Atoms[2].GetProperty<long>(InvPair.CanonicalLabelPropertyKey));
            Assert.AreEqual(4, molecule.Atoms[3].GetProperty<long>(InvPair.CanonicalLabelPropertyKey));
            Assert.AreEqual(5, molecule.Atoms[4].GetProperty<long>(InvPair.CanonicalLabelPropertyKey));
        }

        /// <summary>
        /// Ordering of original should not matter, so the same SMILES
        /// with a different atom order as the test above.
        /// </summary>
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestSomeMoleculeWithDifferentStartingOrder()
        {
            var molecule = parser.ParseSmiles("O=C(C)CBr");
            labeler.CanonLabel(molecule);
            foreach (var atom in molecule.Atoms)
            {
                Assert.IsNotNull(atom.GetProperty<long>(InvPair.CanonicalLabelPropertyKey));
            }
            Assert.AreEqual(1, molecule.Atoms[0].GetProperty<long>(InvPair.CanonicalLabelPropertyKey));
            Assert.AreEqual(2, molecule.Atoms[1].GetProperty<long>(InvPair.CanonicalLabelPropertyKey));
            Assert.AreEqual(3, molecule.Atoms[2].GetProperty<long>(InvPair.CanonicalLabelPropertyKey));
            Assert.AreEqual(4, molecule.Atoms[3].GetProperty<long>(InvPair.CanonicalLabelPropertyKey));
            Assert.AreEqual(5, molecule.Atoms[4].GetProperty<long>(InvPair.CanonicalLabelPropertyKey));
        }

        // @cdk.bug 1014344
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestStabilityAfterRoundtrip()
        {
            var filename = "NCDK.Data.MDL.bug1014344-1.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            var reader = new MDLReader(ins, ChemObjectReaderMode.Strict);
            var mol1 = reader.Read(new AtomContainer());
            AddImplicitHydrogens(mol1);
            var output = new StringWriter();
            var cmlWriter = new CMLWriter(output);
            cmlWriter.Write(mol1);
            var cmlreader = new CMLReader(new MemoryStream(Encoding.UTF8.GetBytes(output.ToString())));
            var mol2 = ((IChemFile)cmlreader.Read(new ChemFile()))[0][0].MoleculeSet[0];
            AddImplicitHydrogens(mol2);

            labeler.CanonLabel(mol1);
            labeler.CanonLabel(mol2);
            var atoms1 = mol1.Atoms.GetEnumerator();
            var atoms2 = mol2.Atoms.GetEnumerator();
            while (atoms1.MoveNext())
            {
                atoms2.MoveNext();
                var atom1 = atoms1.Current;
                var atom2 = atoms2.Current;
                Assert.AreEqual(atom1.GetProperty<long>(InvPair.CanonicalLabelPropertyKey), atom2.GetProperty<long>(InvPair.CanonicalLabelPropertyKey));
            }
        }

        /// <summary>
        /// Convenience method that perceives atom types (CDK scheme) and
        /// adds implicit hydrogens accordingly. It does not create 2D or 3D
        /// coordinates for the new hydrogens.
        /// </summary>
        /// <param name="container">to which implicit hydrogens are added.</param>
        protected override void AddImplicitHydrogens(IAtomContainer container)
        {
            var matcher = CDK.AtomTypeMatcher;
            foreach (var atom in container.Atoms)
            {
                var type = matcher.FindMatchingAtomType(container, atom);
                AtomTypeManipulator.Configure(atom, type);
            }
            var hAdder = CDK.HydrogenAdder;
            hAdder.AddImplicitHydrogens(container);
        }

        // @cdk.bug 2944519
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestBug2944519()
        {
            var ac = ChemObjectBuilder.Instance.NewAtomContainer();
            ac.Atoms.Add(ac.Builder.NewAtom("C"));
            ac.Atoms.Add(ac.Builder.NewAtom("O"));
            ac.AddBond(ac.Atoms[0], ac.Atoms[1], BondOrder.Single);
            var canLabler = new CanonicalLabeler();
            canLabler.CanonLabel(ac);
            var ac2 = ChemObjectBuilder.Instance.NewAtomContainer();
            ac2.Atoms.Add(ac2.Builder.NewAtom("O"));
            ac2.Atoms.Add(ac2.Builder.NewAtom("C"));
            ac2.AddBond(ac2.Atoms[0], ac2.Atoms[1], BondOrder.Single);
            canLabler.CanonLabel(ac2);
            Assert.AreEqual(ac.Atoms[0].GetProperty<long>(InvPair.CanonicalLabelPropertyKey),
                    ac2.Atoms[1].GetProperty<long>(InvPair.CanonicalLabelPropertyKey));
            Assert.AreEqual(ac.Atoms[1].GetProperty<long>(InvPair.CanonicalLabelPropertyKey),
                    ac2.Atoms[0].GetProperty<long>(InvPair.CanonicalLabelPropertyKey));
        }
    }
}
