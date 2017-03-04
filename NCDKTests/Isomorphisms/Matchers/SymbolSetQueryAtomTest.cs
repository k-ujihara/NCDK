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
 *
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;

namespace NCDK.Isomorphisms.Matchers
{
    /// <summary>
    /// Checks the functionality of the IsomorphismTester
    ///
    // @cdk.module test-isomorphism
    /// </summary>
    [TestClass()]
    public class SymbolSetQueryAtomTest : CDKTestCase
    {
        private static SymbolSetQueryAtom symbolSet = null;

        static SymbolSetQueryAtomTest()
        {
            symbolSet = new SymbolSetQueryAtom(Default.ChemObjectBuilder.Instance);
            symbolSet.AddSymbol("C");
            symbolSet.AddSymbol("Fe");
        }

        [TestMethod()]
        public void TestMatches()
        {
            Atom c = new Atom("C");
            Atom n = new Atom("N");
            Assert.IsTrue(symbolSet.Matches(c));
            Assert.IsFalse(symbolSet.Matches(n));
        }

        [TestMethod()]
        public void TestRemoveSymbol()
        {
            symbolSet.RemoveSymbol("Fe");
            Assert.AreEqual(1, symbolSet.GetSymbolSet().Count);
            Assert.IsFalse(symbolSet.HasSymbol("Fe"));
            Assert.IsTrue(symbolSet.HasSymbol("C"));
            symbolSet.AddSymbol("Fe");
        }

        [TestMethod()]
        public void TestHasSymbol()
        {
            Assert.IsTrue(symbolSet.HasSymbol("C"));
            Assert.IsFalse(symbolSet.HasSymbol("N"));
        }
    }
}
