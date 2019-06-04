/* Copyright (C) 2008  Miguel Rojas <miguelrojasch@users.sf.net>
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Dict;
using NCDK.IO;
using NCDK.Reactions.Types.Parameters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NCDK.Reactions
{
    /// <summary>
    /// Tests for IReactionProcess implementations.
    /// </summary>
    // @cdk.module test-reaction
    [TestClass()]
    public abstract class ReactionProcessTest : CDKTestCase
    {
        private IReactionProcess reaction;
        private EntryDictionary dictionary;
        private string entryString = "";
        private IChemObjectBuilder builder = CDK.Builder;

        /// <summary>
        /// Set the IReactionProcess to analyzed
        /// </summary>
        /// <param name="reactionClass">The IReactionProcess class</param>
        // @throws Exception
        public void SetReaction(Type reactionClass)
        {
            if (dictionary == null)
                dictionary = OpeningDictionary();

            var obj = reactionClass.GetConstructor(Type.EmptyTypes).Invoke(Array.Empty<object>());

            if (!(obj is IReactionProcess))
            {
                throw new CDKException("The passed reaction class must be a IReactionProcess");
            }
            else if (reaction == null)
            {
                reaction = (IReactionProcess)obj;

                entryString = reaction.Specification.SpecificationReference;
                entryString = entryString.Substring(entryString.IndexOf("#") + 1);
            }
        }

        /// <summary>
        /// Open the Dictionary OWLReact.
        /// </summary>
        /// <returns>The dictionary reaction-processes</returns>
        private EntryDictionary OpeningDictionary()
        {
            var db = new DictionaryDatabase();
            var dict = db.GetDictionary("reaction-processes");
            return dict;
        }

        /// <summary>
        /// Makes sure that the extending class has set the super.descriptor.
        /// </summary>
        /// <example>
        /// Each extending class should have this bit of code:
        /// <code>
        /// public void SetUp() {
        ///   // Pass a Class, not an Object!
        ///   SetReaction(typeof(SomeReaction));
        /// }
        /// </code>
        /// The unit tests in the extending class may use this instance, but
        /// are not required.
        /// </example>
        [TestMethod()]
        public void TestHasSetSuperDotDescriptor()
        {
            Assert.IsNotNull(reaction, "The extending class must set the super.descriptor in its SetUp() method.");
        }

        /// <summary>
        /// Test if the reaction process is contained in the Dictionary as a entry.
        /// </summary>
        [TestMethod()]
        public void TestGetEntryFromReaction()
        {
            entryString = reaction.Specification.SpecificationReference;
            entryString = entryString.Substring(entryString.IndexOf("#") + 1);

            Assert.AreNotEqual("nothing", entryString, "The Entry ID from  [" + reaction.GetType().ToString() + "] doesn't exist.");
        }

        /// <summary>
        /// Test if the reaction process is contained in the Dictionary as a entry.
        /// </summary>
        [TestMethod()]
        public void TestGetDictionaryEntry()
        {
            var entry = (EntryReact)dictionary[entryString.ToLowerInvariant()];
            Assert.IsNotNull(entry, "The Entry [" + entryString + "] doesn't exist in OWL Dictionary.");
        }

        /// <summary>
        /// Test if this entry has a definition schema in the Dictionary.
        /// </summary>
        [TestMethod()]
        public void TestGetEntryDefinition()
        {
            var entry = (EntryReact)dictionary[entryString.ToLowerInvariant()];
            Assert.IsNotNull(entry.Definition, "The definition entry for [" + entryString + "] must not be null.");
        }

        /// <summary>
        /// Checks if the parameterization key is consistent with those coming from the dictionary.
        /// </summary>
        [TestMethod()]
        public void TestGetParameterList()
        {
            var paramObj = reaction.ParameterList;

            var entry = (EntryReact)dictionary[entryString.ToLowerInvariant()];
            var paramDic = entry.ParameterClass;

            Assert.IsNotNull(paramObj, "The parameters entry for [" + entryString + "]  must contain at least one parameter.");
            Assert.IsNotNull(paramDic, "The parameters entry for [" + entryString + "]  must contain at least one parameter.");
            Assert.AreEqual(paramObj.Count, paramDic.Count, "The parameters entry for [" + entryString
                    + "]  must contain the same lenght as the reaction object.");
        }

        /// <summary>
        /// Test the specification of the IReactionProcess.
        /// </summary>
        [TestMethod()]
        public void TestGetSpecification()
        {
            var spec = reaction.Specification;
            Assert.IsNotNull(spec, "The descriptor specification returned must not be null.");

            Assert.IsNotNull(spec.ImplementationIdentifier, "The specification identifier must not be null.");
            Assert.AreNotSame(0, spec.ImplementationIdentifier.Length, "The specification identifier must not be empty.");

            Assert.IsNotNull(spec.ImplementationTitle, "The specification title must not be null.");
            Assert.AreNotSame(0, spec.ImplementationTitle.Length, "The specification title must not be empty.");

            Assert.IsNotNull(spec.ImplementationVendor, "The specification vendor must not be null.");
            Assert.AreNotSame(0, spec.ImplementationVendor.Length, "The specification vendor must not be empty.");

            Assert.IsNotNull(spec.SpecificationReference, "The specification reference must not be null.");
            Assert.AreNotSame(0, spec.SpecificationReference.Length, "The specification reference must not be empty.");
        }

        /// <summary>
        /// Test if this entry has a definition schema in the Dictionary.
        /// </summary>
        [TestMethod()]
        public void TestGetEntryDescription()
        {

            var entry = (EntryReact)dictionary[entryString.ToLowerInvariant()];

            Assert.IsNotNull(entry.Description, "The description entry for [" + entryString + "] must not be null.");
        }

        /// <summary>
        /// Test if this entry has at least one representation schema in the Dictionary.
        /// </summary>
        [TestMethod()]
        public void TestGetEntryRepresentation()
        {
            var entry = (EntryReact)dictionary[entryString.ToLowerInvariant()];

            Assert.AreNotSame(0, entry.Representations.Count, "The representation entry for [" + entryString
                    + "]  must contain at least one representation.");
        }

        /// <summary>
        /// Test reactive center parameter
        /// </summary>
        [TestMethod()]
        public void TestCentreActive()
        {
            var type = reaction;
            var ipr = type.GetParameterClass(typeof(SetReactionCenter));
            Assert.IsNotNull(ipr);
            Assert.IsFalse(ipr.IsSetParameter);

            var paramList = new List<IParameterReaction>();
            var param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;

            var ipr2 = type.GetParameterClass(typeof(SetReactionCenter));
            Assert.IsTrue(ipr2.IsSetParameter);
        }

        /// <summary>
        /// Test extracting a reaction as example.
        ///
        /// TODO: REACT: One example for each reaction should be set in owl dictionary.
        /// </summary>
        [TestMethod()]
        public void TestGetExampleReaction()
        {
            //        var entry = (EntryReact) dictionary.GetEntry(entryString.ToLowerInvariant());
            //        var xmlList = entry.ExampleReactions;
            //        Assert.IsTrue("The representation entry for ["+entryString+"]  must contain at least one example of reaction.",
            //                xmlList.Count != 0);
            //        Assert.IsTrue("The representation entry for ["+entryString+"]  must contain at least one example of reaction.",
            //                xmlList.Count > 0);
            //        For(Iterator<string> it = xmlList.Iterator(); it.HasNext();){
            //            string xml = it.Next();
            //            var reader = new CMLReader(new MemoryStream(xml.GetBytes()));
            //            var chemFile = (IChemFile)reader.Read(builder.NewChemFile());
            //            var reactionDict = chemFile[0][0].ReactionSet[0];
            //            For(Iterator<IAtomContainer> itM = reactionDict.Reactants.Molecules().Iterator(); itM.HasNext();){
            //                IAtomContainer molecule = (IAtomContainer) itM.Next();
            //                Assert.IsNotNull("The representation entry for ["+entryString+"]  must contain the InChI id for each reactant.",
            //                        molecule.GetProperty<>(CDKPropertyName.INCHI));
            //                Assert.AreNotSame("The representation entry for ["+entryString+"]  must contain the InChI id for each reactant.",
            //                        "",molecule.GetProperty<>(CDKPropertyName.INCHI));
            //
            //            }
            //        }
        }

        /// <summary>
        /// Test extracting a reaction as example and comparing with the initiated.
        ///
        /// TODO: REACT: How to comparing two reaction?
        /// </summary>
        [TestMethod()]
        public virtual void TestInitiate_IAtomContainerSet_IAtomContainerSet()
        {
            var entry = (EntryReact)dictionary[entryString.ToLowerInvariant()];
            var xmlList = entry.ExampleReactions;
            Assert.IsTrue(xmlList.Count != 0, "The representation entry for [" + entryString
                    + "]  must contain at least one example of reaction.");
            Assert.IsTrue(xmlList.Count > 0, "The representation entry for [" + entryString
                    + "]  must contain at least one example of reaction.");
            foreach (var xml in xmlList)
            {
                var reader = new CMLReader(new MemoryStream(Encoding.UTF8.GetBytes(xml)));
                var chemFile = (IChemFile)reader.Read(builder.NewChemFile());
                var reactionDict = chemFile[0][0].ReactionSet[0];

                var reactants = reactionDict.Reactants;
                var agents = reactionDict.Agents;
                var products = reactionDict.Products;
                if (agents.Count == 0)
                    agents = null;

                var reactions = reaction.Initiate(reactants, agents);

                Assert.IsTrue(reactions.Count > 0, "The products for [" + entryString + "] reaction is at least one reaction expected.");

                Assert.AreEqual(products[0].Atoms.Count,
                    reactions[0].Products[0].Atoms.Count,
                    "The products for [" + entryString + "] reaction is not the expected.");
            }
        }        
    }
}
