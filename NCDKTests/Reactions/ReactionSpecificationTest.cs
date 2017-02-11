/* Copyright (C) 2008  Miguel Rojas <miguelrojasch@users.sf.net>
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

namespace NCDK.Reactions
{
    /**
     * @cdk.module test-reaction
     */
    [TestClass()]
    public class ReactionSpecificationTest : CDKTestCase
    {
        public ReactionSpecificationTest()
            : base()
        { }

        private const string REAC_REF = "bla";
        private const string REAC_IMPL_TITLE = "bla2";
        private const string REAC_IMPL_VENDOR = "bla3";
        private const string REAC_IMPL_ID = "bla4";

        [TestMethod()]
        public void TestReactionSpecification_String_String_String_String()
        {
            ReactionSpecification spec = new ReactionSpecification(REAC_REF, REAC_IMPL_TITLE, REAC_IMPL_ID,
                    REAC_IMPL_VENDOR);
            Assert.IsNotNull(spec);
        }

        [TestMethod()]
        public void TestImplementationVendor()
        {
            ReactionSpecification spec = new ReactionSpecification(REAC_REF, REAC_IMPL_TITLE, REAC_IMPL_ID,
                    REAC_IMPL_VENDOR);
            Assert.AreEqual(REAC_IMPL_VENDOR, spec.ImplementationVendor);
        }

        [TestMethod()]
        public void TestSpecificationReference()
        {
            ReactionSpecification spec = new ReactionSpecification(REAC_REF, REAC_IMPL_TITLE, REAC_IMPL_ID,
                    REAC_IMPL_VENDOR);
            Assert.AreEqual(REAC_REF, spec.SpecificationReference);
        }

        [TestMethod()]
        public void TestImplementationIdentifier()
        {
            ReactionSpecification spec = new ReactionSpecification(REAC_REF, REAC_IMPL_TITLE, REAC_IMPL_ID,
                    REAC_IMPL_VENDOR);
            Assert.AreEqual(REAC_IMPL_ID, spec.ImplementationIdentifier);
        }

        [TestMethod()]
        public void TestImplementationTitle()
        {
            ReactionSpecification spec = new ReactionSpecification(REAC_REF, REAC_IMPL_TITLE, REAC_IMPL_ID,
                    REAC_IMPL_VENDOR);
            Assert.AreEqual(REAC_IMPL_TITLE, spec.ImplementationTitle);
        }
    }
}
