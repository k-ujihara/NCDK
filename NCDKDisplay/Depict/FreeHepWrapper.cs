/*
 * Copyright (c) 2015 John May <jwmay@users.sf.net>
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
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */

using System;
using System.IO;
using System.Text;
using System.Windows.Media;

namespace NCDK.Depict
{
    /// <summary>
    /// Internal - wrapper around the FreeHEP vector graphics output that makes things consistent
    ///  in terms of writing the required headers and footers.
    ///  <a href="http://java.freehep.org/">java.freehep.org</a>
    /// </summary>
    sealed class FreeHepWrapper
    {
        private readonly MemoryStream bout;
        private readonly string fmt;
        internal readonly DrawingContext g2;

        public FreeHepWrapper(string fmt, double w, double h)
        {
            try
            {
                this.g2 = CreateGraphics2d(this.fmt = fmt,
                                           this.bout = new MemoryStream(),
                                           new Dimension((int)Math.Ceiling(w), (int)Math.Ceiling(h)));
            }
            catch (IOException e)
            {
                throw new ApplicationException($"Could not create Vector Graphics output: {e.Message}");
            }
        }

        private static DrawingContext CreateGraphics2d(string fmt, Stream out_, Dimension dim)
        {
#if !IMPLEMENTED
            throw new System.NotImplementedException();
#else
            switch (fmt) {
            case Depiction.SVG_FMT:
                SVGGraphics2D svg = new SVGGraphics2D(out, dim);
                svg.SetCreator("Chemistry Development Kit (http://www.github.com/cdk/)");
                svg.WriteHeader();
                return svg;
            case Depiction.PDF_FMT:
                PDFGraphics2D pdf = new PDFGraphics2D(out, dim);
                pdf.SetCreator("Chemistry Development Kit (http://www.github.com/cdk/)");
                Properties props = new Properties();
                props.SetProperty(PDFGraphics2D.FIT_TO_PAGE, "false");
                props.SetProperty(PDFGraphics2D.PAGE_SIZE, PDFGraphics2D.CUSTOM_PAGE_SIZE);
                props.SetProperty(PDFGraphics2D.CUSTOM_PAGE_SIZE, dim.width + ", " + dim.height);
                props.SetProperty(PDFGraphics2D.PAGE_MARGINS, "0, 0, 0, 0");
                pdf.SetProperties(props);
                pdf.WriteHeader();
                return pdf;
            case Depiction.PS_FMT:
                // can't scale page size correctly in FreeEHP atm
            default:
                throw new IOException("Unsupported vector format, " + fmt);
        }
#endif
        }

        public void Dispose()
        {
#if IMPLEMENTED
            try {
                switch (fmt) {
                    case Depiction.SVG_FMT:
                        ((SVGGraphics2D)g2).WriteTrailer();
                        ((SVGGraphics2D)g2).CloseStream();
                        break;
                    case Depiction.PDF_FMT:
                        ((PDFGraphics2D)g2).WriteTrailer();
                        ((PDFGraphics2D)g2).CloseStream();
                        break;
                }
            } catch (IOException e) {
                // ignored we write to an internal array
            }
#endif
            g2.Close();
        }

        public override string ToString()
        {
            string result = Encoding.UTF8.GetString(bout.ToArray());
            // we want SVG in mm not pixels!
            if (fmt.Equals(Depiction.SVG_FMT))
            {
                System.Text.RegularExpressions.Regex.Replace(result, "\"([-+0-9.]+)px\"", "\"$1mm\"");
            }
            return result;
        }
    }
}
