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
using NCDK.Depict;
using NCDK.Graphs.InChI;
using NCDK.IO;
using NCDK.Layout;
using NCDK.Smiles;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NCDK.MolViewer
{
    public partial class MolWindow : Window
    {
        private static IChemObjectBuilder builder = ChemObjectBuilder.Instance;
        private static SmilesParser parser = new SmilesParser(builder);
        private static SmilesGenerator smilesGenerator = new SmilesGenerator(SmiFlavor.Default);
        private static InChIGeneratorFactory inChIGeneratorFactory = new InChIGeneratorFactory();
        private static StructureDiagramGenerator sdg = new StructureDiagramGenerator();
        private static ReaderFactory readerFactory = new ReaderFactory();

        private DepictionGenerator generator = new DepictionGenerator();
        private IChemObject _chemObject = null;

        private TextChangedEventHandler textSmiles_TextChangedEventHandler; 

        public MolWindow()
        {
            InitializeComponent();

            textSmiles_TextChangedEventHandler = new TextChangedEventHandler(this.TextSmiles_TextChanged);
            this.textSmiles.TextChanged += textSmiles_TextChangedEventHandler;

            generator.WithZoom(1.6);
        }

        private IChemObject ChemObject
        {
            get => _chemObject;
            set
            {
                _chemObject = value;
                UpdateMolecular();
            }
        }

        private void UpdateMolecular()
        {
            string smiles = null;

            try
            {
                switch (_chemObject)
                {
                    case IAtomContainer mol:
                        smiles = smilesGenerator.Create(mol);
                        break;
                    case IReaction rxn:
                        smiles = smilesGenerator.Create(rxn);
                        break;
                    default:
                        smiles = $"{_chemObject.GetType()} is not supported.";
                        break;
                }
            }
            catch (Exception)
            {
                smiles = "Failed to create SMILES";
            }

            UpdateSmilesWithoutEvent(smiles);
            Render();
        }

        private static bool IsReactionSmilees(string smiles)
        {
            return smiles.Split(' ')[0].Contains(">");
        }

        private void UpdateSmiles()
        {
            var text = this.textSmiles.Text;
            if (string.IsNullOrWhiteSpace(text))
                return;

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
                if (rxn == null)
                    return;
                this._chemObject = rxn;
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
                if (mol == null)
                    return;
                this._chemObject = mol;
            }

            Render();
        }

        public void Render()
        {
            if (ChemObject == null)
                return;

            generator.WithZoom(1.6);
            var depiction = CreateDepiction();

            if (depiction == null)
                return;
#if true
            var dv = new DrawingVisual();
            depiction.Draw(dv);
            DrawingGroup dg = dv.Drawing;
            Image image = new Image
            {
                Source = new DrawingImage(dg)
            };
            myGrid.Children.Clear();
            myGrid.Children.Add(image);
#else
            var img = depiction.ToBitmap();
            Image image = new Image
            {
                Source = img
            };
            myGrid.Children.Clear();
            myGrid.Children.Add(image);
#endif
        }

        private Depiction CreateDepiction()
        {
            Depiction depiction;
            switch (ChemObject)
            {
                case IAtomContainer mol:
                    depiction = generator.Depict(mol);
                    break;
                case IReaction rxn:
                    depiction = generator.Depict(rxn);
                    break;
                default:
                    depiction = null;
                    break;
            }

            return depiction;
        }

        private void MenuItem_Open_Click(object sender, RoutedEventArgs e)
        {
            IAtomContainer mol = null;
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    FilterIndex = 1,
                    Filter = "MDL Molfile (*.mol)|*.mol|All Files (*.*)|*.*"
                };
                bool? result = openFileDialog.ShowDialog();
                if (result == true)
                {
                    var fn = openFileDialog.FileName;
                    using (var reader = readerFactory.CreateReader(new FileStream(fn, FileMode.Open)))
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

            this.ChemObject = mol;
            Render();
        }

        private void MenuItem_SaveAs_Click(object sender, RoutedEventArgs e)
        {
            if (ChemObject == null)
                return;

            SaveFileDialog fileDialog = new SaveFileDialog
            {
                FilterIndex = 1,
                Filter = 
                    "PNG file (*.png)|*.png|" + 
                    "JPEG file (*.jpg)|*.jpg;*.jpeg|" +
                    "GIF file (*.gif)|*.gif|" +
                    "SVG file (*.svg)|*.svg|" +
                    "All Files (*.*)|*.*"
            };

            if (fileDialog.ShowDialog() != true)
                return;

            Depiction depiction = CreateDepiction();
            depiction.WriteTo(fileDialog.FileName);
        }

        private void MenuItem_PasteAsInchi_Click(object sender, RoutedEventArgs e)
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

            this.ChemObject = mol;
            Render();
        }

        private void Clean_structure_Click(object sender, RoutedEventArgs e)
        {
            if (this.ChemObject != null)
            {
                var mol = (IAtomContainer)this.ChemObject.Clone();
                sdg.Molecule = mol;
                sdg.GenerateCoordinates(mol);
                this.ChemObject = mol;
            }
        }

        private object syncUpdateSmilesWithoutEvent = new object();

        private void UpdateSmilesWithoutEvent(string newSmiles)
        {
            lock (syncUpdateSmilesWithoutEvent)
            {
                this.textSmiles.TextChanged -= textSmiles_TextChangedEventHandler;
                this.textSmiles.Text = newSmiles;
                this.textSmiles.TextChanged += textSmiles_TextChangedEventHandler;
            }
        }

        private void TextSmiles_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateSmiles();
        }
    }
}
