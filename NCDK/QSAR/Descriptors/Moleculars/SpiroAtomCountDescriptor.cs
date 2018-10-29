/* Copyright (C) 2018  Rajarshi Guha <rajarshi.guha@gmail.com>
 *
 * Contact: cdk-devel@lists.sourceforge.net
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

using NCDK.Graphs;
using NCDK.QSAR.Results;
using System;
using System.Collections.Generic;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// Returns the number of spiro atoms.
    /// </summary>
    // @author rguha
    // @cdk.dictref qsar-descriptors:nSpiroAtom
    public class SpiroAtomCountDescriptor : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        private readonly static string[] NAMES = { "nSpiroAtoms" };

        public SpiroAtomCountDescriptor()
        {
        }

        public override void Initialise(IChemObjectBuilder builder)
        {
        }

        /// <inheritdoc/>
        public override IImplementationSpecification Specification => specification;
        private static readonly DescriptorSpecification specification =
         new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#nSpiroAtom",
                typeof(KierHallSmartsDescriptor).FullName,
                "The Chemistry Development Kit");

        /// <inheritdoc/>
        public override IReadOnlyList<object> Parameters
        {
            get => null;
            set { }
        }

        /// <inheritdoc/>
        public override IReadOnlyList<string> DescriptorNames => NAMES;

        private static void TraverseRings(IAtomContainer mol, IAtom atom, IBond prev)
        {
            atom.IsVisited = true;
            prev.IsVisited = true;
            foreach (var bond in mol.GetConnectedBonds(atom))
            {
                var nbr = bond.GetOther(atom);
                if (!nbr.IsVisited)
                    TraverseRings(mol, nbr, bond);
                else
                    bond.IsVisited = true;
            }
        }

        private static int GetSpiroDegree(IAtomContainer mol, IAtom atom)
        {
            if (!atom.IsInRing)
                return 0;
            var rbonds = new List<IBond>(4);
            foreach (var bond in mol.GetConnectedBonds(atom))
            {
                if (bond.IsInRing)
                    rbonds.Add(bond);
            }
            if (rbonds.Count < 4)
                return 0;
            int degree = 0;
            // clear flags
            foreach (var b in mol.Bonds)
                b.IsVisited = false;
            foreach (var a in mol.Atoms)
                a.IsVisited = false;
            // visit rings
            atom.IsVisited = true;
            foreach (var rbond in rbonds)
            {
                if (!rbond.IsVisited)
                {
                    TraverseRings(mol, rbond.GetOther(atom), rbond);
                    degree++;
                }
            }
            return degree < 2 ? 0 : degree;
        }

        public DescriptorValue<Result<int>> Calculate(IAtomContainer atomContainer)
        {
            int nSpiro = 0;

            var local = (IAtomContainer)atomContainer.Clone();
            Cycles.MarkRingAtomsAndBonds(local);
            foreach (var atom in local.Atoms)
            {
                if (GetSpiroDegree(local, atom) != 0)
                    nSpiro++;
            }
            return new DescriptorValue<Result<int>>(specification, ParameterNames, Parameters, new Result<int>(nSpiro), DescriptorNames);
        }

        /// <inheritdoc/>
        public override IDescriptorResult DescriptorResultType { get; } = new Result<int>();

        /// <inheritdoc/>
        public override IReadOnlyList<string> ParameterNames => Array.Empty<string>();

        /// <inheritdoc/>
        public override object GetParameterType(string name) => null;

        IDescriptorValue IMolecularDescriptor.Calculate(IAtomContainer container) => Calculate(container);
    }
}
