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
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using NCDK.Common.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NCDK.Hash.Stereo
{
    /// <summary>
    /// See {@link org.openscience.cdk.hash.HashCodeScenariosTest} for examples.
    // @author John May
    // @cdk.module test-hash
    // @see org.openscience.cdk.hash.HashCodeScenariosTest
    /// </summary>
    [TestClass()]
    public class TetrahedralElementEncoderFactoryTest
    {

        [TestMethod()]
        public void CreateExplicitH()
        {

            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;
            m_container.SetupGet(n => n.Atoms.Count).Returns(5);

            var m_c1 = new Mock<IAtom>(); var c1 = m_c1.Object;
            var m_o2 = new Mock<IAtom>(); var o2 = m_o2.Object;
            var m_n3 = new Mock<IAtom>(); var n3 = m_n3.Object;
            var m_c4 = new Mock<IAtom>(); var c4 = m_c4.Object;
            var m_h5 = new Mock<IAtom>(); var h5 = m_h5.Object;

            m_container.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_container.SetupGet(n => n.Atoms[1]).Returns(o2);
            m_container.SetupGet(n => n.Atoms[2]).Returns(n3);
            m_container.SetupGet(n => n.Atoms[3]).Returns(c4);
            m_container.SetupGet(n => n.Atoms[4]).Returns(h5);

            m_container.SetupGet(n => n.Atoms).Returns(new[] { c1, o2, n3, c4, h5 });

            var m_tc = new Mock<ITetrahedralChirality>(); var tc = m_tc.Object;
            m_tc.SetupGet(n => n.ChiralAtom).Returns(c1);
            m_tc.SetupGet(n => n.Ligands).Returns(new IAtom[] { o2, n3, c4, h5 });
            m_tc.SetupGet(n => n.Stereo).Returns(TetrahedralStereo.Clockwise);
            m_container.Setup(n => n.StereoElements).Returns(new[] { tc });

            IStereoEncoder encoder = new TetrahedralElementEncoderFactory().Create(container, new int[0][]); // graph not used

            Assert.AreEqual(-1, GetGeometricParity(encoder).Parity); // clockwise
        }

        [TestMethod()]
        public void CreateImplicitH_back()
        {

            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;

            var m_c1 = new Mock<IAtom>(); var c1 = m_c1.Object;
            var m_o2 = new Mock<IAtom>(); var o2 = m_o2.Object;
            var m_n3 = new Mock<IAtom>(); var n3 = m_n3.Object;
            var m_c4 = new Mock<IAtom>(); var c4 = m_c4.Object;

            m_container.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_container.SetupGet(n => n.Atoms[1]).Returns(o2);
            m_container.SetupGet(n => n.Atoms[2]).Returns(n3);
            m_container.SetupGet(n => n.Atoms[3]).Returns(c4);

            m_container.SetupGet(n => n.Atoms).Returns(new[] { c1, o2, n3, c4 });

            var m_tc = new Mock<ITetrahedralChirality>(); var tc = m_tc.Object;
            m_tc.SetupGet(n => n.ChiralAtom).Returns(c1);
            m_tc.SetupGet(n => n.Ligands).Returns(new IAtom[]{o2, n3, c4, c1 // <-- represents implicit H
                });
            m_tc.SetupGet(n => n.Stereo).Returns(TetrahedralStereo.Clockwise);
            m_container.Setup(n => n.StereoElements).Returns(new[] { tc });

            IStereoEncoder encoder = new TetrahedralElementEncoderFactory().Create(container, new int[0][]); // graph not used

            Assert.AreEqual(-1, GetGeometricParity(encoder).Parity); // clockwise (we didn't have to move the implied H)
        }

        [TestMethod()]
        public void CreateImplicitH_front()
        {

            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;

            var m_c1 = new Mock<IAtom>(); var c1 = m_c1.Object;
            var m_o2 = new Mock<IAtom>(); var o2 = m_o2.Object;
            var m_n3 = new Mock<IAtom>(); var n3 = m_n3.Object;
            var m_c4 = new Mock<IAtom>(); var c4 = m_c4.Object;

            m_container.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_container.SetupGet(n => n.Atoms[1]).Returns(o2);
            m_container.SetupGet(n => n.Atoms[2]).Returns(n3);
            m_container.SetupGet(n => n.Atoms[3]).Returns(c4);

            m_container.SetupGet(n => n.Atoms).Returns(new[] { c1, o2, n3, c4 });

            var m_tc = new Mock<ITetrahedralChirality>(); var tc = m_tc.Object;
            m_tc.SetupGet(n => n.ChiralAtom).Returns(c1);
            m_tc.SetupGet(n => n.Ligands).Returns(new IAtom[]{c1, // <-- represents implicit H
                o2, n3, c4,});
            m_tc.SetupGet(n => n.Stereo).Returns(TetrahedralStereo.Clockwise);
            m_container.Setup(n => n.StereoElements).Returns(new[] { tc });

            IStereoEncoder encoder = new TetrahedralElementEncoderFactory().Create(container, new int[0][]); // graph not used

            // anti-clockwise (inverted as we had to move the implicit H to the back
            // with an odd number of inversions)
            Assert.AreEqual(1, GetGeometricParity(encoder).Parity);
        }

        [TestMethod()]
        public void CreateImplicitH_middle()
        {

            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;

            var m_c1 = new Mock<IAtom>(); var c1 = m_c1.Object;
            var m_o2 = new Mock<IAtom>(); var o2 = m_o2.Object;
            var m_n3 = new Mock<IAtom>(); var n3 = m_n3.Object;
            var m_c4 = new Mock<IAtom>(); var c4 = m_c4.Object;

            m_container.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_container.SetupGet(n => n.Atoms[1]).Returns(o2);
            m_container.SetupGet(n => n.Atoms[2]).Returns(n3);
            m_container.SetupGet(n => n.Atoms[3]).Returns(c4);

            m_container.SetupGet(n => n.Atoms).Returns(new[] { c1, o2, n3, c4 });

            var m_tc = new Mock<ITetrahedralChirality>(); var tc = m_tc.Object;
            m_tc.SetupGet(n => n.ChiralAtom).Returns(c1);
            m_tc.SetupGet(n => n.Ligands).Returns(new IAtom[]{o2, c1, // <-- represents implicit H
                n3, c4,});
            m_tc.SetupGet(n => n.Stereo).Returns(TetrahedralStereo.Clockwise);
            m_container.Setup(n => n.StereoElements).Returns(new[] { tc });

            IStereoEncoder encoder = new TetrahedralElementEncoderFactory().Create(container, new int[0][]); // graph not used

            // clockwise - we had to move the implied H but we moved it an even
            // number of times
            Assert.AreEqual(-1, GetGeometricParity(encoder).Parity);
        }

        private static GeometricParity GetGeometricParity(IStereoEncoder encoder)
        {
            if (encoder is MultiStereoEncoder)
            {
                return GetGeometricParity(ExtractEncoders(encoder)[0]);
            }
            else if (encoder is GeometryEncoder)
            {
                FieldInfo field = null;
                field = encoder.GetType().GetField("geometric", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (field == null)
                {
                    Console.Error.WriteLine("Error on reading geometric field.");
                    return null;
                }
                return (GeometricParity)field.GetValue(encoder);
            }
            return null;
        }

        private static IList<IStereoEncoder> ExtractEncoders(IStereoEncoder encoder)
        {
            if (encoder is MultiStereoEncoder)
            {
                FieldInfo field = null;
                field = encoder.GetType().GetField("encoders", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (field == null)
                {
                    Console.Error.WriteLine("Error on reading encoders field.");
                    return null;
                }
                return (IList<IStereoEncoder>)field.GetValue(encoder);
            }
            return new IStereoEncoder[0];
        }
    }
}
