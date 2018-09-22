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

using NCDK.Aromaticities;
using NCDK.QSAR.Results;
using NCDK.Smiles.SMARTS;
using NCDK.Tools.Manipulator;
using System;
using System.Collections.Generic;

namespace NCDK.QSAR.Descriptors.Moleculars
{
    /// <summary>
    /// Returns the number of acidic groups. The list of acidic groups is defined
    /// by these SMARTS 
    /// "$([O;H1]-[C,S,P]=O)", 
    /// "$([*;-;!$(*~[*;+])])",
    /// "$([NH](S(=O)=O)C(F)(F)F)", and "$(n1nnnc1)" 
    /// originally presented in
    /// JOELib <token>cdk-cite-WEGNER2006</token>.
    /// </summary>
    // @author      egonw
    // @cdk.module  qsarmolecular
    // @cdk.githash
    // @cdk.dictref qsar-descriptors:acidicGroupCount  
    public class AcidicGroupCountDescriptor 
        : AbstractMolecularDescriptor, IMolecularDescriptor
    {
        private readonly static string[] SMARTS_STRINGS =
        {
            "[$([O;H1]-[C,S,P]=O)]",
            "[$([*;-;!$(*~[*;+])])]",
            "[$([NH](S(=O)=O)C(F)(F)F)]",
            "[$(n1nnnc1)]"
        };
        private readonly static string[] NAMES = { "nAcid" };

        private static readonly List<SMARTSQueryTool> tools = new List<SMARTSQueryTool>();
        private bool checkAromaticity;

        public AcidicGroupCountDescriptor()
        {
            this.checkAromaticity = true;
        }

        static AcidicGroupCountDescriptor()
        {
            foreach (var smarts in SMARTS_STRINGS)
            {
                tools.Add(new SMARTSQueryTool(smarts, Silent.ChemObjectBuilder.Instance));
            }
        }

        public override IImplementationSpecification Specification => specification;
        private static readonly DescriptorSpecification specification =
             new DescriptorSpecification(
                    "http://www.blueobelisk.org/ontologies/chemoinformatics-algorithms/#acidicGroupCount",
                     typeof(AcidicGroupCountDescriptor).FullName,
                    "The Chemistry Development Kit");

        public override IReadOnlyList<object> Parameters
        {
            set
            {
                if (value.Count != 1)
                {
                    throw new CDKException(nameof(AcidicGroupCountDescriptor) + " requires 1 parameter.");
                }
                if (!(value[0] is bool))
                {
                    throw new CDKException("The parameter must be of type " + nameof(System.Boolean));
                }

                // OK, all should be fine
                this.checkAromaticity = (bool)value[0];
            }
            get
            {
                return new object[] { this.checkAromaticity };
            }
        }

        public override IReadOnlyList<string> DescriptorNames => NAMES;

        public DescriptorValue<Result<int>> Calculate(IAtomContainer atomContainer)
        {
            atomContainer = Clone(atomContainer); // don't mod original

            // do aromaticity detection
            if (this.checkAromaticity)
            {
                try
                {
                    AtomContainerManipulator.PercieveAtomTypesAndConfigureAtoms(atomContainer);
                    Aromaticity.CDKLegacy.Apply(atomContainer);
                }
                catch (CDKException exception)
                {
                    return GetDummyDescriptorValue(exception);
                }
            }

            try
            {
                int count = 0;
                foreach (var tool in tools)
                {
                    if (tool.Matches(atomContainer))
                        count += tool.MatchesCount;
                }
                return new DescriptorValue<Result<int>>(specification, ParameterNames, Parameters, new Result<int>(count), DescriptorNames);
            }
            catch (CDKException exception)
            {
                return GetDummyDescriptorValue(exception);
            }
        }

        public override IDescriptorResult DescriptorResultType => Result.Instance<int>();
        public override IReadOnlyList<string> ParameterNames { get; } 
            = new string[] { "checkAromaticity" };

        public override object GetParameterType(string name)
        {
            object obj = null;
            if (string.Equals(name, "checkAromaticity", StringComparison.Ordinal)) 
                obj = true;
            return obj;
        }

        private DescriptorValue<Result<int>> GetDummyDescriptorValue(Exception exception)
        {
            return new DescriptorValue<Result<int>>(specification, ParameterNames, Parameters, new Result<int>(-1), DescriptorNames, exception);
        }

        IDescriptorValue IMolecularDescriptor.Calculate(IAtomContainer container) => Calculate(container);
    }
}
