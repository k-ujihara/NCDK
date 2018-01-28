using NCDK.Default;
using System;

namespace NCDK
{
    class Property_Example
    {
        static void Main(string[] args)
        {
            #region
            IAtom atom = new Atom("C");
            atom.SetProperty("number", 1); // set an integer property
            {
                // access the property and cast to an int
                int number = atom.GetProperty<int>("number");
            }
            {
                // the type cannot be checked and so...
                try
                {
                    string number = atom.GetProperty<string>("number");
                }
                catch (InvalidCastException)
                {
                    Console.WriteLine($"{nameof(InvalidCastException)} is thrown");
                }
            }
            #endregion
        }
    }
}
