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
using System.Linq;

namespace NCDK.SMSD.Ring
{
    /// <summary>
    ///
    // @cdk.module smsd
    // @cdk.githash
    // @author Richard L. Apodaca <rapodaca at metamolecular.com> 2007-2009,
    ///         Syed Asad Rahman <asad@ebi.ac.uk> 2009-2010
    /// </summary>
    public class PathGraph
    {
        private List<PathEdge> edges;
        private List<IAtom> atoms;
        private IAtomContainer mol;

        public PathGraph(IAtomContainer molecule)
        {
            edges = new List<PathEdge>();
            atoms = new List<IAtom>();
            this.mol = molecule;

            LoadEdges(molecule);
            LoadNodes(molecule);
        }

        public void PrintPaths()
        {
            foreach (var edge in edges)
            {
                if (edge.IsCycle)
                {
                    Console.Out.Write("*");
                }

                foreach (var atom in edge.Atoms)
                {
                    Console.Out.Write(mol.Atoms.IndexOf(atom) + "-");
                }

                Console.Out.WriteLine();
            }
        }

        public IList<PathEdge> Remove(IAtom atom)
        {
            var oldEdges = GetEdges(atom).ToList();
            var result = new List<PathEdge>();

            foreach (var edge in oldEdges)
            {
                if (edge.IsCycle)
                {
                    result.Add(edge);
                }
            }

            foreach (var r in result)
            {
                oldEdges.Remove(r);
                edges.Remove(r);
            }

            var newEdges = SpliceEdges(oldEdges);
            foreach (var oldEdge in oldEdges)
            {
                edges.Remove(oldEdge);
            }
            edges.AddRange(newEdges);
            atoms.Remove(atom);

            return result;
        }

        private IEnumerable<PathEdge> SpliceEdges(List<PathEdge> edges)
        {
            for (int i = 0; i < edges.Count; i++)
            {
                for (int j = i + 1; j < edges.Count; j++)
                {
                    PathEdge splice = edges[j].Splice(edges[i]);

                    if (splice != null)
                    {
                        yield return splice;
                    }
                }
            }
            yield break;
        }

        private IEnumerable<PathEdge> GetEdges(IAtom atom)
        {
            foreach (var edge in edges)
            {
                if (edge.IsCycle)
                {
                    if (edge.Atoms.Contains(atom))
                    {
                        yield return edge;
                    }
                }
                else
                {
                    if ((edge.Source == atom) || (edge.Target == atom))
                    {
                        yield return edge;
                    }
                }
            }
            yield break;
        }

        private void LoadEdges(IAtomContainer molecule)
        {
            for (int i = 0; i < molecule.Bonds.Count; i++)
            {
                IBond bond = molecule.Bonds[i];
                edges.Add(new PathEdge(new List<IAtom>() { bond.Atoms[0], bond.Atoms[1] }));
            }
        }

        private void LoadNodes(IAtomContainer molecule)
        {
            for (int i = 0; i < molecule.Atoms.Count; i++)
            {
                atoms.Add(molecule.Atoms[i]);
            }
        }
    }
}
