/*
 * Copyright (C) 2017  Kazuya Ujihara <ujihara.kazuya@gmail.com>
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

using Microsoft.Win32;
using NCDK.Default;
using NCDK.Geometries;
using NCDK.IO;
using NCDK.Layout;
using NCDK.Renderers;
using NCDK.Renderers.Fonts;
using NCDK.Renderers.Generators;
using NCDK.Renderers.Visitors;
using NCDK.Smiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NCDK.MolViewer
{
    public partial class MolWindow : Window
    {
        private static IChemObjectBuilder builder = ChemObjectBuilder.Instance;
        private static SmilesParser parser = new SmilesParser(builder);

        public RendererModel Model { get; private set; }
        private IGenerator<IAtomContainer> molgen;
        private WPFFontManager fontManager = new WPFFontManager();
        private IAtomContainer mol = null;
        private StructureDiagramGenerator sdg = new StructureDiagramGenerator();

        private static FormatFactory formatFactory = new FormatFactory();
        private static ReaderFactory readerFactory = new ReaderFactory();

        class MolWindowListener : Events.ICDKChangeListener
        {
            MolWindow parent;
            public MolWindowListener(MolWindow parent)
            {
                this.parent = parent;
            }

            public void StateChanged(ChemObjectChangeEventArgs evt)
            {
                parent.Render();
            }
        }

        public MolWindow()
        {
            InitializeComponent();

            sdg.BondLength = 40;
            sdg.UseIdentityTemplates = false;
            Model = new RendererModel();
            molgen = new BasicGenerator();
            Model.RegisterParameters(molgen);
            Model.Listeners.Add(new MolWindowListener(this));
        }

        public static IAtomContainer Layout(StructureDiagramGenerator structureGenerator, IAtomContainer mol)
        {
            structureGenerator.Molecule = mol;
            structureGenerator.GenerateCoordinates();
            mol = structureGenerator.Molecule;

            return mol;
        }

        private void MenuItem_PasteAsInchi_Click(object sender, RoutedEventArgs e)
        {
            IAtomContainer mol = null;
            if (Clipboard.ContainsText())
            {
                var text = Clipboard.GetText();
                try
                {
                    using (var ins = new StringReader(text))
                    using (var reader = new InChIPlainTextReader(ins))
                    {
                        var chemFile = reader.Read(new ChemFile());
                        mol = chemFile.First().First().MoleculeSet.First();
                    }
                }
                catch (Exception)
                {
                    // ignore
                }
            }
            if (mol == null)
                return;

            mol = Layout(sdg, mol);

            this.mol = mol;
            Render();
        }

        private void MenuItem_PasteAsSmiles_Click(object sender, RoutedEventArgs e)
        {
            IAtomContainer mol = null;
            if (Clipboard.ContainsText())
            {
                var smiles = Clipboard.GetText();
                try
                {
                    mol = parser.ParseSmiles(smiles);
                }
                catch (Exception)
                {
                    // ignore
                }
            }
            if (mol == null)
                return;

            mol = Layout(sdg, mol);

            this.mol = mol;
            Render();
        }

        private void menuItem_SaveAs_Click(object sender, RoutedEventArgs e)
        {
            if (mol == null)
                return;

            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.FilterIndex = 1;
            fileDialog.Filter = "PNG file (*.png)|*.png|All Files (*.*)|*.*";
            bool? result = fileDialog.ShowDialog();
            if (result != true)
                return;

            // the renderer needs to have a toolkit-specific font manager
            var renderer = Utils.BuildStdRenderer();
            var r = NCDK.MolViewer.Renderers.AtomContainerRenderer.CalculateBounds(mol);
            var drawArea = new Rect(0, 0, r.Width, r.Height);

            // the call to 'setup' only needs to be done on the first paint
            renderer.Setup(mol, drawArea);

            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext g2d = drawingVisual.RenderOpen())
            {
                g2d.DrawRectangle(Brushes.White, null, drawArea);
                renderer.PaintMolecule(mol, new WPFDrawVisitor(g2d), drawArea, true);
                g2d.DrawRectangle(null, new Pen(Brushes.Black, 1), drawArea);
            }

            var bmp = new RenderTargetBitmap((int)drawArea.Width, (int)drawArea.Height, 72, 72, PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));
            using (var stream = new FileStream(fileDialog.FileName, FileMode.Create, FileAccess.Write))
            {
                encoder.Save(stream);
            }
        }

        public void Render()
        {
            if (mol == null)
                return;

            var rendeingrElement = molgen.Generate(mol, Model);
            DrawingGroup dGroup = new DrawingGroup();
            using (DrawingContext g2d = dGroup.Open())
            {
                DrawRendeingrElement(g2d, rendeingrElement);
            }
            ImageSource dImageSource = new DrawingImage(dGroup);
            image.Source = dImageSource;
        }

        private void DrawRendeingrElement(DrawingContext g2d, NCDK.Renderers.Elements.IRenderingElement rendeingrElement)
        {
            var v = new WPFDrawVisitor(g2d);
            v.SetTransform(new ScaleTransform(1, 1));
            v.SetFontManager(fontManager);
            v.SetRendererModel(Model);
            v.Visit(rendeingrElement);
        }

        private void menuItem_Open_Click(object sender, RoutedEventArgs e)
        {
            IAtomContainer mol = null;
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.FilterIndex = 1;
                openFileDialog.Filter = "MDL Molfile (*.mol)|*.mol|All Files (*.*)|*.*";
                bool? result = openFileDialog.ShowDialog();
                if (result == true)
                {
                    var fn = openFileDialog.FileName;
                    using (var srm = new FileStream(fn, FileMode.Open))
                    using (var reader = readerFactory.CreateReader(srm))
                    {
                        mol = reader.Read(new AtomContainer());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (mol == null)
                return;

            if (!GeometryUtil.Has2DCoordinates(mol))
                mol = Layout(sdg, mol);

            this.mol = mol;
            Render();
        }

        private void clean_structure_Click(object sender, RoutedEventArgs e)
        {
            if (mol != null)
                mol = Layout(sdg, mol);
            Render();
        }

        private void OptionsItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new PreferenceWindow(this);
            dialog.Show();
        }
    }
}
