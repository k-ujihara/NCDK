/* Copyright (C) 2004-2007  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Numerics;
using NCDK.IO.Formats;
using System;
using System.IO;

namespace NCDK.IO
{
    /// <summary>
    /// Rather stupid file format used for storing crystal information.
    /// </summary>
    // @author Egon Willighagen
    // @cdk.created 2004-01-01
    ///
    // @cdk.module extra
    // @cdk.githash
    // @cdk.iooptions
    public class CrystClustWriter : DefaultChemObjectWriter
    {
        private TextWriter writer;

        /// <summary>
        /// Constructs a new CrystClustWriter class. Output will be stored in the Writer  class given as parameter.
        /// </summary>
        /// <param name="out_">Writer to redirect the output to.</param>
        public CrystClustWriter(TextWriter out_)
        {
            writer = out_;
        }

        public CrystClustWriter(Stream output)
            : this(new StreamWriter(output))
        { }

        public CrystClustWriter()
            : this(new StringWriter())
        { }

        public override IResourceFormat Format => CrystClustFormat.Instance;

        public override void SetWriter(TextWriter out_)
        {
            writer = out_;
        }

        public override void SetWriter(Stream output)
        {
            SetWriter(new StreamWriter(output));
        }

        public override bool Accepts(Type type)
        {
            if (typeof(ICrystal).IsAssignableFrom(type)) return true;
            if (typeof(IChemSequence).IsAssignableFrom(type)) return true;
            return false;
        }

        /// <summary>
        /// Serializes the IChemObject to CrystClust format and redirects it to the output Writer.
        /// </summary>
        /// <param name="obj">A Molecule of MoleculeSet object</param>
        public override void Write(IChemObject obj)
        {
            if (obj is ICrystal)
            {
                WriteCrystal((ICrystal)obj);
            }
            else if (obj is IChemSequence)
            {
                WriteChemSequence((IChemSequence)obj);
            }
            else
            {
                throw new UnsupportedChemObjectException("This object type is not supported.");
            }
        }

        /// <summary>
        /// Flushes the output and closes this object.
        /// </summary>
        public override void Close()
        {
            writer.Close();
        }

        public override void Dispose()
        {
            Close();
        }

        // Private procedures

        private void WriteChemSequence(IChemSequence cs)
        {
            int count = cs.Count;
            for (int i = 0; i < count; i++)
            {
                Writeln("frame: " + (i + 1));
                WriteCrystal(cs[i].Crystal);
            }
        }

        /// <summary>
        /// Writes a single frame to the Writer.
        /// </summary>
        /// <remarks>
        /// Format:
        ///            line      data
        ///           -------    --------------------------
        ///              1       spacegroup
        ///            2,3,4     cell parameter: a
        ///            5,6,7                     b
        ///            8,9,10                    c
        ///             11       number of atoms
        ///             12       number of asym. units
        ///            13-16     atomtype: charge, atomcoord x, y, z
        ///            17-20     idem second atom
        ///            21-24     idem third atom etc
        /// </remarks>
        /// <param name="crystal">the Crystal to serialize</param>
        private void WriteCrystal(ICrystal crystal)
        {
            string sg = crystal.SpaceGroup;
            if ("P 2_1 2_1 2_1".Equals(sg))
            {
                Writeln("P 21 21 21 (1)");
            }
            else
            {
                Writeln("P 1 (1)");
            }

            // output unit cell axes
            WriteVector3d(crystal.A);
            WriteVector3d(crystal.B);
            WriteVector3d(crystal.C);

            // output number of atoms
            int noatoms = crystal.Atoms.Count;
            Write(noatoms.ToString());
            Writeln("");

            // output number of asym. units (Z)
            if (sg.Equals("P1"))
            {
                Writeln("1");
            }
            else
            {
                // duno
                Writeln("1");
            }

            // output atoms
            for (int i = 0; i < noatoms; i++)
            {
                // output atom sumbol
                IAtom atom = crystal.Atoms[i];
                Write(atom.Symbol);
                Write(":");
                // output atom charge
                Writeln(atom.Charge.ToString());
                // output coordinates
                Writeln(atom.Point3D.Value.X.ToString());
                Writeln(atom.Point3D.Value.Y.ToString());
                Writeln(atom.Point3D.Value.Z.ToString());
            }
        }

        private void Write(string s)
        {
            try
            {
                writer.Write(s);
            }
            catch (IOException e)
            {
                Console.Error.WriteLine("CMLWriter IOException while printing \"" + s + "\":" + e.ToString());
            }
        }

        private void Writeln(string s)
        {
            try
            {
                writer.Write(s);
                writer.WriteLine();
            }
            catch (IOException e)
            {
                Console.Error.WriteLine("CMLWriter IOException while printing \"" + s + "\":" + e.ToString());
            }
        }

        private void WriteVector3d(Vector3 vector)
        {
            Write(vector.X.ToString());
            Writeln("");
            Write(vector.Y.ToString());
            Writeln("");
            Write(vector.Z.ToString());
            Writeln("");
        }
    }
}
