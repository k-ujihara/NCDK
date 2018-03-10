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
using NCDK.Default;
using NCDK.Numerics;
using System;

namespace NCDK.Renderers
{
    [TestClass()]
    public class SymbolVisibilityTest
    {
        [TestMethod()]
        public void AnyAtom()
        {
            Assert.IsTrue(SymbolVisibility.All.Visible(null, null, null));
        }

        [TestMethod()]
        public void IupacOxygen()
        {
            Assert.IsTrue(SymbolVisibility.IupacRecommendations.Visible(new Atom("O"), Array.Empty<IBond>(), new RendererModel()));
        }

        [TestMethod()]
        public void IupacNitrogen()
        {
            Assert.IsTrue(SymbolVisibility.IupacRecommendations.Visible(new Atom("N"), Array.Empty<IBond>(), new RendererModel()));
        }

        [TestMethod()]
        public void IupacMethane()
        {
            Assert.IsTrue(SymbolVisibility.IupacRecommendations.Visible(new Atom("C"), Array.Empty<IBond>(),
                    new RendererModel()));
        }

        [TestMethod()]
        public void IupacMethylPreferred()
        {
            IAtom a1 = new Atom("C");
            IAtom a2 = new Atom("C");
            IBond bond = new Bond(a1, a2);
            a1.Point2D = new Vector2(0, 0);
            a2.Point2D = new Vector2(0, 0);
            Assert.IsTrue(SymbolVisibility.IupacRecommendations.Visible(a1, new[] { bond }, new RendererModel()));
        }

        [TestMethod()]
        public void IupacMethylAcceptable()
        {
            IAtom a1 = new Atom("C");
            IAtom a2 = new Atom("C");
            a1.ImplicitHydrogenCount = null;
            a2.ImplicitHydrogenCount = null;
            IBond bond = new Bond(a1, a2);
            a1.Point2D = new Vector2(0, 0);
            a2.Point2D = new Vector2(0, 0);
            Assert.IsFalse(SymbolVisibility.IupacRecommendationsWithoutTerminalCarbon.Visible(a1, new[] { bond }, new RendererModel()));
        }

        [TestMethod()]
        public void IupacUnlabelledCarbon()
        {
            IAtom a1 = new Atom("C");
            IAtom a2 = new Atom("C");
            IAtom a3 = new Atom("C");

            a1.Point2D = new Vector2(0, 0);
            a2.Point2D = new Vector2(0.5, -0.5);
            a3.Point2D = new Vector2(0.5, 0.5);

            IBond bond1 = new Bond(a1, a2);
            IBond bond2 = new Bond(a1, a3);

            a1.ImplicitHydrogenCount = 2;
            a2.ImplicitHydrogenCount = 3;
            a3.ImplicitHydrogenCount = 3;

            Assert.IsFalse(SymbolVisibility.IupacRecommendations.Visible(a1, new[] { bond1, bond2 }, new RendererModel()));
        }

        [TestMethod()]
        public void IupacCarbonIon()
        {
            IAtom a1 = new Atom("C");
            IAtom a2 = new Atom("C");
            IAtom a3 = new Atom("C");

            a1.Point2D = new Vector2(0, 0);
            a2.Point2D = new Vector2(0.5, -0.5);
            a3.Point2D = new Vector2(0.5, 0.5);

            IBond bond1 = new Bond(a1, a2);
            IBond bond2 = new Bond(a1, a3);

            a1.FormalCharge = +1;
            a1.ImplicitHydrogenCount = 1;
            a2.ImplicitHydrogenCount = 3;
            a3.ImplicitHydrogenCount = 3;

            Assert.IsTrue(SymbolVisibility.IupacRecommendations
                    .Visible(a1, new[] { bond1, bond2 }, new RendererModel()));
        }

        [TestMethod()]
        public void IupacCarbonParallel()
        {
            IAtom a1 = new Atom("C");
            IAtom a2 = new Atom("C");
            IAtom a3 = new Atom("C");

            a1.Point2D = new Vector2(0, 0);
            a2.Point2D = new Vector2(0, -0.5);
            a3.Point2D = new Vector2(0, 0.5);

            IBond bond1 = new Bond(a1, a2);
            IBond bond2 = new Bond(a1, a3);

            a1.ImplicitHydrogenCount = 2;
            a2.ImplicitHydrogenCount = 3;
            a3.ImplicitHydrogenCount = 3;

            var model = new RendererModel();
            Assert.IsTrue(SymbolVisibility.IupacRecommendations.Visible(a1, new[] { bond1, bond2 }, model));
        }

