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
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 */
using NCDK.Common.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace NCDK
{
    /// <summary>
    /// Checks the functionality of {@link IBioPolymer} implementations.
    ///
    // @cdk.module test-interfaces
    /// </summary>
     [TestClass()]
    public abstract class AbstractBioPolymerTest : AbstractPolymerTest
    {
        [TestMethod()]
        public override void TestGetMonomerCount()
        {
            IBioPolymer oBioPolymer = (IBioPolymer)NewChemObject();
            Assert.AreEqual(0, oBioPolymer.GetMonomerMap().Count());

            IStrand oStrand1 = oBioPolymer.Builder.CreateStrand();
            oStrand1.StrandName = "A";
            IStrand oStrand2 = oBioPolymer.Builder.CreateStrand();
            oStrand2.StrandName = "B";
            IMonomer oMono1 = oBioPolymer.Builder.CreateMonomer();
            oMono1.MonomerName = "TRP279";
            IMonomer oMono2 = oBioPolymer.Builder.CreateMonomer();
            oMono2.MonomerName = "HOH";
            IAtom oAtom1 = oBioPolymer.Builder.CreateAtom("C1");
            IAtom oAtom2 = oBioPolymer.Builder.CreateAtom("C2");
            IAtom oAtom3 = oBioPolymer.Builder.CreateAtom("C3");
            oBioPolymer.Atoms.Add(oAtom1);
            oBioPolymer.AddAtom(oAtom2, oMono1, oStrand1);
            oBioPolymer.AddAtom(oAtom3, oMono2, oStrand2);
            Assert.IsNotNull(oBioPolymer.Atoms[0]);
            Assert.IsNotNull(oBioPolymer.Atoms[1]);
            Assert.IsNotNull(oBioPolymer.Atoms[2]);
            Assert.AreEqual(oAtom1, oBioPolymer.Atoms[0]);
            Assert.AreEqual(oAtom2, oBioPolymer.Atoms[1]);
            Assert.AreEqual(oAtom3, oBioPolymer.Atoms[2]);

            Assert.AreEqual(2, oBioPolymer.GetMonomerMap().Count());
        }

        [TestMethod()]

        public override void TestGetMonomerNames()
        {
            IBioPolymer oBioPolymer = (IBioPolymer)NewChemObject();
            Assert.AreEqual(0, oBioPolymer.GetMonomerNames().Count());

            IStrand oStrand1 = oBioPolymer.Builder.CreateStrand();
            oStrand1.StrandName = "A";
            IStrand oStrand2 = oBioPolymer.Builder.CreateStrand();
            oStrand2.StrandName = "B";
            IMonomer oMono1 = oBioPolymer.Builder.CreateMonomer();
            oMono1.MonomerName = "TRP279";
            IMonomer oMono2 = oBioPolymer.Builder.CreateMonomer();
            oMono2.MonomerName = "HOH";
            IAtom oAtom1 = oBioPolymer.Builder.CreateAtom("C1");
            IAtom oAtom2 = oBioPolymer.Builder.CreateAtom("C2");
            IAtom oAtom3 = oBioPolymer.Builder.CreateAtom("C3");
            oBioPolymer.Atoms.Add(oAtom1);
            oBioPolymer.AddAtom(oAtom2, oMono1, oStrand1);
            oBioPolymer.AddAtom(oAtom3, oMono2, oStrand2);
            Assert.IsNotNull(oBioPolymer.Atoms[0]);
            Assert.IsNotNull(oBioPolymer.Atoms[1]);
            Assert.IsNotNull(oBioPolymer.Atoms[2]);
            Assert.AreEqual(oAtom1, oBioPolymer.Atoms[0]);
            Assert.AreEqual(oAtom2, oBioPolymer.Atoms[1]);
            Assert.AreEqual(oAtom3, oBioPolymer.Atoms[2]);

            Assert.AreEqual(3, oBioPolymer.GetMonomerNames().Count());
            Assert.IsTrue(oBioPolymer.GetMonomerNames().Contains(oMono1.MonomerName));
            Assert.IsTrue(oBioPolymer.GetMonomerNames().Contains(oMono2.MonomerName));
        }

        [TestMethod()]
        public virtual void TestGetMonomer_String_String()
        {
            IBioPolymer oBioPolymer = (IBioPolymer)NewChemObject();

            IStrand oStrand1 = oBioPolymer.Builder.CreateStrand();
            oStrand1.StrandName = "A";
            IStrand oStrand2 = oBioPolymer.Builder.CreateStrand();
            oStrand2.StrandName = "B";
            IMonomer oMono1 = oBioPolymer.Builder.CreateMonomer();
            oMono1.MonomerName = "TRP279";
            IMonomer oMono2 = oBioPolymer.Builder.CreateMonomer();
            oMono2.MonomerName = "HOH";
            IAtom oAtom1 = oBioPolymer.Builder.CreateAtom("C1");
            IAtom oAtom2 = oBioPolymer.Builder.CreateAtom("C2");
            IAtom oAtom3 = oBioPolymer.Builder.CreateAtom("C3");
            oBioPolymer.AddAtom(oAtom1, oMono1, oStrand1);
            oBioPolymer.AddAtom(oAtom2, oMono1, oStrand1);
            oBioPolymer.AddAtom(oAtom3, oMono2, oStrand2);

            Assert.AreEqual(oMono1, oBioPolymer.GetMonomer("TRP279", "A"));
            Assert.AreEqual(oMono2, oBioPolymer.GetMonomer("HOH", "B"));
        }

        [TestMethod()]

        public override void TestAddAtom_IAtom()
        {
            IBioPolymer oBioPolymer = (IBioPolymer)NewChemObject();

            IAtom oAtom1 = oBioPolymer.Builder.CreateAtom("C1");
            IAtom oAtom2 = oBioPolymer.Builder.CreateAtom("C2");
            oBioPolymer.Atoms.Add(oAtom1);
            oBioPolymer.Atoms.Add(oAtom2);

            Assert.AreEqual(2, oBioPolymer.Atoms.Count);
        }

        [TestMethod()]
        public virtual void TestAddAtom_IAtom_IStrand()
        {
            IBioPolymer oBioPolymer = (IBioPolymer)NewChemObject();
            IStrand oStrand1 = oBioPolymer.Builder.CreateStrand();
            oStrand1.StrandName = "A";
            IMonomer oMono1 = oBioPolymer.Builder.CreateMonomer();
            oMono1.MonomerName = "TRP279";
            IAtom oAtom1 = oBioPolymer.Builder.CreateAtom("C1");
            IAtom oAtom2 = oBioPolymer.Builder.CreateAtom("C2");
            IAtom oAtom3 = oBioPolymer.Builder.CreateAtom("C3");
            oBioPolymer.AddAtom(oAtom1, oStrand1);
            oBioPolymer.AddAtom(oAtom2, oStrand1);
            oBioPolymer.AddAtom(oAtom3, oMono1, oStrand1);

            Assert.AreEqual(2, oBioPolymer.GetMonomer("", "A").Atoms.Count);
            Assert.AreEqual(1, oBioPolymer.GetMonomer("TRP279", "A").Atoms.Count);
            Assert.AreEqual(3, oBioPolymer.Atoms.Count);
        }

        [TestMethod()]
        public virtual void TestAddAtom_IAtom_IMonomer_IStrand()
        {
            IBioPolymer oBioPolymer = (IBioPolymer)NewChemObject();
            IStrand oStrand1 = oBioPolymer.Builder.CreateStrand();
            oStrand1.StrandName = "A";
            IMonomer oMono1 = oBioPolymer.Builder.CreateMonomer();
            oMono1.MonomerName = "TRP279";
            IAtom oAtom1 = oBioPolymer.Builder.CreateAtom("C1");
            IAtom oAtom2 = oBioPolymer.Builder.CreateAtom("C2");
            oBioPolymer.AddAtom(oAtom1, oMono1, oStrand1);
            oBioPolymer.AddAtom(oAtom2, oMono1, oStrand1);
            oBioPolymer.AddAtom(oAtom1, null, oStrand1);

            Assert.AreEqual(2, oBioPolymer.GetMonomer("TRP279", "A").Atoms.Count);
            Assert.AreEqual(0, oBioPolymer.GetMonomer("", "A").Atoms.Count);
        }

        [TestMethod()]
        public virtual void TestGetStrandCount()
        {
            IBioPolymer oBioPolymer = (IBioPolymer)NewChemObject();
            IStrand oStrand1 = oBioPolymer.Builder.CreateStrand();
            oStrand1.StrandName = "A";
            IMonomer oMono1 = oBioPolymer.Builder.CreateMonomer();
            oMono1.MonomerName = "TRP279";
            IAtom oAtom1 = oBioPolymer.Builder.CreateAtom("C1");
            oBioPolymer.AddAtom(oAtom1, oMono1, oStrand1);

            Assert.AreEqual(1, oBioPolymer.GetStrandMap().Count());
        }

        [TestMethod()]
        public virtual void TestGetStrand_String()
        {
            IBioPolymer oBioPolymer = (IBioPolymer)NewChemObject();
            IStrand oStrand1 = oBioPolymer.Builder.CreateStrand();
            oStrand1.StrandName = "A";
            IMonomer oMono1 = oBioPolymer.Builder.CreateMonomer();
            oMono1.MonomerName = "TRP279";
            IAtom oAtom1 = oBioPolymer.Builder.CreateAtom("C1");
            oBioPolymer.AddAtom(oAtom1, oMono1, oStrand1);

            Assert.AreEqual(oStrand1, oBioPolymer.GetStrand("A"));
        }

        [TestMethod()]
        public virtual void TestGetStrandNames()
        {
            IBioPolymer oBioPolymer = (IBioPolymer)NewChemObject();
            IStrand oStrand1 = oBioPolymer.Builder.CreateStrand();
            IStrand oStrand2 = oBioPolymer.Builder.CreateStrand();
            oStrand1.StrandName = "A";
            oStrand2.StrandName = "B";
            IMonomer oMono1 = oBioPolymer.Builder.CreateMonomer();
            oMono1.MonomerName = "TRP279";
            IMonomer oMono2 = oBioPolymer.Builder.CreateMonomer();
            oMono2.MonomerName = "GLY123";
            IAtom oAtom1 = oBioPolymer.Builder.CreateAtom("C1");
            IAtom oAtom2 = oBioPolymer.Builder.CreateAtom("C2");
            oBioPolymer.AddAtom(oAtom1, oMono1, oStrand1);
            oBioPolymer.AddAtom(oAtom2, oMono2, oStrand2);
            IDictionary<string, IStrand> strands = new Dictionary<string, IStrand>();
            strands.Add("A", oStrand1);
            strands.Add("B", oStrand2);

            Assert.IsTrue(Compares.AreDeepEqual(strands.Keys, oBioPolymer.GetStrandNames()));
        }

        [TestMethod()]
        public virtual void TestRemoveStrand_String()
        {
            IBioPolymer oBioPolymer = (IBioPolymer)NewChemObject();
            IStrand oStrand1 = oBioPolymer.Builder.CreateStrand();
            oStrand1.StrandName = "A";
            IMonomer oMono1 = oBioPolymer.Builder.CreateMonomer();
            oMono1.MonomerName = "TRP279";
            IAtom oAtom1 = oBioPolymer.Builder.CreateAtom("C1");
            oBioPolymer.AddAtom(oAtom1, oMono1, oStrand1);

            Assert.IsTrue(oBioPolymer.GetStrandNames().Contains(oStrand1.StrandName));
            Assert.AreEqual(1, oBioPolymer.Atoms.Count);
            oBioPolymer.RemoveStrand("A");
            Assert.IsFalse(oBioPolymer.GetStrandNames().Contains(oStrand1.StrandName));
            Assert.AreEqual(0, oBioPolymer.Atoms.Count);
        }

        [TestMethod()]
        public virtual void TestGetStrands()
        {
            IBioPolymer oBioPolymer = (IBioPolymer)NewChemObject();
            IStrand oStrand1 = oBioPolymer.Builder.CreateStrand();
            IStrand oStrand2 = oBioPolymer.Builder.CreateStrand();
            oStrand1.StrandName = "A";
            oStrand2.StrandName = "B";
            IMonomer oMono1 = oBioPolymer.Builder.CreateMonomer();
            oMono1.MonomerName = "TRP279";
            IMonomer oMono2 = oBioPolymer.Builder.CreateMonomer();
            oMono2.MonomerName = "GLY123";
            IAtom oAtom1 = oBioPolymer.Builder.CreateAtom("C1");
            IAtom oAtom2 = oBioPolymer.Builder.CreateAtom("C2");
            oBioPolymer.AddAtom(oAtom1, oMono1, oStrand1);
            oBioPolymer.AddAtom(oAtom2, oMono2, oStrand2);
            IDictionary<string, IStrand> strands = new Dictionary<string, IStrand>();
            strands.Add("A", oStrand1);
            strands.Add("B", oStrand2);

            Assert.IsTrue(Compares.AreDeepEqual(strands, oBioPolymer.GetStrandMap()));
        }

        /// <summary>
        /// Method to test whether the class complies with RFC #9.
        /// </summary>
        [TestMethod()]

        public override void TestToString()
        {
            IBioPolymer bp = (IBioPolymer)NewChemObject();
            string description = bp.ToString();
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
            IBioPolymer polymer = (IBioPolymer)NewChemObject();
            object clone = polymer.Clone();
            Assert.IsTrue(clone is IBioPolymer);
        }

    }
}
