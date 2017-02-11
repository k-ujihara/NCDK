/* Copyright (C) 2009  Egon Willighagen <egonw@users.sf.net>
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
using NCDK.IO.Formats;
using System;
using System.IO;

namespace NCDK.IO.RDF
{
    /// <summary>
    /// Reads content from a CDK OWL serialization.
    /// </summary>
    // @cdk.module  iordf
    // @cdk.githash
    // @cdk.keyword file format, OWL
    public class CDKOWLReader : DefaultChemObjectReader
    {
        private TextReader input;

        /// <summary>
        /// Creates a new CDKOWLReader sending output to the given Writer.
        /// </summary>
        /// <param name="input"><see cref="TextReader"/> from which is OWL input is taken.</param>
        public CDKOWLReader(TextReader input)
        {
            this.input = input;
        }

        /// <summary>
        /// Creates a new CDKOWLReader with an undefined input.
        /// </summary>
        public CDKOWLReader()
        {
            this.input = null;
        }

        /// <summary>
        /// The <see cref="IResourceFormat"/> for this reader.
        /// </summary>
        public override IResourceFormat Format => CDKOWLFormat.Instance;

        /// <summary>
        /// This method must not be used; XML reading requires the use of an
        /// <see cref="Stream"/>. Use <see cref="SetReader(Stream)"/> instead.
        /// </summary>
        /// <param name="reader">reader to which should be written.</param>
        [Obsolete]
        public override void SetReader(TextReader reader)
        {
            this.input = reader;
        }

        public override void SetReader(Stream input)
        {
            this.input = new StreamReader(input);
        }

        public override bool Accepts(Type type)
        {
            if (typeof(IAtomContainer).IsAssignableFrom(type)) return true;
            return false;
        }

        public override T Read<T>(T obj)
        {
            if (!(obj is IAtomContainer))
                throw new CDKException("Only supported is reading of IAtomCOntainer objects.");
            IAtomContainer result = (IAtomContainer)obj;

            // do the actual parsing
#if true
            throw new NotImplementedException();
#else
            Model model = ModelFactory.CreateDefaultModel();
            model.Read(input, "", "N3");

            IAtomContainer mol = Convertor.Model2Molecule(model, obj.Builder);
            result.Add(mol);
            return (T)result;
#endif
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
