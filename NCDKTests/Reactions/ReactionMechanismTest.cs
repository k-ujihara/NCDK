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
using System;

namespace NCDK.Reactions
{
    /// <summary>
    /// Tests for IReactionProcess implementations.
    /// </summary>
    // @cdk.module test-reaction
    [TestClass()]
    public abstract class ReactionMechanismTest : CDKTestCase
    {
        protected IReactionMechanism reactionMechanism;

        /// <summary>
        /// Defining reaction mechanism.
        ///
        /// <param name="descriptorClass">/// @throws Exception</param>
        /// </summary>
        public void SetMechanism(Type descriptorClass)
        {
            if (reactionMechanism == null)
            {
                var descriptor = descriptorClass.GetConstructor(Type.EmptyTypes).Invoke(Array.Empty<object>());
                if (!(descriptor is IReactionMechanism))
                {
                    throw new CDKException("The passed reaction class must be a IReactionMechanism");
                }
                reactionMechanism = (IReactionMechanism)descriptor;
            }
        }

        /// <summary>
        /// Makes sure that the extending class has set the super.descriptor.
        /// </summary>
        /// <example>
        /// Each extending class should have this bit of code:
        /// <code>
        /// public void SetUp() {
        ///   // Pass a Class, not an Object!
        ///   SetDescriptor(typeof(SomeDescriptor));
        /// }
        /// </code>
        /// The unit tests in the extending class may use this instance, but
        /// are not required.
        /// </example>
        [TestMethod()]
        public void TestHasSetSuperDotDescriptor()
        {
            Assert.IsNotNull(reactionMechanism, "The extending class must set the super.descriptor in its SetUp() method.");
        }
    }
}
