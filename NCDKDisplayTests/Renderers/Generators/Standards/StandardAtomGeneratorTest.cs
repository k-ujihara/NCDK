/*
 * Copyright (c) 2014 European Bioinformatics Institute (EMBL-EBI)
 *                    John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 2.1 of the License, or (at
 * your option) any later version. All we ask is that proper credit is given
 * for our work, which includes - but is not limited to - adding the above
 * copyright notice to the beginning of your source code files, and to any
 * copyright notice that you may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Windows.Media;
using WPF = System.Windows;

namespace NCDK.Renderers.Generators.Standards
{
    [TestClass()]
    public class StandardAtomGeneratorTest
    {
        private static readonly Typeface font = new Typeface(new FontFamily("Verdana"), WPF::FontStyles.Normal, WPF::FontWeights.Normal, WPF::FontStretches.Normal);
        private static readonly double emSize = 12;
        private readonly TextOutline element = new TextOutline("N", font, emSize);
        private readonly TextOutline hydrogen = new TextOutline("H", font, emSize);
        private readonly StandardAtomGenerator atomGenerator = new StandardAtomGenerator(font, emSize);

        [TestMethod()]
        public void PositionHydrogenAbove()
        {
            var positioned = atomGenerator.PositionHydrogenLabel(HydrogenPosition.Above, element, hydrogen);
            var elementBounds = element.GetBounds();
            var hydrogenBounds = positioned.GetBounds();

            Assert.IsTrue(elementBounds.Top > hydrogenBounds.Bottom);
            Assert.AreEqual(hydrogenBounds.Left, elementBounds.Left, 0.01);
        }

        [TestMethod()]
        public void PositionHydrogenBelow()
        {
            var positioned = atomGenerator.PositionHydrogenLabel(HydrogenPosition.Below, element, hydrogen);
            var elementBounds = element.GetBounds();
            var hydrogenBounds = positioned.GetBounds();

            Assert.IsTrue(elementBounds.Top < hydrogenBounds.Bottom);
            Assert.AreEqual(hydrogenBounds.Left, elementBounds.Left, 0.01);
        }

        [TestMethod()]
        public void PositionHydrogenToLeft()
        {
            var positioned = atomGenerator.PositionHydrogenLabel(HydrogenPosition.Left, element, hydrogen);
            var elementBounds = element.GetBounds();
            var hydrogenBounds = positioned.GetBounds();

            Assert.IsTrue(elementBounds.Right > hydrogenBounds.Left);
            Assert.AreEqual(hydrogenBounds.Bottom, elementBounds.Bottom, 0.01);
        }

        [TestMethod()]
        public void PositionHydrogenToRight()
        {
            var positioned = atomGenerator.PositionHydrogenLabel(HydrogenPosition.Right, element, hydrogen);
            var elementBounds = element.GetBounds();
            var hydrogenBounds = positioned.GetBounds();

            Assert.IsTrue(elementBounds.Right < hydrogenBounds.Left);
            Assert.AreEqual(hydrogenBounds.Bottom, elementBounds.Bottom, 0.01);
        }

        [TestMethod()]
        public void PositionHydrogenCount()
        {
            var hydrogenCount = new TextOutline("2", font, emSize);
            var positioned = atomGenerator.PositionSubscript(hydrogen, hydrogenCount);

            var hydrogenBounds = hydrogen.GetBounds();
            var hydrogenCountBounds = positioned.GetBounds();

            Assert.IsTrue(hydrogenCountBounds.Left > hydrogenBounds.Left);
            Assert.AreEqual(hydrogenBounds.Bottom, hydrogenCountBounds.CenterY(), 0.01);
        }

        [TestMethod()]
        public void PositionMassLabel()
        {
            var mass = new TextOutline("15", font, emSize);
            var positioned = atomGenerator.PositionMassLabel(mass, element);

            var elementBounds = element.GetBounds();
            var massBounds = positioned.GetBounds();

            Assert.IsTrue(massBounds.Right < elementBounds.Left);
            Assert.AreEqual(elementBounds.Top, massBounds.CenterY(), 0.01);
        }

        [TestMethod()]
        public void PositionOfChargeWhenHydrogensAreRight()
        {
            // hydrogen is arbitrarily moved to ensure x/y are different from the element
            var charge = new TextOutline("+", font, emSize);
            var localHydrogen = hydrogen.Translate(10, 10);
            var positioned = atomGenerator.PositionChargeLabel(1, HydrogenPosition.Right, charge, element,
                    localHydrogen);

            var hydrogenBounds = localHydrogen.GetBounds();
            var chargeBounds = positioned.GetBounds();

            Assert.IsTrue(chargeBounds.Left > hydrogenBounds.Left);
            Assert.AreEqual(hydrogenBounds.Top, chargeBounds.CenterY(), 0.01);
        }

        [TestMethod()]
        public void PositionOfChargeWhenNoHydrogensAreRight()
        {
            // hydrogen is arbitrarily moved to ensure x/y are different from the element
            var charge = new TextOutline("+", font, emSize);
            var localHydrogen = hydrogen.Translate(10, 10);

            var positioned = atomGenerator.PositionChargeLabel(0, HydrogenPosition.Right, charge, element,
                    localHydrogen);

            var elementBounds = element.GetBounds();
            var chargeBounds = positioned.GetBounds();

            Assert.IsTrue(chargeBounds.Left > elementBounds.Left);
            Assert.AreEqual(elementBounds.Top, chargeBounds.CenterY(), 0.01);
        }

        [TestMethod()]
        public void PositionOfChargeWhenHydrogensAreLeft()
        {
            // hydrogen is arbitrarily moved to ensure x/y are different from the element
            var charge = new TextOutline("+", font, emSize);
            var localHydrogen = hydrogen.Translate(10, 10);
            var positioned = atomGenerator.PositionChargeLabel(1, HydrogenPosition.Left, charge, element,
                    localHydrogen);

            var elementBounds = element.GetBounds();
            var chargeBounds = positioned.GetBounds();

            Assert.IsTrue(chargeBounds.Left > elementBounds.Left);
            Assert.AreEqual(elementBounds.Top, chargeBounds.CenterY(), 0.01);
        }

        [TestMethod()]
        public void PositionOfChargeWhenHydrogensAreBelow()
        {
            var charge = new TextOutline("+", font, emSize);
            var positioned = atomGenerator.PositionChargeLabel(1, HydrogenPosition.Below, charge, element,
                    hydrogen.Translate(0, 5));

            var elementBounds = element.GetBounds();
            var chargeBounds = positioned.GetBounds();

            Assert.IsTrue(chargeBounds.Left > elementBounds.Left);
            Assert.AreEqual(elementBounds.Top, chargeBounds.CenterY(), 0.01);
        }

        [TestMethod()]
        public void PositionOfChargeWhenTwoHydrogensAreAbove()
        {
            // hydrogen is arbitrarily moved to ensure x/y are different from the element
            var charge = new TextOutline("+", font, emSize);
            var localHydrogen = hydrogen.Translate(10, 10);
            var positioned = atomGenerator.PositionChargeLabel(2, HydrogenPosition.Above, charge, element,
                    localHydrogen);

            var hydrogenBounds = localHydrogen.GetBounds();
            var chargeBounds = positioned.GetBounds();

            Assert.IsTrue(chargeBounds.Left > hydrogenBounds.Left);
            Assert.AreEqual(hydrogenBounds.Top, chargeBounds.CenterY(), 0.01);
        }

        [TestMethod()]
        public void PositionOfChargeWhenOneHydrogenIsAbove()
        {
            // hydrogen is arbitrarily moved to ensure x/y are different from the element
            var charge = new TextOutline("+", font, emSize);
            var localHydrogen = hydrogen.Translate(10, 10);
            var positioned = atomGenerator.PositionChargeLabel(1, HydrogenPosition.Above, charge, element,
                    localHydrogen);

            var elementBounds = element.GetBounds();
            var chargeBounds = positioned.GetBounds();

            Assert.IsTrue(chargeBounds.Left > elementBounds.Left);
            Assert.AreEqual(elementBounds.Top, chargeBounds.CenterY(), 0.01);
        }

        [TestMethod()]
        public void GenerateWithNoAdjuncts()
        {
            var symbol = atomGenerator.GeneratePeriodicSymbol(7, 0, -1, 0, 0, HydrogenPosition.Right);
            Assert.AreEqual(1, symbol.GetOutlines().Count);
        }

        [TestMethod()]
        public void GenerateWithHydrogenAdjunct()
        {
            var symbol = atomGenerator.GeneratePeriodicSymbol(7, 1, -1, 0, 0, HydrogenPosition.Right);
            Assert.AreEqual(2, symbol.GetOutlines().Count);
        }

        [TestMethod()]
        public void GenerateWithHydrogenAndCountAdjunct()
        {
            var symbol = atomGenerator.GeneratePeriodicSymbol(7, 2, -1, 0, 0, HydrogenPosition.Right);
            Assert.AreEqual(3, symbol.GetOutlines().Count);
        }

        [TestMethod()]
        public void GenerateWithMassAdjunct()
        {
            var symbol = atomGenerator.GeneratePeriodicSymbol(7, 0, 15, 0, 0, HydrogenPosition.Right);
            Assert.AreEqual(2, symbol.GetOutlines().Count);
        }

        [TestMethod()]
        public void GenerateWithChargeAdjunct()
        {
            var symbol = atomGenerator.GeneratePeriodicSymbol(7, 0, -1, 1, 0, HydrogenPosition.Right);
            Assert.AreEqual(2, symbol.GetOutlines().Count);
        }

        [TestMethod()]
        public void GenerateWithRadicalAdjunct()
        {
            var symbol = atomGenerator.GeneratePeriodicSymbol(7, 0, -1, 0, 1, HydrogenPosition.Right);
            Assert.AreEqual(2, symbol.GetOutlines().Count);
        }

        [TestMethod()]
        public void HydrogenDodgesMassLabel()
        {
            var symbol = atomGenerator.GeneratePeriodicSymbol(7, 1, 15, 0, 0, HydrogenPosition.Left);
            var outlines = symbol.GetOutlines();
            Assert.AreEqual(3, outlines.Count);
            var hydrogenShape = outlines[1];
            var massShape = outlines[2];
            Assert.IsTrue(hydrogenShape.Bounds.Right < massShape.Bounds.Left);
        }

        [TestMethod()]
        public void HydrogenAndHydrogenCountDodgesMassLabel()
        {
            var symbol = atomGenerator.GeneratePeriodicSymbol(7, 2, 15, 0, 0, HydrogenPosition.Left);
            var outlines = symbol.GetOutlines();
            Assert.AreEqual(4, outlines.Count);
            var hydrogenShape = outlines[1];
            var hydrogenCountShape = outlines[2];
            var massShape = outlines[3];

            Assert.IsTrue(hydrogenShape.Bounds.Right < massShape.Bounds.Left);

            // the count subscript and mass overlap a little
            Assert.IsTrue(hydrogenCountShape.Bounds.Right > massShape.Bounds.Left);
            Assert.IsTrue(hydrogenCountShape.Bounds.Right < massShape.Bounds.Right);

            Assert.IsTrue(hydrogenShape.Bounds.Right < hydrogenCountShape.Bounds.Left);
        }

        [TestMethod()]
        public void HydrogenCountDodgesElement()
        {
            var symbol = atomGenerator.GeneratePeriodicSymbol(7, 2, -1, 0, 0, HydrogenPosition.Left);
            var outlines = symbol.GetOutlines();
            Assert.AreEqual(3, outlines.Count);
            var elementShape = outlines[0];
            var hydrogenShape = outlines[1];
            var hydrogenCountShape = outlines[2];

            Assert.IsTrue(hydrogenCountShape.Bounds.Right < elementShape.Bounds.Left);
            Assert.IsTrue(hydrogenShape.Bounds.Right < hydrogenCountShape.Bounds.Left);
        }

        [TestMethod()]
        public void HydrogenDoesNotNeedToDodge()
        {
            var symbol = atomGenerator.GeneratePeriodicSymbol(7, 1, -1, 0, 0, HydrogenPosition.Left);
            var outlines = symbol.GetOutlines();
            Assert.AreEqual(2, outlines.Count);
            var elementShape = outlines[0];
            var hydrogenShape = outlines[1];
            Assert.IsTrue(hydrogenShape.Bounds.Right < elementShape.Bounds.Left);
        }

        [TestMethod()]
        public void Anion()
        {
            Assert.AreEqual("−", StandardAtomGenerator.ChargeAdjunctText(-1, 0));
        }

        [TestMethod()]
        public void Cation()
        {
            Assert.AreEqual("+", StandardAtomGenerator.ChargeAdjunctText(1, 0));
        }

        [TestMethod()]
        public void Dianion()
        {
            Assert.AreEqual("2−", StandardAtomGenerator.ChargeAdjunctText(-2, 0));
        }

        [TestMethod()]
        public void Dication()
        {
            Assert.AreEqual("2+", StandardAtomGenerator.ChargeAdjunctText(2, 0));
        }

        [TestMethod()]
        public void Radical()
        {
            Assert.AreEqual("•", StandardAtomGenerator.ChargeAdjunctText(0, 1));
        }

        [TestMethod()]
        public void Diradical()
        {
            Assert.AreEqual("2•", StandardAtomGenerator.ChargeAdjunctText(0, 2));
        }

        [TestMethod()]
        public void DiradicalCation()
        {
            Assert.AreEqual("(2•)+", StandardAtomGenerator.ChargeAdjunctText(1, 2));
        }

        [TestMethod()]
        public void RadicalAndAnion()
        {
            Assert.AreEqual("(•)−", StandardAtomGenerator.ChargeAdjunctText(-1, 1));
        }

        [TestMethod()]
        public void AccessNullPseudoLabel()
        {
            var mock_atom = new Mock<IPseudoAtom>(); var atom = mock_atom.Object;
            mock_atom.Setup(n => n.Label).Returns((string)null);
            Assert.AreEqual("*", StandardAtomGenerator.AccessPseudoLabel(atom, "*"));
        }

        [TestMethod()]
        public void AccessEmptyPseudoLabel()
        {
            var mock_atom = new Mock<IPseudoAtom>(); var atom = mock_atom.Object;
            mock_atom.Setup(n => n.Label).Returns("");
            Assert.AreEqual("*", StandardAtomGenerator.AccessPseudoLabel(atom, "*"));
        }

        [TestMethod()]
        public void AccessRgroupPseudoLabel()
        {
            var mock_atom = new Mock<IPseudoAtom>(); var atom = mock_atom.Object;
            mock_atom.Setup(n => n.Label).Returns("R1");
            Assert.AreEqual("R1", StandardAtomGenerator.AccessPseudoLabel(atom, "*"));
        }

        [TestMethod()]
        public void NumberedRgroupSymbol()
        {
            var atomSymbol = atomGenerator.GeneratePseudoSymbol("R1", HydrogenPosition.Right);
            var shapes = atomSymbol.GetOutlines();
            Assert.AreEqual(2, shapes.Count);
        }

        [TestMethod()]
        public void RgroupSymbol2A()
        {
            var atomSymbol = atomGenerator.GeneratePseudoSymbol("R2a", HydrogenPosition.Right);
            var shapes = atomSymbol.GetOutlines();
            Assert.AreEqual(2, shapes.Count);
        }

        [TestMethod()]
        public void RgroupSymbolY()
        {
            var atomSymbol = atomGenerator.GeneratePseudoSymbol("Y1a2", HydrogenPosition.Right);
            var shapes = atomSymbol.GetOutlines();
            Assert.AreEqual(1, shapes.Count);
        }

        [TestMethod()]
        public void RgroupSymbolPrime()
        {
            var atomSymbol = atomGenerator.GeneratePseudoSymbol("R'", HydrogenPosition.Right);
            var shapes = atomSymbol.GetOutlines();
            Assert.AreEqual(2, shapes.Count);
        }

        [TestMethod()]
        public void RgroupSymbolNumberedPrime()
        {
            var atomSymbol = atomGenerator.GeneratePseudoSymbol("R2'", HydrogenPosition.Right);
            var shapes = atomSymbol.GetOutlines();
            Assert.AreEqual(3, shapes.Count);
        }

        [TestMethod()]
        public void PseudoSymbol()
        {
            var atomSymbol = atomGenerator.GeneratePseudoSymbol("Protein", HydrogenPosition.Right);
            var shapes = atomSymbol.GetOutlines();
            Assert.AreEqual(1, shapes.Count);
        }

        [TestMethod()]
        public void GeneratesRgroupPseudoAtom()
        {
            var mock_container = new Mock<IAtomContainer>(); var container = mock_container.Object;
            var mock_atom = new Mock<IPseudoAtom>(); var atom = mock_atom.Object;
            mock_atom.Setup(n => n.Label).Returns("R1");
            var atomSymbol = atomGenerator.GenerateSymbol(container, atom, HydrogenPosition.Left, new RendererModel());
            var shapes = atomSymbol.GetOutlines();
            Assert.AreEqual(2, shapes.Count);
        }

        // the mass symbol is not displayed
        [TestMethod()]
        public void GeneratesCarbon12()
        {
            var mock_container = new Mock<IAtomContainer>(); var container = mock_container.Object;
            var mock_atom = new Mock<IAtom>(); var atom = mock_atom.Object;
            mock_atom.Setup(n => n.AtomicNumber).Returns(6);
            mock_atom.Setup(n => n.MassNumber).Returns(12);
            mock_atom.Setup(n => n.ImplicitHydrogenCount).Returns(0);
            mock_atom.Setup(n => n.FormalCharge).Returns(0);
            var model = new RendererModel();
            model.SetOmitMajorIsotopes(true);
            var atomSymbol = atomGenerator.GenerateSymbol(container, atom, HydrogenPosition.Left, model);
            var shapes = atomSymbol.GetOutlines();
            Assert.AreEqual(1, shapes.Count);
        }

        [TestMethod()]
        public void GeneratesCarbon13()
        {
            var mock_container = new Mock<IAtomContainer>(); var container = mock_container.Object;
            var mock_atom = new Mock<IAtom>(); var atom = mock_atom.Object;
            mock_atom.Setup(n => n.AtomicNumber).Returns(6);
            mock_atom.Setup(n => n.MassNumber).Returns(13);
            mock_atom.Setup(n => n.ImplicitHydrogenCount).Returns(0);
            mock_atom.Setup(n => n.FormalCharge).Returns(0);
            var model = new RendererModel();
            var atomSymbol = atomGenerator.GenerateSymbol(container, atom, HydrogenPosition.Left, model);
            var shapes = atomSymbol.GetOutlines();
            Assert.AreEqual(2, shapes.Count);
        }

        [TestMethod()]
        public void NullMassNumber()
        {
            var mock_container = new Mock<IAtomContainer>(); var container = mock_container.Object;
            var mock_atom = new Mock<IAtom>(); var atom = mock_atom.Object;
            mock_atom.Setup(n => n.AtomicNumber).Returns(6);
            mock_atom.Setup(n => n.MassNumber).Returns((int?)null);
            mock_atom.Setup(n => n.ImplicitHydrogenCount).Returns(0);
            mock_atom.Setup(n => n.FormalCharge).Returns(0);
            var atomSymbol = atomGenerator.GenerateSymbol(container, atom, HydrogenPosition.Left, new RendererModel());
            var shapes = atomSymbol.GetOutlines();
            Assert.AreEqual(1, shapes.Count);
        }

        [TestMethod()]
        public void NullHydrogenCount()
        {
            var mock_container = new Mock<IAtomContainer>(); var container = mock_container.Object;
            var mock_atom = new Mock<IAtom>(); var atom = mock_atom.Object;
            mock_atom.Setup(n => n.AtomicNumber).Returns(6);
            mock_atom.Setup(n => n.MassNumber).Returns(12);
            mock_atom.Setup(n => n.ImplicitHydrogenCount).Returns((int?)null);
            mock_atom.Setup(n => n.FormalCharge).Returns(0);
            var model = new RendererModel();
            var atomSymbol = atomGenerator.GenerateSymbol(container, atom, HydrogenPosition.Left, model);
            var shapes = atomSymbol.GetOutlines();
            Assert.AreEqual(2, shapes.Count);
        }

        [TestMethod()]
        public void NullFormatCharge()
        {
            var mock_container = new Mock<IAtomContainer>(); var container = mock_container.Object;
            var mock_atom = new Mock<IAtom>(); var atom = mock_atom.Object;
            mock_atom.Setup(n => n.AtomicNumber).Returns(6);
            mock_atom.Setup(n => n.MassNumber).Returns(12);
            mock_atom.Setup(n => n.ImplicitHydrogenCount).Returns(0);
            mock_atom.Setup(n => n.FormalCharge).Returns((int?)null);
            var model = new RendererModel();
            var atomSymbol = atomGenerator.GenerateSymbol(container, atom, HydrogenPosition.Left, model);
            var shapes = atomSymbol.GetOutlines();
            Assert.AreEqual(2, shapes.Count);
        }

        [TestMethod()]
        public void UnpairedElectronsAreAccessed()
        {
            var mock_container = new Mock<IAtomContainer>(); var container = mock_container.Object;
            var mock_atom = new Mock<IAtom>(); var atom = mock_atom.Object;
            mock_atom.Setup(n => n.AtomicNumber).Returns(6);
            mock_atom.Setup(n => n.MassNumber).Returns(12);
            mock_atom.Setup(n => n.ImplicitHydrogenCount).Returns(0);
            mock_atom.Setup(n => n.FormalCharge).Returns(0);
            mock_container.Setup(n => n.GetConnectedSingleElectrons(atom)).Returns(new[] { new Default.SingleElectron() });
            var model = new RendererModel();
            var atomSymbol = atomGenerator.GenerateSymbol(container, atom, HydrogenPosition.Left, model);
            var shapes = atomSymbol.GetOutlines();
            Assert.AreEqual(3, shapes.Count);
            mock_container.Verify(n => n.GetConnectedSingleElectrons(atom), Times.Once);
        }
    }
}
