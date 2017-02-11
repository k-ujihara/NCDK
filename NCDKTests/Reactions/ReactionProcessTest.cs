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
    /**
     * Tests for IReactionProcess implementations.
     *
     * @cdk.module test-reaction
     */
    [TestClass()]
    public abstract class ReactionProcessTest : CDKTestCase
    {

        private IReactionProcess reaction;
        private DictionaryMap dictionary;
        private string entryString = "";
        private IChemObjectBuilder builder = Silent.ChemObjectBuilder.Instance;

        /**
         * Set the IReactionProcess to analyzed
         *
         * @param reactionClass   The IReactionProcess class
         * @throws Exception
         */
        public void SetReaction(Type reactionClass)
        {
            if (dictionary == null) dictionary = openingDictionary();

            var obj = reactionClass.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);

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

        /**
         * Open the Dictionary OWLReact.
         *
         * @return The dictionary reaction-processes
         */
        private DictionaryMap openingDictionary()
        {
            DictionaryDatabase db = new DictionaryDatabase();
            DictionaryMap dict = db.GetDictionary("reaction-processes");
            return dict;
        }

        /**
         * Makes sure that the extending class has set the super.descriptor.
         * Each extending class should have this bit of code (JUnit3 formalism):
         * <pre>
         * public void SetUp() {
         *   // Pass a Class, not an Object!
         *   SetReaction(typeof(SomeReaction));
         * }
         *
         * <p>The unit tests in the extending class may use this instance, but
         * are not required.
         *
         * </pre>
         */
        [TestMethod()]
        public void TestHasSetSuperDotDescriptor()
        {
            Assert.IsNotNull(reaction, "The extending class must set the super.descriptor in its SetUp() method.");
        }

        /**
         * Test if the reaction process is contained in the Dictionary as a entry.
         *
         * @throws Exception
         */
        [TestMethod()]
        public void TestGetEntryFromReaction()
        {

            entryString = reaction.Specification.SpecificationReference;
            entryString = entryString.Substring(entryString.IndexOf("#") + 1);

            Assert.AreNotEqual("nothing", entryString, "The Entry ID from  [" + reaction.GetType().ToString() + "] doesn't exist.");
        }

        /**
         * Test if the reaction process is contained in the Dictionary as a entry.
         *
         * @throws Exception
         */
        [TestMethod()]
        public void TestGetDictionaryEntry()
        {

            EntryReact entry = (EntryReact)dictionary.GetEntry(entryString.ToLowerInvariant());
            Assert.IsNotNull(entry, "The Entry [" + entryString + "] doesn't exist in OWL Dictionary.");

        }

        /**
         * Test if this entry has a definition schema in the Dictionary.
         *
         * @throws Exception
         */
        [TestMethod()]
        public void TestGetEntryDefinition()
        {

            EntryReact entry = (EntryReact)dictionary.GetEntry(entryString.ToLowerInvariant());

            Assert.IsNotNull(entry.Definition, "The definition entry for [" + entryString + "] must not be null.");

        }

        /**
         * Checks if the parameterization key is consistent with those coming from the dictionary.
         *
         * @throws Exception
         */
        [TestMethod()]
        public void TestGetParameterList()
        {
            var paramObj = reaction.ParameterList;

            EntryReact entry = (EntryReact)dictionary.GetEntry(entryString.ToLowerInvariant());
            var paramDic = entry.ParameterClass;

            Assert.IsNotNull(paramObj, "The parameters entry for [" + entryString + "]  must contain at least one parameter.");
            Assert.IsNotNull(paramDic, "The parameters entry for [" + entryString + "]  must contain at least one parameter.");
            Assert.AreEqual(paramObj.Count, paramDic.Count, "The parameters entry for [" + entryString
                    + "]  must contain the same lenght as the reaction object.");
        }

        /**
         * Test the specification of the IReactionProcess.
         *
         */
        [TestMethod()]
        public void TestGetSpecification()
        {
            ReactionSpecification spec = reaction.Specification;
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

        /**
         * Test if this entry has a definition schema in the Dictionary.
         *
         * @throws Exception
         */
        [TestMethod()]
        public void TestGetEntryDescription()
        {

            EntryReact entry = (EntryReact)dictionary.GetEntry(entryString.ToLowerInvariant());

            Assert.IsNotNull(entry.Description, "The description entry for [" + entryString + "] must not be null.");
        }

        /**
         * Test if this entry has at least one representation schema in the Dictionary.
         *
         * @throws Exception
         */
        [TestMethod()]
        public void TestGetEntryRepresentation()
        {

            EntryReact entry = (EntryReact)dictionary.GetEntry(entryString.ToLowerInvariant());

            Assert.AreNotSame(0, entry.Representations.Count, "The representation entry for [" + entryString
                    + "]  must contain at least one representation.");
        }

        /**
         * Test reactive center parameter
         *
         * @return    The test suite
         */
        [TestMethod()]
        public void TestCentreActive()
        {
            IReactionProcess type = reaction;

            IParameterReact ipr = type.GetParameterClass(typeof(SetReactionCenter));
            Assert.IsNotNull(ipr);
            Assert.IsFalse(ipr.IsSetParameter);

            List<IParameterReact> paramList = new List<IParameterReact>();
            IParameterReact param = new SetReactionCenter();
            param.IsSetParameter = true;
            paramList.Add(param);
            type.ParameterList = paramList;

            IParameterReact ipr2 = type.GetParameterClass(typeof(SetReactionCenter));
            Assert.IsTrue(ipr2.IsSetParameter);
        }

        /**
         * Test extracting a reaction as example.
         *
         * TODO: REACT: One example for each reaction should be set in owl dictionary.
         * @return    The test suite
         */
        [TestMethod()]
        public void TestGetExampleReaction()
        {
            //		EntryReact entry = (EntryReact) dictionary.GetEntry(entryString.ToLowerInvariant());
            //    	var xmlList = entry.ExampleReactions;
            //    	Assert.IsTrue("The representation entry for ["+entryString+"]  must contain at least one example of reaction.",
            //    			xmlList.Count != 0);
            //    	Assert.IsTrue("The representation entry for ["+entryString+"]  must contain at least one example of reaction.",
            //    			xmlList.Count > 0);
            //    	For(Iterator<string> it = xmlList.iterator(); it.HasNext();){
            //			string xml = it.Next();
            //			CMLReader reader = new CMLReader(new MemoryStream(xml.GetBytes()));
            //	        IChemFile chemFile = (IChemFile)reader.Read(builder.CreateChemFile());
            //	        IReaction reactionDict = chemFile[0][0].ReactionSet[0];
            //	        For(Iterator<IAtomContainer> itM = reactionDict.Reactants.Molecules().iterator(); itM.HasNext();){
            //	        	IAtomContainer molecule = (IAtomContainer) itM.Next();
            //	        	Assert.IsNotNull("The representation entry for ["+entryString+"]  must contain the InChI id for each reactant.",
            //	        			molecule.GetProperty<>(CDKPropertyName.INCHI));
            //	        	Assert.AreNotSame("The representation entry for ["+entryString+"]  must contain the InChI id for each reactant.",
            //	        			"",molecule.GetProperty<>(CDKPropertyName.INCHI));
            //
            //	        }
            //    	}
        }

        /**
         * Test extracting a reaction as example and comparing with the initiated.
         *
         * TODO: REACT: How to comparing two reaction?
         *
         * @return    The test suite
         */
        [TestMethod()]
        public virtual void TestInitiate_IAtomContainerSet_IAtomContainerSet()
        {
            EntryReact entry = (EntryReact)dictionary.GetEntry(entryString.ToLowerInvariant());
            var xmlList = entry.ExampleReactions;
            Assert.IsTrue(xmlList.Count != 0, "The representation entry for [" + entryString
                    + "]  must contain at least one example of reaction.");
            Assert.IsTrue(xmlList.Count > 0, "The representation entry for [" + entryString
                    + "]  must contain at least one example of reaction.");
            foreach (var xml in xmlList)
            {
                CMLReader reader = new CMLReader(new MemoryStream(Encoding.UTF8.GetBytes(xml)));
                IChemFile chemFile = (IChemFile)reader.Read(builder.CreateChemFile());
                IReaction reactionDict = chemFile[0][0].ReactionSet[0];

                var reactants = reactionDict.Reactants;
                var agents = reactionDict.Agents;
                var products = reactionDict.Products;
                if (agents.Count == 0) agents = null;

                IReactionSet reactions = reaction.Initiate(reactants, agents);

                Assert.IsTrue(reactions.Count > 0, "The products for [" + entryString + "] reaction is at least one reaction expected.");

                Assert.AreEqual(products[0].Atoms.Count,
                    reactions[0].Products[0].Atoms.Count,
                    "The products for [" + entryString + "] reaction is not the expected.");
            }
        }
        //	/**
        //	 * Test the reaction center
        //	 *
        //	 * @return    The test suite
        //	 */
        //	[TestMethod()] public void TestCDKConstants_REACTIVE_CENTER() {
        //	}
        //
        //	/**
        //	 * Test mapping in reaction process.
        //	 *
        //	 * @return    The test suite
        //	 */
        //	[TestMethod()] public void TestMapping() {
        //
        //
        //	}
        //
        //	/**
        //	 * Set reaction center and generates the product.
        //	 *
        //	 * @return    The test suite
        //	 */
        //	[TestMethod()] public void TestManuallyCentreActive() {
        //
        //	}
        //
        //	/**
        //	 * Automatically looks for reaction center and generates the product.
        //	 *
        //	 * @return    The test suite
        //	 */
        //	[TestMethod()] public void TestAutomaticallyCentreActive() {
        //
        //	}
        //
        //
        //	/**
        //	 * Control that the reactant is the not modified during the process.
        //	 *
        //	 * @return    The test suite
        //	 */
        //	[TestMethod()] public void TestCalculate_Results() {
        //		EntryReact entry = (EntryReact) dictionary.GetEntry(entryString.ToLowerInvariant());
        //    	var xmlList = entry.ExampleReactions;
        //    	For(Iterator<string> it = xmlList.iterator(); it.HasNext();){
        //			string xml = it.Next();
        //			Console.Out.WriteLine(xml);
        //			CMLReader reader = new CMLReader(new MemoryStream(xml.GetBytes()));
        //	        IChemFile chemFile = (IChemFile)reader.Read(builder.CreateChemFile());
        //	        IReaction reactionDict = chemFile[0][0].ReactionSet[0];
        //
        //	        IReaction reactionTest = builder.CreateReaction();
        //	        For(Iterator<IAtomContainer> itM = reactionDict.Reactants.Molecules(); itM.HasNext();){
        //	        	reactionTest.AddReactant((IAtomContainer) itM.Next());
        //	        }
        //	        For(Iterator<IAtomContainer> itM = reactionDict.Agents.Molecules(); itM.HasNext();){
        //	        	reactionTest.AddAgent((IAtomContainer) itM.Next());
        //	        }
        //	        IAtomContainerSet reactants = reactionDict.Reactants;
        //	        Console.Out.WriteLine(reactants);
        //	        if(reactants.Count == 0)
        //	        	reactants = null;
        //	        IAtomContainerSet agents = reactionDict.Agents;
        //	        if(agents.Count == 0)
        //	        	agents = null;
        //	        Console.Out.WriteLine(agents);
        //	        IReactionSet setOfReactions = reaction.Initiate(reactants, agents);
        //
        //
        //
        //		}
        //	}
        //
        //	/**
        //	 * Control that the reactant is the not modified during the process.
        //	 *
        //	 * @return    The test suite
        //	 */
        //	[TestMethod()] public void TestGetMechanism() {
        //		EntryReact entry = (EntryReact) dictionary.GetEntry(entryString.ToLowerInvariant());
        //
        //		string mechanismName = "NCDK.Reactions.mechanism."+entry.GetMechanism();
        //
        //		Assert.IsNotNull(
        //    			"The representation entry for ["+entryString+"]  must contain at least one mechanism coming from.",
        //    			this.GetClass().GetClassLoader().LoadClass(mechanismName).NewInstance());
        //
        //	}
    }
}
