/* Copyright (C) 2001-2007  Christoph Steinbeck <steinbeck@users.sf.net>
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Config.Isotope;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace NCDK.Config
{
    /// <summary>
    /// Used to store and return data of a particular isotope. As this class is a
    /// singleton class, one gets an instance with:
    /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Config.XMLIsotopeFactory_Example.cs+1"]/*' />
    /// </summary>
    /// <remarks>
    /// Data about the isotopes are read from the NCDK.Config.Data.isotopes.xml resource.
    /// Part of the data in this file was collected from
    /// the website <see href="http://www.webelements.org">webelements.org</see>.
    /// </remarks>
    /// <example>
    /// The use of this class is exemplified as follows. To get information
    /// about the major isotope of hydrogen, one can use this code:
    /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Config.XMLIsotopeFactory_Example.cs+example"]/*' />
    /// </example>
    // @cdk.module     extra
    // @cdk.githash
    // @author     steinbeck
    // @cdk.created    2001-08-29
    // @cdk.keyword    isotope
    // @cdk.keyword    element
    public class XMLIsotopeFactory : IsotopeFactory
    {
        private static XMLIsotopeFactory ifac = null;
        private const bool debug = false;

        /// <summary>
        /// Private constructor for the IsotopeFactory object.
        /// </summary>
        /// <param name="builder">The builder from which we the factory will be generated</param>
        /// <exception cref="IOException">A problem with reading the isotopes.xml file</exception>
        private XMLIsotopeFactory(IChemObjectBuilder builder)
        {
            Trace.TraceInformation("Creating new IsotopeFactory");

            Stream ins;
            // ObjIn in = null;
            string errorMessage = $"There was a problem getting NCDK.Config.Data.isotopes.xml as a stream";
            try
            {
                string configFile = "NCDK.Config.Data.isotopes.xml";
                if (debug) Debug.WriteLine("Getting stream for ", configFile);
                ins = ResourceLoader.GetAsStream(configFile);
            }
            catch (Exception exception)
            {
                Trace.TraceError(errorMessage);
                Debug.WriteLine(exception);
                throw new IOException(errorMessage);
            }
            if (ins == null)
            {
                Trace.TraceError(errorMessage);
                throw new IOException(errorMessage);
            }
            var reader = new IsotopeReader(ins, builder);
            //in = new ObjIn(ins, new Config().aliasID(false));
            this.isotopes = new Dictionary<string, IList<IIsotope>>();
            var isotopes = reader.ReadIsotopes();
            foreach (var isotope in isotopes)
                Add(isotope);
            if (debug) Debug.WriteLine($"Found #isotopes in file: {isotopes.Count}");

            // for (int f = 0; f < isotopes.Size(); f++) { Isotope isotope =
            // (Isotope)isotopes.elementAt(f); } What's this loop for??
            
            majorIsotopes = new Dictionary<string, IIsotope>();
        }

        /// <summary>
        /// Returns an IsotopeFactory instance.
        /// </summary>
        /// <param name="builder">ChemObjectBuilder used to construct the Isotope's</param>
        /// <returns>The instance value</returns>
        /// <exception cref="IOException">if isotopic data files could not be read.</exception>"
        public static XMLIsotopeFactory GetInstance(IChemObjectBuilder builder)
        {
            if (ifac == null)
            {
                ifac = new XMLIsotopeFactory(builder);
            }
            return ifac;
        }
    }
}
