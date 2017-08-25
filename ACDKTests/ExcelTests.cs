using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCDK;
using System;
using System.IO;

namespace NCDK.Tests
{
    [TestClass()]
    public class ExcelTests
    {
        [TestCategory("Excel")]
        [TestMethod()]
        public void ExcelTest()
        {
            string filename = "ACDK_Test.xlsm";
            bool passed = false;
#if HAS_EXCEL_REFERENCE
            MSExcel.Application excel = new MSExcel.Application();
#else
            dynamic excel = Activator.CreateInstance(Type.GetTypeFromProgID("Excel.Application"));
#endif
            try
            {
                excel.Visible = true;
                excel.Workbooks.Open(Path.Combine(Path.GetTempPath(), filename));
                excel.Application.Run(filename + "!TestAll");
                passed = true;
            }
            finally
            {
                try
                {
                    excel.Quit();
                }
                catch (Exception)
                { }
            }
            Assert.IsTrue(passed, "Excel test failed.");
        }
    }
}
