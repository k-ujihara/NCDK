/* Copyright (C) 2009-2010 Syed Asad Rahman <asad@ebi.ac.uk>
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
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Silent;
using NCDK.Smiles;
using NCDK.SMSD.Algorithms.VFLib;
using NCDK.SMSD.Tools;

namespace NCDK.SMSD
{
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    // @cdk.module test-smsd
    [TestClass()]
    public abstract class AbstractSubGraphTest
    {
        protected AbstractSubGraphTest algorithm { get; }

        public AbstractSubGraphTest() { }

        /// <summary>
        /// Test of isSubgraph method, of class AbstractSubGraph.
         /// </summary>
        [TestMethod()]
        public virtual void TestIsSubgraph()
        {
            SmilesParser sp = new SmilesParser(ChemObjectBuilder.Instance);
            IAtomContainer target = sp.ParseSmiles("C\\C=C/Nc1cccc(c1)N(O)\\C=C\\C\\C=C\\C=C/C");
            IAtomContainer queryac = sp.ParseSmiles("Nc1ccccc1");

            VFlibSubStructureHandler smsd1 = new VFlibSubStructureHandler();
            MolHandler mol1 = new MolHandler(queryac, true, true);
            MolHandler mol2 = new MolHandler(target, true, true);
            smsd1.Set(mol1, mol2);
            Assert.AreEqual(true, smsd1.IsSubgraph(true));
        }

        public class ISubGraphImpl : AbstractSubGraph
        {
            public override bool IsSubgraph(bool bondMatch)
            {
                return false;
            }
        }
    }
}
