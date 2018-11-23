/* Copyright (C) 2005-2007  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
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
using System.Linq;

namespace NCDK.Tools
{
    // @cdk.module test-pdb
    [TestClass()]
    public class ProteinBuilderToolTest : CDKTestCase
    {
        [TestMethod()]
        public void TestCreateProtein()
        {
            IBioPolymer protein = ProteinBuilderTool.CreateProtein("GAGA", CDK.Builder);
            Assert.IsNotNull(protein);
            Assert.AreEqual(4, protein.GetMonomerMap().Count());
            Assert.AreEqual(1, protein.GetStrandMap().Count());
            Assert.AreEqual(18 + 1, protein.Atoms.Count);
            // 1=terminal oxygen
            Assert.AreEqual(14 + 3 + 1, protein.Bonds.Count);
            // 3 = extra back bone bonds, 1=bond to terminal oxygen
        }
    }
}
