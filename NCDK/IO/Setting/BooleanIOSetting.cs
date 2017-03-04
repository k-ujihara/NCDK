/* Copyright (C) 2003-2007  The CDK Development Team
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 *  This library is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public
 *  License as published by the Free Software Foundation; either
 *  version 2.1 of the License, or (at your option) any later version.
 *
 *  This library is distributed in the hope that it will be useful,
 *  but WITHOUT Any WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 *  Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public
 *  License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

namespace NCDK.IO.Setting
{
    /// <summary>
    /// An class for a reader setting which must be of type string.
    ///
    // @cdk.module io
    // @cdk.githash
    ///
    // @author Egon Willighagen <egonw@sci.kun.nl>
    /// </summary>
    public class BooleanIOSetting : IOSetting
    {

        public BooleanIOSetting(string name, Importance level, string question, string defaultSetting)
            : base(name, level, question, defaultSetting)
        { }

        private string setting;

        /// <summary>
        /// Sets the setting for a certain question. The setting
        /// is a bool, and it accepts only "true" and "false".
        /// </summary>
        public override string Setting
        {
            get
            {
                return setting;
            }

            set
            {
                if (value.Equals("true") || value.Equals("false"))
                {
                    this.setting = value;
                }
                else if (value.Equals("True") || value.Equals("yes") || value.Equals("y"))
                {
                    this.setting = "true";
                }
                else if (value.Equals("False") || value.Equals("no") || value.Equals("n"))
                {
                    this.setting = "false";
                }
                else
                {
                    throw new CDKException("Setting " + value + " is not a bool.");
                }
            }
        }

        public bool IsSet => string.Equals(setting, "true");
    }
}
