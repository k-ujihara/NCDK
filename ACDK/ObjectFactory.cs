using NCDK.Smiles;
using System;
using System.Runtime.InteropServices;

namespace ACDK
{
    [Guid("8FB35828-51E6-4836-BB04-CCF1B05EB6C9")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IObjectFactory
    {
        [DispId(0x1000)]
        IChemObjectBuilder DefaultChemObjectBuilder { get; }

        [DispId(0x1001)]
        IChemObjectBuilder SilentChemObjectBuilder { get; }

        [DispId(0x1002)]
        SmilesParser NewSmilesParser(IChemObjectBuilder builder);
    }

    [Guid("FB5F852E-C71D-4C2B-ABB9-A4C4DFF433D8")]
    [ComDefaultInterface(typeof(IObjectFactory))]
    public class ObjectFactory
        : IObjectFactory
    {
        public ObjectFactory()
        { }

        [DispId(0x1000)]
        public IChemObjectBuilder DefaultChemObjectBuilder { get; } = new W_IChemObjectBuilder(NCDK.Default.ChemObjectBuilder.Instance);

        [DispId(0x1001)]
        public IChemObjectBuilder SilentChemObjectBuilder { get; } = new W_IChemObjectBuilder(NCDK.Silent.ChemObjectBuilder.Instance);

        [DispId(0x1002)]
        public SmilesParser NewSmilesParser(IChemObjectBuilder builder)
            => new W_SmilesParser(new NCDK.Smiles.SmilesParser(((W_IChemObjectBuilder)builder).Object));
    }
}
