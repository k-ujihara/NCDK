/*
 * Copyright (C) 2017  Kazuya Ujihara <ujihara.kazuya@gmail.com>
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 * All we ask is that proper credit is given for our work, which includes
 * - but is not limited to - adding the above copyright notice to the beginning
 * of your source code files, and to any copyright notice that you may distribute
 * with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */




using System;
using System.Runtime.InteropServices;

namespace ACDK
{
    [Guid("B91F59A3-44EC-44E7-AFA4-D79C6B5FC960")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IBitSetFingerprint
    {
		bool this[int index] {[DispId(0x1001)] get;[DispId(0x1002)] set; }
		int Count {[DispId(0x1003)] get; }
		int Cardinality {[DispId(0x1004)] get; }
		[DispId(0x1005)]void And(IBitFingerprint fingerprint);
		[DispId(0x1006)]void Or(IBitFingerprint fingerprint);
    }

	[Guid("AC21006A-1797-406E-B017-7D73D9C65C39")]
	[ComDefaultInterface(typeof(IBitSetFingerprint))]
    public sealed partial class BitSetFingerprint
        : IBitSetFingerprint
			, IBitFingerprint
    {
        NCDK.Fingerprints.BitSetFingerprint obj;
        public NCDK.Fingerprints.BitSetFingerprint Object => obj;
        public BitSetFingerprint()
			 : this(new NCDK.Fingerprints.BitSetFingerprint())
		{
		}
        public BitSetFingerprint(NCDK.Fingerprints.BitSetFingerprint obj)
        {
            this.obj = obj;
        }
		public bool this[int index] {[DispId(0x1001)] get { return Object[index]; }[DispId(0x1002)] set { Object[index] = value; } }
		public int Count {[DispId(0x1003)] get { return (int)Object.Count; }}
		public int Cardinality {[DispId(0x1004)] get { return Object.Cardinality; }}
		[DispId(0x1005)]public void And(IBitFingerprint fingerprint) => Object.And(((W_IBitFingerprint)fingerprint).Object);
		[DispId(0x1006)]public void Or(IBitFingerprint fingerprint) => Object.Or(((W_IBitFingerprint)fingerprint).Object);
    }
}
namespace ACDK
{
    [Guid("B7552D7E-F1E3-4226-B20C-E379637F17EA")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface ICircularFingerprinter
    {
		[DispId(0x1001)]IBitFingerprint GetBitFingerprint(IAtomContainer container);
		int Count {[DispId(0x1002)] get; }
		[DispId(0x1003)]IDictionary_string_int GetRawFingerprint(IAtomContainer atomContainer);
    }

	[Guid("F754F9D2-8868-4C71-BFC7-983098355C99")]
	[ComDefaultInterface(typeof(ICircularFingerprinter))]
    public sealed partial class CircularFingerprinter
        : ICircularFingerprinter
			, IFingerprinter
    {
        NCDK.Fingerprints.CircularFingerprinter obj;
        public NCDK.Fingerprints.CircularFingerprinter Object => obj;
        public CircularFingerprinter()
			 : this(new NCDK.Fingerprints.CircularFingerprinter())
		{
		}
        public CircularFingerprinter(NCDK.Fingerprints.CircularFingerprinter obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public IBitFingerprint GetBitFingerprint(IAtomContainer container) => new W_IBitFingerprint(Object.GetBitFingerprint(((W_IAtomContainer)container).Object));
		public int Count {[DispId(0x1002)] get { return Object.Count; }}
		[DispId(0x1003)]public IDictionary_string_int GetRawFingerprint(IAtomContainer atomContainer) => new W_IDictionary_string_int(Object.GetRawFingerprint(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("D8FD0773-4B98-4FCF-915A-758048BC8FB6")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IEStateFingerprinter
    {
		[DispId(0x1001)]IBitFingerprint GetBitFingerprint(IAtomContainer container);
		int Count {[DispId(0x1002)] get; }
		[DispId(0x1003)]IDictionary_string_int GetRawFingerprint(IAtomContainer atomContainer);
    }

	[Guid("E109EC36-9DDA-42B2-ABA3-4DA6999519C3")]
	[ComDefaultInterface(typeof(IEStateFingerprinter))]
    public sealed partial class EStateFingerprinter
        : IEStateFingerprinter
			, IFingerprinter
    {
        NCDK.Fingerprints.EStateFingerprinter obj;
        public NCDK.Fingerprints.EStateFingerprinter Object => obj;
        public EStateFingerprinter()
			 : this(new NCDK.Fingerprints.EStateFingerprinter())
		{
		}
        public EStateFingerprinter(NCDK.Fingerprints.EStateFingerprinter obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public IBitFingerprint GetBitFingerprint(IAtomContainer container) => new W_IBitFingerprint(Object.GetBitFingerprint(((W_IAtomContainer)container).Object));
		public int Count {[DispId(0x1002)] get { return Object.Count; }}
		[DispId(0x1003)]public IDictionary_string_int GetRawFingerprint(IAtomContainer atomContainer) => new W_IDictionary_string_int(Object.GetRawFingerprint(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("C99F726F-13E6-4895-A40D-369DA5E8F598")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IExtendedFingerprinter
    {
		[DispId(0x1001)]IBitFingerprint GetBitFingerprint(IAtomContainer container);
		int Count {[DispId(0x1002)] get; }
		[DispId(0x1003)]IDictionary_string_int GetRawFingerprint(IAtomContainer atomContainer);
    }

	[Guid("192B2309-AA94-49CF-A2D5-296C18BB94E4")]
	[ComDefaultInterface(typeof(IExtendedFingerprinter))]
    public sealed partial class ExtendedFingerprinter
        : IExtendedFingerprinter
			, IFingerprinter
    {
        NCDK.Fingerprints.ExtendedFingerprinter obj;
        public NCDK.Fingerprints.ExtendedFingerprinter Object => obj;
        public ExtendedFingerprinter()
			 : this(new NCDK.Fingerprints.ExtendedFingerprinter())
		{
		}
        public ExtendedFingerprinter(NCDK.Fingerprints.ExtendedFingerprinter obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public IBitFingerprint GetBitFingerprint(IAtomContainer container) => new W_IBitFingerprint(Object.GetBitFingerprint(((W_IAtomContainer)container).Object));
		public int Count {[DispId(0x1002)] get { return Object.Count; }}
		[DispId(0x1003)]public IDictionary_string_int GetRawFingerprint(IAtomContainer atomContainer) => new W_IDictionary_string_int(Object.GetRawFingerprint(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("1CE912BE-7D39-4C6F-895E-45BA1DCE9A9E")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IGraphOnlyFingerprinter
    {
		[DispId(0x1001)]IBitFingerprint GetBitFingerprint(IAtomContainer container);
		int Count {[DispId(0x1002)] get; }
		[DispId(0x1003)]IDictionary_string_int GetRawFingerprint(IAtomContainer atomContainer);
    }

	[Guid("33AFCCA5-86BF-4BB8-95D2-F68B54648CC2")]
	[ComDefaultInterface(typeof(IGraphOnlyFingerprinter))]
    public sealed partial class GraphOnlyFingerprinter
        : IGraphOnlyFingerprinter
			, IFingerprinter
    {
        NCDK.Fingerprints.GraphOnlyFingerprinter obj;
        public NCDK.Fingerprints.GraphOnlyFingerprinter Object => obj;
        public GraphOnlyFingerprinter()
			 : this(new NCDK.Fingerprints.GraphOnlyFingerprinter())
		{
		}
        public GraphOnlyFingerprinter(NCDK.Fingerprints.GraphOnlyFingerprinter obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public IBitFingerprint GetBitFingerprint(IAtomContainer container) => new W_IBitFingerprint(Object.GetBitFingerprint(((W_IAtomContainer)container).Object));
		public int Count {[DispId(0x1002)] get { return Object.Count; }}
		[DispId(0x1003)]public IDictionary_string_int GetRawFingerprint(IAtomContainer atomContainer) => new W_IDictionary_string_int(Object.GetRawFingerprint(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("70D1DB1F-9F58-42BE-AF95-A2BDD8A148D9")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IHybridizationFingerprinter
    {
		[DispId(0x1001)]IBitFingerprint GetBitFingerprint(IAtomContainer container);
		int Count {[DispId(0x1002)] get; }
		[DispId(0x1003)]IDictionary_string_int GetRawFingerprint(IAtomContainer atomContainer);
    }

	[Guid("CCEB9C66-E442-4177-86A3-0672802F8926")]
	[ComDefaultInterface(typeof(IHybridizationFingerprinter))]
    public sealed partial class HybridizationFingerprinter
        : IHybridizationFingerprinter
			, IFingerprinter
    {
        NCDK.Fingerprints.HybridizationFingerprinter obj;
        public NCDK.Fingerprints.HybridizationFingerprinter Object => obj;
        public HybridizationFingerprinter()
			 : this(new NCDK.Fingerprints.HybridizationFingerprinter())
		{
		}
        public HybridizationFingerprinter(NCDK.Fingerprints.HybridizationFingerprinter obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public IBitFingerprint GetBitFingerprint(IAtomContainer container) => new W_IBitFingerprint(Object.GetBitFingerprint(((W_IAtomContainer)container).Object));
		public int Count {[DispId(0x1002)] get { return Object.Count; }}
		[DispId(0x1003)]public IDictionary_string_int GetRawFingerprint(IAtomContainer atomContainer) => new W_IDictionary_string_int(Object.GetRawFingerprint(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("0A09F24F-5E55-4970-AB78-857CAE0993D2")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IBitFingerprint
    {
		bool this[int index] {[DispId(0x1001)] get;[DispId(0x1002)] set; }
		int Count {[DispId(0x1003)] get; }
		int Cardinality {[DispId(0x1004)] get; }
		[DispId(0x1005)]void And(IBitFingerprint fingerprint);
		[DispId(0x1006)]void Or(IBitFingerprint fingerprint);
    }

	[ComVisible(false)]
    public sealed partial class W_IBitFingerprint
        : IBitFingerprint
    {
        NCDK.Fingerprints.IBitFingerprint obj;
        public NCDK.Fingerprints.IBitFingerprint Object => obj;
        public W_IBitFingerprint(NCDK.Fingerprints.IBitFingerprint obj)
        {
            this.obj = obj;
        }
		public bool this[int index] { get { return Object[index]; } set { Object[index] = value; } }
		public int Count { get { return (int)Object.Count; }}
		public int Cardinality { get { return Object.Cardinality; }}
		public void And(IBitFingerprint fingerprint) => Object.And(((W_IBitFingerprint)fingerprint).Object);
		public void Or(IBitFingerprint fingerprint) => Object.Or(((W_IBitFingerprint)fingerprint).Object);
    }
}
namespace ACDK
{
    [Guid("8408E871-27AD-4EEE-8AE8-0F4F60B5709F")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IFingerprinter
    {
		[DispId(0x1001)]IBitFingerprint GetBitFingerprint(IAtomContainer container);
		int Count {[DispId(0x1002)] get; }
		[DispId(0x1003)]IDictionary_string_int GetRawFingerprint(IAtomContainer atomContainer);
    }

	[ComVisible(false)]
    public sealed partial class W_IFingerprinter
        : IFingerprinter
    {
        NCDK.Fingerprints.IFingerprinter obj;
        public NCDK.Fingerprints.IFingerprinter Object => obj;
        public W_IFingerprinter(NCDK.Fingerprints.IFingerprinter obj)
        {
            this.obj = obj;
        }
		public IBitFingerprint GetBitFingerprint(IAtomContainer container) => new W_IBitFingerprint(Object.GetBitFingerprint(((W_IAtomContainer)container).Object));
		public int Count { get { return Object.Count; }}
		public IDictionary_string_int GetRawFingerprint(IAtomContainer atomContainer) => new W_IDictionary_string_int(Object.GetRawFingerprint(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("A51681B6-C94A-4CF5-805A-A03FD9DCB14A")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IIntArrayFingerprint
    {
		bool this[int index] {[DispId(0x1001)] get;[DispId(0x1002)] set; }
		int Count {[DispId(0x1003)] get; }
		int Cardinality {[DispId(0x1004)] get; }
		[DispId(0x1005)]void And(IBitFingerprint fingerprint);
		[DispId(0x1006)]void Or(IBitFingerprint fingerprint);
    }

	[Guid("FF43AADE-239E-4088-AA86-25C6EEFED330")]
	[ComDefaultInterface(typeof(IIntArrayFingerprint))]
    public sealed partial class IntArrayFingerprint
        : IIntArrayFingerprint
			, IBitFingerprint
    {
        NCDK.Fingerprints.IntArrayFingerprint obj;
        public NCDK.Fingerprints.IntArrayFingerprint Object => obj;
        public IntArrayFingerprint()
			 : this(new NCDK.Fingerprints.IntArrayFingerprint())
		{
		}
        public IntArrayFingerprint(NCDK.Fingerprints.IntArrayFingerprint obj)
        {
            this.obj = obj;
        }
		public bool this[int index] {[DispId(0x1001)] get { return Object[index]; }[DispId(0x1002)] set { Object[index] = value; } }
		public int Count {[DispId(0x1003)] get { return (int)Object.Count; }}
		public int Cardinality {[DispId(0x1004)] get { return Object.Cardinality; }}
		[DispId(0x1005)]public void And(IBitFingerprint fingerprint) => Object.And(((W_IBitFingerprint)fingerprint).Object);
		[DispId(0x1006)]public void Or(IBitFingerprint fingerprint) => Object.Or(((W_IBitFingerprint)fingerprint).Object);
    }
}
namespace ACDK
{
    [Guid("DB6B34CA-5B84-4AA9-86AF-96D2CEEF4010")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IKlekotaRothFingerprinter
    {
		[DispId(0x1001)]IBitFingerprint GetBitFingerprint(IAtomContainer container);
		int Count {[DispId(0x1002)] get; }
		[DispId(0x1003)]IDictionary_string_int GetRawFingerprint(IAtomContainer atomContainer);
    }

	[Guid("4085774B-E8A7-48FF-83B8-9C39ACC1F2F7")]
	[ComDefaultInterface(typeof(IKlekotaRothFingerprinter))]
    public sealed partial class KlekotaRothFingerprinter
        : IKlekotaRothFingerprinter
			, IFingerprinter
    {
        NCDK.Fingerprints.KlekotaRothFingerprinter obj;
        public NCDK.Fingerprints.KlekotaRothFingerprinter Object => obj;
        public KlekotaRothFingerprinter()
			 : this(new NCDK.Fingerprints.KlekotaRothFingerprinter())
		{
		}
        public KlekotaRothFingerprinter(NCDK.Fingerprints.KlekotaRothFingerprinter obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public IBitFingerprint GetBitFingerprint(IAtomContainer container) => new W_IBitFingerprint(Object.GetBitFingerprint(((W_IAtomContainer)container).Object));
		public int Count {[DispId(0x1002)] get { return Object.Count; }}
		[DispId(0x1003)]public IDictionary_string_int GetRawFingerprint(IAtomContainer atomContainer) => new W_IDictionary_string_int(Object.GetRawFingerprint(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("59C47EE7-8849-4275-A45F-5B1A20BE0C30")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface ILingoFingerprinter
    {
		[DispId(0x1001)]IBitFingerprint GetBitFingerprint(IAtomContainer container);
		int Count {[DispId(0x1002)] get; }
		[DispId(0x1003)]IDictionary_string_int GetRawFingerprint(IAtomContainer atomContainer);
    }

	[Guid("EF8E295B-81C6-446C-AC89-8CAEB53C199E")]
	[ComDefaultInterface(typeof(ILingoFingerprinter))]
    public sealed partial class LingoFingerprinter
        : ILingoFingerprinter
			, IFingerprinter
    {
        NCDK.Fingerprints.LingoFingerprinter obj;
        public NCDK.Fingerprints.LingoFingerprinter Object => obj;
        public LingoFingerprinter()
			 : this(new NCDK.Fingerprints.LingoFingerprinter())
		{
		}
        public LingoFingerprinter(NCDK.Fingerprints.LingoFingerprinter obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public IBitFingerprint GetBitFingerprint(IAtomContainer container) => new W_IBitFingerprint(Object.GetBitFingerprint(((W_IAtomContainer)container).Object));
		public int Count {[DispId(0x1002)] get { return Object.Count; }}
		[DispId(0x1003)]public IDictionary_string_int GetRawFingerprint(IAtomContainer atomContainer) => new W_IDictionary_string_int(Object.GetRawFingerprint(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("5CAC1821-5333-461D-A318-5E4439CD44BE")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IMACCSFingerprinter
    {
		[DispId(0x1001)]IBitFingerprint GetBitFingerprint(IAtomContainer container);
		int Count {[DispId(0x1002)] get; }
		[DispId(0x1003)]IDictionary_string_int GetRawFingerprint(IAtomContainer atomContainer);
    }

	[Guid("41A9917B-6109-48F3-B5B5-251AEE5AFA81")]
	[ComDefaultInterface(typeof(IMACCSFingerprinter))]
    public sealed partial class MACCSFingerprinter
        : IMACCSFingerprinter
			, IFingerprinter
    {
        NCDK.Fingerprints.MACCSFingerprinter obj;
        public NCDK.Fingerprints.MACCSFingerprinter Object => obj;
        public MACCSFingerprinter()
			 : this(new NCDK.Fingerprints.MACCSFingerprinter())
		{
		}
        public MACCSFingerprinter(NCDK.Fingerprints.MACCSFingerprinter obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public IBitFingerprint GetBitFingerprint(IAtomContainer container) => new W_IBitFingerprint(Object.GetBitFingerprint(((W_IAtomContainer)container).Object));
		public int Count {[DispId(0x1002)] get { return Object.Count; }}
		[DispId(0x1003)]public IDictionary_string_int GetRawFingerprint(IAtomContainer atomContainer) => new W_IDictionary_string_int(Object.GetRawFingerprint(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("5F88641A-6FC7-4079-B215-03863533CCF5")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IPubchemFingerprinter
    {
		[DispId(0x1001)]IBitFingerprint GetBitFingerprint(IAtomContainer container);
		int Count {[DispId(0x1002)] get; }
		[DispId(0x1003)]IDictionary_string_int GetRawFingerprint(IAtomContainer atomContainer);
    }

	[Guid("D6E12A59-B707-4E93-AEF6-9D5A605D0601")]
	[ComDefaultInterface(typeof(IPubchemFingerprinter))]
    public sealed partial class PubchemFingerprinter
        : IPubchemFingerprinter
			, IFingerprinter
    {
        NCDK.Fingerprints.PubchemFingerprinter obj;
        public NCDK.Fingerprints.PubchemFingerprinter Object => obj;
        public PubchemFingerprinter()
			 : this(new NCDK.Fingerprints.PubchemFingerprinter())
		{
		}
        public PubchemFingerprinter(NCDK.Fingerprints.PubchemFingerprinter obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public IBitFingerprint GetBitFingerprint(IAtomContainer container) => new W_IBitFingerprint(Object.GetBitFingerprint(((W_IAtomContainer)container).Object));
		public int Count {[DispId(0x1002)] get { return Object.Count; }}
		[DispId(0x1003)]public IDictionary_string_int GetRawFingerprint(IAtomContainer atomContainer) => new W_IDictionary_string_int(Object.GetRawFingerprint(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("94480768-9696-4F43-A25B-C1737760403D")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IShortestPathFingerprinter
    {
		[DispId(0x1001)]IBitFingerprint GetBitFingerprint(IAtomContainer container);
		int Count {[DispId(0x1002)] get; }
		[DispId(0x1003)]IDictionary_string_int GetRawFingerprint(IAtomContainer atomContainer);
    }

	[Guid("34534AE8-13C1-40A2-BB4D-B3350F0485D9")]
	[ComDefaultInterface(typeof(IShortestPathFingerprinter))]
    public sealed partial class ShortestPathFingerprinter
        : IShortestPathFingerprinter
			, IFingerprinter
    {
        NCDK.Fingerprints.ShortestPathFingerprinter obj;
        public NCDK.Fingerprints.ShortestPathFingerprinter Object => obj;
        public ShortestPathFingerprinter()
			 : this(new NCDK.Fingerprints.ShortestPathFingerprinter())
		{
		}
        public ShortestPathFingerprinter(NCDK.Fingerprints.ShortestPathFingerprinter obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public IBitFingerprint GetBitFingerprint(IAtomContainer container) => new W_IBitFingerprint(Object.GetBitFingerprint(((W_IAtomContainer)container).Object));
		public int Count {[DispId(0x1002)] get { return Object.Count; }}
		[DispId(0x1003)]public IDictionary_string_int GetRawFingerprint(IAtomContainer atomContainer) => new W_IDictionary_string_int(Object.GetRawFingerprint(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("5BC0FF04-0521-467B-A8E4-195E07BA723B")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface ISignatureFingerprinter
    {
		[DispId(0x1001)]IBitFingerprint GetBitFingerprint(IAtomContainer container);
		int Count {[DispId(0x1002)] get; }
		[DispId(0x1003)]IDictionary_string_int GetRawFingerprint(IAtomContainer atomContainer);
    }

	[Guid("3AE60D45-046A-4A40-A240-F74057F41CAB")]
	[ComDefaultInterface(typeof(ISignatureFingerprinter))]
    public sealed partial class SignatureFingerprinter
        : ISignatureFingerprinter
			, IFingerprinter
    {
        NCDK.Fingerprints.SignatureFingerprinter obj;
        public NCDK.Fingerprints.SignatureFingerprinter Object => obj;
        public SignatureFingerprinter()
			 : this(new NCDK.Fingerprints.SignatureFingerprinter())
		{
		}
        public SignatureFingerprinter(NCDK.Fingerprints.SignatureFingerprinter obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public IBitFingerprint GetBitFingerprint(IAtomContainer container) => new W_IBitFingerprint(Object.GetBitFingerprint(((W_IAtomContainer)container).Object));
		public int Count {[DispId(0x1002)] get { return Object.Count; }}
		[DispId(0x1003)]public IDictionary_string_int GetRawFingerprint(IAtomContainer atomContainer) => new W_IDictionary_string_int(Object.GetRawFingerprint(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("1357EE46-EC07-47EA-ACFD-D77156CF5493")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface ISubstructureFingerprinter
    {
		[DispId(0x1001)]IBitFingerprint GetBitFingerprint(IAtomContainer container);
		int Count {[DispId(0x1002)] get; }
		[DispId(0x1003)]IDictionary_string_int GetRawFingerprint(IAtomContainer atomContainer);
    }

	[Guid("CF5F546F-F800-4AE9-BD92-0194A6839D0B")]
	[ComDefaultInterface(typeof(ISubstructureFingerprinter))]
    public sealed partial class SubstructureFingerprinter
        : ISubstructureFingerprinter
			, IFingerprinter
    {
        NCDK.Fingerprints.SubstructureFingerprinter obj;
        public NCDK.Fingerprints.SubstructureFingerprinter Object => obj;
        public SubstructureFingerprinter()
			 : this(new NCDK.Fingerprints.SubstructureFingerprinter())
		{
		}
        public SubstructureFingerprinter(NCDK.Fingerprints.SubstructureFingerprinter obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public IBitFingerprint GetBitFingerprint(IAtomContainer container) => new W_IBitFingerprint(Object.GetBitFingerprint(((W_IAtomContainer)container).Object));
		public int Count {[DispId(0x1002)] get { return Object.Count; }}
		[DispId(0x1003)]public IDictionary_string_int GetRawFingerprint(IAtomContainer atomContainer) => new W_IDictionary_string_int(Object.GetRawFingerprint(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("7930EF62-EED8-43B8-B984-D91A87A6D77D")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IAtomContainer
    {
		[DispId(0x1001)]IDictionary_object_object GetProperties();
    }

	[ComVisible(false)]
    public sealed partial class W_IAtomContainer
        : IAtomContainer
			, IChemObject
    {
        NCDK.IAtomContainer obj;
        public NCDK.IAtomContainer Object => obj;
        public W_IAtomContainer(NCDK.IAtomContainer obj)
        {
            this.obj = obj;
        }
		public IDictionary_object_object GetProperties() => new W_IDictionary_object_object(Object.GetProperties());
    }
}
namespace ACDK
{
    [Guid("4DB67F68-082F-49A1-9BCE-8A0FF7848E8A")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IChemObject
    {
		[DispId(0x1001)]IDictionary_object_object GetProperties();
    }

	[ComVisible(false)]
    public sealed partial class W_IChemObject
        : IChemObject
    {
        NCDK.IChemObject obj;
        public NCDK.IChemObject Object => obj;
        public W_IChemObject(NCDK.IChemObject obj)
        {
            this.obj = obj;
        }
		public IDictionary_object_object GetProperties() => new W_IDictionary_object_object(Object.GetProperties());
    }
}
namespace ACDK
{
    [Guid("136161B3-6FC6-42B9-BAA8-DA181C5BD927")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IChemObjectBuilder
    {
    }

	[ComVisible(false)]
    public sealed partial class W_IChemObjectBuilder
        : IChemObjectBuilder
    {
        NCDK.IChemObjectBuilder obj;
        public NCDK.IChemObjectBuilder Object => obj;
        public W_IChemObjectBuilder(NCDK.IChemObjectBuilder obj)
        {
            this.obj = obj;
        }
    }
}
namespace ACDK
{
    [Guid("821CF389-3EFC-44BF-9157-7A4ED1ADA6F1")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IAcidicGroupCountDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("6B6441F5-3131-4E69-BEE3-B94C809CC163")]
	[ComDefaultInterface(typeof(IAcidicGroupCountDescriptor))]
    public sealed partial class AcidicGroupCountDescriptor
        : IAcidicGroupCountDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.AcidicGroupCountDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.AcidicGroupCountDescriptor Object => obj;
        public AcidicGroupCountDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.AcidicGroupCountDescriptor())
		{
		}
        public AcidicGroupCountDescriptor(NCDK.QSAR.Descriptors.Moleculars.AcidicGroupCountDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("49BEF8FE-0978-4A10-BEC9-5AB90C9FD0B5")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IALOGPDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("49E49CF1-52FC-4C01-9884-AC518C5F7147")]
	[ComDefaultInterface(typeof(IALOGPDescriptor))]
    public sealed partial class ALOGPDescriptor
        : IALOGPDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.ALOGPDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.ALOGPDescriptor Object => obj;
        public ALOGPDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.ALOGPDescriptor())
		{
		}
        public ALOGPDescriptor(NCDK.QSAR.Descriptors.Moleculars.ALOGPDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("12634381-787F-4FD2-BB21-CBED1BF80A92")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IAPolDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("AC1CF35F-8A96-4D7F-B1C8-519AC52457FB")]
	[ComDefaultInterface(typeof(IAPolDescriptor))]
    public sealed partial class APolDescriptor
        : IAPolDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.APolDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.APolDescriptor Object => obj;
        public APolDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.APolDescriptor())
		{
		}
        public APolDescriptor(NCDK.QSAR.Descriptors.Moleculars.APolDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("7AB2A629-EE5A-4439-951C-AA336EDCB0DE")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IAromaticAtomsCountDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("9652026F-4AA6-4720-9A93-E78ACFE0175B")]
	[ComDefaultInterface(typeof(IAromaticAtomsCountDescriptor))]
    public sealed partial class AromaticAtomsCountDescriptor
        : IAromaticAtomsCountDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.AromaticAtomsCountDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.AromaticAtomsCountDescriptor Object => obj;
        public AromaticAtomsCountDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.AromaticAtomsCountDescriptor())
		{
		}
        public AromaticAtomsCountDescriptor(NCDK.QSAR.Descriptors.Moleculars.AromaticAtomsCountDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("FCEA971F-4AE8-44E5-B81E-BB16E0A5927E")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IAromaticBondsCountDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("FC6B0455-E099-4C06-A9E9-F262EF8B0879")]
	[ComDefaultInterface(typeof(IAromaticBondsCountDescriptor))]
    public sealed partial class AromaticBondsCountDescriptor
        : IAromaticBondsCountDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.AromaticBondsCountDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.AromaticBondsCountDescriptor Object => obj;
        public AromaticBondsCountDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.AromaticBondsCountDescriptor())
		{
		}
        public AromaticBondsCountDescriptor(NCDK.QSAR.Descriptors.Moleculars.AromaticBondsCountDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("6DFFCF12-F9AB-46A1-AAAA-741FED1F63B6")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IAtomCountDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("2B07DB2A-38DF-4C72-BB29-21D8465CC970")]
	[ComDefaultInterface(typeof(IAtomCountDescriptor))]
    public sealed partial class AtomCountDescriptor
        : IAtomCountDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.AtomCountDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.AtomCountDescriptor Object => obj;
        public AtomCountDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.AtomCountDescriptor())
		{
		}
        public AtomCountDescriptor(NCDK.QSAR.Descriptors.Moleculars.AtomCountDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("33F3637C-02D6-492C-8963-D4EDC69169A7")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IAutocorrelationDescriptorCharge
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("E5A4B43D-B12B-4609-A8B5-41DAA37A4E1C")]
	[ComDefaultInterface(typeof(IAutocorrelationDescriptorCharge))]
    public sealed partial class AutocorrelationDescriptorCharge
        : IAutocorrelationDescriptorCharge
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.AutocorrelationDescriptorCharge obj;
        public NCDK.QSAR.Descriptors.Moleculars.AutocorrelationDescriptorCharge Object => obj;
        public AutocorrelationDescriptorCharge()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.AutocorrelationDescriptorCharge())
		{
		}
        public AutocorrelationDescriptorCharge(NCDK.QSAR.Descriptors.Moleculars.AutocorrelationDescriptorCharge obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("492C8B59-EF61-4BD6-BEF7-C08603628C5D")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IAutocorrelationDescriptorMass
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("6EF41B8E-5D74-4D0E-A989-2CA8406353FA")]
	[ComDefaultInterface(typeof(IAutocorrelationDescriptorMass))]
    public sealed partial class AutocorrelationDescriptorMass
        : IAutocorrelationDescriptorMass
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.AutocorrelationDescriptorMass obj;
        public NCDK.QSAR.Descriptors.Moleculars.AutocorrelationDescriptorMass Object => obj;
        public AutocorrelationDescriptorMass()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.AutocorrelationDescriptorMass())
		{
		}
        public AutocorrelationDescriptorMass(NCDK.QSAR.Descriptors.Moleculars.AutocorrelationDescriptorMass obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("091327C5-4394-4839-942B-3A5499673794")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IAutocorrelationDescriptorPolarizability
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("86DCE91F-2446-4166-8870-CB4654D77505")]
	[ComDefaultInterface(typeof(IAutocorrelationDescriptorPolarizability))]
    public sealed partial class AutocorrelationDescriptorPolarizability
        : IAutocorrelationDescriptorPolarizability
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.AutocorrelationDescriptorPolarizability obj;
        public NCDK.QSAR.Descriptors.Moleculars.AutocorrelationDescriptorPolarizability Object => obj;
        public AutocorrelationDescriptorPolarizability()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.AutocorrelationDescriptorPolarizability())
		{
		}
        public AutocorrelationDescriptorPolarizability(NCDK.QSAR.Descriptors.Moleculars.AutocorrelationDescriptorPolarizability obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("8DB695B3-6830-4B16-A494-F2E5950F5192")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IBasicGroupCountDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("9273CE85-B671-48A0-8B80-D6125708912C")]
	[ComDefaultInterface(typeof(IBasicGroupCountDescriptor))]
    public sealed partial class BasicGroupCountDescriptor
        : IBasicGroupCountDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.BasicGroupCountDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.BasicGroupCountDescriptor Object => obj;
        public BasicGroupCountDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.BasicGroupCountDescriptor())
		{
		}
        public BasicGroupCountDescriptor(NCDK.QSAR.Descriptors.Moleculars.BasicGroupCountDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("AC87E58E-98D9-43BC-8D9A-4BCFCDFC7484")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IBCUTDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("E8380587-3754-4898-9655-57DCA479BCBB")]
	[ComDefaultInterface(typeof(IBCUTDescriptor))]
    public sealed partial class BCUTDescriptor
        : IBCUTDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.BCUTDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.BCUTDescriptor Object => obj;
        public BCUTDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.BCUTDescriptor())
		{
		}
        public BCUTDescriptor(NCDK.QSAR.Descriptors.Moleculars.BCUTDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("BD33E72C-69A7-41EA-B6B0-CD118DE6F65F")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IBondCountDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("04C9A6D4-FF6D-4F22-AE0E-442009ABCB13")]
	[ComDefaultInterface(typeof(IBondCountDescriptor))]
    public sealed partial class BondCountDescriptor
        : IBondCountDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.BondCountDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.BondCountDescriptor Object => obj;
        public BondCountDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.BondCountDescriptor())
		{
		}
        public BondCountDescriptor(NCDK.QSAR.Descriptors.Moleculars.BondCountDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("9F79EDB6-2F31-49B1-BE16-C871FB485D10")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IBPolDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("BE165ECB-619B-4B99-91A0-AC511E251F3F")]
	[ComDefaultInterface(typeof(IBPolDescriptor))]
    public sealed partial class BPolDescriptor
        : IBPolDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.BPolDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.BPolDescriptor Object => obj;
        public BPolDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.BPolDescriptor())
		{
		}
        public BPolDescriptor(NCDK.QSAR.Descriptors.Moleculars.BPolDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("5390A8E0-7406-4D50-859B-62F1BEC6411D")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface ICarbonTypesDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("C915B2B6-92BD-400E-9C35-E398DDFCCE77")]
	[ComDefaultInterface(typeof(ICarbonTypesDescriptor))]
    public sealed partial class CarbonTypesDescriptor
        : ICarbonTypesDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.CarbonTypesDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.CarbonTypesDescriptor Object => obj;
        public CarbonTypesDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.CarbonTypesDescriptor())
		{
		}
        public CarbonTypesDescriptor(NCDK.QSAR.Descriptors.Moleculars.CarbonTypesDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("4C3128A4-3552-45D6-98F1-931C382B3038")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IChiChainDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("70FD43F0-1553-4112-B050-DE923286F6C8")]
	[ComDefaultInterface(typeof(IChiChainDescriptor))]
    public sealed partial class ChiChainDescriptor
        : IChiChainDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.ChiChainDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.ChiChainDescriptor Object => obj;
        public ChiChainDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.ChiChainDescriptor())
		{
		}
        public ChiChainDescriptor(NCDK.QSAR.Descriptors.Moleculars.ChiChainDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("0EA0A718-6763-4C02-90C2-F6282C1AC7AE")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IChiClusterDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("2C2414FB-94E7-4CE7-90AB-604871E2E837")]
	[ComDefaultInterface(typeof(IChiClusterDescriptor))]
    public sealed partial class ChiClusterDescriptor
        : IChiClusterDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.ChiClusterDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.ChiClusterDescriptor Object => obj;
        public ChiClusterDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.ChiClusterDescriptor())
		{
		}
        public ChiClusterDescriptor(NCDK.QSAR.Descriptors.Moleculars.ChiClusterDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("9911A60C-0ACF-41ED-95A0-2ED846E14313")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IChiPathClusterDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("C1B2123B-40BF-4F33-B2FA-8632B3209A99")]
	[ComDefaultInterface(typeof(IChiPathClusterDescriptor))]
    public sealed partial class ChiPathClusterDescriptor
        : IChiPathClusterDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.ChiPathClusterDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.ChiPathClusterDescriptor Object => obj;
        public ChiPathClusterDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.ChiPathClusterDescriptor())
		{
		}
        public ChiPathClusterDescriptor(NCDK.QSAR.Descriptors.Moleculars.ChiPathClusterDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("4524047F-BE73-4E3F-AB45-46F8C18874CD")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IChiPathDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("65C1831C-7757-424B-A3D1-8C7B38DC1D98")]
	[ComDefaultInterface(typeof(IChiPathDescriptor))]
    public sealed partial class ChiPathDescriptor
        : IChiPathDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.ChiPathDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.ChiPathDescriptor Object => obj;
        public ChiPathDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.ChiPathDescriptor())
		{
		}
        public ChiPathDescriptor(NCDK.QSAR.Descriptors.Moleculars.ChiPathDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("92696DAA-9E7A-4521-8771-515F32D14553")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface ICPSADescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("4E30FAE8-D98A-47C6-B9B0-B8E0E9D468A1")]
	[ComDefaultInterface(typeof(ICPSADescriptor))]
    public sealed partial class CPSADescriptor
        : ICPSADescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.CPSADescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.CPSADescriptor Object => obj;
        public CPSADescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.CPSADescriptor())
		{
		}
        public CPSADescriptor(NCDK.QSAR.Descriptors.Moleculars.CPSADescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("AEE5E5B6-0515-4F6D-A053-40E300C518BC")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IEccentricConnectivityIndexDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("DB4E6375-4289-4C11-AC6D-7E63F971565F")]
	[ComDefaultInterface(typeof(IEccentricConnectivityIndexDescriptor))]
    public sealed partial class EccentricConnectivityIndexDescriptor
        : IEccentricConnectivityIndexDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.EccentricConnectivityIndexDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.EccentricConnectivityIndexDescriptor Object => obj;
        public EccentricConnectivityIndexDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.EccentricConnectivityIndexDescriptor())
		{
		}
        public EccentricConnectivityIndexDescriptor(NCDK.QSAR.Descriptors.Moleculars.EccentricConnectivityIndexDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("E48A6380-4B81-4179-818A-D5C493DD00DB")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IFMFDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("126CD5CB-545F-4499-AADC-17104894B7C0")]
	[ComDefaultInterface(typeof(IFMFDescriptor))]
    public sealed partial class FMFDescriptor
        : IFMFDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.FMFDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.FMFDescriptor Object => obj;
        public FMFDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.FMFDescriptor())
		{
		}
        public FMFDescriptor(NCDK.QSAR.Descriptors.Moleculars.FMFDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("23C2FE91-94A6-45F7-B035-C4B68D2B8B75")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IFractionalPSADescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("B1300125-9DCA-49CF-B506-5A7374F6C138")]
	[ComDefaultInterface(typeof(IFractionalPSADescriptor))]
    public sealed partial class FractionalPSADescriptor
        : IFractionalPSADescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.FractionalPSADescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.FractionalPSADescriptor Object => obj;
        public FractionalPSADescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.FractionalPSADescriptor())
		{
		}
        public FractionalPSADescriptor(NCDK.QSAR.Descriptors.Moleculars.FractionalPSADescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("43FE36CE-A225-4C45-8AD0-A83E6B7CD27D")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IFragmentComplexityDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("0A455290-2FB3-4D03-9730-F786788DEF18")]
	[ComDefaultInterface(typeof(IFragmentComplexityDescriptor))]
    public sealed partial class FragmentComplexityDescriptor
        : IFragmentComplexityDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.FragmentComplexityDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.FragmentComplexityDescriptor Object => obj;
        public FragmentComplexityDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.FragmentComplexityDescriptor())
		{
		}
        public FragmentComplexityDescriptor(NCDK.QSAR.Descriptors.Moleculars.FragmentComplexityDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("ECAC5A33-DC1F-4088-8183-CC94729F0C1E")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IGravitationalIndexDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("6DEDF9F5-021A-44AD-BA61-F4900207EB76")]
	[ComDefaultInterface(typeof(IGravitationalIndexDescriptor))]
    public sealed partial class GravitationalIndexDescriptor
        : IGravitationalIndexDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.GravitationalIndexDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.GravitationalIndexDescriptor Object => obj;
        public GravitationalIndexDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.GravitationalIndexDescriptor())
		{
		}
        public GravitationalIndexDescriptor(NCDK.QSAR.Descriptors.Moleculars.GravitationalIndexDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("2BA1F3C5-6475-48B7-BEF2-B7DE728B115D")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IHBondAcceptorCountDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("855ECCC0-9FAF-41D7-8A42-94CBEE7B610A")]
	[ComDefaultInterface(typeof(IHBondAcceptorCountDescriptor))]
    public sealed partial class HBondAcceptorCountDescriptor
        : IHBondAcceptorCountDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.HBondAcceptorCountDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.HBondAcceptorCountDescriptor Object => obj;
        public HBondAcceptorCountDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.HBondAcceptorCountDescriptor())
		{
		}
        public HBondAcceptorCountDescriptor(NCDK.QSAR.Descriptors.Moleculars.HBondAcceptorCountDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("184CD217-3505-45AE-AEAB-2575733E2509")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IHBondDonorCountDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("ECCFDB25-49A7-406B-8ACA-66C2760E5DDE")]
	[ComDefaultInterface(typeof(IHBondDonorCountDescriptor))]
    public sealed partial class HBondDonorCountDescriptor
        : IHBondDonorCountDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.HBondDonorCountDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.HBondDonorCountDescriptor Object => obj;
        public HBondDonorCountDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.HBondDonorCountDescriptor())
		{
		}
        public HBondDonorCountDescriptor(NCDK.QSAR.Descriptors.Moleculars.HBondDonorCountDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("C4F5E996-C825-4DF2-9842-67132B1B48F6")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IHybridizationRatioDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("486DB6CB-3A43-45D1-8BF2-B8D3989789FA")]
	[ComDefaultInterface(typeof(IHybridizationRatioDescriptor))]
    public sealed partial class HybridizationRatioDescriptor
        : IHybridizationRatioDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.HybridizationRatioDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.HybridizationRatioDescriptor Object => obj;
        public HybridizationRatioDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.HybridizationRatioDescriptor())
		{
		}
        public HybridizationRatioDescriptor(NCDK.QSAR.Descriptors.Moleculars.HybridizationRatioDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("F6130C45-2C03-41FA-95A4-DB8B19081DB9")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IKappaShapeIndicesDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("685B1E44-D743-461E-9B84-8DD505D8B0D0")]
	[ComDefaultInterface(typeof(IKappaShapeIndicesDescriptor))]
    public sealed partial class KappaShapeIndicesDescriptor
        : IKappaShapeIndicesDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.KappaShapeIndicesDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.KappaShapeIndicesDescriptor Object => obj;
        public KappaShapeIndicesDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.KappaShapeIndicesDescriptor())
		{
		}
        public KappaShapeIndicesDescriptor(NCDK.QSAR.Descriptors.Moleculars.KappaShapeIndicesDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("B3641A1D-4FAE-4320-946C-A498C75B87A4")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IKierHallSmartsDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("6D2B0A5D-EADC-4B48-ABFB-E137F4B2F51F")]
	[ComDefaultInterface(typeof(IKierHallSmartsDescriptor))]
    public sealed partial class KierHallSmartsDescriptor
        : IKierHallSmartsDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.KierHallSmartsDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.KierHallSmartsDescriptor Object => obj;
        public KierHallSmartsDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.KierHallSmartsDescriptor())
		{
		}
        public KierHallSmartsDescriptor(NCDK.QSAR.Descriptors.Moleculars.KierHallSmartsDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("F1A8385C-4364-4E6B-A8CD-810393BF2CEA")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface ILargestChainDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("D6D6F7A3-8B72-41B3-8DB9-404065CC1437")]
	[ComDefaultInterface(typeof(ILargestChainDescriptor))]
    public sealed partial class LargestChainDescriptor
        : ILargestChainDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.LargestChainDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.LargestChainDescriptor Object => obj;
        public LargestChainDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.LargestChainDescriptor())
		{
		}
        public LargestChainDescriptor(NCDK.QSAR.Descriptors.Moleculars.LargestChainDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("0C01A7F5-6508-400A-8437-F821C451088E")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface ILargestPiSystemDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("C57F6597-BD72-4148-BCFA-D05A3753510E")]
	[ComDefaultInterface(typeof(ILargestPiSystemDescriptor))]
    public sealed partial class LargestPiSystemDescriptor
        : ILargestPiSystemDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.LargestPiSystemDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.LargestPiSystemDescriptor Object => obj;
        public LargestPiSystemDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.LargestPiSystemDescriptor())
		{
		}
        public LargestPiSystemDescriptor(NCDK.QSAR.Descriptors.Moleculars.LargestPiSystemDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("72F7F73D-BD8E-439B-97E2-6AA5EC4D14C5")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface ILengthOverBreadthDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("F612A06E-7818-4D20-96D2-63105D04082B")]
	[ComDefaultInterface(typeof(ILengthOverBreadthDescriptor))]
    public sealed partial class LengthOverBreadthDescriptor
        : ILengthOverBreadthDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.LengthOverBreadthDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.LengthOverBreadthDescriptor Object => obj;
        public LengthOverBreadthDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.LengthOverBreadthDescriptor())
		{
		}
        public LengthOverBreadthDescriptor(NCDK.QSAR.Descriptors.Moleculars.LengthOverBreadthDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("07014CB4-DDFB-4EDF-B4C5-7910C7884A55")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface ILongestAliphaticChainDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("496D4787-8E32-4DF4-9F4A-F27A22662150")]
	[ComDefaultInterface(typeof(ILongestAliphaticChainDescriptor))]
    public sealed partial class LongestAliphaticChainDescriptor
        : ILongestAliphaticChainDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.LongestAliphaticChainDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.LongestAliphaticChainDescriptor Object => obj;
        public LongestAliphaticChainDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.LongestAliphaticChainDescriptor())
		{
		}
        public LongestAliphaticChainDescriptor(NCDK.QSAR.Descriptors.Moleculars.LongestAliphaticChainDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("3B438F2D-D937-4312-A23D-FDE53A4F8B14")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IMannholdLogPDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("789D5541-AF14-42C4-9FE4-C25DEE8C6C35")]
	[ComDefaultInterface(typeof(IMannholdLogPDescriptor))]
    public sealed partial class MannholdLogPDescriptor
        : IMannholdLogPDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.MannholdLogPDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.MannholdLogPDescriptor Object => obj;
        public MannholdLogPDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.MannholdLogPDescriptor())
		{
		}
        public MannholdLogPDescriptor(NCDK.QSAR.Descriptors.Moleculars.MannholdLogPDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("543DB0EF-BCF5-4A34-9EF6-B773D0E4641E")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IMDEDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("C322BD15-0524-4752-BE85-C6690A99A976")]
	[ComDefaultInterface(typeof(IMDEDescriptor))]
    public sealed partial class MDEDescriptor
        : IMDEDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.MDEDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.MDEDescriptor Object => obj;
        public MDEDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.MDEDescriptor())
		{
		}
        public MDEDescriptor(NCDK.QSAR.Descriptors.Moleculars.MDEDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("88B0C32A-A58F-406B-A7BC-03AFD9D02307")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IMomentOfInertiaDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("0EA8CCF0-6BF1-4DAB-8240-1B5AF64AA934")]
	[ComDefaultInterface(typeof(IMomentOfInertiaDescriptor))]
    public sealed partial class MomentOfInertiaDescriptor
        : IMomentOfInertiaDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.MomentOfInertiaDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.MomentOfInertiaDescriptor Object => obj;
        public MomentOfInertiaDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.MomentOfInertiaDescriptor())
		{
		}
        public MomentOfInertiaDescriptor(NCDK.QSAR.Descriptors.Moleculars.MomentOfInertiaDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("9EB2FADF-F696-4F56-BEB1-634228E9CE60")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IPetitjeanNumberDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("D5732DE7-A677-4474-95D2-24AB6DF3E233")]
	[ComDefaultInterface(typeof(IPetitjeanNumberDescriptor))]
    public sealed partial class PetitjeanNumberDescriptor
        : IPetitjeanNumberDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.PetitjeanNumberDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.PetitjeanNumberDescriptor Object => obj;
        public PetitjeanNumberDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.PetitjeanNumberDescriptor())
		{
		}
        public PetitjeanNumberDescriptor(NCDK.QSAR.Descriptors.Moleculars.PetitjeanNumberDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("0ECD919F-A3DC-4AD8-BAB7-D0148588025A")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IPetitjeanShapeIndexDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("FA1CD861-2DF0-47AB-8C9F-2F2C424DD4F3")]
	[ComDefaultInterface(typeof(IPetitjeanShapeIndexDescriptor))]
    public sealed partial class PetitjeanShapeIndexDescriptor
        : IPetitjeanShapeIndexDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.PetitjeanShapeIndexDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.PetitjeanShapeIndexDescriptor Object => obj;
        public PetitjeanShapeIndexDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.PetitjeanShapeIndexDescriptor())
		{
		}
        public PetitjeanShapeIndexDescriptor(NCDK.QSAR.Descriptors.Moleculars.PetitjeanShapeIndexDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("A9B89A02-D6E1-4CC8-8386-BD7B842F04AA")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IRotatableBondsCountDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("A7AACBE5-E75C-475A-AF2C-94F99A3C30AD")]
	[ComDefaultInterface(typeof(IRotatableBondsCountDescriptor))]
    public sealed partial class RotatableBondsCountDescriptor
        : IRotatableBondsCountDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.RotatableBondsCountDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.RotatableBondsCountDescriptor Object => obj;
        public RotatableBondsCountDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.RotatableBondsCountDescriptor())
		{
		}
        public RotatableBondsCountDescriptor(NCDK.QSAR.Descriptors.Moleculars.RotatableBondsCountDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("0A8A7BEB-475E-482E-8A75-1EC1F8391C43")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IRuleOfFiveDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("5B964D88-16B2-46E3-B44C-2ECE3022200C")]
	[ComDefaultInterface(typeof(IRuleOfFiveDescriptor))]
    public sealed partial class RuleOfFiveDescriptor
        : IRuleOfFiveDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.RuleOfFiveDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.RuleOfFiveDescriptor Object => obj;
        public RuleOfFiveDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.RuleOfFiveDescriptor())
		{
		}
        public RuleOfFiveDescriptor(NCDK.QSAR.Descriptors.Moleculars.RuleOfFiveDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("DBB8C01C-B7EF-4D1A-BD74-47839CA5BB15")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface ISmallRingDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("E7910BC5-C6C1-4889-AD0B-9BCF7E887486")]
	[ComDefaultInterface(typeof(ISmallRingDescriptor))]
    public sealed partial class SmallRingDescriptor
        : ISmallRingDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.SmallRingDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.SmallRingDescriptor Object => obj;
        public SmallRingDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.SmallRingDescriptor())
		{
		}
        public SmallRingDescriptor(NCDK.QSAR.Descriptors.Moleculars.SmallRingDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("EA18AB9C-5C7C-4675-9006-42703D5ADCD2")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface ITPSADescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("F6FE69D4-E10E-4997-B355-45E2A9914826")]
	[ComDefaultInterface(typeof(ITPSADescriptor))]
    public sealed partial class TPSADescriptor
        : ITPSADescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.TPSADescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.TPSADescriptor Object => obj;
        public TPSADescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.TPSADescriptor())
		{
		}
        public TPSADescriptor(NCDK.QSAR.Descriptors.Moleculars.TPSADescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("DE8E4ADA-AF09-4EE7-9413-B00F5F3EAE8A")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IVABCDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("67F2C7B9-A129-4284-AECE-F483146D180E")]
	[ComDefaultInterface(typeof(IVABCDescriptor))]
    public sealed partial class VABCDescriptor
        : IVABCDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.VABCDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.VABCDescriptor Object => obj;
        public VABCDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.VABCDescriptor())
		{
		}
        public VABCDescriptor(NCDK.QSAR.Descriptors.Moleculars.VABCDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("B51E04D7-965B-49EB-8550-7ECC4AD6E3A0")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IVAdjMaDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("28EE6422-847C-4B93-AEE6-CF35951D1701")]
	[ComDefaultInterface(typeof(IVAdjMaDescriptor))]
    public sealed partial class VAdjMaDescriptor
        : IVAdjMaDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.VAdjMaDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.VAdjMaDescriptor Object => obj;
        public VAdjMaDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.VAdjMaDescriptor())
		{
		}
        public VAdjMaDescriptor(NCDK.QSAR.Descriptors.Moleculars.VAdjMaDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("A4C53E6D-794C-4DD6-AFB1-25B59E431DC6")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IWeightDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("0CFD74E4-4954-44D9-8BA0-3244A0AA31DE")]
	[ComDefaultInterface(typeof(IWeightDescriptor))]
    public sealed partial class WeightDescriptor
        : IWeightDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.WeightDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.WeightDescriptor Object => obj;
        public WeightDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.WeightDescriptor())
		{
		}
        public WeightDescriptor(NCDK.QSAR.Descriptors.Moleculars.WeightDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("3B87D319-EF78-4DA7-A28D-1DF4E90B0A02")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IWeightedPathDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("3DB2A54C-FF70-4EFE-97A5-101915B45AEB")]
	[ComDefaultInterface(typeof(IWeightedPathDescriptor))]
    public sealed partial class WeightedPathDescriptor
        : IWeightedPathDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.WeightedPathDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.WeightedPathDescriptor Object => obj;
        public WeightedPathDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.WeightedPathDescriptor())
		{
		}
        public WeightedPathDescriptor(NCDK.QSAR.Descriptors.Moleculars.WeightedPathDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("88084E7D-CB12-446A-B43B-327D48661800")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IWHIMDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("C32F991E-3E67-4B5D-B400-E0D681A1B873")]
	[ComDefaultInterface(typeof(IWHIMDescriptor))]
    public sealed partial class WHIMDescriptor
        : IWHIMDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.WHIMDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.WHIMDescriptor Object => obj;
        public WHIMDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.WHIMDescriptor())
		{
		}
        public WHIMDescriptor(NCDK.QSAR.Descriptors.Moleculars.WHIMDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("B7DAEA62-FC3B-4022-AFF4-6CFC9A0287A7")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IWienerNumbersDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("76FE3CA2-0F87-4DE7-83AD-139DEF96041F")]
	[ComDefaultInterface(typeof(IWienerNumbersDescriptor))]
    public sealed partial class WienerNumbersDescriptor
        : IWienerNumbersDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.WienerNumbersDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.WienerNumbersDescriptor Object => obj;
        public WienerNumbersDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.WienerNumbersDescriptor())
		{
		}
        public WienerNumbersDescriptor(NCDK.QSAR.Descriptors.Moleculars.WienerNumbersDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("D3D34DBE-618C-46E3-AFBD-D216028BCA4F")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IXLogPDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("A4C6C4EC-77C6-4D20-B129-7AB291641B00")]
	[ComDefaultInterface(typeof(IXLogPDescriptor))]
    public sealed partial class XLogPDescriptor
        : IXLogPDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.XLogPDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.XLogPDescriptor Object => obj;
        public XLogPDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.XLogPDescriptor())
		{
		}
        public XLogPDescriptor(NCDK.QSAR.Descriptors.Moleculars.XLogPDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("C07724E2-BD3E-4EF9-8EF9-08E55E4F8973")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IZagrebIndexDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer atomContainer);
    }

	[Guid("5E98C730-AF19-4F5B-88BC-C91C2CC29579")]
	[ComDefaultInterface(typeof(IZagrebIndexDescriptor))]
    public sealed partial class ZagrebIndexDescriptor
        : IZagrebIndexDescriptor
			, IMolecularDescriptor
    {
        NCDK.QSAR.Descriptors.Moleculars.ZagrebIndexDescriptor obj;
        public NCDK.QSAR.Descriptors.Moleculars.ZagrebIndexDescriptor Object => obj;
        public ZagrebIndexDescriptor()
			 : this(new NCDK.QSAR.Descriptors.Moleculars.ZagrebIndexDescriptor())
		{
		}
        public ZagrebIndexDescriptor(NCDK.QSAR.Descriptors.Moleculars.ZagrebIndexDescriptor obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public DescriptorValue Calculate(IAtomContainer atomContainer) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)atomContainer).Object));
    }
}
namespace ACDK
{
    [Guid("ECB38294-A140-4566-96D1-66DFB14E050B")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface DescriptorValue
    {
    }

	[ComVisible(false)]
    public sealed partial class W_DescriptorValue
        : DescriptorValue
    {
        NCDK.QSAR.DescriptorValue obj;
        public NCDK.QSAR.DescriptorValue Object => obj;
        public W_DescriptorValue(NCDK.QSAR.DescriptorValue obj)
        {
            this.obj = obj;
        }
    }
}
namespace ACDK
{
    [Guid("3FF6093F-CAFB-48F3-BAA1-69D5549CC720")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IDescriptor
    {
    }

	[ComVisible(false)]
    public sealed partial class W_IDescriptor
        : IDescriptor
    {
        NCDK.QSAR.IDescriptor obj;
        public NCDK.QSAR.IDescriptor Object => obj;
        public W_IDescriptor(NCDK.QSAR.IDescriptor obj)
        {
            this.obj = obj;
        }
    }
}
namespace ACDK
{
    [Guid("124390C0-5C97-4351-800B-D27A31130D70")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IMolecularDescriptor
    {
		[DispId(0x1001)]DescriptorValue Calculate(IAtomContainer container);
    }

	[ComVisible(false)]
    public sealed partial class W_IMolecularDescriptor
        : IMolecularDescriptor
    {
        NCDK.QSAR.IMolecularDescriptor obj;
        public NCDK.QSAR.IMolecularDescriptor Object => obj;
        public W_IMolecularDescriptor(NCDK.QSAR.IMolecularDescriptor obj)
        {
            this.obj = obj;
        }
		public DescriptorValue Calculate(IAtomContainer container) => new W_DescriptorValue(Object.Calculate(((W_IAtomContainer)container).Object));
    }
}
namespace ACDK
{
    [Guid("CDA53F1A-F685-439B-A8D5-2D18CCB5885D")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface ISmilesGenerator
    {
		[DispId(0x1001)]string Create(IAtomContainer molecule);
    }

	[Guid("B2857083-3145-41CA-8A53-14E70064F794")]
	[ComDefaultInterface(typeof(ISmilesGenerator))]
    public sealed partial class SmilesGenerator
        : ISmilesGenerator
    {
        NCDK.Smiles.SmilesGenerator obj;
        public NCDK.Smiles.SmilesGenerator Object => obj;
        public SmilesGenerator()
			 : this(new NCDK.Smiles.SmilesGenerator())
		{
		}
        public SmilesGenerator(NCDK.Smiles.SmilesGenerator obj)
        {
            this.obj = obj;
        }
		[DispId(0x1001)]public string Create(IAtomContainer molecule) => Object.Create(((W_IAtomContainer)molecule).Object);
    }
}
namespace ACDK
{
    [Guid("D4CFC8BC-C66E-49CF-B3CC-14F437406955")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface SmilesParser
    {
		[DispId(0x1001)]IAtomContainer ParseSmiles(string smiles);
    }

	[ComVisible(false)]
    public sealed partial class W_SmilesParser
        : SmilesParser
    {
        NCDK.Smiles.SmilesParser obj;
        public NCDK.Smiles.SmilesParser Object => obj;
        public W_SmilesParser(NCDK.Smiles.SmilesParser obj)
        {
            this.obj = obj;
        }
		public IAtomContainer ParseSmiles(string smiles) => new W_IAtomContainer(Object.ParseSmiles(smiles));
    }
}
