/* Copyright (C) 2006-2007  Todd Martin (Environmental Protection Agency)
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
using System.Collections.Generic;
using System.IO;

namespace NCDK.Tools
{
    /// <summary>
    /// Provides atomic property values for descriptor calculations.
    /// This class currently provides values for mass, van der Waals volume, electronegativity and polarizability.
    /// </summary>
    // @author     Todd Martin
    // @cdk.module qsar
    // @cdk.githash
    public class AtomicProperties
    {
        private static AtomicProperties ap = null;

        private IDictionary<string, double> htMass = new Dictionary<string, double>();
        private IDictionary<string, double> htVdWVolume = new Dictionary<string, double>();
        private IDictionary<string, double> htElectronegativity = new Dictionary<string, double>();
        private IDictionary<string, double> htPolarizability = new Dictionary<string, double>();

        private AtomicProperties()
        {
            string configFile = "NCDK.Config.Data.whim_weights.txt";

            using (var ins = ResourceLoader.GetAsStream(configFile))
            using (var bufferedReader = new StreamReader(ins))
            {
                bufferedReader.ReadLine(); // header

                string Line;
                while (true)
                {
                    Line = bufferedReader.ReadLine();
                    if (Line == null)
                    {
                        break;
                    }
                    string[] components = Line.Split('\t');

                    string symbol = components[0];
                    htMass[symbol] = double.Parse(components[1]);
                    htVdWVolume[symbol] = double.Parse(components[2]);
                    htElectronegativity[symbol] = double.Parse(components[3]);
                    htPolarizability[symbol] = double.Parse(components[4]);
                }
            }
        }

        public double GetVdWVolume(string symbol)
        {
            return htVdWVolume[symbol];
        }

        public double GetNormalizedVdWVolume(string symbol)
        {
            return this.GetVdWVolume(symbol) / this.GetVdWVolume("C");
        }

        public double GetElectronegativity(string symbol)
        {
            return htElectronegativity[symbol];
        }

        public double GetNormalizedElectronegativity(string symbol)
        {
            return this.GetElectronegativity(symbol) / this.GetElectronegativity("C");
        }

        public double GetPolarizability(string symbol)
        {
            return htPolarizability[symbol];
        }

        public double GetNormalizedPolarizability(string symbol)
        {
            return this.GetPolarizability(symbol) / this.GetPolarizability("C");
        }

        public double GetMass(string symbol)
        {
            return htMass[symbol];
        }

        public double GetNormalizedMass(string symbol)
        {
            return this.GetMass(symbol) / this.GetMass("C");
        }

        public static AtomicProperties Instance
        {
            get
            {
                if (ap == null)
                {
                    ap = new AtomicProperties();
                }
                return ap;
            }
        }
    }
}

