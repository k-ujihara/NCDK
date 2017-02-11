/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@lists.sourceforge.net
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
 *
 */
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using NCDK.Common.Collections;
using NCDK.Stereo;

namespace NCDK.Graphs
{
    /**
	 * Tool class for checking whether the (sub)structure in an
	 * AtomContainer is connected.
	 * To check whether an AtomContainer is connected this code
	 * can be used:
	 * <pre>
	 *  bool isConnected = ConnectivityChecker.IsConnected(atomContainer);
	 * </pre>
	 *
	 * <p>A disconnected AtomContainer can be fragmented into connected
	 * fragments by using code like:
	 * <pre>
	 *   MoleculeSet fragments = ConnectivityChecker.PartitionIntoMolecules(disconnectedContainer);
	 *   int fragmentCount = fragments.Count();
	 * </pre>
	 *
	 * @cdk.module standard
	 * @cdk.githash
	 *
	 * @cdk.keyword connectivity
	 */
    public class ConnectivityChecker
    {
        /**
		 * Check whether a set of atoms in an <see cref="IAtomContainer"/> is connected.
		 *
		 * @param   atomContainer  The <see cref="IAtomContainer"/> to be check for connectedness
		 * @return                 true if the <see cref="IAtomContainer"/> is connected
		 */
        public static bool IsConnected(IAtomContainer atomContainer)
        {
            // with one atom or less, we define it to be connected, as there is no
            // partitioning needed
            if (atomContainer.Atoms.Count < 2) return true;

            ConnectedComponents cc = new ConnectedComponents(GraphUtil.ToAdjList(atomContainer));
            return cc.NComponents == 1;
        }

        /**
		 * Partitions the atoms in an AtomContainer into covalently connected components.
		 *
		 * @param   container  The AtomContainer to be partitioned into connected components, i.e. molecules
		 * @return                 A MoleculeSet.
		 *
		 * @cdk.dictref   blue-obelisk:graphPartitioning
		 */
        public static IAtomContainerSet<IAtomContainer> PartitionIntoMolecules(IAtomContainer container)
        {
            ConnectedComponents cc = new ConnectedComponents(GraphUtil.ToAdjList(container));
            int[] components = cc.Components();
            IAtomContainer[] containers = new IAtomContainer[cc.NComponents + 1];
            IDictionary<IAtom, IAtomContainer> componentsMap = new Dictionary<IAtom, IAtomContainer>(2 * container.Atoms.Count);

            for (int i = 1; i < containers.Length; i++)
                containers[i] = container.Builder.CreateAtomContainer();

            IAtomContainerSet<IAtomContainer> containerSet = container.Builder.CreateAtomContainerSet();

            for (int i = 0; i < container.Atoms.Count; i++)
            {
                componentsMap.Add(container.Atoms[i], containers[components[i]]);
                containers[components[i]].Atoms.Add(container.Atoms[i]);
            }

            foreach (var bond in container.Bonds)
                componentsMap[bond.Atoms[0]].Bonds.Add(bond);

            foreach (var electron in container.SingleElectrons)
                componentsMap[electron.Atom].Add(electron);

            foreach (var lonePair in container.LonePairs)
                componentsMap[lonePair.Atom].Add(lonePair);

            foreach (var stereo in container.StereoElements)
            {
                if (stereo is ITetrahedralChirality)
                {
                    IAtom a = ((ITetrahedralChirality)stereo).ChiralAtom;
                    if (componentsMap.ContainsKey(a)) componentsMap[a].Add(stereo);
                }
                else if (stereo is IDoubleBondStereochemistry)
                {
                    IBond bond = ((IDoubleBondStereochemistry)stereo).StereoBond;
                    if (componentsMap.ContainsKey(bond.Atoms[0]) && componentsMap.ContainsKey(bond.Atoms[1]))
                        componentsMap[bond.Atoms[0]].Add(stereo);
                }
                else if (stereo is ExtendedTetrahedral)
                {
                    IAtom atom = ((ExtendedTetrahedral)stereo).Focus;
                    if (componentsMap.ContainsKey(atom)) componentsMap[atom].Add(stereo);
                }
                else
                {
                    Console.Error.WriteLine("New stereochemistry element is not currently partitioned with ConnectivityChecker:"
                            + stereo.GetType());
                }
            }

            for (int i = 1; i < containers.Length; i++)
                containerSet.Add(containers[i]);

            return containerSet;
        }
    }
}
		