/* Copyright (C) 2018  Jeffrey Plante (Lhasa Limited)  <Jeffrey.Plante@lhasalimited.org>
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
using NCDK.Aromaticities;
using NCDK.Silent;
using NCDK.Tools.Manipulator;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    [TestClass()]
    public class AtomTyperTests
    {
        [TestMethod()]
        public void TestIsPolar()
        {
            Assert.IsTrue(JPlogPDescriptor.JPlogPCalculator.IsPolar(new Atom("O")));
            Assert.IsTrue(JPlogPDescriptor.JPlogPCalculator.IsPolar(new Atom("S")));
            Assert.IsTrue(JPlogPDescriptor.JPlogPCalculator.IsPolar(new Atom("N")));
            Assert.IsTrue(JPlogPDescriptor.JPlogPCalculator.IsPolar(new Atom("P")));
            Assert.IsFalse(JPlogPDescriptor.JPlogPCalculator.IsPolar(new Atom("C")));
        }

        [TestMethod()]
        public void TestIsElectronWithdrawing()
        {
            Assert.IsTrue(JPlogPDescriptor.JPlogPCalculator.ElectronWithdrawing(new Atom("O")));
            Assert.IsTrue(JPlogPDescriptor.JPlogPCalculator.ElectronWithdrawing(new Atom("S")));
            Assert.IsTrue(JPlogPDescriptor.JPlogPCalculator.ElectronWithdrawing(new Atom("N")));
            Assert.IsTrue(JPlogPDescriptor.JPlogPCalculator.ElectronWithdrawing(new Atom("S")));
            Assert.IsTrue(JPlogPDescriptor.JPlogPCalculator.ElectronWithdrawing(new Atom("F")));
            Assert.IsTrue(JPlogPDescriptor.JPlogPCalculator.ElectronWithdrawing(new Atom("Cl")));
            Assert.IsTrue(JPlogPDescriptor.JPlogPCalculator.ElectronWithdrawing(new Atom("Br")));
            Assert.IsTrue(JPlogPDescriptor.JPlogPCalculator.ElectronWithdrawing(new Atom("I")));
            Assert.IsFalse(JPlogPDescriptor.JPlogPCalculator.ElectronWithdrawing(new Atom("C")));
        }

        [TestMethod()]
        public void TestNonHNeighbours()
        {
            IAtomContainer molecule;
            IAtom atom;

            molecule = ParseSmiles("CC");
            atom = molecule.Atoms[1];
            Assert.AreEqual(1, JPlogPDescriptor.JPlogPCalculator.NonHNeighbours(atom));

            molecule = ParseSmiles("C");
            atom = molecule.Atoms[0];
            Assert.AreEqual(0, JPlogPDescriptor.JPlogPCalculator.NonHNeighbours(atom));

            molecule = ParseSmiles("C(C)C");
            atom = molecule.Atoms[0];
            Assert.AreEqual(2, JPlogPDescriptor.JPlogPCalculator.NonHNeighbours(atom));

            molecule = ParseSmiles("C(C)(C)C");
            atom = molecule.Atoms[0];
            Assert.AreEqual(3, JPlogPDescriptor.JPlogPCalculator.NonHNeighbours(atom));

            molecule = ParseSmiles("C(C)(C)(C)C");
            atom = molecule.Atoms[0];
            Assert.AreEqual(4, JPlogPDescriptor.JPlogPCalculator.NonHNeighbours(atom));
        }

        [TestMethod()]
        public void TestDoubleBondHetero()
        {
            var desc = new JPlogPDescriptor();
            var molecule = ParseSmiles("c1nnccc1");
            var atom = molecule.Atoms[1];
            Assert.IsFalse(JPlogPDescriptor.JPlogPCalculator.DoubleBondHetero(atom));

            molecule = ParseSmiles("CC(=O)C");
            atom = molecule.Atoms[1];
            Assert.IsTrue(JPlogPDescriptor.JPlogPCalculator.DoubleBondHetero(atom));
        }

        [TestMethod()]
        public void TestCarbonylConjugated()
        {
            var molecule = ParseSmiles("c1nnccc1");
            var atom = molecule.Atoms[0];
            Assert.IsFalse(JPlogPDescriptor.JPlogPCalculator.CarbonylConjugated(atom));

            molecule = ParseSmiles("C=C=C");
            atom = molecule.Atoms[0];
            Assert.IsFalse(JPlogPDescriptor.JPlogPCalculator.CarbonylConjugated(atom));

            molecule = ParseSmiles("CC(=O)C");
            atom = molecule.Atoms[0];
            Assert.IsTrue(JPlogPDescriptor.JPlogPCalculator.CarbonylConjugated(atom));
        }

        [TestMethod()]
        public void TestNextToAromatic()
        {
            var desc = new JPlogPDescriptor();
            var molecule = ParseSmiles("c1nnccc1");
            var atom = molecule.Atoms[0];
            Assert.IsFalse(JPlogPDescriptor.JPlogPCalculator.NextToAromatic(atom));

            molecule = ParseSmiles("C=C=C");
            atom = molecule.Atoms[0];
            Assert.IsFalse(JPlogPDescriptor.JPlogPCalculator.NextToAromatic(atom));

            molecule = ParseSmiles("Nc1ccccc1");
            atom = molecule.Atoms[0];
            Assert.IsTrue(JPlogPDescriptor.JPlogPCalculator.NextToAromatic(atom));
        }

        [TestMethod()]
        public void TestGetPolarBondArray()
        {
            var desc = new JPlogPDescriptor();
            var molecule = ParseSmiles("c1nnccc1");
            var atom = molecule.Atoms[0];
            Assert.AreEqual(1, JPlogPDescriptor.JPlogPCalculator.GetPolarBondArray(atom)[1]);

            molecule = ParseSmiles("CO");
            atom = molecule.Atoms[0];
            Assert.AreEqual(1, JPlogPDescriptor.JPlogPCalculator.GetPolarBondArray(atom)[0]);

            molecule = ParseSmiles("C=O");
            atom = molecule.Atoms[0];
            Assert.AreEqual(1, JPlogPDescriptor.JPlogPCalculator.GetPolarBondArray(atom)[2]);

            molecule = ParseSmiles("C#N");
            atom = molecule.Atoms[0];
            Assert.AreEqual(1, JPlogPDescriptor.JPlogPCalculator.GetPolarBondArray(atom)[3]);
        }

        [TestMethod()]
        public void TestBoundTo()
        {
            var molecule = ParseSmiles("CO");
            var atom = molecule.Atoms[0];
            Assert.IsTrue(JPlogPDescriptor.JPlogPCalculator.BoundTo(atom, AtomicNumbers.O));
            Assert.IsFalse(JPlogPDescriptor.JPlogPCalculator.BoundTo(atom, AtomicNumbers.S));
        }

        [TestMethod()]
        public void TestCheckAlphaCarbonyl()
        {
            var molecule = ParseSmiles("O=CN");
            var atom = molecule.Atoms[0];
            Assert.IsTrue(JPlogPDescriptor.JPlogPCalculator.CheckAlphaCarbonyl(atom, AtomicNumbers.N));
            Assert.IsFalse(JPlogPDescriptor.JPlogPCalculator.CheckAlphaCarbonyl(atom, AtomicNumbers.S));
        }

        [TestMethod()]
        public void TestGetHydrogenSpecial()
        {
            var molecule = ParseSmiles("HO");
            var atom = molecule.Atoms[0];
            Assert.AreEqual(50, JPlogPDescriptor.JPlogPCalculator.GetHydrogenSpecial(atom)); // DD = 50

            molecule = ParseSmiles("HCC=O");
            atom = molecule.Atoms[0];
            Assert.AreEqual(51, JPlogPDescriptor.JPlogPCalculator.GetHydrogenSpecial(atom)); // DD = 51

            molecule = ParseSmiles("HC");
            atom = molecule.Atoms[0];
            Assert.AreEqual(46, JPlogPDescriptor.JPlogPCalculator.GetHydrogenSpecial(atom)); // DD = 46

            molecule = ParseSmiles("HCF");
            atom = molecule.Atoms[0];
            Assert.AreEqual(47, JPlogPDescriptor.JPlogPCalculator.GetHydrogenSpecial(atom)); // DD = 47

            molecule = ParseSmiles("HC(F)F");
            atom = molecule.Atoms[0];
            Assert.AreEqual(48, JPlogPDescriptor.JPlogPCalculator.GetHydrogenSpecial(atom)); // DD = 48

            molecule = ParseSmiles("HC(F)(F)O");
            atom = molecule.Atoms[0];
            Assert.AreEqual(49, JPlogPDescriptor.JPlogPCalculator.GetHydrogenSpecial(atom)); // DD = 49

            molecule = ParseSmiles("HC=C");
            atom = molecule.Atoms[0];
            Assert.AreEqual(47, JPlogPDescriptor.JPlogPCalculator.GetHydrogenSpecial(atom)); // DD = 47

            molecule = ParseSmiles("HC(=C)O");
            atom = molecule.Atoms[0];
            Assert.AreEqual(48, JPlogPDescriptor.JPlogPCalculator.GetHydrogenSpecial(atom)); // DD = 48

            molecule = ParseSmiles("HC(=O)O");
            atom = molecule.Atoms[0];
            Assert.AreEqual(49, JPlogPDescriptor.JPlogPCalculator.GetHydrogenSpecial(atom)); // DD = 49

            molecule = ParseSmiles("HC#C");
            atom = molecule.Atoms[0];
            Assert.AreEqual(48, JPlogPDescriptor.JPlogPCalculator.GetHydrogenSpecial(atom)); // DD = 48

            molecule = ParseSmiles("HC#N");
            atom = molecule.Atoms[0];
            Assert.AreEqual(49, JPlogPDescriptor.JPlogPCalculator.GetHydrogenSpecial(atom)); // DD = 49
        }

        [TestMethod()]
        public void TestGetDefaultSpecial()
        {
            var molecule = ParseSmiles("P(=O)(O)(O)C");
            var atom = molecule.Atoms[0];
            Assert.AreEqual(3, JPlogPDescriptor.JPlogPCalculator.GetDefaultSpecial(atom)); // DD = 03

            molecule = ParseSmiles("o1cccc1");
            atom = molecule.Atoms[0];
            Assert.AreEqual(10, JPlogPDescriptor.JPlogPCalculator.GetDefaultSpecial(atom)); // DD = 10
        }

        [TestMethod()]
        public void TestGetFluorineSpecial()
        {
            var molecule = ParseSmiles("FS");
            var atom = molecule.Atoms[0];
            Assert.AreEqual(8, JPlogPDescriptor.JPlogPCalculator.GetFluorineSpecial(atom)); // DD = 08

            molecule = ParseSmiles("FB");
            atom = molecule.Atoms[0];
            Assert.AreEqual(9, JPlogPDescriptor.JPlogPCalculator.GetFluorineSpecial(atom)); // DD = 09

            molecule = ParseSmiles("FI");
            atom = molecule.Atoms[0];
            Assert.AreEqual(1, JPlogPDescriptor.JPlogPCalculator.GetFluorineSpecial(atom)); // DD = 01

            molecule = ParseSmiles("FC#C");
            atom = molecule.Atoms[0];
            Assert.AreEqual(2, JPlogPDescriptor.JPlogPCalculator.GetFluorineSpecial(atom)); // DD = 02

            molecule = ParseSmiles("FC=C");
            atom = molecule.Atoms[0];
            Assert.AreEqual(3, JPlogPDescriptor.JPlogPCalculator.GetFluorineSpecial(atom)); // DD = 03

            molecule = ParseSmiles("FC(C)(C)C");
            atom = molecule.Atoms[0];
            Assert.AreEqual(5, JPlogPDescriptor.JPlogPCalculator.GetFluorineSpecial(atom)); // DD = 05

            molecule = ParseSmiles("FC(F)(F)F");
            atom = molecule.Atoms[0];
            Assert.AreEqual(7, JPlogPDescriptor.JPlogPCalculator.GetFluorineSpecial(atom)); // DD = 07

            molecule = ParseSmiles("F(F)(F)F");
            atom = molecule.Atoms[0];
            Assert.AreEqual(99, JPlogPDescriptor.JPlogPCalculator.GetFluorineSpecial(atom)); // DD = 99 Nonsense Fluorine
        }

        [TestMethod()]
        public void TestGetOxygenSpecial()
        {
            var molecule = ParseSmiles("ON");
            var atom = molecule.Atoms[0];
            Assert.AreEqual(1, JPlogPDescriptor.JPlogPCalculator.GetOxygenSpecial(atom)); // DD = 01

            molecule = ParseSmiles("OS");
            atom = molecule.Atoms[0];
            Assert.AreEqual(2, JPlogPDescriptor.JPlogPCalculator.GetOxygenSpecial(atom)); // DD = 02

            molecule = ParseSmiles("OC");
            atom = molecule.Atoms[0];
            Assert.AreEqual(3, JPlogPDescriptor.JPlogPCalculator.GetOxygenSpecial(atom)); // DD = 03

            molecule = ParseSmiles("o1cccc1");
            atom = molecule.Atoms[0];
            Assert.AreEqual(8, JPlogPDescriptor.JPlogPCalculator.GetOxygenSpecial(atom)); // DD = 08

            molecule = ParseSmiles("O=N");
            atom = molecule.Atoms[0];
            Assert.AreEqual(4, JPlogPDescriptor.JPlogPCalculator.GetOxygenSpecial(atom)); // DD = 04

            molecule = ParseSmiles("O=S");
            atom = molecule.Atoms[0];
            Assert.AreEqual(5, JPlogPDescriptor.JPlogPCalculator.GetOxygenSpecial(atom)); // DD = 05

            molecule = ParseSmiles("O=CO");
            atom = molecule.Atoms[0];
            Assert.AreEqual(6, JPlogPDescriptor.JPlogPCalculator.GetOxygenSpecial(atom)); // DD = 06

            molecule = ParseSmiles("O=CN");
            atom = molecule.Atoms[0];
            Assert.AreEqual(9, JPlogPDescriptor.JPlogPCalculator.GetOxygenSpecial(atom)); // DD = 09

            molecule = ParseSmiles("O=CS");
            atom = molecule.Atoms[0];
            Assert.AreEqual(10, JPlogPDescriptor.JPlogPCalculator.GetOxygenSpecial(atom)); // DD = 10

            molecule = ParseSmiles("O=CC");
            atom = molecule.Atoms[0];
            Assert.AreEqual(7, JPlogPDescriptor.JPlogPCalculator.GetOxygenSpecial(atom)); // DD = 07
        }

        [TestMethod()]
        public void TestGetNitrogenSpecial()
        {
            var molecule = ParseSmiles("[N+](C)(C)(C)C");
            var atom = molecule.Atoms[0];
            Assert.AreEqual(9, JPlogPDescriptor.JPlogPCalculator.GetNitrogenSpecial(atom)); // DD = 09

            molecule = ParseSmiles("Nc1ccccc1");
            atom = molecule.Atoms[0];
            Assert.AreEqual(1, JPlogPDescriptor.JPlogPCalculator.GetNitrogenSpecial(atom)); // DD = 01

            molecule = ParseSmiles("NC=O");
            atom = molecule.Atoms[0];
            Assert.AreEqual(2, JPlogPDescriptor.JPlogPCalculator.GetNitrogenSpecial(atom)); // DD = 02

            molecule = ParseSmiles("[N+](=O)([O-])C");
            atom = molecule.Atoms[0];
            Assert.AreEqual(10, JPlogPDescriptor.JPlogPCalculator.GetNitrogenSpecial(atom)); // DD = 10

            molecule = ParseSmiles("NO");
            atom = molecule.Atoms[0];
            Assert.AreEqual(3, JPlogPDescriptor.JPlogPCalculator.GetNitrogenSpecial(atom)); // DD = 03

            molecule = ParseSmiles("NC");
            atom = molecule.Atoms[0];
            Assert.AreEqual(4, JPlogPDescriptor.JPlogPCalculator.GetNitrogenSpecial(atom)); // DD = 04

            molecule = ParseSmiles("n1ccccc1");
            atom = molecule.Atoms[0];
            Assert.AreEqual(5, JPlogPDescriptor.JPlogPCalculator.GetNitrogenSpecial(atom)); // DD = 05

            molecule = ParseSmiles("N=O");
            atom = molecule.Atoms[0];
            Assert.AreEqual(6, JPlogPDescriptor.JPlogPCalculator.GetNitrogenSpecial(atom)); // DD = 06

            molecule = ParseSmiles("N=C");
            atom = molecule.Atoms[0];
            Assert.AreEqual(7, JPlogPDescriptor.JPlogPCalculator.GetNitrogenSpecial(atom)); // DD = 07

            molecule = ParseSmiles("N#C");
            atom = molecule.Atoms[0];
            Assert.AreEqual(8, JPlogPDescriptor.JPlogPCalculator.GetNitrogenSpecial(atom)); // DD = 08
        }

        [TestMethod()]
        public void TestGetCarbonSpecial()
        {
            var molecule = ParseSmiles("CC");
            var atom = molecule.Atoms[0];
            Assert.AreEqual(2, JPlogPDescriptor.JPlogPCalculator.GetCarbonSpecial(atom)); // DD = 02

            molecule = ParseSmiles("CO");
            atom = molecule.Atoms[0];
            Assert.AreEqual(3, JPlogPDescriptor.JPlogPCalculator.GetCarbonSpecial(atom)); // DD = 03

            molecule = ParseSmiles("c1ncccc1");
            atom = molecule.Atoms[0];
            Assert.AreEqual(11, JPlogPDescriptor.JPlogPCalculator.GetCarbonSpecial(atom)); // DD = 11

            molecule = ParseSmiles("c1(O)ccccc1");
            atom = molecule.Atoms[0];
            Assert.AreEqual(5, JPlogPDescriptor.JPlogPCalculator.GetCarbonSpecial(atom)); // DD = 05

            molecule = ParseSmiles("c1(O)ncccc1");
            atom = molecule.Atoms[0];
            Assert.AreEqual(13, JPlogPDescriptor.JPlogPCalculator.GetCarbonSpecial(atom)); // DD = 13

            molecule = ParseSmiles("c1ccccc1");
            atom = molecule.Atoms[0];
            Assert.AreEqual(4, JPlogPDescriptor.JPlogPCalculator.GetCarbonSpecial(atom)); // DD = 04

            molecule = ParseSmiles("C=O");
            atom = molecule.Atoms[0];
            Assert.AreEqual(7, JPlogPDescriptor.JPlogPCalculator.GetCarbonSpecial(atom)); // DD = 07

            molecule = ParseSmiles("C(=C)O");
            atom = molecule.Atoms[0];
            Assert.AreEqual(8, JPlogPDescriptor.JPlogPCalculator.GetCarbonSpecial(atom)); // DD = 08

            molecule = ParseSmiles("C(=O)O");
            atom = molecule.Atoms[0];
            Assert.AreEqual(14, JPlogPDescriptor.JPlogPCalculator.GetCarbonSpecial(atom)); // DD = 14

            molecule = ParseSmiles("C=C");
            atom = molecule.Atoms[0];
            Assert.AreEqual(6, JPlogPDescriptor.JPlogPCalculator.GetCarbonSpecial(atom)); // DD = 06

            molecule = ParseSmiles("C#N");
            atom = molecule.Atoms[0];
            Assert.AreEqual(12, JPlogPDescriptor.JPlogPCalculator.GetCarbonSpecial(atom)); // DD = 12

            molecule = ParseSmiles("C(O)#C");
            atom = molecule.Atoms[0];
            Assert.AreEqual(10, JPlogPDescriptor.JPlogPCalculator.GetCarbonSpecial(atom)); // DD = 10

            molecule = ParseSmiles("C(O)#N");
            atom = molecule.Atoms[0];
            Assert.AreEqual(15, JPlogPDescriptor.JPlogPCalculator.GetCarbonSpecial(atom)); // DD = 15

            molecule = ParseSmiles("C#C");
            atom = molecule.Atoms[0];
            Assert.AreEqual(9, JPlogPDescriptor.JPlogPCalculator.GetCarbonSpecial(atom)); // DD = 09
        }

        [TestMethod()]
        public void TestGetNumMoreElectronegativeThanCarbon()
        {
            var molecule = ParseSmiles("c1ncccc1");
            Aromaticity.CDKLegacy.Apply(molecule);
            var atom = molecule.Atoms[0];
            Assert.AreEqual(2.0, JPlogPDescriptor.JPlogPCalculator.GetNumMoreElectronegativethanCarbon(atom), 0.1);

            molecule = ParseSmiles("CO");
            atom = molecule.Atoms[0];
            Assert.AreEqual(1.0, JPlogPDescriptor.JPlogPCalculator.GetNumMoreElectronegativethanCarbon(atom), 0.1);

            molecule = ParseSmiles("C=O");
            atom = molecule.Atoms[0];
            Assert.AreEqual(2.0, JPlogPDescriptor.JPlogPCalculator.GetNumMoreElectronegativethanCarbon(atom), 0.1);

            molecule = ParseSmiles("C#N");
            atom = molecule.Atoms[0];
            Assert.AreEqual(3.0, JPlogPDescriptor.JPlogPCalculator.GetNumMoreElectronegativethanCarbon(atom), 0.1);
        }

        [TestMethod()]
        public void TestDefaultSpecial()
        {
            var molecule = ParseSmiles("P(=O)(O)(O)O");
            var holo = JPlogPDescriptor.JPlogPCalculator.GetMappedHologram(molecule);
            Assert.AreEqual(1, holo[115404]);
        }

        public static IAtomContainer ParseSmiles(string smiles)
        {
            var molecule = CDK.SmilesParser.ParseSmiles(smiles);
            AtomContainerManipulator.PercieveAtomTypesAndConfigureUnsetProperties(molecule);
            AtomContainerManipulator.ConvertImplicitToExplicitHydrogens(molecule);
            Aromaticity.CDKLegacy.Apply(molecule);
            return molecule;
        }
    }
}
