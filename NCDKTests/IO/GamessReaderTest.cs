using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Default;
using System.IO;

namespace NCDK.IO
{
    /// <summary>
    /// </summary>
    // @cdk.module test-extra
    // @author Nathana&euml;l "M.Le_maudit" Mazuir
    [TestClass()]
    public class GamessReaderTest : SimpleChemObjectReaderTest
    {
        protected override string testFile => "NCDK.Data.Gamess.Cl2O.log";
        private GamessReader gamessReaderUnderTest;
        protected override IChemObjectIO ChemObjectIOToTest => gamessReaderUnderTest;

        private TextReader inputReader;

        /// <summary>
        /// Sets up the fixture.
        /// <para>This method is called before a test is executed and performs the
        /// following actions:
        /// <ul>
        ///     <li>Constructs a new FileReader.</li>
        ///     <li>Constructs a new TextReader.</li>
        ///     <li>Constructs a new GamessReader.</li>
        /// </ul>
        /// </para>
        /// </summary>
        public GamessReaderTest()
        {
            Stream ins = ResourceLoader.GetAsStream(testFile);
            this.inputReader = new StreamReader(ins);
            this.gamessReaderUnderTest = new GamessReader(this.inputReader);
        }

        ~GamessReaderTest()
        {
            this.inputReader.Close();
            this.gamessReaderUnderTest.Close(); // TODO Answer the question : Is it necessary ?
        }

        [TestMethod()]
        public void TestGamessReader()
        {
            Assert.IsNotNull(this.inputReader, "TEST: The inputReader is not null.");
            Assert.IsTrue(this.inputReader is TextReader, "TEST: The inputReader is a Reader object.");
            Assert.IsNotNull(this.gamessReaderUnderTest, "TEST: The GamessReader object is constructed.");
            //        Assert.AreEqual("TEST: ", this.gr.input, this.inputReader);
        }

        [TestMethod()]
        public void TestAccepts()
        {
            Assert.IsNotNull(this.gamessReaderUnderTest, "The GamessReader object is not constructed");
            Assert.IsTrue(gamessReaderUnderTest.Accepts(typeof(ChemFile)),
                "GamessReader should accept an IChemFile object.");
        }

        [TestMethod()]
        public void TestRead()
        {
            Assert.IsNotNull(this.gamessReaderUnderTest, "TEST: The GamessReader object is constructed.");
            Assert.IsTrue(
                this.gamessReaderUnderTest.Read(new ChemFile()) is ChemObject,
                "TEST: Read(IChemObject) returns a IChemObject.");
        }
    }
}
