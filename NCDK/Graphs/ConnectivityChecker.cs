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
 */

using System;
using System.Collections.Generic;
using System.Linq;
using NCDK.Sgroups;

namespace NCDK.Graphs
{
    /// <summary>
    /// Tool class for checking whether the (sub)structure in an <see cref="IAtomContainer"/> is connected.
    /// </summary>
    /// <example>
    /// To check whether an <see cref="IAtomContainer"/> is connected this code
    /// can be used:
    /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Graphs.ConnectivityChecker_Example.cs+1"]/*' />
    /// A disconnected AtomContainer can be fragmented into connected
    /// fragments by using code like:
    /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Graphs.ConnectivityChecker_Example.cs+2"]/*' />
    /// </example>
    // @cdk.module standard
    // @cdk.keyword connectivity
    public static class ConnectivityChecker
    {
        /// <summary>
        /// Check whether a set of atoms in an <see cref="IAtomContainer"/> is connected.
        /// </summary>
        /// <param name="atomContainer">The <see cref="IAtomContainer"/> to be check for connectedness</param>
        /// <returns>true if the <see cref="IAtomContainer"/> is connected</returns>
        public static bool IsConnected(IAtomContainer atomContainer)
        {
            // with one atom or less, we define it to be connected, as there is no
            // partitioning needed
            if (atomContainer.Atoms.Count < 2)
                return true;

            var cc = new ConnectedComponents(GraphUtil.ToAdjList(atomContainer));
            return cc.NumberOfComponents == 1;
        }

        /// <summary>
        /// Partitions the atoms in an <see cref="IAtomContainer"/> into covalently connected components.
        /// </summary>
        /// <param name="container">The <see cref="IAtomContainer"/> to be partitioned into connected components, i.e. molecules</param>
        /// <returns>A MoleculeSet.</returns>
        // @cdk.dictref   blue-obelisk:graphPartitioning
        public static IReadOnlyList<IAtomContainer> PartitionIntoMolecules(IAtomContainer container)
        {
            var cc = new ConnectedComponents(GraphUtil.ToAdjList(container));
            return PartitionIntoMolecules(container, cc.GetComponents());
        }

        public static IReadOnlyList<IAtomContainer> PartitionIntoMolecules(IAtomContainer container, int[] components)
        {
            int maxComponentIndex = 0;
            foreach (int component in components)
                if (component > maxComponentIndex)
                    maxComponentIndex = component;

            var containers = new IAtomContainer[maxComponentIndex + 1];
            var componentsMap = new Dictionary<IAtom, IAtomContainer>(2 * container.Atoms.Count);
            var componentAtomMap = new Dictionary<IAtom, IAtom>(2 * container.Atoms.Count);
            var componentBondMap = new Dictionary<IBond, IBond>(2 * container.Bonds.Count);

            for (int i = 1; i < containers.Length; i++)
                containers[i] = container.Builder.NewAtomContainer();

            for (int i = 0; i < container.Atoms.Count; i++)
            {
                var origAtom = container.Atoms[i];
                var newContainer = containers[components[i]];
                componentsMap[origAtom] = newContainer;
                newContainer.Atoms.Add(origAtom);
                //the atom should always be added so this should be safe
                componentAtomMap[origAtom] =newContainer.Atoms[newContainer.Atoms.Count - 1];
            }

            foreach (var bond in container.Bonds)
            {
                var begComp = componentsMap[bond.Begin];
                var endComp = componentsMap[bond.End];
                if (begComp == endComp)
                {
                    begComp.Bonds.Add(bond);
                    //bond should always be added so this should be safe
                    componentBondMap[bond] = begComp.Bonds[begComp.Bonds.Count - 1];
                }
            }

            foreach (var electron in container.SingleElectrons)
                componentsMap[electron.Atom].SingleElectrons.Add(electron);

            foreach (var lonePair in container.LonePairs)
                componentsMap[lonePair.Atom].LonePairs.Add(lonePair);

            foreach (var stereo in container.StereoElements)
            {
                var focus = stereo.Focus;
                switch (focus)
                {
                    case IAtom atom:
                        if (componentsMap.ContainsKey(atom))
                            componentsMap[atom].StereoElements.Add(stereo);
                        break;
                    case IBond bond:
                        if (componentsMap.ContainsKey(bond.Begin))
                            componentsMap[bond.Begin].StereoElements.Add(stereo);
                        break;
                    default:
                        throw new InvalidOperationException("New stereo element not using an atom/bond for focus?");
                }
            }

            //add SGroups
            var sgroups = container.GetCtabSgroups();

            if (sgroups != null)
            {
                var old2NewSgroupMap = new Dictionary<Sgroup, Sgroup>();
                var newSgroups = new IList<Sgroup>[containers.Length];
                foreach (var sgroup in sgroups)
                {
                    var merator = sgroup.Atoms.GetEnumerator();
                    if (!merator.MoveNext())
                        continue;

                    var componentIndex = GetComponentIndexFor(components, containers, merator.Current);
                    var allMatch = componentIndex >= 0;
                    while (allMatch && merator.MoveNext())
                    {
                        //if component index for some atoms
                        //don't match then the sgroup is split across components
                        //so ignore it for now?
                        allMatch = (componentIndex == GetComponentIndexFor(components, containers, merator.Current));
                    }
                    if (allMatch && componentIndex >= 0)
                    {
                        var cpy = new Sgroup();
                        var newComponentSgroups = newSgroups[componentIndex];
                        if (newComponentSgroups == null)
                        {
                            newComponentSgroups = newSgroups[componentIndex] = new List<Sgroup>();
                        }
                        newComponentSgroups.Add(cpy);
                        old2NewSgroupMap[sgroup] = cpy;
                        foreach (var atom in sgroup.Atoms)
                        {
                            cpy.Atoms.Add(componentAtomMap[atom]);

                        }
                        foreach (var bond in sgroup.Bonds)
                        {
                            var newBond = componentBondMap[bond];
                            if (newBond != null)
                            {
                                cpy.Bonds.Add(newBond);
                            }
                        }

                        foreach (var key in sgroup.AttributeKeys)
                            cpy.PutValue(key, sgroup.GetValue(key));

                    }
                }
                //finally update parents
                foreach (var sgroup in sgroups)
                {
                    var newSgroup = old2NewSgroupMap[sgroup];
                    if (newSgroup != null)
                    {
                        foreach (var parent in sgroup.Parents)
                        {
                            var newParent = old2NewSgroupMap[parent];
                            if (newParent != null)
                            {
                                newSgroup.Parents.Add(newParent);
                            }
                        }
                    }
                }
                for (int i = 1; i < containers.Length; i++)
                {
                    var sg = newSgroups[i];
                    if (sg != null)
                    {
                        containers[i].SetCtabSgroups(sg);
                    }
                }
            }

            // do not return IEnumerable, containers are modified above.
            var containerSet = new List<IAtomContainer>(containers.Skip(1));

            return containerSet;
        }

        private static int GetComponentIndexFor(int[] components, IAtomContainer[] containers, IAtom atom)
        {
            if (atom.Index >= 0)
            {
                return components[atom.Index];
            }
            //if index isn't known check each container
            for (int i = 1; i < containers.Length; i++)
            {
                if (containers[i].Contains(atom))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
