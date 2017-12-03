/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@slists.sourceforge.net
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
using NCDK.Default;
using NCDK.IO.Formats;
using NCDK.IO.Listener;
using NCDK.IO.Setting;
using NCDK.Isomorphisms.Matchers;
using System;
using System.IO;

namespace NCDK.IO
{
    /// <summary>
    /// TestCase for CDK IO classes.
    /// </summary>
    // @cdk.module test-io
    [TestClass()]
    public abstract class ChemObjectIOTest : CDKTestCase
    {
        protected abstract Type ChemObjectIOToTestType { get; }

        protected IChemObjectIO CreateChemObjectIO(Stream stream)
        {
            return (IChemObjectIO)ChemObjectIOToTestType.GetConstructor(new Type[] { typeof(Stream) }).Invoke(new object[] { stream });
        }

        protected IChemObjectIO chemObjectIOToTest;
        protected virtual IChemObjectIO ChemObjectIOToTest
        {
            get
            {
                if (chemObjectIOToTest == null)
                {
                    chemObjectIOToTest = CreateChemObjectIO(new MemoryStream());
                }
                return chemObjectIOToTest;
            }
        }
            
        [TestMethod()]
        public virtual void TestChemObjectIOSet()
        {
            Assert.IsNotNull(ChemObjectIOToTestType, $"You must set {nameof(ChemObjectIOToTestType)} to set Type object of the {nameof(IChemObjectIO)}.");
        }

        [TestMethod()]
        public virtual void TestGetFormat()
        {
            IResourceFormat format = ChemObjectIOToTest.Format;
            Assert.IsNotNull(format, "The IChemObjectIO.Format method returned null.");
        }

        private static IChemObject[] acceptableNNChemObjects =
            { new ChemFile(), new ChemModel(), new Silent.AtomContainer(), new Reaction() };

        [TestMethod()]
        public virtual void TestAcceptsAtLeastOneNonotifyObject()
        {
            bool oneAccepted = false;
            foreach (var obj in acceptableNNChemObjects)
            {
                if (ChemObjectIOToTest.Accepts(obj.GetType()))
                {
                    oneAccepted = true;
                }
            }
            Assert.IsTrue(oneAccepted,
                    "At least one of the following IChemObect's should be accepted: IChemFile, IChemModel, IAtomContainer, IReaction");
        }

        /// <summary>static objects, shared between tests - difficult to locate bugs.</summary>
        [Obsolete]
        protected static IChemObject[] acceptableChemObjects = {new ChemFile(), new ChemModel(), new Silent.AtomContainer(),
            new Reaction(), new RGroupQuery(Default.ChemObjectBuilder.Instance)};

        protected static IChemObject[] AcceptableChemObjects()
        {
            return new IChemObject[]{new ChemFile(), new ChemModel(), new Silent.AtomContainer(), new Reaction(),
                new RGroupQuery(Default.ChemObjectBuilder.Instance)};
        }

        [TestMethod()]
        public virtual void TestAcceptsAtLeastOneChemObject()
        {
            bool oneAccepted = false;
            foreach (var obj in acceptableChemObjects)
            {
                if (ChemObjectIOToTest.Accepts(obj.GetType()))
                {
                    oneAccepted = true;
                }
            }
            Assert.IsTrue(oneAccepted,
                    "At least one of the following IChemObect's should be accepted: IChemFile, IChemModel, IAtomContainer, IReaction, IRGroupQuery");
        }

        protected static Type[] acceptableChemObjectClasses = {
            typeof(IChemFile), typeof(IChemModel), typeof(IAtomContainer),
            typeof(IReaction), typeof(IRGroupQuery)          };

        // @cdk.bug 3553780
        [TestMethod()]
        public virtual void TestAcceptsAtLeastOneChemObjectClass()
        {
            bool oneAccepted = false;
            foreach (var clazz in acceptableChemObjectClasses)
            {
                if (ChemObjectIOToTest.Accepts(clazz))
                {
                    oneAccepted = true;
                }
            }
            Assert.IsTrue(oneAccepted,
                    "At least one of the following IChemObect's should be accepted: IChemFile, IChemModel, IAtomContainer, IReaction, IRGroupQuery");
        }

        [TestMethod()]
        public virtual void TestClose()
        {
            ChemObjectIOToTest.Close();
        }

        [TestMethod()]
        public virtual void TestGetIOSetting()
        {
            var settings = ChemObjectIOToTest.IOSettings.Settings;
            foreach (var setting in settings)
            {
                Assert.IsNotNull(setting);
                Assert.IsNotNull(setting.DefaultSetting);
                Assert.IsNotNull(setting.Name);
                Assert.IsNotNull(setting.Question);
                Assert.IsNotNull(setting.Level);
            }
        }

        [TestMethod()]
        public virtual void TestAddChemObjectIOListener()
        {
            MyListener listener = new MyListener();
            ChemObjectIOToTest.Listeners.Add(listener);
        }

        class MyListener : IChemObjectIOListener
        {

            private int timesCalled = 0;

            public virtual void ProcessIOSettingQuestion(IOSetting setting)
            {
                timesCalled++;
            }
        }

        [TestMethod()]
        public virtual void TestRemoveChemObjectIOListener()
        {
            MyListener listener = new MyListener();
            ChemObjectIOToTest.Listeners.Add(listener);
            ChemObjectIOToTest.Listeners.Remove(listener);
        }
    }
}
