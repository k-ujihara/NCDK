/*
 * Copyright (c) 2013  European Bioinformatics Institute (EMBL-EBI)
 *                     John May <jwmay@users.sf.net>
 *               2014  Mark B Vine (orcid:0000-0002-7794-0426)
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using System.IO;

namespace NCDK.IO
{
    // @author John May
    // @cdk.module test-io
    [TestClass()]
    public class MDLV2000PropertiesBlockTest
    {
        private readonly MDLV2000Reader reader = new MDLV2000Reader();
        private readonly IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;

        [TestMethod()]
        public void M_end()
        {
            Assert.AreEqual(MDLV2000Reader.PropertyKey.M_END, MDLV2000Reader.PropertyKey.Of("M  END"));
        }

        [TestMethod()]
        public void M_end_padding()
        {
            Assert.AreEqual(MDLV2000Reader.PropertyKey.M_END, MDLV2000Reader.PropertyKey.Of("M  END  "));
        }    

        [TestMethod()]
        public void M_chg_padding()
        {
            Assert.AreEqual(MDLV2000Reader.PropertyKey.M_CHG, MDLV2000Reader.PropertyKey.Of("M  CHG  "));
        }

        [TestMethod()]
        public void M_Iso_padding()
        {
            Assert.AreEqual(MDLV2000Reader.PropertyKey.M_ISO, MDLV2000Reader.PropertyKey.Of("M  ISO  "));
        }

        [TestMethod()]
        public void Atom_alias()
        {
            Assert.AreEqual(MDLV2000Reader.PropertyKey.ATOM_ALIAS, MDLV2000Reader.PropertyKey.Of("A    1"));
        }

        [TestMethod()]
        public void Atom_value()
        {
            Assert.AreEqual(MDLV2000Reader.PropertyKey.ATOM_VALUE, MDLV2000Reader.PropertyKey.Of("V    1"));
        }

        [TestMethod()]
        public void Group_abrv()
        {
            Assert.AreEqual(MDLV2000Reader.PropertyKey.GROUP_ABBREVIATION, MDLV2000Reader.PropertyKey.Of("G    1"));
        }

        [TestMethod()]
        public void Skip()
        {
            Assert.AreEqual(MDLV2000Reader.PropertyKey.SKIP, MDLV2000Reader.PropertyKey.Of("S  SKP  5"));
        }

        /// <summary>ACDLabs ChemSketch atom labels</summary>
        [TestMethod()]
        public void M_zzc_padding()
        {
            Assert.AreEqual(MDLV2000Reader.PropertyKey.M_ZZC, MDLV2000Reader.PropertyKey.Of("M  ZZC  "));
        }

        [TestMethod()]
        public void Anion()
        {
            IAtomContainer mock = Mock(3);
            Read("M  CHG  1   1  -1", mock);
            Moq.Mock.Get(mock.Atoms[0]).VerifySet(n => n.FormalCharge = -1);
        }

        [TestMethod()]
        public void Cation()
        {
            IAtomContainer mock = Mock(3);
            Read("M  CHG  1   1   1", mock);
            Moq.Mock.Get(mock.Atoms[0]).VerifySet(n => n.FormalCharge = +1);
        }

        [TestMethod()]
        public void MultipleCharges()
        {
            IAtomContainer mock = Mock(6);
            Read("M  CHG  2   2   1   5  -2", mock);
            Moq.Mock.Get(mock.Atoms[1]).VerifySet(n => n.FormalCharge = +1);
            Moq.Mock.Get(mock.Atoms[4]).VerifySet(n => n.FormalCharge = -2);
        }

        [TestMethod()]
        public void MultipleChargesTruncated()
        {
            IAtomContainer mock = Mock(6);
            Read("M  CHG  2   2  -3", mock);
            Moq.Mock.Get(mock.Atoms[1]).VerifySet(n => n.FormalCharge = -3);
        }

        [TestMethod()]
        public void C13()
        {
            IAtomContainer mock = Mock(3);
            Read("M  ISO  1   1  13", mock);
            Moq.Mock.Get(mock.Atoms[0]).VerifySet(n => n.MassNumber = 13);
        }

        [TestMethod()]
        public void C13N14()
        {
            IAtomContainer mock = Mock(4);
            Read("M  ISO  2   1  13   3  14", mock);
            Moq.Mock.Get(mock.Atoms[0]).VerifySet(n => n.MassNumber = 13);
            Moq.Mock.Get(mock.Atoms[2]).VerifySet(n => n.MassNumber = 14);
        }

        [TestMethod()]
        public void AtomValue()
        {
            IAtomContainer mock = Mock(3);
            Read("V    1 A Comment", mock);
            Moq.Mock.Get(mock.Atoms[0]).Verify(n => n.SetProperty(CDKPropertyName.Comment, "A Comment"));
        }

        [TestMethod()]
        public void AtomAlias()
        {
            IAtomContainer mock = Mock(4);
            Read("A    4\n" + "Gly", mock);
            Assert.IsInstanceOfType(mock.Atoms[3], typeof(IPseudoAtom));
            Assert.AreEqual("Gly", ((IPseudoAtom)mock.Atoms[3]).Label);
        }

        [TestMethod()]
        public void AcdAtomLabel()
        {
            IAtomContainer mock = Mock(3);
            Read("M  ZZC   1 6", mock);
            Moq.Mock.Get(mock.Atoms[0]).Verify(n => n.SetProperty(CDKPropertyName.ACDLabsAtomLabel, "6"));
        }

        static IAtomContainer Mock(int n)
        {
            IAtomContainer mock = new AtomContainer();
            for (int i = 0; i < n; i++)
                mock.Atoms.Add(new Moq.Mock<IAtom>().Object);
            return mock;
        }

        void Read(string input, IAtomContainer container)
        {
            reader.ReadPropertiesFast(new StringReader(input), container, container.Atoms.Count);
        }
    }
}
