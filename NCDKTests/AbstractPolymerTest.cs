/* Copyright (C) 2001-2007  The Chemistry Development Kit (CDK) project
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
 *  */
using NCDK.Common.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace NCDK
{
    /// <summary>
    /// TestCase for {@link IPolymer} implementations.
    ///
    // @author      Edgar Luttmann <edgar@uni-paderborn.de>
    // @author      Martin Eklund <martin.eklund@farmbio.uu.se>
    // @cdk.created 2001-08-09
    // @cdk.module  test-interfaces
    /// </summary>
    [TestClass()]
    public abstract class AbstractPolymerTest : AbstractMoleculeTest
    {
        [TestMethod()]
        public override void TestAddAtom_IAtom()
        {
            IPolymer oPolymer = (IPolymer)NewChemObject();

            IAtom oAtom1 = oPolymer.Builder.CreateAtom("C1");
            IAtom oAtom2 = oPolymer.Builder.CreateAtom("C2");
            oPolymer.Atoms.Add(oAtom1);
            oPolymer.Atoms.Add(oAtom2);

            Assert.AreEqual(2, oPolymer.Atoms.Count);
            Assert.AreEqual(0, oPolymer.GetMonomerMap().Count());
        }

        [TestMethod()]
        public virtual void TestAddAtom_IAtom_IMonomer()
        {
            IPolymer oPolymer = (IPolymer)NewChemObject();
            IMonomer oMono1 = oPolymer.Builder.CreateMonomer();
            oMono1.MonomerName = "TRP279";
            IMonomer oMono2 = null;
            IAtom oAtom1 = oPolymer.Builder.CreateAtom("C1");
            IAtom oAtom2 = oPolymer.Builder.CreateAtom("C2");
            IAtom oAtom3 = oPolymer.Builder.CreateAtom("C3");

            oPolymer.Atoms.Add(oAtom1);
            oPolymer.AddAtom(oAtom2, oMono1);
            oPolymer.AddAtom(oAtom3, oMono2);
            Assert.IsNotNull(oPolymer.Atoms[0]);
            Assert.IsNotNull(oPolymer.Atoms[1]);
            Assert.IsNotNull(oPolymer.Atoms[2]);
            Assert.AreEqual(oAtom1, oPolymer.Atoms[0]);
            Assert.AreEqual(oAtom2, oPolymer.Atoms[1]);
            Assert.AreEqual(oAtom3, oPolymer.Atoms[2]);
            Assert.AreEqual(3, oPolymer.Atoms.Count);
            Assert.AreEqual(1, oPolymer.GetMonomer("TRP279").Atoms.Count);
            Assert.AreEqual(1, oPolymer.GetMonomerMap().Count());

            Assert.IsNotNull(oPolymer.GetMonomer("TRP279"));
            Assert.AreEqual(oMono1, oPolymer.GetMonomer("TRP279"));
        }

        [TestMethod()]
        public virtual void TestGetMonomerCount()
        {
            IPolymer oPolymer = (IPolymer)NewChemObject();
            Assert.AreEqual(0, oPolymer.GetMonomerMap().Count());

            IMonomer oMono1 = oPolymer.Builder.CreateMonomer();
            oMono1.MonomerName = "TRP279";
            IMonomer oMono2 = oPolymer.Builder.CreateMonomer();
            oMono2.MonomerName = "HOH";
            IAtom oAtom1 = oPolymer.Builder.CreateAtom("C1");
            IAtom oAtom2 = oPolymer.Builder.CreateAtom("C2");
            IAtom oAtom3 = oPolymer.Builder.CreateAtom("C3");
            oPolymer.Atoms.Add(oAtom1);
            oPolymer.AddAtom(oAtom2, oMono1);
            oPolymer.AddAtom(oAtom3, oMono2);

            Assert.AreEqual(3, oPolymer.Atoms.Count);
            Assert.AreEqual(2, oPolymer.GetMonomerMap().Count());
        }

        [TestMethod()]
        public virtual void TestGetMonomer_String()
        {
            IPolymer oPolymer = (IPolymer)NewChemObject();

            IMonomer oMono1 = oPolymer.Builder.CreateMonomer();
            oMono1.MonomerName = "TRP279";
            IMonomer oMono2 = oPolymer.Builder.CreateMonomer();
            oMono2.MonomerName = "HOH";
            IAtom oAtom1 = oPolymer.Builder.CreateAtom("C1");
            IAtom oAtom2 = oPolymer.Builder.CreateAtom("C2");
            IAtom oAtom3 = oPolymer.Builder.CreateAtom("C3");
            oPolymer.AddAtom(oAtom1, oMono1);
            oPolymer.AddAtom(oAtom2, oMono1);
            oPolymer.AddAtom(oAtom3, oMono2);

            Assert.AreEqual(oMono1, oPolymer.GetMonomer("TRP279"));
            Assert.AreEqual(oMono2, oPolymer.GetMonomer("HOH"));
            Assert.IsNull(oPolymer.GetMonomer("Mek"));
        }

        [TestMethod()]
        public virtual void TestGetMonomerNames()
        {
            IPolymer oPolymer = (IPolymer)NewChemObject();
            Assert.AreEqual(0, oPolymer.GetMonomerNames().Count());

            IMonomer oMono1 = oPolymer.Builder.CreateMonomer();
            oMono1.MonomerName = "TRP279";
            IMonomer oMono2 = oPolymer.Builder.CreateMonomer();
            oMono2.MonomerName = "HOH";
            IAtom oAtom1 = oPolymer.Builder.CreateAtom("C1");
            IAtom oAtom2 = oPolymer.Builder.CreateAtom("C2");
            IAtom oAtom3 = oPolymer.Builder.CreateAtom("C3");
            oPolymer.Atoms.Add(oAtom1);
            oPolymer.AddAtom(oAtom2, oMono1);
            oPolymer.AddAtom(oAtom3, oMono2);
            IDictionary<string, IMonomer> monomers = new Dictionary<string, IMonomer>();
            //IMonomer oMon = Builder.NewMonomer();
            monomers.Add("TRP279", oMono1);
            monomers.Add("HOH", oMono2);

            Assert.AreEqual(2, oPolymer.GetMonomerNames().Count());
            Assert.IsTrue(oPolymer.GetMonomerNames().Contains(oMono1.MonomerName));
            Assert.IsTrue(oPolymer.GetMonomerNames().Contains(oMono2.MonomerName));
            Assert.IsTrue(Compares.AreDeepEqual(monomers.Keys, oPolymer.GetMonomerNames()));
        }

        [TestMethod()]
        public virtual void TestRemoveMonomer_String()
        {
            IPolymer oPolymer = (IPolymer)NewChemObject();
            IMonomer oMono1 = oPolymer.Builder.CreateMonomer();
            oMono1.MonomerName = "TRP279";
            IAtom oAtom1 = oPolymer.Builder.CreateAtom("C1");
            oPolymer.AddAtom(oAtom1, oMono1);
            Assert.IsTrue(oPolymer.GetMonomerNames().Contains(oMono1.MonomerName));
            Assert.AreEqual(1, oPolymer.Atoms.Count);

            oPolymer.RemoveMonomer("TRP279");
            Assert.IsFalse(oPolymer.GetMonomerNames().Contains(oMono1.MonomerName));
            Assert.AreEqual(0, oPolymer.Atoms.Count);
        }

        /// <summary>
        /// Method to test whether the class complies with RFC #9.
        /// </summary>
        [TestMethod()]

        public override void TestToString()
        {
            IPolymer polymer = (IPolymer)NewChemObject();
            IMonomer oMono1 = polymer.Builder.CreateMonomer();
            oMono1.MonomerName = "TRP279";
            IMonomer oMono2 = polymer.Builder.CreateMonomer();
            oMono2.MonomerName = "HOH";
            IAtom oAtom2 = polymer.Builder.CreateAtom("C2");
            IAtom oAtom3 = polymer.Builder.CreateAtom("C3");
            polymer.AddAtom(oAtom2, oMono1);
            polymer.AddAtom(oAtom3, oMono2);
            IDictionary<string, IMonomer> monomers = new Dictionary<string, IMonomer>();
            monomers.Add("TRP279", oMono1);
            monomers.Add("HOH", oMono2);
            string description = polymer.ToString();
            for (int i = 0; i < description.Length; i++)
            {
                Assert.IsTrue('\n' != description[i]);
                Assert.IsTrue('\r' != description[i]);
            }
        }

        /// <summary>
        /// Method to test the Clone() method
        /// </summary>
        [TestMethod()]

        public override void TestClone()
        {
            IPolymer polymer = (IPolymer)NewChemObject();
            object clone = polymer.Clone();
            Assert.IsTrue(clone is IPolymer);
        }

    }
}