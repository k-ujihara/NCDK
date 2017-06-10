using NCDK.Renderers.Elements;
using System.Windows.Media.Imaging;

namespace NCDK.Depict
{
    public class SvgDrawVisitor_Example
    {
        public void Main()
        {
            IRenderingElement renderingElements = null;
            #region
            SvgDrawVisitor visitor = new SvgDrawVisitor(50, 50);
            visitor.Visit(renderingElements);
            string svg = visitor.ToString();
            #endregion
        }
    }
}
