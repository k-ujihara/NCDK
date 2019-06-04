/* Copyright (C) 2008  Miguel Rojas <miguelrojasch@yahoo.es>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Reactions.Types;
using NCDK.Reactions.Types.Parameters;

namespace NCDK.Reactions
{
    /// <summary>
    /// Tests for ReactionEngine implementations.
    /// </summary>
    // @cdk.module test-reaction
    [TestClass()]
    public class ReactionEngineTest : CDKTestCase
    {
        public ReactionEngineTest()
            : base()
        { }

        [TestMethod()]
        public void TestReactionEngine()
        {
            var engine = new AdductionProtonLPReaction();
            Assert.IsNotNull(engine);
        }

        [TestMethod()]
        public void TestGetParameterList()
        {
            var engine = new AdductionProtonLPReaction();
            Assert.IsNotNull(engine.ParameterList);
        }

        [TestMethod()]
        public void TestSetParameterList_List()
        {
            var engine = new AdductionProtonLPReaction();
            engine.ParameterList = engine.ParameterList;
            Assert.IsNotNull(engine.ParameterList);
        }

        [TestMethod()]
        public void TestGetParameterClass_Class()
        {
            var engine = new AdductionProtonLPReaction();
            Assert.IsNotNull(engine.GetParameterClass(typeof(SetReactionCenter)));
        }
    }
}
