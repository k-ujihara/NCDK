/* Copyright (C) 2002-2007  Stephane Werner <mail@ixelis.net>
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
using System.Linq;

namespace NCDK.Isomorphisms.MCSS
{
    /**
     *  An RMap implements the association between an edge (bond) in G1 and an edge
     *  (bond) in G2, G1 and G2 being the compared graphs in a RGraph context.
     *
     * @author      Stephane Werner, IXELIS <mail@ixelis.net>
     * @cdk.created 2002-07-24
     * @cdk.module  standard
     * @cdk.githash
     */
    public class RMap
    {
        public int Id1 { get; set; } = 0;
        public int Id2 { get; set; } = 0;

        /**
         *  Constructor for the RMap.
         *
         * @param  id1  number of the edge (bond) in the graph 1
         * @param  id2  number of the edge (bond) in the graph 2
         */
        public RMap(int id1, int id2)
        {
            Id1 = id1;
            Id2 = id2;
        }

        /**
         *  The equals method.
         *
         * @param  o  The object to compare.
         * @return    true=if both ids equal, else false.
         */
        public override bool Equals(object o)
        {
            var aa = o as RMap;
            if (o == null)
                return false;
            return Id1 == aa.Id1 && Id2 == aa.Id2;
        }

        public override int GetHashCode()
        {
            return Id1 * 31 + Id2;
        }
    }
}
