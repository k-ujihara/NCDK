



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
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
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
    /// Checks the functionality of the <see cref="PDBPolymer"/>.
    /// </summary>
    [TestClass()]
    public class PDBPolymerTest : AbstractPDBPolymerTest
    {
        public override IChemObject NewChemObject()
        {
            return new PDBPolymer();
        }

        [TestMethod()]
        public void TestPDBPolymer()
        {
            IPDBPolymer pdbPolymer = new PDBPolymer();
            Assert.IsNotNull(pdbPolymer);
            Assert.AreEqual(pdbPolymer.GetMonomerMap().Count(), 0);

            IStrand oStrand1 = pdbPolymer.Builder.NewStrand();
            oStrand1.StrandName = "A";
            IStrand oStrand2 = pdbPolymer.Builder.NewStrand();
            oStrand2.StrandName = "B";
            IMonomer oMono1 = pdbPolymer.Builder.NewMonomer();
            oMono1.MonomerName = "TRP279";
            IMonomer oMono2 = pdbPolymer.Builder.NewMonomer();
            oMono2.MonomerName = "HOH";
            IMonomer oMono3 = pdbPolymer.Builder.NewMonomer();
            oMono3.MonomerName = "GLYA16";
            IPDBAtom oPDBAtom1 = pdbPolymer.Builder.NewPDBAtom("C");
            IPDBAtom oPDBAtom2 = pdbPolymer.Builder.NewPDBAtom("C");
            IPDBAtom oPDBAtom3 = pdbPolymer.Builder.NewPDBAtom("C");
            IPDBAtom oPDBAtom4 = pdbPolymer.Builder.NewPDBAtom("C");
            IPDBAtom oPDBAtom5 = pdbPolymer.Builder.NewPDBAtom("C");

            pdbPolymer.Atoms.Add(oPDBAtom1);
            pdbPolymer.AddAtom(oPDBAtom2, oStrand1);
            pdbPolymer.AddAtom(oPDBAtom3, oMono1, oStrand1);
            pdbPolymer.AddAtom(oPDBAtom4, oMono2, oStrand2);
            pdbPolymer.AddAtom(oPDBAtom5, oMono3, oStrand2);
            Assert.IsNotNull(pdbPolymer.Atoms[0]);
            Assert.IsNotNull(pdbPolymer.Atoms[1]);
            Assert.IsNotNull(pdbPolymer.Atoms[2]);
            Assert.IsNotNull(pdbPolymer.Atoms[3]);
            Assert.IsNotNull(pdbPolymer.Atoms[4]);
            Assert.AreEqual(oPDBAtom1, pdbPolymer.Atoms[0]);
            Assert.AreEqual(oPDBAtom2, pdbPolymer.Atoms[1]);
            Assert.AreEqual(oPDBAtom3, pdbPolymer.Atoms[2]);
            Assert.AreEqual(oPDBAtom4, pdbPolymer.Atoms[3]);
            Assert.AreEqual(oPDBAtom5, pdbPolymer.Atoms[4]);

            Assert.IsNull(pdbPolymer.GetMonomer("0815", "A"));
            Assert.IsNull(pdbPolymer.GetMonomer("0815", "B"));
            Assert.IsNull(pdbPolymer.GetMonomer("0815", ""));
            Assert.IsNull(pdbPolymer.GetStrand(""));
            Assert.IsNotNull(pdbPolymer.GetMonomer("TRP279", "A"));
            Assert.AreEqual(oMono1, pdbPolymer.GetMonomer("TRP279", "A"));
            Assert.AreEqual(pdbPolymer.GetMonomer("TRP279", "A").Atoms.Count, 1);
            Assert.IsNotNull(pdbPolymer.GetMonomer("HOH", "B"));
            Assert.AreEqual(oMono2, pdbPolymer.GetMonomer("HOH", "B"));
            Assert.AreEqual(pdbPolymer.GetMonomer("HOH", "B").Atoms.Count, 1);
            Assert.AreEqual(pdbPolymer.GetStrand("B").Atoms.Count, 2);
            Assert.AreEqual(pdbPolymer.GetStrand("B").GetMonomerMap().Count(), 2);
            Assert.IsNull(pdbPolymer.GetStrand("C"));
            Assert.IsNotNull(pdbPolymer.GetStrand("B"));
        }

        [TestMethod()]
        public void TestGetMonomerNamesInSequentialOrder()
        {
            PDBPolymer pdbPolymer = new PDBPolymer();
            Assert.AreEqual(0, pdbPolymer.GetMonomerNames().Count());

            IStrand oStrand1 = pdbPolymer.Builder.NewStrand();
            oStrand1.StrandName = "A";
            IMonomer oMono1 = pdbPolymer.Builder.NewMonomer();
            oMono1.MonomerName = "TRP279";
            IMonomer oMono2 = pdbPolymer.Builder.NewMonomer();
            oMono2.MonomerName = "CYS280";
            IPDBAtom oPDBAtom2 = pdbPolymer.Builder.NewPDBAtom("C");
            IPDBAtom oPDBAtom3 = pdbPolymer.Builder.NewPDBAtom("C");
            pdbPolymer.AddAtom(oPDBAtom2, oMono1, oStrand1);
            pdbPolymer.AddAtom(oPDBAtom3, oMono2, oStrand1);
            Assert.IsNotNull(pdbPolymer.Atoms[0]);
            Assert.IsNotNull(pdbPolymer.Atoms[1]);
            Assert.AreEqual(oPDBAtom2, pdbPolymer.Atoms[0]);
            Assert.AreEqual(oPDBAtom3, pdbPolymer.Atoms[1]);

            var monomers = pdbPolymer.GetMonomerNamesInSequentialOrder().GetEnumerator();
            monomers.MoveNext();
            Assert.AreEqual("TRP279", monomers.Current);
            monomers.MoveNext();
            Assert.AreEqual("CYS280", monomers.Current);
        }
        
    }
}
namespace NCDK.Silent
{
    /// <summary>
    /// Checks the functionality of the <see cref="PDBPolymer"/>.
    /// </summary>
    [TestClass()]
    public class PDBPolymerTest : AbstractPDBPolymerTest
    {
        public override IChemObject NewChemObject()
        {
            return new PDBPolymer();
        }

