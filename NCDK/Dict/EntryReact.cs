/* Copyright (C) 2003-2007  The Chemistry Development Kit (CDK) project
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
using System.Collections.Generic;

namespace NCDK.Dict
{
    /**
    // Entry in a Dictionary for reactions.
     *
    // @author       Miguel Rojas <miguelrojasch@users.sf.net>
    // @cdk.created  2008-01-01
    // @cdk.keyword  dictionary
    // @cdk.module   dict
    // @cdk.githash
     *
    // @see          Dictionary
     */
    public class EntryReact : Entry
    {

        private List<string> reactionInfo;
        private List<string> representations;
        private Dictionary<string, string> parameters;
        private List<string> parametersValue;
        private List<string> reactionExample;
        private List<IList<string>> parameterClass;

        /**
        // Constructor of the EntryReact.
         *
        // @param identifier The ID value
        // @param term
         */
        public EntryReact(string identifier, string term)
            : base(identifier, term)
        {
            this.representations = new List<string>();
            this.parameters = new Dictionary<string, string>();
            this.parametersValue = new List<string>();
            this.reactionExample = new List<string>();
            this.parameterClass = new List<IList<string>>();
            this.reactionInfo = new List<string>();
        }

        /**
        // Constructor of the EntryReact.
         *
        // @param identifier The ID value
         */
        public EntryReact(string identifier)
            : this(identifier, "")
        {
        }

        public void AddReactionMetadata(string metadata)
        {
            this.reactionInfo.Add(metadata);
        }

        public IList<string> ReactionMetadata => this.reactionInfo;

        /**
        // Set the representation of the reaction.
         *
        // @param contentRepr The representation of the reaction as string
         */
        public void AddRepresentation(string contentRepr)
        {
            this.representations.Add(contentRepr);
        }

        /// <summary>
        /// The Representation of the reaction.
        /// </summary>
        public List<string> Representations => this.representations;

        /**
        // Set the parameters of the reaction.
         *
        // @param nameParam The parameter names of the reaction as string
        // @param typeParam The parameter types of the reaction as string
        // @param value     The value default of the parameter
         */
        public void SetParameters(string nameParam, string typeParam, string value)
        {
            this.parameters.Add(nameParam, typeParam);
            this.parametersValue.Add(value);
        }

        /**
        // Get the parameters of the reaction.
         *
        // @return A Dictionary with the parameters
         */
        public IDictionary<string, string> Parameters => this.parameters;

        /// <summary>
        /// The IParameterReact's of the reaction.
        /// </summary>
        public List<IList<string>> ParameterClass=> this.parameterClass;

        /// <summary>
        /// Add a IParameterReact's of the reaction.
        /// </summary>
        /// <param name="param">A string List containing the information about this parameter.</param>
        public void AddParameter(IList<string> param)
        {

            this.parameterClass.Add(param);
        }

        /// <summary>
        /// The parameter value of the reaction.
        /// </summary>
        public IList<string> ParameterValue => this.parametersValue;

        /// <summary>
        /// The mechanism of this reaction.
        /// </summary>
        public string Mechanism { get; set; }

        /**
        // add a example for this reaction.
         *
        // @param xml A reaction in XML scheme
         */
        public void AddExampleReaction(string xml)
        {
            this.reactionExample.Add(xml);
        }

        /// <summary>
        /// A List of reactions in XML schema.
        /// </summary>
        public IList<string> ExampleReactions => this.reactionExample;
    }
}
