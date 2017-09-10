using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ACDK
{
    [Guid("3D9B183C-89E6-4554-90C5-7242450AB23E")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IFileDialog
    {
        [DispId(0x2001)]
        string OpenFile();

        string InitialDirectory
        {
            [DispId(0x2002)]
            get;
            [DispId(0x2003)]
            set;
        }

        string Filter
        {
            [DispId(0x2004)]
            get;
            [DispId(0x2005)]
            set;
        }
    }

    [Guid("E5170650-539E-41DF-A9E4-CA55D420EF0A")]
    [ComDefaultInterface(typeof(IFileDialog))]
    public sealed class FileDialog
        : IFileDialog
    {
        [DispId(0x2001)]
        public string OpenFile()
        {
            var dialog = new OpenFileDialog();
            dialog.InitialDirectory = this.InitialDirectory;
            dialog.Filter = this.Filter;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.FileName;
            }
            return null;
        }

        public string InitialDirectory
        {
            [DispId(0x2002)]
            get;
            [DispId(0x2003)]
            set;
        }

        public string Filter
        {
            [DispId(0x2004)]
            get;
            [DispId(0x2005)]
            set;
        }
    }
}
