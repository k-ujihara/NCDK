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
using NCDK.Renderers;
using NCDK.Renderers.Elements;
using NCDK.Renderers.Fonts;
using NCDK.Renderers.Generators;
using NCDK.Renderers.Visitors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NCDK.Depict
{
    /// <summary>
    /// Base class of a pre-rendered depiction. The class allows introspection of
    /// depiction size (decided at generation time) and serialization to raster
    /// and vector graphic formats.
    /// </summary>
    // @author John May
    public abstract class Depiction
    {
        /// <summary>
        /// For converting MM coordinates to PS Point (1/72 inch)
        /// </summary>
        protected const double MM_TO_POINT = 2.83464566751;

        /// <summary>
        /// When no fixed padding value is specified we use margin
        /// multiplied by this value.
        /// </summary>
        protected const double DEFAULT_PADDING_FACTOR = 2;

        /// <summary>
        /// Structured Vector Graphics (SVG) format key.
        /// </summary>
        public const string SVG_FMT = "svg";

        /// <summary>
        /// PostScript (PS) format key.
        /// </summary>
        public const string PS_FMT = "ps";

        /// <summary>
        /// Portable Document Format (PDF) format key.
        /// </summary>
        public const string PDF_FMT = "pdf";

        /// <summary>
        /// Joint Photographic Experts Group (JPG) format key.
        /// </summary>
        public const string JPG_FMT = "jpg";

        /// <summary>
        /// Portable Network Graphics (PNG) format key.
        /// </summary>
        public const string PNG_FMT = "png";

        /// <summary>
        /// Graphics Interchange Format (GIF) format key.
        /// </summary>
        public const string GIF_FMT = "gif";

        internal const double ACS_1996_BOND_LENGTH_MM = 5.08;

        private const char DOT = '.';

        private readonly RendererModel model;

        /// <summary>
        /// Internal method passes in the rendering model parameters.
        /// </summary>
        /// <param name="model">parameters</param>
        internal Depiction(RendererModel model)
        {
            this.model = model;
        }

        /// <summary>
        /// Render the depiction to a WPF.
        /// </summary>
        /// <returns>WPF Bitmap</returns>
        public abstract RenderTargetBitmap ToImg();

        /// <summary>
        /// Render the image to an SVG image.
        /// </summary>
        /// <returns>svg XML content</returns>
        public string ToSvgStr()
        {
            return ToVecStr(SVG_FMT);
        }

        /// <summary>
        /// Render the image to an EPS format string.
        /// </summary>
        /// <returns>eps content</returns>
        public string ToEpsStr()
        {
            return ToVecStr(PS_FMT);
        }

        /// <summary>
        /// Render the image to an PDF format string.
        /// </summary>
        /// <returns>pdf content</returns>
        public string ToPdfStr()
        {
            return ToVecStr(PDF_FMT);
        }

        /// <summary>
        /// Access the specified padding value or fallback to a provided
        /// default.
        /// </summary>
        /// <param name="defaultPadding">default value if the parameter is 'automatic'</param>
        /// <returns>padding</returns>
        internal double GetPaddingValue(double defaultPadding)
        {
            double padding = model.GetV<double>(typeof(RendererModel.Padding));
            if (padding == DepictionGenerator.AUTOMATIC)
                padding = defaultPadding;
            return padding;
        }

        /// <summary>
        /// Access the specified margin value or fallback to a provided
        /// default.
        /// </summary>
        /// <param name="defaultMargin">default value if the parameter is 'automatic'</param>
        /// <returns>margin</returns>
        internal double GetMarginValue(double defaultMargin)
        {
            double margin = model.GetV<double>(typeof(BasicSceneGenerator.Margin));
            if (margin == DepictionGenerator.AUTOMATIC)
                margin = defaultMargin;
            return margin;
        }

        /// <summary>
        /// Internal - implementations should overload this method for vector graphics
        /// rendering.
        /// </summary>
        /// <param name="fmt">the vector graphics format</param>
        /// <returns>the vector graphics format string</returns>
        internal abstract string ToVecStr(string fmt);

        /// <summary>
        /// List the available formats that can be rendered.
        /// </summary>
        /// <returns>supported formats</returns>
        public virtual IList<string> ListFormats()
        {
            var formats = new List<string>();
            formats.Add(SVG_FMT);
            formats.Add(SVG_FMT.ToUpperInvariant());
            formats.Add(PS_FMT);
            formats.Add(PS_FMT.ToUpperInvariant());
            formats.Add(PDF_FMT);
            formats.Add(PDF_FMT.ToUpperInvariant());

            formats.Add("bmp");
            formats.Add("bmp".ToUpperInvariant());
            formats.Add(GIF_FMT);
            formats.Add(GIF_FMT.ToUpperInvariant());
            formats.Add(JPG_FMT);
            formats.Add(JPG_FMT.ToUpperInvariant());
            formats.Add(PNG_FMT);
            formats.Add(PNG_FMT.ToUpperInvariant());
            formats.Add("tif");
            formats.Add("tif".ToUpperInvariant());
            formats.Add("wmp");
            formats.Add("wmp".ToUpperInvariant());
            return formats;
        }

        /// <summary>
        /// Write the depiction to the provided output stream.
        /// </summary>
        /// <param name="fmt">format</param>
        /// <param name="output">output stream</param>
        /// <exception cref="IOException">depiction could not be written, low level IO problem</exception>
        /// <seealso cref="ListFormats"/>
        public void WriteTo(string fmt, Stream output)
        {
            switch (fmt.ToLowerInvariant())
            {
                case SVG_FMT:
                    {
                        var bytes = Encoding.UTF8.GetBytes(ToSvgStr());
                        output.Write(bytes, 0, bytes.Length);
                    }
                    break;
                case PS_FMT:
                    {
                        var bytes = Encoding.UTF8.GetBytes(ToEpsStr());
                        output.Write(bytes, 0, bytes.Length);
                    }
                    break;
                case PDF_FMT:
                    {
                        var bytes = Encoding.UTF8.GetBytes(ToPdfStr());
                        output.Write(bytes, 0, bytes.Length);
                    }
                    break;
                default:
                    //{
                    //    var bytes = Encoding.UTF8.GetBytes(ToImg());
                    //    ImageIO.Write(ToImg(), fmt, out);
                    //}
                    //break;
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Write the depiction to the provided output stream.
        /// </summary>
        /// <param name="fmt">format</param>
        /// <param name="path">output destination path</param>
        /// <exception cref="IOException">depiction could not be written, low level IO problem</exception>
        /// <seealso cref="ListFormats"/>
        public void WriteTo(string fmt, string path)
        {
            path = ReplaceTildeWithHomeDir(EnsureSuffix(path, fmt));
            using (var output = new FileStream(path, FileMode.Create))
            {
                WriteTo(fmt, output);
            }
        }

        /// <summary>
        /// Write the depiction to the provided file path, the format is determined
        /// by the path suffix.
        /// </summary>
        /// <param name="path">output destination path</param>
        /// <exception cref="IOException">depiction could not be written, low level IO problem</exception>
        /// <seealso cref="listFormats"/>
        public void WriteTo(string path)
        {
            int i = path.LastIndexOf(DOT);
            if (i < 0 || i + 1 == path.Length)
                throw new IOException("Cannot find suffix in provided path: " + path);
            string fmt = path.Substring(i + 1);
            WriteTo(fmt, path);
        }

        /// <summary>
        /// Utility for resolving paths on unix systems that contain tilda for
        /// the home directory.
        /// </summary>
        /// <param name="path">the file system path</param>
        /// <returns>normalised path</returns>
        private static string ReplaceTildeWithHomeDir(string path)
        {
            if (path.StartsWith("~/"))
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), path.Substring(2));
            return path;
        }

        /// <summary>
        /// Ensures a suffix on a file output if the path doesn't
        /// currently end with it. For example calling
        /// {@code WriteTo(SVG_FMT, "~/chemical")} would create a file
        /// {@code ~/chemical.svg}.
        /// </summary>
        /// <param name="path">the file system path</param>
        /// <param name="suffix">the format suffix</param>
        /// <returns>path with correct suffix</returns>
        private static string EnsureSuffix(string path, string suffix)
        {
            if (path.EndsWith(DOT + suffix))
                return path;
            return path + DOT + suffix;
        }

        /// <summary>
        /// Low-level draw method used by other rendering methods.
        /// </summary>
        /// <param name="visitor">the draw visitor</param>
        /// <param name="bounds">a bound rendering element</param>
        /// <param name="zoom">if the diagram is zoomed at all</param>
        /// <param name="viewBounds">the view bounds - the root will be centered in the bounds</param>
        protected void Draw(IDrawVisitor visitor, double zoom, Bounds bounds, Rect viewBounds)
        {
            double modelScale = zoom * model.GetV<double>(typeof(BasicSceneGenerator.Scale));
            double zoomToFit = Math.Min(viewBounds.Width / (bounds.Width * modelScale), viewBounds.Height / (bounds.Height * modelScale));
            Matrix transform = Matrix.Identity;
            transform.Translate(viewBounds.GetCenterX(), viewBounds.GetCenterY());
            transform.Scale(modelScale, -modelScale);

            // default is shrink only unless specified
            if (model.GetV<bool>(typeof(BasicSceneGenerator.FitToScreen)) || zoomToFit < 1)
                transform.Scale(zoomToFit, zoomToFit);

            transform.Translate(-(bounds.minX + bounds.maxX) / 2, -(bounds.minY + bounds.maxY) / 2);

            // not always needed
            var fontManager = new WPFFontManager();
            fontManager.Zoom = zoomToFit;

            visitor.SetRendererModel(model);
            visitor.SetFontManager(fontManager);
            visitor.SetTransform(new MatrixTransform(transform));

            // setup up transform
            visitor.Visit(bounds.Root);
        }

        /// <summary>
        /// Utility method for recalling a depiction in pixels to one in millimeters.
        /// </summary>
        /// <param name="bondLength">the desired bond length (mm)</param>
        /// <returns>the scaling factor</returns>
        internal double RescaleForBondLength(double bondLength)
        {
            return bondLength / model.GetV<double>(typeof(BasicSceneGenerator.BondLength));
        }

        protected internal void SvgPrevisit(string fmt, double rescale, SvgDrawVisitor visitor, IEnumerable<IRenderingElement> elements)
        {
            visitor.SetTransform(new TranslateTransform(rescale, rescale));
            visitor.Previsit(elements);
            visitor.SetTransform(null);
        }
    }
}
