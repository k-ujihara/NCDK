/*  Copyright (C) 2008  Arvid Berg <goglepox@users.sf.net>
 *                2011  Jonty Lawson <jontyl@users.sourceforge.net>
 *
 *  Contact: cdk-devel@list.sourceforge.net
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
using NCDK.Geometries;
using NCDK.Renderers.Elements;
using NCDK.Renderers.Generators.Parameters;
using System;
using System.Collections.Generic;
using static NCDK.Renderers.Generators.Standards.VecmathUtil;
using WPF = System.Windows;

namespace NCDK.Renderers.Generators
{
    /// <summary>
    /// Generates just the aromatic indicators for rings : circles, or light-gray inner bonds, depending on the value of CDKStyleAromaticity.
    /// </summary>
    // @cdk.module renderbasic
    // @cdk.githash
    public class RingGenerator : BasicBondGenerator
    {
        /// <summary>
        /// Determines whether rings should be drawn with a circle if they are
        /// aromatic.
        /// </summary>
        public class ShowAromaticity : AbstractGeneratorParameter<bool?>
        {
            /// <summary> Returns the default value.
            /// <returns><see langword="true"/></returns>
            public override bool? Default => true;
        }

        private IGeneratorParameter<bool?> showAromaticity = new ShowAromaticity();

        /// <summary>
        /// Depicts aromaticity of rings in the original CDK style.
        /// </summary>
        public class CDKStyleAromaticity : AbstractGeneratorParameter<bool?>
        {
            /// <summary> Returns the default value.
            /// <returns><see langword="false"/></returns>
            public override bool? Default => false;
        }

        /// <summary>If true, the aromatic ring is indicated by light gray inner bonds</summary>
        private IGeneratorParameter<bool?> cdkStyleAromaticity = new CDKStyleAromaticity();

        /// <summary>
        /// The maximum ring size for which an aromatic ring should be drawn.
        /// </summary>
        public class MaxDrawableAromaticRing : AbstractGeneratorParameter<int?>
        {
            /// <summary>
            /// The maximum default ring size for which an aromatic ring should be drawn.
            /// </summary>
            /// <returns>the maximum ring size</returns>
            public override int? Default => 8;
        }

        private IGeneratorParameter<int?> maxDrawableAromaticRing = new MaxDrawableAromaticRing();

        /// <summary>
        /// The proportion of a ring bounds to use to draw the ring.
        /// </summary>
        public class RingProportion : AbstractGeneratorParameter<double?>
        {
            /// <summary> Returns the default value.
            /// <returns>0.35</returns>
            public override double? Default => 0.35;
        }

        private IGeneratorParameter<double?> ringProportion = new RingProportion();

        /// <summary>
        /// The rings that have already been painted - that is, a ring element
        /// has been generated for it.
        /// </summary>
        private ISet<IRing> painted_rings;

        /// <summary>
        /// Make a generator for ring elements.
        /// </summary>
        public RingGenerator()
        {
            this.painted_rings = new HashSet<IRing>();
        }

        /// <inheritdoc/>
        public override IRenderingElement GenerateRingElements(IBond bond, IRing ring, RendererModel model)
        {
            if (RingIsAromatic(ring) && showAromaticity.Value.Value
                    && ring.Atoms.Count < maxDrawableAromaticRing.Value)
            {
                ElementGroup pair = new ElementGroup();
                if (cdkStyleAromaticity.Value.Value)
                {
                    pair.Add(GenerateBondElement(bond, BondOrder.Single, model));
                    base.SetOverrideColor(WPF.Media.Colors.LightGray);
                    pair.Add(GenerateInnerElement(bond, ring, model));
                    base.SetOverrideColor(null);
                }
                else
                {
                    pair.Add(GenerateBondElement(bond, BondOrder.Single, model));
                    if (!painted_rings.Contains(ring))
                    {
                        painted_rings.Add(ring);
                        pair.Add(GenerateRingRingElement(bond, ring, model));
                    }
                }
                return pair;
            }
            else
            {
                return base.GenerateRingElements(bond, ring, model);
            }
        }

        private IRenderingElement GenerateRingRingElement(IBond bond, IRing ring, RendererModel model)
        {
            var c = ToPoint(GeometryUtil.Get2DCenter(ring));

            double[] minmax = GeometryUtil.GetMinMax(ring);
            double width = minmax[2] - minmax[0];
            double height = minmax[3] - minmax[1];
            double radius = Math.Min(width, height) * ringProportion.Value.Value;
            var color = GetColorForBond(bond, model);

            return new OvalElement(c, radius, false, color);
        }

        private bool RingIsAromatic(IRing ring)
        {
            bool isAromatic = true;
            foreach (var atom in ring.Atoms)
            {
                if (!atom.IsAromatic)
                {
                    isAromatic = false;
                    break;
                }
            }
            if (!isAromatic)
            {
                isAromatic = true;
                foreach (var b in ring.Bonds)
                {
                    if (!b.IsAromatic)
                    {
                        return false;
                    }
                }
            }
            return isAromatic;
        }

        /// <inheritdoc/>
        public override IList<IGeneratorParameter> Parameters
        {
            get
            {
                // Get our super class's version of things
                var superPars = base.Parameters;

                // Allocate ArrayList with sufficient space for everything.
                // Note that the number should ideally be the same as the number of entries
                // that we add here, though this is *only* an efficiency consideration.
                var pars = new List<IGeneratorParameter>(superPars.Count + 3);

                pars.AddRange(superPars);
                pars.Add(cdkStyleAromaticity);
                pars.Add(showAromaticity);
                pars.Add(maxDrawableAromaticRing);
                pars.Add(ringProportion);
                return pars;
            }
        }
    }
}
