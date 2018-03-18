/* Copyright (C) 2004-2007  Miguel Rojas <miguel.rojas@uni-koeln.de>
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
using NCDK.Common.Base;
using System.Collections.Generic;
using System.Linq;

namespace NCDK
{
    /// <summary>
    /// Checks the functionality of <see cref="IPDBPolymer"/> implementations.
    /// </summary>
    // @cdk.module test-interfaces
    [TestClass()]
    public abstract class AbstractPDBPolymerTest : AbstractBioPolymerTest
    {
        [TestMethod()]
        public virtual void TestGetStructures()
        {
            IPDBPolymer pdbPolymer = (IPDBPolymer)NewChemObject();
            Assert.AreEqual(0, pdbPolymer.GetStructures().Count());
            IPDBStructure structure = pdbPolymer.Builder.NewPDBStructure();
            pdbPolymer.Add(structure);
            Assert.AreEqual(structure, pdbPolymer.GetStructures().First());
        }

        [TestMethod()]
        public virtual void TestAddStructure_IPDBStructure()
        {
            IPDBPolymer pdbPolymer = (IPDBPolymer)NewChemObject();
            IPDBStructure structure = pdbPolymer.Builder.NewPDBStructure();
            pdbPolymer.Add(structure);
            Assert.AreEqual(1, pdbPolymer.GetStructures().Count());
        }

        [TestMethod()]
        public override void TestGetMonomerCount()
        {
            IPDBPolymer pdbPolymer = (IPDBPolymer)NewChemObject();
            Assert.AreEqual(0, pdbPolymer.GetMonomerMap().Count());

            IStrand oStrand1 = pdbPolymer.Builder.NewStrand();
            oStrand1.StrandName = "A";
            IStrand oStrand2 = pdbPolymer.Builder.NewStrand();
            oStrand2.StrandName = "B";
            IMonomer oMono1 = pdbPolymer.Builder.NewMonomer();
            oMono1.MonomerName = "TRP279";
            IMonomer oMono2 = pdbPolymer.Builder.NewMonomer();
            oMono2.MonomerName = "HOH";
            IPDBAtom oPDBAtom1 = pdbPolymer.Builder.NewPDBAtom("C");
            IPDBAtom oPDBAtom2 = pdbPolymer.Builder.NewPDBAtom("C");
            IPDBAtom oPDBAtom3 = pdbPolymer.Builder.NewPDBAtom("C");
            pdbPolymer.Add(oPDBAtom1);
            pdbPolymer.AddAtom(oPDBAtom2, oMono1, oStrand1);
            pdbPolymer.AddAtom(oPDBAtom3, oMono2, oStrand2);
            Assert.IsNotNull(pdbPolymer.Atoms[0]);
            Assert.IsNotNull(pdbPolymer.Atoms[1]);
            Assert.IsNotNull(pdbPolymer.Atoms[2]);
            Assert.AreEqual(oPDBAtom1, pdbPolymer.Atoms[0]);
            Assert.AreEqual(oPDBAtom2, pdbPolymer.Atoms[1]);
            Assert.AreEqual(oPDBAtom3, pdbPolymer.Atoms[2]);

            Assert.AreEqual(2, pdbPolymer.GetMonomerMap().Count());
        }

        [TestMethod()]
        public override void TestGetMonomerNames()
        {
            IPDBPolymer pdbPolymer = (IPDBPolymer)NewChemObject();
            Assert.AreEqual(0, pdbPolymer.GetMonomerNames().Count());

            IStrand oStrand1 = pdbPolymer.Builder.NewStrand();
            oStrand1.StrandName = "A";
            IStrand oStrand2 = pdbPolymer.Builder.NewStrand();
            oStrand2.StrandName = "B";
            IMonomer oMono1 = pdbPolymer.Builder.NewMonomer();
            oMono1.MonomerName = "TRP279";
            IMonomer oMono2 = pdbPolymer.Builder.NewMonomer();
            oMono2.MonomerName = "HOH";
            IPDBAtom oPDBAtom1 = pdbPolymer.Builder.NewPDBAtom("C");
            IPDBAtom oPDBAtom2 = pdbPolymer.Builder.NewPDBAtom("C");
            IPDBAtom oPDBAtom3 = pdbPolymer.Builder.NewPDBAtom("C");
            pdbPolymer.Add(oPDBAtom1);
            pdbPolymer.AddAtom(oPDBAtom2, oMono1, oStrand1);
            pdbPolymer.AddAtom(oPDBAtom3, oMono2, oStrand2);
            Assert.IsNotNull(pdbPolymer.Atoms[0]);
            Assert.IsNotNull(pdbPolymer.Atoms[1]);
            Assert.IsNotNull(pdbPolymer.Atoms[2]);
            Assert.AreEqual(oPDBAtom1, pdbPolymer.Atoms[0]);
            Assert.AreEqual(oPDBAtom2, pdbPolymer.Atoms[1]);
            Assert.AreEqual(oPDBAtom3, pdbPolymer.Atoms[2]);

            Assert.AreEqual(3, pdbPolymer.GetMonomerNames().Count());
            Assert.IsTrue(pdbPolymer.GetMonomerNames().Contains(oMono1.MonomerName));
            Assert.IsTrue(pdbPolymer.GetMonomerNames().Contains(oMono2.MonomerName));
        }

        [TestMethod()]
        public override void TestGetMonomer_String_String()
        {
            IPDBPolymer pdbPolymer = (IPDBPolymer)NewChemObject();

            IStrand oStrand1 = pdbPolymer.Builder.NewStrand();
            oStrand1.StrandName = "A";
            IStrand oStrand2 = pdbPolymer.Builder.NewStrand();
            oStrand2.StrandName = "B";
            IMonomer oMono1 = pdbPolymer.Builder.NewMonomer();
            oMono1.MonomerName = "TRP279";
            IMonomer oMono2 = pdbPolymer.Builder.NewMonomer();
            oMono2.MonomerName = "HOH";
            IPDBAtom oPDBAtom1 = pdbPolymer.Builder.NewPDBAtom("C");
            IPDBAtom oPDBAtom2 = pdbPolymer.Builder.NewPDBAtom("C");
            IPDBAtom oPDBAtom3 = pdbPolymer.Builder.NewPDBAtom("C");
            pdbPolymer.AddAtom(oPDBAtom1, oMono1, oStrand1);
            pdbPolymer.AddAtom(oPDBAtom2, oMono1, oStrand1);
            pdbPolymer.AddAtom(oPDBAtom3, oMono2, oStrand2);

            Assert.AreEqual(oMono1, pdbPolymer.GetMonomer("TRP279", "A"));
            Assert.AreEqual(oMono2, pdbPolymer.GetMonomer("HOH", "B"));
        }

        [TestMethod()]
        public virtual void TestAddAtom_IPDBAtom()
        {
            IPDBPolymer pdbPolymer = (IPDBPolymer)NewChemObject();

            IPDBAtom oPDBAtom1 = pdbPolymer.Builder.NewPDBAtom("C");
            IPDBAtom oPDBAtom2 = pdbPolymer.Builder.NewPDBAtom("C");
            pdbPolymer.Add(oPDBAtom1);
            pdbPolymer.Add(oPDBAtom2);

            Assert.AreEqual(2, pdbPolymer.Atoms.Count);
        }

        [TestMethod()]
        public virtual void TestAddAtom_IPDBAtom_IStrand()
        {
            IPDBPolymer pdbPolymer = (IPDBPolymer)NewChemObject();
            IStrand oStrand1 = pdbPolymer.Builder.NewStrand();
            oStrand1.StrandName = "A";
            IPDBMonomer oMono1 = pdbPolymer.Builder.NewPDBMonomer();
            oMono1.MonomerName = "TRP279";
            IPDBAtom oPDBAtom1 = pdbPolymer.Builder.NewPDBAtom("C");
            IPDBAtom oPDBAtom2 = pdbPolymer.Builder.NewPDBAtom("C");
            IPDBAtom oPDBAtom3 = pdbPolymer.Builder.NewPDBAtom("C");
            pdbPolymer.AddAtom(oPDBAtom1, oStrand1);
            pdbPolymer.AddAtom(oPDBAtom2, oStrand1);
            pdbPolymer.AddAtom(oPDBAtom3, oMono1, oStrand1);

            Assert.AreEqual(2, pdbPolymer.GetMonomer("", "A").Atoms.Count);
            Assert.AreEqual(1, pdbPolymer.GetMonomer("TRP279", "A").Atoms.Count);
            Assert.AreEqual(3, pdbPolymer.Atoms.Count);
        }

        [TestMethod()]
        public virtual void TestAddAtom_IPDBAtom_IMonomer_IStrand()
        {
            IPDBPolymer pdbPolymer = (IPDBPolymer)NewChemObject();
            IStrand oStrand1 = pdbPolymer.Builder.NewStrand();
            oStrand1.StrandName = "A";
            IPDBMonomer oMono1 = pdbPolymer.Builder.NewPDBMonomer();
            oMono1.MonomerName = "TRP279";
            IPDBAtom oPDBAtom1 = pdbPolymer.Builder.NewPDBAtom("C");
            IPDBAtom oPDBAtom2 = pdbPolymer.Builder.NewPDBAtom("C");
            IPDBAtom oPDBAtom3 = pdbPolymer.Builder.NewPDBAtom("C");
            pdbPolymer.AddAtom(oPDBAtom1, oStrand1);
            pdbPolymer.AddAtom(oPDBAtom2, oStrand1);
            pdbPolymer.AddAtom(oPDBAtom3, oMono1, oStrand1);

            Assert.AreEqual(2, pdbPolymer.GetMonomer("", "A").Atoms.Count);
            Assert.AreEqual(1, pdbPolymer.GetMonomer("TRP279", "A").Atoms.Count);
            Assert.AreEqual(3, pdbPolymer.Atoms.Count);
        }

        [TestMethod()]
        public virtual void TestAddAtom_IPDBAtom_IMonomer()
        {
            IPDBPolymer pdbPolymer = (IPDBPolymer)NewChemObject();
            IPDBMonomer oMono1 = pdbPolymer.Builder.NewPDBMonomer();
            oMono1.MonomerName = "TRP279";
            IStrand oStrand1 = pdbPolymer.Builder.NewStrand();
            oStrand1.StrandName = "A";
            IPDBAtom oPDBAtom1 = pdbPolymer.Builder.NewPDBAtom("C");
            pdbPolymer.AddAtom(oPDBAtom1, oMono1, oStrand1);

            Assert.AreEqual(1, pdbPolymer.GetMonomer("TRP279", "A").Atoms.Count);
        }

        [TestMethod()]
        public override void TestGetStrandCount()
        {
            IPDBPolymer pdbPolymer = (IPDBPolymer)NewChemObject();
            IStrand oStrand1 = pdbPolymer.Builder.NewStrand();
            oStrand1.StrandName = "A";
            IMonomer oMono1 = pdbPolymer.Builder.NewMonomer();
            oMono1.MonomerName = "TRP279";
            IPDBAtom oPDBAtom1 = pdbPolymer.Builder.NewPDBAtom("C");
            pdbPolymer.AddAtom(oPDBAtom1, oMono1, oStrand1);

            Assert.AreEqual(1, pdbPolymer.GetStrandMap().Count());
        }

        [TestMethod()]
        public override void TestGetStrand_String()
        {
            IPDBPolymer pdbPolymer = (IPDBPolymer)NewChemObject();
            IStrand oStrand1 = pdbPolymer.Builder.NewStrand();
            oStrand1.StrandName = "A";
            IMonomer oMono1 = pdbPolymer.Builder.NewMonomer();
            oMono1.MonomerName = "TRP279";
            IPDBAtom oPDBAtom1 = pdbPolymer.Builder.NewPDBAtom("C");
            pdbPolymer.AddAtom(oPDBAtom1, oMono1, oStrand1);

            Assert.AreEqual(oStrand1, pdbPolymer.GetStrand("A"));
        }

        [TestMethod()]
        public override void TestGetStrandNames()
        {
            IPDBPolymer pdbPolymer = (IPDBPolymer)NewChemObject();
            IStrand oStrand1 = pdbPolymer.Builder.NewStrand();
            IStrand oStrand2 = pdbPolymer.Builder.NewStrand();
            oStrand1.StrandName = "A";
            oStrand2.StrandName = "B";
            IMonomer oMono1 = pdbPolymer.Builder.NewMonomer();
            oMono1.MonomerName = "TRP279";
            IMonomer oMono2 = pdbPolymer.Builder.NewMonomer();
            oMono2.MonomerName = "GLY123";
            IPDBAtom oPDBAtom1 = pdbPolymer.Builder.NewPDBAtom("C");
            IPDBAtom oPDBAtom2 = pdbPolymer.Builder.NewPDBAtom("C");
            pdbPolymer.AddAtom(oPDBAtom1, oMono1, oStrand1);
            pdbPolymer.AddAtom(oPDBAtom2, oMono2, oStrand2);
            IDictionary<string, IStrand> strands = new Dictionary<string, IStrand>
            {
                { "A", oStrand1 },
                { "B", oStrand2 }
            };

            Assert.IsTrue(Compares.AreDeepEqual(strands.Keys, pdbPolymer.GetStrandNames()));
        }

        [TestMethod()]
        public override void TestRemoveStrand_String()
        {
            IPDBPolymer pdbPolymer = (IPDBPolymer)NewChemObject();
            IStrand oStrand1 = pdbPolymer.Builder.NewStrand();
            oStrand1.StrandName = "A";
            IMonomer oMono1 = pdbPolymer.Builder.NewMonomer();
            oMono1.MonomerName = "TRP279";
            IPDBAtom oPDBAtom1 = pdbPolymer.Builder.NewPDBAtom("C");
            pdbPolymer.AddAtom(oPDBAtom1, oMono1, oStrand1);

            Assert.IsTrue(pdbPolymer.GetStrandNames().Contains(oStrand1.StrandName));
            Assert.AreEqual(1, pdbPolymer.Atoms.Count);
            pdbPolymer.RemoveStrand("A");
            Assert.IsFalse(pdbPolymer.GetStrandNames().Contains(oStrand1.StrandName));
            Assert.AreEqual(0, pdbPolymer.Atoms.Count);
        }

        [TestMethod()]
        public override void TestGetStrands()
        {
            IPDBPolymer pdbPolymer = (IPDBPolymer)NewChemObject();
            IStrand oStrand1 = pdbPolymer.Builder.NewStrand();
            IStrand oStrand2 = pdbPolymer.Builder.NewStrand();
            oStrand1.StrandName = "A";
            oStrand2.StrandName = "B";
            IMonomer oMono1 = pdbPolymer.Builder.NewMonomer();
            oMono1.MonomerName = "TRP279";
            IMonomer oMono2 = pdbPolymer.Builder.NewMonomer();
            oMono2.MonomerName = "GLY123";
            IPDBAtom oPDBAtom1 = pdbPolymer.Builder.NewPDBAtom("C");
            IPDBAtom oPDBAtom2 = pdbPolymer.Builder.NewPDBAtom("C");
            pdbPolymer.AddAtom(oPDBAtom1, oMono1, oStrand1);
            pdbPolymer.AddAtom(oPDBAtom2, oMono2, oStrand2);
            IDictionary<string, IStrand> strands = new Dictionary<string, IStrand>
            {
                { "A", oStrand1 },
                { "B", oStrand2 }
            };

            Assert.IsTrue(Compares.AreDeepEqual(strands, pdbPolymer.GetStrandMap()));
        }

        /// <summary>
        /// Method to test whether the class complies with RFC #9.
        /// </summary>
        [TestMethod()]
        public override void TestToString()
        {
            IPDBPolymer pdbPolymer = (IPDBPolymer)NewChemObject();
            string description = pdbPolymer.ToString();
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
            IPDBPolymer polymer = (IPDBPolymer)NewChemObject();
            object clone = polymer.Clone();
            Assert.IsTrue(clone is IBioPolymer);
        }
    }
}
