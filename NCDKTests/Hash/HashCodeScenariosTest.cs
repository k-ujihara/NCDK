/*
 * Copyright (c) 2013 John May <jwmay@users.sf.net>
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
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NCDK.Silent;
using NCDK.Graphs;
using NCDK.IO.Iterator;
using NCDK.Stereo;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.IO;

namespace NCDK.Hash
{
    /// <summary>
    /// This test class provides several scenario tests for the <see cref="NCDK.Hash"/>
    /// module.
    /// </summary>
    // @author John May
    // @cdk.module test-hash
    public class HashCodeScenariosTest
    {
        /// <summary>
        /// Two molecules with identical Racid identification numbers, these hash
        /// codes should be different.
        /// </summary>
        [TestMethod()]
        public void Figure2a()
        {
            List<IAtomContainer> mols = ExtractSDF("ihlenfeldt93-figure-2a.sdf", 2);

            IAtomContainer a = mols[0];
            IAtomContainer b = mols[1];

            IMoleculeHashGenerator generator = new HashGeneratorMaker().Elemental().Depth(6).Molecular();

            long aHash = generator.Generate(a);
            long bHash = generator.Generate(b);

            Assert.AreNotEqual(aHash, bHash, NonEqMesg(a, b));
        }

        /// <summary>
        /// Two molecules with identical Racid identification numbers, these hash
        /// codes should be different.
        /// </summary>
        [TestMethod()]
        public void Figure2b()
        {
            List<IAtomContainer> mols = ExtractSDF("ihlenfeldt93-figure-2b.sdf", 2);

            IAtomContainer a = mols[0];
            IAtomContainer b = mols[1];

            IMoleculeHashGenerator generator = new HashGeneratorMaker().Elemental().Depth(6).Molecular();

            long aHash = generator.Generate(a);
            long bHash = generator.Generate(b);

            Assert.AreNotEqual(aHash, bHash, NonEqMesg(a, b));
        }

        /// <summary>
        /// Two molecules with identical Racid identification numbers, these hash
        /// codes should be different.
        /// </summary>
        [TestMethod()]
        public void Figure2c()
        {
            List<IAtomContainer> mols = ExtractSDF("ihlenfeldt93-figure-2c.sdf", 2);

            IAtomContainer a = mols[0];
            IAtomContainer b = mols[1];

            IMoleculeHashGenerator generator = new HashGeneratorMaker().Elemental().Depth(6).Molecular();

            long aHash = generator.Generate(a);
            long bHash = generator.Generate(b);

            Assert.AreNotEqual(aHash, bHash, NonEqMesg(a, b));
        }

        /// <summary>
        /// These two molecules from the original publication collide when using a
        /// previous hash coding method (Bawden, 81). The hash codes should be
        /// different using this method.
        /// </summary>
        [TestMethod()]
        public void Figure3()
        {
            List<IAtomContainer> mols = ExtractSDF("ihlenfeldt93-figure-3.sdf", 2);

            IAtomContainer a = mols[0];
            IAtomContainer b = mols[1];

            IMoleculeHashGenerator generator = new HashGeneratorMaker().Elemental().Depth(6).Molecular();

            long aHash = generator.Generate(a);
            long bHash = generator.Generate(b);

            Assert.AreNotEqual(aHash, bHash, NonEqMesg(a, b));
        }

        /// <summary>
        /// These two molecules have atoms experiencing uniform environments but
        /// where the number of atoms between the molecules is different. This
        /// demonstrates the size the molecule is considered when hashing.
        /// </summary>
        [TestMethod()]
        public void Figure7()
        {
            List<IAtomContainer> mols = ExtractSDF("ihlenfeldt93-figure-7.sdf", 2);

            IAtomContainer a = mols[0];
            IAtomContainer b = mols[1];

            IMoleculeHashGenerator generator = new HashGeneratorMaker().Elemental().Depth(6).Molecular();

            long aHash = generator.Generate(a);
            long bHash = generator.Generate(b);

            Assert.AreNotEqual(aHash, bHash, NonEqMesg(a, b));
        }

        /// <summary>
        /// These molecules are erroneous structures from a catalogue file, the
        /// German names are the original names as they appear in the catalogue. The
        /// hash code identifies that the two molecules are the same.
        /// </summary>
        [TestMethod()]
        public void Figure10()
        {
            List<IAtomContainer> mols = ExtractSDF("ihlenfeldt93-figure-10.sdf", 2);

            IAtomContainer a = mols[0];
            IAtomContainer b = mols[1];

            IMoleculeHashGenerator generator = new HashGeneratorMaker().Elemental().Depth(6).Molecular();

            long aHash = generator.Generate(a);
            long bHash = generator.Generate(b);

            Assert.AreEqual(aHash, bHash, eqMesg(a, b));
        }

        /// <summary>
        /// This structure is an example where the Cahn-Ingold-Prelog (CIP) rules can
        /// not discriminate two neighbours of chiral atom. Due to this, the CIP
        /// rules are not used as an atom seed and instead a bootstrap method is
        /// used. Please refer to the original article for the exact method.
        /// </summary>
        [TestMethod()]
        public void Figure11()
        {
            List<IAtomContainer> mols = ExtractSDF("ihlenfeldt93-figure-11.sdf", 1);

            IAtomContainer molecule = mols[0];

            IMoleculeHashGenerator basic = new HashGeneratorMaker().Elemental().Depth(8).Molecular();
            IMoleculeHashGenerator stereo = new HashGeneratorMaker().Elemental().Depth(8).Chiral().Molecular();

            long basicHash = basic.Generate(molecule);
            long stereoHash = stereo.Generate(molecule);

            Assert.AreNotEqual(
                basicHash, stereoHash,
                "If the stereo-centre was perceived then the basic hash should be different from the chiral hash code");
        }

        /// <summary>
        /// This scenario demonstrates how stereo-chemistry encoding is invariant
        /// under permutation. A simple molecule 'Bromo(chloro)fluoromethane' is
        /// permuted to all 120 possible atom orderings. It is checked that the (R)-
        /// configuration  and (S)- configuration values are invariant
        /// </summary>
        [TestMethod()]
        public void Figure12()
        {
            List<IAtomContainer> mols = ExtractSDF("ihlenfeldt93-figure-12.sdf", 2);

            IMoleculeHashGenerator stereo = new HashGeneratorMaker().Elemental().Depth(1).Chiral().Molecular();

            var sHashes = new HashSet<long>();
            var rHashes = new HashSet<long>();

            AtomContainerAtomPermutor rpermutor = new AtomContainerAtomPermutor(mols[0]);
            AtomContainerAtomPermutor spermutor = new AtomContainerAtomPermutor(mols[1]);

            while (rpermutor.MoveNext() && spermutor.MoveNext())
            {
                IAtomContainer r = rpermutor.Current;
                IAtomContainer s = spermutor.Current;
                sHashes.Add(stereo.Generate(s));
                rHashes.Add(stereo.Generate(r));
            }
            Assert.AreEqual(1, sHashes.Count, "all (S)-Bromo(chloro)fluoromethane permutation produce a single hash code");
            Assert.AreEqual(1, rHashes.Count, "all (R)-Bromo(chloro)fluoromethane permutation produce a single hash code");
            foreach (var hash in rHashes)
                sHashes.Add(hash);
            Assert.AreEqual(2, sHashes.Count);
        }

        /// <summary>
        /// This molecule has a tetrahedral stereo-centre depends on the
        /// configuration of two double bonds. Swapping the double bond configuration
        /// inverts the tetrahedral stereo-centre (R/S) and produces different hash
        /// codes.
        /// </summary>
        [TestMethod()]
        public void Figure13a()
        {
            List<IAtomContainer> mols = ExtractSDF("ihlenfeldt93-figure-13a.sdf", 2);

            IAtomContainer a = mols[0];
            IAtomContainer b = mols[1];

            IMoleculeHashGenerator stereo = new HashGeneratorMaker().Elemental().Depth(8).Chiral().Molecular();
            long aHash = stereo.Generate(a);
            long bHash = stereo.Generate(b);

            Assert.AreNotEqual(aHash, bHash, NonEqMesg(a, b));
        }

        /// <summary>
        /// This molecule has double bond stereo chemistry defined only by
        /// differences in the configurations of it's substituents. The two
        /// configurations the bond can take (Z/E) and should produce different hash
        /// codes.
        /// </summary>
        [TestMethod()]
        public void Figure13b()
        {
            List<IAtomContainer> mols = ExtractSDF("ihlenfeldt93-figure-13b.sdf", 2);

            IAtomContainer a = mols[0];
            IAtomContainer b = mols[1];

            IMoleculeHashGenerator stereo = new HashGeneratorMaker().Elemental().Depth(8).Chiral().Molecular();

            Assert.AreNotEqual(stereo.Generate(a), stereo.Generate(b), NonEqMesg(a, b));
        }

        /// <summary>
        /// These two structures were found in the original publication as duplicates
        /// in the catalogue of the CHIRON program. The article notes the second name
        /// is likely incorrect but that this is how it appears in the catalogue. The
        /// two molecules are in fact the same and generate the same hash code.
        /// </summary>
        [TestMethod()]
        public void Figure14()
        {
            List<IAtomContainer> mols = ExtractSDF("ihlenfeldt93-figure-14.sdf", 2);

            IAtomContainer a = mols[0];
            IAtomContainer b = mols[1];

            IMoleculeHashGenerator generator = new HashGeneratorMaker().Elemental().Depth(6).Molecular();

            long aHash = generator.Generate(a);
            long bHash = generator.Generate(b);

            Assert.AreEqual(aHash, bHash, eqMesg(a, b));
        }

        /// <summary>
        /// These two compounds are connected differently but produce the same basic
        /// hash code. In order to discriminate them we must use the perturbed hash
        /// code.
        /// </summary>
        [TestMethod()]
        public void Figure15()
        {
            List<IAtomContainer> mols = ExtractSDF("ihlenfeldt93-figure-15.sdf", 2);

            IAtomContainer a = mols[0];
            IAtomContainer b = mols[1];

            IMoleculeHashGenerator generator = new HashGeneratorMaker().Elemental().Depth(6).Molecular();
            long aHash = generator.Generate(a);
            long bHash = generator.Generate(b);

            Assert.AreEqual(aHash, bHash, eqMesg(a, b));

            IMoleculeHashGenerator perturbed = new HashGeneratorMaker().Elemental().Depth(6).Perturbed().Molecular();
            aHash = perturbed.Generate(a);
            bHash = perturbed.Generate(b);
            Assert.AreNotEqual(aHash, bHash, NonEqMesg(a, b));
        }

        /// <summary>
        /// The molecules cubane and cuneane have the same number of atoms all of
        /// which experience the same environment in the first sphere. Using a
        /// non-perturbed hash code, these will hash to the same value. The perturbed
        /// hash code, allows us to discriminate them.
        /// </summary>
        [TestMethod()]
        public void Figure16a()
        {
            List<IAtomContainer> mols = ExtractSDF("ihlenfeldt93-figure-16a.sdf", 2);

            IAtomContainer a = mols[0];
            IAtomContainer b = mols[1];

            IMoleculeHashGenerator nonperturbed = new HashGeneratorMaker().Elemental().Depth(6).Molecular();
            IMoleculeHashGenerator perturbed = new HashGeneratorMaker().Elemental().Depth(6).Perturbed().Molecular();

            long aHash = nonperturbed.Generate(a);
            long bHash = nonperturbed.Generate(b);
            Assert.AreEqual(aHash, bHash, eqMesg(a, b));

            aHash = perturbed.Generate(a);
            bHash = perturbed.Generate(b);
            Assert.AreNotEqual(aHash, bHash, NonEqMesg(a, b));

            IAtomHashGenerator perturbedAtomic = new HashGeneratorMaker().Elemental().Depth(3).Perturbed().Atomic();
            long[] aHashes = perturbedAtomic.Generate(a);
            long[] bHashes = perturbedAtomic.Generate(b);

            Assert.AreEqual(1, ToSet(aHashes).Count, "cubane has 1 equiavelnt class");
            Assert.AreEqual(3, ToSet(bHashes).Count, "cubane has 3 equiavelnt classes");
        }

        private HashSet<T> ToSet<T>(T[] xs)
        {
            var set = new HashSet<T>();
            foreach (var x in xs)
            {
                set.Add(x);
            }
            return set;
        }

        /// <summary>
        /// A chlorinated cubane and cuneane can not be told apart by the basic hash
        /// code. However using perturbed hash codes is is possible to tell them
        /// apart as well as the 3 different chlorination locations on the cuneane
        /// </summary>
        [TestMethod()]
        public void Figure16b()
        {
            List<IAtomContainer> mols = ExtractSDF("ihlenfeldt93-figure-16b.sdf", 4);

            IAtomContainer a = mols[0];
            IAtomContainer b = mols[1];
            IAtomContainer c = mols[2];
            IAtomContainer d = mols[3];

            IMoleculeHashGenerator generator = new HashGeneratorMaker().Elemental().Depth(6).Perturbed().Molecular();

            long aHash = generator.Generate(a);
            long bHash = generator.Generate(b);
            long cHash = generator.Generate(c);
            long dHash = generator.Generate(d);

            Assert.AreNotEqual(aHash, bHash, NonEqMesg(a, b));
            Assert.AreNotEqual(aHash, cHash, NonEqMesg(a, c));
            Assert.AreNotEqual(aHash, dHash, NonEqMesg(a, d));
            Assert.AreNotEqual(bHash, cHash, NonEqMesg(a, c));
            Assert.AreNotEqual(bHash, dHash, NonEqMesg(b, d));
            Assert.AreNotEqual(cHash, dHash, NonEqMesg(c, d));

        }

        /// <summary>
        /// This scenario demonstrates how the depth influences the hash code. These
        /// two molecules differ only by length of their aliphatic chains. One  has
        /// chains of length 10 and 11 and other of length 11 and 10 (connected the
        /// other way). To tell these apart the depth must be large enough to
        /// propagate  the environments from the ends of both chains.
        /// </summary>
        [TestMethod()]
        public void Aminotetracosanone()
        {
            List<IAtomContainer> aminotetracosanones = ExtractSDF("aminotetracosanones.sdf", 2);

            IAtomContainer a = aminotetracosanones[0];
            IAtomContainer b = aminotetracosanones[1];

            for (int depth = 0; depth < 12; depth++)
            {
                IMoleculeHashGenerator basic = new HashGeneratorMaker().Elemental().Depth(depth).Molecular();
                long aHash = basic.Generate(a);
                long bHash = basic.Generate(b);

                if (depth < 7)
                {
                    Assert.AreEqual(aHash, bHash, eqMesg(a, b) + " at depth " + depth);
                }
                else
                {
                    Assert.AreNotEqual(aHash, bHash, NonEqMesg(a, b) + " at depth " + depth);
                }
            }
        }

        /// <summary>
        /// This test demonstrates that the nine stereo isomers of inositol can be
        /// hashed to the same value or to different values (perturbed).
        /// </summary>
        /// <seealso href="http://en.wikipedia.org/wiki/Inositol#Isomers_and_structure">Inositol Isomers</seealso>
        [TestMethod()]
        public void Inositols()
        {
            List<IAtomContainer> inositols = ExtractSDF("inositols.sdf", 9);

            // non-stereo non-perturbed hash generator
            IMoleculeHashGenerator basic = new HashGeneratorMaker().Elemental().Depth(6).Molecular();

            var hashes = new HashSet<long>();

            foreach (var inositol in inositols)
            {
                long hash = basic.Generate(inositol);
                hashes.Add(hash);
            }

            Assert.AreEqual(1, hashes.Count, "all inositol isomers should hash to the same value");

            // stereo non-perturbed hash generator
            IMoleculeHashGenerator stereo = new HashGeneratorMaker().Elemental().Depth(6).Chiral().Molecular();
            hashes.Clear();

            foreach (var inositol in inositols)
            {
                long hash = stereo.Generate(inositol);
                hashes.Add(hash);
            }

            Assert.AreEqual(1, hashes.Count, "all inositol isomers should hash to the same value");

            // stereo non-perturbed hash generator
            IMoleculeHashGenerator perturbed = new HashGeneratorMaker().Elemental().Depth(6).Chiral().Perturbed()
                    .Molecular();
            hashes.Clear();

            foreach (var inositol in inositols)
            {
                long hash = perturbed.Generate(inositol);
                hashes.Add(hash);
            }

            Assert.AreEqual(9, hashes.Count, "all inositol isomers should hash to different values");
        }

        [TestMethod()]
        public void AllenesWithImplicitHydrogens()
        {
            List<IAtomContainer> allenes = ExtractSDF("allene-implicit_-h.sdf", 2);

            IAtomContainer mAllene = allenes[0];
            IAtomContainer pAllene = allenes[1];

            // non-stereo hash code
            IMoleculeHashGenerator basic = new HashGeneratorMaker().Elemental().Depth(2).Molecular();
            Assert.AreEqual(basic.Generate(mAllene), basic.Generate(pAllene), "(M) and (P) allene should hash the same when non-stereo");

            IMoleculeHashGenerator stereo = new HashGeneratorMaker().Elemental().Depth(2).Chiral().Molecular();

            Assert.AreNotEqual(stereo.Generate(mAllene), stereo.Generate(pAllene), "(M) and (P) allene should not hash the same when stereo");

            // check the hashes are invariant under permutation
            long mAlleneReference = stereo.Generate(mAllene);
            long pAlleneReference = stereo.Generate(pAllene);

            AtomContainerPermutor mAllenePermutor = new AtomContainerAtomPermutor(mAllene);
            while (mAllenePermutor.MoveNext())
            {
                Assert.AreEqual(mAlleneReference, stereo.Generate(mAllenePermutor.Current),
                    "(M)-allene was not invariant under permutation");
            }

            AtomContainerPermutor pAllenePermutor = new AtomContainerAtomPermutor(pAllene);
            while (pAllenePermutor.MoveNext())
            {
                Assert.AreEqual(
                    pAlleneReference, stereo.Generate(pAllenePermutor.Current),
                    "(P)-allene was not invariant under permutation");
            }
        }

        [TestMethod()]
        public void AllenesWithExplicitHydrogens()
        {
            List<IAtomContainer> allenes = ExtractSDF("allene-explicit-h.sdf", 2);

            IAtomContainer mAllene = allenes[0];
            IAtomContainer pAllene = allenes[1];

            // non-stereo hash code
            IMoleculeHashGenerator basic = new HashGeneratorMaker().Elemental().Depth(2).Molecular();
            Assert.AreEqual(basic.Generate(mAllene), basic.Generate(pAllene),
                "(M) and (P) allene should hash the same when non-stereo");

            IMoleculeHashGenerator stereo = new HashGeneratorMaker().Elemental().Depth(2).Chiral().Molecular();
            Assert.AreNotEqual(stereo.Generate(mAllene), stereo.Generate(pAllene),
                "(M) and (P) allene should not hash the same when stereo");

            // check the hashes are invariant under permutation
            long mAlleneReference = stereo.Generate(mAllene);
            long pAlleneReference = stereo.Generate(pAllene);

            AtomContainerPermutor mAllenePermutor = new AtomContainerAtomPermutor(mAllene);
            while (mAllenePermutor.MoveNext())
            {
                Assert.AreEqual(mAlleneReference, stereo.Generate(mAllenePermutor.Current),
                    "(M)-allene was not invariant under permutation");
            }

            AtomContainerPermutor pAllenePermutor = new AtomContainerAtomPermutor(pAllene);
            while (pAllenePermutor.MoveNext())
            {
                Assert.AreEqual(pAlleneReference, stereo.Generate(pAllenePermutor.Current), "(P)-allene was not invariant under permutation");
            }
        }

        [TestMethod()]
        public void Allenes2Dand3D()
        {
            List<IAtomContainer> allenes2D = ExtractSDF("allene-explicit-h.sdf", 2);
            List<IAtomContainer> allenes3D = ExtractSDF("allene-explicit-3d-h.sdf", 2);

            IAtomContainer mAllene2D = allenes2D[0];
            IAtomContainer mAllene3D = allenes3D[0];
            IAtomContainer pAllene2D = allenes2D[1];
            IAtomContainer pAllene3D = allenes3D[1];

            // non-stereo hash code
            IMoleculeHashGenerator basic = new HashGeneratorMaker().Elemental().Depth(2).Molecular();
            Assert.AreEqual(basic.Generate(mAllene2D), basic.Generate(pAllene2D),
                "(M) and (P) allene (2D) should hash the same when non-stereo");
            Assert.AreEqual(basic.Generate(mAllene3D), basic.Generate(pAllene3D),
                "(M) and (P) allene (3D) should hash the same when non-stereo");
            Assert.AreEqual(basic.Generate(mAllene2D), basic.Generate(mAllene3D),
                "(M) allene should hash the same in 2D and 3D");
            Assert.AreEqual(basic.Generate(mAllene2D), basic.Generate(mAllene3D),
                "(P) allene should hash the same in 2D and 3D");

            IMoleculeHashGenerator stereo = new HashGeneratorMaker().Elemental().Depth(2).Chiral().Molecular();
            Assert.AreNotEqual(stereo.Generate(mAllene2D), stereo.Generate(pAllene2D), "(M) and (P) allene should not hash the same when stereo");
            Assert.AreNotEqual(stereo.Generate(mAllene3D), stereo.Generate(pAllene3D), "(M) and (P) allene (3D) should not hash the same when stereo");
            Assert.AreEqual(stereo.Generate(mAllene2D), stereo.Generate(mAllene3D), "(M) allene should hash the same in 2D and 3D (stereo)");
            Assert.AreEqual(stereo.Generate(pAllene2D), stereo.Generate(pAllene3D), "(P) allene should hash the same in 2D and 3D (stereo)");
        }

        [TestMethod()]
        public void AllenesWithUnspecifiedConfiguration()
        {
            List<IAtomContainer> allenes = ExtractSDF("allene-implicit_-h.sdf", 2);
            List<IAtomContainer> unspecified = ExtractSDF("allene-unspecified.sdf", 2);

            IAtomContainer mAllene = allenes[0];
            IAtomContainer pAllene = allenes[1];
            IAtomContainer unspecAllene1 = unspecified[0];
            IAtomContainer unspecAllene2 = unspecified[1];

            // non-stereo hash code
            IMoleculeHashGenerator basic = new HashGeneratorMaker().Elemental().Depth(2).Molecular();

            Assert.AreEqual(basic.Generate(mAllene), basic.Generate(pAllene), "(M) and (P) allene should hash the same when non-stereo");
            Assert.AreNotEqual(basic.Generate(mAllene), basic.Generate(unspecAllene1), "Unspecifed allene should be the same");
            Assert.AreNotEqual(basic.Generate(mAllene), basic.Generate(unspecAllene2), "Unspecifed allene should be the same");

            IMoleculeHashGenerator stereo = new HashGeneratorMaker().Elemental().Depth(2).Chiral().Molecular();
            Assert.AreNotEqual(stereo.Generate(mAllene), stereo.Generate(pAllene), "(M) and (P) allene should not hash the same when using stereo");
            Assert.AreNotEqual(stereo.Generate(mAllene), stereo.Generate(unspecAllene1), "Unspecifed allene should be the different");
            Assert.AreNotEqual(stereo.Generate(mAllene), stereo.Generate(unspecAllene2), "Unspecifed allene should be the different");
            Assert.AreNotEqual(stereo.Generate(unspecAllene1), stereo.Generate(unspecAllene2), "Unspecifed allenes should be the same");
        }

        [TestMethod()]
        public void Cumulenes()
        {
            List<IAtomContainer> cumulenes = ExtractSDF("cumulenes.sdf", 2);

            IAtomContainer eCumulene = cumulenes[0];
            IAtomContainer zCumulene = cumulenes[1];

            // non-stereo hash code
            IMoleculeHashGenerator basic = new HashGeneratorMaker().Elemental().Depth(2).Molecular();
            Assert.AreNotEqual(basic.Generate(eCumulene), basic.Generate(zCumulene), "(E) and (Z) cumulene should hash the same when non-stereo");

            IMoleculeHashGenerator stereo = new HashGeneratorMaker().Elemental().Depth(2).Chiral().Molecular();

            Assert.AreEqual(stereo.Generate(eCumulene), stereo.Generate(zCumulene), "(E) and (Z) cumulene should not hash the same when stereo");
        }

        [TestMethod()]
        public void SuppressedHydrogens()
        {
            List<IAtomContainer> implicits = ExtractSDF("butan-2-ols.sdf", 2);
            List<IAtomContainer> explicits = ExtractSDF("butan-2-ols-explicit-hydrogens.sdf", 2);

            IAtomContainer implicit_ = implicits[0];
            IAtomContainer explicit_ = explicits[0];

            IMoleculeHashGenerator unsuppressed = new HashGeneratorMaker().Elemental().Depth(4).Molecular();
            Assert.AreNotEqual(unsuppressed.Generate(implicit_), unsuppressed.Generate(explicit_), NonEqMesg(implicit_, explicit_));

            IMoleculeHashGenerator suppressed = new HashGeneratorMaker().Elemental().Depth(4).SuppressHydrogens().Molecular();
            Assert.AreEqual(suppressed.Generate(implicit_), suppressed.Generate(explicit_), eqMesg(implicit_, explicit_));
        }

        [TestMethod()]
        public void SuppressedHydrogens_chiral()
        {
            List<IAtomContainer> implicits = ExtractSDF("butan-2-ols.sdf", 2);
            List<IAtomContainer> explicits = ExtractSDF("butan-2-ols-explicit-hydrogens.sdf", 2);

            IAtomContainer implicit_ = implicits[0];
            IAtomContainer explicit_ = explicits[0];

            IMoleculeHashGenerator unsuppressed = new HashGeneratorMaker().Elemental().Depth(4).Chiral().Molecular();
            Assert.AreNotEqual(unsuppressed.Generate(implicit_), unsuppressed.Generate(explicit_), NonEqMesg(implicit_, explicit_));

            IMoleculeHashGenerator suppressed = new HashGeneratorMaker().Elemental().Depth(4).Chiral().SuppressHydrogens().Molecular();
            Assert.AreEqual(suppressed.Generate(implicit_), suppressed.Generate(explicit_), eqMesg(implicit_, explicit_));

            // okay now let's do some permutations can check the hash codes are always the same
            AtomContainerPermutor implicitPermutor = new AtomContainerAtomPermutor(implicit_);
            AtomContainerPermutor explicitPermutor = new AtomContainerAtomPermutor(explicit_);

            while (implicitPermutor.MoveNext() && explicitPermutor.MoveNext())
            {
                implicit_ = implicitPermutor.Current;
                explicit_ = explicitPermutor.Current;
                Assert.AreEqual(suppressed.Generate(implicit_), suppressed.Generate(explicit_), eqMesg(implicit_, explicit_));
            }
        }

        [TestMethod()]
        public void Inositols_suppressedHydrogens()
        {
            List<IAtomContainer> implicits = ExtractSDF("inositols.sdf", 9);
            List<IAtomContainer> explicits = ExtractSDF("inositols-explicit-hydrogens.sdf", 9);

            Assert.AreEqual(implicits.Count, explicits.Count, "different number of implicit and explicit structures");

            IMoleculeHashGenerator unsuppressed = new HashGeneratorMaker().Elemental().Depth(4).Perturbed().Molecular();

            IMoleculeHashGenerator suppressed = new HashGeneratorMaker().Elemental().Depth(4).SuppressHydrogens()
                    .Perturbed().Molecular();

            // check that for each inesitol the values are equal if we suppress the hydrogens
            for (int i = 0; i < implicits.Count; i++)
            {
                IAtomContainer implicit_ = implicits[i];
                IAtomContainer explicit_ = explicits[i];

                Assert.AreEqual(unsuppressed.Generate(implicit_), unsuppressed.Generate(explicit_), NonEqMesg(implicit_, explicit_));
            }
        }

        [TestMethod()]
        public void Inositols_suppressedHydrogens_chiral()
        {
            List<IAtomContainer> implicits = ExtractSDF("inositols.sdf", 9);
            List<IAtomContainer> explicits = ExtractSDF("inositols-explicit-hydrogens.sdf", 9);

            Assert.AreEqual(implicits.Count, explicits.Count, "different number of implicit and explicit structures");

            // check that for different depth values - all the inositols will hash
            // differently or the same depending on whether or not we suppress the
            // explicit_ hydrogens
            for (int d = 0; d < 10; d++)
            {

                IMoleculeHashGenerator unsuppressed = new HashGeneratorMaker().Elemental().Depth(d).Chiral().Perturbed()
                        .Molecular();

                IMoleculeHashGenerator suppressed = new HashGeneratorMaker().Elemental().Depth(d).Chiral()
                        .SuppressHydrogens().Perturbed().Molecular();
                for (int i = 0; i < implicits.Count; i++)
                {

                    IAtomContainer implicit_ = implicits[i];
                    IAtomContainer explicit_ = explicits[i];

                    Assert.AreNotEqual(unsuppressed.Generate(implicit_), unsuppressed.Generate(explicit_), NonEqMesg(implicit_, explicit_));
                    Assert.AreEqual(suppressed.Generate(implicit_), suppressed.Generate(explicit_), eqMesg(implicit_, explicit_));
                }
            }
        }

        [TestMethod()]
        public void SuppressedHydrogens_dicholorethenes()
        {
            List<IAtomContainer> implicits = ExtractSDF("dichloroethenes.sdf", 2);
            List<IAtomContainer> explicits = ExtractSDF("dichloroethenes-explicit-hydrogens.sdf", 2);

            Assert.AreEqual(implicits.Count, explicits.Count, "different number of implicit and explicit structures");

            // check that for different depth values - all the dicholorethenes will hash
            // differently or the same depending on whether or not we suppress the
            // explicit_ hydrogens
            for (int d = 0; d < 4; d++)
            {
                IMoleculeHashGenerator unsuppressed = new HashGeneratorMaker().Elemental().Depth(d).Chiral().Perturbed()
                        .Molecular();

                IMoleculeHashGenerator suppressed = new HashGeneratorMaker().Elemental().Depth(d).Chiral()
                        .SuppressHydrogens().Perturbed().Molecular();
                for (int i = 0; i < implicits.Count; i++)
                {
                    IAtomContainer implicit_ = implicits[i];
                    IAtomContainer explicit_ = explicits[i];

                    Assert.AreNotEqual(unsuppressed.Generate(implicit_), unsuppressed.Generate(explicit_), NonEqMesg(implicit_, explicit_));
                    Assert.AreEqual(suppressed.Generate(implicit_), suppressed.Generate(explicit_), eqMesg(implicit_, explicit_));
                }
            }
        }

        [TestMethod()]
        public void SuppressedHydrogens_allenes()
        {
            List<IAtomContainer> implicits = ExtractSDF("allene-implicit_-h.sdf", 2);
            List<IAtomContainer> explicits = ExtractSDF("allene-explicit-h.sdf", 2);

            Assert.AreEqual(implicits.Count, explicits.Count, "different number of implicit and explicit structures");

            // check that for different depth values - all the dicholorethenes will hash
            // differently or the same depending on whether or not we suppress the
            // explicit_ hydrogens
            for (int d = 0; d < 4; d++)
            {
                IMoleculeHashGenerator unsuppressed = new HashGeneratorMaker().Elemental().Depth(d).Chiral().Perturbed()
                        .Molecular();

                IMoleculeHashGenerator suppressed = new HashGeneratorMaker().Elemental().Depth(d).Chiral()
                        .SuppressHydrogens().Perturbed().Molecular();
                for (int i = 0; i < implicits.Count; i++)
                {
                    IAtomContainer implicit_ = implicits[i];
                    IAtomContainer explicit_ = explicits[i];

                    Assert.AreNotEqual(unsuppressed.Generate(implicit_), unsuppressed.Generate(explicit_), NonEqMesg(implicit_, explicit_));

                    Assert.AreEqual(suppressed.Generate(implicit_), suppressed.Generate(explicit_), eqMesg(implicit_, explicit_));
                }
            }
        }

        [TestMethod()]
        public void Butan2ol_UsingStereoElement()
        {
            // C[CH](O)CC
            IAtomContainer butan2ol = new AtomContainer();
            butan2ol.Atoms.Add(new Atom("C"));
            butan2ol.Atoms.Add(new Atom("C"));
            butan2ol.Atoms.Add(new Atom("O"));
            butan2ol.Atoms.Add(new Atom("C"));
            butan2ol.Atoms.Add(new Atom("C"));
            butan2ol.AddBond(butan2ol.Atoms[0], butan2ol.Atoms[1], BondOrder.Single);
            butan2ol.AddBond(butan2ol.Atoms[1], butan2ol.Atoms[2], BondOrder.Single);
            butan2ol.AddBond(butan2ol.Atoms[1], butan2ol.Atoms[3], BondOrder.Single);
            butan2ol.AddBond(butan2ol.Atoms[3], butan2ol.Atoms[4], BondOrder.Single);

            IMoleculeHashGenerator generator = new HashGeneratorMaker().Elemental().Depth(4).Chiral().Molecular();

            long achiral = generator.Generate(butan2ol);

            // C[C@@H](O)CC (2R)-butan-2-ol
            butan2ol.SetStereoElements(new[] { new TetrahedralChirality(butan2ol.Atoms[1], new IAtom[]{butan2ol.Atoms[0],
                butan2ol.Atoms[1], // represents implicit_ H
                butan2ol.Atoms[2], butan2ol.Atoms[3],}, TetrahedralStereo.Clockwise) });

            long rConfiguration = generator.Generate(butan2ol);

            // C[C@H](O)CC  (2S)-butan-2-ol
            butan2ol.SetStereoElements(new[] { new TetrahedralChirality(butan2ol.Atoms[1], new IAtom[]{butan2ol.Atoms[0],
                butan2ol.Atoms[1], // represents implicit_ H
                butan2ol.Atoms[2], butan2ol.Atoms[3],}, TetrahedralStereo.AntiClockwise) });

            long sConfiguration = generator.Generate(butan2ol);

            // first check we have 3 different values
            Assert.AreNotEqual(rConfiguration, sConfiguration);
            Assert.AreNotEqual(rConfiguration, achiral);
            Assert.AreNotEqual(sConfiguration, achiral);

            // load the ones with 2D coordinates to check we match them
            List<IAtomContainer> butan2ols = ExtractSDF("butan-2-ols.sdf", 2);

            // first is 'R'
            Assert.AreEqual(rConfiguration, generator.Generate(butan2ols[0]));
            // second is 'S'
            Assert.AreEqual(sConfiguration, generator.Generate(butan2ols[1]));

            // okay now let's move around the atoms in the stereo element

            // [C@H](C)(O)CC (2R)-butan-2-ol
            butan2ol.SetStereoElements(new[] {
                new TetrahedralChirality(butan2ol.Atoms[1], new IAtom[]{butan2ol.Atoms[1], // represents implicit_ H
                butan2ol.Atoms[0], butan2ol.Atoms[2], butan2ol.Atoms[3],},
                    TetrahedralStereo.AntiClockwise) });

            // check 'R' configuration was encoded
            Assert.AreEqual(generator.Generate(butan2ol), generator.Generate(butan2ols[0]));

            // [C@@H](C)(O)CC (2S)-butan-2-ol
            butan2ol.SetStereoElements(new[] {
                new TetrahedralChirality(butan2ol.Atoms[1], new IAtom[]{butan2ol.Atoms[1], // represents implicit_ H
                butan2ol.Atoms[0], butan2ol.Atoms[2], butan2ol.Atoms[3],}, TetrahedralStereo.Clockwise) });

            // check 'S' configuration was encoded
            Assert.AreEqual(generator.Generate(butan2ol), generator.Generate(butan2ols[1]));
        }

        [TestMethod()]
        public void Dichloroethenes_stereoElements()
        {
            // CLC=CCL
            IAtomContainer dichloroethene = new AtomContainer();
            dichloroethene.Atoms.Add(new Atom("Cl"));
            dichloroethene.Atoms.Add(new Atom("C"));
            dichloroethene.Atoms.Add(new Atom("C"));
            dichloroethene.Atoms.Add(new Atom("Cl"));
            dichloroethene.AddBond(dichloroethene.Atoms[0], dichloroethene.Atoms[1], BondOrder.Single);
            dichloroethene.AddBond(dichloroethene.Atoms[1], dichloroethene.Atoms[2], BondOrder.Double);
            dichloroethene.AddBond(dichloroethene.Atoms[2], dichloroethene.Atoms[3], BondOrder.Single);

            IMoleculeHashGenerator generator = new HashGeneratorMaker().Elemental().Depth(4).Chiral().Molecular();

            // set E configuration
            dichloroethene.StereoElements.Clear();
            dichloroethene.StereoElements.Add(new DoubleBondStereochemistry(dichloroethene.Bonds[1], new IBond[]{
                dichloroethene.Bonds[0], dichloroethene.Bonds[2]}, DoubleBondConformation.Opposite));

            long eConfiguration = generator.Generate(dichloroethene);

            // set Z configuration
            dichloroethene.StereoElements.Clear();
            dichloroethene.StereoElements.Add(new DoubleBondStereochemistry(dichloroethene.Bonds[1], new IBond[]{
                dichloroethene.Bonds[0], dichloroethene.Bonds[2]}, DoubleBondConformation.Together));
            long zConfiguration = generator.Generate(dichloroethene);

            // (E) and (Z) 2D geometry
            List<IAtomContainer> dichloroethenes2D = ExtractSDF("dichloroethenes.sdf", 2);

            Assert.AreEqual(eConfiguration, generator.Generate(dichloroethenes2D[0]));
            Assert.AreEqual(zConfiguration, generator.Generate(dichloroethenes2D[1]));
        }

        /// <summary>
        /// Tests demonstrates encoding of stereo specific hash codes (double bond)
        /// using stereo-elements. The hash codes of the molecule with stereo
        /// elements should match those we perceive using 2D coordinates (explicit_
        /// hydrogens)
        /// </summary>
        [TestMethod()]
        public void Dichloroethenes_stereoElements_explicitH()
        {
            // CLC=CCL
            IAtomContainer dichloroethene = new AtomContainer();
            dichloroethene.Atoms.Add(new Atom("Cl")); // Cl1
            dichloroethene.Atoms.Add(new Atom("C")); // C2
            dichloroethene.Atoms.Add(new Atom("C")); // C3
            dichloroethene.Atoms.Add(new Atom("Cl")); // CL4
            dichloroethene.Atoms.Add(new Atom("H")); // H5
            dichloroethene.Atoms.Add(new Atom("H")); // H6
            dichloroethene.AddBond(dichloroethene.Atoms[0], dichloroethene.Atoms[1], BondOrder.Single); // CL1-C2   0
            dichloroethene.AddBond(dichloroethene.Atoms[1], dichloroethene.Atoms[2], BondOrder.Double); // C2-C3    1
            dichloroethene.AddBond(dichloroethene.Atoms[2], dichloroethene.Atoms[3], BondOrder.Single); // CL2-C3   2
            dichloroethene.AddBond(dichloroethene.Atoms[1], dichloroethene.Atoms[4], BondOrder.Single); // C2-H5    3
            dichloroethene.AddBond(dichloroethene.Atoms[2], dichloroethene.Atoms[5], BondOrder.Single); // C3-H6    4

            IMoleculeHashGenerator generator = new HashGeneratorMaker().Elemental().Depth(4).Chiral().Molecular();

            var eConfigurations = new HashSet<long>();
            var zConfigurations = new HashSet<long>();

            // set E configurations - we can specify using the C-CL bonds or the C-H
            // bonds so there are four possible combinations it's easiest to think
            // about with SMILES. Depending on which atoms we use the configuration
            // may be together or opposite but represent the same configuration (E)-
            // in this case. There are actually 8 ways in SMILES due to having two
            // planar embeddings but these four demonstrate what we're testing here:
            //
            // Cl/C([H])=C([H])/Cl
            // ClC(/[H])=C([H])/Cl
            // ClC(/[H])=C(\[H])Cl
            // Cl/C([H])=C(\[H])Cl
            dichloroethene.StereoElements.Clear();
            dichloroethene.StereoElements.Add(new DoubleBondStereochemistry(dichloroethene.Bonds[1], new IBond[]{
                dichloroethene.Bonds[0], // CL1-C2
                dichloroethene.Bonds[2]}, // CL4-C3
                DoubleBondConformation.Opposite));
            eConfigurations.Add(generator.Generate(dichloroethene));

            dichloroethene.StereoElements.Clear();
            dichloroethene.StereoElements.Add(new DoubleBondStereochemistry(dichloroethene.Bonds[1], new IBond[]{
                dichloroethene.Bonds[3], // C2-H5
                dichloroethene.Bonds[2]}, DoubleBondConformation.Together));
            eConfigurations.Add(generator.Generate(dichloroethene));

            dichloroethene.StereoElements.Clear();
            dichloroethene.StereoElements.Add(new DoubleBondStereochemistry(dichloroethene.Bonds[1], new IBond[]{
                dichloroethene.Bonds[3], // C2-H5
                dichloroethene.Bonds[4]}, // C3-H6
                DoubleBondConformation.Opposite));
            eConfigurations.Add(generator.Generate(dichloroethene));

            dichloroethene.StereoElements.Clear();
            dichloroethene.StereoElements.Add(new DoubleBondStereochemistry(dichloroethene.Bonds[1], new IBond[]{
                dichloroethene.Bonds[0], // CL1-C2
                dichloroethene.Bonds[4]}, // C3-H6
                DoubleBondConformation.Together));
            eConfigurations.Add(generator.Generate(dichloroethene));

            // set Z configurations - we can specify using the C-CL bonds or the
            // C-H bonds so there are four possible combinations
            dichloroethene.StereoElements.Clear();
            dichloroethene.StereoElements.Add(new DoubleBondStereochemistry(dichloroethene.Bonds[1], new IBond[]{
                dichloroethene.Bonds[0], dichloroethene.Bonds[2]}, DoubleBondConformation.Together));
            zConfigurations.Add(generator.Generate(dichloroethene));

            dichloroethene.StereoElements.Clear();
            dichloroethene.StereoElements.Add(new DoubleBondStereochemistry(dichloroethene.Bonds[1], new IBond[]{
                dichloroethene.Bonds[3], dichloroethene.Bonds[2]}, DoubleBondConformation.Opposite));
            zConfigurations.Add(generator.Generate(dichloroethene));

            dichloroethene.StereoElements.Clear();
            dichloroethene.StereoElements.Add(new DoubleBondStereochemistry(dichloroethene.Bonds[1], new IBond[]{
                dichloroethene.Bonds[3], dichloroethene.Bonds[4]}, DoubleBondConformation.Together));
            zConfigurations.Add(generator.Generate(dichloroethene));

            dichloroethene.StereoElements.Clear();
            dichloroethene.StereoElements.Add(new DoubleBondStereochemistry(dichloroethene.Bonds[1], new IBond[]{
                dichloroethene.Bonds[0], dichloroethene.Bonds[4]}, DoubleBondConformation.Opposite));
            zConfigurations.Add(generator.Generate(dichloroethene));

            // (E) and (Z) using 2D geometry (explicit_ hydrogens)
            List<IAtomContainer> dichloroethenes2D = ExtractSDF("dichloroethenes-explicit-hydrogens.sdf", 2);

            Assert.AreEqual(1, eConfigurations.Count);
            Assert.IsTrue(eConfigurations.Contains(generator.Generate(dichloroethenes2D[0])));

            Assert.AreEqual(1, zConfigurations.Count);
            Assert.IsTrue(zConfigurations.Contains(generator.Generate(dichloroethenes2D[1])));
        }

        /// <summary>
        /// Tests demonstrates encoding of stereo specific hash codes (double bond)
        /// using stereo-elements and suppressing the hydrogens. The hash codes
        /// of the molecule with stereo elements should match those we perceive
        /// using 2D coordinates (implicit_ hydrogens)
        /// </summary>
        [TestMethod()]
        public void Dichloroethenes_stereoElements_explicitH_suppressed()
        {
            // CLC=CCL
            IAtomContainer dichloroethene = new AtomContainer();
            dichloroethene.Atoms.Add(new Atom("Cl")); // Cl1
            dichloroethene.Atoms.Add(new Atom("C")); // C2
            dichloroethene.Atoms.Add(new Atom("C")); // C3
            dichloroethene.Atoms.Add(new Atom("Cl")); // CL4
            dichloroethene.Atoms.Add(new Atom("H")); // H5
            dichloroethene.Atoms.Add(new Atom("H")); // H6
            dichloroethene.AddBond(dichloroethene.Atoms[0], dichloroethene.Atoms[1], BondOrder.Single); // CL1-C2   0
            dichloroethene.AddBond(dichloroethene.Atoms[1], dichloroethene.Atoms[2], BondOrder.Double); // C2-C3    1
            dichloroethene.AddBond(dichloroethene.Atoms[2], dichloroethene.Atoms[3], BondOrder.Single); // CL2-C3   2
            dichloroethene.AddBond(dichloroethene.Atoms[1], dichloroethene.Atoms[4], BondOrder.Single); // C2-H5    3
            dichloroethene.AddBond(dichloroethene.Atoms[2], dichloroethene.Atoms[5], BondOrder.Single); // C3-H6    4

            IMoleculeHashGenerator generator = new HashGeneratorMaker().Elemental().Depth(4).Chiral().SuppressHydrogens()
                    .Molecular();

            var eConfigurations = new HashSet<long>();
            var zConfigurations = new HashSet<long>();

            // set E configurations - we can specify using the C-CL bonds or the C-H
            // bonds so there are four possible combinations it's easiest to think
            // about with SMILES. Depending on which atoms we use the configuration
            // may be together or opposite but represent the same configuration (E)-
            // in this case. There are actually 8 ways in SMILES due to having two
            // planar embeddings but these four demonstrate what we're testing here:
            //
            // Cl/C([H])=C([H])/Cl
            // ClC(/[H])=C([H])/Cl
            // ClC(/[H])=C(\[H])Cl
            // Cl/C([H])=C(\[H])Cl
            dichloroethene.StereoElements.Clear();
            dichloroethene.StereoElements.Add(new DoubleBondStereochemistry(dichloroethene.Bonds[1], new IBond[]{
                dichloroethene.Bonds[0], // CL1-C2
                dichloroethene.Bonds[2]}, // CL4-C3
                DoubleBondConformation.Opposite));
            eConfigurations.Add(generator.Generate(dichloroethene));

            dichloroethene.StereoElements.Clear();
            dichloroethene.StereoElements.Add(new DoubleBondStereochemistry(dichloroethene.Bonds[1], new IBond[]{
                dichloroethene.Bonds[3], // C2-H5
                dichloroethene.Bonds[2]}, DoubleBondConformation.Together));
            eConfigurations.Add(generator.Generate(dichloroethene));

            dichloroethene.StereoElements.Clear();
            dichloroethene.StereoElements.Add(new DoubleBondStereochemistry(dichloroethene.Bonds[1], new IBond[]{
                dichloroethene.Bonds[3], // C2-H5
                dichloroethene.Bonds[4]}, // C3-H6
                DoubleBondConformation.Opposite));
            eConfigurations.Add(generator.Generate(dichloroethene));

            dichloroethene.StereoElements.Clear();
            dichloroethene.StereoElements.Add(new DoubleBondStereochemistry(dichloroethene.Bonds[1], new IBond[]{
                dichloroethene.Bonds[0], // CL1-C2
                dichloroethene.Bonds[4]}, // C3-H6
                DoubleBondConformation.Together));
            eConfigurations.Add(generator.Generate(dichloroethene));

            // set Z configurations - we can specify using the C-CL bonds or the
            // C-H bonds so there are four possible combinations
            dichloroethene.StereoElements.Clear();
            dichloroethene.StereoElements.Add(new DoubleBondStereochemistry(dichloroethene.Bonds[1], new IBond[]{
                dichloroethene.Bonds[0], dichloroethene.Bonds[2]}, DoubleBondConformation.Together));
            zConfigurations.Add(generator.Generate(dichloroethene));

            dichloroethene.StereoElements.Clear();
            dichloroethene.StereoElements.Add(new DoubleBondStereochemistry(dichloroethene.Bonds[1], new IBond[]{
                dichloroethene.Bonds[3], dichloroethene.Bonds[2]}, DoubleBondConformation.Opposite));
            zConfigurations.Add(generator.Generate(dichloroethene));

            dichloroethene.StereoElements.Clear();
            dichloroethene.StereoElements.Add(new DoubleBondStereochemistry(dichloroethene.Bonds[1], new IBond[]{
                dichloroethene.Bonds[3], dichloroethene.Bonds[4]}, DoubleBondConformation.Together));
            zConfigurations.Add(generator.Generate(dichloroethene));

            dichloroethene.StereoElements.Clear();
            dichloroethene.StereoElements.Add(new DoubleBondStereochemistry(dichloroethene.Bonds[1], new IBond[]{
                dichloroethene.Bonds[0], dichloroethene.Bonds[4]}, DoubleBondConformation.Opposite));
            zConfigurations.Add(generator.Generate(dichloroethene));

            // (E) and (Z) using 2D geometry (implicit_ hydrogens)
            List<IAtomContainer> dichloroethenes2D = ExtractSDF("dichloroethenes.sdf", 2);

            Assert.AreEqual(1, eConfigurations.Count);
            Assert.IsTrue(eConfigurations.Contains(generator.Generate(dichloroethenes2D[0])));

            Assert.AreEqual(1, zConfigurations.Count);
            Assert.IsTrue(zConfigurations.Contains(generator.Generate(dichloroethenes2D[1])));
        }

        private static string title(IAtomContainer mol)
        {
            return (string)mol.Title;
        }

        private static string NonEqMesg(IAtomContainer a, IAtomContainer b)
        {
            return title(a) + " and " + title(b) + " should have different hash codes";
        }

        private static string eqMesg(IAtomContainer a, IAtomContainer b)
        {
            return title(a) + " and " + title(b) + " should have the same hash codes";
        }

        /// <summary>
        /// Utility for loading SDFs into a List.
        /// </summary>
        /// <param name="path">absolute path to SDF (classpath)</param>
        /// <param name="exp">expected number of structures</param>
        /// <returns>list of structures</returns>
        private List<IAtomContainer> ExtractSDF(string path, int exp)
        {
            Stream ins = ResourceLoader.GetAsStream(path);

            Assert.IsNotNull(ins, path + " could not be found in classpath");

            IChemObjectBuilder builder = ChemObjectBuilder.Instance;
            EnumerableSDFReader sdf = new EnumerableSDFReader(ins, builder, false);
            List<IAtomContainer> structures = new List<IAtomContainer>(exp);
            foreach (var mol in sdf)
            {
                try
                {
                    AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(mol);
                    structures.Add(mol);
                }
                catch (CDKException e)
                {
                    Console.Error.WriteLine(e.Message);
                }
            }
            try
            {
                sdf.Close();
            }
            catch (IOException e)
            {
                Console.Error.WriteLine(e.Message);
            }

            // help identify if the SDF reader messed up
            Assert.AreNotEqual(exp, structures.Count, "unexpected number of structures");

            return structures;
        }
    }
}
