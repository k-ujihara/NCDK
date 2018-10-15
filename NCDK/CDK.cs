/* Copyright (C) 2010  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All I ask is that proper credit is given for my work, which includes
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

namespace NCDK
{
    /// <summary>
    /// Helper class to provide general information about this CDK library.
    /// </summary>
    // @cdk.module core
    // @cdk.githash
    public static class CDK
    {
        /// <summary>
        /// Returns the version of this CDK library.
        /// </summary>
        /// <returns>The library version</returns>
        public static string Version => typeof(CDK).Assembly.GetName().Version.ToString();

        private static readonly object syncLock = new object();

        private static Config.AtomTypeFactory atomTypeFactory = null;
        internal static Config.AtomTypeFactory JmolAtomTypeFactory
        {
            get
            {
                if (atomTypeFactory == null)
                    lock (syncLock)
                    {
                        if (atomTypeFactory == null)
                            atomTypeFactory = Config.AtomTypeFactory.GetInstance("NCDK.Config.Data.jmol_atomtypes.txt", Silent.ChemObjectBuilder.Instance);
                    }
                return atomTypeFactory;
            }
        }

        private static Config.AtomTypeFactory cdkAtomTypeFactory = null;
        internal static Config.AtomTypeFactory CdkAtomTypeFactory
        {
            get
            {
                if (cdkAtomTypeFactory == null)
                    lock (syncLock)
                    {
                        if (cdkAtomTypeFactory == null)
                            cdkAtomTypeFactory = Config.AtomTypeFactory.GetInstance("NCDK.Dict.Data.cdk-atom-types.owl", Silent.ChemObjectBuilder.Instance);
                    }
                return cdkAtomTypeFactory;
            }
        }

        private static Config.AtomTypeFactory structgenAtomTypeFactory = null;
        internal static Config.AtomTypeFactory StructgenAtomTypeFactory
        {
            get
            {
                if (structgenAtomTypeFactory == null)
                    lock (syncLock)
                    {
                        if (structgenAtomTypeFactory == null)
                            structgenAtomTypeFactory = Config.AtomTypeFactory.GetInstance("NCDK.Config.Data.structgen_atomtypes.xml", Silent.ChemObjectBuilder.Instance);
                    }
                return structgenAtomTypeFactory;
            }
        }

        private static Smiles.SmilesParser silentSmilesParser = null;
        internal static Smiles.SmilesParser SilentSmilesParser
        {
            get
            {
                if (silentSmilesParser == null)
                    lock (syncLock)
                    {
                        if (silentSmilesParser == null)
                            silentSmilesParser = new Smiles.SmilesParser(Silent.ChemObjectBuilder.Instance);
                    }
                return silentSmilesParser;
            }
        }

        private static Tools.SaturationChecker saturationChecker;

        internal static Tools.SaturationChecker SaturationChecker
        {
            get
            {
                if (saturationChecker == null)
                    lock (syncLock)
                    {
                        if (saturationChecker == null)
                            saturationChecker = new Tools.SaturationChecker();
                    }
                return saturationChecker;
            }
        }

        private static Tools.ILonePairElectronChecker lonePairElectronChecker;

        internal static Tools.ILonePairElectronChecker LonePairElectronChecker
        {
            get
            {
                if (lonePairElectronChecker == null)
                    lock (syncLock)
                    {
                        if (lonePairElectronChecker == null)
                            lonePairElectronChecker = new Tools.LonePairElectronChecker();
                    }
                return lonePairElectronChecker;
            }
        }

        private static AtomTypes.CDKAtomTypeMatcher cdkAtomTypeMatcher;

        internal static AtomTypes.CDKAtomTypeMatcher CdkAtomTypeMatcher
        {
            get
            {
                if (cdkAtomTypeMatcher == null)
                    lock (syncLock)
                    {
                        if (cdkAtomTypeMatcher == null)
                            cdkAtomTypeMatcher = AtomTypes.CDKAtomTypeMatcher.GetInstance(Silent.ChemObjectBuilder.Instance);
                    }
                return cdkAtomTypeMatcher;
            }
        }
    }
}
