/* Copyright (C) 2008-2009  Gilleain Torrance <gilleain@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All I ask is that proper credit is given for my work, which includes
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
using System;
using System.Windows;

namespace NCDK.MolViewer.Renderers
{
    // @cdk.module renderbasic 
    public class BoundsCalculator
    {
        public static Rect CalculateBounds(IChemModel chemModel)
        {
            var moleculeSet = chemModel.MoleculeSet;
            var reactionSet = chemModel.ReactionSet;
            Rect totalBounds = Rect.Empty;
            if (moleculeSet != null)
            {
                totalBounds = CalculateBounds(moleculeSet);
            }
            if (reactionSet != null)
            {
                if (totalBounds.IsEmpty)
                {
                    totalBounds = CalculateBounds(reactionSet);
                }
                else
                {
                    totalBounds.Union(CalculateBounds(reactionSet));
                }
            }
            return totalBounds;
        }

        public static Rect CalculateBounds(IReactionSet reactionSet)
        {
            Rect totalBounds = new Rect();
            foreach (IReaction reaction in reactionSet)
            {
                Rect reactionBounds = CalculateBounds(reaction);
                if (totalBounds.IsEmpty)
                {
                    totalBounds = reactionBounds;
                }
                else
                {
                    totalBounds = Rect.Union(reactionBounds, totalBounds);
                }
            }
            return totalBounds;
        }

        public static Rect CalculateBounds(IReaction reaction)
        {
            // get the participants in the reaction
            var reactants = reaction.Reactants;
            var products = reaction.Products;
            if (reactants == null || products == null) return Rect.Empty;

            // determine the bounds of everything in the reaction
            Rect reactantsBounds = CalculateBounds(reactants);
            return Rect.Union(reactantsBounds, CalculateBounds(products));
        }

        public static Rect CalculateBounds(IChemObjectSet<IAtomContainer> moleculeSet)
        {
            Rect totalBounds = new Rect();
            foreach (var ac in moleculeSet)
            {
                Rect acBounds = CalculateBounds(ac);
                if (totalBounds.IsEmpty)
                {
                    totalBounds = acBounds;
                }
                else
                {
                    totalBounds = Rect.Union(acBounds, totalBounds);
                }
            }
            return totalBounds;
        }

        public static Rect CalculateBounds(IAtomContainer ac)
        {
            // this is essential, otherwise a rectangle
            // of (+INF, -INF, +INF, -INF) is returned!
            if (ac == null || ac.Atoms.Count == 0)
            {
                return new Rect();
            }
            else if (ac.Atoms.Count == 1)
            {
                var p = ac.Atoms[0].Point2D.Value;
                return new Rect(p.X, p.Y, 0, 0);
            }

            double xmin = double.PositiveInfinity;
            double xmax = double.NegativeInfinity;
            double ymin = double.PositiveInfinity;
            double ymax = double.NegativeInfinity;

            foreach (IAtom atom in ac.Atoms)
            {
                var p = atom.Point2D.Value;
                xmin = Math.Min(xmin, p.X);
                xmax = Math.Max(xmax, p.X);
                ymin = Math.Min(ymin, p.Y);
                ymax = Math.Max(ymax, p.Y);
            }
            double w = xmax - xmin;
            double h = ymax - ymin;
            return new Rect(xmin, ymin, w, h);
        }
    }
}
