using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace NCDK.MolViewer
{
    /// <summary>
    /// Interaction logic for AppearanceDialog.xaml
    /// </summary>
    public partial class AppearanceDialog : Window
    {
        public AppearanceDialog(object context)
        {
            InitializeComponent();

            DataContext = context;
        }

        private void coloring_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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
