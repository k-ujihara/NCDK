/* Copyright (C) 1997-2007  The Chemistry Development Kit (CDK) project
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
using NCDK.IO.Formats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace NCDK.IO
{
    /**
     * Writes a reaction to a MDL rxn or SDF file. Attention: Stoichiometric
     * coefficients have to be natural numbers.
     *
     * <pre>
     * MDLRXNWriter writer = new MDLRXNWriter(new FileWriter(new File("output.mol")));
     * writer.Write((Molecule)molecule);
     * writer.Close();
     * </pre>
     *
     * See {@cdk.cite DAL92}.
     *
     * @cdk.module io
     * @cdk.githash
     * @cdk.iooptions
     *
     * @cdk.keyword file format, MDL RXN file
     */
    public class MDLRXNWriter : DefaultChemObjectWriter
    {

        private TextWriter writer;
        private int reactionNumber;
        public IDictionary<string, object> rdFields = null;

        /**
         * Constructs a new MDLWriter that can write an array of
         * Molecules to a Writer.
         *
         * @param   out  The Writer to write to
         */
        public MDLRXNWriter(TextWriter out_)
        {
            writer = out_;
            this.reactionNumber = 1;
        }

        /**
         * Constructs a new MDLWriter that can write an array of
         * Molecules to a given Stream.
         *
         * @param   output  The Stream to write to
         */
        public MDLRXNWriter(Stream output)
            : this(new StreamWriter(output))
        { }

        public MDLRXNWriter()
            : this(new StringWriter())
        { }

        public override IResourceFormat Format => MDLFormat.Instance;

        public override void SetWriter(TextWriter out_)
        {
            writer = out_;
        }

        public override void SetWriter(Stream output)
        {
            SetWriter(new StreamWriter(output));
        }

        /**
         * Here you can set a map which will be used to build rd fields in the file.
         * The entries will be translated to rd fields like this:<br>
         * &gt; &lt;key&gt;<br>
         * &gt; value<br>
         * empty line<br>
         *
         * @param  map The map to be used, map of string-string pairs
         */
        public void SetRdFields(IDictionary<string, object> map)
        {
            rdFields = map;
        }

        /**
         * Flushes the output and closes this object.
         */
        public override void Close()
        {
            writer.Close();
        }

        public override bool Accepts(Type type)
        {
            if (typeof(IReaction).IsAssignableFrom(type)) return true;
            if (typeof(IReactionSet).IsAssignableFrom(type)) return true;
            return false;
        }

        /**
         * Writes a IChemObject to the MDL RXN file formated output.
         * It can only output ChemObjects of type Reaction
         *
         * @param object class must be of type Molecule or MoleculeSet.
         *
         * @see org.openscience.cdk.ChemFile
         */
        public override void Write(IChemObject obj)
        {
            if (obj is IReactionSet)
            {
                WriteReactionSet((IReactionSet)obj);
            }
            else if (obj is IReaction)
            {
                WriteReaction((IReaction)obj);
            }
            else
            {
                throw new CDKException("Only supported is writing ReactionSet, Reaction objects.");
            }
        }

        /**
         *  Writes an array of Reaction to an Stream in MDL rdf format.
         *
         * @param   reactions  Array of Reactions that is written to an Stream
         */
        private void WriteReactionSet(IReactionSet reactions)
        {

            foreach (var iReaction in reactions)
            {
                WriteReaction(iReaction);
            }
        }

        /**
         * Writes a Reaction to an Stream in MDL sdf format.
         *
         * @param   reaction  A Reaction that is written to an Stream
         */
        private void WriteReaction(IReaction reaction)
        {
            int reactantCount = reaction.Reactants.Count;
            int productCount = reaction.Products.Count;
            if (reactantCount <= 0 || productCount <= 0)
            {
                throw new CDKException("Either no reactants or no products present.");
            }

            try
            {
                // taking care of the $$$$ signs:
                // we do not write such a sign at the end of the first reaction, thus we have to write on BEFORE the second reaction
                if (reactionNumber == 2)
                {
                    writer.Write("$$$$");
                    writer.WriteLine();
                }

                writer.Write("$RXN");
                writer.WriteLine();
                // reaction name
                string line = reaction.GetProperty<string>(CDKPropertyName.TITLE);
                if (line == null) line = "";
                if (line.Length > 80) line = line.Substring(0, 80);
                writer.Write(line);
                writer.WriteLine();
                // user/program/date&time/reaction registry no. line
                writer.WriteLine();
                // comment line
                line = reaction.GetProperty<string>(CDKPropertyName.REMARK);
                if (line == null) line = "";
                if (line.Length > 80) line = line.Substring(0, 80);
                writer.Write(line);
                writer.WriteLine();

                line = "";
                line += FormatMDLInt(reactantCount, 3);
                line += FormatMDLInt(productCount, 3);
                writer.Write(line);
                writer.WriteLine();

                int i = 0;
                foreach (var mapping in reaction.Mappings)
                {
                    var it = mapping.GetRelatedChemObjects().ToList();
                    it[0].SetProperty(CDKPropertyName.ATOM_ATOM_MAPPING, i + 1);
                    it[1].SetProperty(CDKPropertyName.ATOM_ATOM_MAPPING, i + 1);
                    i++;
                }
                WriteAtomContainerSet(reaction.Reactants);
                WriteAtomContainerSet(reaction.Products);

                //write sdfields, if any
                if (rdFields != null)
                {
                    ICollection<string> set = rdFields.Keys;
                    foreach (var element in set)
                    {
                        writer.Write("> <" + (string)element + ">");
                        writer.WriteLine();
                        writer.Write(rdFields[element].ToString());
                        writer.WriteLine();
                        writer.WriteLine();
                    }
                }
                // taking care of the $$$$ signs:
                // we write such a sign at the end of all except the first molecule
                if (reactionNumber != 1)
                {
                    writer.Write("$$$$");
                    writer.WriteLine();
                }
                reactionNumber++;

            }
            catch (IOException ex)
            {
                Trace.TraceError(ex.Message);
                Debug.WriteLine(ex);
                throw new CDKException("Exception while writing MDL file: " + ex.Message, ex);
            }
        }

        /**
         * Writes a MoleculeSet to an Stream for the reaction.
         *
         * @param   som  The MoleculeSet that is written to an Stream
         */
        private void WriteAtomContainerSet(IAtomContainerSet<IAtomContainer> som)
        {
            for (int i = 0; i < som.Count; i++)
            {
                IAtomContainer mol = som[i];
                for (int j = 0; j < som.GetMultiplier(i); j++)
                {
                    StringWriter sw = new StringWriter();
                    writer.Write("$MOL");
                    writer.WriteLine();
                    MDLV2000Writer mdlwriter = null;
                    try
                    {
                        mdlwriter = new MDLV2000Writer(sw);
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError(ex.Message);
                        Debug.WriteLine(ex);
                        throw new CDKException("Exception while creating MDLWriter: " + ex.Message, ex);
                    }
                    mdlwriter.Write(mol);
                    mdlwriter.Close();
                    writer.Write(sw.ToString());
                }
            }
        }

        /**
         * Formats an int to fit into the connectiontable and changes it
         * to a string.
         *
         * @param   i  The int to be formated
         * @param   l  Length of the string
         * @return     The string to be written into the connectiontable
         */
        private string FormatMDLInt(int i, int l)
        {
            var s = i.ToString(CultureInfo.InvariantCulture.NumberFormat);
            l = Math.Max(s.Length, l);
            return new string(' ', l - s.Length) + s;
        }

        public override void Dispose()
        {
            Close();
        }
    }
}
