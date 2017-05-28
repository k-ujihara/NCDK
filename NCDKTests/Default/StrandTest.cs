/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
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

namespace NCDK.Default
{
    /// <summary>
    /// TODO To change the template for this generated type comment go to
    /// Window - Preferences - Java - Code Style - Code Templates
    /// </summary>
    // @author     Martin Eklund <martin.eklund@farmbio.uu.se>
    // @cdk.module test-data
    [TestClass()]
    public class StrandTest : AbstractStrandTest
    {
        public override IChemObject NewChemObject()
        {
            return new Strand();
        }

        [TestMethod()]
        public virtual void TestStrand()
        {
            IStrand oStrand = new Strand();
            Assert.IsNotNull(oStrand);
            Assert.AreEqual(oStrand.GetMonomerMap().Count(), 0);

            IMonomer oMono1 = oStrand.Builder.CreateMonomer();
            oMono1.MonomerName = "TRP279";
            IMonomer oMono2 = oStrand.Builder.CreateMonomer();
            oMono2.MonomerName = "HOH";
            IMonomer oMono3 = oStrand.Builder.CreateMonomer();
            oMono3.MonomerName = "GLYA16";
            IAtom oAtom1 = oStrand.Builder.CreateAtom("C");
            IAtom oAtom2 = oStrand.Builder.CreateAtom("C");
            IAtom oAtom3 = oStrand.Builder.CreateAtom("C");
            IAtom oAtom4 = oStrand.Builder.CreateAtom("C");
            IAtom oAtom5 = oStrand.Builder.CreateAtom("C");

            oStrand.AddAtom(oAtom1);
            oStrand.AddAtom(oAtom2);
            oStrand.AddAtom(oAtom3, oMono1);
            oStrand.AddAtom(oAtom4, oMono2);
            oStrand.AddAtom(oAtom5, oMono3);
            Assert.IsNotNull(oStrand.Atoms[0]);
            Assert.IsNotNull(oStrand.Atoms[1]);
            Assert.IsNotNull(oStrand.Atoms[2]);
            Assert.IsNotNull(oStrand.Atoms[3]);
            Assert.IsNotNull(oStrand.Atoms[4]);
            Assert.AreEqual(oAtom1, oStrand.Atoms[0]);
            Assert.AreEqual(oAtom2, oStrand.Atoms[1]);
            Assert.AreEqual(oAtom3, oStrand.Atoms[2]);
            Assert.AreEqual(oAtom4, oStrand.Atoms[3]);
            Assert.AreEqual(oAtom5, oStrand.Atoms[4]);

            Assert.IsNull(oStrand.GetMonomer("0815"));
            Assert.IsNotNull(oStrand.GetMonomer(""));
            Assert.IsNotNull(oStrand.GetMonomer("TRP279"));
            Assert.AreEqual(oMono1, oStrand.GetMonomer("TRP279"));
            Assert.AreEqual(oStrand.GetMonomer("TRP279").Atoms.Count, 1);
            Assert.IsNotNull(oStrand.GetMonomer("HOH"));
            Assert.AreEqual(oMono2, oStrand.GetMonomer("HOH"));
            Assert.AreEqual(oStrand.GetMonomer("HOH").Atoms.Count, 1);
            Assert.AreEqual(oStrand.GetMonomer("").Atoms.Count, 2);
            Assert.AreEqual(oStrand.Atoms.Count, 5);
            Assert.AreEqual(oStrand.GetMonomerMap().Count(), 3);
        }
    }
}