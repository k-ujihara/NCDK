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
using System;
using System.Collections.Generic;

namespace NCDK.Hash
{
    // @author John May
    // @cdk.module test-hash
    public class ConjugatedAtomEncoderTest
    {
        [TestMethod()]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void TestConstruction_Null()
        {
            new ConjugatedAtomEncoder(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TestConstruction_Empty()
        {
            new ConjugatedAtomEncoder(new IAtomEncoder[0]);
        }

        /// <summary>
        /// ensure we can modify the order after we have constructed the conjunction
        /// </summary>
        [TestMethod()]
        public void TestConstruction_Modification()
        {
            var m_a = new Mock<IAtomEncoder>(); var a = m_a.Object;
            var m_b = new Mock<IAtomEncoder>(); var b = m_b.Object;
            var m_c = new Mock<IAtomEncoder>(); var c = m_c.Object;
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;

            var encoders = new List<IAtomEncoder>();
            encoders.Add(a);
            encoders.Add(b);
            encoders.Add(c);
            IAtomEncoder encoder = new ConjugatedAtomEncoder(encoders);

            encoders.RemoveAt(2); // removing b should not affect the new encoder

            encoder.Encode(atom, container);

            // TODO: Moq does not support order feature.
            //InOrder order = InOrder(a, b, c);
            //order.m_a.Verify(n => n.Encode(atom, container), Times.Exactly(1));
            //order.m_b.Verify(n => n.Encode(atom, container), Times.Exactly(1));
            //order.m_c.Verify(n => n.Encode(atom, container), Times.Exactly(1));
            //VerifyNoMoreInteractions(a, b, c, atom, container);
        }

        [TestMethod()]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void TestCreate_Null()
        {
            ConjugatedAtomEncoder.Create(null, new IAtomEncoder[0]);
        }

        [TestMethod()]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void TestCreate_Null2()
        {
            ConjugatedAtomEncoder.Create(new Mock<IAtomEncoder>().Object, null);
        }

        [TestMethod()]
        public void TestEncode_Single()
        {
            var m_a = new Mock<IAtomEncoder>(); var a = m_a.Object;
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;

            IAtomEncoder encoder = new ConjugatedAtomEncoder(new[] { a });

            encoder.Encode(atom, container);

            m_a.Verify(n => n.Encode(atom, container), Times.Exactly(1));
            //VerifyNoMoreInteractions(a, atom, container);
        }

        [TestMethod()]
        public void TestEncode()
        {
            var m_a = new Mock<IAtomEncoder>(); var a = m_a.Object;
            var m_b = new Mock<IAtomEncoder>(); var b = m_b.Object;
            var m_c = new Mock<IAtomEncoder>(); var c = m_c.Object;
            var m_atom = new Mock<IAtom>(); var atom = m_atom.Object;
            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;

            IAtomEncoder encoder = new ConjugatedAtomEncoder(new[] { a, b, c, });

            encoder.Encode(atom, container);

            //InOrder order = InOrder(a, b, c);
            //order.m_a.Verify(n => n.Encode(atom, container), Times.Exactly(1));
            //order.m_b.Verify(n => n.Encode(atom, container), Times.Exactly(1));
            //order.m_c.Verify(n => n.Encode(atom, container), Times.Exactly(1));
            //VerifyNoMoreInteractions(a, b, c, atom, container);
        }
    }
}
