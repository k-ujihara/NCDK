/*  Copyright (C) 2008  Arvid Berg <goglepox@users.sf.net>
 *
 *  Contact: cdk-devel@list.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using System.Windows;
using System.Windows.Media;

namespace NCDK.Renderers.Elements
{
    /// <summary>
    /// Text element as used in the chemical drawing. This can be a element symbol.
    /// </summary>
    // @cdk.module render
    // @cdk.githash
    public class TextElement : IRenderingElement
    {
        /// <summary>The coordinate where the text should be displayed.</summary>
        public readonly Point coord;

        /// <summary>The text to be displayed.</summary>
        public readonly string text;

        /// <summary>The color of the text.</summary>
        public readonly Color color;

        /// <summary>
        /// Constructs a new TextElement with the content <paramref name="text"/> to be
        /// drawn at position (x,y) in the color <paramref name="color"/>.
        /// </summary>
        /// <param name="xCoord">x coordinate where the text should be displayed</param>
        /// <param name="yCoord">y coordinate where the text should be displayed</param>
        /// <param name="text">the text to be drawn</param>
        /// <param name="color">the color of the text</param>
        public TextElement(Point coord, string text, Color color)
        {
            this.coord = coord;
            this.text = text;
            this.color = color;
        }

        /// <inheritdoc/>
        public virtual void Accept(IRenderingVisitor visotor)
        {
            visotor.Visit(this);
        }
    }
}
