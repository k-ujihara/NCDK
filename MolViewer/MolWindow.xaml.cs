using Microsoft.Win32;
using NCDK.Default;
using NCDK.Geometries;
using NCDK.IO;
using NCDK.Layout;
using NCDK.Renderers;
using NCDK.Renderers.Fonts;
using NCDK.Renderers.Generators;
using NCDK.Renderers.Generators.Standards;
using NCDK.Renderers.Visitors;
using NCDK.Smiles;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

        ChemModelRenderer ren;

        public MolWindow()
        {
            InitializeComponent();

            sdg.BondLength = 40;
            Model = new RendererModel();
            Model.RegisterParameters(new BasicGenerator());

            molgen = new BasicGenerator();
            Model.RegisterParameters(molgen);
            Model.Listeners.Add(new MolWindowListener(this));
        }

        public static IAtomContainer Layout(StructureDiagramGenerator structureGenerator, IAtomContainer molecule)
        {
            structureGenerator.Molecule = molecule;
            try
            {
                structureGenerator.GenerateCoordinates();
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                return molecule;
            }
            return structureGenerator.Molecule;
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

        public void Render()
        {
            if (mol == null)
                return;

            var render = molgen.Generate(mol, Model);

            DrawingGroup dGroup = new DrawingGroup();
            using (DrawingContext g2d = dGroup.Open())
            {
                var v = new WPFDrawVisitor(g2d);
                v.SetTransform(new ScaleTransform(1, 1));
                v.SetFontManager(fontManager);
                v.SetRendererModel(Model);
                v.Visit(render);
            }

            Image theImage = new Image();
            DrawingImage dImageSource = new DrawingImage(dGroup);
            theImage.Source = dImageSource;
            image.Source = dImageSource;
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
                    using (var reader = new MDLV2000Reader(srm))
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
