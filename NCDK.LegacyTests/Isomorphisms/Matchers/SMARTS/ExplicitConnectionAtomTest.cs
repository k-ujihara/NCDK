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

namespace NCDK.Isomorphisms.Matchers.SMARTS
{
    // @author John May
    // @cdk.module test-smarts
    [TestClass()]
    public class ExplicitConnectionAtomTest
    {
        [TestMethod()]
        public void Matches()
        {
            ExplicitConnectionAtom matcher = new ExplicitConnectionAtom(2);
            var mock_atom = new Mock<IAtom>();
            IAtom atom = mock_atom.Object;
            mock_atom.Setup(n => n.GetProperty<SMARTSAtomInvariants>(SMARTSAtomInvariants.Key)).Returns(
                new SMARTSAtomInvariants(new Mock<IAtomContainer>().Object, 0, 0,
                Array.Empty<int>(), 0, 0, // <- degree not used due to old CDK bug
                        2, 0));
            Assert.IsTrue(matcher.Matches(atom));
        }
    }
}
