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
using NCDK.IO;
using NCDK.Layout;
using NCDK.Smiles;
using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace NCDK.MolViewer
{
    public partial class MolWindow : Window
    {
        private static IChemObjectBuilder builder = ChemObjectBuilder.Instance;
        private static SmilesParser parser = new SmilesParser(builder);
        private static DepictionGenerator generator = new DepictionGenerator();
        private static StructureDiagramGenerator sdg = new StructureDiagramGenerator();
        private static ReaderFactory readerFactory = new ReaderFactory();

        private IAtomContainer mol = null;

        class MolWindowListener 
            : Events.ICDKChangeListener
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

            this.mol = mol;
            Render();
        }

        public void Render()
        {
            if (mol == null)
                return;

            DepictionGenerator myGenerator = generator.WithZoom(1.6);
            Depiction depiction = myGenerator.Depict(mol);
            var img = depiction.ToBitmap();
            image.Source = img;
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

            this.mol = mol;
            Render();
        }

        private void MenuItem_SaveAs_Click(object sender, RoutedEventArgs e)
        {
            if (mol == null)
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

            DepictionGenerator myGenerator = generator.WithZoom(1.6);
            Depiction depiction = myGenerator.Depict(mol);
            depiction.WriteTo(fileDialog.FileName);
        }

        private void Clean_structure_Click(object sender, RoutedEventArgs e)
        {
            if (this.mol != null)
            {
                var mol = (IAtomContainer)this.mol.Clone();
                sdg.Molecule = mol;
                sdg.GenerateCoordinates(mol);
                this.mol = mol;
                Render();
            }
        }
    }
}
