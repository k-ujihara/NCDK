/* Copyright (C) 1997-2010  The Chemistry Development Kit (CDK) project
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
using NCDK.Default;
using System.Xml.Linq;

namespace NCDK.Normalize
{
    /**
     * @cdk.module test-smiles
     */
    [TestClass()]
    public class NormalizerTest : CDKTestCase
    {
        [TestMethod()]
        public void TestNormalize()
        {
            IAtomContainer ac = new AtomContainer();
            ac.Atoms.Add(new Atom("C"));
            ac.Atoms.Add(new Atom("N"));
            ac.Atoms.Add(new Atom("O"));
            ac.Atoms.Add(new Atom("O"));
            ac.Bonds.Add(new Bond(ac.Atoms[0], ac.Atoms[1]));
            ac.Bonds.Add(new Bond(ac.Atoms[1], ac.Atoms[2], BondOrder.Double));
            ac.Bonds.Add(new Bond(ac.Atoms[1], ac.Atoms[3], BondOrder.Double));
            var doc = new XDocument();
			var set = new XElement("replace-set");
            doc.Add(set);
            var replace = new XElement("replace");
            set.Add(replace);
            replace.Add(new XText("O=N=O"));
			var replacement = new XElement("replacement");
            set.Add(replacement);
            replacement.Add(new XText("[O-][N+]=O"));
            Normalizer.Normalize(ac, doc);
            Assert.IsTrue(ac.Bonds[1].Order == BondOrder.Single
                    ^ ac.Bonds[2].Order == BondOrder.Single);
        }
    }
}
