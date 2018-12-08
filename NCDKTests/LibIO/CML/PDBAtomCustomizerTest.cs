using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.IO;
using System.Diagnostics;
using System.IO;

namespace NCDK.LibIO.CML
{
    // @author John May
    // @cdk.module test-pdbcml
    [TestClass()]
    public class PDBAtomCustomizerTest
    {
        private readonly IChemObjectBuilder builder = CDK.Builder;

        [TestMethod()]
        public void TestPDBAtomCustomization()
        {
            StringWriter writer = new StringWriter();
            var molecule = builder.NewAtomContainer();
            IPDBAtom atom = builder.NewPDBAtom("C");
            atom.Name = "CA";
            atom.ResName = "PHE";
            molecule.Atoms.Add(atom);

            CMLWriter cmlWriter = new CMLWriter(writer);
            cmlWriter.RegisterCustomizer(new PDBAtomCustomizer());
            cmlWriter.Write(molecule);
            cmlWriter.Close();
            string cmlContent = writer.ToString();
            Debug.WriteLine("****************************** TestPDBAtomCustomization()");
            Debug.WriteLine(cmlContent);
            Debug.WriteLine("******************************");
            Assert.IsTrue(cmlContent.IndexOf("<scalar dictRef=\"pdb:resName") != -1);
        }
    }
}
