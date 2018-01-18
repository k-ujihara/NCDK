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
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace NCDK.Controls
{
    public partial class MolWindow : Window
    {
        private static IChemObjectBuilder builder = ChemObjectBuilder.Instance;
        private static SmilesParser parser = new SmilesParser(builder);
        private static SmilesGenerator smilesGenerator = new SmilesGenerator(SmiFlavor.Generic);
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
                catch (Exception e)
                {
                    smiles = $"Failed to create SMILES: {e.Message}";
                }

                UpdateSmilesWithoutEvent(smiles);

                objectBox.ChemObject = _chemObject;
            }
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

            this.objectBox.ChemObject = this._chemObject;
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
            IChemObject _mol = null;
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    FilterIndex = 1,
                    Filter = 
                        "All supported files (*.mol;*.rxn;*.mol2)|*.mol;*.rxn;*.mol2|" +
                        "MDL Molfile (*.mol)|*.mol|" +
                        "MDL Rxnfile (*.rxn)|*.rxn|" +
                        "Mol2 (Sybyl) (*.mol2)|*.mol2|" +
                        "All Files (*.*)|*.*"
                };
                bool? result = openFileDialog.ShowDialog();
                if (result == true)
                {
                    var fn = openFileDialog.FileName;
                    var ex = Path.GetExtension(fn);
                    switch (ex)
                    {
                        case ".rxn":
                            using (var reader = readerFactory.CreateReader(new FileStream(fn, FileMode.Open)))
                            {
                                _mol = reader.Read(new Silent.Reaction());
                            }
                            break;
                        default:
                            using (var reader = readerFactory.CreateReader(new FileStream(fn, FileMode.Open)))
                            {
                                if (reader == null)
                                    throw new Exception("Not supported.");
                                _mol = reader.Read(new Silent.AtomContainer());
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (_mol == null)
                return;

            this.ChemObject = _mol;
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
        }

        private void Clean_structure_Click(object sender, RoutedEventArgs e)
        {
            if (this.ChemObject != null)
            {
                var clone = ChemObject.Clone();
                switch (clone)
                {
                    case IAtomContainer o:
                        sdg.GenerateCoordinates(o);
                        ChemObject = o;
                        break;
                    case IReaction o:
                        sdg.GenerateCoordinates(o);
                        ChemObject = o;
                        break;
                    default:
                        Trace.TraceWarning($"'{clone.GetType().ToString()}' is not supported.");
                        break;
                }
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
