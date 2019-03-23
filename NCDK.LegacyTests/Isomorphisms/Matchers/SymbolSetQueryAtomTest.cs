/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
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

namespace NCDK.Isomorphisms.Matchers
{
    /// <summary>
    /// Checks the functionality of the IsomorphismTester
    /// </summary>
    // @cdk.module test-isomorphism
    [TestClass()]
    public class SymbolSetQueryAtomTest : CDKTestCase
    {
        private static SymbolSetQueryAtom symbolSet = null;

        static SymbolSetQueryAtomTest()
        {
            symbolSet = new SymbolSetQueryAtom();
            symbolSet.Symbols.Add("C");
            symbolSet.Symbols.Add("Fe");
        }

        [TestMethod()]
        public void TestMatches()
        {
            var builder = Silent.ChemObjectBuilder.Instance;
            var c = builder.NewAtom("C");
            var n = builder.NewAtom("N");
            Assert.IsTrue(symbolSet.Matches(c));
            Assert.IsFalse(symbolSet.Matches(n));
        }

        [TestMethod()]
        public void TestRemoveSymbol()
        {
            symbolSet.Symbols.Remove("Fe");
            Assert.AreEqual(1, symbolSet.Symbols.Count);
            Assert.IsFalse(symbolSet.Symbols.Contains("Fe"));
            Assert.IsTrue(symbolSet.Symbols.Contains("C"));
            symbolSet.Symbols.Add("Fe");
        }

        [TestMethod()]
        public void TestHasSymbol()
        {
            Assert.IsTrue(symbolSet.Symbols.Contains("C"));
            Assert.IsFalse(symbolSet.Symbols.Contains("N"));
        }
    }
}
