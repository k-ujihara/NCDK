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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace NCDK.Hash {
    /**
     * @author John May
     * @cdk.module test-hash
     */
    public class BasicMoleculeHashGeneratorTest {

        [TestMethod()][ExpectedException(typeof(System.ArgumentNullException))]
        public void TestConstruct_Null() {
            new BasicMoleculeHashGenerator(null);
        }

        [TestMethod()][ExpectedException(typeof(System.ArgumentNullException))]
        public void TestConstruct_NullPRNG() {
            new BasicMoleculeHashGenerator(new Mock<AtomHashGenerator>().Object, null);
        }

        [TestMethod()]
        public void TestGenerate() {

            var m_atomGenerator = new Mock<AtomHashGenerator>(); var atomGenerator = m_atomGenerator.Object;
            var m_prng = new Mock<Pseudorandom>(); var prng = m_prng.Object;
            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;

            MoleculeHashGenerator generator = new BasicMoleculeHashGenerator(atomGenerator, prng);

            m_atomGenerator.Setup(n => n.Generate(container)).Returns(new long[] { 1, 1, 1, 1 });
            m_prng.Setup(n => n.Next(1L)).Returns(1L);

            long hashCode = generator.Generate(container);

            m_atomGenerator.Verify(n => n.Generate(container), Times.Exactly(1));
            m_prng.Verify(n => n.Next(1L), Times.Exactly(3));

            //VerifyNoMoreInteractions(atomGenerator, container, prng);

            long expected = 2147483647L ^ 1L ^ 1L ^ 1L ^ 1L;

            Assert.AreEqual(expected, hashCode);

        }

        [TestMethod()]
        public void TestGenerate_Rotation() {

            var m_atomGenerator = new Mock<AtomHashGenerator>(); var atomGenerator = m_atomGenerator.Object;
            Xorshift xorshift = new Xorshift();
            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;

            MoleculeHashGenerator generator = new BasicMoleculeHashGenerator(atomGenerator, new Xorshift());

            m_atomGenerator.Setup(n => n.Generate(container)).Returns(new long[] { 5L, 5L, 5L, 5L });

            long hashCode = generator.Generate(container);

            m_atomGenerator.Verify(n => n.Generate(container), Times.Exactly(1));

            //VerifyNoMoreInteractions(atomGenerator, container);

            long expected = 2147483647L ^ 5L ^ xorshift.Next(5L) ^ xorshift.Next(xorshift.Next(5L))
                    ^ xorshift.Next(xorshift.Next(xorshift.Next(5L)));

            Assert.AreEqual(expected, hashCode);
        }
    }
}
