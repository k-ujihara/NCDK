/* Copyright (C) 2010  Gilleain Torrance <gilleain.torrance@gmail.com>
 *               2011  Egon Willighagen <egonw@users.sf.net>
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
using System.Windows;

namespace NCDK.Renderers.Generators
{
    /// <summary>
    /// Test the <see cref="BasicBondGenerator"/>.
    /// </summary>
    // @author     maclean
    // @cdk.module test-renderbasic
    [TestClass()]
    public class BasicGeneratorTest : AbstractGeneratorTest<IAtomContainer>
    {
        private BasicGenerator generator;

        public override Rect? GetCustomCanvas()
        {
            return null;
        }

        public BasicGeneratorTest()
            : base()
        {
            this.generator = new BasicGenerator();
            model.RegisterParameters(generator);
            base.SetTestedGenerator(generator);
        }

        [TestMethod()]
        public void TestSingleAtom()
        {
            IAtomContainer singleAtom = MakeSingleAtom();

            // nothing should be made
            var root = generator.Generate(singleAtom, model);
            var elements = elementUtil.GetAllSimpleElements(root);
            Assert.AreEqual(1, elements.Count);
        }

        [TestMethod()]
        public void TestSingleBond()
        {
            IAtomContainer container = MakeSingleBond();

            // generate the single line element
            var root = generator.Generate(container, model);
            var elements = elementUtil.GetAllSimpleElements(root);
            Assert.AreEqual(1, elements.Count);

            // test that the endpoints are distinct
            var line = (Elements.LineElement)elements[0];
            Assert.AreNotSame(0, AbstractGeneratorTest<IAtomContainer>.Length(line));
        }

        [TestMethod()]
        public void TestSquare()
        {
            IAtomContainer square = MakeSquare();

            // generate all four bonds
            var root = generator.Generate(square, model);
            var elements = elementUtil.GetAllSimpleElements(root);
            Assert.AreEqual(4, elements.Count);

            // test that the center is at the origin
            Assert.AreEqual(new Point(0, 0), Center(elements));
        }
    }
}
