/* Copyright (C) 2002-2007  The Chemistry Development Kit (CDK) project
 *
 * Contact: cdk-devel@lists.sf.net
 *
 *  This library is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public
 *  License as published by the Free Software Foundation; either
 *  version 2.1 of the License, or (at your option) any later version.
 *
 *  This library is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 *  Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public
 *  License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using NCDK.Silent;
using NCDK.Utils.Xml;
using System;
using System.Diagnostics;
using System.Xml.Linq;

namespace NCDK.IO.InChI
{
    /// <summary>
    /// XReader handler for INChI XML fragment parsing.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The supported elements are: identifier, formula and
    /// connections. All other elements are not parsed (at this moment).
    /// This parser is written based on the INChI files in data/ichi
    /// for version 1.1Beta.
    /// </para>
    /// <para>The returned ChemFile contains a ChemSequence in
    /// which the ChemModel represents the molecule.
    /// </para>
    /// </remarks>
    /// <seealso cref="InChIReader"/>
    // @cdk.module extra
    // @cdk.githash
    [Obsolete]
    public class InChIHandler : XContentHandler
    {
        private ChemSequence chemSequence;
        private ChemModel chemModel;
        private IChemObjectSet<IAtomContainer> setOfMolecules;
        private IAtomContainer tautomer;

        public override void DoctypeDecl(XDocumentType docType)
        {
            if (docType == null)
                return;
            Trace.TraceInformation($"DocType root element: {docType.Name}");
            Trace.TraceInformation($"DocType root PUBLIC: {docType.PublicId}");
            Trace.TraceInformation($"DocType root SYSTEM: {docType.SystemId}");
        }

        public override void StartDocument()
        {
            ChemFile = new ChemFile();
            chemSequence = new ChemSequence();
            chemModel = new ChemModel();
            setOfMolecules = new ChemObjectSet<IAtomContainer>();
        }

        public override void EndDocument()
        {
            ChemFile.Add(chemSequence);
        }

        public override void EndElement(XElement element)
        {
            Debug.WriteLine($"end element: {element.ToString()}");
            if (string.Equals("identifier", element.Name.LocalName, StringComparison.Ordinal))
            {
                if (tautomer != null)
                {
                    // ok, add tautomer
                    setOfMolecules.Add(tautomer);
                    chemModel.MoleculeSet = setOfMolecules;
                    chemSequence.Add(chemModel);
                }
            }
            else if (string.Equals("formula", element.Name.LocalName, StringComparison.Ordinal))
            {
                if (tautomer != null)
                {
                    Trace.TraceInformation("Parsing <formula> chars: ", element.Value);
                    tautomer = new AtomContainer(InChIContentProcessorTool.ProcessFormula(
                            setOfMolecules.Builder.NewAtomContainer(), element.Value));
                }
                else
                {
                    Trace.TraceWarning("Cannot set atom info for empty tautomer");
                }
            }
            else if (string.Equals("connections", element.Name.LocalName, StringComparison.Ordinal))
            {
                if (tautomer != null)
                {
                    Trace.TraceInformation("Parsing <connections> chars: ", element.Value);
                    InChIContentProcessorTool.ProcessConnections(element.Value, tautomer, -1);
                }
                else
                {
                    Trace.TraceWarning("Cannot set dbond info for empty tautomer");
                }
            }
            else
            {
                // skip all other elements
            }
        }

        /// <summary>
        /// Implementation of the StartElement() procedure overwriting the
        /// DefaultHandler interface.
        /// </summary>
        public override void StartElement(XElement element)
        {
            Debug.WriteLine($"startElement: {element.ToString()}");
            Debug.WriteLine($"uri: {element.Name.NamespaceName}");
            Debug.WriteLine($"local: {element.Name.LocalName}");
            Debug.WriteLine($"raw: {element.ToString()}");
            if (string.Equals("INChI", element.Name.LocalName, StringComparison.Ordinal))
            {
                // check version
                foreach (var att in element.Attributes())
                {
                    if (string.Equals(att.Name.LocalName, "version", StringComparison.Ordinal))
                        Trace.TraceInformation("INChI version: ", att.Value);
                }
            }
            else if (string.Equals("structure", element.Name.LocalName, StringComparison.Ordinal))
            {
                tautomer = new AtomContainer();
            }
            else
            {
                // skip all other elements
            }
        }

        public ChemFile ChemFile { get; private set; }
    }
}
