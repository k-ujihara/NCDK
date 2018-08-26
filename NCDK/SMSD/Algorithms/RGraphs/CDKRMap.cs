/* Copyright (C) 2002-2007  Stephane Werner <mail@ixelis.net>
 *               2007-2009  Syed Asad Rahman <asad@ebi.ac.uk>
 *
 * This code has been kindly provided by Stephane Werner
 * and Thierry Hanser from IXELIS mail@ixelis.net.
 *
 * IXELIS sarl - Semantic Information Systems
 *               17 rue des C?dres 67200 Strasbourg, France
 *               Tel/Fax : +33(0)3 88 27 81 39 Email: mail@ixelis.net
 *
 * CDK Contact: cdk-devel@lists.sf.net
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
namespace NCDK.SMSD.Algorithms.RGraphs
{
    /// <summary>
    ///  An CDKRMap implements the association between an edge (bond) in G1 and an edge
    ///  (bond) in G2, G1 and G2 being the compared graphs in a RGraph context.
    /// </summary>
    // @author      Stephane Werner, IXELIS <mail@ixelis.net>, Syed Asad Rahman <asad@ebi.ac.uk> (modified the orignal code)
    // @cdk.created 2002-07-24
    // @cdk.module  smsd
    // @cdk.githash
    public class CDKRMap
    {
        /// <summary>
        /// Constructor for the CDKRMap
        /// </summary>
        /// <param name="id1">number of the edge (bond) in the graphe 1</param>
        /// <param name="id2">number of the edge (bond) in the graphe 2</param>
        public CDKRMap(int id1, int id2)
        {
            this.Id1 = id1;
            this.Id2 = id2;
        }

        /// <summary>
        /// the id1 attribute of the CDKRMap object
        /// </summary>
        public int Id1 { get; set; }

        /// <summary>
        /// the id2 attribute of the CDKRMap object
        /// </summary>
        public int Id2 { get; set; }

        /// <summary>
        ///  The equals method.
        ///
        /// <param name="obj">The object to compare.</param>
        /// <returns>true=if both ids equal, else false.</returns>
        /// </summary>
        public override bool Equals(object obj)
        {
            if (((CDKRMap)obj).Id1 == Id1 && ((CDKRMap)obj).Id2 == Id2)
            {
                return (true);
            }
            else
            {
                return (false);
            }
        }

        /// <summary>
       /// Returns a hash code for object comparison.
       /// <returns>Returns a hash code for object comparison.</returns>
       /// </summary>
        public override int GetHashCode()
        {
            int hash = 5;
            hash = 79 * hash + this.Id1;
            hash = 79 * hash + this.Id2;
            return hash;
        }
    }
}
