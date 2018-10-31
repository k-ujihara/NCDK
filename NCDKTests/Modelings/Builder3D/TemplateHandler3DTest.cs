/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
 *                    2011  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@list.sourceforge.net
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
using NCDK.Common.Base;
using NCDK.Common.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Fingerprints;
using NCDK.Isomorphisms.Matchers;
using NCDK.RingSearches;
using NCDK.Templates;
using NCDK.Tools.Manipulator;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NCDK.Silent;

namespace NCDK.Modelings.Builder3D
{
    // @cdk.module test-builder3d
    // @author      chhoppe
    // @author      Christoph Steinbeck
    // @cdk.created 2004-11-04
    [TestClass()]
    public class TemplateHandler3DTest : CDKTestCase
    {
        [TestMethod()]
        public void TestInstance()
        {
            TemplateHandler3D th3d = TemplateHandler3D.Instance;
            // need to trigger a load of the templates
            th3d.MapTemplates(new Silent.AtomContainer(), 0);
            Assert.AreEqual(10751, th3d.TemplateCount);
        }

        private static BitArray ParseBitSet(string str)
        {
            return TemplateHandler3D.GetBitSetFromFile(Strings.Tokenize(str, '\t', ' ', ';', '{', ',', '}'));
        }

        [TestMethod()]
        public void TestFingerprints()
        {
            BitArray[] expected = new BitArray[]{
                ParseBitSet("{3, 5, 8, 18, 29, 33, 39, 65, 71, 90, 105, 125, 140, 170, 182, 192, 199, 203, 209, 213, 226, 271, 272, 287, 301, 304, 319, 368, 386, 423, 433, 540, 590, 605, 618, 620, 629, 641, 649, 672, 681, 690, 694, 696, 697, 716, 726, 745, 748, 751, 760, 765, 775, 777, 780, 792, 799, 805, 810, 825, 829, 836, 844, 850, 876, 880, 882, 888, 899, 914, 924, 929, 932, 935, 967, 971, 1004, 1013, 1015, 1023}"),
                ParseBitSet("{3, 8, 18, 29, 33, 65, 90, 101, 109, 117, 125, 127, 140, 170, 190, 192, 209, 213, 218, 226, 271, 272, 286, 287, 301, 304, 319, 386, 423, 433, 566, 590, 605, 618, 629, 641, 646, 649, 672, 690, 694, 696, 716, 726, 745, 748, 765, 775, 777, 780, 783, 792, 805, 810, 825, 829, 836, 844, 850, 876, 882, 899, 914, 924, 932, 934, 956, 967, 971, 994, 1004, 1013, 1015, 1023}"),
                ParseBitSet("{3, 18, 26, 32, 33, 43, 140, 155, 188, 189, 226, 238, 262, 267, 287, 315, 319, 326, 375, 450, 577, 629, 644, 690, 719, 732, 745, 746, 751, 775, 847, 850, 881, 959, 971, 995, 1015, 1019}"),
                ParseBitSet("{3, 18, 33, 192, 319, 745, 780, 882}"),
                ParseBitSet("{3, 13, 18, 22, 26, 29, 33, 43, 71, 85, 90, 101, 103, 109, 117, 118, 125, 127, 140, 145, 153, 155, 182, 188, 189, 190, 199, 218, 225, 226, 238, 269, 272, 286, 287, 301, 304, 315, 319, 326, 370, 375, 386, 408, 423, 433, 450, 493, 502, 556, 566, 577, 590, 598, 605, 617, 618, 629, 644, 649, 672, 679, 690, 691, 694, 696, 716, 719, 727, 732, 745, 748, 751, 760, 762, 765, 775, 777, 783, 805, 806, 810, 821, 829, 844, 847, 850, 876, 888, 899, 914, 923, 924, 926, 927, 929, 934, 956, 959, 966, 971, 990, 995, 1006, 1013, 1015, 1019}"),
                ParseBitSet("{3, 18, 29, 33, 53, 65, 90, 105, 125, 192, 203, 269, 271, 272, 293, 301, 319, 345, 364, 376, 386, 433, 540, 569, 590, 605, 618, 641, 649, 672, 675, 681, 696, 745, 748, 765, 780, 790, 798, 799, 801, 805, 825, 829, 836, 837, 844, 853, 876, 882, 891, 899, 914, 924, 932, 967, 996, 1004, 1013}"),
                ParseBitSet("{3, 18, 33, 192, 319, 745, 780, 882}"),
                ParseBitSet("{3, 18, 33, 192, 319, 745, 780, 882}"),
                ParseBitSet("{3, 18, 26, 32, 33, 43, 140, 155, 188, 189, 226, 238, 262, 267, 287, 315, 319, 326, 375, 450, 577, 629, 644, 690, 719, 732, 745, 746, 751, 775, 847, 850, 881, 959, 971, 995, 1015, 1019}"),
                ParseBitSet("{3, 18, 29, 33, 90, 105, 125, 272, 280, 301, 433, 521, 590, 618, 651, 672, 696, 698, 745, 760, 829, 844, 876, 890, 899, 924, 1013}")};

            string filename = "NCDK.Data.MDL.fingerprints_from_modelbuilder3d.sdf";
            var ins = ResourceLoader.GetAsStream(filename);
            var data = TemplateExtractor.MakeFingerprintsFromSdf(true, false, new Dictionary<string, int>(), new StreamReader(ins), 10);
            QueryChemObject obj = new QueryChemObject(ChemObjectBuilder.Instance);
            var dummy = obj.Builder;
            for (int i = 0; i < data.Count; i++)
            {
                IBitFingerprint bs = data[i];
                Assert.IsTrue(Compares.AreEqual(expected[i], bs.AsBitSet()));
            }
        }

