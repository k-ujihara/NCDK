using System.Windows;

namespace NCDK.Renderers
{
    public static class WPFUtil
    {
        public static double GetCenterX(this Rect rect)
        {
            return (rect.Left + rect.Right) / 2;
        }

        public static double GetCenterY(this Rect rect)
        {
            return (rect.Top + rect.Bottom) / 2;
        }
    }
}