        [TestMethod()]
        public void TestPDBPolymer()
        {
            IPDBPolymer pdbPolymer = new PDBPolymer();
            Assert.IsNotNull(pdbPolymer);
            Assert.AreEqual(pdbPolymer.GetMonomerMap().Count(), 0);

            IStrand oStrand1 = pdbPolymer.Builder.NewStrand();
            oStrand1.StrandName = "A";
            IStrand oStrand2 = pdbPolymer.Builder.NewStrand();
            oStrand2.StrandName = "B";
            IMonomer oMono1 = pdbPolymer.Builder.NewMonomer();
            oMono1.MonomerName = "TRP279";
            IMonomer oMono2 = pdbPolymer.Builder.NewMonomer();
            oMono2.MonomerName = "HOH";
            IMonomer oMono3 = pdbPolymer.Builder.NewMonomer();
            oMono3.MonomerName = "GLYA16";
            IPDBAtom oPDBAtom1 = pdbPolymer.Builder.NewPDBAtom("C");
            IPDBAtom oPDBAtom2 = pdbPolymer.Builder.NewPDBAtom("C");
            IPDBAtom oPDBAtom3 = pdbPolymer.Builder.NewPDBAtom("C");
            IPDBAtom oPDBAtom4 = pdbPolymer.Builder.NewPDBAtom("C");
            IPDBAtom oPDBAtom5 = pdbPolymer.Builder.NewPDBAtom("C");

            pdbPolymer.Atoms.Add(oPDBAtom1);
            pdbPolymer.AddAtom(oPDBAtom2, oStrand1);
            pdbPolymer.AddAtom(oPDBAtom3, oMono1, oStrand1);
            pdbPolymer.AddAtom(oPDBAtom4, oMono2, oStrand2);
            pdbPolymer.AddAtom(oPDBAtom5, oMono3, oStrand2);
            Assert.IsNotNull(pdbPolymer.Atoms[0]);
            Assert.IsNotNull(pdbPolymer.Atoms[1]);
            Assert.IsNotNull(pdbPolymer.Atoms[2]);
            Assert.IsNotNull(pdbPolymer.Atoms[3]);
            Assert.IsNotNull(pdbPolymer.Atoms[4]);
            Assert.AreEqual(oPDBAtom1, pdbPolymer.Atoms[0]);
            Assert.AreEqual(oPDBAtom2, pdbPolymer.Atoms[1]);
            Assert.AreEqual(oPDBAtom3, pdbPolymer.Atoms[2]);
            Assert.AreEqual(oPDBAtom4, pdbPolymer.Atoms[3]);
            Assert.AreEqual(oPDBAtom5, pdbPolymer.Atoms[4]);

            Assert.IsNull(pdbPolymer.GetMonomer("0815", "A"));
            Assert.IsNull(pdbPolymer.GetMonomer("0815", "B"));
            Assert.IsNull(pdbPolymer.GetMonomer("0815", ""));
            Assert.IsNull(pdbPolymer.GetStrand(""));
            Assert.IsNotNull(pdbPolymer.GetMonomer("TRP279", "A"));
            Assert.AreEqual(oMono1, pdbPolymer.GetMonomer("TRP279", "A"));
            Assert.AreEqual(pdbPolymer.GetMonomer("TRP279", "A").Atoms.Count, 1);
            Assert.IsNotNull(pdbPolymer.GetMonomer("HOH", "B"));
            Assert.AreEqual(oMono2, pdbPolymer.GetMonomer("HOH", "B"));
            Assert.AreEqual(pdbPolymer.GetMonomer("HOH", "B").Atoms.Count, 1);
            Assert.AreEqual(pdbPolymer.GetStrand("B").Atoms.Count, 2);
            Assert.AreEqual(pdbPolymer.GetStrand("B").GetMonomerMap().Count(), 2);
            Assert.IsNull(pdbPolymer.GetStrand("C"));
            Assert.IsNotNull(pdbPolymer.GetStrand("B"));
        }