        [TestMethod()]
        public void TestAnonFingerprints()
        {
            BitArray[] expected = new BitArray[]{
                ParseBitSet("{148, 206, 392, 542, 637, 742, 752, 830}"),
                ParseBitSet("{148, 206, 392, 542, 637, 742, 752, 830}"),
                ParseBitSet("{148, 206, 392, 542, 637, 742, 752, 830}"),
                ParseBitSet("{148, 206, 392, 542, 637, 742, 752, 830}"),
                ParseBitSet("{148, 206, 392, 542, 637, 742, 752, 830}"),
                ParseBitSet("{148, 206, 392, 542, 637, 742, 752, 830}"),
                ParseBitSet("{148, 206, 392, 542, 637, 742, 752, 830}"),
                ParseBitSet("{148, 206, 392, 542, 637, 742, 752, 830}"),
                ParseBitSet("{148, 206, 392, 542, 637, 742, 752, 830}"),
                ParseBitSet("{148, 206, 392, 542, 637, 742, 752, 830}")};

            string filename = "NCDK.Data.MDL.fingerprints_from_modelbuilder3d.sdf";
            var ins = this.GetType().Assembly.GetManifestResourceStream(filename);
            var data = TemplateExtractor.MakeFingerprintsFromSdf(true, true,
                new Dictionary<string, int>(), new StreamReader(ins), 10);
            QueryChemObject obj = new QueryChemObject(ChemObjectBuilder.Instance);
            var dummy = obj.Builder;
            for (int i = 0; i < data.Count; i++)
            {
                IBitFingerprint bs = data[i];
                Assert.IsTrue(Compares.AreEqual(expected[i], bs.AsBitSet()));
            }
        }

        [TestMethod()]
        public void TestMapTemplates_IAtomContainer_Double()
        {
            var ac = TestMoleculeFactory.MakeBicycloRings();
            var th3d = TemplateHandler3D.Instance;
            var ffc = new ForceFieldConfigurator();
            ffc.SetForceFieldConfigurator("mm2", ac.Builder);
            var ringSetMolecule = ffc.AssignAtomTyps(ac);
            var ringSystems = RingPartitioner.PartitionRings(ringSetMolecule);
            var largestRingSet = RingSetManipulator.GetLargestRingSet(ringSystems);
            var largestRingSetContainer = RingSetManipulator.GetAllInOneContainer(largestRingSet);
            th3d.MapTemplates(largestRingSetContainer, largestRingSetContainer.Atoms.Count);
            for (int i = 0; i < ac.Atoms.Count; i++)
            {
                Assert.IsNotNull(ac.Atoms[i].Point3D);
            }
            ModelBuilder3DTest.CheckAverageBondLength(ac);
        }
    }
}
