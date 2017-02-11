/* Copyright (C) 2004-2007  Christian Hoppe <c.hoppe_@web.de>
 *                    2011  Egon Willighagen <egonw@users.sf.net>
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
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
using NCDK.Fingerprint;
using NCDK.Graphs;
using NCDK.IO;
using NCDK.IO.Iterator;
using NCDK.Isomorphisms.Matchers;
using NCDK.RingSearches;
using NCDK.Smiles;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace NCDK.Modeling.Builder3D
{
    /**
     * Helper class that help setup a template library of CDK's Builder3D.
     *
     * @author      Christian Hoppe
     * @cdk.module  builder3dtools
     * @cdk.githash
     */
    public class TemplateExtractor
    {
        const string usage = "Usage: TemplateExtractor SDFinfile outfile anyAtom=true/false anyBondAnyAtom=true/false";

        private readonly static IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;

        public TemplateExtractor() { }

        public void CleanDataSet(string dataFile)
        {
            var som = builder.CreateAtomContainerSet();
            try
            {
                Console.Out.WriteLine("Start clean dataset...");
                using (var fin = new StreamReader(dataFile))
                using (var imdl = new IteratingSDFReader(fin, builder))
                {
                    Console.Out.WriteLine("READY");
                    int c = 0;
                    foreach (var m in imdl)
                    {
                        c++;
                        if (c % 1000 == 0)
                        {
                            Console.Out.WriteLine("...");
                        }
                        if (m.Atoms.Count > 2)
                        {
                            if (m.Atoms[0].Point3D != null)
                            {
                                som.Add(m);
                            }
                        }
                    }
                }
                Console.Out.Write("Read File in..");
            }
            catch (Exception exc)
            {
                Console.Out.WriteLine("Could not read Molecules from file " + dataFile + " due to: " + exc.Message);
            }
            Console.Out.WriteLine(som.Count + " Templates are read in");
            WriteChemModel(som, dataFile, "_CLEAN");
        }

        public void ReadNCISdfFileAsTemplate(string dataFile)
        {
            var som = builder.CreateAtomContainerSet();
            try
            {
                Console.Out.WriteLine("Start...");
                using (var fin = new StreamReader(dataFile))
                using (var imdl = new IteratingSDFReader(fin, builder))
                {
                    Console.Out.Write("Read File in..");
                    Console.Out.WriteLine("READY");
                    foreach (var m in imdl)
                    {
                        som.Add(m);
                    }
                }
            }
            catch (Exception exc)
            {
                Console.Out.WriteLine("Could not read Molecules from file " + dataFile + " due to: " + exc.Message);
            }
            Console.Out.WriteLine(som.Count + " Templates are read in");
        }

        public void PartitionRingsFromComplexRing(string dataFile)
        {
            var som = builder.CreateAtomContainerSet();
            try
            {
                Console.Out.WriteLine("Start...");
                using (var fin = new StreamReader(dataFile))
                using (var imdl = new IteratingSDFReader(fin, builder))
                {
                    Console.Out.Write("Read File in..");
                    Console.Out.WriteLine("READY");
                    foreach (var m in imdl)
                    {
                        Console.Out.WriteLine("Atoms:" + m.Atoms.Count);
                        IRingSet ringSetM = Cycles.SSSR(m).ToRingSet();
                        // som.Add(m);
                        for (int i = 0; i < ringSetM.Count; i++)
                        {
                            som.Add(builder.CreateAtomContainer(ringSetM[i]));
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Console.Out.WriteLine("Could not read Molecules from file " + dataFile + " due to: " + exc.Message);
            }
            Console.Out.WriteLine(som.Count + " Templates are read in");
            WriteChemModel(som, dataFile, "_VERSUCH");
        }

        public void ExtractUniqueRingSystemsFromFile(string dataFile)
        {
            Console.Out.WriteLine("****** EXTRACT UNIQUE RING SYSTEMS ******");
            Console.Out.WriteLine("From file:" + dataFile);
            // RingPartitioner ringPartitioner=new RingPartitioner();

            Dictionary<string, string> hashRingSystems = new Dictionary<string, string>();
            SmilesGenerator smilesGenerator = new SmilesGenerator();

            int counterRings = 0;
            int counterMolecules = 0;
            int counterUniqueRings = 0;
            IRingSet ringSet = null;
            string key = "";
            IAtomContainer ac = null;

            string molfile = dataFile + "_UniqueRings";

            // FileOutputStream fout=null;
            try
            {
                using (var fout = new FileStream(molfile, FileMode.Create))
                using (var mdlw = new MDLV2000Writer(fout))
                {
                    try
                    {
                        Console.Out.WriteLine("Start...");
                        using (var fin = new StreamReader(dataFile))
                        using (var imdl = new IteratingSDFReader(fin, builder))
                        {
                            Console.Out.WriteLine("Read File in..");

                            foreach (var m in imdl)
                            {
                                counterMolecules = counterMolecules + 1;
                                /*
                                 * try{ HueckelAromaticityDetector.DetectAromaticity(m);
                                 * }Catch(Exception ex1){ Console.Out.WriteLine("Could not find
                                 * aromaticity due to:"+ex1); }
                                 */
                                IRingSet ringSetM = Cycles.SSSR(m).ToRingSet();

                                if (counterMolecules % 1000 == 0)
                                {
                                    Console.Out.WriteLine("Molecules:" + counterMolecules);
                                }

                                if (ringSetM.Count > 0)
                                {
                                    var ringSystems = RingPartitioner.PartitionRings(ringSetM);

                                    for (int i = 0; i < ringSystems.Count; i++)
                                    {
                                        ringSet = (IRingSet)ringSystems[i];
                                        ac = builder.CreateAtomContainer();
                                        var containers = RingSetManipulator.GetAllAtomContainers(ringSet);
                                        foreach (var container in containers)
                                        {
                                            ac.Add(container);
                                        }
                                        counterRings = counterRings + 1;
                                        // Only connection is important
                                        for (int j = 0; j < ac.Atoms.Count; j++)
                                        {
                                            (ac.Atoms[j]).Symbol = "C";
                                        }

                                        try
                                        {
                                            key = smilesGenerator.Create(builder.CreateAtomContainer(ac));
                                        }
                                        catch (CDKException e)
                                        {
                                            Trace.TraceError(e.Message);
                                            return;
                                        }

                                        // Console.Out.WriteLine("OrgKey:"+key+" For
                                        // Molecule:"+counter);
                                        if (hashRingSystems.ContainsKey(key))
                                        {
                                            // Console.Out.WriteLine("HAS KEY:ADD");
                                            // Vector tmp=(Vector)HashRingSystems[key];
                                            // tmp.Add((AtomContainer)ringSet.GetRingSetInAtomContainer());
                                            // HashRingSystems.Put(key,tmp);
                                            // int
                                            // tmp=((int)HashRingSystems[key]).Value;
                                            // tmp=tmp+1;
                                            // HashRingSystems.Put(key,new int(tmp));
                                        }
                                        else
                                        {
                                            counterUniqueRings = counterUniqueRings + 1;
                                            // Vector rings2=new Vector();
                                            // rings2.Add((AtomContainer)RingSetManipulator.GetAllInOneContainer(ringSet));
                                            hashRingSystems[key] = "1";
                                            try
                                            {
                                                // mdlw.Write(new Molecule
                                                // ((AtomContainer)RingSetManipulator.GetAllInOneContainer(ringSet)));
                                                mdlw.Write(builder.CreateAtomContainer(ac));
                                            }
                                            catch (Exception emdl)
                                            {
                                                if (!(emdl is ArgumentException || emdl is CDKException))
                                                    throw;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        Console.Out.WriteLine("Could not read Molecules from file " + dataFile + " due to: " + exc.Message);
                    }
                }
            }
            catch (Exception ex2)
            {
                Console.Out.WriteLine("IOError:cannot write file due to:" + ex2.ToString());
            }
            // Console.Out.WriteLine("READY Molecules:"+counterMolecules);
            Console.Out.WriteLine($"READY Molecules:{counterMolecules} RingSystems:{counterRings} UniqueRingsSystem:{counterUniqueRings}");
            Console.Out.WriteLine($"HashtableKeys:{hashRingSystems.Count}");

            /*
             * int c=0; Set keyset = HashRingSystems.Keys; Iterator
             * it=keyset.iterator(); IAtomContainerSet som=new AtomContainerSet();
             * SmilesParser smileParser=new SmilesParser(); string ringSmile="";
             * while (it.HasNext()) { key=(string)it.Next();
             * ringSmile=(string)HashRingSystems[key];
             * Console.Out.WriteLine("HashtableSmile:"+ringSmile+" key:"+key); try{
             * som.Add(smileParser.ParseSmiles(ringSmile)); }catch
             * (Exception ex5){ Console.Out.WriteLine("Error in som.addmolecule due
             * to:"+ex5); } }
             */

            // WriteChemModel(som,dataFile,"_TESTTESTTESTTESTTEST");
        }

        public void WriteChemModel(IAtomContainerSet<IAtomContainer> som, string file, string endFix)
        {
            Console.Out.WriteLine($"WRITE Molecules:{som.Count}");
            string molfile = file + endFix;
            try
            {
                using (var fout = new FileStream(molfile, FileMode.Create))
                using (var mdlw = new MDLV2000Writer(fout))
                {
                    mdlw.Write(som);
                }
            }
            catch (Exception ex2)
            {
                if (!(ex2 is CDKException || ex2 is IOException))
                    throw;
                Console.Out.WriteLine("IOError:cannot write file due to:" + ex2.ToString());
            }
        }

        public void MakeCanonicalSmileFromRingSystems(string dataFileIn, string dataFileOut)
        {
            Console.Out.WriteLine("Start make SMILES...");
            // QueryAtomContainer query=null;
            List<string> data = new List<string>();
            SmilesGenerator smiles = new SmilesGenerator();
            try
            {
                Console.Out.WriteLine("Start...");
                using (var fin = new StreamReader(dataFileIn))
                using (var imdl = new IteratingSDFReader(fin, builder))
                {
                    Console.Out.WriteLine("Read File in..");

                    foreach (var m in imdl)
                    {
                        /*
                         * try{ HueckelAromaticityDetector.DetectAromaticity(m);
                         * }Catch(Exception ex1){ Console.Out.WriteLine("Could not find
                         * aromaticity due to:"+ex1); }
                         */
                        // query=QueryAtomContainerCreator.CreateAnyAtomContainer(m,true);
                        // Console.Out.WriteLine("string:"+smiles.CreateSMILES(new
                        // Molecule(m)));
                        try
                        {
                            data.Add((string)smiles.Create(builder.CreateAtomContainer(m)));
                        }
                        catch (Exception exc1)
                        {
                            if (!(exc1 is CDKException || exc1 is IOException))
                                throw;
                            Console.Out.WriteLine("Could not create smile due to: " + exc1.Message);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Console.Out.WriteLine("Could not read Molecules from file " + dataFileIn + " due to: " + exc.Message);
            }

            Console.Out.Write("...ready\nWrite data...");
            try
            {
                using (var fout = new StreamWriter(dataFileOut))
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        // Console.Out.WriteLine("write:"+(string)data[i]);
                        try
                        {
                            fout.Write(((string)data[i]));
                            fout.WriteLine();
                        }
                        catch (Exception)
                        {
                        }
                    }
                    Console.Out.WriteLine("number of smiles:" + data.Count);
                }
            }
            catch (Exception exc3)
            {
                Console.Out.WriteLine("Could not write smile in file " + dataFileOut + " due to: " + exc3.Message);
            }
            Console.Out.WriteLine("...ready");
        }

        public IList<IBitFingerprint> MakeFingerprintsFromSdf(bool anyAtom, bool anyAtomAnyBond, IDictionary<string, int> timings, TextReader fin, int limit)
        {
            IFingerprinter fingerPrinter = new HybridizationFingerprinter(HybridizationFingerprinter.DEFAULT_SIZE, HybridizationFingerprinter.DEFAULT_SEARCH_DEPTH);
            //QueryAtomContainer query=null;
            IAtomContainer query = null;
            List<IBitFingerprint> data = new List<IBitFingerprint>();
            try
            {
                Console.Out.Write("Read data file in ...");
                using (var imdl = new IteratingSDFReader(fin, builder))
                {
                    Console.Out.WriteLine("ready");

                    int moleculeCounter = 0;
                    int fingerprintCounter = 0;
                    Console.Out.Write("Generated Fingerprints: " + fingerprintCounter + "    ");
                    foreach (var m in imdl)
                    {
                        if (!(moleculeCounter < limit || limit == -1))
                            break;
                        moleculeCounter++;
                        if (anyAtom && !anyAtomAnyBond)
                        {
                            query = QueryAtomContainerCreator.CreateAnyAtomContainer(m, false);
                        }
                        else
                        {
                            query = AtomContainerManipulator.Anonymise(m);
                        }
                        try
                        {
                            long time = -DateTime.Now.Ticks / 10000;
                            if (anyAtom || anyAtomAnyBond)
                            {
                                data.Add(fingerPrinter.GetBitFingerprint(query));
                                fingerprintCounter = fingerprintCounter + 1;
                            }
                            else
                            {
                                data.Add(fingerPrinter.GetBitFingerprint(query));
                                fingerprintCounter = fingerprintCounter + 1;
                            }
                            time += (DateTime.Now.Ticks / 10000);
                            // store the time
                            string bin = ((int)Math.Floor(time / 10.0)).ToString();
                            if (timings.ContainsKey(bin))
                            {
                                timings[bin] = (timings[bin]) + 1;
                            }
                            else
                            {
                                timings[bin] = 1;
                            }
                        }
                        catch (Exception exc1)
                        {
                            Console.Out.WriteLine($"QueryFingerprintError: from molecule:{moleculeCounter} due to:{exc1.Message}");

                            // OK, just adds a fingerprint with all ones, so that any
                            // structure will match this template, and leave it up
                            // to substructure match to figure things out
                            IBitFingerprint allOnesFingerprint = new BitSetFingerprint(fingerPrinter.Count);
                            for (int i = 0; i < fingerPrinter.Count; i++)
                            {
                                allOnesFingerprint.Set(i);
                            }
                            data.Add(allOnesFingerprint);
                            fingerprintCounter = fingerprintCounter + 1;
                        }

                        if (fingerprintCounter % 2 == 0)
                            Console.Out.Write("\b" + "/");
                        else
                            Console.Out.Write("\b" + "\\");

                        if (fingerprintCounter % 100 == 0)
                            Console.Out.Write("\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b"
                                    + "Generated Fingerprints: " + fingerprintCounter + "   \n");

                    }// while
                    Console.Out.Write($"...ready with:{moleculeCounter} molecules\nWrite data...of data vector:{data.Count} fingerprintCounter:{fingerprintCounter}");
                }
            }
            catch (Exception exc)
            {
                Console.Out.WriteLine("Could not read Molecules from file" + " due to: " + exc.Message);
            }
            return data;
        }

        public void MakeFingerprintFromRingSystems(string dataFileIn, string dataFileOut, bool anyAtom, bool anyAtomAnyBond)
        {
            IDictionary<string, int> timings = new Dictionary<string, int>();

            Console.Out.WriteLine("Start make fingerprint from file:" + dataFileIn + " ...");
            using (var fin = new StreamReader(dataFileIn))
            {
                var data = MakeFingerprintsFromSdf(anyAtom, anyAtomAnyBond, timings, fin, -1);
                try
                {
                    using (var fout = new StreamWriter(dataFileOut))
                    {
                        for (int i = 0; i < data.Count; i++)
                        {
                            try
                            {
                                fout.Write(data[i].ToString());
                                fout.WriteLine();
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
                catch (Exception exc3)
                {
                    Console.Out.WriteLine($"Could not write Fingerprint in file {dataFileOut} due to: {exc3.Message}");
                }

                Console.Out.WriteLine($"\nFingerprints:{data.Count} are written...ready");
                Console.Out.WriteLine($"\nComputing time statistics:\n{timings.ToString()}");
            }
        }

        public IAtomContainer RemoveLoopBonds(IAtomContainer molecule, int position)
        {
            for (int i = 0; i < molecule.Bonds.Count; i++)
            {
                IBond bond = molecule.Bonds[i];
                if (bond.Atoms[0] == bond.Atoms[1])
                {
                    Console.Out.WriteLine("Loop found! Molecule:" + position);
                    molecule.Bonds.Remove(bond);
                }
            }

            return molecule;
        }

        public IAtomContainer CreateAnyAtomAtomContainer(IAtomContainer atomContainer)
        {
            IAtomContainer query = (IAtomContainer)atomContainer.Clone();
            // Console.Out.WriteLine("createAnyAtomAtomContainer");
            for (int i = 0; i < query.Atoms.Count; i++)
            {
                // Console.Out.Write(" "+i);
                query.Atoms[i].Symbol = "C";
            }
            return query;
        }

        public IAtomContainer ResetFlags(IAtomContainer ac)
        {
            for (int f = 0; f < ac.Atoms.Count; f++)
            {
                ac.Atoms[f].IsVisited = false;
            }
            foreach (var ec in ac.GetElectronContainers())
            {
                ec.IsVisited = false;
            }
            return ac;
        }

        public static void Main(string[] args)
        {
            if (args.Length < 4)
            {
                Console.Out.WriteLine(usage);
            }
            try
            {
                new TemplateExtractor().MakeFingerprintFromRingSystems(args[0], args[1], bool.Parse(args[2]), bool.Parse(args[3]));
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(usage);
                // TODO Auto-generated catch block
                Console.Out.WriteLine(e.StackTrace);
            }
        }
    }
}
