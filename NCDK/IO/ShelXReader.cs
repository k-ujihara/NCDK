/* Copyright (C) 2002-2007  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
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
 *
 */
using NCDK.Common.Primitives;
using NCDK.Numerics;
using NCDK.Geometries;
using NCDK.IO.Formats;
using NCDK.Maths;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace NCDK.IO
{
    /// <summary>
    /// A reader for ShelX output (RES) files. It does not read all information.
    /// The list of fields that is read: REM, END, CELL, SPGR.
    /// In additions atoms are read.
    ///
    /// <p>A reader for ShelX files. It currently supports ShelXL.
    ///
    /// <p>The ShelXL format is described on the net:
    /// <a href="http://www.msg.ucsf.edu/local/programs/shelxl/ch_07.html"
    /// http://www.msg.ucsf.edu/local/programs/shelxl/ch_07.html</a>.
    ///
    // @cdk.module io
    // @cdk.githash
    // @cdk.iooptions
    ///
    // @cdk.keyword file format, ShelXL
    // @author E.L. Willighagen
    /// </summary>
    public class ShelXReader : DefaultChemObjectReader
    {
        private TextReader input;

        /// <summary>
        /// Create an ShelX file reader.
        /// </summary>
        /// <param name="input">source of ShelX data</param>
        public ShelXReader(TextReader input)
        {
            this.input = input;
        }

        public ShelXReader(Stream input)
            : this(new StreamReader(input))
        { }

        public ShelXReader()
                : this(new StringReader(""))
        { }

        public override IResourceFormat Format => ShelXFormat.Instance;

        public override void SetReader(TextReader input)
        {
            this.input = input;
        }

        public override void SetReader(Stream input)
        {
            SetReader(new StreamReader(input));
        }

        public override bool Accepts(Type type)
        {
            if (typeof(IChemFile).IsAssignableFrom(type)) return true;
            if (typeof(ICrystal).IsAssignableFrom(type)) return true;
            return false;
        }

        /// <summary>
        /// Read a <see cref="IChemFile"/> from input.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns> the content in a <see cref="IChemFile"/> object</returns>
        public override T Read<T>(T obj)
        {
            if (obj is IChemFile)
            {
                try
                {
                    return (T)ReadChemFile((IChemFile)obj);
                }
                catch (IOException e)
                {
                    Trace.TraceError("Input/Output error while reading from input: " + e.Message);
                    throw new CDKException(e.Message, e);
                }
            }
            else if (obj is ICrystal)
            {
                try
                {
                    return (T)ReadCrystal((ICrystal)obj);
                }
                catch (IOException e)
                {
                    Trace.TraceError("Input/Output error while reading from input: " + e.Message);
                    throw new CDKException(e.Message, e);
                }
            }
            else
            {
                throw new CDKException("Only supported is reading of ChemFile.");
            }
        }

        /// <summary>
        /// Read the ShelX from input. Each ShelX document is expected to contain one crystal structure.
        /// </summary>
        /// <param name="file"></param>
        /// <returns>a ChemFile with the coordinates, charges, vectors, etc.</returns>
        private IChemFile ReadChemFile(IChemFile file)
        {
            IChemSequence seq = file.Builder.CreateChemSequence();
            IChemModel model = file.Builder.CreateChemModel();
            ICrystal crystal = ReadCrystal(file.Builder.CreateCrystal());
            model.Crystal = crystal;
            seq.Add(model);
            file.Add(seq);
            return file;
        }

        private ICrystal ReadCrystal(ICrystal crystal)
        {
            string line = input.ReadLine();
            bool end_found = false;
            while (line != null && !end_found)
            {
                /* is line continued? */
                if (line.Length > 0 && line.Substring(line.Length - 1).Equals("="))
                {
                    /* yes, line is continued */
                    line = line + input.ReadLine();
                }

                /* determine ShelX command */
                string command;
                try
                {
                    command = line.Substring(0, 4);
                }
                catch (ArgumentOutOfRangeException)
                {
                    // disregard this line
                    break;
                }

                Debug.WriteLine("command: " + command);
                var u_command = command.ToUpperInvariant();
                if (u_command.StartsWith("REM"))
                {
                    /* line is comment, disregard */

                    /* 7.1 Crystal data and general instructions */
                }
                else if (u_command.StartsWith("END"))
                {
                    end_found = true;
                }
                else if (u_command.Equals("TITL"))
                {
                }
                else if (u_command.Equals("CELL"))
                {
                    // example: CELL 1.54184 23.56421 7.13203 18.68928 90.0000
                    // 109.3799 90.0000 CELL 1.54184 7.11174 21.71704 30.95857
                    // 90.000 90.000 90.000
                    var st = Strings.Tokenize(line);
                    //st[0]; // string command_again
                    //st[1]; // string wavelength
                    string sa = st[2];
                    string sb = st[3];
                    string sc = st[4];
                    string salpha = st[5];
                    string sbeta = st[6];
                    string sgamma = st[7];
                    Debug.WriteLine("a: " + sa);
                    Debug.WriteLine("b: " + sb);
                    Debug.WriteLine("c: " + sc);
                    Debug.WriteLine("alpha: " + salpha);
                    Debug.WriteLine("beta : " + sbeta);
                    Debug.WriteLine("gamma: " + sgamma);

                    double a = FortranFormat.Atof(sa);
                    double b = FortranFormat.Atof(sb);
                    double c = FortranFormat.Atof(sc);
                    double alpha = FortranFormat.Atof(salpha);
                    double beta = FortranFormat.Atof(sbeta);
                    double gamma = FortranFormat.Atof(sgamma);

                    Vector3[] axes = CrystalGeometryTools.NotionalToCartesian(a, b, c, alpha, beta, gamma);

                    crystal.A = axes[0];
                    crystal.B = axes[1];
                    crystal.C = axes[2];
                }
                else if (u_command.Equals("ZERR"))
                {
                }
                else if (u_command.Equals("LATT"))
                {
                }
                else if (u_command.Equals("SYMM"))
                {
                }
                else if (u_command.Equals("SFAC"))
                {
                }
                else if (u_command.Equals("DISP"))
                {
                }
                else if (u_command.Equals("UNIT"))
                {
                }
                else if (u_command.Equals("LAUE"))
                {
                }
                else if (u_command.Equals("REM "))
                {
                }
                else if (u_command.Equals("MORE"))
                {
                }
                else if (u_command.Equals("TIME"))
                {
                    /* 7.2 Reflection data input */
                }
                else if (u_command.Equals("HKLF"))
                {
                }
                else if (u_command.Equals("OMIT"))
                {
                }
                else if (u_command.Equals("SHEL"))
                {
                }
                else if (u_command.Equals("BASF"))
                {
                }
                else if (u_command.Equals("TWIN"))
                {
                }
                else if (u_command.Equals("EXTI"))
                {
                }
                else if (u_command.Equals("SWAT"))
                {
                }
                else if (u_command.Equals("HOPE"))
                {
                }
                else if (u_command.Equals("MERG"))
                {
                    /* 7.3 Atom list and least-squares constraints */
                }
                else if (u_command.Equals("SPEC"))
                {
                }
                else if (u_command.Equals("RESI"))
                {
                }
                else if (u_command.Equals("MOVE"))
                {
                }
                else if (u_command.Equals("ANIS"))
                {
                }
                else if (u_command.Equals("AFIX"))
                {
                }
                else if (u_command.Equals("HFIX"))
                {
                }
                else if (u_command.Equals("FRAG"))
                {
                }
                else if (u_command.Equals("FEND"))
                {
                }
                else if (u_command.Equals("EXYZ"))
                {
                }
                else if (u_command.Equals("EXTI"))
                {
                }
                else if (u_command.Equals("EADP"))
                {
                }
                else if (u_command.Equals("EQIV"))
                {
                    /* 7.4 The connectivity list */
                }
                else if (u_command.Equals("CONN"))
                {
                }
                else if (u_command.Equals("PART"))
                {
                }
                else if (u_command.Equals("BIND"))
                {
                }
                else if (u_command.Equals("FREE"))
                {
                    /* 7.5 Least-squares restraints */
                }
                else if (u_command.Equals("DFIX"))
                {
                }
                else if (u_command.Equals("DANG"))
                {
                }
                else if (u_command.Equals("BUMP"))
                {
                }
                else if (u_command.Equals("SAME"))
                {
                }
                else if (u_command.Equals("SADI"))
                {
                }
                else if (u_command.Equals("CHIV"))
                {
                }
                else if (u_command.Equals("FLAT"))
                {
                }
                else if (u_command.Equals("DELU"))
                {
                }
                else if (u_command.Equals("SIMU"))
                {
                }
                else if (u_command.Equals("DEFS"))
                {
                }
                else if (u_command.Equals("ISOR"))
                {
                }
                else if (u_command.Equals("NCSY"))
                {
                }
                else if (u_command.Equals("SUMP"))
                {
                    /* 7.6 Least-squares organization */
                }
                else if (u_command.Equals("L.S."))
                {
                }
                else if (u_command.Equals("CGLS"))
                {
                }
                else if (u_command.Equals("BLOC"))
                {
                }
                else if (u_command.Equals("DAMP"))
                {
                }
                else if (u_command.Equals("STIR"))
                {
                }
                else if (u_command.Equals("WGHT"))
                {
                }
                else if (u_command.Equals("FVAR"))
                {
                    /* 7.7 Lists and tables */
                }
                else if (u_command.Equals("BOND"))
                {
                }
                else if (u_command.Equals("CONF"))
                {
                }
                else if (u_command.Equals("MPLA"))
                {
                }
                else if (u_command.Equals("RTAB"))
                {
                }
                else if (u_command.Equals("HTAB"))
                {
                }
                else if (u_command.Equals("LIST"))
                {
                }
                else if (u_command.Equals("ACTA"))
                {
                }
                else if (u_command.Equals("SIZE"))
                {
                }
                else if (u_command.Equals("TEMP"))
                {
                }
                else if (u_command.Equals("WPDB"))
                {
                    /* 7.8 Fouriers, peak search and lineprinter plots */
                }
                else if (u_command.Equals("FMAP"))
                {
                }
                else if (u_command.Equals("GRID"))
                {
                }
                else if (u_command.Equals("PLAN"))
                {
                }
                else if (u_command.Equals("MOLE"))
                {
                    /* NOT DOCUMENTED BUT USED BY PLATON */
                }
                else if (u_command.Equals("SPGR"))
                {
                    // Line added by PLATON stating the spacegroup
                    var st = Strings.Tokenize(line);
                    //st[0]; // string command_again
                    string spacegroup = st[1];
                    crystal.SpaceGroup = spacegroup;
                }
                else if (u_command.Equals("    "))
                {
                    Debug.WriteLine("Disrgarding line assumed to be added by PLATON: " + line);

                    /* All other is atom */
                }
                else
                {
                    //Debug.WriteLine("Assumed to contain an atom: " + line);
                    
                    // this line gives an atom, because all lines not starting with
                    // a ShelX command is an atom (that sucks!)
                    var st = Strings.Tokenize(line);
                    string atype = st[0];
                    //st[1]; // string scatt_factor
                    string sa = st[2];
                    string sb = st[3];
                    string sc = st[4];
                    // skip the rest

                    if (char.IsDigit(atype[1]))
                    {
                        // atom type has a one letter code
                        atype = atype.Substring(0, 1);
                    }
                    else
                    {
                        StringBuilder sb2 = new StringBuilder();
                        sb2.Append(atype[1]);
                        atype = atype.Substring(0, 1) + sb2.ToString().ToLowerInvariant();
                    }

                    double[] frac = new double[3];
                    frac[0] = FortranFormat.Atof(sa); // fractional coordinates
                    frac[1] = FortranFormat.Atof(sb);
                    frac[2] = FortranFormat.Atof(sc);
                    Debug.WriteLine("fa,fb,fc: " + frac[0] + ", " + frac[1] + ", " + frac[2]);

                    if (string.Equals(atype, "Q", StringComparison.OrdinalIgnoreCase))
                    {
                        // ingore atoms named Q
                    }
                    else
                    {
                        Trace.TraceInformation("Adding atom: " + atype + ", " + frac[0] + ", " + frac[1] + ", " + frac[2]);
                        IAtom atom = crystal.Builder.CreateAtom(atype);
                        atom.FractionalPoint3D = new Vector3(frac[0], frac[1], frac[2]);
                        crystal.Atoms.Add(atom);
                        Debug.WriteLine("Atom added: ", atom);
                    }
                }
                line = input.ReadLine();
            }
            return crystal;
        }

        public override void Close()
        {
            input.Close();
        }

        public override void Dispose()
        {
            Close();
        }
    }
}