        [TestMethod()]
        public void TestGetMonomerNamesInSequentialOrder()
        {
            PDBPolymer pdbPolymer = new PDBPolymer();
            Assert.AreEqual(0, pdbPolymer.GetMonomerNames().Count());

            IStrand oStrand1 = pdbPolymer.Builder.NewStrand();
            oStrand1.StrandName = "A";
            IMonomer oMono1 = pdbPolymer.Builder.NewMonomer();
            oMono1.MonomerName = "TRP279";
            IMonomer oMono2 = pdbPolymer.Builder.NewMonomer();
            oMono2.MonomerName = "CYS280";
            IPDBAtom oPDBAtom2 = pdbPolymer.Builder.NewPDBAtom("C");
            IPDBAtom oPDBAtom3 = pdbPolymer.Builder.NewPDBAtom("C");
            pdbPolymer.AddAtom(oPDBAtom2, oMono1, oStrand1);
            pdbPolymer.AddAtom(oPDBAtom3, oMono2, oStrand1);
            Assert.IsNotNull(pdbPolymer.Atoms[0]);
            Assert.IsNotNull(pdbPolymer.Atoms[1]);
            Assert.AreEqual(oPDBAtom2, pdbPolymer.Atoms[0]);
            Assert.AreEqual(oPDBAtom3, pdbPolymer.Atoms[1]);

            var monomers = pdbPolymer.GetMonomerNamesInSequentialOrder().GetEnumerator();
            monomers.MoveNext();
            Assert.AreEqual("TRP279", monomers.Current);
            monomers.MoveNext();
            Assert.AreEqual("CYS280", monomers.Current);
        }
        
 
                [TestMethod()]
        public override void TestSetAtoms_RemoveListener()
        {
            ChemObjectTestHelper.TestSetAtoms_RemoveListener(NewChemObject());
        }

                // Overwrite default methods: no notifications are expected!

        [TestMethod()]
        public override void TestNotifyChanged()
        {
            ChemObjectTestHelper.TestNotifyChanged(NewChemObject());
        }

        [TestMethod()]
        public override void TestNotifyChanged_SetFlag()
        {
            ChemObjectTestHelper.TestNotifyChanged_SetFlag(NewChemObject());
        }

        [TestMethod()]
        public void TestNotifyChanged_SetFlags()
        {
            ChemObjectTestHelper.TestNotifyChanged_SetFlags(NewChemObject());
        }

        [TestMethod()]
        public override void TestNotifyChanged_IChemObjectChangeEvent()
        {
            ChemObjectTestHelper.TestNotifyChanged_IChemObjectChangeEvent(NewChemObject());
        }

        [TestMethod()]
        public override void TestStateChanged_IChemObjectChangeEvent()
        {
            ChemObjectTestHelper.TestStateChanged_IChemObjectChangeEvent(NewChemObject());
        }

        [TestMethod()]
        public override void TestClone_ChemObjectListeners()
        {
            ChemObjectTestHelper.TestClone_ChemObjectListeners(NewChemObject());
        }

        [TestMethod()]
        public override void TestAddListener_IChemObjectListener()
        {
            ChemObjectTestHelper.TestAddListener_IChemObjectListener(NewChemObject());
        }

        [TestMethod()]
        public override void TestGetListenerCount()
        {
            ChemObjectTestHelper.TestGetListenerCount(NewChemObject());
        }

        [TestMethod()]
        public override void TestRemoveListener_IChemObjectListener()
        {
            ChemObjectTestHelper.TestRemoveListener_IChemObjectListener(NewChemObject());
        }

        [TestMethod()]
        public override void TestSetNotification_true()
        {
            ChemObjectTestHelper.TestSetNotification_true(NewChemObject());
        }

        [TestMethod()]
        public override void TestNotifyChanged_SetProperty()
        {
            ChemObjectTestHelper.TestNotifyChanged_SetProperty(NewChemObject());
        }

        [TestMethod()]
        public override void TestNotifyChanged_RemoveProperty()
        {
            ChemObjectTestHelper.TestNotifyChanged_RemoveProperty(NewChemObject());
        }

    }
}

