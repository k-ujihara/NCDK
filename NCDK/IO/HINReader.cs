/* Copyright (C) 2004-2007  Rajarshi Guha <rajarshi.guha@gmail.com>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 *  This library is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public
 *  License as published by the Free Software Foundation; either
 *  version 2.1 of the License, or (at your option) any later version.
 *
 *  This library is distributed in the hope that it will be useful,
 *  but WITHOUT Any WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 *  Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public
 *  License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using NCDK.IO.Formats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NCDK.Numerics;
using NCDK.Common.Primitives;

namespace NCDK.IO
{
    /// <summary>
    /// Reads an object from HIN formated input.
    /// </summary>
    // @cdk.module io
    // @cdk.githash
    // @author  Rajarshi Guha <rajarshi.guha@gmail.com>
    // @cdk.created 2004-01-27
    // @cdk.keyword file format, HIN
    // @cdk.iooptions
    public class HINReader : DefaultChemObjectReader
    {
        private TextReader input;

        /// <summary>
        /// Construct a new reader from a Reader type object
        /// </summary>
        /// <param name="input">reader from which input is read</param>
        public HINReader(TextReader input)
        {
            this.input = input;
        }

        public HINReader(Stream input)
            : this(new StreamReader(input))
        {
        }

        public override IResourceFormat Format => HINFormat.Instance;

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    input.Dispose();
                }

                input = null;

                disposedValue = true;
                base.Dispose(disposing);
            }
        }
        #endregion

        public override bool Accepts(Type type)
        {
            if (typeof(IChemFile).IsAssignableFrom(type)) return true;
            return false;
        }

        /// <summary>
        /// Reads the content from a HIN input. It can only return a
        /// IChemObject of type ChemFile
        /// </summary>
        /// <param name="obj">class must be of type ChemFile</param>
        /// <seealso cref="IChemFile"/>
        public override T Read<T>(T obj)
        {
            if (obj is IChemFile)
            {
                return (T)ReadChemFile((IChemFile)obj);
            }
            else
            {
                throw new CDKException("Only supported is reading of ChemFile objects.");
            }
        }

        private string GetMolName(string line)
        {
            if (line == null) return ("");
            var toks = line.Split(' ');
            if (toks.Length == 3)
                return (toks[2]);
            else
                return ("");
        }

        /// <summary>
        ///  Private method that actually parses the input to read a ChemFile
        ///  object. In its current state it is able to read all the molecules
        ///  (if more than one is present) in the specified HIN file. These are
        ///  placed in a MoleculeSet object which in turn is placed in a ChemModel
        ///  which in turn is placed in a ChemSequence object and which is finally
        ///  placed in a ChemFile object and returned to the user.
        /// </summary>
        /// <returns>A ChemFile containing the data parsed from input.</returns>
        private IChemFile ReadChemFile(IChemFile file)
        {
            IChemSequence chemSequence = file.Builder.NewChemSequence();
            IChemModel chemModel = file.Builder.NewChemModel();
            IChemObjectSet<IAtomContainer> setOfMolecules = file.Builder.NewAtomContainerSet();
            string info;

            IList<string> aroringText = new List<string>();
            IList<IAtomContainer> mols = new List<IAtomContainer>();

            try
            {
                string line;

                // read in header info
                while (true)
                {
                    line = input.ReadLine();
                    if (line.StartsWith("mol", StringComparison.Ordinal))
                    {
                        info = GetMolName(line);
                        break;
                    }
                }

                // start the actual molecule data - may be multiple molecule
                line = input.ReadLine();
                while (true)
                {
                    if (line == null) break; // end of file
                    if (line.StartsWithChar(';')) continue; // comment line

                    if (line.StartsWith("mol", StringComparison.Ordinal))
                    {
                        info = GetMolName(line);
                        line = input.ReadLine();
                    }
                    IAtomContainer m = file.Builder.NewAtomContainer();
                    m.Title = info;

                    // Each element of cons is an List of length 3 which stores
                    // the start and end indices and bond order of each bond
                    // found in the HIN file. Before adding bonds we need to reduce
                    // the number of bonds so as not to count the same bond twice
                    List<List<Object>> cons = new List<List<Object>>();

                    // read data for current molecule
                    int atomSerial = 0;
                    while (true)
                    {
                        if (line == null || line.Contains("endmol"))
                        {
                            break;
                        }
                        if (line.StartsWithChar(';')) continue; // comment line

                        string[] toks = line.Split(' ');

                        string sym = toks[3];
                        double charge = double.Parse(toks[6]);
                        double x = double.Parse(toks[7]);
                        double y = double.Parse(toks[8]);
                        double z = double.Parse(toks[9]);
                        int nbond = int.Parse(toks[10]);

                        IAtom atom = file.Builder.NewAtom(sym, new Vector3(x, y, z));
                        atom.Charge = charge;

                        BondOrder bo = BondOrder.Single;

                        for (int j = 11; j < (11 + nbond * 2); j += 2)
                        {
                            int s = int.Parse(toks[j]) - 1; // since atoms start from 1 in the file
                            char bt = toks[j + 1][0];
                            switch (bt)
                            {
                                case 's':
                                    bo = BondOrder.Single;
                                    break;
                                case 'd':
                                    bo = BondOrder.Double;
                                    break;
                                case 't':
                                    bo = BondOrder.Triple;
                                    break;
                                case 'a':
                                    bo = BondOrder.Quadruple;
                                    break;
                            }
                            List<object> ar = new List<object>(3)
                            {
                                atomSerial,
                                s,
                                bo
                            };
                            cons.Add(ar);
                        }
                        m.Atoms.Add(atom);
                        atomSerial++;
                        line = input.ReadLine();
                    }

                    // now just store all the bonds we have
                    foreach (var ar in cons)
                    {
                        IAtom s = m.Atoms[(int)ar[0]];
                        IAtom e = m.Atoms[(int)ar[1]];
                        BondOrder bo = (BondOrder)ar[2];
                        if (!IsConnected(m, s, e)) m.Bonds.Add(file.Builder.NewBond(s, e, bo));
                    }
                    mols.Add(m);

                    // we may not get a 'mol N' immediately since
                    // the aromaticring keyword might be present
                    // and doesn't seem to be located within the molecule
                    // block. However, if we do see this keyword we save this
                    // since it can contain aromatic specs for any molecule
                    // listed in the file
                    //
                    // The docs do not explicitly state the the keyword comes
                    // after *all* molecules. So we save and then reprocess
                    // all the molecules in a second pass
                    while (true)
                    {
                        line = input.ReadLine();
                        if (line == null || line.StartsWith("mol", StringComparison.Ordinal))
                            break;
                        if (line.StartsWith("aromaticring", StringComparison.Ordinal))
                            aroringText.Add(line.Trim());
                    }
                }

            }
            catch (IOException)
            {
                // FIXME: should make some noise now
                file = null;
            }

            if (aroringText.Count > 0)
            { // process aromaticring annotations
                foreach (var line in aroringText)
                {
                    string[] toks = line.Split(' ');
                    int natom = int.Parse(toks[1]);
                    int n = 0;
                    for (int i = 2; i < toks.Length; i += 2)
                    {
                        int molnum = int.Parse(toks[i]); // starts from 1
                        int atnum = int.Parse(toks[i + 1]); // starts from 1
                        mols[molnum - 1].Atoms[atnum - 1].IsAromatic = true;
                        n++;
                    }
                    Trace.Assert(n == natom);
                }
            }

            foreach (var mol in mols)
                setOfMolecules.Add(mol);
            chemModel.MoleculeSet = setOfMolecules;
            chemSequence.Add(chemModel);
            file.Add(chemSequence);

            return file;
        }

        private bool IsConnected(IAtomContainer atomContainer, IAtom atom1, IAtom atom2)
        {
            foreach (var bond in atomContainer.Bonds)
            {
                if (bond.Contains(atom1) && bond.Contains(atom2)) return true;
            }
            return false;
        }
    }
}
