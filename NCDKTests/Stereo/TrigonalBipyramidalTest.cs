/*
 * Copyright (c) 2017 John Mayfield <jwmay@users.sf.net>
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
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NCDK.Common.Base;
using NCDK.Smiles;
using System;
using System.Linq;

namespace NCDK.Stereo
{
    [TestClass()]
    public class TrigonalBipyramidalTest
    {
        [TestMethod()]
        public void Normalize()
        {
            SmilesParser smipar = new SmilesParser(Silent.ChemObjectBuilder.Instance);
            var mol = smipar.ParseSmiles("C[As@TB3](Cl)(Cl)(C)Cl");
            var se = mol.StereoElements.First();
            Assert.IsInstanceOfType(se, typeof(TrigonalBipyramidal));
            Assert.AreEqual(3, se.Configure.Order());
            TrigonalBipyramidal tb = (TrigonalBipyramidal)se;
            TrigonalBipyramidal tbNorm = tb.Normalize();
            Assert.IsTrue(Compares.AreDeepEqual(
                new[]
                {
                        mol.Atoms[0],
                        mol.Atoms[2],
                        mol.Atoms[3],
                        mol.Atoms[5],
                        mol.Atoms[4],
                },
                tbNorm.Carriers));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TooManyCarriers()
        {
            IAtom a0 = new Mock<IAtom>().Object;
            IAtom a1 = new Mock<IAtom>().Object;
            IAtom a2 = new Mock<IAtom>().Object;
            IAtom a3 = new Mock<IAtom>().Object;
            IAtom a4 = new Mock<IAtom>().Object;
            IAtom a5 = new Mock<IAtom>().Object;
            IAtom a6 = new Mock<IAtom>().Object;
            var dummy = new TrigonalBipyramidal(a0, new IAtom[] { a1, a2, a3, a4, a5, a6 }, 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), AllowDerivedTypes = true)]
        public void BadConfigurationOrder()
        {
            IAtom a0 = new Mock<IAtom>().Object;
            IAtom a1 = new Mock<IAtom>().Object;
            IAtom a2 = new Mock<IAtom>().Object;
            IAtom a3 = new Mock<IAtom>().Object;
            IAtom a4 = new Mock<IAtom>().Object;
            IAtom a5 = new Mock<IAtom>().Object;
            var dummy = new TrigonalBipyramidal(a0, new IAtom[] { a1, a2, a3, a4, a5 }, 32);
        }
    }
}
