using System.Windows;
using System.Windows.Media;

namespace NCDK.Renderers
{
    internal static class RendererTool
    {
        public static Typeface DeriveFont(this Typeface font, FontStyle? style=null, FontWeight? weight=null)
        {
            if (style == null)
                style = font.Style;
            if (weight == null)
                weight = font.Weight;

            return new Typeface(font.FontFamily, style.Value, weight.Value, font.Stretch);
        }
    }
}
