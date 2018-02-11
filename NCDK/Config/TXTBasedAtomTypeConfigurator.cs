/* Copyright (C) 1997-2007  Bradley A. Smith <bradley@baysmith.com>
 *
 * Contact: cdk-developers@lists.sourceforge.net
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

using NCDK.Common.Primitives;
using System;
using System.Collections.Generic;
using System.IO;

namespace NCDK.Config
{
    /// <summary>
    /// AtomType list configurator that uses the AtomTypes originally
    /// defined in Jmol v5.This class was added to be able to port
    /// Jmol to CDK.The AtomType's themselves seems have a computational
    /// background, but this is not clear.
    /// </summary>
    // @cdk.module core
    // @cdk.githash
    // @author Bradley A. Smith &lt;bradley@baysmith.com&gt;
    // @cdk.keyword    atom, type
    public class TXTBasedAtomTypeConfigurator
        : IAtomTypeConfigurator
    {
        private const string configFile = "NCDK.Config.Data.jmol_atomtypes.txt";

        /// <inheritdoc/>
        public Stream Stream { get; set; }

        public TXTBasedAtomTypeConfigurator() { }

        /// <summary>
        /// Reads a text based configuration file.
        /// </summary>
        /// <param name="builder">used to construct the <see cref="IAtomType"/>'s.</param>
        /// <returns>A <see cref="IEnumerable{IAtomType}"/> with read <see cref="IAtomType"/>'s.</returns>
        /// <exception cref="IOException">when a problem occurred with reading from the <see cref="Stream"/></exception>
        public IEnumerable<IAtomType> ReadAtomTypes(IChemObjectBuilder builder)
        {
            if (Stream == null)
            {
                Stream = ResourceLoader.GetAsStream(configFile);
            }

            using (var reader = new StreamReader(Stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWithChar('#'))
                        continue;
                    var tokens = line.Split("\t ,;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length != 9)
                        throw new IOException("AtomTypeTable.ReadAtypes: " + "Wrong Number of fields");

                    IAtomType atomType;
                    try
                    {
                        var name = tokens[0];
                        var rootType = tokens[1];
                        var san = tokens[2];
                        var sam = tokens[3];
                        // skip the vdw radius value
                        var scovalent = tokens[5];
                        var sColorR = tokens[6];
                        var sColorG = tokens[7];
                        var sColorB = tokens[8];

                        var mass = double.Parse(sam);
                        var covalent = double.Parse(scovalent);
                        var atomicNumber = int.Parse(san);
                        var colorR = int.Parse(sColorR);
                        var colorG = int.Parse(sColorG);
                        var colorB = int.Parse(sColorB);

                        atomType = builder.NewAtomType(name, rootType);
                        atomType.AtomicNumber = atomicNumber;
                        atomType.ExactMass = mass;
                        atomType.CovalentRadius = covalent;
                        atomType.SetProperty(CDKPropertyName.Color, CDKPropertyName.RGB2Int(colorR, colorG, colorB));
                    }
                    catch (FormatException)
                    {
                        throw new IOException("AtomTypeTable.ReadAtypes: " + "Malformed Number");
                    }
                    yield return atomType;
                }
            }
            yield break;
        }
    }
}