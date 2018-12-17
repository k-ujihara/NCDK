/* Copyright (C) 2010  Gilleain Torrance <gilleain.torrance@gmail.com>
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
using NCDK.Layout;
using NCDK.Renderers.Fonts;
using NCDK.Renderers.Generators;
using System;
using System.Collections.Generic;
using System.Windows;

namespace NCDK.Renderers
{
    // @author     maclean
    // @cdk.module test-renderbasic
    [TestClass()]
    public class AtomContainerRendererTest
    {
        private IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;
        private StructureDiagramGenerator sdg = new StructureDiagramGenerator();

        public IAtomContainer Layout(IAtomContainer molecule)
        {
            sdg.Molecule = molecule;
            try
            {
                sdg.GenerateCoordinates();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
            return sdg.Molecule;
        }

        public IAtomContainer MakeSquare()
        {
            IAtomContainer square = builder.NewAtomContainer();
            square.Atoms.Add(builder.NewAtom("C"));
            square.Atoms.Add(builder.NewAtom("C"));
            square.Atoms.Add(builder.NewAtom("C"));
            square.Atoms.Add(builder.NewAtom("C"));
            square.AddBond(square.Atoms[0], square.Atoms[1], BondOrder.Single);
            square.AddBond(square.Atoms[0], square.Atoms[3], BondOrder.Single);
            square.AddBond(square.Atoms[1], square.Atoms[2], BondOrder.Single);
            square.AddBond(square.Atoms[2], square.Atoms[3], BondOrder.Single);

            return Layout(square);
        }

        [TestMethod()]
        public void TestSquareMolecule()
        {
            IAtomContainer square = MakeSquare();

            var generators = new List<IGenerator<IAtomContainer>>
            {
                new BasicSceneGenerator(),
                new BasicBondGenerator()
            };
            BasicAtomGenerator atomGenerator = new BasicAtomGenerator();
            generators.Add(atomGenerator);

            AtomContainerRenderer renderer = new AtomContainerRenderer(generators, new WPFFontManager());
            RendererModel model = renderer.GetRenderer2DModel();
            model.SetCompactShape(AtomShapeType.Oval);
            model.SetCompactAtom(true);
            model.SetKekuleStructure(true);
            model.SetShowEndCarbons(true);

            ElementUtility visitor = new ElementUtility();
            var screen = new Rect(0, 0, 100, 100);
            renderer.Setup(square, screen);
            renderer.Paint(square, visitor);

            foreach (var element in visitor.GetElements())
            {
                Assert.IsTrue(visitor.ToString(element).Contains("Line") || visitor.ToString(element).Contains("Oval"));
            }
        }
    }
}
