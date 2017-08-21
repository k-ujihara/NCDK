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
    [Guid("7930EF62-EED8-43B8-B984-D91A87A6D77D")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface IAtomContainer
    {
    }

    [ComVisible(false)]
    public sealed partial class W_IAtomContainer
        : IAtomContainer
    {
        NCDK.IAtomContainer obj;
        public NCDK.IAtomContainer Object => obj;

        public W_IAtomContainer(NCDK.IAtomContainer obj)
        {
            this.obj = obj;
        }
    }
}
namespace ACDK
{
    [Guid("D4CFC8BC-C66E-49CF-B3CC-14F437406955")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public partial interface SmilesParser
    {
		[DispId(0x1001)]
		IAtomContainer ParseSmiles(string smiles);
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
		public IAtomContainer ParseSmiles(string smiles)
			=> new W_IAtomContainer(Object.ParseSmiles(smiles));
    }
}

