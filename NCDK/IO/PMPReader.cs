/* Copyright (C) 2004-2008  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.Graphs.Rebond;
using NCDK.IO.Formats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace NCDK.IO
{
    /**
     * Reads an frames from a PMP formated input.
     * Both compilation and use of this class requires Java 1.4.
     *
     * @cdk.module  io
     * @cdk.githash
     * @cdk.iooptions
     *
     * @cdk.keyword file format, Polymorph Predictor (tm)
     *
     * @author E.L. Willighagen
     * @cdk.require java1.4+
     */
    public class PMPReader : DefaultChemObjectReader
    {
        private const string PMP_ZORDER = "ZOrder";
        private const string PMP_ID = "Id";

        private TextReader input;

        /* Keep a copy of the PMP model */
        private IAtomContainer modelStructure;
        private IChemObject chemObject;
        /* Keep an index of PMP id -> AtomCountainer id */
        private IDictionary<int, int> atomids = new Dictionary<int, int>();
        private IDictionary<int, int> atomGivenIds = new Dictionary<int, int>();
        private IDictionary<int, int> atomZOrders = new Dictionary<int, int>();
        private IDictionary<int, int> bondids = new Dictionary<int, int>();
        private IDictionary<int, int> bondAtomOnes = new Dictionary<int, int>();
        private IDictionary<int, int> bondAtomTwos = new Dictionary<int, int>();
        private IDictionary<int, double> bondOrders = new Dictionary<int, double>();

        /* Often used patterns */
        Regex objHeader;
        Regex objCommand;
        Regex atomTypePattern;

        int lineNumber = 0;
        int bondCounter = 0;
        private RebondTool rebonder;

        /*
         * construct a new reader from a Reader type object
         * @param input reader from which input is read
         */
        public PMPReader(TextReader input)
        {
            this.input = input;
            this.lineNumber = 0;

            /* compile patterns */
            objHeader = new Regex(".*\\((\\d+)\\s(\\w+)$", RegexOptions.Compiled);
            objCommand = new Regex(".*\\(A\\s(C|F|D|I|O)\\s(\\w+)\\s+\"?(.*?)\"?\\)$", RegexOptions.Compiled);
            atomTypePattern = new Regex("^(\\d+)\\s+(\\w+)$", RegexOptions.Compiled);

            rebonder = new RebondTool(2.0, 0.5, 0.5);
        }

        public PMPReader(Stream input)
                : this(new StreamReader(input))
        { }

        public PMPReader()
                : this(new StringReader(""))
        { }

        public override IResourceFormat Format => PMPFormat.Instance;

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
            return false;
        }

        /**
         * reads the content from a PMP input. It can only return a
         * IChemObject of type ChemFile
         *
         * @param object class must be of type ChemFile
         *
         * @see IChemFile
         */
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

        // private procedures

        private string ReadLine()
        {
            string line = input.ReadLine();
            lineNumber = lineNumber + 1;
            Debug.WriteLine("LINE (" + lineNumber + "): ", line);
            return line;
        }

        /**
         *  Private method that actually parses the input to read a ChemFile
         *  object.
         *
         *  Each PMP frame is stored as a Crystal in a ChemModel. The PMP
         *  file is stored as a ChemSequence of ChemModels.
         *
         * @return A ChemFile containing the data parsed from input.
         */
        private IChemFile ReadChemFile(IChemFile chemFile)
        {
            IChemSequence chemSequence = chemFile.Builder.CreateChemSequence();
            IChemModel chemModel = chemFile.Builder.CreateChemModel();
            ICrystal crystal = chemFile.Builder.CreateCrystal();

            try
            {
                string line = ReadLine();
                while (line != null)
                {
                    if (line.StartsWith("%%Header Start"))
                    {
                        // parse Header section
                        while (line != null && !(line.StartsWith("%%Header End")))
                        {
                            if (line.StartsWith("%%Version Number"))
                            {
                                string version = ReadLine().Trim();
                                if (!version.Equals("3.00"))
                                {
                                    Trace.TraceError("The PMPReader only supports PMP files with version 3.00");
                                    return null;
                                }
                            }
                            line = ReadLine();
                        }
                    }
                    else if (line.StartsWith("%%Model Start"))
                    {
                        // parse Model section
                        modelStructure = chemFile.Builder.CreateAtomContainer();
                        while (line != null && !(line.StartsWith("%%Model End")))
                        {
                            var objHeaderMatcher = objHeader.Match(line);
                            if (objHeaderMatcher.Success)
                            {
                                string obj = objHeaderMatcher.Groups[2].Value;
                                ConstructObject(chemFile.Builder, obj);
                                int id = int.Parse(objHeaderMatcher.Groups[1].Value);
                                // Debug.WriteLine(object + " id: " + id);
                                line = ReadLine();
                                while (line != null && !(line.Trim().Equals(")")))
                                {
                                    // parse object command (or new object header)
                                    var objCommandMatcher = objCommand.Match(line);
                                    objHeaderMatcher = objHeader.Match(line);
                                    if (objHeaderMatcher.Success)
                                    {
                                        // ok, forget about nesting and hope for the best
                                        obj = objHeaderMatcher.Groups[2].Value;
                                        id = int.Parse(objHeaderMatcher.Groups[1].Value);
                                        ConstructObject(chemFile.Builder, obj);
                                    }
                                    else if (objCommandMatcher.Success)
                                    {
                                        string format = objCommandMatcher.Groups[1].Value;
                                        string command = objCommandMatcher.Groups[2].Value;
                                        string field = objCommandMatcher.Groups[3].Value;

                                        ProcessModelCommand(obj, command, format, field);
                                    }
                                    else
                                    {
                                        Trace.TraceWarning("Skipping line: " + line);
                                    }
                                    line = ReadLine();
                                }
                                if (chemObject is IAtom)
                                {
                                    atomids[id] = modelStructure.Atoms.Count;
                                    atomZOrders[int.Parse(chemObject.GetProperty<string>(PMP_ZORDER))] = id;
                                    atomGivenIds[int.Parse(chemObject.GetProperty<string>(PMP_ID))] = id;
                                    modelStructure.Atoms.Add((IAtom)chemObject);
                                    //                            } else if (chemObject is IBond) {
                                    //                                bondids[new int(id)] = new int(molecule.Atoms.Count);
                                    //                                molecule.Bonds.Add((IBond)chemObject);
                                }
                                else
                                {
                                    Trace.TraceError("chemObject is not initialized or of bad class type");
                                }
                                // Debug.WriteLine(molecule.ToString());
                            }
                            line = ReadLine();
                        }
                        if (line.StartsWith("%%Model End"))
                        {
                            // during the Model Start, all bonds are cached as PMP files might
                            // define bonds *before* the involved atoms :(
                            // the next lines dump the cache into the atom container

                            //                  	bondids[new int(id)] = new int(molecule.Atoms.Count);
                            //                  	molecule.Bonds.Add((IBond)chemObject);
                            int bondsFound = bondids.Count;
                            Debug.WriteLine("Found #bonds: ", bondsFound);
                            Debug.WriteLine("#atom ones: ", bondAtomOnes.Count);
                            Debug.WriteLine("#atom twos: ", bondAtomTwos.Count);
                            Debug.WriteLine("#orders: ", bondOrders.Count);
                            foreach (var index in bondids.Keys)
                            {
                                double order;
                                if (!bondOrders.TryGetValue(index, out order))
                                    order = 1;
                                Debug.WriteLine("index: ", index);
                                Debug.WriteLine("ones: ", bondAtomOnes[index]);
                                IAtom atom1 = modelStructure.Atoms[atomids[bondAtomOnes[index]]];
                                IAtom atom2 = modelStructure.Atoms[atomids[bondAtomTwos[index]]];
                                IBond bond = modelStructure.Builder.CreateBond(atom1, atom2);
                                if (order == 1.0)
                                {
                                    bond.Order = BondOrder.Single;
                                }
                                else if (order == 2.0)
                                {
                                    bond.Order = BondOrder.Double;
                                }
                                else if (order == 3.0)
                                {
                                    bond.Order = BondOrder.Triple;
                                }
                                else if (order == 4.0)
                                {
                                    bond.Order = BondOrder.Quadruple;
                                }
                                modelStructure.Bonds.Add(bond);
                            }
                        }
                    }
                    else if (line.StartsWith("%%Traj Start"))
                    {
                        chemSequence = chemFile.Builder.CreateChemSequence();
                        double energyFragment = 0.0;
                        double energyTotal = 0.0;
                        int Z = 1;
                        while (line != null && !(line.StartsWith("%%Traj End")))
                        {
                            if (line.StartsWith("%%Start Frame"))
                            {
                                chemModel = chemFile.Builder.CreateChemModel();
                                crystal = chemFile.Builder.CreateCrystal();
                                while (line != null && !(line.StartsWith("%%End Frame")))
                                {
                                    // process frame data
                                    if (line.StartsWith("%%Atom Coords"))
                                    {
                                        // calculate Z: as it is not explicitely given, try to derive it from the
                                        // energy per fragment and the total energy
                                        if (energyFragment != 0.0 && energyTotal != 0.0)
                                        {
                                            Z = (int)Math.Round(energyTotal / energyFragment);
                                            Debug.WriteLine("Z derived from energies: ", Z);
                                        }
                                        // add atomC as atoms to crystal
                                        int expatoms = modelStructure.Atoms.Count;
                                        for (int molCount = 1; molCount <= Z; molCount++)
                                        {
                                            IAtomContainer clone = modelStructure.Builder.CreateAtomContainer();
                                            for (int i = 0; i < expatoms; i++)
                                            {
                                                line = ReadLine();
                                                IAtom a = clone.Builder.CreateAtom();
                                                var st = Strings.Tokenize(line, ' ');
                                                a.Point3D = new Vector3(double.Parse(st[0]), double.Parse(st[1]), double.Parse(st[2]));
                                                a.CovalentRadius = 0.6;
                                                IAtom modelAtom = modelStructure.Atoms[atomids[atomGivenIds[i + 1]]];
                                                a.Symbol = modelAtom.Symbol;
                                                clone.Atoms.Add(a);
                                            }
                                            rebonder.Rebond(clone);
                                            crystal.Add(clone);
                                        }
                                    }
                                    else if (line.StartsWith("%%E/Frag"))
                                    {
                                        line = ReadLine().Trim();
                                        energyFragment = double.Parse(line);
                                    }
                                    else if (line.StartsWith("%%Tot E"))
                                    {
                                        line = ReadLine().Trim();
                                        energyTotal = double.Parse(line);
                                    }
                                    else if (line.StartsWith("%%Lat Vects"))
                                    {
                                        line = ReadLine();
                                        IList<string> st;
                                        st = Strings.Tokenize(line, ' ');
                                        crystal.A = new Vector3(double.Parse(st[0]), double.Parse(st[1]), double.Parse(st[2]));
                                        line = ReadLine();
                                        st = Strings.Tokenize(line, ' ');
                                        crystal.B = new Vector3(double.Parse(st[0]), double.Parse(st[1]), double.Parse(st[2]));
                                        line = ReadLine();
                                        st = Strings.Tokenize(line, ' ');
                                        crystal.C = new Vector3(double.Parse(st[0]), double.Parse(st[1]), double.Parse(st[2]));
                                    }
                                    else if (line.StartsWith("%%Space Group"))
                                    {
                                        line = ReadLine().Trim();
                                        /*
                                         * standardize space group name. See
                                         * Crystal.SetSpaceGroup()
                                         */
                                        if ("P 21 21 21 (1)".Equals(line))
                                        {
                                            crystal.SpaceGroup = "P 2_1 2_1 2_1";
                                        }
                                        else
                                        {
                                            crystal.SpaceGroup = "P1";
                                        }
                                    }
                                    else
                                    {
                                    }
                                    line = ReadLine();
                                }
                                chemModel.Crystal = crystal;
                                chemSequence.Add(chemModel);
                            }
                            line = ReadLine();
                        }
                        chemFile.Add(chemSequence);
                    }
                    else
                    {
                        // disregard line
                    }
                    // read next line
                    line = ReadLine();
                }
            }
            catch (IOException e)
            {
                Trace.TraceError("An IOException happened: ", e.Message);
                Debug.WriteLine(e);
                chemFile = null;
            }
            catch (CDKException e)
            {
                Trace.TraceError("An CDKException happened: ", e.Message);
                Debug.WriteLine(e);
                chemFile = null;
            }

            return chemFile;
        }

        private void ProcessModelCommand(string obj, string command, string format, string field)
        {
            Debug.WriteLine(obj + "->" + command + " (" + format + "): " + field);
            if ("Model".Equals(obj))
            {
                Trace.TraceWarning("Unkown PMP Model command: " + command);
            }
            else if ("Atom".Equals(obj))
            {
                if ("ACL".Equals(command))
                {
                    var atomTypeMatcher = atomTypePattern.Match(field);
                    if (atomTypeMatcher.Success)
                    {
                        int atomicnum = int.Parse(atomTypeMatcher.Groups[1].Value);
                        string type = atomTypeMatcher.Groups[2].Value;
                        ((IAtom)chemObject).AtomicNumber = atomicnum;
                        ((IAtom)chemObject).Symbol = type;
                    }
                    else
                    {
                        Trace.TraceError("Incorrectly formated field value: " + field + ".");
                    }
                }
                else if ("Charge".Equals(command))
                {
                    try
                    {
                        double charge = double.Parse(field);
                        ((IAtom)chemObject).Charge = charge;
                    }
                    catch (FormatException)
                    {
                        Trace.TraceError("Incorrectly formated float field: " + field + ".");
                    }
                }
                else if ("CMAPPINGS".Equals(command))
                {
                }
                else if ("FFType".Equals(command))
                {
                }
                else if ("Id".Equals(command))
                {
                    // ok, should take this into account too
                    chemObject.SetProperty(PMP_ID, field);
                }
                else if ("Mass".Equals(command))
                {
                }
                else if ("XYZ".Equals(command))
                {
                }
                else if ("ZOrder".Equals(command))
                {
                    // ok, should take this into account too
                    chemObject.SetProperty(PMP_ZORDER, field);
                }
                else
                {
                    Trace.TraceWarning("Unkown PMP Atom command: " + command);
                }
            }
            else if ("Bond".Equals(obj))
            {
                if ("Atom1".Equals(command))
                {
                    int atomid = int.Parse(field);
                    // this assumes that the atoms involved in this bond are
                    // already added, which seems the case in the PMP files
                    bondAtomOnes[bondCounter] = atomid;
                    //                IAtom a = molecule.Atoms[realatomid];
                    //                ((IBond)chemObject).SetAtomAt(a, 0);
                }
                else if ("Atom2".Equals(command))
                {
                    int atomid = int.Parse(field);
                    // this assumes that the atoms involved in this bond are
                    // already added, which seems the case in the PMP files
                    Debug.WriteLine("atomids: " + atomids);
                    Debug.WriteLine("atomid: " + atomid);
                    bondAtomTwos[bondCounter] = atomid;
                    //                IAtom a = molecule.Atoms[realatomid];
                    //                ((IBond)chemObject).SetAtomAt(a, 1);
                }
                else if ("Order".Equals(command))
                {
                    double order = double.Parse(field);
                    bondOrders[bondCounter] = order;
                    //                ((IBond)chemObject).Order = order;
                }
                else if ("Id".Equals(command))
                {
                    int bondid = int.Parse(field);
                    bondids[bondCounter] = bondid;
                }
                else if ("Label".Equals(command))
                {
                }
                else if ("3DGridOrigin".Equals(command))
                {
                }
                else if ("3DGridMatrix".Equals(command))
                {
                }
                else if ("3DGridDivision".Equals(command))
                {
                }
                else
                {
                    Trace.TraceWarning("Unkown PMP Bond command: " + command);
                }
            }
            else
            {
                Trace.TraceWarning("Unkown PMP object: " + obj);
            }
        }

        private void ConstructObject(IChemObjectBuilder builder, string obj)
        {
            if ("Atom".Equals(obj))
            {
                chemObject = builder.CreateAtom("C");
            }
            else if ("Bond".Equals(obj))
            {
                bondCounter++;
                chemObject = builder.CreateBond();
            }
            else if ("Model".Equals(obj))
            {
                modelStructure = builder.CreateAtomContainer();
            }
            else
            {
                Trace.TraceError("Cannot construct PMP object type: " + obj);
            }
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
