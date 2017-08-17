﻿/* Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
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
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Default
{
    /// <summary>
    /// TestCase for the Reaction class.
    /// </summary>
    // @cdk.module test-data
    [TestClass()]
    public class ReactionTest : AbstractReactionTest
    {
        public override IChemObject NewChemObject()
        {
            return new Reaction();
        }

        [TestMethod()]
        public void TestReaction()
        {
            IReaction reaction = new Reaction();
            Assert.IsNotNull(reaction);
            Assert.AreEqual(0, reaction.Reactants.Count);
            Assert.AreEqual(0, reaction.Products.Count);
            Assert.AreEqual(ReactionDirections.Forward, reaction.Direction);
        }
    }
}
