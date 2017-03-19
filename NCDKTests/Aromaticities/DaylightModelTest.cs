/*
 * Copyright (c) 2013 European Bioinformatics Institute (EMBL-EBI)
 *                    John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 2.1 of the License, or (at
 * your option) any later version. All we ask is that proper credit is given
 * for our work, which includes - but is not limited to - adding the above
 * copyright notice to the beginning of your source code files, and to any
 * copyright notice that you may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * Any WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using NCDK.Common.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.RingSearches;
using NCDK.Smiles;
using NCDK.Templates;
using NCDK.Tools;
using NCDK.Tools.Manipulator;

namespace NCDK.Aromaticities
{
    /// <summary>
    /// Test the electron contribution using the Daylight aromatic model.
    ///
    // @author John May
    // @cdk.module test-standard
    /// </summary>
    [TestClass()]
    public class DaylightModelTest
    {

        private static ElectronDonation model = ElectronDonation.DaylightModel;

        [TestMethod()]
        public void Benzene()
        {
            Test(AddHydrogens(TestMoleculeFactory.MakeBenzene()), 1, 1, 1, 1, 1, 1);
        }

        [TestMethod()]
        public void Triazole()
        {
            Test(AddHydrogens(TestMoleculeFactory.Make123Triazole()), 1, 2, 1, 1, 1);
        }

        [TestMethod()]
        public void Furan()
        {
            Test(CreateFromSmiles("O1C=CC=C1"), 2, 1, 1, 1, 1);
        }

        [TestMethod()]
        public void Pyrrole()
        {
            Test(CreateFromSmiles("N1C=CC=C1"), 2, 1, 1, 1, 1);
        }

        [TestMethod()]
        public void Methylpyrrole()
        {
            Test(CreateFromSmiles("CN1C=CC=C1"), -1, 2, 1, 1, 1, 1);
        }

        [TestMethod()]
        public void Pyridine()
        {
            Test(CreateFromSmiles("C1=CC=NC=C1"), 1, 1, 1, 1, 1, 1);
        }

        [TestMethod()]
        public void Hexamethylidenecyclohexane()
        {
            Test(CreateFromSmiles("C=C1C(=C)C(=C)C(=C)C(=C)C1=C"), -1, 1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1);
        }

        [TestMethod()]
        public void Cyclopentadienyl()
        {
            Test(CreateFromSmiles("[CH-]1C=CC=C1"), 2, 1, 1, 1, 1);
        }

        [TestMethod()]
        public void PyridineOxide()
        {
            Test(AddHydrogens(TestMoleculeFactory.MakePyridineOxide()), 1, 1, 1, 1, 1, 1, -1);
        }

        [TestMethod()]
        public void Isoindole()
        {
            Test(CreateFromSmiles("C2=C1C=CC=CC1=CN2"), 1, 1, 1, 1, 1, 1, 1, 1, 2);
        }

        [TestMethod()]
        public void Azulene()
        {
            Test(AddHydrogens(TestMoleculeFactory.MakeAzulene()), 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);
        }

        [TestMethod()]
        public void Indole()
        {
            Test(AddHydrogens(TestMoleculeFactory.MakeIndole()), 1, 1, 1, 1, 1, 1, 1, 1, 2);
        }

        [TestMethod()]
        public void Thiazole()
        {
            Test(AddHydrogens(TestMoleculeFactory.MakeThiazole()), 1, 1, 1, 2, 1);
        }

        [TestMethod()]
        public void Tetradehydrodecaline()
        {
            Test(CreateFromSmiles("C1CCC2=CC=CC=C2C1"), -1, -1, -1, 1, 1, 1, 1, 1, 1, -1);
        }

        [TestMethod()]
        public void Tropyliumcation()
        {
            Test(CreateFromSmiles("[CH+]1C=CC=CC=C1"), 0, 1, 1, 1, 1, 1, 1);
        }

        [TestMethod()]
        public void Tropone()
        {
            Test(CreateFromSmiles("O=C1C=CC=CC=C1"), -1, 0, 1, 1, 1, 1, 1, 1);
        }

        [TestMethod()]
        public void Porphyrine()
        {
            Test(CreateFromSmiles("N1C2=CC=C1C=C1C=CC(C=C3NC(C=C3)=CC3=NC(C=C3)=C2)=N1"), 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1,
                    1, 1, 1, 1, 1, 1, 1, 1, 1, 1);
        }

        [TestMethod()]
        public void Quinone()
        {
            Test(CreateFromSmiles("O=C1C=CC(=O)C=C1"), -1, 0, 1, 1, 0, -1, 1, 1);
        }

        [TestMethod()]
        public void Cyclobutadiene()
        {
            Test(CreateFromSmiles("C1=CC=C1"), 1, 1, 1, 1);
        }

        [TestMethod()]
        public void Aminomethylpyridine()
        {
            Test(CreateFromSmiles("CC1=NC=CC=C1N"), -1, 1, 1, 1, 1, 1, 1, -1);
        }

        [TestMethod()]
        public void Indolizine()
        {
            Test(CreateFromSmiles("C1=CN2C=CC=CC2=C1"), 1, 1, 2, 1, 1, 1, 1, 1, 1);
        }

        [TestMethod()]
        public void Imidazothiazole()
        {
            Test(CreateFromSmiles("S1C=CN2C=CN=C12"), 2, 1, 1, 2, 1, 1, 1, 1);
        }

        // 1-oxide pyridine
        [TestMethod()]
        public void Oxidepyridine()
        {
            Test(CreateFromSmiles("O=N1=CC=CC=C1"), -1, 1, 1, 1, 1, 1, 1);
        }

        // 2-Pyridone
        [TestMethod()]
        public void Pyridinone()
        {
            Test(CreateFromSmiles("O=C1NC=CC=C1"), -1, 0, 2, 1, 1, 1, 1);
        }

        // 1H-oxepin-1-ium
        [TestMethod()]
        public void Oxepinium()
        {
            Test(CreateFromSmiles("[OH+]1C=CC=CC=C1"), -1, 1, 1, 1, 1, 1, 1);
        }

        // 4-oxo-1H-pyridin-1-ide
        [TestMethod()]
        public void Oxypyridinide()
        {
            Test(CreateFromSmiles("O=C1C=C[N-]C=C1"), -1, 0, 1, 1, 2, 1, 1);
        }

        [TestMethod()]
        public void AbnormalValence_carbon()
        {
            Test(CreateFromSmiles("[C]1[C][C][C][C]1"), -1, -1, -1, -1, -1);
        }

        [TestMethod()]
        public void AbnormalValence_nitrogen()
        {
            Test(CreateFromSmiles("[N]1[N][N][N][N]1"), -1, -1, -1, -1, -1);
        }

        [TestMethod()]
        public void AbnormalValence_phosphorus()
        {
            Test(CreateFromSmiles("[P]1[P][P][P][P]1"), -1, -1, -1, -1, -1);
        }

        /// <summary>
        /// A 3 valent nitrogen cation should be aromatic, otherwise when we make it
        /// lower case we can not convert it back.
        ///
        // @cdk.bug 1332
        /// </summary>
        [TestMethod()]
        public void AbnormalValence_nitrogenCation()
        {
            Test(CreateFromSmiles("C1=CC(=C([N+]=C1)N)[N+](=O)[O-]"), 1, 1, 1, 1, -1, 1, -1, -1, -1, -1);
        }

        [TestMethod()]
        public void Thiazole1oxide()
        {
            Test(CreateFromSmiles("O=S1C=NC=C1"), -1, 2, 1, 1, 1, 1);
        }

        static IAtomContainer AddHydrogens(IAtomContainer container)
        {
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(container);
            CDKHydrogenAdder.GetInstance(container.Builder).AddImplicitHydrogens(container);
            return container;
        }

        static IAtomContainer CreateFromSmiles(string smi)
        {
            return new SmilesParser(Silent.ChemObjectBuilder.Instance).ParseSmiles(smi);
        }

        /// <summary>Check the electron contribution is the same as expected.</summary>
        static void Test(IAtomContainer m, params int[] expected)
        {
            AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(m);
            Assert.IsTrue(Compares.AreDeepEqual(expected, model.Contribution(m, new RingSearch(m))));
        }
    }
}
