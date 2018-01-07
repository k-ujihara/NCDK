using System;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using GDI = System.Drawing;
using WPF = System.Windows;

namespace NCDK.Renderers
{
    internal static class WPFUtil
    {
        public static double GetCenterX(this WPF.Rect rect)
        {
            return (rect.Left + rect.Right) / 2;
        }

        public static double GetCenterY(this WPF.Rect rect)
        {
            return (rect.Top + rect.Bottom) / 2;
        }

        internal static GDI.FontStyle ToGDI(WPF.FontStyle style, WPF.FontWeight weight)
        {
            GDI.FontStyle s = 0;
            if (style == WPF.FontStyles.Normal)
                s |= GDI.FontStyle.Regular;
            else if (style == WPF.FontStyles.Italic)
                s |= GDI.FontStyle.Italic;
            else if (style == WPF.FontStyles.Oblique)
                s |= GDI.FontStyle.Italic;
            else
                s |= GDI.FontStyle.Regular;

            if (weight >= WPF.FontWeights.Bold)
                s |= GDI.FontStyle.Bold;
            return s;
        }

        public static WPF.Media.PathGeometry ToPathGeometry(string text, WPF.Media.Typeface font, double emSize)
        {
            return ToPathGeometry(text, font.FontFamily.Source, ToGDI(font.Style, font.Weight), (float)emSize);
        }

        internal static GDI::FontFamily GetFontFamily(string name)
        {
            switch (name)
            {
                case "GlobalMonospace.CompositeFont":
                    return GDI::FontFamily.GenericMonospace;
                case "GlobalSanSerif.CompositeFont":
                    return GDI::FontFamily.GenericSansSerif;
                case "GlobalSerif.CompositeFont":
                    return GDI::FontFamily.GenericSerif;
                case "GlobalUserInterface.CompositeFont":
                    name = new WPF.Media.FontFamily().Source;
                    goto default;
                default:
                    return new GDI.FontFamily(name);
            }
        }

        internal static WPF.Media.PathGeometry ToPathGeometry(string text, string fontFamilyName, GDI.FontStyle style, float emSize)
        {
            var pathGDI = new GraphicsPath();
            pathGDI.AddString(text, GetFontFamily(fontFamilyName), (int)style, emSize, GDI.Point.Empty, GDI.StringFormat.GenericDefault);

            var pathGeometry = new WPF.Media.PathGeometry();
            WPF.Media.PathFigure currFigure = null;

            for (int i = 0; i < pathGDI.PointCount; i++)
            {
                var t = (PathPointType)pathGDI.PathTypes[i];
                var p = pathGDI.PathPoints[i];
                p.X = p.X;
                p.Y = p.Y;

                switch (t & PathPointType.PathTypeMask)
                {
                    case PathPointType.Start:
                        if (currFigure != null)
                        {
                            currFigure.IsClosed = true;
                            pathGeometry.Figures.Add(currFigure);
                            currFigure = null;
                        }
                        currFigure = new WPF.Media.PathFigure
                        {
                            StartPoint = new WPF.Point(p.X, p.Y)
                        };
                        break;
                    case PathPointType.Line:
                        {
                            var segment = new WPF.Media.LineSegment(new WPF.Point(p.X, p.Y), true);
                            currFigure.Segments.Add(segment);
                        }
                        break;
                    case PathPointType.Bezier:
                        {
                            if (pathGDI.PointCount < i + 3)
                            {
                                Trace.TraceWarning($"{nameof(GraphicsPath.AddString)} returned invalid sequence in {nameof(ToPathGeometry)}.");
                                break;
                            }

                            var t1 = (PathPointType)pathGDI.PathTypes[i + 1];
                            if ((t1 & PathPointType.PathTypeMask) != PathPointType.Bezier)
                            {
                                Trace.TraceWarning($"{nameof(GraphicsPath.AddString)} returned invalid sequence in {nameof(ToPathGeometry)}.");
                                break;
                            }
                            var p1 = pathGDI.PathPoints[i + 1];
                            p1.X = p1.X;
                            p1.Y = p1.Y;
                            i++;
                            var t2 = (PathPointType)pathGDI.PathTypes[i + 1];
                            if ((t2 & PathPointType.PathTypeMask) != PathPointType.Bezier)
                            {
                                Trace.TraceWarning($"{nameof(GraphicsPath.AddString)} returned invalid sequence in {nameof(ToPathGeometry)}.");
                                break;
                            }
                            var p2 = pathGDI.PathPoints[i + 1];
                            p2.X = p2.X;
                            p2.Y = p2.Y;
                            i++;
                            var segment = new WPF.Media.BezierSegment(
                                new WPF.Point(p.X, p.Y),
                                new WPF.Point(p1.X, p1.Y),
                                new WPF.Point(p2.X, p2.Y),
                                true);
                            currFigure.Segments.Add(segment);
                        }
                        break;
                    default:
                        throw new Exception();
                }
                switch (t & PathPointType.CloseSubpath)
                {
                    case PathPointType.DashMode:
                    // not supported.
                    case PathPointType.PathMarker:
                        // ignore.
                        break;
                    case PathPointType.CloseSubpath:
                        if (currFigure != null)
                        {
                            currFigure.IsClosed = true;
                            pathGeometry.Figures.Add(currFigure);
                            currFigure = null;
                        }
                        break;
                    default:
                        break;
                }
            }
            if (currFigure != null)
            {
                currFigure.IsClosed = true;
                pathGeometry.Figures.Add(currFigure);
                currFigure = null;
            }

            return pathGeometry;
        }
    }
}
