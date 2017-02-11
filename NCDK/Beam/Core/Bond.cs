/*
 * Copyright (c) 2013, European Bioinformatics Institute (EMBL-EBI)
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 * 1. Redistributions of source code must retain the above copyright notice, this
 *    list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright notice,
 *    this list of conditions and the following disclaimer in the documentation
 *    and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * Any EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
 * Any DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON Any THEORY OF LIABILITY, WHETHER IN CONTRACT, Strict LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN Any WAY OUT OF THE USE OF THIS
 * SOFTWARE, Even IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 * The views and conclusions contained in the software and documentation are those
 * of the authors and should not be interpreted as representing official policies,
 * either expressed or implied, of the FreeBSD Project.
 */

using System;
using System.Collections.Generic;

namespace NCDK.Beam
{
    /// <summary>
    /// Enumeration of valid <see cref="Edge"/> labels. The connections include all the
    /// valid undirected and directed bond types and <see cref="Dot"/>. Opposed to the
    /// other types, <see cref="Dot"/> indicates that two atoms are not connected.
    /// <table style="font-family: Courier, monospace;"> 
    /// <tr>
    /// <th><see cref="Bond"/></th>
    /// <th><see cref="Token"/></th>
    /// <th><see cref="Order"/></th>
    /// <th><see cref="Inverse()"/></th>
    /// </tr>
    /// <tr><td><see cref="Dot"/></td><td>.</td><td>0</td><td></td></tr>
    /// <tr><td><see cref="Implicit"/></td><td></td><td>undefined (2 or 3)</td><td></td></tr>
    /// <tr><td><see cref="Single"/></td><td>-</td><td>2</td><td></td></tr>
    /// <tr><td><see cref="Aromatic"/></td><td>:</td><td>3</td><td></td></tr>
    /// <tr><td><see cref="Double"/></td><td>=</td><td>4</td><td></td></tr>
    /// <tr><td><see cref="Triple"/></td><td>#</td><td>6</td><td></td></tr>
    /// <tr><td><see cref="Quadruple"/></td><td>$</td><td>8</td><td></td></tr>
    /// <tr><td><see cref="Up"/></td><td>/</td><td>2</td><td><see cref="Down"/></td></tr>
    /// <tr><td><see cref="Down"/></td><td>\</td><td>2</td><td><see cref="Up"/></td></tr>
    /// </table>
    /// </summary>
    /// <author>John May</author>
    /// <remarks><a href="http://www.opensmiles.org/opensmiles.html#bonds">Bonds,
    /// OpenSMILES Specification</a></remarks>
    public class Bond
    {
        /// <summary>Atoms are not bonded. </summary>
        public static readonly Bond Dot = new Bond(".", 0);

        /// <summary>Atoms are bonded by either a single or aromatic bond. </summary>
        public static readonly Bond Implicit = new Bond("", 1);

        /// <summary>An implicit bond which is delocalised. </summary>
        public static readonly Bond ImplicitAromatic = new Bond("", 1);

        /// <summary>Atoms are bonded by a single pair of electrons. </summary>
        public static readonly Bond Single = new Bond("-", 1);

        /// <summary>Atoms are bonded by two pairs of electrons. </summary>
        public static readonly Bond Double = new Bond("=", 2);

        /// <summary>A double bond which is delocalised. </summary>
        public static readonly Bond DoubleAromatic = new Bond("=", 2);

        /// <summary>Atoms are bonded by three pairs of electrons. </summary>
        public static readonly Bond Triple = new Bond("#", 3);

        /// <summary>Atoms are bonded by four pairs of electrons. </summary>
        public static readonly Bond Quadruple = new Bond("$", 4);

        /// <summary>Atoms are bonded by a delocalized bond of an aromatic system. </summary>
        public static readonly Bond Aromatic = new Bond(":", 1);

        /// <summary>
		/// Directional, single or aromatic bond (currently always single). The bond
		/// is relative to each endpoint such that the second endpoint is
		/// <i>above</i> the first or the first end point is <i>below</i> the
		/// second.
		/// </summary>
        public static readonly Bond Up = new Bond_Up("/", 1);

        public static IEnumerable<Bond> Values = new[]
        {
            Dot, Implicit, ImplicitAromatic, Single, Double, DoubleAromatic, Triple, Aromatic, Up,
        };

        class Bond_Up : Bond
        {
            public Bond_Up(string token, int Order)
                : base(token, Order)
            {
            }

            public override Bond Inverse()
            {
                return Down;
            }

            public override bool IsDirectional => true;
        }

        /// <summary>
        /// Directional, single or aromatic bond (currently always single). The bond
        /// is relative to each endpoint such that the second endpoint is
        /// <i>below</i> the first or the first end point is <i>above</i> the
        /// second.
        /// </summary>
        public static readonly Bond Down = new Bond_Down("\\", 1);

        class Bond_Down : Bond
        {
            public Bond_Down(string token, int Order)
                : base(token, Order)
            {
            }

            public override Bond Inverse()
            {
                return Up;
            }

            public override bool IsDirectional => true;
        }

        /// <summary>The token for the bond in the SMILES grammar. </summary>
        private readonly string token;

        private readonly int order;

        public Bond(string token, int Order)
        {
            this.token = token;
            this.order = Order;
        }

        /// <summary>
		/// The token of the bond in the SMILES grammar.
		/// </summary>
        public string Token => token;

        /// <summary>
        /// The Order of the bond.
        /// </summary>
        public int Order => order;

        /// <summary>
        /// Access the inverse of a directional bond ({@link #Up}, {@link #Down}). If
        /// a bond is non-directional the same bond is returned.
        /// </summary>
        /// <returns>inverse of the bond</returns>
        public virtual Bond Inverse()
        {
            return this;
        }

        /// <summary>
        /// Create an edge between the vertices <paramref name="u"/> and <paramref name="v"/> with this
        /// label.
        /// </summary>
        /// <example><code>Edge e = Bond.Implicit.CreateEdge(2, 3);</code></example>
        /// <param name="u">an end point of the edge</param>
        /// <param name="v">the other endpoint of the edge</param>
        /// <returns>a new edge labeled with this value</returns>
        /// <seealso cref="Edge"/>
        public Edge CreateEdge(int u, int v)
        {
            return new Edge(u, v, this);
        }

        public virtual bool IsDirectional => false;

        /// <inheritdoc/>
        public override string ToString() => token;
    }
}
