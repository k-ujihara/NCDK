/* Copyright (C) 2002-2003  The Jmol Development Team
 * Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT Any WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Common.Primitives;
using NCDK.Numerics;
using NCDK.Config;
using NCDK.IO.Formats;
using System;
using System.Diagnostics;
using System.IO;

namespace NCDK.IO
{
    /// <summary>
    /// Reads Ghemical (<see href="http://www.uku.fi/~thassine/ghemical/">http://www.uku.fi/~thassine/ghemical/</see>) molecular mechanics (*.mm1gp) files.
    /// </summary>
    // @cdk.module ioformats
    // @cdk.githash
    // @author Egon Willighagen <egonw@sci.kun.nl>   
    public class GhemicalMMReader : DefaultChemObjectReader
    {
        private TextReader input = null;

        public GhemicalMMReader(TextReader input)
        {
            this.input = input;
        }

        public GhemicalMMReader(Stream input)
         : this(new StreamReader(input))
        { }

        public GhemicalMMReader()
            : this(new StringReader(""))
        { }

        public override IResourceFormat Format => GhemicalMMFormat.Instance;

        public override void SetReader(TextReader input)
        {
            this.input = input;
        }

        public override void SetReader(Stream input)
        {
            SetReader(new StreamReader(input));
        }

        public override void Close() { }

        public override void Dispose()
        {
            Close();
        }

        public override bool Accepts(Type type)
        {
            if (typeof(IChemFile).IsAssignableFrom(type)) return true;
            if (typeof(IChemModel).IsAssignableFrom(type)) return true;
            return false;
        }

        public override T Read<T>(T obj)
        {
            if (obj is IChemModel)
            {
                return (T)ReadChemModel((IChemModel)obj);
            }
            else if (obj is IChemFile)
            {
                IChemSequence sequence = obj.Builder.CreateChemSequence();
                sequence.Add((IChemModel)this.ReadChemModel(obj.Builder.CreateChemModel()));
                ((IChemFile)obj).Add(sequence);
                return obj;
            }
            else
            {
                throw new CDKException("Only supported is ChemModel.");
            }
        }

        private IChemModel ReadChemModel(IChemModel model)
        {
            int[] atoms = new int[1];
            double[] atomxs = new double[1];
            double[] atomys = new double[1];
            double[] atomzs = new double[1];
            double[] atomcharges = new double[1];

            int[] bondatomid1 = new int[1];
            int[] bondatomid2 = new int[1];
            BondOrder[] bondorder = new BondOrder[1];

            int numberOfAtoms = 0;
            int numberOfBonds = 0;

            try
            {
                string line = input.ReadLine();
                while (line != null)
                {
                    var st = Strings.Tokenize(line);
                    string command = st[0];
                    if ("!Header".Equals(command))
                    {
                        Trace.TraceWarning("Ignoring header");
                    }
                    else if ("!Info".Equals(command))
                    {
                        Trace.TraceWarning("Ignoring info");
                    }
                    else if ("!Atoms".Equals(command))
                    {
                        Trace.TraceInformation("Reading atom block");
                        // determine number of atoms to read
                        try
                        {
                            numberOfAtoms = int.Parse(st[1]);
                            Debug.WriteLine("  #atoms: " + numberOfAtoms);
                            atoms = new int[numberOfAtoms];
                            atomxs = new double[numberOfAtoms];
                            atomys = new double[numberOfAtoms];
                            atomzs = new double[numberOfAtoms];
                            atomcharges = new double[numberOfAtoms];

                            for (int i = 0; i < numberOfAtoms; i++)
                            {
                                line = input.ReadLine();
                                var atomInfoFields = Strings.Tokenize(line);
                                int atomID = int.Parse(atomInfoFields[0]);
                                atoms[atomID] = int.Parse(atomInfoFields[1]);
                                Debug.WriteLine("Set atomic number of atom (" + atomID + ") to: " + atoms[atomID]);
                            }
                        }
                        catch (Exception exception)
                        {
                            if (!(exception is FormatException || exception is IOException))
                                throw;
                            Trace.TraceError("Error while reading Atoms block");
                            Debug.WriteLine(exception);
                        }
                    }
                    else if ("!Bonds".Equals(command))
                    {
                        Trace.TraceInformation("Reading bond block");
                        try
                        {
                            // determine number of bonds to read
                            numberOfBonds = int.Parse(st[1]);
                            bondatomid1 = new int[numberOfAtoms];
                            bondatomid2 = new int[numberOfAtoms];
                            bondorder = new BondOrder[numberOfAtoms];

                            for (int i = 0; i < numberOfBonds; i++)
                            {
                                line = input.ReadLine();
                                var bondInfoFields = Strings.Tokenize(line);
                                bondatomid1[i] = int.Parse(bondInfoFields[0]);
                                bondatomid2[i] = int.Parse(bondInfoFields[1]);
                                string order = bondInfoFields[2];
                                if ("D".Equals(order))
                                {
                                    bondorder[i] = BondOrder.Double;
                                }
                                else if ("S".Equals(order))
                                {
                                    bondorder[i] = BondOrder.Single;
                                }
                                else if ("T".Equals(order))
                                {
                                    bondorder[i] = BondOrder.Triple;
                                }
                                else
                                {
                                    // ignore order, i.e. set to single
                                    Trace.TraceWarning("Unrecognized bond order, using single bond instead. Found: " + order);
                                    bondorder[i] = BondOrder.Single;
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            if (!(exception is FormatException || exception is IOException))
                                throw;
                            Trace.TraceError("Error while reading Bonds block");
                            Debug.WriteLine(exception);
                        }
                    }
                    else if ("!Coord".Equals(command))
                    {
                        Trace.TraceInformation("Reading coordinate block");
                        try
                        {
                            for (int i = 0; i < numberOfAtoms; i++)
                            {
                                line = input.ReadLine();
                                var atomInfoFields = Strings.Tokenize(line);
                                int atomID = int.Parse(atomInfoFields[0]);
                                double x = double.Parse(atomInfoFields[1]);
                                double y = double.Parse(atomInfoFields[2]);
                                double z = double.Parse(atomInfoFields[3]);
                                atomxs[atomID] = x;
                                atomys[atomID] = y;
                                atomzs[atomID] = z;
                            }
                        }
                        catch (Exception exception)
                        {
                            if (!(exception is FormatException || exception is IOException))
                                throw;
                            Trace.TraceError("Error while reading Coord block");
                            Debug.WriteLine(exception);
                        }
                    }
                    else if ("!Charges".Equals(command))
                    {
                        Trace.TraceInformation("Reading charges block");
                        try
                        {
                            for (int i = 0; i < numberOfAtoms; i++)
                            {
                                line = input.ReadLine();
                                var atomInfoFields = Strings.Tokenize(line);
                                int atomID = int.Parse(atomInfoFields[0]);
                                double charge = double.Parse(atomInfoFields[1]);
                                atomcharges[atomID] = charge;
                            }
                        }
                        catch (Exception exception)
                        {
                            if (!(exception is FormatException || exception is IOException))
                                throw;
                            Trace.TraceError("Error while reading Charges block");
                            Debug.WriteLine(exception);
                        }
                    }
                    else if ("!End".Equals(command))
                    {
                        Trace.TraceInformation("Found end of file");
                        // Store atoms
                        IAtomContainer container = model.Builder.CreateAtomContainer();
                        for (int i = 0; i < numberOfAtoms; i++)
                        {
                            try
                            {
                                IAtom atom = model.Builder.CreateAtom(Isotopes.Instance.GetElementSymbol(atoms[i]));
                                atom.AtomicNumber = atoms[i];
                                atom.Point3D = new Vector3(atomxs[i], atomys[i], atomzs[i]);
                                atom.Charge = atomcharges[i];
                                container.Atoms.Add(atom);
                                Debug.WriteLine("Stored atom: " + atom);
                            }
                            catch (Exception exception)
                            {
                                if (!(exception is IOException || exception is ArgumentException))
                                    throw;
                                Trace.TraceError("Cannot create an atom with atomic number: " + atoms[i]);
                                Debug.WriteLine(exception);
                            }
                        }

                        // Store bonds
                        for (int i = 0; i < numberOfBonds; i++)
                        {
                            container.AddBond(container.Atoms[bondatomid1[i]], container.Atoms[bondatomid2[i]], bondorder[i]);
                        }

                        var moleculeSet = model.Builder.CreateAtomContainerSet();
                        moleculeSet.Add(model.Builder.CreateAtomContainer(container));
                        model.MoleculeSet = moleculeSet;

                        return model;
                    }
                    else
                    {
                        Trace.TraceWarning("Skipping line: " + line);
                    }

                    line = input.ReadLine();
                }
            }
            catch (Exception exception)
            {
                if (!(exception is IOException || exception is ArgumentException))
                    throw;
                Trace.TraceError("Error while reading file");
                Debug.WriteLine(exception);
            }

            // this should not happen, file is lacking !End command
            return null;
        }
    }
}
