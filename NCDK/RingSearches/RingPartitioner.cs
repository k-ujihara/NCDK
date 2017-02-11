/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *  All we ask is that proper credit is given for our work, which includes
 *  - but is not limited to - adding the above copyright notice to the beginning
 *  of your source code files, and to any copyright notice that you may distribute
 *  with programs based on this work.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT Any WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using System.Collections.Generic;
using System.Linq;

namespace NCDK.RingSearches
{
    /**
     *  Partitions a RingSet into RingSets of connected rings. Rings which share an
     *  Atom, a Bond or three or more atoms with at least on other ring in the
     *  RingSet are considered connected.
     *
     *
     * @cdk.module standard
     * @cdk.githash
     */
    public class RingPartitioner
    {

        /**
         *  Debugging on/off
         */
        public const bool debug = false;

        // minimum details

        /**
         *  Partitions a RingSet into RingSets of connected rings. Rings which share
         *  an Atom, a Bond or three or more atoms with at least on other ring in
         *  the RingSet are considered connected. Thus molecules such as azulene and
         * indole will return a List with 1 element.
         *
         * <p>Note that an isolated ring is considered to be <i>self-connect</i>. As a result
         * a molecule such as biphenyl will result in a 2-element List being returned (each
         * element corresponding to a phenyl ring).
         *
         *@param  ringSet  The RingSet to be partitioned
         *@return          A {@link List} of connected RingSets
         */
        public static IList<IRingSet> PartitionRings(IEnumerable<IRing> ringSet)
        {
           var ringSets = new List<IRingSet>();
            if (!ringSet.Any()) return ringSets;
            var ring = ringSet.First();
            if (ring == null) return ringSets;
            IRingSet rs = ring.Builder.CreateRingSet();
            foreach (var r in ringSet)
                rs.Add(r);
            do
            {
                ring = rs[0];
                var newRs = ring.Builder.CreateRingSet();
                newRs.Add(ring);
                ringSets.Add(WalkRingSystem(rs, ring, newRs));
            } while (rs.Count > 0);

            return ringSets;
        }

        /**
         *  Converts a RingSet to an AtomContainer.
         *
         *@param  ringSet  The RingSet to be converted.
         *@return          The AtomContainer containing the bonds and atoms of the ringSet.
         */
        public static IAtomContainer ConvertToAtomContainer(IRingSet ringSet)
        {
            IRing ring = (IRing)ringSet[0];
            if (ring == null) return null;
            IAtomContainer ac = ring.Builder.CreateAtomContainer();
            for (int i = 0; i < ringSet.Count; i++)
            {
                ring = (IRing)ringSet[i];
                for (int r = 0; r < ring.Bonds.Count; r++)
                {
                    IBond bond = ring.Bonds[r];
                    if (!ac.Contains(bond))
                    {
                        for (int j = 0; j < bond.Atoms.Count; j++)
                        {
                            ac.Add(bond.Atoms[j]);
                        }
                        ac.Add(bond);
                    }
                }
            }
            return ac;
        }

        /**
         *  Perform a walk in the given RingSet, starting at a given Ring and
         *  recursively searching for other Rings connected to this ring. By doing
         *  this it finds all rings in the RingSet connected to the start ring,
         *  putting them in newRs, and removing them from rs.
         *
         *@param  rs     The RingSet to be searched
         *@param  ring   The ring to start with
         *@param  newRs  The RingSet containing all Rings connected to ring
         *@return        newRs The RingSet containing all Rings connected to ring
         */
        private static IRingSet WalkRingSystem(IRingSet rs, IRing ring, IRingSet newRs)
        {
            IRing tempRing;
            var tempRings = rs.GetConnectedRings(ring);
            //        Debug.WriteLine("walkRingSystem -> tempRings.Count: " + tempRings.Count);
            rs.Remove(ring);
            foreach (var container in tempRings)
            {
                tempRing = (IRing)container;
                if (!newRs.Contains(tempRing))
                {
                    newRs.Add(tempRing);
                    newRs.Add(WalkRingSystem(rs, tempRing, newRs));
                }
            }
            return newRs;
        }
    }
}
