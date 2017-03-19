using NCDK.Renderers.Generators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NCDK.MolViewer
{
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class PreferenceWindow : Window
    {
        MolWindow root;

        public PreferenceWindow(MolWindow root)
        {
            InitializeComponent();

            this.root = root;
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {

            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = new Item(root);
        }

        public class Item : INotifyPropertyChanged
        {
            MolWindow root;

            public Item(MolWindow root)
            {
                this.root = root;
            }

            public bool? ShowExplicitHydrogens
            {
                get { return root.Model.GetV<bool>(typeof(BasicAtomGenerator.ShowExplicitHydrogens)); }
                set
                {
                    root.Model.SetV(typeof(BasicAtomGenerator.ShowExplicitHydrogens), value ?? false);
                    OnPropertyChanged(nameof(ShowExplicitHydrogens));
                }
            }

            public bool? CompactAtom
            {
                get { return root.Model.GetV<bool>(typeof(BasicAtomGenerator.CompactAtom)); }
                set
                {
                    root.Model.SetV(typeof(BasicAtomGenerator.CompactAtom), value ?? false);
                    OnPropertyChanged(nameof(CompactAtom));
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            private void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
