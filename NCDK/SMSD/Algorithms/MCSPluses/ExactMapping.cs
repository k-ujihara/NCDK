/*
 *
 * Copyright (C) 2006-2010  Syed Asad Rahman <asad@ebi.ac.uk>
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
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
*/
using System;
using System.Collections.Generic;

namespace NCDK.SMSD.Algorithms.MCSPluses
{
    /// <summary>
    /// This class handles MCS between two identical molecules.
    /// Hence they generate am MCS where all atoms are mapped.
    // @cdk.module smsd
    // @cdk.githash
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    /// </summary>
    public class ExactMapping
    {

        /// <summary>
        ///
        /// Extract atom mapping from the cliques and stores it in a List
        /// <param name="compGraphNodes">/// @param cliqueListOrg</param>
        /// </summary>
        private static IList<int> ExtractCliqueMapping(IList<int> compGraphNodes, IList<int> cliqueListOrg)
        {

            List<int> cliqueMapping = new List<int>();
            List<int> cliqueList = new List<int>(cliqueListOrg);
            int cliqueSize = cliqueList.Count;
            int vecSize = compGraphNodes.Count;
            //        Console.Out.WriteLine("VEC  SIZE " + vec_size);
            for (int a = 0; a < cliqueSize; a++)
            {
                for (int b = 0; b < vecSize; b += 3)
                {
                    if (cliqueList[a] == compGraphNodes[b + 2])
                    {
                        cliqueMapping.Add(compGraphNodes[b]);
                        cliqueMapping.Add(compGraphNodes[b + 1]);
                    }
                }
            }

            return cliqueMapping;
        }

        //extract atom mapping from the clique List and print it on the screen
        /// <summary>
        ///
        /// <param name="mappings">/// @param compGraphNodes</param>
        /// <param name="cliqueListOrg">/// <returns>mappings</param></returns>
        /// </summary>
        public static IList<IList<int>> ExtractMapping(IList<IList<int>> mappings, IList<int> compGraphNodes,
                IList<int> cliqueListOrg)
        {
            try
            {
                IList<int> cliqueList = ExtractCliqueMapping(compGraphNodes, cliqueListOrg);
                mappings.Add(cliqueList);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Error in FinalMapping List: " + e.InnerException);
                Console.Out.WriteLine(e.StackTrace);
                throw;
            }
            return mappings;
        }
    }
}
