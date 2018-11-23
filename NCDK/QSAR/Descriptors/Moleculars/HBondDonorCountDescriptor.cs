/* Copyright (C) 2004-2007  The Chemistry Development Kit (CDK) project
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

using NCDK.Aromaticities;
using NCDK.Config;
using NCDK.Tools.Manipulator;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// This descriptor calculates the number of hydrogen bond donors using a slightly simplified version of the
    /// <see href="http://www.chemie.uni-erlangen.de/model2001/abstracts/rester.html">PHACIR atom types</see>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The following groups are counted as hydrogen bond donors:
    /// <list type="bullet"> 
    /// <item>Any-OH where the formal charge of the oxygen is non-negative (i.e. formal charge >= 0)</item>
    /// <item>Any-NH where the formal charge of the nitrogen is non-negative (i.e. formal charge >= 0)</item>
    /// </list>
    /// </para>
    /// <para>
    /// This descriptor uses no parameters.
    /// </para>
    /// <para>
    /// This descriptor works properly with AtomContainers whose atoms contain either <b>implicit</b> or <b>explicit
    /// hydrogen</b> atoms. It does not work with atoms that contain neither implicit nor explicit hydrogens.
    /// </para>
    /// </remarks>
    // @author      ulif
    // @cdk.created 2005-22-07
    // @cdk.module  qsarmolecular
    // @cdk.dictref qsar-descriptors:hBondDonors
    [DescriptorSpecification("http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#hBondDonors")]
    public class HBondDonorCountDescriptor : AbstractDescriptor, IMolecularDescriptor
    {
        private readonly IAtomContainer container;

        public HBondDonorCountDescriptor(IAtomContainer container)
        {
            container = (IAtomContainer)container.Clone(); // don't mod original
            this.container = container;
        }

        [DescriptorResult]
        public class Result : AbstractDescriptorResult
        {
            public Result(int value)
            {
                this.NumberOfHBondDonor = value;
            }

            [DescriptorResultProperty("nHBDon")]
            public int NumberOfHBondDonor { get; private set; }

            public int Value => NumberOfHBondDonor;
        }

        /// <summary>
        /// Calculates the number of H bond donors.
        /// </summary>
        /// <returns>number of H bond donors</returns>
        public Result Calculate()
        {
            int hBondDonors = 0;

            // iterate over all atoms of this AtomContainer; use label atomloop to allow for labelled continue

            //atomloop:
            for (int atomIndex = 0; atomIndex < container.Atoms.Count; atomIndex++)
            {
                var atom = container.Atoms[atomIndex];
                // checking for O and N atoms where the formal charge is >= 0
                switch (atom.AtomicNumber)
                {
                    case NaturalElements.O.AtomicNumber:
                    case NaturalElements.N.AtomicNumber:
                        if (atom.FormalCharge >= 0)
                        {
                            // implicit hydrogens
                            var implicitH = atom.ImplicitHydrogenCount ?? 0;
                            if (implicitH > 0)
                            {
                                hBondDonors++;
                                goto continue_atomloop; // we skip the explicit hydrogens part cause we found implicit hydrogens
                            }
                            // explicit hydrogens
                            var neighbours = container.GetConnectedAtoms(atom);
                            foreach (var neighbour in neighbours)
                            {
                                if ((neighbour).AtomicNumber.Equals(NaturalElements.H.AtomicNumber))
                                {
                                    hBondDonors++;
                                    goto continue_atomloop;
                                }
                            }
                        }
                        break;
                }
                continue_atomloop:
                ;
            }

            return new Result(hBondDonors);
        }

        IDescriptorResult IMolecularDescriptor.Calculate() => Calculate();
    }
}
