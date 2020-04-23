using System;
using NCDK;
using static NCDK.CDK;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NCDK.IO;

namespace NCDK
{
    class Issue20
    {
        static void Main(string[] args)
        {
            var container = Builder.NewAtomContainer();
            container.Add(Builder.NewAtom("C"));
            container.Add(Builder.NewAtom("C"));
            container.Add(Builder.NewBond(container.Atoms[0], container.Atoms[1], BondOrder.Unset));

            using (var file = new FileStream("./cml.cml", FileMode.Create, FileAccess.Write))
            {
                using (var writer = new CMLWriter(file))
                {
                    writer.Write(container);
                }
            }

            using (var cmlFile = new FileStream("./cml.cml", FileMode.Open, FileAccess.Read))
            {
                var reader = new CMLReader(cmlFile);
                var chemFile = reader.Read(Builder.NewChemFile());
                Console.WriteLine(chemFile[0][0].MoleculeSet[0].Bonds[0].Order);
            }
        }
    }
}
