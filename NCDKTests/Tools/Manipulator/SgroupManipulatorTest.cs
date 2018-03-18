/*
 * Copyright (c) 2018 John Mayfield
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
using NCDK.Common.Base;
using NCDK.Sgroups;
using System.Collections.Generic;

namespace NCDK.Tools.Manipulator
{
    [TestClass()]
    public class SgroupManipulatorTest
    {
        [TestMethod()]
        public void CopyNull()
        {
            Assert.IsNull(SgroupManipulator.Copy(null, null));
        }

        [TestMethod()]
        public void CopySgroups()
        {
            List<Sgroup> sgroups = new List<Sgroup>();
            IAtom a1 = new Mock<IAtom>().Object;
            IAtom a2 = new Mock<IAtom>().Object;
            IBond b1 = new Mock<IBond>().Object;
            IBond b2 = new Mock<IBond>().Object;
            Sgroup sgroup = new Sgroup
            {
                Type = SgroupType.CtabStructureRepeatUnit,
                Subscript = "n"
            };
            sgroup.Atoms.Add(a1);
            sgroup.Atoms.Add(a2);
            sgroup.Bonds.Add(b1);
            sgroup.Bonds.Add(b2);
            sgroups.Add(sgroup);
            var copied = SgroupManipulator.Copy(sgroups, null);
            Sgroup copiedSgroup = copied[0];
            Assert.AreNotSame(sgroup, copiedSgroup);
            Assert.AreEqual(sgroup.Type, copiedSgroup.Type);
            Assert.AreEqual(sgroup.Subscript, copiedSgroup.Subscript);
            Assert.IsTrue(Compares.AreDeepEqual(sgroup.Atoms, copiedSgroup.Atoms));
            Assert.IsTrue(Compares.AreDeepEqual(sgroup.Bonds, copiedSgroup.Bonds));
        }

        [TestMethod()]
        public void CopySgroups2()
        {
            var sgroups = new List<Sgroup>();
            var replace = new CDKObjectMap();

            IAtom a1 = new Mock<IAtom>().Object;
            IAtom a2 = new Mock<IAtom>().Object;
            IBond b1 = new Mock<IBond>().Object;
            IBond b2 = new Mock<IBond>().Object;

            IAtom a1copy = new Mock<IAtom>().Object;
            IAtom a2copy = new Mock<IAtom>().Object;
            IBond b1copy = new Mock<IBond>().Object;
            IBond b2copy = new Mock<IBond>().Object;

            replace.Add(a1, a1copy);
            replace.Add(a2, a2copy);
            replace.Add(b1, b1copy);
            replace.Add(b2, b2copy);

            Sgroup sgroup = new Sgroup
            {
                Type = SgroupType.CtabStructureRepeatUnit,
                Subscript = "n"
            };
            sgroup.Atoms.Add(a1);
            sgroup.Atoms.Add(a2);
            sgroup.Bonds.Add(b1);
            sgroup.Bonds.Add(b2);
            sgroups.Add(sgroup);
            var copied = SgroupManipulator.Copy(sgroups, replace);
            Sgroup copiedSgroup = copied[0];
            Assert.AreNotSame(sgroup, copiedSgroup);
            Assert.AreEqual(sgroup.Type, copiedSgroup.Type);
            Assert.AreEqual(sgroup.Subscript, copiedSgroup.Subscript);
            Assert.IsFalse(Compares.AreDeepEqual(sgroup.Atoms, copiedSgroup.Atoms));
            Assert.IsFalse(Compares.AreDeepEqual(sgroup.Bonds, copiedSgroup.Bonds));
            Assert.IsTrue(copiedSgroup.Atoms.Contains(a1copy));
            Assert.IsTrue(copiedSgroup.Atoms.Contains(a2copy));
            Assert.IsTrue(copiedSgroup.Bonds.Contains(b1copy));
            Assert.IsTrue(copiedSgroup.Bonds.Contains(b2copy));
        }
    }
}
