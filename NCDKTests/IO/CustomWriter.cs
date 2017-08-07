/* Copyright (C) 2009  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.IO.Formats;
using NCDK.IO.Listener;
using NCDK.IO.Setting;
using System;
using System.IO;

namespace NCDK.IO
{
    /// <summary>
    /// Dummy class to test the <see cref="WriterFactory"/> registerWriter functionality.
    /// </summary>
    // @cdk.module test-io
    public class CustomWriter : ChemObjectIO, IChemObjectWriter
    {
        public CustomWriter(TextWriter writer) { }
        public void Write(IChemObject obj) { }
        public override bool Accepts(Type classObject) => false;
        public void AddChemObjectIOListener(IChemObjectIOListener listener) { }
        public override IResourceFormat Format => null;
        public override SettingManager<IOSetting> IOSettings => null;
        public void RemoveChemObjectIOListener(IChemObjectIOListener listener) { }
    }
}
