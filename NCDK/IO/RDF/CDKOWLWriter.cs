/* Copyright (C) 2009  Egon Willighagen <egonw@users.sf.net>
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

using NCDK.IO.Formats;
using NCDK.LibIO.DotNetRDF;
using System;
using System.IO;
using VDS.RDF;
using VDS.RDF.Writing;

namespace NCDK.IO.RDF
{
    /// <summary>
    /// Serializes the data model into CDK OWL.
    /// </summary>
    // @cdk.module iordf
    public class CDKOWLWriter : DefaultChemObjectWriter
    {
        private TextWriter output;

        /// <summary>
        /// Creates a new CDKOWLWriter sending output to the given Writer.
        /// </summary>
        /// <param name="output"><see cref="TextWriter"/> to which is OWL output is routed.</param>
        public CDKOWLWriter(TextWriter output)
        {
            this.output = output;
        }

        /// <summary>
        /// Returns the <see cref="IResourceFormat"/> for this writer.
        /// </summary>
        public override IResourceFormat Format => CDKOWLFormat.Instance;

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    output.Dispose();
                }

                output = null;

                disposedValue = true;
                base.Dispose(disposing);
            }
        }
        #endregion

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
                    throw new CDKException($"Error while writing HIN file: {ex.Message}", ex);
                }
            }
            else
            {
                throw new CDKException("CDKOWLWriter only supports output of IAtomContainer classes.");
            }
        }

        private void WriteMolecule(IAtomContainer mol)
        {
            Graph g = CreateCDKModel();
            var convertor = new Convertor(g);
            convertor.Molecule2Model(mol);

            NTriplesWriter ntwriter = new NTriplesWriter();
            ntwriter.Save(g, output);
        }

        private static Graph CreateCDKModel()
        {
            Graph g = new Graph();
            g.NamespaceMap.AddNamespace("cdk", new Uri("http://cdk.sourceforge.net/model.owl#"));
            return g;
        }
    }
}
