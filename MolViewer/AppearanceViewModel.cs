/*
 * Copyright (C) 2018  Kazuya Ujihara <ujihara.kazuya@gmail.com>
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using NCDK.Geometries;
using NCDK.Graphs.InChI;
using NCDK.Layout;
using NCDK.Renderers;
using NCDK.Renderers.Colors;
using NCDK.Silent;
using NCDK.Smiles;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using WPF = System.Windows;

namespace NCDK.MolViewer
{
    partial class AppearanceViewModel : BindableBase
    {
        private static StructureDiagramGenerator sdg = new StructureDiagramGenerator();
        private static IChemObjectBuilder builder = ChemObjectBuilder.Instance;
        private static SmilesParser parser = new SmilesParser(builder);
        private static SmilesGenerator smilesGenerator = new SmilesGenerator(SmiFlavor.Generic);
        private static InChIGeneratorFactory inChIGeneratorFactory = InChIGeneratorFactory.Instance;

        private string _Smiles = null;
        private IChemObject _ChemObject = null;
        private ColoringStyle _Coloring = ColoringStyle.COW;
        private DelegateCommand _CleanStructureCommand;
        private DelegateCommand _PasteAsInChICommand;

        public string Smiles
        {
            get { return _Smiles; }

            set
            {
                if (this._Smiles != value)
                {
                    var text = value;
                    if (string.IsNullOrWhiteSpace(text))
                        return;

                    IChemObject o;

                    if (IsReactionSmilees(text))
                    {
                        IReaction rxn = null;
                        try
                        {
                            rxn = parser.ParseReactionSmiles(text);
                        }
                        catch (Exception)
                        {
                            // ignore
                        }

                        o = rxn;
                    }
                    else
                    {
                        IAtomContainer mol = null;
                        try
                        {
                            mol = parser.ParseSmiles(text);
                        }
                        catch (Exception)
                        {
                            // ignore
                        }

                        o = mol;
                    }

                    if (o != null)
                    {
                        _ChemObject = o;
                        this.RaisePropertyChanged(nameof(ChemObject));
                    }

                    this.SetProperty(ref this._Smiles, text);
                }
            }
        }

        public IChemObject ChemObject
        {
            get => _ChemObject;
            set
            {
                switch (value)
                {
                    case IAtomContainer mol:
                        if (!GeometryUtil.Has2DCoordinates(mol))
                        {
                            mol = (IAtomContainer)mol.Clone();
                            sdg.GenerateCoordinates(mol);
                            value = mol;
                        }
                        break;
                    case IReaction rxn:
                        if (!GeometryUtil.Has2DCoordinates(rxn))
                        {
                            rxn = (IReaction)rxn.Clone();
                            sdg.GenerateCoordinates(rxn);
                            value = rxn;
                        }
                        break;
                    default:
                        break;
                }

                this.SetProperty(ref this._ChemObject, value);
                OnChemObjectChanged();
            }
        }

        public ColoringStyle Coloring
        {
            get { return _Coloring; }
            set
            {
                this.SetProperty(ref this._Coloring, value);

                switch (value)
                {
                    case ColoringStyle.COW:
                        AtomColorer = new CDK2DAtomColors();
                        BackgroundColor = WPF.Media.Colors.White;
                        Highlighting = HighlightStyle.OuterGlow;
                        OuterGlowWidth = 4;
                        break;
                    case ColoringStyle.COT:
                        AtomColorer = new CDK2DAtomColors();
                        BackgroundColor = WPF.Media.Colors.Transparent;
                        Highlighting = HighlightStyle.OuterGlow;
                        OuterGlowWidth = 4;
                        break;
                    case ColoringStyle.BOW:
                        AtomColorer = new UniColor(WPF.Media.Colors.Black);
                        BackgroundColor = WPF.Media.Colors.White;
                        Highlighting = HighlightStyle.None;
                        OuterGlowWidth = RendererModelTools.DefaultOuterGlowWidth;
                        break;
                    case ColoringStyle.BOT:
                        AtomColorer = new UniColor(WPF.Media.Colors.Black);
                        BackgroundColor = WPF.Media.Colors.Transparent;
                        Highlighting = HighlightStyle.None;
                        OuterGlowWidth = RendererModelTools.DefaultOuterGlowWidth;
                        break;
                    case ColoringStyle.WOB:
                        AtomColorer = new UniColor(WPF.Media.Colors.White);
                        BackgroundColor = WPF.Media.Colors.Black;
                        Highlighting = HighlightStyle.None;
                        OuterGlowWidth = RendererModelTools.DefaultOuterGlowWidth;
                        break;
                    case ColoringStyle.COB:
                        AtomColorer = new CobColorer();
                        BackgroundColor = WPF.Media.Colors.Transparent;
                        Highlighting = HighlightStyle.OuterGlow;
                        OuterGlowWidth = 4;
                        break;
                    case ColoringStyle.NOB:
                        AtomColorer = new NobColorer();
                        BackgroundColor = WPF.Media.Colors.Black;
                        Highlighting = HighlightStyle.OuterGlow;
                        OuterGlowWidth = 4;
                        break;
                }
            }
        }

        public DelegateCommand CleanStructureCommand
        {
            get { return _CleanStructureCommand = _CleanStructureCommand ?? new DelegateCommand(CleanStructure); }
        }

        private void CleanStructure()
        {
            if (ChemObject == null)
                return;

            switch (ChemObject)
            {
                case IAtomContainer o:
                    o = (IAtomContainer)o.Clone();
                    sdg.GenerateCoordinates(o);
                    ChemObject = o;
                    break;
                case IReaction o:
                    o = (IReaction)o.Clone();
                    sdg.GenerateCoordinates(o);
                    ChemObject = o;
                    break;
                default:
                    Trace.TraceWarning($"'{ChemObject.GetType()}' is not supported.");
                    break;
            }
        }

        public DelegateCommand PasteAsInChICommand
        {
            get { return _PasteAsInChICommand = _PasteAsInChICommand ?? new DelegateCommand(PasteAsInChI); }
        }

        private void PasteAsInChI()
        {
            IAtomContainer mol = null;
            if (Clipboard.ContainsText())
            {
                var text = Clipboard.GetText();
                try
                {
                    // Get InChIToStructure
                    InChIToStructure converter = inChIGeneratorFactory.GetInChIToStructure(text, builder);
                    mol = converter.AtomContainer;
                }
                catch (Exception)
                {
                    // ignore
                }
            }
            if (mol == null)
                return;

            ChemObject = mol;
        }

        private void OnChemObjectChanged()
        {
            string smiles = null;

            try
            {
                switch (ChemObject)
                {
                    case IAtomContainer mol:
                        smiles = smilesGenerator.Create(mol);
                        break;
                    case IReaction rxn:
                        smiles = smilesGenerator.Create(rxn);
                        break;
                    default:
                        smiles = $"{ChemObject.GetType()} is not supported.";
                        break;
                }
            }
            catch (Exception e)
            {
                smiles = $"Failed to create SMILES: {e.Message}";
            }

            _Smiles = smiles; // not to change ChemObject
            base.RaisePropertyChanged(nameof(Smiles));
        }

        private static bool IsReactionSmilees(string smiles)
        {
            return smiles.Split(' ')[0].Contains(">");
        }
    }

    class CobColorer : IAtomColorer
    {
        private static readonly CDK2DAtomColors colors = new CDK2DAtomColors();

        public WPF.Media.Color GetAtomColor(IAtom atom)
        {
            var res = colors.GetAtomColor(atom);
            if (res.Equals(WPF.Media.Colors.Black))
                return WPF.Media.Colors.White;
            else
                return res;
        }

        public WPF.Media.Color GetAtomColor(IAtom atom, WPF.Media.Color color)
        {
            var res = colors.GetAtomColor(atom, color);
            if (res.Equals(WPF.Media.Colors.Black))
                return WPF.Media.Colors.White;
            else
                return res;
        }
    }

    class NobColorer : IAtomColorer
    {
        private static readonly CDK2DAtomColors colors = new CDK2DAtomColors();
        private static readonly WPF.Media.Color Neon = WPF.Media.Color.FromRgb(0x00, 0xFF, 0x0E);

        public WPF.Media.Color GetAtomColor(IAtom atom)
        {
            var res = colors.GetAtomColor(atom);
            if (res.Equals(WPF.Media.Colors.Black))
                return Neon;
            else
                return res;
        }

        public WPF.Media.Color GetAtomColor(IAtom atom, WPF.Media.Color color)
        {
            var res = colors.GetAtomColor(atom, color);
            if (res.Equals(WPF.Media.Colors.Black))
                return Neon;
            else
                return res;
        }
    }

    public enum ColoringStyle
    {
        COW, 
        COT,
        BOW,
        BOT,
        WOB,
        COB,
        NOB,
    }

    public class ColoringStyleToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((ColoringStyle)value).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ToEnum(value);
        }

        private ColoringStyle ToEnum(object parameter)
        {
            if (parameter is string p)
                return (ColoringStyle)Enum.Parse(typeof(ColoringStyle), p);
            return (ColoringStyle)0;
        }
    }

    public class F2Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is double))
                throw new ApplicationException();
            return ((double)value).ToString("F2");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string))
                throw new ApplicationException();
            return double.Parse((string)value);
        }
    }

    public class Power10Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is double))
                throw new ApplicationException();
            return Math.Log10((double)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is double))
                throw new ApplicationException();
            return Math.Pow(10, (double)value);
        }
    }
}