        // produces an NaN internally
        [TestMethod(), Ignore()] // Multiple Group Sgroup rendering can have zero length C-C bonds (e.g. overlaid coords), we don't want to show the symbols
        public void IupacCarbonCornerCase()
        {
            IAtom a1 = new Atom("C");
            IAtom a2 = new Atom("C");
            IAtom a3 = new Atom("C");

            a1.Point2D = new Vector2(0, 0);
            a2.Point2D = new Vector2(0, 0);
            a3.Point2D = new Vector2(0, 0);

            IBond bond1 = new Bond(a1, a2);
            IBond bond2 = new Bond(a1, a3);

            a1.ImplicitHydrogenCount = 2;
            a2.ImplicitHydrogenCount = 3;
            a3.ImplicitHydrogenCount = 3;

            Assert.IsTrue(SymbolVisibility.IupacRecommendations
                    .Visible(a1, new[] { bond1, bond2 }, new RendererModel()));
        }

        [TestMethod()]
        public void CarbonIsotope()
        {
            IAtom a1 = new Atom("C");
            IAtom a2 = new Atom("C");
            IAtom a3 = new Atom("C");

            a1.Point2D = new Vector2(0, 0);
            a2.Point2D = new Vector2(0.5, -0.5);
            a3.Point2D = new Vector2(0.5, 0.5);

            IBond bond1 = new Bond(a1, a2);
            IBond bond2 = new Bond(a1, a3);

            a1.MassNumber = 13;

            a1.ImplicitHydrogenCount = 2;
            a2.ImplicitHydrogenCount = 3;
            a3.ImplicitHydrogenCount = 3;

            Assert.IsTrue(SymbolVisibility.IupacRecommendations
                    .Visible(a1, new[] { bond1, bond2 }, new RendererModel()));
        }

        [TestMethod()]
        public void EthaneNonTerminal()
        {
            IAtom a1 = new Atom("C");
            IAtom a2 = new Atom("C");

            a1.Point2D = new Vector2(0, 0);
            a2.Point2D = new Vector2(0.5, -0.5);

            IBond bond1 = new Bond(a1, a2);

            a1.ImplicitHydrogenCount = 3;
            a2.ImplicitHydrogenCount = 3;

            Assert.IsTrue(SymbolVisibility.IupacRecommendationsWithoutTerminalCarbon.Visible(a1, new[] { bond1 }, new RendererModel()));
        }

        [TestMethod()]
        public void AlwaysDisplayCharges()
        {
            IAtom a1 = new Atom("CH-");
            IAtom a2 = new Atom("CH2");
            IAtom a3 = new Atom("CH3");

            a1.Point2D = new Vector2(0, 0);
            a2.Point2D = new Vector2(0.5, -0.5);
            a3.Point2D = new Vector2(1, 0);

            IBond bond1 = new Bond(a1, a2, BondOrder.Double);
            IBond bond2 = new Bond(a2, a3, BondOrder.Single);

            Assert.IsTrue(SymbolVisibility.IupacRecommendationsWithoutTerminalCarbon
                                       .Visible(a1, new[] { bond1 }, new RendererModel()));
        }

        [TestMethod()]
        public void DelocalisedCarbons()
        {
            IAtom a1 = new Atom("CH");
            IAtom a2 = new Atom("CH");
            IAtom a3 = new Atom("CH");

            a1.Point2D = new Vector2(0, 0);
            a2.Point2D = new Vector2(0.5, -0.5);
            a3.Point2D = new Vector2(1, 0);

            IBond bond1 = new Bond(a1, a2, BondOrder.Unset);
            IBond bond2 = new Bond(a2, a3, BondOrder.Unset);
            bond1.IsAromatic = true;
            bond2.IsAromatic = true;
            a1.IsAromatic = true;
            a2.IsAromatic = true;
            a3.IsAromatic = true;

            Assert.IsFalse(SymbolVisibility.IupacRecommendationsWithoutTerminalCarbon
                .Visible(a2, new[] { bond1, bond2 }, new RendererModel()));
        }
    }
}
