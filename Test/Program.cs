using NCDK;
using NCDK.IO;
using NCDK.Smiles;
using System;
using System.IO;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //var mol = CDK.SmilesParser.ParseSmiles("**.c1ccccc1CC |$R'$|");
            var mol = CDK.SmilesParser.ParseSmiles("Cl[*](Br)I |$;_R1;;$,LO:1:0.2.3|");
            //var smigen = new SmilesGenerator(SmiFlavors.CxSmiles);
            ///*Assert.AreEqual("Cl*(Br)I |$;R1$,LO:1:0.2.3|", */smigen.Create(mol));
            //Console.WriteLine(smigen.Create(mol));

            //using (var smigen = new SMILESWriter(new StringWriter()))
            //{
            //    smigen.SetFlavor(SmiFlavors.CxSmiles);
            //    smigen.IOSettings.
            //    smigen.SetWriteTitle(false);
            //    smigen.Write(mol1);
            //    smigen.Write(mol2);
            //}
        }
    }
}
