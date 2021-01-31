using System;
using System.Windows;
using System.Windows.Media;

namespace NCDK.Renderers
{
    internal static class RendererTool
    {
        public static Typeface DeriveFont(this Typeface font, Tuple<FontStyle, FontWeight> style)
            => font.DeriveFont(style.Item1, style.Item2);

        public static Typeface DeriveFont(this Typeface font, FontStyle style)
            => font.DeriveFont(style, font.Weight);

        public static Typeface DeriveFont(this Typeface font, FontWeight weight)
            => font.DeriveFont(font.Style, weight);

        public static Typeface DeriveFont(this Typeface font, FontStyle style, FontWeight weight)
        {
            if (style == null)
                style = font.Style;
            if (weight == null)
                weight = font.Weight;

            return new Typeface(font.FontFamily, style, weight, font.Stretch);
        }
    }
}
