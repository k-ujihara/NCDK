/*
 * Copyright (c) 2014 European Bioinformatics Institute (EMBL-EBI)
 *                    John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 2.1 of the License, or (at
 * your option) any later version. All we ask is that proper credit is given
 * for our work, which includes - but is not limited to - adding the above
 * copyright notice to the beginning of your source code files, and to any
 * copyright notice that you may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using NCDK.Common.Mathematics;
using NCDK.Numerics;
using NCDK.Renderers.Colors;
using NCDK.Renderers.Elements;
using NCDK.Renderers.Generators.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using static NCDK.Renderers.Generators.Standards.VecmathUtil;
using WPF = System.Windows;

namespace NCDK.Renderers.Generators.Standards
{
    /// <summary>
    /// The standard generator creates <see cref="IRenderingElement"/>s for the atoms and bonds of a structure
    /// diagram. These are generated together allowing the bonds to drawn cleanly without overlap. The
    /// generate is heavily based on ideas documented in <token>cdk-cite-Brecher08</token> and <token>cdk-cite-Clark13</token>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Atom symbols are provided as <see cref="GeneralPath"/> outlines. This allows the depiction to be
    /// independent of the system used to view the diagram (primarily important for vector graphic
    /// depictions). The font used to generate the diagram must be provided to the constructor. <p/>
    /// </para>
    /// <para>
    /// Atoms and bonds can be highlighted by setting the <see cref="HIGHLIGHT_COLOR"/>. The style of
    /// highlight is set with the <see cref="Highlighting"/> parameter.
    /// </para>
    /// <para>
    /// The <see href="https://github.com/cdk/cdk/wiki/Standard-Generator">Standard Generator - CDK Wiki page</see>
    /// provides extended details of using and configuring this generator.
    /// </para>
    /// </remarks>
    /// <seealso href="https://github.com/cdk/cdk/wiki/Standard-Generator">Standard Generator - CDK Wiki</seealso>
    // @author John May
    public sealed class StandardGenerator : IGenerator<IAtomContainer>
    {
        /// <summary>
        /// Defines that a chem object should be highlighted in a depiction. Only atom symbols that are
        /// displayed are highlighted, the visibility of symbols can be modified with <see cref="SymbolVisibility"/>.
        /// </summary>
        /// <example>
        /// <code>
        /// atom.SetProperty(StandardGenerator.HIGHLIGHT_COLOR, WPF::Media.Colors.Red);
        /// </code></example>
        public const string HIGHLIGHT_COLOR = "stdgen.highlight.color";

        /// <summary>
        /// Defines the annotation Label(s) of a chem object in a depiction. The annotation
        /// must be a string.
        /// </summary>
        /// <example>
        /// <code>
        /// string number = Integer.ToString(1 + container.Atoms.IndexOf(atom));
        /// atom.SetProperty(CDKConstants.ANNOTATION_LABEL, number);
        /// </code></example>
        public const string ANNOTATION_LABEL = "stdgen.annotation.label";

        /// <summary>
        /// A special markup for annotation labels that hints the generator to renderer
        /// the annotation label in italic. The primary use case is for Cahn-Ingold-Prelog
        /// descriptors.
        /// </summary>
        /// <example><code>
        /// string cipLabel = "R";
        /// atom.SetProperty(CDKConstants.ANNOTATION_LABEL, StandardGenerator.ITALIC_DISPLAY_PREFIX + cipLabel);
        /// </code></example>
        public const string ITALIC_DISPLAY_PREFIX = "std.itl:";

        /// <summary>
        /// Marks atoms and bonds as being hidden from the actual depiction. Set this
        /// property to non-null to indicate this.
        /// </summary>
        public const string HIDDEN = "stdgen.hidden";
        public const string HIDDEN_FULLY = "stdgen.hidden.fully";

        private readonly Typeface font;
        private readonly double emSize;
        private readonly StandardAtomGenerator atomGenerator;

        /// <summary>
        /// Enumeration of highlight style.
        /// </summary>
        public enum HighlightStyle
        {
            /// <summary>
            /// Ignore highlight hints.
            /// </summary>
            None,

            /// <summary>
            /// Displayed atom symbols and bonds are coloured.
            /// </summary>
            Colored,

            /// <summary>
            /// An outer glow is placed in the background behind the depiction.
            /// </summary>
            /// <seealso cref="StandardGenerator.OuterGlowWidth"/> 
            OuterGlow
        }

        private readonly IGeneratorParameter atomColor = new AtomColor(), visibility = new Visibility(),
                strokeRatio = new StrokeRatio(), separationRatio = new BondSeparation(), wedgeRatio = new WedgeRatio(),
                marginRatio = new SymbolMarginRatio(), hatchSections = new HashSpacing(), dashSections = new DashSection(),
                waveSections = new WaveSpacing(), fancyBoldWedges = new FancyBoldWedges(),
                fancyHashedWedges = new FancyHashedWedges(), highlighting = new Highlighting(),
                glowWidth = new OuterGlowWidth(), annCol = new AnnotationColor(), annDist = new AnnotationDistance(),
                annFontSize = new AnnotationFontScale(), sgroupBracketDepth = new SgroupBracketDepth(),
                sgroupFontScale = new SgroupFontScale(), omitMajorIsotopes = new OmitMajorIsotopes();

        /// <summary>
        /// Create a new standard generator that utilises the specified font to display atom symbols.
        /// </summary>
        /// <param name="font">the font family, size, and style</param>
        public StandardGenerator(Typeface font, double emSize)
        {
            this.font = font;
            this.emSize = emSize;
            this.atomGenerator = new StandardAtomGenerator(font, emSize);
        }

        /// <inheritdoc/>
        public IRenderingElement Generate(IAtomContainer container, RendererModel parameters)
        {
            if (container.Atoms.Count == 0)
                return new ElementGroup();

            var symbolRemap = new Dictionary<IAtom, string>();
            StandardSgroupGenerator.PrepareDisplayShortcuts(container, symbolRemap);

            double scale = parameters.GetV<double>(typeof(BasicSceneGenerator.Scale));

            SymbolVisibility visibility = parameters.Get<SymbolVisibility>(typeof(Visibility));
            IAtomColorer coloring = parameters.Get<IAtomColorer>(typeof(AtomColor));
            Color annotationColor = parameters.GetV<Color>(typeof(AnnotationColor));
            var foreground = coloring.GetAtomColor(container.Builder.NewAtom("C"));

            // the stroke width is based on the font. a better method is needed to get
            // the exact font stroke but for now we use the width of the pipe character.
            double fontStroke = new TextOutline("|", font, emSize).Resize(1 / scale, 1 / scale).GetBounds().Width;
            double stroke = parameters.GetV<double>(typeof(StrokeRatio)) * fontStroke;

            ElementGroup annotations = new ElementGroup();

            AtomSymbol[] symbols = GenerateAtomSymbols(container, symbolRemap, visibility, parameters, annotations, foreground, stroke);
            IRenderingElement[] bondElements = StandardBondGenerator.GenerateBonds(container, symbols, parameters, stroke, font, emSize, annotations);

            HighlightStyle style = parameters.GetV<HighlightStyle>(typeof(Highlighting));
            double glowWidth = parameters.GetV<double>(typeof(OuterGlowWidth));

            ElementGroup backLayer = new ElementGroup();
            ElementGroup middleLayer = new ElementGroup();
            ElementGroup frontLayer = new ElementGroup();

            // bond elements can simply be added to the element group
            for (int i = 0; i < container.Bonds.Count; i++)
            {
                IBond bond = container.Bonds[i];

                if (IsHidden(bond))
                    continue;

                var highlight = GetHighlightColor(bond, parameters);
                if (highlight != null && style == HighlightStyle.OuterGlow)
                {
                    backLayer.Add(MarkedElement.Markup(OuterGlow(bondElements[i], highlight.Value, glowWidth, stroke), "outerglow"));
                }
                if (highlight != null && style == HighlightStyle.Colored)
                {
                    frontLayer.Add(MarkedElement.MarkupBond(Recolor(bondElements[i], highlight.Value), bond));
                }
                else
                {
                    middleLayer.Add(MarkedElement.MarkupBond(bondElements[i], bond));
                }
            }

            // convert the atom symbols to IRenderingElements
            for (int i = 0; i < container.Atoms.Count; i++)
            {
                IAtom atom = container.Atoms[i];

                if (IsHidden(atom))
                    continue;

                var highlight = GetHighlightColor(atom, parameters);
                Color color = GetColorOfAtom(symbolRemap, coloring, foreground, style, atom, highlight);

                if (symbols[i] == null)
                {
                    // we add a 'ball' around atoms with no symbols (e.g. carbons)
                    if (highlight != null && style == HighlightStyle.OuterGlow)
                    {
                        backLayer.Add(MarkedElement.Markup(new OvalElement(ToPoint(atom.Point2D.Value), 1.75 * glowWidth * stroke, true, highlight.Value), "outerglow"));
                    }
                    continue;
                }

                ElementGroup symbolElements = new ElementGroup();
                foreach (var shape in symbols[i].GetOutlines())
                {
                    GeneralPath path = GeneralPath.ShapeOf(shape, color);
                    symbolElements.Add(path);
                }

                // add the annotations of the symbol to the annotations ElementGroup
                foreach (var shape in symbols[i].GetAnnotationOutlines())
                {
                    annotations.Add(MarkedElement.Markup(GeneralPath.ShapeOf(shape, annotationColor), "annotation"));
                }

                if (highlight != null && style == HighlightStyle.OuterGlow)
                {
                    backLayer.Add(MarkedElement.Markup(OuterGlow(symbolElements, highlight.Value, glowWidth, stroke), "outerglow"));
                }

                if (highlight != null && style == HighlightStyle.Colored)
                {
                    frontLayer.Add(MarkedElement.MarkupAtom(symbolElements, atom));
                }
                else
                {
                    middleLayer.Add(MarkedElement.MarkupAtom(symbolElements, atom));
                }
            }

            // Add the Sgroups display elements to the front layer
            IRenderingElement sgroups = StandardSgroupGenerator.Generate(parameters, stroke, font, emSize, foreground, atomGenerator, symbols, container);
            frontLayer.Add(sgroups);

            // Annotations are added to the front layer.
            frontLayer.Add(annotations);

            ElementGroup group = new ElementGroup
            {
                backLayer,
                middleLayer,
                frontLayer
            };

            return MarkedElement.MarkupMol(group, container);
        }

        private Color GetColorOfAtom(IDictionary<IAtom, string> symbolRemap, IAtomColorer coloring, Color foreground,
                             HighlightStyle style, IAtom atom, Color? highlight)
        {
            // atom is highlighted...?
            if (highlight != null && style == HighlightStyle.Colored)
                return highlight.Value;
            // abbreviations default to foreground color
            if (symbolRemap.ContainsKey(atom))
                return foreground;
            // use the atom colorer
            return coloring.GetAtomColor(atom);
        }

        /// <summary>
        /// Generate the intermediate <see cref="AtomSymbol"/> instances.
        /// </summary>
        /// <param name="container">structure representation</param>
        /// <param name="symbolRemap">use alternate symbols (used for Sgroup shortcuts)</param>
        /// <param name="visibility">defines whether an atom symbol is displayed</param>
        /// <param name="parameters">render model parameters</param>
        /// <returns>generated atom symbols (can contain <see langword="null"/>)</returns>
        private AtomSymbol[] GenerateAtomSymbols(IAtomContainer container,
                                                IDictionary<IAtom, string> symbolRemap,
                                                SymbolVisibility visibility,
                                                RendererModel parameters, ElementGroup annotations,
                                                Color foreground,
                                                double stroke)
        {
            double scale = parameters.GetV<double>(typeof(BasicSceneGenerator.Scale));
            double annDist = parameters.GetV<double>(typeof(AnnotationDistance))
                              * (parameters.GetV<double>(typeof(BasicSceneGenerator.BondLength)) / scale);
            double annScale = (1 / scale) * parameters.GetV<double>(typeof(AnnotationFontScale));
            Color annColor = parameters.GetV<Color>(typeof(AnnotationColor));
            double halfStroke = stroke / 2;

            AtomSymbol[] symbols = new AtomSymbol[container.Atoms.Count];
            IChemObjectBuilder builder = container.Builder;

            // check if we should annotate attachment point numbers (maxAttach>1)
            // and queue them all up for processing
            var attachPoints = new List<IPseudoAtom>();
            int maxAttach = 0;

            for (int i = 0; i < container.Atoms.Count; i++)
            {
                var atom = container.Atoms[i];

                if (IsHiddenFully(atom))
                    continue;

                if (atom is IPseudoAtom)
                {
                    int attachNum = ((IPseudoAtom)atom).AttachPointNum;
                    if (attachNum > 0)
                        attachPoints.Add((IPseudoAtom)atom);
                    if (attachNum > maxAttach)
                        maxAttach = attachNum;
                }

                bool remapped = symbolRemap.ContainsKey(atom);
                var bonds = container.GetConnectedBonds(atom);
                var neighbors = container.GetConnectedAtoms(atom);
                var visNeighbors = new List<IAtom>();

                // if a symbol is remapped we only want to consider
                // visible neighbors in the alignment calculation, otherwise
                // we include all neighbors
                foreach (var neighbor in neighbors)
                {
                    if (!remapped || !IsHidden(neighbor))
                        visNeighbors.Add(neighbor);
                }

                var auxVectors = new List<Vector2>(1);

                // only generate if the symbol is visible
                if (visibility.Visible(atom, bonds, parameters) || remapped)
                {
                     HydrogenPosition hPosition = HydrogenPositionTools.Position(atom, visNeighbors);

                    if (atom.ImplicitHydrogenCount != null && atom.ImplicitHydrogenCount > 0)
                        auxVectors.Add(hPosition.Vector());

                    if (remapped)
                    {
                        symbols[i] = atomGenerator.GenerateAbbreviatedSymbol(symbolRemap[atom], hPosition);
                    }
                    else
                    {
                        symbols[i] = atomGenerator.GenerateSymbol(container, atom, hPosition, parameters);
                    }

                    if (symbols[i] != null)
                    {
                        // defines how the element is aligned on the atom point, when
                        // aligned to the left, the first character 'e.g. Cl' is used.
                        if (visNeighbors.Count < 4)
                        {
                            if (hPosition == HydrogenPosition.Left)
                            {
                                symbols[i] = symbols[i].AlignTo(AtomSymbol.SymbolAlignment.Right);
                            }
                            else
                            {
                                symbols[i] = symbols[i].AlignTo(AtomSymbol.SymbolAlignment.Left);
                            }
                        }

                        var p = atom.Point2D;

                        if (p == null)
                            throw new ArgumentException("Atom did not have 2D coordinates");

                        symbols[i] = symbols[i].Resize(1 / scale, 1 / -scale).Center(p.Value.X, p.Value.Y);
                    }
                }

                string label = GetAnnotationLabel(atom);

                if (label != null)
                {
                    // to ensure consistent draw distance we need to adjust the annotation distance
                    // depending on whether we are drawing next to an atom symbol or not.
                     double strokeAdjust = symbols[i] != null ? -halfStroke : 0;

                    var vector = NewAtomAnnotationVector(atom, bonds, auxVectors);
                    var annOutline = GenerateAnnotation(atom.Point2D.Value, label, vector, annDist + strokeAdjust, annScale, font, emSize, symbols[i]);

                    // the AtomSymbol may migrate during bond generation and therefore the annotation
                    // needs to be tied to the symbol. If no symbol is available the annotation is
                    // fixed and we can add it to the annotation ElementGroup right away.
                    if (symbols[i] != null)
                    {
                        symbols[i] = symbols[i].AddAnnotation(annOutline);
                    }
                    else
                    {
                        annotations.Add(GeneralPath.ShapeOf(annOutline.GetOutline(), annColor));
                    }
                }
            }

            // label attachment points
            if (maxAttach > 1)
            {
                var attachNumOutlines = new List<TextOutline>();
                double maxRadius = 0;

                foreach (IPseudoAtom atom in attachPoints)
                {
                    int attachNum = atom.AttachPointNum;

                    // to ensure consistent draw distance we need to adjust the annotation distance
                    // depending on whether we are drawing next to an atom symbol or not.
                     double strokeAdjust = -halfStroke;

   var vector = NewAttachPointAnnotationVector(atom,
                                                                           container.GetConnectedBonds(atom),
                                                                           new List<Vector2>());

                    var outline = GenerateAnnotation(atom.Point2D.Value,
                            attachNum.ToString(),
                            vector,
                            1.75 * annDist + strokeAdjust,
                            annScale,
                            new Typeface(font.FontFamily, font.Style, WPF.FontWeights.Bold, font.Stretch),
                            emSize,
                            null);

                    attachNumOutlines.Add(outline);

                    double w = outline.GetBounds().Width;
                    double h = outline.GetBounds().Height;
                    double r = Math.Sqrt(w * w + h * h) / 2;
                    if (r > maxRadius)
                        maxRadius = r;
                }

                foreach (TextOutline outline in attachNumOutlines)
                {
                    ElementGroup group = new ElementGroup();
                    double radius = 2 * stroke + maxRadius;
                    var shape = new EllipseGeometry(outline.GetCenter(), radius, radius);
                    var area1 = Geometry.Combine(shape, outline.GetOutline(), GeometryCombineMode.Exclude, Transform.Identity);
                    group.Add(GeneralPath.ShapeOf(area1, foreground));
                    annotations.Add(group);
                }
            }

            return symbols;
        }


        /// <summary>
        /// Generate an annotation 'label' for an atom (located at 'basePoint'). The label is offset from
        /// the basePoint by the provided 'distance' and 'direction'.
        /// </summary>
        /// <param name="basePoint">the relative (0,0) reference</param>
        /// <param name="label">the annotation text</param>
        /// <param name="direction">the direction along which the label is laid out</param>
        /// <param name="distance">the distance along the direct to travel</param>
        /// <param name="scale">the font scale of the label</param>
        /// <param name="font">the font to use</param>
        /// <param name="symbol">the atom symbol to avoid overlap with</param>
        /// <returns>the position text outline for the annotation</returns>
        internal static TextOutline GenerateAnnotation(Vector2 basePoint, string label, Vector2 direction, double distance, double scale, Typeface font, double emSize, AtomSymbol symbol)
        {
            bool italicHint = label.StartsWith(ITALIC_DISPLAY_PREFIX);

            label = italicHint ? label.Substring(ITALIC_DISPLAY_PREFIX.Length) : label;

            var annFont = italicHint ? new Typeface(font.FontFamily, WPF.FontStyles.Italic, font.Weight, font.Stretch) : font;

            TextOutline annOutline = new TextOutline(label, annFont, emSize).Resize(scale, -scale);

            // align to the first or last character of the annotation depending on the direction
            var center = direction.X > 0.3 ? annOutline.GetFirstGlyphCenter() : direction.X < -0.3 ? annOutline.GetLastGlyphCenter() : annOutline.GetCenter();

            // Avoid atom symbol
            if (symbol != null)
            {
                var intersect = symbol.GetConvexHull().Intersect(VecmathUtil.ToPoint(basePoint),
                       VecmathUtil.ToPoint(VecmathUtil.Sum(basePoint, direction)));
                // intersect should never be null be check against this
                if (intersect != null) basePoint = VecmathUtil.ToVector(intersect);
            }

            direction *= distance;
            direction += basePoint;

            // move to position
            return annOutline.Translate(direction.X - center.X, direction.Y - center.Y);
        }

        /// <summary>
        /// Make an embedded text label for display in a CDK renderer. If a piece of text contains newlines
        /// they are centred aligned below each other with a line height of 1.4.
        /// </summary>
        /// <param name="font">the font to embedded</param>
        /// <param name="text">the text label</param>
        /// <param name="color">the color</param>
        /// <param name="scale">the resize, should include the model scale</param>
        /// <returns>pre-rendered element</returns>
        public static IRenderingElement EmbedText(Typeface font, double emSize, string text, Color color, double scale)
        {
            string[] lines = text.Split('\n');

            ElementGroup group = new ElementGroup();

            double yOffset = 0;
            double lineHeight = 1.4d;

            foreach (var line in lines)
            {
                TextOutline outline = new TextOutline(line, font, emSize).Resize(scale, -scale);
                var center = outline.GetCenter();
                outline = outline.Translate(-center.X, -(center.Y + yOffset));

                yOffset += lineHeight * outline.GetBounds().Height;

                group.Add(GeneralPath.ShapeOf(outline.GetOutline(), color));
                var logicalBounds = outline.GetLogicalBounds();
                group.Add(new Bounds(logicalBounds.Left, logicalBounds.Top, logicalBounds.Right, logicalBounds.Bottom));
            }

            return group;
        }

        /// <inheritdoc/>
        public IList<IGeneratorParameter> Parameters =>
            new[] {atomColor, visibility, strokeRatio, separationRatio, wedgeRatio, marginRatio,
                    hatchSections, dashSections, waveSections, fancyBoldWedges, fancyHashedWedges, highlighting, glowWidth,
                    annCol, annDist, annFontSize, sgroupBracketDepth, sgroupFontScale, omitMajorIsotopes };

        internal static string GetAnnotationLabel(IChemObject chemObject)
        {
            object obj = chemObject.GetProperty<object>(ANNOTATION_LABEL);
            return obj is string ? (string)obj : null;
        }

        private Color? GetHighlightColor(IChemObject bond, RendererModel parameters)
        {
            var propCol = GetColorProperty(bond, HIGHLIGHT_COLOR);

            if (propCol != null)
            {
                return propCol;
            }

            if (parameters.GetSelection() != null && parameters.GetSelection().Contains(bond))
            {
                return parameters.GetV<Color>(typeof(RendererModel.SelectionColor));
            }

            return null;
        }

        /// <summary>
        /// Safely access a chem object color property for a chem object.
        /// </summary>
        /// <param name="obj">chem object</param>
        /// <returns>the highlight color</returns>
        /// <exception cref="ArgumentException">the highlight property was set but was not a <see cref="Color"/> instance</exception>
        internal static Color? GetColorProperty(IChemObject obj, string key)
        {
            var value = obj.GetProperty<object>(key);
            if (value is Color) return (Color)value;
            if (value != null) throw new ArgumentException(key + " property should be a java.awt.Color");
            return null;
        }

        /// <summary>
        /// Recolor a rendering element after it has been generated. Since rendering elements are
        /// immutable, the input element remains unmodified.
        /// </summary>
        /// <param name="element">the rendering element</param>
        /// <param name="color">the new color</param>
        /// <returns>recolored rendering element</returns>
        private static IRenderingElement Recolor(IRenderingElement element, Color color)
        {
            if (element is ElementGroup orgGroup)
            {
                ElementGroup newGroup = new ElementGroup();
                foreach (var child in orgGroup)
                {
                    newGroup.Add(Recolor(child, color));
                }
                return newGroup;
            }
            else if (element is LineElement lineElement)
            {
                return new LineElement(lineElement.firstPoint, lineElement.secondPoint, lineElement.width, color);
            }
            else if (element is GeneralPath)
            {
                return ((GeneralPath)element).Recolor(color);
            }
            throw new ArgumentException($"Cannot highlight rendering element, {element.GetType()}");
        }

        /// <summary>
        /// Generate an outer glow for the provided rendering element. The glow is defined by the glow
        /// width and the stroke size.
        /// </summary>
        /// <param name="element">rendering element</param>
        /// <param name="color">color of the glow</param>
        /// <param name="glowWidth">the width of the glow</param>
        /// <param name="stroke">the stroke width</param>
        /// <returns>generated outer glow</returns>
        internal static IRenderingElement OuterGlow(IRenderingElement element, Color color, double glowWidth, double stroke)
        {
            switch (element)
            {
                case ElementGroup orgGroup:
                    ElementGroup newGroup = new ElementGroup();
                    foreach (var child in orgGroup)
                    {
                        newGroup.Add(OuterGlow(child, color, glowWidth, stroke));
                    }
                    return newGroup;
                case LineElement lineElement:
                    return new LineElement(lineElement.firstPoint, lineElement.secondPoint, stroke + (2 * (glowWidth * stroke)), color);
                case GeneralPath org:
                    if (org.fill)
                    {
                        return org.Outline(2 * (glowWidth * stroke)).Recolor(color);
                    }
                    else
                    {
                        return org.Outline(stroke + (2 * (glowWidth * stroke))).Recolor(color);
                    }
                default:
                    throw new ArgumentException($"Cannot generate glow for rendering element,{element.GetType()}");
            }
        }

        /// <summary>
        /// Generate a new annotation vector for an atom using the connected bonds and any other occupied
        /// space (auxiliary vectors). The fall back method is to use the largest available space but
        /// some common cases are handled differently. For example, when the number of bonds is two
        /// the annotation is placed in the acute angle of the bonds (providing there is space). This
        /// improves labelling of atoms saturated rings. When there are three bonds and two are 'plain'
        /// the label is again placed in the acute section of the plain bonds.
        /// </summary>
        /// <param name="atom">the atom having an annotation</param>
        /// <param name="bonds">the bonds connected to the atom</param>
        /// <param name="auxVectors">additional vectors to avoid (filled spaced)</param>
        /// <returns>unit vector along which the annotation should be placed.</returns>
        /// <seealso cref="IsPlainBond(IBond)"/>
        /// <seealso cref="VecmathUtil.NewVectorInLargestGap(IList{Vector2})"/>
        internal static Vector2 NewAtomAnnotationVector(IAtom atom, IEnumerable<IBond> bonds, List<Vector2> auxVectors)
        {
            var vectors = new List<Vector2>();
            foreach (var bond in bonds)
                vectors.Add(VecmathUtil.NewUnitVector(atom, bond));

            if (vectors.Count == 0)
            {
                // no bonds, place below
                if (auxVectors.Count == 0) return new Vector2(0, -1);
                if (auxVectors.Count == 1) return VecmathUtil.Negate(auxVectors[0]);
                return VecmathUtil.NewVectorInLargestGap(auxVectors);
            }
            else if (vectors.Count == 1)
            {
                // 1 bond connected
                // H0, then label simply appears on the opposite side
                if (auxVectors.Count == 0) return VecmathUtil.Negate(vectors[0]);
                // !H0, then place it in the largest gap
                vectors.AddRange(auxVectors);
                return VecmathUtil.NewVectorInLargestGap(vectors);
            }
            else if (vectors.Count == 2 && auxVectors.Count == 0)
            {
                // 2 bonds connected to an atom with no hydrogen labels

                // sum the vectors such that the label appears in the acute/nook of the two bonds
                Vector2 combined = VecmathUtil.Sum(vectors[0], vectors[1]);

                // shallow angle (< 30 deg) means the label probably won't fit
                if (Vectors.Angle(vectors[0], vectors[1]) < Vectors.DegreeToRadian(65))
                    combined = Vector2.Negate(combined);

                else
                {
                    // flip vector if either bond is a non-single bond or a wedge, this will
                    // place the label in the largest space.
                    // However - when both bonds are wedged (consider a bridging system) to
                    // keep the label in the nook of the wedges
                    var bonds_ = bonds.ToList();
                    if ((!IsPlainBond(bonds_[0]) || !IsPlainBond(bonds_[1]))
                        && !(IsWedged(bonds_[0]) && IsWedged(bonds_[1]))) combined = Vector2.Negate(combined);
                }

                combined = Vector2.Normalize(combined);

                // did we divide by 0? whoops - this happens when the bonds are collinear
                if (double.IsNaN(combined.Length())) return VecmathUtil.NewVectorInLargestGap(vectors);

                return combined;
            }
            else
            {
                if (vectors.Count == 3 && auxVectors.Count == 0)
                {
                    // 3 bonds connected to an atom with no hydrogen label

                    // the easy and common case is to check when two bonds are plain
                    // (i.e. non-stereo sigma bonds) and use those. This gives good
                    // placement for fused conjugated rings

                    List<Vector2> plainVectors = new List<Vector2>();
                    List<Vector2> wedgeVectors = new List<Vector2>();

                    foreach (var bond in bonds)
                    {
                        if (IsPlainBond(bond)) plainVectors.Add(VecmathUtil.NewUnitVector(atom, bond));
                        if (IsWedged(bond)) wedgeVectors.Add(VecmathUtil.NewUnitVector(atom, bond));
                    }

                    if (plainVectors.Count == 2)
                    {
                        return VecmathUtil.Sum(plainVectors[0], plainVectors[1]);
                    }
                    else if (plainVectors.Count + wedgeVectors.Count == 2)
                    {
                        plainVectors.AddRange(wedgeVectors);
                        return VecmathUtil.Sum(plainVectors[0], plainVectors[1]);
                    }
                }

                // the default option is to find the largest gap
                if (auxVectors.Count > 0) vectors.AddRange(auxVectors);
                return VecmathUtil.NewVectorInLargestGap(vectors);
            }
        }

        static Vector2 NewAttachPointAnnotationVector(IAtom atom, IEnumerable<IBond> bonds, List<Vector2> auxVectors)
        {
            if (!bonds.Any())
                return new Vector2(0, -1);
            else if (bonds.Count() > 1)
                return NewAtomAnnotationVector(atom, bonds, auxVectors);

            // only one bond (as expected)
            var bondVector = VecmathUtil.NewUnitVector(atom, bonds.First());
            var perpVector = VecmathUtil.NewPerpendicularVector(bondVector);

            // want the annotation below
            if (perpVector.Y > 0)
                perpVector = -perpVector;
            
            var vector = new Vector2((bondVector.X + perpVector.X) / 2,
                                           (bondVector.Y + perpVector.Y) / 2);
            vector = Vector2.Normalize(vector);
            return vector;
        }

        /// <summary>
        /// A plain bond is a non-stereo sigma bond that is displayed simply as a line.
        /// </summary>
        /// <param name="bond">a non-null bond</param>
        /// <returns>the bond is plain</returns>
        internal static bool IsPlainBond(IBond bond)
        {
            return bond.Order == BondOrder.Single
                    && (bond.Stereo == BondStereo.None || bond.Stereo == BondStereo.None);
        }

        /// <summary>
        /// A bond is wedge if it points up or down.
        /// </summary>
        /// <param name="bond">a non-null bond</param>
        /// <returns>the bond is wedge (bold or hashed)</returns>
        internal static bool IsWedged(IBond bond)
        {
            return (bond.Stereo == BondStereo.Up || bond.Stereo == BondStereo.Down
                    || bond.Stereo == BondStereo.UpInverted || bond.Stereo == BondStereo.DownInverted);
        }

        /// <summary>
        /// Is the chem object hidden?
        /// </summary>
        /// <param name="chemobj">a chem object</param>
        /// <returns>whether it is hidden</returns>
        public static bool IsHidden(IChemObject chemobj)
        {
            return chemobj.GetProperty<bool>(HIDDEN, false);
        }

        /// <summary>
        /// Is the chem object hidden fully?
        /// </summary>
        /// <param name="chemobj">a chem object</param>
        /// <returns>whether it is hidden</returns>
        internal static bool IsHiddenFully(IChemObject chemobj)
        {
            return chemobj.GetProperty<bool>(HIDDEN_FULLY, false);
        }

        /// <summary>
        /// Hide the specified <paramref name="chemobj"/>, if an atom still use the bounds of its
        /// symbol.
        /// </summary>
        /// <param name="chemobj">a chem obj (atom or bond) to hide</param>
        public static void Hide(IChemObject chemobj)
        {
            chemobj.SetProperty(HIDDEN, true);
        }

        /// <summary>
        /// Hide the specified <paramref name="chemobj"/> and don't use the bounds of its symbol.
        /// </summary>
        /// <param name="chemobj">a chem obj (atom or bond) to hide</param>
        internal static void HideFully(IChemObject chemobj)
        {
            chemobj.SetProperty(HIDDEN_FULLY, true);
        }

        /// <summary>
        /// Unhide the specified <paramref name="chemobj"/>.
        /// </summary>
        /// <param name="chemobj">a chem obj (atom or bond) to unhide</param>
        internal static void Unhide(IChemObject chemobj)
        {
            chemobj.SetProperty(HIDDEN, false);
            chemobj.SetProperty(HIDDEN_FULLY, false);
        }

        /// <summary>
        /// Defines the color of unselected atoms (and bonds). Bonds colored is defined by the carbon
        /// color. The default option is uniform black coloring as recommended by IUPAC.
        /// </summary>
        public sealed class AtomColor : AbstractGeneratorParameter<IAtomColorer>
        {
            /// <inheritdoc/>
            public override IAtomColorer Default { get; } = new UniColor(Color.FromRgb(0x44, 0x44, 0x44));
        }

        /// <summary>
        /// Defines which atoms have their symbol displayed. The default option is 
        /// <see cref="SymbolVisibility.IupacRecommendationsWithoutTerminalCarbon"/> wrapped with 
        /// <see cref="SelectionVisibility.Disconnected(SymbolVisibility)"/>.
        /// </summary>
        public sealed class Visibility : AbstractGeneratorParameter<SymbolVisibility>
        {
            /// <inheritdoc/>
            public override SymbolVisibility Default { get; } = SelectionVisibility.Disconnected(SymbolVisibility.IupacRecommendationsWithoutTerminalCarbon);
        }

        /// <summary>
        /// Defines the ratio of the stroke to the width of the stroke of the font used to depict atom
        /// symbols. Default = 1.
        /// </summary>
        public sealed class StrokeRatio : AbstractGeneratorParameter<double?>
        {
            /// <inheritdoc/>
            public override double? Default => 1;
        }

        /// <summary>
        /// Defines the ratio of the separation between lines in double bonds as a percentage of length
        /// (<see cref="BasicSceneGenerator.bondLength"/>). The default value is 18% (0.18).
        /// </summary>
        public sealed class BondSeparation : AbstractGeneratorParameter<double?>
        {
            /// <inheritdoc/>
            public override double? Default => 0.18;
        }

        /// <summary>
        /// Defines the margin between an atom symbol and a connected bond based on the stroke width.
        /// Default = 2.
        /// </summary>
        public sealed class SymbolMarginRatio : AbstractGeneratorParameter<double?>
        {
            /// <inheritdoc/>
            public override double? Default => 2;
        }

        /// <summary>
        /// Ratio of the wide end of wedge compared to the narrow end (stroke width). Default = 8.
        /// </summary>
        public sealed class WedgeRatio : AbstractGeneratorParameter<double?>
        {
            /// <inheritdoc/>
            public override double? Default => 6;
        }

        /// <summary>
        /// The preferred spacing between lines in hashed bonds. The number of hashed sections displayed
        /// is then <see cref="BasicSceneGenerator.BondLength"/> / spacing. The default value is 5.
        /// </summary>
        public sealed class HashSpacing : AbstractGeneratorParameter<double?>
        {
            /// <inheritdoc/>
            public override double? Default => 5;
        }

        /// <summary>
        /// The spacing of waves (semi circles) drawn in wavy bonds with. Default = 5.
        /// </summary>
        public sealed class WaveSpacing : AbstractGeneratorParameter<double?>
        {
            /// <inheritdoc/>
            public override double? Default => 5;
        }

        /// <summary>
        /// The number of sections to render in a dashed 'unknown' bond, default = 4;
        /// </summary>
        public sealed class DashSection : AbstractGeneratorParameter<int?>
        {
            /// <inheritdoc/>
            public override int? Default => 8;
        }

        /// <summary>
        /// Modify bold wedges to be flush with adjacent bonds, default = true.
        /// </summary>
        public sealed class FancyBoldWedges : AbstractGeneratorParameter<bool?>
        {
            /// <inheritdoc/>
            public override bool? Default => true;
        }

        /// <summary>
        /// Modify hashed wedges to be flush when there is a single adjacent bond, default = true.
        /// </summary>
        public sealed class FancyHashedWedges : AbstractGeneratorParameter<bool?>
        {
            /// <inheritdoc/>
            public override bool? Default => true;
        }

        /// <summary>
        /// The width of outer glow as a percentage of stroke width. The default value is 200% (2.0d).
        /// This means the bond outer glow, is 5 times the width as the glow extends to twice the width
        /// on each side.
        /// </summary>
        public sealed class OuterGlowWidth : AbstractGeneratorParameter<double?>
        {
            /// <inheritdoc/>
            public override double? Default => 2;
        }

        /// <summary>
        /// Parameter defines the style of highlight used to emphasis atoms and bonds. The default option
        /// is to color the atom and bond symbols (<see cref="HighlightStyle.Colored"/>).
        /// </summary>
        public sealed class Highlighting : AbstractGeneratorParameter<HighlightStyle?>
        {
            /// <inheritdoc/>
            public override HighlightStyle? Default => HighlightStyle.Colored;
        }

        /// <summary>
        /// The color of the atom numbers. The parameter value is null, the color of the symbol
        /// <see cref="AtomColor"/> is used. The default color is red to distinguish from normal atom symbols.
        /// </summary>
        public sealed class AnnotationColor : AbstractGeneratorParameter<Color?>
        {
            /// <inheritdoc/>
            public override Color? Default => WPF.Media.Colors.Red;
        }

        /// <summary>
        /// The distance of atom numbers from their parent atom as a percentage of bond length, default
        /// value is 0.25 (25%)
        /// </summary>
        public sealed class AnnotationDistance : AbstractGeneratorParameter<double?>
        {
            /// <inheritdoc/>
            public override double? Default => 0.25;
        }

        /// <summary>
        /// Annotation font size relative to element symbols, default = 0.4 (40%).
        /// </summary>
        public sealed class AnnotationFontScale : AbstractGeneratorParameter<double?>
        {
            /// <inheritdoc/>
            public override double? Default => 0.5;
        }

        /// <summary>
        /// How "deep" are brackets drawn. The value is relative to bond length.
        /// </summary>
        public sealed class SgroupBracketDepth : AbstractGeneratorParameter<double?>
        {
            /// <inheritdoc/>
            public override double? Default => 0.18;
        }

        /// <summary>
        /// Scale Sgroup annotations relative to the normal font size (atom symbol).
        /// </summary>
        public sealed class SgroupFontScale : AbstractGeneratorParameter<double?>
        {
            /// <inheritdoc/>
            public override double? Default => 0.6;
        }

        /// <summary>
        /// Whether Major Isotopes e.g. 12C, 16O should be omitted.
        /// </summary>
        public sealed class OmitMajorIsotopes : AbstractGeneratorParameter<bool?>
        {
            /// <inheritdoc/>
            public override bool? Default => false;
        }
    }
}

