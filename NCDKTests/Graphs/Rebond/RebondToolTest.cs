/* Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
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
 *
 */
using NCDK.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Config;
using NCDK.Silent;

namespace NCDK.Graphs.Rebond
{
    /// <summary>
    /// Checks the functionality of the RebondTool.
    /// </summary>
    // @cdk.module test-standard
    [TestClass()]
    public class RebondToolTest : CDKTestCase
    {
        public RebondToolTest()
                : base()
        { }

        [TestMethod()]
        public void TestRebondTool_Double_double_double()
        {
            RebondTool rebonder = new RebondTool(2.0, 0.5, 0.5);
            Assert.IsNotNull(rebonder);
        }

        [TestMethod()]
        public void TestRebond_IAtomContainer()
        {
            RebondTool rebonder = new RebondTool(2.0, 0.5, 0.5);
            IAtomContainer methane = new AtomContainer();
            methane.Atoms.Add(new Atom("C", new Vector3(0.0, 0.0, 0.0)));
            methane.Atoms.Add(new Atom("H", new Vector3(0.6, 0.6, 0.6)));
            methane.Atoms.Add(new Atom("H", new Vector3(-0.6, -0.6, 0.6)));
            methane.Atoms.Add(new Atom("H", new Vector3(0.6, -0.6, -0.6)));
            methane.Atoms.Add(new Atom("H", new Vector3(-0.6, 0.6, -0.6)));

            // configure atoms
            AtomTypeFactory factory = AtomTypeFactory.GetInstance("NCDK.Config.Data.jmol_atomtypes.txt",
                    methane.Builder);
            //IAtom[] atoms = methane.GetAtoms();
            for (int i = 0; i < methane.Atoms.Count; i++)
            {
                factory.Configure(methane.Atoms[i]);
            }
            // rebond
            rebonder.Rebond(methane);

            Assert.AreEqual(5, methane.Atoms.Count);
            Assert.AreEqual(4, methane.Bonds.Count);
        }
    }
}
