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
using NCDK.Numerics;
using NCDK.Renderers.Elements;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace NCDK.Renderers.Generators
{
    /// <summary>
    /// Base class for test classes that test <see cref="IAtomContainerGenerator"/>s.
    /// </summary>
    // @author     maclean
    // @cdk.module test-renderbasic
    [TestClass()]
    public abstract class AbstractGeneratorTest<T> where T : IChemObject
    {
        protected IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;
        protected RendererModel model;
        protected ElementUtility elementUtil;
        protected BasicSceneGenerator sceneGenerator;
        private IGenerator<T> testedGenerator;

        /// <summary>
        /// Sets the <see cref="IGenerator"/> that is being tested.
        /// </summary>
        /// <param name="generator"></param>
        public void SetTestedGenerator(IGenerator<T> generator) 
        {
            testedGenerator = generator;
        }

        /// <summary>
        /// Sets up the model and transform.
        /// Call from the 'Before' method in subclasses.
        /// </summary>
        public AbstractGeneratorTest()
        {
            if (model != null) return; // things are already set up
            model = new RendererModel();
            elementUtil = new ElementUtility();
            sceneGenerator = new BasicSceneGenerator();
        }

        public IList<IRenderingElement> GetAllSimpleElements(IGenerator<IAtomContainer> generator, IAtomContainer container)
        {
            IRenderingElement root = generator.Generate(container, model);
            return elementUtil.GetAllSimpleElements(root);
        }

        /// <summary>
        /// Implement this in derived classes, either returning null if no custom canvas is desired, or with a Rectangle with the appropriate size.
        /// </summary>
        /// <returns>a Rectangle representing a custom drawing canvas</returns>
        public abstract Rect? GetCustomCanvas();

        /// <summary>
        /// Gets the default canvas for drawing on.
        /// </summary>
        /// <returns>a Rectangle representing the default drawing canvas</returns>
        public Rect GetDefaultCanvas()
        {
            return new Rect(0, 0, 100, 100);
        }

        /// <summary>
        /// Gets the transform to be used when converting rendering elements into graphical objects.
        /// </summary>
        /// <returns>the affine transform based on the current canvas</returns>
        public Transform GetTransform()
        {
            var canvas = GetCustomCanvas() ?? this.GetDefaultCanvas();
            return MakeTransform(canvas);
        }

        /// <summary>
        /// Uses a rectangle canvas to work out how to translate from model space to screen space.
        /// </summary>
        /// <param name="canvas">the rectangular canvas to draw on</param>
        /// <returns>the transform needed to translate to screen space</returns>
        public Transform MakeTransform(Rect canvas)
        {
            var transform =new TranslateTransform(canvas.X + canvas.Width / 2, canvas.Y + canvas.Height / 2);
            // TODO : include scale and zoom!
            return transform;
        }

        /// <summary>
        /// A utility method to determine the length of a line element (in model space)
        /// </summary>
        /// <param name="line">the line element to determine the length of</param>
        /// <returns>a length</returns>
        public static double Length(LineElement line)
        {
            return Distance(line.FirstPoint, line.SecondPoint);
        }

        /// <summary>
        /// The distance between two coordinates.
        /// </summary>
        /// <param name="firstCoordinate">the first x coordinate</param>
        /// <param name="secondCoordinate">the second x coordinate</param>
        /// <returns>the distance</returns>
        public static double Distance(Point firstCoordinate, Point secondCoordinate)
        {
            return Math.Sqrt(Math.Pow(secondCoordinate.X - firstCoordinate.X, 2) + Math.Pow(secondCoordinate.Y - firstCoordinate.Y, 2));
        }

        /// <summary>
        /// Find the center of a list of elements.
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        public static Point Center(IList<IRenderingElement> elements)
        {
            var center = new Point(0, 0);
            double counter = 0;
            foreach (var element in elements)
            {
                switch (element)
                {
                    case OvalElement o:
                        {
                            center.X += o.Coord.X;
                            center.Y += o.Coord.Y;
                            counter++;
                        }
                        break;
                    case LineElement e:
                        {
                            LineElement l = e;
                            center.X += l.FirstPoint.X;
                            center.X += l.SecondPoint.X;
                            center.Y += l.FirstPoint.Y;
                            center.Y += l.SecondPoint.Y;
                            counter += 2;
                        }
                        break;
                    default:
                        break;
                }
            }
            if (counter > 0)
            {
                return new Point(center.X / counter, center.Y / counter);
            }
            else
            {
                return new Point(0, 0);
            }
        }

        /// <summary>
        /// Makes a single carbon atom, centered on the origin.
        /// </summary>
        /// <returns>an atom container with a single atom</returns>
        public IAtomContainer MakeSingleAtom()
        {
            IAtomContainer container = builder.NewAtomContainer();
            container.Atoms.Add(builder.NewAtom("C", new Vector2(0, 0)));
            return container;
        }

        /// <summary>
        /// Makes a single atom of a particular element, centered on the origin.
        /// </summary>
        /// <param name="elementSymbol"></param>
        /// <returns>an atom container with a single atom</returns>
        public IAtomContainer MakeSingleAtom(string elementSymbol)
        {
            IAtomContainer container = builder.NewAtomContainer();
            container.Atoms.Add(builder.NewAtom(elementSymbol, new Vector2(0, 0)));
            return container;
        }

        public IAtomContainer MakeMethane()
        {
            IAtomContainer methane = builder.NewAtomContainer();
            methane.Atoms.Add(builder.NewAtom("C", new Vector2(0, 0)));
            methane.Atoms.Add(builder.NewAtom("H", new Vector2(1, 1)));
            methane.Atoms.Add(builder.NewAtom("H", new Vector2(1, -1)));
            methane.Atoms.Add(builder.NewAtom("H", new Vector2(-1, 1)));
            methane.Atoms.Add(builder.NewAtom("H", new Vector2(-1, -1)));
            return methane;
        }

        /// <summary>
        /// Makes a single C-C bond aligned horizontally. The endpoints are (0, -1) and (0, 1) in model space.
        /// </summary>
        /// <returns>an atom container with a single C-C bond</returns>
        public IAtomContainer MakeSingleBond()
        {
            IAtomContainer container = builder.NewAtomContainer();
            container.Atoms.Add(builder.NewAtom("C", new Vector2(0, -1)));
            container.Atoms.Add(builder.NewAtom("C", new Vector2(0, 1)));
            container.AddBond(container.Atoms[0], container.Atoms[1], BondOrder.Single);
            return container;
        }

        public IAtomContainer MakeCCC()
        {
            IAtomContainer container = builder.NewAtomContainer();
            container.Atoms.Add(builder.NewAtom("C", new Vector2(-1, -1)));
            container.Atoms.Add(builder.NewAtom("C", new Vector2(0, 0)));
            container.Atoms.Add(builder.NewAtom("C", new Vector2(1, -1)));
            container.AddBond(container.Atoms[0], container.Atoms[1], BondOrder.Single);
            container.AddBond(container.Atoms[1], container.Atoms[2], BondOrder.Single);
            return container;
        }

        /// <summary>
        /// Make a square (sort-of cyclobutane, without any hydrogens) centered on the origin, with a bond length of 2.
        /// </summary>
        /// <returns>four carbon atoms connected by bonds into a square</returns>
        public IAtomContainer MakeSquare()
        {
            IAtomContainer container = builder.NewAtomContainer();
            container.Atoms.Add(builder.NewAtom("C", new Vector2(-1, -1)));
            container.Atoms.Add(builder.NewAtom("C", new Vector2(1, -1)));
            container.Atoms.Add(builder.NewAtom("C", new Vector2(1, 1)));
            container.Atoms.Add(builder.NewAtom("C", new Vector2(-1, 1)));
            container.AddBond(container.Atoms[0], container.Atoms[1], BondOrder.Single);
            container.AddBond(container.Atoms[0], container.Atoms[3], BondOrder.Single);
            container.AddBond(container.Atoms[1], container.Atoms[2], BondOrder.Single);
            container.AddBond(container.Atoms[2], container.Atoms[3], BondOrder.Single);
            return container;
        }
        
        /// <summary>
        /// Make a square with four different elements, to test things like atom color.
        /// </summary>
        /// <returns>an unlikely S-N-O-P square</returns>
        public IAtomContainer MakeSNOPSquare()
        {
            IAtomContainer container = builder.NewAtomContainer();
            container.Atoms.Add(builder.NewAtom("S", new Vector2(-1, -1)));
            container.Atoms.Add(builder.NewAtom("N", new Vector2(1, -1)));
            container.Atoms.Add(builder.NewAtom("O", new Vector2(1, 1)));
            container.Atoms.Add(builder.NewAtom("P", new Vector2(-1, 1)));
            container.AddBond(container.Atoms[0], container.Atoms[1], BondOrder.Single);
            container.AddBond(container.Atoms[0], container.Atoms[3], BondOrder.Single);
            container.AddBond(container.Atoms[1], container.Atoms[2], BondOrder.Single);
            container.AddBond(container.Atoms[2], container.Atoms[3], BondOrder.Single);
            return container;
        }

        [TestMethod()]
        public void TestGetParameters()
        {
            Assert.IsNotNull(this.testedGenerator, "The tested generator is not set.");
        }
    }
}
