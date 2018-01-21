

using NCDK.Renderers;
using NCDK.Renderers.Colors;
using Prism.Mvvm;
using WPF = System.Windows;

namespace NCDK.MolViewer
{
    partial class AppearanceViewModel : BindableBase
    {
		private double _Zoom = 1;
        public double Zoom
        {
            get { return _Zoom; }
            set { this.SetProperty(ref this._Zoom, value); }
        }
		private bool _AlignMappedReaction = true;
        public bool AlignMappedReaction
        {
            get { return _AlignMappedReaction; }
            set { this.SetProperty(ref this._AlignMappedReaction, value); }
        }
		private WPF.Media.Color _BackgroundColor = WPF.Media.Colors.Transparent;
        public WPF.Media.Color BackgroundColor
        {
            get { return _BackgroundColor; }
            set { this.SetProperty(ref this._BackgroundColor, value); }
        }
		private IAtomColorer _AtomColorer = new UniColor(WPF.Media.Colors.Black);
        public IAtomColorer AtomColorer
        {
            get { return _AtomColorer; }
            set { this.SetProperty(ref this._AtomColorer, value); }
        }
		private HighlightStyle _Highlighting = HighlightStyle.None;
        public HighlightStyle Highlighting
        {
            get { return _Highlighting; }
            set { this.SetProperty(ref this._Highlighting, value); }
        }
		private double _OuterGlowWidth = RendererModelTools.DefaultOuterGlowWidth;
        public double OuterGlowWidth
        {
            get { return _OuterGlowWidth; }
            set { this.SetProperty(ref this._OuterGlowWidth, value); }
        }
	}
}
