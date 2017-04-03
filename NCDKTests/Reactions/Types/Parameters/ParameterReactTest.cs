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
    /// Tests for ParameterReact implementations.
    /// </summary>
    // @cdk.module test-reaction
    [TestClass()]
    public class ParameterReactTest : CDKTestCase
    {
        /// <summary>
        ///  Constructor for the ParameterReactTest object.
        /// </summary>
        public ParameterReactTest()
            : base()
        { }

        [TestMethod()]
        public void TestParameterReact()
        {
            IParameterReaction paramSet = new ParameterReaction();
            Assert.IsNotNull(paramSet);
        }

        [TestMethod()]
        public void TestSetParameter_boolean()
        {
            IParameterReaction paramSet = new ParameterReaction();

            paramSet.IsSetParameter = true;
            Assert.IsTrue(paramSet.IsSetParameter);

        }

        [TestMethod()]
        public void TestIsSetParameter()
        {
            IParameterReaction paramSet = new ParameterReaction();
            Assert.IsFalse(paramSet.IsSetParameter);
        }

        [TestMethod()]
        public void TestSetValue_object()
        {
            IParameterReaction paramSet = new ParameterReaction();
            paramSet.Value = null;
            Assert.IsNull(paramSet.Value);

        }

        [TestMethod()]
        public void TestGetValue()
        {
            IParameterReaction paramSet = new ParameterReaction();
            paramSet.Value = new object();
            Assert.IsNotNull(paramSet.Value);
        }
    }
}
