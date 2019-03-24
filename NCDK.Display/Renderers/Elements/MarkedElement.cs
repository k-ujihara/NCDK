/*
 * Copyright (c) 2015 John May <jwmay@users.sf.net>
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
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media;

namespace NCDK.Renderers.Elements
{
    /// <summary>
    /// A marked element adds meta-data (id and tags) to a CDK rendering
    /// element (or group of elements). The id should be unique per depiction.
    /// The primary use case it to be able to set the 'id' and 'class'
    /// attributes in SVG.
    /// </summary>
    /// <example>
    /// To set the mol, atom, or bond id set a string property to <see cref="IdKey"/>.
    /// Similarly, the <see cref="ClassKey"/> can be used to set the classes.
    /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Renderers.Elements.MarkedElement_Example.cs"]/*' />
    /// </example>
    public sealed class MarkedElement : IRenderingElement
    {
        public const string IdKey = nameof(MarkedElement) + "_ID";
        public const string ClassKey = nameof(MarkedElement) + "_CLS";

        readonly IRenderingElement elem;

        private MarkedElement(IRenderingElement elem)
        {
            this.elem = elem;
        }

        /// <summary>
        /// The origin of the element.
        /// </summary>
        public IChemObject Origin { get; private set; }

        /// <summary>
        /// The identifier of the tagged element.
        /// </summary>
        public string Id { get; private set; }

        private readonly List<string> classes = new List<string>(5);

        /// <summary>
        /// The classes of the element.
        /// </summary>
        public IReadOnlyCollection<string> Classes => classes;

        public void Accept(IRenderingVisitor visitor, Transform transform)
        {
            visitor.Visit(this, transform);
        }

        public void Accept(IRenderingVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// Access the element of which the id and classes apply.
        /// </summary>
        /// <returns>rendering element</returns>
        public IRenderingElement Element()
        {
            return elem;
        }

        /// <summary>
        /// Markup a rendering element with the specified classes.
        /// </summary>
        /// <param name="elem">rendering element</param>
        /// <param name="classes">classes</param>
        /// <returns>the marked element</returns>
        public static MarkedElement Markup(IRenderingElement elem, params string[] classes)
        {
            Debug.Assert(elem != null);
            var tagElem = new MarkedElement(elem);
            foreach (var cls in classes)
                tagElem.classes.Add(cls);
            return tagElem;
        }

        private static MarkedElement MarkupFinal(IRenderingElement elem, IChemObject chemObj)
        {
            Debug.Assert(elem != null);
            var tagElem = new MarkedElement(elem);
            if (chemObj != null)
            {
                tagElem.Origin = chemObj;
                tagElem.Id = chemObj.GetProperty<string>(IdKey);
                tagElem.classes.Add(chemObj.GetProperty<string>(ClassKey));
            }
            return tagElem;
        }

        /// <summary>
        /// Markup a molecule with the class <paramref name="mol"/> and optionally the ids/classes
        /// from it's properties.
        /// </summary>
        /// <param name="elem">rendering element</param>
        /// <param name="mol">molecule</param>
        /// <returns>the marked element</returns>
        public static MarkedElement Markup(IRenderingElement elem, IAtomContainer mol)
        {
            Debug.Assert(elem != null);
            var tagElem = MarkupFinal(elem, mol);
            tagElem.classes.Add("mol");
            return tagElem;
        }

        /// <summary>
        /// Markup a atom with the class 'atom' and optionally the ids/classes
        /// from it's properties.
        /// </summary>
        /// <param name="elem">rendering element</param>
        /// <param name="atom">atom</param>
        /// <returns>the marked element</returns>
        public static MarkedElement Markup(IRenderingElement elem, IAtom atom)
        {
            if (elem == null)
                return null;
            var tagElem = MarkupFinal(elem, atom);
            tagElem.classes.Add("atom");
            return tagElem;
        }

        /// <summary>
        /// Markup a bond with the class 'bond' and optionally the ids/classes
        /// from it's properties.
        /// </summary>
        /// <param name="elem">rendering element</param>
        /// <param name="bond">bond</param>
        /// <returns>the marked element</returns>
        public static MarkedElement Markup(IRenderingElement elem, IBond bond)
        {
            if (elem == null)
                throw new ArgumentNullException(nameof(elem));
            var tagElem = MarkupFinal(elem, bond);
            tagElem.classes.Add("bond");
            return tagElem;
        }
    }
}
