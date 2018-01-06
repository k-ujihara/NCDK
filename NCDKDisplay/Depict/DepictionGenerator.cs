/* Copyright (C) 2015  The Chemistry Development Kit (CDK) project
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
using NCDK.Common.Collections;
using NCDK.Geometries;
using NCDK.Layout;
using NCDK.Numerics;
using NCDK.Renderers;
using NCDK.Renderers.Colors;
using NCDK.Renderers.Elements;
using NCDK.Renderers.Generators;
using NCDK.Renderers.Generators.Standards;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;
using static NCDK.Renderers.Generators.BasicSceneGenerator;
using WPF = System.Windows;

namespace NCDK.Depict
{
    /// <summary>
    /// A high-level API for depicting molecules and reactions.
    /// </summary>
    /// <example>
    /// <b>General Usage</b>
    /// <para>
    /// Create a generator and reuse it for multiple depictions. Configure how
    /// the depiction will look using <c>With...()</c> methods.
    /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Depict.DepictionGenerator_Example.cs+1"]/*' />
    /// </para>
    /// <b>One Line Quick Use</b>
    /// <para>
    /// For simplified use we can create a generator and use it once for a single depiction.
    /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Depict.DepictionGenerator_Example.cs+2"]/*' />
    /// </para>
    /// <para>
    /// The intermediate <see cref="Depiction"/> object can write to many different formats
    /// through a variety of API calls.
    /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.Depict.DepictionGenerator_Example.cs+3"]/*' />
    /// </para>
    /// </example>
    // @author John may 
    public sealed class DepictionGenerator
    {
        /// <summary>
        /// Visually distinct colors for highlighting.
        /// http://stackoverflow.com/a/4382138
        /// Kenneth L. Kelly and Deanne B. Judd.
        /// "Color: Universal Language and Dictionary of Names",
        /// National Bureau of Standards,
        /// Spec. Publ. 440, Dec. 1976, 189 pages.
        /// </summary>
        private static readonly Color[] KELLY_MAX_CONTRAST = new Color[]
        {
            Color.FromRgb(0x00, 0x53, 0x8A), // Strong Blue (sub-optimal for defective color vision)
            Color.FromRgb(0x93, 0xAA, 0x00), // Vivid Yellowish Green (sub-optimal for defective color vision)
            Color.FromRgb(0xC1, 0x00, 0x20), // Vivid Red
            Color.FromRgb(0xFF, 0xB3, 0x00), // Vivid Yellow
            Color.FromRgb(0x00, 0x7D, 0x34), // Vivid Green (sub-optimal for defective color vision)
            Color.FromRgb(0xFF, 0x68, 0x00), // Vivid Orange
            Color.FromRgb(0xCE, 0xA2, 0x62), // Grayish Yellow
            Color.FromRgb(0x81, 0x70, 0x66), // Medium Gray
            Color.FromRgb(0xA6, 0xBD, 0xD7), // Very Light Blue
            Color.FromRgb(0x80, 0x3E, 0x75), // Strong Purple

            Color.FromRgb(0xF6, 0x76, 0x8E), // Strong Purplish Pink (sub-optimal for defective color vision)

            Color.FromRgb(0xFF, 0x7A, 0x5C), // Strong Yellowish Pink (sub-optimal for defective color vision)
            Color.FromRgb(0x53, 0x37, 0x7A), // Strong Violet (sub-optimal for defective color vision)
            Color.FromRgb(0xFF, 0x8E, 0x00), // Vivid Orange Yellow (sub-optimal for defective color vision)
            Color.FromRgb(0xB3, 0x28, 0x51), // Strong Purplish Red (sub-optimal for defective color vision)
            Color.FromRgb(0xF4, 0xC8, 0x00), // Vivid Greenish Yellow (sub-optimal for defective color vision)
            Color.FromRgb(0x7F, 0x18, 0x0D), // Strong Reddish Brown (sub-optimal for defective color vision)

            Color.FromRgb(0x59, 0x33, 0x15), // Deep Yellowish Brown (sub-optimal for defective color vision)
            Color.FromRgb(0xF1, 0x3A, 0x13), // Vivid Reddish Orange (sub-optimal for defective color vision)
            Color.FromRgb(0x23, 0x2C, 0x16), // Dark Olive Green (sub-optimal for defective color vision)
        };

        /// <summary>
        /// Magic value for indicating automatic parameters. These can
        /// be overridden by a caller.
        /// </summary>
        public static double AUTOMATIC = -1;

        /// <summary>
        /// Default margin for vector graphics formats.
        /// </summary>
        public static double DEFAULT_MM_MARGIN = 0.56;

        /// <summary>
        /// Default margin for raster graphics formats.
        /// </summary>
        public static double DEFAULT_PX_MARGIN = 4;

        /// <summary>
        /// The dimensions (width x height) of the depiction.
        /// </summary>
        private Dimensions dimensions = Dimensions.AUTOMATIC;

        /// <summary>
        /// Storage of rendering parameters.
        /// </summary>
        private readonly IDictionary<Type, IGeneratorParameter> parameters = new Dictionary<Type, IGeneratorParameter>();

        /// <summary>
        /// Font used for depictions.
        /// </summary>
        private readonly Typeface font;

        private readonly double emSize = 9;

        /// <summary>
        /// Diagram generators.
        /// </summary>
        private readonly List<IGenerator<IAtomContainer>> gens = new List<IGenerator<IAtomContainer>>();

        /// <summary>
        /// Flag to indicate atom numbers should be displayed.
        /// </summary>
        private bool annotateAtomNum = false;

        /// <summary>
        /// Flag to indicate atom values should be displayed.
        /// </summary>
        private bool annotateAtomVal = false;

        /// <summary>
        /// Flag to indicate atom maps should be displayed.
        /// </summary>
        private bool annotateAtomMap = false;

        /// <summary>
        /// Flag to indicate atom maps should be highlighted with colored.
        /// </summary>
        private bool highlightAtomMap = false;

        /// <summary>
        /// Colors to use in atom-map highlighting.
        /// </summary>
        private Color[] atomMapColors = null;

        /// <summary>
        /// Reactions are aligned such that mapped atoms have the same coordinates on the left/right.
        /// </summary>
        private bool alignMappedReactions = true;

        /// <summary>
        /// Object that should be highlighted
        /// </summary>
        private Dictionary<IChemObject, Color> highlight = new Dictionary<IChemObject, Color>();

        /// <summary>
        /// Create a depiction generator using the standard sans-serif
        /// system font.
        /// </summary>
        public DepictionGenerator()
            : this(
                  new Typeface(
                      WPF.SystemFonts.MessageFontFamily,
                      WPF.SystemFonts.MessageFontStyle,
                      WPF.SystemFonts.MessageFontWeight,
                      new WPF.FontStretch()),
                  WPF.SystemFonts.MessageFontSize)
        {
            SetParam(typeof(BasicSceneGenerator.BondLength), (double?)26.1d);
            SetParam(typeof(StandardGenerator.HashSpacing), (double?)(26 / 8d));
            SetParam(typeof(StandardGenerator.WaveSpacing), (double?)(26 / 8d));
        }

        /// <summary>
        /// Create a depiction generator that will render atom
        /// labels using the specified AWT font.
        /// </summary>
        /// <param name="font">the font to use to display</param>
        public DepictionGenerator(Typeface font, double emSize)
        {
            gens.Add(new BasicSceneGenerator());
            gens.Add(new StandardGenerator(this.font = font, emSize));

            foreach (var gen in gens)
            {
                foreach (var param in gen.Parameters)
                {
                    parameters[param.GetType()] = param;
                }
            }
            foreach (var param in new RendererModel().GetRenderingParameters())
            {
                parameters[param.GetType()] = param;
            }

            // default margin and separation is automatic
            // since it depends on raster (px) vs vector (mm)
            SetParam(typeof(BasicSceneGenerator.Margin), (double?)AUTOMATIC);
            SetParam(typeof(RendererModel.Padding), (double?)AUTOMATIC);
        }

        /// <summary>
        /// Internal copy constructor.
        /// </summary>
        /// <param name="org">original depiction</param>
        private DepictionGenerator(DepictionGenerator org)
        {
            this.annotateAtomMap = org.annotateAtomMap;
            this.annotateAtomVal = org.annotateAtomVal;
            this.annotateAtomNum = org.annotateAtomNum;
            this.highlightAtomMap = org.highlightAtomMap;
            this.atomMapColors = org.atomMapColors;
            this.dimensions = org.dimensions;
            this.font = org.font;
            foreach (var e in org.highlight)
                this.highlight[e.Key] = e.Value;
            this.gens.AddRange(org.gens);
            foreach (var e in org.parameters)
                this.parameters[e.Key] = e.Value;
            this.alignMappedReactions = org.alignMappedReactions;
        }

        private U GetParameterValue<U>(Type key)
        {
            if (!parameters.TryGetValue(key, out IGeneratorParameter parama))
                throw new ArgumentException($"No parameter registered: {key} {Common.Primitives.Strings.ToJavaString(parameters.Keys)}");
            var param = (IGeneratorParameter<U>)parama;
            return param.Value;
        }

        private U GetParameterValueV<U>(Type key) where U : struct
        {
            return GetParameterValue<U?>(key).Value;
        }

        //private <T extends IGeneratorParameter<S>, S, U extends S> void SetParam(Class<T> key, U val)
        private void SetParam<U>(Type key, U val)
        {
            IGeneratorParameter<U> param = null;
            try
            {
                param = (IGeneratorParameter<U>)key.GetConstructor(Type.EmptyTypes).Invoke(Array.Empty<object>());
                param.Value = val;
                parameters[key] = param;
            }
            catch (Exception)
            {
                Trace.TraceError($"Could not copy rendering parameter: {key}");
            }
        }

        private RendererModel GetModel()
        {
            RendererModel model = new RendererModel();
            foreach (var gen in gens)
                model.RegisterParameters(gen);
            foreach (var param in parameters.Values)
            {
                var value = param.GetType().GetProperty("Value").GetValue(param);
                model.Set(param.GetType(), value);
            }
            return model;
        }

        /// <summary>
        /// Depict a single molecule.
        /// </summary>
        /// <param name="mol">molecule</param>
        /// <returns>depiction instance</returns>
        /// <exception cref="CDKException">a depiction could not be generated</exception>
        public Depiction Depict(IAtomContainer mol)
        {
            return Depict(new[] { mol }, 1, 1);
        }

        /// <summary>
        /// Depict a set of molecules, they will be depicted in a grid. The grid
        /// size (nrow x ncol) is determined automatically based on the number
        /// molecules.
        /// </summary>
        /// <param name="mols">molecules</param>
        /// <returns>depiction</returns>
        /// <exception cref="CDKException">a depiction could not be generated</exception>
        /// <seealso cref="Depict(IEnumerable{IAtomContainer}, int, int)"/>
        public Depiction Depict(IEnumerable<IAtomContainer> mols)
        {
            var molList = mols.ToList();
            var grid = Dimensions.DetermineGrid(molList.Count());
            return Depict(mols, grid.Height, grid.Width);
        }

        /// <summary>
        /// Depict a set of molecules, they will be depicted in a grid with the
        /// specified number of rows and columns. Rows are filled first and then
        /// columns.
        /// </summary>
        /// <param name="mols">molecules</param>
        /// <param name="nrow">number of rows</param>
        /// <param name="ncol">number of columns</param>
        /// <returns>depiction</returns>
        /// <exception cref="CDKException">a depiction could not be generated</exception>
        public Depiction Depict(IEnumerable<IAtomContainer> mols, int nrow, int ncol)
        {
            var layoutBackups = new List<LayoutBackup>();
            int molId = 0;
            foreach (var mol in mols)
            {
                SetIfMissing(mol, MarkedElement.ID_KEY, "mol" + ++molId);
                layoutBackups.Add(new LayoutBackup(mol));
            }

            // ensure we have coordinates, generate them if not
            // we also rescale the molecules such that all bond
            // lengths are the same.
            PrepareCoords(mols);

            // highlight parts
            foreach (var e in highlight)
                e.Key.SetProperty(StandardGenerator.HIGHLIGHT_COLOR, e.Value);

            // setup the model scale
            List<IAtomContainer> molList = mols.ToList();
            DepictionGenerator copy = this.WithParam(typeof(BasicSceneGenerator.Scale), (double?)CaclModelScale(molList));

            // generate bound rendering elements
            RendererModel model = copy.GetModel();
            var molElems = copy.Generate(molList, model, 1);

            // reset molecule coordinates
            foreach (LayoutBackup backup in layoutBackups)
                backup.Reset();

            // generate titles (if enabled)
            var titles = new List<Bounds>();
            if (copy.GetParameterValueV<bool>(typeof(BasicSceneGenerator.ShowMoleculeTitle)))
            {
                foreach (var mol in mols)
                    titles.Add(copy.GenerateTitle(mol, model.GetV<double>(typeof(BasicSceneGenerator.Scale))));
            }

            // remove current highlight buffer
            foreach (var obj in this.highlight.Keys)
                obj.RemoveProperty(StandardGenerator.HIGHLIGHT_COLOR);
            this.highlight.Clear();

            return new MolGridDepiction(model, molElems, titles, dimensions, nrow, ncol);
        }

        /// <summary>
        /// Prepare a collection of molecules for rendering. If coordinates are not
        /// present they are generated, if coordinates exists they are scaled to
        /// be consistent (length=1.5).
        ///
        /// <param name="mols">molecules</param>
        /// <returns>coordinates</returns>
        /// <exception cref="CDKException"></exception>
        /// </summary>
        private void PrepareCoords(IEnumerable<IAtomContainer> mols)
        {
            foreach (IAtomContainer mol in mols)
            {
                if (!Ensure2dLayout(mol) && mol.Bonds.Count > 0)
                {
                    double factor = GeometryUtil.GetScaleFactor(mol, 1.5);
                    GeometryUtil.ScaleMolecule(mol, factor);
                }
            }
        }

        /// <summary>
        /// Reset the coordinates to their position before rendering.
        /// </summary>
        /// <param name="mols">molecules</param>
        /// <param name="scales">how molecules were scaled</param>
        private static void ResetCoords(IEnumerable<IAtomContainer> mols, List<double> scales)
        {
            var it = scales.GetEnumerator();
            foreach (var mol in mols)
            {
                it.MoveNext();
                double factor = it.Current;
                if (!double.IsNaN(factor))
                {
                    GeometryUtil.ScaleMolecule(mol, 1 / factor);
                }
                else
                {
                    foreach (var atom in mol.Atoms)
                        atom.Point2D = null;
                }
            }
        }

        private static void SetIfMissing(IChemObject chemObject, string key, string val)
        {
            if (chemObject.GetProperty<string>(key) == null)
                chemObject.SetProperty(key, val);
        }

        /// <summary>
        /// Depict a reaction.
        /// </summary>
        /// <param name="rxn">reaction instance</param>
        /// <returns>depiction</returns>
        /// <exception cref="CDKException">a depiction could not be generated</exception>
        public Depiction Depict(IReaction rxn)
        {
            Ensure2dLayout(rxn); // can reorder components!

            Color fgcol = GetParameterValue<IAtomColorer>(typeof(StandardGenerator.AtomColor)).GetAtomColor(rxn.Builder.NewAtom("C"));

            var reactants = ToList(rxn.Reactants);
            var products = ToList(rxn.Products);
            var agents = ToList(rxn.Agents);
            List<LayoutBackup> layoutBackups = new List<LayoutBackup>();

            // set ids for tagging elements
            int molId = 0;
            foreach (var mol in reactants)
            {
                SetIfMissing(mol, MarkedElement.ID_KEY, "mol" + ++molId);
                SetIfMissing(mol, MarkedElement.CLASS_KEY, "reactant");
                layoutBackups.Add(new LayoutBackup(mol));
            }
            foreach (var mol in products)
            {
                SetIfMissing(mol, MarkedElement.ID_KEY, "mol" + ++molId);
                SetIfMissing(mol, MarkedElement.CLASS_KEY, "product");
                layoutBackups.Add(new LayoutBackup(mol));
            }
            foreach (var mol in agents)
            {
                SetIfMissing(mol, MarkedElement.ID_KEY, "mol" + ++molId);
                SetIfMissing(mol, MarkedElement.CLASS_KEY, "agent");
                layoutBackups.Add(new LayoutBackup(mol));
            }

            var myHighlight = new Dictionary<IChemObject, Color>();
            if (highlightAtomMap)
            {
                foreach (var e in MakeHighlightAtomMap(reactants, products))
                    myHighlight[e.Key] = e.Value;
            }
            // user highlight buffer pushes out the atom-map highlight if provided
            foreach (var e in highlight)
                myHighlight[e.Key] = e.Value;
            highlight.Clear();

            PrepareCoords(reactants);
            PrepareCoords(products);
            PrepareCoords(agents);

            // highlight parts
            foreach (var e in myHighlight)
                e.Key.SetProperty(StandardGenerator.HIGHLIGHT_COLOR, e.Value);

            // setup the model scale based on bond length
            double scale = this.CaclModelScale(rxn);
            DepictionGenerator copy = this.WithParam(typeof(BasicSceneGenerator.Scale), (double?)scale);
            RendererModel model = copy.GetModel();

            // reactant/product/agent element generation, we number the reactants, then products then agents
            List<Bounds> reactantBounds = copy.Generate(reactants, model, 1);
            List<Bounds> productBounds = copy.Generate(ToList(rxn.Products), model, rxn.Reactants.Count);
            List<Bounds> agentBounds = copy.Generate(ToList(rxn.Agents), model, rxn.Reactants.Count + rxn.Products.Count);

            // remove current highlight buffer
            foreach (var obj in myHighlight.Keys)
                obj.RemoveProperty(StandardGenerator.HIGHLIGHT_COLOR);

            // generate a 'plus' element
            Bounds plus = copy.GeneratePlusSymbol(scale, fgcol);

            // reset the coordinates to how they were before we invoked depict
            foreach (LayoutBackup backup in layoutBackups)
                backup.Reset();

            Bounds emptyBounds = new Bounds();
            Bounds title = copy.GetParameterValueV<bool>(typeof(BasicSceneGenerator.ShowReactionTitle)) ? copy.GenerateTitle(rxn, scale) : emptyBounds;
            var reactantTitles = new List<Bounds>();
            var productTitles = new List<Bounds>();
            if (copy.GetParameterValueV<bool>(typeof(BasicSceneGenerator.ShowMoleculeTitle)))
            {
                foreach (IAtomContainer reactant in reactants)
                    reactantTitles.Add(copy.GenerateTitle(reactant, scale));
                foreach (IAtomContainer product in products)
                    productTitles.Add(copy.GenerateTitle(product, scale));
            }

            Bounds conditions = GenerateReactionConditions(rxn, fgcol, model.GetV<double>(typeof(BasicSceneGenerator.Scale)));

            return new ReactionDepiction(model,
                                         reactantBounds, productBounds, agentBounds,
                                         plus, rxn.Direction, dimensions,
                                         reactantTitles,
                                         productTitles,
                                         title,
                                         conditions,
                                         fgcol);
        }

        /// <summary>
        /// Internal - makes a map of the highlights for reaction mapping.
        /// </summary>
        /// <param name="reactants">reaction reactants</param>
        /// <param name="products">reaction products</param>
        /// <returns>the highlight map</returns>
        private Dictionary<IChemObject, Color> MakeHighlightAtomMap(List<IAtomContainer> reactants,
                                                             List<IAtomContainer> products)
        {
            var colorMap = new Dictionary<IChemObject, Color>();
            var mapToColor = new Dictionary<int, Color>();
            int colorIdx = -1;
            foreach (var mol in reactants)
            {
                int prevPalletIdx = colorIdx;
                foreach (var atom in mol.Atoms)
                {
                    int mapidx = AccessAtomMap(atom);
                    if (mapidx > 0)
                    {
                        if (prevPalletIdx == colorIdx)
                        {
                            colorIdx++; // select next color
                            if (colorIdx >= atomMapColors.Length)
                                throw new ArgumentException("Not enough colors to highlight atom mapping, please provide mode");
                        }
                        Color color = atomMapColors[colorIdx];
                        colorMap[atom] = color;
                        mapToColor[mapidx] = color;
                    }
                }
                if (colorIdx > prevPalletIdx)
                {
                    foreach (var bond in mol.Bonds)
                    {
                        IAtom a1 = bond.Begin;
                        IAtom a2 = bond.End;
                        Color c1 = colorMap[a1];
                        Color c2 = colorMap[a2];
                        if (c1 != null && c1 == c2)
                            colorMap[bond] = c1;
                    }
                }
            }

            foreach (var mol in products)
            {
                foreach (var atom in mol.Atoms)
                {
                    int mapidx = AccessAtomMap(atom);
                    if (mapidx > 0)
                    {
                        colorMap[atom] = mapToColor[mapidx];
                    }
                }
                foreach (var bond in mol.Bonds)
                {
                    IAtom a1 = bond.Begin;
                    IAtom a2 = bond.End;
                    Color c1 = colorMap[a1];
                    Color c2 = colorMap[a2];
                    if (c1 != null && c1 == c2)
                        colorMap[bond] = c1;
                }
            }

            return colorMap;
        }

        private int AccessAtomMap(IAtom atom)
        {
            var mapidx = atom.GetProperty<int?>(CDKPropertyName.AtomAtomMapping);
            if (mapidx == null)
                return 0;
            return mapidx.Value;
        }

        private Bounds GeneratePlusSymbol(double scale, Color fgcol)
        {
            return new Bounds(StandardGenerator.EmbedText(font, emSize, "+", fgcol, 1 / scale));
        }

        private List<IAtomContainer> ToList(IChemObjectSet<IAtomContainer> set)
        {
            return set.ToList();
        }

        private IRenderingElement Generate(IAtomContainer molecule, RendererModel model, int atomNum)
        {
            // tag the atom and bond ids
            string molId = molecule.GetProperty<string>(MarkedElement.ID_KEY);
            if (molId != null)
            {
                int atomId = 0, bondid = 0;
                foreach (var atom in molecule.Atoms)
                    SetIfMissing(atom, MarkedElement.ID_KEY, molId + "atm" + ++atomId);
                foreach (var bond in molecule.Bonds)
                    SetIfMissing(bond, MarkedElement.ID_KEY, molId + "bnd" + ++bondid);
            }

            if (annotateAtomNum)
            {
                foreach (var atom in molecule.Atoms)
                {
                    if (atom.GetProperty<string>(StandardGenerator.ANNOTATION_LABEL) != null)
                        throw new InvalidOperationException("Multiple annotation labels are not supported.");
                    atom.SetProperty(StandardGenerator.ANNOTATION_LABEL, (atomNum++).ToString());
                }
            }
            else if (annotateAtomVal)
            {
                foreach (IAtom atom in molecule.Atoms)
                {
                    if (atom.GetProperty<string>(StandardGenerator.ANNOTATION_LABEL) != null)
                        throw new NotSupportedException("Multiple annotation labels are not supported.");
                    atom.SetProperty(StandardGenerator.ANNOTATION_LABEL,
                                     atom.GetProperty<string>(CDKPropertyName.Comment));
                }
            }
            else if (annotateAtomMap)
            {
                foreach (var atom in molecule.Atoms)
                {
                    if (atom.GetProperty<string>(StandardGenerator.ANNOTATION_LABEL) != null)
                        throw new InvalidOperationException("Multiple annotation labels are not supported.");
                    int mapidx = AccessAtomMap(atom);
                    if (mapidx > 0)
                    {
                        atom.SetProperty(StandardGenerator.ANNOTATION_LABEL, mapidx.ToString());
                    }
                }
            }

            ElementGroup grp = new ElementGroup();
            foreach (var gen in gens)
                grp.Add(gen.Generate(molecule, model));

            // cleanup
            if (annotateAtomNum || annotateAtomMap)
            {
                foreach (var atom in molecule.Atoms)
                {
                    atom.RemoveProperty(StandardGenerator.ANNOTATION_LABEL);
                }
            }

            return grp;
        }

        private List<Bounds> Generate(List<IAtomContainer> mols, RendererModel model, int atomNum)
        {
            var elems = new List<Bounds>();
            foreach (var mol in mols)
            {
                elems.Add(new Bounds(Generate(mol, model, atomNum)));
                atomNum += mol.Atoms.Count;
            }
            return elems;
        }

        /// <summary>
        /// Generate a bound element that is the title of the provided molecule. If title
        /// is not specified an empty bounds is returned.
        /// </summary>
        /// <param name="chemObj">molecule or reaction</param>
        /// <returns>bound element</returns>
        private Bounds GenerateTitle(IChemObject chemObj, double scale)
        {
            string title = chemObj.GetProperty<string>(CDKPropertyName.Title);
            if (string.IsNullOrEmpty(title))
                return new Bounds();
            scale = 1 / scale * GetParameterValueV<double>(typeof(RendererModel.TitleFontScale));
            return new Bounds(MarkedElement.Markup(StandardGenerator.EmbedText(font, emSize, title, GetParameterValueV<Color>(typeof(RendererModel.TitleColor)), scale), "title"));
        }

        private Bounds GenerateReactionConditions(IReaction chemObj, Color fg, double scale)
        {
            string title = chemObj.GetProperty<string>(CDKPropertyName.ReactionConditions);
            if (string.IsNullOrEmpty(title))
                return new Bounds();
            return new Bounds(MarkedElement.Markup(StandardGenerator.EmbedText(font, emSize, title, fg, 1 / scale),
                                              "conditions"));
        }

        /// <summary>
        /// Automatically generate coordinates if a user has provided a molecule without them.
        /// </summary>
        /// <param name="container">a molecule</param>
        /// <returns>if coordinates needed to be generated</returns>
        /// <exception cref="CDKException">coordinates could not be generated</exception>
        private bool Ensure2dLayout(IAtomContainer container)
        {
            if (!GeometryUtil.Has2DCoordinates(container))
            {
                StructureDiagramGenerator sdg = new StructureDiagramGenerator();
                sdg.GenerateCoordinates(container);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Automatically generate coordinates if a user has provided reaction without them.
        /// </summary>
        /// <param name="rxn">reaction</param>
        /// <exception cref="CDKException">coordinates could not be generated</exception>
        private void Ensure2dLayout(IReaction rxn)
        {
            if (!GeometryUtil.Has2DCoordinates(rxn))
            {
                StructureDiagramGenerator sdg = new StructureDiagramGenerator();
                sdg.SetAlignMappedReaction(alignMappedReactions);
                sdg.GenerateCoordinates(rxn);
            }
        }

        /// <summary>
        /// Color atom symbols using typical colors, oxygens are red, nitrogens are
        /// blue, etc.
        /// </summary>
        /// <returns>new generator for method chaining</returns>
        /// <seealso cref="StandardGenerator.AtomColor"/>
        /// <seealso cref="StandardGenerator.Highlighting"/>
        /// <seealso cref="StandardGenerator.HighlightStyle"/>
        /// <seealso cref="CDK2DAtomColors"/>
        public DepictionGenerator WithAtomColors()
        {
            return WithAtomColors(new CDK2DAtomColors());
        }

        /// <summary>
        /// Color atom symbols using provided colorer.
        /// </summary>
        /// <returns>new generator for method chaining</returns>
        /// <seealso cref="StandardGenerator.AtomColor"/>
        /// <seealso cref="StandardGenerator.Highlighting"/>
        /// <seealso cref="StandardGenerator.HighlightStyle"/>
        /// <seealso cref="CDK2DAtomColors"/>
        /// <seealso cref="UniColor"/>
        public DepictionGenerator WithAtomColors(IAtomColorer colorer)
        {
            return WithParam(typeof(StandardGenerator.AtomColor), colorer);
        }

        /// <summary>
        /// Change the background color.
        /// </summary>
        /// <param name="color">background color</param>
        /// <returns>new generator for method chaining</returns>
        /// <seealso cref="BackgroundColor"/>
        public DepictionGenerator WithBackgroundColor(Color color)
        {
            return WithParam(typeof(BackgroundColor), (Color?)color);
        }

        /// <summary>
        /// Highlights are shown as an outer glow around the atom symbols and bonds
        /// rather than recoloring. The width of the glow can be set but defaults to
        /// 4x the stroke width.
        /// </summary>
        /// <returns>new generator for method chaining</returns>
        /// <seealso cref="StandardGenerator.Highlighting"/>
        /// <seealso cref="StandardGenerator.HighlightStyle"/>
        public DepictionGenerator WithOuterGlowHighlight()
        {
            return WithOuterGlowHighlight(4);
        }

        /// <summary>
        /// Highlights are shown as an outer glow around the atom symbols and bonds
        /// rather than recoloring.
        /// </summary>
        /// <param name="width">width of the outer glow relative to the bond stroke</param>
        /// <returns>new generator for method chaining</returns>
        /// <seealso cref="StandardGenerator.Highlighting"/>
        /// <seealso cref="StandardGenerator.HighlightStyle"/>
        public DepictionGenerator WithOuterGlowHighlight(double width)
        {
            return WithParam(typeof(StandardGenerator.Highlighting), (StandardGenerator.HighlightStyle?)StandardGenerator.HighlightStyle.OuterGlow).WithParam(typeof(StandardGenerator.OuterGlowWidth), (double?)width);
        }

        /// <summary>
        /// Display atom numbers on the molecule or reaction. The numbers are based on the
        /// ordering of atoms in the molecule data structure and not a systematic system
        /// such as IUPAC numbering.
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// A depiction can not have both atom numbers and atom maps visible
        /// (but this can be achieved by manually setting the annotation).
        /// </note>
        /// </remarks>
        /// <returns>new generator for method chaining</returns>
        /// <seealso cref="WithAtomMapNumbers"/>
        /// <seealso cref="StandardGenerator.ANNOTATION_LABEL"/>
        public DepictionGenerator WithAtomNumbers()
        {
            if (annotateAtomMap || annotateAtomVal)
                throw new ArgumentException("Can not annotated atom numbers, atom values or maps are already annotated");
            DepictionGenerator copy = new DepictionGenerator(this)
            {
                annotateAtomNum = true
            };
            return copy;
        }

        /// <summary>
        /// Display atom values on the molecule or reaction. The values need to be assigned by 
        /// <c>atom.SetProperty(CDKConstants.COMMENT, myValueToBeDisplayedNextToAtom);</c>
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// A depiction can not have both atom numbers and atom maps visible
        /// (but this can be achieved by manually setting the annotation).
        /// </note>
        /// </remarks>
        /// <returns>new generator for method chaining</returns>
        /// <seealso cref="WithAtomMapNumbers()"/>
        /// <seealso cref="StandardGenerator.ANNOTATION_LABEL"/>
        public DepictionGenerator WithAtomValues()
        {
            if (annotateAtomNum || annotateAtomMap)
                throw new InvalidOperationException("Can not annotated atom values, atom numbers or maps are already annotated");
            DepictionGenerator copy = new DepictionGenerator(this)
            {
                annotateAtomVal = true
            };
            return copy;
        }

        /// <summary>
        /// Display atom-atom mapping numbers on a reaction. Each atom map index
        /// is loaded from the property <see cref="CDKPropertyName.AtomAtomMapping"/>.
        /// </summary>
        /// <remarks>
        /// <note type="note">
        /// A depiction can not have both atom numbers and atom
        /// maps visible (but this can be achieved by manually setting
        /// the annotation).
        /// </note>
        /// </remarks>
        /// <returns>new generator for method chaining</returns>
        /// <seealso cref="WithAtomNumbers"/>
        /// <seealso cref="CDKPropertyName.AtomAtomMapping"/>
        /// <seealso cref="StandardGenerator.ANNOTATION_LABEL"/>
        public DepictionGenerator WithAtomMapNumbers()
        {
            if (annotateAtomNum)
                throw new InvalidOperationException("Can not annotated atom maps, atom numbers or values are already annotated");
            DepictionGenerator copy = new DepictionGenerator(this)
            {
                annotateAtomMap = true
            };
            return copy;
        }

        /// <summary>
        /// Adds to the highlight the coloring of reaction atom-maps. The
        /// optional color array is used as the pallet with which to
        /// highlight. If none is provided a set of high-contrast colors
        /// will be used.
        /// </summary>
        /// <returns>new generator for method chaining</returns>
        /// <seealso cref="WithAtomMapNumbers"/>
        /// <seealso cref="WithAtomMapHighlight(Color[])"/>
        public DepictionGenerator WithAtomMapHighlight()
        {
            return WithAtomMapHighlight(KELLY_MAX_CONTRAST);
        }

        /// <summary>
        /// Adds to the highlight the coloring of reaction atom-maps. The
        /// optional color array is used as the pallet with which to
        /// highlight. If none is provided a set of high-contrast colors
        /// will be used.
        /// </summary>
        /// <param name="colors">array of colors</param>
        /// <returns>new generator for method chaining</returns>
        /// <seealso cref="WithAtomMapNumbers"/>
        /// <seealso cref="WithAtomMapHighlight()"/>
        public DepictionGenerator WithAtomMapHighlight(Color[] colors)
        {
            DepictionGenerator copy = new DepictionGenerator(this)
            {
                highlightAtomMap = true,
                atomMapColors = (Color[])colors.Clone()
            };
            return copy;
        }

        /// <summary>
        /// Display a molecule title with each depiction. The title
        /// is specified by setting the <see cref="CDKPropertyName.Title"/>
        /// property. For reactions only the main components have their
        /// title displayed.
        /// </summary>
        /// <returns>new generator for method chaining</returns>
        /// <seealso cref="BasicSceneGenerator.ShowMoleculeTitle"/>
        public DepictionGenerator WithMolTitle()
        {
            return WithParam(typeof(BasicSceneGenerator.ShowMoleculeTitle), (bool?)true);
        }

        /// <summary>
        /// Display a reaction title with the depiction. The title
        /// is specified by setting the <see cref="CDKPropertyName.Title"/>
        /// property on the <see cref="IReaction"/> instance.
        /// </summary>
        /// <returns>new generator for method chaining</returns>
        /// <seealso cref="BasicSceneGenerator.ShowReactionTitle"/>
        public DepictionGenerator WithRxnTitle()
        {
            return WithParam(typeof(BasicSceneGenerator.ShowReactionTitle), (bool?)true);
        }

        /// <summary>
        /// Specifies that reactions with atom-atom mappings should have their reactants/product
        /// coordinates aligned. Default: true.
        /// </summary>
        /// <param name="val">setting value</param>
        /// <returns>new generator for method chaining</returns>
        public DepictionGenerator WithMappedRxnAlign(bool val)
        {
            DepictionGenerator copy = new DepictionGenerator(this)
            {
                alignMappedReactions = val
            };
            return copy;
        }

        /// <summary>
        /// Set the color annotations (e.g. atom-numbers) will appear in.
        /// </summary>
        /// <param name="color">the color of annotations</param>
        /// <returns>new generator for method chaining</returns>
        /// <seealso cref="StandardGenerator.AnnotationColor"/>
        public DepictionGenerator WithAnnotationColor(Color color)
        {
            return WithParam(typeof(StandardGenerator.AnnotationColor), (Color?)color);
        }

        /// <summary>
        /// Set the size of annotations relative to atom symbols.
        /// </summary>
        /// <param name="scale">the scale of annotations</param>
        /// <returns>new generator for method chaining</returns>
        /// <seealso cref="StandardGenerator.AnnotationFontScale"/>
        public DepictionGenerator WithAnnotationScale(double scale)
        {
            return WithParam(typeof(StandardGenerator.AnnotationFontScale), (double?)scale);
        }

        /// <summary>
        /// Set the color titles will appear in.
        /// </summary>
        /// <param name="color">the color of titles</param>
        /// <returns>new generator for method chaining</returns>
        /// <seealso cref="RendererModel.TitleColor"/>
        public DepictionGenerator WithTitleColor(Color color)
        {
            return WithParam(typeof(RendererModel.TitleColor), (Color?)color);
        }

        /// <summary>
        /// Set the size of titles compared to atom symbols.
        /// </summary>
        /// <param name="scale">the scale of titles</param>
        /// <returns>new generator for method chaining</returns>
        /// <seealso cref="RendererModel.TitleFontScale"/>
        public DepictionGenerator WithTitleScale(double scale)
        {
            return WithParam(typeof(RendererModel.TitleFontScale), (double?)scale);
        }

        /// <summary>
        /// Display atom symbols for terminal carbons (i.e. Methyl)
        /// groups.
        /// </summary>
        /// <returns>new generator for method chaining</returns>
        /// <seealso cref="StandardGenerator.Visibility"/>
        public DepictionGenerator WithTerminalCarbons()
        {
            return WithParam(typeof(StandardGenerator.Visibility), SelectionVisibility.Disconnected(SymbolVisibility.IupacRecommendations));
        }

        /// <summary>
        /// Display atom symbols for all atoms in the molecule.
        /// </summary>
        /// <returns>new generator for method chaining</returns>
        /// <seealso cref="StandardGenerator.Visibility"/>
        public DepictionGenerator WithCarbonSymbols()
        {
            return WithParam(typeof(StandardGenerator.Visibility), SymbolVisibility.All);
        }

        /// <summary>
        /// Highlight the provided set of atoms and bonds in the depiction in the
        /// specified color.
        /// </summary>
        /// <remarks>
        /// Calling this methods appends to the current highlight buffer. The buffer
        /// is cleared after each depiction is generated (e.g. <see cref="Depict(IAtomContainer)"/>).
        /// </remarks>
        /// <param name="chemObjs">set of atoms and bonds</param>
        /// <param name="color">the color to highlight</param>
        /// <returns>new generator for method chaining</returns>
        /// <seealso cref="StandardGenerator.HIGHLIGHT_COLOR"/>
        public DepictionGenerator WithHighlight(IEnumerable<IChemObject> chemObjs, Color color)
        {
            DepictionGenerator copy = new DepictionGenerator(this);
            foreach (var chemObj in chemObjs)
                copy.highlight[chemObj] = color;
            return copy;
        }

        /// <summary>
        /// Specify a desired size of depiction. The units depend on the output format with
        /// raster images using pixels and vector graphics using millimeters. By default depictions
        /// are only ever made smaller if you would also like to make depictions fill all available
        /// space use the <see cref="WithFillToFit"/> option. 
        /// </summary>
        /// <remarks>
        /// Currently the size must either both be precisely specified (e.g. 256x256) or
        /// automatic (e.g. <see cref="AUTOMATIC"/>x<see cref="AUTOMATIC"/>) you cannot for example
        /// specify a fixed height and automatic width.
        /// </remarks>
        /// <param name="w">max width</param>
        /// <param name="h">max height</param>
        /// <returns>new generator for method chaining</returns>
        /// <seealso cref="WithFillToFit"/>
        public DepictionGenerator WithSize(double w, double h)
        {
            if (w < 0 && h >= 0 || h < 0 && w >= 0)
                throw new ArgumentException("Width and height must either both be automatic or both specified");
            DepictionGenerator copy = new DepictionGenerator(this)
            {
                dimensions = w == AUTOMATIC ? Dimensions.AUTOMATIC : new Dimensions(w, h)
            };
            return copy;
        }

        /// <summary>
        /// Specify a desired size of margin. The units depend on the output format with
        /// raster images using pixels and vector graphics using millimeters.
        /// </summary>
        /// <param name="m">margin</param>
        /// <returns>new generator for method chaining</returns>
        /// <seealso cref="BasicSceneGenerator.Margin"/>
        public DepictionGenerator WithMargin(double m)
        {
            return WithParam(typeof(BasicSceneGenerator.Margin), (double?)m);
        }

        /// <summary>
        /// Specify a desired size of padding for molecule sets and reactions. The units
        /// depend on the output format with raster images using pixels and vector graphics
        /// using millimeters.
        /// </summary>
        /// <param name="p">padding</param>
        /// <returns>new generator for method chaining</returns>
        /// <seealso cref="RendererModel.Padding"/>
        public DepictionGenerator WithPadding(double p)
        {
            return WithParam(typeof(RendererModel.Padding), (double?)p);
        }

        /// <summary>
        /// Specify a desired zoom factor - this changes the base size of a
        /// depiction and is used for uniformly making depictions bigger. If
        /// you would like to simply fill all available space (not recommended)
        /// use <see cref="WithFillToFit"/>.
        /// </summary>
        /// <remarks>
        /// The zoom is a scaling factor, specifying a zoom of 2 is double size,
        /// 0.5 half size, etc.
        /// </remarks>
        /// <param name="zoom">zoom factor</param>
        /// <returns>new generator for method chaining</returns>
        /// <seealso cref="BasicSceneGenerator.ZoomFactor"/>
        public DepictionGenerator WithZoom(double zoom)
        {
            return WithParam(typeof(BasicSceneGenerator.ZoomFactor), (double?)zoom);
        }

        /// <summary>
        /// Resize depictions to fill all available space (only if a size is specified).
        /// This generally isn't wanted as very small molecules (e.g. acetaldehyde) may
        /// become huge.
        /// </summary>
        /// <returns>new generator for method chaining</returns>
        /// <seealso cref="BasicSceneGenerator.FitToScreen"/>
        public DepictionGenerator WithFillToFit()
        {
            return WithParam(typeof(BasicSceneGenerator.FitToScreen), (bool?)true);
        }

        /// <summary>
        /// Low-level option method to set a rendering model parameter.
        /// </summary>
        /// <typeparam name="U">option value type</typeparam>
        /// <param name="key">option key, IGeneratorParameter&lt;S&gt;, S, U extends S</param>
        /// <param name="value">option value</param>
        /// <returns>new generator for method chaining</returns>
        public DepictionGenerator WithParam<U>(Type key, U value)
        {
            DepictionGenerator copy = new DepictionGenerator(this);
            copy.SetParam(key, value);
            return copy;
        }

        private double CaclModelScale(ICollection<IAtomContainer> mols)
        {
            var bonds = new List<IBond>();
            foreach (var mol in mols)
            {
                foreach (var bond in mol.Bonds)
                {
                    bonds.Add(bond);
                }
            }
            return CalcModelScaleForBondLength(MedianBondLength(bonds));
        }

        private double CaclModelScale(IReaction rxn)
        {
            var mols = new List<IAtomContainer>();
            foreach (var mol in rxn.Reactants)
                mols.Add(mol);
            foreach (var mol in rxn.Products)
                mols.Add(mol);
            foreach (var mol in rxn.Agents)
                mols.Add(mol);
            return CaclModelScale(mols);
        }

        private double MedianBondLength(ICollection<IBond> bonds)
        {
            if (!bonds.Any())
                return 1.5;
            int nBonds = 0;
            double[] lengths = new double[bonds.Count];
            foreach (var bond in bonds)
            {
                Vector2 p1 = bond.Begin.Point2D.Value;
                Vector2 p2 = bond.End.Point2D.Value;
                // watch out for overlaid atoms (occur in multiple group Sgroups)
                if (!p1.Equals(p2))
                    lengths[nBonds++] = Vector2.Distance(p1, p2);
            }
            Array.Sort(lengths, 0, nBonds);
            return lengths[nBonds / 2];
        }

        private double CalcModelScaleForBondLength(double bondLength)
        {
            return GetParameterValueV<double>(typeof(BasicSceneGenerator.BondLength)) / bondLength;
        }

        private static FontFamily GetDefaultOsFont()
        {
            // TODO: Native Font Support - choose best for Win/Linux/OS X etc
            return WPF.SystemFonts.MessageFontFamily;
        }

        /// <summary>
        /// Utility class for storing coordinates and bond types and resetting them after use.
        /// </summary>
        private sealed class LayoutBackup
        {
            private readonly Vector2?[] coords;
            private readonly BondStereo[] btypes;
            private readonly IAtomContainer mol;

            public LayoutBackup(IAtomContainer mol)
            {
                int numAtoms = mol.Atoms.Count;
                int numBonds = mol.Bonds.Count;
                this.coords = new Vector2?[numAtoms];
                this.btypes = new BondStereo[numBonds];
                this.mol = mol;
                for (int i = 0; i < numAtoms; i++)
                {
                    IAtom atom = mol.Atoms[i];
                    coords[i] = atom.Point2D;
                    if (coords[i] != null)
                        atom.Point2D = coords[i]; // copy
                }
                for (int i = 0; i < numBonds; i++)
                {
                    IBond bond = mol.Bonds[i];
                    btypes[i] = bond.Stereo;
                }
            }

            internal void Reset()
            {
                int numAtoms = mol.Atoms.Count;
                int numBonds = mol.Bonds.Count;
                for (int i = 0; i < numAtoms; i++)
                    mol.Atoms[i].Point2D = coords[i];
                for (int i = 0; i < numBonds; i++)
                    mol.Bonds[i].Stereo = btypes[i];
            }
        }
    }
}
