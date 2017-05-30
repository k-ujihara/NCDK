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
using System;
using System.Collections.Generic;
using System.Linq;
using NCDK.Common.Base;

namespace NCDK
{
    /// <summary>
    /// Tests the functionality of <see cref="IStrand"/> implementations.
    /// </summary>
    // @author     Martin Eklund <martin.eklund@farmbio.uu.se>
    // @cdk.module test-interfaces
    [TestClass()]
    public abstract class AbstractStrandTest
        : AbstractAtomContainerTest
    {
        [TestMethod()]
        public virtual void TestGetStrandName()
        {
            IStrand oStrand = (IStrand)NewChemObject();
            oStrand.StrandName = "A";

            Assert.AreEqual("A", oStrand.StrandName);
        }

        [TestMethod()]
        public virtual void TestGetStrandType()
        {
            IStrand oStrand = (IStrand)NewChemObject();
            oStrand.StrandType = "DNA";

            Assert.AreEqual("DNA", oStrand.StrandType);
        }

        /// <summary> The methods above effectively test SetStrandName and
        /// SetStrandType as well, but I include SetStrandName and
        /// SetStrandType explicitly as well (for concinstency).
        /// </summary>

        [TestMethod()]
        public virtual void TestSetStrandName_String()
        {
            IStrand oStrand = (IStrand)NewChemObject();
            oStrand.StrandName = "A";

            Assert.AreEqual("A", oStrand.StrandName);
        }

        [TestMethod()]
        public virtual void TestSetStrandType_String()
        {
            IStrand oStrand = (IStrand)NewChemObject();
            oStrand.StrandType = "DNA";

            Assert.AreEqual("DNA", oStrand.StrandType);
        }

        [TestMethod()]

        public override void TestAddAtom_IAtom()
        {
            IStrand oStrand = (IStrand)NewChemObject();
            IAtom oAtom1 = oStrand.Builder.CreateAtom("C");
            IAtom oAtom2 = oStrand.Builder.CreateAtom("C");
            oStrand.Atoms.Add(oAtom1);
            oStrand.Atoms.Add(oAtom2);

            Assert.AreEqual(2, oStrand.Atoms.Count);
        }

        [TestMethod()]
        public virtual void TestAddAtom_IAtom_IMonomer()
        {
            IStrand oStrand = (IStrand)NewChemObject();
            IMonomer oMono1 = oStrand.Builder.CreateMonomer();
            oMono1.MonomerName = "TRP279";
            IAtom oAtom1 = oStrand.Builder.CreateAtom("C");
            IAtom oAtom2 = oStrand.Builder.CreateAtom("C");
            IAtom oAtom3 = oStrand.Builder.CreateAtom("C");
            oStrand.AddAtom(oAtom1);
            oStrand.AddAtom(oAtom2);
            oStrand.AddAtom(oAtom3, oMono1);

            Assert.AreEqual(2, oStrand.GetMonomer("").Atoms.Count);
            Assert.AreEqual(1, oStrand.GetMonomer("TRP279").Atoms.Count);
        }

        [TestMethod()]
        public virtual void TestGetMonomerCount()
        {
            IStrand oStrand = (IStrand)NewChemObject();
            IMonomer oMono1 = oStrand.Builder.CreateMonomer();
            oMono1.MonomerName = "TRP279";
            IMonomer oMono2 = oStrand.Builder.CreateMonomer();
            oMono2.MonomerName = "HOH";
            IAtom oAtom2 = oStrand.Builder.CreateAtom("C");
            IAtom oAtom3 = oStrand.Builder.CreateAtom("C");
            oStrand.AddAtom(oAtom2, oMono1);
            oStrand.AddAtom(oAtom3, oMono2);

            Assert.AreEqual(2, oStrand.GetMonomerMap().Count());
        }

        [TestMethod()]
        public virtual void TestGetMonomer_String()
        {
            IStrand oStrand = (IStrand)NewChemObject();
            IMonomer oMono1 = oStrand.Builder.CreateMonomer();
            oMono1.MonomerName = "TRP279";
            IMonomer oMono2 = oStrand.Builder.CreateMonomer();
            oMono2.MonomerName = "HOH";
            IAtom oAtom2 = oStrand.Builder.CreateAtom("C");
            IAtom oAtom3 = oStrand.Builder.CreateAtom("C");
            oStrand.AddAtom(oAtom2, oMono1);
            oStrand.AddAtom(oAtom3, oMono2);

            Assert.AreEqual(oMono1, oStrand.GetMonomer("TRP279"));
            Assert.AreEqual(oMono2, oStrand.GetMonomer("HOH"));
            Assert.IsNull(oStrand.GetMonomer("TEST"));
        }

        [TestMethod()]
        public virtual void TestGetMonomerNames()
        {
            IStrand oStrand = (IStrand)NewChemObject();
            IMonomer oMono1 = oStrand.Builder.CreateMonomer();
            oMono1.MonomerName = "TRP279";
            IMonomer oMono2 = oStrand.Builder.CreateMonomer();
            oMono2.MonomerName = "HOH";
            IAtom oAtom2 = oStrand.Builder.CreateAtom("C");
            IAtom oAtom3 = oStrand.Builder.CreateAtom("C");
            oStrand.AddAtom(oAtom2, oMono1);
            oStrand.AddAtom(oAtom3, oMono2);
            IDictionary<string, IMonomer> monomers = new Dictionary<string, IMonomer>();
            IMonomer oMon = oStrand.Builder.CreateMonomer();
            oMon.MonomerName = "";
            oMon.MonomerType = "Unknown";
            monomers.Add("", oMon);
            monomers.Add("TRP279", oMono1);
            monomers.Add("HOH", oMono2);

            Assert.IsTrue(Compares.AreDeepEqual(monomers.Keys, oStrand.GetMonomerNames()));
            // Assert.AreEqual(3, oStrand.GetMonomerNames().Count());
            // Assert.IsTrue
            // (oStrand.GetMonomerNames().Contains(oMono1.MonomerName));
            // Assert.
            // IsTrue(oStrand.GetMonomerNames().Contains(oMono2.getMonomerName
            // ()));
        }

        [TestMethod()]
        public virtual void TestRemoveMonomer_String()
        {
            IStrand oStrand = (IStrand)NewChemObject();
            IMonomer oMono1 = oStrand.Builder.CreateMonomer();
            oMono1.MonomerName = "TRP279";
            IAtom oAtom1 = oStrand.Builder.CreateAtom("C");
            oStrand.AddAtom(oAtom1, oMono1);
            Assert.IsTrue(oStrand.GetMonomerNames().Contains(oMono1.MonomerName));
            Assert.AreEqual(1, oStrand.Atoms.Count);
            oStrand.RemoveMonomer("TRP279");
            Assert.IsFalse(oStrand.GetMonomerNames().Contains(oMono1.MonomerName));
            Assert.AreEqual(0, oStrand.Atoms.Count);
        }

        [TestMethod()]
        public virtual void TestGetMonomers()
        {
            IStrand oStrand = (IStrand)NewChemObject();
            IMonomer oMono1 = oStrand.Builder.CreateMonomer();
            oMono1.MonomerName = "TRP279";
            IMonomer oMono2 = oStrand.Builder.CreateMonomer();
            oMono2.MonomerName = "HOH";
            IAtom oAtom2 = oStrand.Builder.CreateAtom("C");
            IAtom oAtom3 = oStrand.Builder.CreateAtom("C");
            oStrand.AddAtom(oAtom2, oMono1);
            oStrand.AddAtom(oAtom3, oMono2);
            IDictionary<string, IMonomer> monomers = new Dictionary<string, IMonomer>();
            IMonomer oMon = oStrand.Builder.CreateMonomer();
            oMon.MonomerName = "";
            oMon.MonomerType = "Unknown";
            monomers.Add("", oMon);
            monomers.Add("TRP279", oMono1);
            monomers.Add("HOH", oMono2);

            Assert.IsTrue(Compares.AreDeepEqual(monomers.Keys, oStrand.GetMonomerNames()));
        }

        /// <summary>
        /// Method to test whether the class complies with RFC #9.
        /// </summary>
        [TestMethod()]
        public override void TestToString()
        {
            IStrand oStrand = (IStrand)NewChemObject();
            IMonomer oMono1 = oStrand.Builder.CreateMonomer();
            oMono1.MonomerName = "TRP279";
            IMonomer oMono2 = oStrand.Builder.CreateMonomer();
            oMono2.MonomerName = "HOH";
            IAtom oAtom2 = oStrand.Builder.CreateAtom("C");
            IAtom oAtom3 = oStrand.Builder.CreateAtom("C");
            oStrand.AddAtom(oAtom2, oMono1);
            oStrand.AddAtom(oAtom3, oMono2);
            IDictionary<string, IMonomer> monomers = new Dictionary<string, IMonomer>();
            IMonomer oMon = oStrand.Builder.CreateMonomer();
            oMon.MonomerName = "";
            oMon.MonomerType = "Unknown";
            monomers.Add("", oMon);
            monomers.Add("TRP279", oMono1);
            monomers.Add("HOH", oMono2);
            string description = oStrand.ToString();
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
            IStrand strand = (IStrand)NewChemObject();
            object clone = strand.Clone();
            Assert.IsTrue(clone is IStrand);
        }
    }
}
