using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NCDK.Default;
using NCDK.Numerics;
using System.Windows.Media;
using WPF = System.Windows;

namespace NCDK.Renderers.Generators.Standards
{
    [TestClass()]
    public class SelectionVisibilityTest
    {
        [TestMethod()]
        public void NoHighlightOrGlow()
        {
            IAtomContainer methyl = new AtomContainer();
            methyl.Atoms.Add(AtomAt("C", new Vector2(0, 0)));
            methyl.Atoms.Add(AtomAt("H", new Vector2(0, 1)));
            methyl.Atoms.Add(AtomAt("H", new Vector2(0, -1)));
            methyl.Atoms.Add(AtomAt("H", new Vector2(1, 0)));
            methyl.Atoms.Add(AtomAt("H", new Vector2(-1, 0)));
            methyl.AddBond(methyl.Atoms[0], methyl.Atoms[1], BondOrder.Single);
            methyl.AddBond(methyl.Atoms[0], methyl.Atoms[2], BondOrder.Single);
            methyl.AddBond(methyl.Atoms[0], methyl.Atoms[3], BondOrder.Single);
            methyl.AddBond(methyl.Atoms[0], methyl.Atoms[4], BondOrder.Single);
            SymbolVisibility visibility = SelectionVisibility.GetAll(SymbolVisibility.IupacRecommendations);
            Assert.IsFalse(visibility.Visible(methyl.Atoms[0], methyl.GetConnectedBonds(methyl.Atoms[0]), new RendererModel()));
        }

        [TestMethod()]
        public void WithHighlight()
        {
            IAtomContainer methyl = new AtomContainer();
            methyl.Atoms.Add(AtomAt("C", new Vector2(0, 0)));
            methyl.Atoms.Add(AtomAt("H", new Vector2(0, 1)));
            methyl.Atoms.Add(AtomAt("H", new Vector2(0, -1)));
            methyl.Atoms.Add(AtomAt("H", new Vector2(1, 0)));
            methyl.Atoms.Add(AtomAt("H", new Vector2(-1, 0)));
            methyl.Atoms[0].SetProperty(StandardGenerator.HighlightColorKey, WPF::Media.Colors.Red);
            methyl.AddBond(methyl.Atoms[0], methyl.Atoms[1], BondOrder.Single);
            methyl.AddBond(methyl.Atoms[0], methyl.Atoms[2], BondOrder.Single);
            methyl.AddBond(methyl.Atoms[0], methyl.Atoms[3], BondOrder.Single);
            methyl.AddBond(methyl.Atoms[0], methyl.Atoms[4], BondOrder.Single);
            SymbolVisibility visibility = SelectionVisibility.GetAll(SymbolVisibility.IupacRecommendations);
            Assert.IsTrue(visibility.Visible(methyl.Atoms[0], methyl.GetConnectedBonds(methyl.Atoms[0]),
                    new RendererModel()));
        }

        [TestMethod()]
        public void Isolated()
        {
            IAtomContainer methyl = new AtomContainer();
            methyl.Atoms.Add(AtomAt("C", new Vector2(0, 0)));
            methyl.Atoms.Add(AtomAt("H", new Vector2(0, 1)));
            methyl.Atoms.Add(AtomAt("H", new Vector2(0, -1)));
            methyl.Atoms.Add(AtomAt("H", new Vector2(1, 0)));
            methyl.Atoms.Add(AtomAt("H", new Vector2(-1, 0)));
            methyl.AddBond(methyl.Atoms[0], methyl.Atoms[1], BondOrder.Single);
            methyl.AddBond(methyl.Atoms[0], methyl.Atoms[2], BondOrder.Single);
            methyl.AddBond(methyl.Atoms[0], methyl.Atoms[3], BondOrder.Single);
            methyl.AddBond(methyl.Atoms[0], methyl.Atoms[4], BondOrder.Single);
            methyl.Atoms[0].SetProperty(StandardGenerator.HighlightColorKey, WPF::Media.Colors.Red);
            SymbolVisibility visibility = SelectionVisibility.Disconnected(SymbolVisibility.IupacRecommendations);
            Assert.IsTrue(visibility.Visible(methyl.Atoms[0], methyl.GetConnectedBonds(methyl.Atoms[0]), new RendererModel()));
        }

        [TestMethod()]
        public void UnIsolated()
        {
            IAtomContainer methyl = new AtomContainer();
            methyl.Atoms.Add(AtomAt("C", new Vector2(0, 0)));
            methyl.Atoms.Add(AtomAt("H", new Vector2(0, 1)));
            methyl.Atoms.Add(AtomAt("H", new Vector2(0, -1)));
            methyl.Atoms.Add(AtomAt("H", new Vector2(1, 0)));
            methyl.Atoms.Add(AtomAt("H", new Vector2(-1, 0)));
            methyl.AddBond(methyl.Atoms[0], methyl.Atoms[1], BondOrder.Single);
            methyl.AddBond(methyl.Atoms[0], methyl.Atoms[2], BondOrder.Single);
            methyl.AddBond(methyl.Atoms[0], methyl.Atoms[3], BondOrder.Single);
            methyl.AddBond(methyl.Atoms[0], methyl.Atoms[4], BondOrder.Single);
            methyl.Atoms[0].SetProperty(StandardGenerator.HighlightColorKey, WPF::Media.Colors.Red);
            methyl.Bonds[0].SetProperty(StandardGenerator.HighlightColorKey, WPF::Media.Colors.Red);
            SymbolVisibility visibility = SelectionVisibility.Disconnected(SymbolVisibility.IupacRecommendations);
            Assert.IsFalse(visibility.Visible(methyl.Atoms[0], methyl.GetConnectedBonds(methyl.Atoms[0]),
                    new RendererModel()));
        }

        [TestMethod()]
        public void HighlightIsSelected()
        {
            var mock_chemObject = new Mock<IChemObject>(); var chemObject = mock_chemObject.Object;
            mock_chemObject.Setup(n => n.GetProperty<WPF::Media.Color?>(StandardGenerator.HighlightColorKey)).Returns(WPF::Media.Colors.Red);
            Assert.IsTrue(SelectionVisibility.IsSelected(chemObject, new RendererModel()));
        }

        static IAtom AtomAt(string symb, Vector2 p)
        {
            IAtom atom = new Atom(symb)
            {
                Point2D = p
            };
            return atom;
        }
    }
}
