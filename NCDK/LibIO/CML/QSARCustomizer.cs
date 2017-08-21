/*  Copyright (C) 2005-2007  The Chemistry Development Kit (CDK) project
 *                     2011  Dmitry Katsubo <dmitry.katsubo@gmail.com>
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *  All we ask is that proper credit is given for our work, which includes
 *  - but is not limited to - adding the above copyright notice to the beginning
 *  of your source code files, and to any copyright notice that you may distribute
 *  with programs based on this work.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 */
using NCDK.QSAR;
using NCDK.QSAR.Result;
using System.Text;
using System.Xml.Linq;

namespace NCDK.LibIO.CML
{
    /// <summary>
    /// Customizer for the libio-cml Convertor to be able to export details for
    /// QSAR descriptors calculated for Molecules.
    /// </summary>
    // @author        egonw
    // @cdk.created   2005-05-04
    // @cdk.module    qsarcml
    // @cdk.githash
    public class QSARCustomizer : ICMLCustomizer
    {
        private const string QSAR_NAMESPACE = "qsar";
        private const string QSAR_URI = "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/";

        public void Customize(IBond bond, object nodeToAdd)
        {
            CustomizeIChemObject(bond, nodeToAdd);
        }

        public void Customize(IAtom atom, object nodeToAdd)
        {
            CustomizeIChemObject(atom, nodeToAdd);
        }

        public void Customize(IAtomContainer molecule, object nodeToAdd)
        {
            CustomizeIChemObject(molecule, nodeToAdd);
        }

        private XElement CreateScalar(IDescriptorResult value)
        {
            XElement scalar = null;
            if (value is DoubleResult)
            {
                scalar = new CMLScalar(((DoubleResult)value).Value);
            }
            else if (value is IntegerResult)
            {
                scalar = new CMLScalar(((IntegerResult)value).Value);
            }
            else if (value is BooleanResult)
            {
                scalar = new CMLScalar(((BooleanResult)value).Value);
            }
            else if (value is IntegerArrayResult)
            {
                IntegerArrayResult result = (IntegerArrayResult)value;
                scalar = new CMLArray();
                scalar.SetAttributeValue(CMLElement.Attribute_dataType, "xsd:int");
                scalar.SetAttributeValue(CMLElement.Attribute_size, result.Length.ToString());
                StringBuilder buffer = new StringBuilder();
                for (int i = 0; i < result.Length; i++)
                {
                    buffer.Append(result[i] + " ");
                }
                scalar.Add(buffer.ToString());
            }
            else if (value is DoubleArrayResult)
            {
                DoubleArrayResult result = (DoubleArrayResult)value;
                scalar = new CMLArray();
                scalar.SetAttributeValue(CMLElement.Attribute_dataType, "xsd:double");
                scalar.SetAttributeValue(CMLElement.Attribute_size, "" + result.Length);
                StringBuilder buffer = new StringBuilder();
                for (int i = 0; i < result.Length; i++)
                {
                    buffer.Append(result[i] + " ");
                }
                scalar.Add(buffer.ToString());
            }
            else
            {
                // Trace.TraceError("Could not convert this object to a scalar element: ", value);
                scalar = new CMLScalar();
                scalar.Add(value.ToString());
            }
            return scalar;
        }

        private void CustomizeIChemObject(IChemObject obj, object nodeToAdd)
        {
            if (!(nodeToAdd is XElement)) throw new CDKException("NodeToAdd must be of type nu.xom.Element!");

            var element = (XElement)nodeToAdd;
            var props = obj.GetProperties();
            XElement propList = null;
            foreach (var key in props.Keys)
            {
                if (key is DescriptorSpecification)
                {
                    DescriptorSpecification specs = (DescriptorSpecification)key;
                    DescriptorValue value = (DescriptorValue)props[key];
                    IDescriptorResult result = value.Value;
                    if (propList == null)
                    {
                        propList = new CMLPropertyList();
                    }
                    XElement property = new CMLProperty();
                    // setup up the metadata list
                    var metadataList = new CMLMetadataList();
                    metadataList.SetAttributeValue(XNamespace.Xmlns + QSAR_NAMESPACE, QSAR_URI);
                    property.SetAttributeValue(CMLElement.Attribute_convention, QSAR_NAMESPACE + ":" + "DescriptorValue");
                    string specsRef = specs.SpecificationReference;
                    if (specsRef.StartsWith(QSAR_URI))
                    {
                        property.SetAttributeValue(XNamespace.Xmlns + QSAR_NAMESPACE, QSAR_URI);
                    }
                    CMLMetadata metaData = new CMLMetadata();
                    metaData.SetAttributeValue(CMLElement.Attribute_dictRef, QSAR_NAMESPACE + ":" + "specificationReference");
                    metaData.SetAttributeValue(CMLElement.Attribute_content, specsRef);
                    metadataList.Add(metaData);
                    metaData = new CMLMetadata();
                    metaData.SetAttributeValue(CMLElement.Attribute_dictRef, QSAR_NAMESPACE + ":" + "implementationTitle");
                    metaData.SetAttributeValue(CMLElement.Attribute_content, specs.ImplementationTitle);
                    metadataList.Add(metaData);
                    metaData = new CMLMetadata();
                    metaData.SetAttributeValue(CMLElement.Attribute_dictRef, QSAR_NAMESPACE + ":" + "implementationIdentifier");
                    metaData.SetAttributeValue(CMLElement.Attribute_content, specs.ImplementationIdentifier);
                    metadataList.Add(metaData);
                    metaData = new CMLMetadata();
                    metaData.SetAttributeValue(CMLElement.Attribute_dictRef, QSAR_NAMESPACE + ":" + "implementationVendor");
                    metaData.SetAttributeValue(CMLElement.Attribute_content, specs.ImplementationVendor);
                    metadataList.Add(metaData);
                    // add parameter setting to the metadata list
                    object[] parameters = value.Parameters;
                    //                Debug.WriteLine("Value: " + value.Specification.ImplementationIdentifier);
                    if (parameters != null && parameters.Length > 0)
                    {
                        string[] paramNames = value.ParameterNames;
                        var paramSettings = new CMLMetadataList();
                        paramSettings.SetAttributeValue(CMLElement.Attribute_title, QSAR_NAMESPACE + ":" + "descriptorParameters");
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            var paramSetting = new CMLMetadata();
                            string paramName = paramNames[i];
                            object paramVal = parameters[i];
                            if (paramName == null)
                            {
                                // Trace.TraceError("Parameter name was null! Cannot output to CML.");
                            }
                            else if (paramVal == null)
                            {
                                // Trace.TraceError("Parameter setting was null! Cannot output to CML. Problem param: " + paramName);
                            }
                            else
                            {
                                paramSetting.SetAttributeValue(CMLElement.Attribute_title, paramNames[i]);
                                paramSetting.SetAttributeValue(CMLElement.Attribute_content, parameters[i].ToString());
                                paramSettings.Add(paramSetting);
                            }
                        }
                        metadataList.Add(paramSettings);
                    }
                    property.Add(metadataList);
                    var scalar = this.CreateScalar(result);
                    scalar.SetAttributeValue(CMLElement.Attribute_dictRef, specsRef);
                    // add the actual descriptor value
                    property.Add(scalar);
                    propList.Add(property);
                } // else: disregard all other properties
            }
            if (propList != null)
            {
                element.Add(propList);
            }
        }
    }
}
