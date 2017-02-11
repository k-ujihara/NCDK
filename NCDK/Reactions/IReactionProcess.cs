/* Copyright (C) 2006-2007  Miguel Rojas <miguel.rojas@uni-koeln.de>
 *
 *  Contact: cdk-devel@lists.sourceforge.net
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public License
 *  as published by the Free Software Foundation; either version 2.1
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT Any WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using NCDK.Reactions.Types.Parameters;
using System.Collections.Generic;

namespace NCDK.Reactions
{
    /**
     * Classes that implement this interface are Reactions types.
     *
     * @author      Miguel Rojas
     * @cdk.module  reaction
     * @cdk.githash
     */
    public interface IReactionProcess
    {

        /**
         * Returns a <code>IDictionary</code> which specifies which reaction
         * is implemented by this class.
         *
         * These fields are used in the map:
         * <ul>
         * <li>Specification-Reference: refers to an entry in a unique dictionary or web page
         * <li>Implementation-Title: anything
         * <li>Implementation-Identifier: a unique identifier for this version of
         *  this class
         * <li>Implementation-Vendor: CDK, JOELib, or anything else
         * </ul>
         *
         * @return An object containing the reaction specification
         */
        ReactionSpecification Specification { get; }

        /// <summary>
        /// the parameters for this reaction.
        /// </summary>
        /// <remarks>Must be done before calling 
        /// calculate as the parameters influence the calculation outcome.</remarks>
        IList<IParameterReact> ParameterList { get; set; }

        /**
         * Initiates the process for the given Reaction.
         *
         * Optionally, parameters may be set which can affect the course of the process.
         *
         *
         * @param reactants   An {@link IAtomContainerSet} for which this process should be initiate.
         * @param agents      An {@link IAtomContainerSet} for which this process should be initiate.
         * @return the set of reactions.
         * @ if an error occurs during the reaction process. See documentation for individual reaction processes
         */
        IReactionSet Initiate(IAtomContainerSet<IAtomContainer> reactants, IAtomContainerSet<IAtomContainer> agents);

        /**
         * Return the IParameterReact if it exists given the class.
         *
         * @param paramClass The class
         * @return           The IParameterReact
         */
        IParameterReact GetParameterClass(System.Type paramClass);
    }
}
