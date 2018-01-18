using NCDK.Depict;
using System.Windows.Media;

namespace NCDK.Controls
{
    public class ChemObjectBox : System.Windows.Controls.UserControl
    {
        private static DepictionGenerator Generator { get; } = new DepictionGenerator();
        private Depiction depiction;
        private IChemObject _ChemObject = null;

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (depiction != null)
            {
                depiction.Draw(dc);
            }
        }

        public IChemObject ChemObject
        {
            get => _ChemObject;
            set
            {
                if (_ChemObject != value)
                {
                    _ChemObject = value;

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

                    this.InvalidateVisual();
                }
            }
        }
    }
}
