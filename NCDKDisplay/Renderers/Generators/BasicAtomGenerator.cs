/*  Copyright (C) 2008  Arvid Berg <goglepox@users.sf.net>
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
using NCDK.Renderers.Colors;
using NCDK.Renderers.Elements;
using NCDK.Renderers.Generators.Parameters;
using NCDK.Validate;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using static NCDK.Renderers.Generators.BasicSceneGenerator;
using static NCDK.Renderers.Generators.Standards.VecmathUtil;
using WPF = System.Windows;

namespace NCDK.Renderers.Generators
{
    /// <summary>
    /// Generates basic <see cref="IRenderingElement"/>s for atoms in an atom container.
    /// </summary>
    // @cdk.module renderbasic
    // @cdk.githash
    public class BasicAtomGenerator : IGenerator<IAtomContainer>
    {
        /// <summary>
        /// Class to hold the color by which atom labels are drawn.
        /// This color is overwritten by the <see cref="IAtomContainer"/>.
        /// </summary>
        public class AtomColor : AbstractGeneratorParameter<Color?>
        {
            /// <summary>
            /// Returns the default value, <see cref="WPF.Media.Colors.Black"/>.
            /// </summary>
            public override Color? Default => WPF.Media.Colors.Black;
        }

        /// <summary>The default atom color.</summary>
        private IGeneratorParameter<Color?> atomColor = new AtomColor();

        /// <summary><see cref="IAtomContainer"/> used to draw elements.</summary>
        public class AtomColorer : AbstractGeneratorParameter<IAtomColorer>
        {
            /// <summary>
            /// Returns the default value, <see cref="CDK2DAtomColors"/>.
            /// </summary>
            public override IAtomColorer Default => new CDK2DAtomColors();
        }

        /// <summary>Converter between atoms and colors.</summary>
        private IGeneratorParameter<IAtomColorer> atomColorer = new AtomColorer();

        /// <summary> bool property that triggers atoms to be colored by type
        ///  when set to true.</summary>
        public class ColorByType : AbstractGeneratorParameter<bool?>
        {
            /// <summary> Returns the default value.</summary>
            /// <returns><see langword="true"/></returns>
            public override bool? Default => true;
        }

        /// <summary>If true, colors atoms by their type.</summary>
        private IGeneratorParameter<bool?> colorByType = new ColorByType();

        /// <summary>bool property that triggers explicit hydrogens to be
        ///  drawn if set to true.</summary>
        public class ShowExplicitHydrogens : AbstractGeneratorParameter<bool?>
        {
            /// <summary> Returns the default value.</summary>
            /// <returns><see langword="true"/></returns>
            public override bool? Default => true;
        }

        /// <summary>If true, explicit hydrogens are displayed.</summary>
        private IGeneratorParameter<bool?> showExplicitHydrogens = new ShowExplicitHydrogens();

        /// <summary> Magic number with unknown units that defines the radius
        ///  around an atom, e.g. used for highlighting atoms.</summary>
        public class AtomRadius : AbstractGeneratorParameter<double?>
        {
            /// <summary>
            /// Returns the default value. 8.0
            /// </summary>
            public override double? Default => 8;
        }

        /// <summary>The atom radius on screen.</summary>
        private IGeneratorParameter<double?> atomRadius = new AtomRadius();

        /// <summary> bool parameters that will cause atoms to be drawn as
        ///  filled shapes when set to true. The actual used shape used
        ///  is defined by the <see cref="CompactShape"/> parameter.</summary>
        public class CompactAtom : AbstractGeneratorParameter<bool?>
        {
            /// <summary> Returns the default value.</summary>
            /// <returns><see langword="false"/></returns>
            public override bool? Default => false;
        }

        /// <summary>If true, atoms are displayed as 'compact' symbols, not text.</summary>
        private IGeneratorParameter<bool?> isCompact = new CompactAtom();

        /// <summary>Determines whether structures should be drawn as Kekule structures, thus
        /// giving each carbon element explicitly, instead of not displaying the
        /// element symbol. Example C-C-C instead of /\.
        /// </summary>
        public class KekuleStructure : AbstractGeneratorParameter<bool?>
        {
            /// <summary> Returns the default value.</summary>
            /// <returns><see langword="false"/></returns>
            public override bool? Default => false;
        }

        /// <summary>
        /// Determines whether structures should be drawn as Kekule structures, thus
        /// giving each carbon element explicitly, instead of not displaying the
        /// element symbol. Example C-C-C instead of /\.
        /// </summary>
        private IGeneratorParameter<bool?> isKekule = new KekuleStructure();

        /// <summary>
        /// When atoms are selected or in compact mode, they will
        /// be covered by a shape determined by this enumeration.
        /// </summary>
        public enum Shapes
        {
            Oval, Square
        };

        /// <summary>
        /// Shape to be used when drawing atoms in compact mode,
        /// as defined by the <see cref="CompactAtom"/> parameter.
        /// </summary>
        public class CompactShape : AbstractGeneratorParameter<Shapes?>
        {
            /// <summary> Returns the default value.</summary>
            /// <returns>Shape.Square</returns>
            public override Shapes? Default => Shapes.Square;
        }

        /// <summary>The compact shape used to display atoms when isCompact is true.</summary>
        private IGeneratorParameter<Shapes?> compactShape = new CompactShape();

        /// <summary> bool parameters that will show carbons with only one
        /// (non-hydrogen) neighbor to be drawn with an element symbol.
        ///  This setting overwrites and is used in combination with
        ///  the <see cref="KekuleStructure"/> parameter.
        /// </summary>
        public class ShowEndCarbons : AbstractGeneratorParameter<bool?>
        {
            /// <summary> Returns the default value.</summary>
            /// <returns><see langword="false"/></returns>
            public override bool? Default => false;
        }

        /// <summary>
        /// Determines whether methyl carbons' symbols should be drawn explicit for
        /// methyl carbons. Example C/\C instead of /\.
        /// </summary>
        private IGeneratorParameter<bool?> showEndCarbons = new ShowEndCarbons();

        /// <summary>
        /// An empty constructor necessary for reflection.
        /// </summary>
        public BasicAtomGenerator() { }

        /// <inheritdoc/>
        public virtual IRenderingElement Generate(IAtomContainer container, RendererModel model)
        {
            ElementGroup elementGroup = new ElementGroup();
            foreach (var atom in container.Atoms)
            {
                elementGroup.Add(MarkedElement.MarkupAtom(this.Generate(container, atom, model), atom));
            }
            return elementGroup;
        }

        /// <summary>
        /// Checks an atom to see if it has 2D coordinates.
        /// </summary>
        /// <param name="atom">the atom to check</param>
        /// <returns>true if the atom is not null, and it has non-null coordinates</returns>
        protected internal virtual bool HasCoordinates(IAtom atom)
        {
            return atom != null && atom.Point2D != null;
        }

        /// <summary>
        /// Determines if the atom is a hydrogen.
        /// </summary>
        /// <param name="atom"><see cref="IAtom"/> to be tested</param>
        /// <returns>true, if the atom is a hydrogen, and false, otherwise.</returns>
        protected virtual bool IsHydrogen(IAtom atom)
        {
            return "H".Equals(atom.Symbol);
        }

        /// <summary>
        /// Determines if the atom is a carbon.
        /// </summary>
        /// <param name="atom"><see cref="IAtom"/> to be tested</param>
        /// <returns>true, if the atom is a carbon, and false, otherwise.</returns>
        private bool IsCarbon(IAtom atom)
        {
            return "C".Equals(atom.Symbol);
        }

        /// <summary>
        /// Checks an atom to see if it is an 'invisible hydrogen' - that is, it
        /// is a) an (explicit) hydrogen, and b) explicit hydrogens are set to off.
        /// </summary>
        /// <param name="atom">the atom to check</param>
        /// <param name="model">the renderer model</param>
        /// <returns>true if this atom should not be shown</returns>
        protected internal virtual bool InvisibleHydrogen(IAtom atom, RendererModel model)
        {
            return IsHydrogen(atom) && !model.GetV<bool>(typeof(ShowExplicitHydrogens));
        }

        /// <summary>
        /// Checks an atom to see if it is an 'invisible carbon' - that is, it is:
        /// a) a carbon atom and b) this carbon should not be shown.
        /// </summary>
        /// <param name="atom">the atom to check</param>
        /// <param name="atomContainer">the atom container the atom is part of</param>
        /// <param name="model">the renderer model</param>
        /// <returns>true if this atom should not be shown</returns>
        protected internal virtual bool InvisibleCarbon(IAtom atom, IAtomContainer atomContainer, RendererModel model)
        {
            return IsCarbon(atom) && !ShowCarbon(atom, atomContainer, model);
        }

        /// <summary>
        /// Checks an atom to see if it should be drawn. There are three reasons
        /// not to draw an atom - a) no coordinates, b) an invisible hydrogen or
        /// c) an invisible carbon.
        /// </summary>
        /// <param name="atom">the atom to check</param>
        /// <param name="container">the atom container the atom is part of</param>
        /// <param name="model">the renderer model</param>
        /// <returns>true if the atom should be drawn</returns>
        protected internal virtual bool CanDraw(IAtom atom, IAtomContainer container, RendererModel model)
        {
            // don't draw atoms without coordinates
            if (!HasCoordinates(atom))
            {
                return false;
            }

            // don't draw invisible hydrogens
            if (InvisibleHydrogen(atom, model))
            {
                return false;
            }

            // don't draw invisible carbons
            if (InvisibleCarbon(atom, container, model))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Generate the rendering Element(s) for a particular atom.
        /// </summary>
        /// <param name="atomContainer">the atom container that the atom is from</param>
        /// <param name="atom">the atom to generate the rendering element for</param>
        /// <param name="model">the renderer model</param>
        /// <returns>a rendering element, or group of elements</returns>
        public virtual IRenderingElement Generate(IAtomContainer atomContainer, IAtom atom, RendererModel model)
        {
            if (!CanDraw(atom, atomContainer, model))
            {
                return null;
            }
            else if (model.GetV<bool>(typeof(CompactAtom)))
            {
                return this.GenerateCompactElement(atom, model);
            }
            else
            {
                int alignment = 0;
                if (atom.Symbol.Equals("C"))
                {
                    alignment = GeometryUtil.GetBestAlignmentForLabel(atomContainer, atom);
                }
                else
                {
                    alignment = GeometryUtil.GetBestAlignmentForLabelXY(atomContainer, atom);
                }

                return GenerateElement(atom, alignment, model);
            }
        }

        /// <summary>
        /// Generate a compact element for an atom, such as a circle or a square,
        /// rather than text element.
        /// </summary>
        /// <param name="atom">the atom to generate the compact element for</param>
        /// <param name="model">the renderer model</param>
        /// <returns>a compact rendering element</returns>
        public virtual IRenderingElement GenerateCompactElement(IAtom atom, RendererModel model)
        {
            var point = atom.Point2D.Value;
            double radius = model.GetV<double>(typeof(AtomRadius)) / model.GetV<double>(typeof(Scale));
            double distance = 2 * radius;
            if (model.GetV<Shapes>(typeof(CompactShape)) == Shapes.Square)
            {
                return new RectangleElement(
                    new WPF.Point(point.X - radius, point.Y - radius), 
                    distance, distance, 
                    true, GetAtomColor(atom, model));
            }
            else
            {
                return new OvalElement(ToPoint(point), radius, true, GetAtomColor(atom, model));
            }
        }

        /// <summary>
        /// Generate an atom symbol element.
        /// </summary>
        /// <param name="atom">the atom to use</param>
        /// <param name="alignment">the alignment of the atom's label</param>
        /// <param name="model">the renderer model</param>
        /// <returns>an atom symbol element</returns>
        public virtual AtomSymbolElement GenerateElement(IAtom atom, int alignment, RendererModel model)
        {
            string text;
            if (atom is IPseudoAtom)
            {
                text = ((IPseudoAtom)atom).Label;
            }
            else
            {
                text = atom.Symbol;
            }
            return new AtomSymbolElement(
                ToPoint(atom.Point2D.Value), text, atom.FormalCharge,
                atom.ImplicitHydrogenCount, alignment, GetAtomColor(atom, model));
        }

        /// <summary>
        /// Checks a carbon atom to see if it should be shown.
        /// </summary>
        /// <param name="carbonAtom">the carbon atom to check</param>
        /// <param name="container">the atom container</param>
        /// <param name="model">the renderer model</param>
        /// <returns>true if the carbon should be shown</returns>
        public virtual bool ShowCarbon(IAtom carbonAtom, IAtomContainer container, RendererModel model)
        {
            if (model.GetV<bool>(typeof(KekuleStructure))) return true;
            if (carbonAtom.FormalCharge != 0) return true;
            int connectedBondCount = container.GetConnectedBonds(carbonAtom).Count();
            if (connectedBondCount < 1) return true;
            if ((bool)model.GetV<bool>(typeof(ShowEndCarbons)) && connectedBondCount == 1) return true;
            if (carbonAtom.GetProperty<bool>(ProblemMarker.ErrorMarker, false)) return true;
            if (container.GetConnectedSingleElectrons(carbonAtom).Any()) return true;
            return false;
        }

        /// <summary>
        /// Returns the drawing color of the given atom. An atom is colored as
        /// highlighted if highlighted. The atom is color marked if in a
        /// substructure. If not, the color from the CDK2DAtomColor is used (if
        /// selected). Otherwise, the atom is colored black.
        /// </summary>
        protected internal virtual Color GetAtomColor(IAtom atom, RendererModel model)
        {
            var atomColor = model.GetV<Color>(typeof(AtomColor));
            if (model.GetV<bool>(typeof(ColorByType)))
            {
                atomColor = model.Get<IAtomColorer>(typeof(AtomColorer)).GetAtomColor(atom);
            }
            return atomColor;
        }

        /// <inheritdoc/>
        public virtual IList<IGeneratorParameter> Parameters
                => new IGeneratorParameter[] { atomColor, atomColorer, atomRadius, colorByType, compactShape, isCompact, isKekule, showEndCarbons, showExplicitHydrogens };
    }
}
