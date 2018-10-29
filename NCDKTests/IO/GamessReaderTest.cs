using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK.Silent;
using System.IO;
using System;

namespace NCDK.IO
{
    /// <summary>
    /// </summary>
    // @cdk.module test-extra
    // @author Nathana&euml;l "M.Le_maudit" Mazuir
    [TestClass()]
    public class GamessReaderTest : SimpleChemObjectReaderTest
    {
        protected override string TestFile => "NCDK.Data.Gamess.Cl2O.log";
        protected override Type ChemObjectIOToTestType => typeof(GamessReader);

        private TextReader inputReader;
        private GamessReader gamessReaderUnderTest;

        /// <summary>
        /// Sets up the fixture.
        /// <para>This method is called before a test is executed and performs the
        /// following actions:
        /// <ul>
        ///     <item>Constructs a new FileReader.</item>
        ///     <item>Constructs a new TextReader.</item>
        ///     <item>Constructs a new GamessReader.</item>
        /// </ul>
        /// </para>
        /// </summary>
        public GamessReaderTest()
        {
            var ins = ResourceLoader.GetAsStream(TestFile);
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
