using System.Windows.Media.Imaging;

namespace NCDK.Depict
{
    public class DepictionGenerator_Example
    {
        public void Main()
        {
            {
                IChemObjectSet<IAtomContainer> mols = null;
                #region 1
                DepictionGenerator dg = new DepictionGenerator().WithSize(512, 512).WithAtomColors();
                foreach (IAtomContainer mol in mols)
                    dg.Depict(mol).WriteTo("mol.png");
                #endregion
            }
            {
                IAtomContainer mol = null;
                #region 2
                new DepictionGenerator().Depict(mol).WriteTo("mo.png");
                #endregion
            }
            {
                IAtomContainer mol = null;
                #region 3
                Depiction depiction = new DepictionGenerator().Depict(mol);
                 
                 // quick use, format determined by name by path
                 depiction.WriteTo("mol.png");
                 depiction.WriteTo("mol.svg");
                 depiction.WriteTo("mol.pdf");
                 depiction.WriteTo("mol.jpg");
                 
                 // manually specify the format
                 depiction.WriteTo(Depiction.SVG_FMT, "~/mol");

                // convert to a Java buffered image
                RenderTargetBitmap img = depiction.ToImg();
                 
                 // get the SVG XML string
                 string svg = depiction.ToSvgStr();
                #endregion
            }
        }
    }
}
