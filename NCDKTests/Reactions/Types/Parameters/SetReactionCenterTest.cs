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
using NCDK.Reactions.Types.Parameters;

namespace NCDK.Types.Parameters
{
    /// <summary>
    /// Tests for SetReactionCenter implementations.
    /// </summary>
    // @cdk.module test-reaction
    [TestClass()]
    public class SetReactionCenterTest : CDKTestCase
    {
        public SetReactionCenterTest()
            : base()
        { }

        [TestMethod()]
        public void TestSetReactionCenter()
        {
            SetReactionCenter react = new SetReactionCenter();
            Assert.IsNotNull(react);
        }
    }
}
