using NCDK.Depict;
using System.Windows.Media;

namespace NCDK.Controls
{
    public class ChemObjectBox : System.Windows.Controls.Image
    {
        public DepictionGenerator Generator { get; } = new DepictionGenerator();
        private IChemObject _ChemObject = null;


        public IChemObject ChemObject
        {
            get => _ChemObject;
            set
            {
                if (_ChemObject != value)
                {
                    _ChemObject = value;

                    Depiction depiction;
                    switch (ChemObject)
                    {
                        case IAtomContainer mol:
                            depiction = Generator.Depict(mol);
                            break;
                        case IReaction rxn:
                            depiction = Generator.Depict(rxn);
                            break;
                        default:
                            depiction = null;
                            break;
                    }

                    if (depiction != null)
                    {
                        var drawingVisual = new DrawingVisual();
                        using (var g2 = drawingVisual.RenderOpen())
                        {
                            depiction.Draw(g2);
                        }
                        Source = new DrawingImage(drawingVisual.Drawing);
                    }

                    this.InvalidateVisual();
                }
            }
        }
    }
}
