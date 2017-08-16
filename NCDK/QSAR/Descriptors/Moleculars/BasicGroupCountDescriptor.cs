/* Copyright (C) 2010  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.QSAR.Result;
using NCDK.Smiles.SMARTS;
using System;
using System.Collections.Generic;
using System.Linq;
using NCDK.Aromaticities;
using NCDK.Tools.Manipulator;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// Returns the number of basic groups. The list of basic groups is defined
    /// by these SMARTS 
    /// "[$([NH2]-[CX4])]", 
    /// "[$([NH](-[CX4])-[CX4])]",
    /// "[$(N(-[CX4])(-[CX4])-[CX4])]",    
    /// "[$([*;+;!$(*~[*;-])])]",
    /// "[$(N=C-N)]", and "[$(N-C=N)]" 
    /// originally presented in
    /// JOELib <token>cdk-cite-WEGNER2006</token>.
    /// </summary>
    // @author      egonw
    // @cdk.module  qsarmolecular
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:acidicGroupCount
    public class BasicGroupCountDescriptor 
        : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        private readonly static string[] SMARTS_STRINGS = 
        {
            "[$([NH2]-[CX4])]",
            "[$([NH](-[CX4])-[CX4])]",
            "[$(N(-[CX4])(-[CX4])-[CX4])]",
            "[$([*;+;!$(*~[*;-])])]",
            "[$(N=C-N)]", 
            "[$(N-C=N)]" 
        };
        private readonly static string[] NAMES = { "nBase" };

        private List<SMARTSQueryTool> tools = new List<SMARTSQueryTool>();

        /// <summary>
        /// Creates a new <see cref="BasicGroupCountDescriptor"/>.
        /// </summary>
        public BasicGroupCountDescriptor() 
        { 
        }

        public override void Initialise(IChemObjectBuilder builder)
        {
            foreach (var smarts in SMARTS_STRINGS)
            {
                tools.Add(new SMARTSQueryTool(smarts, builder));
            }
        }

        public override IImplementationSpecification Specification => _Specification;
        private static DescriptorSpecification _Specification { get; } =
            new DescriptorSpecification(
                "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#basicGroupCount",
                typeof(BasicGroupCountDescriptor).FullName, 
                "The Chemistry Development Kit");

        public override object[] Parameters
        {
            get { return null; }
            set { }
        }

        public override string[] DescriptorNames => NAMES;

        public override DescriptorValue Calculate(IAtomContainer atomContainer)
        {
            if (!tools.Any())
            {
                throw new InvalidOperationException("descriptor is not initialised, invoke 'initialise' first");
            }

            try
            {
                int count = 0;
                foreach (var tool in tools)
                {
                    if (tool.Matches(atomContainer)) count += tool.MatchesCount;
                }
                return new DescriptorValue(_Specification, ParameterNames, Parameters, new IntegerResult(count), DescriptorNames);
            }
            catch (CDKException exception)
            {
                return GetDummyDescriptorValue(exception);
            }
        }

        public override IDescriptorResult DescriptorResultType => IntegerResultType.Instance;
        public override string[] ParameterNames { get; } 
            = new string[] { };

        public override object GetParameterType(string name) 
        {
            object obj = null;
            return obj;
        }

        private DescriptorValue GetDummyDescriptorValue(Exception exception)
        {
            return new DescriptorValue(_Specification, ParameterNames, Parameters, new IntegerResult(-1),
                    DescriptorNames, exception);
        }
    }
}
