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
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NCDK.Silent
{
    /// <summary>
    /// Helper class to test the functionality of the <see cref="IChemObject"/>s.
    /// </summary>
    // @cdk.module test-silent
    public class ChemObjectTestHelper
    {
        public static void TestNotifyChanged(IChemObject chemObject)
        {
            ChemObjectListener listener = new ChemObjectListener();
            chemObject.Listeners.Add(listener);

            chemObject.Id = "Changed";
            Assert.IsFalse(listener.Changed);
        }

        public static void TestNotifyChanged_IChemObjectChangeEvent(IChemObject chemObject)
        {
            ChemObjectListener listener = new ChemObjectListener();
            chemObject.Listeners.Add(listener);

            chemObject.Id = "Changed";
            Assert.IsNull(listener.Event);
        }

        public static void TestStateChanged_IChemObjectChangeEvent(IChemObject chemObject)
        {
            ChemObjectListener listener = new ChemObjectListener();
            chemObject.Listeners.Add(listener);

            chemObject.Id = "Changed";
            Assert.IsFalse(listener.Changed);

            listener.Reset();
            Assert.IsFalse(listener.Changed);
            chemObject.SetProperty("Changed", "Again");
            Assert.IsFalse(listener.Changed);

            listener.Reset();
            Assert.IsFalse(listener.Changed);
            chemObject.IsPlaced = true;
            Assert.IsFalse(listener.Changed);
        }

        public static void TestNotifyChanged_SetFlag(IChemObject chemObject)
        {
            ChemObjectListener listener = new ChemObjectListener();
            chemObject.Listeners.Add(listener);

            Assert.IsFalse(listener.Changed);
            chemObject.IsPlaced = true;
            Assert.IsFalse(listener.Changed);
        }

        public static void TestNotifyChanged_SetFlags(IChemObject chemObject)
        {
            ChemObjectListener listener = new ChemObjectListener();
            chemObject.Listeners.Add(listener);

            Assert.IsFalse(listener.Changed);
            chemObject.IsVisited = true;
            Assert.IsFalse(listener.Changed);
        }

        public static void TestClone_ChemObjectListeners(IChemObject chemObject)
        {
            ChemObjectListener listener = new ChemObjectListener();
            chemObject.Listeners.Add(listener);
            IChemObject chemObject2 = (IChemObject)chemObject.Clone();

            // test lack of cloning of listeners
            Assert.AreEqual(0, chemObject.Listeners.Count);
            Assert.AreEqual(0, chemObject2.Listeners.Count);
        }

        public static void TestAddListener_IChemObjectListener(IChemObject chemObject)
        {
            Assert.AreEqual(0, chemObject.Listeners.Count);
            ChemObjectListener listener = new ChemObjectListener();
            chemObject.Listeners.Add(listener);
            Assert.AreEqual(0, chemObject.Listeners.Count);
        }

        public static void TestGetListenerCount(IChemObject chemObject)
        {
            ChemObjectListener listener = new ChemObjectListener();
            chemObject.Listeners.Add(listener);
            Assert.AreEqual(0, chemObject.Listeners.Count);
        }

        public static void TestRemoveListener_IChemObjectListener(IChemObject chemObject)
        {
            ChemObjectListener listener = new ChemObjectListener();
            chemObject.Listeners.Add(listener);
            Assert.AreEqual(0, chemObject.Listeners.Count);
            chemObject.Listeners.Remove(listener);
            Assert.AreEqual(0, chemObject.Listeners.Count);
        }

        public static void TestSetNotification_true(IChemObject chemObject)
        {
            ChemObjectListener listener = new ChemObjectListener();
            chemObject.Listeners.Add(listener);
            chemObject.Notification = true;

            chemObject.Id = "Changed";
            Assert.IsFalse(listener.Changed);
        }

        public static void TestNotifyChanged_SetProperty(IChemObject chemObject)
        {
            ChemObjectListener listener = new ChemObjectListener();
            chemObject.Listeners.Add(listener);

            chemObject.SetProperty("Changed", "Yes");
            Assert.IsFalse(listener.Changed);
        }

        public static void TestNotifyChanged_RemoveProperty(IChemObject chemObject)
        {
            chemObject.SetProperty("Changed", "Yes");

            ChemObjectListener listener = new ChemObjectListener();
            chemObject.Listeners.Add(listener);

            chemObject.RemoveProperty("Changed");
            Assert.IsFalse(listener.Changed);
        }

        public static void TestSetAtoms_RemoveListener(IChemObject newChemObject)
        {
            IAtomContainer container = (IAtomContainer)newChemObject;

            IAtom[] atoms = new IAtom[4];
            atoms[0] = container.Builder.NewAtom("C");
            atoms[1] = container.Builder.NewAtom("C");
            atoms[2] = container.Builder.NewAtom("C");
            atoms[3] = container.Builder.NewAtom("O");
            container.SetAtoms(atoms);

            // if an atom changes, the atomcontainer will throw a change event too
            ChemObjectListener listener = new ChemObjectListener();
            container.Listeners.Add(listener);
            Assert.IsFalse(listener.Changed);

            // ok, change the atom, and make sure we do get an event
            atoms[0].AtomTypeName = "C.sp2";
            Assert.IsFalse(listener.Changed);

            // reset the listener, overwrite the atoms, and change an old atom.
            // if all is well, we should not get a change event this time
            listener.Reset();
            Assert.IsFalse(listener.Changed); // make sure the reset worked
            container.SetAtoms(new IAtom[0]);
            atoms[1].AtomTypeName = "C.sp2"; // make a change to an old atom
            Assert.IsFalse(listener.Changed); // but no change event should happen
        }
    }
}
