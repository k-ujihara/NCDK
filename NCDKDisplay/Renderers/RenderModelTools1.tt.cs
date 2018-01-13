
using WPF = System.Windows;

namespace NCDK.Renderers
{

    public class RenderModelTools
    {
        /// <summary>
        /// Get the color of a selection.
        /// </summary>
        /// <returns>the color of a selection</returns>
        public WPF.Media.Color GetSelectionColor(this RendererModel model)
        {
            const string key = "SelectionColor";
            WPF.Media.Color value;
            if (model.Parameters.TryGetValue(key, out object v))
                value = (WPF.Media.Color)v;
            else
                model.Parameters[key] = value = WPF.Media.Color.FromRgb(0x49, 0xdf, 0xff);

            return value;
        }

        /// <summary>
        /// Set the color of a selection.
        /// </summary>
		public void SetSelectionColor(this RendererModel model, WPF.Media.Color value)
        {
            const string key = "SelectionColor";
            model.Parameters[key] = value;
        }

    }
}
