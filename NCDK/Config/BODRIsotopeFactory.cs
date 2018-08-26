/* Copyright (C) 2001-2007  Christoph Steinbeck <steinbeck@users.sf.net>
 *                    2013,2016  Egon Willighagen <egonw@users.sf.net>
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

using NCDK.Tools;
using System;
using System.Diagnostics;
using System.IO;

namespace NCDK.Config
{
    /// <summary>
    /// List of isotopes. Data is taken from the <see href="https://github.com/egonw/bodr">Blue Obelisk Data Repository</see>,
    /// <see href="https://github.com/egonw/bodr/releases/tag/BODR-10">version 10</see> <token>cdk-cite-BODR10</token>.
    /// The data set is described in the first Blue Obelisk paper <token>cdk-cite-Guha2006</token>.
    /// </summary>
    /// <remarks>
    /// <para>The "isotopes.dat" file that is used by this class is a binary class
    /// of this data, improving loading times over the BODR XML representation. It is created
    /// from the original BODR files using tools from the "cdk-build-util"
    /// repository.</para>
    /// </remarks>
    // @author      egonw
    // @cdk.module  core
    // @cdk.githash
    public class BODRIsotopeFactory
        : IsotopeFactory
    {
        /// <summary>
        /// A singleton instance of this class.
        /// </summary>
        /// <exception cref="System.IO.IOException">when reading of the data file did not work</exception>
        public static BODRIsotopeFactory Instance { get; } = new BODRIsotopeFactory();

        private BODRIsotopeFactory()
        {
            string configFile = "NCDK.Config.Data.isotopes.dat";
            var ins = ResourceLoader.GetAsStream(configFile);

            var buffer = new byte[8];
            ins.Read(buffer, 0, 4);
            Array.Reverse(buffer, 0, 4);
            int isotopeCount = BitConverter.ToInt32(buffer, 0);

            for (int i = 0; i < isotopeCount; i++)
            {
                int atomicNum = ins.ReadByte();
                ins.Read(buffer, 0, 2);
                Array.Reverse(buffer, 0, 2);
                int massNum = BitConverter.ToInt16(buffer, 0);
                ins.Read(buffer, 0, 8);
                Array.Reverse(buffer, 0, 8);
                double exactMass = BitConverter.ToDouble(buffer, 0);
                double natAbund;
                if (ins.ReadByte() == 1)
                {
                    ins.Read(buffer, 0, 8);
                    Array.Reverse(buffer, 0, 8);
                    natAbund = BitConverter.ToDouble(buffer, 0);
                }
                else
                {
                    natAbund = 0;
                }
                var isotope = new BODRIsotope(PeriodicTable.GetSymbol(atomicNum), atomicNum, massNum, exactMass, natAbund);
                Add(isotope);
            }
        }

        private static bool IsMajor(IIsotope atom)
        {
            var mass = atom.MassNumber;
            if (mass == null)
                return false;
            try
            {
                var major = Instance.GetMajorIsotope(atom.AtomicNumber.Value);
                if (major == null)
                    return false; // no major isotope
                return major.MassNumber.Equals(mass);
            }
            catch (IOException e)
            {
                Trace.TraceError($"Could not load Isotope data: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Clear the isotope information from atoms that are major isotopes (e.g.
        /// <sup>12</sup>C, <sup>1</sup>H, etc).
        /// </summary>
        /// <param name="mol">the molecule</param>
        public static void ClearMajorIsotopes(IAtomContainer mol)
        {
            foreach (var atom in mol.Atoms)
            {
                if (IsMajor(atom))
                {
                    atom.MassNumber = null;
                    atom.ExactMass = null;
                    atom.NaturalAbundance = null;
                }
            }
        }

        /// <summary>
        /// Clear the isotope information from isotopes that are major (e.g.
        /// <sup>12</sup>C, <sup>1</sup>H, etc).
        /// </summary>
        /// <param name="formula">the formula</param>
        public static void ClearMajorIsotopes(IMolecularFormula formula)
        {
            foreach (var _iso in formula.Isotopes)
            {
                var iso = _iso;
                if (IsMajor(iso))
                {
                    int count = formula.GetCount(iso);
                    formula.Remove(iso);
                    iso.MassNumber = null;
                    // may be immutable
                    if (iso.MassNumber != null)
                    {
                        iso = formula.Builder.NewIsotope(iso.Symbol);
                    }
                    iso.ExactMass = null;
                    iso.NaturalAbundance = null;
                    formula.Add(iso, count);
                }
            }
        }
    }
}
