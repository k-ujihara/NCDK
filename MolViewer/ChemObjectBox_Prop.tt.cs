

using System;
using NCDK.Renderers;
using NCDK.Renderers.Colors;
using System.Windows;
using WPF = System.Windows;

namespace NCDK.MolViewer
{
    public partial class ChemObjectBox : System.Windows.Controls.UserControl
    {
        public static readonly DependencyProperty ChemObjectProperty =
			DependencyProperty.Register(
				"ChemObject",
				typeof(IChemObject),
				typeof(ChemObjectBox),
				new FrameworkPropertyMetadata(
					(IChemObject)(null),
					new PropertyChangedCallback(OnChemObjectPropertyChanged)));

        public IChemObject ChemObject
        {
            get { return (IChemObject)GetValue(ChemObjectProperty); }
            set { SetValue(ChemObjectProperty, value); }
        }

		private static void OnChemObjectPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
			if (d is ChemObjectBox o)
            {
                var value = (IChemObject)e.NewValue;var old = o._ChemObject;if (old != value) {    o._ChemObject = value;    o.ChemObjectChanged?.Invoke(d, new ChemObjectChangedEventArgs(old, value));}
				o.UpdateVisual();
            }
        }
        public static readonly DependencyProperty AtomColorerProperty =
			DependencyProperty.Register(
				"AtomColorer",
				typeof(IAtomColorer),
				typeof(ChemObjectBox),
				new FrameworkPropertyMetadata(
					(IAtomColorer)(new UniColor(WPF.Media.Colors.Black)),
					new PropertyChangedCallback(OnAtomColorerPropertyChanged)));

        public IAtomColorer AtomColorer
        {
            get { return (IAtomColorer)GetValue(AtomColorerProperty); }
            set { SetValue(AtomColorerProperty, value); }
        }

		private static void OnAtomColorerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
			if (d is ChemObjectBox o)
            {
                o.Generator.AtomColorer = (IAtomColorer)e.NewValue;
				o.UpdateVisual();
            }
        }
        public static readonly DependencyProperty BackgroundColorProperty =
			DependencyProperty.Register(
				"BackgroundColor",
				typeof(WPF.Media.Color),
				typeof(ChemObjectBox),
				new FrameworkPropertyMetadata(
					(WPF.Media.Color)(WPF.Media.Colors.Transparent),
					new PropertyChangedCallback(OnBackgroundColorPropertyChanged)));

        public WPF.Media.Color BackgroundColor
        {
            get { return (WPF.Media.Color)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

		private static void OnBackgroundColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
			if (d is ChemObjectBox o)
            {
                o.Generator.BackgroundColor = (WPF.Media.Color)e.NewValue;
				o.UpdateVisual();
            }
        }
        public static readonly DependencyProperty HighlightingProperty =
			DependencyProperty.Register(
				"Highlighting",
				typeof(HighlightStyle),
				typeof(ChemObjectBox),
				new FrameworkPropertyMetadata(
					(HighlightStyle)(HighlightStyle.None),
					new PropertyChangedCallback(OnHighlightingPropertyChanged)));

        public HighlightStyle Highlighting
        {
            get { return (HighlightStyle)GetValue(HighlightingProperty); }
            set { SetValue(HighlightingProperty, value); }
        }

		private static void OnHighlightingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
			if (d is ChemObjectBox o)
            {
                o.Generator.Highlighting = (HighlightStyle)e.NewValue;
				o.UpdateVisual();
            }
        }
        public static readonly DependencyProperty OuterGlowWidthProperty =
			DependencyProperty.Register(
				"OuterGlowWidth",
				typeof(double),
				typeof(ChemObjectBox),
				new FrameworkPropertyMetadata(
					(double)(RendererModelTools.DefaultOuterGlowWidth),
					new PropertyChangedCallback(OnOuterGlowWidthPropertyChanged)));

        public double OuterGlowWidth
        {
            get { return (double)GetValue(OuterGlowWidthProperty); }
            set { SetValue(OuterGlowWidthProperty, value); }
        }

		private static void OnOuterGlowWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
			if (d is ChemObjectBox o)
            {
                o.Generator.OuterGlowWidth = (double)e.NewValue;
				o.UpdateVisual();
            }
        }
        public static readonly DependencyProperty ZoomProperty =
			DependencyProperty.Register(
				"Zoom",
				typeof(double),
				typeof(ChemObjectBox),
				new FrameworkPropertyMetadata(
					(double)(1),
					new PropertyChangedCallback(OnZoomPropertyChanged)));

        public double Zoom
        {
            get { return (double)GetValue(ZoomProperty); }
            set { SetValue(ZoomProperty, value); }
        }

		private static void OnZoomPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
			if (d is ChemObjectBox o)
            {
                o.Generator.Zoom = (double)e.NewValue;
				o.UpdateVisual();
            }
        }
        public static readonly DependencyProperty AlignMappedReactionProperty =
			DependencyProperty.Register(
				"AlignMappedReaction",
				typeof(bool),
				typeof(ChemObjectBox),
				new FrameworkPropertyMetadata(
					(bool)(true),
					new PropertyChangedCallback(OnAlignMappedReactionPropertyChanged)));

        public bool AlignMappedReaction
        {
            get { return (bool)GetValue(AlignMappedReactionProperty); }
            set { SetValue(AlignMappedReactionProperty, value); }
        }

		private static void OnAlignMappedReactionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
			if (d is ChemObjectBox o)
            {
                o.Generator.AlignMappedReaction = (bool)e.NewValue;
				o.UpdateVisual();
            }
        }
	}
}
