/*  Copyright (C) 2005-2007  Christian Hoppe <chhoppe@users.sf.net>
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
using NCDK.Graphs;
using NCDK.Graphs.Matrix;
using NCDK.QSAR.Results;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    ///  Class that returns the number of atoms in the longest aliphatic chain.
    /// </summary>
    /// <remarks>
    /// <para>This descriptor uses these parameters:
    /// <list type="table">
    ///   <item>
    ///     <term>Name</term>
    ///     <term>Default</term>
    ///     <term>Description</term>
    ///   </item>
    ///   <item>
    ///     <term>checkRingSystem</term>
    ///     <term>false</term>
    ///     <term>True is the CDKConstant.ISINRING has to be set</term>
    ///   </item>
    /// </list>
    /// </para>
    /// </remarks>
    /// Returns a single value named <i>nAtomLAC</i>
    // @author      chhoppe from EUROSCREEN
    // @cdk.created 2006-1-03
    // @cdk.module  qsarmolecular
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:largestAliphaticChain
    public class LongestAliphaticChainDescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        private bool checkRingSystem = false;
        private static readonly string[] NAMES = { "nAtomLAC" };

        /// <summary>
        ///  Constructor for the LongestAliphaticChainDescriptor object.
        /// </summary>
        public LongestAliphaticChainDescriptor() { }

        /// <inheritdoc/> 
        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
         new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#longestAliphaticChain",
                typeof(LongestAliphaticChainDescriptor).FullName, "The Chemistry Development Kit");

        /// <summary>
        ///  Sets the parameters attribute of the LongestAliphaticChainDescriptor object.
        /// <para>
        /// This descriptor takes one parameter, which should be bool to indicate whether
        /// aromaticity has been checked (TRUE) or not (FALSE).</para>
        /// </summary>
        /// <exception cref="CDKException">if more than one parameter or a non-bool parameter is specified</exception>
        public override object[] Parameters
        {
            set
            {
                if (value.Length > 1)
                {
                    throw new CDKException("LongestAliphaticChainDescriptor only expects one parameter");
                }
                if (!(value[0] is bool))
                {
                    throw new CDKException("Both parameters must be of type bool");
                }
                // ok, all should be fine
                checkRingSystem = (bool)value[0];
            }
            get
            {
                // return the parameters as used for the descriptor calculation
                return new object[] { checkRingSystem };
            }
        }

        public override string[] DescriptorNames => NAMES;

        private DescriptorValue GetDummyDescriptorValue(Exception e)
        {
            return new DescriptorValue(_Specification, ParameterNames, Parameters, new IntegerResult(0), DescriptorNames, e);
        }

        /// <summary>
        /// Calculate the count of atoms of the longest aliphatic chain in the supplied <see cref="IAtomContainer"/>.
        /// </summary>
        /// <remarks>
        ///  The method require one parameter:
        ///  if checkRingSyste is true the <see cref="IMolecularEntity.IsInRing"/> will be set
        ///  </remarks>
        /// <param name="atomContainer">The <see cref="IAtomContainer"/> for which this descriptor is to be calculated</param>
        /// <returns>the number of atoms in the longest aliphatic chain of this AtomContainer</returns>
        /// <seealso cref="Parameters"/>
        public override DescriptorValue Calculate(IAtomContainer atomContainer)
        {
            IAtomContainer container = (IAtomContainer)atomContainer.Clone();
            container = AtomContainerManipulator.RemoveHydrogens(container);
            IRingSet rs;
            if (checkRingSystem)
            {
                try
                {
                    rs = new SpanningTree(container).GetBasicRings();
                }
                catch (NoSuchAtomException e)
                {
                    return GetDummyDescriptorValue(e);
                }
                for (int i = 0; i < container.Atoms.Count; i++)
                {
                    if (rs.Contains(container.Atoms[i]))
                    {
                        container.Atoms[i].IsInRing = true;
                    }
                }
            }

            int longestChainAtomsCount = 0;
            int tmpLongestChainAtomCount;

            for (int i = 0; i < container.Atoms.Count; i++)
            {
                container.Atoms[i].IsVisited = false;
            }

            for (int i = 0; i < container.Atoms.Count; i++)
            {
                IAtom atomi = container.Atoms[i];
                if (atomi.Symbol.Equals("H")) continue;

                if ((!atomi.IsAromatic && !atomi.IsInRing
                        & atomi.Symbol.Equals("C"))
                        & !atomi.IsVisited)
                {

                    var startSphere = new List<IAtom>();
                    var path = new List<IAtom>();
                    startSphere.Add(atomi);
                    try
                    {
                        BreadthFirstSearch(container, startSphere, path);
                    }
                    catch (CDKException e)
                    {
                        return GetDummyDescriptorValue(e);
                    }
                    IAtomContainer aliphaticChain = CreateAtomContainerFromPath(container, path);
                    if (aliphaticChain.Atoms.Count > 1)
                    {
                        double[][] conMat = ConnectionMatrix.GetMatrix(aliphaticChain);
                        int[][] apsp = PathTools.ComputeFloydAPSP(conMat);
                        tmpLongestChainAtomCount = GetLongestChainPath(apsp);
                        if (tmpLongestChainAtomCount > longestChainAtomsCount)
                        {
                            longestChainAtomsCount = tmpLongestChainAtomCount;
                        }
                    }
                }
            }

            return new DescriptorValue(_Specification, ParameterNames, Parameters, new IntegerResult(longestChainAtomsCount), DescriptorNames);
        }

        /// <inheritdoc/>
        public override IDescriptorResult DescriptorResultType { get; } = new IntegerResult(1);

        private int GetLongestChainPath(int[][] apsp)
        {
            int longestPath = 0;
            for (int i = 0; i < apsp.Length; i++)
            {
                for (int j = 0; j < apsp.Length; j++)
                {
                    if (apsp[i][j] + 1 > longestPath)
                    {
                        longestPath = apsp[i][j] + 1;
                    }
                }
            }
            return longestPath;
        }

        private IAtomContainer CreateAtomContainerFromPath(IAtomContainer container, List<IAtom> path)
        {
            IAtomContainer aliphaticChain = container.Builder.NewAtomContainer();
            for (int i = 0; i < path.Count - 1; i++)
            {
                if (!aliphaticChain.Contains(path[i]))
                {
                    aliphaticChain.Atoms.Add(path[i]);
                }
                for (int j = 1; j < path.Count; j++)
                {
                    if (container.GetBond(path[i], path[j]) != null)
                    {
                        if (!aliphaticChain.Contains(path[j]))
                        {
                            aliphaticChain.Atoms.Add(path[j]);
                        }
                        aliphaticChain.Bonds.Add(container.GetBond(path[i], path[j]));
                    }
                }
            }

            //for (int i=0;i<aliphaticChain.Atoms.Count;i++){
            //    Debug.WriteLine("container-->atom:"+i+" Nr: "+container.Atoms.IndexOf(aliphaticChain.GetAtomAt(i))+" maxBondOrder:"+aliphaticChain.GetMaximumBondOrder(aliphaticChain.GetAtomAt(i))+" Aromatic:"+aliphaticChain.GetAtomAt(i).IsAromatic+" Ring:"+aliphaticChain.GetAtomAt(i).IsInRing+" FormalCharge:"+aliphaticChain.GetAtomAt(i).FormalCharge+" Charge:"+aliphaticChain.GetAtomAt(i).Charge+" Flag:"+aliphaticChain.GetAtomAt(i).IsVisited);
            //}
            //Debug.WriteLine("BondCount:"+aliphaticChain.Bonds.Count);
            if (aliphaticChain.Bonds.Count == 0)
            {
                aliphaticChain.RemoveAllElements();
            }
            return aliphaticChain;
        }

        /// <summary>
        ///  Performs a breadthFirstSearch in an AtomContainer starting with a
        ///  particular sphere, which usually consists of one start atom, and searches
        ///  for a pi system.
        /// </summary>
        /// <param name="container">The AtomContainer to be searched</param>
        /// <param name="sphere">A sphere of atoms to start the search with</param>
        /// <param name="path">A vector which stores the atoms belonging to the pi system</param>
        /// <exception cref="CDKException"></exception>
        private void BreadthFirstSearch(IAtomContainer container, List<IAtom> sphere, List<IAtom> path)
        {
            IAtom nextAtom;
            List<IAtom> newSphere = new List<IAtom>();
            foreach (var atom in sphere)
            {
                var bonds = container.GetConnectedBonds(atom);
                foreach (var bond in bonds)
                {
                    nextAtom = bond.GetConnectedAtom(atom);
                    if ((!nextAtom.IsAromatic && !nextAtom.IsInRing
                            & nextAtom.Symbol.Equals("C"))
                            & !nextAtom.IsVisited)
                    {
                        path.Add(nextAtom);
                        nextAtom.IsVisited = true;
                        if (container.GetConnectedBonds(nextAtom).Count() > 1)
                        {
                            newSphere.Add(nextAtom);
                        }
                    }
                    else
                    {
                        nextAtom.IsVisited = true;
                    }
                }
            }
            if (newSphere.Count > 0)
            {
                BreadthFirstSearch(container, newSphere, path);
            }
        }

        /// <summary>
        /// The parameterNames attribute of the LongestAliphaticChainDescriptor object.
        /// </summary>
        public override string[] ParameterNames { get; } = new string[] { "checkRingSystem" };

        /// <summary>
        ///  Gets the parameterType attribute of the LongestAliphaticChainDescriptor object.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>An Object of class equal to that of the parameter being requested</returns>
        public override object GetParameterType(string name)
        {
            return true;
        }
    }
}
