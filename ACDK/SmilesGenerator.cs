
using System.Runtime.InteropServices;

namespace ACDK
{
    public partial interface ISmilesGenerator
    {
        [DispId(0x2001)]
        ISmilesGenerator NewGenerator(SmiFlavor flavor);
    }

    public sealed partial class SmilesGenerator
        : ISmilesGenerator
    {
        [DispId(0x2001)]
        public ISmilesGenerator NewGenerator(SmiFlavor flavor)
        {
            return new SmilesGenerator(new NCDK.Smiles.SmilesGenerator((NCDK.Smiles.SmiFlavor)(int)flavor));
        }
    }
}
