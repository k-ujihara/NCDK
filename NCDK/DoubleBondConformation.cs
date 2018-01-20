/* Copyright (C) 2012  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
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
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using System.Reflection;
using static NCDK.DoubleBondConformation;

namespace NCDK
{
    /// <summary>
    /// Enumeration that defines the two possible values for this stereochemistry type.
    /// </summary>
    [Obfuscation(ApplyToMembers = true, Exclude = true)]
    public enum DoubleBondConformation
    {
        Unset = 0,

        /// <summary>Z-form</summary>
        Together,
        
		/// <summary>E-form</summary>
        Opposite,
    }

    public static class DoubleBondConformationTools
    {
        public static bool IsUnset(this DoubleBondConformation value)
        {
            return value == Unset;
        }

        /// <summary>
        /// Invert this conformation.
        /// <see cref="Together"/>.Invert() = <see cref="Opposite"/>, <see cref="Opposite"/>.Invert() = <see cref="Together"/>.
        /// </summary>
        /// <returns>the inverse conformation</returns>
        public static DoubleBondConformation Invert(this DoubleBondConformation value)
            => value == Together ? Opposite : Together;

        public static DoubleBondConformation ToConformation(this StereoElement.Configuration configure)
        {
            switch (configure)
            {
                case StereoElement.Configuration.Together:
                    return Together;
                case StereoElement.Configuration.Opposite:
                    return Opposite;
                default:
                    throw new System.ArgumentException("Cannot map config to enum: " + configure);
            }
        }

        public static StereoElement.Configuration ToConfiguration(this DoubleBondConformation conformation)
        {
            switch (conformation)
            {
                case DoubleBondConformation.Together:
                    return StereoElement.Configuration.Together;
                case DoubleBondConformation.Opposite:
                    return StereoElement.Configuration.Opposite;
                default:
                    throw new System.ArgumentException("Cannot map enum to config: " + conformation);
            }
        }
    }
}
