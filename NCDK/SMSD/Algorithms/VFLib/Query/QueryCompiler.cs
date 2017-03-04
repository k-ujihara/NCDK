/* Copyright (C) 2009-2010  Syed Asad Rahman <asad@ebi.ac.uk>
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
 *
 * MX Cheminformatics Tools for Java
 *
 * Copyright (c) 2007-2009 Metamolecular, LLC
 *
 * http://metamolecular.com
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
using NCDK.Isomorphisms.Matchers;
using NCDK.SMSD.Algorithms.Matchers;
using NCDK.SMSD.Algorithms.VFLib.Builder;


namespace NCDK.SMSD.Algorithms.VFLib.Query
{
    /// <summary>
    /// This class creates an template for MCS/substructure query.
    // @cdk.module smsd
    // @cdk.githash
    // @author Syed Asad Rahman <asad@ebi.ac.uk>
    /// </summary>
    public class QueryCompiler : IQueryCompiler
    {

        private IAtomContainer molecule = null;
        private IQueryAtomContainer queryMolecule = null;
        private bool shouldMatchBonds = true;

        /// <summary>
        /// Construct query object from the molecule
        /// <param name="molecule">/// @param shouldMatchBonds</param>
        /// </summary>
        public QueryCompiler(IAtomContainer molecule, bool shouldMatchBonds)
        {
            this.molecule = molecule;
            this.IsBondMatchFlag = shouldMatchBonds;
        }

        /// <summary>
        /// Construct query object from the molecule
        /// <param name="molecule">/// </summary></param>
        public QueryCompiler(IQueryAtomContainer molecule)
        {
            this.SetQueryMolecule(molecule);
        }

        /// <summary>
        /// Set Molecule
        /// <param name="molecule">/// </summary></param>
        private void SetMolecule(IAtomContainer molecule)
        {
            this.molecule = molecule;
        }

        /// <summary>
        /// Set Molecule
        /// <param name="molecule">/// </summary></param>
        private void SetQueryMolecule(IQueryAtomContainer molecule)
        {
            this.queryMolecule = molecule;
        }

        /// <summary>
        /// Return molecule
        /// <returns>Atom Container</returns>
        /// </summary>
        private IAtomContainer Molecule => queryMolecule == null ? molecule : queryMolecule;

        public IQuery Compile()
        {
            return this.queryMolecule == null ? Build(molecule) : Build(queryMolecule);
        }

        private IQuery Build(IAtomContainer queryMolecule)
        {
            VFQueryBuilder result = new VFQueryBuilder();
            foreach (var atom in queryMolecule.Atoms)
            {
                VFAtomMatcher matcher = CreateAtomMatcher(queryMolecule, atom);
                if (matcher != null)
                {
                    result.AddNode(matcher, atom);
                }
            }
            for (int i = 0; i < queryMolecule.Bonds.Count; i++)
            {
                IBond bond = queryMolecule.Bonds[i];
                IAtom atomI = bond.Atoms[0];
                IAtom atomJ = bond.Atoms[1];
                result.Connect(result.GetNode(atomI), result.GetNode(atomJ), CreateBondMatcher(queryMolecule, bond));
            }
            return result;
        }

        private IQuery Build(IQueryAtomContainer queryMolecule)
        {
            VFQueryBuilder result = new VFQueryBuilder();
            foreach (var atoms in queryMolecule.Atoms)
            {
                IQueryAtom atom = (IQueryAtom)atoms;
                VFAtomMatcher matcher = CreateAtomMatcher(atom, queryMolecule);
                if (matcher != null)
                {
                    result.AddNode(matcher, atom);
                }
            }
            for (int i = 0; i < queryMolecule.Bonds.Count; i++)
            {
                IBond bond = queryMolecule.Bonds[i];
                IQueryAtom atomI = (IQueryAtom)bond.Atoms[0];
                IQueryAtom atomJ = (IQueryAtom)bond.Atoms[1];
                result.Connect(result.GetNode(atomI), result.GetNode(atomJ), CreateBondMatcher((IQueryBond)bond));
            }
            return result;
        }

        private VFAtomMatcher CreateAtomMatcher(IAtomContainer mol, IAtom atom)
        {
            return new DefaultVFAtomMatcher(mol, atom, IsBondMatchFlag);
        }

        private VFBondMatcher CreateBondMatcher(IAtomContainer mol, IBond bond)
        {
            return new DefaultVFBondMatcher(mol, bond, IsBondMatchFlag);
        }

        private VFAtomMatcher CreateAtomMatcher(IQueryAtom atom, IQueryAtomContainer container)
        {
            return new DefaultVFAtomMatcher(atom, container);
        }

        private VFBondMatcher CreateBondMatcher(IQueryBond bond)
        {
            return new DefaultVFBondMatcher(bond);
        }

        /// <summary>
        /// <returns>the shouldMatchBonds</returns>
        /// </summary>
        private bool IsBondMatchFlag
        {
            get
            {
                return shouldMatchBonds;
            }
            set
            {
                this.shouldMatchBonds = value;
            }
        }
    }
}
