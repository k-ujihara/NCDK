/* Copyright (C) 2003-2008  Egon Willighagen <egonw@sci.kun.nl>
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
using NCDK.Numerics;
using NCDK.IO.Formats;
using NCDK.IO.Setting;
using System;
using System.Collections.Generic;
using System.IO;

namespace NCDK.IO
{
    /// <summary>
    /// File writer thats generates input files for Gaussian calculation jobs. It was tested with Gaussian98.
    /// </summary>
    // @cdk.module io
    // @cdk.githash
    // @author  Egon Willighagen <egonw@sci.kun.nl>
    // @cdk.keyword Gaussian (tm), input file
    public class GaussianInputWriter : DefaultChemObjectWriter
    {
        static TextWriter writer;

        IOSetting method;
        IOSetting basis;
        IOSetting comment;
        IOSetting command;
        IOSetting memory;
        BooleanIOSetting shell;
        IntegerIOSetting proccount;
        BooleanIOSetting usecheckpoint;

        /// <summary>
        /// Constructs a new writer that produces input files to run a Gaussian QM job.
        /// </summary>
        /// <param name="out_"></param>
        public GaussianInputWriter(TextWriter out_)
        {
            writer = out_;
            InitIOSettings();
        }

        public GaussianInputWriter(Stream output)
                : this(new StreamWriter(output))
        { }

        public GaussianInputWriter()
                : this(new StringWriter())
        { }

        public override IResourceFormat Format => GaussianInputFormat.Instance;

        public override void SetWriter(TextWriter out_)
        {
            writer = out_;
        }

        public override void SetWriter(Stream output)
        {
            SetWriter(new StreamWriter(output));
        }

        public override void Close()
        {
            writer.Close();
        }

        public override bool Accepts(Type type)
        {
            if (typeof(IAtomContainer).IsAssignableFrom(type)) return true;
            return false;
        }

        public override void Write(IChemObject obj)
        {
            if (obj is IAtomContainer)
            {
                try
                {
                    WriteMolecule((IAtomContainer)obj);
                }
                catch (Exception ex)
                {
                    throw new CDKException($"Error while writing Gaussian input file: {ex.Message}", ex);
                }
            }
            else
            {
                throw new CDKException("GaussianInputWriter only supports output of Molecule classes.");
            }
        }

        /// <summary>
        /// Writes a molecule for input for Gaussian.
        /// </summary>
        /// <param name="mol"></param>
        public void WriteMolecule(IAtomContainer mol)
        {
            CustomizeJob();

            // write extra statements
            if (proccount.GetSettingValue() > 1)
            {
                writer.Write("%nprocl=" + proccount.GetSettingValue());
                writer.WriteLine();
            }
            if (!memory.Setting.Equals("unset"))
            {
                writer.Write("%Mem=" + memory.Setting);
                writer.WriteLine();
            }
            if (usecheckpoint.IsSet)
            {
                if (mol.Id != null && mol.Id.Length > 0)
                {
                    writer.Write($"%chk={mol.Id}.chk");
                }
                else
                {
                    // force different file names
                    writer.Write($"%chk={DateTime.Now.Ticks}.chk"); // TODO: Better to use Guid?
                }
                writer.WriteLine();
            }

            // write the command line
            writer.Write("# " + method.Setting + "/" + basis.Setting + " ");
            string commandString = command.Setting;
            if (commandString.Equals("energy calculation"))
            {
                // ok, no special command needed
            }
            else if (commandString.Equals("geometry optimization"))
            {
                writer.Write("opt");
            }
            else if (commandString.Equals("IR frequency calculation"))
            {
                writer.Write("freq");
            }
            else if (commandString.Equals("IR frequency calculation (with Raman)"))
            {
                writer.Write("freq=noraman");
            }
            else
            {
                // assume that user knows what he's doing
                writer.Write(commandString);
            }
            writer.WriteLine();

            // next line is empty
            writer.WriteLine();

            // next line is comment
            writer.Write(comment.Setting);
            writer.WriteLine();

            // next line is empty
            writer.WriteLine();

            // next line contains two digits the first is the total charge the
            // second is bool indicating: 0 = open shell 1 = closed shell
            writer.Write("0 "); // FIXME: should write total charge of molecule
            if (shell.IsSet)
            {
                writer.Write("0");
            }
            else
            {
                writer.Write("1");
            }
            writer.WriteLine();

            // then come all the atoms.
            // Loop through the atoms and write them out:
            foreach (var a in mol.Atoms)
            {
                string st = a.Symbol;

                // export Eucledian coordinates (indicated by the 0)
                st = st + " 0 ";

                // export the 3D coordinates
                var p3 = a.Point3D;
                if (p3 != null)
                {
                    st = st + p3.Value.X.ToString() + " " + p3.Value.Y.ToString() + " "
                            + p3.Value.Z.ToString();
                }

                writer.Write(st, 0, st.Length);
                writer.WriteLine();
            }

            // G98 expects an empty line at the end
            writer.WriteLine();
        }

        private void InitIOSettings()
        {
            List<string> basisOptions = new List<string>();
            basisOptions.Add("6-31g");
            basisOptions.Add("6-31g*");
            basisOptions.Add("6-31g(d)");
            basisOptions.Add("6-311g");
            basisOptions.Add("6-311+g**");
            basis = new OptionIOSetting("Basis", IOSetting.Importance.Medium, "Which basis set do you want to use?",
                    basisOptions, "6-31g");

            List<string> methodOptions = new List<string>();
            methodOptions.Add("rb3lyp");
            methodOptions.Add("b3lyp");
            methodOptions.Add("rhf");
            method = new OptionIOSetting("Method", IOSetting.Importance.Medium, "Which method do you want to use?",
                    methodOptions, "b3lyp");

            List<string> commandOptions = new List<string>();
            commandOptions.Add("energy calculation");
            commandOptions.Add("geometry optimization");
            commandOptions.Add("IR frequency calculation");
            commandOptions.Add("IR frequency calculation (with Raman)");
            command = IOSettings.Add(new OptionIOSetting("Command", IOSetting.Importance.High,
                    "What kind of job do you want to perform?", commandOptions, "energy calculation"));

            comment = IOSettings.Add(new StringIOSetting("Comment", IOSetting.Importance.Low,
                    "What comment should be put in the file?", "Created with CDK (http://cdk.sf.net/)"));

            memory = IOSettings.Add(new StringIOSetting("Memory", IOSetting.Importance.Low,
                    "How much memory do you want to use?", "unset"));

            shell = IOSettings.Add(new BooleanIOSetting("OpenShell", IOSetting.Importance.Medium,
                    "Should the calculation be open shell?", "false"));

            proccount = IOSettings.Add(new IntegerIOSetting("ProcessorCount", IOSetting.Importance.Low,
                    "How many processors should be used by Gaussian?", "1"));

            usecheckpoint = new BooleanIOSetting("UseCheckPointFile", IOSetting.Importance.Low,
                    "Should a check point file be saved?", "false");
        }

        private void CustomizeJob()
        {
            foreach (var setting in IOSettings.Settings)
            {
                FireIOSettingQuestion(setting);
            }
        }

        public override void Dispose()
        {
            Close();
        }
    }
}
