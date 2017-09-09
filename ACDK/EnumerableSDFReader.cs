using NCDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace ACDK
{
    [Guid("DAF6D958-C01B-4289-9673-3BE3DBB4E2B9")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IEnumerableSDFReader
    {
        [DispId(0x2001)]
        void Open(string filename, IChemObjectBuilder builder);
        [DispId(0x2002)]
        void Close();
        [DispId(0x2003)]
        IAtomContainer Read();
    }

    [Guid("2AB919D2-C897-4111-B175-BD5517185BBE")]
    [ComDefaultInterface(typeof(IEnumerableSDFReader))]
    public sealed class EnumerableSDFReader
        : IEnumerableSDFReader
    {
        NCDK.IO.Iterator.EnumerableSDFReader obj;
        public NCDK.IO.Iterator.EnumerableSDFReader Object => obj;
        IEnumerator<NCDK.IAtomContainer> enumerator;

        public EnumerableSDFReader()
        { }

        [DispId(0x2001)]
        public void Open(string filename, IChemObjectBuilder builder)
        {
            obj = new NCDK.IO.Iterator.EnumerableSDFReader(new StreamReader(filename), ((W_IChemObjectBuilder)builder).Object);
            enumerator = obj.GetEnumerator();
        }

        [DispId(0x2002)]
        public void Close()
        {
            enumerator.Dispose();
            obj.Close();
        }

        [DispId(0x2003)]
        public IAtomContainer Read()
        {
            if (enumerator.MoveNext())
                return new W_IAtomContainer(enumerator.Current);
            return null;
        }
    }
}
