/* Copyright (C) 2012  Gilleain Torrance <gilleain.torrance@gmail.com>
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
using System.Collections.Generic;

namespace NCDK.Groups
{
    /// <summary>
    /// A tool for determining the automorphism group of the atoms in a molecule, or
    /// for checking for a canonical form of a molecule.
    /// </summary>
    /// <remarks>
    /// If two bonds are equivalent under an automorphism in the group, then
    /// roughly speaking they are in symmetric positions in the molecule. For
    /// example, the C-C bonds attaching two methyl groups to a benzene ring
    /// are 'equivalent' in this sense.
    /// </remarks>
    /// <example>
    /// There are a couple of ways to use it - firstly, get the automorphisms.
    /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Groups.BondDiscretePartitionRefiner_Example.cs+1"]/*' />
    /// Another is to check an atom container to see if it is canonical:
    /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Groups.BondDiscretePartitionRefiner_Example.cs+2"]/*' />
    /// Note that it is not necessary to call <see cref="Refine(IAtomContainer)"/> before
    /// either of these methods. However if both the group and the canonical check
    /// are required, then the code should be:
    /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Groups.BondDiscretePartitionRefiner_Example.cs+3"]/*' />
    /// This way, the refinement is not carried out multiple times. Finally, remember
    /// to call <see cref="Reset()"/> if the refiner is re-used on multiple structures.
    /// </example>
    // @author maclean
    // @cdk.module group
    public class BondDiscretePartitionRefiner : AbstractDiscretePartitionRefiner
    {
        /// <summary>
        /// The connectivity between bonds; two bonds are connected
        /// if they share an atom.
        /// </summary>
        private int[][] connectionTable;

        /// <summary>
        /// Specialised option to allow generating automorphisms that ignore the bond order.
        /// </summary>
        private bool ignoreBondOrders;

        /// <summary>
        /// Make a bond partition refiner that takes bond-orders into account.
        /// </summary>
        public BondDiscretePartitionRefiner()
                : this(false)
        { }

        /// <summary>
        /// Make a bond partition refiner and specify whether bonds-orders should be
        /// considered when calculating the automorphisms.
        /// </summary>
        /// <param name="ignoreBondOrders">if true, ignore the bond orders</param>
        public BondDiscretePartitionRefiner(bool ignoreBondOrders)
        {
            this.ignoreBondOrders = ignoreBondOrders;
        }

        public override int GetVertexCount()
        {
            return connectionTable.Length;
        }

        public override int GetConnectivity(int i, int j)
        {
            int indexInRow;
            int maxRowIndex = connectionTable[i].Length;
            for (indexInRow = 0; indexInRow < maxRowIndex; indexInRow++)
            {
                if (connectionTable[i][indexInRow] == j)
                {
                    return 1;
                }
            }
            return 0;
        }

        /// <summary>
        /// Used by the equitable refiner to get the indices of bonds connected to
        /// the bond at <paramref name="bondIndex"/>.
        /// </summary>
        /// <param name="bondIndex">the index of the incident bond</param>
        /// <returns>an array of bond indices</returns>
        public virtual int[] GetConnectedIndices(int bondIndex)
        {
            return connectionTable[bondIndex];
        }

        /// <summary>
        /// Get the bond partition, based on the element types of the atoms at either end
        /// of the bond, and the bond order.
        /// </summary>
        /// <param name="atomContainer">the container with the bonds to partition</param>
        /// <returns>a partition of the bonds based on the element types and bond order</returns>
        public Partition GetBondPartition(IAtomContainer atomContainer)
        {
            int bondCount = atomContainer.Bonds.Count;
            IDictionary<string, SortedSet<int>> cellMap = new Dictionary<string, SortedSet<int>>();

            // make mini-'descriptors' for bonds like "C=O" or "C#N" etc
            for (int bondIndex = 0; bondIndex < bondCount; bondIndex++)
            {
                IBond bond = atomContainer.Bonds[bondIndex];
                string el0 = bond.Atoms[0].Symbol;
                string el1 = bond.Atoms[1].Symbol;
                string boS;
                if (ignoreBondOrders)
                {
                    // doesn't matter what it is, so long as it's constant
                    boS = "1";
                }
                else
                {
                    bool isArom = bond.IsAromatic;
                    int orderNumber = (isArom) ? 5 : bond.Order.Numeric;
                    boS = orderNumber.ToString();
                }
                string bondString;
                if (el0.CompareTo(el1) < 0)
                {
                    bondString = el0 + boS + el1;
                }
                else
                {
                    bondString = el1 + boS + el0;
                }
                SortedSet<int> cell;
                if (cellMap.ContainsKey(bondString))
                {
                    cell = cellMap[bondString];
                }
                else
                {
                    cell = new SortedSet<int>();
                    cellMap[bondString] = cell;
                }
                cell.Add(bondIndex);
            }

            // sorting is necessary to get cells in order
            List<string> bondStrings = new List<string>(cellMap.Keys);
            bondStrings.Sort();

            // the partition of the bonds by these 'descriptors'
            Partition bondPartition = new Partition();
            foreach (var key in bondStrings)
            {
                SortedSet<int> cell = cellMap[key];
                bondPartition.AddCell(cell);
            }
            bondPartition.Order();
            return bondPartition;
        }

        /// <summary>
        /// Reset the connection table.
        /// </summary>
        public void Reset()
        {
            connectionTable = null;
        }

        /// <summary>
        /// Refine an atom container, which has the side effect of calculating
        /// the automorphism group.
        /// </summary>
        /// <remarks>
        /// If the group is needed afterwards, call <see cref="AbstractDiscretePartitionRefiner.GetAutomorphismGroup"/>
        /// instead of <see cref="GetAutomorphismGroup(IAtomContainer)"/> otherwise the
        /// refine method will be called twice.
        /// </remarks>
        /// <param name="atomContainer">the atomContainer to refine</param>
        public void Refine(IAtomContainer atomContainer)
        {
            Refine(atomContainer, GetBondPartition(atomContainer));
        }

        /// <summary>
        /// Refine a bond partition based on the connectivity in the atom container.
        /// </summary>
        /// <param name="partition">the initial partition of the bonds</param>
        /// <param name="atomContainer">the atom container to use</param>
        public void Refine(IAtomContainer atomContainer, Partition partition)
        {
            Setup(atomContainer);
            base.Refine(partition);
        }

        /// <summary>
        /// Checks if the atom container is canonical. Note that this calls
        /// <see cref="Refine(IAtomContainer)"/> first.
        /// </summary>
        /// <param name="atomContainer">the atom container to check</param>
        /// <returns>true if the atom container is canonical</returns>
        public bool IsCanonical(IAtomContainer atomContainer)
        {
            Setup(atomContainer);
            base.Refine(GetBondPartition(atomContainer));
            return IsCanonical();
        }

        /// <summary>
        /// Gets the automorphism group of the atom container. By default it uses an
        /// initial partition based on the bond 'types' (so all the C-C bonds are in
        /// one cell, all the C=N in another, etc). If this behaviour is not
        /// desired, then use the <see cref="ignoreBondOrders"/> flag in the constructor.
        /// </summary>
        /// <param name="atomContainer">the atom container to use</param>
        /// <returns>the automorphism group of the atom container</returns>
        public PermutationGroup GetAutomorphismGroup(IAtomContainer atomContainer)
        {
            Setup(atomContainer);
            base.Refine(GetBondPartition(atomContainer));
            return base.GetAutomorphismGroup();
        }

        /// <summary>
        /// Speed up the search for the automorphism group using the automorphisms in
        /// the supplied group. Note that the behaviour of this method is unknown if
        /// the group does not contain automorphisms...
        /// </summary>
        /// <param name="atomContainer">the atom container to use</param>
        /// <param name="group">the group of known automorphisms</param>
        /// <returns>the full automorphism group</returns>
        public PermutationGroup GetAutomorphismGroup(IAtomContainer atomContainer, PermutationGroup group)
        {
            Setup(atomContainer, group);
            base.Refine(GetBondPartition(atomContainer));
            return GetAutomorphismGroup();
        }

        /// <summary>
        /// Get the automorphism group of the molecule given an initial partition.
        /// </summary>
        /// <param name="atomContainer">the atom container to use</param>
        /// <param name="initialPartition">an initial partition of the bonds</param>
        /// <returns>the automorphism group starting with this partition</returns>
        public PermutationGroup GetAutomorphismGroup(IAtomContainer atomContainer, Partition initialPartition)
        {
            Setup(atomContainer);
            base.Refine(initialPartition);
            return base.GetAutomorphismGroup();
        }

        /// <summary>
        /// Get the automorphism partition (equivalence classes) of the bonds.
        /// </summary>
        /// <param name="atomContainer">the molecule to calculate equivalence classes for</param>
        /// <returns>a partition of the bonds into equivalence classes</returns>
        public Partition GetAutomorphismPartition(IAtomContainer atomContainer)
        {
            Setup(atomContainer);
            base.Refine(GetBondPartition(atomContainer));
            return base.GetAutomorphismPartition();
        }

        private void Setup(IAtomContainer atomContainer)
        {
            // have to setup the connection table before making the group
            // otherwise the size may be wrong
            if (connectionTable == null)
            {
                SetupConnectionTable(atomContainer);
            }

            int size = GetVertexCount();
            PermutationGroup group = new PermutationGroup(new Permutation(size));
            base.Setup(group, new BondEquitablePartitionRefiner(this));
        }

        private void Setup(IAtomContainer atomContainer, PermutationGroup group)
        {
            SetupConnectionTable(atomContainer);
            base.Setup(group, new BondEquitablePartitionRefiner(this));
        }

        private void SetupConnectionTable(IAtomContainer atomContainer)
        {
            int bondCount = atomContainer.Bonds.Count;
            // unfortunately, we have to sort the bonds
            List<IBond> bonds = new List<IBond>();
            IDictionary<string, IBond> bondMap = new Dictionary<string, IBond>();
            for (int bondIndexI = 0; bondIndexI < bondCount; bondIndexI++)
            {
                IBond bond = atomContainer.Bonds[bondIndexI];
                bonds.Add(bond);
                int a0 = atomContainer.Atoms.IndexOf(bond.Atoms[0]);
                int a1 = atomContainer.Atoms.IndexOf(bond.Atoms[1]);
                string boS;
                if (ignoreBondOrders)
                {
                    // doesn't matter what it is, so long as it's constant
                    boS = "1";
                }
                else
                {
                    bool isArom = bond.IsAromatic;
                    int orderNumber = (isArom) ? 5 : bond.Order.Numeric;
                    boS = orderNumber.ToString();
                }
                string bondString;
                if (a0 < a1)
                {
                    bondString = a0 + "," + boS + "," + a1;
                }
                else
                {
                    bondString = a1 + "," + boS + "," + a0;
                }
                bondMap[bondString] = bond;
            }

            List<string> keys = new List<string>(bondMap.Keys);
            keys.Sort();
            foreach (var key in keys)
            {
                bonds.Add(bondMap[key]);
            }

            connectionTable = new int[bondCount][];
            for (int bondIndexI = 0; bondIndexI < bondCount; bondIndexI++)
            {
                IBond bondI = bonds[bondIndexI];
                List<int> connectedBondIndices = new List<int>();
                for (int bondIndexJ = 0; bondIndexJ < bondCount; bondIndexJ++)
                {
                    if (bondIndexI == bondIndexJ) continue;
                    IBond bondJ = bonds[bondIndexJ];
                    if (bondI.IsConnectedTo(bondJ))
                    {
                        connectedBondIndices.Add(bondIndexJ);
                    }
                }
                int connBondCount = connectedBondIndices.Count;
                connectionTable[bondIndexI] = new int[connBondCount];
                for (int index = 0; index < connBondCount; index++)
                {
                    connectionTable[bondIndexI][index] = connectedBondIndices[index];
                }
            }
        }
    }
}
