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
 *
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.AtomTypes;
using NCDK.Default;
using NCDK.IO;
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
        private SmilesParser parser = new SmilesParser(Default.ChemObjectBuilder.Instance);
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
            IAtomContainer molecule = parser.ParseSmiles("CC(=O)CBr");

            labeler.CanonLabel(molecule);
            foreach (var atom in molecule.Atoms)
            {
                Assert.IsNotNull(atom.GetProperty<long?>(InvPair.CANONICAL_LABEL));
            }

            Assert.AreEqual(3, molecule.Atoms[0].GetProperty<long>(InvPair.CANONICAL_LABEL));
            Assert.AreEqual(2, molecule.Atoms[1].GetProperty<long>(InvPair.CANONICAL_LABEL));
            Assert.AreEqual(1, molecule.Atoms[2].GetProperty<long>(InvPair.CANONICAL_LABEL));
            Assert.AreEqual(4, molecule.Atoms[3].GetProperty<long>(InvPair.CANONICAL_LABEL));
            Assert.AreEqual(5, molecule.Atoms[4].GetProperty<long>(InvPair.CANONICAL_LABEL));
        }

        /// <summary>
        /// Ordering of original should not matter, so the same SMILES
        /// with a different atom order as the test above.
        /// </summary>
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestSomeMoleculeWithDifferentStartingOrder()
        {
            IAtomContainer molecule = parser.ParseSmiles("O=C(C)CBr");
            labeler.CanonLabel(molecule);
            foreach (var atom in molecule.Atoms)
            {
                Assert.IsNotNull(atom.GetProperty<long>(InvPair.CANONICAL_LABEL));
            }
            Assert.AreEqual(1, molecule.Atoms[0].GetProperty<long>(InvPair.CANONICAL_LABEL));
            Assert.AreEqual(2, molecule.Atoms[1].GetProperty<long>(InvPair.CANONICAL_LABEL));
            Assert.AreEqual(3, molecule.Atoms[2].GetProperty<long>(InvPair.CANONICAL_LABEL));
            Assert.AreEqual(4, molecule.Atoms[3].GetProperty<long>(InvPair.CANONICAL_LABEL));
            Assert.AreEqual(5, molecule.Atoms[4].GetProperty<long>(InvPair.CANONICAL_LABEL));
        }

        // @cdk.bug 1014344
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestStabilityAfterRoundtrip()
        {
            string filename = "NCDK.Data.MDL.bug1014344-1.mol";
            var ins = ResourceLoader.GetAsStream(filename);
            MDLReader reader = new MDLReader(ins, ChemObjectReaderModes.Strict);
            IAtomContainer mol1 = reader.Read(new AtomContainer());
            AddImplicitHydrogens(mol1);
            StringWriter output = new StringWriter();
            CMLWriter cmlWriter = new CMLWriter(output);
            cmlWriter.Write(mol1);
            CMLReader cmlreader = new CMLReader(new MemoryStream(Encoding.UTF8.GetBytes(output.ToString())));
            IAtomContainer mol2 = ((IChemFile)cmlreader.Read(new ChemFile()))[0][0]
                    .MoleculeSet[0];
            AddImplicitHydrogens(mol2);

            labeler.CanonLabel(mol1);
            labeler.CanonLabel(mol2);
            var atoms1 = mol1.Atoms.GetEnumerator();
            var atoms2 = mol2.Atoms.GetEnumerator();
            while (atoms1.MoveNext())
            {
                atoms2.MoveNext();
                IAtom atom1 = atoms1.Current;
                IAtom atom2 = atoms2.Current;
                Assert.AreEqual(atom1.GetProperty<long>(InvPair.CANONICAL_LABEL), atom2.GetProperty<long>(InvPair.CANONICAL_LABEL));
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
            CDKAtomTypeMatcher matcher = CDKAtomTypeMatcher.GetInstance(container.Builder);
            foreach (var atom in container.Atoms)
            {
                IAtomType type = matcher.FindMatchingAtomType(container, atom);
                AtomTypeManipulator.Configure(atom, type);
            }
            CDKHydrogenAdder hAdder = CDKHydrogenAdder.GetInstance(container.Builder);
            hAdder.AddImplicitHydrogens(container);
        }

        // @cdk.bug 2944519
        [TestMethod()]
        [TestCategory("SlowTest")]
        public void TestBug2944519()
        {
            IAtomContainer ac = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            ac.Atoms.Add(ac.Builder.CreateAtom("C"));
            ac.Atoms.Add(ac.Builder.CreateAtom("O"));
            ac.AddBond(ac.Atoms[0], ac.Atoms[1], BondOrder.Single);
            CanonicalLabeler canLabler = new CanonicalLabeler();
            canLabler.CanonLabel(ac);
            IAtomContainer ac2 = Default.ChemObjectBuilder.Instance.CreateAtomContainer();
            ac2.Atoms.Add(ac2.Builder.CreateAtom("O"));
            ac2.Atoms.Add(ac2.Builder.CreateAtom("C"));
            ac2.AddBond(ac2.Atoms[0], ac2.Atoms[1], BondOrder.Single);
            canLabler.CanonLabel(ac2);
            Assert.AreEqual(ac.Atoms[0].GetProperty<long>(InvPair.CANONICAL_LABEL),
                    ac2.Atoms[1].GetProperty<long>(InvPair.CANONICAL_LABEL));
            Assert.AreEqual(ac.Atoms[1].GetProperty<long>(InvPair.CANONICAL_LABEL),
                    ac2.Atoms[0].GetProperty<long>(InvPair.CANONICAL_LABEL));
        }
    }
}
