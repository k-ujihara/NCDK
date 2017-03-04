/*
 * Copyright (C) 2012 John May <jwmay@users.sf.net>
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
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace NCDK.IO.Setting
{
    /// <summary>
    /// Provides dynamic management of settings. This
    /// was created with the intention of managing <see cref="IOSetting"/>'s for {@link org.openscience.cdk.io.IChemObjectIO}
    /// however it could be recycled for other purposes where dynamic settings are required.
    /// Settings are stored in a <see cref="IDictionary"/> using the name of the setting as the key. The name is
    /// normalised (lowercase and whitespace removal) to allow 'fuzzy' setting access. This means
    /// that character case differences do not affect the retrieval of objects.
    /// <p/>
    /// <b>Usage:</b>
    /// <code>{@code
    /// // create the manager and add a setting
    /// SettingManager<IOSetting>   manager = new SettingManager<IOSetting>();
    /// manager.Add(new BooleanIOSetting("Sample", IOSetting.Medium, "This is a sample?", "true"));
    ///
    /// // check the setting is present (case insensitive
    /// if(manager.Has("sample")) {
    ///
    ///      // access requiring multiple lines of code
    ///      BooleanIOSetting setting = manager["sample"];
    ///      string           v1      = setting.Setting;
    ///
    ///      // single line access (useful for conditional statements)
    ///      string v2 = manager["sample", typeof(BooleanIOSetting]).Setting;
    ///
    /// }
    /// }</code>
    ///
    // @param <T> the type of setting that will be managed (e.g. IOSetting).
    ///
    // @author       John May
    // @cdk.module   io
    // @cdk.githash
    // @cdk.created  20.03.2012
    // @see          ISetting
    // @see          IOSetting
    /// </summary>
    public class SettingManager<T> where T : ISetting
    {
        /// <summary>
        /// Uses to remove white space from names.
        /// </summary>
        /// <seealso cref="MakeKey(ISetting)"/>
        /// <seealso cref="MakeKey(string)"/>
        private static readonly Regex WHITE_SPACE = new Regex("\\s+", RegexOptions.Compiled);

        /// <summary>
        /// Settings are stored in a map of name -> instance.
        /// </summary>
        private IDictionary<string, T> settings = new Dictionary<string, T>(3);

        /// <summary>
        /// Generate a simple key for the given name. This method normalises the name by
        /// converting to lower case and replacing spaces with '.' (e.g. "Buffer Size" is
        /// converted to "buffer.size").
        /// </summary>
        /// <param name="name">the name of a setting</param>
        /// <returns>keyed setting name</returns>
        private static string MakeKey(string name)
        {
            return WHITE_SPACE.Replace(name, ".").ToLowerInvariant();
        }

        /// <summary>
        /// Generate a simple key for the given setting. This method is a convenience
        /// method for <see cref="MakeKey(string)"/>
        /// </summary>
        /// <param name="setting">setting the setting to which a key will be generated for</param>
        /// <returns>the keyed name for the setting</returns>
        private static string MakeKey(ISetting setting)
        {
            return MakeKey(setting.Name);
        }

        /// <summary>
        /// Add a setting to the manager and return the instance to use. If a 'new' setting is added
        /// to the manager which matches the name and class of an previously added 'original' setting,
        /// the original setting will be returned. Otherwise the new setting is returned. This allows
        /// the add to be used in assignments as follows:
        ///
        /// <code>{@code
        ///
        /// SettingManager   manager  = new SettingManager();
        /// BooleanIOSetting setting1 = manager.Add(new BooleanIOSetting("use.3d", ...));
        /// BooleanIOSetting setting2 = manager.Add(new BooleanIOSetting("use.3d", ...));
        ///
        /// // setting1 == setting2 and so changing a field in setting1 will also change the field
        /// // in setting2
        ///
        /// }</code>
        ///
        /// If the names are not equal or the names are equal but the classes are not the new setting
        /// is added and returned.
        ///
        /// <param name="setting">the setting to add</param>
        /// <returns>usable setting</returns>
        /// </summary>
        public TT Add<TT>(TT setting) where TT : T
        {
            string key = MakeKey(setting);

            if (settings.ContainsKey(key))
            {
                T instance = settings[key];
                if (instance.GetType() == setting.GetType())
                {
                    return (TT)instance;
                }
            }

            // we could not add if we have a clash, but actual it might be useful
            // to 'override' a setting in sub classes with a new one
            settings[MakeKey(setting)] = setting;

            return setting;
        }

        /// <summary>
        /// Access the setting stored for given name. If not setting is found the provided
        /// name an <see cref="InvalidParameterException"/> will be thrown. The method is generic
        /// to allow simplified access to settings. This however means that if the incorrect
        /// type is provided a <see cref="ClassCastException"/> may be thrown.
        /// <p/>
        /// <code>{@code
        /// SettingManager manager = ...;
        /// manger.Add(new BooleanIOSetting("name", ...));
        /// <p/>
        /// BooleanIOSetting setting = manager["Name"]; // okay
        /// OptionIOSetting setting  = manager["Name"]; // class cast exception
        /// }</code>
        ///
        /// <param name="name">name of the setting to retrieve</param>
        // @param <S>  type that will be return
        ///
        /// <returns>instance of the setting for the provided name</returns>
        ///
        /// <seealso cref="Get(string, Class)"/>
        /// </summary>
        public T this[string name]
        {
            get
            {
                string key = MakeKey(name);
                if (settings.ContainsKey(key)) return settings[key];
                throw new ArgumentException("No setting found for name " + name + "(key=" + key + ") "
                        + "available settings are: " + settings.Keys);
            }
        }

        /// <summary>
        /// Determines whether the manager currently holds a setting of
        /// the provided name.
        ///
        /// <param name="name">name of the setting</param>
        ///
        /// <returns>whether the manager currently contains the desired setting</returns>
        /// </summary>
        public bool Has(string name)
        {
            return settings.ContainsKey(MakeKey(name));
        }

        /// <summary>
        /// Access a collection of all settings in the manager.
        ///
        /// <returns>collection of managed settings</returns>
        /// </summary>
        public ICollection<T> Settings => settings.Values;
    }
}
