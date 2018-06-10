/* Copyright (C) 2010  Egon Willighagen <egon.willighagen@gmail.com>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License as published by the Free
 * Software Foundation; either version 2.1 of the License, or (at your option)
 * any later version. All we ask is that proper credit is given for our work,
 * which includes - but is not limited to - adding the above copyright notice to
 * the beginning of your source code files, and to any copyright notice that you
 * may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License for more
 * details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software Foundation, Inc.,
 * 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.Events;
using NCDK.Renderers.Elements;
using NCDK.Renderers.Generators;
using NCDK.Renderers.Selection;
using System.Collections.Generic;

namespace NCDK.Renderers
{
    // @cdk.module test-render
    [TestClass()]
    public class RendererModelTest
    {
        class Class_IGenerator_IChemObject : IGenerator<IChemObject>
        {
            public IRenderingElement Generate(IChemObject obj, RendererModel model) => null;
        }
        
        [TestMethod()]
        public void TestGetSetNotification()
        {
            RendererModel model = new RendererModel();
            // test the default setting
            Assert.IsTrue(model.Notification);
            model.Notification = false;
            Assert.IsFalse(model.Notification);
            model.Notification = true;
            Assert.IsTrue(model.Notification);
        }

        [TestMethod()]
        public void TestNoDefaultToolTips()
        {
            RendererModel model = new RendererModel();
            // test: no default tool tips
            Assert.IsNull(model.GetToolTipText(new Atom()));
            // but a non-null map
            Assert.IsNotNull(model.GetToolTipTextMap());
        }

        [TestMethod()]
        public void TestToolTipFunctionality()
        {
            var tips = new Dictionary<IAtom, string>();
            IAtom anonAtom = new Atom();
            tips[anonAtom] = "Repelsteeltje";
            RendererModel model = new RendererModel();
            model.SetToolTipTextMap(tips);
            Assert.AreEqual(tips, model.GetToolTipTextMap());
            Assert.AreEqual("Repelsteeltje", model.GetToolTipText(anonAtom));
        }

        [TestMethod()]
        public void TestExternalSelectedPart()
        {
            RendererModel model = new RendererModel();
            // test default
            Assert.IsNull(model.GetExternalSelectedPart());
            IAtomContainer content = new AtomContainer();
            model.SetExternalSelectedPart(content);
            Assert.AreEqual(content, model.GetExternalSelectedPart());
            model.SetExternalSelectedPart(null);
            Assert.IsNull(model.GetExternalSelectedPart());
        }

        [TestMethod()]
        public void TestHighlightedAtom()
        {
            RendererModel model = new RendererModel();
            // test default
            Assert.IsNull(model.GetHighlightedAtom());
            IAtom content = new Atom();
            model.SetHighlightedAtom(content);
            Assert.AreEqual(content, model.GetHighlightedAtom());
            model.SetHighlightedAtom(null);
            Assert.IsNull(model.GetHighlightedAtom());
        }

        [TestMethod()]
        public void TestHighlightedBond()
        {
            RendererModel model = new RendererModel();
            // test default
            Assert.IsNull(model.GetHighlightedBond());
            IBond content = new Bond();
            model.SetHighlightedBond(content);
            Assert.AreEqual(content, model.GetHighlightedBond());
            model.SetHighlightedBond(null);
            Assert.IsNull(model.GetHighlightedBond());
        }

        class MockSelection : IChemObjectSelection
        {
            public void Select(IChemModel chemModel) { }
            public IAtomContainer GetConnectedAtomContainer() => null;
            public bool IsFilled() => false;
            public bool Contains(IChemObject obj) => false;
            ICollection<T> IChemObjectSelection.Elements<T>() => null;
        }

        [TestMethod()]
        public void TestSelection()
        {
            RendererModel model = new RendererModel();
            // test default
            Assert.IsNull(model.GetSelection());
            IChemObjectSelection content = new MockSelection();
            model.SetSelection(content);
            Assert.AreEqual(content, model.GetSelection());
            model.SetSelection(null);
            Assert.IsNull(model.GetSelection());
        }

        class MockListener : ICDKChangeListener
        {
            public bool IsChanged { get; set; } = false;

            public void StateChanged(ChemObjectChangeEventArgs evt)
            {
                IsChanged = true;
            }
        }

        [TestMethod()]
        public void TestListening()
        {
            RendererModel model = new RendererModel();
            // test default
            MockListener listener = new MockListener();
            model.Listeners.Add(listener);
            Assert.IsFalse(listener.IsChanged);
            model.OnStateChanged(null);
            Assert.IsTrue(listener.IsChanged);

            // test unregistering
            listener.IsChanged = false;
            Assert.IsFalse(listener.IsChanged);
            model.Listeners.Remove(listener);
            model.OnStateChanged(null);
            Assert.IsFalse(listener.IsChanged);
        }

        [TestMethod()]
        public void TestMerge()
        {
            RendererModel model = new RendererModel();
            Assert.IsNotNull(model.Merge);
            // any further testing I can do here?
        }
    }
}
