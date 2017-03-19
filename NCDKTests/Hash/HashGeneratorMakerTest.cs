/*
 * Copyright (c) 2013. John May <jwmay@users.sf.net>
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

using NCDK.Common.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NCDK.Hash.Stereo;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NCDK.Hash
{
    // @author John May
    // @cdk.module test-hash
    [TestClass()]
    public class HashGeneratorMakerTest
    {
        [TestMethod()]
        public void TestElemental()
        {
            IAtomHashGenerator generator = new HashGeneratorMaker().Depth(0).Elemental().Atomic();
            var encoders = GetEncoders((BasicAtomHashGenerator)generator);
            Assert.AreEqual(1, encoders.Count);
            Assert.AreEqual(BasicAtomEncoder.AtomicNumber, encoders[0]);
        }

        [TestMethod()]
        public void TestIsotopic()
        {
            IAtomHashGenerator generator = new HashGeneratorMaker().Depth(0).Isotopic().Atomic();
            var encoders = GetEncoders((BasicAtomHashGenerator)generator);
            Assert.AreEqual(1, encoders.Count);
            Assert.AreEqual(BasicAtomEncoder.MassNumber, encoders[0]);
        }

        [TestMethod()]
        public void TestCharged()
        {
            IAtomHashGenerator generator = new HashGeneratorMaker().Depth(0).Charged().Atomic();
            var encoders = GetEncoders((BasicAtomHashGenerator)generator);
            Assert.AreEqual(1, encoders.Count);
            Assert.AreEqual(BasicAtomEncoder.FormalCharge, encoders[0]);
        }

        [TestMethod()]
        public void TestRadical()
        {
            IAtomHashGenerator generator = new HashGeneratorMaker().Depth(0).Radical().Atomic();
            var encoders = GetEncoders((BasicAtomHashGenerator)generator);
            Assert.AreEqual(1, encoders.Count);
            Assert.AreEqual(BasicAtomEncoder.FreeRadicals, encoders[0]);
        }

        [TestMethod()]
        public void TestOrbital()
        {
            IAtomHashGenerator generator = new HashGeneratorMaker().Depth(0).Orbital().Atomic();
            var encoders = GetEncoders((BasicAtomHashGenerator)generator);
            Assert.AreEqual(1, encoders.Count);
            Assert.AreEqual(BasicAtomEncoder.OrbitalHybridization, encoders[0]);
        }

        [TestMethod()]
        public void TestChiral()
        {
            IAtomHashGenerator generator = new HashGeneratorMaker().Depth(0).Elemental().Chiral().Atomic();
            Assert.AreNotEqual(StereoEncoderFactory.Empty, Encoder(generator));
        }

        [TestMethod()]
        public void TestPerturbed()
        {
            IAtomHashGenerator g1 = new HashGeneratorMaker().Depth(0).Elemental().Perturbed().Atomic();

            Assert.IsTrue(g1 is PerturbedAtomHashGenerator);
        }

        [TestMethod()]
        public void TestPerturbedWith()
        {
            var m_mock = new Mock<EquivalentSetFinder>(); var mock = m_mock.Object;
            IAtomHashGenerator g1 = new HashGeneratorMaker().Depth(0).Elemental().PerturbWith(mock).Atomic();

            Assert.IsTrue(g1 is PerturbedAtomHashGenerator);
            var field = g1.GetType().GetField("finder", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.AreSame(mock, (EquivalentSetFinder)field.GetValue(g1));
        }

        [TestMethod()]
        public void TestOrdering()
        {
            IAtomHashGenerator g1 = new HashGeneratorMaker().Depth(0).Elemental().Isotopic().Charged().Atomic();
            IAtomHashGenerator g2 = new HashGeneratorMaker().Depth(0).Isotopic().Charged().Elemental().Atomic();
            Assert.AreEqual(3, GetEncoders(g1).Count);
            Assert.IsTrue(Compares.AreDeepEqual(GetEncoders(g1), GetEncoders(g2)));
        }

        [TestMethod()]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void TestEncode_Null()
        {
            new HashGeneratorMaker().Encode(null);
        }

        [TestMethod()]
        public void TestEncode()
        {
            var m_e1 = new Mock<IAtomEncoder>(); var e1 = m_e1.Object;
            var m_e2 = new Mock<IAtomEncoder>(); var e2 = m_e2.Object;
            IAtomHashGenerator generator = new HashGeneratorMaker().Depth(0).Encode(e1).Encode(e2).Atomic();
            var encoders = GetEncoders((BasicAtomHashGenerator)generator);
            Assert.AreEqual(2, encoders.Count);
            Assert.AreEqual(e1, encoders[0]);
            Assert.AreEqual(e2, encoders[1]);

            generator = new HashGeneratorMaker().Depth(0).Encode(e2).Encode(e1).Atomic();
            encoders = GetEncoders((BasicAtomHashGenerator)generator);
            Assert.AreEqual(2, encoders.Count);
            Assert.AreEqual(e2, encoders[0]);
            Assert.AreEqual(e1, encoders[1]);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestNoDepth()
        {
            new HashGeneratorMaker().Atomic();
        }

        [TestMethod()]
        public void TestAtomic()
        {
            Assert.IsNotNull(new HashGeneratorMaker().Depth(0).Elemental().Atomic());
        }

        [TestMethod()]
        public void TestMolecular()
        {
            Assert.IsNotNull(new HashGeneratorMaker().Depth(0).Elemental().Molecular());
        }

        [TestMethod()]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestEnsemble()
        {
            new HashGeneratorMaker().Depth(0).Elemental().Ensemble();
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestMissingEncoders()
        {
            new HashGeneratorMaker().Depth(0).Atomic();
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInvalidDepth()
        {
            new HashGeneratorMaker().Depth(-1);
        }

        [TestMethod()]
        public void SuppressHydrogens()
        {
            IAtomHashGenerator generator = new HashGeneratorMaker().Elemental().Depth(0).SuppressHydrogens().Atomic();
            Assert.IsInstanceOfType(generator, typeof(SuppressedAtomHashGenerator));
        }

        [TestMethod()]
        public void TestDepth()
        {
            IAtomHashGenerator generator = new HashGeneratorMaker().Depth(5).Elemental().Atomic();
            var depthField = generator.GetType().GetField("depth", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            int value = (int)depthField.GetValue(generator);
            Assert.AreEqual(5, value);
        }

        public static IStereoEncoderFactory Encoder(IAtomHashGenerator generator)
        {
            if (generator is BasicAtomHashGenerator)
            {
                var f = generator.GetType().GetField("factory", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (f == null)
                    Console.Error.WriteLine("Field 'factory' is not found.");
                return (IStereoEncoderFactory)f.GetValue(generator);
            }
            return null;
        }

        /// <summary>
        /// Extract the AtomEncoders using reflection
        ///
        /// <param name="generator">/// @return</param>
        /// </summary>
        public static IList<IAtomEncoder> GetEncoders(IAtomHashGenerator generator)
        {
            var field = generator.GetType().GetField("seedGenerator", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null)
            {
                Console.Error.WriteLine("Field 'seedGenerator' is not found.");
                goto Exit;
            }
            object o1 = field.GetValue(generator);
            if (o1 is SeedGenerator)
            {
                SeedGenerator seedGenerator = (SeedGenerator)o1;
                var f2 = seedGenerator.GetType().GetField("encoder", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (f2 == null)
                {
                    Console.Error.WriteLine("Field 'encoder' is not found.");
                    goto Exit;
                }
                object o2 = f2.GetValue(seedGenerator);
                return GetEncoders((ConjugatedAtomEncoder)o2);
            }
            Exit:
            return new List<IAtomEncoder>();
        }

        internal static IList<IAtomEncoder> GetEncoders(ConjugatedAtomEncoder conjugated)
        {
            var field = conjugated.GetType().GetField("encoders", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null)
            {
                Console.Error.WriteLine("Field 'encoders' is not found.");
                goto Exit;
            }
            return (IList<IAtomEncoder>)field.GetValue(conjugated);
            Exit:
            return new List<IAtomEncoder>();
        }
    }
}
