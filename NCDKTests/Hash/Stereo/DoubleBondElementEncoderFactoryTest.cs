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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NCDK.Hash.Stereo
{
    /// <summary>
    /// See. <see cref="HashCodeScenariosTest"/> for test which show
    /// example usage.
    /// </summary>
    // @author John May
    // @cdk.module test-hash
    [TestClass()]
    public class DoubleBondElementEncoderFactoryTest
    {
        [TestMethod()]
        public void Opposite()
        {
            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;
            m_container.SetupGet(n => n.Atoms.Count).Returns(4);

            var m_c1 = new Mock<IAtom>(); var c1 = m_c1.Object;
            var m_c2 = new Mock<IAtom>(); var c2 = m_c2.Object;
            var m_cl3 = new Mock<IAtom>(); var cl3 = m_cl3.Object;
            var m_cl4 = new Mock<IAtom>(); var cl4 = m_cl4.Object;

            m_container.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_container.SetupGet(n => n.Atoms[1]).Returns(c2);
            m_container.SetupGet(n => n.Atoms[2]).Returns(cl3);
            m_container.SetupGet(n => n.Atoms[3]).Returns(cl4);

            m_container.SetupGet(n => n.Atoms).Returns(new[] { c1, c2, cl3, cl4 });

            var m_stereoBond = new Mock<IBond>(); var stereoBond = m_stereoBond.Object;
            var m_left = new Mock<IBond>(); var left = m_left.Object;
            var m_right = new Mock<IBond>(); var right = m_right.Object;

            m_stereoBond.SetupGet(n => n.Begin).Returns(c1);
            m_stereoBond.SetupGet(n => n.End).Returns(c2);
            m_left.Setup(n => n.GetOther(c1)).Returns(cl3);
            m_right.Setup(n => n.GetOther(c2)).Returns(cl4);

            var m_dbs = new Mock<IDoubleBondStereochemistry>(); var dbs = m_dbs.Object;
            m_dbs.SetupGet(n => n.StereoBond).Returns(stereoBond);
            m_dbs.SetupGet(n => n.Bonds).Returns(new IBond[] { left, right });
            m_dbs.SetupGet(n => n.Stereo).Returns(DoubleBondConformation.Opposite);
            m_container.Setup(n => n.StereoElements).Returns(new[] { dbs });

            IStereoEncoder encoder = new DoubleBondElementEncoderFactory().Create(container,
                new int[][] { new[] { 1, 2 }, new[] { 0, 3 }, new[] { 0 }, new[] { 1 } });

            Assert.AreEqual(1, GetGeometricParity(encoder).Parity);
        }

        [TestMethod()]
        public void Together()
        {
            var m_container = new Mock<IAtomContainer>(); var container = m_container.Object;
            m_container.SetupGet(n => n.Atoms.Count).Returns(4);

            var m_c1 = new Mock<IAtom>(); var c1 = m_c1.Object;
            var m_c2 = new Mock<IAtom>(); var c2 = m_c2.Object;
            var m_cl3 = new Mock<IAtom>(); var cl3 = m_cl3.Object;
            var m_cl4 = new Mock<IAtom>(); var cl4 = m_cl4.Object;

            m_container.SetupGet(n => n.Atoms[0]).Returns(c1);
            m_container.SetupGet(n => n.Atoms[1]).Returns(c2);
            m_container.SetupGet(n => n.Atoms[2]).Returns(cl3);
            m_container.SetupGet(n => n.Atoms[3]).Returns(cl4);

            m_container.SetupGet(n => n.Atoms).Returns(new[] { c1, c2, cl3, cl4 });

            var m_stereoBond = new Mock<IBond>(); var stereoBond = m_stereoBond.Object;
            var m_left = new Mock<IBond>(); var left = m_left.Object;
            var m_right = new Mock<IBond>(); var right = m_right.Object;

            m_stereoBond.SetupGet(n => n.Begin).Returns(c1);
            m_stereoBond.SetupGet(n => n.End).Returns(c2);
            m_left.Setup(n => n.GetOther(c1)).Returns(cl3);
            m_right.Setup(n => n.GetOther(c2)).Returns(cl4);

            var m_dbs = new Mock<IDoubleBondStereochemistry>(); var dbs = m_dbs.Object;
            m_dbs.SetupGet(n => n.StereoBond).Returns(stereoBond);
            m_dbs.SetupGet(n => n.Bonds).Returns(new IBond[] { left, right });
            m_dbs.SetupGet(n => n.Stereo).Returns(DoubleBondConformation.Together);
            m_container.Setup(n => n.StereoElements).Returns(new[] { dbs });

            IStereoEncoder encoder = new DoubleBondElementEncoderFactory().Create(container,
                new int[][] { new[] { 1, 2 }, new[] { 0, 3 }, new[] { 0 }, new[] { 1 } });

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
                    Console.Error.WriteLine("No geometric field found.");
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
                    Console.Error.WriteLine("No encoders field found.");
                    return null;
                }
                return (IList<IStereoEncoder>)field.GetValue(encoder);
            }
            return new IStereoEncoder[0];
        }
    }
}
