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

namespace NCDK
{
    [TestClass()]
    public class BondRefTest
    {
        [TestMethod()]
        public void DereferenceNullPointer()
        {
            Assert.IsNull(BondRef.Deref(null));
        }

        [TestMethod()]
        public void DereferenceNonPointer()
        {
            IBond mock = new Mock<IBond>().Object;
            Assert.AreSame(mock, BondRef.Deref(mock));
        }

        [TestMethod()]
        public void DereferencePointer()
        {
            IBond mock = new Mock<IBond>().Object;
            IBond ptr = new BondRef(mock);
            Assert.AreSame(mock, BondRef.Deref(ptr));
        }

        [TestMethod()]
        public void DereferencePointerPointer()
        {
            IBond mock = new Mock<IBond>().Object;
            IBond ptr = new BondRef(new BondRef(mock));
            Assert.AreSame(mock, BondRef.Deref(ptr));
        }
    }
}
