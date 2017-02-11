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
    /**
     * An interface for reader settings. It is subclassed by implementations,
     * one for each type of field, e.g. IntReaderSetting.
     *
     * @cdk.module io
     * @cdk.githash
     *
     * @author Egon Willighagen <egonw@sci.kun.nl>
     */
    public abstract class IOSetting : ISetting
    {
        public struct Importance
        {
            private int ordinal;
            public int Ordinal => ordinal;
            public Importance(int ordinal)
            {
                this.ordinal = ordinal;
            }
            public static readonly Importance High = new Importance(0);
            public static readonly Importance Medium = new Importance(1);
            public static readonly Importance Low = new Importance(2);
        }

        /**
         * The default constructor that sets this field. All textual
         * information is supposed to be English. Localization is taken care
         * off by the ReaderConfigurator.
         *
         * @param name           Name of the setting
         * @param level          Level at which question is asked
         * @param question       Question that is poped to the user when the
         *                       ReaderSetting needs setting
         * @param defaultSetting The default setting, used if not overwritten
         *                       by a user
         */
        public IOSetting(string name, Importance level, string question, string defaultSetting)
        {
            Level = level;
            Name = name;
            Question = question;
            Setting = defaultSetting;
        }

        public virtual string Name { get; protected set; }
        public virtual string Question { get; protected set; }
        public virtual string DefaultSetting => Setting;
        public virtual Importance Level { get; protected set; }

        /// <summary>
        /// The setting for a certain question. It will throw a CDKException when the setting is not valid.
        /// </summary>
        public virtual string Setting { get; set; } // by default, except all input, so no setting checking
    }
}
