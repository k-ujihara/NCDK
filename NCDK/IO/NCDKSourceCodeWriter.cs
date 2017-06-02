/* Copyright (C) 2003-2007,2010  Egon Willighagen <egonw@users.sf.net>
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
 */
using NCDK.Numerics;
using NCDK.IO.Formats;
using NCDK.IO.Setting;
using NCDK.Tools;
using System;
using System.Diagnostics;
using System.IO;

namespace NCDK.IO
{
    /// <summary>
    /// Converts a Molecule into NCDK source code that would build the same
    /// molecule. 
    /// </summary>
    /// <example>
    /// <include file='IncludeExamples.xml' path='Comments/Codes[@id="NCDK.IO.NCDKSourceCodeWriter_Example.cs"]/*' />
    /// </example>
    // @cdk.module io
    // @cdk.githash
    // @author  Egon Willighagen <egonw@sci.kun.nl>
    // @cdk.created 2003-10-01
    // @cdk.keyword file format, CDK source code
    // @cdk.iooptions
    public class NCDKSourceCodeWriter : DefaultChemObjectWriter
    {
        private TextWriter writer;

        private BooleanIOSetting write2DCoordinates;
        private BooleanIOSetting write3DCoordinates;
        private StringIOSetting builder;

        public NCDKSourceCodeWriter(TextWriter output)
        {
            InitIOSettings();
            try
            {
                SetWriter(output);
            }
            catch (Exception)
            {
            }
        }

        public NCDKSourceCodeWriter(Stream output)
            : this(new StreamWriter(output))
        { }

        public NCDKSourceCodeWriter()
        : this(new StringWriter())
        { }

        public override IResourceFormat Format => CDKSourceCodeFormat.Instance;

        public override void SetWriter(TextWriter output)
        {
            writer = output;
        }

        public override void SetWriter(Stream output)
        {
            SetWriter(new StreamWriter(output));
        }

        /// <summary>
        /// Flushes the output and closes this object.
        /// </summary>
        public override void Close()
        {
            writer.Flush();
            writer.Close();
        }

        public override bool Accepts(Type type)
        {
            if (typeof(IAtomContainer).IsAssignableFrom(type)) return true;
            return false;
        }

        public override void Write(IChemObject obj)
        {
            CustomizeJob();
            if (obj is IAtomContainer)
            {
                try
                {
                    WriteAtomContainer((IAtomContainer)obj);
                    writer.Flush();
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                    Debug.WriteLine(ex);
                    throw new CDKException("Exception while writing to CDK source code: " + ex.Message, ex);
                }
            }
            else
            {
                throw new CDKException("Only supported is writing of IAtomContainer objects.");
            }
        }

        private void WriteAtoms(IAtomContainer molecule)
        {
            foreach (var atom in molecule.Atoms)
            {
                WriteAtom(atom);
                writer.Write("  mol.Atoms.Add(" + atom.Id + ");");
                writer.WriteLine();
            }
        }

        private void WriteBonds(IAtomContainer molecule)
        {
            foreach (var bond in molecule.Bonds)
            {
                WriteBond(bond);
                writer.Write("  mol.Bonds.Add(" + bond.Id + ");");
                writer.WriteLine();
            }
        }

        private void WriteAtomContainer(IAtomContainer molecule)
        {
            writer.Write("{");
            writer.WriteLine();
            writer.Write("  IChemObjectBuilder builder = ");
            writer.Write(builder.Setting);
            writer.Write(".Instance;");
            writer.WriteLine();
            writer.Write("  IAtomContainer mol = builder.CreateAtomContainer();");
            writer.WriteLine();
            IDCreator.CreateIDs(molecule);
            WriteAtoms(molecule);
            WriteBonds(molecule);
            writer.Write("}");
            writer.WriteLine();
        }

        private void WriteAtom(IAtom atom)
        {
            if (atom is IPseudoAtom)
            {
                writer.Write($"  IPseudoAtom {atom.Id} = builder.CreatePseudoAtom();");
                writer.WriteLine();
                writer.Write($"  atom.Label = \"{((IPseudoAtom)atom).Label}\");");
                writer.WriteLine();
            }
            else
            {
                writer.Write($"  IAtom {atom.Id} = builder.CreateAtom(\"{atom.Symbol}\");");
                writer.WriteLine();
            }
            if (atom.FormalCharge != null)
            {
                writer.Write($"  {atom.Id}.FormalCharge = {atom.FormalCharge};");
                writer.WriteLine();
            }
            if (write2DCoordinates.IsSet && atom.Point2D != null)
            {
                Vector2 p2d = atom.Point2D.Value;
                writer.Write($"  {atom.Id}.Point2D = new Vector2({p2d.X}, {p2d.Y});");
                writer.WriteLine();
            }
            if (write3DCoordinates.IsSet && atom.Point3D != null)
            {
                Vector3 p3d = atom.Point3D.Value;
                writer.Write($"  {atom.Id}.Point3D = new Vector3({p3d.X}, {p3d.Y}, {p3d.Z});");
                writer.WriteLine();
            }
        }

        private void WriteBond(IBond bond)
        {
            writer.Write($"  IBond {bond.Id} = builder.CreateBond({bond.Begin.Id}, {bond.End.Id}, BondOrder.{bond.Order});");
            writer.WriteLine();
        }

        public int SupportedDataFeatures =>
                DataFeatures.HAS_2D_COORDINATES | DataFeatures.HAS_3D_COORDINATES
                    | DataFeatures.HAS_GRAPH_REPRESENTATION | DataFeatures.HAS_ATOM_ELEMENT_SYMBOL;

        public int RequiredDataFeatures =>
                DataFeatures.HAS_GRAPH_REPRESENTATION | DataFeatures.HAS_ATOM_ELEMENT_SYMBOL;

        private void InitIOSettings()
        {
            write2DCoordinates = IOSettings.Add(new BooleanIOSetting("write2DCoordinates", IOSetting.Importance.Low,
                    "Should 2D coordinates be added?", "true"));

            write3DCoordinates = IOSettings.Add(new BooleanIOSetting("write3DCoordinates", IOSetting.Importance.Low,
                    "Should 3D coordinates be added?", "true"));

            builder = IOSettings.Add(new StringIOSetting("builder", IOSetting.Importance.Low,
                    "Which IChemObjectBuilder should be used?", "Default.ChemObjectBuilder"));
        }

        private void CustomizeJob()
        {
            FireIOSettingQuestion(write2DCoordinates);
            FireIOSettingQuestion(write3DCoordinates);
            FireIOSettingQuestion(builder);
        }

        public override void Dispose()
        {
            Close();
        }
    }
}
