/* Copyright (C) 2005-2006  Ideaconsult Ltd.
 *               2012       Egon Willighagen <egonw@users.sf.net>
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
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
using NCDK.IO.Formats;
using NCDK.IO.Setting;
using NCDK.Tools.Manipulator;
using System;
using System.Diagnostics;
using System.IO;
using NCDK.Numerics;
using System.Runtime.CompilerServices;

namespace NCDK.IO
{
    /**
     * Prepares input file for running MOPAC.
     * Optimization is switched on if there are no coordinates.
     *
     * @author      Nina Jeliazkova <nina@acad.bg>
     * @cdk.githash
     * @cdk.module  io
     */
    public class Mopac7Writer : DefaultChemObjectWriter
    {

        private TextWriter writer;


        private const char BLANK = ' ';
        /**
         * Creates a writer to serialize a molecule as Mopac7 input.
         *
         * @
         */
        public Mopac7Writer()
            : this(new StreamWriter(new MemoryStream()))
        { }

        /**
         * Creates a writer to serialize a molecule as Mopac7 input. Output is written to the
         * given {@link Stream}.
         *
         * @param  out {@link Stream} to which the output is written
         * @throws     Exception
         */
        public Mopac7Writer(Stream out_)
                : this(new StreamWriter(out_))
        { }

        /**
         * Creates a writer to serialize a molecule as Mopac7 input. Output is written to the
         * given {@link Writer}.
         *
         * @param  out {@link Writer} to which the output is written
         * @throws     Exception
         */
        public Mopac7Writer(TextWriter out_)
        {
            //numberFormat = NumberFormat.GetInstance(Locale.US);
            //numberFormat.SetMaximumFractionDigits(4);
            writer = out_;
            InitIOSettings();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Write(IChemObject arg0)
        {
            CustomizeJob();
            if (arg0 is IAtomContainer)
                try
                {
                    IAtomContainer container = (IAtomContainer)arg0;
                    writer.Write(mopacCommands.Setting);
                    int formalCharge = AtomContainerManipulator.GetTotalFormalCharge(container);
                    if (formalCharge != 0) writer.Write(" CHARGE=" + formalCharge);
                    writer.WriteLine();
                    //if (container.GetProperty("Names") != null) writer.Write(container.GetProperty("Names").ToString());
                    writer.WriteLine();
                    writer.Write(Title);
                    writer.WriteLine();

                    for (int i = 0; i < container.Atoms.Count; i++)
                    {
                        IAtom atom = container.Atoms[i];
                        if (atom.Point3D != null)
                        {
                            Vector3 point = atom.Point3D.Value;
                            WriteAtom(atom, point.X, point.Y, point.Z, optimize.IsSet ? 1 : 0);
                        }
                        else if (atom.Point2D != null)
                        {
                            Vector2 point = atom.Point2D.Value;
                            WriteAtom(atom, point.X, point.Y, 0, optimize.IsSet ? 1 : 0);
                        }
                        else
                            WriteAtom(atom, 0, 0, 0, 1);
                    }
                    writer.Write("0");
                    writer.WriteLine();

                }
                catch (IOException ioException)
                {
                    Trace.TraceError(ioException.Message);
                    throw new CDKException(ioException.Message, ioException);
                }
            else
                throw new CDKException("Unsupported object!\t" + arg0.GetType().Name);
        }

        private void WriteAtom(IAtom atom, double xCoord, double yCoord, double zCoord, int optimize)
        {

            writer.Write(atom.Symbol);
            writer.Write(BLANK);
            writer.Write(xCoord.ToString("F4"));
            writer.Write(BLANK);
            writer.Write(optimize);
            writer.Write(BLANK);
            writer.Write(yCoord.ToString("F4"));
            writer.Write(BLANK);
            writer.Write(optimize);
            writer.Write(BLANK);
            writer.Write(zCoord.ToString("F4"));
            writer.Write(BLANK);
            writer.Write(optimize);
            writer.Write(BLANK);
            writer.WriteLine();
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

        public override IResourceFormat Format => MOPAC7InputFormat.Instance;
        public override void SetWriter(Stream writer) => SetWriter(new StreamWriter(writer));
        public override void SetWriter(TextWriter writer)
        {
            if (this.writer != null)
            {
                try
                {
                    this.writer.Close();
                }
                catch (IOException exception)
                {
                    Trace.TraceError(exception.Message);
                }
                this.writer = null;
            }
            this.writer = writer;
        }

        private string Title => "Generated by " + GetType().Name + " at " + DateTime.Now.Ticks;

        private StringIOSetting mopacCommands;
        private BooleanIOSetting optimize;

        private void InitIOSettings()
        {
            optimize = Add(new BooleanIOSetting("Optimize", IOSetting.Importance.Medium,
                    "Should the structure be optimized?", "true"));
            mopacCommands = Add(new StringIOSetting("Commands", IOSetting.Importance.Low,
                    "What Mopac commands should be used (overwrites other choices)?",
                    "PM3 NOINTER NOMM BONDS MULLIK PRECISE"));
        }

        private void CustomizeJob()
        {
            FireIOSettingQuestion(optimize);
            try
            {
                if (optimize.IsSet)
                {
                    mopacCommands.Setting = "PM3 NOINTER NOMM BONDS MULLIK PRECISE";
                }
                else
                {
                    mopacCommands.Setting = "PM3 NOINTER NOMM BONDS MULLIK XYZ 1SCF";
                }
            }
            catch (CDKException exception)
            {
                throw new ArgumentException(exception.Message);
            }
            FireIOSettingQuestion(mopacCommands);
        }

        public override void Dispose()
        {
            Close();
        }
    }
}
