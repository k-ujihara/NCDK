/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@lists.sourceforge.net
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

namespace NCDK.Silent
{
    /// <summary>
    /// Helper class to test the functionality of the <see cref="ChemObjectTestHelper"/>.
    /// </summary>
    // @cdk.module test-silent
    public class ChemObjectListener : IChemObjectListener
    {
        public bool Changed { get; set; }
        private ChemObjectChangeEventArgs evt;

        public ChemObjectListener()
        {
            Changed = false;
            evt = null;
        }

        public void OnStateChanged(ChemObjectChangeEventArgs evt)
        {
            Changed = true;
            this.evt = evt;
        }

        public void Reset()
        {
            Changed = false;
            evt = null;
        }

        public ChemObjectChangeEventArgs Event => evt;
    }
}
