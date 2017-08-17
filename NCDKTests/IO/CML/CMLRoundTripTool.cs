/* Copyright (C) 2003-2008  The Chemistry Development Kit (CDK) project
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
 *  */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using NCDK.LibIO.CML;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace NCDK.IO.CML
{
    /// <summary>
    /// Helper tool for round tripping CDK classes via CML.
    /// </summary>
    // @cdk.module  test-libiocml
    public class CMLRoundTripTool : CDKTestCase
    {
        /// <summary>
        /// Convert a Molecule to CML and back to a Molecule again.
        /// Given that CML reading is working, the problem is with the CMLWriter.
        /// </summary>
        /// <param name="convertor"></param>
        /// <param name="mol"></param>
        /// <returns></returns>
        /// <seealso cref="CMLFragmentsTest"/>
        public static IAtomContainer RoundTripMolecule(Convertor convertor, IAtomContainer mol)
        {
            string cmlString = "<!-- failed -->";
            var cmlDOM = convertor.CDKAtomContainerToCMLMolecule(mol);
            cmlString = cmlDOM.ToString();

            Debug.WriteLine("CML string: ", cmlString);
            IChemFile file;
            using (var reader = new CMLReader(new MemoryStream(Encoding.UTF8.GetBytes(cmlString))))
            {
                file = (IChemFile)reader.Read(new ChemFile());
            }
            Assert.IsNotNull(file);
            Assert.AreEqual(1, file.Count);
            IChemSequence sequence = file[0];
            Assert.IsNotNull(sequence);
            Assert.AreEqual(1, sequence.Count);
            IChemModel chemModel = sequence[0];
            Assert.IsNotNull(chemModel);
            var moleculeSet = chemModel.MoleculeSet;
            Assert.IsNotNull(moleculeSet);
            Assert.AreEqual(1, moleculeSet.Count);
            var roundTrippedMol = moleculeSet[0];
            Assert.IsNotNull(roundTrippedMol);

            return roundTrippedMol;
        }

        public static IChemModel RoundTripChemModel(Convertor convertor, IChemModel model)
        {
            string cmlString = "<!-- failed -->";
            var cmlDOM = convertor.CDKChemModelToCMLList(model);
            cmlString = cmlDOM.ToString();

            Debug.WriteLine("CML string: ", cmlString);
            IChemFile file;
            using (var reader = new CMLReader(new MemoryStream(Encoding.UTF8.GetBytes(cmlString))))
            {
                file = (IChemFile)reader.Read(model.Builder.NewChemFile());
            }
            Assert.IsNotNull(file);
            Assert.AreEqual(1, file.Count);
            IChemSequence sequence = file[0];
            Assert.IsNotNull(sequence);
            Assert.AreEqual(1, sequence.Count);
            IChemModel chemModel = sequence[0];
            Assert.IsNotNull(chemModel);

            return chemModel;
        }

        public static IReaction RoundTripReaction(Convertor convertor, IReaction reaction)
        {
            string cmlString = "<!-- failed -->";
            var cmlDOM = convertor.CDKReactionToCMLReaction(reaction);
            cmlString = cmlDOM.ToString();

            IReaction roundTrippedReaction = null;
            Debug.WriteLine("CML string: ", cmlString);
            CMLReader reader = new CMLReader(new MemoryStream(Encoding.UTF8.GetBytes(cmlString)));

            IChemFile file = (IChemFile)reader.Read(new ChemFile());
            reader.Close();
            Assert.IsNotNull(file);
            Assert.AreEqual(1, file.Count);
            IChemSequence sequence = file[0];
            Assert.IsNotNull(sequence);
            Assert.AreEqual(1, sequence.Count);
            IChemModel chemModel = sequence[0];
            Assert.IsNotNull(chemModel);
            IReactionSet reactionSet = chemModel.ReactionSet;
            Assert.IsNotNull(reactionSet);
            Assert.AreEqual(1, reactionSet.Count);
            roundTrippedReaction = reactionSet[0];
            Assert.IsNotNull(roundTrippedReaction);

            return roundTrippedReaction;
        }
    }
}
