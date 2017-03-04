/* Copyright (C) 2000-2003  The Jmol Development Team
 * Copyright (C) 2003-2007  The CDK Project
 *
 * Contact: cdk-devel@lists.sf.net
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
using NCDK.Geometries;
using NCDK.IO.Formats;
using NCDK.IO.Setting;
using NCDK.Tools.Manipulator;
using System;
using System.IO;
using System.Linq;
using NCDK.Numerics;
using System.Text;

namespace NCDK.IO
{
    /// <summary>
    /// Saves small molecules in a rudimentary PDB format. It does not allow
    /// writing of PDBProtein data structures.
    ///
    // @author      Gilleain Torrance <gilleain.torrance@gmail.com>
    // @cdk.module pdb
    // @cdk.iooptions
    // @cdk.githash
    /// </summary>
    public class PDBWriter : DefaultChemObjectWriter
    {

        public static string F_SERIAL_FORMAT(int serial) => serial.ToString().PadLeft(5);
        public static string F_ATOM_NAME_FORMAT(string atomName) => atomName.PadRight(5);
        public static string F_POSITION_FORMAT(double f) => f.ToString("F3").PadLeft(12);
        public static string F_RESIDUE_FORMAT(string residue) => residue;

        private BooleanIOSetting writeAsHET;
        private BooleanIOSetting useElementSymbolAsAtomName;
        private BooleanIOSetting writeCONECTRecords;
        private BooleanIOSetting writeTERRecord;
        private BooleanIOSetting writeENDRecord;

        private TextWriter writer;

        public PDBWriter()
            : this(new StringWriter())
        { }

        /// <summary>
        /// Creates a PDB writer.
        ///
        /// <param name="out">the stream to write the PDB file to.</param>
        /// </summary>
        public PDBWriter(TextWriter out_)
        {
            writer = out_;
            writeAsHET = IOSettings.Add(new BooleanIOSetting("WriteAsHET", IOSetting.Importance.Low,
                    "Should the output file use HETATM", "false"));
            useElementSymbolAsAtomName = IOSettings.Add(new BooleanIOSetting("UseElementSymbolAsAtomName",
                    IOSetting.Importance.Low, "Should the element symbol be written as the atom name", "false"));
            writeCONECTRecords = IOSettings.Add(new BooleanIOSetting("WriteCONECT", IOSetting.Importance.Low,
                    "Should the bonds be written as CONECT records?", "true"));
            writeTERRecord = IOSettings.Add(new BooleanIOSetting("WriteTER", IOSetting.Importance.Low,
                    "Should a TER record be put at the end of the atoms?", "false"));
            writeENDRecord = IOSettings.Add(new BooleanIOSetting("WriteEND", IOSetting.Importance.Low,
                    "Should an END record be put at the end of the file?", "true"));
        }

        public PDBWriter(Stream output)
            : this(new StreamWriter(output))
        { }

        public override IResourceFormat Format => PDBFormat.Instance;

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
            if (typeof(IChemFile).IsAssignableFrom(type)) return true;
            if (typeof(ICrystal).IsAssignableFrom(type)) return true;
            if (typeof(IAtomContainer).IsAssignableFrom(type)) return true;
            return false;
        }

        public override void Write(IChemObject obj)
        {
            if (obj is ICrystal)
            {
                WriteCrystal((ICrystal)obj);
            }
            else if (obj is IAtomContainer)
            {
                WriteMolecule((IAtomContainer)obj);
            }
            else if (obj is IChemFile)
            {
                IChemFile chemFile = (IChemFile)obj;
                IChemSequence sequence = chemFile[0];
                if (sequence != null)
                {
                    IChemModel model = sequence[0];
                    if (model != null)
                    {
                        ICrystal crystal = model.Crystal;
                        if (crystal != null)
                        {
                            Write(crystal);
                        }
                        else
                        {
                            var containers = ChemModelManipulator.GetAllAtomContainers(model);
                            foreach (var container in containers)
                            {
                                WriteMolecule(model.Builder.CreateAtomContainer(container));
                            }
                        }
                    }
                }
            }
            else
            {
                throw new CDKException("Only supported is writing of Molecule, Crystal and ChemFile objects.");
            }
        }

        /// <summary>
        /// Writes a single frame in PDB format to the Writer.
        ///
        /// <param name="molecule">the Molecule to write</param>
        /// </summary>
        public void WriteMolecule(IAtomContainer molecule)
        {

            try
            {
                WriteHeader();
                int atomNumber = 1;

                string hetatmRecordName = (writeAsHET.IsSet) ? "HETATM" : "ATOM  ";
                string id = molecule.Id;
                string residueName = (id == null || id.Equals("")) ? "MOL" : id;
                string terRecordName = "TER";

                // Loop through the atoms and write them out:
                string[] connectRecords = null;
                if (writeCONECTRecords.IsSet)
                {
                    connectRecords = new string[molecule.Atoms.Count];
                }
                foreach (var atom in molecule.Atoms)
                {
                    StringBuilder buffer = new StringBuilder();
                    buffer.Append(hetatmRecordName);
                    buffer.Append(F_SERIAL_FORMAT(atomNumber));
                    buffer.Append(' ');
                    string name;
                    if (useElementSymbolAsAtomName.IsSet)
                    {
                        name = atom.Symbol;
                    }
                    else
                    {
                        if (atom.Id == null || atom.Id.Equals(""))
                        {
                            name = atom.Symbol;
                        }
                        else
                        {
                            name = atom.Id;
                        }
                    }
                    buffer.Append(F_ATOM_NAME_FORMAT(name));
                    buffer.Append(F_RESIDUE_FORMAT(residueName)).Append("     0    ");
                    Vector3 position = atom.Point3D.Value;
                    buffer.Append(F_POSITION_FORMAT(position.X));
                    buffer.Append(F_POSITION_FORMAT(position.Y));
                    buffer.Append(F_POSITION_FORMAT(position.Z));
                    buffer.Append("  1.00  0.00           ") // occupancy + temperature factor
                          .Append(atom.Symbol);
                    int? formalCharge = atom.FormalCharge;
                    if (formalCharge == null)
                    {
                        buffer.Append("+0");
                    }
                    else
                    {
                        if (formalCharge < 0)
                        {
                            buffer.Append(formalCharge);
                        }
                        else
                        {
                            buffer.Append('+').Append(formalCharge);
                        }
                    }

                    if (connectRecords != null && writeCONECTRecords.IsSet)
                    {
                        var neighbours = molecule.GetConnectedAtoms(atom);
                        if (neighbours.Any())
                        {
                            StringBuilder connectBuffer = new StringBuilder("CONECT");
                            connectBuffer.Append(F_SERIAL_FORMAT(atomNumber));
                            foreach (var neighbour in neighbours)
                            {
                                int neighbourNumber = molecule.Atoms.IndexOf(neighbour) + 1;
                                connectBuffer.Append(F_SERIAL_FORMAT(neighbourNumber));
                            }
                            connectRecords[atomNumber - 1] = connectBuffer.ToString();
                        }
                        else
                        {
                            connectRecords[atomNumber - 1] = null;
                        }
                    }

                    writer.Write(buffer.ToString(), 0, buffer.Length);
                    writer.WriteLine();
                    ++atomNumber;
                }

                if (writeTERRecord.IsSet)
                {
                    writer.Write(terRecordName, 0, terRecordName.Length);
                    writer.WriteLine();
                }

                if (connectRecords != null && writeCONECTRecords.IsSet)
                {
                    foreach (var connectRecord in connectRecords)
                    {
                        if (connectRecord != null)
                        {
                            writer.Write(connectRecord);
                            writer.WriteLine();
                        }
                    }
                }

                if (writeENDRecord.IsSet)
                {
                    writer.Write("END   ");
                    writer.WriteLine();
                }

            }
            catch (IOException exception)
            {
                throw new CDKException("Error while writing file: " + exception.Message, exception);
            }
        }

        private void WriteHeader()
        {
            writer.Write("HEADER created with the CDK (http://cdk.sf.net/)");
            writer.WriteLine();
        }

        private static string F_LENGTH_FORMAT(double length) => length.ToString("F3").PadLeft(8);   //"%4.3f";
        private static string F_ANGLE_FORMAT(double angle) => angle.ToString("F3").PadLeft(7);  //"%3.3f";

        public void WriteCrystal(ICrystal crystal)
        {
            try
            {
                WriteHeader();
                Vector3 a = crystal.A;
                Vector3 b = crystal.B;
                Vector3 c = crystal.C;
                double[] ucParams = CrystalGeometryTools.CartesianToNotional(a, b, c);
                writer.Write("CRYST1 " + F_LENGTH_FORMAT(ucParams[0]));
                writer.Write(F_LENGTH_FORMAT(ucParams[1]));
                writer.Write(F_LENGTH_FORMAT(ucParams[2]));
                writer.Write(F_ANGLE_FORMAT(ucParams[3]));
                writer.Write(F_ANGLE_FORMAT(ucParams[4]));
                writer.WriteLine();

                // before saving the atoms, we need to create cartesian coordinates
                foreach (var atom in crystal.Atoms)
                {
                    //                Debug.WriteLine("PDBWriter: atom -> " + atom);
                    // if it got 3D coordinates, use that. If not, try fractional coordinates
                    if (atom.Point3D == null && atom.FractionalPoint3D != null)
                    {
                        Vector3 frac = atom.FractionalPoint3D.Value;
                        Vector3 cart = CrystalGeometryTools.FractionalToCartesian(a, b, c, frac);
                        atom.Point3D = cart;
                    }
                }
                WriteMolecule(crystal.Builder.CreateAtomContainer(crystal));
            }
            catch (IOException exception)
            {
                throw new CDKException("Error while writing file: " + exception.Message, exception);
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
    }
}
