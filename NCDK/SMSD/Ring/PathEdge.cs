/*
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
 *
 * Copyright (C) 2009-2010  Syed Asad Rahman <asad@ebi.ac.uk>
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
 */
using System;
using System.Collections.Generic;

namespace NCDK.SMSD.Ring
{
    // @cdk.module smsd
    // @cdk.githash
    // @author Richard L. Apodaca <rapodaca at metamolecular.com> 2007-2009, Syed Asad Rahman <asad@ebi.ac.uk> 2009-2010
    public class PathEdge
    {
        private IList<IAtom> atoms;

        public PathEdge(IList<IAtom> atoms)
        {
            this.atoms = atoms;
        }

        public IList<IAtom> Atoms => atoms;

        public IAtom Source => atoms[0];

        public IAtom Target => atoms[atoms.Count - 1];

        public bool IsCycle => (atoms.Count > 2) && atoms[0].Equals(atoms[atoms.Count - 1]);

        public PathEdge Splice(PathEdge other)
        {
            IAtom intersection = GetIntersection(other.atoms);
            List<IAtom> newAtoms = new List<IAtom>(atoms);

            if (atoms[0] == intersection)
            {
                newAtoms.Reverse();
            }

            if (other.atoms[0] == intersection)
            {
                for (int i = 1; i < other.atoms.Count; i++)
                {
                    newAtoms.Add(other.atoms[i]);
                }
            }
            else
            {
                for (int i = other.atoms.Count - 2; i >= 0; i--)
                {
                    newAtoms.Add(other.atoms[i]);
                }
            }

            if (!IsRealPath(newAtoms))
            {
                return null;
            }

            return new PathEdge(newAtoms);
        }

        private bool IsRealPath(List<IAtom> atoms)
        {
            for (int i = 1; i < atoms.Count - 1; i++)
            {
                for (int j = 1; j < atoms.Count - 1; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    if (atoms[i] == atoms[j])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private IAtom GetIntersection(IList<IAtom> others)
        {
            if (atoms[atoms.Count - 1] == others[0]
                    || atoms[atoms.Count - 1] == others[others.Count - 1])
            {
                return atoms[atoms.Count - 1];
            }

            if (atoms[0] == others[0] || atoms[0] == others[others.Count - 1])
            {
                return atoms[0];
            }

            throw new ApplicationException("Couldn't splice - no intersection.");
        }
    }
}
