/* Copyright (C) 2009  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@list.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
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
namespace NCDK.Renderers.Generators.Parameters
{
    /// <summary>
    /// Abstract class to provide the base functionality for <see cref="IGeneratorParameter{T}"/> implementations.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    // @cdk.module  render
    // @cdk.githash
    public abstract class AbstractGeneratorParameter<T> : IGeneratorParameter<T> 
    {
        private T parameterSetting;

        /// <summary>
        /// The value for this parameter.
        /// It must provide a reasonable default when no other value has been set.
        /// </summary>
        public virtual T Value
        {
            get
            {
                if (this.parameterSetting == null)
                    return Default;
                else
                    return this.parameterSetting;
            }
            set
            {
                this.parameterSetting = value;
            }
        }

        public abstract T Default { get; }
    }
}
