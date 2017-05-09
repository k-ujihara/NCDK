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
                ParseBitSet("{1, 4, 10, 14, 24, 28, 37, 40, 41, 44, 50, 53, 54, 59, 61, 65, 67, 73, 74, 77, 86, 94, 103, 104, 112, 117, 119, 120, 125, 127, 131, 134, 154, 155, 160, 162, 166, 182, 186, 187, 189, 196, 198, 205, 221, 222, 223, 224, 228, 232, 234, 248, 249, 253, 255, 262, 286, 287, 293, 296, 299, 300, 303, 305, 308, 309, 317, 320, 325, 331, 335, 337, 339, 342, 344, 354, 357, 371, 374, 376, 385, 386, 388, 395, 398, 408, 424, 429, 432, 438, 443, 447, 454, 463, 464, 473, 482, 489, 490, 492, 506, 510, 511, 515, 522, 523, 533, 534, 544, 548, 555, 557, 561, 562, 568, 571, 573, 576, 588, 590, 602, 606, 608, 612, 628, 652, 665, 670, 671, 672, 678, 680, 681, 709, 711, 714, 729, 730, 741, 742, 747, 752, 753, 760, 766, 769, 770, 773, 794, 798, 800, 808, 816, 832, 833, 834, 839, 841, 843, 847, 850, 859, 868, 873, 876, 879, 881, 882, 884, 887, 905, 911, 921, 922, 924, 925, 930, 936, 939, 944, 946, 949, 952, 953, 954, 955, 960, 963, 986, 987, 995, 998, 1003, 1006, 1009, 1010, 1017, 1020}"),
                ParseBitSet("{1, 4, 10, 14, 24, 28, 44, 50, 54, 59, 61, 65, 67, 73, 77, 79, 86, 94, 103, 104, 112, 119, 120, 134, 153, 154, 155, 160, 162, 166, 182, 186, 187, 189, 196, 222, 223, 224, 228, 234, 236, 239, 242, 253, 262, 264, 287, 291, 293, 296, 300, 303, 305, 308, 309, 317, 320, 323, 325, 335, 337, 339, 354, 356, 357, 371, 376, 381, 383, 385, 386, 395, 398, 422, 424, 432, 443, 464, 473, 482, 489, 490, 492, 506, 510, 515, 522, 523, 533, 544, 548, 561, 562, 573, 578, 588, 597, 602, 609, 645, 652, 653, 665, 671, 672, 678, 709, 711, 714, 726, 730, 740, 741, 742, 747, 752, 760, 766, 769, 770, 773, 781, 792, 793, 798, 801, 808, 816, 825, 832, 833, 834, 841, 843, 850, 851, 856, 876, 884, 885, 888, 914, 921, 922, 924, 925, 933, 936, 944, 949, 952, 955, 963, 977, 980, 981, 986, 987, 993, 995, 998, 1001, 1003, 1010, 1020}"),
                ParseBitSet("{4, 17, 51, 52, 58, 110, 114, 162, 177, 182, 208, 224, 230, 231, 242, 259, 262, 266, 270, 277, 289, 307, 309, 325, 335, 339, 349, 351, 354, 363, 371, 376, 387, 406, 433, 437, 489, 491, 500, 504, 513, 522, 533, 539, 545, 561, 573, 588, 601, 603, 611, 614, 618, 665, 686, 735, 746, 747, 752, 755, 764, 776, 777, 784, 795, 798, 801, 808, 822, 823, 851, 876, 882, 886, 944, 969, 987}"),
                ParseBitSet("{4, 10, 77, 162, 522, 714, 747, 752, 816}"),
                ParseBitSet("{4, 14, 16, 17, 18, 27, 30, 37, 40, 44, 50, 51, 52, 54, 55, 58, 61, 65, 67, 73, 75, 78, 79, 86, 103, 104, 106, 110, 112, 114, 120, 125, 127, 141, 142, 144, 151, 153, 154, 158, 162, 177, 178, 182, 189, 196, 199, 204, 205, 208, 210, 215, 216, 220, 222, 224, 228, 230, 231, 236, 239, 245, 253, 255, 259, 262, 264, 266, 280, 286, 287, 289, 291, 296, 297, 300, 303, 305, 308, 309, 311, 317, 320, 321, 322, 323, 325, 329, 334, 335, 337, 339, 342, 344, 347, 349, 351, 354, 356, 359, 363, 370, 371, 376, 377, 378, 381, 382, 383, 385, 386, 387, 395, 398, 400, 406, 412, 415, 425, 432, 433, 437, 439, 467, 473, 475, 485, 489, 490, 491, 492, 495, 500, 502, 504, 506, 510, 512, 515, 522, 523, 526, 531, 534, 536, 539, 544, 545, 548, 550, 555, 557, 561, 562, 566, 568, 572, 573, 575, 578, 588, 592, 597, 601, 602, 603, 609, 611, 614, 615, 616, 618, 624, 626, 638, 644, 645, 646, 651, 653, 654, 655, 658, 665, 666, 671, 674, 680, 686, 691, 700, 733, 735, 740, 742, 745, 746, 747, 752, 755, 764, 766, 768, 770, 773, 777, 781, 792, 793, 795, 798, 800, 801, 822, 825, 830, 832, 834, 843, 845, 851, 856, 857, 874, 876, 882, 884, 885, 886, 888, 896, 905, 910, 912, 914, 921, 922, 924, 925, 930, 931, 933, 939, 944, 946, 952, 955, 957, 961, 963, 977, 978, 979, 980, 987, 998, 999, 1001, 1003, 1008, 1010, 1017, 1020}"),
                ParseBitSet("{4, 10, 14, 16, 24, 28, 40, 50, 61, 65, 67, 77, 86, 92, 103, 104, 108, 117, 120, 132, 136, 154, 155, 158, 160, 162, 182, 185, 189, 196, 198, 222, 223, 228, 231, 255, 275, 280, 287, 295, 299, 300, 303, 308, 317, 320, 328, 339, 352, 357, 369, 385, 386, 395, 398, 401, 424, 429, 432, 443, 449, 464, 473, 481, 488, 490, 505, 506, 507, 510, 513, 515, 522, 527, 533, 544, 546, 548, 561, 562, 566, 596, 598, 599, 613, 628, 645, 652, 671, 672, 678, 680, 683, 686, 696, 709, 714, 729, 730, 736, 747, 752, 760, 770, 773, 777, 779, 794, 797, 808, 816, 829, 834, 843, 868, 886, 889, 910, 911, 921, 922, 923, 925, 936, 944, 946, 952, 955, 958, 963, 971, 988, 995, 998, 1003, 1010, 1020, 1022}"),
                ParseBitSet("{4, 10, 77, 162, 522, 714, 747, 752, 816}"),
                ParseBitSet("{4, 10, 162, 522, 714, 747, 752, 816}"),
                ParseBitSet("{4, 17, 51, 52, 58, 110, 114, 162, 177, 182, 208, 224, 230, 231, 242, 259, 262, 266, 270, 277, 289, 307, 309, 325, 335, 339, 349, 351, 354, 363, 371, 376, 387, 406, 433, 437, 489, 491, 500, 504, 513, 522, 533, 539, 545, 561, 573, 588, 601, 603, 611, 614, 618, 665, 686, 735, 746, 747, 752, 755, 764, 776, 777, 784, 795, 798, 801, 808, 822, 823, 851, 876, 882, 886, 944, 969, 987}"),
                ParseBitSet("{4, 14, 50, 61, 65, 67, 83, 86, 91, 103, 104, 120, 133, 154, 162, 166, 188, 192, 222, 300, 308, 317, 320, 398, 426, 429, 432, 465, 468, 473, 475, 490, 505, 506, 510, 515, 522, 671, 682, 689, 704, 739, 747, 770, 782, 834, 921, 922, 925, 944, 952, 953, 960, 963, 998, 1020}")};

            string filename = "NCDK.Data.MDL.fingerprints_from_modelbuilder3d.sdf";
            Stream ins = ResourceLoader.GetAsStream(filename);
            var data = new TemplateExtractor().MakeFingerprintsFromSdf(true, false, new Dictionary<string, int>(), new StreamReader(ins), 10);
            QueryChemObject obj = new QueryChemObject(Default.ChemObjectBuilder.Instance);
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
            IAtomContainer ac = TestMoleculeFactory.MakeBicycloRings();
            TemplateHandler3D th3d = TemplateHandler3D.Instance;
            ForceFieldConfigurator ffc = new ForceFieldConfigurator();
            ffc.SetForceFieldConfigurator("mm2", ac.Builder);
            IRingSet ringSetMolecule = ffc.AssignAtomTyps(ac);
            var ringSystems = RingPartitioner.PartitionRings(ringSetMolecule);
            IRingSet largestRingSet = RingSetManipulator.GetLargestRingSet(ringSystems);
            IAtomContainer largestRingSetContainer = RingSetManipulator.GetAllInOneContainer(largestRingSet);
            th3d.MapTemplates(largestRingSetContainer, largestRingSetContainer.Atoms.Count);
            for (int i = 0; i < ac.Atoms.Count; i++)
            {
                Assert.IsNotNull(ac.Atoms[i].Point3D);
            }
            ModelBuilder3DTest.CheckAverageBondLength(ac);
        }
    }
}
