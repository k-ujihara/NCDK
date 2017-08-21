/* Copyright (C) 2002-2007  Egon Willighagen <egonw@users.sf.net>
 *                    2014  Mark B Vine (orcid:0000-0002-7794-0426)
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
using NCDK.QSAR;
using NCDK.QSAR.Results;
using System.Xml.Linq;

namespace NCDK.IO.CML
{
    /// <summary>
    /// This is an implementation for the CDK convention.
    /// </summary>
    // @cdk.module io
    // @cdk.githash
    // @author egonw
    public class QSARConvention : CMLCoreModule
    {
        private string currentDescriptorAlgorithmSpecification;
        private string currentDescriptorImplementationTitel;
        private string currentDescriptorImplementationVendor;
        private string currentDescriptorImplementationIdentifier;
        private string currentDescriptorDataType;
        private string currentDescriptorResult;
        private bool currentDescriptorDataIsArray;

        public QSARConvention(IChemFile chemFile)
            : base(chemFile)
        {
        }

        public QSARConvention(ICMLModule conv)
            : base(conv)
        {
        }

        public override void StartElement(CMLStack xpath, XElement element)
        {
            //        <property xmlns:qsar="http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/"
            //            convention="qsar:DescriptorValue">
            //            <metadataList>
            //              <metadata dictRef="qsar:specificationReference" content="qsar:weight"/>
            //              <metadata dictRef="qsar:implementationTitle" content="NCDK.QSAR.Descriptors.Atomic.WeightDescriptor"/>
            //              <metadata dictRef="qsar:implementationIdentifier" content="$Id$"/>
            //              <metadata dictRef="qsar:implementationVendor" content="The Chemistry Development Kit"/>
            //              <metadataList title="qsar:descriptorParameters">
            //                <metadata title="elementSymbol" content="*"/>
            //              </metadataList>
            //            </metadataList>
            //            <scalar dataType="xsd:double" dictRef="qsar:weight">72.0</scalar>
            //          </property>

            if (xpath.EndsWith("molecule", "propertyList", "property"))
            {
                //            cdo.StartObject("MolecularDescriptor");
                currentDescriptorDataIsArray = false;
                currentDescriptorAlgorithmSpecification = "";
                currentDescriptorImplementationTitel = "";
                currentDescriptorImplementationVendor = "";
                currentDescriptorImplementationIdentifier = "";
                currentDescriptorDataType = "";
                currentDescriptorResult = "";
            }
            else if (xpath.EndsWith("property", "metadataList", "metadata"))
            {
                base.StartElement(xpath, element);
                if (DICTREF.Equals("qsar:specificationReference"))
                {
                    //                cdo.SetObjectProperty("MolecularDescriptor", "SpecificationReference", atts.GetValue("content"));
                    currentDescriptorAlgorithmSpecification = AttGetValue(element.Attributes(), "content");
                }
                else if (DICTREF.Equals("qsar:implementationTitle"))
                {
                    //                cdo.SetObjectProperty("MolecularDescriptor", "ImplementationTitle", atts.GetValue("content"));
                    currentDescriptorImplementationTitel = AttGetValue(element.Attributes(), "content");
                }
                else if (DICTREF.Equals("qsar:implementationIdentifier"))
                {
                    //                cdo.SetObjectProperty("MolecularDescriptor", "ImplementationIdentifier", atts.GetValue("content"));
                    currentDescriptorImplementationIdentifier = AttGetValue(element.Attributes(), "content");
                }
                else if (DICTREF.Equals("qsar:implementationVendor"))
                {
                    //                cdo.SetObjectProperty("MolecularDescriptor", "ImplementationVendor", atts.GetValue("content"));
                    currentDescriptorImplementationVendor = AttGetValue(element.Attributes(), "content");
                }
            }
            else if (xpath.EndsWith("propertyList", "property", "scalar"))
            {
                //            cdo.SetObjectProperty("MolecularDescriptor", "DataType", atts.GetValue("dataType"));
                currentDescriptorDataType = AttGetValue(element.Attributes(), "dataType");
                base.StartElement(xpath, element);
            }
            else
            {
                base.StartElement(xpath, element);
            }
        }

        public override void EndElement(CMLStack xpath, XElement element)
        {
            if (xpath.EndsWith("molecule", "propertyList", "property"))
            {
                //            cdo.EndObject("MolecularDescriptor");
                DescriptorSpecification descriptorSpecification = new DescriptorSpecification(
                        currentDescriptorAlgorithmSpecification, currentDescriptorImplementationTitel,
                        currentDescriptorImplementationIdentifier, currentDescriptorImplementationVendor);
                CurrentMolecule.SetProperty(descriptorSpecification, new DescriptorValue(descriptorSpecification,
                        new string[0], new object[0],
                        currentDescriptorDataIsArray ? NewDescriptorResultArray(currentDescriptorResult)
                                : NewDescriptorResult(currentDescriptorResult), new string[0]));
            }
            else if (xpath.EndsWith("property", "scalar"))
            {
                //            cdo.SetObjectProperty("MolecularDescriptor", "DescriptorValue", currentChars);
                currentDescriptorResult = element.Value;
            }
            else
            {
                base.EndElement(xpath, element);
            }
        }

        private IDescriptorResult NewDescriptorResult(string descriptorValue)
        {
            IDescriptorResult result = null;
            if ("xsd:double".Equals(currentDescriptorDataType))
            {
                result = new Result<double>(double.Parse(descriptorValue));
            }
            else if ("xsd:integer".Equals(currentDescriptorDataType))
            {
                result = new Result<int>(int.Parse(descriptorValue));
            }
            else if ("xsd:boolean".Equals(currentDescriptorDataType))
            {
                result = new Result<bool>(bool.Parse(descriptorValue));
            }
            return result;
        }

        private IDescriptorResult NewDescriptorResultArray(string descriptorValue)
        {
            IDescriptorResult result = null;
            if ("xsd:double".Equals(currentDescriptorDataType))
            {
                result = new ArrayResult<double>();
                foreach (var token in descriptorValue.Split(' '))
                {
                    ((ArrayResult<double>)result).Add(double.Parse(token));
                }
            }
            else if ("xsd:integer".Equals(currentDescriptorDataType))
            {
                result = new ArrayResult<int>();
                foreach (var token in descriptorValue.Split(' '))
                {
                    ((ArrayResult<int>)result).Add(int.Parse(token));
                }
            }
            return result;
        }
    }
}
