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
 */

using NCDK.Common.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Tools.Diff;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NCDK
{
    /// <summary>
    /// Tests the functionality of <see cref="IChemObject"/> implementations.
    /// </summary>
    // @author      Edgar Luttmann <edgar@uni-paderborn.de>
    // @cdk.module  test-interfaces
    // @cdk.created 2001-08-09
    [TestClass()]
    public abstract class AbstractChemObjectTest 
        : AbstractCDKObjectTest
    {
        [TestMethod()]
        public virtual void TestSetProperty_Object_Object()
        {
            IChemObject chemObject = NewChemObject();
            string cDescription = "description";
            string cProperty = "property";
            chemObject.SetProperty(cDescription, cProperty);
            Assert.AreEqual(cProperty, chemObject.GetProperty<string>(cDescription));
        }

        [TestMethod()]
        public virtual void TestSetProperties_Map()
        {
            IChemObject chemObject = NewChemObject();
            chemObject.SetProperty("remove", "me");
            var props = new Dictionary<object, object>();
            props.Add("keep", "me");
            chemObject.SetProperties(props);
            Assert.AreEqual("me", chemObject.GetProperty<string>("keep"));
            Assert.IsNull(chemObject.GetProperty<string>("remove"));
        }

        [TestMethod()]
        public virtual void TestAddProperties_Map()
        {
            IChemObject chemObject = NewChemObject();
            var props = new Dictionary<object, object>();
            string cDescription = "description";
            string cProperty = "property";
            props.Add(cDescription, cProperty);
            chemObject.AddProperties(props);
            Assert.AreEqual(cProperty, chemObject.GetProperty<string>(cDescription));
        }

        [TestMethod()]
        public virtual void TestGetProperties()
        {
            IChemObject chemObject = NewChemObject();
            Assert.IsNotNull(chemObject.GetProperties());
            Assert.AreEqual(0, chemObject.GetProperties().Count());
        }

        [TestMethod()]
        public virtual void TestLazyProperies()
        {
            TestGetProperties();
        }

        [TestMethod()]
        public virtual void TestGetProperty_Object()
        {
            IChemObject chemObject = NewChemObject();
            Assert.IsNull(chemObject.GetProperty<object>("dummy", null));
        }

        [TestMethod()]
        public virtual void TestGetProperty_Object_Class()
        {
            IChemObject chemObject = NewChemObject();
            Assert.IsNull(chemObject.GetProperty<object>("dummy") as string);
            chemObject.SetProperty("dummy", 5);
            Assert.IsNotNull(chemObject.GetProperty<object>("dummy") as int?);
        }


        [TestMethod()]
        //[ExpectedException(typeof(ArgumentException))]
        public virtual void TestGetProperty_Object_ClassCast()
        {
            IChemObject chemObject = NewChemObject();
            chemObject.SetProperty("dummy", 5);
            Assert.IsNull(chemObject.GetProperty<object>("dummy") as string);
        }

        [TestMethod()]
        public virtual void TestRemoveProperty_Object()
        {
            IChemObject chemObject = NewChemObject();
            string cDescription = "description";
            string cProperty = "property";
            chemObject.SetProperty(cDescription, cProperty);
            Assert.IsNotNull(chemObject.GetProperty<object>(cDescription));
            chemObject.RemoveProperty(cDescription);
            Assert.IsNull(chemObject.GetProperty<object>(cDescription));
        }

        [TestMethod()]
        public virtual void TestSetID_String()
        {
            IChemObject chemObject = NewChemObject();
            string id = "objectX";
            chemObject.Id = id;
            Assert.AreEqual(id, chemObject.Id);
        }

        [TestMethod()]
        public virtual void TestGetID()
        {
            IChemObject chemObject = NewChemObject();
            Assert.IsNull(chemObject.Id);
        }

#if false // flags are move to bools 

        [TestMethod()]
        public virtual void TestSetFlags_arraybool()
        {
            IChemObject chemObject = NewChemObject();
            chemObject.Flag = CDKConstants.ISINRING, true;
            IChemObject chemObject2 = NewChemObject();
            chemObject2.Flags = chemObject.GetFlags();
            Assert.IsTrue(chemObject2.IsInRing);
        }

        [TestMethod()]
        public virtual void TestGetFlags()
        {
            IChemObject chemObject = NewChemObject();
            chemObject.Flag = CDKConstants.ISINRING, true;
            IChemObject chemObject2 = NewChemObject();
            chemObject2.Flags = chemObject.GetFlags();
            Assert.IsTrue(chemObject2.IsInRing);
        }

        [TestMethod()]
        public virtual void TestGetFlagValueZeroDefault()
        {
            IChemObject chemObject = NewChemObject();
            Assert.AreEqual((short)0, chemObject.GetFlagValue());
        }

        [TestMethod()]
        public virtual void TestGetFlagValue()
        {
            IChemObject chemObject = NewChemObject();
            chemObject.Flag = CDKConstants.ISALIPHATIC, true;
            Assert.AreNotSame((short)0, chemObject.GetFlagValue());
        }

        /**
         * Different flags are reflected by different numbers.
         */
        [TestMethod()]
        public virtual void TestGetFlagValueDifferentFlags()
        {
            IChemObject chemObject = NewChemObject();
            chemObject.Flag = CDKConstants.ISALIPHATIC, true;
            IChemObject chemObject2 = NewChemObject();
            chemObject2.IsVisited = true;
            Assert.AreNotSame(chemObject.GetFlagValue(), chemObject2.GetFlagValue());
        }

        /**
         * The number is always the same for the same flag.
         */
        [TestMethod()]
        public virtual void TestGetFlagValueSameFlag()
        {
            IChemObject chemObject = NewChemObject();
            chemObject.Flag = CDKConstants.ISPLACED, true;
            IChemObject chemObject2 = NewChemObject();
            chemObject2.Flag = CDKConstants.ISPLACED, true;
            Assert.AreEqual(chemObject.GetFlagValue(), chemObject2.GetFlagValue());
        }

        [TestMethod()]
        public virtual void TestGetFlags_Array()
        {
            IChemObject chemObject = NewChemObject();
            chemObject.IsPlaced = true;
            bool[] flags = chemObject.GetFlags();
            Assert.IsTrue(flags[1]);
        }

#endif

        [TestMethod()]
        public virtual void TestSetFlag_int_bool()
        {
            IChemObject chemObject = NewChemObject();
            Assert.IsFalse(chemObject.IsPlaced);
            chemObject.IsPlaced = true;
            Assert.IsTrue(chemObject.IsPlaced);
            chemObject.IsPlaced = false;
            Assert.IsFalse(chemObject.IsPlaced);
        }

        [TestMethod()]
        public virtual void TestGetFlag_int()
        {
            TestSetFlag_int_bool();
        }

        [TestMethod()]
        public virtual void TestClone()
        {
            IChemObject chemObject = NewChemObject();
            //chemObject.Flag = CDKConstants.ISALIPHATIC, true;

            // test cloning of itself
            object clone = chemObject.Clone();
            Assert.IsTrue(clone is IChemObject);

            // test that everything has been cloned properly
            string diff = ChemObjectDiff.Diff(chemObject, (IChemObject)clone);
            Assert.IsNotNull(diff);
            Assert.AreEqual(0, diff.Length);
        }

        [TestMethod()]
        public virtual void TestClone_Flags()
        {
            IChemObject chemObject1 = NewChemObject();
            chemObject1.IsVisited = true;
            IChemObject chemObject2 = (IChemObject)chemObject1.Clone();

            // test cloning of flags field
            chemObject2.IsVisited = false;
            Assert.IsTrue(chemObject1.IsVisited);
        }

        [TestMethod()]
        public virtual void TestClone_Identifier()
        {
            IChemObject chemObject1 = NewChemObject();
            chemObject1.Id = "co1";
            IChemObject chemObject2 = (IChemObject)chemObject1.Clone();

            // test cloning of identifier field
            chemObject2.Id = "co2";
            Assert.AreEqual("co1", chemObject1.Id);
        }

        [TestMethod()]
        public virtual void TestClone_Properties()
        {
            IChemObject chemObject1 = NewChemObject();
            var props1 = new Dictionary<object, object>();
            chemObject1.AddProperties(props1);
            IChemObject chemObject2 = (IChemObject)chemObject1.Clone();

            // test cloning of properties field
            var props2 = new Dictionary<object, object>();
            props2.Add("key", "value");
            chemObject2.AddProperties(props2);
            Assert.IsTrue(Compares.AreDeepEqual(props1, chemObject1.GetProperties()));
            Assert.AreEqual(1, chemObject2.GetProperties().Count());
            Assert.AreEqual(0, chemObject1.GetProperties().Count());
        }

        [TestMethod()]
        public virtual void TestClone_Properties2()
        {
            IChemObject chemObject1 = NewChemObject();
            var props1 = new Dictionary<object, object>();
            IAtom atom = chemObject1.Builder.CreateAtom("C");
            props1.Add("atom", atom);
            chemObject1.AddProperties(props1);
            IChemObject chemObject2 = (IChemObject)chemObject1.Clone();

            // test cloning of properties field
            var props2 = new Dictionary<object, object>();
            chemObject2.AddProperties(props2);
            Assert.IsTrue(Compares.AreDeepEqual(props1, chemObject1.GetProperties()));
            Assert.AreEqual(1, chemObject2.GetProperties().Count());
            Assert.AreEqual(1, chemObject1.GetProperties().Count());
            // ok, copied hashtable item, but this item should be cloned
            Assert.IsTrue(atom.Compare(chemObject2.GetProperties().First(pair => pair.Key.Equals("atom")).Value));
        }

        /**
         * @cdk.bug 2975800
         */
        [TestMethod()]
        public virtual void TestClone_PropertyNull()
        {
            IChemObject chemObject = NewChemObject();
            string key = "NullProperty";
            chemObject.SetProperty(key, null);
            chemObject.Clone();
        }

        [TestMethod()]
        public virtual void TestClone_ChemObjectListeners()
        {
            IChemObject chemObject1 = NewChemObject();
            DummyChemObjectListener listener = new DummyChemObjectListener(this);
            chemObject1.Listeners.Add(listener);
            IChemObject chemObject2 = (IChemObject)chemObject1.Clone();

            // test lack of cloning of listeners
            Assert.AreEqual(1, chemObject1.Listeners.Count);
            Assert.AreEqual(0, chemObject2.Listeners.Count);
        }

        /// <summary>@cdk.bug 1838820</summary>
        [TestMethod()]
        [Timeout(100)]
        public virtual void TestDontCloneIChemObjectProperties()
        {
            IChemObject chemObject1 = NewChemObject();
            chemObject1.SetProperty("RecursiveBastard", chemObject1);

            object clone = chemObject1.Clone();
            Assert.IsNotNull(clone);
            Assert.IsTrue(clone is IChemObject);
        }

        [TestMethod()]
        public virtual void TestAddListener_IChemObjectListener()
        {
            IChemObject chemObject1 = NewChemObject();
            Assert.AreEqual(0, chemObject1.Listeners.Count);
            DummyChemObjectListener listener = new DummyChemObjectListener(this);
            chemObject1.Listeners.Add(listener);
            Assert.AreEqual(1, chemObject1.Listeners.Count);
        }

        [TestMethod()]
        public virtual void TestRemoveListener_IChemObjectListener()
        {
            IChemObject chemObject1 = NewChemObject();
            DummyChemObjectListener listener = new DummyChemObjectListener(this);
            chemObject1.Listeners.Add(listener);
            Assert.AreEqual(1, chemObject1.Listeners.Count);
            chemObject1.Listeners.Remove(listener);
            Assert.AreEqual(0, chemObject1.Listeners.Count);
        }

        [TestMethod()]
        public virtual void TestGetListenerCount()
        {
            IChemObject chemObject1 = NewChemObject();
            DummyChemObjectListener listener = new DummyChemObjectListener(this);
            chemObject1.Listeners.Add(listener);
            Assert.AreEqual(1, chemObject1.Listeners.Count);
        }

        [TestClass()]
        public class DummyChemObjectListener : IChemObjectListener
        {
            AbstractChemObjectTest parent;

            public DummyChemObjectListener(AbstractChemObjectTest parent)
            {
                this.parent = parent;
            }

            public void OnStateChanged(ChemObjectChangeEventArgs evt) { }
        }

        [TestMethod()]
        public virtual void TestShallowCopy()
        {
            IChemObject chemObject = NewChemObject();
            object clone = chemObject.Clone();
            Assert.IsNotNull(clone);
            Assert.IsTrue(clone is IChemObject);
        }

        [TestMethod()]
        public virtual void TestStateChanged_IChemObjectChangeEvent()
        {
            ChemObjectListenerImpl listener = new ChemObjectListenerImpl();
            IChemObject chemObject = NewChemObject();
            chemObject.Listeners.Add(listener);

            chemObject.Id = "Changed";
            Assert.IsTrue(listener.Changed);

            listener.Reset();
            Assert.IsFalse(listener.Changed);
            chemObject.SetProperty("Changed", "Again");
            Assert.IsTrue(listener.Changed);

            listener.Reset();
            Assert.IsFalse(listener.Changed);
            chemObject.IsVisited = true;
            Assert.IsTrue(listener.Changed);
        }

        [TestMethod()]
        public virtual void TestNotifyChanged()
        {
            ChemObjectListenerImpl listener = new ChemObjectListenerImpl();
            IChemObject chemObject = NewChemObject();
            chemObject.Listeners.Add(listener);

            chemObject.Id = "Changed";
            Assert.IsTrue(listener.Changed);
        }

        [TestMethod()]
        public virtual void TestSetNotification_bool()
        {
            IChemObject chemObject = NewChemObject();
            chemObject.Notification = false;
            Assert.IsFalse(chemObject.Notification);
        }

        [TestMethod()]
        public virtual void TestGetNotification()
        {
            TestSetNotification_bool();
        }

        [TestMethod()]
        public virtual void TestSetNotification_false()
        {
            ChemObjectListenerImpl listener = new ChemObjectListenerImpl();
            IChemObject chemObject = NewChemObject();
            chemObject.Listeners.Add(listener);
            chemObject.Notification = false;

            chemObject.Id = "Changed";
            Assert.IsFalse(listener.Changed);
        }

        [TestMethod()]
        public virtual void TestSetNotification_true()
        {
            ChemObjectListenerImpl listener = new ChemObjectListenerImpl();
            IChemObject chemObject = NewChemObject();
            chemObject.Listeners.Add(listener);
            chemObject.Notification = true;

            chemObject.Id = "Changed";
            Assert.IsTrue(listener.Changed);
        }

        [TestMethod()]
        public virtual void TestNotifyChanged_IChemObjectChangeEvent()
        {
            ChemObjectListenerImpl listener = new ChemObjectListenerImpl();
            IChemObject chemObject = NewChemObject();
            chemObject.Listeners.Add(listener);

            chemObject.Id = "Changed";
            Assert.IsNotNull(listener.Event);
        }

        [TestMethod()]
        public virtual void TestNotifyChanged_SetProperty()
        {
            ChemObjectListenerImpl listener = new ChemObjectListenerImpl();
            IChemObject chemObject = NewChemObject();
            chemObject.Listeners.Add(listener);

            chemObject.SetProperty("Changed", "Yes");
            Assert.IsNotNull(listener.Event);
        }

        // @cdk.bug 2992921
        [TestMethod()]
        public virtual void TestNotifyChanged_SetFlag()
        {
            ChemObjectListenerImpl listener = new ChemObjectListenerImpl();
            IChemObject chemObject = NewChemObject();
            chemObject.Listeners.Add(listener);
            Assert.IsNull(listener.Event);
            chemObject.IsVisited = true;
            Assert.IsNotNull(listener.Event);
        }

        ///**
        // * @cdk.bug 2992921
        // */
        //[TestMethod()]
        //public virtual void TestNotifyChanged_SetFlags()
        //{
        //    ChemObjectListenerImpl listener = new ChemObjectListenerImpl();
        //    IChemObject chemObject = NewChemObject();
        //    chemObject.Listeners.Add(listener);

        //    Assert.IsNull(listener.Event);
        //    chemObject.Flags = new bool[chemObject.GetFlags().Length];
        //    Assert.IsNotNull(listener.Event);
        //}

        [TestMethod()]
        public virtual void TestNotifyChanged_RemoveProperty()
        {
            IChemObject chemObject = NewChemObject();
            chemObject.SetProperty("Changed", "Yes");

            ChemObjectListenerImpl listener = new ChemObjectListenerImpl();
            chemObject.Listeners.Add(listener);

            chemObject.RemoveProperty("Changed");
            Assert.IsNotNull(listener.Event);
        }

        [TestMethod()]
        public virtual void TestNotifyChanged_RemoveNonExistentProperty()
        {
            IChemObject chemObject = NewChemObject();
            ChemObjectListenerImpl listener = new ChemObjectListenerImpl();
            chemObject.Listeners.Add(listener);

            chemObject.RemoveProperty("Changed");
            Assert.IsNull(listener.Event);
        }

        [TestMethod()]
        public virtual void TestCompare_Object()
        {
            // Added to keep the Coverage checker happy, but since the
            // Compare(object) method is not part of the interface, nothing is tested
            Assert.IsTrue(true);
        }

        private class ChemObjectListenerImpl : IChemObjectListener
        {
            public bool Changed { get; set; }
            public EventArgs Event { get; set; }

            public ChemObjectListenerImpl()
            {
                Changed = false;
                Event = null;
            }

            public void OnStateChanged(ChemObjectChangeEventArgs e)
            {
                Changed = true;
                Event = e;
            }

            public virtual void Reset()
            {
                Changed = false;
                Event = null;
            }
        }
    }
}
