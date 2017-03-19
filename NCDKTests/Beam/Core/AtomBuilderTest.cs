using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static NCDK.Beam.Element;

namespace NCDK.Beam
{
   /// <summary> <author>John May </author></summary>
    [TestClass()]
    public class AtomBuilderTest
    {
        [TestMethod()]
        public void Aliphatic_element_c()
        {
            Atom a = AtomBuilder.Aliphatic(Element.Carbon)
                                .Build();
            Assert.AreEqual(a.Element, Element.Carbon);
            Assert.AreEqual(a.Isotope, -1);
            Assert.AreEqual(a.Charge, 0);
            Assert.AreEqual(a.AtomClass, 0);
            Assert.AreEqual(a.IsAromatic(), false);
        }

        [TestMethod()]
        public void Aliphatic_element_n()
        {
            Atom a = AtomBuilder.Aliphatic(Element.Nitrogen)
                                .Build();
            Assert.AreEqual(a.Element, Element.Nitrogen);
            Assert.AreEqual(a.Isotope, -1);
            Assert.AreEqual(a.Charge, 0);
            Assert.AreEqual(a.AtomClass, 0);
            Assert.AreEqual(a.IsAromatic(), false);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Aliphatic_element_null()
        {
            Atom a = AtomBuilder.Aliphatic((Element)null)
                                .Build();
        }

        [TestMethod()]
        public void IsAromatic_element_c()
        {
            Atom a = AtomBuilder.Aromatic(Carbon)
                                .Build();
            Assert.AreEqual(a.Element, Element.Carbon);
            Assert.AreEqual(a.Isotope, -1);
            Assert.AreEqual(a.Charge, 0);
            Assert.AreEqual(a.AtomClass, 0);
            Assert.AreEqual(a.IsAromatic(), true);
        }

        [TestMethod()]
        public void IsAromatic_element_n()
        {
            Atom a = AtomBuilder.Aromatic(Nitrogen)
                                .Build();
            Assert.AreEqual(a.Element, Element.Nitrogen);
            Assert.AreEqual(a.Isotope, -1);
            Assert.AreEqual(a.Charge, 0);
            Assert.AreEqual(a.AtomClass, 0);
            Assert.AreEqual(a.IsAromatic(), true);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void IsAromatic_element_cl()
        {
            Atom a = AtomBuilder.Aromatic(Chlorine)
                                .Build();
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsAromatic_element_null()
        {
            Atom a = AtomBuilder.Aromatic((Element)null)
                                .Build();
        }

        [TestMethod()]
        public void Aliphatic_symbol_c()
        {
            Atom a = AtomBuilder.Aliphatic("C")
                                .Build();
            Assert.AreEqual(a.Element, Element.Carbon);
            Assert.AreEqual(a.Isotope, -1);
            Assert.AreEqual(a.Charge, 0);
            Assert.AreEqual(a.AtomClass, 0);
            Assert.AreEqual(a.IsAromatic(), false);
        }

        [TestMethod()]
        public void Aliphatic_symbol_n()
        {
            Atom a = AtomBuilder.Aliphatic("N")
                                .Build();
            Assert.AreEqual(a.Element, Element.Nitrogen);
            Assert.AreEqual(a.Isotope, -1);
            Assert.AreEqual(a.Charge, 0);
            Assert.AreEqual(a.AtomClass, 0);
            Assert.AreEqual(a.IsAromatic(), false);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Aliphatic_symbol_null()
        {
            Atom a = AtomBuilder.Aliphatic((string)null)
                                .Build();
        }

        [TestMethod()]
        public void IsAromatic_symbol_c()
        {
            Atom a = AtomBuilder.Aromatic("C")
                                .Build();
            Assert.AreEqual(a.Element, Element.Carbon);
            Assert.AreEqual(a.Isotope, -1);
            Assert.AreEqual(a.Charge, 0);
            Assert.AreEqual(a.AtomClass, 0);
            Assert.AreEqual(a.IsAromatic(), true);
        }

        [TestMethod()]
        public void IsAromatic_symbol_n()
        {
            Atom a = AtomBuilder.Aromatic("N")
                                .Build();
            Assert.AreEqual(a.Element, Element.Nitrogen);
            Assert.AreEqual(a.Isotope, -1);
            Assert.AreEqual(a.Charge, 0);
            Assert.AreEqual(a.AtomClass, 0);
            Assert.AreEqual(a.IsAromatic(), true);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void IsAromatic_symbol_cl()
        {
            Atom a = AtomBuilder.Aromatic("Cl")
                                .Build();
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsAromatic_symbol_null()
        {
            Atom a = AtomBuilder.Aromatic((string)null)
                                .Build();
        }

        [TestMethod()]
        public void Create_symbol_aliphatic_c()
        {
            Atom a = AtomBuilder.Create("C")
                                .Build();
            Assert.AreEqual(a.Element, Element.Carbon);
            Assert.AreEqual(a.Isotope, -1);
            Assert.AreEqual(a.Charge, 0);
            Assert.AreEqual(a.AtomClass, 0);
            Assert.AreEqual(a.IsAromatic(), false);
        }

        [TestMethod()]
        public void Create_symbol_IsAromatic_c()
        {
            Atom a = AtomBuilder.Create("c")
                                .Build();
            Assert.AreEqual(a.Element, Element.Carbon);
            Assert.AreEqual(a.Isotope, -1);
            Assert.AreEqual(a.Charge, 0);
            Assert.AreEqual(a.AtomClass, 0);
            Assert.AreEqual(a.IsAromatic(), true);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void Create_symbol_non_IsAromatic()
        {
            Atom a = AtomBuilder.Create("cl")
                                .Build();
        }

        [TestMethod()]
        public void Create_symbol_defaultToUnknown()
        {
            Atom a = AtomBuilder.Create("N/A")
                                .Build();
            Assert.AreEqual(a.Element, Element.Unknown);
        }

        [TestMethod()]
        public void Create_symbol_null()
        {
            Atom a = AtomBuilder.Create((string)null)
                                .Build();
            Assert.AreEqual(a.Element, Element.Unknown);
        }

        [TestMethod()]
        public void Aliphatic_charged_carbon_minus2()
        {
            Atom a = AtomBuilder.Aliphatic(Element.Carbon)
                                .Charge(-2)
                                .Build();
            Assert.AreEqual(a.Element, Element.Carbon);
            Assert.AreEqual(a.Charge, -2);
        }

        [TestMethod()]
        public void Aliphatic_charged_carbon_plus2()
        {
            Atom a = AtomBuilder.Aliphatic(Element.Carbon)
                                .Charge(+2)
                                .Build();
            Assert.AreEqual(a.Element, Element.Carbon);
            Assert.AreEqual(a.Charge, +2);
        }

        [TestMethod()]
        public void Aliphatic_charged_carbon_anion()
        {
            Atom a = AtomBuilder.Aliphatic(Element.Carbon)
                                .Anion
                                .Build();
            Assert.AreEqual(a.Element, Element.Carbon);
            Assert.AreEqual(a.Charge, -1);
        }

        [TestMethod()]
        public void Aliphatic_charged_carbon_plus1()
        {
            Atom a = AtomBuilder.Aliphatic(Element.Carbon)
                                .Cation
                                .Build();
            Assert.AreEqual(a.Element, Element.Carbon);
            Assert.AreEqual(a.Charge, +1);
        }

        [TestMethod()]
        public void Aliphatic_carbon_13()
        {
            Atom a = AtomBuilder.Aliphatic(Element.Carbon)
                                .Isotope(13)
                                .Build();
            Assert.AreEqual(a.Element, Element.Carbon);
            Assert.AreEqual(a.Isotope, 13);
        }

        [TestMethod()]
        public void Aliphatic_carbon_14()
        {
            Atom a = AtomBuilder.Aliphatic(Element.Carbon)
                                .Isotope(13)
                                .Build();
            Assert.AreEqual(a.Element, Element.Carbon);
            Assert.AreEqual(a.Isotope, 13);
        }

        [TestMethod()]
        public void Aliphatic_carbon_class1()
        {
            Atom a = AtomBuilder.Aliphatic(Element.Carbon)
                                .AtomClass(1)
                                .Build();
            Assert.AreEqual(a.Element, Element.Carbon);
            Assert.AreEqual(a.AtomClass, 1);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Aliphatic_carbon_class_negative()
        {
            Atom a = AtomBuilder.Aliphatic(Element.Carbon)
                                .AtomClass(-10)
                                .Build();
        }

        [TestMethod()]
        public void Aliphatic_carbon_3_hydrogens()
        {
            Atom a = AtomBuilder.Aliphatic(Element.Carbon)
                                .NumOfHydrogens(3)
                                .Build();
            Assert.AreEqual(a.Element, Element.Carbon);
            Assert.AreEqual(a.NumOfHydrogens, 3);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Aliphatic_carbon_negative_hydrogens()
        {
            Atom a = AtomBuilder.Aliphatic(Element.Carbon)
                                .NumOfHydrogens(-3)
                                .Build();
        }

        [TestMethod()]
        public void IsAromatic_Unknown_from_element()
        {
            Assert.IsNotNull(AtomBuilder.Aromatic(Unknown)
                                     .Build());
        }

        [TestMethod()]
        public void IsAromatic_Unknown_from_symbol()
        {
            Assert.IsNotNull(AtomBuilder.Aromatic("*")
                                     .Build());
            Assert.IsNotNull(AtomBuilder.Aromatic("R")
                                     .Build());
        }
    }
}
